using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database;
using App1.Droid.Workspace.Database.Table;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Controllers
{
    public class TableController
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
            tableView.Initiate(tableModel.Rows);
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

        internal void ItemsSortedOrFiltered()
        {
            foreach(TableView v in tableViews)
            {
                v.RefreshViews(tableModel.Rows);
            }
        }

        class FallthroughInterceptor : Java.Lang.Object, View.IOnTouchListener
        {
            public bool OnTouch(View v, MotionEvent e)
            {
                return false;
            }
        }

        internal void CallSortDialog(View anchor, Activity context)
        {
            SortDialog d = tableModel.GetSortView(context, anchor);

            PopupWindow w = d.GetView();

            w.ShowAsDropDown(anchor);
            w.OutsideTouchable = true;
            w.Touchable = true;
            w.SetBackgroundDrawable(new BitmapDrawable());
        }

        internal void CallFilterDialog(View anchor, Activity context)
        {
            FilterDialog d = tableModel.GetFilterView(context, anchor);

            PopupWindow w = d.GetView();

            w.ShowAsDropDown(anchor);
            w.OutsideTouchable = true;
            w.Touchable = true;
            w.SetBackgroundDrawable(new BitmapDrawable());
        }

        internal void NotifyViewCleared()
        {
            foreach (TableView view in tableViews)
            {
                view.ClearView();
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

        internal void NotifySortedAmount(int count)
        {
            foreach (TableView view in tableViews)
            {
                view.SortsSet(count);
            }
        }

        internal void NotifyFilteredAmount(int count)
        {
            foreach (TableView view in tableViews)
            {
                view.FiltersSet(count);
            }
        }

        internal void EditSchema(Activity contextActivity, int color)
        {
            Intent intent = new Intent(contextActivity, typeof(TableEditActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("tableSource", tableModel.TableRef.ToString());
            intent.PutExtra("tableName", tableModel.NameRef.ToString());
            intent.PutExtra("colorSource", tableModel.ColorRefString);
            contextActivity.StartActivity(intent);
        }

        internal void AddBlankRecordAndEdit(Activity contextActivity)
        {
            if (!tableModel.TableSet())
            {
                return;
            }
            //shitty code begin
            DatabaseReference colorRef = tableModel.TableRef.Parent.Parent;
            string databasekey = colorRef.Key;
            colorRef = colorRef.Parent.Parent.Child("database_data").Child("databases").Child(databasekey).Child("color");
            //shitty code end
            Intent intent = new Intent(contextActivity, typeof(ItemEditActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("rowReference", tableModel.AddNewBlankRow().ToString());
            intent.PutExtra("tableSource", tableModel.TableRef.ToString());
            intent.PutExtra("colorSource", colorRef.ToString());
            contextActivity.StartActivity(intent);
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