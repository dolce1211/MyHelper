using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Entities
{
    /// <summary>
    /// 単一のテーブル情報を表すエンティティ
    /// </summary>
    [Table("TableInfos")]
    [DebuggerDisplay("{TableName}:{TableComment}  列数:{TableColumnInfos.Count}")]
    public class TableInfo : IDbContextEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// テーブル名
        /// </summary>
        public string TableName { get; set; } = "";

        /// <summary>
        /// テーブル日本語名
        /// </summary>
        public string TableComment { get; set; } = "";

        /// <summary>
        /// 列情報
        /// </summary>
        public virtual ICollection<TableColumnInfo> TableColumnInfos { get; set; } = new List<TableColumnInfo>();
    }
}
