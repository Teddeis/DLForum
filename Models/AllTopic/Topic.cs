using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("topics")]
public class Topic : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("content")]
    public string Content { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("likes_count")]
    public int LikesCount { get; set; }
    [Column("status")]
    public string Status { get; set; }
    [Column("author")]
    public string Author { get; set; }
    [Column("comments_count")]
    public int CommentsCount { get; set; }
    [Column("topic_image")]
    public string ImageUrl { get; set; } = "https://feehcmjymwvyhganwaqw.supabase.co/storage/v1/object/public/default//DLFStartTopicItem.png";
    [Column("categories")]
    public string Categories { get; set; }
    [Column("tags")]
    public string Tags { get; set; }
}
