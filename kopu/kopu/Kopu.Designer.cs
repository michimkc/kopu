namespace kopu
{
    partial class Kopu
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
            this.txtBox = new System.Windows.Forms.TextBox();
            this.historyText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtBox
            // 
            this.txtBox.BackColor = System.Drawing.Color.Black;
            this.txtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBox.ForeColor = System.Drawing.Color.White;
            this.txtBox.Location = new System.Drawing.Point(20, 99);
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(583, 26);
            this.txtBox.TabIndex = 0;
            this.txtBox.TextChanged += new System.EventHandler(this.txtBox_TextChanged);
            this.txtBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBox_KeyDown);
            // 
            // historyText
            // 
            this.historyText.BackColor = System.Drawing.Color.Black;
            this.historyText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.historyText.ForeColor = System.Drawing.Color.White;
            this.historyText.Location = new System.Drawing.Point(20, 7);
            this.historyText.Multiline = true;
            this.historyText.Name = "historyText";
            this.historyText.ReadOnly = true;
            this.historyText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.historyText.Size = new System.Drawing.Size(583, 86);
            this.historyText.TabIndex = 1;
            this.historyText.TextChanged += new System.EventHandler(this.historyText_TextChanged);
            this.historyText.DragDrop += new System.Windows.Forms.DragEventHandler(this.HistoryText_DragDrop);
            this.historyText.DragEnter += new System.Windows.Forms.DragEventHandler(this.HistoryText_DragEnter);
            // 
            // Kopu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(624, 133);
            this.Controls.Add(this.historyText);
            this.Controls.Add(this.txtBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Kopu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kopu";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.TextBox historyText;
    }
}

