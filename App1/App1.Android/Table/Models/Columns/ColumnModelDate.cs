using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelDate : ColumnModel
    {
        DateTimeFormatInfo sdf;

        public ColumnModelDate() : base()
        {
            controller = new ColumnController(this);
        }
        
        public ColumnModelDate(DataSnapshot data) : base(data)
        {
            controller = new ColumnController(this);
        }

        public override ColumnView GetView(Activity context)
        {
            ColumnView cv = new ColumnView(context, controller);
            return cv;
        }

        public DateTimeFormatInfo GetFormat()
        {
            if (sdf == null)
            {
                sdf = CultureInfo.CurrentUICulture.DateTimeFormat;
            }
            return sdf;
        }

        public override CellModel constructCell()
        {
            return new CellModelDate(this);
        }
    }
}