using System;
using Android.Views;

namespace App1.Droid.Table.Views
{
    public abstract class CellView : Java.Lang.Object
    {
        protected bool consume_send;
        protected bool consume_update;

        public abstract View GetView();
        public abstract View GetEditView();
        public abstract void SetData(String data);
        public abstract void DeleteView();
    }
}