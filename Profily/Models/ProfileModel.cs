using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class Profile
    {

        public String Description { get; set; }

        public String Birthday { get; set; }

        public String Work { get; set; }
        public String School { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [Required]
        [Key,ForeignKey("ApplicationUser")]

        public String UserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Album> Albums { get; set; }

        public virtual ICollection<ProfileGroup> ProfileGroups { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }


       
    }
   
}