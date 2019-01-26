using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class Image
    {
        [Key]
        public long Id { get; set; }
        public string code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Uri { get; set; }
        //0: chưa crop
        //1: đang crop
        //2: đã crop
        public int TrangThai { get; set; }
    }
   
}