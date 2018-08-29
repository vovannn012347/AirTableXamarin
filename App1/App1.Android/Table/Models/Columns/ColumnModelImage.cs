using System;
using System.Collections.Generic;

using Android.App;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views;
using Firebase.Database;
using Firebase.Storage;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelImage : ColumnModel
    {
        StorageReference Ref;

        public ColumnModelImage() : base()
        {
            controller = new ColumnController(this);
        }

        public ColumnModelImage(DataSnapshot data) : base(data)
        {
            controller = new ColumnController(this);
        }

        public override ColumnView GetView(Activity context)
        {
            ColumnView cv = new ColumnView(context, controller);
            return cv;
        }

        public StorageReference getRef()
        {
            if (Ref == null){
                String path = data.GetValueOrDefault("ref");
                if (!String.IsNullOrEmpty(path))
                {
                    Ref = FirebaseStorage.Instance.Reference.Child(path);
                }
                else
                {
                    Ref = FirebaseStorage.Instance.Reference;
                }
            }
            return Ref;
        }

        public override CellModel constructCell()
        {
            return new CellModelImage(this);
        }
    }
}