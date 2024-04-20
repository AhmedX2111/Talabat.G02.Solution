using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; set; } // p => p.Id == 1

        public List<Expression<Func<T, object>>> Includes { get; set; }

        public Expression<Func<T, Object>> OrderBy { get; set; }

        public Expression<Func<T, Object>> OrderByDesc { get; set; }
    }
}
