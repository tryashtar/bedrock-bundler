namespace ShulkerBundle;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        var minecraft = new Minecraft(@"D:\Minecraft\Bedrock Storage\Launcher\installations\Latest\dev\packageData");
        foreach (var world in minecraft.Worlds)
        {
            var view = new WorldView(world);
            WorldsPanel.Controls.Add(view);
            view.Click += View_Click;
        }
    }

    private void View_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(((WorldView)sender).World.WorldName);
    }
}
