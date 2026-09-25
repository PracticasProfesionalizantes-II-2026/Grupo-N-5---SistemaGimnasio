# Notas del proyecto – Sistema Gimnasio (GYM+)

Archivo de contexto para retomar el trabajo (con o sin Claude).
Al empezar una conversación nueva con Claude: conectar la carpeta y pedirle que lea este archivo.

## Contexto

- Trabajo de facultad hecho de a dos: Juan Cruz Clausen y Ainara Paredes. El uso de IA está autorizado.
- **Reglas para cualquier cambio:**
  - El código tiene que ser comprensible y estudiable, sin soluciones demasiado complejas ni "súper profesionales".
  - Hay que respetar la estructura actual y cambiar lo mínimo posible.
  - Si un cambio de estructura es realmente necesario, primero se explica claramente y se consulta.
  - Claude no hace commits, push ni pull: solo edita archivos. Nosotros decidimos qué se sube a Git.

## Estructura

```
Grupo-N-5---SistemaGimnasio
├── SistemaGYM_                     ← resto vacío de compilaciones viejas (solo bin/obj), se puede borrar
├── SistemaGYM_ordenado
│   ├── SistemaGYM_                 ← PROYECTO REAL (abrir esta carpeta)
│   │   ├── API/                    ← API: base de datos + lógica (http://localhost:5215)
│   │   ├── SistemaGYM.Web/         ← Web MVC: pantallas (http://localhost:5012)
│   │   ├── .vscode/                ← configuración para ejecutar desde VS Code
│   │   ├── SistemaGYM.sln
│   │   └── AUDITORIA_Y_DESPLIEGUE.md
│   └── SistemaGYM_ordenado         ← resto vacío, se puede borrar
├── README.md
├── NOTAS.md                        ← este archivo
└── PROYECTO.pdf
```

- **API:** Endpoints → Logica (DTO + Logicas) → Repositorios → Datos (GimnasioContext, EF Core + SQL Server).
- **Web:** Controllers → Services (llaman a la API por HTTP) → Views (Bootstrap).
- La Web no accede a la base de datos: todo lo pide a la API. **Tienen que correr las dos a la vez.**
- Roles: Administrador, Profesor y Cliente (alumno). El rol se guarda en la sesión de la Web.

## Cómo ejecutar

1. En VS Code: **File → Open Folder** → `SistemaGYM_ordenado\SistemaGYM_`.
2. Requisitos: .NET 10 SDK y SQL Server local (la conexión está en `API/appsettings.json`).
3. Crear o actualizar la base de datos (la primera vez y cada vez que haya migraciones nuevas):
   ```
   dotnet tool install --global dotnet-ef    (solo la primera vez)
   dotnet ef database update --project API\SistemaGYM_.csproj
   ```
4. En **Run and Debug** elegir **"API + Web"** y dar play.
5. Abrir `http://localhost:5012` (pantalla de Login).

## Decisiones tomadas (24/09/2026)

- **Profesor:** solo la parte académica. Ve clientes (sin editarlos), sus actividades, rutinas, alimentación y anuncios. No ve pagos, suscripciones ni reportes, y no registra clientes ni profesores.
- **Rutina a varios alumnos:** no se cambió la base. Al crearla se eligen varios alumnos y se guarda una copia por alumno.
- **Cupo:** se agregó la columna `Cupo` a Actividad con la migración `AgregarCupoActividad`. Las actividades que ya existían quedan con cupo 20.
- **Estilo:** fiel a los mockups (gris, tarjetas blancas, botones redondeados) con acento naranja. Todo está en `wwwroot/css/site.css`.
- **Mensajes:** los avisos de éxito o error salen en una ventana centrada (`Views/Shared/_Mensajes.cshtml`). En los controladores se usa `TempData["Mensaje"]` para éxito y `TempData["Error"]` o `ViewBag.Error` para errores. Para pedir confirmación, al `<form>` se le agrega `data-confirmar="..."`.

## Cambios hechos (24/09/2026)

- Validaciones en la API: DNI de 7 u 8 dígitos; email y DNI únicos entre clientes y profesores; nombre de actividad único; solo profesores activos a cargo de una actividad; hora de inicio anterior a la de fin; cupo de 1 a 500 y no menor a los inscriptos.
- Inscripción: la API rechaza inscripciones sin cupo. En "Clases disponibles" el botón pasa a ser una tilde ✓ al inscribirse y aparece "Sin cupo" cuando está lleno.
- Admin y profesor pueden quitar alumnos de una actividad.
- Clientes: buscador por nombre, DNI, email, suscripción y fecha de alta. La suscripción se elige al registrar o modificar un cliente.
- Profesor: edita y elimina solo sus rutinas y planes de alimentación; publica anuncios a su nombre.
- Errores corregidos de paso: al modificar un cliente o profesor sin cambiar la contraseña, la API lo rechazaba; en el detalle del profesor estaban invertidos Título y Descripción; al editar un plan de alimentación se perdía el alumno asignado.

## Cambios hechos (25/09/2026) – revisión de validaciones

- **"Crasheo" con DNI repetido:** no era un error del código (la API ya lo valida). Con "Run and Debug", VS Code se frena en el `throw` de la validación. Solución: en Run and Debug → panel Breakpoints, destildar "User-Unhandled Exceptions" (o apretar F5 para continuar).
- Validaciones agregadas en los DTO (mensajes claros en vez de "datos en conflicto"): nombre, apellido y teléfono obligatorios y con largo máximo en clientes y profesores; nombre y descripción obligatorios en rutinas y alimentación; precio máximo del plan.
- Eliminar un plan de suscripción: se bloquea si algún cliente lo tiene o lo tuvo (antes la base borraba en cascada el historial y los **pagos**).
- Eliminar una actividad: se bloquea si tiene alumnos inscriptos (antes se borraban las inscripciones sin avisar).
- Dar de baja un profesor: se bloquea si tiene actividades a cargo.
- Registrarse y Registrar profesor: si hay un error (ej: DNI repetido) ya no se borra lo que se había escrito.
- No se cambió la base de datos, ni los endpoints, ni la estructura.

## Pendientes

- [ ] **Compilar y probar:** no se pudo compilar desde Claude. Correr `dotnet build` y luego `dotnet ef database update --project API\SistemaGYM_.csproj` para aplicar la migración del cupo.
- [ ] **Probar los cambios del 25/09:** borrar un plan con clientes, una actividad con inscriptos, un profesor con actividades, y registrarse con un DNI repetido.
- [ ] **Verificar precios con decimales:** crear un plan con precio 1500.50 y ver que no se guarde como 150050 (depende de la configuración regional de Windows).
- [ ] Recordar: la API y la Web tienen que correr juntas ("API + Web"); si la API está apagada, la Web muestra una pantalla de error.
- [ ] **Documentación:** completar el `README.md` (qué es el sistema, cómo ejecutarlo, roles). Se puede aprovechar `AUDITORIA_Y_DESPLIEGUE.md`.
- [ ] **Mockups del profesor:** faltan en Figma.
- [ ] Opcional: borrar las carpetas vacías `SistemaGYM_` (raíz) y `SistemaGYM_ordenado\SistemaGYM_ordenado`.
- [ ] Opcional: `Views/Shared/_Layout.cshtml.css` es un archivo de la plantilla que no se usa (sus estilos no se cargan). Se puede borrar.

## Git (recordatorios)

- Antes de empezar a trabajar: en GitHub Desktop, **Fetch origin** y, si aparece, **Pull origin**.
- Para guardar cambios propios sin subirlos: **Branch → Stash all changes**.
- Hacer commits chicos con mensajes claros, y hacer Pull antes de hacer Push.
