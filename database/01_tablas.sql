-- =====================================================================
-- COINK · Prueba técnica — 01. Tablas, restricciones e índices
-- Motor: PostgreSQL
-- Idempotente: puede ejecutarse varias veces sin error.
-- =====================================================================

-- ---------------------------------------------------------------------
-- Catálogo: país
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS pais (
    pais_id INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo  VARCHAR(3)   NOT NULL,
    nombre  VARCHAR(100) NOT NULL,
    CONSTRAINT uq_pais_codigo UNIQUE (codigo),
    CONSTRAINT uq_pais_nombre UNIQUE (nombre),
    CONSTRAINT ck_pais_codigo CHECK (codigo ~ '^[A-Z]{3}$'),
    CONSTRAINT ck_pais_nombre CHECK (char_length(btrim(nombre)) >= 2)
);

COMMENT ON TABLE  pais        IS 'Catálogo paramétrico de países.';
COMMENT ON COLUMN pais.codigo IS 'Código ISO 3166-1 alfa-3 (COL, PER, ...).';

-- ---------------------------------------------------------------------
-- Catálogo: departamento (pertenece a un país)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS departamento (
    departamento_id INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    pais_id         INTEGER      NOT NULL,
    codigo          VARCHAR(10)  NOT NULL,
    nombre          VARCHAR(100) NOT NULL,
    CONSTRAINT fk_departamento_pais
        FOREIGN KEY (pais_id) REFERENCES pais (pais_id) ON DELETE RESTRICT,
    CONSTRAINT uq_departamento_pais_codigo UNIQUE (pais_id, codigo),
    CONSTRAINT uq_departamento_pais_nombre UNIQUE (pais_id, nombre),
    CONSTRAINT ck_departamento_nombre CHECK (char_length(btrim(nombre)) >= 2)
);

COMMENT ON TABLE  departamento        IS 'Catálogo paramétrico de departamentos / regiones, dependiente de país.';
COMMENT ON COLUMN departamento.codigo IS 'Código oficial dentro del país (DANE en Colombia). Único por país, no globalmente.';

-- ---------------------------------------------------------------------
-- Catálogo: municipio (pertenece a un departamento)
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS municipio (
    municipio_id    INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    departamento_id INTEGER      NOT NULL,
    codigo          VARCHAR(10)  NOT NULL,
    nombre          VARCHAR(100) NOT NULL,
    CONSTRAINT fk_municipio_departamento
        FOREIGN KEY (departamento_id) REFERENCES departamento (departamento_id) ON DELETE RESTRICT,
    CONSTRAINT uq_municipio_departamento_codigo UNIQUE (departamento_id, codigo),
    CONSTRAINT uq_municipio_departamento_nombre UNIQUE (departamento_id, nombre),
    CONSTRAINT ck_municipio_nombre CHECK (char_length(btrim(nombre)) >= 2)
);

COMMENT ON TABLE  municipio        IS 'Catálogo paramétrico de municipios, dependiente de departamento.';
COMMENT ON COLUMN municipio.codigo IS 'Código oficial dentro del departamento. Único por departamento.';

-- ---------------------------------------------------------------------
-- Usuario
-- Guarda únicamente municipio_id: el municipio ya determina el
-- departamento y el país
-- ---------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS usuario (
    usuario_id     INTEGER      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre         VARCHAR(100) NOT NULL,
    telefono       VARCHAR(16)  NOT NULL,
    direccion      VARCHAR(200) NOT NULL,
    municipio_id   INTEGER      NOT NULL,
    fecha_registro TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CONSTRAINT fk_usuario_municipio
        FOREIGN KEY (municipio_id) REFERENCES municipio (municipio_id) ON DELETE RESTRICT,
    CONSTRAINT uq_usuario_telefono UNIQUE (telefono),
    CONSTRAINT ck_usuario_nombre    CHECK (char_length(btrim(nombre))    BETWEEN 2 AND 100),
    CONSTRAINT ck_usuario_direccion CHECK (char_length(btrim(direccion)) BETWEEN 5 AND 200),
    CONSTRAINT ck_usuario_telefono  CHECK (telefono ~ '^\+[1-9][0-9]{7,14}$')
);

COMMENT ON TABLE  usuario                IS 'Usuarios registrados a través de la API.';
COMMENT ON COLUMN usuario.telefono       IS 'Formato E.164: + seguido de 8 a 15 dígitos. Se almacena como texto para conservar el prefijo.';
COMMENT ON COLUMN usuario.fecha_registro IS 'Marca de auditoría en UTC (TIMESTAMPTZ).';

-- ---------------------------------------------------------------------
-- Índices
-- ---------------------------------------------------------------------
CREATE INDEX IF NOT EXISTS ix_usuario_municipio_id ON usuario (municipio_id);
