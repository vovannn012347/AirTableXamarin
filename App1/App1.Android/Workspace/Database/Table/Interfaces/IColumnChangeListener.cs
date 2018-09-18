using App1.Droid.Table.Models;

namespace App1.Droid.Workspace.Database.Table.Interfaces
{
    public interface IColumnChangeListener
    {
        void ColumnChangedType(ColumnModel model, string type);
    }
}