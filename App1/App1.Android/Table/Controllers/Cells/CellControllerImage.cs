using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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

        public void NotifyDataChanged(StorageReference Ref)
        {
            foreach(ICellViewImage view in cellViews)
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

        public void HookView(ICellViewImage cellViewImage)
        {
            cellViews.Add(cellViewImage);

            cellViewImage.SetImage(cellModelImage.GetRef());
        }

        public void UnhookView(ICellViewImage cellViewImage)
        {
            cellViews.Remove(cellViewImage);
        }
        //i should put this into hookview
        public StorageReference GetFullImageRef()
        {
            return cellModelImage.getFullSizeReference();
        }

    }
}