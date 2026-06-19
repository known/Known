namespace Known.Reports;

/// <summary>
/// 报表设置表单类。
/// </summary>
public partial class ReportForm
{
    private List<ReportBlock> blocks = [];
    private ReportBlock selectedBlock;
    private ReportBlock draggingBlock;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        LoadConfig();
        Model.OnSaving = data =>
        {
            data.Blocks = blocks;
            return Task.FromResult(true);
        };
    }

    private void LoadConfig()
    {
        blocks = Model.Data.Blocks ?? [];
        selectedBlock = blocks.FirstOrDefault();
    }

    private string GetGridStyle()
    {
        var cols = Math.Clamp(Model.Data.GridColumn ?? 1, 1, 6);
        return $"grid-template-columns: repeat({cols}, 1fr);";
    }

    private List<(int Row, int Col)> GetEmptyCells()
    {
        var occupied = new HashSet<(int, int)>();
        foreach (var block in blocks)
        {
            for (int r = block.Row; r < block.Row + block.Height; r++)
                for (int c = block.Col; c < block.Col + (block.Width ?? 1); c++)
                    occupied.Add((r, c));
        }

        var maxRow = blocks.Count > 0 ? blocks.Max(b => b.Row + b.Height - 1) : 0;
        var cols = Math.Clamp(Model.Data.GridColumn ?? 1, 1, 6);
        var empty = new List<(int, int)>();
        for (int r = 1; r <= maxRow + 1; r++)
            for (int c = 1; c <= cols; c++)
                if (!occupied.Contains((r, c)))
                    empty.Add((r, c));
        return empty;
    }

    private void OnAddChartBlock()
    {
        var pos = GetNextPosition();
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = ReportBlockType.Chart,
            Title = $"图表 {blocks.Count + 1}",
            Row = pos.Row,
            Col = pos.Col,
            Width = 1,
            Height = 1,
            Chart = new ChartConfig { Title = $"图表 {blocks.Count + 1}", DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
        StateChanged();
    }

    private void OnAddTableBlock()
    {
        var pos = GetNextPosition();
        var block = new ReportBlock
        {
            Id = Utils.GetGuid(),
            BlockType = ReportBlockType.Table,
            Title = $"表格 {blocks.Count + 1}",
            Row = pos.Row,
            Col = pos.Col,
            Width = 1,
            Height = 1,
            Table = new TableConfig { Columns = [], DataSource = new DataSourceConfig() },
            DataSource = new DataSourceConfig()
        };
        blocks.Add(block);
        selectedBlock = block;
        StateChanged();
    }

    private (int Row, int Col) GetNextPosition()
    {
        var cols = Math.Clamp(Model.Data.GridColumn ?? 1, 1, 6);
        var occupied = new HashSet<(int, int)>();
        foreach (var b in blocks)
        {
            for (int r = b.Row; r < b.Row + b.Height; r++)
                for (int c = b.Col; c < b.Col + (b.Width ?? 1); c++)
                    occupied.Add((r, c));
        }
        int row = 1;
        while (true)
        {
            for (int c = 1; c <= cols; c++)
            {
                if (!occupied.Contains((row, c)))
                    return (row, c);
            }
            row++;
        }
    }

    private void OnSelectBlock(ReportBlock block)
    {
        selectedBlock = block;
        StateChanged();
    }

    private void OnConfigBlock(ReportBlock block)
    {
        selectedBlock = block;
        UI.ShowDialog(new DialogModel
        {
            Title = $"配置 - {block.Title}",
            Width = 700,
            Content = b => b.Component<ConfigForm>().Set(c => c.Block, block).Build(),
            OnOk = async () => await Task.CompletedTask
        });
    }

    private void OnRemoveBlock(ReportBlock block)
    {
        blocks.Remove(block);
        if (selectedBlock?.Id == block.Id)
            selectedBlock = blocks.FirstOrDefault();
        StateChanged();
    }

    private DropdownModel GetBlockMenu(ReportBlock block)
    {
        return new DropdownModel
        {
            Icon = "ellipsis",
            TriggerType = "Click",
            Items =
            [
                new ActionInfo("配置") { Icon = "setting", OnClick = this.Callback<MouseEventArgs>(e => OnConfigBlock(block)) },
                new ActionInfo(Language.Delete) { OnClick = this.Callback<MouseEventArgs>(e => OnRemoveBlock(block)) }
            ]
        };
    }

    private void OnDragStart(DragEventArgs e, ReportBlock block)
    {
        draggingBlock = block;
        e.DataTransfer.EffectAllowed = "move";
    }

    private void OnDrop(DragEventArgs e, ReportBlock targetBlock)
    {
        if (draggingBlock == null || draggingBlock.Id == targetBlock.Id)
        {
            draggingBlock = null;
            return;
        }
        (draggingBlock.Row, targetBlock.Row) = (targetBlock.Row, draggingBlock.Row);
        (draggingBlock.Col, targetBlock.Col) = (targetBlock.Col, draggingBlock.Col);
        draggingBlock = null;
        StateChanged();
    }

    private void OnDropEmpty(DragEventArgs e, int row, int col)
    {
        if (draggingBlock == null) return;
        var existing = blocks.FirstOrDefault(b => b.Row == row && b.Col == col);
        if (existing != null)
        {
            (draggingBlock.Row, existing.Row) = (existing.Row, draggingBlock.Row);
            (draggingBlock.Col, existing.Col) = (existing.Col, draggingBlock.Col);
        }
        else
        {
            draggingBlock.Row = row;
            draggingBlock.Col = col;
        }
        draggingBlock = null;
        StateChanged();
    }
}
