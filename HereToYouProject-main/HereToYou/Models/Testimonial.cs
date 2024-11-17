using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HereToYou.Models
{
    public class Testimonial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [MaxLength(255)]
        public string Content { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }// 'Pending', 'Approved', 'Rejected'


    }
}
