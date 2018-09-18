using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table;
using App1.Droid.Table.Models;
using App1.Droid.Table.Models.Columns;
using Firebase.Database;
using Java.IO;
using System;
using System.Collections.Generic;
using static Android.Views.View;

namespace App1.Droid.Workspace.Database.Table
{
    [Activity(Label = "ItemEditActivity", Theme = "@style/ColoredActivityTheme")]
    public class ItemEditActivity : AppCompatActivity, IOnClickListener, IDialogInterfaceOnClickListener, IImageProvider
    {
        DatabaseReference rowDataRef;
        DatabaseReference rowSchemaRef;
        DatabaseReference colorRef;
        private FirebaseDatabase database;

        RowModel row;
        List<ColumnModel> columns;

        RelativeLayout deleteButton;
        RelativeLayout editButton;
        LinearLayout editContentLayout;

        ColorListener colorListener;
        ColumnChangeListener columnListener;
        Android.Support.V7.Widget.Toolbar toolbar;

        IImageReciever imageReciever;
        const int LOAD_IMAGE = 11111;

        public ItemEditActivity()
        {
            columnListener = new ColumnChangeListener(this);
            columns = new List<ColumnModel>();
            colorListener = new ColorListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ItemEditActivity);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                toolbar.SetBackgroundColor(Color.Gray);
                toolbar.SetTitleTextColor(Color.White);
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetTitle(Resource.String.item_edit_string);
            }

            database = FirebaseDatabase.Instance;
            rowDataRef = database.GetReferenceFromUrl(Intent.GetStringExtra("rowReference"));
            rowSchemaRef = database.GetReferenceFromUrl(Intent.GetStringExtra("tableSource")).Child("columns");
            rowSchemaRef.AddChildEventListener(columnListener);

            colorRef = database.GetReferenceFromUrl(Intent.GetStringExtra("colorSource"));


            deleteButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutDelete);
            editButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutCustomize);

            editContentLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutEditContent);

            if (colorRef != null)
            {
                colorRef.AddValueEventListener(colorListener);
            }

            deleteButton.SetOnClickListener(this);
            editButton.SetOnClickListener(this);

            row = new RowModel(columns, rowDataRef);

            editContentLayout.AddView(row.GetEditView(this).GetView(), 0);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SaveOnlyMenu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                case Resource.Id.action_save:
                    row.Save();
                    this.Finish();
                    return true;

            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnDestroy()
        {
            try
            {
                UnbindListeners();
            }catch(ArgumentException e)
            {
                System.Console.WriteLine("Database was bug-disposed again " + e.Message);
            }
            
            base.OnDestroy();
        }

        private void UnbindListeners()
        {            
            row.UnbindListeners();
            rowSchemaRef.RemoveEventListener(columnListener);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutDelete:
                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);

                    builder.SetMessage(Resources.GetString(Resource.String.remove_record_string_question));
                    builder.SetNegativeButton("Cancel", this);
                    builder.SetPositiveButton("Delete", this);

                    builder.Create().Show();
                    break;
                case Resource.Id.relativeLayoutCustomize:
                    Intent intent = new Intent(this, typeof(ColumnsEditActivity));
                    intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
                    intent.PutExtra("tableSource", rowSchemaRef.Parent.ToString());
                    intent.PutExtra("colorSource", colorRef.ToString());
                    StartActivity(intent);
                    break;
            }
        }

        private void DeleteData()
        {
            rowDataRef.RemoveValue();
            this.Finish();
        }

        public ColumnModel GetColumnCell(DataSnapshot cell)
        {
            String type = cell.Child("type").Value.ToString();
            switch (type)
            {
                case "TEXT":
                    return new ColumnModelText(cell);
                case "NUMBER":
                    return new ColumnModelNumber(cell);
                case "CHOICE":
                    return new ColumnModelChoice(cell);
                case "IMAGE":
                    return new ColumnModelImage(cell);
                case "DATE":
                    return new ColumnModelDate(cell);

            }
            return new ColumnModelText();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch ((DialogButtonType)which)
            {
                case DialogButtonType.Positive:
                    DeleteData();
                    break;
                default:
                    dialog.Dismiss();
                    break;

            }
        }

        
        private void DeleteColumn(int columnPosition)
        {
            columns.RemoveAt(columnPosition);
            if (row != null)
            {
                row.DeletedColumn(columnPosition);
            }
        }

        private void AddColumn(DataSnapshot dataSnapshot, int columnPosition)
        {
            if (!dataSnapshot.Exists()) return;

            ColumnModel column = GetColumnCell(dataSnapshot);
            column.ColumnId = dataSnapshot.Key;

            columns.Insert(columnPosition, column);
            if(row != null)
            {
                row.AddedColumn(column, columnPosition);
            }
            
        }

        private void UpdateColumn(DataSnapshot columnData, int index)
        {
            String type = columnData.Child("type").Value.ToString();
            if (type != columns[index].Type)
            {
                ColumnModel column = GetColumnCell(columnData);
                column.ColumnId = columnData.Key;

                columns[index] = column;
                if (row != null)
                {
                    row.UpdatedColumn(column, index);
                }
            }
            else
            {
                columns[index].SetData(columnData);
            }
        }

        public void LoadImage(IImageReciever requester)
        {
            imageReciever = requester;

            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), LOAD_IMAGE);
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

        class ColorListener : Java.Lang.Object, IValueEventListener
        {
            ItemEditActivity parentTable;
            public ColorListener(ItemEditActivity parent)
            {
                parentTable = parent;
            }

            public void OnCancelled(DatabaseError error)
            {
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    int val = int.Parse(snapshot.Value.ToString()) % DatabaseController.colors.Length;

                    parentTable.SupportActionBar.SetBackgroundDrawable(new ColorDrawable(DatabaseController.colors[val]));
                }
            }
        }

        class ColumnChangeListener : Java.Lang.Object, IChildEventListener
        {
            ItemEditActivity parentTable;

            public ColumnChangeListener(ItemEditActivity parent)
            {
                parentTable = parent;
            }
            public void OnChildAdded(DataSnapshot dataSnapshot, string previousChildName)
            {

                int columnPosition = -1;

                if (previousChildName == null)
                {
                    columnPosition = parentTable.columns.Count;
                }
                else
                {
                    for (int i = 0; i < parentTable.columns.Count; ++i)
                    {
                        if (parentTable.columns[i].ColumnId.Equals(previousChildName))
                        {
                            columnPosition = i + 1;
                            break;
                        }
                    }
                }
                if (columnPosition >= 0)
                {
                    parentTable.AddColumn(dataSnapshot, columnPosition);

                }
            }

            public void OnChildChanged(DataSnapshot dataSnapshot, String previousChildName)
            {

                int columnPosition = -1;
                for (int i = 0; i < parentTable.columns.Count; ++i)
                {
                    if (parentTable.columns[i].ColumnId.Equals(dataSnapshot.Key))
                    {
                        columnPosition = i;
                        break;
                    }
                }

                if (columnPosition >= 0)
                {
                    parentTable.UpdateColumn(dataSnapshot, columnPosition);
                }

            }

            public void OnChildRemoved(DataSnapshot dataSnapshot)
            {
                String columnId = dataSnapshot.Key;

                int columnPosition = -1;

                for (int i = 0; i < parentTable.columns.Count; ++i)
                {
                    if (parentTable.columns[i].ColumnId.Equals(columnId))
                    {
                        columnPosition = i;
                        break;
                    }
                }
                parentTable.DeleteColumn(columnPosition);
            }

            public void OnChildMoved(DataSnapshot dataSnapshot, String s)
            {
                //will not happen?
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                System.Console.WriteLine("column listener failed" + databaseError.Message);
            }
        }
        
    }
}