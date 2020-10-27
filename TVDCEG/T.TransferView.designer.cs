namespace TVDCEG
{
    partial class TransferView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransferView));
            this.group_Options = new System.Windows.Forms.GroupBox();
            this.lb_viewtype = new System.Windows.Forms.ListBox();
            this.label_Search = new System.Windows.Forms.Label();
            this.textBox_search = new System.Windows.Forms.TextBox();
            this.Viewtype = new System.Windows.Forms.Label();
            this.Cb_ViewType = new System.Windows.Forms.ComboBox();
            this.label_From = new System.Windows.Forms.Label();
            this.comboBox_proj = new System.Windows.Forms.ComboBox();
            this.btn_Transfer = new System.Windows.Forms.Button();
            this.Document = new System.Windows.Forms.GroupBox();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.checkedListBox_prj = new System.Windows.Forms.CheckedListBox();
            this.label_To = new System.Windows.Forms.Label();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.group_Options.SuspendLayout();
            this.Document.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_Options
            // 
            this.group_Options.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.group_Options.Controls.Add(this.lb_viewtype);
            this.group_Options.Controls.Add(this.label_Search);
            this.group_Options.Controls.Add(this.textBox_search);
            this.group_Options.Controls.Add(this.Viewtype);
            this.group_Options.Controls.Add(this.Cb_ViewType);
            this.group_Options.Location = new System.Drawing.Point(12, 5);
            this.group_Options.Name = "group_Options";
            this.group_Options.Size = new System.Drawing.Size(398, 472);
            this.group_Options.TabIndex = 0;
            this.group_Options.TabStop = false;
            this.group_Options.Text = "Options";
            this.group_Options.Enter += new System.EventHandler(this.group_Options_Enter);
            // 
            // lb_viewtype
            // 
            this.lb_viewtype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lb_viewtype.FormattingEnabled = true;
            this.lb_viewtype.Location = new System.Drawing.Point(0, 143);
            this.lb_viewtype.Name = "lb_viewtype";
            this.lb_viewtype.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_viewtype.Size = new System.Drawing.Size(392, 316);
            this.lb_viewtype.TabIndex = 1;
            this.lb_viewtype.SelectedIndexChanged += new System.EventHandler(this.lb_view_SelectedIndexChanged);
            // 
            // label_Search
            // 
            this.label_Search.AutoSize = true;
            this.label_Search.Location = new System.Drawing.Point(6, 27);
            this.label_Search.Name = "label_Search";
            this.label_Search.Size = new System.Drawing.Size(41, 13);
            this.label_Search.TabIndex = 3;
            this.label_Search.Text = "Search";
            // 
            // textBox_search
            // 
            this.textBox_search.Location = new System.Drawing.Point(0, 48);
            this.textBox_search.Name = "textBox_search";
            this.textBox_search.Size = new System.Drawing.Size(392, 20);
            this.textBox_search.TabIndex = 2;
            this.textBox_search.TextChanged += new System.EventHandler(this.textBox_search_TextChanged);
            // 
            // Viewtype
            // 
            this.Viewtype.AutoSize = true;
            this.Viewtype.Location = new System.Drawing.Point(3, 79);
            this.Viewtype.Name = "Viewtype";
            this.Viewtype.Size = new System.Drawing.Size(57, 13);
            this.Viewtype.TabIndex = 1;
            this.Viewtype.Text = "View Type";
            // 
            // Cb_ViewType
            // 
            this.Cb_ViewType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Cb_ViewType.DropDownHeight = 110;
            this.Cb_ViewType.FormattingEnabled = true;
            this.Cb_ViewType.IntegralHeight = false;
            this.Cb_ViewType.Location = new System.Drawing.Point(0, 97);
            this.Cb_ViewType.Name = "Cb_ViewType";
            this.Cb_ViewType.Size = new System.Drawing.Size(392, 21);
            this.Cb_ViewType.TabIndex = 0;
            this.Cb_ViewType.SelectedIndexChanged += new System.EventHandler(this.Cb_ViewType_SelectedIndexChanged);
            // 
            // label_From
            // 
            this.label_From.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_From.AutoSize = true;
            this.label_From.Location = new System.Drawing.Point(6, 28);
            this.label_From.Name = "label_From";
            this.label_From.Size = new System.Drawing.Size(30, 13);
            this.label_From.TabIndex = 6;
            this.label_From.Text = "From";
            // 
            // comboBox_proj
            // 
            this.comboBox_proj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_proj.DropDownHeight = 120;
            this.comboBox_proj.FormattingEnabled = true;
            this.comboBox_proj.IntegralHeight = false;
            this.comboBox_proj.Location = new System.Drawing.Point(0, 48);
            this.comboBox_proj.Name = "comboBox_proj";
            this.comboBox_proj.Size = new System.Drawing.Size(424, 21);
            this.comboBox_proj.TabIndex = 5;
            this.comboBox_proj.SelectedIndexChanged += new System.EventHandler(this.comboBox_proj_SelectedIndexChanged);
            // 
            // btn_Transfer
            // 
            this.btn_Transfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Transfer.Location = new System.Drawing.Point(656, 496);
            this.btn_Transfer.Name = "btn_Transfer";
            this.btn_Transfer.Size = new System.Drawing.Size(79, 27);
            this.btn_Transfer.TabIndex = 2;
            this.btn_Transfer.Text = "Transfer";
            this.btn_Transfer.UseVisualStyleBackColor = true;
            this.btn_Transfer.Click += new System.EventHandler(this.btn_Transfer_Click);
            // 
            // Document
            // 
            this.Document.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Document.Controls.Add(this.elementHost1);
            this.Document.Controls.Add(this.checkedListBox_prj);
            this.Document.Controls.Add(this.label_To);
            this.Document.Controls.Add(this.label_From);
            this.Document.Controls.Add(this.comboBox_proj);
            this.Document.Location = new System.Drawing.Point(416, 5);
            this.Document.Name = "Document";
            this.Document.Size = new System.Drawing.Size(424, 472);
            this.Document.TabIndex = 3;
            this.Document.TabStop = false;
            this.Document.Text = "Document";
            this.Document.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 208);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(424, 258);
            this.elementHost1.TabIndex = 10;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // checkedListBox_prj
            // 
            this.checkedListBox_prj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox_prj.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBox_prj.FormattingEnabled = true;
            this.checkedListBox_prj.Location = new System.Drawing.Point(0, 95);
            this.checkedListBox_prj.Name = "checkedListBox_prj";
            this.checkedListBox_prj.Size = new System.Drawing.Size(424, 107);
            this.checkedListBox_prj.TabIndex = 8;
            this.checkedListBox_prj.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // label_To
            // 
            this.label_To.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_To.AutoSize = true;
            this.label_To.Location = new System.Drawing.Point(6, 76);
            this.label_To.Name = "label_To";
            this.label_To.Size = new System.Drawing.Size(20, 13);
            this.label_To.TabIndex = 7;
            this.label_To.Text = "To";
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(750, 496);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(79, 27);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // TransferView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 536);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.Document);
            this.Controls.Add(this.btn_Transfer);
            this.Controls.Add(this.group_Options);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransferView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: TransferView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransferView_FormClosing);
            this.Load += new System.EventHandler(this.TransferView_Load);
            this.group_Options.ResumeLayout(false);
            this.group_Options.PerformLayout();
            this.Document.ResumeLayout(false);
            this.Document.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox group_Options;
        private System.Windows.Forms.Label Viewtype;
        private System.Windows.Forms.ComboBox Cb_ViewType;
        private System.Windows.Forms.ListBox lb_viewtype;
        private System.Windows.Forms.TextBox textBox_search;
        private System.Windows.Forms.Label label_Search;
        private System.Windows.Forms.Button btn_Transfer;
        private System.Windows.Forms.Label label_From;
        private System.Windows.Forms.ComboBox comboBox_proj;
        private System.Windows.Forms.GroupBox Document;
        private System.Windows.Forms.CheckedListBox checkedListBox_prj;
        private System.Windows.Forms.Label label_To;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
    }
}