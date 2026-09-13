-- =====================================================================
-- COINK · Prueba técnica — 03. Datos semilla
-- Muestra representativa, suficiente para probar todos los escenarios
-- de validación. Colombia usa códigos DANE reales; el segundo país
-- (Perú) se incluye para poder probar el caso "el departamento no
-- pertenece al país" y sus códigos son ilustrativos.
-- =====================================================================

-- ---------------------------------------------------------------------
-- Países
-- ---------------------------------------------------------------------
INSERT INTO pais (codigo, nombre)
SELECT v.codigo, v.nombre
FROM (VALUES
    ('COL', 'Colombia'),
    ('PER', 'Perú')
) AS v(codigo, nombre)
ORDER BY v.codigo
ON CONFLICT (codigo) DO NOTHING;

-- ---------------------------------------------------------------------
-- Departamentos
-- Atlántico (Colombia) y Cusco (Perú) comparten el código
-- '08': por eso la unicidad es (pais_id, codigo).
-- ---------------------------------------------------------------------
INSERT INTO departamento (pais_id, codigo, nombre)
SELECT p.pais_id, v.codigo, v.nombre
FROM (VALUES
    ('COL', '05', 'Antioquia'),
    ('COL', '08', 'Atlántico'),
    ('COL', '11', 'Bogotá D.C.'),
    ('COL', '25', 'Cundinamarca'),
    ('COL', '76', 'Valle del Cauca'),
    ('PER', '08', 'Cusco'),
    ('PER', '15', 'Lima')
) AS v(pais_codigo, codigo, nombre)
JOIN pais p ON p.codigo = v.pais_codigo
ORDER BY v.pais_codigo, v.codigo
ON CONFLICT (pais_id, codigo) DO NOTHING;

-- ---------------------------------------------------------------------
-- Municipios
-- ---------------------------------------------------------------------
INSERT INTO municipio (departamento_id, codigo, nombre)
SELECT d.departamento_id, v.codigo, v.nombre
FROM (VALUES
    ('COL', '05', '05001', 'Medellín'),
    ('COL', '05', '05088', 'Bello'),
    ('COL', '05', '05266', 'Envigado'),
    ('COL', '05', '05615', 'Rionegro'),
    ('COL', '08', '08001', 'Barranquilla'),
    ('COL', '08', '08758', 'Soledad'),
    ('COL', '11', '11001', 'Bogotá D.C.'),
    ('COL', '25', '25175', 'Chía'),
    ('COL', '25', '25754', 'Soacha'),
    ('COL', '25', '25899', 'Zipaquirá'),
    ('COL', '76', '76001', 'Cali'),
    ('COL', '76', '76520', 'Palmira'),
    ('PER', '08', '0801',  'Cusco'),
    ('PER', '08', '0813',  'Urubamba'),
    ('PER', '15', '1501',  'Lima'),
    ('PER', '15', '1506',  'Huaral')
) AS v(pais_codigo, departamento_codigo, codigo, nombre)
JOIN pais p         ON p.codigo = v.pais_codigo
JOIN departamento d ON d.pais_id = p.pais_id AND d.codigo = v.departamento_codigo
ORDER BY d.departamento_id, v.codigo
ON CONFLICT (departamento_id, codigo) DO NOTHING;
