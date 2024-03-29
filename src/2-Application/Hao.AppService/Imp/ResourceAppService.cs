﻿using Hao.Core;
using Hao.Enum;
using Hao.Model;
using Mapster;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hao.AppService
{
    /// <summary>
    /// 模块应用服务-资源
    /// </summary>
    public partial class ModuleAppService
    {
        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("ModuleAppService_AddResource")]
        public async Task AddResource(ResourceAddInput input)
        {
            var parentNode = await _moduleDomainService.Get(input.ParentId.Value);

            if (parentNode.Type != ModuleType.Sub) return;

            await _moduleDomainService.CheckName(input.Name, ModuleType.Resource, input.ParentId);

            await _moduleDomainService.CheckAlias(input.Alias, ModuleType.Resource, input.ParentId);

            var module = input.Adapt<SysModule>();
            module.Type = ModuleType.Resource;
            module.Sort = 0;
            module.ParentAlias = parentNode.Alias;

            await _moduleDomainService.Add(module);
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteResource(long id)
        {
            _moduleDomainService.MustNotRoot(id);

            var node = await _moduleDomainService.Get(id);

            if (node.Type != ModuleType.Resource) return;

            await _moduleRep.DeleteAsync(node);
        }

        /// <summary>
        /// 资源列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<ResourceItemOutput>> GetResourceList(long parentId)
        {
            var query = new ModuleQuery { ParentId = parentId };

            query.OrderBy(a => a.Sort).OrderBy(a => a.CreateTime);

            var resources = await _moduleRep.GetListAsync(query);

            var result = resources.Adapt<List<ResourceItemOutput>>();
            return result;
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        [DistributedLock("ModuleAppService_UpdateResource",Order = 1)]
        [UnitOfWork(Order = 2)]
        public async Task UpdateResource(long id, ResourceUpdateInput vm)
        {
            _moduleDomainService.MustNotRoot(id);

            var module = await _moduleDomainService.Get(id);

            if (module.Type != ModuleType.Resource) return;

            await _moduleDomainService.CheckName(vm.Name, ModuleType.Resource, module.ParentId, id);

            await _moduleDomainService.CheckAlias(vm.Alias, ModuleType.Resource, module.ParentId, id);

            module = vm.Adapt(module);
            await _moduleRep.UpdateAsync(module, a => new { a.Name, a.Sort, a.Alias });
        }
    }
}
