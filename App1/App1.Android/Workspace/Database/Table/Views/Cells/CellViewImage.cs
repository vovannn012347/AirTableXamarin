using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using Firebase.Storage;
using Android.Gms.Tasks;
using System;
using static Android.Support.Constraints.Constraints;
using Bumptech.Glide;

namespace App1.Droid.Table.Views.Cells
{
    class CellViewImage : CellView, Android.Views.View.IOnClickListener, 
        IImageReciever, ICellViewImage
    {
        string imageName;
        RelativeLayout mainView;
        ImageView imageView;
        ProgressBar bar;
        CellControllerImage controller;
        Activity context;
        //int count;
        
        public CellViewImage(Activity context, CellControllerImage controller)
        {
            this.context = context;

            mainView = new RelativeLayout(context);

            imageView = new ImageButton(context);
            imageView.SetBackgroundColor(Color.Transparent);
            imageView.SetImageResource(Android.Resource.Drawable.IcMenuGallery);

            mainView.AddView(imageView);

            this.controller = controller;
            controller.HookView(this);
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

            if(bar == null)
            {
                /*
                bar = new ProgressBar(context)
                {
                    Indeterminate = true
                };
                imageView.Visibility = ViewStates.Invisible;

                mainView.AddView(bar);*/
            }
            
            controller.ImageUpdated(imageBitmap, name);
        }

        public void SetImageUiThread(StorageReference storageReference)
        {
            imageName = storageReference.Name;
            storageReference.DownloadUrl.AddOnSuccessListener(new Uploader(this));
        }

        public void SetImage(StorageReference Ref)
        {
            if (Ref != null)
            {
                /*if (bar == null)
                {
                    bar = new ProgressBar(context)
                    {
                        Indeterminate = true
                    };
                    mainView.AddView(bar);
                }*/
                //imageView.Visibility = ViewStates.Invisible;

                imageName = Ref.Name;

                Ref.DownloadUrl.AddOnSuccessListener(new Uploader(this));

            }
            else
            {
                imageName = null;
                imageView.SetImageResource(Android.Resource.Drawable.IcMenuGallery);
            }
        }
        public override void SetData(string data)
        {
            // decode image from string?
        }

        public override View GetView()
        {
            if (imageView.HasOnClickListeners)
            {
                imageView.SetOnClickListener(null);
            }
            imageView.SetImageResource(Android.Resource.Drawable.IcMenuGallery);
            imageView.LayoutParameters = new RelativeLayout.LayoutParams(
                Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension.database_table_item_height)),
                Convert.ToInt32(context.Resources.GetDimension(Resource.Dimension.database_table_item_height))
                );
            imageView.Clickable = false;
            imageView.Focusable = false;
            imageView.FocusableInTouchMode = false;
            

            return mainView;
        }
        public override View GetEditView()
        {
            imageView.SetOnClickListener(this);
            imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);
            imageView.LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
            imageView.Clickable = true;
            imageView.Focusable = true;

            return mainView;
        }
        public override void DeleteView()
        {
            controller.UnhookView(this);
            controller = null;
        }

        public void SetImageUi(Bitmap btm)
        {
            context.RunOnUiThread(() =>
            {
                imageView.Visibility = ViewStates.Visible;
                imageView.SetImageBitmap(btm);
                imageView.SetScaleType(ImageView.ScaleType.FitCenter);
                imageView.SetAdjustViewBounds(true);

                if (bar != null)
                {
                    mainView.RemoveView(bar);
                    //bar.Dispose();
                    bar = null;
                }
            });
        }

        class Uploader : Java.Lang.Object, IOnSuccessListener
        {
            private CellViewImage cellViewImage;

            public Uploader(CellViewImage cellViewImage)
            {
                this.cellViewImage = cellViewImage;
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                if(!cellViewImage.context.IsDestroyed)
                Glide.With(cellViewImage.context)
                .Load(result.ToString())
                .Transition(Bumptech.Glide.Load.Resource.Drawable.DrawableTransitionOptions.WithCrossFade())
                .Into(cellViewImage.imageView);

                
            }
        }



    }
}