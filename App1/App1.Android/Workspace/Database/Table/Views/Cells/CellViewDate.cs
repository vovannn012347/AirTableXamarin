using System;
using System.Globalization;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewDate : CellView, ICellViewDate, View.IOnClickListener
    {
        TextView dateText;
        Activity context;
        CellControllerDate controller;
        DateTimeFormatInfo DateFormat;

        public CellViewDate(Activity context, CellControllerDate controller)
        {
            this.context = context;
            this.controller = controller;

            dateText = new EditText(context)
            {
                InputType = InputTypes.ClassDatetime
            };

            dateText.SetMaxLines(1);
            dateText.SetLines(1);
            dateText.SetSingleLine(true);


            int padding = context.Resources.GetDimensionPixelSize(Resource.Dimension.dateView_padding);
            
            controller.HookView(this);
        }
        
        public void UploadChanges()
        {
            string text = ((EditText)dateText).Text.ToString();

            consume_update = true;
            controller.UserSetData(text);
        }
        public void OnClick(View v)
        {
            if (context.FragmentManager.FindFragmentByTag("date_dialog") != null)
            {
                return;
            }
            EditDateDialog dialog = new EditDateDialog(context, controller);
            dialog.Show(context.FragmentManager, "date_dialog");
        }

        public void SetDateFormat(System.Globalization.DateTimeFormatInfo format)
        {
            DateFormat = format;
            dateText.Hint = format.ShortDatePattern;
        }
        public override void SetData(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                dateText.Text = DateFormat.ShortDatePattern;
            }
            else if (long.TryParse(data, out long result))
            {
                dateText.Text = DateTime.FromFileTimeUtc(result).ToString(DateFormat.ShortDatePattern);
            }
            else
            {
                dateText.Text = data;
            }
           
        }

        public override View GetView()
        {
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.CenterHorizontal
            };
            dateText.LayoutParameters = p;

            GradientDrawable d = new GradientDrawable();
            d.SetColorFilter(Color.LightGray, PorterDuff.Mode.Src);
            d.SetCornerRadius(10);

            dateText.Background = d;

            dateText.Clickable = false;
            dateText.Focusable = false;
            dateText.FocusableInTouchMode = false;
            dateText.MovementMethod = null;
            dateText.KeyListener = null;
            if (dateText.HasOnClickListeners)
            {
                dateText.SetOnClickListener(null);
            }

            return dateText;
        }

        public override View GetEditView()
        {
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            dateText.LayoutParameters = p;
            dateText.Clickable = true;
            dateText.Focusable = false;
            dateText.FocusableInTouchMode = false;
            dateText.MovementMethod = null;
            dateText.KeyListener = null;
            dateText.SetOnClickListener(this);
            return dateText;
        }

        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
        }

        public void Init(DateTimeFormatInfo format, string data)
        {
            DateFormat = format;
            dateText.Hint = format.ShortDatePattern;

            if (String.IsNullOrWhiteSpace(data))
            {
                dateText.Text = DateFormat.ShortDatePattern;
            }
            else if(long.TryParse(data, out long result))
            {
                dateText.Text = DateTime.FromFileTimeUtc(result).ToString(DateFormat);
            }
            else
            {
                dateText.Text = DateFormat.ShortDatePattern;
            }
        }
    }
}     