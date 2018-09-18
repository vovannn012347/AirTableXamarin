using Android.App;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelNumber : CellModel
    {
        string number;
        CellControllerNumber controller;

        public CellModelNumber(ColumnModel parent) : base(parent)
        {
            number = "";
            controller = new CellControllerNumber(this);
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewNumber(context, controller);
        }


        public override void ColumnChangeSetData(string data)
        {
            number = data;
            controller.NotifyDataChanged(number);
        }
        public override void SetData(DataSnapshot data)
        {
            number = data.Value.ToString();
            controller.NotifyDataChanged(number);
        }
        public override void ColumnDeleted()
        {
            number = "";
            controller.NotifyDataChanged(number);
        }

        public override Type CellType()
        {
            return Type.Number;
        }

        public override void Save()
        {
            Row_Ref.Child(parentColumn.ColumnId).SetValue(number);
        }

        public override string Data
        {
            get
            {
                return number;
            }
            set
            {
                if(value!= number)
                {
                    number = value;
                    controller.NotifyDataChanged(number);  
                }
                              
            }
        }
}
}