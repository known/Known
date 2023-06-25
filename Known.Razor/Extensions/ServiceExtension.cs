namespace Known.Razor.Extensions;

static class ServiceExtension
{
    internal static void ShowImport(this UIService ui, ImportOption option)
    {
        ui.Show<Importer>($"导入{option.Name}", new Size(450, 220), action: attr => attr.Set(c => c.Option, option));
    }
}