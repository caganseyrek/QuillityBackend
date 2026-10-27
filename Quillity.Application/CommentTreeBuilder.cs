using Quillity.Application.DTOs;
using Quillity.Common.Models;

namespace Quillity.Application;

public static class CommentTreeBuilder
{
    public static List<HealthUpdateCommentModel> ProcessComments(List<HealthUpdateCommentModel> comments)
    {
        var commentDictionary = comments.ToDictionary(comment => comment.Id);
        var rootComments = new List<HealthUpdateCommentModel>();

        foreach (var comment in comments)
        {
            if (comment.ParentCommentId.HasValue &&
                commentDictionary.TryGetValue(comment.ParentCommentId.Value, out var parentComment))
            {
                parentComment.Replies ??= [];
                parentComment.Replies.Add(comment);
            }
            else
            {
                rootComments.Add(comment);
            }
        }
        return rootComments;
    }

    public static List<TaskItemCommentModel> ProcessComments(List<TaskItemCommentModel> comments)
    {
        var commentDictionary = comments.ToDictionary(comment => comment.Id);
        var rootComments = new List<TaskItemCommentModel>();

        foreach (var comment in comments)
        {
            if (comment.ParentCommentId.HasValue &&
                commentDictionary.TryGetValue(comment.ParentCommentId.Value, out var parentComment))
            {
                parentComment.Replies ??= [];
                parentComment.Replies.Add(comment);
            }
            else
            {
                rootComments.Add(comment);
            }
        }
        return rootComments;
    }
}
