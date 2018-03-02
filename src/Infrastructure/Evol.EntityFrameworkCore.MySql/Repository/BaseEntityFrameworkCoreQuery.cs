﻿using Evol.Common;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Evol.EntityFrameworkCore.MySql.Repository
{
    public abstract class BaseEntityFrameworkCoreQuery<T, TDbContext> where TDbContext : DbContext where T : class, IPrimaryKey, new()
    {
        private InnerBaseEntityFrameworkRepository<T, TDbContext> innerBaseRepository;

        public BaseEntityFrameworkCoreQuery(IEfCoreDbContextProvider efDbContextProvider)
        {
            innerBaseRepository = new InnerBaseEntityFrameworkRepository<T, TDbContext>(efDbContextProvider);
        }

        public async Task<T> FindAsync(Guid id)
        {
            return await innerBaseRepository.FindAsync(id);
        }

        protected async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await innerBaseRepository.FindAsync(predicate);
        }

        public async Task<List<T>> SelectAsync(Guid[] ids)
        {
            return await innerBaseRepository.SelectAsync(ids);
        }

        protected IQueryable<T> Query()
        {
            return innerBaseRepository.Query();
        }

        protected async Task<List<T>> SelectAsync(Expression<Func<T, bool>> predicate)
        {
            return await innerBaseRepository.SelectAsync(predicate);
        }

        protected async Task<List<T>> SelectAsync(Func<IQueryable<T>, IQueryable<T>> condition)
        {
            var items = await innerBaseRepository.SelectAsync(condition);
            return items;
        }

        protected async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await innerBaseRepository.AnyAsync(predicate);
        }


        public async Task<IPaged<T>> PagedAsync(int pageIndex, int pageSize)
        {
            return await innerBaseRepository.PagedAsync(pageIndex, pageSize);
        }

        protected async Task<IPaged<T>> PagedAsync(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize)
        {
            return await innerBaseRepository.PagedAsync(predicate, pageIndex, pageSize);
        }

        protected async Task<IPaged<T>> PagedAsync(Func<IQueryable<T>, IQueryable<T>> condition, int pageIndex, int pageSize)
        {
            var paged = await innerBaseRepository.PagedAsync(condition, pageIndex, pageSize);
            return paged;
        }
    }


    public class InnerBaseEntityFrameworkRepository<T, TDbContext> : BaseEntityFrameworkCoreRepository<T, TDbContext> where TDbContext : DbContext where T : class, IPrimaryKey, new()
    {
        public InnerBaseEntityFrameworkRepository(IEfCoreDbContextProvider dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
