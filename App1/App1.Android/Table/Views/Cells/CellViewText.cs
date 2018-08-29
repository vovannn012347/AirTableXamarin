using Android.App;
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
        Timer timer;
        EditText numberView;
        CellControllerText cell_controller;

        public CellViewText(Activity context, CellControllerText controller)
        {
            numberView = new EditText(context);
            numberView.AddTextChangedListener(this);

            cell_controller = controller;
            cell_controller.HookView(this);
        }


        public override void DeleteView()
        {
            cell_controller.UnhookView(this);
        }

        public override View GetView()
        {
            return numberView;
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

        //uhh, so microsoft imported a class and did not supply base class features dependent on language
        class UploadChangesTimerTask : TimerTask
        {
            CellViewText caller;

            public UploadChangesTimerTask(CellViewText called_by)
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
