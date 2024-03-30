namespace TreeCrawler.Domain;

public class Crawler
{
    #region Methods

    private bool[] _visitRegistry = Array.Empty<bool>();
    int _colorIndex = 0;

    private async Task ExploreAsync(List<int>[] tree,int previous, int current, Action<int, int, int> handler, int delay)
    {
        _visitRegistry[current] = true;
        handler(previous, current, _colorIndex);
        await Task.Delay(delay);

        foreach (var node in tree[current])
        {
            if (_visitRegistry[node]) continue;

            await ExploreAsync(tree, current, node, handler, delay);
            _colorIndex++;
        }
    }

    /// <summary>
    /// Implements DFS (Depth First Search) to crawl all the nodes of the tree
    /// </summary>
    /// <param name="tree">The tree to crawl</param>
    /// <param name="handler">Whenever a node is visited, this handler is executed</param>
    /// <param name="delay">Delay to wait before visiting the next node</param>
    public async Task ExploreAsync(List<int>[] tree, Action<int, int, int> handler, int delay)
    {
        _visitRegistry = new bool[tree.Length];
        await ExploreAsync(tree,0, 0, handler, delay);
    }

    #endregion Methods
}