namespace MyHelper.Dialogs
{
    partial class frmTableDefinition
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chkFix = new CheckBox();
            lblTitle = new Label();
            DataGridView1 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            btnCloseAll = new Button();
            ((System.ComponentModel.ISupportInitialize)DataGridView1).BeginInit();
            SuspendLayout();
            // 
            // chkFix
            // 
            chkFix.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkFix.AutoSize = true;
            chkFix.BackColor = Color.PaleTurquoise;
            chkFix.Location = new Point(551, 4);
            chkFix.Margin = new Padding(4);
            chkFix.Name = "chkFix";
            chkFix.Size = new Size(50, 19);
            chkFix.TabIndex = 18;
            chkFix.Text = "固定";
            chkFix.UseVisualStyleBackColor = false;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitle.BackColor = Color.PaleTurquoise;
            lblTitle.BorderStyle = BorderStyle.FixedSingle;
            lblTitle.Location = new Point(2, 2);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(600, 22);
            lblTitle.TabIndex = 17;
            lblTitle.Text = "SomeTable - なんとかテーブル";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.MouseDown += frmTableDefinition_MouseDown;
            lblTitle.MouseMove += frmTableDefinition_MouseMove;
            lblTitle.MouseUp += frmTableDefinition_MouseUp;
            // 
            // DataGridView1
            // 
            DataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.Location = new Point(2, 28);
            DataGridView1.Margin = new Padding(4);
            DataGridView1.Name = "DataGridView1";
            DataGridView1.RowTemplate.Height = 21;
            DataGridView1.Size = new Size(504, 420);
            DataGridView1.TabIndex = 19;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(510, 28);
            button1.Name = "button1";
            button1.Size = new Size(90, 26);
            button1.TabIndex = 20;
            button1.Text = "reader開く";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(510, 60);
            button2.Name = "button2";
            button2.Size = new Size(90, 26);
            button2.TabIndex = 21;
            button2.Text = "writer開く";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button1_Click;
            // 
            // btnCloseAll
            // 
            btnCloseAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCloseAll.Location = new Point(510, 380);
            btnCloseAll.Name = "btnCloseAll";
            btnCloseAll.Size = new Size(90, 26);
            btnCloseAll.TabIndex = 22;
            btnCloseAll.Text = "全て閉じる";
            btnCloseAll.UseVisualStyleBackColor = true;
            btnCloseAll.Click += btnCloseAll_Click;
            // 
            // frmTableDefinition
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(603, 454);
            Controls.Add(btnCloseAll);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(DataGridView1);
            Controls.Add(chkFix);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            Name = "frmTableDefinition";
            Text = "frmTableDefinition";
            Load += frmTableDefinition_Load;
            KeyDown += frmTableDefinition_KeyDown;
            MouseDown += frmTableDefinition_MouseDown;
            MouseMove += frmTableDefinition_MouseMove;
            MouseUp += frmTableDefinition_MouseUp;
            ((System.ComponentModel.ISupportInitialize)DataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox chkFix;
        private Label lblTitle;
        private DataGridView DataGridView1;
        private Button button1;
        private Button button2;
        private Button btnCloseAll;
    }
}