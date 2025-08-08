using Library.Domain;
namespace Library.Services.Impl
{
    public class LibraryService : ILibraryService
    {
        public Task<IEnumerable<Book>> GetBooksAsync()
        {
            return null;
        }
        public Task AddBookAsync(Book book)
        {
            return null;
        }
    }
}