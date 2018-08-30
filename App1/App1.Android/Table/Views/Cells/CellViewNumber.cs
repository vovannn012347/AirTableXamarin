using System;
using Android.App;
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
        Timer timer;
        EditText numberView;
        CellControllerNumber cell_controller;

        public CellViewNumber(Activity context, CellControllerNumber controller)
        {
            numberView = new EditText(context);
            numberView.InputType = InputTypes.ClassNumber |
                    InputTypes.NumberFlagDecimal |
                    InputTypes.NumberFlagSigned;
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
                consume_send = true;
                numberView.Text = "";
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
            return numberView;
        }
 
        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }
        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (timer != null)
                timer.Cancel();
        }
        public void AfterTextChanged(IEditable s)
        {

            if (consume_send)
            {
                consume_send = false;
                return;
            }

            timer = new Timer();
            timer.Schedule(new UploadChangesTimerTask(this), 3000);
        }

        class UploadChangesTimerTask : TimerTask
        {
            CellViewNumber caller;

            public UploadChangesTimerTask(CellViewNumber called_by)
            {
                caller = called_by;
            }

            public override void Run()
            {
                caller.UploadChanges();
            }
        }
    }
}