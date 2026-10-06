IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Administradores] (
        [Id] int NOT NULL IDENTITY,
        [Usuario] nvarchar(50) NOT NULL,
        [Contrasenia] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Administradores] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Suscripciones] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Precio] decimal(10,2) NOT NULL,
        [DuracionDias] int NOT NULL,
        CONSTRAINT [PK_Suscripciones] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Usuario] (
        [Id] int NOT NULL IDENTITY,
        [Dni] int NOT NULL,
        [Direccion] nvarchar(max) NULL,
        [Contrasenia] nvarchar(100) NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellido] nvarchar(100) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Telefono] nvarchar(20) NOT NULL,
        [FechaAlta] datetime2 NOT NULL,
        [TipoUsuario] nvarchar(8) NOT NULL,
        [Alumno_EstaActivo] bit NULL,
        [EstaActivo] bit NULL,
        [Titulo] nvarchar(100) NULL,
        [Descripcion] nvarchar(500) NULL,
        CONSTRAINT [PK_Usuario] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Actividades] (
        [ActividadId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [HoraInicio] time NOT NULL,
        [HoraFin] time NOT NULL,
        [ProfesorId] int NOT NULL,
        [Dias] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Actividades] PRIMARY KEY ([ActividadId]),
        CONSTRAINT [FK_Actividades_Usuario_ProfesorId] FOREIGN KEY ([ProfesorId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Alimentaciones] (
        [Id] int NOT NULL IDENTITY,
        [TipoAlimentacion] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(700) NOT NULL,
        [ProfesorId] int NOT NULL,
        CONSTRAINT [PK_Alimentaciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Alimentaciones_Usuario_ProfesorId] FOREIGN KEY ([ProfesorId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [AlumnoSuscripciones] (
        [Id] int NOT NULL IDENTITY,
        [AlumnoId] int NOT NULL,
        [SuscripcionId] int NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NULL,
        [Activa] bit NOT NULL,
        CONSTRAINT [PK_AlumnoSuscripciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AlumnoSuscripciones_Suscripciones_SuscripcionId] FOREIGN KEY ([SuscripcionId]) REFERENCES [Suscripciones] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AlumnoSuscripciones_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Anuncios] (
        [Id] int NOT NULL IDENTITY,
        [Titulo] nvarchar(70) NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [FechaPublicacion] datetime2 NOT NULL,
        [ProfesorId] int NOT NULL,
        CONSTRAINT [PK_Anuncios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Anuncios_Usuario_ProfesorId] FOREIGN KEY ([ProfesorId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Rutinas] (
        [Id] int NOT NULL IDENTITY,
        [Descripcion] nvarchar(700) NOT NULL,
        [Nombre] nvarchar(70) NOT NULL,
        [ProfesorId] int NOT NULL,
        [AlumnoId] int NOT NULL,
        CONSTRAINT [PK_Rutinas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rutinas_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Rutinas_Usuario_ProfesorId] FOREIGN KEY ([ProfesorId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [ActividadesAlumno] (
        [Id] int NOT NULL IDENTITY,
        [AlumnoId] int NOT NULL,
        [ActividadId] int NOT NULL,
        [FechaInscripcion] datetime2 NOT NULL,
        [Activa] bit NOT NULL,
        CONSTRAINT [PK_ActividadesAlumno] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ActividadesAlumno_Actividades_ActividadId] FOREIGN KEY ([ActividadId]) REFERENCES [Actividades] ([ActividadId]) ON DELETE CASCADE,
        CONSTRAINT [FK_ActividadesAlumno_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE TABLE [Pagos] (
        [Id] int NOT NULL IDENTITY,
        [Monto] decimal(10,2) NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [MetodoPago] nvarchar(max) NOT NULL,
        [AlumnoId] int NOT NULL,
        [AlumnoSuscripcionId] int NOT NULL,
        CONSTRAINT [PK_Pagos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Pagos_AlumnoSuscripciones_AlumnoSuscripcionId] FOREIGN KEY ([AlumnoSuscripcionId]) REFERENCES [AlumnoSuscripciones] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Pagos_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Contrasenia', N'Usuario') AND [object_id] = OBJECT_ID(N'[Administradores]'))
        SET IDENTITY_INSERT [Administradores] ON;
    EXEC(N'INSERT INTO [Administradores] ([Id], [Contrasenia], [Usuario])
    VALUES (1, N''/7cq1tNYOK1W9U+6SEyBkA==.Z/OMk9+ZDCsUNaT8Z0Ysy9vW9w8lJZU9GwVgTRhdM4o='', N''Admin123'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Contrasenia', N'Usuario') AND [object_id] = OBJECT_ID(N'[Administradores]'))
        SET IDENTITY_INSERT [Administradores] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Actividades_ProfesorId] ON [Actividades] ([ProfesorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_ActividadesAlumno_ActividadId] ON [ActividadesAlumno] ([ActividadId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_ActividadesAlumno_AlumnoId] ON [ActividadesAlumno] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Alimentaciones_ProfesorId] ON [Alimentaciones] ([ProfesorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_AlumnoSuscripciones_AlumnoId] ON [AlumnoSuscripciones] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_AlumnoSuscripciones_SuscripcionId] ON [AlumnoSuscripciones] ([SuscripcionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Anuncios_ProfesorId] ON [Anuncios] ([ProfesorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Pagos_AlumnoId] ON [Pagos] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Pagos_AlumnoSuscripcionId] ON [Pagos] ([AlumnoSuscripcionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Rutinas_AlumnoId] ON [Rutinas] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    CREATE INDEX [IX_Rutinas_ProfesorId] ON [Rutinas] ([ProfesorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260703003940_InicialCompleta'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260703003940_InicialCompleta', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913195938_UnificarEstaActivoEnUsuario'
)
BEGIN

                    UPDATE Usuario
                    SET EstaActivo = Alumno_EstaActivo
                    WHERE TipoUsuario = 'Alumno';
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913195938_UnificarEstaActivoEnUsuario'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuario]') AND [c].[name] = N'Alumno_EstaActivo');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Usuario] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Usuario] DROP COLUMN [Alumno_EstaActivo];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913195938_UnificarEstaActivoEnUsuario'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuario]') AND [c].[name] = N'EstaActivo');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Usuario] DROP CONSTRAINT ' + @var1 + ';');
    EXEC(N'UPDATE [Usuario] SET [EstaActivo] = CAST(0 AS bit) WHERE [EstaActivo] IS NULL');
    ALTER TABLE [Usuario] ALTER COLUMN [EstaActivo] bit NOT NULL;
    ALTER TABLE [Usuario] ADD DEFAULT CAST(0 AS bit) FOR [EstaActivo];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913195938_UnificarEstaActivoEnUsuario'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913195938_UnificarEstaActivoEnUsuario', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    DROP INDEX [IX_ActividadesAlumno_AlumnoId] ON [ActividadesAlumno];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    ALTER TABLE [Pagos] ADD [EstaActivo] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    ALTER TABLE [Pagos] ADD [FechaBaja] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    CREATE INDEX [IX_Usuario_Dni] ON [Usuario] ([Dni]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    CREATE INDEX [IX_Usuario_Email] ON [Usuario] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    CREATE INDEX [IX_ActividadesAlumno_AlumnoId_ActividadId] ON [ActividadesAlumno] ([AlumnoId], [ActividadId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204152_PagoBajaLogicaYRestricciones'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920204152_PagoBajaLogicaYRestricciones', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204357_EstablecerPagoActivoPorDefecto'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Pagos]') AND [c].[name] = N'EstaActivo');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Pagos] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Pagos] ADD DEFAULT CAST(1 AS bit) FOR [EstaActivo];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920204357_EstablecerPagoActivoPorDefecto'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920204357_EstablecerPagoActivoPorDefecto', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920221807_SuscripcionMensualConFrecuencia'
)
BEGIN
    ALTER TABLE [Suscripciones] ADD [DiasPorSemana] int NOT NULL DEFAULT 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920221807_SuscripcionMensualConFrecuencia'
)
BEGIN

                    UPDATE Suscripciones
                    SET DiasPorSemana = CASE
                        WHEN DuracionDias BETWEEN 1 AND 7 THEN DuracionDias
                        ELSE 7
                    END,
                    DuracionDias = 30;

                    UPDATE AlumnoSuscripciones
                    SET FechaFin = DATEADD(month, 1, FechaInicio)
                    WHERE Activa = 1;
                
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920221807_SuscripcionMensualConFrecuencia'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920221807_SuscripcionMensualConFrecuencia', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920230509_AlimentacionPorAlumno'
)
BEGIN
    ALTER TABLE [Alimentaciones] ADD [AlumnoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920230509_AlimentacionPorAlumno'
)
BEGIN
    CREATE INDEX [IX_Alimentaciones_AlumnoId] ON [Alimentaciones] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920230509_AlimentacionPorAlumno'
)
BEGIN
    ALTER TABLE [Alimentaciones] ADD CONSTRAINT [FK_Alimentaciones_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920230509_AlimentacionPorAlumno'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920230509_AlimentacionPorAlumno', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920231522_RutinaPorActividad'
)
BEGIN
    ALTER TABLE [Rutinas] ADD [ActividadId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920231522_RutinaPorActividad'
)
BEGIN
    CREATE INDEX [IX_Rutinas_ActividadId] ON [Rutinas] ([ActividadId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920231522_RutinaPorActividad'
)
BEGIN
    ALTER TABLE [Rutinas] ADD CONSTRAINT [FK_Rutinas_Actividades_ActividadId] FOREIGN KEY ([ActividadId]) REFERENCES [Actividades] ([ActividadId]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260920231522_RutinaPorActividad'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260920231522_RutinaPorActividad', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924150000_AgregarCupoActividad'
)
BEGIN
    ALTER TABLE [Actividades] ADD [Cupo] int NOT NULL DEFAULT 20;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924150000_AgregarCupoActividad'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924150000_AgregarCupoActividad', N'10.0.8');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    CREATE TABLE [AlimentacionAlumnos] (
        [AlimentacionId] int NOT NULL,
        [AlumnoId] int NOT NULL,
        CONSTRAINT [PK_AlimentacionAlumnos] PRIMARY KEY ([AlimentacionId], [AlumnoId]),
        CONSTRAINT [FK_AlimentacionAlumnos_Alimentaciones_AlimentacionId] FOREIGN KEY ([AlimentacionId]) REFERENCES [Alimentaciones] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AlimentacionAlumnos_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    CREATE TABLE [RutinaAlumnos] (
        [RutinaId] int NOT NULL,
        [AlumnoId] int NOT NULL,
        CONSTRAINT [PK_RutinaAlumnos] PRIMARY KEY ([RutinaId], [AlumnoId]),
        CONSTRAINT [FK_RutinaAlumnos_Rutinas_RutinaId] FOREIGN KEY ([RutinaId]) REFERENCES [Rutinas] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RutinaAlumnos_Usuario_AlumnoId] FOREIGN KEY ([AlumnoId]) REFERENCES [Usuario] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    CREATE INDEX [IX_AlimentacionAlumnos_AlumnoId] ON [AlimentacionAlumnos] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    CREATE INDEX [IX_RutinaAlumnos_AlumnoId] ON [RutinaAlumnos] ([AlumnoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    INSERT INTO RutinaAlumnos (RutinaId, AlumnoId) SELECT Id, AlumnoId FROM Rutinas;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    INSERT INTO AlimentacionAlumnos (AlimentacionId, AlumnoId) SELECT Id, AlumnoId FROM Alimentaciones WHERE AlumnoId IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    ALTER TABLE [Alimentaciones] DROP CONSTRAINT [FK_Alimentaciones_Usuario_AlumnoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    ALTER TABLE [Rutinas] DROP CONSTRAINT [FK_Rutinas_Usuario_AlumnoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    DROP INDEX [IX_Rutinas_AlumnoId] ON [Rutinas];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    DROP INDEX [IX_Alimentaciones_AlumnoId] ON [Alimentaciones];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Rutinas]') AND [c].[name] = N'AlumnoId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Rutinas] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Rutinas] DROP COLUMN [AlumnoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alimentaciones]') AND [c].[name] = N'AlumnoId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Alimentaciones] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Alimentaciones] DROP COLUMN [AlumnoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001202413_AlumnosMultiplesEnRutinasYAlimentacion', N'10.0.8');
END;

COMMIT;
GO

