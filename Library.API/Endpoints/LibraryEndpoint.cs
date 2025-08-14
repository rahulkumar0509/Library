using Library.Domain;
using Library.Services;

namespace Library.API.Endpoints
{
    public static class LibraryEndpoint
    {
        public static void MapLibraryEndpoint(this WebApplication app) // extension method
        {
            var booksApi = app.MapGroup("/v1/books");
            var authorsAPi = app.MapGroup("/v1/authors");

            // books endpoints
            // booksApi.CacheOutput // how to cache the API
            booksApi.MapGet("", (ILibraryService libraryService) =>
            {
                var books = libraryService.GetBooks();
                if (books != null)
                {
                    return Results.Ok(books);
                }
                return Results.Empty;
            });

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

            // authors end point
            authorsAPi.MapPost("", (Author author) =>
            {
                return Results.Ok();
            });
        }
    }
}