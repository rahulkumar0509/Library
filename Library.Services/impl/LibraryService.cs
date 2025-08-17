using System.Text.Json;
using Library.Domain;
using Library.Domain.Dto;
using Library.Repository;
namespace Library.Services.Impl
{
    public class LibraryService : ILibraryService
    {
        IBookRepository _bookRepository;
        IAuthorRepository _authorRepository;
        IBookAuthorsRepository _bookAuthorsRepository;
        IMemberRepository _memberRepository;
        ILoanRepository _loanRepository;
        ILogger<LibraryService> _logger;
        public LibraryService(ILoanRepository loanRepository, IBookRepository bookRepository, IAuthorRepository authorRepository, IBookAuthorsRepository bookAuthorsRepository,
            IMemberRepository memberRepository, ILogger<LibraryService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookAuthorsRepository = bookAuthorsRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
            _logger = logger;
        }
        public IEnumerable<Book> GetBooks()
        {
            return _bookRepository.FetchBook();
        }
        public int AddBook(Book book)
        {
            return _bookRepository.AddBook(book);
        }

        public int AddAuthor(Author author)
        {
            return _authorRepository.AddAuthor(author);
        }
        public IEnumerable<Author> GetAuthors()
        {
            return _authorRepository.GetAuthors();
        }

        public int AddBookAuthorDetails(BookInfo bookInfo)
        {
            var bookId = _bookRepository.AddBook(bookInfo.Book);
            var authorId = _authorRepository.AddAuthor(bookInfo.Author);
            return _bookAuthorsRepository.AddBookAuthor(bookId, authorId);
        }

        public int AddMember(Member member)
        {
            if (String.IsNullOrWhiteSpace(member.Email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(member.Email));
            }
            Member existingMember = _memberRepository.GetMemberByEmail(member.Email);
            if (existingMember == null)
            {
                return _memberRepository.AddMember(member);
            }
            else
            {
                throw new InvalidOperationException($"User with email '{member.Email}' already exists.");
            }
        }

        public IEnumerable<Member> GetMembers()
        {
            return _memberRepository.GetMembers();
        }

        public int BorrowBook(BorrowBook bookRequest)
        {
            // find author id
            if(bookRequest == null){
                throw new ArgumentNullException("Borrow request can not be null");
            }
            if (String.IsNullOrEmpty(bookRequest.BookIsbn) || bookRequest.MemberId <= 0)
            {
                throw new ArgumentException("Book details and MemberId must be valid.", nameof(bookRequest));
            }

            // check if book is already rented;

            // var author = _authorRepository.GetAuthorByName(bookRequest.AuthorFullName);
            var book = _bookRepository.GetBookByDetails(bookRequest.BookIsbn, bookRequest.BookTitle);
            Loan loan = new Loan();
            loan.MemberId = bookRequest.MemberId;
            loan.BookId = book.BookId;
            var loanJson = JsonSerializer.Serialize(loan);
            _logger.LogInformation("Loggin Loan Object: {loanJson}", loanJson);
            return _loanRepository.BorrowBook(loan);
        }
    }
}