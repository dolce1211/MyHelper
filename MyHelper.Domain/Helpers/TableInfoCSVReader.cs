using CsvHelper;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Helpers
{
    public class TableInfoCSVReader
    {
        private string _csvPath = "";

        /// <summary>
        /// tables.csvを読み込み、List<TableInfo>に変換して返す
        /// </summary>
        /// <paramref name="csvPath">tables.csvのパス</paramref>
        /// <paramref name="lastWriteTime">前回読み込んだ時のtables.csvの更新日時</paramref>
        /// <paramref name="forceUpdate">trueの場合、prevDateTimeに関わらず再取得する</paramref>
        /// <returns></returns>
        public static (List<TableInfo>?,DateTime) ReadCSV(string csvPath, DateTime lastWriteTime, bool forceUpdate)
        {
            if (!System.IO.File.Exists(csvPath) || System.IO.Path.GetExtension(csvPath).ToLower().Trim() != ".csv")
                return (null, default);

            var file = new System.IO.FileInfo(csvPath);
            if (!forceUpdate && file.LastWriteTime.ToString("yyyyMMddHHmmss") == lastWriteTime.ToString("yyyyMMddHHmmss"))
                //前回読み込んだものと同じなら再取得しない
                return (null, default);

            var tableInfos = new List<TableInfo>();
            var tableColumnInfos = new List<TableColumnInfo>();

            using (var reader = new StreamReader(csvPath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    var tableName = record.table_name;
                    var tableComment = record.table_comment;
                    var columnName = record.column_name;
                    var isPrimaryKey = int.Parse( record.is_primary_key);
                    var dataType = record.data_type;
                    var isNullable = bool.Parse(record.is_nullable);
                    var columnComment = record.column_comment;

                    var tableInfo = tableInfos.FirstOrDefault(t => t.TableName == tableName);
                    if (tableInfo == null)
                    {
                        tableInfo = new TableInfo
                        {
                            TableName = tableName,
                            TableComment = tableComment,
                            TableColumnInfos = new List<TableColumnInfo>()
                        };
                        tableInfos.Add(tableInfo);
                    }
                    
                    var tableColumnInfo = new TableColumnInfo
                    {
                        ColumnName = columnName,
                        ColumnComment = columnComment,
                        IsPrimaryKey = isPrimaryKey,
                        DataType = dataType,
                        IsNullable = isNullable,
                    };
                    tableInfo.TableColumnInfos.Add(tableColumnInfo);
                    tableColumnInfos.Add(tableColumnInfo);
                }
            }
            return (tableInfos, file.LastWriteTime);
        }
    }
}
