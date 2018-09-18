
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Lang;
using Java.Util;
using System;
using static Android.Views.View;

namespace App1.Droid.Workspace.Database
{
    [Activity(Label = "DatabaseEditActivity", Theme = "@style/ColoredActivityTheme")]
    public class DatabaseEditActivity : AppCompatActivity, IOnClickListener, IDialogInterfaceOnClickListener
    {
        DatabaseReference databaseDataRef;
        DatabaseReference databaseSchemaRef;
        private FirebaseDatabase database;

        private NameChangeListener nameListener;
        private SchemaListener databaseListener;

        private EditText nameText;
        LinearLayout imagePicker, colorPicker;
        ImageView selectedDrawable;
        ImageView selectedColor;
        Android.Support.V7.Widget.Toolbar toolbar;

        float imageSize;
        private int selectedcolor;
        private int selecteddrawable;

        public DatabaseEditActivity()
        {
            nameListener = new NameChangeListener(this);
            databaseListener = new SchemaListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.DatabaseSchemaEditActivity);

            database = FirebaseDatabase.Instance;
            databaseSchemaRef = database.GetReferenceFromUrl(Intent.GetStringExtra("databaseSchema"));
            databaseDataRef = database.GetReferenceFromUrl(Intent.GetStringExtra("databaseDataPath"));

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                toolbar.SetTitleTextColor(Color.White.ToArgb());
            }

            nameText = FindViewById<EditText>(Resource.Id.editTextDatabaseName);
            nameText.AddTextChangedListener(nameListener);

            RelativeLayout
                deleteButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutDelete);
            deleteButton.SetOnClickListener(this);

            imagePicker = FindViewById<LinearLayout>(Resource.Id.linearLayoutImagePicker);
            colorPicker = FindViewById<LinearLayout>(Resource.Id.linearlayoutColorPicker);

            imageSize = Resources.GetDimension(Resource.Dimension.workplace_image_size);

            foreach (Color c in DatabaseController.colors)
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
            selectedcolor = -1;

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
            selecteddrawable = -1; 

            databaseSchemaRef.AddChildEventListener(databaseListener);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SaveOnlyMenu, menu);
            for (int i = 0; i < menu.Size(); ++i)
            {
                IMenuItem item = menu.GetItem(i);
                string str = item.TitleFormatted.ToString();
                SpannableString s = new SpannableString(str);
                s.SetSpan(new Android.Text.Style.ForegroundColorSpan(Color.White), 0, str.Length, 0);

                item.SetTitle(s);
            }

            return base.OnPrepareOptionsMenu(menu);
        }

        protected override void OnDestroy()
        {
            UnbindListeners();
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                case Resource.Id.action_save:
                    if(selecteddrawable > -1)
                    {
                        databaseSchemaRef.Child("icon").SetValue(selecteddrawable);
                    }
                    if(selectedcolor > -1)
                    {
                        databaseSchemaRef.Child("color").SetValue(selectedcolor);
                    }

                    databaseSchemaRef.Child("name").SetValue(nameText.Text);
                    this.Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void UnbindListeners()
        {
            databaseSchemaRef.RemoveEventListener(databaseListener);
        }

        private void FinalizeSelf()
        {
            Finish();
        }

        public void OnClick(View v)
        {
            GradientDrawable backgroundShape;
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutDelete:
                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);

                    builder.SetMessage(Resources.GetString(Resource.String.remove_database_string_question));
                    builder.SetNegativeButton("Cancel", this);
                    builder.SetPositiveButton("Delete", this);

                    builder.Create().Show();
                    break;
                case Resource.Id.linearLayoutImagePicker:

                    if(selectedDrawable != null)
                    {
                        selectedDrawable.SetBackgroundColor(Color.Transparent);
                        selectedDrawable.ClearColorFilter();
                    }
                    

                    selectedDrawable = (ImageView)v;
                    selecteddrawable = imagePicker.IndexOfChild(selectedDrawable);
                    backgroundShape = new GradientDrawable();
                    if (selectedcolor > -1)
                    {
                        backgroundShape.SetColor(DatabaseController.colors[selectedcolor].ToArgb());
                    }
                    else
                    {
                        backgroundShape.SetColor(Color.Black);
                    }

                    backgroundShape.SetCornerRadius(imageSize / 8);

                    selectedDrawable.Background = backgroundShape;
                    selectedDrawable.SetColorFilter(Color.White);
                    
                    break;
                case Resource.Id.linearlayoutColorPicker:
                    if(selectedColor != null)
                    {
                        selectedColor.SetColorFilter(DatabaseController.colors[selectedcolor]);
                    }
                    
                    selectedColor = (ImageView)v;
                    selectedcolor = colorPicker.IndexOfChild(selectedColor);
                    selectedColor.SetImageResource(Resource.Drawable.icons_checkmark);
                    selectedColor.SetColorFilter(Color.White);

                    backgroundShape = new GradientDrawable();
                    if(selectedcolor > -1)
                    {
                        backgroundShape.SetColor(DatabaseController.colors[selectedcolor].ToArgb());
                    }
                    else
                    {
                        backgroundShape.SetColor(Color.Black);
                    }
                    

                    backgroundShape.SetCornerRadius(imageSize / 8);

                    if (selectedDrawable != null)
                    {
                        selectedDrawable.Background = backgroundShape;
                    }
                    if (selectedcolor > -1)
                    {
                        SupportActionBar.SetBackgroundDrawable(new ColorDrawable(DatabaseController.colors[selectedcolor]));
                    }
                    else
                    {
                        SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.Black));
                    }
                    break;
            }
        }


        public void OnClick(IDialogInterface dialog, int which)
        {
            switch ((DialogButtonType)which)
            {
                case DialogButtonType.Positive:
                    DeleteData();
                    this.Finish();
                    break;
                default:
                    dialog.Dismiss();
                break;

            }
        }

        private void DeleteData()
        {
            UnbindListeners();
            databaseSchemaRef.RemoveValue();
            databaseDataRef.RemoveValue();
        }

        public string Name {
            get {
                return nameText.Text;
            }
            set
            {
                nameText.Text = value;
            }
        }
        public int BackColor
        {
            get
            {
                return selectedcolor;
            }
            set
            {
                if(value > -1)
                {
                    int val = value % DatabaseController.colors.Length;
                    if (selectedcolor == val)
                    {
                        return;
                    }
                    View v = colorPicker.GetChildAt(val);
                    v.CallOnClick();
                }
                
            }
        }
        public int Icon {
            get
            {
                return selecteddrawable;
            }
            set
            {
                if(value > -1)
                {
                    int val = value % DatabaseController.imageResources.Length;
                    if (selecteddrawable == val)
                    {
                        return;
                    }
                    View v = imagePicker.GetChildAt(val);
                    v.CallOnClick();
                }
            }
        }

        class NameChangeListener : Java.Lang.Object, ITextWatcher
        {
            DatabaseEditActivity parent;

            public NameChangeListener(DatabaseEditActivity parent)
            {
                this.parent = parent;
            }

            public void AfterTextChanged(IEditable s)
            {
                parent.SupportActionBar.Title = s.ToString();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {

            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
            }
        }

        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            private readonly DatabaseEditActivity parent;

            public SchemaListener(DatabaseEditActivity parent)
            {
                this.parent = parent;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace Schema Error:" + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if (!snapshot.Exists())
                {
                    return;
                }
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
                        break;
                    case "color":
                        parent.BackColor = int.Parse(snapshot.Value.ToString());
                        break;
                    case "icon":
                        parent.Icon = int.Parse(snapshot.Value.ToString());
                        break;
                }
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                if (!snapshot.Exists())
                {
                    return;
                }
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
                        break;
                    case "color":
                        parent.BackColor = int.Parse(snapshot.Value.ToString());
                        break;
                    case "icon":
                        parent.Icon = int.Parse(snapshot.Value.ToString());
                        break;
                }
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                //??
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                switch (snapshot.Key)
                {
                    case "name":
                        parent.FinalizeSelf();
                        break;
                    case "color":
                        parent.BackColor = -1;
                        break;
                    case "icon":
                        parent.Icon = -1;
                        break;
                }
            }
        }
    }
}