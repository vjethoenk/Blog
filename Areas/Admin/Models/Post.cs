namespace Blog.Areas.Admin.Models
{
    public class Post
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } 

        public string Content { get; set; } 

        public string? Image { get; set; }

        public int CategoryId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int CommentCount { get; set; }
        public virtual Category Category { get; set; }

        public virtual List<Comment> Comments { get; set; } = new List<Comment>();

        public virtual User User { get; set; }

    }
}
