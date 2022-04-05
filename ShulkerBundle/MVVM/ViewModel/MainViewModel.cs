using GongSolutions.Wpf.DragDrop;
using ShulkerBundle.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShulkerBundle.MVVM.ViewModel;
class MainViewModel : ObservableObject, IDropTarget
{
    private Minecraft minecraft;
    public Minecraft Minecraft
    {
        get { return minecraft; }
        set { minecraft = value; OnPropertyChanged(); }
    }

    private World selectedworld;
    public World SelectedWorld
    {
        get { return selectedworld; }
        set { selectedworld = value; OnPropertyChanged(); }
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
        var sourceItem = dropInfo.Data as ReferencedPack;
        var targetItem = dropInfo.TargetItem as ReferencedPack;

        if (sourceItem != null && targetItem != null)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        var sourceItem = dropInfo.Data as ReferencedPack;
        var targetItem = dropInfo.TargetItem as ReferencedPack;
    }
}
