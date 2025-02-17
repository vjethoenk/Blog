namespace Blog.Models
{
    public class CategoryView
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Image { get; set; }

    }
    public class IndexView
    {
        public List<Areas.Admin.Models.Post> Posts { get; set; }
        public List<CategoryView> UniqueCategories { get; set; }
        public List<Areas.Admin.Models.Post> MostComment {  get; set; }
    }
}
