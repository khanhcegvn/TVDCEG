namespace TVDCEG
{
    partial class FrmSaveDetach
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSaveDetach));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_Open = new System.Windows.Forms.Button();
            this.Rbtn_Workset = new System.Windows.Forms.RadioButton();
            this.Rbtn_DisWorkset = new System.Windows.Forms.RadioButton();
            this.btn_OK = new System.Windows.Forms.Button();
            this.checkbox_Activedocument = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(484, 20);
            this.textBox1.TabIndex = 0;
            // 
            // btn_Open
            // 
            this.btn_Open.Location = new System.Drawing.Point(534, 24);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(75, 23);
            this.btn_Open.TabIndex = 1;
            this.btn_Open.Text = "Open";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.Btn_Open_Click);
            // 
            // Rbtn_Workset
            // 
            this.Rbtn_Workset.AutoSize = true;
            this.Rbtn_Workset.Location = new System.Drawing.Point(152, 64);
            this.Rbtn_Workset.Name = "Rbtn_Workset";
            this.Rbtn_Workset.Size = new System.Drawing.Size(115, 17);
            this.Rbtn_Workset.TabIndex = 2;
            this.Rbtn_Workset.TabStop = true;
            this.Rbtn_Workset.Text = "Preserve Worksets";
            this.Rbtn_Workset.UseVisualStyleBackColor = true;
            // 
            // Rbtn_DisWorkset
            // 
            this.Rbtn_DisWorkset.AutoSize = true;
            this.Rbtn_DisWorkset.Location = new System.Drawing.Point(294, 64);
            this.Rbtn_DisWorkset.Name = "Rbtn_DisWorkset";
            this.Rbtn_DisWorkset.Size = new System.Drawing.Size(109, 17);
            this.Rbtn_DisWorkset.TabIndex = 3;
            this.Rbtn_DisWorkset.TabStop = true;
            this.Rbtn_DisWorkset.Text = "Discard Worksets";
            this.Rbtn_DisWorkset.UseVisualStyleBackColor = true;
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(534, 61);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 4;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.Btn_OK_Click_1);
            // 
            // checkbox_Activedocument
            // 
            this.checkbox_Activedocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkbox_Activedocument.AutoSize = true;
            this.checkbox_Activedocument.Location = new System.Drawing.Point(23, 65);
            this.checkbox_Activedocument.Name = "checkbox_Activedocument";
            this.checkbox_Activedocument.Size = new System.Drawing.Size(108, 17);
            this.checkbox_Activedocument.TabIndex = 5;
            this.checkbox_Activedocument.Text = "Active Document";
            this.checkbox_Activedocument.UseVisualStyleBackColor = true;
            this.checkbox_Activedocument.CheckedChanged += new System.EventHandler(this.Checkbox_Activedocument_CheckedChanged);
            // 
            // FrmSaveDetach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 99);
            this.Controls.Add(this.checkbox_Activedocument);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.Rbtn_DisWorkset);
            this.Controls.Add(this.Rbtn_Workset);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSaveDetach";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: FrmSaveDetach";
            this.Load += new System.EventHandler(this.FrmSaveDetach_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.RadioButton Rbtn_Workset;
        private System.Windows.Forms.RadioButton Rbtn_DisWorkset;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.CheckBox checkbox_Activedocument;
    }
}