using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Profily.Models
{
    public class ProfileGroup
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("Profile")]
        public String ProfileId { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }

        public virtual Profile Profile {get;set;}

        public virtual Group Group { get; set; }
    }
}