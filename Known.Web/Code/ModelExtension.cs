namespace Known.Web
{
    public static class ModelExtension
    {
        public static object ToTree(this MenuInfo menu)
        {
            return new
            {
                id = menu.Id,
                pid = menu.ParentId,
                type = menu.Type,
                code = menu.Code,
                name = menu.Name,
                title = menu.Name,
                icon = menu.Icon,
                url = menu.Url,
                target = menu.Target,
                @checked = menu.Checked,
            };
        }
    }
}