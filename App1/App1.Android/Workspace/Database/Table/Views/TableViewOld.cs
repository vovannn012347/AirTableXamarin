using System;
using System.Collections.Generic;

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
    public class TableViewOld : Java.Lang.Object, Android.Views.View.IOnClickListener
    {
        private TableController controller;
        private readonly Activity parentActivity;

        private List<ColumnView> columns;
        private List<TableRowView> rows;

        private readonly NameChangeListener nameListener;
        private readonly OnCheckedListener onCheckedListener;

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

        const int ADDITIONAL_COLUMNS = 2;
        const int ADDITIONAL_ROWS = 1;

        public TableViewOld()
        {
            columns = new List<ColumnView>();
            rows = new List<TableRowView>();
        }

        public TableViewOld(Activity context, TableController controller) : this()
        {
            this.controller = controller;
            this.parentActivity = context;

            table_view = context.LayoutInflater.Inflate(Resource.Layout.TableLayoutOld, null);

            tableView =        table_view.FindViewById<TableLayout>(Resource.Id.table_data);
            column_row_view =  table_view.FindViewById<TableRow>(Resource.Id.columnRow);

            deleteButton = table_view.FindViewById<ImageButton>(Resource.Id.deleteButton);
            deleteButton.SetOnClickListener(this);

            tableName = table_view.FindViewById<EditText>(Resource.Id.nameEdit);
            nameListener = new NameChangeListener(this);
            tableName.AddTextChangedListener(nameListener);

            checkBoxView = new CheckBox(context);
            onCheckedListener = new OnCheckedListener(this);
            checkBoxView.SetOnCheckedChangeListener(onCheckedListener);
            column_row_view.AddView(checkBoxView);

            View dummy = new View(context);
            column_row_view.AddView(dummy);
            
            //controller.HookView(this);
        }

        internal void ClearView()
        {
            columns.Clear();
            rows.Clear();

            while(column_row_view.ChildCount > ADDITIONAL_COLUMNS)
            {
                column_row_view.RemoveViewAt(ADDITIONAL_COLUMNS);
            }

            while (tableView.ChildCount > ADDITIONAL_ROWS)
            {
                tableView.RemoveViewAt(ADDITIONAL_ROWS);
            }
        }

        public void Initiate(List<ColumnModel> columns, List<RowModel> rows)
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
            tableView.AddView(v.GetView(), index+ADDITIONAL_ROWS);
        }

        public void RowDeleted(int index)
        {
            rows[index].DeleteView();
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

        private void Checked(bool checkedChanged)
        {
            if (consume_checked)
            {
                consume_checked = false;
                return;
            }

            foreach (TableRowView row in rows)
            {
                row.UserChecked(checkedChanged);
                 row.SetChecked(checkedChanged);
            }
        }

        public void UserChangedName()
        {
            consume_change_name = true;
            controller.UserChangedName(tableName.Text);
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

        public View GetView()
        {
            return table_view;
        }

        public void DeleteView()
        {
            consume_checked = false;
            Checked(false);
            //controller.UnhookView(this);

            /*foreach(ColumnView v in columns)
            {
                v.DeleteView();
            }*/

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

        class NameChangeListener : Java.Lang.Object, ITextWatcher {
            
            Timer timer;
            readonly TableViewOld parentTable;

            public NameChangeListener(TableViewOld parent)
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
                    timer = null;
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
            TableViewOld updated;

            public UpdatenameTask(TableViewOld updated)
            {
                this.updated = updated;
            }

            public override void Run()
            {
                updated.UserChangedName();
                this.Cancel();
            }
        }

        class OnCheckedListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
        {

            TableViewOld parentView;
            public OnCheckedListener(TableViewOld parent){
                parentView = parent;
            }
            
            public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
            {
                parentView.Checked(isChecked);
            }
        }
    }
}