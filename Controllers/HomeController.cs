using System.Diagnostics;
using Blog.Areas.Admin.Models;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User = Blog.Areas.Admin.Models.User;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogContext db;

        public HomeController(ILogger<HomeController> logger, BlogContext context)
        {
            _logger = logger;
            db = context;
        }

        //Hiển thị các bài viết
        [HttpGet]
        public IActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Category).Include(p => p.User).Include(p => p.Comments)
                .Select(p => new Areas.Admin.Models.Post
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Title = p.Title,
                    Content = p.Content,
                    Image = p.Image,
                    CategoryId = p.CategoryId,
                    CreatedAt = p.CreatedAt,
                    CommentCount = p.Comments.Count(),
                    Category = new Areas.Admin.Models.Category
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        Image = p.Category.Image
                    },
                    User = new Areas.Admin.Models.User
                    {
                        Username = p.User.Username,
                        Image = p.User.Image,
                        Email = p.User.Email
                    }
                })
                .ToList();

            var newCategories = posts
                .Select(p => new CategoryView
                {
                    Id =p.Category.Id,
                    Name = p.Category.Name,
                    Image = p.Category.Image
                })
                .GroupBy(c => new { c.Name, c.Image })  
                .Select(g => g.First())  
                .ToList();
            
            var newMostComment = posts.OrderByDescending(p => p.CommentCount).Take(5).ToList();
            var model = new IndexView
            {
                Posts = posts,
                UniqueCategories = newCategories,
                MostComment = newMostComment
            };
            ViewBag.Categories = db.Categories.ToList();
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(User user)
        {
            ViewBag.Categories = db.Categories.ToList();
            var result = db.Users.FirstOrDefault(p => p.Email == user.Email && p.Password == user.Password && p.Role == "user");

            if (result != null)
            {
                HttpContext.Session.SetInt32("User", result.Id);
                ViewBag.LoginSuccess = true;
                ViewBag.UserId = result.Id;
                var postId = HttpContext.Session.GetInt32("PostId");

                if (postId != null)
                {
                    HttpContext.Session.Remove("PostId");
                    return RedirectToAction("Index", "Detail", new { id = postId.Value });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { result.Id });
                }
    
            }

            ViewBag.Message = "Email hoặc mật khẩu không đúng!";
            return View();
        }

        public IActionResult Register()
        {
            ViewBag.Categories = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            bool isEmail = db.Users.Any(p => p.Email == user.Email);
            bool isName = db.Users.Any(p => p.Username == user.Username);
            if (isEmail || isName)
            {
                ViewBag.Error = isEmail ? "Email đã tồn tại!" : "Username đã tồn tại!";
                return View(); 
            }
            var result = new Data.User
            {
                Email = user.Email,
                Password = user.Password,
                Role = "user",
                Username = user.Username,
            };
            db.Users.Add(result);
            db.SaveChanges();
            ViewBag.Register = true;

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
