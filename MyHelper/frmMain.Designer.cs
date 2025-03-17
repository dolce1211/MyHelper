using System.Drawing;
using System.Windows.Forms;

namespace MyHelper
{
    partial class frmMain
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
            MenuStrip1 = new MenuStrip();
            ファイルFToolStripMenuItem = new ToolStripMenuItem();
            設定ToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tablescsv再読込ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            閉じるXToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            btnSQLWatch = new Button();
            MenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStrip1
            // 
            MenuStrip1.Items.AddRange(new ToolStripItem[] { ファイルFToolStripMenuItem, toolStripMenuItem1 });
            MenuStrip1.Location = new Point(0, 0);
            MenuStrip1.Name = "MenuStrip1";
            MenuStrip1.Padding = new Padding(7, 2, 0, 2);
            MenuStrip1.Size = new Size(81, 24);
            MenuStrip1.TabIndex = 0;
            MenuStrip1.Text = "menuStrip1";
            // 
            // ファイルFToolStripMenuItem
            // 
            ファイルFToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 設定ToolStripMenuItem1, toolStripSeparator1, tablescsv再読込ToolStripMenuItem, toolStripSeparator2, 閉じるXToolStripMenuItem });
            ファイルFToolStripMenuItem.Name = "ファイルFToolStripMenuItem";
            ファイルFToolStripMenuItem.Size = new Size(67, 20);
            ファイルFToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // 設定ToolStripMenuItem1
            // 
            設定ToolStripMenuItem1.Name = "設定ToolStripMenuItem1";
            設定ToolStripMenuItem1.Size = new Size(161, 22);
            設定ToolStripMenuItem1.Text = "設定";
            設定ToolStripMenuItem1.Click += 設定ToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(158, 6);
            // 
            // tablescsv再読込ToolStripMenuItem
            // 
            tablescsv再読込ToolStripMenuItem.Name = "tablescsv再読込ToolStripMenuItem";
            tablescsv再読込ToolStripMenuItem.Size = new Size(161, 22);
            tablescsv再読込ToolStripMenuItem.Text = "tables.csv再読込";
            tablescsv再読込ToolStripMenuItem.Click += tablescsv再読込ToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(158, 6);
            // 
            // 閉じるXToolStripMenuItem
            // 
            閉じるXToolStripMenuItem.Name = "閉じるXToolStripMenuItem";
            閉じるXToolStripMenuItem.Size = new Size(161, 22);
            閉じるXToolStripMenuItem.Text = "閉じる(&X)";
            閉じるXToolStripMenuItem.Click += 閉じるXToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(12, 20);
            // 
            // btnSQLWatch
            // 
            btnSQLWatch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSQLWatch.Location = new Point(7, 26);
            btnSQLWatch.Margin = new Padding(4);
            btnSQLWatch.Name = "btnSQLWatch";
            btnSQLWatch.Size = new Size(69, 25);
            btnSQLWatch.TabIndex = 1;
            btnSQLWatch.Text = "SQL監視";
            btnSQLWatch.UseVisualStyleBackColor = true;
            btnSQLWatch.Click += btnSQLWatch_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(81, 57);
            Controls.Add(btnSQLWatch);
            Controls.Add(MenuStrip1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MainMenuStrip = MenuStrip1;
            Margin = new Padding(4);
            Name = "frmMain";
            Text = "MyHelper";
            TopMost = true;
            FormClosed += frmMain_FormClosed;
            Load += frmMain_Load;
            MenuStrip1.ResumeLayout(false);
            MenuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.MenuStrip MenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ファイルFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 閉じるXToolStripMenuItem;
        private System.Windows.Forms.Button btnSQLWatch;
        private ToolStripMenuItem 設定ToolStripMenuItem1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem tablescsv再読込ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
    }
}

