using System;

namespace App1.Droid.Table.Views
{
    interface ICellViewDate
    {
        void DeleteView();
        void SetData(String data);
        void SetDateFormat(System.Globalization.DateTimeFormatInfo format);
    }
}