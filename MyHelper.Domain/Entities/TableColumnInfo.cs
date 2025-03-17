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
    /// 単一のテーブル列情報を表すエンティティ
    /// </summary>
    [Table("TableColumnInfos")]
    [DebuggerDisplay("{ColumnName}:{ColumnComment}  Nullable:{IsNullable}")]
    public class TableColumnInfo : IDbContextEntity
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 外部キー→TableInfo.Id
        /// </summary>
        public int Table_Id { get; set; }

        /// <summary>
        /// プライマリキーなら1
        /// </summary>
        public int IsPrimaryKey { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; } = "";

        /// <summary>
        /// 列日本語名
        /// </summary>
        public string ColumnComment { get; set; } = "";

        /// <summary>
        /// データ型
        /// </summary>
        public string DataType { get; set; } = "";  

        /// <summary>
        /// Nullableならtrue
        /// </summary>
        public bool IsNullable { get; set; } = false;

        public string IsPrimaryKeyString
        {
            get
            {
                return IsPrimaryKey == 1 ? "○" : "";
            }
        }
        public string IsNullableString
        {
            get
            {
                return IsNullable ? "○" : "";
            }
        }
        /// <summary>
        /// テーブル名
        /// </summary>

        public virtual TableInfo TableInfo { get; set; } = null;
    }
}
