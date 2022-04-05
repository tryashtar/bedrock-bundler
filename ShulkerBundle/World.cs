using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShulkerBundle;
public class World
{
    public string WorldName { get; private set; }
    public string WorldIcon => Path.Combine(Folder, "world_icon.jpeg");
    public string FolderName => Path.GetFileName(Folder);
    public readonly string Folder;
    public List<BehaviorPack> LocalBehaviorPacks { get; private set; }
    public List<ResourcePack> LocalResourcePacks { get; private set; }
    public List<PackReference> ReferencedBehaviorPacks { get; private set; }
    public List<PackReference> ReferencedResourcePacks { get; private set; }
    public World(string folder)
    {
        Folder = folder;
        WorldName = File.ReadAllText(Path.Combine(folder, "levelname.txt"));
        LocalBehaviorPacks = Pack.Load(Path.Combine(folder, "behavior_packs"), x => new BehaviorPack(x));
        LocalResourcePacks = Pack.Load(Path.Combine(folder, "resource_packs"), x => new ResourcePack(x));
        using var bpf = File.OpenRead(Path.Combine(folder, "world_behavior_packs.json"));
        using var bpm = JsonDocument.Parse(bpf);
        ReferencedBehaviorPacks = bpm.RootElement.EnumerateArray().Select(PackReference.ParseReference).ToList();
        using var rpf = File.OpenRead(Path.Combine(folder, "world_resource_packs.json"));
        using var rpm = JsonDocument.Parse(rpf);
        ReferencedResourcePacks = rpm.RootElement.EnumerateArray().Select(PackReference.ParseReference).ToList();
    }
}
