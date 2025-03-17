using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MyHelper.Dialogs;
using MyHelper.Domain;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Helpers;
using MyHelper.Domain.Repositories;
using MyHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyHelper.Infrastructure;
using CsvHelper;
using MyHelper.Infrastructure.MyHelperDbContext;


namespace MyHelper
{
    public partial class frmMain : Form
    {
        private KeyboardMouseMonitor? _keyboardMouseMonitor = null;
        private CacheManager? _cacheManager = null;

        private FloatingDialog? _floatingDialog = null;
        /// <summary>
        /// SQL監視ダイアログ
        /// </summary>
        private frmSQLWatcher? _frmSQLWatcher = null;

        /// <summary>
        /// 簡易ランチャー
        /// </summary>
        private frmLauncher? _frmLauncher = null;
        /// <summary>
        /// てーぶる定義ダイアログ
        /// </summary>
        private List<frmTableDefinition> _frmTableDefinitions = new List<frmTableDefinition>();
        
        private const int WM_COPYDATA = 0x004A;
        private Setting _setting;



        public frmMain()
        {
            //設定情報を取得
            using var dataRepository = RepositoryFactory.CreateSettingDataRepository();
            _setting = dataRepository.GetSingleAsync().Result;

            InitializeComponent();
        }

        /// <summary>
        /// メッセージを監視する.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_COPYDATA)
                {
                    if (_frmSQLWatcher != null)
                    {
                        COPYDATASTRUCT cds = Marshal.PtrToStructure<COPYDATASTRUCT>(m.LParam);
                        string receivedMessage = Marshal.PtrToStringUni(cds.lpData, cds.cbData / 2);
                        if (receivedMessage != null)
                        {
                            //SQL監視ダイアログにメッセージを送る
                            _frmSQLWatcher.TryAddMessage(receivedMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                base.WndProc(ref m);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            _cacheManager = new CacheManager();
            bool hookMouse = true;

            if (Debugger.IsAttached)
            {
                // デバッグ時にマウスのフックを行うとステップ実行時にカクついてうざいので無効化できるようにする
                var ret = MessageBox.Show(this, "マウスのフックを行うことでマウスボタン両押しを検知できますが、これを有効にするとデバッグ時のステップ実行がもたつくことがあります。\n\nマウスのフックを行いますか？", "開発者向け確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                hookMouse = ret == DialogResult.Yes;
            }

            _keyboardMouseMonitor = new KeyboardMouseMonitor();
            _keyboardMouseMonitor.SetHook(true, hookMouse);
            _keyboardMouseMonitor.ControlCDoublePressed += _keyboardMouseMonitor_ControlCDoublePressed;
            _keyboardMouseMonitor.BothMouseButtonClicked += _keyboardMouseMonitor_BothMouseButtonClicked;
            _keyboardMouseMonitor.BothControlKeyPressed += _keyboardMouseMonitor_BothControlKeyPressed;
            Extensions.PrepareKeyboardMouseMonitor(_keyboardMouseMonitor);

            _floatingDialog = new FloatingDialog(_cacheManager);
            _floatingDialog.PassToFrmTableDefinition += (s, e) =>
            {
                Show_frmTableDefinition(e?.TableInfo);
            };
            _frmSQLWatcher = new frmSQLWatcher(_cacheManager);
            _frmSQLWatcher.PassToFrmTableDefinition += (s, e) =>
            {
                Show_frmTableDefinition(e?.TableInfo);
            };

            _frmLauncher = new frmLauncher(_cacheManager);
            _frmLauncher.PassToFloatingDialog += (ss, ee) => { _frmLauncher.Hide(); TryShowFloatingDialog(ee.Text); };


            
            // 初回はテーブル情報を読み込む
            _ = Task.Run(() => RefreshTableInfos(false));
            InitializeDialogs(new Form[] { _floatingDialog, _frmSQLWatcher, _frmLauncher });
        }


        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_keyboardMouseMonitor != null)
            {
                _keyboardMouseMonitor.Unhook();
                _keyboardMouseMonitor.ControlCDoublePressed -= _keyboardMouseMonitor_ControlCDoublePressed;
                _keyboardMouseMonitor.BothMouseButtonClicked -= _keyboardMouseMonitor_BothMouseButtonClicked;
                _keyboardMouseMonitor.BothControlKeyPressed -= _keyboardMouseMonitor_BothControlKeyPressed;
            }
     
        }

        /// <summary>
        /// Ctrl+Cが2回押された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _keyboardMouseMonitor_ControlCDoublePressed(object? sender, EventArgs e)
        {
            if (_floatingDialog != null && _floatingDialog.Visible) return;

            string clipboardStr = Clipboard.GetText().Trim().ToLower();

            if (_setting.OpenValidPathByCtrlCC)
            {
                if (Directory.Exists(clipboardStr))
                {
                    // 有効なファイルやフォルダのパスが選択されている状態でCtrl+CCが押された場合、
                    // そのパスを開く
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = clipboardStr,
                        UseShellExecute = true
                    });
                    return;
                }
                else if (File.Exists(clipboardStr))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = clipboardStr,
                        UseShellExecute = true
                    });
                }
            }
            TryShowFloatingDialog(clipboardStr);
        }

        /// <summary>
        /// 左右のマウスボタンが同時に押された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _keyboardMouseMonitor_BothMouseButtonClicked(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (!_setting.EnableLauncherByMouseButton) return;
                ShowLauncherDialog();
            }));
        }

        /// <summary>
        /// 左右のCtrlキーが同時に押された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _keyboardMouseMonitor_BothControlKeyPressed(object? sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (!_setting.EnableLauncherByCtrlKeys) return;
                ShowLauncherDialog();
            }));
        }

        private void btnSQLWatch_Click(object sender, EventArgs e)
        {
            if (_frmSQLWatcher != null && _frmSQLWatcher.Visible)
            {
                _frmSQLWatcher.BringWindowToFront();
                return;
            }

            _frmSQLWatcher!.Show();
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new frmSetting())
            {
                if (frm.ShowDialog(this) != DialogResult.OK) return;

                var result = frm.Result;
                if (result != null)
                {
                    using var repo = RepositoryFactory.CreateSettingDataRepository();
                    _setting = repo.GetSingleAsync().Result;
                }
            }
        }

        private void 閉じるXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// 各種ダイアログは親フォームが閉じるまでは殺さないようにする
        /// </summary>
        /// <param name="dialogs"></param>
        private void InitializeDialogs(IEnumerable<Form> dialogs)
        {
            foreach (var frm in dialogs)
            {
                frm.FormClosing += (s, e) =>
                {
                    if (e.CloseReason != CloseReason.FormOwnerClosing)
                    {
                        e.Cancel = true;
                        frm.Hide();
                    }
                };
            }

        }


        /// <summary>
        /// テーブル定義ダイアログを表示する
        /// </summary>
        /// <param name="tableInfo"></param>
        private void Show_frmTableDefinition(TableInfo? tableInfo)
        {
            if (tableInfo == null)
                return;

            var existing = _frmTableDefinitions.FirstOrDefault(f => f.TableInfo.TableName == tableInfo.TableName);
            if (existing != null)
            {
                existing.BringWindowToFront();
                return;
            }

            var frm = new frmTableDefinition(_cacheManager!, tableInfo);
            frm.Show();
            _frmTableDefinitions.Add(frm);
            frm.FormClosed += (s, e) =>
            {
                _frmTableDefinitions.Remove(frm);
            };
            frm.CloseAllFrmTableDefinition += (s, e) =>
            {
                //今開いているすべてのfrmTableDefinitionを閉じる
                var dels = new List<frmTableDefinition>();
                dels.AddRange(_frmTableDefinitions);
                foreach (var f in dels)
                {
                    f.Close();
                }
            };
        }

        /// <summary>
        /// クリップボードの文字列を元にフローティングダイアログを表示する
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool TryShowFloatingDialog(string str)
        {
            var extractedKeywords = _cacheManager!.TryExtractKeywords(str);
            if (extractedKeywords != null && extractedKeywords.Keywords != null && extractedKeywords.Keywords.Count > 0)
            {
                _floatingDialog!.ShowFloatingDialog(extractedKeywords);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ランチャーダイアログを表示する
        /// </summary>
        private void ShowLauncherDialog()
        {
            if (_frmLauncher == null) return;

            if (_frmLauncher.Visible)
            {
                _frmLauncher.BringWindowToFront();
                return;
            }

            _frmLauncher.ShowFloatingDialog();
        }


        #region "TableInfo登録作業"


        private void tablescsv再読込ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("tables.csvの再読み込みを行います。\n\nよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            _ = Task.Run(()=> RefreshTableInfos(true));
        }

        /// <summary>
        /// TableInfo,TableColumnInfoをcsvからDBに登録する
        /// </summary>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        private async Task RefreshTableInfos(bool forceUpdate)
        {
            // tables.csvを読み込んでDBに格納するas    
            await ReadCSVAndRegisterTables(forceUpdate);

            // tableinfoの内容をキャッシュする
            using var tableRepo = RepositoryFactory.CreateTableInfoRepository();
            var tableinfos = await tableRepo.GetAsync();
            _cacheManager?.CreateTableCache(tableinfos);
        }
        private async Task ReadCSVAndRegisterTables(bool forceUpdate)
        { 
            using var settingRepo = RepositoryFactory.CreateSettingDataRepository();
            var setting = await settingRepo.GetSingleAsync();

            var csvPath = @"datas\tables.csv";
            (List<TableInfo>?, DateTime) tuple = default;
            try
            {
                tuple = TableInfoCSVReader.ReadCSV(csvPath, setting.LastWrittenTimeForTables, forceUpdate);
            }
            catch (IOException ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("テーブル情報の取得に失敗しました。\r\n\r\ntable.csvをエクセル等で開いていないか確認してください。","table.csv読み取り失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }));
            }
            catch(Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("テーブル情報の取得に失敗しました");
                }));               
            }
            
            var tableinfos = tuple.Item1; //CSVから取得した情報
            var lastWriteTime = tuple.Item2; // CSVファイルの最終更新時間
            if (tableinfos != null)
            {
                var prevcolor = this.MenuStrip1.BackColor;
                prevcolor = this.BackColor;
                this.Invoke(new Action(() =>
                {
                    this.BackColor = System.Drawing.Color.Red;
                    this.MenuStrip1.BackColor = System.Drawing.Color.Red;
                }));
                try
                {

                    //テーブル情報が取れたなら、DBに保存する
                    MyHelperDBContext.Debug_DisableLog = true; //　重くなるからDbContextのログを無効化
                    var flg = false;
                    using var tableRepo = RepositoryFactory.CreateTableInfoRepository();
                    using var tableColumnRepo = RepositoryFactory.CreateTableColumnInfoRepository();
                    using var transaction = await tableRepo.TransactionManager.BeginTransactionAsync();
                    
                    try
                    {
                        // テーブル全削除
                        await tableRepo.DeleteAllAsync();
                        // テーブル全登録
                        await tableRepo.UpdateAllItemsAsync(tableinfos);                        
                        // コミット
                        await transaction.CommitAsync();
                        flg = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("テーブル情報の登録に失敗しました");
                        }));

                    }
                    finally
                    {
                        if (flg)
                        {
                            // setttingにtables.csvの最終更新時間を登録する
                            setting.LastWrittenTimeForTables = lastWriteTime;
                            await settingRepo.AddOrUpdateAsync(setting.Id, setting);
                        }
                    }
                }

                finally
                {
                    this.Invoke(new Action(() =>
                    {
                        this.BackColor = prevcolor;
                        this.MenuStrip1.BackColor = prevcolor;
                    }));
                    MyHelperDBContext.Debug_DisableLog = false;
                }
            }
        }
        #endregion
    }
}