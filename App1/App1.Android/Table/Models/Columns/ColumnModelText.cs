using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelText : ColumnModel
    {
        
        public ColumnModelText() : base()
        {
            controller = new ColumnController(this);
        }

        public ColumnModelText(DataSnapshot data) : base(data)
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
            return new CellModelText(this);
        }
        
    }
}