using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table.Interfaces;
using Firebase.Database;

namespace App1.Droid.Table.Models
{
    public abstract class ColumnModel 
    {
        IColumnChangeListener changeListener;
        protected ColumnController controller;
        protected DatabaseReference Col_Ref;
        private string columnId;

        protected string type;
        protected string name;
        protected string RefString;
        protected bool isPrimary;
        public Dictionary<string, string> data;

        public ColumnModel()
        {
            this.isPrimary = false;
            this.data = new Dictionary<String, String>();
            this.controller = new ColumnController(this);
        }
        public ColumnModel(DataSnapshot data) : this()
        {
            Col_Ref = data.Ref;
            columnId = data.Key;

            if (data.Child("type").Exists())
            {
                Type = data.Child("type").Value.ToString();
            }
            if (data.Child("name").Exists())
            {
                name = data.Child("name").Value.ToString();
            }
            if (data.Child("primary").Exists())
            {
                string primary = data.Child("primary").Value.ToString();
                if(primary == "true" || primary == "1")
                {
                    isPrimary = true;
                }
            }

            if (data.Child("data").Exists())
            {
                foreach (DataSnapshot datum in data.Child("data").Children.ToEnumerable())
                {
                    this.data.Add(datum.Key, datum.Value.ToString());
                }
            }
        }

        public abstract CellModel ConstructCell();

        public abstract ColumnView GetEditView(Activity columnsEditActivity);

        public ColumnView GetView(Activity context) {
            return new ColumnView(context, controller);
        }

        internal ColumnView GetItemView(Activity context)
        {
            return new ColumnTextView(context, controller);
        }

        public CellModel GetCell()
        {
            CellModel ret = ConstructCell();

            ret.ParentColumn = this;

            return ret;
        }

        public void SetData(DataSnapshot data)
        {
            if (data.Child("name").Exists())
            {
                Name = data.Child("name").Value.ToString();
            }

            if (data.Child("data").Exists())
            {
                foreach (DataSnapshot datum in data.Child("data").Children.ToEnumerable())
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
            else
            {
                this.data.Clear();
            }
            controller.NotifyDataChanged(this.data);
            
        }

        public void Save()
        {

            Col_Ref = FirebaseDatabase.Instance.GetReferenceFromUrl(RefString);

            if (name == null || type == null)
            {
                Col_Ref.RemoveValue();
            }
            else
            {
                Col_Ref.Child("type").SetValue(type);
                Col_Ref.Child("name").SetValue(name);
                SaveData();
            }
        }

        public virtual void SaveData()
        {
            Col_Ref.Child("data").RemoveValue();
            foreach (string key in data.Keys)
            {
                Col_Ref.Child("data").Child(key).SetValue(data[key]);
            }
        }

        public void Dispose()
        {
            controller.NotifyDisposed();
        }

        public void SetTypeChangeListener(IColumnChangeListener l)
        {
            changeListener = l;
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

        public void SetReference(DatabaseReference Ref)
        {
            this.RefString = Ref.ToString();
        }

        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if(name != value)
                {
                    this.name = value;
                    this.controller.NotifyNameChanged(name);
                }
            }
        }

        public abstract Dictionary<string, string> Data { get; set; }
        public bool Primary {
            get
            {
                return isPrimary;
            }
            set
            {
                isPrimary = value;
            }
        }

        public string Type {
            get => type;
            set {
                if(type != value)
                {
                    this.type = value;
                    if(changeListener != null)
                    {
                        changeListener.ColumnChangedType(this, type);
                    }
                }
            }
        }

        public void UnbindListeners()
        {
        }

        internal void SetReference(string v)
        {
            this.RefString = v;
        }

        public void Remove()
        {
            type = null;
            name = null;
        }
    }
}