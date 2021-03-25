﻿using Hao.Core;
using Hao.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hao.Enum;
using Mapster;
using Hao.Service;

namespace Hao.AppService
{
    /// <summary>
    /// 模块应用服务
    /// </summary>
    public partial class ModuleAppService : ApplicationService, IModuleAppService
    {
        private readonly IModuleRepository _moduleRep;

        private readonly IModuleDomainService _moduleDomainService;

        public ModuleAppService(IModuleRepository moduleRep, IModuleDomainService moduleDomainService)
        {
            _moduleRep = moduleRep;
            _moduleDomainService = moduleDomainService;
        }


        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("ModuleAppService_AddModule")]
        public async Task Add(ModuleAddInput input)
        {
            var parentNode = await _moduleDomainService.Get(input.ParentId.Value);

            H_AssertEx.That(parentNode.Type == ModuleType.Sub, "子菜单无法继续添加节点");

            await _moduleDomainService.IsExistSameName(input.Name, input.Type, input.ParentId);

            await _moduleDomainService.IsExistSameAlias(input.Alias, input.Type, input.ParentId);

            var module = input.Adapt<SysModule>();

            if (parentNode.Type == ModuleType.Sub)
            {
                module.Alias = $"{parentNode.Alias}_{module.Alias}";
            }
            else if (parentNode.Type == ModuleType.Main)
            {
                module.ParentId = 0;
            }

            module.ParentAlias = parentNode.Alias;

            await _moduleDomainService.Add(module);
        }

        /// <summary>
        /// 获取所有模块列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModuleTreeOutput>> GetTreeList()
        {
            var modules = await _moduleRep.GetListAsync(new ModuleQuery() { IncludeResource = false });
            var result = new List<ModuleTreeOutput>();
            InitModuleTree(result, -1, modules);
            return result;
        }

        /// <summary>
        /// 获取模块详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ModuleDetailOutput> Get(long id)
        {
            var module = await _moduleDomainService.Get(id);
            var result = module.Adapt<ModuleDetailOutput>();

            if (result.Type == ModuleType.Sub)
            {
                var query = new ModuleQuery { ParentId = id };

                query.OrderBy(a => a.Sort).OrderBy(a => a.CreateTime);

                var resources = await _moduleRep.GetListAsync(query);

                result.Resources = resources.Adapt<List<ResourceItemOutput>>();
            }

            return result;
        }

        /// <summary>
        /// 更新模块信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("ModuleAppService_UpdateModule")]
        [UnitOfWork]
        public async Task Update(long id, ModuleUpdateInput input)
        {
            H_AssertEx.That(id == 0, "无法操作系统根节点");

            var module = await _moduleDomainService.Get(id);

            await _moduleDomainService.IsExistSameName(input.Name, module.Type, module.ParentId, id);

            await _moduleDomainService.IsExistSameName(input.Alias, module.Type, module.ParentId, id);

            if (module.Alias != input.Alias)
            {
                var sons = await _moduleRep.GetListAsync(new ModuleQuery { ParentId = id });
                sons.ForEach(a => a.ParentAlias = input.Alias);

                await _moduleRep.UpdateAsync(sons, a => new { a.ParentAlias });
            }

            module = input.Adapt(module);

            if (module.Type == ModuleType.Main)
            {
                await _moduleRep.UpdateAsync(module, a => new { a.Name, a.Icon, a.Sort, a.Alias });
            }
            else if (module.Type == ModuleType.Sub)
            {
                await _moduleRep.UpdateAsync(module, a => new { a.Name, a.RouterUrl, a.Sort, a.Alias });
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(long id)
        {
            H_AssertEx.That(id == 0, "无法操作系统根节点");

            var module = await _moduleRep.GetAsync(id);

            var childs = await _moduleRep.GetListAsync(new ModuleQuery()
            {
                ParentId = module.Id
            });

            H_AssertEx.That(childs != null && childs.Count > 0, "存在子节点无法删除");

            await _moduleRep.DeleteAsync(module);
        }


        #region private

        /// <summary>
        /// 递归初始化模块树
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parentId"></param>
        /// <param name="sources"></param>
        private void InitModuleTree(List<ModuleTreeOutput> result, long parentId, List<SysModule> sources)
        {
            //递归寻找子节点  
            var tempTree = sources.Where(item => item.ParentId == parentId).OrderBy(a => a.Sort);
            foreach (var item in tempTree)
            {
                var node = new ModuleTreeOutput()
                {
                    key = item.Id.ToString(),
                    title = item.Name,
                    isLeaf = item.Type == ModuleType.Sub,
                    expanded = true,
                    children = new List<ModuleTreeOutput>()
                };
                result.Add(node);
                InitModuleTree(node.children, item.Id, sources);
            }
        }

        #endregion
    }
}