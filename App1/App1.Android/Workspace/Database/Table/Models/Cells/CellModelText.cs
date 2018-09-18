using Android.App;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelText : CellModel
    {
        string text;
        CellControllerText controller;

        public CellModelText(ColumnModelText parent):base(parent)
        {
            text = "";
            controller = new CellControllerText(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewText(context, controller);
        }

        public override Type CellType()
        {
            return Type.Text;
        }

        public override void ColumnChangeSetData(string data)
        {
            text = data;
            controller.NotifyDataChanged(text);
        }
        public override void SetData(DataSnapshot data)
        {
            text = data.Value.ToString();
            controller.NotifyDataChanged(text);
        }
        public override void ColumnDeleted()
        {
            text = "";
            controller.NotifyDataChanged(text);
        }

        public override void Save()
        {
            Row_Ref.Child(parentColumn.ColumnId).SetValue(text);
        }

        public override string Data
        {
            get
            {
                return text;
            }
            set
            {
                if(text != value)
                {
                    text = value;
                    controller.NotifyDataChanged(text);
                }
                
            }
        }
    }
}