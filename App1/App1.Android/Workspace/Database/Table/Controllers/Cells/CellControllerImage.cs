using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views.Cells;
using Firebase.Storage;

namespace App1.Droid.Table.Controllers.Cells
{
    class CellControllerImage
    {
        private CellModelImage cellModelImage;
        private List<ICellViewImage> cellViews;

        public CellControllerImage(CellModelImage cellModelImage)
        {
            this.cellModelImage = cellModelImage;
            cellViews = new List<ICellViewImage>();
        }

        public void HookView(ICellViewImage cellViewImage)
        {
            cellViews.Add(cellViewImage);

            cellViewImage.SetImage(cellModelImage.GetRef());
        }

        internal void NotifyImageUploaded(StorageReference storageReference)
        {
            foreach (ICellViewImage view in cellViews)
            {
                view.SetImageUiThread(storageReference);
            }
        }

        public void UnhookView(ICellViewImage cellViewImage)
        {
            cellViews.Remove(cellViewImage);
        }
       
        public void NotifyDataChanged(StorageReference Ref)
        {
            foreach (ICellViewImage view in cellViews)
            {
                view.SetImage(Ref);
            }
        }
        public void ImageUpdated(Bitmap imageBitmap, string name)
        {
            cellModelImage.SetImage(imageBitmap, name);
            
        }
        public void ImageDeleted()
        {
            cellModelImage.Data = null;
        }
        
        public StorageReference GetFullImageRef()
        {
            return cellModelImage.GetRef();
        }

    }
}