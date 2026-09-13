-- =====================================================================
-- COINK · Prueba técnica — 00. Creación de la base de datos
-- Motor: PostgreSQL
-- ---------------------------------------------------------------------
-- Este script solo es necesario en una instalación MANUAL.
-- Con Docker Compose la base la crea la propia imagen a partir de la
-- variable POSTGRES_DB.
--
-- Requiere psql: usa la meta-instrucción \gexec porque en PostgreSQL
--
--   psql -U postgres -f 00_crear_base_datos.sql
-- =====================================================================

SELECT 'CREATE DATABASE coink_db'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'coink_db')\gexec
