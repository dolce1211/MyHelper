
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
using NetTopologySuite.Operation.Distance;

namespace MyHelper.Dialogs
{
    public partial class frmTableDefinition : Form
    {
        private CacheManager _cacheManager = null!;
        private TableInfo _tableInfo = null!;
        private System.Windows.Forms.Timer _mouseCheckTimer = new System.Windows.Forms.Timer();
        private bool _suspendHideForm = false;
        public event EventHandler? CloseAllFrmTableDefinition;
        public TableInfo TableInfo => _tableInfo;
        public frmTableDefinition(CacheManager pathCacheManager, TableInfo tableinfo)
        {
            InitializeComponent();
            _cacheManager = pathCacheManager;
            _tableInfo = tableinfo;
            Initialize_Grid();
        }

        private void frmTableDefinition_Load(object sender, EventArgs e)
        {
            if (_tableInfo == null || _tableInfo.TableColumnInfos == null)
                return;

            this.Text = _tableInfo.TableName.Trim();
            this.lblTitle.Text = $"{_tableInfo.TableName.Trim()} - {_tableInfo.TableComment.Trim()}";
            _suspendHideForm = false;
            chkFix.Checked = false;

            // フォームをマウスカーソルの位置までもってくる
            this.BringFormToMouseCursor(50, 50);

            _mouseCheckTimer.Start();
            _mouseCheckTimer.Tick += this._mouseCheckTimer_Tick;
            var allItems = new List<string>(); //_cacheManager.GetAllItems(true, true);

            DataGridView1.DataSource = null;
            DataGridView1.DataSource = _tableInfo.TableColumnInfos;
        }



        private void Initialize_Grid()
        {
            DataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            DataGridView1.MultiSelect = false;
            DataGridView1.AutoGenerateColumns = false;
            DataGridView1.RowHeadersVisible = false;
            DataGridView1.AllowUserToResizeRows = false;
            DataGridView1.VirtualMode = true;
            DataGridView1.ReadOnly = true;



            DataGridViewTextBoxColumn colIsPrimaryKey = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IsPrimaryKeyString",
                HeaderText = "key",
                Width = 40,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            DataGridViewTextBoxColumn colColumnName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ColumnName",
                HeaderText = "列名",
            };
            DataGridViewTextBoxColumn colDataType = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataType",
                HeaderText = "データ型",
                Width = 160
            };

            DataGridViewTextBoxColumn colColumnComment = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ColumnComment",
                HeaderText = "列日本語名"
            };

            DataGridViewTextBoxColumn colIsNullable = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IsNullableString",
                HeaderText = "nullable",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };


            foreach (DataGridViewColumn column in DataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            DataGridView1.Columns.AddRange(new DataGridViewColumn[] { colIsPrimaryKey, colColumnName, colDataType, colColumnComment, colIsNullable });
        }

        private Point _dragStartPoint;
        private bool _isDragging = false;
        private void frmTableDefinition_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = new Point(e.X, e.Y);
            }

        }

        private void frmTableDefinition_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentScreenPos = this.PointToScreen(new Point(e.X, e.Y));
                // フォームのタイトルバーや境界線の高さを考慮する
                int offsetX = currentScreenPos.X - _dragStartPoint.X;
                int offsetY = currentScreenPos.Y - _dragStartPoint.Y - SystemInformation.CaptionHeight;
                Location = new Point(offsetX, offsetY);
            }
        }
        private void frmTableDefinition_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }
        private void _mouseCheckTimer_Tick(object? sender, EventArgs e)
        {
            if (_suspendHideForm) return;
            if (chkFix.Checked) return;
            //if (ContextMenuStrip1.Visible) return; // 右クリックメニューが出ている間は閉じない
            if (!this.IsCursorWithinDistance(20))
            {
                // マウスカーソルが離れたら隠す

                _isDragging = false;
                _mouseCheckTimer.Stop();
                this.Close();
            }
        }


        private void frmTableDefinition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (this.ActiveControl != this.chkFix)
                    this.chkFix.Checked = !this.chkFix.Checked;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Todo");
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            this.CloseAllFrmTableDefinition?.Invoke(this, new EventArgs());
            this.Close();
        }
    }
}
