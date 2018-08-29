using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;
using Java.Lang;
using Java.Util;

namespace App1.Droid.Table.Views
{
    class TableView : Java.Lang.Object, Android.Views.View.IOnClickListener
    {
        private TableController controller;
        private Activity parentActivity;

        private List<ColumnView> columns;
        private List<TableRowView> rows;

        private View table_view;
        private TableLayout tableView;
        private EditText tableName;
        private ImageButton deleteButton;
        private CheckBox checkBoxView;
        private TableRow column_row_view;

        private bool consume_checked;
        private int checked_amount;

        private bool consume_send_name;
        private bool consume_change_name;

        private NameChangeListener nameListener;
        private OnCheckedListener onCheckedListener;

        const int ADDITIONAL_COLUMNS = 2;
        const int ADDITIONAL_ROWS = 1;

        public TableView(Activity context, TableController controller)
        {

            table_view = context.LayoutInflater.Inflate(Resource.Layout.table_layout, null);

            tableView =        table_view.FindViewById<TableLayout>(Resource.Id.table_data);
            column_row_view =  table_view.FindViewById<TableRow>(Resource.Id.columnRow);

            deleteButton = table_view.FindViewById<ImageButton>(Resource.Id.deleteButton);
            deleteButton.SetOnClickListener(this);

            nameListener = new NameChangeListener(this);
            tableName = table_view.FindViewById<EditText>(Resource.Id.nameEdit);
            tableName.AddTextChangedListener(nameListener);

            onCheckedListener = new OnCheckedListener(this);
            checkBoxView = new CheckBox(context);
            column_row_view.AddView(checkBoxView);
            checkBoxView.SetOnCheckedChangeListener(onCheckedListener);

            View dummy = new View(context);
            column_row_view.AddView(dummy);

            this.controller = controller;
            parentActivity = context;

            columns = new List<ColumnView>();
            rows = new List<TableRowView>();

            controller.HookView(this);
        }

        internal void Initiate(List<ColumnModel> columns, List<RowModel> rows)
        {
            foreach(ColumnModel model in columns)
            {
                ColumnView v = model.GetView(parentActivity);
                this.columns.Add(v);
                column_row_view.AddView(v.GetView());
            }

            foreach (RowModel model in rows)
            {
                TableRowView v = model.GetView(parentActivity);
                this.rows.Add(v);
                tableView.AddView(v.GetView());
            }
        }

        public View GetView()
        {
            return table_view;
        }

        public void DeleteView()
        {
            consume_checked = false;
            Checked(false);
            controller.UnhookView(this);

            foreach(ColumnView v in columns)
            {
                v.DeleteView();
            }

            foreach(TableRowView v in rows)
            {
                v.DeleteView();
            }
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.deleteButton && checked_amount > 0)
            {
                controller.UserDeletedCheckedRows();
            }
            //just to be safe
            checked_amount = 0;
        }

        private void Checked(bool checkedChanged)
        {
            if (consume_checked)
            {
                consume_checked = false;
                return;
            }

            foreach (TableRowView row in rows)
            {
                 row.SetChecked(checkedChanged);
            }
        }

        public void ColumnAdded(ColumnModel model, int index)
        {
            ColumnView v = model.GetView(parentActivity);
            columns.Insert(index, v);
            column_row_view.AddView(v.GetView(), index+ADDITIONAL_COLUMNS);
        }

        public void ColumnDeleted(int index)
        {
            columns[index].DeleteView();
            columns.RemoveAt(index);
            column_row_view.RemoveViewAt(index+ADDITIONAL_COLUMNS);
        }

        public void ColumnReplaced(ColumnModel column, int index)
        {
            columns[index].DeleteView();
            column_row_view.RemoveViewAt(index+ADDITIONAL_COLUMNS);
            columns[index] = column.GetView(parentActivity);
            column_row_view.AddView(columns[index].GetView(), index+ADDITIONAL_COLUMNS);
        }

        public void NewRowAdded(RowModel row)
        {
            TableRowView v = row.GetView(parentActivity);
            rows.Add(v);
            tableView.AddView(v.GetView());
        }

        public void RowAdded(RowModel model,int index)
        {
            TableRowView v = model.GetView(parentActivity);
            rows.Insert(index, v);
            tableView.AddView(v.GetView(), index + ADDITIONAL_ROWS);
        }

        public void RowDeleted(int index)
        {
            rows.ElementAt(index).DeleteView();
            rows.RemoveAt(index);
            tableView.RemoveViewAt(index + ADDITIONAL_ROWS);
        }

        public void RowChecked(bool Checked)
        {
            if (checkBoxView.Checked)
            {
                if (!Checked){
                    consume_checked = true;
                    checkBoxView.Checked = false;
                }
            }
            
            if (Checked && checked_amount == 0)
            {
                deleteButton.SetColorFilter(Android.Graphics.Color.DarkOrange);
            }
            else if(!Checked && checked_amount == 1)
            {
                deleteButton.SetColorFilter(Android.Graphics.Color.Transparent);
            }

            if (Checked){
                ++checked_amount;
            }
            else
            {
                --checked_amount;
            }
            
        }

        public void SetName(string name)
        {
            if (consume_change_name)
            {
                consume_change_name = false;
                return;
            }
            consume_send_name = true;
            tableName.Text = name;
        }

        public void UserChangedName()
        {
            consume_change_name = true;
            controller.UserChangedName(tableName.Text);
        }

        class NameChangeListener : Java.Lang.Object, ITextWatcher {
            
            Timer timer;
            readonly TableView parentTable;

            public NameChangeListener(TableView parent)
            {
                parentTable = parent;
            }
            
            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {

            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                if (timer != null)
                {
                    timer.Cancel();
                }
            }
            
            public void AfterTextChanged(IEditable s)
            {
                if (parentTable.consume_send_name)
                {
                    parentTable.consume_send_name = false;
                    return;
                }

                timer = new Timer();
                timer.Schedule(new UpdatenameTask(this.parentTable), 3000);

            }
        }

        class UpdatenameTask : TimerTask
        {
            TableView updated;

            public UpdatenameTask(TableView updated)
            {
                this.updated = updated;
            }

            public override void Run()
            {
                updated.UserChangedName();
            }
        }

        class OnCheckedListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {

            TableView parentView;
            public OnCheckedListener(TableView parent){
                parentView = parent;
            }

            
            public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
            {
                parentView.Checked(isChecked);
            }
        }
    }
}