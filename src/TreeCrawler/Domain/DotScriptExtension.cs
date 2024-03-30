namespace TreeCrawler.Domain;

public static class TreeExtension
{
    public static List<int>[] ToTree(this string dotScript)
    {
        var result = dotScript.Replace("digraph G {", "")
                              .Replace("\n}", "")
                              .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var tree = new List<int>[result.Length];
        for (var i = 0; i < tree.Length; i++) { tree[i] = []; }

        for (var i = 0; i < result.Length; i++)
        {
            var item = result[i];
            var r = item.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);


            if (r.Length != 2) continue;

            var a = int.Parse(r[0]);
            var b = int.Parse(r[1]);
            tree[a].Add(b);
        }

        return tree;
    }
}

public static class DotScriptExtension
{
    private const string Closing = "\n}";

    public static string MarkNodeExplored(this string dotScript,int previous, int current, int colorId)
    {
        dotScript = MarkPathExplored(dotScript,previous, current, colorId);
        dotScript = dotScript.Replace(Closing, "");
        dotScript += $"\n    {current} [style=filled, color={ColorManager.GetColorFromIndex(colorId + 5)}]" + Closing;
        return dotScript;
    }
    private static string MarkPathExplored(this string dotScript,int previous, int current, int colorId)
    {
        var line = $"{previous} -> {current}";
        dotScript = dotScript.Replace(line + Environment.NewLine, $"{line} [color={ColorManager.GetColorFromIndex(colorId + 5)}, penwidth=2, arrowhead=normal]\n");
        return dotScript;
    }
}