using System;

using Android.App;
using Android.Content.PM;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.OS;
using Firebase.Database;
using System.Collections.Generic;
using Android.Views;
using Android.Text;
using Java.Lang;
using App1.Droid.Workspace;

namespace App1.Droid
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
        Android.Views.View.IOnClickListener
    {
        FirebaseDatabase database;
        readonly DatabaseReference workspaceDataReference;
        readonly DatabaseReference workspacesReference;

        readonly WorkspaceIdListener idListener;
        readonly WorkspaceDataListener dataListener;
        //readonly SearchTextChangedListener searchListener;

        readonly Dictionary<int, string> workspaceNames;
        readonly List<WorkspaceModel> workspaces;
        //readonly List<DatabaseListView> databasesFound;

        string newWorkSpaceId;

        LinearLayout mainLayout;
        RelativeLayout newWorkspaceButton;
        //LinearLayout searchLayout;
        Android.Support.V7.Widget.Toolbar toolbar;

        public MainActivity()
        {
            database = FirebaseDatabase.Instance;
            database.SetPersistenceEnabled(true);
            workspaceDataReference = database.Reference.Child("workspace_data");
            workspacesReference = database.Reference.Child("workspaces");

            idListener = new WorkspaceIdListener(this);
            dataListener = new WorkspaceDataListener(this);
            //searchListener = new SearchTextChangedListener(this);
            
            workspaceNames = new Dictionary<int, string>();
            workspaces = new List<WorkspaceModel>();
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OverridePendingTransition(Android.Resource.Animation.FadeIn, Android.Resource.Animation.FadeOut);
            SetContentView(Resource.Layout.MainLayout);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMain);
            if(null != toolbar)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetTitle(Resource.String.app_name);
                
            }

            mainLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutMain);

            newWorkspaceButton = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutNewItem);
            newWorkspaceButton.SetOnClickListener(this);

            
            workspaceDataReference.Child("new_workspace_id").AddValueEventListener(idListener);
            workspaceDataReference.Child("workspaces").AddChildEventListener(dataListener);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainActivityMenu, menu);

            return true;// base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //todo: implement

            return true;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutNewItem:
                    AddNewWorkspace();
                    break;
            }
        }

        void AddNewWorkspace()
        {
            workspaceDataReference.Child("workspaces").Child(newWorkSpaceId).Child("name").SetValue("New workspace");

            string[] keyParts = newWorkSpaceId.Split('_');
            string newKey = keyParts[0] + "_" + (Int64.Parse(keyParts[1]) + 1);

            workspaceDataReference.Child("new_workspace_id").SetValue(newKey);
        }

        new public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        /*
        private void Search(ICharSequence s)
        {
            string searched = s.ToString();
            
            foreach (DatabaseListView view in databasesFound)
            {
                view.DeleteSelf();
            }
            
            List<DatabaseListView> foundDatabases = new List<DatabaseListView>();

            foreach (WorkspaceModel model in workspaces){
                foundDatabases.AddRange(model.FindDatabases(searched));
            }

            foreach (DatabaseListView model in foundDatabases)
            {
                databasesFound.Add(model.GetView(this));
            }

            searchLayout.RemoveAllViews();
            foreach (DatabaseListView view in databasesFound)
            {
                searchLayout.AddView(view.GetView());
            }
        }

        private void Filter(ICharSequence s)
        {
            string filtered = s.ToString();
            
            foreach (DatabaseListView view in databasesFound)
            {
                if (!view.Filetered(filtered))
                {
                    searchLayout.RemoveView(view.GetView());
                    view.DeleteSelf();
                    view.Deleted = true;
                }
            }

            databasesFound.RemoveAll(v => v.Deleted);
        }*/
         /*
        class SearchTextChangedListener : Java.Lang.Object, ITextWatcher
        {
            readonly MainActivity parent;

            public SearchTextChangedListener(MainActivity parentActivity)
            {
                parent = parentActivity;
            }

            public void AfterTextChanged(IEditable s){}

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after){}

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                if (start == count)
                {
                    parent.Filter(s);
                }
                else
                {
                    parent.Search(s);
                }
            }
        }*/
        
        class WorkspaceIdListener : Java.Lang.Object, IValueEventListener
        {
            readonly MainActivity parent;

            public WorkspaceIdListener(MainActivity parentActivity)
            {
                parent = parentActivity;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("WorkspaceIdError :" + error.Message);
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                parent.newWorkSpaceId = snapshot.Value.ToString();
            }
        }

        class WorkspaceDataListener : Java.Lang.Object, IChildEventListener
        {
            readonly MainActivity parent;

            public WorkspaceDataListener(MainActivity parentActivity)
            {
                parent = parentActivity;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("WorkspaceDataError :" + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                WorkspaceModel created = new WorkspaceModel(parent.workspacesReference.Child(snapshot.Key), snapshot);

                int index_in = parent.workspaces.FindIndex(v => v.Id == previousChildName);

                ++index_in;
                parent.workspaces.Insert(index_in, created);
                parent.mainLayout.AddView(created.GetView(parent).GetView(), index_in);
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                //handled in workspaces itself 
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                //???
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                int deleted = parent.workspaces.FindIndex(v => v.Id == snapshot.Key);
                parent.mainLayout.RemoveViewAt(deleted);

                WorkspaceModel model = parent.workspaces[deleted];
                parent.workspaces.RemoveAt(deleted);
                model.DeleteSelf(false);
            }
        }
        
    }
}