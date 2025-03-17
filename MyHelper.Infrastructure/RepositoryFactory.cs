using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.MyHelperDbContext;
using MyHelper.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Infrastructure
{
    /// <summary>
    /// リポジトリのファクトリ
    /// </summary>
    public static class RepositoryFactory
    {

        /// <summary>
        /// 設定用データリポジトリを生成する
        /// </summary>
        /// <returns></returns>
        public static ISettingDataRepository CreateSettingDataRepository()
        {
            return new SettingDataRepository();
        }

        /// <summary>
        /// AutoCompleteTextBox用データリポジトリを生成する
        /// </summary>
        /// <returns></returns>
        public static IAutoCompleteTextRepository CreateAutoCompleteTextRepository()
        {
            return new AutoCompleteTextRepository();
        }

        /// <summary>
        /// ランチャー用データリポジトリを生成する
        /// </summary>
        /// <returns></returns>
        public static ILauncherDataRepository CreateLauncherDataRepository()
        {
            return new LauncherDataRepository();
        }


        /// <summary>
        /// テーブル情報用データリポジトリを生成する
        /// </summary>
        /// <returns></returns>
        public static ITableInfoRepository CreateTableInfoRepository()
        {
            return new TableInfoRepository();
        }

        /// <summary>
        /// テーブル列情報用データリポジトリを生成する
        /// </summary>
        /// <returns></returns>
        public static ITableColumnInfoRepository CreateTableColumnInfoRepository()
        {
            return new TableColumnInfoRepository();
        }
    }
}
