using System;

using Android.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewChoice : CellView, AdapterView.IOnItemSelectedListener
    {
        Spinner view;
        CellControllerChoice controller;

        public CellViewChoice(Activity context, CellControllerChoice controller, ArrayAdapter<String> adapter)
        {
            view = new Spinner(context);
            view.Adapter = adapter;
            view.OnItemSelectedListener = this;

            this.controller = controller;
            controller.HookView(this);

        }
        
        public void OnItemSelected(AdapterView parentView, View selectedItemView, int position, long id)

        {
            if (consume_send)
            {
                consume_send = false;
                return;
            }
            consume_update = true;
            controller.UserSetData("" + position);
        }
        public void OnNothingSelected(AdapterView parentView)
        {
        }

        public override void SetData(String data)
        {
            //not used
        }
        public void SetSelection(int index)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }
            consume_send = true;
            view.SetSelection(index);
        }
        public override View GetView()
        {
            return view;
        }
        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
            this.Dispose();
        }
}
}