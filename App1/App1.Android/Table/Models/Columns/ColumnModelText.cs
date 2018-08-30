using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelText : ColumnModel
    {
        public override CellModel ConstructCell()
        {
            return new CellModelText(this);
        }
    }
}