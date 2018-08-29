using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

        public CellModelDate(ColumnModel parent)
        {
            parentColumn = parent;
            controller = new CellControllerDate(this);
            consume_update = false;
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewDate(context, controller);
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

        public override void ColumnChangeSetData(string data)
        {
            date = data;
            controller.NotifyDataChanged(date);
        }

        public DateTimeFormatInfo GetFormat()
        {
            return ((ColumnModelDate)parentColumn).GetFormat();
        }

        public override String Data
        {
            get
            {
                return this.date;
            }
            set
            {
                date = value;
                controller.NotifyDataChanged(date);
                consume_update = true;
                if (!System.String.IsNullOrEmpty(value))
                {
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
                else
                {
                    Row_Ref.Child(parentColumn.ColumnId).RemoveValue();
                }
            }
        }
        
        public override void EraseData()
        {
            date = "";
            controller.NotifyDataChanged(date);
        }


    }
}