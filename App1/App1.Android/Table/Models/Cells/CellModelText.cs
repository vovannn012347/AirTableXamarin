using System;
using Android.App;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelText : CellModel
    {
        String text;
        CellControllerText controller;

        public CellModelText(ColumnModelText parent)
        {
            parentColumn = parent;
            controller = new CellControllerText(this);
        }

        public CellModelText(ColumnModel parent)
        {
            parentColumn = parent;
            controller = new CellControllerText(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewText(context, controller);
        }

        public override void SetData(DataSnapshot data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            text = data.Value.ToString();
            controller.NotifyDataChanged(text);
        }

        public override void ColumnChangeSetData(string data)
        {
            text = data;
            controller.NotifyDataChanged(text);
        }

        public override String Data
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                controller.NotifyDataChanged(text);
                consume_update = true;
                if (!System.String.IsNullOrEmpty(value))
                {
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
            }
        }

        public override void EraseData()
        {
            text = "";
            controller.NotifyDataChanged(text);
        }
    }
}