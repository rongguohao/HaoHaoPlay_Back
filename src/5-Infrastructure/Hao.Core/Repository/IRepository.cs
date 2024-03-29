﻿using Hao.Dependency;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hao.Core
{
    /// <summary>
    /// 仓储通用接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<T, TKey> : ITransientDependency where T : IEntity<TKey>, new() where TKey : struct
    {
        /// <summary>
        /// 根据主键查询单条数据
        /// </summary>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        Task<T> GetAsync(TKey pkValue);

        /// <summary>
        /// 根据主键集合查询多条数据
        /// </summary>
        /// <param name="pkValues"></param>
        /// <returns>泛型实体</returns>
        Task<List<T>> GetListAsync(List<TKey> pkValues);

        /// <summary>
        /// 查询所有数据（未删除）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync();

        /// <summary>
        /// 根据条件查询所有数据（未删除）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Query<T> query);

        /// <summary>
        /// 根据条件查询所有数据数量（未删除）
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync(Query<T> query);

        /// <summary>
        /// 根据条件查询分页数据（未删除）
        /// </summary>
        /// <returns></returns>
        Task<Paged<T>> GetPagedAsync(Query<T> query);


        /// <summary>
        /// 查询所有数据（包含已删除）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// 查询所有数据（包含已删除）
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(Query<T> query);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// 插入数据（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<List<T>> InsertAsync(List<T> entities);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, params Expression<Func<T, bool>>[] whereColumns);

        /// <summary>
        /// 更新数据（指定列）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateColumns"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity, Expression<Func<T, object>> updateColumns, params Expression<Func<T, bool>>[] whereColumns);

        /// <summary>
        /// 更新实体数据（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entities, params Expression<Func<T, bool>>[] whereColumns);

        /// <summary>
        /// 异步更新实体数据（批量）（指定列）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateColumns"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(List<T> entities, Expression<Func<T, object>> updateColumns, params Expression<Func<T, bool>>[] whereColumns);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity, params Expression<Func<T, bool>>[] whereColumns);

        /// <summary>
        /// 删除数据（批量）
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="whereColumns"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(List<T> entities, params Expression<Func<T, bool>>[] whereColumns);

        ///// <summary>
        ///// 删除数据
        ///// </summary>
        ///// <param name="pkValue"></param>
        ///// <param name="whereColumns"></param>
        ///// <returns></returns>
        //Task<int> DeleteAsync(TKey pkValue, params Expression<Func<T, bool>>[] whereColumns);

        ///// <summary>
        ///// 删除数据
        ///// </summary>
        ///// <param name="pkValues"></param>
        ///// <param name="whereColumns"></param>
        ///// <returns></returns>
        //Task<int> DeleteAsync(List<TKey> pkValues, params Expression<Func<T, bool>>[] whereColumns);
    }
}