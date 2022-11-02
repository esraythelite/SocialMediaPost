using Microsoft.EntityFrameworkCore;
using Post.Cmd.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query_Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory databaseContextFactory;

        public PostRepository( DatabaseContextFactory databaseContextFactory )
        {
            this.databaseContextFactory = databaseContextFactory;
        }

        public async Task CreateAsync( PostEntity post )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            context.Posts.Add(post);
            _ = await context.SaveChangesAsync();            
        }

        public async Task DeleteAsync( Guid postId )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            var post = await GetByIdAsync( postId );

            if (post == null) return;
           
            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync( Guid postId )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(x => x.PosId == postId);
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAuthorAsync( string author )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(x => x.Author.Contains(author)).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync()
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(x => x.Comments != null && x.Comments.Any()).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikesAsync( int likes )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(p => p.Comments).Where(x => x.Likes >= likes).ToListAsync();
        }

        public async Task UpdateAsync( PostEntity post )
        {
            using DatabaseContext context = databaseContextFactory.CreateDbContext();
            context.Posts.Update(post);

            await context.SaveChangesAsync();
        }
    }
}
