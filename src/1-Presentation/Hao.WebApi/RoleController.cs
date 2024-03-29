using Hao.AppService;
using Hao.Core;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hao.WebApi
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
        [AuthCode("Role_Search_1_131072")]
        public async Task<List<RoleOutput>> GetRoleList() => await _roleAppService.GetList();

        /// <summary>
        /// 获取角色用户的模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthCode("Role_ViewAuth_1_262144")]
        public async Task<RoleModuleOutput> GetRoleModule(long? id) => await _roleAppService.GetRoleModule(id.Value);

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("Role_UpdateAuth_1_524288")]
        public async Task UpdateRoleAuth(long? id, [FromBody] RoleUpdateAuthInput input) => await _roleAppService.UpdateRoleAuth(id.Value, input);

    }
}