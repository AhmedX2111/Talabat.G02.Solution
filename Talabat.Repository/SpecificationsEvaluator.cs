using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Infrastructure
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery; // _dbContext.Set<Product>()

            if (spec.Criteria is not null) // P => P.Id == 1
                query = query.Where(spec.Criteria);

            // query = _dbContext.Set<Product>().Where(P => P.Id == 1)
            // include expressions
            // 1. p => p.Brand
            // 2. p => p.Category

            spec.Includes.Aggregate(query, (currentQuery, includesExpression) => currentQuery.Include(includesExpression));

            // query = _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.Brand)
            // query = _dbContext.Set<Product>().Where(P => P.Id == 1).Include(P => P.Brand).Include(p => p.Category)

            // Immediate
            return query;
        }
    }
}
