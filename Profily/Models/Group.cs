using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String Name { get; set; }

        public String Description { get; set; }

        public virtual ICollection<ProfileGroup> ProfileGroups { get; set; }


    }
}