using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class Role
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public string DesCription { get; set; }
      
        public virtual ICollection<AccountRole> AccountRoles { get; set; }

    }
    
}