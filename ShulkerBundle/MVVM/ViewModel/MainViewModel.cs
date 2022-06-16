using GongSolutions.Wpf.DragDrop;
using ShulkerBundle.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShulkerBundle.MVVM.ViewModel;
class MainViewModel : ObservableObject, IDropTarget
{
    private Minecraft minecraft;
    private World selectedworld;
    private ObservableCollection<ReferencedPack> availableBehaviorPacks;
    private ObservableCollection<ReferencedPack> availableResourcePacks;
    private ObservableCollection<ReferencedPack> activeBehaviorPacks;
    private ObservableCollection<ReferencedPack> activeResourcePacks;

    public Minecraft Minecraft
    {
        get { return minecraft; }
        set { minecraft = value; OnPropertyChanged(); }
    }

    public World SelectedWorld
    {
        get { return selectedworld; }
        set
        {
            selectedworld = value;
            OnPropertyChanged();
            ActiveBehaviorPacks = new(selectedworld.ActiveBehaviorPacks.Select(selectedworld.FindBehaviorPack));
            ActiveResourcePacks = new(selectedworld.ActiveResourcePacks.Select(selectedworld.FindResourcePack));
            var bps = selectedworld.LocalBehaviorPacks.Select(x => new ReferencedPack(x.GetReference(), x, ReferenceStatus.Local)).Concat(
                minecraft.DevBehaviorPacks.Select(x => new ReferencedPack(x.GetReference(), x, ReferenceStatus.Dev))).ToList();
            for (int i = bps.Count - 1; i >= 0; i--)
            {
                if (ActiveBehaviorPacks.Any(x => x.Pack == bps[i].Pack))
                    bps.RemoveAt(i);
            }
            var rps = selectedworld.LocalResourcePacks.Select(x => new ReferencedPack(x.GetReference(), x, ReferenceStatus.Local)).Concat(
                minecraft.DevResourcePacks.Select(x => new ReferencedPack(x.GetReference(), x, ReferenceStatus.Dev))).ToList();
            for (int i = rps.Count - 1; i >= 0; i--)
            {
                if (ActiveResourcePacks.Any(x => x.Pack == rps[i].Pack))
                    rps.RemoveAt(i);
            }
            AvailableBehaviorPacks = new(bps);
            AvailableResourcePacks = new(rps);
        }
    }

    public bool CanUnbundle => ActiveBehaviorPacks != null && ActiveResourcePacks != null && ActiveBehaviorPacks.Concat(ActiveResourcePacks).Any(x => x.Status == ReferenceStatus.Local);
    public ObservableCollection<ReferencedPack> AvailableBehaviorPacks { get => availableBehaviorPacks; private set { availableBehaviorPacks = value; OnPropertyChanged(); } }
    public ObservableCollection<ReferencedPack> AvailableResourcePacks { get => availableResourcePacks; private set { availableResourcePacks = value; OnPropertyChanged(); } }
    public ObservableCollection<ReferencedPack> ActiveBehaviorPacks
    {
        get => activeBehaviorPacks;
        private set
        {
            if (activeBehaviorPacks != null)
                activeBehaviorPacks.CollectionChanged -= ActiveBehaviorPacks_CollectionChanged;
            activeBehaviorPacks = value;
            activeBehaviorPacks.CollectionChanged += ActiveBehaviorPacks_CollectionChanged;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanUnbundle));
        }
    }

    public ObservableCollection<ReferencedPack> ActiveResourcePacks
    {
        get => activeResourcePacks;
        private set
        {
            if (activeResourcePacks != null)
                activeResourcePacks.CollectionChanged -= ActiveResourcePacks_CollectionChanged;
            activeResourcePacks = value;
            activeResourcePacks.CollectionChanged += ActiveResourcePacks_CollectionChanged;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanUnbundle));
        }
    }

    private void ActiveBehaviorPacks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanUnbundle));
        SelectedWorld.ActiveBehaviorPacks.Clear();
        foreach (var item in ActiveBehaviorPacks)
        {
            SelectedWorld.ActiveBehaviorPacks.Add(item.Reference);
        }
    }

    private void ActiveResourcePacks_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanUnbundle));
        SelectedWorld.ActiveResourcePacks.Clear();
        foreach (var item in ActiveResourcePacks)
        {
            SelectedWorld.ActiveResourcePacks.Add(item.Reference);
        }
    }

    public MainViewModel()
    {
        Properties.Settings.Default.MinecraftFolder = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.MinecraftFolder);
        Refresh();
    }

    public void Refresh()
    {
        Minecraft = new(Properties.Settings.Default.MinecraftFolder);
    }

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        GetDropAction(dropInfo);
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        GetDropAction(dropInfo)();
    }

    private Action GetDropAction(IDropInfo info)
    {
        var moving = info.Data as ReferencedPack;
        var from = info.DragInfo.SourceCollection as ObservableCollection<ReferencedPack>;
        var to = info.TargetCollection as ObservableCollection<ReferencedPack>;
        // equip pack
        if ((from == AvailableBehaviorPacks && to == ActiveBehaviorPacks) || (from == AvailableResourcePacks && to == ActiveResourcePacks))
        {
            info.Effects = DragDropEffects.Move;
            info.DropTargetAdorner = DropTargetAdorners.Insert;
            return () =>
            {
                from.Remove(moving);
                to.Insert(info.InsertIndex, moving);
            };
        }
        // reorder pack
        if (to == from)
        {
            info.Effects = DragDropEffects.Move;
            if (to == ActiveBehaviorPacks || to == ActiveResourcePacks)
            {
                info.DropTargetAdorner = DropTargetAdorners.Insert;
                return () =>
                {
                    from.RemoveAt(info.DragInfo.SourceIndex);
                    int index = info.InsertIndex;
                    if (index > info.DragInfo.SourceIndex)
                        index--;
                    from.Insert(index, moving);
                };
            }
        }
        // remove pack
        if ((from == ActiveBehaviorPacks && to == AvailableBehaviorPacks) || (from == ActiveResourcePacks && to == AvailableResourcePacks))
        {
            info.Effects = DragDropEffects.Move;
            return () =>
            {
                from.RemoveAt(info.DragInfo.SourceIndex);
                to.Insert(0, moving);
            };
        }
        return () => { };
    }
}
