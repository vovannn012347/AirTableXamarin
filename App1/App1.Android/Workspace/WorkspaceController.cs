using System.Collections.Generic;
using Android.App;
using Android.Content;
using App1.Droid.Workspace.Database;
using Firebase.Database;

namespace App1.Droid.Workspace
{
    class WorkspaceController
    {
        private WorkspaceModel workspaceModel;
        List<WorkspaceView> views;
        

        public WorkspaceController(WorkspaceModel workspaceModel)
        {
            this.workspaceModel = workspaceModel;
            views = new List<WorkspaceView>();
        }

        internal void HookView(WorkspaceView workspaceView)
        {
            views.Add(workspaceView);
            workspaceView.NameChanged(workspaceModel.Name);

            workspaceView.Init(workspaceModel.DatabaseSchemaReferences, workspaceModel.DatabaseDataReferences);
        }

        internal void NotifyNameChanged(string name)
        {
            foreach(WorkspaceView v in views)
            {
                v.NameChanged(name);
            }
        }

        internal void NotifyDeleteCommencing()
        {
            foreach (WorkspaceView v in views)
            {
                v.DeleteCommencing();
            }
        }

        internal void NotifyDatabaseAdded(DatabaseReference dataRef, DatabaseReference nameRef, int index)
        {
            foreach (WorkspaceView v in views)
            {
                v.DatabaseAdded(dataRef, nameRef, index);
            }
        }

        internal void NoifyDatabaseRemoved(int index_del)
        {
            foreach (WorkspaceView v in views)
            {
                v.DatabaseRemoved(index_del);
            }
        }

        internal void AddNewDatabase(Activity contextActivity)
        {
            Intent intent = new Intent(contextActivity, typeof(DatabaseAddActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("databasesSchema", workspaceModel.DatabasesSchema.ToString());
            contextActivity.StartActivity(intent);
        }

        internal void EditSchema(Activity contextActivity)
        {
            Intent intent = new Intent(contextActivity, typeof(WorkspaceSchemaEditActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("selfSchema", workspaceModel.SelfSchema.ToString());
            contextActivity.StartActivity(intent);
        }
    }
}