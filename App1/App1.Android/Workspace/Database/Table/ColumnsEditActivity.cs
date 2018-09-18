using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Workspace.Database.Table.Interfaces;
using Firebase.Database;
using System;
using System.Collections.Generic;
using static Android.Views.View;

namespace App1.Droid.Workspace.Database.Table
{
    [Activity(Label = "ColumnsEditActivity", Theme = "@style/ColoredActivityTheme")]
    public class ColumnsEditActivity : AppCompatActivity, IOnClickListener, IColumnChangeListener
    {
        FirebaseDatabase database;
        DatabaseReference tableRef;
        DatabaseReference columnsRef;
        DatabaseReference columnsData;
        DatabaseReference colorRef;

        ColorListener colorListener;
        NewColumnIdValueListener columnIdListener;
        ColumnDataListener columnsDataListener;

        LinearLayout columnsEditView;
        View addButtonView;
        Android.Support.V7.Widget.Toolbar toolbar;

        List<ColumnModel> columns;
        //ColumnModel primaryColumn;
        string newColumnid;

        public ColumnsEditActivity()
        {
            columns = new List<ColumnModel>();
            columnIdListener = new NewColumnIdValueListener(this);
            columnsDataListener = new ColumnDataListener(this);
            colorListener = new ColorListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ColumnsEditActivity);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                toolbar.SetBackgroundColor(Color.LightGray);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetTitle(Resource.String.columns_edit_string);
            toolbar.SetTitleTextColor(Color.White);
            }

            database = FirebaseDatabase.Instance;

            tableRef = database.GetReferenceFromUrl(Intent.GetStringExtra("tableSource"));
            columnsRef = tableRef.Child("columns");
            columnsData = tableRef.Child("column_data");
            colorRef = database.GetReferenceFromUrl(Intent.GetStringExtra("colorSource"));

            columnsEditView = FindViewById<LinearLayout>(Resource.Id.linearLayoutEditContent);

            addButtonView = FindViewById(Resource.Id.relativeLayoutActionButtons);
            addButtonView.SetOnClickListener(this);
            addButtonView = FindViewById(Resource.Id.imageViewAdd);
            addButtonView.SetOnClickListener(this);

            if (colorRef != null)
            {
                colorRef.AddValueEventListener(colorListener);
            }

            columnsData.Child("new_column_id").AddValueEventListener(columnIdListener);
            columnsRef.AddChildEventListener(columnsDataListener);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SaveOnlyMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                case Resource.Id.action_save:
                    foreach(ColumnModel model in columns)
                    {
                        model.Save();
                    }
                    Finish();
                    break;
                    //return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnClick(View v)
        {
            if(v.Id == Resource.Id.relativeLayoutActionButtons || 
               v.Id == Resource.Id.imageViewAdd)
            {
                AddNewColumn();
            }
        }

        void AddNewColumn()
        {
            ColumnModel newModel = new ColumnModelText
            {
                Type = "TEXT",
                Name = "new column",
                ColumnId = newColumnid
            };
            newModel.SetTypeChangeListener(this);

            newModel.SetReference(columnsRef.Child("columns").Child(newColumnid).Ref.ToString());

            string[] keyParts = newColumnid.Split('_');
            string newKey = keyParts[0] + "_" + (System.Int64.Parse(keyParts[1]) + 1);


            columns.Add(newModel);
            columnsEditView.AddView(newModel.GetEditView(this).GetView(), columns.Count-1);

            columnsData.Child("new_column_id").SetValue(newKey);
        }

        protected override void OnDestroy()
        {
            try
            {
                columnsData.Child("new_column_id").RemoveEventListener(columnIdListener);
                columnsRef.RemoveEventListener(columnsDataListener);
                foreach(ColumnModel column in columns)
                {
                    column.UnbindListeners();
                }
            }catch(ArgumentException e)
            {
                System.Console.WriteLine("Another database disposal" + e.Message);
            }
            

            base.OnDestroy();
        }

        class NewColumnIdValueListener : Java.Lang.Object, IValueEventListener
        {
            ColumnsEditActivity parent;

            public NewColumnIdValueListener(ColumnsEditActivity parent)
            {
                this.parent = parent;
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    parent.newColumnid = snapshot.Value.ToString();
                }
            }

            public void OnCancelled(DatabaseError error)
            {

            }

        }

        void AddColumn(DataSnapshot data, int index)
        {
            ColumnModel column = TableModel.GetColumnCell(data);
            column.SetReference(database.GetReferenceFromUrl(columnsRef.ToString()).Child(data.Key).Ref.ToString());
            column.ColumnId = data.Key;
            column.SetTypeChangeListener(this);

            columns.Insert(index, column);
            

            columnsEditView.AddView(column.GetEditView(this).GetView(), index);
        }
        void RemoveColumn(int index)
        {
            columns[index].Remove();

            columnsEditView.RemoveViewAt(index);
        }

        void UpdateColumn(DataSnapshot columnData, int index)
        {
            string type = columnData.Child("type").Value.ToString();
            if (type != columns[index].Type)
            {

                RemoveColumn(index);
                AddColumn(columnData, index);
            }
            else
            {
                columns[index].SetData(columnData);
            }
        }

        public void ColumnChangedType(ColumnModel model, string typeS)
        {
            int index = columns.IndexOf(model);

            if (index > -1)
            {
                ColumnModel newmodel;
                if (Enum.TryParse<ColumnController.ColumnType>(typeS, out ColumnController.ColumnType type))
                {
                    switch (type)
                    {
                        case ColumnController.ColumnType.TEXT:
                            newmodel = new ColumnModelText();
                            break;
                        case ColumnController.ColumnType.NUMBER:
                            newmodel = new ColumnModelNumber();
                            break;
                        case ColumnController.ColumnType.CHOICE:
                            newmodel = new ColumnModelChoice();
                            break;
                        case ColumnController.ColumnType.IMAGE:
                            newmodel = new ColumnModelImage();
                            break;
                        case ColumnController.ColumnType.DATE:
                            newmodel = new ColumnModelDate();
                            break;
                        default:
                            newmodel = new ColumnModelText();
                            break;
                    }
                }
                else
                {
                    newmodel = new ColumnModelText();
                }

                newmodel.Type = model.Type;
                newmodel.Name = model.Name;
                newmodel.ColumnId = model.ColumnId;
                newmodel.Data = model.Data;
                newmodel.Primary = model.Primary;
                newmodel.SetTypeChangeListener(this);
                newmodel.SetReference(database.GetReferenceFromUrl(columnsRef.ToString()).Child(newmodel.ColumnId).Ref.ToString());

                columns[index] = newmodel;
                columnsEditView.RemoveViewAt(index);
                columnsEditView.AddView(newmodel.GetEditView(this).GetView(), index);
            }
        }

        class ColorListener : Java.Lang.Object, IValueEventListener
        {
            ColumnsEditActivity parentTable;
            public ColorListener(ColumnsEditActivity parent)
            {
                parentTable = parent;
            }

            public void OnCancelled(DatabaseError error)
            {
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    int val = int.Parse(snapshot.Value.ToString()) % DatabaseController.colors.Length;

                    parentTable.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(DatabaseController.colors[val]));
                }
            }
        }

        class ColumnDataListener : Java.Lang.Object, IChildEventListener
        {
            ColumnsEditActivity parent;

            public ColumnDataListener(ColumnsEditActivity parent)
            {
                this.parent = parent;
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if (snapshot.Exists())
                {
                    int columnPosition = -1;

                    if (previousChildName == null)
                    {
                        columnPosition = 0;
                    }
                    else
                    {
                        for (int i = 0; i < parent.columns.Count; ++i)
                        {
                            if (parent.columns[i].ColumnId.Equals(previousChildName))
                            {
                                columnPosition = i + 1;
                                break;
                            }
                        }
                    }
                    if (columnPosition >= 0)
                    {
                        parent.AddColumn(snapshot, columnPosition);
                    }

                }
            }
            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                if (snapshot.Exists())
                {
                    int columnPosition = -1;
                    for (int i = 0; i < parent.columns.Count; ++i)
                    {
                        if (parent.columns[i].ColumnId.Equals(snapshot.Key))
                        {
                            columnPosition = i;
                            break;
                        }
                    }

                    if (columnPosition >= 0)
                    {
                        parent.UpdateColumn(snapshot, columnPosition);
                    }
                }
            }
            public void OnChildRemoved(DataSnapshot snapshot)
            {
                string columnId = snapshot.Key;

                int columnPosition = -1;

                for (int i = 0; i < parent.columns.Count; ++i)
                {
                    if (parent.columns[i].ColumnId.Equals(columnId))
                    {
                        columnPosition = i;
                        break;
                    }
                }
                parent.RemoveColumn(columnPosition);
            }


            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
            }
            public void OnCancelled(DatabaseError error)
            {
            }
        }
    }
}