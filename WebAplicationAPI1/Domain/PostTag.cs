using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Domain
{
    public class PostTag
    {
        public Guid PostTagId { get; set; }
        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }
        public string TagName { get; set; }
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }
        public Guid PostId { get; set; }
    }
}
