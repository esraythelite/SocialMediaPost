using Post.Cmd.Domain.Entities;
using Post.Common.Events;
using Post.Query.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;

        public EventHandler( IPostRepository postRepository, ICommentRepository commentRepository )
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }

        public async Task On( PostCreatedEvent evnt )
        {
            var post = new PostEntity
            {
                PosId = evnt.Id,
                Author = evnt.Author,
                DatePosted = evnt.DatePosted,
                Message = evnt.Message
            };

            await postRepository.CreateAsync( post );
        }

        public async Task On( MessageUpdatedEvent evnt )
        {
            var post = await postRepository.GetByIdAsync(evnt.Id );

            if (post == null) return;

            post.Message = evnt.Message;
            await postRepository.UpdateAsync(post);
        }

        public async Task On( PostLikedEvent evnt )
        {
            var post = await postRepository.GetByIdAsync(evnt.Id);

            if (post == null) return;

            post.Likes++;
            await postRepository.UpdateAsync(post);
        }

        public async Task On( CommentAddedEvent evnt )
        {
            var comment = new CommentEntity
            {
                PostId = evnt.Id,
                CommentId = evnt.CommentId,
                CommentDate = evnt.CommentDate,
                Comment = evnt.Comment,
                Username = evnt.Username,
                Edited = false
            };

            await commentRepository.CreateAsync(comment);
        }

        public async Task On( CommentUpdatedEvent evnt )
        {
            var comment = await commentRepository.GetByIdAsync(evnt.CommentId);

            if (comment == null) return;

            comment.Comment = evnt.Comment;
            comment.Edited = true;
            comment.CommentDate = evnt.EditDate;

            await commentRepository.UpdateAsync(comment);
        }

        public async Task On( CommentRemovedEvent evnt )
        {
            await commentRepository.DeleteAsync(evnt.CommentId);
        }

        public async Task On( PostRemovedEvent evnt )
        {
            await postRepository.DeleteAsync(evnt.Id);
        }
    }
}
