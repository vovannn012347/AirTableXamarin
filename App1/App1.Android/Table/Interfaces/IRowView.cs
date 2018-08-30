using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models;

namespace App1.Droid.Table.Views
{
    interface IRowView : CompoundButton.IOnCheckedChangeListener
    {
        void ColumnAdded(int index, CellModel cellModel, ColumnModel columnModel);
        void ColumnDeleted(int index);
        void ColumnChanged(int index, CellModel cellModel, ColumnModel columnModel);
        void Initiate(List<CellModel> cells, List<ColumnModel> coluns);
        void DeleteView();
        new void OnCheckedChanged(CompoundButton buttonView, bool isChecked);
        void OnClick(View v);
        View GetView();
        void SetChecked(bool check);
        void UserChecked(bool check);
    
    }
}