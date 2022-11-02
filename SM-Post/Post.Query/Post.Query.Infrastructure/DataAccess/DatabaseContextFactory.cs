using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query_Infrastructure.DataAccess
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> configureDbContext;

        public DatabaseContextFactory( Action<DbContextOptionsBuilder> configureDbContext )
        {
            this.configureDbContext = configureDbContext;
        }

        public DatabaseContext CreateDbContext()
        {
            DbContextOptionsBuilder<DatabaseContext> options = new();
            configureDbContext(options);

            return new DatabaseContext(options.Options);
        }
    }
}
