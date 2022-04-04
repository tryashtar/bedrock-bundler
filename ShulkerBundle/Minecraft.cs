using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundle;
public class Minecraft
{
    public readonly string Folder;
    public readonly List<World> Worlds;
    public Minecraft(string folder)
    {
        Folder = folder;
        Worlds = new();
        foreach (var world in Directory.GetDirectories(Path.Combine(folder, "minecraftWorlds")))
        {
            if (File.Exists(Path.Combine(world, "level.dat")))
                Worlds.Add(new World(world));
        }
    }
}
