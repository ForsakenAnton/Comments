namespace Comments.Server.Models.RequestFeatures;

public class CommentParameters : RequestParameters
{
    public CommentParameters()
    {
        
        OrderBy = "date desc";
    }
}
