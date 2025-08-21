using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
        public string? Title { get; set; }
        [StringLength(13)]
        public string? ISBN { get; set; }
        public int PublicationYear { get; set; }
        [StringLength(50)]
        public string? Genre { get; set; }
        public int CopiesAvailable { get; set; }
        
        // [InverseProperty("Book")]
        // public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}