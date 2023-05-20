namespace Known.Test;

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
}