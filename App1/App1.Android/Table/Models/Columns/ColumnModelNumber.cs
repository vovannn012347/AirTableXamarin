using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Views;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelNumber : ColumnModel
    {
        public ColumnModelNumber() : base()
        {
            controller = new ColumnController(this);
        }

        public ColumnModelNumber(DataSnapshot data) : base(data)
        {
            controller = new ColumnController(this);
        }

        public override ColumnView GetView(Activity context)
        {
            ColumnView cv = new ColumnView(context, controller);

            return cv;
        }
        
        public override CellModel constructCell()
        {
            return new CellModelNumber(this);
        }
    }
}