using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Property:
        protected readonly DbContext _dbContext;
        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Implementation: 
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            var item = await _dbContext.Set<T>().FindAsync(id); // Hàm Find nhận vào primary key để tìm.
            return item!; 
        }

        public async Task Add(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
             _dbContext.Set<T>().Update(entity);
        }

        public async void Delete(object id)
        {
            var item = await _dbContext.Set<T>().FindAsync(id);
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item to delete not found");
            else
                if (_dbContext.Entry(item).State == EntityState.Detached)
                {
                _dbContext.Attach(item);
                }
            _dbContext.Remove(item);// Chỗ này Remove() constraint 1 Object truyền vào phải khác null.
        }
    }
}
