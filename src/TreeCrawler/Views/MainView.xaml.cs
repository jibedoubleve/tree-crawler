using System.Windows;
using System.Windows.Controls;

namespace TreeCrawler.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm) await vm.RenderTree.ExecuteAsync(null);
    }
}