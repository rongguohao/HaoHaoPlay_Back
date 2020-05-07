﻿using Hao.Core;
using Hao.Model;
using System.Threading.Tasks;

namespace Hao.Repository
{
    public interface ISysModuleRepository : IRepository<SysModule, long>
    {
        /// <summary>
        /// 获取每一层的数量，包括已删除的，最多64个
        /// </summary>
        /// <returns></returns>
        Task<ModuleLayerCountDto> GetLayerCount();
    }
}
