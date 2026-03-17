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
IF SCHEMA_ID(N'LibrarySchema') IS NULL EXEC(N'CREATE SCHEMA [LibrarySchema];');

CREATE TABLE [LibrarySchema].[Authors] (
    [AuthorId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [Biography] nvarchar(max) NULL,
    CONSTRAINT [PK_Authors] PRIMARY KEY ([AuthorId])
);

CREATE TABLE [LibrarySchema].[Books] (
    [falana] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NULL,
    [ISBN] nvarchar(13) NULL,
    [PublicationYear] int NOT NULL,
    [Genre] nvarchar(50) NULL,
    [CopiesAvailable] int NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([falana])
);

CREATE TABLE [LibrarySchema].[Members] (
    [MemberId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [RegistrationDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([MemberId])
);

CREATE TABLE [LibrarySchema].[BookAuthors] (
    [BookId] int NOT NULL,
    [AuthorId] int NOT NULL,
    CONSTRAINT [FK_BookAuthors_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [LibrarySchema].[Authors] ([AuthorId]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookAuthors_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [LibrarySchema].[Books] ([falana]) ON DELETE CASCADE
);

CREATE TABLE [LibrarySchema].[Loans] (
    [LoanId] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [MemberId] int NOT NULL,
    [LoanDate] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [ReturnDate] datetime2 NOT NULL,
    [isReturned] bit NOT NULL,
    [AuthorId] int NULL,
    CONSTRAINT [PK_Loans] PRIMARY KEY ([LoanId]),
    CONSTRAINT [FK_Loans_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [LibrarySchema].[Authors] ([AuthorId]),
    CONSTRAINT [FK_Loans_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [LibrarySchema].[Books] ([falana]) ON DELETE CASCADE
);

CREATE INDEX [IX_BookAuthors_AuthorId] ON [LibrarySchema].[BookAuthors] ([AuthorId]);

CREATE INDEX [IX_BookAuthors_BookId] ON [LibrarySchema].[BookAuthors] ([BookId]);

CREATE INDEX [IX_Loans_AuthorId] ON [LibrarySchema].[Loans] ([AuthorId]);

CREATE INDEX [IX_Loans_BookId] ON [LibrarySchema].[Loans] ([BookId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111193835_InitialCreate', N'9.0.8');

EXEC sp_rename N'[LibrarySchema].[Books].[Genre]', N'GenreNew', 'COLUMN';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111194955_RenameGenre', N'9.0.8');

COMMIT;
GO