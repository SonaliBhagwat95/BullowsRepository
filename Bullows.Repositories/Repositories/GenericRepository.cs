
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bullows.Repositories.Contracts;
using Bullows.Database;
using Microsoft.EntityFrameworkCore;

namespace Bullows.Repositories.Repositories
{
    public class GenericRepository<T>: IRepository<T> where T : class
    {
        public BullowsDbContext _DbContext;
        private readonly DbSet<T> _DbSet;

        public GenericRepository(BullowsDbContext context)
        {
            this._DbContext = context;
            this._DbSet = this._DbContext.Set<T>();
        }
        public void Add(T entity)
        {
            this._DbSet.Add(entity);
        }
        public IEnumerable<T> GetAll()
        {
            return this._DbSet.ToList();
        }

        public T GetById(int Id)
        {
            return this._DbSet.Find(Id);
        }

        public void Update(T entity)
        {
            this._DbSet.Attach(entity);
            this._DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            this._DbSet.Remove(entity);
        }
    }
}
