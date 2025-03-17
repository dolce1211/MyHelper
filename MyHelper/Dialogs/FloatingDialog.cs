using MyHelper.Domain;
using MyHelper.Domain.Entities;
using MyHelper.Domain.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyHelper
    .Dialogs
{
    public partial class FloatingDialog : Form
    {

        public event EventHandler<PassToFrmTableDefinitionEventArgs>? PassToFrmTableDefinition;

        private System.Windows.Forms.Timer _mouseCheckTimer = new System.Windows.Forms.Timer();
        private CacheManager _cacheManager;
        private bool _suspendHideForm = false;
        
        

        public FloatingDialog(CacheManager pathCacheManager)
        {
            InitializeComponent();
            _cacheManager = pathCacheManager;
        }

        public void ShowFloatingDialog(ExtractedInformations extractedKeywords)
        {
            if (extractedKeywords == null || extractedKeywords.Keywords == null || extractedKeywords.Keywords.Count == 0)
            {
                return;
            }

            lstTable.Items.Clear();
            lstTable.Items.AddRange(extractedKeywords.Keywords.ToArray());

            TopMost = true;
            _isDragging = false;
            Point cursorPos = Cursor.Position;

            this.BringFormToMouseCursor(20, 10);

            _mouseCheckTimer.Start();
            _mouseCheckTimer.Tick += this.mouseCheckTimer_Tick;
            Show();

            Task.Run(async () =>
            {
                await Task.Delay(100);
                Invoke(new Action(() =>
                {
                    this.BringWindowToFront();
                    lstTable.Focus();
                    if (lstTable.Items.Count > 0)
                    {
                        lstTable.SelectedItem = lstTable.Items[0];
                    }
                }));
            });
        }

        private void AsIsTodoを比較するForVBFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecutePrimaryContextMenu();
        }



        private async void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            テーブル定義を開くToolStripMenuItem.Visible = false;
            RedMineを開くToolStripMenuItem.Visible = false;


            int count = 0;
            ExtractedKeywordBase? keyword = null;
            do
            {
                keyword = lstTable?.SelectedItem as ExtractedKeywordBase;
                if (keyword != null) break;
                await Task.Delay(100);
                count++;
                if (count >= 5) break;
            } while (true);

            if (keyword == null) return;

            if (keyword.KeywordType == KeyWordType.TableName)
            {
                テーブル定義を開くToolStripMenuItem.Visible = true;

            }
            else if (keyword.KeywordType == KeyWordType.TicketNumber)
            {
                RedMineを開くToolStripMenuItem.Visible = true;
            }
        }

        private void ExecutePrimaryContextMenu()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (lstTable.SelectedItem == null) return;

                ExtractedKeywordBase? keyword = lstTable.SelectedItem as ExtractedKeywordBase;
                if (keyword == null) return;

                if (keyword.KeywordType == KeyWordType.TableName)
                {
                    var tableInfo = _cacheManager.TryGetTableInfo(keyword.KeywordString);
                    if (tableInfo != null)
                        PassToFrmTableDefinition?.Invoke(this, new PassToFrmTableDefinitionEventArgs(tableInfo));
                }
                else if (keyword.KeywordType == KeyWordType.TicketNumber)
                {
                    if (Regex.Matches(keyword.KeywordString, "#\\d+").Count > 0)
                    {
                        string ticketnum = keyword.KeywordString.Replace("#", "");
                        string url = $"http://*****/redmine/issues/{ticketnum}";
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    if (System.IO.File.Exists(keyword.KeywordString))
                    {
                        Process.Start("explorer.exe", keyword.KeywordString);
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void lstTable_DoubleClick(object sender, EventArgs e)
        {
            ExecutePrimaryContextMenu();
        }

        private void lstTable_Format(object sender, ListControlConvertEventArgs e)
        {
            ExtractedKeywordBase? keyword = e.Value as ExtractedKeywordBase;
            if (keyword == null) return;

            e.Value = keyword.DisplayString(_cacheManager);
        }

        private void lstTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (lstTable.SelectedItem == null) return;
                string? text = lstTable.GetItemText(lstTable.SelectedItem);
                Clipboard.SetText(text ?? "");
            }
            else if (e.KeyCode == Keys.Return)
            {
                ContextMenuStrip1.Show(Cursor.Position);
                e.SuppressKeyPress = true;
            }
        }

        private void mouseCheckTimer_Tick(object? sender, EventArgs e)
        {
            if (_suspendHideForm) return;
            if (ContextMenuStrip1.Visible) return;

            if (!this.IsCursorWithinDistance(20))
            {
                Hide();
                _mouseCheckTimer.Stop();
            }
        }

        private void MouseTrackingForm_Load(object sender, EventArgs e)
        {
            _mouseCheckTimer.Interval = 100;
            _mouseCheckTimer.Start();
        }

        private void RedMineを開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecutePrimaryContextMenu();
        }

        private void SQLを開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecutePrimaryContextMenu();
        }



        private void テーブル定義を開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExecutePrimaryContextMenu();
        }

        private Point _dragStartPoint;
        private bool _isDragging = false;
        private void ListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                int index = lstTable.IndexFromPoint(e.Location);
                if (index >= 0 && index < lstTable.Items.Count)
                {
                    lstTable.SelectedIndex = index;
                }
            }
        }

        private void ListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentScreenPos = lstTable.PointToScreen(new Point(e.X, e.Y));
                Location = new Point(currentScreenPos.X - _dragStartPoint.X, currentScreenPos.Y - _dragStartPoint.Y);
            }
        }

        private void ListBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }

        private void lstTable_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class PassToFrmTableDefinitionEventArgs : EventArgs
    {
        public PassToFrmTableDefinitionEventArgs(TableInfo tableInfo)
        {
            TableInfo = tableInfo;
        }

        public TableInfo TableInfo { get; }
    }
}