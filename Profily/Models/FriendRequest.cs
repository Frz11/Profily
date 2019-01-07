using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String SenderId { get; set; }
        [Required]
        public String ReceiverId { get; set; }
        [Required]
        public bool Accepted { get; set; }

    }
}