using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public bool Accepted { get; set; }

        [ForeignKey("Photo")]
        public int PhotoId { get; set; }

        public virtual Photo Photo { get; set; }

        [ForeignKey("Profile")]
        public String ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

    }
}