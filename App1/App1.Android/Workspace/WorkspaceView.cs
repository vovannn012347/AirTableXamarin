using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using static Android.Views.View;

namespace App1.Droid.Workspace
{
    class WorkspaceView : Java.Lang.Object, IOnClickListener
    {
        private WorkspaceController controller;
        private Activity context;

        View mainView;
        TextView nameView;
        ImageButton editButton;
        LinearLayout contentLayout;
        RelativeLayout newElementButton;
        List<DatabaseListView> databases;

        internal void Init(List<DatabaseReference> schemaReferences, List<DatabaseReference> dataReferences)
        {
            for(int i = 0; i < schemaReferences.Count; ++i)
            {
                databases.Add(new DatabaseListView(dataReferences[i], schemaReferences[i], context));
                contentLayout.AddView(databases[i].GetView());
            }
        }

        public WorkspaceView(WorkspaceController controller, Activity context)
        {
            this.controller = controller;
            this.context = context;
            databases = new List<DatabaseListView>();
            
            mainView = context.LayoutInflater.Inflate(Resource.Layout.WorkplaceLayout, null);

            nameView = mainView.FindViewById<TextView>(Resource.Id.WorkspaceName);
            editButton = mainView.FindViewById<ImageButton>(Resource.Id.editButton);
            contentLayout = mainView.FindViewById<LinearLayout>(Resource.Id.workspaceDatabases);
            newElementButton = mainView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutNewElement);

            controller.HookView(this);

            editButton.SetOnClickListener(this);
            newElementButton.SetOnClickListener(this);
        }

        internal void DatabaseAdded(DatabaseReference dataRef, DatabaseReference nameRef, int index)
        {
            databases.Insert(index, new DatabaseListView(dataRef, nameRef, context));
            contentLayout.AddView(databases[index].GetView(), index);
        }

        internal void NameChanged(string name)
        {
            nameView.Text = name;
        }

        internal void DatabaseRemoved(int index_del)
        {
            contentLayout.RemoveViewAt(index_del);
        }

        internal void DeleteCommencing()
        {
            controller = null;
            context = null;

            foreach(DatabaseListView v in databases)
            {
                v.DeleteSelf();
            }
        }

        internal View GetView()
        {
            return mainView;
        }
        
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.relativeLayoutNewElement:
                    controller.AddNewDatabase(context);
                    break;
                case Resource.Id.editButton:
                    controller.EditSchema(context);
                    break;
            }
        }
    }
}