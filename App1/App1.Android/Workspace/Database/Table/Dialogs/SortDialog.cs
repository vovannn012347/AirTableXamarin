using System;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models;

namespace App1.Droid
{
    internal class SortDialog : Java.Lang.Object, PopupWindow.IOnDismissListener, View.IOnClickListener
    {
        SortController controller;
        Activity context;

        View anchor;
        PopupWindow retView;
        View mainView;
        LinearLayout sortsList;

        List<SortStruct> sorts;
        List<ColumnModel> columns;
        List<bool> isSorted;

        SortColumnChangeListener columnChangeListener;
        SortTypeChangeListener typeChangeListener;

        View updated;

        public SortDialog(SortController controller, Activity context, View anchor)
        {
            this.controller = controller;
            this.context = context;
            this.anchor = anchor;

            columnChangeListener = new SortColumnChangeListener(this);
            typeChangeListener = new SortTypeChangeListener(this);

            mainView = context.LayoutInflater.Inflate(Resource.Layout.SortPopup, null, false);
            mainView.SetBackgroundColor(Color.White);

            sortsList = mainView.FindViewById<LinearLayout>(Resource.Id.linearLayoutSortsList);

            View closeButton = mainView.FindViewById(Resource.Id.imageButtonClose);
            closeButton.SetOnClickListener(this);

            View saveButton = mainView.FindViewById(Resource.Id.buttonSave);
            saveButton.SetOnClickListener(this);

            View addButton = mainView.FindViewById(Resource.Id.relativeLayoutAddSort);
            addButton.SetOnClickListener(this);

            retView = new PopupWindow(context)
            {
                ContentView = mainView
            };

            retView.SetOnDismissListener(this);

            controller.HookView(this);
        }


        public void OnDismiss()
        {
            this.controller = null;
            this.context = null;
            this.Dispose();
        }

        private View MakeNewSortItem(int sortStructIndex, string columnId, SortStruct.SortMode mode)
        {
            ColumnModel m = columns.Find(C => C.ColumnId == columnId);
            
            View ret = context.LayoutInflater.Inflate(Resource.Layout.SortItem, null, false);

            GradientDrawable d = new GradientDrawable();
            d.SetColor(Color.Transparent);
            d.SetStroke(2, Color.Black);
            d.SetCornerRadius(8);

            TextView columnButton = ret.FindViewById<TextView>(Resource.Id.ViewSortColumn);
            columnButton.SetOnClickListener(this);
            columnButton.Background = d;
            columnButton.Tag = sortStructIndex;

            TextView sortTypeButton = ret.FindViewById<TextView>(Resource.Id.ViewSortType);
            sortTypeButton.SetOnClickListener(this);
            sortTypeButton.Background = d;
            sortTypeButton.Tag = sortStructIndex;

            View removeButton = ret.FindViewById(Resource.Id.ViewSortRemove);
            removeButton.SetOnClickListener(this);
            removeButton.Tag = sortStructIndex;

            //populate default views

            columnButton.Text = m.Name;
            switch (mode)
            {
                case SortStruct.SortMode.ASCENDING:
                    sortTypeButton.Text = "Ascending";
                    break;
                case SortStruct.SortMode.DESCENDING:
                    sortTypeButton.Text = "Descending";
                    break;
            }
            
            return ret;
        }

        class SortColumnChangeListener : Java.Lang.Object, PopupMenu.IOnMenuItemClickListener
        {
            SortDialog parent;

            public SortColumnChangeListener(SortDialog parent)
            {
                this.parent = parent;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                if (!item.IsEnabled) return false;

                SortStruct s = parent.sorts[(int)parent.updated.Tag];
                int colIndex = parent.columns.FindIndex(C => C.ColumnId == s.ColumnId);

                parent.isSorted[colIndex] = false;

                s.ColumnId = parent.columns[item.ItemId].ColumnId;
                parent.isSorted[item.ItemId] = true;

                ((TextView)parent.updated).Text = item.TitleFormatted.ToString();
                return true;
            }
        }

        class SortTypeChangeListener : Java.Lang.Object, PopupMenu.IOnMenuItemClickListener
        {
            SortDialog parent;

            public SortTypeChangeListener(SortDialog parent)
            {
                this.parent = parent;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                if (!item.IsEnabled) return false;

                SortStruct s = parent.sorts[(int)parent.updated.Tag];

                s.Mode = (SortStruct.SortMode)item.ItemId;

                ((TextView)parent.updated).Text = item.TitleFormatted.ToString();
                return true;
            }
        }

        public void OnClick(View v)
        {
            updated = v;
            switch (v.Id)
            {
                case Resource.Id.ViewSortColumn:
                    PopupMenu m = new PopupMenu(context, anchor);
                    
                    for(int i = 0; i < columns.Count; ++i)
                    {
                        IMenuItem added = m.Menu.Add(0, i, 0, columns[i].Name);
                        added.SetEnabled(!isSorted[i]);
                    }

                    updated = v;
                    m.SetOnMenuItemClickListener(columnChangeListener);
                    m.Show();

                    break;
                case Resource.Id.ViewSortType:
                    PopupMenu m2 = new PopupMenu(context, anchor);

                    m2.Menu.Add(0, (int)SortStruct.SortMode.ASCENDING, 0, "Ascending");
                    m2.Menu.Add(0, (int)SortStruct.SortMode.DESCENDING, 0, "Descending");

                    updated = v;
                    m2.SetOnMenuItemClickListener(typeChangeListener);
                    m2.Show();

                    break;
                case Resource.Id.ViewSortRemove:
                    int index = (int)v.Tag;
                    RemoveSort(index);
                    break;
                case Resource.Id.imageButtonClose:
                    Close();
                    break;
                case Resource.Id.buttonSave:
                    SaveSort();
                    break;
                case Resource.Id.relativeLayoutAddSort:
                    AddNewSort();
                    break;
            }
        }

        public void RemoveSort(int index)
        {
            SortStruct removed = sorts[index];

            isSorted[sorts.IndexOf(removed)] = false;
            sortsList.RemoveViewAt(index);
            sorts.RemoveAt(index);

            for(int i = index; i < sortsList.ChildCount-1; ++i)
            {
                View updated = sortsList.GetChildAt(i);

                TextView columnButton = updated.FindViewById<TextView>(Resource.Id.ViewSortColumn);
                columnButton.Tag = i;

                TextView sortTypeButton = updated.FindViewById<TextView>(Resource.Id.ViewSortType);
                sortTypeButton.Tag = i;

                View removeButton = updated.FindViewById(Resource.Id.ViewSortRemove);
                removeButton.Tag = i;

            }
        }

        public void Init(List<SortStruct> sorts, List<ColumnModel> models)
        {
            this.columns = models;
            this.sorts = new List<SortStruct>(sorts);

            isSorted = new List<bool>(columns.Count);
            for(int i = 0; i < columns.Count; ++i)
            {
                isSorted.Add(false);
            }

            for(int i = 0; i < sorts.Count; ++i)
            {
                int index = models.FindIndex( V => V.ColumnId == sorts[i].ColumnId);

                if ( index != -1){
                    sortsList.AddView(MakeNewSortItem(i, sorts[i].ColumnId, sorts[i].Mode), sortsList.ChildCount-1);
                    isSorted[index] = true;
                }
            }
        }

        public void SaveSort()
        {
            controller.ApplySort(sorts);
            this.Close();
        }

        public void Close()
        {
            retView.Dismiss();
        }

        public void AddNewSort()
        {
            for(int i = 0; i < isSorted.Count; ++i)
            {
                if (!isSorted[i])
                {
                    SortStruct s = new SortStruct(columns[i].ColumnId, SortStruct.SortMode.ASCENDING);

                    sorts.Add(s);
                    sortsList.AddView(MakeNewSortItem(sorts.Count-1, s.ColumnId, s.Mode), sortsList.ChildCount - 1);
                    isSorted[i] = true;
                    break;
                }
            }
        }
        
        public PopupWindow GetView()
        {
            return retView;
        }
    }
}