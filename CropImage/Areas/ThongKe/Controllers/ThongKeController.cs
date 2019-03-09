using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CropImage.Controllers;
using CropImage.Models.ViewModels.ThongKe;
using Newtonsoft.Json;

namespace CropImage.Areas.ThongKe.Controllers
{
    public class ThongKeController : BaseController
    {
        // GET: ThongKe/ThongKe
        public async Task<ActionResult> Index()
        {
            if (accountId == -1) return Redirect(GoToLogIn(Request.Url.AbsolutePath));  
            var tongQuan = new TongQuan();
            // lấy ra danh sách các ng làm dữ liệu
            var ac = await db.AccountRoles.Where(o=>o.Roles.Code== "NguoiCat").CountAsync();
            // lấy danh sách các user dang onine
            var acOnline = await db.Accounts.Where(o => o.IsOnline == true).CountAsync();

            tongQuan.User = ac ;
            tongQuan.UserOnline = acOnline ;
            ViewBag.tongQuan = tongQuan;
            return View();
        }
        /// <summary>
        /// xem tất cả hình con trên ảnh gốc
        /// </summary>
        /// <param name="id">id Image</param>
        /// <returns></returns>
        public async Task<ActionResult> ListInRoot(long? id)
        {
            if (id == null) id = 2; // return Redirect("/Core/Images/Index");
            var item = await db.Images.FindAsync(id);
            ViewBag.Positions = JsonConvert.SerializeObject(item.ListCroped);
            ViewBag.Img = item.Uri;
            return View();
        }


    }
}