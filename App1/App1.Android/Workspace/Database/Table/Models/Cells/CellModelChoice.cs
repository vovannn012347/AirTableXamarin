using Android.App;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelChoice : CellModel
    {
        string choice;
        CellControllerChoice controller;

        public CellModelChoice(ColumnModelChoice parent) : base(parent)
        {
            controller = new CellControllerChoice(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewChoice(context, controller,
                            ((ColumnModelChoice)parentColumn).GetChoicesAdapter(context));
        }

        public override void ColumnChangeSetData(string data)
        {
            choice = data;
            controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(data));
        }
        public override void SetData(DataSnapshot data)
        {
            choice = data.Value.ToString();
            controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(choice));
        }
        public override void ColumnDeleted()
        {
            choice = "";
            controller.NotifyDataChanged(choice);
        }

        public override Type CellType()
        {
            return Type.Choice;
        }

        public override string Data
        {
            get {
                return ((ColumnModelChoice)parentColumn).IndexOfChoice(choice);
            }
            set {
                choice = value;
                controller.NotifyDataChanged(((ColumnModelChoice)parentColumn).IndexOfChoice(choice));
            }
        }

        public override void Save()
        {
            if (!string.IsNullOrEmpty(choice))
            {
                Row_Ref.Child(parentColumn.ColumnId).SetValue(choice);
            }
        }
    }
}