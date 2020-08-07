using Hao.AppService;
using Hao.AppService.ViewModel;
using Hao.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hao.WebApi.Controllers
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RoleController : H_Controller
    {
        private readonly IRoleAppService _roleAppService;

        public RoleController(IRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("1_16")]
        public async Task<List<RoleVM>> GetRoleList() => await _roleAppService.GetRoleList();

        /// <summary>
        /// 获取角色用户的模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthCode("1_16")]
        public async Task<RoleModuleVM> GetRoleModule(long? id) => await _roleAppService.GetRoleModule(id.Value);

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("1_262144")]
        public async Task UpdateRoleAuth(long? id, [FromBody]RoleUpdateRequest request) => await _roleAppService.UpdateRoleAuth(id.Value, request);

        ///// <summary>
        ///// 添加角色
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task Add([FromBody] RoleAddRequest request) => await _roleAppService.AddRole(request);

        ///// <summary>
        ///// 删除角色
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpDelete("{id}")]
        //public async Task DeleteRole(long id) => await _roleAppService.DeleteRole(id);
    }
}