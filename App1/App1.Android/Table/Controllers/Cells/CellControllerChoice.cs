﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Models.Cells;
using App1.Droid.Table.Views.Cells;

namespace App1.Droid.Table.Controllers.Cells
{
    class CellControllerChoice : CellController
    {
        private CellModelChoice cellModelChoice;
        private List<CellViewChoice> cellViews;

        public CellControllerChoice(CellModelChoice cellModelChoice)
        {
            this.cellModelChoice = cellModelChoice;
            cellViews = new List<CellViewChoice>();
        }

        public void HookView(CellViewChoice cellViewChoice)
        {
            cellViews.Add(cellViewChoice);

            int choiceIndex = int.Parse(cellModelChoice.Data);
            cellViewChoice.SetSelection(choiceIndex);
        }

        public void UnhookView(CellViewChoice cellViewChoice)
        {
            cellViews.Remove(cellViewChoice);
        }

        public void NotifyDataChanged(string data)
        {
            int choiceIndex = int.Parse(cellModelChoice.Data);

            foreach (CellViewChoice v in cellViews)
            {
                v.SetSelection(choiceIndex);
            }
        }
        
        public void UserSetData(string v)
        {
            cellModelChoice.Data = v;
        }


    }
}