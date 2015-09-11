using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Known.Models;

namespace Known.Services
{
    public interface IMenuService
    {
        List<MenuInfo> GetMenus();
    }

    public class MenuService : IMenuService
    {
        public List<MenuInfo> GetMenus()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Menus where IsVisible=1 order by Sequence";
            var list = command.ToList(r =>
            {
                return new MenuInfo
                {
                    Id = r.Get<string>("Id"),
                    Parent = r.Get<string>("Parent"),
                    Code = r.Get<string>("Code"),
                    Name = r.Get<string>("Name"),
                    Icon = r.Get<string>("Icon"),
                    Url = r.Get<string>("Url"),
                    Sequence = r.Get<string>("Sequence"),
                    IsVisible = true
                };
            });
            return list;
        }
    }
}
