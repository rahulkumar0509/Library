using Library.Domain;
using Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Endpoints
{
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private ILibraryService _libraryService;
        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet("/v2/Books")] // name is path parameter and mandatory
        public IResult GetBooks() // type is query parameter
        {
            var books = _libraryService.GetBooks();
            if (books is not null)
            {
                return Results.Ok(books);
            }
            return Results.Empty;
        }

        
    }
}