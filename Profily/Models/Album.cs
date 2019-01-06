using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class Album
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }

        public String Description { get; set; }

        public Boolean IsDefault { get; set; }

        [ForeignKey("Profile")]
        public String ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}