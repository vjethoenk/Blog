namespace Blog.Areas.Admin.Models
{
    public class Comment
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public virtual User User { get; set; }
    }
}
