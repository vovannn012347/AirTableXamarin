using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Lang;

namespace App1.Droid.Workspace
{
    class WorkspaceModel
    {
        string modelId;

        bool consume_name_update;
       //bool consume_name_send;
        string name;

        readonly DatabaseReference selfSchemaRef;
        readonly DatabaseReference databasesDataRef;
        readonly DatabaseReference databasesSchemaRef;

        readonly DatabaseDataListener databaseDataListener;
        readonly SchemaListener schemaListener;
        readonly WorkspaceController controller;

        readonly List<string> databases;
        readonly List<DatabaseReference> databasesDataReferences;
        readonly List<DatabaseReference> databasesSchemaReferences;


        public WorkspaceModel(DatabaseReference baseData, DataSnapshot schemaData)
        {
            selfSchemaRef = schemaData.Ref;

            databasesSchemaRef = baseData.Child("database_data");
            databasesDataRef = baseData.Child("databases");

            schemaListener = new SchemaListener(this);
            databaseDataListener = new DatabaseDataListener(this);

            controller = new WorkspaceController(this);

            databases = new List<string>();
            databasesDataReferences = new List<DatabaseReference>();
            databasesSchemaReferences = new List<DatabaseReference>();
            
            modelId = schemaData.Key;
            name = schemaData.Child("name").Value.ToString();

            consume_name_update = true;
            selfSchemaRef.AddChildEventListener(schemaListener);
            databasesSchemaRef.Child("databases").AddChildEventListener(databaseDataListener);
        }
        
        public string Id {
            get {
                return modelId;
            }
        }

        public void UserSetName(string s)
        {
            name = s;
            controller.NotifyNameChanged(name);
            consume_name_update = true;
            selfSchemaRef.Child("name").SetValue(s);
        }

        public List<DatabaseReference> DatabaseSchemaReferences
        {
            get => databasesSchemaReferences;
        }

        public List<DatabaseReference> DatabaseDataReferences
        {
            get => databasesDataReferences;
        }

        public DatabaseReference DatabasesSchema
        {
            get => databasesSchemaRef;
        }

        public DatabaseReference SelfSchema
        {
            get => selfSchemaRef;
        }


        public string Name {
            get => name;
            set {
                if (consume_name_update)
                {
                    consume_name_update = false;
                    return;
                }
                
                name = value;
                controller.NotifyNameChanged(name);
            }
        }
        /*
        public List<DatabaseReference> FindDatabases(string searched)
        {
            List<string> found = new List<string>();

            foreach(string tested in databases)
            {
                if (tested.Contains(searched))
                {
                    found.Add(tested);
                }
            }

            List<DatabaseReference> ret = new List<DatabaseReference>();

            foreach (string tested in found)
            {
                ret.Add(databasesSchemaRef.Child);
            }

            return ret;
        }
        */
        void UnbindListeners()
        {
            selfSchemaRef.RemoveEventListener(schemaListener);
            databasesSchemaRef.Child("databases").RemoveEventListener(databaseDataListener);
        }

        public void DeleteSelf(bool eraseData)
        {
            UnbindListeners();

            //notify views
            controller.NotifyDeleteCommencing();

            //delete data at root node
            if (eraseData)
            {
                databasesSchemaRef.RemoveValue();
                databasesDataRef.RemoveValue();
                selfSchemaRef.RemoveValue();
            }
        }

        public WorkspaceView GetView(MainActivity parent)
        {
            return new WorkspaceView(controller, parent);
        }

        private void DatabaseAdded(string key, int index)
        {
            databases.Insert(index, key);
            databasesDataReferences.Insert(index, databasesDataRef.Child(key));
            databasesSchemaReferences.Insert(index, databasesSchemaRef.Child("databases").Child(key));

            controller.NotifyDatabaseAdded(databasesDataRef.Child(key), databasesSchemaRef.Child("databases").Child(key), index);
        }
                
        private void DatabaseRemoved(int index)
        {
            databases.RemoveAt(index);
            databasesDataReferences.RemoveAt(index);
            databasesSchemaReferences.RemoveAt(index);

            controller.NoifyDatabaseRemoved(index);
        }
        
        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            private readonly WorkspaceModel parent;

            public SchemaListener(WorkspaceModel workspaceModel)
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
                        parent.DeleteSelf(true);
                        break;
                }
            }
        }

        class DatabaseDataListener : Java.Lang.Object, IChildEventListener
        {
            private readonly WorkspaceModel parent;

            public DatabaseDataListener(WorkspaceModel workspaceModel)
            {
                this.parent = workspaceModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace Database Eroor: " + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if(snapshot.Exists())
                {
                    int index_in = parent.databases.FindIndex(v => v == previousChildName);
                    ++index_in;

                    parent.DatabaseAdded(snapshot.Key, index_in);
                }
                
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                //handled in database stub view
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                //??
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                int index_del = parent.databases.FindIndex(v => v == snapshot.Key);
                parent.DatabaseRemoved(index_del);
            }
        }
    }
}