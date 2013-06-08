namespace FCBot.UI
{
    partial class UcProgressBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pbValue = new System.Windows.Forms.ProgressBar();
            this.lblText = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnAdditionals = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbValue
            // 
            this.pbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbValue.Cursor = System.Windows.Forms.Cursors.PanNorth;
            this.pbValue.Location = new System.Drawing.Point(119, 2);
            this.pbValue.Name = "pbValue";
            this.pbValue.Size = new System.Drawing.Size(186, 21);
            this.pbValue.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbValue.TabIndex = 3;
            this.pbValue.Value = 50;
            this.pbValue.Click += new System.EventHandler(this.pbValue_Click);
            this.pbValue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CommonProgressBar_MouseAction);
            this.pbValue.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CommonProgressBar_MouseAction);
            // 
            // lblText
            // 
            this.lblText.Location = new System.Drawing.Point(3, 3);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(110, 19);
            this.lblText.TabIndex = 2;
            this.lblText.Text = "Label";
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // btnAdditionals
            // 
            this.btnAdditionals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdditionals.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdditionals.Location = new System.Drawing.Point(274, 2);
            this.btnAdditionals.Name = "btnAdditionals";
            this.btnAdditionals.Size = new System.Drawing.Size(31, 21);
            this.btnAdditionals.TabIndex = 9;
            this.btnAdditionals.Text = "...";
            this.toolTip1.SetToolTip(this.btnAdditionals, "Show additional options");
            this.btnAdditionals.UseVisualStyleBackColor = true;
            this.btnAdditionals.Visible = false;
            this.btnAdditionals.Click += new System.EventHandler(this.btnAdditionals_Click);
            // 
            // ucProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAdditionals);
            this.Controls.Add(this.pbValue);
            this.Controls.Add(this.lblText);
            this.Name = "UcProgressBar";
            this.Size = new System.Drawing.Size(308, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbValue;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnAdditionals;
    }
}
