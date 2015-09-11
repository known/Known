using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace System.Web.UI
{
    public static class GridViewExtension
    {
        public static DataControlField GetColumn(this GridView grid, string header)
        {
            foreach (DataControlField column in grid.Columns)
            {
                if (column.HeaderText == header)
                {
                    return column;
                }
            }
            return grid.Columns[0];
        }

        public static void BindGrid(this GridView grid, object dataSource)
        {
            grid.BindGrid(dataSource, null);
        }

        public static void BindGrid(this GridView grid, object dataSource, string emptyText)
        {
            grid.DataSource = dataSource;
            grid.DataBind();

            if (grid.Rows.Count == 0)
            {
                var table = new Table();
                table.CssClass = grid.CssClass;
                var tr = new TableHeaderRow();
                foreach (DataControlField column in grid.Columns)
                {
                    if (column.Visible)
                    {
                        var th = new TableHeaderCell();
                        th.Width = column.HeaderStyle.Width;
                        th.Text = column.HeaderText;
                        tr.Controls.Add(th);
                    }
                }
                table.Controls.Add(tr);
                if (!string.IsNullOrEmpty(emptyText))
                {
                    var emptyRow = new TableRow();
                    var emptyCell = new TableCell();
                    emptyCell.CssClass = "empty";
                    emptyCell.ColumnSpan = tr.Controls.Count;
                    emptyCell.Text = emptyText;
                    emptyRow.Controls.Add(emptyCell);
                    table.Controls.Add(emptyRow);
                }
                grid.Parent.Controls.Add(table);
            }
        }
    }
}
