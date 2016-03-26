namespace MonyDataMacro
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.wbMain = new System.Windows.Forms.WebBrowser();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCopy = new System.Windows.Forms.TextBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.tmrPos = new System.Windows.Forms.Timer(this.components);
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnHtmlSource = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wbMain
            // 
            this.wbMain.Location = new System.Drawing.Point(12, 42);
            this.wbMain.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbMain.Name = "wbMain";
            this.wbMain.Size = new System.Drawing.Size(1019, 507);
            this.wbMain.TabIndex = 0;
            this.wbMain.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbMain_DocumentCompleted);
            this.wbMain.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wbMain_Navigating);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(220, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Enter name and pass";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCopy);
            this.groupBox1.Controls.Add(this.lstLog);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(1037, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 537);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // txtCopy
            // 
            this.txtCopy.Location = new System.Drawing.Point(7, 226);
            this.txtCopy.Name = "txtCopy";
            this.txtCopy.Size = new System.Drawing.Size(288, 20);
            this.txtCopy.TabIndex = 3;
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(6, 252);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(289, 277);
            this.lstLog.TabIndex = 2;
            this.lstLog.SelectedIndexChanged += new System.EventHandler(this.lstLog_SelectedIndexChanged);
            // 
            // tmrPos
            // 
            this.tmrPos.Enabled = true;
            this.tmrPos.Interval = 500;
            this.tmrPos.Tick += new System.EventHandler(this.tmrPos_Tick);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(12, 12);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(1019, 20);
            this.txtUrl.TabIndex = 4;
            // 
            // btnHtmlSource
            // 
            this.btnHtmlSource.Location = new System.Drawing.Point(900, 555);
            this.btnHtmlSource.Name = "btnHtmlSource";
            this.btnHtmlSource.Size = new System.Drawing.Size(131, 23);
            this.btnHtmlSource.TabIndex = 5;
            this.btnHtmlSource.Text = "Copy HTML Source";
            this.btnHtmlSource.UseVisualStyleBackColor = true;
            this.btnHtmlSource.Click += new System.EventHandler(this.btnHtmlSource_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 589);
            this.Controls.Add(this.btnHtmlSource);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.wbMain);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbMain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Timer tmrPos;
        private System.Windows.Forms.TextBox txtCopy;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnHtmlSource;
    }
}

