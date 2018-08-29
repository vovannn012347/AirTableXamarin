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
        }

        public ColumnModel(DataSnapshot data)
        {
            this.data = new Dictionary<String, String>();

            type = data.Child("type").Value.ToString();
            name = data.Child("name").Value.ToString();

            foreach (DataSnapshot datum in data.Child("data").Children.ToEnumerable())
            {
                this.data.Add(datum.Key, datum.Value.ToString());
            }
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

        public abstract ColumnView GetView(Activity context);

        public abstract CellModel constructCell();
        public CellModel GetCell()
        {
            CellModel ret = constructCell();

            ret.ParentColumn = this;

            return ret;
        }


    }
}