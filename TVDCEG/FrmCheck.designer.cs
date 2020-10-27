namespace TVDCEG
{
    partial class FrmCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCheck));
            this.listBoxAssembly = new System.Windows.Forms.ListBox();
            this.bnt_ok = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Searchtxt = new System.Windows.Forms.Label();
            this.SearchtextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_Lifting = new System.Windows.Forms.CheckBox();
            this.checkBox_STRAND = new System.Windows.Forms.CheckBox();
            this.checkBox_EMBED = new System.Windows.Forms.CheckBox();
            this.checkBox_REBAR = new System.Windows.Forms.CheckBox();
            this.btn_Invert = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxAssembly
            // 
            this.listBoxAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAssembly.FormattingEnabled = true;
            this.listBoxAssembly.Location = new System.Drawing.Point(12, 184);
            this.listBoxAssembly.Name = "listBoxAssembly";
            this.listBoxAssembly.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAssembly.Size = new System.Drawing.Size(326, 290);
            this.listBoxAssembly.Sorted = true;
            this.listBoxAssembly.TabIndex = 0;
            this.listBoxAssembly.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // bnt_ok
            // 
            this.bnt_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnt_ok.Location = new System.Drawing.Point(131, 503);
            this.bnt_ok.Name = "bnt_ok";
            this.bnt_ok.Size = new System.Drawing.Size(114, 25);
            this.bnt_ok.TabIndex = 1;
            this.bnt_ok.Text = "Check Interference";
            this.bnt_ok.UseVisualStyleBackColor = true;
            this.bnt_ok.Click += new System.EventHandler(this.bnt_ok_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(252, 503);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Searchtxt
            // 
            this.Searchtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Searchtxt.AutoSize = true;
            this.Searchtxt.Location = new System.Drawing.Point(12, 124);
            this.Searchtxt.Name = "Searchtxt";
            this.Searchtxt.Size = new System.Drawing.Size(44, 13);
            this.Searchtxt.TabIndex = 3;
            this.Searchtxt.Text = "Search:";
            this.Searchtxt.Click += new System.EventHandler(this.Searchtxt_Click);
            // 
            // SearchtextBox
            // 
            this.SearchtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchtextBox.Location = new System.Drawing.Point(12, 146);
            this.SearchtextBox.Name = "SearchtextBox";
            this.SearchtextBox.Size = new System.Drawing.Size(326, 20);
            this.SearchtextBox.TabIndex = 4;
            this.SearchtextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkBox_Lifting);
            this.groupBox1.Controls.Add(this.checkBox_STRAND);
            this.groupBox1.Controls.Add(this.checkBox_EMBED);
            this.groupBox1.Controls.Add(this.checkBox_REBAR);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 104);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Clash Detection Options";
            // 
            // checkBox_Lifting
            // 
            this.checkBox_Lifting.AutoSize = true;
            this.checkBox_Lifting.Location = new System.Drawing.Point(10, 67);
            this.checkBox_Lifting.Name = "checkBox_Lifting";
            this.checkBox_Lifting.Size = new System.Drawing.Size(67, 17);
            this.checkBox_Lifting.TabIndex = 3;
            this.checkBox_Lifting.Text = "LIFTING";
            this.checkBox_Lifting.UseVisualStyleBackColor = true;
            this.checkBox_Lifting.CheckedChanged += new System.EventHandler(this.checkBox_Lifting_CheckedChanged);
            // 
            // checkBox_STRAND
            // 
            this.checkBox_STRAND.AutoSize = true;
            this.checkBox_STRAND.Location = new System.Drawing.Point(189, 30);
            this.checkBox_STRAND.Name = "checkBox_STRAND";
            this.checkBox_STRAND.Size = new System.Drawing.Size(71, 17);
            this.checkBox_STRAND.TabIndex = 2;
            this.checkBox_STRAND.Text = "STRAND";
            this.checkBox_STRAND.UseVisualStyleBackColor = true;
            this.checkBox_STRAND.CheckedChanged += new System.EventHandler(this.checkBox_STRAND_CheckedChanged);
            // 
            // checkBox_EMBED
            // 
            this.checkBox_EMBED.AutoSize = true;
            this.checkBox_EMBED.Location = new System.Drawing.Point(92, 30);
            this.checkBox_EMBED.Name = "checkBox_EMBED";
            this.checkBox_EMBED.Size = new System.Drawing.Size(64, 17);
            this.checkBox_EMBED.TabIndex = 1;
            this.checkBox_EMBED.Text = "EMBED";
            this.checkBox_EMBED.UseVisualStyleBackColor = true;
            this.checkBox_EMBED.CheckedChanged += new System.EventHandler(this.checkBox_EMBED_CheckedChanged);
            // 
            // checkBox_REBAR
            // 
            this.checkBox_REBAR.AutoSize = true;
            this.checkBox_REBAR.Location = new System.Drawing.Point(10, 30);
            this.checkBox_REBAR.Name = "checkBox_REBAR";
            this.checkBox_REBAR.Size = new System.Drawing.Size(63, 17);
            this.checkBox_REBAR.TabIndex = 0;
            this.checkBox_REBAR.Text = "REBAR";
            this.checkBox_REBAR.UseVisualStyleBackColor = true;
            this.checkBox_REBAR.CheckedChanged += new System.EventHandler(this.checkBox_REBAR_CheckedChanged);
            // 
            // btn_Invert
            // 
            this.btn_Invert.Location = new System.Drawing.Point(12, 504);
            this.btn_Invert.Name = "btn_Invert";
            this.btn_Invert.Size = new System.Drawing.Size(113, 25);
            this.btn_Invert.TabIndex = 6;
            this.btn_Invert.Text = "Invert Selection";
            this.btn_Invert.UseVisualStyleBackColor = true;
            this.btn_Invert.Click += new System.EventHandler(this.btn_Invert_Click);
            // 
            // FrmCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 538);
            this.Controls.Add(this.btn_Invert);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SearchtextBox);
            this.Controls.Add(this.Searchtxt);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bnt_ok);
            this.Controls.Add(this.listBoxAssembly);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCheck";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Iven EXT: Assembly Clash Detection";
            this.Load += new System.EventHandler(this.FrmCheck_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAssembly;
        private System.Windows.Forms.Button bnt_ok;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label Searchtxt;
        private System.Windows.Forms.TextBox SearchtextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_STRAND;
        private System.Windows.Forms.CheckBox checkBox_EMBED;
        private System.Windows.Forms.CheckBox checkBox_REBAR;
        private System.Windows.Forms.CheckBox checkBox_Lifting;
        private System.Windows.Forms.Button btn_Invert;
    }
}