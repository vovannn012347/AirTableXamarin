using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;

namespace App1.Droid.Workspace.Database.Table.Dialogs
{
    class TypeChangeDialog : DialogFragment, Android.Views.View.IOnClickListener, IDialogInterfaceOnClickListener
    {
        ColumnController updatedColumn;
        ColumnController.ColumnType[] availiableTypes;
        AlertDialog.Builder b;
        Activity context;

        View dialogLayout;

        public TypeChangeDialog(Activity context, ColumnController controller)
        {
            this.context = context;
            updatedColumn = controller;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            b = new AlertDialog.Builder(context);

            availiableTypes = updatedColumn.GetAvailiableColumnChangeTypes();

            dialogLayout = context.LayoutInflater.Inflate(Resource.Layout.ColumnEditTypeChange, null);
            GridLayout grid = dialogLayout.FindViewById<GridLayout>(Resource.Id.gridLayoutChangeTypes);

            float imageSize = context.Resources.GetDimension(Resource.Dimension.workplace_image_size);

            foreach (ColumnController.ColumnType type in availiableTypes)
            {
                LinearLayout addlayout =
                    context.LayoutInflater.Inflate(Resource.Layout.ColumnTypeImageLayout, grid, false)
                    .FindViewById<LinearLayout>(Resource.Id.linearLayoutType);

                ImageView image = (ImageView)addlayout.GetChildAt(0);
                int typeint = (int)type;
                GradientDrawable g = new GradientDrawable();
                g.SetCornerRadius(imageSize / 3);
                g.SetColor(DatabaseController.colors[(int)type % DatabaseController.colors.Length]);

                image.Background = g;
                image.SetColorFilter(Color.White);

                switch (type)
                {
                    case ColumnController.ColumnType.CHOICE:
                        image.SetImageResource(Resource.Drawable.icons_menu_32);
                        break;
                    case ColumnController.ColumnType.DATE:
                        image.SetImageResource(Resource.Drawable.icons_calendar_48);
                        break;
                    case ColumnController.ColumnType.IMAGE:
                        image.SetImageResource(Resource.Drawable.icons_picture_32);
                        break;
                    case ColumnController.ColumnType.NUMBER:
                        image.SetImageResource(Resource.Drawable.icons_bars_30);
                        break;
                    case ColumnController.ColumnType.TEXT:
                        image.SetImageResource(Resource.Drawable.icons_text_30);
                        break;
                }

                String title = type.ToString().ToLower();
                char ch = title[0];
                title.Remove(0);

                TextView titleText = addlayout.FindViewById<TextView>(Resource.Id.textViewType);

                titleText.Text = ""+ char.ToUpper(ch) + title;

                image.Id = (int)type;
                image.SetOnClickListener(this);
                grid.AddView(addlayout);
            }

            b.SetView(dialogLayout);
            b.SetNegativeButton("Cancel", this);
            b.SetNeutralButton("Delete", this);

            return b.Create();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Android.Resource.Id.Home:
                    this.Dismiss();
                    break;
                default:
                    updatedColumn.UserSetType((ColumnController.ColumnType)v.Id);
                    Dismiss();
                    break;
            }
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            this.Dismiss();
        }
    }
}