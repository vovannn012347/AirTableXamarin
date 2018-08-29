using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Gms.Tasks;
using Android.Graphics;
using Android.Views;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using Firebase.Database;
using Firebase.Storage;
using Java.IO;
using Java.Lang;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelImage : CellModel, IOnSuccessListener
    {
        string imageName;
        StorageReference StorageRef;
        CellControllerImage controller;

        public CellModelImage(ColumnModel parent)
        {
            parentColumn = parent;
            controller = new CellControllerImage(this);
            consume_update = false;
        }

        public override CellView GetView(Activity context)
        {
            return new CellViewImage(context, controller);
        }
        
        public override void SetData(DataSnapshot data)
        {
            if (consume_update)
            {
                consume_update = false;
                return;
            }

            imageName = data.Value.ToString();

            if (!System.String.IsNullOrEmpty(imageName))
            {
                StorageRef = ((ColumnModelImage)parentColumn).getRef().Child(imageName);
                
                controller.NotifyDataChanged(((ColumnModelImage)parentColumn).getRef().Child("thumbnail_" + imageName));
            }
        }

        public override void ColumnChangeSetData(string data)
        {
            imageName = data;
            if (!System.String.IsNullOrEmpty(imageName))
            {
                StorageRef = ((ColumnModelImage)parentColumn).getRef().Child(imageName);

                controller.NotifyDataChanged(
                    ((ColumnModelImage)parentColumn).getRef().Child("thumbnail_" + imageName)
                   );
            }
            else
            {
                StorageRef = null;
                controller.NotifyDataChanged(null);
            }
        }

        public StorageReference getFullSizeReference()
        {
            return StorageRef;
        }

        public override string Data{
            get
            {
                return imageName;
            }
            set
            {
                imageName = value;
                if (!System.String.IsNullOrEmpty(value))
                {
                    StorageRef = ((ColumnModelImage)parentColumn).getRef().Child(imageName);
                    if(StorageRef != null)
                    {
                        controller.NotifyDataChanged(
                            ((ColumnModelImage)parentColumn).getRef().Child("thumbnail_" +imageName)
                            );
                    }
                    consume_update = true;
                    Row_Ref.Child(parentColumn.ColumnId).SetValue(value);
                }
                else
                {
                    StorageRef = null;
                    controller.NotifyDataChanged(null);
                    consume_update = true;
                    Row_Ref.Child(parentColumn.ColumnId).RemoveValue();
                }
                
            }
        }

        public StorageReference GetRef()
        {
            return StorageRef;
        }

        public override void EraseData()
        {
            imageName = "";
            StorageRef = null;
            controller.NotifyDataChanged(StorageRef);
        }
        
        public void SetImage(Bitmap imageBitmap, string name)
        {
            int THUMBNAIL_SIZE = 128;

            int outWidth;
            int outHeight;
            int inWidth = imageBitmap.Width;
            int inHeight = imageBitmap.Height;
            if (inWidth > inHeight)
            {
                outWidth = THUMBNAIL_SIZE;
                outHeight = (inHeight * THUMBNAIL_SIZE) / inWidth;
            }
            else
            {
                outHeight = THUMBNAIL_SIZE;
                outWidth = (inWidth * THUMBNAIL_SIZE) / inHeight;
            }

            MemoryStream baos1 = new MemoryStream();
            imageBitmap.Compress(Bitmap.CompressFormat.Png, 100, baos1);
            byte[] arr = baos1.ToArray();

            ((ColumnModelImage) parentColumn).getRef().Child(name).PutBytes(arr);

            Bitmap thumbnail =
                    Bitmap.CreateScaledBitmap(imageBitmap, outWidth, outHeight, false);

            MemoryStream baos2 = new MemoryStream();
            thumbnail.Compress(Bitmap.CompressFormat.Jpeg, 100, baos2);
             arr = baos2.ToArray();
           
            ((ColumnModelImage)parentColumn).getRef().Child("thumbnail_" + name)
                .PutBytes(arr).AddOnSuccessListener(this);
            
            this.Data = name;
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            controller.NotifyDataChanged(StorageRef);
        }
    }
}