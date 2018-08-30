using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    abstract class ColumnModel
    {
        protected ColumnController controller;
        private string columnId;

        public string type;
        public string name;
        public Dictionary<string, string> data;

        public ColumnModel()
        {
            this.data = new Dictionary<String, String>();
            this.controller = new ColumnController(this);
        }
        public ColumnModel(DataSnapshot data) : this()
        {
            type = data.Child("type").Value.ToString();
            name = data.Child("name").Value.ToString();

            foreach (DataSnapshot datum in data.Child("data").Children.ToEnumerable())
            {
                this.data.Add(datum.Key, datum.Value.ToString());
            }
        }

        public abstract CellModel ConstructCell();
        public ColumnView GetView(Activity context) {
            return new ColumnView(context, controller);
        }
        public CellModel GetCell()
        {
            CellModel ret = ConstructCell();

            ret.ParentColumn = this;

            return ret;
        }

        public void SetData(DataSnapshot data)
        {
            Name = data.Child("name").Value.ToString();
            
            foreach(DataSnapshot datum in data.Child("data").Children.ToEnumerable())
            {
                if (this.data.Keys.Contains(datum.Key))
                {
                    this.data[datum.Key] = datum.Value.ToString();
                }
                else
                {
                    this.data.Add(datum.Key, datum.Value.ToString());
                }
            }
            
        }

        public String ColumnId
        {
            get {
                return this.columnId;
            }
            set
            {
                this.columnId = value;
            }
        }
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.controller.NotifyNameChanged(name);
            }
        }
    }
}