using System;
using System.Collections.Generic;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    class TableModel
    {
        private TableController controller;

        private String name;
        private String newRowId;

        private List<RowModel> rows;
        private List<ColumnModel> columns;

        private readonly ColumnChangeListener columnListener;
        private readonly RowDataListener rowDataListener;
        private readonly RowChangeListener rowListener;
        private readonly NameChangeListener nameListener;

        private DatabaseReference databaseTable;
        private DatabaseReference databaseColumns;
        private DatabaseReference databaseRows;
        private DatabaseReference tableRowData;
        private DatabaseReference tableNameData;

        bool consume_name_update;

        public TableModel()
        {
            rows = new List<RowModel>();
            columns = new List<ColumnModel>();
            controller = new TableController(this);

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

            tableNameData = tableName;
            tableNameData.AddValueEventListener(nameListener);

            databaseTable = table;

            this.name = "somename";

            //load columns
            databaseColumns =
                    databaseTable.Child("columns");
            databaseColumns.AddChildEventListener(columnListener);

            //load rows
            databaseRows =
                    databaseTable.Child("rows");
            databaseRows.AddChildEventListener(rowListener);

            tableRowData = databaseTable.Child("data").Child("new_row_id");
            tableRowData.AddValueEventListener(rowDataListener);
        }

        public void UnbindListeners()
        {
            databaseColumns.RemoveEventListener(columnListener);
            databaseRows.RemoveEventListener(rowListener);
            tableRowData.RemoveEventListener(rowDataListener);
            tableNameData.RemoveEventListener(nameListener);
        }

        private void AddColumn(DataSnapshot dataSnapshot, int index)
        {

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
            if (type != columns[index].type)
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

        private void AddNewRow()
        {
            DatabaseReference newRowRef =
                    databaseRows.Child(newRowId);

            RowModel newRow = new RowModel(this, newRowRef)
            {
                RowKey = newRowId
            };

            rows.Add(newRow);
            controller.NewRowAdded(newRow);
        }
        private void AddRow(RowModel row, int index)
        {
            rows.Add(row);
            controller.NotifyRowAdded(index, row);
        }
        private void DeleteRow(int index)
        {
            rows.RemoveAt(index);
            controller.NotifyRowDeleted(index);
        }
        public void DeleteCheckedRows()
        {
            foreach (RowModel r in rows)
            {
                if (r.Checked)
                {
                    r.DeleteSelf();
                }
            }
        }

        public void UserCheckedRow(bool check)
        {
            controller.UserCheckedRow(check);
        }

        public TableView GetTableView(Activity context)
        {
            return new TableView(context, controller);
        }
        private ColumnModel GetColumnCell(DataSnapshot cell)
        {
            String type = cell.Child("type").Value.ToString();
                switch(type){
                    case "TEXT":
                        return new ColumnModelText(cell);
                    case "NUMBER":
                        return new ColumnModelNumber(cell);
                    case "CHOICE":
                        return new ColumnModelChoice(cell);
                    case "IMAGE":
                        return new ColumnModelImage(cell);
                    case "DATE":
                        return new ColumnModelDate(cell);

            }
                return new ColumnModelText();
        }
        public TableView GetView(MainActivity mainActivity)
        {
            return new TableView(mainActivity, controller);
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
                tableNameData.SetValue(value);
                controller.NotifyNameChanged(value);
            }
        }
        public List<ColumnModel> Columns {
            get { return columns; }
        }
        public List<RowModel> Rows {
            get
            {
                return rows;
            }
        }

        class RowDataListener : Java.Lang.Object, IValueEventListener
        {
            TableModel parentTable;

            public RowDataListener(TableModel parent)
            {
                parentTable = parent;
            }

            public void OnDataChange(DataSnapshot dataSnapshot)
            { 
                parentTable.newRowId = dataSnapshot.Value.ToString();
                parentTable.AddNewRow();
            }
            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("D_COL", "column listener failed" + databaseError.Message);
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
                Console.WriteLine("D_COL", "column listener failed" + databaseError.Message);
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
                            columnPosition = i + 1;
                            break;
                        }
                    }
                }

                parentTable.AddColumn(dataSnapshot, columnPosition);
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

                parentTable.UpdateColumn(dataSnapshot, columnPosition);

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
                Console.WriteLine("D_COL", "column listener failed" + databaseError.Message);
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
                else
                {
                    RowModel row = new RowModel(parentTable, dataSnapshot.Ref);
                    row.RowKey = dataSnapshot.Key;

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
                Console.WriteLine("D_ROW", "Row listener failed" + databaseError.Message);
            }

        }
    }
}