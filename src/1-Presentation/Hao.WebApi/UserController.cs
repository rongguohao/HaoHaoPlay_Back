﻿using Hao.AppService;
using Hao.Core;
using Hao.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hao.WebApi
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserController : H_Controller
    {
        private readonly IUserAppService _userAppService;

        private readonly IRoleAppService _roleAppService;

        public UserController(IUserAppService userService, IRoleAppService roleAppService)
        {
            _userAppService = userService;
            _roleAppService = roleAppService;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCode("User_Add_1_32")]
        public async Task Add([FromBody]UserAddRequest request) => await _userAppService.Add(request);

        /// <summary>
        /// 查询用户分页列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Search_1_8388608")]
        public async Task<PagedResult<UserVM>> GetPagedList([FromQuery]UserQueryInput queryInput) => await _userAppService.GetPagedList(queryInput);

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthCode("User_Search_1_8388608")]
        public async Task<UserDetailVM> Get(long? id) => await _userAppService.Get(id.Value);

        ///// <summary>
        ///// 根据id获取用户 [Required]
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[AuthCode("1_4")]
        //public async Task<UserDetailVM> Get([Required] long? id) => await _userAppService.Get(id.Value);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Edit_1_64")]
        public async Task Update(long? id, [FromBody]UserUpdateRequest request) => await _userAppService.Update(id.Value, request);

        /// <summary>
        /// 注销用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Disable_1_128")]
        public async Task Disable(long? id) => await _userAppService.UpdateStatus(id.Value, false);

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Enable_1_256")]
        public async Task Enable(long? id) => await _userAppService.UpdateStatus(id.Value, true);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuthCode("User_Delete_1_512")]
        public async Task Delete(long? id) => await _userAppService.Delete(id.Value);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Search_1_8388608")]
        public async Task<List<RoleSelectVM>> GetRoleList() => await _roleAppService.GetRoleListByCurrentRole();

        /// <summary>
        /// 是否存在用户
        /// </summary>
        /// <param name="queryInput"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Search_1_8388608")]
        public async Task<bool> IsExistUser([FromQuery] UserQueryInput queryInput) => await _userAppService.IsExist(queryInput);


        /// <summary>
        /// 导出用户
        /// </summary>
        /// <param name="queryInput"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Export_1_2048")]
        public async Task<UserExcelVM> Export([FromQuery]UserQueryInput queryInput) => await _userAppService.Export(queryInput);


        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCode("User_Import_1_1024")]
        public async Task Import() => await _userAppService.Import(HttpContext.Request.Form.Files);
    }
}
