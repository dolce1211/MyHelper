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
        /// SQL�Ď��_�C�A���O
        /// </summary>
        private frmSQLWatcher? _frmSQLWatcher = null;

        /// <summary>
        /// �ȈՃ����`���[
        /// </summary>
        private frmLauncher? _frmLauncher = null;
        /// <summary>
        /// �ā[�Ԃ��`�_�C�A���O
        /// </summary>
        private List<frmTableDefinition> _frmTableDefinitions = new List<frmTableDefinition>();
        
        private const int WM_COPYDATA = 0x004A;
        private Setting _setting;



        public frmMain()
        {
            //�ݒ�����擾
            using var dataRepository = RepositoryFactory.CreateSettingDataRepository();
            _setting = dataRepository.GetSingleAsync().Result;

            InitializeComponent();
        }

        /// <summary>
        /// ���b�Z�[�W���Ď�����.
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
                            //SQL�Ď��_�C�A���O�Ƀ��b�Z�[�W�𑗂�
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
                // �f�o�b�O���Ƀ}�E�X�̃t�b�N���s���ƃX�e�b�v���s���ɃJ�N���Ă������̂Ŗ������ł���悤�ɂ���
                var ret = MessageBox.Show(this, "�}�E�X�̃t�b�N���s�����ƂŃ}�E�X�{�^�������������m�ł��܂����A�����L���ɂ���ƃf�o�b�O���̃X�e�b�v���s�����������Ƃ�����܂��B\n\n�}�E�X�̃t�b�N���s���܂����H", "�J���Ҍ����m�F", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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


            
            // ����̓e�[�u������ǂݍ���
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
        /// Ctrl+C��2�񉟂��ꂽ
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
                    // �L���ȃt�@�C����t�H���_�̃p�X���I������Ă����Ԃ�Ctrl+CC�������ꂽ�ꍇ�A
                    // ���̃p�X���J��
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
        /// ���E�̃}�E�X�{�^���������ɉ����ꂽ
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
        /// ���E��Ctrl�L�[�������ɉ����ꂽ
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

        private void �ݒ�ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ����XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// �e��_�C�A���O�͐e�t�H�[��������܂ł͎E���Ȃ��悤�ɂ���
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
        /// �e�[�u����`�_�C�A���O��\������
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
                //���J���Ă��邷�ׂĂ�frmTableDefinition�����
                var dels = new List<frmTableDefinition>();
                dels.AddRange(_frmTableDefinitions);
                foreach (var f in dels)
                {
                    f.Close();
                }
            };
        }

        /// <summary>
        /// �N���b�v�{�[�h�̕���������Ƀt���[�e�B���O�_�C�A���O��\������
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
        /// �����`���[�_�C�A���O��\������
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


        #region "TableInfo�o�^���"


        private void tablescsv�ēǍ�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("tables.csv�̍ēǂݍ��݂��s���܂��B\n\n��낵���ł����H", "�m�F", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                return;
            _ = Task.Run(()=> RefreshTableInfos(true));
        }

        /// <summary>
        /// TableInfo,TableColumnInfo��csv����DB�ɓo�^����
        /// </summary>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        private async Task RefreshTableInfos(bool forceUpdate)
        {
            // tables.csv��ǂݍ����DB�Ɋi�[����as    
            await ReadCSVAndRegisterTables(forceUpdate);

            // tableinfo�̓��e���L���b�V������
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
                    MessageBox.Show("�e�[�u�����̎擾�Ɏ��s���܂����B\r\n\r\ntable.csv���G�N�Z�����ŊJ���Ă��Ȃ����m�F���Ă��������B","table.csv�ǂݎ�莸�s", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }));
            }
            catch(Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("�e�[�u�����̎擾�Ɏ��s���܂���");
                }));               
            }
            
            var tableinfos = tuple.Item1; //CSV����擾�������
            var lastWriteTime = tuple.Item2; // CSV�t�@�C���̍ŏI�X�V����
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

                    //�e�[�u����񂪎�ꂽ�Ȃ�ADB�ɕۑ�����
                    MyHelperDBContext.Debug_DisableLog = true; //�@�d���Ȃ邩��DbContext�̃��O�𖳌���
                    var flg = false;
                    using var tableRepo = RepositoryFactory.CreateTableInfoRepository();
                    using var tableColumnRepo = RepositoryFactory.CreateTableColumnInfoRepository();
                    using var transaction = await tableRepo.TransactionManager.BeginTransactionAsync();
                    
                    try
                    {
                        // �e�[�u���S�폜
                        await tableRepo.DeleteAllAsync();
                        // �e�[�u���S�o�^
                        await tableRepo.UpdateAllItemsAsync(tableinfos);                        
                        // �R�~�b�g
                        await transaction.CommitAsync();
                        flg = true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("�e�[�u�����̓o�^�Ɏ��s���܂���");
                        }));

                    }
                    finally
                    {
                        if (flg)
                        {
                            // settting��tables.csv�̍ŏI�X�V���Ԃ�o�^����
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