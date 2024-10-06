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
GO

CREATE TABLE [DoctorTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_DoctorTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Humans] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Surname] nvarchar(100) NOT NULL,
    [Patronymic] nvarchar(100) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(100) NOT NULL,
    [Type] int NOT NULL,
    [Description] nvarchar(500) NULL,
    [Office] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Humans] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Services] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Price] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Services] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [HumanDoctorTypes] (
    [Id] int NOT NULL IDENTITY,
    [HumanId] int NOT NULL,
    [DoctorTypeId] int NOT NULL,
    CONSTRAINT [PK_HumanDoctorTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HumanDoctorTypes_DoctorTypes_DoctorTypeId] FOREIGN KEY ([DoctorTypeId]) REFERENCES [DoctorTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HumanDoctorTypes_Humans_HumanId] FOREIGN KEY ([HumanId]) REFERENCES [Humans] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ServiceDoctorTypes] (
    [Id] int NOT NULL IDENTITY,
    [ServiceId] int NOT NULL,
    [DoctorTypeId] int NOT NULL,
    CONSTRAINT [PK_ServiceDoctorTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceDoctorTypes_DoctorTypes_DoctorTypeId] FOREIGN KEY ([DoctorTypeId]) REFERENCES [DoctorTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ServiceDoctorTypes_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ServiceLists] (
    [Id] int NOT NULL IDENTITY,
    [DateTime] datetime2 NOT NULL,
    [ClientId] int NOT NULL,
    [DoctorId] int NOT NULL,
    [ServiceId] int NOT NULL,
    [Price] int NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_ServiceLists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceLists_Humans_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Humans] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ServiceLists_Humans_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Humans] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ServiceLists_Services_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_HumanDoctorTypes_DoctorTypeId] ON [HumanDoctorTypes] ([DoctorTypeId]);
GO

CREATE INDEX [IX_HumanDoctorTypes_HumanId] ON [HumanDoctorTypes] ([HumanId]);
GO

CREATE INDEX [IX_ServiceDoctorTypes_DoctorTypeId] ON [ServiceDoctorTypes] ([DoctorTypeId]);
GO

CREATE INDEX [IX_ServiceDoctorTypes_ServiceId] ON [ServiceDoctorTypes] ([ServiceId]);
GO

CREATE INDEX [IX_ServiceLists_ClientId] ON [ServiceLists] ([ClientId]);
GO

CREATE INDEX [IX_ServiceLists_DoctorId] ON [ServiceLists] ([DoctorId]);
GO

CREATE INDEX [IX_ServiceLists_ServiceId] ON [ServiceLists] ([ServiceId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241006162134_InitialCreate', N'8.0.8');
GO

COMMIT;
GO

