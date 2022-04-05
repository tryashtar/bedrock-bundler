namespace ShulkerBundle;

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
            this.WorldsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SelectedPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // WorldsPanel
            // 
            this.WorldsPanel.AutoScroll = true;
            this.WorldsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.WorldsPanel.Location = new System.Drawing.Point(0, 0);
            this.WorldsPanel.Name = "WorldsPanel";
            this.WorldsPanel.Size = new System.Drawing.Size(439, 450);
            this.WorldsPanel.TabIndex = 0;
            // 
            // SelectedPanel
            // 
            this.SelectedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectedPanel.Location = new System.Drawing.Point(439, 0);
            this.SelectedPanel.Name = "SelectedPanel";
            this.SelectedPanel.Size = new System.Drawing.Size(391, 450);
            this.SelectedPanel.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 450);
            this.Controls.Add(this.SelectedPanel);
            this.Controls.Add(this.WorldsPanel);
            this.Name = "MainForm";
            this.Text = "Shulker Bundle";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

    }

    #endregion

    private FlowLayoutPanel WorldsPanel;
    private FlowLayoutPanel SelectedPanel;
}
