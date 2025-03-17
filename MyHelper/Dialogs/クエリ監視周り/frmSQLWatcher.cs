using MyHelper.Domain;
using MyHelper.Domain.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    public partial class frmSQLWatcher : Form
    {
        public event EventHandler<PassToFrmTableDefinitionEventArgs>? PassToFrmTableDefinition;
        private int _currentSeq = 0;
        private Queue<SqlInfo> _sqlQueue = new Queue<SqlInfo>();
        private BindingList<SqlInfo> _sqlInfos = new BindingList<SqlInfo>();
        private CacheManager _cacheManager = null!;
        public frmSQLWatcher(CacheManager cacheManager)
        {
            InitializeComponent();
            _cacheManager = cacheManager;
        }

        public bool TryAddMessage(string receivedMessage)
        {
            if (receivedMessage == null) return false;

            bool ret = false;
            _currentSeq++;
            SqlInfo sqlinfo = new SqlInfo(_currentSeq, receivedMessage);
            if (!string.IsNullOrEmpty(sqlinfo.AddedTime))
            {
                _sqlQueue.Enqueue(sqlinfo);
                Timer1.Stop();
                Timer1.Start();
                ret = true;
            }
            if (chkTopmostOnReceive.Checked)
            {
                this.Visible = true;
                this.BringWindowToFront();
            }
            return ret;
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnJumpToBlazor.PerformClick();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SqlInfo? sqlInfo = TryGetInstanceFromActiveRow();
            if (sqlInfo != null && DataGridView1.Enabled)
            {
                ApplySqlInfo(sqlInfo);
            }
        }

        private void Initialize_Grid()
        {
            DataGridView1.Columns.Clear();
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DataGridView1.MultiSelect = false;
            DataGridView1.AutoGenerateColumns = false;
            DataGridView1.RowHeadersVisible = false;
            DataGridView1.AllowUserToResizeRows = false;
            DataGridView1.VirtualMode = true;

            DataGridViewTextBoxColumn colSeq = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Seq",
                HeaderText = "",
                Width = 30
            };

            DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AddedTime",
                HeaderText = "発行時刻",
                Width = 110
            };

            DataGridViewTextBoxColumn colSQL = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SQLFormatted",
                HeaderText = "クエリ",
                Width = 60,
            };

            DataGridViewTextBoxColumn colConnectString = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ParamString",
                HeaderText = "Params"
            };

            DataGridViewTextBoxColumn colFileName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BlazorFileName",
                HeaderText = "呼び元ファイル"
            };

            DataGridViewTextBoxColumn colMemberName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BlazorMemberName",
                HeaderText = "メンバ"
            };

            DataGridViewTextBoxColumn colLineNumber = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BlazorLineNumber",
                HeaderText = "行数",
                Width = 60
            };

            foreach (DataGridViewColumn column in DataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            DataGridView1.Columns.AddRange(new DataGridViewColumn[] { colSeq, colTime, colSQL, colFileName, colLineNumber, colMemberName, colConnectString });
            DataGridView1.DataSource = _sqlInfos;

            ApplyFilter();

        }

        private SqlInfo? TryGetInstanceFromActiveRow()
        {
            if (DataGridView1.Rows.Count <= 0 || DataGridView1.SelectedRows == null || DataGridView1.SelectedRows.Count <= 0)
                return null;

            DataGridViewRow activeRow = DataGridView1.SelectedRows[0];
            SqlInfo? sqlInfo = activeRow != null ? activeRow.DataBoundItem as SqlInfo : null;
            return sqlInfo;
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            SqlInfo? sqlInfo = TryGetInstanceFromActiveRow();
            if (sqlInfo != null)
            {
                var filePath = "";
                var lineNumber = 0;
                if (sender == btnJumpToReaderWriter)
                {
                    filePath = sqlInfo.ReaderWriterFilePath;
                    lineNumber = sqlInfo.ReaderWriterLineNumber;
                }
                else if (sender == btnJumpToBlazor)
                {
                    filePath = sqlInfo.BlazorFilePath;
                    lineNumber = sqlInfo.BlazorLineNumber;
                }
                if (!System.IO.File.Exists(filePath))
                    return;

                VisualStudioOpener.OpenFileAndGoToLine(filePath, lineNumber);
            }
        }

        /// <summary>
        /// クエリ情報をグリッドに反映します。
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        private bool ApplySqlInfo(SqlInfo sqlInfo)
        {
            if (sqlInfo == null) return false;
            this.SuspendDrawing();
            try
            {
                RichTextBox1.AddSQLAndColorize(sqlInfo.SQL);
                txtParam.Text = sqlInfo.ParamString;

                txtBlazorFilePath.Text = sqlInfo.BlazorFilePath;

                lblBlazorLineNum.Text = sqlInfo.BlazorLineNumber > 0 ? $"blazor: {sqlInfo.BlazorLineNumber}行目" : "";
                txtReaderWriterFilePath.Text = sqlInfo.ReaderWriterFilePath;
                lblReaderWriterLineNum.Text = sqlInfo.ReaderWriterLineNumber > 0 ? $"reader/writer: {sqlInfo.ReaderWriterLineNumber}行目" : "";
                lblElapsedTime.Text = $"処理時間:{sqlInfo.ElapsedMilliseconds}ms";
                lstTables.Items.Clear();
                List<string> tables = sqlInfo.SQL.ExtractTableNames().Select(n => n.TableName.ToLower()).Distinct().ToList();
                if (tables != null && tables.Count > 0)
                {
                    lstTables.Items.AddRange(tables.ToArray());
                }

                txtBlazorFilePath.SelectionStart = txtBlazorFilePath.Text.Length;
                txtReaderWriterFilePath.SelectionStart = txtReaderWriterFilePath.Text.Length;

                btnJumpToBlazor.Enabled = System.IO.File.Exists(sqlInfo.BlazorFilePath);
                btnJumpToReaderWriter.Enabled = System.IO.File.Exists(sqlInfo.ReaderWriterFilePath);
                return true;
            }
            finally
            {
                if (!Timer1.Enabled)
                {
                    this.ResumeDrawing();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DataGridView1.Rows.Clear();
            RichTextBox1.Clear();
            txtParam.Clear();
            ApplySqlInfo(new SqlInfo(0, ""));
            _currentSeq = 0;
        }

        private void btnTableDefinition_Click(object sender, EventArgs e)
        {
            if (lstTables.SelectedItem == null) return;
            var tableInfo = _cacheManager.TryGetTableInfo(lstTables.SelectedItem?.ToString() ?? "");
            if (tableInfo != null)
                PassToFrmTableDefinition?.Invoke(this, new PassToFrmTableDefinitionEventArgs(tableInfo));

        }

        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = chkTopMost.Checked;
        }

        private void DebugForm_Load(object sender, EventArgs e)
        {
            Initialize_Grid();
            ApplySqlInfo(new SqlInfo(0, ""));
        }

        private void lstTables_DoubleClick(object sender, EventArgs e)
        {
            btnTableDefinition.PerformClick();
        }

        private void lstTables_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetText(lstTables.SelectedItem?.ToString() ?? string.Empty);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
                this.SuspendDrawing(true);
            try
            {
                while (_sqlQueue.Count > 0)
                {
                    SqlInfo sqlInfo = _sqlQueue.Dequeue();
                    _sqlInfos.Insert(0, sqlInfo);

                    //とりあえず2000行まで
                    while (_sqlInfos.Count > 2000)
                    {
                        _sqlInfos.RemoveAt(_sqlInfos.Count - 1);
                    }

                    string sql = sqlInfo.SQL.Trim().ToUpper();
                    if (sql.ToUpper().Contains("INSERT") || sql.Contains("UPDATE") || sql.Contains("DELETE"))
                    {
                        foreach (DataGridViewCell cell in DataGridView1.Rows[0].Cells)
                        {
                            cell.Style.BackColor = Color.White;
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
                if (DataGridView1.Rows.Count > 0)
                {
                    DataGridView1.ClearSelection();
                    DataGridView1.Rows[0].Selected = true;
                    DataGridView1.FirstDisplayedScrollingRowIndex = 0;
                }

                ApplyFilter();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (this.Visible)
                    this.ResumeDrawing(true);
            }

            Timer1.Stop();
            //Initialize_Grid();
        }

        private void 閉じるXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chkFilterByTime_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlFilterByTime.Enabled = this.chkFilterByTime.Checked;
        }





        #region "フィルター周り"

        private FilterInfo _filterInfo = new FilterInfo();
        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (this.pnlFilter.Visible)
            {
                this.pnlFilter.Visible = false;
            }
            else
            {
                ShowPnlFilter(this._filterInfo);
            }
            
        }
        private void ShowPnlFilter(FilterInfo filterInfo)
        {
            chkFilterByTime.Checked = false;
            numFilterByTime.Value = 0;
            if (filterInfo.FilterByTime>0)
            {
                chkFilterByTime.Checked = true;
                numFilterByTime.Value = filterInfo.FilterByTime;
            }

            var blazorMethods = _sqlInfos.Select(n => n.BlazorWriterFileMember).Distinct().OrderBy(n => n).ToList();
            var readerWriterMethods = _sqlInfos.Select(n => n.ReaderWriterFileMember).Distinct().OrderBy(n => n).ToList();

            chkLstBlazor.Enabled = false;
            chkLstReaderWriter.Enabled = false;
            try
            {

                chkLstBlazor.Items.Clear();
                chkLstReaderWriter.Items.Clear();

                chkLstBlazor.Items.Add("すべて");
                chkLstBlazor.Items.AddRange(blazorMethods.ToArray());
                chkLstReaderWriter.Items.Add("すべて");
                chkLstReaderWriter.Items.AddRange(readerWriterMethods.ToArray());

                for (int i = 1; i < chkLstBlazor.Items.Count; i++)
                {
                    chkLstBlazor.SetItemChecked(i, !filterInfo.HiddenFiles_Blazor.Contains(chkLstBlazor.Items[i].ToString()!));
                }
                for (int i = 1; i < chkLstReaderWriter.Items.Count; i++)
                {
                    chkLstReaderWriter.SetItemChecked(i, !filterInfo.HiddenFiles_ReaderWriters.Contains(chkLstReaderWriter.Items[i].ToString()!));
                }
                pnlFilter.Visible = true;
            }
            finally
            {
                chkLstBlazor.Enabled = true;
                chkLstReaderWriter.Enabled = true;
                ApplyAllChecked(chkLstBlazor);
                ApplyAllChecked(chkLstReaderWriter);
            }

        }


        private void btnFilterOK_Click(object sender, EventArgs e)
        {
            if (sender == this.btnFilterOK)
            {
                _filterInfo = new FilterInfo();
                for (int i = 1; i < chkLstBlazor.Items.Count; i++)
                {
                    if (!chkLstBlazor.GetItemChecked(i))
                    {
                        _filterInfo.HiddenFiles_Blazor.Add(chkLstBlazor.Items[i].ToString()!);
                    }
                }
                for (int i = 1; i < chkLstReaderWriter.Items.Count; i++)
                {
                    if (!chkLstReaderWriter.GetItemChecked(i))
                    {
                        _filterInfo.HiddenFiles_ReaderWriters.Add(chkLstReaderWriter.Items[i].ToString()!);
                    }
                }
                _filterInfo.FilterByTime = 0;
                if (chkFilterByTime.Checked)
                    _filterInfo.FilterByTime = (int)numFilterByTime.Value;

                if (_filterInfo.IsFilterValid)
                {
                    this.btnFilter.BackColor = Color.Black;
                    this.btnFilter.ForeColor = Color.FromArgb(240, 240, 240);
                }
                else
                {
                    this.btnFilter.BackColor = Color.FromArgb(240, 240, 240);
                    this.btnFilter.ForeColor = Color.Black;

                }
                Initialize_Grid();
            }
            pnlFilter.Visible = false;
        }



       
        private void chkLstBlazor_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox? checkedListBox = sender as CheckedListBox;
            if (checkedListBox != null)
            {
                if (!checkedListBox.Enabled)
                    return;

                var index = e.Index;
                bool chked = e.NewValue== CheckState.Checked?true:false;
                if (index == 0)
                {
                    // すべての場合、他の項目も同じ状態にする
                    checkedListBox.Enabled = false;
                    for (int i = 1; i < checkedListBox.Items.Count; i++)
                    {
                        checkedListBox.SetItemChecked(i, chked);
                    }
                    checkedListBox.Enabled = true;
                }
                else
                {
                    //　それ以外の場合、すべての項目がチェックされていれば、最初の項目もチェックする
                    this.BeginInvoke(new Action(() =>ApplyAllChecked(checkedListBox)));
                }
            }
        }
        private void ApplyAllChecked(CheckedListBox checkedListBox)
        {
            //　それ以外の場合、すべての項目がチェックされていれば、最初の項目もチェックする
            bool allChecked = true;
            for (int i = 1; i < checkedListBox.Items.Count; i++)
            {
                if (!checkedListBox.GetItemChecked(i))
                {
                    allChecked = false;
                    break;
                }
            }
            checkedListBox.Enabled = false;
            checkedListBox.SetItemChecked(0, allChecked);
            checkedListBox.Enabled = true;
        }

        private void btnFilterReset_Click(object sender, EventArgs e)
        {
            ShowPnlFilter(new FilterInfo());
        }
        private void ApplyFilter()
        {
            CurrencyManager currencyManager = (CurrencyManager)BindingContext[DataGridView1.DataSource];
            currencyManager.SuspendBinding();
            try
            {
                foreach (DataGridViewRow row in DataGridView1.Rows)
                {
                    row.Selected = false; // カレント行を無効化
                    SqlInfo? sqlInfo = row.DataBoundItem as SqlInfo;
                    if (sqlInfo != null)
                    {
                        bool isVisible = true;
                        // Blazorメソッドのフィルタリング
                        if (_filterInfo.HiddenFiles_Blazor.Contains(sqlInfo.BlazorWriterFileMember))
                            isVisible = false;

                        // ReaderWriterメソッドのフィルタリング
                        if (_filterInfo.HiddenFiles_ReaderWriters.Contains(sqlInfo.ReaderWriterFileMember))
                            isVisible = false;

                        // 時間フィルタリング
                        if (_filterInfo.FilterByTime > 0 && sqlInfo.ElapsedMilliseconds < _filterInfo.FilterByTime)
                            isVisible = false;

                        row.Visible = isVisible;
                    }
                }
            }
            finally
            {
                currencyManager.ResumeBinding();
            }           

        }


        private class FilterInfo
        {
            public List<string> HiddenFiles_Blazor { get; set; } = new List<string>();
            public List<string> HiddenFiles_ReaderWriters { get; set; } = new List<string>();
            public int FilterByTime { get; set; } = 0;

            public bool IsFilterValid => HiddenFiles_Blazor.Count > 0 || HiddenFiles_ReaderWriters.Count > 0 || FilterByTime > 0;
        }

        #endregion

        private class SqlInfo
        {
            public string AddedTime { get; }

            public string ReaderWriterFileName => System.IO.Path.GetFileName(ReaderWriterFilePath);
            public string ReaderWriterFileMember => $"{this.ReaderWriterFileName.Replace(".cs", "")}.{this.ReaderWriterMemberName}";
            public string BlazorFileName => System.IO.Path.GetFileName(BlazorFilePath);
            public string BlazorWriterFileMember => $"{this.BlazorFileName.Replace(".cs","")}.{this.BlazorMemberName}";


            // FSZDataReaderかFSZDataWriterのサブクラスのファイルパス
            public string ReaderWriterFilePath { get; } = "";
            // FSZDataReaderかFSZDataWriterのサブクラスの行数
            public int ReaderWriterLineNumber { get; }
            // FSZDataReaderかFSZDataWriterのサブクラスのメソッド名
            public string ReaderWriterMemberName { get; } = "";

            // Blazor内ファイルのファイルパス
            public string BlazorFilePath { get; } = "";
            // Blazor内ファイルの行数
            public int BlazorLineNumber { get; }
            // Blazor内ファイルのメソッド名
            public string BlazorMemberName { get; } = "";
            public int Seq { get; }
            public string SQL { get; } = "";
            public string ParamString { get; } = "";

            public int ElapsedMilliseconds { get; } = 0;
            public string SQLFormatted => SQL.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            public SqlInfo(int seq, string receivedMessage)
            {
                string[] array = receivedMessage.Split('#');
                AddedTime = "";
                ReaderWriterFilePath = "";
                ReaderWriterMemberName = "";
                SQL = "";

                if (array.Length >= 9)
                {
                    SQL = array[0].Trim();
                    ParamString = array[1].Trim();
                    ReaderWriterMemberName = array[2].Trim();
                    ReaderWriterFilePath = array[3].Trim();
                    int.TryParse(array[4].Trim(), out int readerwriterLineNumber);
                    if (System.IO.File.Exists(ReaderWriterFilePath) && readerwriterLineNumber > 0)
                        ReaderWriterLineNumber = readerwriterLineNumber;


                    BlazorMemberName = array[5].Trim();
                    BlazorFilePath = array[6].Trim();
                    int.TryParse(array[7].Trim(), out int blazorLineNumber);
                    if (System.IO.File.Exists(BlazorFilePath) && blazorLineNumber > 0)
                        BlazorLineNumber = blazorLineNumber;

                    int.TryParse(array[8].Trim(), out int elapsedMilliseconds);

                    ElapsedMilliseconds = elapsedMilliseconds;
                    AddedTime = DateTime.Now.ToString("MM/dd HH:mm:ss,fff");
                    Seq = seq;
                }
            }
        }

    }
}