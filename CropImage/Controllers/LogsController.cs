using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CropImage.Models;
using CropImage.Models.SysTem;

namespace CropImage.Controllers
{
    public class LogsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Logs
        [HttpGet]
        public async Task<ActionResult> Index(int? take, int? skip, long? acid)
        {

            try
            {
                int tk = take ?? 10;
                int sk = skip ?? 0;
                
                if(acid!= null)
                {
                    var listByAc = await db.Logs.Where(o=>o.AccountId == acid).OrderByDescending(o => o.CreateDate).Skip(sk).Take(tk).ToListAsync();
                    return View(listByAc);
                }
                var list = await db.Logs.OrderByDescending(o => o.CreateDate).Skip(sk).Take(tk).ToListAsync();
                //  return View(await db.Logs.ToListAsync());
                return View(list);
            }
            catch (Exception ex)
            {
                return View(await db.Logs.ToListAsync());
            }
        }

        // GET: Logs/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = await db.Logs.FindAsync(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

       

        // GET: Logs/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Log log = await db.Logs.FindAsync(id);
            if (log == null)
            {
                return HttpNotFound();
            }
            return View(log);
        }

        // POST: Logs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Log log = await db.Logs.FindAsync(id);
            db.Logs.Remove(log);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
