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

        [HttpPost("v2/Members")]
        public IResult AddMember(Member member)
        {
            var memberId = _libraryService.AddMember(member);
            if (memberId > 0)
            {
                return Results.Ok(memberId);
            }
            return Results.BadRequest();
        }
        [HttpGet("v2/Members")]
        public IResult GetMembers()
        {
            return Results.Ok(_libraryService.GetMembers());
        }

    }
}