using System.Globalization;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelDate : ColumnModel
    {
        DateTimeFormatInfo sdf;

        public DateTimeFormatInfo GetFormat()
        {
            if (sdf == null)
            {
                sdf = CultureInfo.CurrentUICulture.DateTimeFormat;
            }
            return sdf;
        }
        public override CellModel ConstructCell()
        {
            return new CellModelDate(this);
        }
    }
}