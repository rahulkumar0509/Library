using Library.Domain;
using Library.Domain.Dto;
using Library.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Endpoints
{
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private ILibraryService _libraryService;
        private readonly ILogger<LibraryController> _logger;
        public LibraryController(ILibraryService libraryService, ILogger<LibraryController> logger)
        {
            _libraryService = libraryService;
            _logger = logger;
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

        [HttpPost("v2/borrow-book")]
        public IResult BorrowBook(BorrowBook borrowBook)
        {
            _logger.LogInformation("Borrow Book API started!");
            var result = _libraryService.BorrowBook(borrowBook);
            if (result > 0)
            {
                return Results.Created();
            }
            throw new InvalidOperationException("couldn't crate book transaction");
        }

    }
}