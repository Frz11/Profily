using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public String Sender { get; set; }
        public String Receiver { get; set; }

        [Required]
        public String Text { get; set; }

        [Required]
        public String SentAt { get; set; }
    }
}