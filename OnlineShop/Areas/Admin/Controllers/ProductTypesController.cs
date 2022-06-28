using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Data;
using OnlineShop.Models;
using Dapper;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Super User")]
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _db; 
        private readonly IDbConnection _dapper;

        public ProductTypesController(ApplicationDbContext db, IDbConnection dapper)
        {
            _db = db;
            _dapper = dapper;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var data = _db.ProductTypes.ToList();
            //var data = _dapper.GetListAsync<ProductTypes>();
            return View(data);
        }

        //GET Create Action Method
        
        public ActionResult Create()
        {
            return View();
        }

        //POST Create Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if(ModelState.IsValid)
            { 
                await _dapper.InsertAsync<ProductTypes>(productTypes);
                TempData["save"] = "Product type has been saved";
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }

        //GET Edit Action Method
       
        public ActionResult Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var productType = _db.ProductTypes.Find(id);
            if(productType==null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //POST Edit Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                await _dapper.UpdateAsync<ProductTypes>(productTypes);
                TempData["edit"] = "Product type has been updated";
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }


        //GET Details Action Method

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = _db.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //POST Edit Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Details(ProductTypes productTypes)
        {
            return RedirectToAction(nameof(Index));
            
        }

        //GET Delete Action Method

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = _db.ProductTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            return View(productType);
        }

        //POST Delete Action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, ProductTypes productTypes)
        {
            if(id==null)
            {
                return NotFound();
            }

            if(id!=productTypes.Id)
            {
                return NotFound();
            }

            var productType = _db.ProductTypes.Find(id);
            if(productType==null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Remove(productType);
                await _db.SaveChangesAsync();
                TempData["delete"] = "Product type has been deleted";
                return RedirectToAction(nameof(Index));
            }

            return View(productTypes);
        }

    }
}