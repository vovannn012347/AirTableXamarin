using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;

namespace App1.Droid.Table.Views
{
    class TableRowView : Java.Lang.Object, Android.Views.View.IOnClickListener, IRowView
    {
        private Activity context;
        private RowController controller;

        private List<CellView> cells;

        private TableRow row_view;
        private CheckBox checkBox;
        private ImageButton expand_view_button;

        const int ADDITIONAL_COLUMNS = 2;

        bool consume_checked_update;
        bool consume_checked_send;

        public TableRowView(Activity context, RowController controller)
        {
            this.context = context;
            row_view = new TableRow(context);

            checkBox = new CheckBox(context);
            row_view.AddView(checkBox);
            checkBox.SetOnCheckedChangeListener(this);

            expand_view_button = new ImageButton(context);
            expand_view_button.SetImageResource(Android.Resource.Drawable.IcInputGet);
            expand_view_button.SetBackgroundColor(Android.Graphics.Color.Transparent);
            expand_view_button.SetScaleType(ImageView.ScaleType.FitCenter);

            expand_view_button.SetOnClickListener(this);
            row_view.AddView(expand_view_button);

            cells = new List<CellView>();

            this.controller = controller;
            controller.HookView(this);
        }

        public void ColumnAdded(int index, CellModel cellModel, ColumnModel columnModel)
        {
            CellView v = cellModel.GetView(context);
            cells.Insert(index, v);
            row_view.AddView(v.GetView(), index + ADDITIONAL_COLUMNS);
        }
        public void ColumnDeleted(int index)
        {
            cells.RemoveAt(index);
            row_view.RemoveViewAt(index + ADDITIONAL_COLUMNS);
        }
        public void ColumnChanged(int index, CellModel cellModel, ColumnModel columnModel)
        {
            CellView v = cellModel.GetView(context);
            cells[index] = v;
        
            row_view.RemoveViewAt(index + ADDITIONAL_COLUMNS);
            row_view.AddView(v.GetView(), index + ADDITIONAL_COLUMNS);
        }

        public void Initiate(List<CellModel> cells, List<ColumnModel> columns)
        {
            foreach (CellModel model in cells)
            {
                CellView v = model.GetView(context);
                this.cells.Add(v);
                row_view.AddView(v.GetView());
            }
        }

        public void DeleteView()
        {
            UserChecked(false);
            controller.UnhookView(this);
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            UserChecked(isChecked);
        }

        public void OnClick(View v)
        {
            EditRowDialog dialog
                = new EditRowDialog(context, controller);

            dialog.Show(context.FragmentManager, "EditTextDialog");
        }

        public View GetView()
        {
            return row_view;
        }

        public void SetChecked(bool check)
        {
            if (consume_checked_update)
            {
                consume_checked_update = false;
                return;
            }
            consume_checked_send = true;
            checkBox.Checked = check;
        }

        public void UserChecked(bool check)
        {
            if (consume_checked_send)
            {
                consume_checked_send = false;
                return;
            }
            consume_checked_update = true;
            controller.UserCheckedRow(check);
        }

    }
}