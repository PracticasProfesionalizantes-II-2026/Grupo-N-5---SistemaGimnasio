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

## Estado / pendientes

- [ ] **Ejecutar el programa:** falla la compilación de la API (`build-api` termina con exit code 1). Falta ver el error real con `dotnet --version` y `dotnet build API\SistemaGYM_.csproj`.
- [ ] **CSS:** `Views/Shared/_Layout.cshtml.css` no se aplica, porque el layout no incluye el archivo `SistemaGYM.Web.styles.css`. Además, `wwwroot/css/site.css` tiene muy poco estilo propio.
- [ ] **Validación de DNI:** el commit "validaciones" dice email y DNI, pero `AlumnoCreateDto` no valida el DNI. Consultar con Ainara si quedó pendiente.
- [ ] **Documentación:** completar el `README.md` (qué es el sistema, cómo ejecutarlo, roles). Se puede aprovechar `AUDITORIA_Y_DESPLIEGUE.md`.
- [ ] Opcional: borrar las carpetas vacías `SistemaGYM_` (raíz) y `SistemaGYM_ordenado\SistemaGYM_ordenado`.

## Git (recordatorios)

- Antes de empezar a trabajar: en GitHub Desktop, **Fetch origin** y, si aparece, **Pull origin**.
- Para guardar cambios propios sin subirlos: **Branch → Stash all changes**.
- Hacer commits chicos con mensajes claros, y hacer Pull antes de hacer Push.
