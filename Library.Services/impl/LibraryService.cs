using Library.Domain;
using Library.Repository;
namespace Library.Services.Impl
{
    public class LibraryService : ILibraryService
    {
        IBookRepository _bookRepository;
        IAuthorRepository _authorRepository;
        IBookAuthorsRepository _bookAuthorsRepository;
        IMemberRepository _memberRepository;
        public LibraryService(IBookRepository bookRepository, IAuthorRepository authorRepository, IBookAuthorsRepository bookAuthorsRepository, IMemberRepository memberRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookAuthorsRepository = bookAuthorsRepository;
            _memberRepository = memberRepository;
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

        public int AddBookAuthor(int bookId, int authorId)
        {
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
    }
}