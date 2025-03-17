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
    /// なんでも検索の使用履歴。
    /// 3回以上使用されたものは優先的に上にもっていく
    /// </summary>
    [Table("AutoCompleteTextRecords")]
    [DebuggerDisplay("{Word} :使用回数{Count} 回")]
    public class AutoCompleteTextRecord: IDbContextEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// なんでも検索の単語
        /// </summary>
        public string Word { get; set; } = "";

        /// <summary>
        /// これまでの使用回数
        /// </summary>
        public int Count { get; set; }
    }
}
