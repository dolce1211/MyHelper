using MyHelper.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Entities
{
    /// <summary>
    /// 単一の簡易ランチャのアイテム
    /// </summary>
    [Table("LauncherItems")]
    [DebuggerDisplay("{LauncherType}:{Title}  path: {Path}")]
    public class LauncherItem : IDbContextEntity
    {
        [Key]
        public int Id { get; set; } = 0;
        /// <summary>
        /// ファイルパスかフォルダパスかURLか
        /// </summary>
        public LauncherType LauncherType { get; set; } = LauncherType.none;

        /// <summary>
        /// 並び順
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// ファイルパスだったりフォルダパスだったりURLだったり
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// 表示名称
        /// </summary>
        public string Title { get; set; } = "";
    }
    /// <summary>
    /// 簡易ランチャのタイプ
    /// </summary>
    public enum LauncherType
    {
        none = -1,

        /// <summary>
        /// ファイルパス
        /// </summary>
        FilePath = 0,

        /// <summary>
        /// フォルダパス
        /// </summary>
        FolderPath = 1,

        /// <summary>
        /// URL
        /// </summary>
        URL = 2
    }
}
