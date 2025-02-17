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
        [Route("Category/{id}")]
        public IActionResult Index(int id)
        {
            // Lấy danh mục theo ID
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy danh mục
            }

            // Lấy danh sách bài viết thuộc danh mục này
            var posts = db.Posts
                          .Where(p => p.CategoryId == id) // Lọc theo CategoryId
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

            // Đưa vào ViewModel
            var model = new IndexView
            {
                Posts = posts, // Danh sách bài viết
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
