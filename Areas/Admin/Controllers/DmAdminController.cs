using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DmAdminController : Controller
    {
        private BlogContext db;

        public DmAdminController(BlogContext context) 
        {
            db = context;
        }
        [HttpGet]
        public IActionResult DanhMuc()
        {
            var userId = HttpContext.Session.GetInt32("Admin");
            if (userId == null) { return RedirectToAction("Loginn"); }
            var categories = db.Categories
            .Select(c => new Blog.Areas.Admin.Models.Category
            {
                Id = c.Id,
                Name = c.Name,
                Image = c.Image
            })
            .ToList();

            return View(categories);
        }
        public IActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddNew(Category list, IFormFile Image)
        {
            var userId = HttpContext.Session.GetInt32("Admin");
            if (userId == null) { return RedirectToAction("Loginn"); }

            string fileName = null;
            //Xử lí ảnh 
            if (Image != null)
            {
                fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyToAsync(stream);
                    stream.Flush();
                }

            }
            var newCategory = new Category 
            { 
                Name = list.Name,
                Image = fileName,
            };

            db.Categories.Add(newCategory);
            db.SaveChanges();
            return RedirectToAction("DanhMuc");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if(category == null) { return NotFound(); }
            var result = new Category
            {
                Name = category.Name,
                Image = category.Image,
            };
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Category list, IFormFile Image) 
        {
            var userId = HttpContext.Session.GetInt32("Admin");
            if (userId == null) { return RedirectToAction("Loginn"); }

            string fileName = null;
            //Xử lí ảnh 
            if (Image != null)
            {
                //string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyToAsync(stream);
                    stream.Flush();
                }

            }
            var newCategory = db.Categories.Find(list.Id);
            if (newCategory == null) {return NotFound();}    
            newCategory.Name = list.Name;
            newCategory.Image = fileName;
            db.Categories.Update(newCategory);
            db.SaveChanges();
            return RedirectToAction("DanhMuc");
        }



        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var task = db.Categories.Find(id);
            if (id == null) { return NotFound(); }
            if (task == null) { return NotFound(); }
            db.Categories.Remove(task);
            db.SaveChanges();
            TempData["IsLoggedIn"] = true;
            return Ok();
        }

    }
}
