using Android.App;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models;
using App1.Droid.Workspace.Database;
using App1.Droid.Workspace.Database.Table.Views;
using System;
using System.Collections.Generic;
using static Android.Views.View;

namespace App1.Droid
{
    public class DatabaseView : Java.Lang.Object, IOnClickListener
    {
        private DatabaseController controller;
        private Activity context;

        View mainLayout;
        LinearLayout tableNameList;
        //ActionBar nameView;
        TableView tableView;
        View currentSelettedTableViewButton;
        FrameLayout tableContainer;
        Color mainColor;
        
        public DatabaseView(DatabaseController controller, TableView tableView, Activity context)
        {
            this.controller = controller;
            this.context = context;
            this.tableView = tableView;
            //nameView = context.ActionBar; 
            mainColor = Color.Black;

            mainLayout = context.LayoutInflater.Inflate(Resource.Layout.Databaselayout, null);
            tableNameList = mainLayout.FindViewById<LinearLayout>(Resource.Id.tablesListLinearLayout);

            tableContainer = mainLayout.FindViewById<FrameLayout>(Resource.Id.tableContentView);
            tableContainer.AddView(tableView.GetView());

            ImageButton 
            addButton = mainLayout.FindViewById<ImageButton>(Resource.Id.imageButtonAddTable);
            addButton.SetOnClickListener(this);

            controller.HookView(this);
        }

        public void UpdateTableView(TableModel model)
        {
            tableContainer.RemoveAllViews();
            tableContainer.AddView(model.GetView(context).GetView());
        }

        internal void TableNameChanged(int index, string table_name)
        {
            View newTableName = tableNameList.GetChildAt(index);

            newTableName.FindViewById<TextView>(Resource.Id.tableButtonTextView).Text = table_name;
        }

        internal void DeleteCommencing()
        {
            context = null;
            controller = null;
            mainLayout = null;
            tableNameList = null;
        }
        
        internal void SetColor(Color color)
        {
            mainColor = color;
            tableView.SetColor(color);

            if (null != currentSelettedTableViewButton)
            {
                currentSelettedTableViewButton.SetBackgroundColor(color);
            }
        }

        public void Initialize(List<string> names)
        {
            for(int i = 0; i < names.Count; ++i)
            {
                View newTableNameView = context.LayoutInflater.Inflate(Resource.Layout.TableSelectionButtonLayout, tableNameList, true);
                newTableNameView.SetOnClickListener(this);

                newTableNameView.FindViewById<TextView>(Resource.Id.tableButtonTextView).Text = names[i];
            }
        }

        internal void TableNameAdded(int index, string name)
        {
            View newTableNameView = context.LayoutInflater.Inflate(Resource.Layout.TableSelectionButtonLayout, tableNameList, false);
            newTableNameView.SetOnClickListener(this);

            newTableNameView.FindViewById<TextView>(Resource.Id.tableButtonTextView).Text = name;
            
            tableNameList.AddView(newTableNameView, index);
            if (tableNameList.ChildCount == 1)
            {
                newTableNameView.PerformClick();
            }
        }

        internal void NameChanged(string name)
        {
            //nameView.Title = name;
        }

        internal void TableNameDeleted(int index)
        {
            tableNameList.RemoveViewAt(index);
        }

        public View GetView()
        {
            return mainLayout;
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.imageButtonAddTable)
            {
                controller.NewTableAdded();
            }else
            if(v.Id == Resource.Id.layoutSelectionButton)
            {
                if (currentSelettedTableViewButton == v)
                {
                    tableView.LaunchSchemaEdit(mainColor.ToArgb());
                }
                else
                {
                    if (currentSelettedTableViewButton != null)
                    {
                        currentSelettedTableViewButton.SetBackgroundColor(Android.Graphics.Color.White);
                        currentSelettedTableViewButton.FindViewById<TextView>(Resource.Id.tableButtonTextView).SetTextColor(Color.Black);
                        (currentSelettedTableViewButton.FindViewById<ImageView>(Resource.Id.imageViewCanEditDetector).Visibility) = ViewStates.Invisible;
                        (currentSelettedTableViewButton.FindViewById<ImageView>(Resource.Id.imageViewCanEditDetector).Visibility) = ViewStates.Gone;
                        (currentSelettedTableViewButton.FindViewById<ImageView>(Resource.Id.imageViewCanEditDetector).Visibility) = ViewStates.Gone;
                    }
                    currentSelettedTableViewButton = v;
                    currentSelettedTableViewButton.SetBackgroundColor(mainColor);
                    currentSelettedTableViewButton.FindViewById<TextView>(Resource.Id.tableButtonTextView).SetTextColor(Color.White);
                    (currentSelettedTableViewButton.FindViewById<ImageView>(Resource.Id.imageViewCanEditDetector).Visibility) = ViewStates.Visible;

                    controller.TableSelected(tableNameList.IndexOfChild(currentSelettedTableViewButton));
                }
            }
            
        }
    }
}