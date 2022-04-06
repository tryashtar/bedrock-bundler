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

namespace ShulkerBundle.MVVM.ViewModel;
class WorldViewModel : ObservableObject
{
    public World World { get; private set; }
    public bool CanUnbundle { get; private set; }
    public ObservableCollection<ReferencedPack> AvailableBehaviorPacks { get; private set; }
    public ObservableCollection<ReferencedPack> AvailableResourcePacks { get; private set; }
    public ObservableCollection<ReferencedPack> ActiveBehaviorPacks { get; private set; }
    public ObservableCollection<ReferencedPack> ActiveResourcePacks { get; private set; }
    public WorldViewModel()
    {

    }
}