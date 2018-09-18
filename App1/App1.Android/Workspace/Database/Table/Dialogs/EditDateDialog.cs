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
    class EditDateDialog : DialogFragment, Android.Widget.DatePicker.IOnDateChangedListener, ICellViewDate, IDialogInterfaceOnClickListener
    {
        LinearLayout mainView;
        DatePicker picker;

        Activity context;
        CellControllerDate controller;
        DateTimeFormatInfo format;
 
        public EditDateDialog(Activity context, CellControllerDate controller)
        {
            this.context = context;
            this.controller = controller;

            mainView = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };
                        
            picker = new DatePicker(context);
            mainView.AddView(picker);

            controller.HookView(this);            
        }

        public override void Dismiss()
        {
            DeleteView();
            base.Dismiss();
        }
        public void DeleteView()
        {
            controller.UnhookView(this);
        }


        public void OnClick(IDialogInterface dialog, int which)
        {
            switch((DialogButtonType)which)
            {
                case DialogButtonType.Positive:
                    controller.UserSetData(picker.DateTime.ToFileTimeUtc().ToString());
                    break;
                case DialogButtonType.Neutral:
                    controller.UserSetData(null);
                    break;
                default:
                    dialog.Dismiss();
                break;

            }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);

            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetView(mainView);
            builder.SetNegativeButton("Cancel", this);
            builder.SetPositiveButton("Ok", this);
            builder.SetNeutralButton("Delete", this);

            return builder.Create();
        }
        public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            //
        }

        public void OnClick(View v)
        {
            controller.UserSetData(null);
            Dismiss();
        }

        public void SetDateFormat(DateTimeFormatInfo format)
        {
            this.format = format;
        }
        public void SetData(string data)
        {
            DateTime time;
            if(DateTime.TryParse(data, format, DateTimeStyles.None, out time))
            {
                picker.DateTime = time;
            }
        }

        public void Init(DateTimeFormatInfo format, string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                picker.Init(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this);
            }
            else if (long.TryParse(data, out long result))
            {
                picker.DateTime = DateTime.FromFileTimeUtc(result);
            }
            else
            {
                picker.Init(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this);
            }
        }
    }
}