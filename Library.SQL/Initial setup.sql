CREATE Database LibraryDb;
Use LibraryDb;

SELECT * FROM LibraryDb.LibrarySchema.Loans;

Create SCHEMA LibrarySchema; -- run this single line alone

CREATE Table LibrarySchema.Books(
    BookId Int Identity(1000,1) Primary Key,
    Title Varchar(255),
    ISBN Varchar(13) NOT NULL UNIQUE,
    PublicationYear Int,
    Genre VARCHAR(50)
);

ALTER TABLE LibrarySchema.Books 
ALTER COLUMN ISBN VARCHAR(13) NOT NULL;

ALTER TABLE LibrarySchema.Books
ADD CONSTRAINT UQ_ISBN UNIQUE(ISBN);

Select * from LibrarySchema.Books;

CREATE TABLE LibrarySchema.Authors(
    AuthorId Int IDENTITY(5000,1) PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Biography Text
);

CREATE TABLE LibrarySchema.BookAuthors(
    BookId Int,
    AuthorId Int,
    PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthors_BookId FOREIGN KEY (BookId) REFERENCES LibrarySchema.Books(BookId),
    CONSTRAINT FK_BookAuthors_AuthorId FOREIGN KEY (AuthorId) REFERENCES LibrarySchema.Authors(AuthorId)
);

CREATE TABLE LibrarySchema.Members(
    MemberId Int IDENTITY(10000, 1) PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    RegistrationDate DATETIME
);

CREATE TABLE LibrarySchema.Loans(
    LoanId Int IDENTITY(20000,1) PRIMARY KEY,
    BookId Int,
    MemberId Int,
    LoanDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME,
    IsReturned BIT NOT NULL,
    CONSTRAINT FK_Loans_BookId FOREIGN KEY (BookId) REFERENCES LibrarySchema.Books(BookId),
    CONSTRAINT FK_Loans_MemberId FOREIGN KEY (MemberId) REFERENCES LibrarySchema.Members(MemberId)
);

Select * FROM LibrarySchema.Loans;

SELECT * FROM LibrarySchema.Loans WHERE BookId = (SELECT BookId from LibrarySchema.Books WHERE ISBN = '9780330505543' AND Title = 'India After Gandhi');

ALTER TABLE LibrarySchema.Books ADD CopiesAvailable INT;

SELECT * FROM LibrarySchema.Books;

Update LibrarySchema.Books SET CopiesAvailable = 10;
SELECT GETDATE() + 5

DELETE LibrarySchema.Loans;

SELECT * FROM LibrarySchema.Members WHERE Email = 'Shivangi.singh@gmail.com';

DELETE LibrarySchema.Members;