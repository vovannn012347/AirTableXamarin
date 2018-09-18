using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using Android.Widget;
using App1.Droid.Workspace.Database;
using Firebase.Database;
using static Android.Views.View;

namespace App1.Droid
{
    internal class DatabaseListView : Java.Lang.Object, IOnClickListener
    {
        private Context context;

        View mainView;
        TextView textView;
        ImageView image;
        GradientDrawable backgroundShape;

        DatabaseReference data;
        DatabaseReference SchemaData;
        readonly SchemaListener nameListener;
        string name;
        bool imageRes;

        public DatabaseListView(DatabaseReference data, DatabaseReference nameData, Activity context)
        {
            this.context = context;
            this.data = data;
            this.SchemaData = nameData;
            this.imageRes = false;

            mainView = context.LayoutInflater.Inflate(Resource.Layout.DatabaseItemLayout, null);

            textView = mainView.FindViewById<TextView>(Resource.Id.textViewNameElement);
            nameListener = new SchemaListener(this);
            
            backgroundShape = new GradientDrawable();
            backgroundShape.SetColor(Color.Black.ToArgb());

            float size = context.Resources.GetDimension(Resource.Dimension.workplace_image_size);
            backgroundShape.SetCornerRadius(size / 8);

            image = mainView.FindViewById<ImageView>(Resource.Id.imageViewElement);
            image.Background = backgroundShape;
            image.SetColorFilter(Color.White);
            image.SetImageResource(Resource.Drawable.icons_question_mark);

            mainView.SetOnClickListener(this);
            SchemaData.AddChildEventListener(nameListener);
        }

        public bool Deleted { get; internal set; }
        public string Name {
            get => name;

            set {
                name = value;
                textView.Text = value;
                if (!imageRes)
                {/*
                    float height = context.Resources.GetDimension(Resource.Dimension.workplace_image_size);
                    float width = context.Resources.GetDimension(Resource.Dimension.workplace_image_size);
                    
                    Bitmap.Config conf =
                        Bitmap.Config.RgbaF16;

                    Bitmap newImage = Bitmap.CreateBitmap(
                        Convert.ToInt32(height),
                        Convert.ToInt32(width),
                        conf
                        );
                    
                    Canvas c = new Canvas(newImage);

                    Paint paint = new Paint
                    {
                        Color = Color.White,
                        TextSize = (height / 5) * 4
                    };
                    paint.SetStyle(Paint.Style.Fill);
                    c.DrawText(name, 0, 2,  height/5, width/5, paint);

                    image.SetImageBitmap(newImage);*/
                }
            }
        }
        public int BackColor
        {
            set
            {
                int val = value % DatabaseController.colors.Length;

                backgroundShape.SetColor(DatabaseController.colors[value]);
            }
        }

        public int Icon
        {
            set
            {
                int val = value % DatabaseController.imageResources.Length;

                imageRes = true;

                image.SetImageResource(DatabaseController.imageResources[val]);

            }
        }

        internal bool Filetered(string filtered)
        {
            return Name.Contains(filtered);
        }

        public void OnClick(View v)
        {
            Intent intent = new Intent(context, typeof(DatabaseActivity));
            intent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
            intent.PutExtra("databasePath", data.ToString());
            intent.PutExtra("databaseSchema", SchemaData.ToString());
            context.StartActivity(intent);
        }

        internal void DeleteSelf()
        {
            SchemaData.RemoveEventListener(nameListener);

        }

        internal View GetView()
        {
            return mainView;
        }

        class SchemaListener : Java.Lang.Object, IChildEventListener
        {
            private readonly DatabaseListView parent;

            public SchemaListener(DatabaseListView workspaceModel)
            {
                parent = workspaceModel;
            }

            public void OnCancelled(DatabaseError error)
            {
                System.Diagnostics.Debug.WriteLine("Workspace Schema Error:" + error.Message);
            }

            public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
            {
                if (snapshot.Exists())
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
                        break;
                    case "color":
                        parent.BackColor = int.Parse(snapshot.Value.ToString());
                        break;
                    case "icon":
                        parent.Icon = int.Parse(snapshot.Value.ToString());
                        break;
                }
            }

            public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
            {
                if(snapshot.Exists())
                switch (snapshot.Key)
                {
                    case "name":
                        parent.Name = snapshot.Value.ToString();
                        break;
                    case "color":
                        parent.BackColor = int.Parse(snapshot.Value.ToString());
                        break;
                    case "icon":
                        parent.Icon = int.Parse(snapshot.Value.ToString());
                        break;
                }
            }

            public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
            {
                //??
            }

            public void OnChildRemoved(DataSnapshot snapshot)
            {
                switch (snapshot.Key)
                {
                    case "name":
                        parent.DeleteSelf();
                        break;
                    case "color":
                        parent.backgroundShape.SetColor(Color.Black.ToArgb());
                        break;
                    case "icon":
                        parent.Icon = 0;
                        break;
                }

            }
        }
    }
}