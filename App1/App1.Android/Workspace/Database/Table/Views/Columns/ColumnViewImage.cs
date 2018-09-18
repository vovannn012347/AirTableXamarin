using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;

namespace App1.Droid.Workspace.Database.Table.Views.Columns
{
    class ColumnViewImage : ColumnView
    {
        View MainView;
        GradientDrawable backgroundShape;

        public ColumnViewImage(Activity context, ColumnController controller) : base(context, controller)
        {
            nameListener = new NameChangeListener(this);

            MainView = context.LayoutInflater.Inflate(Resource.Layout.ColumnEditView, null, false);

            MainView.FindViewById<FrameLayout>(Resource.Id.frameLayoutViewName).AddView(column_view);
            column_view.AddTextChangedListener(nameListener);

            ImageView image = MainView.FindViewById<ImageView>(Resource.Id.imageViewColumnType);

            image.SetImageResource(Resource.Drawable.icons_picture_32);

            image.SetScaleType(ImageView.ScaleType.FitXy);
            image.SetColorFilter(Color.White);

            backgroundShape = new GradientDrawable();
            backgroundShape.SetColor(DatabaseController.colors[Math.Abs(column_view.Text.GetHashCode()) % DatabaseController.colors.Length].ToArgb());
            backgroundShape.SetCornerRadius(image.Height / 3);

            image.Background = backgroundShape;
            image.SetOnClickListener(this);
        }

        public override View GetView()
        {
            return MainView;
        }

        public override void DataChanged(Dictionary<string, string> data)
        {
        }
    }
}