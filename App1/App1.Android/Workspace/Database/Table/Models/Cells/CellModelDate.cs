using System;
using System.Globalization;

using Android.App;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelDate : CellModel
    {
        string date;
        CellControllerDate controller;

        public CellModelDate(ColumnModel parent) : base(parent)
        {
            controller = new CellControllerDate(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewDate(context, controller);
        }

        public DateTimeFormatInfo GetFormat()
        {
            return ((ColumnModelDate)parentColumn).GetFormat();
        }

        public override Type CellType()
        {
            return Type.Date;
        }

        public override void ColumnChangeSetData(string data)
        {
            date = data;
            controller.NotifyDataChanged(date);
        }
        public override void SetData(DataSnapshot data)
        {
            /*
            if (consume_update)
            {
                consume_update = false;
                return;
            }
            */
            date = data.Value.ToString();
            controller.NotifyDataChanged(date);
        }

        public override void ColumnDeleted()
        {
            date = "";
            controller.NotifyDataChanged(date);
        }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(date))
            {
                Row_Ref.Child(parentColumn.ColumnId).SetValue(date);
            }
            else
            {
                Row_Ref.Child(parentColumn.ColumnId).RemoveValue();
            }
        }

        public override string Data
        {
            get
            {
                return date;
            }
            set
            {
                if(date != value)
                {
                    date = value;
                    controller.NotifyDataChanged(date);
                }
            }
        }
        

    }
}