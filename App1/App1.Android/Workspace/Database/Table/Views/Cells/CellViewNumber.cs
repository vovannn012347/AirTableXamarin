using System;
using Android.App;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using Java.Lang;
using Java.Util;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewNumber : CellView, ITextWatcher
    {
        TextView numberView;
        CellControllerNumber cell_controller;

        public CellViewNumber(Activity context, CellControllerNumber controller)
        {
            numberView = new EditText(context)
            {
                InputType = InputTypes.ClassNumber |
                    InputTypes.NumberFlagDecimal |
                    InputTypes.NumberFlagSigned
            };
            numberView.SetMaxLines(1);
            numberView.SetLines(1);
            numberView.SetSingleLine(true);
            numberView.AddTextChangedListener(this);

            cell_controller = controller;

            

            controller.HookView(this);
        }
        
        public override void SetData(string data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            try
            {
                consume_send = true;
                numberView.Text = data;
            }
            catch (Java.Lang.NumberFormatException e)
            {
                Console.WriteLine("System number " + e.Message);
                consume_send = false;
            }
        }
        private void UploadChanges()
        {
            string text = numberView.Text.ToString();
            consume_update = true;
            cell_controller.UserSetData(text);
        }

        public override void DeleteView()
        {
            cell_controller.UnhookView(this);
        }
        public override View GetView()
        {
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.CenterHorizontal
            };

            numberView.SetTypeface(null, TypefaceStyle.Italic);

            numberView.LayoutParameters = p;
            numberView.Background.SetColorFilter(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            numberView.Focusable = false;
            numberView.Clickable = false;
            numberView.FocusableInTouchMode = false;
            numberView.MovementMethod = null;
            numberView.KeyListener = null;

            return numberView;
        }

        public override View GetEditView()
        {
            //bump
            numberView.Background.ClearColorFilter();
            numberView.Focusable = true;
            numberView.Clickable = true;
            numberView.FocusableInTouchMode = true;

            return numberView;
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }
        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }
        public void AfterTextChanged(IEditable s)
        {

            if (consume_send)
            {
                consume_send = false;
                return;
            }

            UploadChanges();
        }
    }
}