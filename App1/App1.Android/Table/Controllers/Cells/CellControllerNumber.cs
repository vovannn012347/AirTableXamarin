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
using App1.Droid.Table.Views.Cells;

namespace App1.Droid.Table.Controllers.Cells
{
    class CellControllerNumber
    {
        private CellModelNumber cellModelNumber;
        private List<CellViewNumber> cellViews;

        public CellControllerNumber(CellModelNumber cellModelNumber)
        {
            this.cellModelNumber = cellModelNumber;
            cellViews = new List<CellViewNumber>();
        }

        internal void HookView(CellViewNumber cellViewNumber)
        {
            cellViews.Add(cellViewNumber);
            cellViewNumber.SetData(cellModelNumber.Data);
        }

        internal void UnhookView(CellViewNumber cellViewNumber)
        {
            cellViews.Remove(cellViewNumber);
        }

        internal void NotifyDataChanged(string number)
        {
            foreach(CellViewNumber view in cellViews)
            {
                view.SetData(number);
            }
        }
        
        internal void UserSetData(string text)
        {
            cellModelNumber.Data = text;
        }
    }
}