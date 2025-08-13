using Library.Domain;
using Library.Repository;
namespace Library.Services.Impl
{
    public class LibraryService : ILibraryService
    {
        IBookRepository _bookRepository;
        public LibraryService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }
        public Task<IEnumerable<Book>> GetBooksAsync()
        {
            return null;
        }
        public Task AddBookAsync(Book book)
        {
            _bookRepository.FetchBook();
            return null;
        }
    }
}