namespace TVDCEG
{
    partial class FrmModify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmModify));
            this.Btn_OK = new System.Windows.Forms.Button();
            this.EMBEDSTANDARD = new System.Windows.Forms.RadioButton();
            this.EMBEDSTANDARD_finish = new System.Windows.Forms.RadioButton();
            this.EMBEDCUSTOM = new System.Windows.Forms.RadioButton();
            this.EMBEDCUSTOM_FINISH = new System.Windows.Forms.RadioButton();
            this.CIPCUSTOM = new System.Windows.Forms.RadioButton();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.CIPSTANDARD = new System.Windows.Forms.RadioButton();
            this.ERECTIONSTANDARD = new System.Windows.Forms.RadioButton();
            this.ERECTIONSTANDARD_FINISH = new System.Windows.Forms.RadioButton();
            this.Keep_data = new System.Windows.Forms.RadioButton();
            this.AddPanew = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // Btn_OK
            // 
            this.Btn_OK.Location = new System.Drawing.Point(229, 190);
            this.Btn_OK.Name = "Btn_OK";
            this.Btn_OK.Size = new System.Drawing.Size(75, 23);
            this.Btn_OK.TabIndex = 1;
            this.Btn_OK.Text = "OK";
            this.Btn_OK.UseVisualStyleBackColor = true;
            this.Btn_OK.Click += new System.EventHandler(this.Btn_OK_Click);
            // 
            // EMBEDSTANDARD
            // 
            this.EMBEDSTANDARD.AutoSize = true;
            this.EMBEDSTANDARD.Location = new System.Drawing.Point(12, 21);
            this.EMBEDSTANDARD.Name = "EMBEDSTANDARD";
            this.EMBEDSTANDARD.Size = new System.Drawing.Size(126, 17);
            this.EMBEDSTANDARD.TabIndex = 4;
            this.EMBEDSTANDARD.TabStop = true;
            this.EMBEDSTANDARD.Text = "EMBED STANDARD";
            this.EMBEDSTANDARD.UseVisualStyleBackColor = true;
            // 
            // EMBEDSTANDARD_finish
            // 
            this.EMBEDSTANDARD_finish.AutoSize = true;
            this.EMBEDSTANDARD_finish.Location = new System.Drawing.Point(12, 54);
            this.EMBEDSTANDARD_finish.Name = "EMBEDSTANDARD_finish";
            this.EMBEDSTANDARD_finish.Size = new System.Drawing.Size(170, 17);
            this.EMBEDSTANDARD_finish.TabIndex = 5;
            this.EMBEDSTANDARD_finish.TabStop = true;
            this.EMBEDSTANDARD_finish.Text = "EMBED STANDARD (FINISH)";
            this.EMBEDSTANDARD_finish.UseVisualStyleBackColor = true;
            // 
            // EMBEDCUSTOM
            // 
            this.EMBEDCUSTOM.AutoSize = true;
            this.EMBEDCUSTOM.Location = new System.Drawing.Point(12, 87);
            this.EMBEDCUSTOM.Name = "EMBEDCUSTOM";
            this.EMBEDCUSTOM.Size = new System.Drawing.Size(112, 17);
            this.EMBEDCUSTOM.TabIndex = 6;
            this.EMBEDCUSTOM.TabStop = true;
            this.EMBEDCUSTOM.Text = "EMBED CUSTOM";
            this.EMBEDCUSTOM.UseVisualStyleBackColor = true;
            this.EMBEDCUSTOM.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // EMBEDCUSTOM_FINISH
            // 
            this.EMBEDCUSTOM_FINISH.AutoSize = true;
            this.EMBEDCUSTOM_FINISH.Location = new System.Drawing.Point(12, 119);
            this.EMBEDCUSTOM_FINISH.Name = "EMBEDCUSTOM_FINISH";
            this.EMBEDCUSTOM_FINISH.Size = new System.Drawing.Size(156, 17);
            this.EMBEDCUSTOM_FINISH.TabIndex = 7;
            this.EMBEDCUSTOM_FINISH.TabStop = true;
            this.EMBEDCUSTOM_FINISH.Text = "EMBED CUSTOM (FINISH)";
            this.EMBEDCUSTOM_FINISH.UseVisualStyleBackColor = true;
            // 
            // CIPCUSTOM
            // 
            this.CIPCUSTOM.AutoSize = true;
            this.CIPCUSTOM.Location = new System.Drawing.Point(223, 54);
            this.CIPCUSTOM.Name = "CIPCUSTOM";
            this.CIPCUSTOM.Size = new System.Drawing.Size(91, 17);
            this.CIPCUSTOM.TabIndex = 8;
            this.CIPCUSTOM.TabStop = true;
            this.CIPCUSTOM.Text = "CIP CUSTOM";
            this.CIPCUSTOM.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(320, 190);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // CIPSTANDARD
            // 
            this.CIPSTANDARD.AutoSize = true;
            this.CIPSTANDARD.Location = new System.Drawing.Point(223, 21);
            this.CIPSTANDARD.Name = "CIPSTANDARD";
            this.CIPSTANDARD.Size = new System.Drawing.Size(105, 17);
            this.CIPSTANDARD.TabIndex = 10;
            this.CIPSTANDARD.TabStop = true;
            this.CIPSTANDARD.Text = "CIP STANDARD";
            this.CIPSTANDARD.UseVisualStyleBackColor = true;
            // 
            // ERECTIONSTANDARD
            // 
            this.ERECTIONSTANDARD.AutoSize = true;
            this.ERECTIONSTANDARD.Location = new System.Drawing.Point(223, 87);
            this.ERECTIONSTANDARD.Name = "ERECTIONSTANDARD";
            this.ERECTIONSTANDARD.Size = new System.Drawing.Size(143, 17);
            this.ERECTIONSTANDARD.TabIndex = 11;
            this.ERECTIONSTANDARD.TabStop = true;
            this.ERECTIONSTANDARD.Text = "ERECTION STANDARD";
            this.ERECTIONSTANDARD.UseVisualStyleBackColor = true;
            this.ERECTIONSTANDARD.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged_1);
            // 
            // ERECTIONSTANDARD_FINISH
            // 
            this.ERECTIONSTANDARD_FINISH.AutoSize = true;
            this.ERECTIONSTANDARD_FINISH.Location = new System.Drawing.Point(223, 119);
            this.ERECTIONSTANDARD_FINISH.Name = "ERECTIONSTANDARD_FINISH";
            this.ERECTIONSTANDARD_FINISH.Size = new System.Drawing.Size(187, 17);
            this.ERECTIONSTANDARD_FINISH.TabIndex = 12;
            this.ERECTIONSTANDARD_FINISH.TabStop = true;
            this.ERECTIONSTANDARD_FINISH.Text = "ERECTION STANDARD (FINISH)";
            this.ERECTIONSTANDARD_FINISH.UseVisualStyleBackColor = true;
            // 
            // Keep_data
            // 
            this.Keep_data.AutoSize = true;
            this.Keep_data.Location = new System.Drawing.Point(223, 149);
            this.Keep_data.Name = "Keep_data";
            this.Keep_data.Size = new System.Drawing.Size(76, 17);
            this.Keep_data.TabIndex = 13;
            this.Keep_data.TabStop = true;
            this.Keep_data.Text = "Keep Data";
            this.Keep_data.UseVisualStyleBackColor = true;
            // 
            // AddPanew
            // 
            this.AddPanew.AutoSize = true;
            this.AddPanew.Location = new System.Drawing.Point(12, 149);
            this.AddPanew.Name = "AddPanew";
            this.AddPanew.Size = new System.Drawing.Size(84, 17);
            this.AddPanew.TabIndex = 14;
            this.AddPanew.TabStop = true;
            this.AddPanew.Text = "Add PA new";
            this.AddPanew.UseVisualStyleBackColor = true;
            this.AddPanew.CheckedChanged += new System.EventHandler(this.AddPanew_CheckedChanged);
            // 
            // FrmModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 225);
            this.Controls.Add(this.AddPanew);
            this.Controls.Add(this.Keep_data);
            this.Controls.Add(this.ERECTIONSTANDARD_FINISH);
            this.Controls.Add(this.ERECTIONSTANDARD);
            this.Controls.Add(this.CIPSTANDARD);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.CIPCUSTOM);
            this.Controls.Add(this.EMBEDCUSTOM_FINISH);
            this.Controls.Add(this.EMBEDCUSTOM);
            this.Controls.Add(this.EMBEDSTANDARD_finish);
            this.Controls.Add(this.Btn_OK);
            this.Controls.Add(this.EMBEDSTANDARD);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmModify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Add Parameter";
            this.Load += new System.EventHandler(this.FrmModify_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_OK;
        private System.Windows.Forms.RadioButton EMBEDSTANDARD;
        private System.Windows.Forms.RadioButton EMBEDSTANDARD_finish;
        private System.Windows.Forms.RadioButton EMBEDCUSTOM;
        private System.Windows.Forms.RadioButton EMBEDCUSTOM_FINISH;
        private System.Windows.Forms.RadioButton CIPCUSTOM;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.RadioButton CIPSTANDARD;
        private System.Windows.Forms.RadioButton ERECTIONSTANDARD;
        private System.Windows.Forms.RadioButton ERECTIONSTANDARD_FINISH;
        private System.Windows.Forms.RadioButton Keep_data;
        private System.Windows.Forms.RadioButton AddPanew;
    }
}