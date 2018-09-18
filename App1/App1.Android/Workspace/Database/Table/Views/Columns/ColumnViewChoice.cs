using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Views;
using static Android.Views.View;

namespace App1.Droid.Workspace.Database.Table.Views.Columns
{
    class ColumnViewChoice : ColumnView, IOnClickListener, IDialogInterfaceOnClickListener
    {
        View MainView;
        LinearLayout options;
        GradientDrawable backgroundShape;
        EditText editView;

        public ColumnViewChoice(Activity context, ColumnController controller) : base()
        {
            nameListener = new NameChangeListener(this);

            this.context = context;
            this.controller = controller;

            column_view = new EditText(context);
            MainView = context.LayoutInflater.Inflate(Resource.Layout.ColumnEditView_Choice, null, false);
            MainView.Clickable = false;
            MainView.Focusable = false;

            MainView.FindViewById<FrameLayout>(Resource.Id.frameLayoutViewName).AddView(column_view);
            column_view.AddTextChangedListener(nameListener);

            ImageView image = MainView.FindViewById<ImageView>(Resource.Id.imageViewColumnType);

            image.SetImageResource(Resource.Drawable.icons_menu_32);

            image.SetScaleType(ImageView.ScaleType.FitXy);
            image.SetColorFilter(Color.White);

            backgroundShape = new GradientDrawable();
            backgroundShape.SetColor(DatabaseController.colors[Math.Abs(column_view.Text.GetHashCode()) % DatabaseController.colors.Length].ToArgb());
            backgroundShape.SetCornerRadius(image.Height / 3);

            image.Background = backgroundShape;
            image.SetOnClickListener(this);

            options = MainView.FindViewById<LinearLayout>(Resource.Id.linearLayoutOptions);

            ImageButton addButton = MainView.FindViewById<ImageButton>(Resource.Id.imageButtonAddOption);
            addButton.SetOnClickListener(this);

            controller.HookView(this);
        }

        public override View GetView()
        {
            return MainView;
        }

        public override void OnClick(View v)
        {
            if(v.Id == 6667)
            {
                CallOptionEditDialog(options.IndexOfChild(v));
            }
            else if(v.Id == Resource.Id.imageButtonAddOption)
            {
                AddNewOption();
            }
            else
            {
                base.OnClick(v);
            }
        }

        private void AddNewOption()
        {
            Dictionary<string, string> update = new Dictionary<string, string>
            {
                { "operation", "new" },
                { "text", "new option" }
            };

            controller.DataUpdated(update);
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch ((DialogButtonType)which)
            {
                case DialogButtonType.Positive:
                    Dictionary<string, string> update = new Dictionary<string, string>
                    {
                        { "operation", "patch" },
                        { "text", editView.Text },
                        { "key", editView.Tag.ToString() }
                    };

                    controller.DataUpdated(update);
                    ((TextView)options.FindViewWithTag(editView.Tag)).Text = editView.Text;
                    dialog.Dismiss();
                    break;
                case DialogButtonType.Neutral:
                    Dictionary<string, string> del = new Dictionary<string, string>
                    {
                        { "operation", "del" },
                        { "key", editView.Tag.ToString() }
                    };
                    controller.DataUpdated(del);
                    options.RemoveView(options.FindViewWithTag(editView.Tag));
                    dialog.Dismiss();
                    break;
                default:
                    dialog.Dismiss();
                    break;

            }
        }

        private void CallOptionEditDialog(int pos)
        {

            editView = new EditText(context);
            editView.Text = ((TextView)options.GetChildAt(pos)).Text;
            editView.Tag = ((TextView)options.GetChildAt(pos)).Tag;

            AlertDialog.Builder b = new AlertDialog.Builder(context);
                b.SetView(editView);
                b.SetNegativeButton("Cancel",this);
                b.SetPositiveButton("Ok", this);
                b.SetNeutralButton("Delete", this);

            b.Create().Show();
        }

        public override void DataChanged(Dictionary<string, string> data)
        {
            if (data.ContainsKey("operation"))
            {
                switch (data["operation"])
                {
                    case "del":
                        options.RemoveViewAt(int.Parse(data["index"]));
                        break;
                    case "patch":
                        TextView updated = (TextView)options.FindViewWithTag(data["key"]);
                        updated.Text = data["value"];
                        break;
                    case "put":
                        TextView option = new TextView(context)
                        {
                            Id = 6667,
                            Tag = data["key"],
                            Text = data["value"]
                        };
                        option.SetOnClickListener(this);
                        options.AddView( option, int.Parse(data["index"]));
                        break;
                    case "new":
                        TextView newoption = new TextView(context)
                        {
                            Id = 6667,
                            Tag = data["key"],
                            Text = data["value"]
                        };
                        newoption.SetOnClickListener(this);
                        options.AddView(newoption, options.ChildCount-1);
                        break;
                }
            }
            else
            {
                foreach (string key in data.Keys)
                {
                    TextView option = new TextView(context)
                    {
                        Id = 6667,
                        Tag = key,
                        Text = data[key]
                    };

                    option.SetOnClickListener(this);
                    options.AddView(option, options.ChildCount-1);
                }
            }

        }
    }
}