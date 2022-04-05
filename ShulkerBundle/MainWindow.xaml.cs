using Microsoft.Win32;
using ShulkerBundle.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShulkerBundle;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Bundle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var model = (MainViewModel)this.DataContext;
        var save = new SaveFileDialog();
        save.FileName = model.SelectedWorld.WorldName;
        save.DefaultExt = ".mcworld";
        save.Filter = "Minecraft Worlds (.mcworld)|*.mcworld";
        var result = save.ShowDialog();
        if (result == true)
            model.SelectedWorld.BundleTo(save.FileName);
    }

    private void Unbundle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var model = (MainViewModel)this.DataContext;
    }
}
