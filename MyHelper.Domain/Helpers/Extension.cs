using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;


namespace MyHelper.Domain.Helpers
{
    public static class Extensions
    {
        private static KeyboardMouseMonitor? _keyboardMouseMonitor;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(nint hWnd);

        public static void PrepareKeyboardMouseMonitor(KeyboardMouseMonitor keyboardMouseMonitor)
        {
            _keyboardMouseMonitor = keyboardMouseMonitor;
        }

        // WM_SETREDRAWの定義
        private const int WM_SETREDRAW = 0xB;

        // SendMessage関数のインポート
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern nint SendMessage(HandleRef hWnd, int msg, nint wParam, nint lParam);

        /// <summary>
        /// マウスカーソルがウィンドウから指定ピクセル以内にあるかを判定、外れていたらfalseを返します。
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static bool IsCursorWithinDistance(this Form form, int distance)
        {
            if (form == null) return true;
            if (form.IsDisposed) return true;

            // 現在のマウスカーソルの位置を取得
            Point cursorPos = Cursor.Position;
            Rectangle windowBounds = form.Bounds;
            int leftBound = windowBounds.Left - distance;
            int rightBound = windowBounds.Right + distance;
            int topBound = windowBounds.Top - distance;
            int bottomBound = windowBounds.Bottom + distance;
            //マウスカーソルがウィンドウから指定ピクセル内にあるか判定
            return cursorPos.X >= leftBound && cursorPos.X <= rightBound &&
                   cursorPos.Y >= topBound && cursorPos.Y <= bottomBound;
        }

        /// <summary>
        /// フォームの位置をマウスカーソルに持ってきます。
        /// </summary>
        /// <param name="form"></param>
        /// <param name="xmargin">x方向のマージン</param>
        /// <param name="ymargin">y方向のマージン</param>
        /// <returns></returns>
        public static bool BringFormToMouseCursor(this Form form, int xmargin = 0, int ymargin = 0)
        {
            if (form == null) return false;
            Point cursorPos = Cursor.Position;
            form.StartPosition = FormStartPosition.Manual;

            // フォームの領域のトップとレフトの計算
            int top = cursorPos.Y - ymargin;
            int left = cursorPos.X - xmargin;

            // スクリーン情報を取得
            Screen screen = Screen.FromPoint(cursorPos);

            // フォームがスクリーンの範囲内に収まるように調整
            if (top < screen.WorkingArea.Top)
            {
                top = screen.WorkingArea.Top;
            }

            if (left < screen.WorkingArea.Left)
            {
                left = screen.WorkingArea.Left;
            }

            if (top + form.Height > screen.WorkingArea.Bottom)
            {
                top = screen.WorkingArea.Bottom - form.Height;
            }

            if (left + form.Width > screen.WorkingArea.Right)
            {
                left = screen.WorkingArea.Right - form.Width;
            }

            // フォームの位置を設定
            form.Location = new Point(left, top);
            return true;
        }

        /// <summary>
        /// コントロールの描画を一時停止
        /// </summary>
        /// <param name="ctrl"></param>
        public static void SuspendDrawing(this Control ctrl, bool unhookWhileSuspended = false)
        {
            if (unhookWhileSuspended && _keyboardMouseMonitor != null)
            {
                //重い処理でマウスががたつくことがあるので、このタイミングで一時的にフックを解除する
                _keyboardMouseMonitor.Unhook();
            }
            SendMessage(new HandleRef(ctrl, ctrl.Handle), WM_SETREDRAW, nint.Zero, nint.Zero);
        }

        /// <summary>
        /// コントロールの描画処理を再開
        /// </summary>
        /// <param name="ctrl"></param>
        public static void ResumeDrawing(this Control ctrl, bool unhookWhileSuspended = false)
        {
            SendMessage(new HandleRef(ctrl, ctrl.Handle), WM_SETREDRAW, new nint(1), nint.Zero);
            ctrl.Refresh(); // コントロールを再描画
            if (unhookWhileSuspended && _keyboardMouseMonitor != null)
            {
                //SuspendDrawingで解除したフックを再開する
                _keyboardMouseMonitor.ReSetHook();
            }
        }

        // SQLのキーワードリスト
        private static readonly string[] sqlKeywords = { "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", "INNER", "LEFT", "RIGHT", "JOIN", "ON", "ORDER", "BY", "GROUP", "HAVING" };

        /// <summary>
        /// フォームを手前にもっていく
        /// </summary>
        /// <param name="form"></param>
        public static void BringWindowToFront(this Form form)
        {
            if (form == null) return;
            form.Handle.BringWindowToFront();
        }

        /// <summary>
        /// フォームを手前にもっていく
        /// </summary>
        /// <param name="hwnd"></param>
        public static void BringWindowToFront(this nint hwnd)
        {
            if (hwnd == nint.Zero) return;
            SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// フォームを手前にもっていく
        /// </summary>
        /// <param name="hwnd"></param>
        public static void BringWindowToFront(this int hwnd)
        {
            if (hwnd == 0) return;
            SetForegroundWindow(new nint(hwnd));
        }

        /// <summary>
        /// filePathの中をserachStringを検索して行数を返します。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static List<int> SearchAndFindLineNumber(this string filePath, string searchString)
        {
            var ret = new List<int>();

            if (!File.Exists(filePath)) return ret;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    int currentLine = 0;

                    while (reader.Peek() >= 0)
                    {
                        line = reader.ReadLine()!;
                        currentLine++;

                        if (line!.Contains(searchString))
                        {
                            ret.Add(currentLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラー: " + ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// RichTextBoxに色を付けてSQLを表示する
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="sql"></param>
        public static void AddSQLAndColorize(this RichTextBox richTextBox, string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;

            richTextBox.SuspendDrawing();
            try
            {
                // RichTextBoxの内容をクリア
                richTextBox.Clear();

                richTextBox.Text = sql;

                richTextBox.SuspendLayout();

                // SQLクエリを単語ごとに解析し、色を設定
                var words = sql.Split(new char[] { ' ', '\t', '\r', '\n', '(', ')', ',', ';', '\'' }, StringSplitOptions.RemoveEmptyEntries);

                int startIndex = 0;

                foreach (var word in words)
                {
                    // 現在の単語のインデックスを検索
                    startIndex = richTextBox.Find(word, startIndex, RichTextBoxFinds.None);

                    if (startIndex >= 0)
                    {
                        // SQLキーワードの場合
                        if (sqlKeywords.Contains(word.ToUpper()))
                        {
                            richTextBox.Select(startIndex, word.Length);
                            richTextBox.SelectionColor = Color.Blue;
                        }

                        // 文字列リテラルの場合（シングルクォートで囲まれた部分）
                        if (word.StartsWith("'") && word.EndsWith("'"))
                        {
                            richTextBox.Select(startIndex, word.Length);
                            richTextBox.SelectionColor = Color.Green;
                        }

                        // コメントの場合（-- で始まる行）
                        if (word.StartsWith("--"))
                        {
                            richTextBox.Select(startIndex, word.Length);
                            richTextBox.SelectionColor = Color.Gray;
                        }
                    }
                    // 次の単語に進む
                    startIndex += word.Length;
                }

                // テキストの自動フォーマットを有効化
                richTextBox.ResumeLayout();
            }
            finally
            {
                richTextBox.ResumeDrawing();
            }
        }

        /// <summary>
        /// 半角に変換して返します。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToHankaku(this string str)
        {
            var sb = new StringBuilder();

            foreach (var ch in str)
            {
                // 全角英数字・記号の範囲を確認
                if (ch >= '\uFF01' && ch <= '\uFF5E')
                {
                    // 半角に変換
                    sb.Append((char)(ch - 0xFEE0));
                }
                else if (ch == '\u3000')
                {
                    // 全角スペースを半角スペースに変換
                    sb.Append(' ');
                }
                else
                {
                    // それ以外はそのまま
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// いい感じにエンコードを判定してファイルを読み込みます。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string TryReadFileWithEncodingDetection(this string filePath)
        {
            if (!File.Exists(filePath)) return "";
            var encoding = DetectEncoding(filePath);
            return File.ReadAllText(filePath, encoding);
        }

        private static Encoding DetectEncoding(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Encoding.UTF8;
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var bom = new byte[4];
                fs.Read(bom, 0, 4);

                // UTF-8 with BOM
                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                {
                    return Encoding.UTF8;
                }

                // UTF-16 LE BOM
                if (bom[0] == 0xFF && bom[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }

                // UTF-16 BE BOM
                if (bom[0] == 0xFE && bom[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
            }
            var utf8Text = File.ReadAllText(filePath, Encoding.UTF8);
            if (utf8Text.Contains("�")) //文字化け判定の簡易チェック
            {
                return Encoding.Default;
            }
            else
            {
                return Encoding.UTF8;
            }
        }


        /// <summary>
        /// sql文からテーブル名を抽出する
        /// NugetからTSQL Parserあたりを持ってくれば楽なんだが、使っていいかわからんので泥臭く自力で…。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<TableInfoFromSQL> ExtractTableNames(this string sql)
        {
            // コメントアウト部分を除外
            sql = RemoveSqlComments(sql);

            // SQL文を大文字に変換して余分な空白を削除
            sql = sql.ToUpper();
            sql = Regex.Replace(sql, "\\s+", " ");

            // 抽出したテーブル情報を格納するリスト
            var tableInfos = new List<TableInfoFromSQL>();

            // SQLキーワードのリスト
            var sqlKeywords = new string[] { "SELECT", "FROM", "WHERE", "GROUP BY", "ORDER BY", "HAVING", "UNION", "JOIN", "INNER JOIN", "LEFT JOIN", "LEFT OUTER JOIN", "RIGHT JOIN", "FULL JOIN", "CROSS JOIN", "ON" };

            // ヘルパーメソッド: 複数のキーワードの中で最初に見つかった位置を返す
            int FindFirstIndexOfAny(string s, string[] substrings)
            {
                int firstIndex = -1;
                foreach (var substr in substrings)
                {
                    int idx = s.IndexOf(" " + substr + " ");
                    if (idx >= 0)
                    {
                        if (firstIndex == -1 || idx < firstIndex)
                        {
                            firstIndex = idx;
                        }
                    }
                }
                return firstIndex;
            }

            // FROM句からテーブル名とエイリアスを抽出
            var fromPattern = "\\bFROM\\b\\s+";
            var fromMatches = Regex.Matches(sql, fromPattern);
            foreach (Match fromMatch in fromMatches)
            {
                int startIndex = fromMatch.Index + fromMatch.Length;
                var remainingSql = sql.Substring(startIndex);

                // 次のキーワードまでを取得
                int endIndex = FindFirstIndexOfAny(remainingSql, sqlKeywords);
                if (endIndex == -1)
                {
                    endIndex = remainingSql.Length;
                }
                var fromClause = remainingSql.Substring(0, endIndex).Trim();
                // カンマで分割してテーブル定義を取得
                var tableDefs = fromClause.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tmptableDef in tableDefs)
                {
                    var tableDef = tmptableDef.Replace("(", "").Replace(")", "");
                    if (string.IsNullOrEmpty(tableDef)) continue;

                    // テーブル名とエイリアスを抽出
                    var tokens = tableDef.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length > 0)
                    {
                        var tableNameInfo = new TableInfoFromSQL { CRUD = CRUDInfoFromSQL.R };
                        if (tokens.Length >= 2 && sqlKeywords.Contains(tokens[0]))
                        {
                            //多分 SELECT FROM (SELECT ～～みたいな構文をやってる
                            //tokens = tokens;
                        }
                        else
                        {
                            tableNameInfo.TableName = tokens[0];
                            if (tokens.Length > 1 && !tokens[1].StartsWith("--"))
                            {
                                tableNameInfo.Prefix = tokens[1];
                            }
                            else
                            {
                                tableNameInfo.Prefix = "";
                            }
                            // 重複を避けるため、既に存在しない場合のみ追加
                            if (!tableInfos.Any(t => t.TableName == tableNameInfo.TableName && t.Prefix == tableNameInfo.Prefix))
                            {
                                tableInfos.Add(tableNameInfo);
                            }
                        }
                    }
                }
            }

            // JOIN句からテーブル名とエイリアスを抽出
            var joinPattern = "\\b(?:INNER\\s+JOIN|LEFT\\s+JOIN|RIGHT\\s+JOIN|FULL\\s+JOIN|CROSS\\s+JOIN|JOIN)\\b\\s+([A-Z0-9_]+)(?:\\s+([A-Z0-9_]+))?";
            var joinMatches = Regex.Matches(sql, joinPattern);
            foreach (Match joinMatch in joinMatches)
            {
                var tableNameInfo = new TableInfoFromSQL { CRUD = CRUDInfoFromSQL.R };
                tableNameInfo.TableName = joinMatch.Groups[1].Value;

                if (joinMatch.Groups.Count > 2 && !string.IsNullOrEmpty(joinMatch.Groups[2].Value) && joinMatch.Groups[2].Value != "ON")
                {
                    tableNameInfo.Prefix = joinMatch.Groups[2].Value;
                }
                else
                {
                    tableNameInfo.Prefix = "";
                }
                // 重複を避けるため、既に存在しない場合のみ追加
                if (!tableInfos.Any(t => t.TableName == tableNameInfo.TableName && t.Prefix == tableNameInfo.Prefix))
                {
                    tableInfos.Add(tableNameInfo);
                }
            }

            // INSERT文からテーブル名を抽出（エイリアスはなし）
            var insertPattern = "INSERT\\s+INTO\\s+([A-Z0-9_]+)";
            var insertMatch = Regex.Match(sql, insertPattern);
            if (insertMatch.Success)
            {
                var tableNameInfo = new TableInfoFromSQL { CRUD = CRUDInfoFromSQL.C };
                tableNameInfo.TableName = insertMatch.Groups[1].Value;
                tableNameInfo.Prefix = "";
                var sameTables = tableInfos.Where(t => t.TableName == tableNameInfo.TableName && t.CRUD == CRUDInfoFromSQL.R).ToList();
                if (sameTables?.Count > 0)
                {
                    //おそらくSelectInsertだ。
                    sameTables.ForEach(t => tableInfos.Remove(t));
                }
                tableInfos.Add(tableNameInfo);
            }

            // UPDATE文からテーブル名を抽出（エイリアスはなし）
            var updatePattern = "UPDATE\\s+([A-Z0-9_]+)";
            var updateMatch = Regex.Match(sql, updatePattern);
            if (updateMatch.Success)
            {
                var tableNameInfo = new TableInfoFromSQL { CRUD = CRUDInfoFromSQL.U };
                tableNameInfo.TableName = updateMatch.Groups[1].Value;
                tableNameInfo.Prefix = "";
                var sameTables = tableInfos.Where(t => t.TableName == tableNameInfo.TableName && t.CRUD == CRUDInfoFromSQL.R).ToList();
                if (sameTables?.Count > 0)
                {
                    //おそらくSelectInsertだ。
                    sameTables.ForEach(t => tableInfos.Remove(t));
                }
                tableInfos.Add(tableNameInfo);
            }

            // DELETE文からテーブル名を抽出（エイリアスはなし）
            var deletePattern = "DELETE\\s+FROM\\s+([A-Z0-9_]+)";
            var deleteMatch = Regex.Match(sql, deletePattern);
            if (deleteMatch.Success)
            {
                var tableNameInfo = new TableInfoFromSQL { CRUD = CRUDInfoFromSQL.D };
                tableNameInfo.TableName = deleteMatch.Groups[1].Value;
                tableNameInfo.Prefix = "";
                var sameTables = tableInfos.Where(t => t.TableName == tableNameInfo.TableName && t.CRUD == CRUDInfoFromSQL.R).ToList();
                if (sameTables?.Count > 0)
                {
                    //おそらくSelectInsertだ。
                    sameTables.ForEach(t => tableInfos.Remove(t));
                }
                tableInfos.Add(tableNameInfo);
            }

            return tableInfos;
        }


        /// <summary>
        /// SQL文からコメントを除去します。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static string RemoveSqlComments(string sql)
        {
            // -- で始まるコメントを除去
            sql = Regex.Replace(sql, "--.*$", "", RegexOptions.Multiline);
            // /* ... */ のコメントを除去
            sql = Regex.Replace(sql, "/\\*.*?\\*/", "", RegexOptions.Singleline);
            return sql;
        }

        /// <summary>
        /// 文字列からキーワードを抽出します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static List<string> ExtractKeywords(this string text, IEnumerable<string> keywords)
        {
            var extractedKeywords = new List<string>();

            // 各キーワードをチェックし、原文に含まれているか確認
            foreach (var keyword in keywords)
            {
                if (text.Contains(keyword))
                {
                    extractedKeywords.Add(keyword);
                }
            }
            if (extractedKeywords.Any(n => n == text))
            {
                //そのものズバリなら「含む」アイテムは無視してそれだけを拾う
                extractedKeywords.Clear();
                extractedKeywords.Add(text);
            }
            else
            {
                // "コードマスタ"と"コードマスタ履歴"みたいに、部分一致してしまう場合は長い方を優先する
                var orderedList = new List<string>();
                orderedList.AddRange( extractedKeywords.OrderByDescending(k => k.Length).ToList());
            
                foreach (var keyword in orderedList)
                {
                    var prevKeyword = orderedList.FirstOrDefault(n => n != keyword && n.Contains(keyword));
                    if (prevKeyword != null)
                    {
                        var prevIndexes = text.AllIndexOf(prevKeyword);
                        var currentIndexes = text.AllIndexOf(keyword);
                        if(prevIndexes.Count== currentIndexes.Count)
                        {
                            var tmpIndex = 0;
                            if (prevIndexes.All(prevIndex =>
                                {
                                    var currentIndex = currentIndexes[tmpIndex];
                                    var ln = prevIndex + prevKeyword.Length - 1;
                                    tmpIndex++;

                                    if (currentIndex >= ln && currentIndex < ln)
                                    {
                                        return true;
                                    }
                                    return false;
                                }));
                            extractedKeywords.Remove(keyword);
                        }
                    }
                    prevKeyword = keyword;
                }
            }

            //出現順に並び変え
            extractedKeywords= extractedKeywords.OrderBy(n=> text.IndexOf(n)).ToList(); 

            // # +数字はチケットNoとみなす
            var ticketNumbers = text.ExtractTicketNumbers();
            if (ticketNumbers?.Count > 0)
            {
                extractedKeywords.AddRange(ticketNumbers);
            }
            return extractedKeywords;
        }

        /// <summary>
        /// strの中に一致するkeywordが複数ある場合、全てのインデックスを返します。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List <int> AllIndexOf(this string str, string keyword)
        {
            var ret = new List<int>();
            int index = -1;
            do
            {
                index = str.IndexOf(keyword,index+1);
                if (index >= 0)
                    ret.Add(index);
                else
                    break;
            } while (true);

            return ret;
        }
        /// <summary>
        /// 全角文字が含まれているかを判定します。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsDoubleByteCharacters(this string input)
        {
            foreach (var c in input)
            {
                // Unicode値が255を超える場合、それは全角（2バイト文字）の可能性が高い
                if (c > 255)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 英数字以外の記号が含まれているかを判定します。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsNonAlphanumeric(this string input)
        {
            // 正規表現を使用して英数字以外の記号を検索
            var regex = new Regex("[^a-zA-Z0-9]");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Redmineのチケット番号を抽出します。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> ExtractTicketNumbers(this string text)
        {
            // 正規表現パターン: #の後に1つ以上の数字
            var pattern = "#\\d+";
            var matches = Regex.Matches(text, pattern);

            // マッチ結果をリストに変換
            var result = new List<string>();
            foreach (Match match in matches)
            {
                result.Add(match.Value);
            }

            return result;
        }

        public static void ReleaseComObject(this object? obj)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                for (int index = 1; index <= 3; index++) //高速でアクティブシートが切り替わるとReleaseComObjectでエラーが発生することがあるので3度までリトライする
                {
                    try
                    {
                        if (obj != null)
                        {
                            Marshal.ReleaseComObject(obj);
                            obj = null;
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        if (index == 3)
                        {
                            obj = null;
                        }
                        else
                        {
                            Thread.Sleep(20);
                        }
                    }
                    finally
                    {
                        GC.Collect();
                    }
                }
            }
        }

        public static bool IsFloatEqualsTo(this float a, float b, float epsilon = 0.001f)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static DiffInfo GetDiff(this string str1, string str2)
        {
            var tokens1 = Tokenize(str1);
            var tokens2 = Tokenize(str2);
            var lcs = GetLCS(tokens1, tokens2);

            var diffInfo = new DiffInfo();
            var commonBuilder = new StringBuilder();

            int i = 0;
            int j = 0;

            foreach (var tokenInfo in lcs)
            {
                var token = tokenInfo.Token;
                var tokenStartIndex = tokenInfo.StartIndex;

                // str1側の追加部分
                var added = new List<TokenInfo>();
                while (i < tokens1.Count && tokens1[i].Token != token)
                {
                    added.Add(tokens1[i]);
                    i++;
                }

                // str2側の削除部分
                var removed = new List<TokenInfo>();
                while (j < tokens2.Count && tokens2[j].Token != token)
                {
                    removed.Add(tokens2[j]);
                    j++;
                }

                // 追加・削除があればDiffInfoItemに追加
                if (added.Count > 0 || removed.Count > 0)
                {
                    var item = new DiffInfoItem();
                    if (added.Count > 0)
                    {
                        item.AddedString = string.Concat(added.Select(t => t.Token));
                        item.StartIndex = added[0].StartIndex; // 追加部分の開始インデックス
                    }
                    if (removed.Count > 0)
                    {
                        item.RemovedString = string.Concat(removed.Select(t => t.Token));
                        if (added.Count == 0)
                        {
                            item.StartIndex = removed[0].StartIndex; // 削除部分の開始インデックス
                        }
                    }
                    diffInfo.Items.Add(item);
                }

                // 共通部分を追加
                commonBuilder.Append(token);
                i++;
                j++;
            }

            // 残りの部分を追加
            if (i < tokens1.Count || j < tokens2.Count)
            {
                var item = new DiffInfoItem();
                if (i < tokens1.Count)
                {
                    item.AddedString = string.Concat(tokens1.Skip(i).Select(t => t.Token));
                    item.StartIndex = tokens1[i].StartIndex;
                }
                if (j < tokens2.Count)
                {
                    item.RemovedString = string.Concat(tokens2.Skip(j).Select(t => t.Token));
                    if (i >= tokens1.Count)
                    {
                        item.StartIndex = tokens1!.LastOrDefault()!.StartIndex;
                    }
                }
                diffInfo.Items.Add(item);
            }

            diffInfo.CommonString = commonBuilder.ToString();
            return diffInfo;
        }

        /// <summary>
        /// 文字列をトークンに分割します。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static List<TokenInfo> Tokenize(string input)
        {
            var tokens = new List<TokenInfo>();
            // 漢字: \u4E00-\u9FFF, ひらがな: \u3040-\u309F, カタカナ: \u30A0-\u30FF
            var pattern = "([\u4E00-\u9FFF]+)|([\u3040-\u309F]+)|([\u30A0-\u30FF]+)|([^一-龥ぁ-んァ-ン]+)";
            var matches = Regex.Matches(input, pattern);
            int currentIndex = 0;

            foreach (Match m in matches)
            {
                var token = m.Value;
                int tokenStart = input.IndexOf(token, currentIndex);
                if (tokenStart >= 0)
                {
                    tokens.Add(new TokenInfo(token, tokenStart));
                    currentIndex = tokenStart + token.Length;
                }
                else
                {
                    // フォールバック: 見つからない場合は現在のインデックスを使用
                    tokens.Add(new TokenInfo(token, currentIndex));
                    currentIndex += token.Length;
                }
            }
            return tokens;
        }

        /// <summary>
        /// 最長共通部分列（LCS）を取得します。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static List<TokenInfo> GetLCS(List<TokenInfo> a, List<TokenInfo> b)
        {
            int m = a.Count;
            int n = b.Count;
            var dp = new int[m + 1, n + 1];

            // DPテーブルの構築
            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        dp[i, j] = 0;
                    }
                    else if (a[i - 1].Token == b[j - 1].Token)
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            // LCSの復元
            var lcs = new List<TokenInfo>();
            int x = m;
            int y = n;
            while (x > 0 && y > 0)
            {
                if (a[x - 1].Token == b[y - 1].Token)
                {
                    lcs.Insert(0, a[x - 1]);
                    x--;
                    y--;
                }
                else if (dp[x - 1, y] > dp[x, y - 1])
                {
                    x--;
                }
                else
                {
                    y--;
                }
            }
            return lcs;
        }

        /// <summary>
        /// pythonのenumerateのようにインデックスを付与して返します。
        /// C#のSelect関数の順序とは逆になるが、enumerate関数とそろえて第一要素をindex、第二要素をvalueとします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<(int Index, T Value)> Enumerate<T>(this IEnumerable<T> item)
        {
            return item.Select((value, index) => (index, value));
        }

        public class DiffInfo
        {
            /// <summary>
            /// 変更部分
            /// </summary>
            public List<DiffInfoItem> Items { get; set; } = new List<DiffInfoItem>();

            /// <summary>
            /// 共通部分
            /// </summary>
            public string CommonString { get; set; } = "";
        }

        public class DiffInfoItem
        {
            /// <summary>
            /// 元の文字列の何文字目から始まる単語か
            /// </summary>
            public int StartIndex { get; set; } = 0;

            /// <summary>
            /// 追加文字列
            /// </summary>
            public string AddedString { get; set; } = "";

            /// <summary>
            /// 削除文字列
            /// </summary>
            public string RemovedString { get; set; } = "";
        }

        public class TokenInfo
        {
            public string Token { get; set; }
            public int StartIndex { get; set; } = 0;

            public TokenInfo(string token, int startIndex)
            {
                Token = token;
                StartIndex = startIndex;
            }
        }

        public class TableInfoFromSQL
        {
            /// <summary>
            /// CRUD情報。単一のはずだけど一応念のためにコレクションで持って置く
            /// </summary>
            public CRUDInfoFromSQL CRUD { get; set; }

            /// <summary>
            /// テーブル名
            /// </summary>
            public string TableName { get; set; } = "";

            /// <summary>
            /// プレフィックス
            /// </summary>
            public string Prefix { get; set; } = "";
        }

        public enum CRUDInfoFromSQL
        {
            C,
            R,
            U,
            D
        }
    }
}