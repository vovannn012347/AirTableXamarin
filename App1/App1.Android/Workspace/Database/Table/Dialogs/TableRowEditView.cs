using Android.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using System;
using System.Collections.Generic;

namespace App1.Droid.Table.Models
{
    public class TableRowEditView : Java.Lang.Object, IRowView
    {
        private Activity context;
        private RowController controller;

        private List<CellView> cells;

        private LinearLayout mainLayout;

        private View mainView;

        public TableRowEditView(Activity context, RowController controller)
        {
            this.context = context;
            this.controller = controller;

            mainView = context.LayoutInflater.Inflate(Resource.Layout.ItemsEditlinearlayout, null, false);
            mainLayout = mainView.FindViewById<LinearLayout>(Resource.Id.editCellItemsLayout);

            cells = new List<CellView>();

            controller.HookView(this);
        }

        public View GetView()
        {
            return mainLayout;
        }

        public void Initiate(List<CellModel> cells, List<ColumnModel> columns)
        {
            for(int i = 0; i < columns.Count; ++i)
            {
                ColumnView c = columns[i].GetView(context);
                CellView v = cells[i].GetView(context);
                this.cells.Add(v);

                mainLayout.AddView(c.GetView());
                mainLayout.AddView(v.GetEditView());
            }
        }

        public void ColumnAdded(int index, CellModel cellModel, ColumnModel columnModel)
        {
            ColumnView c = columnModel.GetView(context);
            CellView v = cellModel.GetView(context);
            cells.Insert(index, v);

            mainLayout.AddView(v.GetEditView(), index * 2);
            mainLayout.AddView(c.GetView(), index * 2);
        }
        public void ColumnDeleted(int index)
        {
            cells.RemoveAt(index);
            mainLayout.RemoveViewAt(index * 2);
            mainLayout.RemoveViewAt(index * 2);
        }
        public void ColumnChanged(int index, CellModel cellModel, ColumnModel columnModel)
        {
            ColumnDeleted(index);
            ColumnAdded(index, cellModel, columnModel);
        }

        public void DeleteView()
        {
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
        }

        public void OnClick(View v)
        {
        }

        public void SetChecked(bool check)
        {
        }

        public void UserChecked(bool check)
        {
        }

    }
}