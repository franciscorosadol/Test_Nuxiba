# EVALUACIÓN TÉCNICA NUXIBA

Prueba: **DESARROLLADOR JR**

Deadline: **1 día**

Nombre: Francisco Javier Rosado Lara

---

## Clona y crea tu repositorio para la evaluación

1. Clona este repositorio en tu máquina local.
2. Crea un repositorio público en tu cuenta personal de GitHub, BitBucket o Gitlab.
3. Cambia el origen remoto para que apunte al repositorio público que acabas de crear en tu cuenta.
4. Coloca tu nombre en este archivo README.md y realiza un push al repositorio remoto.

---

## Instrucciones Generales

1. Cada pregunta tiene un valor asignado. Asegúrate de explicar tus respuestas y mostrar las consultas o procedimientos que utilizaste.
2. Se evaluará la claridad de las explicaciones, el pensamiento crítico, y la eficiencia de las consultas.
3. Utiliza **SQL Server** para realizar todas las pruebas y asegúrate de que las consultas funcionen correctamente antes de entregar.
4. Justifica tu enfoque cuando encuentres una pregunta sin una única respuesta correcta.
5. Configura un Contenedor de **SQL Server con Docker** utilizando los siguientes pasos:

### Pasos para ejecutar el contenedor de SQL Server

Asegúrate de tener Docker instalado y corriendo en tu máquina. Luego, ejecuta el siguiente comando para levantar un contenedor con SQL Server:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd'    -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

6. Conéctate al servidor de SQL con cualquier herramienta como **SQL Server Management Studio** o **Azure Data Studio** utilizando las siguientes credenciales:
   - **Servidor**: localhost, puerto 1433
   - **Usuario**: sa
   - **Contraseña**: YourStrong!Passw0rd

---

# Examen Práctico para Desarrollador Junior en .NET 8 y SQL Server

**Tiempo estimado:** 1 día  
**Total de puntos:** 100

---

## Instrucciones Generales:

El examen está compuesto por tres ejercicios prácticos. Sigue las indicaciones en cada uno y asegúrate de entregar el código limpio y funcional.

Además, se proporciona un archivo **CCenterRIA.xlsx** para que te bases en la estructura de las tablas y datos proporcionados.

[Descargar archivo de ejemplo](CCenterRIA.xlsx)

---

## Ejercicio 1: API RESTful con ASP.NET Core y Entity Framework (40 puntos)

**Instrucciones:**  
Desarrolla una API RESTful con ASP.NET Core y Entity Framework que permita gestionar el acceso de usuarios.

1. **Creación de endpoints**:
   - **GET /logins**: Devuelve todos los registros de logins y logouts de la tabla `ccloglogin`. (5 puntos)
   - **POST /logins**: Permite registrar un nuevo login/logout. (5 puntos)
   - **PUT /logins/{id}**: Permite actualizar un registro de login/logout. (5 puntos)
   - **DELETE /logins/{id}**: Elimina un registro de login/logout. (5 puntos)

2. **Modelo de la entidad**:  
   Crea el modelo `Login` basado en los datos de la tabla `ccloglogin`:
   - `User_id` (int)
   - `Extension` (int)
   - `TipoMov` (int) → 1 es login, 0 es logout
   - `fecha` (datetime)

3. **Base de datos**:  
   Utiliza **Entity Framework Core** para crear la tabla en una base de datos SQL Server basada en este modelo. Aplica migraciones para crear la tabla en la base de datos. (10 puntos)

4. **Validaciones**:  
   Implementa las validaciones necesarias para asegurar que las fechas sean válidas y que el `User_id` esté presente en la tabla `ccUsers`. Además, maneja errores como intentar registrar un login sin un logout anterior. (10 puntos)

5. **Pruebas Unitarias** (Opcional):  
   Se valorará si incluyes pruebas unitarias para los endpoints de tu API utilizando un framework como **xUnit** o **NUnit**. (Puntos extra)

---

## Ejercicio 2: Consultas SQL y Optimización (30 puntos)

**Instrucciones:**

Trabaja en SQL Server y realiza las siguientes consultas basadas en la tabla `ccloglogin`:

1. **Consulta del usuario que más tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta que devuelva el usuario que ha pasado más tiempo logueado. Para calcular el tiempo de logueo, empareja cada "login" (TipoMov = 1) con su correspondiente "logout" (TipoMov = 0) y suma el tiempo total por usuario.

   Ejemplo de respuesta:  
   - `User_id`: 92  
   - Tiempo total: 361 días, 12 horas, 51 minutos, 8 segundos

2. **Consulta del usuario que menos tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta similar a la anterior, pero que devuelva el usuario que ha pasado menos tiempo logueado.

   Ejemplo de respuesta:  
   - `User_id`: 90  
   - Tiempo total: 244 días, 43 minutos, 15 segundos

3. **Promedio de logueo por mes** (10 puntos):
   - Escribe una consulta que calcule el tiempo promedio de logueo por usuario en cada mes.

   Ejemplo de respuesta:  
   - Usuario 70 en enero 2023: 3 días, 14 horas, 1 minuto, 16 segundos

---

## Ejercicio 3: API RESTful para generación de CSV (30 puntos)

**Instrucciones:**

1. **Generación de CSV**:  
   Crea un endpoint adicional en tu API que permita generar un archivo CSV con los siguientes datos:
   - Nombre de usuario (`Login` de la tabla `ccUsers`)
   - Nombre completo (combinación de `Nombres`, `ApellidoPaterno`, y `ApellidoMaterno` de la tabla `ccUsers`)
   - Área (tomado de la tabla `ccRIACat_Areas`)
   - Total de horas trabajadas (basado en los registros de login y logout de la tabla `ccloglogin`)

   El CSV debe calcular el total de horas trabajadas por usuario sumando el tiempo entre logins y logouts.

2. **Formato y Entrega**:
   - El CSV debe ser descargable a través del endpoint de la API.
   - Asegúrate de probar este endpoint utilizando herramientas como **Postman** o **curl** y documenta los pasos en el archivo README.md.

---

## Entrega

1. Sube tu código a un repositorio en GitHub o Bitbucket y proporciona el enlace para revisión.
2. El repositorio debe contener las instrucciones necesarias en el archivo **README.md** para:
   - Levantar el contenedor de SQL Server.
   - Conectar la base de datos.
   - Ejecutar la API y sus endpoints.
   - Descargar el CSV generado.
3. **Opcional**: Si incluiste pruebas unitarias, indica en el README cómo ejecutarlas.

---

Este examen evalúa tu capacidad para desarrollar APIs RESTful, realizar consultas avanzadas en SQL Server y generar reportes en formato CSV. Se valorará la organización del código, las mejores prácticas y cualquier documentación adicional que proporciones.

---

# Solución

Solución en .NET 8 con Entity Framework Core y SQL Server.

```
NuxibaAccesos.Api/     API (controladores, servicios, modelos, migraciones)
NuxibaAccesos.Tests/   Pruebas unitarias (xUnit)
Database/datos.sql     Carga de los datos de CCenterRIA.xlsx
Database/consultas.sql Consultas del Ejercicio 2
```

## 1. Levantar SQL Server

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

## 2. Crear la base de datos y cargar los datos

La cadena de conexión está en `NuxibaAccesos.Api/appsettings.json` y apunta al contenedor anterior (base `CCenterRIA`).

Desde la raíz del repositorio:

```bash
# Crea la base y las tablas a partir de la migración
dotnet tool install --global dotnet-ef
dotnet ef database update --project NuxibaAccesos.Api

# Carga los datos del Excel (ccUsers, ccRIACat_Areas y ccloglogin)
docker cp Database/datos.sql sqlserver:/datos.sql
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -d CCenterRIA -f 65001 -i /datos.sql
```

También se puede abrir `Database/datos.sql` en Azure Data Studio o SSMS y ejecutarlo sobre la base `CCenterRIA`.

## 3. Ejecutar la API

```bash
dotnet run --project NuxibaAccesos.Api
```

Queda en `http://localhost:5026` y en desarrollo tiene Swagger en `http://localhost:5026/swagger`.

## Ejercicio 1: endpoints

| Método | Ruta | Descripción |
| ------ | ---- | ----------- |
| GET | `/logins` | Todos los registros de `ccloglogin` |
| GET | `/logins/{id}` | Un registro |
| POST | `/logins` | Registra un login/logout |
| PUT | `/logins/{id}` | Actualiza un registro |
| DELETE | `/logins/{id}` | Elimina un registro |

Cuerpo para POST y PUT:

```json
{ "User_id": 70, "Extension": 5, "TipoMov": 1, "fecha": "2024-10-05T10:00:00" }
```

`ccloglogin` no tiene llave primaria en el Excel, así que se agregó una columna `Id` autoincremental para poder identificar los registros en PUT y DELETE.

Validaciones (400 para datos inválidos, 409 para conflictos de secuencia):

- `TipoMov` solo puede ser 1 o 0.
- La fecha no puede ser anterior al mínimo de `datetime` de SQL Server ni futura.
- `User_id` debe existir en `ccUsers`.
- El usuario no puede tener dos movimientos en la misma fecha.
- Los movimientos de un usuario deben alternar: no se puede registrar un login si el movimiento anterior también es un login, ni un logout si el anterior también es un logout. Al actualizar se revisan también el movimiento anterior y el siguiente.

Ejemplo de conflicto:

```bash
curl -X POST http://localhost:5026/logins -H "Content-Type: application/json" \
  -d '{"User_id":70,"Extension":5,"TipoMov":1,"fecha":"2024-10-05T10:00:00"}'
# 409 {"title":"El usuario ya tiene un login sin logout anterior.","status":409}
```

Notas sobre los datos: en `ccRIACat_Areas` el `IDArea` 2 aparece dos veces (BBVA y Banamex), por eso la llave de esa tabla es (`IDArea`, `AreaName`). Todos los usuarios pertenecen al área 1, así que no afecta el reporte.

## Ejercicio 2: consultas

Están en `Database/consultas.sql`. Cada login se empareja con el movimiento que le sigue para el mismo usuario (con `LEAD`), y solo cuenta como sesión si ese movimiento es un logout. Así la tabla se recorre una sola vez, sin auto-joins.

| Consulta | Resultado |
| -------- | --------- |
| Más tiempo logueado | Usuario 92: 361 días, 12 horas, 51 minutos, 8 segundos |
| Menos tiempo logueado | Usuario 90: 244 días, 0 horas, 43 minutos, 15 segundos |
| Promedio por mes | Usuario 70 en enero 2023: 3 días, 14 horas, 1 minuto, 16 segundos |

El promedio mensual es el promedio de duración de las sesiones que iniciaron en ese mes. En caso de empate en las dos primeras consultas se devuelven todos los usuarios empatados.

## Ejercicio 3: CSV

```bash
curl -o horas_trabajadas.csv http://localhost:5026/reportes/horas-trabajadas
```

Con Postman: `GET http://localhost:5026/reportes/horas-trabajadas` y "Send and Download".

Columnas: `Usuario`, `NombreCompleto`, `Area`, `TotalHorasTrabajadas` (horas en decimal con dos decimales). Incluye todos los usuarios de `ccUsers`; los que no tienen sesiones aparecen con 0.00.

## Pruebas unitarias

```bash
dotnet test
```
