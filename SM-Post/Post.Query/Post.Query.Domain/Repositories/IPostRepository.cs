using Post.Query.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostEntity = Post.Query.Domain.Entities.PostEntity;

namespace Post.Query.Domain.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync( PostEntity post );
        Task UpdateAsync( PostEntity post );
        Task DeleteAsync( Guid postId );
        Task<PostEntity> GetByIdAsync( Guid postId );
        Task<List<PostEntity>> ListAllAsync();
        Task<List<PostEntity>> ListByAuthorAsync( string author );
        Task<List<PostEntity>> ListWithLikesAsync( int likes );
        Task<List<PostEntity>> ListWithCommentsAsync() ;
    }
}
