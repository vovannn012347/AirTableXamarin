using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers.Cells;
using App1.Droid.Table.Views.Cells;
using Com.Bumptech.Glide;
using Firebase.Storage;
using Java.Lang;

namespace App1.Droid.Table.Views
{
    class EditImageDialog : DialogFragment, ICellViewImage, View.IOnClickListener, IOnSuccessListener
    {
        LinearLayout mainView;
        Button deleteButton;
        ImageView image;

        Activity context;
        CellControllerImage controller;
       // bool consume_update;
        
        public EditImageDialog(Activity context, CellControllerImage controller)
        {
            this.context = context;
            this.controller = controller;
            //consume_update = false;

            mainView = new LinearLayout(context);
            mainView.Orientation = Orientation.Vertical;

            deleteButton = new Button(context);
            deleteButton.SetText(Resource.String.delete_button_txt);
            deleteButton.SetBackgroundColor(Android.Graphics.Color.Red);
            mainView.AddView(deleteButton);

            image = new ImageView(context);
            mainView.AddView(image);

            controller.HookView(this);

            deleteButton.SetOnClickListener(this);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new AlertDialog.Builder(context).SetView(mainView).Create();
        }

        public void OnClick(View v)
        {
            controller.ImageDeleted();
            Dismiss();
        }

        public void SetImage(StorageReference Ref)
        {
            if (Ref != null)
            {
                controller.GetFullImageRef().DownloadUrl.AddOnSuccessListener(this);
            }
            else
            {
                image.SetImageBitmap(null);
            }
        }

        public void OnSuccess(Object result)
        {
            Glide.With(context).Load(result.ToString()).Into(image);
        }

        public void DeleteView()
        {
            controller.UnhookView(this);
        }

        public override void Dismiss()
        {
            this.DeleteView();
            base.Dismiss();
        }
    }
}