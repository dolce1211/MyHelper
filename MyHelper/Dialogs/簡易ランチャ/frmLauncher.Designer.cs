using MyHelper.Domain;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    partial class frmLauncher : Form
    {
        private System.ComponentModel.IContainer components = null;

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
            components = new System.ComponentModel.Container();
            Label1 = new Label();
            lstLauncher = new ListBox();
            ContextMenuStrip1 = new ContextMenuStrip(components);
            開くToolStripMenuItem = new ToolStripMenuItem();
            ToolStripSeparator2 = new ToolStripSeparator();
            編集ToolStripMenuItem = new ToolStripMenuItem();
            ToolStripSeparator1 = new ToolStripSeparator();
            削除XToolStripMenuItem = new ToolStripMenuItem();
            btnUp = new Button();
            btnDown = new Button();
            btnAdd = new Button();
            btnEdit = new Button();
            txtFilter = new TextBox();
            Label2 = new Label();
            Label3 = new Label();
            chkFix = new CheckBox();
            AutoCompleteTextBox1 = new AutoCompleteTextBox();
            ContextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label1.Location = new Point(5, 9);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(80, 18);
            Label1.TabIndex = 1;
            Label1.Text = "なんでも検索";
            // 
            // lstLauncher
            // 
            lstLauncher.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstLauncher.ContextMenuStrip = ContextMenuStrip1;
            lstLauncher.FormattingEnabled = true;
            lstLauncher.IntegralHeight = false;
            lstLauncher.ItemHeight = 15;
            lstLauncher.Location = new Point(4, 118);
            lstLauncher.Margin = new Padding(4);
            lstLauncher.Name = "lstLauncher";
            lstLauncher.Size = new Size(428, 179);
            lstLauncher.TabIndex = 15;
            lstLauncher.Format += lstLauncher_Format;
            lstLauncher.DoubleClick += lstLauncher_DoubleClick;
            lstLauncher.KeyDown += lstLauncher_KeyDown;
            lstLauncher.MouseDown += lstLauncher_MouseDown;
            // 
            // ContextMenuStrip1
            // 
            ContextMenuStrip1.Items.AddRange(new ToolStripItem[] { 開くToolStripMenuItem, ToolStripSeparator2, 編集ToolStripMenuItem, ToolStripSeparator1, 削除XToolStripMenuItem });
            ContextMenuStrip1.Name = "ContextMenuStrip1";
            ContextMenuStrip1.Size = new Size(114, 82);
            // 
            // 開くToolStripMenuItem
            // 
            開くToolStripMenuItem.Name = "開くToolStripMenuItem";
            開くToolStripMenuItem.Size = new Size(113, 22);
            開くToolStripMenuItem.Text = "開く";
            開くToolStripMenuItem.Click += 開くToolStripMenuItem_Click;
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(110, 6);
            // 
            // 編集ToolStripMenuItem
            // 
            編集ToolStripMenuItem.Name = "編集ToolStripMenuItem";
            編集ToolStripMenuItem.Size = new Size(113, 22);
            編集ToolStripMenuItem.Text = "編集(&E)";
            編集ToolStripMenuItem.Click += 編集ToolStripMenuItem_Click;
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(110, 6);
            // 
            // 削除XToolStripMenuItem
            // 
            削除XToolStripMenuItem.Name = "削除XToolStripMenuItem";
            削除XToolStripMenuItem.Size = new Size(113, 22);
            削除XToolStripMenuItem.Text = "削除(&X)";
            削除XToolStripMenuItem.Click += 削除XToolStripMenuItem_Click;
            // 
            // btnUp
            // 
            btnUp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUp.Location = new Point(369, 90);
            btnUp.Margin = new Padding(4);
            btnUp.Name = "btnUp";
            btnUp.Size = new Size(31, 25);
            btnUp.TabIndex = 13;
            btnUp.Text = "↑";
            btnUp.UseVisualStyleBackColor = true;
            btnUp.Click += btnUp_Click;
            // 
            // btnDown
            // 
            btnDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDown.Location = new Point(400, 90);
            btnDown.Margin = new Padding(4);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(31, 25);
            btnDown.TabIndex = 14;
            btnDown.Text = "↓";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // btnAdd
            // 
            btnAdd.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnAdd.Location = new Point(4, 92);
            btnAdd.Margin = new Padding(4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(39, 25);
            btnAdd.TabIndex = 11;
            btnAdd.Text = "追加";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnEdit
            // 
            btnEdit.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnEdit.Location = new Point(42, 92);
            btnEdit.Margin = new Padding(4);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(39, 25);
            btnEdit.TabIndex = 12;
            btnEdit.Text = "編集";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // txtFilter
            // 
            txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilter.Location = new Point(100, 64);
            txtFilter.Margin = new Padding(4);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(331, 23);
            txtFilter.TabIndex = 10;
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label2.Location = new Point(5, 67);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(80, 18);
            Label2.TabIndex = 8;
            Label2.Text = "ランチャ絞込";
            // 
            // Label3
            // 
            Label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label3.BackColor = Color.PaleTurquoise;
            Label3.BorderStyle = BorderStyle.FixedSingle;
            Label3.Location = new Point(2, 36);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(431, 22);
            Label3.TabIndex = 9;
            Label3.Text = "簡易ランチャ";
            Label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // chkFix
            // 
            chkFix.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkFix.AutoSize = true;
            chkFix.BackColor = Color.PaleTurquoise;
            chkFix.Location = new Point(383, 38);
            chkFix.Margin = new Padding(4);
            chkFix.Name = "chkFix";
            chkFix.Size = new Size(50, 19);
            chkFix.TabIndex = 16;
            chkFix.Text = "固定";
            chkFix.UseVisualStyleBackColor = false;
            // 
            // AutoCompleteTextBox1
            // 
            AutoCompleteTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            AutoCompleteTextBox1.Location = new Point(102, 6);
            AutoCompleteTextBox1.Margin = new Padding(4);
            AutoCompleteTextBox1.Name = "AutoCompleteTextBox1";
            AutoCompleteTextBox1.Size = new Size(330, 23);
            AutoCompleteTextBox1.TabIndex = 0;
            AutoCompleteTextBox1.AutoCompleteTextBoxValidated += AutoCompleteTextBox1_AutoCompleteTextBoxValidated;
            // 
            // frmLauncher
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 300);
            Controls.Add(chkFix);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(txtFilter);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Controls.Add(btnDown);
            Controls.Add(btnUp);
            Controls.Add(lstLauncher);
            Controls.Add(Label1);
            Controls.Add(AutoCompleteTextBox1);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            Margin = new Padding(4);
            Name = "frmLauncher";
            Text = "簡易ランチャ";
            VisibleChanged += frmLauncher_VisibleChanged;

            ContextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private AutoCompleteTextBox AutoCompleteTextBox1;
        private Label Label1;
        private ListBox lstLauncher;
        private Button btnUp;
        private Button btnDown;
        private Button btnAdd;
        private Button btnEdit;
        private TextBox txtFilter;
        private Label Label2;
        private ContextMenuStrip ContextMenuStrip1;
        private ToolStripMenuItem 編集ToolStripMenuItem;
        private ToolStripSeparator ToolStripSeparator1;
        private ToolStripMenuItem 削除XToolStripMenuItem;
        private Label Label3;
        private CheckBox chkFix;
        private ToolStripMenuItem 開くToolStripMenuItem;
        private ToolStripSeparator ToolStripSeparator2;
    }
}
