using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class DetailController : Controller
    {
        private BlogContext db;

        public DetailController(BlogContext context) 
        {
            db = context;
        }
        [Route("Detail/{id}")]
        public IActionResult Index(int id)
        {
            HttpContext.Session.SetInt32("PostId",id);
            var posts = db.Posts
                          .Where(p => p.Id == id)
                          .Include(p => p.Comments)
                           .ThenInclude(c => c.User)
                          .Select(p => new Areas.Admin.Models.Post
                          {
                              Id = p.Id,
                              Title = p.Title,
                              Content = p.Content,
                              Image = p.Image,
                              CategoryId = p.CategoryId,
                              CreatedAt = p.CreatedAt,
                              Category = new Areas.Admin.Models.Category
                              { 
                                 Id = p.Category.Id,
                                 Name = p.Category.Name,
                              },
                             
                              Comments = p.Comments.Select(c => new Areas.Admin.Models.Comment
                              {
                                   Content = c.Content,
                                   User = new Areas.Admin.Models.User
                                   {
                                       Username = c.User.Username,
                                   },
                              }).ToList()

                          })
                          .ToList();
            var relatedPosts = db.Posts
                                 .Where(p => p.Id != id) 
                                 .OrderBy(x => Guid.NewGuid()) 
                                 .Take(3) 
                                 .Select(p => new Areas.Admin.Models.Post
                                 {
                                     Id = p.Id,
                                     Title = p.Title,
                                     Content = p.Content,
                                     Image = p.Image,
                                     CreatedAt = p.CreatedAt,
                                     Category = new Areas.Admin.Models.Category
                                     {
                                         Name = p.Category.Name
                                     }
                                 })
                                 .ToList();
            var coutComment = db.Comments.Where(p => p.PostId == id).Count();

            ViewBag.CoutComment = coutComment;
            ViewBag.RelatedPosts = relatedPosts;

            ViewBag.Categories = db.Categories.ToList(); 
            return View(posts);
        }
        [Route("Detail/AddComment")]
        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            var session = HttpContext.Session.GetInt32("User");


            int userId = session.Value;

            var newComment = new Comment
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = userId,
                Content = comment.Content,
                CreatedAt = DateTime.Now,
            };

            db.Comments.Add(newComment);
            db.SaveChanges();

            return RedirectToAction("Index", "Detail",new { id = comment.PostId });

        }


    }
}
