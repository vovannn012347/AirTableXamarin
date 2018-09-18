using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Workspace.Database;
using Firebase.Database;

namespace App1.Droid.Workspace
{
    [Activity(Label = "Add Database", Theme = "@style/MainTheme")]
    public class DatabaseAddActivity : AppCompatActivity, View.IOnClickListener
    {
        FirebaseDatabase database;
        DatabaseReference databasesSchemaReference;
        NewDatabaseIdListener idListener;
        LinearLayout imagePicker, colorPicker;
        
        EditText newName;
        ImageView selectedColor;
        ImageView selectedDrawable;

        int selectedcolor;
        float imageSize;
        string newDatabaseId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            SetContentView(Resource.Layout.DatabaseAddLayout);

            idListener = new NewDatabaseIdListener(this);
            database = FirebaseDatabase.Instance;
            databasesSchemaReference = database.GetReferenceFromUrl(Intent.GetStringExtra("databasesSchema"));

            imageSize = Resources.GetDimension(Resource.Dimension.workplace_image_size);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if(null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //SupportActionBar.SetTitle(Resource.String.add_new_database_string);
            }

            RelativeLayout layout = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutAddEmptyDatabase);
                           layout.SetOnClickListener(this);

            imagePicker = FindViewById<LinearLayout>(Resource.Id.linearLayoutImagePicker);
            colorPicker = FindViewById<LinearLayout>(Resource.Id.linearlayoutColorPicker);

            newName = FindViewById<EditText>(Resource.Id.editTextName);

            foreach(Color c in DatabaseController.colors)
            {
                ImageView newView = new ImageView(this)
                {
                    Id = Resource.Id.linearlayoutColorPicker
                };
                newView.SetOnClickListener(this);
                newView.SetImageResource(Resource.Drawable.icons_checkmark);
                newView.SetBackgroundColor(c);
                newView.SetColorFilter(c);
                colorPicker.AddView(newView);
            }
            selectedcolor = 0;
            selectedColor = (ImageView)colorPicker.GetChildAt(0);
            selectedColor.SetColorFilter(Color.White);

            foreach (int res in DatabaseController.imageResources)
            {
                ImageView newView = new ImageView(this)
                {
                    Id = Resource.Id.linearLayoutImagePicker
                };

                newView.SetImageResource(res);
                int padding = (int)imageSize / 8;
                newView.SetPadding(padding, padding, padding, padding);

                newView.SetMaxHeight((int)imageSize);
                newView.SetMaxWidth((int)imageSize);
                newView.SetMinimumHeight((int)imageSize);
                newView.SetMinimumHeight((int)imageSize);

                newView.SetScaleType(ImageView.ScaleType.FitXy);
                    
                newView.SetOnClickListener(this);
                imagePicker.AddView(newView);
                
            }
            selectedDrawable = (ImageView)imagePicker.GetChildAt(0);
            GradientDrawable backgroundShape = new GradientDrawable();
            backgroundShape.SetColor(DatabaseController.colors[selectedcolor].ToArgb());

            backgroundShape.SetCornerRadius(imageSize / 8);

            selectedDrawable.Background = backgroundShape;
            selectedDrawable.SetColorFilter(Color.White);

            Window.SetSoftInputMode(SoftInput.StateVisible);

            databasesSchemaReference.Child("new_database_id").AddValueEventListener(idListener);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            databasesSchemaReference.Child("new_database_id").RemoveEventListener(idListener);

        }

        public void OnClick(View v)
        {
            GradientDrawable backgroundShape;
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutAddEmptyDatabase:


                    if (string.IsNullOrWhiteSpace(newName.Text))
                    {
                        databasesSchemaReference.Child("databases").Child(newDatabaseId).Child("name").SetValue(Resources.GetString(Resource.String.unnamed_database_string));
                    }
                    else
                    {
                        databasesSchemaReference.Child("databases").Child(newDatabaseId).Child("name").SetValue(newName.Text);
                    }

                    databasesSchemaReference.Child("databases").Child(newDatabaseId).Child("color").SetValue(colorPicker.IndexOfChild(selectedColor));
                    databasesSchemaReference.Child("databases").Child(newDatabaseId).Child("icon").SetValue(imagePicker.IndexOfChild(selectedDrawable));

                    //todo: and init that database with empty table

                    string[] keyParts = newDatabaseId.Split('_');
                    string newKey = keyParts[0] + "_" + (Int64.Parse(keyParts[1]) + 1);

                    databasesSchemaReference.Child("new_database_id").SetValue(newKey);
                    this.Finish();
                    break;
                case Resource.Id.linearLayoutImagePicker:

                    selectedDrawable.SetBackgroundColor(Color.Transparent);
                    selectedDrawable.ClearColorFilter();

                    selectedDrawable = (ImageView)v;
                    backgroundShape = new GradientDrawable();
                    backgroundShape.SetColor(DatabaseController.colors[selectedcolor].ToArgb());

                    backgroundShape.SetCornerRadius(imageSize / 8);

                    selectedDrawable.Background = backgroundShape;
                    selectedDrawable.SetColorFilter(Color.White);

                    break;
                case Resource.Id.linearlayoutColorPicker:
                    selectedColor.SetColorFilter(DatabaseController.colors[selectedcolor]);

                    selectedColor = (ImageView)v;
                    selectedcolor = colorPicker.IndexOfChild(selectedColor);
                    selectedColor.SetImageResource(Resource.Drawable.icons_checkmark);
                    selectedColor.SetColorFilter(Color.White);

                    backgroundShape = new GradientDrawable();
                    backgroundShape.SetColor(DatabaseController.colors[selectedcolor].ToArgb());

                    backgroundShape.SetCornerRadius(imageSize / 8);

                    selectedDrawable.Background = backgroundShape;

                    break;
            }
            
        }

        public void OnCancel(IDialogInterface dialog)
        {
        }

        public void InitDatabaseSchema()
        {
            databasesSchemaReference.Child("new_database_id").SetValue("database_1");
        }

        class NewDatabaseIdListener : Java.Lang.Object, IValueEventListener
        {
            private DatabaseAddActivity parent;

            public NewDatabaseIdListener(DatabaseAddActivity workspaceModel)
            {
                this.parent = workspaceModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace New Database Id Error:" + error.Message);
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    parent.newDatabaseId = snapshot.Value.ToString();
                }
                else
                {
                    parent.InitDatabaseSchema();
                }

            }
        }
    }
}