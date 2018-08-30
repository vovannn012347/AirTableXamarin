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

        public void HookView(TableView tableView)
        {
            tableViews.Add(tableView);
            tableView.Initiate(tableModel.Columns, tableModel.Rows);
        }
        public void UnhookView(TableView tableView)
        {
            tableViews.Remove(tableView);
        }

        public void NotifyColumnAdded(ColumnModel column, int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnAdded(column, index);
            }
        }
        public void NotifyColumnUpdated(ColumnModel column, int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnReplaced(column, index);
            }
        }
        public void NotifyColumnDeleted(int index)
        {
            foreach (TableView view in tableViews)
            {
                view.ColumnDeleted(index);
            }
        }

        public void NewRowAdded(RowModel newRow)
        {
            foreach (TableView view in tableViews)
            {
                view.NewRowAdded(newRow);
            }
        }
        public void NotifyRowAdded(int index, RowModel row)
        {
            foreach (TableView view in tableViews)
            {
                view.RowAdded(row, index);
            }
        }
        public void NotifyRowDeleted(int index)
        {
            foreach(TableView view in tableViews)
            {
                view.RowDeleted(index);
            }
        }

        public void NotifyNameChanged(string name)
        {
            foreach (TableView view in tableViews)
            {
                view.SetName(name);
            }
        }
        public void UserChangedName(string name)
        {
            tableModel.Name = name;
        }

        public void UserDeletedCheckedRows()
        {
            tableModel.DeleteCheckedRows();
        }
        public void UserCheckedRow(bool check)
        {
            foreach (TableView view in tableViews)
            {
                view.RowChecked(check);
            }
        }
    }
}