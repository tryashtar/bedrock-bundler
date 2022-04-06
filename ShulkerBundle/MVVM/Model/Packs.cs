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

public class Pack
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PackIcon
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
    public string BundleName { get; private set; }
    public readonly string Folder;
    public readonly Version Version;
    public readonly Version MinEngineVersion;
    public Guid UUID { get; private set; }
    public readonly List<PackReference> Dependencies;
    public Pack(string folder)
    {
        Folder = folder;
        using var file = File.OpenRead(Path.Combine(folder, "manifest.json"));
        using var manifest = JsonDocument.Parse(file);
        var header = manifest.RootElement.GetProperty("header");
        Version = new Version(header.GetProperty("version"));
        string name = header.GetProperty("name").GetString();
        string desc = header.GetProperty("description").GetString();
        Name = name;
        Description = desc;
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
                if (trim[..eq] == name)
                    Name = trim[(eq + 1)..];
                if (trim[..eq] == desc)
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
    }

    public PackReference GetReference() => new PackReference(UUID, Version);

    public static IEnumerable<Pack> Load(string folder)
    {
        foreach (var pack in Directory.GetDirectories(folder))
        {
            if (File.Exists(Path.Combine(pack, "manifest.json")))
                yield return new Pack(pack);
        }
    }
}

public enum ReferenceStatus
{
    Dev,
    Local,
    Missing
}

public class ReferencedPack
{
    public PackReference Reference { get; private set; }
    public Pack? Pack { get; private set; }
    public ReferenceStatus Status { get; private set; }
    public ReferencedPack(PackReference reference, Pack? pack, ReferenceStatus status)
    {
        Reference = reference;
        Pack = pack;
        Status = status;
    }
}

public record PackReference
{
    public Guid UUID { get; private set; }
    public Version Version { get; private set; }

    public PackReference(Guid uuid, Version version)
    {
        UUID = uuid;
        Version = version;
    }

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

public record Version
{
    public readonly int Major;
    public readonly int Minor;
    public readonly int Patch;
    public Version(JsonElement json)
    {
        Major = json[0].GetInt32();
        Minor = json[1].GetInt32();
        Patch = json[2].GetInt32();
    }
    public JsonArray ToJson()
    {
        return new JsonArray(Major, Minor, Patch);
    }
}