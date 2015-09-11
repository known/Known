using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.SLite
{
    public class Link : Entity<Link>
    {
        public Link()
        {
            IsShow = true;
            DisplayOrder = 1000;
            Position = 1;
        }

        public int Position { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsShow { get; set; }
        public bool IsNewWindow { get; set; }

        public override List<string> Validate()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(Name))
                list.Add("链接名称不能为空！");
            if (string.IsNullOrEmpty(Url))
                list.Add("链接地址不能为空！");
            return list;
        }

        public static List<Link> GetNavigationLinks()
        {
            return Entity.FindByField<Link>("\"Position\":0")
                .Where(l => l.IsShow)
                .OrderBy(l => l.DisplayOrder)
                .ToList();
        }

        public static List<Link> GetLinks()
        {
            return Entity.FindByField<Link>("\"Position\":1")
                .Where(l => l.IsShow)
                .OrderBy(l => l.DisplayOrder)
                .ToList();
        }
    }
}
