-- Ejercicio 2: consultas sobre ccloglogin (SQL Server)
--
-- Criterio para emparejar sesiones: dentro de cada usuario se ordenan los movimientos por fecha
-- y cada login (TipoMov = 1) se une con el movimiento que le sigue inmediatamente, siempre que
-- sea un logout (TipoMov = 0). Un login sin logout posterior o un logout sin login previo
-- no cuentan como sesión.
--
-- LEAD evita el auto-join: recorre la tabla una sola vez por usuario en lugar de buscar,
-- para cada login, el logout mas cercano. El indice (User_id, fecha) creado por la migracion
-- permite que el particionado por usuario no requiera un ordenamiento adicional.

USE CCenterRIA;
GO

-- 1. Usuario que mas tiempo ha estado logueado
WITH Movimientos AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteTipo,
        LEAD(fecha)   OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteFecha
    FROM ccloglogin
),
Sesiones AS (
    SELECT User_id, fecha AS Inicio, SiguienteFecha AS Fin
    FROM Movimientos
    WHERE TipoMov = 1 AND SiguienteTipo = 0
),
Totales AS (
    SELECT User_id, SUM(DATEDIFF_BIG(SECOND, Inicio, Fin)) AS Segundos
    FROM Sesiones
    GROUP BY User_id
)
SELECT TOP (1) WITH TIES
    User_id,
    CONCAT(
        Segundos / 86400, ' días, ',
        Segundos % 86400 / 3600, ' horas, ',
        Segundos % 3600 / 60, ' minutos, ',
        Segundos % 60, ' segundos') AS TiempoTotal
FROM Totales
ORDER BY Segundos DESC;
GO

-- 2. Usuario que menos tiempo ha estado logueado
-- Solo se consideran usuarios con al menos una sesion completa
WITH Movimientos AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteTipo,
        LEAD(fecha)   OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteFecha
    FROM ccloglogin
),
Sesiones AS (
    SELECT User_id, fecha AS Inicio, SiguienteFecha AS Fin
    FROM Movimientos
    WHERE TipoMov = 1 AND SiguienteTipo = 0
),
Totales AS (
    SELECT User_id, SUM(DATEDIFF_BIG(SECOND, Inicio, Fin)) AS Segundos
    FROM Sesiones
    GROUP BY User_id
)
SELECT TOP (1) WITH TIES
    User_id,
    CONCAT(
        Segundos / 86400, ' días, ',
        Segundos % 86400 / 3600, ' horas, ',
        Segundos % 3600 / 60, ' minutos, ',
        Segundos % 60, ' segundos') AS TiempoTotal
FROM Totales
ORDER BY Segundos ASC;
GO

-- 3. Promedio de logueo por usuario en cada mes
-- La sesion se asigna al mes en que inicio (mes del login) y el promedio es
-- el promedio de duracion de las sesiones de ese usuario en ese mes
WITH Movimientos AS (
    SELECT
        User_id,
        TipoMov,
        fecha,
        LEAD(TipoMov) OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteTipo,
        LEAD(fecha)   OVER (PARTITION BY User_id ORDER BY fecha) AS SiguienteFecha
    FROM ccloglogin
),
Sesiones AS (
    SELECT User_id, fecha AS Inicio, SiguienteFecha AS Fin
    FROM Movimientos
    WHERE TipoMov = 1 AND SiguienteTipo = 0
),
Promedios AS (
    SELECT
        User_id,
        YEAR(Inicio)  AS Anio,
        MONTH(Inicio) AS Mes,
        AVG(DATEDIFF_BIG(SECOND, Inicio, Fin)) AS Segundos
    FROM Sesiones
    GROUP BY User_id, YEAR(Inicio), MONTH(Inicio)
)
SELECT
    User_id,
    Anio,
    Mes,
    CONCAT(
        Segundos / 86400, ' días, ',
        Segundos % 86400 / 3600, ' horas, ',
        Segundos % 3600 / 60, ' minutos, ',
        Segundos % 60, ' segundos') AS PromedioLogueo
FROM Promedios
ORDER BY User_id, Anio, Mes;
GO
