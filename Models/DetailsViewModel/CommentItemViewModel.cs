public class CommentItemViewModel
{
    public comment Comment { get; set; }
    public IEnumerable<comment> AllComments { get; set; }
    public int TopicId { get; set; }
    public bool IsBanned { get; set; }
}