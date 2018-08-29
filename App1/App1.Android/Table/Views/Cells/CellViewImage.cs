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

        public CellViewImage(Activity context, CellControllerImage controller)
        {
            this.context = context;
            imageView = new ImageButton(context);
            imageView.SetOnClickListener(this);
            imageView.SetBackgroundColor(Color.Transparent);
            imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);

            this.controller = controller;

            controller.HookView(this);
        }

        
        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
        }
        
        public override void SetData(string data)
        {
            //todo: decode image from string
            //not used
        }

        public void OnSuccess(Object result)
        {
            Glide.With(imageView.Context).Load(result.ToString()).Into(imageView);
        }

        public void SetImage(StorageReference Ref)
        {
            //todo: redo this into asynktask
            if(Ref != null)
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
        
        public void OnClick(View v)
        {
            if (imageName != null)
            {
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

        public override View GetView()
        {
            return imageView;
        }

        public override View GetDialogView()
        {
            Task uploadtask = controller.GetFullImageRef().DownloadUrl;
            uploadtask.AddOnSuccessListener(this);

            return imageView;
        }

        /* a dump of wasted time
Bitmap getBitmap(Android.Net.Uri uri)
{
   return Media.GetBitmap(imageView.Context.ContentResolver, uri);
}
public void OnSuccess(Java.Lang.Object result)
{
   //ok, lets assume it is uri
   Android.Net.Uri uri = (Android.Net.Uri)result;
   //got uri? upload it

   Thread t = new Thread(
       () =>
       {
           Bitmap b = getBitmap(uri);

           t->run
       }

    );

   Task<Bitmap> load = getBitmap(uri);

   //redo into async
   imageView.SetImageBitmap(b);
}*/
    }
}