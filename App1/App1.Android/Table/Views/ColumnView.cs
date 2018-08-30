using Android.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;

namespace App1.Droid.Table.Views
{
    class ColumnView
    {
        private ColumnController controller;
        View column_view;

        public ColumnView(Activity context, ColumnController controller)
        {
            column_view = context.LayoutInflater.Inflate(Resource.Layout.column_view, null, false);
            
            this.controller = controller;
            controller.HookView(this);
        }

        internal View GetView()
        {
            return column_view;
        }

        internal void SetName(string name)
        {
            column_view.FindViewById<TextView>(Resource.Id.column_text).Text = name;
        }

        internal void DeleteView()
        {
            controller.UnhookView(this);
        }
    }
}