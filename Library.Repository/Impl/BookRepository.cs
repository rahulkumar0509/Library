using Library.Domain;
using Library.Repository;
namespace Library.Repository.Impl{
    public class BookRepository: IBookRepository {
        public Task AddBookAsync(Book book)
        {
            return null;
        }
        public Task DeleteBookAsync(int id)
        {
            return null;
        }

    }
}