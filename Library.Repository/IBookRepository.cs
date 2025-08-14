using Library.Domain;
namespace Library.Repository{
    public interface IBookRepository{
        public int AddBook(Book book);
        Task DeleteBookAsync(int id);
        public IEnumerable<Book> FetchBook();
    }
}