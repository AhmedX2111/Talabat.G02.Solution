﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Infrastructure.Data;

namespace Talabat.Infrastructure
{
    public class GenericInfrastructure<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericInfrastructure(StoreContext dbContext) // Ask CLR for Creating Object from DbContext Implicitly
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            ///if (typeof(T) == typeof(Product))
            ///    return (IEnumerable<T>)await _dbContext.Set<Product>().Include(P => P.Brand)
            ///        .Include(P => P.Category).ToListAsync();

            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }


        public async Task<T?> GetAsync(int id)
        {
            ///if (typeof(T) == typeof(Product))
            ///    return await _dbContext.Set<Product>().Where(P => P.Id == id).Include(P => P.Brand)
            ///        .Include(P => P.Category).FirstOrDefaultAsync() as T;

            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }
        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
