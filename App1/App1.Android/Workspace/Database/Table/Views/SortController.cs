using System;
using System.Collections.Generic;
using App1.Droid.Table.Models;

namespace App1.Droid
{
    public class SortController
    {
        private TableModel tableModel;

        public SortController(TableModel model)
        {
            this.tableModel = model;
        }

        internal void HookView(SortDialog sortView)
        {
            sortView.Init(tableModel.Sorts, tableModel.Columns);
        }

        internal void ApplySort(List<SortStruct> sorts)
        {
            tableModel.ApplySorts(sorts);
        }
    }
}