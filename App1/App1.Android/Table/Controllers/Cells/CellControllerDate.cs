using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;

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

        public void HookView(ICellViewDate cellViewDate)
        {
            cellViews.Add(cellViewDate);
            cellViewDate.SetDateFormat(cellModelDate.GetFormat());
            cellViewDate.SetData(cellModelDate.Data);
        }

        public void UnhookView(ICellViewDate cellViewDate)
        {
            cellViews.Remove(cellViewDate);
        }
    }
}