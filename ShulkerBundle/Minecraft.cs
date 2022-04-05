using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundle;
public class Minecraft
{
    public readonly string Folder;
    public List<World> Worlds { get; private set; }
    public List<BehaviorPack> DevBehaviorPacks { get; private set; }
    public List<ResourcePack> DevResourcePacks { get; private set; }
    public Minecraft(string folder)
    {
        Folder = folder;
        Worlds = new();
        foreach (var world in Directory.GetDirectories(Path.Combine(folder, "minecraftWorlds")))
        {
            if (File.Exists(Path.Combine(world, "level.dat")))
                Worlds.Add(new World(world));
        }
        DevBehaviorPacks = Pack.Load(Path.Combine(folder, "development_behavior_packs"), x => new BehaviorPack(x));
        DevResourcePacks = Pack.Load(Path.Combine(folder, "development_resource_packs"), x => new ResourcePack(x));
    }
}
