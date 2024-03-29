﻿using Hao.Core;
using Hao.Enum;
using Hao.Library;
using Hao.Model;
using Mapster;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hao.Service
{
    /// <summary>
    /// 菜单模块领域服务
    /// </summary>
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

            H_AssertEx.That(module == null, "模块或资源不存在");
            H_AssertEx.That(module.IsDeleted, "模块或资源已删除");

            return module;
        }

        /// <summary>
        /// 是否存在相同名字的模块
        /// </summary>
        public async Task CheckName(string name, ModuleType? moduleType, long? parentId, long? id = null)
        {
            var modules = await _moduleRep.GetSameName(name, moduleType, parentId, id);

            H_AssertEx.That(modules.Count > 0, "存在相同名称的模块或资源");

        }

        /// <summary>
        /// 是否存在相同别名的模块
        /// </summary>
        public async Task CheckAlias(string alias, ModuleType? moduleType, long? parentId, long? id = null)
        {
            var modules = await _moduleRep.GetSameAlias(alias, moduleType, parentId, id);

            H_AssertEx.That(modules.Count > 0, "存在相同别名的模块或资源");
        }

        /// <summary>
        /// 获取权限数组值对应的应用菜单树
        /// </summary>
        /// <param name="authNums"></param>
        /// <returns></returns>
        public async Task<List<MenuTreeDto>> GetMenuTree(List<long> authNums)
        {
            var modules = await _moduleRep.GetListAsync(new ModuleQuery { IncludeResource = false });

            var menus = new List<MenuTreeDto>();

            //找主菜单一级 parentId=0
            GetMenuTree(menus, 0, modules, authNums);

            return menus;
        }


        /// <summary>
        /// 获取权限数组值对应的应用菜单树
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parentId"></param>
        /// <param name="sources"></param>
        /// <param name="authNums"></param>
        private void GetMenuTree(List<MenuTreeDto> result, long? parentId, List<SysModule> sources, List<long> authNums)
        {
            //递归寻找子节点  
            var tempTree = sources.Where(item => item.ParentId == parentId).OrderBy(a => a.Sort);
            foreach (var item in tempTree)
            {
                if (authNums?.Count < 1 || item.Layer.Value > authNums.Count) continue;

                if ((authNums[item.Layer.Value - 1] & item.Number) != item.Number) continue;

                var node = item.Adapt<MenuTreeDto>();
                node.ChildMenus = new List<MenuTreeDto>();

                result.Add(node);
                GetMenuTree(node.ChildMenus, item.Id, sources, authNums);
                if (item.Type == ModuleType.Main && node.ChildMenus.Count < 1) result.Remove(node);
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(long id)
        {
            MustNotRoot(id);

            var module = await Get(id);

            var childs = await _moduleRep.GetListAsync(new ModuleQuery()
            {
                ParentId = module.Id
            });

            H_AssertEx.That(childs != null && childs.Count > 0, "存在子节点无法删除");

            await _moduleRep.DeleteAsync(module);
        }

        /// <summary>
        /// 检测必须不是根节点方可继续操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void MustNotRoot(long id)
        {
            H_AssertEx.That(id == 0, "无法操作系统根节点");
        }
    }
}
