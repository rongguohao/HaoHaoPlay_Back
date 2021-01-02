﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Hao.Snowflake;
using Hao.Utility;
using System.Linq;
using AspectCore.DependencyInjection;

namespace Hao.Core
{
    public abstract class Repository<T, TKey> : IRepository<T, TKey>
        where T : Entity<TKey>, new() where TKey : struct
    {
        [FromServiceContext] protected ICurrentUser CurrentUser { get; set; }

        [FromServiceContext] protected IdWorker IdWorker { get; set; }
        
        private IFreeSqlContext _dbContext;
        
        [FromServiceContext]
        protected IFreeSqlContext DbContext
        {
            get {  return _dbContext;  }
            
            set
            {
                // 根据当前用户信息  不同租户 可以设置不同的查询条件 全局过滤器  例如以下 
                // value.GlobalFilter.ApplyOnly<ICompanyInfo>(nameof(ICompanyInfo),x => x.CompanyId == CurrentUser.CompanyId); // 相同名称的过滤器 最后的会覆盖前面的
                _dbContext = value;
            }
        }

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

            var flag = string.IsNullOrWhiteSpace(query.OrderByFileds);
            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    select.Where(item);
                }
            }

            
            return await select.OrderByDescending(flag, a => a.CreateTime)
                                .OrderBy(!flag, query.OrderByFileds)
                                .ToListAsync();
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
        public virtual async Task<PagedResult<T>> GetPagedListAysnc(Query<T> query)
        {
            H_Check.Argument.NotNull(query, nameof(query));

            var flag = string.IsNullOrWhiteSpace(query.OrderByFileds);
            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    select.Where(item);
                }
            }
            
            var items = await select.OrderByDescending(flag, a => a.CreateTime)
                                    .OrderBy(!flag, query.OrderByFileds)
                                    .Count(out var total)
                                    .Page(query.PageIndex, query.PageSize).ToListAsync();

            var pageList = new PagedResult<T>()
            {
                Items = items,
                TotalCount = (int)total,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPageCount = ((int)total + query.PageSize - 1) / query.PageSize
            };
            return pageList;
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

            var flag = string.IsNullOrWhiteSpace(query.OrderByFileds);
            var select = DbContext.Select<T>();

            if (query.QueryExpressions?.Count > 0)
            {
                foreach (var item in query.QueryExpressions)
                {
                    select.Where(item);
                }
            }

            return await select.DisableGlobalFilter(nameof(IsSoftDelete))
                                .OrderByDescending(flag, a => a.CreateTime)
                                .OrderBy(!flag, query.OrderByFileds)
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

            var type = typeof(T);
            var isGuid = typeof(TKey) == H_Util.GuidType;
            var id = type.GetProperty(nameof(Entity<TKey>.Id));

            if (isGuid)
            {
                id.SetValue(entity, Guid.NewGuid());
            }
            else
            {
                id.SetValue(entity, IdWorker.NextId());
            }

            if (CurrentUser.Id.HasValue)
            {
                entity.CreatorId = CurrentUser.Id;
                entity.CreateTime = DateTime.Now;
            }


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

            var isGuid = typeof(TKey) == H_Util.GuidType;
            var type = typeof(T);
            var id = type.GetProperty(nameof(Entity<TKey>.Id));
            var timeNow = DateTime.Now;
            entities.ForEach(item =>
            {
                if (isGuid)
                {
                    id.SetValue(item, Guid.NewGuid());
                }
                else
                {
                    id.SetValue(item, IdWorker.NextId());
                }

                if (CurrentUser.Id.HasValue)
                {
                    item.CreatorId = CurrentUser.Id;
                    item.CreateTime = timeNow;
                }
            });
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

            if (CurrentUser.Id.HasValue)
            {
                entity.ModifierId = CurrentUser.Id;
                entity.ModifyTime = DateTime.Now;
            }

            var update = DbContext.Update<T>()
                                    .SetSource(entity);

            foreach (var item in whereColumns)
            {
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

            if (CurrentUser.Id.HasValue)
            {
                entity.ModifierId = CurrentUser.Id;
                entity.ModifyTime = DateTime.Now;
                columns.Add(nameof(entity.ModifierId));
                columns.Add(nameof(entity.ModifyTime));
            }


            var update = DbContext.Update<T>()
                                    .SetSource(entity)
                                    .UpdateColumns(columns.ToArray());

            foreach (var item in whereColumns)
            {
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

            if (CurrentUser.Id.HasValue)
            {
                var timeNow = DateTime.Now;
                entities.ForEach(item =>
                {
                    item.ModifierId = CurrentUser.Id;
                    item.ModifyTime = timeNow;
                });
            }

            var update = DbContext.Update<T>()
                                    .SetSource(entities);

            foreach (var item in whereColumns)
            {
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

            if (CurrentUser.Id.HasValue)
            {
                var timeNow = DateTime.Now;
                entities.ForEach(item =>
                {
                    item.ModifierId = CurrentUser.Id;
                    item.ModifyTime = timeNow;
                });
                columns.Add(nameof(Entity<TKey>.ModifierId));
                columns.Add(nameof(Entity<TKey>.ModifyTime));
            }


            var update = DbContext.Update<T>()
                                    .SetSource(entities)
                                    .UpdateColumns(columns.ToArray());

            foreach (var item in whereColumns)
            {
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
                                    .SetIf(CurrentUser.Id.HasValue, a => a.ModifierId, CurrentUser.Id)
                                    .SetIf(CurrentUser.Id.HasValue, a => a.ModifyTime, DateTime.Now)
                                    .Where(a => a.Id.Equals(pkValue));

            foreach (var item in whereColumns)
            {
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
                                    .SetIf(CurrentUser.Id.HasValue, a => a.ModifierId, CurrentUser.Id)
                                    .SetIf(CurrentUser.Id.HasValue, a => a.ModifyTime, DateTime.Now)
                                    .Where(a => pkValues.Contains(a.Id));

            foreach (var item in whereColumns)
            {
                delete.Where(item);
            }

            return await delete.ExecuteAffrowsAsync();
        }
    }
}