﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Hao.Utility;
using System.Linq;
using AspectCore.DependencyInjection;

namespace Hao.Core
{
    public abstract class Repository<T, TKey> : IRepository<T, TKey> where T : Entity<TKey>, new() where TKey : struct
    {
        [FromServiceContext] public IFreeSqlContext DbContext { get; set; }

        
        /// <summary>
        /// 根据主键查询单条数据
        /// </summary>s
        /// <param name="pkValue"></param>
        /// <returns></returns>
        public virtual async Task<T> GetAysnc(TKey pkValue)
        {
            var entity = await DbContext.Select<T>().Where(a => a.Id.Equals(pkValue)).ToOneAsync();
            return entity;
        }

        /// <summary>
        /// 根据主键集合查询多条数据
        /// </summary>s
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAysnc(List<TKey> pkValues)
        {
            H_Check.Argument.NotEmpty(pkValues, nameof(pkValues));

            return await DbContext.Select<T>().Where(x => pkValues.Contains(x.Id)).ToListAsync();
        }

        /// <summary>
        /// 查询所有数据（未删除）
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAysnc()
        {
            return await DbContext.Select<T>().ToListAsync();
        }

        /// <summary>
        /// 根据条件查询所有数据（未删除）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAysnc(Query<T> query)
        {
            H_Check.Argument.NotNull(query, nameof(query));

            var select = DbContext.Select<T>();
            
            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    if (item == null) continue;
                    select.Where(item);
                }
            }

            if (query.OrderByConditions?.Count > 0)
            {
                foreach (var item in query.OrderByConditions)
                {
                    if (item == null) continue;
                    select.OrderByPropertyName(item.FieldName, item.IsAsc);
                }
            }
            else
            {
                select.OrderByDescending(a => a.CreateTime);
            }


            return await select.ToListAsync();
        }


        /// <summary>
        /// 根据条件查询所有数据数量（未删除）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<int> GetCountAysnc(Query<T> query)
        {
            H_Check.Argument.NotNull(query, nameof(query));

            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    if (item == null) continue;
                    select.Where(item);
                }
            }

            return (int)await select.CountAsync();
        }

        /// <summary>
        /// 根据条件查询分页数据（未删除）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<Paged<T>> GetPagedAysnc(Query<T> query)
        {
            H_Check.Argument.NotNull(query, nameof(query));

            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    if (item == null) continue;
                    select.Where(item);
                }
            }

            if (query.OrderByConditions?.Count > 0)
            {
                foreach(var item in query.OrderByConditions)
                {
                    if (item == null) continue;
                    select.OrderByPropertyName(item.FieldName, item.IsAsc);
                }
            }
            else
            {
                select.OrderByDescending(a => a.CreateTime);
            }
            
            var items = await select.Count(out var total)
                                    .Page(query.PageIndex, query.PageSize)
                                    .ToListAsync();

            return ToPaged(items, query.PageIndex, query.PageSize, total);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAysnc()
        {
            return await DbContext.Select<T>()
                                   .DisableGlobalFilter(nameof(IsSoftDelete))
                                   .OrderByDescending(a => a.CreateTime)
                                   .ToListAsync();
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAysnc(Query<T> query)
        {
            H_Check.Argument.NotNull(query, nameof(query));

            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    if (item == null) continue;
                    select.Where(item);
                }
            }

            if (query.OrderByConditions?.Count > 0)
            {
                foreach (var item in query.OrderByConditions)
                {
                    if (item == null) continue;
                    select.OrderByPropertyName(item.FieldName, item.IsAsc);
                }
            }
            else
            {
                select.OrderByDescending(a => a.CreateTime);
            }
            
            return await select.DisableGlobalFilter(nameof(IsSoftDelete))
                                .ToListAsync();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> InsertAysnc(T entity)
        {
            H_Check.Argument.NotNull(entity, nameof(entity));

            var obj = await DbContext.Insert(entity).ExecuteInsertedAsync();
            
            return obj?.FirstOrDefault();
        }

        /// <summary>
        /// 插入数据（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> InsertAysnc(List<T> entities)
        {
            H_Check.Argument.NotEmpty(entities, nameof(entities));

            return await DbContext.Insert(entities).ExecuteInsertedAsync();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotNull(entity, nameof(entity));

            var update = DbContext.Update<T>()
                                    .SetSource(entity);

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                update.Where(item);
            }

            return await update.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 更新数据（指定列）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotNull(entity, nameof(entity));

            H_Check.Argument.NotNull(updateColumns, nameof(updateColumns));

            var body = updateColumns.Body as NewExpression;

            H_Check.Argument.NotNull(body, nameof(updateColumns));
            H_Check.Argument.NotEmpty(body.Members, nameof(updateColumns));

            var columns = body.Members.Select(a => a.Name).ToList();

            var update = DbContext.Update<T>()
                                    .SetSource(entity)
                                    .UpdateColumns(columns.ToArray());

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                update.Where(item);
            }

            return await update.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 更新数据（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entities, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotEmpty(entities, nameof(entities));

            var update = DbContext.Update<T>()
                                    .SetSource(entities);

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                update.Where(item);
            }

            return await update.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 更新数据（批量）（指定列）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateColumns"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(List<T> entities, Expression<Func<T, object>> updateColumns, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotEmpty(entities, nameof(entities));

            H_Check.Argument.NotNull(updateColumns, nameof(updateColumns));

            var body = updateColumns.Body as NewExpression;

            H_Check.Argument.NotNull(body, nameof(updateColumns));
            H_Check.Argument.NotEmpty(body.Members, nameof(updateColumns));

            var columns = body.Members.Select(a => a.Name).ToList();

            var update = DbContext.Update<T>()
                                    .SetSource(entities)
                                    .UpdateColumns(columns.ToArray());

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                update.Where(item);
            }

            return await update.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除数据（逻辑删除）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAysnc(T entity, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotNull(entity, nameof(entity));

            return await DeleteAysnc(entity.Id, whereColumns);
        }

        /// <summary>
        /// 删除数据（逻辑删除）（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAysnc(List<T> entities, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotEmpty(entities, nameof(entities));

            return await DeleteAysnc(entities.Select(a => a.Id).ToList(), whereColumns);
        }

        /// <summary>
        /// 删除数据（逻辑删除）
        /// </summary>
        /// <param name="pkValue"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        private async Task<int> DeleteAysnc(TKey pkValue, params Expression<Func<T, bool>>[] whereColumns)
        {
            var delete = DbContext.Update<T>()
                                    .Set(a => a.IsDeleted, true)
                                    .Where(a => a.Id.Equals(pkValue));

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                delete.Where(item);
            }

            return await delete.ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除数据（逻辑删除）（批量）
        /// </summary>
        /// <param name="pkValues"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        private async Task<int> DeleteAysnc(List<TKey> pkValues, params Expression<Func<T, bool>>[] whereColumns)
        {
            H_Check.Argument.NotEmpty(pkValues, nameof(pkValues));

            var delete = DbContext.Update<T>()
                                    .Set(a => a.IsDeleted, true)
                                    .Where(a => pkValues.Contains(a.Id));

            foreach (var item in whereColumns)
            {
                if (item == null) continue;
                delete.Where(item);
            }

            return await delete.ExecuteAffrowsAsync();
        }
        
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        protected Paged<T2> ToPaged<T2>(List<T2> items, int pageIndex, int pageSize, long total) where T2 : class, new()
        {
            var pageList = new Paged<T2>()
            {
                Items = items,
                TotalCount = (int)total,
                PageIndex = pageIndex,
                PageSize =  pageSize,
                TotalPageCount = ((int)total + pageSize - 1) / pageSize
            };
            return pageList;
        }
    }
}