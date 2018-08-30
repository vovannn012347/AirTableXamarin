using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;

namespace App1.Droid.Table.Views
{
    class EditRowDialog : DialogFragment, IRowView, Android.Views.View.IOnClickListener
    {
        private readonly Activity context;
        private RowController controller;

        private ScrollView mainView;
        private LinearLayout layout;
        private Button deleteButton;
        readonly List<CellView> cells;
        readonly List<ColumnView> columns;

        const int ADDITIONAL_ROWS = 1;

        public EditRowDialog(Activity context, RowController controller)
        {
            this.context = context;
            this.controller = controller;

            mainView = new ScrollView(context);

            layout = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };
            mainView.AddView(layout);

            deleteButton = new Button(context);
            deleteButton.SetText(Resource.String.delete_button_txt);
            deleteButton.SetBackgroundColor(Android.Graphics.Color.Red);
            deleteButton.SetOnClickListener(this);
            layout.AddView(deleteButton);

            cells = new List<CellView>();
            columns = new List<ColumnView>();

            this.controller = controller;
            controller.HookView(this);
        }

        public void Initiate(List<CellModel> cells, List<ColumnModel> columns)
        {
            for(int i = 0; i < cells.Count; ++i)
            {
                ColumnView Cmnv = columns[i].GetView(context);
                this.columns.Add(Cmnv);
                layout.AddView(Cmnv.GetView());

                CellView Cllv = cells[i].GetView(context);
                this.cells.Add(Cllv);
                layout.AddView(Cllv.GetDialogView());
            }
        }
        new public void Dispose()
        {
            base.Dispose();
        }

        public void OnClick(View v)
        {
            controller.UserDeletedRow();
            this.Dismiss();
        }
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AlertDialog.Builder builder = new AlertDialog.Builder(context);

            builder.SetView(mainView);

            return builder.Create();
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            DeleteView();
            base.OnDismiss(dialog);
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            //not working here
        }
        public void SetChecked(bool check)
        {
            // not working here
        }
        public void UserChecked(bool check)
        {
            //not working here
        }
  
        public void ColumnAdded(int index, CellModel cellModel, ColumnModel column)
        {
            CellView Cllv = cellModel.GetView(context);
            this.cells.Add(Cllv);
            layout.AddView(Cllv.GetDialogView(), index*2 + ADDITIONAL_ROWS);

            ColumnView Cmnv = column.GetView(context);
            this.columns.Add(Cmnv);
            layout.AddView(Cmnv.GetView(), index * 2 + ADDITIONAL_ROWS);
        }
        public void ColumnDeleted(int index)
        {
            cells[index].DeleteView();
            columns[index].DeleteView();
            layout.RemoveViewAt(index * 2 + ADDITIONAL_ROWS);
            layout.RemoveViewAt(index * 2 + ADDITIONAL_ROWS);
        }
        public void ColumnChanged(int index, CellModel cellModel, ColumnModel column)
        {
            ColumnDeleted(index);
            ColumnAdded(index, cellModel, column);
        }
        
        public View GetView()
        {
            return null;
        }
        public void DeleteView()
        {
            foreach(CellView v in cells)
            {
                v.DeleteView();
            }
            foreach (ColumnView v in columns)
            {
                v.DeleteView();
            }
            controller.UnhookView(this);
        }
        


    }
}