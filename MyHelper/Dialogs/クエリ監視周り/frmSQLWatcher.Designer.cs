using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    partial class frmSQLWatcher : Form
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
            DataGridView1 = new DataGridView();
            Panel1 = new Panel();
            lblBlazorLineNum = new Label();
            btnJumpToBlazor = new Button();
            btnJumpToReaderWriter = new Button();
            txtBlazorFilePath = new TextBox();
            txtReaderWriterFilePath = new TextBox();
            txtParam = new TextBox();
            chkTopmostOnReceive = new CheckBox();
            btnTableDefinition = new Button();
            lstTables = new ListBox();
            RichTextBox1 = new RichTextBox();
            chkTopMost = new CheckBox();
            lblReaderWriterLineNum = new Label();
            lblElapsedTime = new Label();
            SplitContainer1 = new SplitContainer();
            Panel2 = new Panel();
            pnlFilter = new Panel();
            btnFilterReset = new Button();
            btnFilterCancel = new Button();
            btnFilterOK = new Button();
            label3 = new Label();
            label2 = new Label();
            chkFilterByTime = new CheckBox();
            pnlFilterByTime = new Panel();
            numFilterByTime = new NumericUpDown();
            label1 = new Label();
            chkLstReaderWriter = new CheckedListBox();
            chkLstBlazor = new CheckedListBox();
            btnFilter = new Button();
            btnClear = new Button();
            Timer1 = new System.Windows.Forms.Timer(components);
            MenuStrip1 = new MenuStrip();
            ファイルFToolStripMenuItem = new ToolStripMenuItem();
            閉じるXToolStripMenuItem = new ToolStripMenuItem();
            ((ISupportInitialize)DataGridView1).BeginInit();
            Panel1.SuspendLayout();
            ((ISupportInitialize)SplitContainer1).BeginInit();
            SplitContainer1.Panel1.SuspendLayout();
            SplitContainer1.Panel2.SuspendLayout();
            SplitContainer1.SuspendLayout();
            Panel2.SuspendLayout();
            pnlFilter.SuspendLayout();
            pnlFilterByTime.SuspendLayout();
            ((ISupportInitialize)numFilterByTime).BeginInit();
            MenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // DataGridView1
            // 
            DataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(4, 29);
            DataGridView1.Margin = new Padding(4);
            DataGridView1.Name = "DataGridView1";
            DataGridView1.RowTemplate.Height = 21;
            DataGridView1.Size = new Size(344, 438);
            DataGridView1.TabIndex = 0;
            DataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
            DataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            // 
            // Panel1
            // 
            Panel1.BackColor = Color.OldLace;
            Panel1.Controls.Add(lblBlazorLineNum);
            Panel1.Controls.Add(btnJumpToBlazor);
            Panel1.Controls.Add(btnJumpToReaderWriter);
            Panel1.Controls.Add(txtBlazorFilePath);
            Panel1.Controls.Add(txtReaderWriterFilePath);
            Panel1.Controls.Add(txtParam);
            Panel1.Controls.Add(chkTopmostOnReceive);
            Panel1.Controls.Add(btnTableDefinition);
            Panel1.Controls.Add(lstTables);
            Panel1.Controls.Add(RichTextBox1);
            Panel1.Controls.Add(chkTopMost);
            Panel1.Controls.Add(lblReaderWriterLineNum);
            Panel1.Controls.Add(lblElapsedTime);
            Panel1.Dock = DockStyle.Fill;
            Panel1.Location = new Point(0, 0);
            Panel1.Margin = new Padding(4);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(301, 471);
            Panel1.TabIndex = 8;
            // 
            // lblBlazorLineNum
            // 
            lblBlazorLineNum.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblBlazorLineNum.AutoSize = true;
            lblBlazorLineNum.Location = new Point(5, 333);
            lblBlazorLineNum.Margin = new Padding(4, 0, 4, 0);
            lblBlazorLineNum.Name = "lblBlazorLineNum";
            lblBlazorLineNum.Size = new Size(49, 15);
            lblBlazorLineNum.TabIndex = 17;
            lblBlazorLineNum.Text = "123行目";
            // 
            // btnJumpToBlazor
            // 
            btnJumpToBlazor.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnJumpToBlazor.Enabled = false;
            btnJumpToBlazor.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnJumpToBlazor.Location = new Point(136, 327);
            btnJumpToBlazor.Margin = new Padding(4);
            btnJumpToBlazor.Name = "btnJumpToBlazor";
            btnJumpToBlazor.Size = new Size(160, 25);
            btnJumpToBlazor.TabIndex = 7;
            btnJumpToBlazor.Text = "Blazorの呼び元へ";
            btnJumpToBlazor.UseVisualStyleBackColor = true;
            btnJumpToBlazor.Click += btnJump_Click;
            // 
            // btnJumpToReaderWriter
            // 
            btnJumpToReaderWriter.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnJumpToReaderWriter.Enabled = false;
            btnJumpToReaderWriter.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnJumpToReaderWriter.Location = new Point(136, 384);
            btnJumpToReaderWriter.Margin = new Padding(4);
            btnJumpToReaderWriter.Name = "btnJumpToReaderWriter";
            btnJumpToReaderWriter.Size = new Size(160, 25);
            btnJumpToReaderWriter.TabIndex = 6;
            btnJumpToReaderWriter.Text = "Reader/Writerの呼び元へ";
            btnJumpToReaderWriter.UseVisualStyleBackColor = true;
            btnJumpToReaderWriter.Click += btnJump_Click;
            // 
            // txtBlazorFilePath
            // 
            txtBlazorFilePath.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBlazorFilePath.Location = new Point(7, 352);
            txtBlazorFilePath.Margin = new Padding(4);
            txtBlazorFilePath.Name = "txtBlazorFilePath";
            txtBlazorFilePath.ReadOnly = true;
            txtBlazorFilePath.Size = new Size(288, 23);
            txtBlazorFilePath.TabIndex = 16;
            // 
            // txtReaderWriterFilePath
            // 
            txtReaderWriterFilePath.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReaderWriterFilePath.Location = new Point(7, 408);
            txtReaderWriterFilePath.Margin = new Padding(4);
            txtReaderWriterFilePath.Name = "txtReaderWriterFilePath";
            txtReaderWriterFilePath.ReadOnly = true;
            txtReaderWriterFilePath.Size = new Size(288, 23);
            txtReaderWriterFilePath.TabIndex = 15;
            // 
            // txtParam
            // 
            txtParam.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtParam.BackColor = Color.White;
            txtParam.Location = new Point(4, 246);
            txtParam.Multiline = true;
            txtParam.Name = "txtParam";
            txtParam.ReadOnly = true;
            txtParam.ScrollBars = ScrollBars.Both;
            txtParam.Size = new Size(293, 53);
            txtParam.TabIndex = 1;
            txtParam.Text = "aaa\r\neee\r\nqqq\r\n";
            // 
            // chkTopmostOnReceive
            // 
            chkTopmostOnReceive.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkTopmostOnReceive.AutoSize = true;
            chkTopmostOnReceive.Location = new Point(113, 447);
            chkTopmostOnReceive.Margin = new Padding(4);
            chkTopmostOnReceive.Name = "chkTopmostOnReceive";
            chkTopmostOnReceive.Size = new Size(110, 19);
            chkTopmostOnReceive.TabIndex = 14;
            chkTopmostOnReceive.Text = "クエリ受信で手前";
            chkTopmostOnReceive.UseVisualStyleBackColor = true;
            chkTopmostOnReceive.Click += chkTopMost_CheckedChanged;
            // 
            // btnTableDefinition
            // 
            btnTableDefinition.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTableDefinition.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnTableDefinition.Location = new Point(239, 2);
            btnTableDefinition.Margin = new Padding(4);
            btnTableDefinition.Name = "btnTableDefinition";
            btnTableDefinition.Size = new Size(60, 27);
            btnTableDefinition.TabIndex = 13;
            btnTableDefinition.Text = "定義へ";
            btnTableDefinition.UseVisualStyleBackColor = true;
            btnTableDefinition.Click += btnTableDefinition_Click;
            // 
            // lstTables
            // 
            lstTables.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lstTables.FormattingEnabled = true;
            lstTables.ItemHeight = 15;
            lstTables.Location = new Point(4, 4);
            lstTables.Margin = new Padding(4);
            lstTables.Name = "lstTables";
            lstTables.Size = new Size(235, 49);
            lstTables.TabIndex = 11;
            lstTables.DoubleClick += lstTables_DoubleClick;
            lstTables.KeyDown += lstTables_KeyDown;
            // 
            // RichTextBox1
            // 
            RichTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RichTextBox1.BackColor = Color.White;
            RichTextBox1.BorderStyle = BorderStyle.FixedSingle;
            RichTextBox1.Location = new Point(3, 54);
            RichTextBox1.Margin = new Padding(4);
            RichTextBox1.Name = "RichTextBox1";
            RichTextBox1.ReadOnly = true;
            RichTextBox1.Size = new Size(293, 190);
            RichTextBox1.TabIndex = 1;
            RichTextBox1.Text = "";
            // 
            // chkTopMost
            // 
            chkTopMost.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            chkTopMost.AutoSize = true;
            chkTopMost.Location = new Point(226, 447);
            chkTopMost.Margin = new Padding(4);
            chkTopMost.Name = "chkTopMost";
            chkTopMost.Size = new Size(71, 19);
            chkTopMost.TabIndex = 1;
            chkTopMost.Text = "常に手前";
            chkTopMost.UseVisualStyleBackColor = true;
            chkTopMost.Click += chkTopMost_CheckedChanged;
            // 
            // lblReaderWriterLineNum
            // 
            lblReaderWriterLineNum.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblReaderWriterLineNum.AutoSize = true;
            lblReaderWriterLineNum.Location = new Point(5, 389);
            lblReaderWriterLineNum.Margin = new Padding(4, 0, 4, 0);
            lblReaderWriterLineNum.Name = "lblReaderWriterLineNum";
            lblReaderWriterLineNum.Size = new Size(49, 15);
            lblReaderWriterLineNum.TabIndex = 7;
            lblReaderWriterLineNum.Text = "123行目";
            // 
            // lblElapsedTime
            // 
            lblElapsedTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblElapsedTime.AutoSize = true;
            lblElapsedTime.Location = new Point(5, 303);
            lblElapsedTime.Margin = new Padding(4, 0, 4, 0);
            lblElapsedTime.Name = "lblElapsedTime";
            lblElapsedTime.Size = new Size(58, 15);
            lblElapsedTime.TabIndex = 4;
            lblElapsedTime.Text = "処理時間:";
            // 
            // SplitContainer1
            // 
            SplitContainer1.Dock = DockStyle.Fill;
            SplitContainer1.Location = new Point(0, 24);
            SplitContainer1.Margin = new Padding(4);
            SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            SplitContainer1.Panel1.Controls.Add(Panel2);
            // 
            // SplitContainer1.Panel2
            // 
            SplitContainer1.Panel2.Controls.Add(Panel1);
            SplitContainer1.Size = new Size(659, 471);
            SplitContainer1.SplitterDistance = 351;
            SplitContainer1.SplitterWidth = 7;
            SplitContainer1.TabIndex = 9;
            // 
            // Panel2
            // 
            Panel2.Controls.Add(pnlFilter);
            Panel2.Controls.Add(btnFilter);
            Panel2.Controls.Add(btnClear);
            Panel2.Controls.Add(DataGridView1);
            Panel2.Dock = DockStyle.Fill;
            Panel2.Location = new Point(0, 0);
            Panel2.Margin = new Padding(4);
            Panel2.Name = "Panel2";
            Panel2.Size = new Size(351, 471);
            Panel2.TabIndex = 0;
            // 
            // pnlFilter
            // 
            pnlFilter.Controls.Add(btnFilterReset);
            pnlFilter.Controls.Add(btnFilterCancel);
            pnlFilter.Controls.Add(btnFilterOK);
            pnlFilter.Controls.Add(label3);
            pnlFilter.Controls.Add(label2);
            pnlFilter.Controls.Add(chkFilterByTime);
            pnlFilter.Controls.Add(pnlFilterByTime);
            pnlFilter.Controls.Add(chkLstReaderWriter);
            pnlFilter.Controls.Add(chkLstBlazor);
            pnlFilter.Location = new Point(3, 29);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new Size(321, 333);
            pnlFilter.TabIndex = 9;
            pnlFilter.Visible = false;
            // 
            // btnFilterReset
            // 
            btnFilterReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFilterReset.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnFilterReset.Location = new Point(4, 298);
            btnFilterReset.Margin = new Padding(4);
            btnFilterReset.Name = "btnFilterReset";
            btnFilterReset.Size = new Size(77, 27);
            btnFilterReset.TabIndex = 25;
            btnFilterReset.Text = "リセット";
            btnFilterReset.UseVisualStyleBackColor = true;
            btnFilterReset.Click += btnFilterReset_Click;
            // 
            // btnFilterCancel
            // 
            btnFilterCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFilterCancel.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnFilterCancel.Location = new Point(238, 298);
            btnFilterCancel.Margin = new Padding(4);
            btnFilterCancel.Name = "btnFilterCancel";
            btnFilterCancel.Size = new Size(77, 27);
            btnFilterCancel.TabIndex = 24;
            btnFilterCancel.Text = "キャンセル";
            btnFilterCancel.UseVisualStyleBackColor = true;
            btnFilterCancel.Click += btnFilterOK_Click;
            // 
            // btnFilterOK
            // 
            btnFilterOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFilterOK.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnFilterOK.Location = new Point(158, 298);
            btnFilterOK.Margin = new Padding(4);
            btnFilterOK.Name = "btnFilterOK";
            btnFilterOK.Size = new Size(77, 27);
            btnFilterOK.TabIndex = 23;
            btnFilterOK.Text = "確定";
            btnFilterOK.UseVisualStyleBackColor = true;
            btnFilterOK.Click += btnFilterOK_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(4, 134);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(80, 15);
            label3.TabIndex = 22;
            label3.Text = "Reader/Writer";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 4);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 21;
            label2.Text = "Blazor";
            // 
            // chkFilterByTime
            // 
            chkFilterByTime.AutoSize = true;
            chkFilterByTime.Location = new Point(8, 273);
            chkFilterByTime.Name = "chkFilterByTime";
            chkFilterByTime.Size = new Size(15, 14);
            chkFilterByTime.TabIndex = 20;
            chkFilterByTime.UseVisualStyleBackColor = true;
            chkFilterByTime.CheckedChanged += chkFilterByTime_CheckedChanged;
            // 
            // pnlFilterByTime
            // 
            pnlFilterByTime.Controls.Add(numFilterByTime);
            pnlFilterByTime.Controls.Add(label1);
            pnlFilterByTime.Enabled = false;
            pnlFilterByTime.Location = new Point(24, 267);
            pnlFilterByTime.Name = "pnlFilterByTime";
            pnlFilterByTime.Size = new Size(211, 28);
            pnlFilterByTime.TabIndex = 19;
            // 
            // numFilterByTime
            // 
            numFilterByTime.Location = new Point(3, 3);
            numFilterByTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numFilterByTime.Name = "numFilterByTime";
            numFilterByTime.Size = new Size(65, 23);
            numFilterByTime.TabIndex = 2;
            numFilterByTime.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(71, 6);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 18;
            label1.Text = "ms以上かかったクエリのみ";
            label1.Click += label1_Click;
            // 
            // chkLstReaderWriter
            // 
            chkLstReaderWriter.CheckOnClick = true;
            chkLstReaderWriter.FormattingEnabled = true;
            chkLstReaderWriter.Location = new Point(4, 152);
            chkLstReaderWriter.Name = "chkLstReaderWriter";
            chkLstReaderWriter.Size = new Size(311, 112);
            chkLstReaderWriter.TabIndex = 1;
            chkLstReaderWriter.ItemCheck += chkLstBlazor_ItemCheck;
            // 
            // chkLstBlazor
            // 
            chkLstBlazor.CheckOnClick = true;
            chkLstBlazor.FormattingEnabled = true;
            chkLstBlazor.Items.AddRange(new object[] { "chkLstBlazor" });
            chkLstBlazor.Location = new Point(4, 22);
            chkLstBlazor.Name = "chkLstBlazor";
            chkLstBlazor.Size = new Size(311, 112);
            chkLstBlazor.TabIndex = 0;
            chkLstBlazor.ItemCheck += chkLstBlazor_ItemCheck;
            // 
            // btnFilter
            // 
            btnFilter.BackgroundImageLayout = ImageLayout.Zoom;
            btnFilter.FlatStyle = FlatStyle.Flat;
            btnFilter.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnFilter.Location = new Point(5, 2);
            btnFilter.Margin = new Padding(4);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(42, 26);
            btnFilter.TabIndex = 8;
            btnFilter.Text = "ﾌｨﾙﾀ";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnClear.Location = new Point(289, 2);
            btnClear.Margin = new Padding(4);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(60, 27);
            btnClear.TabIndex = 7;
            btnClear.Text = "クリア";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // Timer1
            // 
            Timer1.Interval = 500;
            Timer1.Tick += Timer1_Tick;
            // 
            // MenuStrip1
            // 
            MenuStrip1.Items.AddRange(new ToolStripItem[] { ファイルFToolStripMenuItem });
            MenuStrip1.Location = new Point(0, 0);
            MenuStrip1.Name = "MenuStrip1";
            MenuStrip1.Padding = new Padding(7, 2, 0, 2);
            MenuStrip1.Size = new Size(659, 24);
            MenuStrip1.TabIndex = 14;
            MenuStrip1.Text = "MenuStrip1";
            // 
            // ファイルFToolStripMenuItem
            // 
            ファイルFToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 閉じるXToolStripMenuItem });
            ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
            ファイルFToolStripMenuItem.Size = new Size(67, 20);
            ファイルFToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // 閉じるXToolStripMenuItem
            // 
            閉じるXToolStripMenuItem.Name = "閉じるXToolStripMenuItem";
            閉じるXToolStripMenuItem.Size = new Size(119, 22);
            閉じるXToolStripMenuItem.Text = "閉じる(&X)";
            閉じるXToolStripMenuItem.Click += 閉じるXToolStripMenuItem_Click;
            // 
            // frmSQLWatcher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(659, 495);
            Controls.Add(SplitContainer1);
            Controls.Add(MenuStrip1);
            MainMenuStrip = MenuStrip1;
            Margin = new Padding(4);
            Name = "frmSQLWatcher";
            Text = "クエリ監視";
            Load += DebugForm_Load;
            ((ISupportInitialize)DataGridView1).EndInit();
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            SplitContainer1.Panel1.ResumeLayout(false);
            SplitContainer1.Panel2.ResumeLayout(false);
            ((ISupportInitialize)SplitContainer1).EndInit();
            SplitContainer1.ResumeLayout(false);
            Panel2.ResumeLayout(false);
            pnlFilter.ResumeLayout(false);
            pnlFilter.PerformLayout();
            pnlFilterByTime.ResumeLayout(false);
            pnlFilterByTime.PerformLayout();
            ((ISupportInitialize)numFilterByTime).EndInit();
            MenuStrip1.ResumeLayout(false);
            MenuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridView DataGridView1;
        private Panel Panel1;
        private Label lblReaderWriterLineNum;
        private Button btnJumpToReaderWriter;
        private Label lblElapsedTime;
        private SplitContainer SplitContainer1;
        private System.Windows.Forms.Timer Timer1;
        private RichTextBox RichTextBox1;
        private Panel Panel2;
        private CheckBox chkTopMost;
        private Button btnClear;
        private ListBox lstTables;
        private Button btnTableDefinition;
        private MenuStrip MenuStrip1;
        private ToolStripMenuItem ファイルFToolStripMenuItem;
        private ToolStripMenuItem 閉じるXToolStripMenuItem;
        private CheckBox chkTopmostOnReceive;
        private TextBox txtParam;
        private Button btnJumpToBlazor;
        private TextBox txtReaderWriterFilePath;
        private TextBox txtBlazorFilePath;
        private Label lblBlazorLineNum;
        private Button btnFilter;
        private Panel pnlFilter;
        private Panel pnlFilterByTime;
        private Label label1;
        private NumericUpDown numFilterByTime;
        private CheckedListBox chkLstReaderWriter;
        private CheckedListBox chkLstBlazor;
        private CheckBox chkFilterByTime;
        private Button btnFilterCancel;
        private Button btnFilterOK;
        private Label label3;
        private Label label2;
        private Button btnFilterReset;
    }
}
