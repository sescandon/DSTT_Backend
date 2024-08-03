using DSTT_Backend.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTT_Test.RepositoriesTests
{
    public abstract class BaseRepositoryTest
    {
        protected readonly DsttDbContext _context;

        protected BaseRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new DsttDbContext(options);
        }

        protected void ClearDatabase()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}
