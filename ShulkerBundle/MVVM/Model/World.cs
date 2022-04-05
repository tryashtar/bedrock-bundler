using ShulkerBundle.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ShulkerBundle;
public class World : ObservableObject, IPackSource
{
    public readonly Minecraft Minecraft;
    public string WorldName { get; private set; }
    public string WorldIcon => Path.Combine(Folder, "world_icon.jpeg");
    public string FolderName => Path.GetFileName(Folder);
    public bool CanUnbundle => BehaviorPacks.Concat(ResourcePacks).Any(x => x.Status == ReferenceStatus.Local);
    public readonly string Folder;
    public ObservableCollection<Pack> LocalBehaviorPacks { get; private set; }
    public ObservableCollection<Pack> LocalResourcePacks { get; private set; }
    public ObservableCollection<PackReference> ReferencedBehaviorPacks { get; private set; }
    public ObservableCollection<PackReference> ReferencedResourcePacks { get; private set; }
    public IEnumerable<ReferencedPack> BehaviorPacks => ReferencedBehaviorPacks.Select(FindBehaviorPack);
    public IEnumerable<ReferencedPack> ResourcePacks => ReferencedResourcePacks.Select(FindResourcePack);
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
        ReferencedBehaviorPacks.CollectionChanged += (s, e) => UpdatePacks();
        ReferencedResourcePacks.CollectionChanged += (s, e) => UpdatePacks();
    }

    private ReferencedPack FindPack(PackReference reference, Func<IPackSource, PackReference, Pack?> getter)
    {
        var pack = getter(this, reference);
        if (pack != null)
            return new ReferencedPack(reference, pack, ReferenceStatus.Local);
        pack = getter(Minecraft, reference);
        if (pack != null)
            return new ReferencedPack(reference, pack, ReferenceStatus.Dev);
        return new ReferencedPack(reference, pack, ReferenceStatus.Missing);
    }

    public ReferencedPack FindBehaviorPack(PackReference reference) => FindPack(reference, (x, y) => x.GetBehaviorPack(y));
    public ReferencedPack FindResourcePack(PackReference reference) => FindPack(reference, (x, y) => x.GetResourcePack(y));

    public void Unbundle()
    {
        UpdatePacks();
        foreach (var pack in BehaviorPacks.Where(x => x.Status == ReferenceStatus.Local))
        {
            var dest = Path.Combine(Minecraft.Folder, "development_behavior_packs", pack.Pack.FolderName);
            Directory.Move(pack.Pack.Folder, dest);
            File.WriteAllText(Path.Combine(dest, "bundlename.txt"), pack.Pack.FolderName);
        }
        foreach (var pack in ResourcePacks.Where(x => x.Status == ReferenceStatus.Local))
        {
            var dest = Path.Combine(Minecraft.Folder, "development_resource_packs", pack.Pack.FolderName);
            Directory.Move(pack.Pack.Folder, dest);
            File.WriteAllText(Path.Combine(dest, "bundlename.txt"), pack.Pack.FolderName);
        }
    }

    public void BundleTo(string destination)
    {
        UpdatePacks();
        if (File.Exists(destination))
            File.Delete(destination);
        ZipFile.CreateFromDirectory(this.Folder, destination);
        using var zip = ZipFile.Open(destination, ZipArchiveMode.Update);
        foreach (var pack in BehaviorPacks.Where(x => x.Status == ReferenceStatus.Dev))
        {
            ZipPack(zip, pack.Pack, $"behavior_packs/{pack.Pack.BundleName}");
        }
        foreach (var pack in ResourcePacks.Where(x => x.Status == ReferenceStatus.Dev))
        {
            ZipPack(zip, pack.Pack, $"resource_packs/{pack.Pack.BundleName}");
        }
    }

    private void ZipPack(ZipArchive zip, Pack pack, string entry)
    {
        foreach (var file in Directory.EnumerateFiles(pack.Folder, "*", SearchOption.AllDirectories))
        {
            string relative_path = file.Replace(pack.Folder, "").TrimStart('\\');
            if (relative_path == "bundlename.txt")
                continue;
            zip.CreateEntryFromFile(file, Path.Combine(entry, relative_path));
        }
    }

    public Pack? GetBehaviorPack(PackReference reference) => IPackSource.Find(LocalBehaviorPacks, reference);
    public Pack? GetResourcePack(PackReference reference) => IPackSource.Find(LocalResourcePacks, reference);

    private void UpdatePacks()
    {
        OnPropertyChanged(nameof(CanUnbundle));
        OnPropertyChanged(nameof(BehaviorPacks));
        OnPropertyChanged(nameof(ResourcePacks));
        var indented = new JsonSerializerOptions() { WriteIndented = true };
        File.WriteAllText(Path.Combine(Folder, "world_behavior_packs.json"), new JsonArray(ReferencedBehaviorPacks.Select(x => x.ToJsonReference()).ToArray()).ToJsonString(indented));
        File.WriteAllText(Path.Combine(Folder, "world_resource_packs.json"), new JsonArray(ReferencedResourcePacks.Select(x => x.ToJsonReference()).ToArray()).ToJsonString(indented));
    }
}
