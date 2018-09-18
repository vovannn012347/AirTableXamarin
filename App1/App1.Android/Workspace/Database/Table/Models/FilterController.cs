using System;
using System.Collections.Generic;

namespace App1.Droid.Table.Models
{
    internal class FilterController
    {
        private TableModel tableModel;

        public FilterController(TableModel model)
        {
            this.tableModel = model;
        }

        public void HookView(FilterDialog f)
        {
            f.Init(tableModel.Filters, tableModel.Columns);
        }

        internal void ApplyFiltes(List<FilterStruct> filters)
        {
            tableModel.ApplyFilters(filters);
        }
    }
}