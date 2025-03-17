using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyHelper.Domain
{


    public class AutoCompleteTextBox : TextBox
    {
        public event EventHandler? AutoCompleteTextBoxValidated;

        private ListBox _suggestionsListBox = new ListBox();
        private System.Windows.Forms.Timer _debounceTimer = new System.Windows.Forms.Timer();
        private List<string>? _items;
        private Dictionary<string, AutoCompleteTextRecord> _recordsHash = new Dictionary<string, AutoCompleteTextRecord>();
        private List<string> _recentRecords = new List<string>();
        private bool _suspendDebounceTimer = false;
        private ICustomOrderEditor? _customOrderEditor = null;
        private IAutoCompleteTextRepository? _autoCompleteRepository = null;
        public AutoCompleteTextBox()
        {
            _debounceTimer.Interval = 300;
            _debounceTimer.Tick += OnDebounceTimerTick;

            // リストボックスの設定
            _suggestionsListBox.Visible = false;
            _suggestionsListBox.TabStop = false;
            _suggestionsListBox.FormattingEnabled = true;

            // イベントハンドラの登録
            this.TextChanged += OnTextChangedInternal;
            _suggestionsListBox.Click += OnSuggestionSelected;
            this.LostFocus += OnMyselfLostFocus;
            _suggestionsListBox.LostFocus += OnMyselfLostFocus;

            _suggestionsListBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ValidateSelectedItem();
                }
            };
            


            Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(100);
                    var parentForm = this.FindForm();

                    if (parentForm != null)
                    {
                        if (!parentForm.Controls.Contains(_suggestionsListBox))
                        {
                            // suggestionsListBoxを自分の子供に加える。
                            // コンストラクタのタイミングだとまだParent.Controlsのインスタンス化が完了していないので
                            // 少しタイミングをずらして行う
                            parentForm.Controls.Add(_suggestionsListBox);
                        }
                        break;
                    }
                } while (true);

                
            });
        }

        /// <summary>
        /// 全データを渡してもらう
        /// </summary>
        /// <param name="items">全データ</param>
        /// <param name="customOrderEditor">候補の表示順調整用インタフェース</param>
        public void SetItems(List<string> items, IAutoCompleteTextRepository autoCompleteRepository, ICustomOrderEditor? customOrderEditor = null)
        {
            _items = items;
            _customOrderEditor = customOrderEditor;
            _autoCompleteRepository = autoCompleteRepository;
            _suspendDebounceTimer = true;
            Task.Run(async () =>
            {
                await Task.Delay(100);
                _suspendDebounceTimer = false;

                // 使用履歴をロード
                _recordsHash = new Dictionary<string, AutoCompleteTextRecord>();
                var records = new List<AutoCompleteTextRecord>();
                if (_autoCompleteRepository != null)
                {
                    records = await _autoCompleteRepository.GetAllAsync();
                    if (records == null) records = new List<AutoCompleteTextRecord>();
                    records.ForEach(n =>
                    {
                        if (!_recordsHash.ContainsKey(n.Word!))
                        {
                            _recordsHash.Add(n.Word!, n);
                        }
                    });
                }
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //Debug.WriteLine($"OnKeyDown:{e.KeyCode}");
            if (e.KeyCode == Keys.Down)
            {
                if (_suggestionsListBox.Visible)
                {
                    _suggestionsListBox.Focus();
                    if (_suggestionsListBox.Items.Count > 1)
                    {
                        _suggestionsListBox.SelectedIndex = 1;
                    }
                }
                else if (_recentRecords?.Count > 0)
                {
                    // 直近使用履歴があるならそれを表示
                    if (ShowSuggestionsListBox(AutoCompleteListBoxSuggestionMode.Selecting, _recentRecords))
                    {
                        _suggestionsListBox.Focus();
                        _suggestionsListBox.SelectedIndex = 0;
                    }
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (_suggestionsListBox.Visible)
                {
                    ValidateSelectedItem();
                }
                else if (string.IsNullOrEmpty(this.Text))
                {
                    ValidateSelectedItem();
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Text = "";
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                // Ctrl+Cでいま選択されているものをクリップボードへ
                if (_suggestionsListBox.Visible && _suggestionsListBox.SelectedItem != null)
                {
                    Clipboard.SetText(_suggestionsListBox.SelectedItem.ToString());
                }
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// ドロップダウン用のリストをテキストボックスの場所へ
        /// </summary>
        /// <returns></returns>
        private Point GetDropdownLocation()
        {
            var parentForm = this.FindForm();
            var screenPoint = this.PointToScreen(new Point());
            // フォームのクライアント座標に変換
            var locationInForm = parentForm.PointToClient(screenPoint);
            var point = new Point();
            point.X = locationInForm.X;
            point.Y += this.Height + locationInForm.Y;
            return point;
        }

        private void OnDebounceTimerTick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            UpdateSuggestions();
        }

        private void OnMyselfLostFocus(object? sender, EventArgs e)
        {
            if (!_suggestionsListBox.Focused && !this.Focused)
            {
                _suggestionsListBox.Visible = false;
            }
        }

        private void OnSuggestionSelected(object? sender, EventArgs e)
        {
            if (_suggestionsListBox.SelectedItem != null)
            {
                this.Text = _suggestionsListBox.SelectedItem.ToString();
                _suggestionsListBox.Visible = false;
                this.Select(this.Text!.Length, 0);
            }
        }

        private void OnTextChangedInternal(object? sender, EventArgs e)
        {
            if (_suspendDebounceTimer)
            {
                return;
            }
            if (!this.Enabled) return;
            Console.WriteLine("OnTextChangedInternal");
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void SuggestionsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (_suggestionsListBox.SelectedIndex == 0)
                {
                    // リストボックスの選択がトップの状態で上を押すとテキストボックスにフォーカスを合わせる
                    this.Focus();
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (_suggestionsListBox.Visible)
                {
                    ValidateSelectedItem();
                }
                else
                {
                    UpdateSuggestions();
                }
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                // Ctrl+Cでいま選択されているものをクリップボードへ
                if (_suggestionsListBox.Visible && _suggestionsListBox.SelectedItem != null)
                {
                    Clipboard.SetText(_suggestionsListBox.SelectedItem.ToString());
                }
            }
        }

        private void SuggestionsListBox_MouseDown(object sender, MouseEventArgs e)
        {
            ValidateSelectedItem();
        }

        private async void UpdateSuggestions()
        {
            if (string.IsNullOrEmpty(this.Text.Trim())) return;
            if (_items == null) return;

            // スペース区切りがあればAND条件で検索する
            var inputArray = this.Text.Replace("　", " ").Split(new string[] { " " }, StringSplitOptions.None);
            if (inputArray.Length >= 1)
            {
                // 非同期でデータをフィルタリング
                var matches = await Task.Run(() =>
                {
                    return _items.Where(n =>
                    {
                        // スペース区切りがあればAND条件で検索する
                        bool ret = true;
                        foreach (var input in inputArray)
                        {
                            if (!n.ToLower().Contains(input.ToLower()))
                            {
                                ret = false;
                            }
                        }
                        return ret;
                    }).ToList();
                });

                if (matches != null)
                {
                    // 開始一致があればトップに持ってくる
                    var leftMatches = matches.Where(n => n.ToLower().Trim().StartsWith(this.Text.ToLower().Trim()));
                    if (leftMatches != null)
                    {
                        leftMatches = leftMatches.Reverse();
                        foreach (var match in leftMatches)
                        {
                            matches.Remove(match);
                            matches.Insert(0, match);
                        }
                    }

                    // 独自のソートルールを依存注入されているならそれを反映させる
                    if (_customOrderEditor != null)
                    {
                        matches = _customOrderEditor.CustomOrder(inputArray, matches);
                    }

                    // 使用回数が多いものをトップに持ってくる
                    // 条件:三回以上使用履歴のある単語を抽出し、使用回数が多いもの順に表示順をあげていく
                    var frequentlyUsedList = matches.Where(n =>
                    {
                        if (_recordsHash.ContainsKey(n) && _recordsHash[n].Count >= 3)
                        {
                            // 使用履歴が3回以上あるものは優先して上にあげる
                            return true;
                        }
                        return false;
                    })
                    .Select(n => _recordsHash[n])
                    .OrderBy(n => n.Count);

                    foreach (var frequentlyUsed in frequentlyUsedList)
                    {
                        if (matches.Contains(frequentlyUsed.Word!))
                        {
                            matches.Remove(frequentlyUsed.Word!);
                            matches.Insert(0, frequentlyUsed.Word!);
                        }
                    }

                    // 50個まで
                    matches = matches.Take(50).ToList();
                }

                // リストボックスの更新と表示
                ShowSuggestionsListBox(AutoCompleteListBoxSuggestionMode.Writing, matches);
            }
            else
            {
                _suggestionsListBox.Visible = false;
            }
        }

        /// <summary>
        /// 引数のitemsの内容でベロを出します。
        /// </summary>
        /// <param name="items"></param>
        private bool ShowSuggestionsListBox(AutoCompleteListBoxSuggestionMode mode, List<string>? items)
        {
            // If items?.Count == 0 Then Return False
            _suggestionsListBox.Tag = mode;
            _suggestionsListBox.DataSource = items;
            _suggestionsListBox.Location = GetDropdownLocation();
            _suggestionsListBox.Size = new Size(this.Width, 100);
            _suggestionsListBox.Visible = true;
            _suggestionsListBox.BringToFront();
            return true;
        }

        /// <summary>
        /// アイテムが確定した
        /// </summary>
        private void ValidateSelectedItem()
        {
            _suspendDebounceTimer = true;
            if (_suggestionsListBox.Visible)
            {
                this.Text = _suggestionsListBox.SelectedItem.ToString();
            }
            _suspendDebounceTimer = false;
            _suggestionsListBox.Visible = false;
            this.SelectionStart = this.Text!.Length;
            this.Focus();

            var myText = this.Text;
            Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(myText)) return false;
                // 使用履歴に追加
                AutoCompleteTextRecord? record = null;
                if (_recordsHash.ContainsKey(myText))
                {
                    record = _recordsHash[myText];
                }
                else
                {
                    record = new AutoCompleteTextRecord { Word = myText };
                    _recordsHash.Add(myText, record);
                }
                record.Count += 1;
                // 使用履歴にセーブ
                if(_autoCompleteRepository != null)
                {
                    await _autoCompleteRepository.AddOrUpdateAsync(record.Id, record);
                }

                // 最近の使用履歴に追加
                if (_recentRecords.Contains(myText))
                {
                    _recentRecords.Remove(myText);
                }
                // 先頭に追加し、直近のものほど上に上がってくるようにする
                _recentRecords.Insert(0, myText);
                return true;
            });

            // 確定イベントを発火させる
            AutoCompleteTextBoxValidated?.Invoke(this, new EventArgs());
        }

        private void SuggestionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mode = (AutoCompleteListBoxSuggestionMode)_suggestionsListBox.Tag;
            if (mode == AutoCompleteListBoxSuggestionMode.Selecting)
            {
                // 決まった候補から選択しているなら、選択内容をテキストに反映する
                _suspendDebounceTimer = true;
                try
                {
                    this.Text = _suggestionsListBox.SelectedItem.ToString();
                }
                finally
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(100);
                        _suspendDebounceTimer = false;
                    });
                }
            }
        }
    }

    /// <summary>
    /// 結果のソート順をカスタマイズする機能を注入するためのインタフェース
    /// </summary>
    public interface ICustomOrderEditor
    {
        /// <summary>
        /// 入力内容に一致した候補を並び変える
        /// </summary>
        /// <param name="inputArray">半角スペースで区切った入力内容</param>
        /// <param name="matches"></param>
        /// <returns></returns>
        List<string> CustomOrder(string[] inputArray, List<string> matches);
    }

    /// <summary>
    /// suggestionListBoxの挙動
    /// </summary>
    internal enum AutoCompleteListBoxSuggestionMode
    {
        none = -1,

        // いまテキストに候補を書いている
        Writing,

        // 決まった候補から選択している
        Selecting
    }


}