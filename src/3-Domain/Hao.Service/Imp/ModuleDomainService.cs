﻿using Hao.Core;
using Hao.Library;
using Hao.Model;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Hao.Service
{
    public class ModuleDomainService : DomainService, IModuleDomainService
    {
        private readonly IModuleRepository _moduleRep;

        public ModuleDomainService(IModuleRepository moduleRep)
        {
            _moduleRep = moduleRep;
        }

        /// <summary>
        /// 添加菜单模块
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task Add(SysModule module)
        {
            var max = await _moduleRep.GetLayerCount();
            if (max.Count < 31)
            {
                module.Layer = max.Layer;
                module.Number = Convert.ToInt64(Math.Pow(2, max.Count.Value));
            }
            else if (max.Count == 31) //0次方 到 30次方 共31个数              js语言的限制 导致  位运算 32位  
            {
                module.Layer = ++max.Layer;
                module.Number = 1;
            }
            else
            {
                throw new H_Exception("数据库数据异常，请检查");
            }

            try
            {
                await _moduleRep.InsertAsync(module);
            }
            catch (PostgresException ex)
            {
                H_AssertEx.That(ex.SqlState == H_PostgresSqlState.E23505, "添加失败，请重新添加");
            }
        }

        /// <summary>
        /// 获取菜单模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SysModule> Get(long id)
        {
            var module = await _moduleRep.GetAsync(id);

            H_AssertEx.That(module == null, "节点不存在");
            H_AssertEx.That(module.IsDeleted, "节点已删除");

            return module;
        }
    }
}
