using System;
using System.Collections.Generic;

using Android.App;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;

namespace App1.Droid.Workspace.Database.Table.Views
{
    public class TableView : Java.Lang.Object, Android.Views.View.IOnClickListener
    {
        private TableController controller;
        private Activity parentActivity;

        private List<ItemView> items;
        
        private View mainView;
        private LinearLayout itemsView;
        private ScrollView itemsScroll;
        RelativeLayout sortButton;
        RelativeLayout filterButton;
        RelativeLayout viewOptionsLayout;

        public TableView()
        {
            items = new List<ItemView>();
        }

        public TableView(Activity context, TableController controller) : this()
        {
            this.controller = controller;
            this.parentActivity = context;

            mainView = context.LayoutInflater.Inflate(Resource.Layout.TableLayout, null);

            itemsView = mainView.FindViewById<LinearLayout>(Resource.Id.linearLayoutTableElements);
            itemsScroll = mainView.FindViewById<ScrollView>(Resource.Id.scrollViewTAbleElements);
            
            FloatingActionButton
            addTableItemButton = mainView.FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonAddRecord);
            addTableItemButton.SetOnClickListener(this);

            viewOptionsLayout = mainView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutViewOptions);

            sortButton =  mainView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutSortButton);
            sortButton.SetOnClickListener(this);

            filterButton = mainView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutFilterButton);
            filterButton.SetOnClickListener(this);

            controller.HookView(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.floatingActionButtonAddRecord:
                    controller.AddBlankRecordAndEdit(parentActivity);
                    break;
                case Resource.Id.relativeLayoutSortButton:
                    controller.CallSortDialog(v, parentActivity);
                    break;
                case Resource.Id.relativeLayoutFilterButton:
                    controller.CallFilterDialog(v, parentActivity);
                    break;
            }
            
        }

        public void ClearView()
        {
            itemsView.RemoveAllViews();
        }

        public void Initiate(List<RowModel> rows)
        {
            foreach (RowModel row in rows)
            {
                ItemView v = row.GetItemView(parentActivity);
                items.Add(v);
                itemsView.AddView(v.GetView());
            }
        }

        public void RefreshViews(List<RowModel> rows)
        {
            ClearView();
            foreach(ItemView v in items)
            {
                v.DeleteView();
            }

            foreach (RowModel row in rows)
            {
                ItemView v = row.GetItemView(parentActivity);
                items.Add(v);
                itemsView.AddView(v.GetView());
            }
        }

        public void NewRowAdded(RowModel row)
        {
            ItemView v = row.GetItemView(parentActivity);
            items.Add(v);
            itemsView.AddView(v.GetView());
        }
    
        public void RowAdded(RowModel row, int index)
        {
            ItemView v = row.GetItemView(parentActivity);
            items.Add(v);
            itemsView.AddView(v.GetView());
        }

        internal void SetColor(Color color)
        {
            viewOptionsLayout.SetBackgroundColor(color);
        }

        public void RowDeleted(int index)
        {
            items[index].DeleteView();
            items.RemoveAt(index);
            itemsView.RemoveViewAt(index);
        }

        public View GetView()
        {
            return mainView;
        }

        public void DeleteView()
        {
            controller.UnhookView(this);

            foreach (ItemView v in items)
            {
                v.DeleteView();
            }

            parentActivity = null;
            controller = null;
        }

        internal void FiltersSet(int count)
        {
            if (count > 0)
            {
                filterButton.FindViewById<TextView>(Resource.Id.textCount).Text = "" + count;
                filterButton.FindViewById<ImageView>(Resource.Id.imageButtonFilterButton).SetImageResource(Resource.Drawable.icons_filter_filled_50);
            }
            else
            {
                filterButton.FindViewById<TextView>(Resource.Id.textCount).Text = "";
                filterButton.FindViewById<ImageView>(Resource.Id.imageButtonFilterButton).SetImageResource(Resource.Drawable.icons_filter_outline_50);
            }
        }

        internal void SortsSet(int count)
        {
            if (count > 0)
            {
                sortButton.FindViewById<TextView>(Resource.Id.textCount).Text = "" + count;
                sortButton.FindViewById<ImageView>(Resource.Id.imageButtonImageSort).SetImageResource(Resource.Drawable.icons_menu_32);
            }
            else
            {
                sortButton.FindViewById<TextView>(Resource.Id.textCount).Text = "";
                sortButton.FindViewById<ImageView>(Resource.Id.imageButtonImageSort).SetImageResource(Resource.Drawable.icons_menu_outline_80);
            }
        }

        internal void LaunchSchemaEdit(int color)
        {
            controller.EditSchema(parentActivity, color);
        }

        internal void ColumnAdded(ColumnModel column, int index)
        {
        }

        internal void ColumnReplaced(ColumnModel column, int index)
        {
        }

        internal void ColumnDeleted(int index)
        {
        }

        internal void RowChecked(bool check)
        {
        }

        internal void SetName(string name)
        {
        }
    }
}