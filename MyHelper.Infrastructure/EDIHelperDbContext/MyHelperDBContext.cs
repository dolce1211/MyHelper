using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyHelper.Domain.Entities;

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using Microsoft.VisualStudio.TextManager.Interop;

namespace MyHelper.Infrastructure.MyHelperDbContext
{
    public class MyHelperDBContext : DbContext
    {
        public static bool Debug_DisableLog = false;

        /// <summary>
        /// 設定情報
        /// </summary>
        public DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// 簡易ランチャーのアイテム
        /// </summary>
        public DbSet<LauncherItem> LauncherItems { get; set; }

        /// <summary>
        /// 自動補完テキストの選択履歴履歴
        /// </summary>
        public DbSet<AutoCompleteTextRecord> AutoCompleteTextRecords { get; set; }

        /// <summary>
        /// テーブル情報
        /// </summary>
        public DbSet<TableInfo> TableInfos { get; set; }

        /// <summary>
        /// テーブルの列情報
        /// </summary>
        public DbSet<TableColumnInfo> TableColumnInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var databasePath = "MyHelper.db";
#if DEBUG
            databasePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\MyHelper\\MyHelper.db");
#endif

            optionsBuilder
                .UseSqlite($"Data Source={databasePath}")
                .LogTo(Write_Log, LogLevel.Information);
        }

        private void Write_Log(string log)
        {
            if (Debug_DisableLog)
                return;
            Debug.WriteLine(log);

            return;
            var stackTrace = new StackTrace(true); // true を指定してファイル情報を取得
            var stackFrames = stackTrace.GetFrames();
            var methodDetails = stackFrames?.Select(frame =>
                        {
                            var method = frame.GetMethod();
                            var methodName = method?.Name;
                            if (methodName == "MoveNext")
                                //asyncメソッドかTaskと思われる
                                methodName = ExtractOriginalMethodName(method?.DeclaringType?.FullName);

                            return new
                            {
                                MethodName = methodName,
                                FileName = frame.GetFileName(),
                                LineNumber = frame.GetFileLineNumber()
                            };
                        }
                        ).Where(n => !string.IsNullOrEmpty(n.FileName))
                        .ToList();

            if (methodDetails != null)
            {
                var detail = methodDetails.FirstOrDefault()!;
                Debug.WriteLine($"Method: {detail.MethodName}, File: {detail.FileName}, Line: {detail.LineNumber}");
            }
        }

        private string ExtractOriginalMethodName(string? declaringTypeFullName)
        {
            if (string.IsNullOrEmpty(declaringTypeFullName))
                return "";
            var match = System.Text.RegularExpressions.Regex.Match(declaringTypeFullName, @"<(.+)>d__\d+");
            return match.Success ? match.Groups[1].Value : declaringTypeFullName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableInfo>()
                .ToTable("TableInfos")
                .HasKey(p => p.Id);
            modelBuilder.Entity<TableColumnInfo>()
                .ToTable("TableColumnInfos")
                .HasKey(p => p.Id);

            // TableInfo.TableNameにindex付与、重複不許可
            modelBuilder.Entity<TableInfo>()
                .HasIndex(t => t.TableName)
                .HasDatabaseName("IX_TableInfo_TableName")
                .IsUnique();

            // TableInfoとTableColumnInfoを外部キーで紐付け
            modelBuilder.Entity<TableColumnInfo>()
                .HasOne(c => c.TableInfo)
                .WithMany(t => t.TableColumnInfos)
                .HasForeignKey(c => c.Table_Id)
                .IsRequired(false);
        }
    }
}