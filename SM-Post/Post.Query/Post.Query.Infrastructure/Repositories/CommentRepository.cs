using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContextFactory databaseContextFactory;

        public CommentRepository( DatabaseContextFactory databaseContextFactory )
        {
            this.databaseContextFactory = databaseContextFactory;
        }

        public async Task CreateAsync( CommentEntity comment )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            context.Comments.Add( comment );

            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync( Guid commentId )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            var comment = await GetByIdAsync( commentId );

            if (comment == null) return;

            context.Comments.Remove(comment);
            _ = await context.SaveChangesAsync();
        }

        public async Task<CommentEntity> GetByIdAsync( Guid commentId )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);
        }

        public async Task UpdateAsync( CommentEntity comment )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            context.Comments.Update(comment);
            
            _ = await context.SaveChangesAsync();
        }
    }
}
