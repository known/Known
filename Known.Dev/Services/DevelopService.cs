/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Known.Dev.Models;

namespace Known.Dev.Services;

class DevelopService : ServiceBase, IService
{
    private static readonly string path = Path.Combine(Config.RootPath, "data_domain.data");
    private static readonly List<DomainInfo> models = new();

    static DevelopService()
    {
        var json = Utils.ReadFile(path);
        var data = Utils.FromJson<List<DomainInfo>>(json);
        if (data != null && data.Count > 0)
            models.AddRange(data);
    }

    #region Code
    public List<object> GetModels()
    {
        var lists = new List<object>();
        var systems = models.Select(d => d.System).Distinct();
        foreach (var sys in systems)
        {
            lists.Add(new { pid = "0", id = sys, name = sys, title = sys });
            var categories = models.Where(d => d.System == sys)
                                   .Select(d => d.Category).Distinct();
            foreach (var item in categories)
            {
                lists.Add(new { pid = sys, id = $"{sys}_{item}", name = item, title = item });
            }
        }
        lists.AddRange(models.Select(l => new
        {
            id = l.Id,
            name = l.Name,
            title = l.Name,
            pid = $"{l.System}_{l.Category}"
        }));

        return lists;
    }

    public Result GetModel(string id)
    {
        var model = models.FirstOrDefault(m => m.Id == id);
        if (model == null)
            return Result.Error("模型不存在！");

        var helper = new CodeHelper(model);
        return Result.Success("", new
        {
            Model = model,
            View = HttpUtility.HtmlEncode(helper.GetView()),
            Entity = HttpUtility.HtmlEncode(helper.GetEntity()),
            Controller = HttpUtility.HtmlEncode(helper.GetController()),
            Service = HttpUtility.HtmlEncode(helper.GetService()),
            Repository = HttpUtility.HtmlEncode(helper.GetRepository()),
            Sql = HttpUtility.HtmlEncode(helper.GetSql())
        });
    }

    public Result SaveModel(string data)
    {
        var form = Utils.FromJson<DomainInfo>(data);
        if (form == null)
            return Result.Error(Language.NotPostData);

        var model = models.FirstOrDefault(m => m.Id == form.Id);
        if (model == null)
        {
            model = new DomainInfo();
            model.Id = Utils.GetGuid();
            models.Add(model);
        }

        model.System = form.System;
        model.Category = form.Category;
        model.Prefix = form.Prefix;
        model.Code = form.Code;
        model.Name = form.Name;
        model.FieldData = form.FieldData;

        SaveModels();
        return Result.Success(Language.SaveSuccess, model.Id);
    }

    public Result DeleteModel(string id)
    {
        var model = models.FirstOrDefault(m => m.Id == id);
        if (model == null)
            return Result.Error("模型不存在！");

        models.Remove(model);
        SaveModels();
        return Result.Success(Language.DeleteSuccess);
    }

    private void SaveModels()
    {
        var json = Utils.ToJson(models);
        Utils.SaveFile(path, json);
    }
    #endregion

    #region Encrypt
    public Result GenerateSerialNo(string type, int num)
    {
        var values = new List<string>();
        for (int i = 0; i < num; i++)
        {
            values.Add(Utils.GetGuid());
        }
        return Result.Success("生成成功！", string.Join(Environment.NewLine, values));
    }
    #endregion
}