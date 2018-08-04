using System.Collections.Generic;
using System.Linq;

namespace Known.Platform.Business
{
    public class UserBusiness : PlatformBusiness
    {
        public UserBusiness(Context context) : base(context)
        {
        }

        public List<Module> GetUserModules()
        {
            var sql = "select * from t_plt_modules";
            var modules = Context.Database.QueryList<Module>(sql);
            return GetHierarchicalModules(modules);
        }

        private List<Module> GetHierarchicalModules(List<Module> source)
        {
            var modules = new List<Module>();
            var topModules = source.Where(m => m.ParentId == "0")
                                   .OrderBy(m => m.Sort)
                                   .ToList();
            foreach (var item in topModules)
            {
                modules.Add(item);
                SetModuleChildren(source, modules, item);
            }

            return modules;
        }

        private void SetModuleChildren(List<Module> source, List<Module> modules, Module module)
        {
            var children = source.Where(m => m.ParentId == module.Id)
                                 .OrderBy(m => m.Sort)
                                 .ToList();
            if (children == null || children.Count == 0)
                return;

            if (module.Children == null)
                module.Children = new List<Module>();

            foreach (var item in children)
            {
                module.Children.Add(item);
                SetModuleChildren(source, modules, item);
            }
        }
    }
}
