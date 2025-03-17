
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Entities
{
    /// <summary>
    /// 設定情報
    /// </summary>
    [Table("Settings")]
    public class Setting : IDbContextEntity
    {
        [Key]
        public int Id { get; set; } 


        /// <summary>
        /// マウス左右ボタン同時押しで簡易ランチャーを開くならtrue
        /// </summary>
        public bool EnableLauncherByMouseButton { get; set; } = false;

        /// <summary>
        /// 左右のCtrlキー同時押しで簡易ランチャーを開くならtrue
        /// </summary>
        public bool EnableLauncherByCtrlKeys { get; set; } = false;

        /// <summary>
        /// 有効なパスを選択してCtrl+CCでそのパスを開くならtrue
        /// </summary>
        public bool OpenValidPathByCtrlCC { get; set; } = false;

        /// <summary>
        /// 前回読み込んだtables.csvの更新日時
        /// </summary>
        public DateTime LastWrittenTimeForTables { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 設定済みのパスが無効になっている場合など、設定を見直すよう促すならtrue
        /// </summary>
        /// <returns></returns>
        public bool NeedsSettingRenewed()
        {
            return false;
        }
    }
}
