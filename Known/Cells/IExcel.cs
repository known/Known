using System.Collections.Generic;

namespace Known.Cells
{
    public interface IExcel
    {
        IList<ISheet> Sheets { get; }

        ISheet AddSheet(string name);
        void DeleteSheet(string name);
        void Save(string fileName);
        void Save(string fileName, SavedFormat format);
    }
}
