namespace TVDCEG
{
    partial class FrmAddsurfixsheet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAddsurfixsheet));
            this.listBox_sheet = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.TXT_SEARCH = new System.Windows.Forms.Label();
            this.label_Prefix = new System.Windows.Forms.Label();
            this.textBox_prefix = new System.Windows.Forms.TextBox();
            this.label_Suffix = new System.Windows.Forms.Label();
            this.textBox_suffix = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label_SheetNumber = new System.Windows.Forms.Label();
            this.textBox_SheetNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox_sheet
            // 
            this.listBox_sheet.FormattingEnabled = true;
            this.listBox_sheet.Location = new System.Drawing.Point(6, 202);
            this.listBox_sheet.Name = "listBox_sheet";
            this.listBox_sheet.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox_sheet.Size = new System.Drawing.Size(367, 264);
            this.listBox_sheet.Sorted = true;
            this.listBox_sheet.TabIndex = 0;
            this.listBox_sheet.SelectedIndexChanged += new System.EventHandler(this.listBox_sheet_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(217, 482);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(367, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // TXT_SEARCH
            // 
            this.TXT_SEARCH.AutoSize = true;
            this.TXT_SEARCH.Location = new System.Drawing.Point(3, 15);
            this.TXT_SEARCH.Name = "TXT_SEARCH";
            this.TXT_SEARCH.Size = new System.Drawing.Size(41, 13);
            this.TXT_SEARCH.TabIndex = 3;
            this.TXT_SEARCH.Text = "Search";
            // 
            // label_Prefix
            // 
            this.label_Prefix.AutoSize = true;
            this.label_Prefix.Location = new System.Drawing.Point(9, 62);
            this.label_Prefix.Name = "label_Prefix";
            this.label_Prefix.Size = new System.Drawing.Size(33, 13);
            this.label_Prefix.TabIndex = 4;
            this.label_Prefix.Text = "Prefix";
            // 
            // textBox_prefix
            // 
            this.textBox_prefix.Location = new System.Drawing.Point(6, 79);
            this.textBox_prefix.Name = "textBox_prefix";
            this.textBox_prefix.Size = new System.Drawing.Size(367, 20);
            this.textBox_prefix.TabIndex = 5;
            // 
            // label_Suffix
            // 
            this.label_Suffix.AutoSize = true;
            this.label_Suffix.Location = new System.Drawing.Point(9, 105);
            this.label_Suffix.Name = "label_Suffix";
            this.label_Suffix.Size = new System.Drawing.Size(33, 13);
            this.label_Suffix.TabIndex = 6;
            this.label_Suffix.Text = "Suffix";
            // 
            // textBox_suffix
            // 
            this.textBox_suffix.Location = new System.Drawing.Point(6, 123);
            this.textBox_suffix.Name = "textBox_suffix";
            this.textBox_suffix.Size = new System.Drawing.Size(367, 20);
            this.textBox_suffix.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(298, 482);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label_SheetNumber
            // 
            this.label_SheetNumber.AutoSize = true;
            this.label_SheetNumber.Location = new System.Drawing.Point(9, 149);
            this.label_SheetNumber.Name = "label_SheetNumber";
            this.label_SheetNumber.Size = new System.Drawing.Size(72, 13);
            this.label_SheetNumber.TabIndex = 9;
            this.label_SheetNumber.Text = "SheetNumber";
            // 
            // textBox_SheetNumber
            // 
            this.textBox_SheetNumber.Location = new System.Drawing.Point(6, 166);
            this.textBox_SheetNumber.Name = "textBox_SheetNumber";
            this.textBox_SheetNumber.Size = new System.Drawing.Size(367, 20);
            this.textBox_SheetNumber.TabIndex = 10;
            // 
            // FrmAddsurfixsheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 517);
            this.Controls.Add(this.textBox_SheetNumber);
            this.Controls.Add(this.label_SheetNumber);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox_suffix);
            this.Controls.Add(this.label_Suffix);
            this.Controls.Add(this.textBox_prefix);
            this.Controls.Add(this.label_Prefix);
            this.Controls.Add(this.TXT_SEARCH);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox_sheet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmAddsurfixsheet";
            this.Text = "Iven EXT: Add Text Fields";
            this.Load += new System.EventHandler(this.FrmAddsurfixsheet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_sheet;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label TXT_SEARCH;
        private System.Windows.Forms.Label label_Prefix;
        private System.Windows.Forms.TextBox textBox_prefix;
        private System.Windows.Forms.Label label_Suffix;
        private System.Windows.Forms.TextBox textBox_suffix;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label_SheetNumber;
        private System.Windows.Forms.TextBox textBox_SheetNumber;
    }
}