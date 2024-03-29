using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Hao.Core;
using Hao.Enum;
using Hao.EventData;
using Hao.Model;
using Hao.Runtime;
using Hao.Service;
using Mapster;
using Newtonsoft.Json;

namespace Hao.AppService
{
    /// <summary>
    /// 角色应用服务
    /// </summary>
    public class RoleAppService : ApplicationService, IRoleAppService
    {
        private readonly IRoleRepository _roleRep;

        private readonly IModuleRepository _moduleRep;

        private readonly IUserRepository _userRep;

        private readonly ICapPublisher _publisher;

        private readonly ICurrentUser _currentUser;

        private readonly IRoleDomainService _roleDomainService;

        public RoleAppService(ICurrentUser currentUser, ICapPublisher publisher, IRoleRepository roleRep, IModuleRepository moduleRep, IUserRepository userRep, IRoleDomainService roleDomainService)
        {
            _roleRep = roleRep;
            _moduleRep = moduleRep;
            _userRep = userRep;
            _publisher = publisher;
            _currentUser = currentUser;
            _roleDomainService = roleDomainService;
        }


        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleOutput>> GetList()
        {
            var query = new RoleQuery();

            if (_currentUser.RoleLevel != (int)RoleLevel.SuperAdministrator) //超级管理员能看见所有，其他用户只能看见比自己等级低的用户列表
            {
                query.CurrentRoleLevel = _currentUser.RoleLevel;
            }

            var roles = await _roleRep.GetRoleList(query);

            var result = roles.Adapt<List<RoleOutput>>();

            return result;
        }

        /// <summary>
        /// 根据当前用户角色，获取可以操作得角色列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleSelectOutput>> GetRoleListByCurrentRole()
        {
            var query = new RoleQuery()
            {
                CurrentRoleLevel = _currentUser.RoleLevel
            };

            query.OrderBy(a => a.Level);

            var roles = await _roleRep.GetListAsync(query);

            var result = roles.Adapt<List<RoleSelectOutput>>();
            return result;
        }

        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [CapUnitOfWork]
        public async Task UpdateRoleAuth(long id, RoleUpdateAuthInput input)
        {
            var role = await _roleDomainService.Get(id);
            role.AuthNumbers = null;
            
            if (input.ModuleIds?.Count > 0)
            {
                var modules = await _moduleRep.GetListAsync(input.ModuleIds);
                var maxLayer = modules.Max(a => a.Layer);

                var authNumbers = new List<long>();
                for (int i = 1; i <= maxLayer; i++)
                {
                    var authNumber = 0L;
                    var items = modules.Where(a => a.Layer == i);
                    foreach (var x in items)
                    {
                        authNumber = authNumber | x.Number.Value;
                    }
                    authNumbers.Add(authNumber);
                }

                role.AuthNumbers = JsonConvert.SerializeObject(authNumbers);
            }

            var users = await _userRep.GetListAsync(new UserQuery() { RoleLevel = role.Level });
            var ids = users.Where(a => a.AuthNumbers != role.AuthNumbers).Select(a => a.Id).ToList();

            await _roleDomainService.UpdateRoleAuth(role);

            await _userRep.UpdateAuth(role.Id, role.AuthNumbers);

            //注销该角色下用户的登录信息
            if (ids.Count < 1) return;

            await _publisher.PublishAsync(nameof(LogoutForUpdateAuthEventData), new LogoutForUpdateAuthEventData
            {
                UserIds = ids
            });
        }

        /// <summary>
        /// 获取角色拥有的模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoleModuleOutput> GetRoleModule(long id)
        {
            var role = await _roleDomainService.Get(id);
            var authNumbers = string.IsNullOrWhiteSpace(role.AuthNumbers) ? null : JsonConvert.DeserializeObject<List<long>>(role.AuthNumbers);
            var modules = await _moduleRep.GetListAsync();
            var result = new RoleModuleOutput();
            result.Nodes = new List<RoleModuleItemOutput>();
            result.CheckedKeys = new List<string>();
            InitModuleTree(result.Nodes, -1, modules, authNumbers, result.CheckedKeys);
            return result;
        }

        #region private

        /// <summary>
        /// 递归初始化模块树
        /// </summary>
        /// <param name="result"></param>
        /// <param name="parentID"></param>
        /// <param name="sources"></param>
        /// <param name="authNumbers"></param>
        /// <param name="checkedKeys"></param>
        private void InitModuleTree(List<RoleModuleItemOutput> result, long parentID, List<SysModule> sources, List<long> authNumbers, List<string> checkedKeys)
        {
            //递归寻找子节点  
            var tempTree = sources.Where(item => item.ParentId == parentID).OrderBy(a => a.Sort).ToList();
            foreach (var item in tempTree)
            {
                var node = new RoleModuleItemOutput()
                {
                    key = item.Id.ToString(),
                    title = item.Name,
                    isLeaf = item.Type == ModuleType.Resource,
                    expanded = (int)item.Type.Value < (int)ModuleType.Sub,
                    children = new List<RoleModuleItemOutput>()
                };

                result.Add(node);

                InitModuleTree(node.children, item.Id, sources, authNumbers, checkedKeys);

                if (item.Type == ModuleType.Sub) node.isLeaf = node.children.Count == 0;

                if (node.isLeaf && authNumbers?.Count > 0 && item.Layer.Value <= authNumbers.Count)
                {
                    if ((authNumbers[item.Layer.Value - 1] & item.Number) == item.Number)
                    {
                        checkedKeys.Add(node.key);
                    }
                }

                if (item.Type == ModuleType.Main && node.children.Count < 1) result.Remove(node);
            }
        }
        #endregion
    }
}

