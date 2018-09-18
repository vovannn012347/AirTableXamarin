
using System.Collections.Generic;
using Android.App;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using App1.Droid.Workspace.Database.Table;
using App1.Droid.Workspace.Database.Table.Views.Columns;
using Firebase.Database;
using Firebase.Storage;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelImage : ColumnModel
    {
        FirebaseStorage storage;
        StorageReference Ref;

        public ColumnModelImage(DataSnapshot data) : base(data)
        {
            this.Data = this.data;
        }

        public ColumnModelImage()
        {
        }

        ~ColumnModelImage()
        {
            foreach( FileDownloadTask i in Ref.ActiveDownloadTasks)
            {
                i.Cancel();
            }
            
        }

        public StorageReference GetRef()
        {
            if (Ref == null){
                string path = data.GetValueOrDefault("ref");
                storage = FirebaseStorage.Instance;
                
                if (!string.IsNullOrEmpty(path))
                {
                    Ref = storage.Reference.Child(path);
                }
                else
                {
                    Ref = storage.Reference;
                }
            }
            return Ref;
        }

        public override Dictionary<string, string> Data {
            get
            {
                if(Ref != null)
                {
                    return new Dictionary<string, string>() { {"ref", Ref.ToString() } };
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if(value!=null)
                if (value.ContainsKey("ref"))
                {
                    Ref = storage.GetReferenceFromUrl(value.GetValueOrDefault("ref"));
                }
            }
        }

        public override CellModel ConstructCell()
        {
            return new CellModelImage(this);
        }

        public override ColumnView GetEditView(Activity context)
        {
            return new ColumnViewImage(context, controller);
        }
        
    }
}