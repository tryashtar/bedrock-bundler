using ShulkerBundleWPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShulkerBundleWPF;
public class World : ObservableObject, IPackSource
{
    public readonly Minecraft Minecraft;
    public string WorldName { get; private set; }
    public string WorldIcon => Path.Combine(Folder, "world_icon.jpeg");
    public string FolderName => Path.GetFileName(Folder);
    public readonly string Folder;
    public ObservableCollection<Pack> LocalBehaviorPacks { get; private set; }
    public ObservableCollection<Pack> LocalResourcePacks { get; private set; }
    public ObservableCollection<PackReference> ReferencedBehaviorPacks { get; private set; }
    public ObservableCollection<PackReference> ReferencedResourcePacks { get; private set; }
    public IEnumerable<ReferencedPack> BehaviorPacks => ReferencedBehaviorPacks.Select(x => new ReferencedPack(x, GetBehaviorPack(x) ?? Minecraft.GetBehaviorPack(x)));
    public IEnumerable<ReferencedPack> ResourcePacks => ReferencedResourcePacks.Select(x => new ReferencedPack(x, GetResourcePack(x) ?? Minecraft.GetResourcePack(x)));
    public World(Minecraft mc, string folder)
    {
        Minecraft = mc;
        Folder = folder;
        WorldName = File.ReadAllText(Path.Combine(folder, "levelname.txt"));
        LocalBehaviorPacks = new(Pack.Load(Path.Combine(folder, "behavior_packs")));
        LocalResourcePacks = new(Pack.Load(Path.Combine(folder, "resource_packs")));
        using var bpf = File.OpenRead(Path.Combine(folder, "world_behavior_packs.json"));
        using var bpm = JsonDocument.Parse(bpf);
        ReferencedBehaviorPacks = new(bpm.RootElement.EnumerateArray().Select(PackReference.ParseReference));
        using var rpf = File.OpenRead(Path.Combine(folder, "world_resource_packs.json"));
        using var rpm = JsonDocument.Parse(rpf);
        ReferencedResourcePacks = new(rpm.RootElement.EnumerateArray().Select(PackReference.ParseReference));
    }

    public Pack? GetBehaviorPack(PackReference reference) => IPackSource.Find(LocalBehaviorPacks, reference);
    public Pack? GetResourcePack(PackReference reference) => IPackSource.Find(LocalResourcePacks, reference);
}
