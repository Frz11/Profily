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
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public bool ProfilePhoto { get; set; }

        [Required]
        public bool BackgroundPhoto { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("Album")]
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }
    }
  
}