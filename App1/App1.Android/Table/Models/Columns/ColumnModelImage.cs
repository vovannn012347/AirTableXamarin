using System;
using System.Collections.Generic;

using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models.Cells;
using Firebase.Database;
using Firebase.Storage;

namespace App1.Droid.Table.Models.Columns
{
    class ColumnModelImage : ColumnModel
    {
        StorageReference Ref;

        public StorageReference GetRef()
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
        public override CellModel ConstructCell()
        {
            return new CellModelImage(this);
        }
    }
}