using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    public readonly string Folder;
    public readonly Version Version;
    public readonly Version MinEngineVersion;
    public Guid UUID { get; private set; }
    public readonly List<PackReference> Dependencies;
    public readonly List<Module> Modules;
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
        UUID = Guid.Parse(header.GetProperty("uuid").GetString());
        Dependencies = new();
        if (manifest.RootElement.TryGetProperty("dependencies", out var dep))
            Dependencies.AddRange(dep.EnumerateArray().Select(PackReference.ParseDependency));
        Modules = manifest.RootElement.GetProperty("modules").EnumerateArray().Select(x => new Module(x)).ToList();
    }

    public static IEnumerable<Pack> Load(string folder)
    {
        foreach (var pack in Directory.GetDirectories(folder))
        {
            if (File.Exists(Path.Combine(pack, "manifest.json")))
                yield return new Pack(pack);
        }
    }
}

public class ReferencedPack
{
    public PackReference Reference { get; private set; }
    public Pack Pack { get; private set; }
    public ReferencedPack(PackReference reference, Pack pack)
    {
        Reference = reference;
        Pack = pack;
    }
}

public record PackReference
{
    public Guid UUID { get; private set; }
    public Version Version { get; private set; }

    private PackReference(Guid uuid, Version version)
    {
        UUID = uuid;
        Version = version;
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
}

public record Module
{
    public readonly string Type;
    public readonly Guid UUID;
    public readonly Version Version;
    public Module(JsonElement json)
    {
        Type = json.GetProperty("type").GetString();
        UUID = Guid.Parse(json.GetProperty("uuid").GetString());
        Version = new Version(json.GetProperty("version"));
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
}