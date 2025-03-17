using System.Drawing;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    partial class FloatingDialog
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
            lstTable = new ListBox();
            ContextMenuStrip1 = new ContextMenuStrip(components);
            テーブル定義を開くToolStripMenuItem = new ToolStripMenuItem();
            RedMineを開くToolStripMenuItem = new ToolStripMenuItem();
            ContextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // lstTable
            // 
            lstTable.ContextMenuStrip = ContextMenuStrip1;
            lstTable.Dock = DockStyle.Fill;
            lstTable.Font = new Font("MS UI Gothic", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lstTable.FormattingEnabled = true;
            lstTable.IntegralHeight = false;
            lstTable.ItemHeight = 13;
            lstTable.Items.AddRange(new object[] { "aaa", "iii", "uuu", "eee", "ooo", "aaaa" });
            lstTable.Location = new Point(0, 0);
            lstTable.Margin = new Padding(4);
            lstTable.Name = "lstTable";
            lstTable.Size = new Size(246, 42);
            lstTable.TabIndex = 0;
            lstTable.SelectedIndexChanged += lstTable_SelectedIndexChanged;
            lstTable.Format += lstTable_Format;
            lstTable.DoubleClick += lstTable_DoubleClick;
            lstTable.KeyDown += lstTable_KeyDown;
            lstTable.MouseDown += ListBox1_MouseDown;
            lstTable.MouseMove += ListBox1_MouseMove;
            lstTable.MouseUp += ListBox1_MouseUp;
            // 
            // ContextMenuStrip1
            // 
            ContextMenuStrip1.Items.AddRange(new ToolStripItem[] { テーブル定義を開くToolStripMenuItem, RedMineを開くToolStripMenuItem });
            ContextMenuStrip1.Name = "ContextMenuStrip1";
            ContextMenuStrip1.Size = new Size(162, 48);
            ContextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            // 
            // テーブル定義を開くToolStripMenuItem
            // 
            テーブル定義を開くToolStripMenuItem.Name = "テーブル定義を開くToolStripMenuItem";
            テーブル定義を開くToolStripMenuItem.Size = new Size(161, 22);
            テーブル定義を開くToolStripMenuItem.Text = "テーブル定義を開く";
            テーブル定義を開くToolStripMenuItem.Click += テーブル定義を開くToolStripMenuItem_Click;
            // 
            // RedMineを開くToolStripMenuItem
            // 
            RedMineを開くToolStripMenuItem.Name = "RedMineを開くToolStripMenuItem";
            RedMineを開くToolStripMenuItem.Size = new Size(161, 22);
            RedMineを開くToolStripMenuItem.Text = "RedMineを開く";
            RedMineを開くToolStripMenuItem.Click += RedMineを開くToolStripMenuItem_Click;
            // 
            // FloatingDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(246, 42);
            Controls.Add(lstTable);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "FloatingDialog";
            Text = "FloatingDialog";
            Load += MouseTrackingForm_Load;
            ContextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.ListBox lstTable;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem テーブル定義を開くToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RedMineを開くToolStripMenuItem;
    }
}

