using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

[Table("topics")]
public class Topic : BaseModel
{
    [PrimaryKey("id", false)] // Убедитесь, что у вас есть поле `id` в таблице, которое является первичным ключом
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("content")]
    public string Content { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("views_count")]
    public int ViewsCount { get; set; }

    [Column("likes_count")]
    public int LikesCount { get; set; }
    [Column("status")]
    public string Status { get; set; }
    [Column("author")]
    public string Author { get; set; }
    [Column("comments_count")]
    public int CommentsCount { get; set; }
}
