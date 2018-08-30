using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using Com.Bumptech.Glide;
using Firebase.Storage;
using Android.Gms.Tasks;
using Java.Lang;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewImage : CellView, Android.Views.View.IOnClickListener, IImageReciever, IOnSuccessListener, ICellViewImage
    {
        string imageName;
        ImageButton imageView;
        CellControllerImage controller;
        Activity context;
        bool editdialog;
        
        public CellViewImage(Activity context, CellControllerImage controller)
        {
            this.context = context;
            editdialog = false;
            imageView = new ImageButton(context);
            imageView.SetOnClickListener(this);
            imageView.SetBackgroundColor(Color.Transparent);
            imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);

            this.controller = controller;
            controller.HookView(this);
        }
        
        public void OnSuccess(Object result)
        {
            Glide.With(imageView.Context).Load(result.ToString()).Into(imageView);
        }
        public void OnClick(View v)
        {
            if (imageName != null)
            {
                if (context.FragmentManager.FindFragmentByTag("image_dialog") != null)
                {
                    return;
                }
                EditImageDialog dialog = new EditImageDialog(context, controller);
                dialog.Show(context.FragmentManager, "image_dialog");
            }
            else
            {
                ((IImageProvider)imageView.Context).LoadImage(this);
            }
        }
        public void receiveImage(Bitmap imageBitmap, string name)
        {
            controller.ImageUpdated(imageBitmap, name);
        }

        public void SetImage(StorageReference Ref)
        {
            //todo: redo this into asynktask
            if (Ref != null)
            {
                imageName = Ref.Name;
                Task uploadtask = Ref.DownloadUrl;
                uploadtask.AddOnSuccessListener(this);
            }
            else
            {
                imageName = null;
                imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);
            }
        }
        public override void SetData(string data)
        {
            // decode image from string?
        }

        public override View GetView()
        {
            return imageView;
        }
        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
        }
    }
}