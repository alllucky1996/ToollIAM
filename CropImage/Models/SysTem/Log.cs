using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CropImage.Models.SysTem
{
    public class Log
    {
        [Key]
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime TimeOccur { get; set; }
        public string Descript { get; set; }
        public DateTime CreateDate { get; set; }
        public Log()
        {
            TimeOccur = DateTime.Now;
            this.CreateDate = DateTime.Now;
        }
        private DataContext _db;
        public async Task<int> CreateAsync(long? accountId, Log log)
        {
            _db = new DataContext();
            log.AccountId = accountId.Value;
            _db.Logs.Add(log);
            return await _db.SaveChangesAsync();
        }
    }
    public class LogHelper<T> where T:class, new()
    {
        Log l = new Log();
        private DataContext _db;
        public LogHelper() { _db = new DataContext(); }
        public LogHelper(DataContext db) {
              _db = db;
          
        }
        //public async Task<int> CreateAsync(long? accountId, string value)
        //{
        //    var _log = new Log();

        //    _log.EntityName = typeof(T).Name;
        //    _log.Action = "Create";
        //    _log.AccountId = accountId.Value;
        //    _log.NewValue = value;
        //    _log.Descript = "Thêm mới" + _log.EntityName;
        //    _db.Logs.Add(_log);
        //    return await _db.SaveChangesAsync();
        //}
        public async Task<int> CreateAsync(long? accountId, string value, string descript= null)
        {
            var _log = new Log();

            _log.EntityName = typeof(T).Name;
           
            _log.Action = "Create";
            _log.AccountId = accountId.Value;
            _log.NewValue = value;
            _log.Descript = descript==null? "Thêm mới " + _log.EntityName: descript;
            _db.Logs.Add(_log);
            return await _db.SaveChangesAsync();
        }
        public async Task<int> CreateAsync(long? accountId, string value,string action, string descript = null)
        {
            var _log = new Log();

            _log.EntityName = typeof(T).Name; 
            _log.Action = action;
            _log.AccountId = accountId.Value;
            _log.NewValue = value;
            _log.Descript = descript == null ? _log.Action + _log.EntityName : descript;
            _db.Logs.Add(_log);
            return await _db.SaveChangesAsync();
        }
        public async Task<int> CreateLogAsync(long? accountId, Log log)
        {
            _db.Logs.Add(log);
            return await _db.SaveChangesAsync();
        }
        public int Create(long? accountId, string value)
        {
            var _log = new Log();

            _log.EntityName = typeof(T).Name;
            _log.Action = "Create";
            _log.AccountId = accountId.Value;
            _log.NewValue = value;
            _log.Descript = "Thêm mới" + _log.EntityName;
            _db.Logs.Add(_log);
            return  _db.SaveChanges();
        }
    }
    
}