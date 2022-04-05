using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShulkerBundle.MVVM.ViewModel;
class MainViewModel : IDropTarget
{
    public Minecraft Minecraft { get; private set; }
    public World? SelectedWorld { get; set; }
    public MainViewModel()
    {
        Refresh();
    }

    public void Refresh()
    {
        Minecraft = new(@"D:\Minecraft\Bedrock Storage\Launcher\installations\Latest\dev\packageData");
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
        var reference = sourceItem.Reference;
        int index = SelectedWorld.ReferencedResourcePacks.IndexOf(reference);
        SelectedWorld.ReferencedResourcePacks.RemoveAt(index);
        int destination = dropInfo.InsertIndex;
        if (destination > index)
            destination--;
        SelectedWorld.ReferencedResourcePacks.Insert(destination, reference);
    }
}
