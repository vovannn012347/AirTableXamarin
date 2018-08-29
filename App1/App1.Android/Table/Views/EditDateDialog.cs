using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Views.Cells;

namespace App1.Droid.Table.Views
{
    class EditDateDialog : DialogFragment, Android.Views.View.IOnClickListener, Android.Widget.DatePicker.IOnDateChangedListener, ICellViewDate
    {
        LinearLayout mainView;
        Button deleteButton;
        DatePicker picker;

        Activity context;
        CellControllerDate controller;
        DateTimeFormatInfo format;
        bool consume_update;
      

        public EditDateDialog(Activity context, CellControllerDate controller)
        {
            this.context = context;
            this.controller = controller;
            consume_update = false;

            mainView = new LinearLayout(context);
            mainView.Orientation = Orientation.Vertical;

            deleteButton = new Button(context);
            deleteButton.SetText(Resource.String.delete_button_txt);
            deleteButton.SetBackgroundColor(Android.Graphics.Color.Red);
            mainView.AddView(deleteButton);
            
            picker = new DatePicker(context);
            mainView.AddView(picker);

            controller.HookView(this);

            deleteButton.SetOnClickListener(this);
            picker.Init(1, 1, 1, this);
        }

        public void DeleteView()
        {
            controller.UnhookView(this);
        }

        public override void Dismiss()
        {
            DeleteView();
            base.Dismiss();
        }

        public void OnClick(View v)
        {
            controller.UserSetData(null);
            Dismiss();
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetView(mainView);

            return builder.Create();
        }

        public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            consume_update = true;
            controller.UserSetData(view.DateTime.ToString(format.ShortDatePattern));
        }

        public void SetData(string data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }
            DateTime time;
            if(DateTime.TryParse(data, format, DateTimeStyles.None, out time))
            {
                picker.DateTime = time;
            }
        }

        public void SetDateFormat(DateTimeFormatInfo format)
        {
            this.format = format;
        }
    }
}