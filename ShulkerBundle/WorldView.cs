using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShulkerBundle;
public partial class WorldView : UserControl
{
    public readonly World World;
    public WorldView(World world)
    {
        InitializeComponent();
        World = world;
        WorldIcon.ImageLocation = Path.Combine(world.Folder, "world_icon.jpeg");
        WorldName.Text = File.ReadAllText(Path.Combine(world.Folder, "levelname.txt"));
        FolderName.Text = Path.GetFileName(world.Folder);
    }

    private void WorldView_Load(object sender, EventArgs e)
    {

    }

    private void WorldIcon_DoubleClick(object sender, EventArgs e)
    {
        Process.Start("explorer.exe", World.Folder);
    }
}
