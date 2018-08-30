using System.Collections.Generic;

using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Table.Controllers
{
    class ColumnController
    {
        private ColumnModel columnModel;
        private List<ColumnView> columnViews;

        public ColumnController(ColumnModel columnModel)
        {
            this.columnModel = columnModel;
            columnViews = new List<ColumnView>();
        }

        internal void HookView(ColumnView columnView)
        {
            columnViews.Add(columnView);
            columnView.SetName(columnModel.name);
        }
        internal void UnhookView(ColumnView columnView)
        {
            columnViews.Remove(columnView);
        }

        internal void NotifyNameChanged(string name)
        {
            foreach(ColumnView v in columnViews)
            {
                v.SetName(name);
            }
        }
    }
}