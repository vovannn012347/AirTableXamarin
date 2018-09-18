using System;
using System.Collections.Generic;

using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Table.Controllers
{
    public class ColumnController
    {
        private ColumnModel columnModel;
        private List<ColumnView> columnViews;

        public enum ColumnType { CHOICE, DATE, IMAGE, NUMBER, TEXT }


        public ColumnController(ColumnModel columnModel)
        {
            this.columnModel = columnModel;
            columnViews = new List<ColumnView>();
        }

        internal void HookView(ColumnView columnView)
        {
            columnViews.Add(columnView);
            columnView.SetName(columnModel.Name);
            columnView.DataChanged(columnModel.Data);
        }
        internal void UnhookView(ColumnView columnView)
        {
            columnViews.Remove(columnView);
        }

        internal ColumnType[] GetAvailiableColumnChangeTypes()
        {
            if (columnModel.Primary)
            {
                return new ColumnType[] { ColumnType.TEXT, ColumnType.NUMBER, ColumnType.DATE };
            }
            else
            {
                return new ColumnType[]{ ColumnType.TEXT, ColumnType.NUMBER, ColumnType.DATE, ColumnType.CHOICE, ColumnType.IMAGE };
            }
        }

        internal void UserSetType(ColumnType type)
        {
            columnModel.Type = type.ToString();

        }

        public void NotifyNameChanged(string name)
        {
            foreach(ColumnView v in columnViews)
            {
                v.SetName(name);
            }
        }

        public void NotifyDataChanged(Dictionary<string, string> data)
        {
            foreach (ColumnView v in columnViews)
            {
                v.DataChanged(data);
            }
        }

        internal void NotifyDisposed()
        {
            //TODO: implement this
        }

        internal void NameChanged(string text)
        {
            columnModel.Name = text;
        }

        internal void DataUpdated(Dictionary<string, string> update)
        {
            columnModel.Data = update;
        }

        public void Delete()
        {
            columnModel.Remove();
        }
    }
}