
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Lang;
using Java.Util;

namespace App1.Droid.Workspace.Database
{
    [Activity(Label = "Edit Workspace")]
    public class WorkspaceSchemaEditActivity : AppCompatActivity, View.IOnClickListener, IDialogInterfaceOnCancelListener, IDialogInterfaceOnClickListener
    {
        DatabaseReference schemaRef;
        private FirebaseDatabase database;

        private bool consume_send_name;
        private bool consume_change_name;

        readonly NameChangeListener namelistener;
        readonly SchemaListener schemaListener;

        RelativeLayout removeButton;
        EditText nameText;

        public WorkspaceSchemaEditActivity() : base()
        {
            namelistener = new NameChangeListener(this);
            schemaListener = new SchemaListener(this);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            SetContentView(Resource.Layout.WorkspaceSchemaEditlayout);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if (null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                //SupportActionBar.SetTitle(Resource.String.add_new_database_string);
            }

            database = FirebaseDatabase.Instance;
            schemaRef = database.GetReferenceFromUrl(Intent.GetStringExtra("selfSchema"));

            schemaRef.AddChildEventListener(schemaListener);

            removeButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutDelete);
            removeButton.SetOnClickListener(this);

            nameText = FindViewById<EditText>(Resource.Id.editTextWorkspaceName);
            nameText.AddTextChangedListener(namelistener);

        }

        protected override void OnDestroy()
        {
            UnbindListeners();
            base.OnDestroy();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutDelete:
                    Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);

                    builder.SetMessage(Resources.GetString(Resource.String.remove_workspace_string_question));
                    builder.SetNegativeButton("Cancel", this);
                    builder.SetPositiveButton("Delete", this);

                    builder.Create().Show();
                    break;
            }
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

        private void DeleteData()
        {
            schemaRef.Child("name").RemoveValue();
            this.Finish();
        }

        private void UnbindListeners()
        {
            schemaRef.RemoveEventListener(schemaListener);
            nameText.RemoveTextChangedListener(namelistener);
        }

        private void FinalizeSelf()
        {
            this.Finish();
        }

        private void UserChangedName()
        {
            consume_change_name = true;
            schemaRef.Child("name").SetValue(nameText.Text);
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
            set
            {
                if (consume_change_name)
                {
                    consume_change_name = false;
                    return;
                }
                consume_send_name = true;
                nameText.Text = value;
            }
        }

        class NameChangeListener : Java.Lang.Object, ITextWatcher
        {

            Timer timer;
            readonly WorkspaceSchemaEditActivity parent;

            public NameChangeListener(WorkspaceSchemaEditActivity parent)
            {
                this.parent = parent;
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

            public void AfterTextChanged(IEditable s)
            {
                if (parent.consume_send_name)
                {
                    parent.consume_send_name = false;
                    return;
                }

                timer = new Timer();
                timer.Schedule(new UpdatenameTask(this.parent), 3000);
            }
        }

        class UpdatenameTask : TimerTask
        {
            WorkspaceSchemaEditActivity updated;

            public UpdatenameTask(WorkspaceSchemaEditActivity updated)
            {
                this.updated = updated;
            }

            public override void Run()
            {
                updated.UserChangedName();
                this.Cancel();
            }
        }

        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            private readonly WorkspaceSchemaEditActivity parent;

            public SchemaListener(WorkspaceSchemaEditActivity workspaceModel)
            {
                parent = workspaceModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace Schema Error:" + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
                        break;
                }
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
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
                }
            }
        }
    }
}