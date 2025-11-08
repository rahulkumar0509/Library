using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain
{
    public class BookAuthors
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        // [ForeignKey("BookId")]
        public Book? book { get; set; }
        // [ForeignKey("AuthorId")]
        public Author? author { get; set; }
    }
}