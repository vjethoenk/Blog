using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
   
    public class CategoryController : Controller
    {
        private BlogContext db;

        public CategoryController(BlogContext context) {
            db = context;
        }
        
        //Hiển thị danh mục bài viết theo id

        [Route("Category/{id}")]
        [HttpGet]
        public IActionResult Index(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound(); 
            }

            var posts = db.Posts
                          .Where(p => p.CategoryId == id) 
                          .Select(p => new Areas.Admin.Models.Post
                          {
                              Id = p.Id,
                              Title = p.Title,
                              Content = p.Content,
                              Image = p.Image,
                              CategoryId = p.CategoryId,
                              CreatedAt = p.CreatedAt
                          })
                          .ToList();

            var model = new IndexView
            {
                Posts = posts,
                UniqueCategories = db.Categories.Select(c => new CategoryView
                {
                    Id = c.Id,
                    Name = c.Name,
                    Image = c.Image
                }).ToList()
            };

            ViewBag.Categories = db.Categories.ToList(); 
            return View(model);
        }



    }
}
