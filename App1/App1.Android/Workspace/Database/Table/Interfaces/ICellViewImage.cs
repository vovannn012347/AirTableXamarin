using Firebase.Storage;

namespace App1.Droid.Table.Views.Cells
{
    interface ICellViewImage
    {
        void SetImage(StorageReference Ref);
        void DeleteView();
        void SetImageUiThread(StorageReference storageReference);
    }
}