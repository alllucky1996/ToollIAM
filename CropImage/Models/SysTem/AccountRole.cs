using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class AccountRole
    {
        [Key]
        public long Id { get; set; }
        public long AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account Accounts { get; set; }
        public string CoreRole { get; set; }
        [ForeignKey("CoreRole")]
        public virtual  Role Roles { get; set; }
        public DateTime CreateDate { get; set; }
        public string DesCription { get; set; }
      public AccountRole()
        {
            this.CreateDate = DateTime.Now;
        }

    }
    
}