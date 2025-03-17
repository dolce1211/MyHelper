using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.Repositories;
using MyHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows.Forms;
using MyHelper.Infrastructure;

namespace MyHelper.Dialogs
{
    public partial class frmSetting : Form
    {

        private Setting? _setting;
        private Setting? _result = null;
        public Setting? Result => _result;

        public frmSetting()
        {
            InitializeComponent();

            using var dataRepository = RepositoryFactory.CreateSettingDataRepository();    
            _setting = dataRepository.GetSingleAsync().Result;
            chkOpenValidPathByCtrlCC.Checked = _setting.OpenValidPathByCtrlCC;
            chkEnableLauncherByMouseButton.Checked = _setting.EnableLauncherByMouseButton;
            chkEnableLauncherByCtrlKeys.Checked = _setting.EnableLauncherByCtrlKeys;

        }



        private void btnAsIsPath_Click(object sender, EventArgs e)
        {
            string title = "";
            string filter = "";
            string defExt = "";

            Button? button = sender as Button;
            if (button == null) return;
            TextBox? textBox = null;
            bool isFolder = false;
            

            if (button == btnAsIsSlnPath)
            {
                filter = "ソリューションファイル (*.sln)|*.sln|All Files (*.*)|*.*";
                defExt = "sln";
                title = "AsIsシステムの.slnファイルの場所を指定してください";
                textBox = txtAsIsSlnPath;
            }
            //else if (button == btnWinMergePath)
            //{
            //    filter = "実行ファイル (*.exe)|*.exe|All Files (*.*)|*.*";
            //    defExt = "exe";
            //    title = "WinMergeがインストールされている場所を指定してください";
            //    textBox = txtWinMergePath;
            //}
            if (textBox == null) return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = title;
                if (isFolder)
                {
                    openFileDialog.ValidateNames = false;
                    openFileDialog.CheckFileExists = false;
                    openFileDialog.CheckPathExists = true;
                    openFileDialog.FileName = "このフォルダを選択";
                    if (Directory.Exists(textBox.Text))
                    {
                        openFileDialog.InitialDirectory = textBox.Text;
                    }
                }
                else
                {
                    openFileDialog.DefaultExt = defExt;
                    openFileDialog.Filter = filter;
                    if (textBox != null && File.Exists(textBox.Text))
                    {
                        openFileDialog.FileName = textBox.Text;
                        openFileDialog.InitialDirectory = Path.GetDirectoryName(textBox.Text);
                    }
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (isFolder)
                    {
                        textBox!.Text = Path.GetDirectoryName(openFileDialog.FileName) ?? string.Empty;
                    }
                    else
                    {
                        textBox!.Text = openFileDialog.FileName ?? string.Empty;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string addStr = "";
                TextBox? txtbox = null;
                Setting result = new Setting
                {
                    OpenValidPathByCtrlCC = chkOpenValidPathByCtrlCC.Checked,
                    EnableLauncherByMouseButton = chkEnableLauncherByMouseButton.Checked,
                    EnableLauncherByCtrlKeys = chkEnableLauncherByCtrlKeys.Checked
                };

                if (!string.IsNullOrEmpty(addStr))
                {
                    string msg = $"{addStr}\n\n以上のパスが正しく指定されていません。\n\nよろしいですか？";
                    if (MessageBox.Show(this, msg, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        txtbox?.Focus();
                        return;
                    }
                }

                _result = result;
                using var settingDataRepository = RepositoryFactory.CreateSettingDataRepository();
                await settingDataRepository.AddOrUpdateAsync(_setting!.Id, result);
                DialogResult = DialogResult.OK;
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(this, $"データベースの更新中にエラーが発生しました: {ex.Message}", "データベースエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"予期しないエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
        }
    }
}