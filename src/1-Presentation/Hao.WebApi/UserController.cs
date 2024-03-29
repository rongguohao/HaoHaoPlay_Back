﻿using Hao.AppService;
using Hao.Core;
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

        public UserController(IUserAppService userService)
        {
            _userAppService = userService;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCode("User_Add_1_128")]
        public async Task Add([FromBody] UserAddInput input) => await _userAppService.Add(input);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Add_1_128")]
        public async Task<List<RoleSelectOutput>> GetRoleList([FromServices] IRoleAppService roleAppService) => await roleAppService.GetRoleListByCurrentRole();

        /// <summary>
        /// 是否存在相同账号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Add_1_128")]
        public async Task<bool> IsExistAccount(string account) => await _userAppService.IsExistAccount(account);

        /// <summary>
        /// 查询用户分页列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Search_1_64")]
        //[AntiDuplicateRequest(GlobalLockKey = "User_GetPagedList", Interval = 5, Type = LockType.User)]
        public async Task<Paged<UserOutput>> GetPagedList([FromQuery] UserQueryInput queryInput) => await _userAppService.GetPaged(queryInput);

        /// <summary>
        /// 根据id获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AuthCode("User_View_1_1024")]
        public async Task<UserDetailOutput> Get(long? id) => await _userAppService.Get(id.Value);

        ///// <summary>
        ///// 根据id获取用户 [Required]
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[AuthCode("1_4")]
        //public async Task<UserDetailVM> Get([Required] long? id) => await _userAppService.Get(id.Value);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Update_1_256")]
        public async Task Update(long? id, [FromBody] UserUpdateInput input) => await _userAppService.Update(id.Value, input);

        /// <summary>
        /// 注销用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Disable_1_16777216")]
        public async Task Disable(long? id) => await _userAppService.UpdateStatus(id.Value, false);

        /// <summary>
        /// 启用用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AuthCode("User_Enable_1_33554432")]
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
        /// 导出用户
        /// </summary>
        /// <param name="queryInput"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthCode("User_Export_1_2048")]
        public async Task<UserExcelOutput> Export([FromQuery] UserQueryInput queryInput) => await _userAppService.Export(queryInput);


        /// <summary>
        /// 导入用户
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCode("User_Import_1_4096")]
        public async Task Import() => await _userAppService.Import(HttpContext.Request.Form.Files);
    }
}
