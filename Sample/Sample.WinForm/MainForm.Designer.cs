using System.Windows.Forms;

namespace Sample.WinForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            blazorWebView = new BlazorWebView();
            SuspendLayout();
            // 
            // blazorWebView
            // 
            blazorWebView.BackColor = Color.White;
            blazorWebView.Dock = DockStyle.Fill;
            blazorWebView.Location = new Point(0, 0);
            blazorWebView.Name = "blazorWebView";
            blazorWebView.Size = new Size(1350, 729);
            blazorWebView.TabIndex = 0;
            blazorWebView.Visible = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1350, 729);
            Controls.Add(blazorWebView);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView;
    }
}