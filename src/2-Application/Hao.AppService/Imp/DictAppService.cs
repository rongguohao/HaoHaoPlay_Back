using Hao.Core;
using Hao.Model;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Mapster;
using Hao.Enum;

namespace Hao.AppService
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class DictAppService : ApplicationService, IDictAppService
    {
        private readonly IDictRepository _dictRep;

        public DictAppService(IDictRepository dictRep)
        {
            _dictRep = dictRep;
        }


        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("DictAppService_AddDict")]
        public async Task Add(DictAddInput input)
        {
            var sameItems = await _dictRep.GetListAsync(new DictQuery { DictName = input.DictName });

            H_AssertEx.That(sameItems.Count > 0, "字典名称已存在，请重新输入");

            sameItems = await _dictRep.GetListAsync(new DictQuery { DictCode = input.DictCode });

            H_AssertEx.That(sameItems.Count > 0, "字典编码已存在，请重新输入");

            var dict = input.Adapt<SysDict>();
            dict.ParentId = -1;
            dict.Sort = 0;
            await _dictRep.InsertAsync(dict);
        }

        /// <summary>
        /// 修改字典
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("DictAppService_UpdateDict")]
        public async Task Update(long id, DictUpdateInput input)
        {
            var sameItems = await _dictRep.GetListAsync(new DictQuery { DictName = input.DictName });

            H_AssertEx.That(sameItems.Any(a => a.Id != id), "字典名称已存在，请重新输入");

            sameItems = await _dictRep.GetListAsync(new DictQuery { DictCode = input.DictCode });

            H_AssertEx.That(sameItems.Any(a => a.Id != id), "字典编码已存在，请重新输入");

            var dict = await _dictRep.GetAsync(id);
            dict.DictCode = input.DictCode;
            dict.DictName = input.DictName;
            dict.Remark = input.Remark;
            dict.Sort = input.Sort;
            await _dictRep.UpdateAsync(dict, a => new { a.DictCode, a.DictName, a.Remark, a.Sort });
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task Delete(long id)
        {
            var dict = await _dictRep.GetAsync(id);

            var dictItems = await _dictRep.GetListAsync(new DictQuery { ParentId = id });

            await _dictRep.DeleteAsync(dict);

            if (dictItems.Count < 1) return;

            await _dictRep.DeleteAsync(dictItems);
        }



        /// <summary>
        /// 查询字典
        /// </summary>
        /// <param name="queryInput"></param>
        /// <returns></returns>
        public async Task<Paged<DictOutput>> GetPaged(DictQueryInput queryInput)
        {
            var query = queryInput.Adapt<DictQuery>();
            query.DictType = DictType.Main;

            var result = await _dictRep.GetDictPagedResult(query);

            return result.Adapt<Paged<DictOutput>>();

        }

        /// <summary>
        /// 添加字典项
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("DictAppService_AddDictItem")]
        public async Task AddDictItem(DictItemAddInput input)
        {
            var sameItems = await _dictRep.GetListAsync(new DictQuery { ParentId = input.ParentId, ItemName = input.ItemName });

            H_AssertEx.That(sameItems.Count > 0, "数据项名称已存在，请重新输入");


            sameItems = await _dictRep.GetListAsync(new DictQuery { ParentId = input.ParentId, ItemValue = input.ItemValue });

            H_AssertEx.That(sameItems.Count > 0, "数据项值已存在，请重新输入");


            var parentDict = await GetAsync(input.ParentId.Value);
            var dict = input.Adapt<SysDict>();
            dict.ParentId = parentDict.Id;
            dict.DictCode = parentDict.DictCode;
            dict.DictName = parentDict.DictName;
            dict.DictType = DictType.Sub;
            if (!dict.Sort.HasValue)
            {
                var dictItems = await _dictRep.GetListAsync(new DictQuery { ParentId = input.ParentId.Value });
                dict.Sort = dictItems.Count + 1;
            }
            await _dictRep.InsertAsync(dict);
        }

        /// <summary>
        /// 获取字典数据
        /// </summary>
        /// <returns></returns>
        public async Task<Paged<DictItemOutput>> GetDictItemPaged(DictQueryInput queryInput)
        {
            var query = queryInput.Adapt<DictQuery>();

            query.OrderBy(a => a.Sort).OrderBy(a => a.CreateTime);

            var dicts = await _dictRep.GetPagedAsync(query);

            return dicts.Adapt<Paged<DictItemOutput>>();
        }

        /// <summary>
        /// 更新数据项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [DistributedLock("DictAppService_UpdateDictItem")]
        public async Task UpdateDictItem(long id, DictItemUpdateInput input)
        {
            var item = await _dictRep.GetAsync(id);

            var sameItems = await _dictRep.GetListAsync(new DictQuery { ParentId = item.ParentId, ItemName = input.ItemName });

            H_AssertEx.That(sameItems.Any(a => a.Id != id), "数据项名称已存在，请重新输入");

            sameItems = await _dictRep.GetListAsync(new DictQuery { ParentId = item.ParentId, ItemValue = input.ItemValue });

            H_AssertEx.That(sameItems.Any(a => a.Id != id), "数据项值已存在，请重新输入");

            item.ItemName = input.ItemName;
            item.ItemValue = input.ItemValue;
            item.Remark = input.Remark;
            item.Sort = input.Sort;
            await _dictRep.UpdateAsync(item, a => new { a.ItemName, a.ItemValue, a.Remark, a.Sort });
        }

        /// <summary>
        /// 删除数据项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteDictItem(long id)
        {
            var dict = await _dictRep.GetAsync(id);
            await _dictRep.DeleteAsync(dict);
        }

        /// <summary>
        /// 根据字典编码查询数据项
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>

        public async Task<List<DictDataItemOutput>> GetDictDataItem(string dictCode)
        {
            var query = new DictQuery
            {
                DictCode = dictCode,
                DictType = DictType.Sub
            };

            query.OrderBy(a => a.Sort).OrderBy(a => a.CreateTime);

            var dictItems = await _dictRep.GetListAsync(query);

            return dictItems.Adapt<List<DictDataItemOutput>>();
        }


        #region private

        /// <summary>
        /// 字典详情
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        private async Task<SysDict> GetAsync(long dictId)
        {
            var dict = await _dictRep.GetAsync(dictId);

            H_AssertEx.That(dict == null, "字典数据不存在");
            H_AssertEx.That(dict.IsDeleted, "字典数据已删除");

            return dict;
        }
        #endregion
    }
}