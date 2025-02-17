using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Blog.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class HomeAdminController : Controller
    {
        private BlogContext db;

        public HomeAdminController(BlogContext context) 
        {
            db = context;
        }
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetInt32("Admin");
            if(session == null)
            {
                return RedirectToAction("Loginn");
            }
            var result = db.Posts.Include(p => p.Category).Select(p => new Areas.Admin.Models.Post
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Content = p.Content,
                Image = p.Image,    
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                Category = new Areas.Admin.Models.Category
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                }
            }).ToList();
            return View(result);
        }
  

        public IActionResult Loginn()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Add()
        {
            var categories = db.Categories
           .Select(c => new Blog.Areas.Admin.Models.Category
           {
               Id = c.Id,
               Name = c.Name,

           })
           .ToList();

            return View(categories);
        }

        [HttpPost]
        public IActionResult Add(Post list, IFormFile Image)
        {
            var userId = HttpContext.Session.GetInt32("Admin");
            if(userId == null) { return RedirectToAction("Loginn"); }

            string fileName = null;
            //Xử lí ảnh 
            if (Image != null)
            {
                //string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/img", fileName);
                 
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                     Image.CopyToAsync(stream);
                     stream.Flush();
                }
                
            }
            var newPost = new Post
            {
                Id = list.Id,
                UserId = userId.Value,
                Title = list.Title,
                Content = list.Content,
                Image = fileName,
                CategoryId = list.CategoryId,
                CreatedAt = list.CreatedAt,
            };
            db.Posts.Add(newPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            
            var session = HttpContext.Session.GetInt32("Admin");
            if(session == null) { return RedirectToAction("Loginn"); }
            var post = db.Posts.Include(p => p.Category).Where(p => p.Id == id).Select(p => new Post
            {
                Id = p.Id,
                UserId= p.UserId,
                Title = p.Title,
                Content = p.Content,
                Image = p.Image,
                CategoryId= p.CategoryId,
                CreatedAt = p.CreatedAt,
                Category = p.Category
            }).FirstOrDefault();
            ViewBag.Categories = db.Categories.ToList();
            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(Post list, IFormFile Image)
        {
            string fileName = null;

            if (Image != null)
            {
                fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/img", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
            }

            var newPost = db.Posts.FirstOrDefault(p => p.Id == list.Id);
            if (newPost == null) return NotFound();

            newPost.Title = list.Title;
            newPost.Content = list.Content;
            newPost.CreatedAt = list.CreatedAt;
            newPost.CategoryId = list.CategoryId;

            if (fileName != null) 
            {
                newPost.Image = fileName;
            }

            db.SaveChanges(); 

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var post = db.Posts.Find(id);
            if (post == null) return NotFound();
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult signOut()
        {
            HttpContext.Session.Remove("Admin");          
            return RedirectToAction("Loginn");
        }

        [HttpPost]
        public ActionResult Loginn(User us)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == us.Email && u.Password == us.Password && u.Role =="admin");
            if(user != null)
            {
                HttpContext.Session.SetInt32("Admin",user.Id);
                return RedirectToAction("Index", new { userId = user.Id });
            }
            return View();
        }
    }
}
