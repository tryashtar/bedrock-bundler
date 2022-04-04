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
            WorldPanel.Controls.Add(new WorldView(world));
        }
    }
}
