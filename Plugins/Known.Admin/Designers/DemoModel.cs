namespace Known.Designers;

class DemoModel : TableModel<Dictionary<string, object>>
{
    internal DemoModel(BaseComponent page) : base(page)
    {
        OnQuery = OnQueryDatas;
        OnAction = OnRowAction;
        Toolbar.OnItemClick = OnItemClick;
    }

    internal SysModule1 Module { get; set; }
    internal EntityInfo Entity { get; set; }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryDatas(PagingCriteria criteria)
    {
        var total = 0;
        var statis = new Dictionary<string, object>();
        var datas = new List<Dictionary<string, object>>();
        for (int i = 0; i < 3; i++)
        {
            var data = new Dictionary<string, object>();
            foreach (var item in Columns)
            {
                data[item.Id] = GetValue(item, i);
                if (item.IsSum)
                {
                    total += i;
                    statis[item.Id] = total;
                }
            }
            datas.Add(data);
        }

        var result = new PagingResult<Dictionary<string, object>>(datas) { Statis = statis };
        return Task.FromResult(result);
    }

    private Task<Result> OnSave(Dictionary<string, object> dictionary)
    {
        return Known.Result.SuccessAsync(Language.Success(Language.Save));
    }

    private void OnItemClick(ActionInfo info)
    {
        if (info.Id == "New")
        {
            NewForm(OnSave, []);
            return;
        }

        ShowNoMethod(info);
    }

    private void OnRowAction(ActionInfo info, Dictionary<string, object> row)
    {
        if (info.Id == "Edit")
        {
            EditForm(OnSave, row);
            return;
        }

        ShowNoMethod(info);
    }

    private void ShowNoMethod(ActionInfo info)
    {
        var message = Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{info.Id}]");
        UI.Error(message);
    }

    private object GetValue(ColumnInfo item, int index)
    {
        var value = $"{item.Id}{index}";
        var field = Entity?.Fields?.FirstOrDefault(f => f.Id == item.Id);
        if (field != null)
        {
            switch (field.Type)
            {
                case FieldType.Text:
                    return value;
                case FieldType.TextArea:
                    return "";
                case FieldType.Date:
                case FieldType.DateTime:
                    return DateTime.Now;
                case FieldType.Integer:
                    return index;
                case FieldType.Number:
                    return index * 0.1M;
                case FieldType.Switch:
                case FieldType.CheckBox:
                    return true;
                case FieldType.CheckList:
                    return "Item1,Item2";
                case FieldType.RadioList:
                case FieldType.Select:
                    return item.Id;
                case FieldType.File:
                    return "File";
            }
        }

        if (item.Id.EndsWith("Date") || item.Id.EndsWith("Time"))
            return DateTime.Now;

        return value;
    }
}