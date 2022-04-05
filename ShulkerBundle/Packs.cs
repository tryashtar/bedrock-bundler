using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShulkerBundle;

public class Pack
{
    public readonly string Folder;
    public readonly Version Version;
    public readonly Version MinEngineVersion;
    public readonly Guid UUID;
    public readonly List<PackReference> Dependencies;
    public readonly List<Module> Modules;
    public Pack(string folder)
    {
        Folder = folder;
        using var file = File.OpenRead(Path.Combine(folder, "manifest.json"));
        using var manifest = JsonDocument.Parse(file);
        var header = manifest.RootElement.GetProperty("header");
        Version = new Version(header.GetProperty("version"));
        Dependencies = new();
        if (manifest.RootElement.TryGetProperty("dependencies", out var dep))
            Dependencies.AddRange(dep.EnumerateArray().Select(PackReference.ParseDependency));
        Modules = manifest.RootElement.GetProperty("modules").EnumerateArray().Select(x => new Module(x)).ToList();
    }

    public static List<T> Load<T>(string folder, Func<string, T> creator) where T : Pack
    {
        var list = new List<T>();
        foreach (var pack in Directory.GetDirectories(folder))
        {
            if (File.Exists(Path.Combine(pack, "manifest.json")))
                list.Add(creator(pack));
        }
        return list;
    }
}

public class BehaviorPack : Pack
{
    public BehaviorPack(string folder) : base(folder) { }
}

public class ResourcePack : Pack
{
    public ResourcePack(string folder) : base(folder) { }
}

public record PackReference
{
    public readonly Guid UUID;
    public readonly Version Version;

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