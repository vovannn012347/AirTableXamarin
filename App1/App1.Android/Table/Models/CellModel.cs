using System;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    abstract class CellModel : Java.Lang.Object
    {
        protected ColumnModel parentColumn;
        private CellController controller;
        protected DatabaseReference Row_Ref;
        // to be used with Data.set
        protected bool consume_update;

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

        public abstract String Data { get; set; }
        public DatabaseReference RowReference {
            get {
                return Row_Ref;
            }
            set
            {
                Row_Ref = value;
            }
        }

        public abstract CellView GetView(Activity context);
        public abstract void SetData(DataSnapshot data);
        public abstract void ColumnChangeSetData(string data);
        public abstract void EraseData();

    }
}