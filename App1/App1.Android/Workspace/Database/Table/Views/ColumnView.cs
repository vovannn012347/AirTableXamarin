using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Workspace.Database.Table.Dialogs;
using Java.Lang;

namespace App1.Droid.Table.Views
{
    public class ColumnView : Java.Lang.Object, View.IOnClickListener
    {
        protected ColumnController controller;
        protected TextView column_view;
        protected NameChangeListener nameListener;
        protected Activity context;

        bool consume_name_update;
        bool consume_name_send;

        public ColumnView()
        {
            consume_name_update = false;
            consume_name_send = false;
        }

        public ColumnView(Activity context, ColumnController controller) : this()
        {
            this.context = context;
            this.controller = controller;

            column_view = new EditText(context);
           
            controller.HookView(this);
        }

        public virtual View GetView()
        {
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.Left
            };

            column_view.LayoutParameters = p;
            column_view.SetTextColor(Color.Gray);
            column_view.Background.SetColorFilter(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            column_view.Focusable = false;
            column_view.Clickable = false;
            column_view.FocusableInTouchMode = false;
            column_view.MovementMethod = null;
            column_view.KeyListener = null;
            column_view.SetPadding(0, 0, 0, 0);

            return column_view;
        }

        public virtual void DataChanged(Dictionary<string, string> data)
        {
        }

        internal void SetName(string name)
        {
            if (consume_name_update)
            {
                consume_name_update = false;
                return;
            }
            consume_name_send = true;
            column_view.Text = name;
        }

        private void UpdateName()
        {
            consume_name_update = true;
            controller.NameChanged(column_view.Text);
        }

        internal void DeleteView()
        {
            controller.UnhookView(this);
        }

        protected void CallTypeChangeDialog()
        {
            if (context.FragmentManager.FindFragmentByTag("TypeChangeDialog") != null)
            {
                return;
            }

            TypeChangeDialog dialog
                = new TypeChangeDialog(context, controller);

            dialog.Show(context.FragmentManager, "TypeChangeDialog");
        }

        public virtual void OnClick(View v)
        {
            CallTypeChangeDialog();
        }

        protected class NameChangeListener : Java.Lang.Object, ITextWatcher
        {
            ColumnView parent;
            
            public NameChangeListener(ColumnView parent)
            {
                this.parent = parent;
            }

            public void AfterTextChanged(IEditable s)
            {
                if (parent.consume_name_send)
                {
                    parent.consume_name_send = false;
                    return;
                }

                parent.UpdateName();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
            }
        }
    }
}