using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    abstract class CellModel : Java.Lang.Object
    {
        protected ColumnModel parentColumn;
        protected DatabaseReference Row_Ref;
        protected bool consume_update;
        protected bool consume_send;

        public CellModel() { consume_update = false; consume_send = false; }
        public CellModel(ColumnModel parent) : this()
        {
            parentColumn = parent;
        }

        public abstract CellView GetView(Activity context);
        public abstract void SetData(DataSnapshot data);
        public abstract void ColumnChangeSetData(string data);
        public abstract void EraseData();

        public abstract string Data { get; set; }
        public ColumnModel ParentColumn
        {
            get
            {
                return parentColumn; 
            }
            set
            {
                parentColumn = value;
            }
        }
        public DatabaseReference RowReference {
            get {
                return Row_Ref;
            }
            set
            {
                Row_Ref = value;
            }
        }

    }
}