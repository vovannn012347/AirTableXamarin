using System.Collections.Generic;
using System.Globalization;
using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table;
using App1.Droid.Workspace.Database.Table.Views.Columns;
using Firebase.Database;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelDate : ColumnModel
    {
        DateTimeFormatInfo sdf;

        public ColumnModelDate(DataSnapshot data) : base(data)
        {
            this.Data = this.data;
        }

        public ColumnModelDate() : base()
        {
        }

        public DateTimeFormatInfo GetFormat()
        {
            if (sdf == null)
            {
                sdf = CultureInfo.CurrentUICulture.DateTimeFormat;
                data.Clear();
                data.Add("format", sdf.ShortDatePattern);
            }
            return sdf;
        }
        public override CellModel ConstructCell()
        {
            return new CellModelDate(this);
        }

        public override ColumnView GetEditView(Activity context)
        {
            return new ColumnViewDate(context, controller);
        }

        public override Dictionary<string, string> Data {
            get {
                if (sdf == null)
                {
                    sdf = CultureInfo.CurrentUICulture.DateTimeFormat;
                }
                return new Dictionary<string, string>() { { "format", sdf.ShortDatePattern } };
            }
            set {
                if(value!= null)
                if (value.ContainsKey("format"))
                {
                    sdf.FullDateTimePattern = value.GetValueOrDefault("format");
                    data["format"] = value.GetValueOrDefault("format");
                }
            }
        }
    }
}