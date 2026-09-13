-- =====================================================================
-- COINK · Prueba técnica — 02. Stored Procedures y funciones
-- Motor: PostgreSQL
-- Idempotente: usa CREATE OR REPLACE.
-- ---------------------------------------------------------------------
-- Códigos devueltos por sp_registrar_usuario (p_codigo_resultado):
--   0 = registro exitoso
--   1 = el país no existe
--   2 = el departamento no existe
--   3 = el municipio no existe
--   4 = el departamento no pertenece al país indicado
--   5 = el municipio no pertenece al departamento indicado
--   6 = el teléfono ya está registrado
-- La API traduce estos códigos a HTTP: 0 -> 201, 1..5 -> 422, 6 -> 409.
-- =====================================================================

-- ---------------------------------------------------------------------
-- sp_registrar_usuario
-- Valida la cadena país -> departamento -> municipio e inserta el
-- usuario dentro de la MISMA transacción, de modo que no existe ventana
-- entre la verificación y la escritura.
-- ---------------------------------------------------------------------
CREATE OR REPLACE PROCEDURE sp_registrar_usuario(
    IN  p_nombre           VARCHAR,
    IN  p_telefono         VARCHAR,
    IN  p_pais_id          INTEGER,
    IN  p_departamento_id  INTEGER,
    IN  p_municipio_id     INTEGER,
    IN  p_direccion        VARCHAR,
    OUT p_codigo_resultado SMALLINT,
    OUT p_usuario_id       INTEGER,
    OUT p_fecha_registro   TIMESTAMPTZ
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_pais_del_departamento  INTEGER;
    v_departamento_del_mpio  INTEGER;
    v_restriccion            TEXT;
BEGIN
    p_codigo_resultado := 0;
    p_usuario_id       := NULL;
    p_fecha_registro   := NULL;

    -- 1. El país debe existir
    IF NOT EXISTS (SELECT 1 FROM pais p WHERE p.pais_id = p_pais_id) THEN
        p_codigo_resultado := 1;
        RETURN;
    END IF;

    -- 2. El departamento debe existir y pertenecer al país
    SELECT d.pais_id INTO v_pais_del_departamento
    FROM departamento d
    WHERE d.departamento_id = p_departamento_id;

    IF NOT FOUND THEN
        p_codigo_resultado := 2;
        RETURN;
    END IF;

    IF v_pais_del_departamento <> p_pais_id THEN
        p_codigo_resultado := 4;
        RETURN;
    END IF;

    -- 3. El municipio debe existir y pertenecer al departamento
    SELECT m.departamento_id INTO v_departamento_del_mpio
    FROM municipio m
    WHERE m.municipio_id = p_municipio_id;

    IF NOT FOUND THEN
        p_codigo_resultado := 3;
        RETURN;
    END IF;

    IF v_departamento_del_mpio <> p_departamento_id THEN
        p_codigo_resultado := 5;
        RETURN;
    END IF;

    -- 4. Inserción
    INSERT INTO usuario (nombre, telefono, direccion, municipio_id)
    VALUES (btrim(p_nombre), btrim(p_telefono), btrim(p_direccion), p_municipio_id)
    RETURNING usuario.usuario_id, usuario.fecha_registro
    INTO p_usuario_id, p_fecha_registro;

EXCEPTION
    -- Dos peticiones simultáneas con el mismo teléfono: la restricción
    -- UNIQUE es la que decide.
    --
    -- Se comprueba CUÁL restricción falló
    WHEN unique_violation THEN
        GET STACKED DIAGNOSTICS v_restriccion = CONSTRAINT_NAME;

        IF v_restriccion = 'uq_usuario_telefono' THEN
            p_codigo_resultado := 6;
            p_usuario_id       := NULL;
            p_fecha_registro   := NULL;
        ELSE
            RAISE;
        END IF;
    -- El municipio fue eliminado entre la verificación y la inserción.
    WHEN foreign_key_violation THEN
        p_codigo_resultado := 3;
        p_usuario_id       := NULL;
        p_fecha_registro   := NULL;
END;
$$;

COMMENT ON PROCEDURE sp_registrar_usuario(VARCHAR, VARCHAR, INTEGER, INTEGER, INTEGER, VARCHAR)
    IS 'Registra un usuario validando la coherencia país -> departamento -> municipio. Devuelve código de resultado, id y fecha.';

-- ---------------------------------------------------------------------
-- fn_obtener_usuario
-- Devuelve el detalle del usuario con los nombres de país, departamento
-- y municipio ya resueltos, para no exponer identificadores internos.
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_obtener_usuario(p_usuario_id INTEGER)
RETURNS TABLE (
    usuario_id     INTEGER,
    nombre         VARCHAR,
    telefono       VARCHAR,
    direccion      VARCHAR,
    pais           VARCHAR,
    departamento   VARCHAR,
    municipio      VARCHAR,
    fecha_registro TIMESTAMPTZ
)
LANGUAGE sql
STABLE
AS $$
    SELECT u.usuario_id,
           u.nombre,
           u.telefono,
           u.direccion,
           p.nombre,
           d.nombre,
           m.nombre,
           u.fecha_registro
    FROM usuario u
    JOIN municipio    m ON m.municipio_id    = u.municipio_id
    JOIN departamento d ON d.departamento_id = m.departamento_id
    JOIN pais         p ON p.pais_id         = d.pais_id
    WHERE u.usuario_id = p_usuario_id;
$$;

-- ---------------------------------------------------------------------
-- Catálogos de solo lectura
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION fn_listar_paises()
RETURNS TABLE (pais_id INTEGER, codigo VARCHAR, nombre VARCHAR)
LANGUAGE sql
STABLE
AS $$
    SELECT p.pais_id, p.codigo, p.nombre
    FROM pais p
    ORDER BY p.nombre;
$$;

CREATE OR REPLACE FUNCTION fn_listar_departamentos(p_pais_id INTEGER)
RETURNS TABLE (departamento_id INTEGER, pais_id INTEGER, codigo VARCHAR, nombre VARCHAR)
LANGUAGE sql
STABLE
AS $$
    SELECT d.departamento_id, d.pais_id, d.codigo, d.nombre
    FROM departamento d
    WHERE d.pais_id = p_pais_id
    ORDER BY d.nombre;
$$;

CREATE OR REPLACE FUNCTION fn_listar_municipios(p_departamento_id INTEGER)
RETURNS TABLE (municipio_id INTEGER, departamento_id INTEGER, codigo VARCHAR, nombre VARCHAR)
LANGUAGE sql
STABLE
AS $$
    SELECT m.municipio_id, m.departamento_id, m.codigo, m.nombre
    FROM municipio m
    WHERE m.departamento_id = p_departamento_id
    ORDER BY m.nombre;
$$;
