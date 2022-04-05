using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundleWPF;
public class Minecraft : IPackSource
{
    public readonly string Folder;
    public ObservableCollection<World> Worlds { get; private set; }
    public ObservableCollection<Pack> DevBehaviorPacks { get; private set; }
    public ObservableCollection<Pack> DevResourcePacks { get; private set; }
    public Minecraft(string folder)
    {
        Folder = folder;
        Worlds = new();
        foreach (var world in Directory.GetDirectories(Path.Combine(folder, "minecraftWorlds")))
        {
            if (File.Exists(Path.Combine(world, "level.dat")))
                Worlds.Add(new World(this, world));
        }
        DevBehaviorPacks = new(Pack.Load(Path.Combine(folder, "development_behavior_packs")));
        DevResourcePacks = new(Pack.Load(Path.Combine(folder, "development_resource_packs")));
    }

    public Pack? GetBehaviorPack(PackReference reference) => IPackSource.Find(DevBehaviorPacks, reference);
    public Pack? GetResourcePack(PackReference reference) => IPackSource.Find(DevResourcePacks, reference);
}

public interface IPackSource
{
    Pack? GetBehaviorPack(PackReference reference);
    Pack? GetResourcePack(PackReference reference);
    public static Pack? Find(IEnumerable<Pack> source, PackReference reference)
    {
        foreach (var item in source)
        {
            if (item.UUID == reference.UUID && item.Version == reference.Version)
                return item;
        }
        return null;
    }
}
