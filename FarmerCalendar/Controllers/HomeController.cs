using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DHTMLX.Scheduler;
using System.Data;
using System.Data.Entity;
using DHTMLX.Scheduler.Data;
using DHTMLX.Common;
using FarmerCalendar.Models;
using DHTMLX.Scheduler.Controls;

namespace FarmerCalendar.Controllers
{
    public class HomeController : Controller
    {
        private CalendarContext db = new CalendarContext();

        //
        // GET: /Home/
        public ActionResult Index()
        {
            var scheduler = new DHXScheduler(this);
            
            

            scheduler.Skin = DHXScheduler.Skins.Flat;
                                                
            // Checkbox
            var check = new LightboxCheckbox("highlighting", "Task done");
            check.MapTo = "textColor";
            check.CheckedValue = "red";
            


            scheduler.Lightbox.Add(check);
            scheduler.Lightbox.Add(new LightboxText("text", "Description"));
            


            scheduler.Config.first_hour = 6;
            scheduler.Config.last_hour = 20;

            scheduler.LoadData = true;
            scheduler.EnableDataprocessor = true;

            var select = new LightboxSelect("key", "User");
            var items = new List<object>(){
                new { key = "1", label = "Farmer1" },
                new { key = "2", label = "Farmer2" },
                new { key = "3", label = "Farmer3" }
};
            select.AddOptions(items);
            scheduler.Lightbox.Add(select);


            
            
                       
       
            return View(scheduler);
        }

        public ContentResult Data()
        {
           

            {
                var apps = db.Tasks.ToList();
                return new SchedulerAjaxData(apps);


            }
        }



        public ActionResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            

            try
            {
                var changedEvent = DHXEventsHelper.Bind<Task>(actionValues);
                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        db.Tasks.Add(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        db.Entry(changedEvent).State = EntityState.Deleted;
                        break;
                    default:// "update"  
                        db.Entry(changedEvent).State = EntityState.Modified;
                        break;
                }
                db.SaveChanges();
                action.TargetId = changedEvent.Id;
            }
            catch (Exception)
            {
                action.Type = DataActionTypes.Error;
            }

            return (new AjaxSaveResponse(action));
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