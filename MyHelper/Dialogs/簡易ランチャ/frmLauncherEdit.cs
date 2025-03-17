using MyHelper.Domain;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    public partial class frmLauncherEdit : Form
    {
        /// <summary>
        /// テキスト入力のデバウンスタイマー
        /// </summary>
        private System.Windows.Forms.Timer _debounceTimer = new System.Windows.Forms.Timer();

        /// <summary>
        /// テーブル定義を管理するオブジェクト
        /// </summary>
        private ILauncherDataRepository _launcherDataRepository = null!;

        /// <summary>
        /// 新規ならnull、編集なら編集元の情報が入る
        /// </summary>
        private LauncherItem? _prevLauncherItem = null;

        private LauncherItem? _result;

        public frmLauncherEdit(ILauncherDataRepository launcherDataRepository, LauncherItem? launcherItem)
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            _prevLauncherItem = launcherItem; // 追加モードならnullが渡る
            _launcherDataRepository = launcherDataRepository;
            this.rbFolder.Tag = LauncherType.FolderPath;
            this.rbFile.Tag = LauncherType.FilePath;
            this.rbURL.Tag = LauncherType.URL;

            // デバウンスタイマーの設定
            _debounceTimer.Interval = 300; // ミリ秒
            _debounceTimer.Tick += OnDebounceTimerTick;
        }

        /// <summary>
        /// 結果
        /// </summary>
        public LauncherItem? Result
        {
            get { return _result; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            
            if (!rbURL.Checked)
            {
                string defText = await TryGetDefaultTitleString();
                if (string.IsNullOrEmpty(defText))
                {
                    //パス欄がうまくいってない
                    MessageBox.Show($"{lblPathCaption.Text} が正しく入力されているか確認してください");
                    this.lblPathCaption.Focus();
                    return;
                }
            }
            if (string.IsNullOrEmpty(this.txtTitle.Text))
            {
                MessageBox.Show("タイトルを入力してください");
                return;
            }
            LauncherType launcherType = TryGetLauncherType();
            if (launcherType == LauncherType.none)
            {
                //こんなことはあり得ないはず
                MessageBox.Show("形式を選択してください");
                return;
            }

          
            Expression<Func<LauncherItem, bool>> predicate = n => n.Title.ToLower() == this.txtTitle.Text.Trim().ToLower() && (_prevLauncherItem == null || n.Id != this._prevLauncherItem.Id);
                
            var prevExists = _launcherDataRepository.Any(predicate).Result;
            if (prevExists)
            {
                MessageBox.Show($"{this.txtTitle.Text}\n\nこのタイトルは既に登録されています。\n他のタイトルを使用してください。");
                return;
            }
            


            this._result = new LauncherItem()
            {
                LauncherType = launcherType,
                Title = this.txtTitle.Text.Trim(),
                Path = this.txtPath.Text.Trim()
            };

            this.DialogResult = DialogResult.OK;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            string title = "";
            string filter = "";
            string defExt = "";
            bool isFolder = false;

            if (rbFile.Checked)
            {
                filter = "すべてのファイル (*.*)|*.*";
                defExt = "";
                title = "ファイルの場所を指定してください";
            }
            else if (rbFolder.Checked)
            {
                title = "フォルダの場所を指定してください";
                isFolder = true;
            }

            if (string.IsNullOrEmpty(title)) return;

            // 規定の拡張子を設定
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = title;
                if (isFolder)
                {
                    //フォルダ指定
                    openFileDialog.ValidateNames = false;
                    openFileDialog.CheckFileExists = false;
                    openFileDialog.CheckPathExists = true;
                    openFileDialog.FileName = "このフォルダを選択";
                    if (System.IO.Directory.Exists(txtPath.Text))
                    {
                        openFileDialog.InitialDirectory = txtPath.Text;
                    }
                }
                else
                {
                    //ファイル指定
                    openFileDialog.DefaultExt = defExt;
                    openFileDialog.Filter = filter;
                    if (System.IO.File.Exists(txtPath.Text))
                    {
                        openFileDialog.FileName = txtPath.Text;
                        openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(txtPath.Text);
                    }
                }

                // ダイアログを表示し、ユーザーがファイルを選択した場合にそのファイルパスを表示
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (isFolder)
                    {
                        //フォルダ指定
                        txtPath.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    }
                    else
                    {
                        //ファイル指定
                        txtPath.Text = openFileDialog.FileName;
                    }
                }
            }
        }

        private void chkEditTitle_CheckedChanged(object sender, EventArgs e)
        {
            this.txtTitle.Enabled = true;
            if (!rbURL.Checked)
            {
                this.txtTitle.Enabled = this.chkEditTitle.Checked;
            }
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void frmLauncherEdit_Load(object sender, EventArgs e)
        {
            if (this._prevLauncherItem == null)
            {
                //新規モードだ
                this.chkEditTitle.Checked = false;

                this.txtTitle.Text = "";
                this.txtPath.Text = "";
                this.rbFolder.Checked = true;
                this.Panel1.Enabled = true;
            }
            else
            {
                //編集モードだ
                this.chkEditTitle.Checked = true;
                this.txtTitle.Text = this._prevLauncherItem.Title;
                this.txtPath.Text = this._prevLauncherItem.Path;
                TrySetLauncherType(this._prevLauncherItem.LauncherType);
                this.Panel1.Enabled = false;
            }
        }

        private async void OnDebounceTimerTick(object? sender, EventArgs e)
        {
            _debounceTimer.Stop();
            if (!this.chkEditTitle.Checked && !this.rbURL.Checked)
            {
                this.txtTitle.Text = await TryGetDefaultTitleString();
            }
        }

        private void rbFolder_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void rbURL_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void rbFile_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
            this.chkEditTitle.Enabled = true;
            this.btnPath.Enabled = true;
            if (this.rbURL.Checked)
            {
                //URLではパス指定ボタンは不要
                this.btnPath.Enabled = false;
                this.lblPathCaption.Text = "URL";
                this.chkEditTitle.Enabled = false;
                this.txtTitle.Enabled = true;
            }
            else if (this.rbFolder.Checked)
            {
                this.lblPathCaption.Text = "フォルダパス";
            }
            else if (this.rbFile.Checked)
            {
                this.lblPathCaption.Text = "ファイルパス";
            }
        }

        private async Task<string> TryGetDefaultTitleString()
        {
            string text = this.txtPath.Text;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                switch (true)
                {
                    case bool _ when rbFolder.Checked:
                        if (System.IO.Directory.Exists(text))
                        {
                            //Dim dir As String = System.IO.Path.GetDirectoryName(text)
                            return new System.IO.DirectoryInfo(text).Name;
                        }
                        break;

                    case bool _ when rbFile.Checked:
                        if (System.IO.File.Exists(text))
                        {
                            return System.IO.Path.GetFileNameWithoutExtension(text);
                        }
                        break;

                    case bool _ when rbURL.Checked:
                        
                        if (text.StartsWith("http://") || text.StartsWith("https://"))
                        {
                            using (HttpClient httpClient = new HttpClient())
                            {
                                try
                                {
                                    // URLからHTMLを取得
                                    string htmlContent = await httpClient.GetStringAsync(text);

                                    // 正規表現を使用して<title>タグの内容を取得
                                    Match titleMatch = Regex.Match(htmlContent, "<title>(.*?)</title>", RegexOptions.IgnoreCase);
                                    if (titleMatch.Success)
                                    {
                                        return titleMatch.Groups[1].Value;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        
                        break;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

            return "";
        }

        /// <summary>
        /// 選択中のラジオボタンに対応するLauncherTypeを返します。
        /// </summary>
        /// <returns></returns>
        private LauncherType TryGetLauncherType()
        {
            foreach (Control control in this.Panel1.Controls)
            {
                RadioButton? rb = control as RadioButton;
                if (rb != null && rb.Checked && rb.Tag != null)
                {
                    return (LauncherType)rb.Tag;
                }
            }
            return LauncherType.none;
        }

        /// <summary>
        /// 引数のlauncherTypeに一致するラジオボタンを選択します。
        /// </summary>
        /// <param name="launcherType"></param>
        private void TrySetLauncherType(LauncherType launcherType)
        {
            foreach (Control control in this.Panel1.Controls)
            {
                RadioButton? rb = control as RadioButton;
                if (rb?.Tag != null)
                {
                    if ((LauncherType)rb.Tag == launcherType)
                    {
                        rb.Checked = true;
                        return;
                    }
                }
            }
            //なぜか取れなかったのでフォルダを規定とする
            this.rbFolder.Checked = true;
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
    }
}