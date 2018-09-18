
using System.IO;

using Android.App;
using Android.Gms.Tasks;
using Android.Graphics;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Models.Columns;
using App1.Droid.Table.Views;
using App1.Droid.Table.Views.Cells;
using App1.Droid.Workspace.Database.Table.Views;
using Firebase.Database;
using Firebase.Storage;

namespace App1.Droid.Table.Models.Cells
{
    class CellModelImage : CellModel, IOnSuccessListener
    {
        string imageName;
        string uploadingImage;
        CellControllerImage controller;

        public CellModelImage(ColumnModel parent) : base(parent)
        {
            controller = new CellControllerImage(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Data = uploadingImage;
            
        }
    
        public override CellView GetView(Activity context)
        {
            return new CellViewImage(context, controller);
        }
        public StorageReference GetRef()
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                return ((ColumnModelImage)parentColumn).GetRef().Child(imageName);
            }
            else
            {
                return null;
            }
        }

        public override Type CellType()
        {
            return Type.Image;
        }

        public override void ColumnChangeSetData(string data)
        {
            imageName = data;
            if (!System.String.IsNullOrEmpty(imageName))
            {
                controller.NotifyDataChanged(
                    ((ColumnModelImage)parentColumn).GetRef().Child(imageName)
                   );
            }
            else
            {
                controller.NotifyDataChanged(null);
            }
        }
        public override void SetData(DataSnapshot data)
        {

            imageName = data.Value.ToString();

            if (!System.String.IsNullOrEmpty(imageName))
            {                
                controller.NotifyDataChanged(((ColumnModelImage)parentColumn).GetRef().Child(imageName));
            }
            else
            {
                controller.NotifyDataChanged(null);
            }
        }
        public void SetImage(Bitmap imageBitmap, string name)
        {
            if (imageBitmap == null)
            {
                this.Data = null;
                return;
            }

            this.uploadingImage = name;
            

            MemoryStream baos1 = new MemoryStream();
            imageBitmap.Compress(Bitmap.CompressFormat.Png, 100, baos1);
            byte[] arr = baos1.ToArray();

            ((ColumnModelImage) parentColumn).GetRef().Child(name)
                .PutBytes(arr).AddOnSuccessListener(this);
            /*
            Bitmap thumbnail =
                    Bitmap.CreateScaledBitmap(imageBitmap, outWidth, outHeight, false);

            MemoryStream baos2 = new MemoryStream();
            thumbnail.Compress(Bitmap.CompressFormat.Jpeg, 100, baos2);
             arr = baos2.ToArray();
           
            ((ColumnModelImage)parentColumn).GetRef().Child("thumbnail_" + name)
                .PutBytes(arr).AddOnSuccessListener(this);
            */
            /*
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
            }*/

        }

        public override void ColumnDeleted()
        {
            imageName = "";
        }

        public override void Save()
        {
            if (!System.String.IsNullOrEmpty(imageName))
            {
                Row_Ref.Child(parentColumn.ColumnId).SetValue(imageName);
            }
            else
            {
                Row_Ref.Child(parentColumn.ColumnId).RemoveValue();
            }
        }

        public override string Data{
            get
            {
                return imageName;
            }
            set
            {
                if(imageName != value)
                {
                    imageName = value;

                    if (!System.String.IsNullOrEmpty(value))
                    {
                        controller.NotifyImageUploaded(((ColumnModelImage)parentColumn).GetRef().Child(imageName));
                    }
                    else
                    {
                        controller.NotifyDataChanged(null);
                    }
                }
                
                
            }
        }
        
    }
}