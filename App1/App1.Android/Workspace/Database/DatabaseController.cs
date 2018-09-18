using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using App1.Droid.Table.Models;
using App1.Droid.Workspace.Database.Table;

namespace App1.Droid.Workspace.Database
{
    public class DatabaseController
    {
        public static readonly int[] imageResources = new int[]
{
            Resource.Drawable.icons_question_mark,
            Resource.Drawable.icons_beer,
            Resource.Drawable.icons_globe,
            Resource.Drawable.icons_literature,
            Resource.Drawable.icons_notification,
            Resource.Drawable.icons_screenshot,
            Resource.Drawable.icons_test_tube_filled
        };

        public static readonly Color[] colors = new Color[]
        {
            Color.DarkKhaki,
            Color.DarkGreen,
            Color.DarkOrange,
            Color.DarkRed,
            Color.DarkViolet,
            Color.DarkBlue
        };

        private DatabaseModel databaseModel;
        List<DatabaseView> views;

        public DatabaseController(DatabaseModel databaseModel)
        {
            this.databaseModel = databaseModel;
            views = new List<DatabaseView>();
        }

        internal void HookView(DatabaseView databaseView)
        {
            views.Add(databaseView);
            databaseView.Initialize(databaseModel.TableNames);
        }

        internal void UnhookView(DatabaseView databaseView)
        {
            views.Remove(databaseView);
        }

        internal void NotifyNameChanged(string name)
        {
            foreach(DatabaseView v in views)
            {
                v.NameChanged(name);
            }
        }
        
        internal void NotifyDeleteCommencing()
        {
            foreach (DatabaseView v in views)
            {
                v.DeleteCommencing();
            }

            databaseModel = null;
            views = null;
        }

        internal void NotifyTableNameUpdated(int index, string table_name)
        {
            foreach (DatabaseView v in views)
            {
                v.TableNameChanged(index, table_name);
            }
        }

        internal void NotifyTableNameAdded(string table_name, int index)
        {
            foreach (DatabaseView v in views)
            {
                v.TableNameAdded(index, table_name);
            }
        }

        internal void NotifyTableNameDeleted(int index_del)
        {
            foreach (DatabaseView v in views)
            {
                v.TableNameDeleted(index_del);
            }
        }
        
        internal void NewTableAdded()
        {
            databaseModel.AddNewTable();
        }
        
        internal void TableSelected(int v)
        {
            databaseModel.SelectTable(v);
        }

        internal void NotifymodelChanged(TableModel currentModel)
        {
            foreach(DatabaseView v in views)
            {
                v.UpdateTableView(currentModel);
            }
        }
    }
}