using CropImage.Areas;
using CropImage.Models;
using CropImage.Models.SysTem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CropImage.Controllers
{
    public class BaseController : Controller
    {
        // protected long accountId;
        public string AccountName
        {
            get
            {
                object objAccountName = Session[SessionEnum.FullName];
                if (objAccountName == null)
                {
                    if (accountId == -1) return "unnamed";
                    return db.Accounts.Find(accountId).FullName;
                }
                return objAccountName.ToString();
            }
            set { Session[SessionEnum.AccountId] = value; }
        }
        protected long accountId
        {
            get
            {
                long _AccountId = -1;
                if (Session == null) return -1;
                object objAccountId = Session[SessionEnum.AccountId];

                if (objAccountId == null)
                    _AccountId = -1;
                else
                    _AccountId = Convert.ToInt64(objAccountId.ToString());
                return _AccountId;
            }
            set { Session[SessionEnum.AccountId] = value; }
        }//Session[SessionEnum.IsManageAccount] = account.IsManageAccount;
        protected DataContext db = new DataContext();
    //    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        if (Session[SessionEnum.FullName] == null || Session[SessionEnum.FullName].ToString() == "")
    //        {
    //            base.OnActionExecuting(filterContext);
    //            filterContext.Result = new RedirectToRouteResult(
    //  new RouteValueDictionary {
    
    //  { "controller", "Login" },
    //  { "action", "Login" }
    //});
    //            return;
    //        }
    //    }
        public string GoToLogIn(string urlTarget) {
            return "/Login/Index?returnUrl="+urlTarget;
        }
    }
}