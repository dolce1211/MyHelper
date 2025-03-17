using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    public partial class frmLauncherEdit : Form
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            txtPath = new TextBox();
            lblPathCaption = new Label();
            btnPath = new Button();
            txtTitle = new TextBox();
            Label1 = new Label();
            Panel1 = new Panel();
            rbURL = new RadioButton();
            rbFile = new RadioButton();
            rbFolder = new RadioButton();
            btnCancel = new Button();
            btnOK = new Button();
            chkEditTitle = new CheckBox();
            Label2 = new Label();
            Panel1.SuspendLayout();
            SuspendLayout();
            // 
            // txtPath
            // 
            txtPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPath.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtPath.Location = new Point(100, 106);
            txtPath.Margin = new Padding(4);
            txtPath.Name = "txtPath";
            txtPath.Size = new Size(534, 19);
            txtPath.TabIndex = 20;
            txtPath.TextChanged += txtPath_TextChanged;
            // 
            // lblPathCaption
            // 
            lblPathCaption.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblPathCaption.Location = new Point(2, 108);
            lblPathCaption.Margin = new Padding(4, 0, 4, 0);
            lblPathCaption.Name = "lblPathCaption";
            lblPathCaption.Size = new Size(98, 20);
            lblPathCaption.TabIndex = 8;
            lblPathCaption.Text = "フォルダパス";
            lblPathCaption.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnPath
            // 
            btnPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPath.Location = new Point(636, 102);
            btnPath.Margin = new Padding(4);
            btnPath.Name = "btnPath";
            btnPath.Size = new Size(28, 30);
            btnPath.TabIndex = 21;
            btnPath.Text = "...";
            btnPath.UseVisualStyleBackColor = true;
            btnPath.Click += btnPath_Click;
            // 
            // txtTitle
            // 
            txtTitle.Enabled = false;
            txtTitle.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtTitle.Location = new Point(100, 60);
            txtTitle.Margin = new Padding(4);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(275, 19);
            txtTitle.TabIndex = 10;
            // 
            // Label1
            // 
            Label1.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label1.Location = new Point(2, 61);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(98, 20);
            Label1.TabIndex = 11;
            Label1.Text = "タイトル";
            Label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Panel1
            // 
            Panel1.Controls.Add(rbURL);
            Panel1.Controls.Add(rbFile);
            Panel1.Controls.Add(rbFolder);
            Panel1.Location = new Point(100, 15);
            Panel1.Margin = new Padding(4);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(215, 30);
            Panel1.TabIndex = 12;
            // 
            // rbURL
            // 
            rbURL.AutoSize = true;
            rbURL.Location = new Point(152, 4);
            rbURL.Margin = new Padding(4);
            rbURL.Name = "rbURL";
            rbURL.Size = new Size(46, 19);
            rbURL.TabIndex = 2;
            rbURL.TabStop = true;
            rbURL.Text = "URL";
            rbURL.UseVisualStyleBackColor = true;
            rbURL.CheckedChanged += rbURL_CheckedChanged;
            // 
            // rbFile
            // 
            rbFile.AutoSize = true;
            rbFile.Location = new Point(78, 4);
            rbFile.Margin = new Padding(4);
            rbFile.Name = "rbFile";
            rbFile.Size = new Size(59, 19);
            rbFile.TabIndex = 1;
            rbFile.TabStop = true;
            rbFile.Text = "ファイル";
            rbFile.UseVisualStyleBackColor = true;
            rbFile.CheckedChanged += rbFile_CheckedChanged;
            // 
            // rbFolder
            // 
            rbFolder.AutoSize = true;
            rbFolder.Location = new Point(4, 4);
            rbFolder.Margin = new Padding(4);
            rbFolder.Name = "rbFolder";
            rbFolder.Size = new Size(60, 19);
            rbFolder.TabIndex = 0;
            rbFolder.TabStop = true;
            rbFolder.Text = "フォルダ";
            rbFolder.UseVisualStyleBackColor = true;
            rbFolder.CheckedChanged += rbFolder_CheckedChanged;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(565, 146);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(99, 38);
            btnCancel.TabIndex = 31;
            btnCancel.Text = "キャンセル(&X)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(458, 146);
            btnOK.Margin = new Padding(4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(99, 38);
            btnOK.TabIndex = 30;
            btnOK.Text = "確定(&S)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // chkEditTitle
            // 
            chkEditTitle.AutoSize = true;
            chkEditTitle.Location = new Point(383, 62);
            chkEditTitle.Margin = new Padding(4);
            chkEditTitle.Name = "chkEditTitle";
            chkEditTitle.Size = new Size(114, 19);
            chkEditTitle.TabIndex = 11;
            chkEditTitle.Text = "タイトルを編集する";
            chkEditTitle.UseVisualStyleBackColor = true;
            chkEditTitle.CheckedChanged += chkEditTitle_CheckedChanged;
            // 
            // Label2
            // 
            Label2.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label2.Location = new Point(2, 19);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(98, 20);
            Label2.TabIndex = 16;
            Label2.Text = "形式";
            Label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // frmLauncherEdit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(678, 199);
            Controls.Add(Label2);
            Controls.Add(chkEditTitle);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(Panel1);
            Controls.Add(Label1);
            Controls.Add(txtTitle);
            Controls.Add(txtPath);
            Controls.Add(lblPathCaption);
            Controls.Add(btnPath);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4);
            Name = "frmLauncherEdit";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ランチャー項目編集";
            Load += frmLauncherEdit_Load;
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal TextBox txtPath;
        internal Label lblPathCaption;
        internal Button btnPath;
        internal TextBox txtTitle;
        internal Label Label1;
        internal Panel Panel1;
        internal RadioButton rbURL;
        internal RadioButton rbFile;
        internal RadioButton rbFolder;
        internal Button btnCancel;
        internal Button btnOK;
        internal CheckBox chkEditTitle;
        internal Label Label2;
    }
}