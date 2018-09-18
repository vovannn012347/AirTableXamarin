using Android.App;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    public abstract class CellModel : Java.Lang.Object
    {
        public enum Type
        {
            Text, Number, Choice, Date, Image 
        }


        protected ColumnModel parentColumn;
        protected DatabaseReference Row_Ref;
        
        public CellModel() {  }
        public CellModel(ColumnModel parent) : this()
        {
            parentColumn = parent;
        }

        public abstract CellView GetView(Activity context);
        public abstract void SetData(DataSnapshot data);
        public abstract void ColumnChangeSetData(string data);
        public abstract void ColumnDeleted();
        public abstract void Save();
      

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

        public abstract Type CellType();
    }
}