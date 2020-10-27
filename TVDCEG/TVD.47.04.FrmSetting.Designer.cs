namespace TVDCEG
{
    partial class FrmSettingManagerSection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettingManagerSection));
            this.cbb_FamilySymbol = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbb_Parameter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbb_FamilySymbol
            // 
            this.cbb_FamilySymbol.FormattingEnabled = true;
            this.cbb_FamilySymbol.Location = new System.Drawing.Point(97, 22);
            this.cbb_FamilySymbol.Name = "cbb_FamilySymbol";
            this.cbb_FamilySymbol.Size = new System.Drawing.Size(270, 21);
            this.cbb_FamilySymbol.TabIndex = 0;
            this.cbb_FamilySymbol.SelectedIndexChanged += new System.EventHandler(this.cbb_FamilySymbol_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Family Symbol:";
            // 
            // cbb_Parameter
            // 
            this.cbb_Parameter.FormattingEnabled = true;
            this.cbb_Parameter.Location = new System.Drawing.Point(97, 67);
            this.cbb_Parameter.Name = "cbb_Parameter";
            this.cbb_Parameter.Size = new System.Drawing.Size(270, 21);
            this.cbb_Parameter.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Parameter:";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(288, 101);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(75, 23);
            this.btn_Save.TabIndex = 4;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // FrmSettingManagerSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 133);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbb_Parameter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbb_FamilySymbol);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSettingManagerSection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Setting";
            this.Load += new System.EventHandler(this.FrmSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbb_FamilySymbol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbb_Parameter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Save;
    }
}