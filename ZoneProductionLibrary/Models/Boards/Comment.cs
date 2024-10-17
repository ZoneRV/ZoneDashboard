namespace ZoneProductionLibrary.Models.Boards
{
    public class Comment
    {
        public TrelloMember CreatorMember { get; }
        public DateTime DateCreated { get; }
        public string Content { get; }

        public Comment(CommentObject commentObject, TrelloMember member)
        {
            CreatorMember = member;
            DateCreated = commentObject.DateCreated.LocalDateTime;
            Content = commentObject.Content;
        }

        public Comment(TrelloMember member, DateTime dateCreated, string content)
        {
            CreatorMember = member;
            DateCreated = dateCreated;
            Content = content;
        }
    }
}
