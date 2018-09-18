using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models;
using System.Collections.Generic;
using static App1.Droid.Table.Controllers.ColumnController;

namespace App1.Droid
{
    internal class FilterDialog: Java.Lang.Object, PopupWindow.IOnDismissListener, View.IOnClickListener, Android.Content.IDialogInterfaceOnClickListener
    {
        FilterController controller;
        Activity context;

        View anchor;
        PopupWindow retView;
        View mainView;
        LinearLayout filtersList;

        List<FilterStruct> filters;
        List<ColumnModel> columns;

        FilterColumnChangeListener columnChangeListener;
        FilterTypeChangeListener typeChangeListener;

        View updated;
        EditText editText;

        public FilterDialog(FilterController controller, Activity context, View anchor)
        {
            this.controller = controller;
            this.context = context;
            this.anchor = anchor;

            columnChangeListener = new FilterColumnChangeListener(this);
            typeChangeListener = new FilterTypeChangeListener(this);

            mainView = context.LayoutInflater.Inflate(Resource.Layout.FilterPopup, null, false);
            mainView.SetBackgroundColor(Color.White);

            filtersList = mainView.FindViewById<LinearLayout>(Resource.Id.linearLayoutFiltersList);

            View closeButton = mainView.FindViewById(Resource.Id.imageButtonClose);
            closeButton.SetOnClickListener(this);

            View saveButton = mainView.FindViewById(Resource.Id.buttonSave);
            saveButton.SetOnClickListener(this);

            View addButton = mainView.FindViewById(Resource.Id.relativeLayoutAddFilter);
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

        private View MakeNewFilterItem(int sortStructIndex, string columnId, string predicate, FilterStruct.FilterMode mode)
        {
            ColumnModel m = columns.Find(C => C.ColumnId == columnId);
            
            View ret = context.LayoutInflater.Inflate(Resource.Layout.FilterItem, null, false);

            GradientDrawable d = new GradientDrawable();
            d.SetColor(Color.Transparent);
            d.SetStroke(2, Color.Black);
            d.SetCornerRadius(8);

            TextView columnButton = ret.FindViewById<TextView>(Resource.Id.ViewFilterColumn);
            columnButton.SetOnClickListener(this);
            columnButton.Tag = sortStructIndex;

            TextView sortTypeButton = ret.FindViewById<TextView>(Resource.Id.ViewFilterType);
            sortTypeButton.SetOnClickListener(this);
            sortTypeButton.Tag = sortStructIndex;

            TextView sortStringButton = ret.FindViewById<TextView>(Resource.Id.ViewFilterPredicate);
            sortStringButton.SetOnClickListener(this);
            sortStringButton.SetPadding(20, 0, 20, 0);
            sortStringButton.Tag = sortStructIndex;

            View removeButton = ret.FindViewById(Resource.Id.ViewFilterRemove);
            removeButton.SetOnClickListener(this);
            removeButton.Tag = sortStructIndex;

            columnButton.Text = m.Name;
            switch (mode)
            {
                case FilterStruct.FilterMode.IS:
                    sortTypeButton.Text = "is";
                    break;
                case FilterStruct.FilterMode.NOT_IS:
                    sortTypeButton.Text = "is not";
                    break;
                case FilterStruct.FilterMode.CONTAINS:
                    sortTypeButton.Text = "contains";
                    break;
                case FilterStruct.FilterMode.NOT_CONTAINS:
                    sortTypeButton.Text = "not contain";
                    break;
                case FilterStruct.FilterMode.EMPTY:
                    sortTypeButton.Text = "empty";
                    break;
                case FilterStruct.FilterMode.NOT_EMPTY:
                    sortTypeButton.Text = "not empty";
                    break;
                /*case FilterStruct.FilterMode.ONE_OF:
                    sortTypeButton.Text = "one of";
                    break;
                case FilterStruct.FilterMode.NOT_ONE_OF:
                    sortTypeButton.Text = "not one of";
                    break;*/
            }
            sortStringButton.Text = predicate;


            columnButton.Background = d;
            sortTypeButton.Background = d;
            sortStringButton.Background = d;

            return ret;
        }

        class FilterColumnChangeListener : Java.Lang.Object, PopupMenu.IOnMenuItemClickListener
        {
            FilterDialog parent;

            public FilterColumnChangeListener(FilterDialog parent)
            {
                this.parent = parent;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                FilterStruct s = parent.filters[(int)parent.updated.Tag];
                int colIndex = parent.columns.FindIndex(C => C.ColumnId == s.ColumnId);
                
                s.ColumnId = parent.columns[item.ItemId].ColumnId;

                ((TextView)parent.updated).Text = item.TitleFormatted.ToString();
                return true;
            }
        }

        class FilterTypeChangeListener : Java.Lang.Object, PopupMenu.IOnMenuItemClickListener
        {
            FilterDialog parent;

            public FilterTypeChangeListener(FilterDialog parent)
            {
                this.parent = parent;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                FilterStruct s = parent.filters[(int)parent.updated.Tag];

                s.Mode = (FilterStruct.FilterMode)item.ItemId;

                ((TextView)parent.updated).Text = item.TitleFormatted.ToString();
                return true;
            }
        }

        public void OnClick(View v)
        {
            updated = v;
            switch (v.Id)
            {
                case Resource.Id.ViewFilterColumn:
                    PopupMenu m = new PopupMenu(context, anchor);
                    
                    for(int i = 0; i < columns.Count; ++i)
                    {
                        if(columns[i].Type == ColumnType.CHOICE.ToString())
                        {
                            continue;
                        }
                        IMenuItem added = m.Menu.Add(0, i, 0, columns[i].Name);
                    }

                    updated = v;
                    m.SetOnMenuItemClickListener(columnChangeListener);
                    m.Show();

                    break;
                case Resource.Id.ViewFilterType:
                    PopupMenu m2 = new PopupMenu(context, anchor);
                    
                    ColumnModel cm = columns.Find(C => C.ColumnId == filters[(int)v.Tag].ColumnId);

                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.IS, 0, "is");
                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.NOT_IS, 0, "is not");

                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.CONTAINS, 0, "contain");
                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.NOT_CONTAINS, 0, "not contain");

                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.EMPTY, 0, "empty");
                    m2.Menu.Add(0, (int)FilterStruct.FilterMode.NOT_EMPTY, 0, "not empty");

                    updated = v;
                    m2.SetOnMenuItemClickListener(typeChangeListener);
                    m2.Show();

                    break;
                case Resource.Id.ViewFilterPredicate:

                    editText = new EditText(context);
                    if (!string.IsNullOrWhiteSpace(((TextView)v).Text) || !string.IsNullOrEmpty(((TextView)v).Text))
                    {
                        editText.Text = ((TextView)v).Text;
                    }
                    

                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(context);

                    builder.SetMessage("Comparsion value");
                    builder.SetView(editText);
                    builder.SetNegativeButton( "Cancel", this);
                    builder.SetPositiveButton("Ok", this);
                   

                    updated = v;
                    Android.Support.V7.App.AlertDialog d = builder.Create();
                    
                    d.Window.SetSoftInputMode(SoftInput.StateVisible);
                    
                    d.Show();

                    break;
                case Resource.Id.ViewFilterRemove:
                    int index = (int)v.Tag;
                    RemoveSort(index);
                    break;
                case Resource.Id.imageButtonClose:
                    Close();
                    break;
                case Resource.Id.buttonSave:
                    SaveSort();
                    break;
                case Resource.Id.relativeLayoutAddFilter:
                    AddNewFilter();
                    break;
            }
        }
        
        public void OnClick(IDialogInterface dialog, int which)
        {
            if(which == (int)DialogButtonType.Positive)
            {
                ((TextView)updated).Text = editText.Text;
                filters[(int)updated.Tag].Condition = editText.Text;
            }
        }

        public void RemoveSort(int index)
        {
            FilterStruct removed = filters[index];

            filtersList.RemoveViewAt(index);
            filters.RemoveAt(index);

            for(int i = index; i < filtersList.ChildCount-1; ++i)
            {
                View updated = filtersList.GetChildAt(i);

                TextView columnButton = updated.FindViewById<TextView>(Resource.Id.ViewFilterColumn);
                columnButton.Tag = i;

                TextView sortTypeButton = updated.FindViewById<TextView>(Resource.Id.ViewFilterType);
                sortTypeButton.Tag = i;

                TextView sortStringButton = updated.FindViewById<TextView>(Resource.Id.ViewFilterPredicate);
                sortStringButton.Tag = i;

                View removeButton = updated.FindViewById(Resource.Id.ViewFilterRemove);
                removeButton.Tag = i;

            }
        }

        public void Init(List<FilterStruct> filters, List<ColumnModel> models)
        {
            this.columns = models;
            this.filters = new List<FilterStruct>(filters);

            for(int i = 0; i < filters.Count; ++i)
            {
                int index = models.FindIndex( V => V.ColumnId == filters[i].ColumnId);

                if ( index != -1){
                    filtersList.AddView(MakeNewFilterItem(i, filters[i].ColumnId, filters[i].Condition, filters[i].Mode), filtersList.ChildCount-1);
                }
            }
        }

        public void SaveSort()
        {
            controller.ApplyFiltes(filters);
            this.Close();
        }

        public void Close()
        {
            retView.Dismiss();
        }

        public void AddNewFilter()
        {
            FilterStruct s = new FilterStruct(columns[0].ColumnId, FilterStruct.FilterMode.NOT_CONTAINS, " ");

            filters.Add(s);
            filtersList.AddView(MakeNewFilterItem(filters.Count-1, s.ColumnId, s.Condition, s.Mode), filtersList.ChildCount - 1);

        }
        
        public PopupWindow GetView()
        {
            return retView;
        }
    }
}