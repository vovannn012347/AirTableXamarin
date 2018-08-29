using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Storage;

namespace App1.Droid.Table.Views.Cells
{
    interface ICellViewImage
    {
        void SetImage(StorageReference Ref);
        void DeleteView();
    }
}