using System;
using System.Collections.Generic;

using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Table.Controllers
{
    class RowController
    {
        private RowModel rowModel;
        private List<IRowView> rowViews; 

        public RowController(RowModel rowModel)
        {
            this.rowModel = rowModel;
            rowViews = new List<IRowView>();
        }
        
        public void HookView(IRowView rowView)
        {
            rowViews.Add(rowView);
            rowView.Initiate(rowModel.Cells, rowModel.Columns);
        }
        public void UnhookView(IRowView rowView)
        {
            rowViews.Remove(rowView);
        }
        
        public void NotifyColumnAdded(int index, CellModel cellModel, ColumnModel columnModel)
        {
            foreach (IRowView v in rowViews)
            {
                v.ColumnAdded(index, cellModel, columnModel);
            }
        }
        public void NotifyColumnUpdated(int index, CellModel newCell, ColumnModel columnModel)
        {
            foreach (IRowView v in rowViews)
            {
                v.ColumnChanged(index, newCell, columnModel);
            }
        }
        public void NotifyColumnDeleted(int index)
        {
            foreach (IRowView v in rowViews)
            {
                v.ColumnDeleted(index);
            }
        }

        public void NotifyCheckedChanged(bool isChecked)
        {
            foreach(IRowView v in rowViews)
            {
                v.SetChecked(isChecked);
            }
        }
        public void UserCheckedRow(bool check)
        {
            rowModel.Checked = check;
        }

        public void UserDeletedRow()
        {
            rowModel.DeleteSelf();
        }
        
    }
}