using System.Collections.Generic;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views.Cells;

namespace App1.Droid.Table.Controllers.Cells
{
    class CellControllerText
    {
        private CellModelText cellModelText;
        private List<CellViewText> cellViews;

        public CellControllerText(CellModelText cellModelText)
        {
            this.cellModelText = cellModelText;
            cellViews = new List<CellViewText>();
        }

        internal void HookView(CellViewText cellViewText)
        {
            cellViews.Add(cellViewText);
            cellViewText.SetData(cellModelText.Data);
        }
        internal void UnhookView(CellViewText cellViewText)
        {
            cellViews.Remove(cellViewText);
        }

        internal void NotifyDataChanged(string text)
        {
            foreach(CellViewText view in cellViews)
            {
                view.SetData(text);
            }
        }
        internal void UserSetData(string text)
        {
            cellModelText.Data = text;
        }
    }
}