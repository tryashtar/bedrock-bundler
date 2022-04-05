using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundle;
public class World
{
    public readonly string WorldName;
    public readonly string Folder;
    public readonly List<BehaviorPack> LocalBehaviorPacks;
    public readonly List<ResourcePack> LocalResourcePacks;
    public readonly List<PackReference> ReferencedBehaviorPacks;
    public readonly List<PackReference> ReferencedResourcePacks;
    public World(string folder)
    {
        Folder = folder;
        WorldName = File.ReadAllText(Path.Combine(folder, "levelname.txt"));
        LocalBehaviorPacks = Pack.Load(Path.Combine(folder, "behavior_packs"), x => new BehaviorPack(x));
        LocalResourcePacks = Pack.Load(Path.Combine(folder, "resource_packs"), x => new ResourcePack(x));
    }
}
