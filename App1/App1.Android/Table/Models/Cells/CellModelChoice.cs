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
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelChoice : CellModel
    {
        String choice;
        CellControllerChoice controller;

        public CellModelChoice(ColumnModelChoice parent)
        {
            parentColumn = parent;
            controller = new CellControllerChoice(this);
        }

        public override CellView GetView(Activity context)
        {
            CellViewChoice ret =
                    new CellViewChoice(context, controller,
                            ((ColumnModelChoice)parentColumn).getChoicesAdapter(context));
            return ret;
        }

        public override string Data
        {
            get {
                return ((ColumnModelChoice)parentColumn).IndexOfChoice(choice);
            }
            set {
                choice = value;
                controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(choice));
                
                if (!System.String.IsNullOrEmpty(value))
                {
                    consume_update = true;
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
            }
        }

        public override void SetData(DataSnapshot data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            choice = data.Value.ToString();
            controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(choice));
        }

        public override void EraseData()
        {
            choice = "";
            controller.NotifyDataChanged(choice);
        }

        public override void ColumnChangeSetData(string data)
        {
            choice = data;
            controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(data));
        }
    }
}