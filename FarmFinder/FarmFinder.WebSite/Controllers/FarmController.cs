﻿using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using CH.Tutteli.FarmFinder.Dtos;
using CH.Tutteli.FarmFinder.Website.Models;

namespace CH.Tutteli.FarmFinder.Website.Controllers
{
    public class FarmController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Farm
        public async Task<ActionResult> Index()
        {
            return View(await db.Farms.Where(f => f.DeleteWhenRemovedFromIndex == false).ToListAsync());
        }

        // GET: Farm/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Farm farm = await db.Farms.FindAsync(id);
            if (farm == null)
            {
                return HttpNotFound();
            }
            return View(farm);
        }

        // GET: Farm/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Farm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FarmId,Name,Latitude,Longitude,Address,City,Zip,Email,Website,PhoneNumber")] Farm farm)
        {
            if (ModelState.IsValid)
            {
                farm.UpdateDateTime = DateTime.Now;
                farm = db.Farms.Add(farm);
                await db.SaveChangesAsync();

                QueueHelper.Send(farm, EUpdateMethod.Create);

                return RedirectToAction("Index");
            }

            return View(farm);
        }

        // GET: Farm/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Farm farm = await db.Farms.FindAsync(id);
            if (farm == null)
            {
                return HttpNotFound();
            }
            return View(farm);
        }

        // POST: Farm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FarmId,Name,Latitude,Longitude,Address,City,Zip,Email,Website,PhoneNumber")] Farm farm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(farm).State = EntityState.Modified;
                farm.UpdateDateTime = DateTime.Now;
                await db.SaveChangesAsync();

                QueueHelper.Send(farm, EUpdateMethod.Update);

                return RedirectToAction("Index");
            }
            return View(farm);
        }

        // GET: Farm/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Farm farm = await db.Farms.FindAsync(id);
            if (farm == null)
            {
                return HttpNotFound();
            }
            return View(farm);
        }

        // POST: Farm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Farm farm = await db.Farms.FindAsync(id);
            farm.DeleteWhenRemovedFromIndex = true;
            db.Entry(farm).State = EntityState.Modified;
            await db.SaveChangesAsync();

            QueueHelper.Send(farm, EUpdateMethod.Delete);

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
