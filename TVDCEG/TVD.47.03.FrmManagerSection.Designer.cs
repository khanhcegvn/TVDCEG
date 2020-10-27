namespace TVDCEG
{
    partial class FrmManagerSection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmManagerSection));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Sheets = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SheetId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lb_view = new System.Windows.Forms.ListBox();
            this.txt_Search = new System.Windows.Forms.TextBox();
            this.previewHost = new System.Windows.Forms.Integration.ElementHost();
            this.btn_Setting = new System.Windows.Forms.Button();
            this.btn_Run = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Sheets,
            this.SheetId});
            this.dataGridView1.Location = new System.Drawing.Point(291, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(296, 445);
            this.dataGridView1.TabIndex = 0;
            // 
            // Sheets
            // 
            this.Sheets.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Sheets.HeaderText = "Sheets";
            this.Sheets.Name = "Sheets";
            // 
            // SheetId
            // 
            this.SheetId.HeaderText = "SheetId";
            this.SheetId.Name = "SheetId";
            this.SheetId.Visible = false;
            // 
            // lb_view
            // 
            this.lb_view.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lb_view.FormattingEnabled = true;
            this.lb_view.Location = new System.Drawing.Point(12, 51);
            this.lb_view.Name = "lb_view";
            this.lb_view.Size = new System.Drawing.Size(259, 407);
            this.lb_view.TabIndex = 1;
            this.lb_view.SelectedIndexChanged += new System.EventHandler(this.lb_view_SelectedIndexChanged);
            // 
            // txt_Search
            // 
            this.txt_Search.Location = new System.Drawing.Point(12, 13);
            this.txt_Search.Name = "txt_Search";
            this.txt_Search.Size = new System.Drawing.Size(259, 20);
            this.txt_Search.TabIndex = 2;
            this.txt_Search.TextChanged += new System.EventHandler(this.txt_Search_TextChanged);
            // 
            // previewHost
            // 
            this.previewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewHost.Location = new System.Drawing.Point(607, 12);
            this.previewHost.Name = "previewHost";
            this.previewHost.Size = new System.Drawing.Size(648, 445);
            this.previewHost.TabIndex = 3;
            this.previewHost.Text = "previewHost";
            this.previewHost.Child = null;
            // 
            // btn_Setting
            // 
            this.btn_Setting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Setting.Location = new System.Drawing.Point(1142, 464);
            this.btn_Setting.Name = "btn_Setting";
            this.btn_Setting.Size = new System.Drawing.Size(96, 30);
            this.btn_Setting.TabIndex = 4;
            this.btn_Setting.Text = "Setting";
            this.btn_Setting.UseVisualStyleBackColor = true;
            this.btn_Setting.Click += new System.EventHandler(this.btn_Setting_Click);
            // 
            // btn_Run
            // 
            this.btn_Run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Run.Location = new System.Drawing.Point(1040, 464);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(96, 30);
            this.btn_Run.TabIndex = 5;
            this.btn_Run.Text = "Run";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // FrmManagerSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1267, 506);
            this.Controls.Add(this.btn_Run);
            this.Controls.Add(this.btn_Setting);
            this.Controls.Add(this.previewHost);
            this.Controls.Add(this.txt_Search);
            this.Controls.Add(this.lb_view);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmManagerSection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Manager Section";
            this.Load += new System.EventHandler(this.FrmManagerSection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ListBox lb_view;
        private System.Windows.Forms.TextBox txt_Search;
        private System.Windows.Forms.Integration.ElementHost previewHost;
        private System.Windows.Forms.Button btn_Setting;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sheets;
        private System.Windows.Forms.DataGridViewTextBoxColumn SheetId;
    }
}