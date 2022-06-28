using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineShop.Data;
using OnlineShop.Models;
using Dapper;
using System.Data;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super User")]
    public class ProductController : Controller
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _he;
        private readonly IDbConnection _configuration;

        public ProductController(ApplicationDbContext db,IHostingEnvironment he,IDbConnection configuration)
        {
            _db = db;
            _he = he;
              _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View(_db.Products.Include(c=>c.ProductTypes).Include(f=>f.SpecialTag).ToList());
        }

        //POST Index action method
       [HttpPost]
        public IActionResult Index(decimal? lowAmount, decimal? largeAmount)
        {
            var products = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .Where(c => c.Price >= lowAmount && c.Price <= largeAmount).ToList();
            if(lowAmount==null ||largeAmount==null)
            {
               products = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList();
            }
            return View(products);
        }

        //Get Create method
        public async Task<IActionResult> Create()
        {
            ViewData["productTypeId"] = await _configuration.GetListAsync<ProductTypes>();/*new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");*/
            ViewData["TagId"] = await _configuration.GetListAsync<SpecialTag>();
            return View();
        }
        

        //Post Create method
        [HttpPost]
        public async Task<IActionResult> Create(Products product)
        {
            if(ModelState.IsValid)
            {
                var searchProduct =await  _configuration.QueryAsync<Products>("select * from Products where Name=@Name", new {Name=product.Name});
                //var searchProduct = _db.Products.FirstOrDefault(c => c.Name == product.Name);
                if(searchProduct.Any())
                {
                    ViewBag.message = "This product is already exist";

                    ViewData["productTypeId"] = await _configuration.GetListAsync<ProductTypes>();
                    /*new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");*/
                    ViewData["TagId"] = await _configuration.GetListAsync<SpecialTag>();
                    return View(product);
                }
               
                if(product.image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(product.image.FileName));
                    await product.image.CopyToAsync(new FileStream(name, FileMode.Create));
                    product.Image = "Images/" + product.image.FileName;
                }

                if(product.image == null)
                {
                    product.Image = "Images/noimage.PNG";
                }
                await _configuration.InsertAsync<Products>(product); 

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        //GET Edit Action Method

        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["productTypeId"] = await _configuration.GetListAsync<ProductTypes>(); 
            ViewData["TagId"] = await _configuration.GetListAsync<SpecialTag>();
            if (id==null)
            {
                return NotFound();
            }

            var product = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);
            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST Edit Action Method
        [HttpPost]
        public async Task<IActionResult> Edit(Products products)
        {
            if (ModelState.IsValid)
             {
                if (products.image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(products.image.FileName));
                    await products.image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "Images/" + products.image.FileName;
                }

                if (products.image == null)
                {
                    products.Image = "Images/noimage.PNG";
                }
                _db.Products.Update(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(products);
        }

        //GET Details Action Method
        public ActionResult Details(int? id)
        {

            if(id==null)
            {
                return NotFound();
            }

            var product = _db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag)
                .FirstOrDefault(c => c.Id == id);
            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }

        //GET Delete Action Method

        public ActionResult Delete(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var product = _db.Products.Include(c=>c.SpecialTag).Include(c=>c.ProductTypes).Where(c => c.Id == id).FirstOrDefault();
            if(product==null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST Delete Action Method

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var product = _db.Products.FirstOrDefault(c => c.Id == id);
            if(product==null)
            {
                return NotFound();
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
     
    }
}