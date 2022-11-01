using CQRS.Core.Domain;
using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate:AggregateRoot
    {
        private bool active;

        private string author;

        private readonly Dictionary<Guid, Tuple<string, string>> comments = new();

        public bool Active { get => active; set => active = value; }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent evnt)
        {
            id = evnt.Id;
            active = true;
            author = evnt.Author;
        }

        public void EditMessage(string message)
        {
            if (!active)
            {
                throw new InvalidOperationException("You cannot edit the message of an inactive post!");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}!");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent evnt)
        {
            id= evnt.Id;
        }

        public void LikePost()
        {
            if (!active)
            {
                throw new InvalidOperationException("You cannot like an inactive post!");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = id
            });
        }

        public void Apply(PostLikedEvent evnt)
        {
            id = evnt.Id;
        }

        public void AddComment(string comment, string username)
        {
            if (!active)
            {
                throw new InvalidOperationException("You cannot add a comment to an inactive post!");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}!");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.Now
            });

        }

        public void Apply(CommentAddedEvent evnt)
        {
            id = evnt.Id;
            comments.Add(evnt.CommentId, new Tuple<string, string>(evnt.Comment, evnt.Username));
        }

        public void EditComment(Guid commentId, string comment, string username)
        {
            if (!active)
            {
                throw new InvalidOperationException("You cannot edit a comment of an inactive post!");
            }

            if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user!");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent evnt)
        {
            id = evnt.Id;
            comments[evnt.CommentId] = new Tuple<string, string>(evnt.Comment, evnt.Username);
        }

        public void RemoveComment(Guid commentId, string username)
        {
            if (!active)
            {
                throw new InvalidOperationException("You cannot remove a comment of an inactive post!");
            }

            if (!comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user!");
            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent evnt)
        {
            id = evnt.Id;
            comments.Remove(evnt.CommentId);
        }

        public void DeletePost(string username)
        {
            if (!active)
            {
                throw new InvalidOperationException("The post has already been removed!");
            }

            if (!author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else");
            }

            RaiseEvent(new PostRemovedEvent
            {
                Id = id,
            });
        }

        public void Apply(PostRemovedEvent evnt)
        {
            id = evnt.Id;
            active = false;
        }

    }
}
