using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using App1.Droid.Table.Controllers;
using App1.Droid.Table.Models;
using App1.Droid.Table.Views;

namespace App1.Droid.Workspace.Database.Table.Views
{
    public class ItemView : Java.Lang.Object, View.IOnClickListener, IRowView
    {
        private Activity context;
        private RowController controller;

        private List<ItemCellView> cells;

        private View mainView;
        private LinearLayout contentlayout;
        private readonly RelativeLayout mainIdentifierLayout;
        private readonly FrameLayout imageView;
        
        private ItemCellView ImageCell;
        int imagesAmount;

        public ItemView(Activity context, RowController controller)
        {
            this.context = context;
            this.controller = controller;
            imagesAmount = 0;

            cells = new List<ItemCellView>();

            mainView = context.LayoutInflater.Inflate(Resource.Layout.TableItem, null);
            mainView.SetOnClickListener(this);

            contentlayout = mainView.FindViewById<LinearLayout>(Resource.Id.linearLayoutItemContent);
            mainIdentifierLayout = mainView.FindViewById<RelativeLayout>(Resource.Id.relativeLayoutFirstCell);
            imageView = mainView.FindViewById<FrameLayout>(Resource.Id.imageViewItemImage);

            controller.HookView(this);
        }

        private void RebuildView()
        {
            if (cells.Count == 0) return;
            mainIdentifierLayout.RemoveAllViews();
            ImageCell = null;
            imageView.RemoveAllViews();
            contentlayout.RemoveAllViews();

            mainIdentifierLayout.AddView(cells[0].GetHeadlineView());

            for(int i = 1; i < cells.Count; ++i)
            {
                if(cells[i].CellType() == CellModel.Type.Image && null == ImageCell)
                {
                    imageView.AddView(cells[i].GetView());
                    ImageCell = cells[i];
                }
                else
                {
                    contentlayout.AddView(cells[i].GetView());
                }
            }
        }

        public void Initiate(List<CellModel> cells, List<ColumnModel> columns)
        {
            for(int i = 0; i < columns.Count; ++i)
            {
                ColumnAdded(i,cells[i],columns[i]);
            }
            RebuildView();
        }

        public void ColumnAdded(int index, CellModel cellModel, ColumnModel columnModel)
        {
            ItemCellView v = new ItemCellView(
                columnModel, 
                cellModel,  
                context);
        
            cells.Insert(index, v);

            RebuildView();

            /*
            if (cellModel.CellType() == CellModel.Type.Image)
            {
                imagesAmount++;

                if (null != ImageCell  && cells.IndexOf(ImageCell) < index)
                {
                    //just leave it, its not going to be displayed
                }
                else
                {
                    ImageCell = v;
                    imageView.RemoveAllViews();
                    imageView.AddView(v.GetView());
                }
            }else
            if (index == 0)
            {
                if(null != IdentifierCell)
                {
                    mainIdentifierLayout.RemoveAllViews();
                    contentlayout.AddView(IdentifierCell.GetView(), 0);

                    IdentifierCell = v;
                    mainIdentifierLayout.AddView(IdentifierCell.GetHeadlineView());
                }
                else
                {
                    IdentifierCell = v;
                    mainIdentifierLayout.AddView(IdentifierCell.GetHeadlineView());
                }
            }
            else
            {
                contentlayout.AddView(v.GetView(), index - imagesAmount);
            }
            */
        }

        public void ColumnDeleted(int index)
        {
            ItemCellView prev = cells[index];
            cells.RemoveAt(index);
            RebuildView();

            if (prev.CellType() == CellModel.Type.Image)
            {
                imagesAmount--;
            }
                /*if(prev == ImageCell)
                {
                    ImageCell = null;
                    imageView.RemoveAllViews();
                    if(imagesAmount > 0)
                    {
                        for(int i = 0; i < cells.Count; ++i)
                        {
                            if (cells[i].CellType() == CellModel.Type.Image)
                            {
                                ImageCell = cells[i];
                                imageView.AddView(cells[i].GetView());
                                break;
                            }
                                
                        }
                    }
                }*/
            //}
            
            prev.DeleteView();

        }
        public void ColumnChanged(int index, CellModel cellModel, ColumnModel columnModel)
        {
            ColumnDeleted(index);
            ColumnAdded(index, cellModel, columnModel);
        }

        public void OnClick(View v)
        {
            controller.EditRecord(context);
        }

        public View GetView()
        {
            return mainView;
        }

        public void DeleteView()
        {
            controller.UnhookView(this);
            context = null;
        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {

        }

        public void SetChecked(bool check)
        {
        }

        public void UserChecked(bool check)
        {
        }
    }
}