using ShulkerBundle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShulkerBundleWPF.MVVM.ViewModel;
class MainViewModel
{
    public Minecraft Minecraft { get; private set; }
    public MainViewModel()
    {
        Minecraft = new(@"D:\Minecraft\Bedrock Storage\Launcher\installations\Latest\dev\packageData");
    }
}
