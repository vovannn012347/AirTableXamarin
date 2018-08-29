using System;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Firebase.Database;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using App1.Droid.Table.Models;
using App1.Droid.Table;
using Android.Support.Design.Widget;
using Android.Support.Constraints;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using System.IO;
using System.Text.RegularExpressions;
using Android.Support.V4.View;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
        Android.Support.Design.Widget.NavigationView.IOnNavigationItemSelectedListener,
        IChildEventListener, IImageProvider
    {
        const int LOAD_IMAGE = 11111;

        //Firebase.FirebaseApp app;
        FirebaseDatabase database;
        DatabaseReference mTableReference;
        DatabaseReference mTableNamesReference;

        IMenu menuGroup;
        DrawerLayout mDrawer;
        ConstraintLayout mainLayout;

        int currentTableId;
        Dictionary<int, string> tables;
        Dictionary<int, string> tableNames;

        TableModel currentTable;

        IImageReciever updatedCell;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.testlayout);


            mainLayout = FindViewById<ConstraintLayout>(Resource.Id.main_layout);

            NavigationView tableSelectorMenu = FindViewById<NavigationView>(Resource.Id.nav_view);
            tableSelectorMenu.SetNavigationItemSelectedListener(this);
            mDrawer = FindViewById<DrawerLayout>(Resource.Id.drawerMenu);
            menuGroup = tableSelectorMenu.Menu;

            database = FirebaseDatabase.Instance;//.setPersistenceEnabled(true);
            tables = new Dictionary<int, string>();
            tableNames = new Dictionary<int, string>();

            currentTable = new TableModel();

            currentTableId = -1;

            loadTableData();
            loadMainScreen();

            //global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected void loadMainScreen()
        {


            if (currentTableId != -1)
            {
                mainLayout.AddView(currentTable.GetView(this).GetView());

                selectTable(currentTableId);
            }
            else
            {
                while (mainLayout.GetChildAt(0) != null)
                {
                    mainLayout.RemoveView(mainLayout.GetChildAt(0));
                }

                this.LayoutInflater.Inflate(Resource.Layout.tutorial_layout, mainLayout, true);
            }
        }

        protected void loadTableData()
        {
            
            mTableNamesReference = database.Reference
                    .Child("data").Child("table_names");

            //handle table name changes
            mTableNamesReference.AddChildEventListener(this);

        }

        protected void selectTable(int tableId)
        {

            if (tableId == currentTableId)
            {
                return;
            }

            if (currentTableId != -1)
            {
                currentTable.UnbindListeners();
            }
            else
            {
                while (mainLayout.GetChildAt(0) != null)
                {
                    mainLayout.RemoveView(mainLayout.GetChildAt(0));
                }

                mainLayout.AddView(currentTable.GetView(this).GetView());
            }

            currentTableId = tableId;
            mTableReference =
                    database.Reference
                            .Child("tables")
                            .Child(tables.GetValueOrDefault(tableId));

            currentTable.SetTable(mTableReference, mTableNamesReference.Child(tables.GetValueOrDefault(tableId)));
        }

        public void LoadImage(IImageReciever imageCell)
        {
            updatedCell = imageCell;

            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), LOAD_IMAGE);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            Console.WriteLine("\n"+item.ItemId+"\n");

            if (item.ItemId == Resource.Id.new_table_menu)
            {

            }
            else
            {
                selectTable(item.ItemId);
            }

            mDrawer.CloseDrawer(GravityCompat.Start);

            return true;
        }

        public void OnChildAdded(DataSnapshot dataSnapshot, String previousChildName)
        {
            tables.Add(dataSnapshot.Key.GetHashCode(), dataSnapshot.Key.ToString());
            tableNames.Add(dataSnapshot.Key.GetHashCode(), dataSnapshot.Value.ToString());

            menuGroup.Add(Resource.Id.tableSelection, dataSnapshot.Key.GetHashCode(), Menu.None, dataSnapshot.Value.ToString());
        }
        public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
        {
            IMenuItem item = menuGroup.FindItem(dataSnapshot.Key.GetHashCode());

            tableNames[dataSnapshot.Key.GetHashCode()] = dataSnapshot.Value.ToString();
            if (item != null)
            {
                item.SetTitle(dataSnapshot.Value.ToString());
            }
        }
        public void OnChildRemoved(DataSnapshot dataSnapshot)
        {
            int key = dataSnapshot.Key.GetHashCode();

            tables.Remove(key);
            tableNames.Remove(key);
            menuGroup.RemoveItem(key);
        }
        public void OnChildMoved(DataSnapshot dataSnapshot, String s)
        {
            //dont care
        }
        public void OnCancelled(DatabaseError databaseError)
        {
            //maybe should print something
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == LOAD_IMAGE && resultCode == Result.Ok && updatedCell != null)
            {

                Android.Net.Uri imageUri = data.Data;
                Bitmap bitmap;
                try
                {
                    bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, imageUri);
                }
                catch (IOException e)
                {
                    Console.WriteLine("LOAD ERROR " + e.Message);
                    return;
                }

                String namepart = Regex.Replace(imageUri.EncodedPath, "[/\\:.$% -+*?]", "");

                updatedCell.receiveImage(bitmap, namepart + imageUri.GetHashCode());
                updatedCell = null;
            }
        }

        /*
        class NewTableListener : Java.Lang.Object, IValueEventListener
        {

            public void OnCancelled(DatabaseError error)
            {
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
            }
        }
        */
    }
}