using Library.Domain;
namespace Library.Repository{
    public interface IBookRepository{
        Task AddBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}