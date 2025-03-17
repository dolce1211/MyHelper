
using MyHelper.Domain;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Helpers;
using MyHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.Repositories;
using Microsoft.VisualStudio.OLE.Interop;
using MyHelper.Infrastructure;
using System.Linq.Expressions;

namespace MyHelper.Dialogs
{
    public partial class frmLauncher : Form
    {
        private System.Windows.Forms.Timer _mouseCheckTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer _debounceTimer = new System.Windows.Forms.Timer();
        private List<LauncherItem> _launcherItems = new List<LauncherItem>();
        private bool _suspendHideForm = false;
        public event EventHandler<PassToFloatingDialogrEventArgs>? PassToFloatingDialog;

        private CacheManager _cacheManager = null;
        private ILauncherDataRepository _launcherDataRepository = null!;
        public frmLauncher(CacheManager cacheManager)
        {
            InitializeComponent();
            _cacheManager = cacheManager;
            // デバウンスタイマーの設定
            _debounceTimer.Interval = 300; // ミリ秒
            _debounceTimer.Tick += OnDebounceTimerTick;


        }



        /// <summary>
        /// マウスカーソルの場所に表示する
        /// </summary>
        public void ShowFloatingDialog()
        {
            _suspendHideForm = false;
            chkFix.Checked = false;

            // Repositoryのインスタンスを取得
            _launcherDataRepository = RepositoryFactory.CreateLauncherDataRepository();
            // ランチャー情報読みなおし

            _launcherItems = _launcherDataRepository.GetAllAsync(orderby: t => t.Order).Result!;
            if (_launcherItems == null)
                _launcherItems = new List<LauncherItem>();

            CreateLstLauncher();

            // フォームをマウスカーソルの位置までもってくる
            this.BringFormToMouseCursor(50, 50);

            _mouseCheckTimer.Start();
            _mouseCheckTimer.Tick += this._mouseCheckTimer_Tick;
            var allItems = _cacheManager.GetAllItemStrings();
            allItems.AddRange(_launcherItems.Select(n => n.Title));
            AutoCompleteTextBox1.SetItems(allItems, RepositoryFactory.CreateAutoCompleteTextRepository());//, new frmLauncherCustomOrderEditor());
            AutoCompleteTextBox1.Text = "";

            txtFilter.Text = "";

            // ちょっと時間をあけないとちゃんとフォーカスしてくれないようだ
            Task.Run(async () =>
            {
                await Task.Delay(100);
                this.Invoke(new Action(() =>
                {
                    this.BringWindowToFront();
                    AutoCompleteTextBox1.Focus();
                }));
            });

            this.Show();
            this.TopMost = true;
        }

        private void _mouseCheckTimer_Tick(object? sender, EventArgs e)
        {
            if (_suspendHideForm) return;
            if (chkFix.Checked) return;
            if (ContextMenuStrip1.Visible) return; // 右クリックメニューが出ている間は閉じない
            if (!this.IsCursorWithinDistance(20))
            {
                // マウスカーソルが離れたら隠す
                this.Hide();

                _mouseCheckTimer.Stop();
            }
        }

        /// <summary>
        /// 直接指定テキストが確定した
        /// </summary>
        private void AutoCompleteTextBox1_AutoCompleteTextBoxValidated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AutoCompleteTextBox1.Text)) return;

            var item = _launcherItems?.FirstOrDefault(n => n.Title == AutoCompleteTextBox1.Text);
            if (item != null)
            {
                // ランチャーのアイテムだ。開く。
                OpenItem(item);
                AutoCompleteTextBox1.Text = "";
            }
            else
            {
                // ランチャーのアイテムではない。フローティングダイアログに渡す
                PassToFloatingDialog?.Invoke(this, new PassToFloatingDialogrEventArgs(AutoCompleteTextBox1.Text));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 追加
            OpenEditDialog(null);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            編集ToolStripMenuItem.PerformClick();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void MoveSelectedItem(int direction)
        {
            if (lstLauncher.SelectedItem == null) return;
            int selectedIndex = lstLauncher.SelectedIndex;
            var items = new List<LauncherItem>(_launcherItems ?? new List<LauncherItem>());
            LauncherItem? itemToMove = null;

            if (direction == -1 && selectedIndex > 0)
            {
                // 一つ前のインデックスに移動
                itemToMove = items[selectedIndex];
                items.RemoveAt(selectedIndex);
                items.Insert(selectedIndex - 1, itemToMove);
            }
            else if (direction == 1 && selectedIndex < items.Count - 1)
            {
                // 一つ後のインデックスに移動
                itemToMove = items[selectedIndex];
                items.RemoveAt(selectedIndex);
                items.Insert(selectedIndex + 1, itemToMove);
            }

            if (itemToMove != null)
            {
                _launcherItems = items;
                CreateLstLauncher();
                lstLauncher.SelectedItem = itemToMove;
            }
        }

        private void CreateLstLauncher()
        {
            if (_launcherItems == null) return;
            string src = txtFilter.Text.ToLower();
            lstLauncher.Items.Clear();
            lstLauncher.Items.AddRange(_launcherItems.Where(n =>
            {
                if (!string.IsNullOrEmpty(src))
                {
                    return n.Title.ToLower().Contains(src);
                }
                return true;
            }).ToArray());
        }

        private void frmLauncher_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                // 閉じるタイミングで非同期で再保存する
                Task.Run(async () =>
                {
                    await TrySaveAsync();
                    _launcherDataRepository?.Dispose();
                });
            }
        }
        private async Task<bool> TrySaveAsync()
        {
            if (_launcherItems == null) return false;
            foreach (var item in _launcherItems.Enumerate())
                item.Value.Order = item.Index + 1;

            try
            {
                await _launcherDataRepository.UpdateAllItemsAsync(_launcherItems);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"データ登録中に予期しないエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        private void lstLauncher_DoubleClick(object sender, EventArgs e)
        {
            開くToolStripMenuItem.PerformClick();
        }

        private void lstLauncher_Format(object sender, ListControlConvertEventArgs e)
        {
            var item = e.Value as LauncherItem;
            if (item == null) return;

            e.Value = item.Title;
        }

        private void lstLauncher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                開くToolStripMenuItem.PerformClick();
            }
        }

        private void lstLauncher_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 右クリックでも選択させる
                int index = lstLauncher.IndexFromPoint(e.Location);
                if (index >= 0 && index < lstLauncher.Items.Count)
                {
                    lstLauncher.SelectedIndex = index;
                }
            }
        }

        private void OnDebounceTimerTick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            CreateLstLauncher();
        }

        private bool OpenEditDialog(LauncherItem? item)
        {
            bool isNew = item == null;

            _suspendHideForm = true;
            this.TopMost = false;
            try
            {
                TrySaveAsync().Wait();
                using (var frm = new frmLauncherEdit(_launcherDataRepository, item))
                {
                    var result = frm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        // 登録成功
                        var newItem = frm.Result;
                        if (newItem != null)
                        {
                            if (item != null)
                            {
                                // 編集
                                item.Title = newItem.Title;
                                item.Path = newItem.Path;
                                item.LauncherType = newItem.LauncherType;
                                newItem = item;
                            }
                            else
                            {
                                _launcherItems.Add(newItem);
                            }
                            TrySaveAsync().Wait();
                            CreateLstLauncher();
                            lstLauncher.SelectedItem = newItem;
                        }
                    }
                }
            }
            finally
            {
                this.TopMost = true;
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    if (this.TopMost)
                        _suspendHideForm = false;
                });
            }
            return true;
        }

        /// <summary>
        /// ランチャーのアイテムを開く
        /// </summary>
        private bool OpenItem(LauncherItem item)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (item.LauncherType == LauncherType.URL)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = item.Path,
                        UseShellExecute = true
                    });
                }
                else if (item.LauncherType == LauncherType.FolderPath)
                {
                    Process.Start("explorer.exe", item.Path);
                }
                else if (item.LauncherType == LauncherType.FilePath)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = item.Path,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(item.Path),
                        UseShellExecute = true
                    });
                }
                lstLauncher.SelectedItem = item;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{item.Path}\n\n{ex.Message}");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            return false;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstLauncher.SelectedItem == null) return;
            var item = lstLauncher.SelectedItem as LauncherItem;
            if (item == null) return;
            // 開く
            OpenItem(item);
        }

        private void 削除XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstLauncher.SelectedItem == null) return;
            var item = lstLauncher.SelectedItem as LauncherItem;
            if (item == null) return;

            _suspendHideForm = true;
            try
            {
                string msg = $"「{item.Title}」\n\nこの項目を削除します。\nよろしいですか？";
                if (MessageBox.Show(this, msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                _launcherDataRepository.DeleteByIdAsync(item.Id).Wait();
                _launcherItems = _launcherDataRepository.GetAllAsync().Result!;
                CreateLstLauncher();
            }
            finally
            {
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    _suspendHideForm = false;
                });
            }
        }

        private void 編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 編集
            if (lstLauncher.SelectedItem == null) return;
            var item = lstLauncher.SelectedItem as LauncherItem;
            if (item == null) return;
            OpenEditDialog(item);
        }


    }

    public class PassToFloatingDialogrEventArgs : EventArgs
    {
        public string Text { get; }

        public PassToFloatingDialogrEventArgs(string text)
        {
            Text = text;
        }
    }
}