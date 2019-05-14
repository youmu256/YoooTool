namespace YoooTool.Code
{
    partial class LevelEdit
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
            this.RoomsContent = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // RoomsContent
            // 
            this.RoomsContent.AutoScroll = true;
            this.RoomsContent.AutoSize = true;
            this.RoomsContent.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.RoomsContent.Location = new System.Drawing.Point(12, 212);
            this.RoomsContent.Name = "RoomsContent";
            this.RoomsContent.Size = new System.Drawing.Size(629, 264);
            this.RoomsContent.TabIndex = 0;
            // 
            // LevelEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 488);
            this.Controls.Add(this.RoomsContent);
            this.Name = "LevelEdit";
            this.Text = "关卡配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel RoomsContent;
    }
}