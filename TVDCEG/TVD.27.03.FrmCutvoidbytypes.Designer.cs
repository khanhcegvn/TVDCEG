namespace TVDCEG
{
    partial class FrmCutvoidbytypes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCutvoidbytypes));
            this.btn_cut = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_uncut = new System.Windows.Forms.Button();
            this.Treeviewtypecut = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // btn_cut
            // 
            this.btn_cut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_cut.Location = new System.Drawing.Point(12, 411);
            this.btn_cut.Name = "btn_cut";
            this.btn_cut.Size = new System.Drawing.Size(95, 27);
            this.btn_cut.TabIndex = 1;
            this.btn_cut.Text = "Cut";
            this.btn_cut.UseVisualStyleBackColor = true;
            this.btn_cut.Click += new System.EventHandler(this.btn_cut_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Cancel.Location = new System.Drawing.Point(214, 411);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(95, 27);
            this.btn_Cancel.TabIndex = 1;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_uncut
            // 
            this.btn_uncut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_uncut.Location = new System.Drawing.Point(113, 411);
            this.btn_uncut.Name = "btn_uncut";
            this.btn_uncut.Size = new System.Drawing.Size(95, 27);
            this.btn_uncut.TabIndex = 1;
            this.btn_uncut.Text = "UnCut";
            this.btn_uncut.UseVisualStyleBackColor = true;
            this.btn_uncut.Click += new System.EventHandler(this.btn_uncut_Click);
            // 
            // Treeviewtypecut
            // 
            this.Treeviewtypecut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Treeviewtypecut.Location = new System.Drawing.Point(12, 12);
            this.Treeviewtypecut.Name = "Treeviewtypecut";
            this.Treeviewtypecut.Size = new System.Drawing.Size(297, 390);
            this.Treeviewtypecut.TabIndex = 2;
            this.Treeviewtypecut.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.Treeviewtypecut_AfterCheck);
            // 
            // FrmCutvoidbytypes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 450);
            this.Controls.Add(this.Treeviewtypecut);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_uncut);
            this.Controls.Add(this.btn_cut);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCutvoidbytypes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Cut void by types";
            this.Load += new System.EventHandler(this.FrmCutvoidbytypes_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_cut;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_uncut;
        private System.Windows.Forms.TreeView Treeviewtypecut;
    }
}