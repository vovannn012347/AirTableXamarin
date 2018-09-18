using System;
using System.Drawing;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewChoice : CellView, AdapterView.IOnItemSelectedListener, Spinner.IOnTouchListener
    {
        Spinner view;
        CellControllerChoice controller;

        public CellViewChoice(Activity context, CellControllerChoice controller, ArrayAdapter<String> adapter)
        {
            view = new Spinner(context)
            {
                Adapter = adapter
            };

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
            controller.UserSetData("0");
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
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.CenterHorizontal
            };

            GradientDrawable d = new GradientDrawable();
            d.SetColor(Android.Graphics.Color.Transparent);
            d.SetStroke(2, Android.Graphics.Color.Black);
            d.SetCornerRadius(10);

            view.Background = d;
           
            view.OnItemSelectedListener = null;
            view.Clickable = false;
            view.Focusable = false;
            view.FocusableInTouchMode = false;

            view.LayoutParameters = p;

            return view;
        }

        public override View GetEditView()
        {
            GradientDrawable d = new GradientDrawable();
            d.SetColor(Android.Graphics.Color.Transparent);
            d.SetStroke(2, Android.Graphics.Color.Black);
            d.SetCornerRadius(1);

            view.Background = d;

            view.OnItemSelectedListener = this;
            view.Clickable = true;
            view.Focusable = true;

            return view;
        }
        //diallow spinner dropdown
        public bool OnTouch(View v, MotionEvent e)
        {
            return true;
        }
        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
            //this.Dispose();
        }
    }
}