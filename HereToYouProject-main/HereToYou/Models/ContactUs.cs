using System.ComponentModel.DataAnnotations.Schema;

namespace HereToYou.Models
{
	public class ContactUs
	{
		public int ContactUsId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }

        public string Message { get; set; }
		public DateTime DateSubmitted { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

    }

}
