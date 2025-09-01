using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain
{
    [Table("Authors")]
    public class Author
    {
        [Key]
        [Column("AuthorId")]
        public int AuthorId { get; set; } // define as public so it can be used in other class; which means same model can be used as payload for endpoint
        [StringLength(50, ErrorMessage = "First Name should be a maximum of 50 chars.")]
        public string? FirstName { get; set; }
        [StringLength(50, ErrorMessage = "Last Name should be a maximum of 50 chars.")]
        public string? LastName { get; set; }
        public string? Biography { get; set; }
    }
}