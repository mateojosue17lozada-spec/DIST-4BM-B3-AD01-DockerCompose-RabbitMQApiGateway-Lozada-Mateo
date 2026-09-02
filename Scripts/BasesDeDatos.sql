USE [master]
GO

CREATE LOGIN [usuario_pacientes] WITH PASSWORD = '1234', CHECK_POLICY = OFF
GO
CREATE LOGIN [usuario_historial] WITH PASSWORD = '1234', CHECK_POLICY = OFF
GO

CREATE DATABASE [PacientesDB]
GO

ALTER DATABASE [PacientesDB] SET COMPATIBILITY_LEVEL = 150
GO

USE [PacientesDB]
GO

CREATE USER [usuario_pacientes] FOR LOGIN [usuario_pacientes] WITH DEFAULT_SCHEMA=[dbo]
GO

ALTER ROLE [db_datareader] ADD MEMBER [usuario_pacientes]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [usuario_pacientes]
GO

CREATE TABLE [dbo].[Pacientes](
    [IdPaciente] [int] IDENTITY(1,1) NOT NULL,
    [Cedula] [varchar](20) NOT NULL,
    [Nombre] [varchar](100) NOT NULL,
    [Apellido] [varchar](100) NOT NULL,
    [Direccion] [varchar](200) NOT NULL,
    CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([IdPaciente] ASC)
)
GO

CREATE UNIQUE INDEX [IX_Pacientes_Cedula] ON [dbo].[Pacientes] ([Cedula])
GO

CREATE DATABASE [HistorialDB]
GO

ALTER DATABASE [HistorialDB] SET COMPATIBILITY_LEVEL = 150
GO

USE [HistorialDB]
GO

CREATE USER [usuario_historial] FOR LOGIN [usuario_historial] WITH DEFAULT_SCHEMA=[dbo]
GO

ALTER ROLE [db_datareader] ADD MEMBER [usuario_historial]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [usuario_historial]
GO

CREATE TABLE [dbo].[HistorialClinico](
    [IdHistorial] [int] IDENTITY(1,1) NOT NULL,
    [IdPaciente] [int] NOT NULL,
    [NumHistoria] [varchar](50) NOT NULL,
    [Diagnostico] [varchar](500) NOT NULL,
    [Tratamiento] [varchar](500) NOT NULL,
    [Fecha] [datetime] NOT NULL,
    CONSTRAINT [PK_HistorialClinico] PRIMARY KEY CLUSTERED ([IdHistorial] ASC)
)
GO

CREATE INDEX [IX_HistorialClinico_IdPaciente] ON [dbo].[HistorialClinico] ([IdPaciente])
GO

CREATE UNIQUE INDEX [IX_HistorialClinico_NumHistoria] ON [dbo].[HistorialClinico] ([NumHistoria])
GO

ALTER TABLE [dbo].[HistorialClinico] ADD CONSTRAINT [DF_HistorialClinico_Fecha] DEFAULT (GETDATE()) FOR [Fecha]
GO

USE [master]
GO