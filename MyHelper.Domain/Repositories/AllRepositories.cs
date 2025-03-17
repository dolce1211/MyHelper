using MyHelper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Repositories
{
    /// <summary>
    /// 設定用リポジトリ
    /// </summary>
    public interface ISettingDataRepository : IDataRepositoryBase<Setting>
    {
        Task<Setting> GetSingleAsync();
    }

    /// <summary>
    /// ランチャー情報用リポジトリ
    /// </summary>
    public interface ILauncherDataRepository : IDataRepositoryBase<LauncherItem>
    {   
    }

    /// <summary>
    /// オートコンプリート使用履歴用リポジトリ
    /// </summary>
    public interface IAutoCompleteTextRepository : IDataRepositoryBase<AutoCompleteTextRecord>
    {
    }

    /// <summary>
    /// テーブル情報用リポジトリ
    /// </summary>
    public interface ITableInfoRepository : IDataRepositoryBase<TableInfo>
    {
    }

    /// <summary>
    /// テーブル列情報用リポジトリ
    /// </summary>
    public interface ITableColumnInfoRepository : IDataRepositoryBase<TableColumnInfo>
    {
    }

}
