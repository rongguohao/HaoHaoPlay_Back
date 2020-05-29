using System.Collections.Generic;
using System.Threading.Tasks;
using Hao.AppService.ViewModel;
using Hao.Core;

namespace Hao.AppService
{
    /// <summary>
    /// 字典服务接口
    /// </summary>
    public interface IDictAppService
    {
        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddDict(DictAddRequest request);
        
        /// <summary>
        /// 查询字典
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PagedList<DictVM>> GetDictPagedList(DictQuery query);

        /// <summary>
        /// 修改字典
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateDict(long id,DictUpdateRequest request);

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteDict(long id);
        
        /// <summary>
        /// 添加字典数据项
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddDictItem(DictItemAddRequest request);

        /// <summary>
        /// 查询字典数据项
        /// </summary>
        /// <returns></returns>
        Task<PagedList<DictItemVM>> GetDictItemPagedList(DictQuery query);

        /// <summary>
        /// 更新数据项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateDictItem(long id, DictItemUpdateRequest request);

        /// <summary>
        /// 删除数据项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteDictItem(long id);

        /// <summary>
        /// 根据字典编码查询数据项
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        Task<List<DictDataItemVM>> GetDictDataItem(string dictCode);
    }
}