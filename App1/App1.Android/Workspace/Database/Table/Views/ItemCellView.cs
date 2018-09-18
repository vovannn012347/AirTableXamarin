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
using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Workspace.Database.Table.Views
{
    class ItemCellView
    {
        LinearLayout mainView;

        readonly ColumnView column;
        readonly CellView cell;
        readonly CellModel.Type type;

        public ItemCellView(ColumnModel column, CellModel cell, Activity context)
        {
            this.type = cell.CellType();
            this.column = column.GetItemView(context);
            this.cell = cell.GetView(context);

            mainView = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };
            LinearLayout.LayoutParams p = new LinearLayout.LayoutParams
                (LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent)
            {
                Gravity = GravityFlags.CenterHorizontal
            };
            mainView.LayoutParameters = p;

            mainView.Clickable = false;
            mainView.Focusable = false;
        }

        public CellModel.Type CellType()
        {
            return type;
        }

        public void DeleteView()
        {
            this.column.DeleteView();
            this.cell.DeleteView();
        }

        public View GetView()
        {
            mainView.RemoveAllViews();

            if (type != CellModel.Type.Image)
            {
                mainView.AddView(this.column.GetView());
            }
            mainView.AddView(this.cell.GetView());

            mainView.SetGravity(GravityFlags.Center);
            mainView.SetPadding(0, 0, 20, 0);

            return mainView;
        }

        internal View GetHeadlineView()
        {
            if (type != CellModel.Type.Image)
            {
                mainView.AddView(this.column.GetView());
            }
            mainView.RemoveAllViews();
            mainView.AddView(this.cell.GetView());

            return mainView;
        }
    }
}