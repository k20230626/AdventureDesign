namespace adventuredesign8puzzle.Extension;

public static class Extension_Grid {
    private static readonly ColumnDefinition column = new ColumnDefinition
        { Width = new GridLength(1, GridUnitType.Star) };

    private static readonly RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };

    public static Grid GenNxNGrid(this Grid grid, int n) {
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        for (int i = 0; i < n; i++) {
            grid.ColumnDefinitions.Add(column);
            grid.RowDefinitions.Add(row);
        }
        
        return grid;
    }
}
