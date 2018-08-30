using System;
using System.Collections.Generic;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    class RowModel : IDisposable
    {
        private string rowKey;
        private bool isChecked;

        private List<CellModel> cells;
        private RowController controller;

        private DatabaseReference rowReference;
        private RowChildChangeListener childListener;
        TableModel parent;

        public RowModel()
        {
            controller = new RowController(this);
            cells = new List<CellModel>();
            childListener = new RowChildChangeListener(this);
            isChecked = false;
        }

        public RowModel(TableModel table, DatabaseReference Ref) : this()
        {
            parent = table;

            foreach (ColumnModel columnCell in table.Columns)
            {
                CellModel m = columnCell.GetCell();
                m.RowReference = Ref;
                cells.Add(m);
            }

            rowReference = Ref;
            rowReference.AddChildEventListener(childListener);
        }

        public void AddedColumn(ColumnModel column, int index) {
            cells[index] = column.GetCell();
            controller.NotifyColumnAdded(index, cells[index], column);
        }
        public void DeletedColumn(int index){
            cells[index].EraseData();
            controller.NotifyColumnDeleted(index);
        }
        public void UpdatedColumn(ColumnModel column, int index)
        {
            CellModel oldCell = cells[index];
            CellModel newCell = column.GetCell();
                newCell.RowReference = this.rowReference;
                newCell.ColumnChangeSetData(oldCell.Data);
                cells[index] = newCell;

            controller.NotifyColumnUpdated(index, newCell, column);
        }

        public void DeleteSelf(){
            rowReference.RemoveValue();
        }
        public void Dispose()
        {
            childListener.Dispose();
        }

        public TableRowView GetView(Activity context){

            TableRowView tv = new TableRowView(context, controller);

            return tv;
        }

        public List<CellModel> Cells
        {
            get
            {
                return this.cells;
            }
        }
        public List<ColumnModel> Columns
        {
            get { return parent.Columns; }
        }
        public String RowKey
        {
            get
            {
                return rowKey;
            }
            set
            {
                rowKey = value;
            }
        }
        public bool Checked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if(value != isChecked)
                {
                    isChecked = value;
                    controller.NotifyCheckedChanged(isChecked);
                    parent.UserCheckedRow(value);
                }

            }
        }

        class RowChildChangeListener : Java.Lang.Object, IChildEventListener
        {
            RowModel parentRow;

            public RowChildChangeListener(RowModel row){
                parentRow = row;
            }
            
            public void OnChildAdded(DataSnapshot dataSnapshot, String previousChildName)
            {
                String columnKey = dataSnapshot.Key;
                foreach (CellModel cell in parentRow.cells)
                {
                    if (cell.ParentColumn.ColumnId.Equals(columnKey))
                    {
                        cell.SetData(dataSnapshot);
                        break;
                    }
                }
            }
    
            public void OnChildChanged( DataSnapshot dataSnapshot, String previousChildName)
            {
                String columnKey = dataSnapshot.Key;
                foreach (CellModel cell in parentRow.cells)
                {
                    if (cell.ParentColumn.ColumnId.Equals(columnKey))
                    {
                        cell.SetData(dataSnapshot);
                        break;
                    }
                }
            }
    
            public void OnChildRemoved( DataSnapshot dataSnapshot)
            {
                String columnKey = dataSnapshot.Key;
                foreach (CellModel cell in parentRow.cells)
                {
                    if (cell.ParentColumn.ColumnId.Equals(columnKey))
                    {
                        cell.EraseData();
                        break;
                    }
                }
            }
    
            public void OnChildMoved( DataSnapshot dataSnapshot, String s)
            {
                //this will not happen
            }
    
            public void OnCancelled( DatabaseError databaseError)
            {
                Console.WriteLine("CELL", "Something happened " + databaseError.Message);
            }
        }


    }
}