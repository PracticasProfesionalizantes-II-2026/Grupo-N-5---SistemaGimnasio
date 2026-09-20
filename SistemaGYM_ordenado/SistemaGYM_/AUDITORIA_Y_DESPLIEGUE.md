# Auditoría y despliegue

## Correcciones aplicadas

- `Pago` ahora se da de baja de forma lógica (`EstaActivo` y `FechaBaja`), para conservar trazabilidad contable.
- El alta y la modificación de pagos verifican que el alumno exista y esté activo, y que la suscripción sea del mismo alumno, esté activa y vigente. Los errores de negocio retornan HTTP 400 con un mensaje claro, en vez de fallar por una clave foránea.
- La interfaz solo ofrece suscripciones activas y vigentes al registrar un pago.
- Alumno y Profesor ya tenían baja lógica; se mantiene ese criterio. Profesor vuelve a persistir el campo `EstaActivo` al editarse.
- Las entidades operativas restantes (actividad, rutina, alimentación, anuncio y plan de suscripción) mantienen baja física. Sus dependencias impiden por FK eliminar datos referenciados.
- Se agregaron índices únicos para email, DNI e inscripción alumno-actividad, evitando duplicados de datos críticos.
- Un usuario inactivo ya no puede iniciar sesión. La verificación de contraseñas tolera hashes corruptos sin responder 500.

## ABM

La API cuenta con alta, consulta, modificación y baja para alumnos, profesores, actividades, alimentación, anuncios, pagos, rutinas y suscripciones. Las relaciones Alumno-Suscripción y Alumno-Actividad tienen operaciones específicas de alta/baja lógica. Administrador se mantiene fuera de ABM público por seguridad.

## Aplicar el cambio de base de datos

Desde la carpeta raíz de la solución, ejecutar:

```powershell
dotnet ef database update --project API\SistemaGYM_.csproj --startup-project API\SistemaGYM_.csproj
```

Las migraciones `PagoBajaLogicaYRestricciones` y `EstablecerPagoActivoPorDefecto` preservan los pagos existentes como activos. Antes de aplicar los índices únicos, verificá que la base no tenga DNI, emails o pares alumno-actividad duplicados, porque SQL Server rechazará crear el índice en ese caso.

## Riesgo de arquitectura pendiente

La API no tiene autenticación/autorización propia: la autorización actual está en la aplicación web mediante sesión. Si la API se expone fuera de la red local, se debe agregar autenticación de API (JWT o un gateway) y políticas por rol antes de publicarla.
