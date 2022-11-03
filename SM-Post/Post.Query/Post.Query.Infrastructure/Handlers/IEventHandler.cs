using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.Handlers
{
    public interface IEventHandler
    {
        Task On( PostCreatedEvent evnt );
        Task On( MessageUpdatedEvent evnt );
        Task On( PostLikedEvent evnt );
        Task On( CommentAddedEvent evnt );
        Task On( CommentUpdatedEvent evnt );
        Task On( CommentRemovedEvent evnt );
        Task On( PostRemovedEvent evnt );
    }
}
