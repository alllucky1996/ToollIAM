﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models
{
    public class TrangThai
    {
        [Key]
        public int Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? isTTBatDau { get; set; }
        public bool? isTTKetThuc { get; set; }
    }
    
}