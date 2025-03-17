using MyHelper.Domain;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Helpers;
using MyHelper.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MyHelper.Domain.Helpers.Extensions;


namespace MyHelper.Domain
{

    /// <summary>
    /// クリップボードで取れたキーワードのタイプ
    /// </summary>
    public enum KeyWordType
    {
        none = -1,

        /// <summary>
        /// テーブル名
        /// </summary>
        TableName,

        /// <summary>
        /// RedMineのチケットNo
        /// </summary>
        TicketNumber,
    }



    /// <summary>
    /// クリップボードから抽出された情報を表すエンティティ
    /// </summary>
    public class ExtractedInformations
    {
        /// <summary>
        /// OriginalStringから抽出されたキーワード
        /// </summary>
        public List<ExtractedKeywordBase> Keywords { get; set; } = new List<ExtractedKeywordBase>();

        /// <summary>
        /// ひっかかった文字列
        /// </summary>
        public string OriginalString { get; set; } = "";
    }

    /// <summary>
    /// クリップボードから抽出された単一の情報を表すエンティティ
    /// </summary>
    public class ExtractedKeywordBase
    {
        /// <summary>
        /// クリップボードから抽出されたキーワード
        /// </summary>
        public string KeywordString { get; set; } = "";

        /// <summary>
        /// 取得したKeywordの種類。
        /// SQLIDの場合、AsIsとToBeで複数入る可能性がある。
        /// </summary>
        public KeyWordType KeywordType { get; set; } = KeyWordType.none;

        /// <summary>
        /// 表示用文字列を返します。
        /// </summary>
        /// <returns></returns>
        public string DisplayString(CacheManager pathCacheManager, int mode = 0)
        {
            string ret = KeywordString;
            switch (KeywordType)
            {
                case KeyWordType.TableName:
                    var tableinfo = pathCacheManager.TryGetTableInfo(this.KeywordString);
                    if (tableinfo != null)
                        ret = $"{tableinfo.TableName}  ({tableinfo.TableComment.Trim()})";
                    break;

                default:
                    //それ以外は素直に表示
                    break;
            }
            return ret;
        }
    }




    /// <summary>
    /// 各種キャッシュを管理するクラス
    /// </summary>
    public class CacheManager
    {
        private bool _isCachingOnGoing = false;

        /// <summary>
        /// 現在キャッシュ処理中ならtrueを返す
        /// </summary>
        public bool IsCachingOngoing
        {
            get { return _isCachingOnGoing; }
        }

        /// <summary>
        /// テーブル定義を引っ張るためのハッシュ情報
        /// </summary>
        private Dictionary<string, TableInfo> _tableHash = null!;

        /// <summary>
        /// 各種ファイルのパスを一括でキャッシュする
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task<bool> CacheQueryPathsAsync(Setting setting)
        {
            _isCachingOnGoing = true;
            try
            {
                List<Task> tasks = new List<Task>();

                //ファイルパスを並列でキャッシュする
                for (int i = 0; i <= 0; i++)
                {
                    int x = i;
                    //tasks.Add(Task.Run(() =>
                    //{
                    //    string filePath = "";
                    //    string searchPattern = "";
                    //    switch (x)
                    //    {
                    //        case 2:
                    //            //詳細設計書
                    //            filePath = setting?.DetailExcelFolderPath;
                    //            if (!string.IsNullOrEmpty(filePath))
                    //            {
                    //                filePath += "\\x.txt"; //他と合わせるためフォルダパスではなくファイルパスの体にする
                    //            }
                    //            searchPattern = "*詳細設計*.xls*";
                    //            break;
                    //    }
                    //    if (!string.IsNullOrEmpty(filePath))
                    //    {
                    //        Dictionary<string, string> pathHash = CacheQueryPathsInternal(x, filePath, searchPattern);
                    //        if (pathHash == null) pathHash = new Dictionary<string, string>();

                    //        switch (x)
                    //        {
                    //            case 2:
                    //                //詳細設計書
                    //                _detailExcelPathHash = pathHash;
                    //                break;
                    //        }
                    //    }
                    //    return true;
                    //}));
                }

                // すべてのタスクが完了するのを待つ
                await Task.WhenAll(tasks);

                return true;
            }
            finally
            {
                _isCachingOnGoing = false;
            }
        }

        /// <summary>
        /// テーブル情報をキャッシュ
        /// </summary>
        /// <param name="tables"></param>
        /// <returns></returns>
        public bool CreateTableCache(List<TableInfo>? tables)
        {
            if (tables == null)
                return false;
            _tableHash = new Dictionary<string, TableInfo>();
            foreach (TableInfo table in tables)
            {
                if (!_tableHash.ContainsKey(table.TableName.ToLower()))
                {
                    _tableHash.Add(table.TableName.ToLower(), table);
                }
                if (!_tableHash.ContainsKey(table.TableComment.ToLower()))
                {
                    _tableHash.Add(table.TableComment.ToLower(), table);
                }
            }
            return true;
        }

        /// <summary>
        /// 全アイテムを取得します。
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllItemStrings()
        {
            List<string> ret = new List<string>();
            if (_tableHash != null)
            {
                ret.AddRange(_tableHash.Keys.Select(n => n.ToLower()));
            }
            ret = ret.Distinct().ToList();
            ret = ret.OrderBy(n => n).ToList();
            ret = ret.Where(n => !string.IsNullOrEmpty(n)).ToList();
            return ret;
        }

        /// <summary>
        /// クリップボードから抽出したキーワードの種類を返します。
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<KeyWordType> GetKeywordType(string keyword)
        {
            List<KeyWordType> ret = new List<KeyWordType>();
            string pattern = @"#\d+";
            if (Regex.Matches(keyword, pattern).Count > 0)
            {
                //RedMineのチケットNoだ
                ret.Add(KeyWordType.TicketNumber);
            }
            if (_tableHash?.ContainsKey(keyword) == true)
            {
                //テーブル名だ
                ret.Add(KeyWordType.TableName);
            }


            return ret;
        }

        /// <summary>
        /// クリップボードに格納された文字列からこのツールで扱える情報を抽出して返します。
        /// </summary>
        /// <param name="clipboardStr"></param>
        /// <returns></returns>
        public ExtractedInformations TryExtractKeywords(string clipboardStr)
        {
            List<string> extractedList = new List<string>();

            //クリップボードの中にほしい情報があるか探す
            List<string> tmp = clipboardStr.ExtractKeywords(GetAllItemStrings());
            extractedList.AddRange(tmp);

            List<ExtractedKeywordBase> keywords = new List<ExtractedKeywordBase>();
            //必要な情報を抽出しおえたら、内容に応じて解釈を加える
            if (extractedList.Count > 0)
            {
                foreach (string extracted in extractedList)
                {
                    var extractedKeyword = new ExtractedKeywordBase();

                    string keywordString = extracted;
                    List<KeyWordType> keywordTypes = GetKeywordType(keywordString);
                    if (keywordTypes.Count > 0)
                    {
                        switch (true)
                        {
                            case bool _ when keywordTypes.Contains(KeyWordType.TableName):
                                //テーブル名

                                break;

                            default:
                                //それ以外は普通に格納する
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(keywordString))
                    {
                        if (!keywords.Any(n => n.KeywordString == keywordString)) //重複除外
                        {
                            extractedKeyword.KeywordType = keywordTypes.FirstOrDefault();
                            extractedKeyword.KeywordString = keywordString;

                            keywords.Add(extractedKeyword);
                        }
                    }
                }
            }

            return new ExtractedInformations()
            {
                OriginalString = clipboardStr,
                Keywords = keywords
            };
        }

        public TableInfo? TryGetTableInfo(string keyword)
        {
            if (keyword == null)
                return null;
            if (_tableHash == null)
                return null;
            if (_tableHash.ContainsKey(keyword.ToLower()))
            {
                return _tableHash[keyword.ToLower()];
            }
            return null;
        }
    }
}