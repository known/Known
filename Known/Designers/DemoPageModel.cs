using Known.Blazor;
using Known.Entities;

namespace Known.Designers;

class DemoPageModel : TableModel<Dictionary<string, object>>
{
    private readonly PageInfo _info;
    private readonly EntityInfo _entity;

    internal DemoPageModel(IUIService ui, SysModule module) : base(ui, module)
    {
        _info = module.Page;
        _entity = DataHelper.GetEntity(module.EntityData);
        OnQuery = OnQueryDatas;
    }

    internal DemoPageModel(IUIService ui, PageInfo info, EntityInfo entity) : base(ui, info)
    {
        _info = info;
        _entity = entity;
        OnQuery = OnQueryDatas;
    }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryDatas(PagingCriteria criteria)
    {
        var datas = new List<Dictionary<string, object>>();
        for (int i = 0; i < 3; i++)
        {
            var data = new Dictionary<string, object>();
            foreach (var item in _info.Columns)
            {
                data[item.Id] = GetValue(item, i);
            }
            datas.Add(data);
        }

        var result = new PagingResult<Dictionary<string, object>>(datas);
        return Task.FromResult(result);
    }

    private object GetValue(PageColumnInfo item, int index)
    {
        var value = $"{item.Id}{index}";
        var field = _entity?.Fields?.FirstOrDefault(f => f.Id == item.Id);
        if (field != null)
        {
            switch (field.Type)
            {
                case FieldType.Text:
                    return value;
                case FieldType.TextArea:
                    return "";
                case FieldType.Date:
                    return DateTime.Now;
                case FieldType.Number:
                    return index;
                case FieldType.Switch:
                case FieldType.CheckBox:
                    return true;
                case FieldType.CheckList:
                    return "Item1,Item2";
                case FieldType.RadioList:
                case FieldType.Select:
                    return item.Id;
                case FieldType.File:
                    return "附件";
            }
        }

        if (item.Id.EndsWith("Date") || item.Id.EndsWith("Time"))
            return DateTime.Now;

        return value;
    }
}