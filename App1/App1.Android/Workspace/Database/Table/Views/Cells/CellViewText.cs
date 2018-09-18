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
    class CellViewText : CellView, ITextWatcher
    {
        TextView numberView;
        CellControllerText cell_controller;

        public CellViewText(Activity context, CellControllerText controller)
        {
            numberView = new EditText(context);

            numberView.SetMaxLines(1);
            numberView.SetLines(1);
            numberView.SetSingleLine(true);

            numberView.AddTextChangedListener(this);

            

            cell_controller = controller;
            cell_controller.HookView(this);
        }
        
        public override void SetData(string data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            consume_send = true;
            numberView.Text = data;

        }
        private void UploadChanges()
        {
            string text = numberView.Text.ToString();
            consume_update = true;
            cell_controller.UserSetData(text);
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

        public override void DeleteView()
        {
            cell_controller.UnhookView(this);
        }
        public override View GetView()
        {
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                   (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Left
            };

            numberView.LayoutParameters = p;
            numberView.Background.SetColorFilter(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            numberView.Focusable = false;
            numberView.Clickable = false;
            numberView.FocusableInTouchMode = false;
            numberView.MovementMethod = null;
            numberView.KeyListener = null;
            numberView.SetOnTouchListener(null);


            return numberView;
        }
        public override View GetEditView()
        {
            numberView.Background.ClearColorFilter();
            numberView.Focusable = true;
            numberView.Clickable = true;
            numberView.FocusableInTouchMode = true;

            return numberView;
        }
    }
}
