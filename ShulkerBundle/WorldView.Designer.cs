namespace ShulkerBundle;

partial class WorldView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldView));
            this.WorldIcon = new System.Windows.Forms.PictureBox();
            this.WorldName = new System.Windows.Forms.Label();
            this.FolderName = new System.Windows.Forms.Label();
            this.BundleButton = new ShulkerBundle.InterpPictureBox();
            this.UnbundleButton = new ShulkerBundle.InterpPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.WorldIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BundleButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnbundleButton)).BeginInit();
            this.SuspendLayout();
            // 
            // WorldIcon
            // 
            this.WorldIcon.Location = new System.Drawing.Point(3, 3);
            this.WorldIcon.Name = "WorldIcon";
            this.WorldIcon.Size = new System.Drawing.Size(100, 56);
            this.WorldIcon.TabIndex = 0;
            this.WorldIcon.TabStop = false;
            this.WorldIcon.DoubleClick += new System.EventHandler(this.WorldIcon_DoubleClick);
            // 
            // WorldName
            // 
            this.WorldName.AutoSize = true;
            this.WorldName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.WorldName.Location = new System.Drawing.Point(109, 3);
            this.WorldName.Name = "WorldName";
            this.WorldName.Size = new System.Drawing.Size(98, 21);
            this.WorldName.TabIndex = 1;
            this.WorldName.Text = "World Name";
            // 
            // FolderName
            // 
            this.FolderName.AutoSize = true;
            this.FolderName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FolderName.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FolderName.Location = new System.Drawing.Point(109, 24);
            this.FolderName.Name = "FolderName";
            this.FolderName.Size = new System.Drawing.Size(100, 21);
            this.FolderName.TabIndex = 2;
            this.FolderName.Text = "Folder Name";
            // 
            // BundleButton
            // 
            this.BundleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BundleButton.Image = global::ShulkerBundle.Properties.Resources.bundle_filled;
            this.BundleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.BundleButton.Location = new System.Drawing.Point(351, 3);
            this.BundleButton.Name = "BundleButton";
            this.BundleButton.Size = new System.Drawing.Size(32, 32);
            this.BundleButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BundleButton.TabIndex = 3;
            this.BundleButton.TabStop = false;
            // 
            // UnbundleButton
            // 
            this.UnbundleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UnbundleButton.Image = ((System.Drawing.Image)(resources.GetObject("UnbundleButton.Image")));
            this.UnbundleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.UnbundleButton.Location = new System.Drawing.Point(313, 3);
            this.UnbundleButton.Name = "UnbundleButton";
            this.UnbundleButton.Size = new System.Drawing.Size(32, 32);
            this.UnbundleButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.UnbundleButton.TabIndex = 4;
            this.UnbundleButton.TabStop = false;
            // 
            // WorldView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UnbundleButton);
            this.Controls.Add(this.BundleButton);
            this.Controls.Add(this.FolderName);
            this.Controls.Add(this.WorldName);
            this.Controls.Add(this.WorldIcon);
            this.Name = "WorldView";
            this.Size = new System.Drawing.Size(386, 62);
            this.Load += new System.EventHandler(this.WorldView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WorldIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BundleButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UnbundleButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private PictureBox WorldIcon;
    private Label WorldName;
    private Label FolderName;
    private InterpPictureBox BundleButton;
    private InterpPictureBox UnbundleButton;
}
