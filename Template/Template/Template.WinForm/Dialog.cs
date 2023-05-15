using System.Drawing.Printing;

namespace Template.WinForm;

class Dialog
{
    internal static DialogResult Show(string text)
    {
        return MessageBox.Show(text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    internal static DialogResult Confirm(string text)
    {
        return MessageBox.Show(text, "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
    }

    internal static void Print(string content, bool landscape = false)
    {
        var print = new PrintDocument();
        print.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        print.DefaultPageSettings.Landscape = landscape;
        print.PrintPage += (o, e) =>
        {
            var img = new Bitmap(e.PageBounds.Width, e.PageBounds.Height);
            var br = new WebBrowser();
            br.ScrollBarsEnabled = false;
            br.Width = e.PageBounds.Width;
            br.Height = e.PageBounds.Height;
            br.DocumentText = content;
            br.Document.Write(content);
            br.DrawToBitmap(img, e.PageBounds);
            e.Graphics?.DrawImage(img, e.PageBounds);
        };
        print.Print();
    }
}