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

namespace App1.Droid.Table.Views
{
    abstract class CellView : Java.Lang.Object
    {
        protected bool consume_send;
        protected bool consume_update;

        public abstract View GetView();
        public abstract void SetData(String data);
        public abstract void DeleteView();
        public virtual View GetDialogView()
        {
            return GetView();
        }
    }
}