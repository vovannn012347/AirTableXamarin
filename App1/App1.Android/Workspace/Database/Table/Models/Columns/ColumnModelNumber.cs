using System.Collections.Generic;
using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table;
using App1.Droid.Workspace.Database.Table.Views.Columns;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelNumber : ColumnModel
    {
        public ColumnModelNumber(DataSnapshot data) : base(data)
        {

        }

        public ColumnModelNumber()
        {
        }

        public override CellModel ConstructCell()
        {
            return new CellModelNumber(this);
        }

        public override ColumnView GetEditView(Activity context)
        {
            return new ColumnViewNumber(context, controller);
        }
        public override Dictionary<string, string> Data { get { return null; } set { } }
    }
}