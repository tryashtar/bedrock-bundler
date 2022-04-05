using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
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
        var response = model.SelectedWorld.Unbundle();
        if (response.Unfinished)
        {
            var answer = MessageBox.Show($"These packs already exist:\n{String.Join('\n', response.RelevantPacks.Select(x => x.Name))}\n\nWould you like to overwrite your packs with these?", "Duplicate packs!", MessageBoxButton.YesNoCancel);
            if (answer == MessageBoxResult.Yes)
                response.ContinueWith(World.UnbundleDecision.Overwrite);
            else if (answer == MessageBoxResult.No)
                response.ContinueWith(World.UnbundleDecision.Discard);
        }
    }

    private void SelectFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();
        dialog.SelectedPath = Properties.Settings.Default.MinecraftFolder;
        if (dialog.ShowDialog() == true)
        {
            Properties.Settings.Default.MinecraftFolder = dialog.SelectedPath;
            Properties.Settings.Default.Save();
            ((MainViewModel)this.DataContext).Refresh();
        }
    }
}
