namespace App1.Droid.Table.Views
{
    interface ICellViewDate
    {
        void DeleteView();
        void SetData(string data);
        void SetDateFormat(System.Globalization.DateTimeFormatInfo format);
    }
}