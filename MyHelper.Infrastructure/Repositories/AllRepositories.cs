using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Infrastructure.Repositories
{

    /// <summary>
    /// 設定用データリポジトリ
    /// </summary>
    public class SettingDataRepository : DBContextRepositoryBase<Setting>, ISettingDataRepository
    {
        public async Task<Setting> GetSingleAsync()
        {
            // _context.Settingsが空白の場合、ダミーデータを登録する
            if (!await _context.Settings.AnyAsync())
            {
                await base.AddOrUpdateAsync(1, new Setting()
                {
                    OpenValidPathByCtrlCC = false,
                    EnableLauncherByMouseButton = false,
                    EnableLauncherByCtrlKeys = false,
                });
            }
            return await _context.Settings.SingleAsync()!;
        }
    }

    /// <summary>
    /// ランチャー用データリポジトリ
    /// </summary>
    public class LauncherDataRepository : DBContextRepositoryBase<LauncherItem>, ILauncherDataRepository
    {
    }

    /// <summary>
    /// オートコンプリート使用履歴用データリポジトリ
    /// </summary>
    public class AutoCompleteTextRepository : DBContextRepositoryBase<AutoCompleteTextRecord>, IAutoCompleteTextRepository
    {
    }

    /// <summary>
    /// TableInfo用データリポジトリ
    /// </summary>
    public class TableInfoRepository : DBContextRepositoryBase<TableInfo>, ITableInfoRepository
    {
        /// <summary>
        /// 全件削除
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> DeleteAllAsync()
        {
            var allColumnInfos = _context.TableColumnInfos.ToList();
            _context.TableColumnInfos.RemoveRange(allColumnInfos);
            await _context.SaveChangesAsync();
            return await base.DeleteAllAsync();
        }
        public override async Task<List<TableInfo>?> GetAsync(Expression<Func<TableInfo, bool>>? where = null,Expression<Func<TableInfo, object>>? orderby = null)
        {
            var query = _context.TableInfos.Include(t => t.TableColumnInfos).AsQueryable();
            if (where != null)
                query = query.Where(where);
            if (orderby != null)
                query = query.OrderBy(orderby);

            //TableColumnInfoをInclude
            var ret = await query.ToListAsync();
            return ret;
        }
    }

    /// <summary>
    /// TableColumnInfo用データリポジトリ
    /// </summary>
    public class TableColumnInfoRepository : DBContextRepositoryBase<TableColumnInfo>, ITableColumnInfoRepository
    {
    }


}
