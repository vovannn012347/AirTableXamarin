using System;
using System.Collections.Generic;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
   public class RowModel 
    {
        private string rowKey;
        private bool isChecked;

        private readonly List<ColumnModel> columns;
        private List<CellModel> cells;
        private RowController controller;

        private DatabaseReference rowReference;
        private RowChildChangeListener childListener;

        public RowModel()
        {
            controller = new RowController(this);
            cells = new List<CellModel>();
            childListener = new RowChildChangeListener(this);
            isChecked = false;
        }

        public RowModel(List<ColumnModel> columns, DatabaseReference Ref) : this()
        {
            this.columns = columns;

            foreach (ColumnModel columnCell in columns)
            {
                CellModel m = columnCell.GetCell();
                m.RowReference = Ref;
                cells.Add(m);
            }

            rowReference = Ref;
            rowReference.AddChildEventListener(childListener);
            rowKey = Ref.Key;
        }

        public void UnbindListeners()
        {
            rowReference.RemoveEventListener(childListener);
        }

        public void AddedColumn(ColumnModel column, int index) {
            CellModel newCell = column.GetCell();

            newCell.RowReference = this.rowReference;
            if (index >= cells.Count)
            {
                cells.Add(newCell);
            }
            else
            {
                cells.Insert(index, newCell);
            }
            controller.NotifyColumnAdded(index, cells[index], column);
        }
        public void DeletedColumn(int index){
            cells[index].ColumnDeleted();
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
            UnbindListeners();
            rowReference.RemoveValue();
        }
        public void Dispose()
        {
            UnbindListeners();
            //childListener.Dispose();
        }

        public void Save()
        {
            foreach(CellModel model in cells)
            {
                model.Save();
            }
        }

        public ItemView GetItemView(Activity context)
        {
            return new ItemView(context, controller);
        }

        public TableRowEditView GetEditView(Activity context)
        {
            TableRowEditView tv = new TableRowEditView(context, controller);

            return tv;
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
            get { return columns; }
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

        public DatabaseReference RowRef
        {
            get
            {
                return rowReference;
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
    
            public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
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
                        cell.ColumnDeleted();
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
                Console.WriteLine("Something happened " + databaseError.Message);
            }
        }


    }
}