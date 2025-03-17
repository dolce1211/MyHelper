using System.Drawing;
using System.Windows.Forms;

namespace MyHelper.Dialogs
{
    partial class frmSetting
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
            btnOK = new Button();
            btnCancel = new Button();
            btnAsIsSlnPath = new Button();
            txtAsIsSlnPath = new TextBox();
            Label2 = new Label();
            GroupBox1 = new GroupBox();
            chkEnableLauncherByMouseButton = new CheckBox();
            chkEnableLauncherByCtrlKeys = new CheckBox();
            chkOpenValidPathByCtrlCC = new CheckBox();
            GroupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(76, 91);
            btnOK.Margin = new Padding(4);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(90, 30);
            btnOK.TabIndex = 0;
            btnOK.Text = "確定(&S)";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(174, 91);
            btnCancel.Margin = new Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "キャンセル(&X)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAsIsSlnPath
            // 
            btnAsIsSlnPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAsIsSlnPath.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            btnAsIsSlnPath.Location = new Point(236, 26);
            btnAsIsSlnPath.Margin = new Padding(4);
            btnAsIsSlnPath.Name = "btnAsIsSlnPath";
            btnAsIsSlnPath.Size = new Size(28, 30);
            btnAsIsSlnPath.TabIndex = 2;
            btnAsIsSlnPath.Text = "...";
            btnAsIsSlnPath.UseVisualStyleBackColor = true;
            btnAsIsSlnPath.Click += btnAsIsPath_Click;
            // 
            // txtAsIsSlnPath
            // 
            txtAsIsSlnPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAsIsSlnPath.Font = new Font("MS UI Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            txtAsIsSlnPath.Location = new Point(155, 30);
            txtAsIsSlnPath.Margin = new Padding(4);
            txtAsIsSlnPath.Name = "txtAsIsSlnPath";
            txtAsIsSlnPath.Size = new Size(81, 19);
            txtAsIsSlnPath.TabIndex = 3;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Label2.Location = new Point(115, 30);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(33, 18);
            Label2.TabIndex = 5;
            Label2.Text = "AsIs";
            // 
            // GroupBox1
            // 
            GroupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            GroupBox1.Controls.Add(txtAsIsSlnPath);
            GroupBox1.Controls.Add(Label2);
            GroupBox1.Controls.Add(btnAsIsSlnPath);
            GroupBox1.Font = new Font("メイリオ", 9F, FontStyle.Bold, GraphicsUnit.Point, 128);
            GroupBox1.Location = new Point(379, 13);
            GroupBox1.Margin = new Padding(4);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Padding = new Padding(4);
            GroupBox1.Size = new Size(272, 66);
            GroupBox1.TabIndex = 6;
            GroupBox1.TabStop = false;
            GroupBox1.Text = ".slnファイルのパス";
            GroupBox1.Visible = false;
            // 
            // chkEnableLauncherByMouseButton
            // 
            chkEnableLauncherByMouseButton.AutoSize = true;
            chkEnableLauncherByMouseButton.Location = new Point(11, 33);
            chkEnableLauncherByMouseButton.Margin = new Padding(4);
            chkEnableLauncherByMouseButton.Name = "chkEnableLauncherByMouseButton";
            chkEnableLauncherByMouseButton.Size = new Size(253, 19);
            chkEnableLauncherByMouseButton.TabIndex = 14;
            chkEnableLauncherByMouseButton.Text = "左右マウスボタン同時押しで簡易ランチャーを開く";
            chkEnableLauncherByMouseButton.UseVisualStyleBackColor = true;
            // 
            // chkEnableLauncherByCtrlKeys
            // 
            chkEnableLauncherByCtrlKeys.AutoSize = true;
            chkEnableLauncherByCtrlKeys.Location = new Point(11, 60);
            chkEnableLauncherByCtrlKeys.Margin = new Padding(4);
            chkEnableLauncherByCtrlKeys.Name = "chkEnableLauncherByCtrlKeys";
            chkEnableLauncherByCtrlKeys.Size = new Size(243, 19);
            chkEnableLauncherByCtrlKeys.TabIndex = 15;
            chkEnableLauncherByCtrlKeys.Text = "左右のCtrlキー同時押しで簡易ランチャーを開く";
            chkEnableLauncherByCtrlKeys.UseVisualStyleBackColor = true;
            // 
            // chkOpenValidPathByCtrlCC
            // 
            chkOpenValidPathByCtrlCC.AutoSize = true;
            chkOpenValidPathByCtrlCC.Location = new Point(11, 6);
            chkOpenValidPathByCtrlCC.Margin = new Padding(4);
            chkOpenValidPathByCtrlCC.Name = "chkOpenValidPathByCtrlCC";
            chkOpenValidPathByCtrlCC.Size = new Size(245, 19);
            chkOpenValidPathByCtrlCC.TabIndex = 16;
            chkOpenValidPathByCtrlCC.Text = "有効なパスを選択してCtrl+CCでそのパスを開く";
            chkOpenValidPathByCtrlCC.UseVisualStyleBackColor = true;
            // 
            // frmSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(277, 134);
            Controls.Add(chkOpenValidPathByCtrlCC);
            Controls.Add(chkEnableLauncherByCtrlKeys);
            Controls.Add(chkEnableLauncherByMouseButton);
            Controls.Add(GroupBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4);
            Name = "frmSetting";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "設定";
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAsIsSlnPath;
        private System.Windows.Forms.TextBox txtAsIsSlnPath;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.CheckBox chkEnableLauncherByMouseButton;
        private System.Windows.Forms.CheckBox chkEnableLauncherByCtrlKeys;
        private CheckBox chkOpenValidPathByCtrlCC;
    }
}

