using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        [Required]
        public DateTime LoanDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        [Required]
        public bool isReturned { get; set; }

        [ForeignKey("BookId")]
        public Book? book { get; set; }

        [ForeignKey("AuthorId")]
        public Author? author { get; set; }
    }
}