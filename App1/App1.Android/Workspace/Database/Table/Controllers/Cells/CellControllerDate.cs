using System.Collections.Generic;

using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;

namespace App1.Droid.Table.Controllers.Cells
{
    class CellControllerDate
    {
        private CellModelDate cellModelDate;
        private List<ICellViewDate> cellViews;

        public CellControllerDate(CellModelDate cellModelDate)
        {
            this.cellModelDate = cellModelDate;
            cellViews = new List<ICellViewDate>();
        }

        public void HookView(ICellViewDate cellViewDate)
        {
            cellViews.Add(cellViewDate);
            cellViewDate.Init(cellModelDate.GetFormat(), cellModelDate.Data);
        }
        public void UnhookView(ICellViewDate cellViewDate)
        {
            cellViews.Remove(cellViewDate);
        }
        
        public void NotifyDataChanged(string date)
        {
            foreach (ICellViewDate viw in cellViews)
            {
                viw.SetData(date);
            }
        }
        public void UserSetData(string text)
        {
            cellModelDate.Data = text;   
        }
    }
}