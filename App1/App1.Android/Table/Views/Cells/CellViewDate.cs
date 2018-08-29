using System;
using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using Java.Util;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewDate : CellView, ICellViewDate, View.IOnClickListener
    {
        TextView dateText;
        Activity context;
        //private Timer timer;
        CellControllerDate controller;
        System.Globalization.DateTimeFormatInfo DateFormat;

        public CellViewDate(Activity context, CellControllerDate controller)
        {
            this.context = context;
            this.controller = controller;
            dateText = new TextView(context);
            dateText.InputType = InputTypes.ClassDatetime;
            dateText.SetOnClickListener(this);
            
            controller.HookView(this);
        }

        public void SetDateFormat(System.Globalization.DateTimeFormatInfo format)
        {
            DateFormat = format;
            dateText.Hint = format.ShortDatePattern;
        }

        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
        }

        public override View GetView()
        {
            return dateText;
        }

        public override void SetData(String data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            if (String.IsNullOrWhiteSpace(data))
            {
                dateText.Text = DateFormat.ShortDatePattern;
            }
            else
            {
                dateText.Text = data;
            }
           
        }
        
        public  void UploadChanges()
        {
            string text = ((EditText)dateText).Text.ToString();

            consume_update = true;
            controller.UserSetData(text);
        }
        
        public void OnClick(View v)
        {
            EditDateDialog dialog = new EditDateDialog(context, controller);
            dialog.Show(context.FragmentManager, "date_dialog");
        }

    }
}     