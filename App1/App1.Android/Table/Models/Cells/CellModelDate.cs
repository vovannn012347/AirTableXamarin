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
            consume_update = false;
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewDate(context, controller);
        }
        public DateTimeFormatInfo GetFormat()
        {
            return ((ColumnModelDate)parentColumn).GetFormat();
        }

        public override void ColumnChangeSetData(string data)
        {
            date = data;
            controller.NotifyDataChanged(date);
        }
        public override void SetData(DataSnapshot data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            date = data.Value.ToString();
            controller.NotifyDataChanged(date);
        }
        public override void EraseData()
        {
            date = "";
            controller.NotifyDataChanged(date);
        }

        public override string Data
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                controller.NotifyDataChanged(date);
                consume_update = true;
                if (!string.IsNullOrEmpty(value))
                {
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
                else
                {
                    Row_Ref.Child(parentColumn.ColumnId).RemoveValue();
                }
            }
        }
        

    }
}