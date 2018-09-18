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

namespace App1.Droid.Workspace.Database.Table
{
    [Activity(Label = "TableEditActivity", Theme = "@style/ColoredActivityTheme")]
    public class TableEditActivity : AppCompatActivity, View.IOnClickListener, IDialogInterfaceOnCancelListener, IDialogInterfaceOnClickListener
    {
        DatabaseReference tableDataRef;
        DatabaseReference tableNameRef;
        DatabaseReference colorRef;
        //DatabaseReference databaseSchemaRef;
        private FirebaseDatabase database;

        private bool consume_name_update;
        private bool consume_name_send;

        ColorListener colorListener;
        private NameChangeListener nameListener;
        private EditText nameText;
        int color;

        public TableEditActivity()
        {
            nameListener = new NameChangeListener(this);
            colorListener = new ColorListener(this);
           
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TableSchemaEditActivity);

            database = FirebaseDatabase.Instance;
            tableDataRef = database.GetReferenceFromUrl(Intent.GetStringExtra("tableSource"));
            tableNameRef = database.GetReferenceFromUrl(Intent.GetStringExtra("tableName"));
            colorRef = database.GetReferenceFromUrl(Intent.GetStringExtra("colorSource"));

            Android.Support.V7.Widget.Toolbar
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(color)));
            toolbar.SetTitleTextColor(Color.White);
            }

            nameText = FindViewById<EditText>(Resource.Id.editTextTableName);
            nameText.AddTextChangedListener(nameListener);
            tableNameRef.AddValueEventListener(nameListener);

            if (colorRef != null)
            {
                colorRef.AddValueEventListener(colorListener);
            }

            RelativeLayout
                deleteButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutDelete);
            deleteButton.SetOnClickListener(this);

            RelativeLayout
                editColumnsButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutCustomize);
            editColumnsButton.SetOnClickListener(this);
        }

        protected override void OnDestroy()
        {
            UnbindListeners();
            base.OnDestroy();
        }

        void UnbindListeners()
        {
            tableNameRef.RemoveEventListener(nameListener);
        }

        private void DeleteData()
        {
            tableNameRef.RemoveValue();
            tableDataRef.RemoveValue();
            this.Finish();
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

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutDelete:
                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);

                    builder.SetMessage(Resources.GetString(Resource.String.remove_table_string_question));
                    builder.SetNegativeButton("Cancel", this);
                    builder.SetPositiveButton("Delete", this);

                    builder.Create().Show();
                    break;
                case Resource.Id.relativeLayoutCustomize:
                    Intent intent = new Intent(this, typeof(ColumnsEditActivity));
                    intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
                    intent.PutExtra("tableSource", tableDataRef.ToString());
                    intent.PutExtra("colorSource", colorRef.ToString());
                    StartActivity(intent);
                    break;
            }
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            DeleteData();
        }

        public void OnCancel(IDialogInterface dialog)
        {
        }

        public string Name
        {
            get
            {
                return nameText.Text;
            }

            private set
            {
                nameText.Text = value;
                SupportActionBar.Title = value;
            }
        }

        private void UserChangedName()
        {
            consume_name_update = true;
            tableNameRef.SetValue(Name);
        }

        class ColorListener : Java.Lang.Object, IValueEventListener
        {
            TableEditActivity parentTable;
            public ColorListener(TableEditActivity parent)
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

        class NameChangeListener : Java.Lang.Object, IValueEventListener, ITextWatcher
        {
            TableEditActivity parent;
            Timer timer;

            public NameChangeListener(TableEditActivity parent)
            {
                this.parent = parent;
            }

            public void OnDataChange(DataSnapshot dataSnapshot)
            {
                if (!dataSnapshot.Exists()) return;
                if (parent.consume_name_update)
                {
                    parent.consume_name_update = false;
                    return;
                }
                parent.consume_name_send = true;
                parent.Name = dataSnapshot.Value.ToString();
            }

            public void OnCancelled(DatabaseError databaseError)
            {
                Console.WriteLine("column listener failed" + databaseError.Message);
            }

            public void AfterTextChanged(IEditable s)
            {
                if (parent.consume_name_send)
                {
                    parent.consume_name_send = false;
                    return;
                }

                timer = new Timer();
                timer.Schedule(new UpdatenameTask(this.parent), 3000);

                parent.SupportActionBar.Title = s.ToString();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
               
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                if (timer != null)
                {
                    timer.Cancel();
                    timer = null;
                }
            }
        }

        class UpdatenameTask : TimerTask
        {
            TableEditActivity updated;

            public UpdatenameTask(TableEditActivity updated)
            {
                this.updated = updated;
            }

            public override void Run()
            {
                updated.UserChangedName();
                this.Cancel();
            }
        }
    }
}