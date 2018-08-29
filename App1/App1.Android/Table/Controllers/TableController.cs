using System;
using System.Collections.Generic;
using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Table.Controllers
{
    class TableController
    {
        private TableModel tableModel;
        private List<TableView> tableViews;
 
        public TableController(TableModel tableModel)
        {
            this.tableModel = tableModel;
            tableViews = new List<TableView>();
        }

        internal void NotifyRowDeleted(int index)
        {
            foreach(TableView view in tableViews)
            {
                view.RowDeleted(index);
            }
        }

        public void HookView(TableView tableView)
        {
            tableViews.Add(tableView);
            tableView.Initiate(tableModel.Columns, tableModel.Rows);
        }

        public void UnhookView(TableView tableView)
        {
            tableViews.Remove(tableView);
        }

        internal void NotifyNameChanged(string name)
        {
            foreach (TableView view in tableViews)
            {
                view.SetName(name);
            }
        }

        internal void NotifyColumnAdded(ColumnModel column, int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnAdded(column, index);
            }
        }

        internal void NotifyColumnUpdated(ColumnModel column, int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnReplaced(column, index);
            }
        }

        internal void NotifyColumnDeleted(int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnDeleted(index);
            }
        }

        internal void NotifyRowAdded(int index, RowModel row)
        {
            foreach (TableView view in tableViews)
            {
                view.RowAdded(row, index);
            }
        }

        internal void UserChangedName(string name)
        {
            tableModel.Name = name;
        }

        internal void UserDeletedCheckedRows()
        {
            tableModel.DeleteCheckedRows();
        }

        internal void NewRowAdded(RowModel newRow)
        {
            foreach (TableView view in tableViews)
            {
                view.NewRowAdded(newRow);
            }
        }

        internal void UserCheckedRow(bool check)
        {
            foreach (TableView view in tableViews)
            {
                view.RowChecked(check);
            }
        }
    }
}