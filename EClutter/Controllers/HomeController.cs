using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EClutter.Models;
using System.Data.Entity;
using System.Net;
using System.IO;
using System.Web.Mvc.Ajax;


namespace EClutter.Controllers
{
    public class HomeController : Controller
    {
        private DB_A12AA1_DeclutterEntities db = new DB_A12AA1_DeclutterEntities();


        public ActionResult Index()
        {

            var clutter = from i in db.Items
                          where i.ActionCompleted == false
                          orderby  i.SpaceTaken descending

                               select i;
            foreach(Item citem in clutter)
            {
                if (citem.Dispose)
                { citem.ClassActions += " dispose"; }
                if (citem.Sell)
                { citem.ClassActions += " sell"; }
                if (citem.GiveAway)
                { citem.ClassActions += " give"; }
                if (citem.Repurpose)
                { citem.ClassActions += " repupose"; }

            }

            return View(clutter);
        }


        public ActionResult History()
        {

            var clutter = from i in db.Items
                          where i.ActionCompleted == true
                          select i;
            foreach (Item citem in clutter)
            {
                if (citem.Dispose)
                { citem.ClassActions += " dispose"; }
                if (citem.Sell)
                { citem.ClassActions += " sell"; }
                if (citem.GiveAway)
                { citem.ClassActions += " give"; }
                if (citem.Repurpose)
                { citem.ClassActions += " repupose"; }

            }

            return View(clutter);
        }

        public ActionResult Progress()
        {
            var clutter = from i in db.Items
                          select i;
            clutter.First().Disposed = clutter.Where(x => x.Dispose && x.ActionCompleted).Count();
            clutter.First().ForDisposal = clutter.Where(x => x.Dispose && !x.ActionCompleted).Count();
            clutter.First().Sold = clutter.Where(x => x.Sell && x.ActionCompleted).Count();
            clutter.First().ForSale = clutter.Where(x => x.Sell && !x.ActionCompleted).Count();
            clutter.First().GivenAway = clutter.Where(x => x.GiveAway && x.ActionCompleted).Count();
            clutter.First().ForGivingAway = clutter.Where(x => x.GiveAway && !x.ActionCompleted).Count();
            clutter.First().Repurposed = clutter.Where(x => x.Repurpose && x.ActionCompleted).Count();
            clutter.First().ForRepurposing = clutter.Where(x => x.Repurpose && !x.ActionCompleted).Count();
            clutter.First().AmountNotCompleted = clutter.Where(x => !x.ActionCompleted).Count();
            clutter.First().AmountCompleted = clutter.Where(x =>x.ActionCompleted).Count();

            return View(clutter);
        }

        public ActionResult Display(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            Item clutter = db.Items.Find(id);

            if (clutter == null)
            {
                return HttpNotFound();
            }
            return View(clutter);
        }

        public ActionResult CommitAction(int iid,String act)
        {
            Item clutter = db.Items.Find(iid);


            foreach (var item in clutter.Item1.ToList())
            {
                Item subclutter = db.Items.Find(item.Item_ID);
                switch (act)
                {

                    case "Dispose":
                        Defaults(subclutter);
                        subclutter.Dispose = true;
                        break;
                    case "Sell":
                        Defaults(subclutter);
                        subclutter.Sell = true;
                        break;
                    case "Give":
                        Defaults(subclutter);
                        subclutter.GiveAway = true;
                        break;
                    case "Repurpose":
                        Defaults(subclutter);
                        subclutter.Repurpose = true;
                        break;


                } db.Entry(subclutter).State = EntityState.Modified;
                db.SaveChanges();
            }

            switch (act)
            {

                case "Dispose":
                    Defaults(clutter);
                    clutter.Dispose = true;
                    break;
                case "Sell":
                    Defaults(clutter);
                    clutter.Sell = true;
                    break;
                case "Give":
                    Defaults(clutter);
                    clutter.GiveAway = true;
                    break;
                case "Repurpose":
                    Defaults(clutter);
                    clutter.Repurpose = true;
                    break;
           

            }
            db.Entry(clutter).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");

      

        }




        public ActionResult Rooms(String roomName, int? id, String newRoomName)
        {

            var areas = from a in db.Areas
                        select a;


            foreach (var area in areas)
            {
                area.Clutter_Count = area.Items.Where(x => !x.ActionCompleted).Count();
            }

            if (!String.IsNullOrEmpty(roomName))
            { 
            Area area = new Area();
            area.Area_Name = roomName;
            db.Areas.Add(area);
            db.SaveChanges();
            }

            if (!String.IsNullOrEmpty(newRoomName) && id > 0)
            {
                Area area = db.Areas.Find(id);
                area.Area_Name = newRoomName;
                db.Entry(area).State = EntityState.Modified;
                db.SaveChanges();
            }
            else {
                if (id > 0)
                {
                    areas.First().Edit_Name = id;
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("Areas", areas);
            }
            else {
                return View(areas);
            }
        }



        public ActionResult Create(int roomnum, int sub)
        {
            if (roomnum < 1)
            {
                return HttpNotFound();
            }
            Item item = new Item();
            item.Area_ID = roomnum;
            item.SubItem = sub;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HttpPostedFileBase file, [Bind(Include = "Item_ID,Name,SpaceTaken,Location,Sell,Dispose,Repurpose,GiveAway,SubItem,Area_ID")]Item clutteritem)
        {
            if (ModelState.IsValid)
            {

                if (Request.Files.Count > 0)
                {
                    file = Request.Files[0];
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var checkextension = Path.GetExtension(file.FileName).ToLower();
                    bool validextension = false;
                    if (file.ContentLength > 0)
                    {
                        foreach (var itm in allowedExtensions)
                        {
                            if (itm.Contains(checkextension))
                            {
                                validextension = true;
                            }
                        }
                        if (validextension) { 
                        string fileName = Path.GetFileName(file.FileName);
                        string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                        file.SaveAs(path);
                        clutteritem.CoverImage = fileName;}
                    
                    }
                    else
                    {
                        clutteritem.CoverImage = "DSC_0001.JPG";
                    }
                }
                if (clutteritem.CoverImage == null || clutteritem.CoverImage == "")
                {
                    clutteritem.CoverImage = "NoImage?";
                }
                if (clutteritem.Location == null || clutteritem.Location == "")
                {
                    clutteritem.Location = "Unspecified";
                }
                clutteritem.ActionCompleted = false;
                clutteritem.Moved = 0;
                db.Items.Add(clutteritem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        

               
        public ActionResult Delete(int? id)
        {
            Item clutteritem = db.Items.Find(id);
            foreach (var item in clutteritem.Item1.ToList())
            {
                db.Items.Remove(item);
            }
            db.Items.Remove(clutteritem);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult DeleteCollectionOnly(int? id)
        {
            Item clutteritem = db.Items.Find(id);
            foreach (var item in clutteritem.Item1.ToList())
            {
                Item subclutter = db.Items.Find(item.Item_ID);
                subclutter.SubItem = clutteritem.SubItem;
                db.Entry(subclutter).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Items.Remove(clutteritem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteRoom(int? id)
        {
            List<Item> items = new List<Item>();
            items = db.Items.Where(x => x.Area_ID == id).ToList();

            foreach (var item in items)
            {
                db.Items.Remove(item);
            }
            Area area = db.Areas.Find(id);
            db.Areas.Remove(area);
            db.SaveChanges();
            return RedirectToAction("Rooms");

        }

        public ActionResult Edit(int? id)
        {
            Item clutteritem = db.Items.Find(id);

            return View(clutteritem);
        }

                [HttpPost, ActionName("Edit")]
        public ActionResult Edit(HttpPostedFileBase file,
    [Bind(Include = "Item_ID,Name,SpaceTaken,Location,Moved,CoverImage,Sell,Dispose,Repurpose,GiveAway,ActionCompleted,SubItem,Area_ID")]
            Item clutteritem)
        {
            if (ModelState.IsValid)
            {

                if (Request.Files.Count > 0)
                {
                    file = Request.Files[0];
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var checkextension = Path.GetExtension(file.FileName).ToLower();
                    bool validextension = false;
                    if (file.ContentLength > 0)
                    {
                        foreach (var itm in allowedExtensions)
                        {
                            if (itm.Contains(checkextension))
                            {
                                validextension = true;
                            }
                        }
                        if (validextension)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                            file.SaveAs(path);
                            clutteritem.CoverImage = fileName;
                        }

                    }
                 
                }
                if (clutteritem.Location == null || clutteritem.Location == "")
                {
                    clutteritem.Location = "Unspecified";
                }

                db.Entry(clutteritem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clutteritem);
        }

                public Item Defaults(Item clutter)
                {
                    clutter.Dispose = false;
                    clutter.GiveAway = false;
                    clutter.Sell = false;
                    clutter.Repurpose = false;
                    clutter.ActionCompleted = true; 
                    return clutter;
                }
    }
}