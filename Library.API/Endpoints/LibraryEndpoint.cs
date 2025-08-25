using Library.Domain;
using Library.Domain.Dto;
using Library.Services;

namespace Library.API.Endpoints
{
    public static class LibraryEndpoint
    {
        public static void MapLibraryEndpoint(this WebApplication app) // extension method
        {
            var booksApi = app.MapGroup("/v1/books");
            var authorsAPi = app.MapGroup("/v1/authors");
            var bookAuthors = app.MapGroup("/v1/bookAuthors");

            // books endpoints
            // booksApi.CacheOutput // how to cache the API
            booksApi.MapGet("/mostpopularbooks", ()=>
            {
                
            });

            booksApi.MapGet("", (ILibraryService libraryService) =>
            {
                var books = libraryService.GetBooks();
                if (books != null)
                {
                    return Results.Ok(books);
                }
                return Results.Empty;
            }).RequireAuthorization();

            booksApi.MapPost("", (Book book, ILibraryService libraryService) =>
            {
                int bookId = libraryService.AddBook(book);
                if (bookId > 0)
                {
                    return Results.Ok(bookId);
                }
                else
                {
                    throw new Exception("Book failed to add in the table");
                }
            });

            booksApi.MapPost("/details", (BookInfo bookInfo, ILibraryService libraryService) =>
            {
                libraryService.AddBookAuthorDetails(bookInfo);
            });

            // authors end point
            authorsAPi.MapPost("", (Author author, ILibraryService libraryService) =>
            {
                int authorId = libraryService.AddAuthor(author);
                if (authorId > 0)
                {
                    return Results.Ok(authorId);
                }
                else
                {
                    throw new Exception("Author couldn't added");
                }
            });

            authorsAPi.MapGet("", (ILibraryService libraryService) =>
            {
                var authors = libraryService.GetAuthors();
                return Results.Ok(authors);
            });

            // bookAuthors.MapPost("", )
            // add new book, author and their entry
        }
    }
}