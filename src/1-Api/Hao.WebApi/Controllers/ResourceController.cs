﻿using Hao.AppService;
using Hao.AppService.ViewModel;
using Hao.Core;
using Hao.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hao.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceController:HController
    {
        private readonly IModuleAppService _moduleAppService;

        public ResourceController(IModuleAppService moduleAppService)
        {
            _moduleAppService = moduleAppService;
        }


        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Add([FromBody]ResourceAddRequest request) => await _moduleAppService.AddResource(request);

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(long id) => await _moduleAppService.DeleteResource(id);

        /// <summary>
        /// 查询资源分页列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetList/{parentId}")]
        public async Task<List<ResourceItemVM>> GetList(long parentId) => await _moduleAppService.GetResourceList(parentId);


        /// <summary>
        /// 更新资源信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update(long id, [FromBody]ResourceUpdateRequest request) => await _moduleAppService.UpdateResource(id, request);
    }
}
