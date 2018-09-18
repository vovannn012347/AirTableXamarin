using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    public class TableModel : IComparer<RowModel>
    {
        private TableController controller;
        private SortController sortController;
        private FilterController filterController;

        private String name;
        private String newRowId;

        private List<RowModel> rows;
        private List<RowModel> filteredRows;

        private List<ColumnModel> columns;

        private List<FilterStruct> filters;
        private List<SortStruct> sorts;

        private ColumnChangeListener columnListener;
        private RowDataListener rowDataListener;
        private RowChangeListener rowListener;
        private NameChangeListener nameListener;

        private DatabaseReference databaseTable;
        private DatabaseReference databaseColumns;
        private DatabaseReference databaseRows;
        private DatabaseReference tableRowData;
        private DatabaseReference tableNameData;

        bool consume_name_update;

        public TableModel()
        {
            rows = new List<RowModel>();
            filteredRows = new List<RowModel>();
            columns = new List<ColumnModel>();

            filters = new List<FilterStruct>();
            sorts = new List<SortStruct>();
            
            controller = new TableController(this);
            sortController = new SortController(this);
            filterController = new FilterController(this);

            columnListener = new ColumnChangeListener(this);
            rowListener = new RowChangeListener(this);
            nameListener = new NameChangeListener(this);
            rowDataListener = new RowDataListener(this);
        }

        public void SetTable(DatabaseReference table, DatabaseReference tableName)
        {
            consume_name_update = false;
            if (databaseTable != null)
            {
                UnbindListeners();
            }
            databaseTable = table;
            tableNameData = tableName;
            this.name = "somename";

            columnListener = new ColumnChangeListener(this);
            rowListener = new RowChangeListener(this);
            nameListener = new NameChangeListener(this);
            rowDataListener = new RowDataListener(this);

            columns.Clear();

            foreach (RowModel model in rows)
            {
               model.UnbindListeners();
            }
            rows.Clear();

            controller.NotifyViewCleared();

            tableNameData.AddValueEventListener(nameListener);

            tableRowData = databaseTable.Child("row_data").Child("new_row_id");
            tableRowData.AddValueEventListener(rowDataListener);

            //load columns
            databaseColumns =
                    databaseTable.Child("columns");
            databaseColumns.AddChildEventListener(columnListener);

            //load rows
            databaseRows =
                    databaseTable.Child("rows");
            databaseRows.AddChildEventListener(rowListener);
        }

        internal SortDialog GetSortView(Activity context, Android.Views.View anchor)
        {
            return new SortDialog(sortController, context, anchor);
        }

        internal FilterDialog GetFilterView(Activity context, View anchor)
        {
            return new FilterDialog(filterController, context, anchor);
        }

        public void ApplySorts(List<SortStruct> sorts)
        {
            this.sorts = sorts;

            ResortItems();
        }

        private void ResortItems()
        {
            filteredRows.Sort(this);

            controller.ItemsSortedOrFiltered();
            controller.NotifySortedAmount(sorts.Count);
        }

        public void Refilter()
        {
            filteredRows.Clear();

            foreach(RowModel m in rows)
            {
                if (Fltered(m)) filteredRows.Add(m);
            }

            controller.ItemsSortedOrFiltered();
            controller.NotifyFilteredAmount(filters.Count);
        }

        public void ApplyFilters(List<FilterStruct> filters)
        {
            this.filters = filters;

            Refilter();
        }

        protected bool Fltered(RowModel tested)
        {
            foreach(FilterStruct filter in filters)
            {
                if (!filter.Filtered(tested)) return false;
            }

            return true;
        }

        public int Compare(RowModel r1, RowModel r2)
        {
            int ret = 0;

            for (int i = 0; i < sorts.Count; ++i)
            {
                if ((ret = sorts[i].Compare(r1, r2)) != 0)
                {
                    break;
                }
            }

            if (ret == 0)
            {
                if (r1.RowKey.Length < r2.RowKey.Length)
                {
                    return 1;
                }
                else
                if (r1.RowKey.Length > r2.RowKey.Length)
                {
                    return -1;
                }
                else
                {
                    for (int i = 0; i < r1.RowKey.Length; ++i)
                    {
                        if (r1.RowKey[i] < r2.RowKey[i])
                        {
                            ret = 1;
                            break;
                        }
                        else
                        if (r1.RowKey[i] > r2.RowKey[i])
                        {
                            ret = -1;
                            break;
                        }
                    }
                }

            }

            return ret;
        }

        public bool TableSet()
        {
            return databaseTable != null;
        }

        public DatabaseReference AddNewBlankRow()
        {
            DatabaseReference newRowRef =
                  databaseRows.Child(newRowId);
            return newRowRef;
        }

        public void UnbindListeners()
        {
            if(null != databaseColumns)
            {
                databaseColumns.RemoveEventListener(columnListener);
            }
            if (null != databaseRows)
            {
                databaseRows.RemoveEventListener(rowListener);
            }
            if (null != tableRowData)
            {
                tableRowData.RemoveEventListener(rowDataListener);
            }
            if (null != tableNameData)
            {
                tableNameData.RemoveEventListener(nameListener);

            }
        }
        
        internal void DeleteSelf(bool v)
        {
            UnbindListeners();

            //controller.NotifyDeleteCommencing();
            if (v)
            {
                databaseTable.RemoveValue();
                tableNameData.RemoveValue();
            }
        }

        private void AddColumn(DataSnapshot dataSnapshot, int index)
        {
            if (!dataSnapshot.Exists()) return;

            ColumnModel column = GetColumnCell(dataSnapshot);
            column.ColumnId = dataSnapshot.Key;

            columns.Insert(index, column);
            foreach (RowModel row in rows)
            {
                row.AddedColumn(column, index);
            }
            
            controller.NotifyColumnAdded(column, index);
        }
        private void UpdateColumn(DataSnapshot columnData, int index)
        {
            String type = columnData.Child("type").Value.ToString();
            if (type != columns[index].Type)
            {
                ColumnModel column = GetColumnCell(columnData);
                column.ColumnId = columnData.Key;

                columns[index] = column;
                foreach (RowModel row in rows)
                {
                    row.UpdatedColumn(column, index);
                }

                controller.NotifyColumnUpdated(column, index);
            }
            else
            {
                columns[index].SetData(columnData);
            }
        }
        private void DeleteColumn(int index)
        {
            columns.RemoveAt(index);
            foreach (RowModel row in rows)
            {
                row.DeletedColumn(index);
            }
            controller.NotifyColumnDeleted(index);
        }
        
        private void AddRow(RowModel row, int index)
        {
            rows.Add(row);
            if (Fltered(row))
            {
                int insert = 0;
                if (filteredRows.Count > 0)
                {   while ( insert < filteredRows.Count && Compare(filteredRows[insert], row) < 0 )
                    {
                        ++insert; 
                    }
                    filteredRows.Insert(insert, row);
                }
                else
                {
                    filteredRows.Add(row);
                }
                controller.NotifyRowAdded(insert, row);
            }
        }
        private void DeleteRow(int index)
        {
            RowModel removed = rows[index];
            rows.RemoveAt(index);

            int remove = filteredRows.IndexOf(removed);

            if(remove != -1)
            {
                controller.NotifyRowDeleted(index);
                filteredRows.RemoveAt(remove);
            }
        }
        
        public void DeleteCheckedRows()
        {
            foreach (RowModel r in rows)
            {/*
                if (r.Checked)
                {
                    r.DeleteSelf();
                }*/
            }
        }

        public void UserCheckedRow(bool check)
        {
            controller.UserCheckedRow(check);
        }

        public TableViewOld GetTableView(Activity context)
        {
            return new TableViewOld(context, controller);
        }

        public TableView GetView(Activity context)
        {
            return new TableView(context, controller);
        }

        public static ColumnModel GetColumnCell(DataSnapshot cell)
        {
            string typeS = cell.Child("type").Value.ToString();
            if (Enum.TryParse<ColumnController.ColumnType>(typeS, out ColumnController.ColumnType type))
            {
                switch (type)
                {
                    case ColumnController.ColumnType.TEXT:
                        return new ColumnModelText(cell);
                    case ColumnController.ColumnType.NUMBER:
                        return new ColumnModelNumber(cell);
                    case ColumnController.ColumnType.CHOICE:
                        return new ColumnModelChoice(cell);
                    case ColumnController.ColumnType.IMAGE:
                        return new ColumnModelImage(cell);
                    case ColumnController.ColumnType.DATE:
                        return new ColumnModelDate(cell);
                }
            }
            return new ColumnModelText(cell);
        }

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                consume_name_update = true;
                if(tableNameData != null)
                {
                    tableNameData.SetValue(value);
                    controller.NotifyNameChanged(value);
                }
            }
        }

        public string ColorRefString
        {
            get
            {
                string datname = databaseTable
                    .Parent.Parent.Key;
                return databaseTable
                    .Parent.Parent.Parent.Parent.Child("database_data").Child("databases").Child(datname).Child("color").ToString();
            }
        }

        public DatabaseReference TableRef
        {
            get
            {
                return this.databaseTable;
            }
        }

        public DatabaseReference NameRef
        {
            get
            {
                return this.tableNameData;
            }
        }

        public List<ColumnModel> Columns {
            get { return columns; }
        }

        public List<RowModel> Rows {
            get
            {
                return filteredRows;
            }
        }

        public List<SortStruct> Sorts { get => sorts; }
        public List<FilterStruct> Filters { get => filters; }

        class RowDataListener : Java.Lang.Object, IValueEventListener
        {
            TableModel parentTable;

            public RowDataListener(TableModel parent)
            {
                parentTable = parent;
            }

            public void OnDataChange(DataSnapshot dataSnapshot)
            {
                if (dataSnapshot.Exists())
                {
                    parentTable.newRowId = dataSnapshot.Value.ToString();
                }
            }
            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("column listener failed" + databaseError.Message);
            }
        }

        class NameChangeListener : Java.Lang.Object, IValueEventListener
        {
            TableModel parentTable;

            public NameChangeListener(TableModel parent)
            {
                parentTable = parent;
            }
            
            public void OnDataChange(DataSnapshot dataSnapshot)
            {
                if (parentTable.consume_name_update)
                {
                    parentTable.consume_name_update = false;
                    return;
                }
                parentTable.Name = dataSnapshot.Value.ToString();
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("column listener failed" + databaseError.Message);
            }

        }

        class ColumnChangeListener : Java.Lang.Object, IChildEventListener
        {
            TableModel parentTable;

            public ColumnChangeListener(TableModel parent)
            {
                parentTable = parent;
            }
            public void OnChildAdded(DataSnapshot dataSnapshot, String previousChildName)
            {

                int columnPosition = -1;

                if (previousChildName == null)
                {
                    columnPosition = parentTable.columns.Count;
                }
                else
                {
                    for (int i = 0; i < parentTable.columns.Count; ++i)
                    {
                        if (parentTable.columns[i].ColumnId.Equals(previousChildName))
                        {
                            columnPosition = i+1;
                            break;
                        }
                    }
                }
                if (columnPosition >= 0)
                {
                    parentTable.AddColumn(dataSnapshot, columnPosition);
                }
            }
            
            public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
            {

                int columnPosition = -1;
                for (int i = 0; i < parentTable.columns.Count; ++i)
                {
                    if (parentTable.columns[i].ColumnId.Equals(dataSnapshot.Key))
                    {
                        columnPosition = i;
                        break;
                    }
                }

                if (columnPosition >= 0)
                {
                    parentTable.UpdateColumn(dataSnapshot, columnPosition);
                }

            }

            public void OnChildRemoved(DataSnapshot dataSnapshot)
            {
                String columnId = dataSnapshot.Key;

                int columnPosition = -1;

                for (int i = 0; i < parentTable.columns.Count; ++i)
                {
                    if (parentTable.columns[i].ColumnId.Equals(columnId))
                    {
                        columnPosition = i;
                        break;
                    }
                }
                parentTable.DeleteColumn(columnPosition);
            }

            public void OnChildMoved(DataSnapshot dataSnapshot, String s)
            {
                //will not happen?
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("column listener failed" + databaseError.Message);
            }
        }

        class RowChangeListener : Java.Lang.Object, IChildEventListener
        {
            TableModel parentTable;

            public RowChangeListener(TableModel parent)
            {
                parentTable = parent;
            }
            
            public void OnChildAdded(DataSnapshot dataSnapshot, String previousChildName)
            {
                //detect new row added
                if (dataSnapshot.Key.Equals(parentTable.newRowId))
                {
                    String[] rowKeyParts = parentTable.newRowId.Split('_');
                    String newRowKey = rowKeyParts[0] + "_" + (Int64.Parse(rowKeyParts[1]) + 1);
                    parentTable.tableRowData.SetValue(newRowKey);
                }
                
                RowModel row = new RowModel(parentTable.Columns, dataSnapshot.Ref)
                {
                    RowKey = dataSnapshot.Key
                };

                if (previousChildName == null || previousChildName.Equals(""))
                {
                    parentTable.AddRow(row, 0);
                }
                else
                {
                    bool rowadded = false;

                    for (int i = 0; i < parentTable.rows.Count; ++i)
                    {
                        if (parentTable.rows[i].RowKey.Equals(previousChildName))
                        {
                            parentTable.AddRow(row, i + 1);
                            rowadded = true;
                            break;
                        }
                    }
                    if (!rowadded)
                    {
                        if (parentTable.rows.Count > 0)
                        {
                            parentTable.AddRow(row, parentTable.rows.Count - 1);
                        }
                        else
                        {
                            parentTable.AddRow(row, 0);
                        }
                    }
                }

                

            }

            public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
            {
                //this is handled in cell changes
            }

            public void OnChildRemoved(DataSnapshot dataSnapshot)
            {
                String rowKey = dataSnapshot.Key;

                for (int i = 0; i < parentTable.rows.Count; ++i)
                {
                    if (parentTable.rows[i].RowKey.Equals(rowKey))
                    {
                        parentTable.DeleteRow(i);
                        break;
                    }
                }
            }

            public void OnChildMoved(DataSnapshot dataSnapshot, String s)
            {
                //this will not happen, maybe
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("Row listener failed" + databaseError.Message);
            }

        }
    }
}