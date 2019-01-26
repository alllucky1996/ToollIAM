using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class Khoa
    {
        [Key]
        public long Id { get; set; }
        public string KeyValue { get; set; }
        public string Description { get; set; }

    }
   
}