using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ShulkerBundle;

public class Pack : IStructureSource
{
    public string Name { get; }
    public string Description { get; }
    public string? PackIcon
    {
        get
        {
            string icon = Path.Combine(Folder, "pack_icon.png");
            if (File.Exists(icon))
                return icon;
            return null;
        }
    }
    public string FolderName => Path.GetFileName(Folder);
    public string BundleName { get; }
    public readonly string Folder;
    public readonly Version Version;
    public readonly Version MinEngineVersion;
    public Guid UUID { get; }
    public readonly List<PackReference> Dependencies;
    public List<Structure> Structures { get; }
    public Pack(string folder)
    {
        Folder = folder;
        using var file = File.OpenRead(Path.Combine(folder, "manifest.json"));
        using var manifest = JsonDocument.Parse(file);
        var header = manifest.RootElement.GetProperty("header");
        Version = new Version(header.GetProperty("version"));
        string raw_name = header.GetProperty("name").GetString();
        string raw_desc = header.GetProperty("description").GetString();
        Name = raw_name;
        Description = raw_desc;
        string lang = Path.Combine(folder, "texts", "en_US.lang");
        if (File.Exists(lang))
        {
            foreach (var line in File.ReadLines(lang))
            {
                string trim = line.Trim();
                if (trim.Length == 0 || trim.StartsWith('#'))
                    continue;
                int eq = trim.IndexOf('=');
                if (eq == -1)
                    continue;
                if (trim[..eq] == raw_name)
                    Name = trim[(eq + 1)..];
                if (trim[..eq] == raw_desc)
                    Description = trim[(eq + 1)..];
            }
        }
        var bn = Path.Combine(Folder, "bundlename.txt");
        if (File.Exists(bn))
            BundleName = File.ReadAllText(bn);
        else
            BundleName = FolderName;
        UUID = Guid.Parse(header.GetProperty("uuid").GetString());
        Dependencies = new();
        if (manifest.RootElement.TryGetProperty("dependencies", out var dep))
            Dependencies.AddRange(dep.EnumerateArray().Select(PackReference.ParseDependency));
        Structures = new();
        var structures_folder = Path.Combine(folder, "structures");
        if (Directory.Exists(structures_folder))
        {
            foreach (var item in Directory.GetFiles(structures_folder, "*.mcstructure", SearchOption.AllDirectories))
            {
                string id = Path.ChangeExtension(item[(structures_folder.Length + 1)..], null).Replace('\\', '/');
                int first_slash = id.IndexOf('/');
                if (first_slash == -1)
                    id = "mystructure:" + id;
                else
                    id = id[..first_slash] + ':' + id[(first_slash + 1)..];
                Structures.Add(new Structure(id));
            }
        }
    }

    public PackReference GetReference() => new PackReference(UUID, Version);

    public static IEnumerable<Pack> Load(string folder)
    {
        if (!Directory.Exists(folder))
            yield break;
        foreach (var pack in Directory.GetDirectories(folder))
        {
            if (File.Exists(Path.Combine(pack, "manifest.json")))
                yield return new Pack(pack);
        }
    }

    public Structure? GetStructure(string identifier)
    {
        return Structures.FirstOrDefault(x => x.Identifier == identifier);
    }
}

public enum ReferenceStatus
{
    Dev,
    Local,
    Missing
}

public record ReferencedPack(PackReference Reference, Pack? Pack, ReferenceStatus Status);

public record PackReference(Guid UUID, Version Version)
{
    public JsonObject ToJsonDependency()
    {
        return new JsonObject
        {
            ["uuid"] = UUID,
            ["version"] = Version.ToJson()
        };
    }

    public JsonObject ToJsonReference()
    {
        return new JsonObject
        {
            ["pack_id"] = UUID,
            ["version"] = Version.ToJson()
        };
    }

    public static PackReference ParseDependency(JsonElement json)
    {
        return new PackReference(
            Guid.Parse(json.GetProperty("uuid").GetString()),
            new Version(json.GetProperty("version"))
        );
    }

    public static PackReference ParseReference(JsonElement json)
    {
        return new PackReference(
            Guid.Parse(json.GetProperty("pack_id").GetString()),
            new Version(json.GetProperty("version"))
        );
    }

    public override int GetHashCode()
    {
        return UUID.GetHashCode();
    }
}

public record Version(int Major, int Minor, int Patch)
{
    public Version(JsonElement json) : this(
        json[0].GetInt32(),
        json[1].GetInt32(),
        json[2].GetInt32())
    { }
    public JsonArray ToJson()
    {
        return new JsonArray(Major, Minor, Patch);
    }
}

public record Structure(string Identifier);
