using Android.Animation;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Views.Cells;
using Bumptech.Glide;
using Firebase.Storage;
using Java.Lang;

namespace App1.Droid.Table.Views
{
    class EditImageDialog : DialogFragment, ICellViewImage, IOnSuccessListener, IDialogInterfaceOnClickListener
    {
        LinearLayout mainView;
        ImageView imageView;

        Activity context;
        CellControllerImage controller;
        
        public EditImageDialog(Activity context, CellControllerImage controller)
        {
            this.context = context;
            this.controller = controller;

            mainView = new LinearLayout(context)
            {
                Orientation = Orientation.Vertical
            };
            LayoutTransition lt = new LayoutTransition();
            lt.EnableTransitionType(LayoutTransitionType.Appearing);
            lt.EnableTransitionType(LayoutTransitionType.Changing);
            mainView.LayoutTransition = lt;

            imageView = new ImageView(context);
            mainView.AddView(imageView);

            controller.HookView(this);
                    }

        public override void Dismiss()
        {
            DeleteView();
            base.Dismiss();
        }
    
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new 
                AlertDialog.Builder(context)
                .SetView(mainView)
                .SetNeutralButton("Delete", this)
                .SetPositiveButton("Back", this)
                .Create();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch ((DialogButtonType)which)
            {
                case DialogButtonType.Neutral:
                    controller.ImageDeleted();
                    break;
                default:
                    dialog.Dismiss();
                    break;
            }
        }

        public void OnSuccess(Object result)
        {
            if (!context.IsDestroyed)
                Glide.With(context)
                .Load(result.ToString())
                .Transition(Bumptech.Glide.Load.Resource.Drawable.DrawableTransitionOptions.WithCrossFade())
                .Into(imageView);
        }

        public void DeleteView()
        {
            controller.UnhookView(this);
        }

        public void SetImage(StorageReference Ref)
        {
            if (Ref != null)
            {
                imageView.SetImageResource(Resource.Drawable.empty);
                
                Task uploadtask = Ref.DownloadUrl;
                uploadtask.AddOnSuccessListener(this);
            }
            else
            {
                imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);
            }
        }

        public void SetImageUiThread(StorageReference Ref)
        {
            if (Ref != null)
            {
                context.RunOnUiThread(() =>
                {
                    imageView.SetImageResource(Resource.Drawable.empty);
                    Task uploadtask = Ref.DownloadUrl;
                    uploadtask.AddOnSuccessListener(this);
                });

            }
            else
            {
                imageView.SetImageResource(Android.Resource.Drawable.IcInputAdd);
            }
        }
    }
}