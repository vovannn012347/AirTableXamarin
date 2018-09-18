using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table;
using App1.Droid.Workspace.Database;
using Firebase.Database;
using Firebase.Storage;
using Java.IO;

namespace App1.Droid
{
    [Activity(Label = "Name Placeholder", Theme = "@style/ColoredActivityTheme")]
    public class DatabaseActivity : AppCompatActivity, IImageProvider
    {
        FirebaseDatabase database;
        FirebaseStorage sorage;

        DatabaseReference databaseDataReference;
        DatabaseReference databaseSchemaReference;
        readonly SchemaListener schemaListener;
        IImageReciever imageReciever;

        DatabaseModel model;
        DatabaseView view;

        Android.Support.V7.Widget.Toolbar toolbar;
        FrameLayout mainLayout;

        Color color;
        const int LOAD_IMAGE = 11111;

        public DatabaseActivity()
        {
            schemaListener = new SchemaListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            SetContentView(Resource.Layout.DatabaseActivity);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                toolbar.SetTitleTextColor(Color.White);
            }

            database = FirebaseDatabase.Instance;

            databaseSchemaReference = database.GetReferenceFromUrl(Intent.GetStringExtra("databaseSchema"));
            databaseDataReference = database.GetReferenceFromUrl(Intent.GetStringExtra("databasePath"));
            sorage = FirebaseStorage.Instance;

            mainLayout = FindViewById<FrameLayout>(Resource.Id.tableFrame);

            model = new DatabaseModel(databaseDataReference, databaseSchemaReference);
            view = model.GetView(this);

            mainLayout.AddView(view.GetView());
            databaseSchemaReference.AddChildEventListener(schemaListener);
        }

        private void UnbindListeners()
        {
            databaseSchemaReference.RemoveEventListener(schemaListener);
        }

        protected override void OnDestroy()
        {
            try
            {
                UnbindListeners();
            }
            catch (ArgumentException e)
            {
                System.Console.WriteLine("Database bug-disposed" + e.Message);
            }
            base.OnDestroy();
        }

        private void DeleteSelf()
        {
            this.Finish();
        }

        private void LaunchDatabaseEditActivity()
        {
            Intent intent = new Intent(this, typeof(DatabaseEditActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("databaseSchema", databaseSchemaReference.ToString());
            intent.PutExtra("databaseDataPath", databaseDataReference.ToString());
            StartActivity(intent);
        }

        public void LoadImage(IImageReciever requester)
        {
            imageReciever = requester;

            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), LOAD_IMAGE);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.DatabaseActivityMenu, menu);

            return base.OnCreateOptionsMenu(menu);

            //var searchItem = menu.FindItem(Resource.Id.action_search);

            //searchView = searchItem.ActionProvider.JavaCast<Android.Widget.SearchView>();

            //searchView.QueryTextSubmit += (sender, args) =>
            //{
            //    Toast.MakeText(this, "You searched: " + args.Query, ToastLength.Short).Show();

            //};

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                case Resource.Id.menu_item_database_settings:
                    this.LaunchDatabaseEditActivity();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, Result result, Intent data)
        {
            base.OnActivityResult(requestCode, result, data);

            if (requestCode == LOAD_IMAGE && result == Result.Ok && imageReciever != null)
            {
                Android.Net.Uri imageUri = data.Data;
                Bitmap bitmap;
                try
                {
                    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(this.ContentResolver, imageUri);
                }
                catch (IOException e)
                {
                    System.Console.WriteLine("Activity Result Error " + e.Message + "\n");
                    return;
                }

                string encodedPath = imageUri.EncodedPath;

                imageReciever.receiveImage(bitmap,
                        "" +
                        System.Text.RegularExpressions.Regex.Replace(imageUri.EncodedPath, "[/\\:.$%-+*?]", "")
                        + imageUri.GetHashCode());
                imageReciever = null;
            }
        }

        public string Name
        {

            set
            {
                SupportActionBar.Title = value;
            }
        }

        public int BackColor
        {
            set
            {
                int val = value % DatabaseController.colors.Length;

                color = DatabaseController.colors[val];

                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(color));
                toolbar.SetTitleTextColor(Color.White.ToArgb());
                view.SetColor(new Color(
                    Convert.ToByte(color.R + (255 - color.R) * 0.3), 
                    Convert.ToByte(color.G + (255 - color.G) * 0.3), 
                    Convert.ToByte((255 - color.B) * 0.3 + color.B)));
            }
        }
        public int Icon
        {
            set
            {
                int val = value % DatabaseController.imageResources.Length;

                Drawable d = GetDrawable(DatabaseController.imageResources[val]);//, GravityFlags.Center, 50, 50);

                // Drawable d = scale);
                //d = DrawableCompat.Wrap(d);
                //DrawableCompat.SetTint(d, Color.White);

                SupportActionBar.SetIcon(d);
            }
        }

        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            private readonly DatabaseActivity parent;

            public SchemaListener(DatabaseActivity parent)
            {
                this.parent = parent;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace Schema Error:" + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if(snapshot.Exists())
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
                if (snapshot.Exists())
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
                        parent.DeleteSelf();
                        break;
                    case "color":
                        parent.color = Color.Black;

                        parent.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(parent.color));
                        parent.toolbar.SetTitleTextColor(Color.White.ToArgb());
                        parent.view.SetColor(Color.Black);

                        break;
                    case "icon":
                        parent.Icon = 0;
                        break;
                }
            }
        }
    }
}
