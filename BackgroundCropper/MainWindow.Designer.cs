namespace BackgroundCropper
{
    partial class MainWindow
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_SourceFileImageBox = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.m_OutputFolderTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_SelectedAspectRatio = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.m_SourceImageFileTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.m_OpenSourceImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_SourceFileImageBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.m_SourceFileImageBox);
            this.groupBox1.Location = new System.Drawing.Point(5, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(834, 442);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Result Selection";
            // 
            // m_SourceFileImageBox
            // 
            this.m_SourceFileImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_SourceFileImageBox.Location = new System.Drawing.Point(3, 16);
            this.m_SourceFileImageBox.Name = "m_SourceFileImageBox";
            this.m_SourceFileImageBox.Size = new System.Drawing.Size(828, 423);
            this.m_SourceFileImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.m_SourceFileImageBox.TabIndex = 0;
            this.m_SourceFileImageBox.TabStop = false;
            this.m_SourceFileImageBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageToCrop_Paint);
            this.m_SourceFileImageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SourceFileImageBox_MouseDown);
            this.m_SourceFileImageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SourceFileImageBox_MouseMove);
            this.m_SourceFileImageBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SourceFileImageBox_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.m_OutputFolderTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.m_SelectedAspectRatio);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.m_SourceImageFileTextBox);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(5, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(834, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crop Settings";
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button4.Location = new System.Drawing.Point(660, 95);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(135, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "Set as background";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.SetCropAsBackground_Click);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button3.Location = new System.Drawing.Point(802, 67);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(26, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.SelectOutputFolderButton_Click);
            // 
            // m_OutputFolderTextBox
            // 
            this.m_OutputFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_OutputFolderTextBox.Enabled = false;
            this.m_OutputFolderTextBox.Location = new System.Drawing.Point(116, 69);
            this.m_OutputFolderTextBox.Name = "m_OutputFolderTextBox";
            this.m_OutputFolderTextBox.Size = new System.Drawing.Size(679, 20);
            this.m_OutputFolderTextBox.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Output Folder:";
            // 
            // m_SelectedAspectRatio
            // 
            this.m_SelectedAspectRatio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_SelectedAspectRatio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_SelectedAspectRatio.FormattingEnabled = true;
            this.m_SelectedAspectRatio.Items.AddRange(new object[] {
            "4:3",
            "16:9",
            "16:10"});
            this.m_SelectedAspectRatio.Location = new System.Drawing.Point(116, 43);
            this.m_SelectedAspectRatio.Name = "m_SelectedAspectRatio";
            this.m_SelectedAspectRatio.Size = new System.Drawing.Size(679, 21);
            this.m_SelectedAspectRatio.TabIndex = 5;
            this.m_SelectedAspectRatio.SelectedIndexChanged += new System.EventHandler(this.SelectedAspectRatio_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Crop Aspect Ratio:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source Image:";
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button2.Location = new System.Drawing.Point(802, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SelectSourceImageButton_Click);
            // 
            // m_SourceImageFileTextBox
            // 
            this.m_SourceImageFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_SourceImageFileTextBox.Enabled = false;
            this.m_SourceImageFileTextBox.Location = new System.Drawing.Point(116, 17);
            this.m_SourceImageFileTextBox.Name = "m_SourceImageFileTextBox";
            this.m_SourceImageFileTextBox.Size = new System.Drawing.Size(679, 20);
            this.m_SourceImageFileTextBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(519, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Crop to Selection";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CropToSelectionButton_Click);
            // 
            // m_OpenSourceImageDialog
            // 
            this.m_OpenSourceImageDialog.Filter = "Image Files|*.jpeg;*.jpg;*.png;*.bmp";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 575);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainWindow";
            this.Text = "Background Cropper";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResizeBegin += new System.EventHandler(this.MainWindow_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.MainWindow_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_SourceFileImageBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox m_SourceFileImageBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox m_SelectedAspectRatio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox m_SourceImageFileTextBox;
        private System.Windows.Forms.OpenFileDialog m_OpenSourceImageDialog;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox m_OutputFolderTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button4;
    }
}

