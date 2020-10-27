namespace TVDCEG
{
    partial class FrmCutVoidConnection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCutVoidConnection));
            this.lb_Connection = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Txt_Search = new System.Windows.Forms.TextBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.lb_Typecut = new System.Windows.Forms.ListBox();
            this.TypeCut = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_Connection
            // 
            this.lb_Connection.FormattingEnabled = true;
            this.lb_Connection.Location = new System.Drawing.Point(12, 47);
            this.lb_Connection.Name = "lb_Connection";
            this.lb_Connection.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Connection.Size = new System.Drawing.Size(304, 329);
            this.lb_Connection.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search:";
            // 
            // Txt_Search
            // 
            this.Txt_Search.Location = new System.Drawing.Point(53, 12);
            this.Txt_Search.Name = "Txt_Search";
            this.Txt_Search.Size = new System.Drawing.Size(263, 20);
            this.Txt_Search.TabIndex = 2;
            this.Txt_Search.TextChanged += new System.EventHandler(this.Txt_Search_TextChanged);
            // 
            // btn_OK
            // 
            this.btn_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_OK.Location = new System.Drawing.Point(482, 384);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 25);
            this.btn_OK.TabIndex = 4;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(563, 384);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 25);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // lb_Typecut
            // 
            this.lb_Typecut.FormattingEnabled = true;
            this.lb_Typecut.Location = new System.Drawing.Point(336, 47);
            this.lb_Typecut.Name = "lb_Typecut";
            this.lb_Typecut.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_Typecut.Size = new System.Drawing.Size(304, 329);
            this.lb_Typecut.TabIndex = 6;
            // 
            // TypeCut
            // 
            this.TypeCut.AutoSize = true;
            this.TypeCut.Location = new System.Drawing.Point(333, 16);
            this.TypeCut.Name = "TypeCut";
            this.TypeCut.Size = new System.Drawing.Size(113, 13);
            this.TypeCut.TabIndex = 7;
            this.TypeCut.Text = "Select Type in project:";
            // 
            // FrmCutVoidConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(650, 418);
            this.Controls.Add(this.TypeCut);
            this.Controls.Add(this.lb_Typecut);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.Txt_Search);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_Connection);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCutVoidConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Cut Void Connections";
            this.Load += new System.EventHandler(this.FrmCutVoidConnection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lb_Connection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Txt_Search;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ListBox lb_Typecut;
        private System.Windows.Forms.Label TypeCut;
    }
}