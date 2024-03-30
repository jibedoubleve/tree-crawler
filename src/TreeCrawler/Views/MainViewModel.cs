using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GraphVizNet;
using TreeCrawler.Domain;

namespace TreeCrawler.Views;

public class MainViewModel : ObservableObject
{
    #region Fields

    private List<int>[] _tree = [];
    private string _dotScript = "";
    private string _dotScriptRenderer = "";
    private ImageSource? _imageSource;

    #endregion Fields²²

    #region Constructors

    public MainViewModel()
    {
        CrawlTree = new AsyncRelayCommand(OnCrawlTree);
        RenderTree = new AsyncRelayCommand(OnRenderTree);

        DotScript = """
                    digraph G {
                        0 -> 1
                        0 -> 2
                        1 -> 3
                        2 -> 3
                        2 -> 15
                        3 -> 4
                        3 -> 7
                        4 -> 5
                        4 -> 10
                        4 -> 6
                        4 -> 7
                        4 -> 8
                        7 -> 8
                        7 -> 9
                        7 -> 5
                        8 -> 9
                        9 -> 10
                        9 -> 11
                        9 -> 12
                        10 -> 11
                        11 -> 12
                        12 -> 14
                        12 -> 15
                    }
                    """;
        DotScriptRenderer = DotScript;
    }
    #endregion Constructors

    #region Properties

    public string DotScript
    {
        get => _dotScript;
        set => SetProperty(ref _dotScript, value);
    }
    public string DotScriptRenderer
    {
        get => _dotScriptRenderer;
        set
        {      
            const string toReplace = "digraph G {";
            const string replacement = """
                                       digraph G {
                                       edge [
                                           arrowhead=onormal
                                           penwidth=0.2
                                       ]
                                       """;
            if (!value.Contains(replacement))
            {
                value = value.Replace(toReplace, replacement);
            }

            SetProperty(ref _dotScriptRenderer, value);
        }
    }

    public ImageSource? ImageSource
    {
        get => _imageSource;
        set => SetProperty(ref _imageSource, value);
    }

    public IAsyncRelayCommand CrawlTree { get; }
    public IAsyncRelayCommand RenderTree { get; }

    #endregion Properties

    #region Methods

    private static BitmapFrame GetImageFrom(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
    }

    private async Task OnCrawlTree()
    {
        DotScriptRenderer = DotScript;
        _tree = DotScript.ToTree();
        await new Crawler().ExploreAsync(_tree, (previous, current, colorId) =>
        {
            DotScriptRenderer = DotScriptRenderer.MarkNodeExplored(previous, current, colorId);            
        }, 250);
    }

    private async Task OnRenderTree()
    {
        var bytes = await Task.Run(() =>
        {
            var graphviz = new GraphViz();
            return graphviz.LayoutAndRenderDotGraph(DotScriptRenderer, "png");
        });
        ImageSource = GetImageFrom(bytes);
        OnPropertyChanged(nameof(ImageSource));
    }

    #endregion Methods
}