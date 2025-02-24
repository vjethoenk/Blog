using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Blog.Areas.Admin.Models;
using Microsoft.Extensions.Hosting;

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        private BlogContext db;

        public PostController(BlogContext context) 
        {
            db = context;
        }

        [Route("Post")]
        [HttpGet]
        public IActionResult Index()
        {
           var post=db.Posts.Include(p => p.Category).Select(p => new Areas.Admin.Models.Post
           {
               Id = p.Id,
               UserId = p.UserId,
               Title = p.Title,
               Content =p.Content,
               CreatedAt= p.CreatedAt,
               CategoryId = p.CategoryId,
               Image =p.Image,
               Category = new Areas.Admin.Models.Category
               {
                   Name = p.Category.Name,
                   Image =p.Category.Image,
               }
           }).ToList();
            var newCategory = post.Select(p=>new CategoryView
            {
                Name = p.Category.Name,
                Image = p.Category.Image
            }).GroupBy(g => new {g.Name,g.Image }).Select(s => s.First()).ToList();
            var model = new IndexView
            {
                Posts = post,
                UniqueCategories = newCategory,
            };
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }

        [Route("Posts")]
        [HttpGet]
        public IActionResult Search(string timkiem)
        {
            var result = db.Posts.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(timkiem))
            {
                result = result.Where(p => p.Title.Contains(timkiem) || p.Content.Contains(timkiem));
            }

            var posts = result.Select(p => new Blog.Areas.Admin.Models.Post
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                Image = p.Image,
                Category = new Blog.Areas.Admin.Models.Category
                {
                    Name = p.Category.Name,
                    Image = p.Category.Image
                }
            }).ToList();

            var model = new IndexView
            {
                Posts = posts,
                UniqueCategories = db.Categories
                                    .Select(c => new CategoryView { Name = c.Name, Image = c.Image })
                                    .ToList()
            };
            ViewBag.Categories = db.Categories.ToList();
            return View("Index", model);
        }


    }
}
