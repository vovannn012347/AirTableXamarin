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
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelNumber : CellModel
    {
        String number;
        CellControllerNumber controller;

        public CellModelNumber(ColumnModel parent)
        {
            parentColumn = parent;
            controller = new CellControllerNumber(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewNumber(context, controller);
        }

        public override void SetData(DataSnapshot data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            number = data.Value.ToString();
            controller.NotifyDataChanged(number);
        }

        public override void ColumnChangeSetData(string data)
        {
            number = data;
            controller.NotifyDataChanged(number);
        }

        public override String Data
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                controller.NotifyDataChanged(number);
                consume_update = true;
                if (!System.String.IsNullOrEmpty(value))
                {
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
            }
        }

        public override void EraseData()
        {
            number = "";
            controller.NotifyDataChanged(number);
        }
    }
}