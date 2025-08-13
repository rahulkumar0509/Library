CREATE Table Book(
    Id Int Identity(1000,1),
    Title Varchar(250),
    Description Varchar(500),
    ISBN Varchar(100),
    AuthorId Int,
    PublisherId Int,
    ReviewId Int
);