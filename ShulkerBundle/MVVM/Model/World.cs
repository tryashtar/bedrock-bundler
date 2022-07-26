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
public class World : ObservableObject, IPackSource, IStructureSource
{
    public readonly Minecraft Minecraft;
    public string WorldName { get; }
    public string WorldIcon => Path.Combine(Folder, "world_icon.jpeg");
    public string FolderName => Path.GetFileName(Folder);
    public readonly string Folder;
    public ObservableCollection<Pack> LocalBehaviorPacks { get; }
    public ObservableCollection<Pack> LocalResourcePacks { get; }
    public ObservableCollection<PackReference> ActiveBehaviorPacks { get; }
    public ObservableCollection<PackReference> ActiveResourcePacks { get; }
    public ObservableCollection<Structure> EmbeddedStructures { get; }

    public World(Minecraft mc, string folder)
    {
        Minecraft = mc;
        Folder = folder;
        WorldName = File.ReadAllText(Path.Combine(folder, "levelname.txt"));
        string bp = Path.Combine(folder, "behavior_packs");
        string rp = Path.Combine(folder, "resource_packs");
        LocalBehaviorPacks = new(Pack.Load(bp));
        LocalResourcePacks = new(Pack.Load(rp));
        string wbp = Path.Combine(folder, "world_behavior_packs.json");
        string wrp = Path.Combine(folder, "world_resource_packs.json");
        if (File.Exists(wbp))
        {
            using var bpf = File.OpenRead(wbp);
            using var bpm = JsonDocument.Parse(bpf);
            ActiveBehaviorPacks = new(bpm.RootElement.EnumerateArray().Select(PackReference.ParseReference));
        }
        else
            ActiveBehaviorPacks = new();
        if (File.Exists(wrp))
        {
            using var rpf = File.OpenRead(wrp);
            using var rpm = JsonDocument.Parse(rpf);
            ActiveResourcePacks = new(rpm.RootElement.EnumerateArray().Select(PackReference.ParseReference));
        }
        else
            ActiveResourcePacks = new();
        ActiveBehaviorPacks.CollectionChanged += (s, e) => UpdatePacks();
        ActiveResourcePacks.CollectionChanged += (s, e) => UpdatePacks();
        EmbeddedStructures = new();
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

    public enum UnbundleDecision
    {
        Overwrite,
        Discard
    }

    public class UnbundleResponse
    {
        public bool Unfinished => RelevantPacks.Any();
        public List<Pack> RelevantPacks = new();
        private readonly List<(UnbundleDecision decision, Action code)> Continues = new();
        public void ContinueWith(UnbundleDecision decision)
        {
            foreach (var c in Continues.Where(x => x.decision == decision))
            {
                c.code();
            }
        }
        public void AddContinue(UnbundleDecision decision, Action code)
        {
            Continues.Add((decision, code));
        }
    }

    public UnbundleResponse Unbundle()
    {
        UpdatePacks();
        var response = new UnbundleResponse();
        void handle_packs(IEnumerable<ReferencedPack> packlist, Func<PackReference, Pack> exist_check, string dev_folder)
        {
            foreach (var pack in packlist.Where(x => x.Status == ReferenceStatus.Local))
            {
                var existing_pack = exist_check(pack.Reference);
                if (existing_pack != null)
                {
                    response.RelevantPacks.Add(pack.Pack);
                    response.AddContinue(UnbundleDecision.Overwrite, () =>
                    {
                        Directory.Delete(existing_pack.Folder, true);
                        Directory.Move(pack.Pack.Folder, existing_pack.Folder);
                        File.WriteAllText(Path.Combine(existing_pack.Folder, "bundlename.txt"), pack.Pack.FolderName);
                    });
                    response.AddContinue(UnbundleDecision.Discard, () =>
                    {
                        Directory.Delete(pack.Pack.Folder, true);
                    });
                }
                else
                {
                    var dest = Path.Combine(Minecraft.Folder, dev_folder, pack.Pack.FolderName);
                    while (Directory.Exists(dest))
                    {
                        dest += '_';
                    }
                    Directory.Move(pack.Pack.Folder, dest);
                    File.WriteAllText(Path.Combine(dest, "bundlename.txt"), pack.Pack.FolderName);
                }
            }
        }
        handle_packs(ActiveBehaviorPacks.Select(FindBehaviorPack), Minecraft.GetBehaviorPack, "development_behavior_packs");
        handle_packs(ActiveResourcePacks.Select(FindResourcePack), Minecraft.GetResourcePack, "development_resource_packs");
        return response;
    }

    public void BundleTo(string destination)
    {
        UpdatePacks();
        if (File.Exists(destination))
            File.Delete(destination);
        ZipFile.CreateFromDirectory(this.Folder, destination);
        using var zip = ZipFile.Open(destination, ZipArchiveMode.Update);
        foreach (var pack in ActiveBehaviorPacks.Select(FindBehaviorPack).Where(x => x.Status == ReferenceStatus.Dev))
        {
            ZipPack(zip, pack.Pack, $"behavior_packs/{pack.Pack.BundleName}");
        }
        foreach (var pack in ActiveResourcePacks.Select(FindResourcePack).Where(x => x.Status == ReferenceStatus.Dev))
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
        var indented = new JsonSerializerOptions() { WriteIndented = true };
        File.WriteAllText(Path.Combine(Folder, "world_behavior_packs.json"), new JsonArray(ActiveBehaviorPacks.Select(x => x.ToJsonReference()).ToArray()).ToJsonString(indented));
        File.WriteAllText(Path.Combine(Folder, "world_resource_packs.json"), new JsonArray(ActiveResourcePacks.Select(x => x.ToJsonReference()).ToArray()).ToJsonString(indented));
    }

    public Structure? GetStructure(string identifier)
    {
        return EmbeddedStructures.FirstOrDefault(x => x.Identifier == identifier);
    }
}
