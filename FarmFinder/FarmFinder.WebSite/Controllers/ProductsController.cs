using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CH.Tutteli.FarmFinder.Dtos;
using CH.Tutteli.FarmFinder.Website.Models;
using Microsoft.ServiceBus.Messaging;

namespace CH.Tutteli.FarmFinder.Website.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Farm/1/Products/
        public async Task<ActionResult> Index(int? farmId)
        {
            if (farmId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Farm farm = await db.Farms.Include(f => f.Products).Where(f => f.FarmId == farmId.Value).FirstAsync();
            if (farm == null)
            {
                return HttpNotFound();
            }
            ViewBag.Farm = farm;
            return View(farm.Products);
        }

        // GET: Farm/1/Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Farm/1/Products/Create
        public async Task<ActionResult> Create(int farmId)
        {
            Farm farm = await db.Farms.FindAsync(farmId);
            if (farm == null)
            {
                return HttpNotFound();
            }
            return View(new Product {Farm = farm, FarmRefId = farm.FarmId});
        }

        // POST: Farm/1/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "ProductId,FarmRefId,Name,Description,InStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await SaveChangesAndReIndex(product);

                return RedirectToAction("Index", new {farmId = product.FarmRefId});
            }
            Farm farm = await db.Farms.FindAsync(product.FarmRefId);
            if (farm == null)
            {
                return HttpNotFound();
            }
            product.Farm = farm;
            return View(product);
        }

        private async Task SaveChangesAndReIndex(Product product)
        {
            //create/delete/update a product also means we need to reindex the farm entry, hence modify the UpdateDateTime 
            db.Entry(product.Farm).State = EntityState.Modified;
            product.Farm.UpdateDateTime = DateTime.Now;
            await db.SaveChangesAsync();

            //db was saved successfully, inform worker role via IndexUpdatingQueue
            InformWorkerRoleToReIndex(new UpdateIndexDto {FarmId = product.FarmRefId, UpdateMethod = EUpdateMethod.Update});
        }

        private void InformWorkerRoleToReIndex(UpdateIndexDto updateIndexDto)
        {
            WebApiApplication.QueueClient.Send(new BrokeredMessage(updateIndexDto));
        }

        // GET: Farm/1/Products/Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "ProductId,FarmRefId,Name,Description,InStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await SaveChangesAndReIndex(product);

                return RedirectToAction("Index", new {farmId = product.FarmRefId});
            }
            Farm farm = await db.Farms.FindAsync(product.FarmRefId);
            if (farm == null)
            {
                return HttpNotFound();
            }
            product.Farm = farm;
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);

            await SaveChangesAndReIndex(product);
            return RedirectToAction("Index", new {farmId = product.FarmRefId});
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