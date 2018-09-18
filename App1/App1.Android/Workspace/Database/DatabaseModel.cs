using Android.App;
using Android.Content;
using App1.Droid.Table.Models;
using App1.Droid.Workspace.Database;
using Firebase.Database;
using System;
using System.Collections.Generic;

namespace App1.Droid
{
    public class DatabaseModel
    {
        //string databaseId;
        bool consume_name_update;
        string name;

        readonly DatabaseReference tablesSchemaReference;
        readonly DatabaseReference tableDataReference;
        readonly DatabaseReference selfSchemaRef;
        readonly SchemaListener schemaListener;
        readonly NewTableIdListener newIdListener;
        readonly TableNamesListener tableSchemaListener;

        readonly DatabaseController controller;
        TableModel currentModel;

        readonly List<string> tableNames;
        readonly List<string> tableIds;

        bool initScreen;

        string newTableName;

        public DatabaseModel(DatabaseReference dataPlaceReference, DatabaseReference schemaReference)
        {
            selfSchemaRef = schemaReference.Ref;
            tablesSchemaReference = dataPlaceReference.Child("table_data");
            tableDataReference = dataPlaceReference.Child("tables");

            schemaListener = new SchemaListener(this);
            newIdListener = new NewTableIdListener(this);
            tableSchemaListener = new TableNamesListener(this);

            controller = new DatabaseController(this);

            tableNames = new List<string>();
            tableIds = new List<string>();

            currentModel = new TableModel();

            consume_name_update = true;
            selfSchemaRef.AddChildEventListener(schemaListener);
            tablesSchemaReference.Child("table_names").AddChildEventListener(tableSchemaListener);
            tablesSchemaReference.Child("new_table_id").AddValueEventListener(newIdListener);
        }

        public string Id { get; }
        public string Name {
            get => name;
            set
            {
                if (consume_name_update)
                {
                    consume_name_update = false;
                    return;
                }

                name = value;
                controller.NotifyNameChanged(name);
            }
        }

        public List<string> TableNames { get => tableNames; }
        public List<string> TableIds { get => tableIds; }

        public void SelectTable(int v)
        {
            //currentModel.DeleteSelf(false);
            //currentModel = new TableModel();
            currentModel.SetTable(tableDataReference.Child(tableIds[v]), tablesSchemaReference.Child("table_names").Child(tableIds[v]));
            //controller.NotifymodelChanged(currentModel);
        }

        public DatabaseView GetView(Activity context)
        {
            return new DatabaseView(controller, currentModel.GetView(context), context);
        }

        void Unbindlisteners()
        {
            selfSchemaRef.RemoveEventListener(schemaListener);
            tablesSchemaReference.Child("table_names").RemoveEventListener(tableSchemaListener);
            tablesSchemaReference.Child("new_table_id").RemoveEventListener(newIdListener);
        }

        internal void DeleteSelf(bool eraseData)
        {
            Unbindlisteners();

            currentModel.DeleteSelf(false);

            controller.NotifyDeleteCommencing();
            //in case workspace is deleted no need to erase data in database
            if (eraseData)
            {
                selfSchemaRef.RemoveValue();
                tableDataReference.RemoveValue();
                tablesSchemaReference.RemoveValue();
            }
        }

        public void InitTableSchema()
        {
            tablesSchemaReference.Child("new_table_id").SetValue("table_1");
        }

        public void AddNewTable()
        {
            tablesSchemaReference.Child("table_names").Child(newTableName).SetValue("New Table");

            tableDataReference.Child(newTableName).Child("column_data").Child("new_column_id").SetValue("column_2");
            tableDataReference.Child(newTableName).Child("row_data").Child("new_row_id").SetValue("row_1");
            tableDataReference.Child(newTableName).Child("columns").Child("column_1").Child("type").SetValue("TEXT");
            tableDataReference.Child(newTableName).Child("columns").Child("column_1").Child("name").SetValue("Primary element");
            tableDataReference.Child(newTableName).Child("columns").Child("column_1").Child("primary").SetValue("true");

            String[] keyParts = newTableName.Split('_');
            String newKey = keyParts[0] + "_" + (Int64.Parse(keyParts[1]) + 1);

            tablesSchemaReference.Child("new_table_id").SetValue(newKey);


        }

        private void TableUpdated(string table_name, int index_in)
        {
            TableNames[index_in] = table_name;
         
            controller.NotifyTableNameUpdated(index_in, table_name);
        }
        
        private void TableAdded(string tableId, string tableName, int index_in)
        {
            TableIds.Insert(index_in, tableId);
            TableNames.Insert(index_in, tableName);

            if (!currentModel.TableSet())
            {
                SelectTable(index_in);
            }

            controller.NotifyTableNameAdded(tableName, index_in);
        }

        private void TableDeleted(int index_del)
        {
            TableNames.RemoveAt(index_del);
            TableIds.RemoveAt(index_del);
              
            controller.NotifyTableNameDeleted(index_del);
        }
        
        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            readonly DatabaseModel parent;

            public SchemaListener(DatabaseModel databaseModel)
            {
                parent = databaseModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Database Schema Error:" + error.Message);
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
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                //??
            }
        }

        class NewTableIdListener : Java.Lang.Object, IValueEventListener
        {
            readonly DatabaseModel parent;

            public NewTableIdListener(DatabaseModel databaseModel)
            {
                this.parent = databaseModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace New Database Id Error:" + error.Message);
            }
            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot.Exists())
                {
                    parent.newTableName = snapshot.Value.ToString();
                }
                else
                {
                    parent.InitTableSchema();
                }
            }
        }

        class TableNamesListener : Java.Lang.Object, IChildEventListener
        {
            readonly DatabaseModel parent;

            public TableNamesListener(DatabaseModel databaseModel)
            {
                this.parent = databaseModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Database table names Eroor: " + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if(snapshot.Exists())
                {
                    string tableId = snapshot.Key;
                    string tableName = snapshot.Value.ToString();

                    int index_in = parent.TableIds.IndexOf(previousChildName);
                    ++index_in;

                    parent.TableAdded(tableId, tableName, index_in);
                    if (parent.initScreen)
                    {
                        parent.SelectTable(index_in);
                        parent.initScreen = false;
                    }
                }
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                string tableId = snapshot.Key;
                string tableName = snapshot.Value.ToString();

                int index_U = parent.TableIds.IndexOf(tableId);

                if (index_U > -1)
                {
                    parent.TableUpdated(tableName, index_U);
                }
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                //??
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                int index_del = parent.TableIds.IndexOf(snapshot.Key);

                parent.TableDeleted(index_del);
            }
        }
    }
}