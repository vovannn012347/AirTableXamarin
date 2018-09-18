namespace App1.Droid.Table.Models
{
    public class SortStruct
    {
        public enum SortMode { ASCENDING, DESCENDING }

        string columnId;
        SortMode mode;

        public string ColumnId { get => columnId; set => columnId = value; }
        public SortMode Mode { get => mode; set => mode = value; }

        public SortStruct(string columnId, SortMode mode)
        {
            this.ColumnId = columnId;
            this.Mode = mode;
        }

        public int Compare(RowModel model1, RowModel model2)
        {
            CellModel m1 = model1.Cells.Find(V => V.ParentColumn.ColumnId == ColumnId);
            CellModel m2 = model2.Cells.Find(V => V.ParentColumn.ColumnId == ColumnId);

            int ret = 0;

            if(m1 == null && m2 == null)
            {
                return 0;
            }
            else if(m1 == null)
            {
                ret = -1;
            }
            else if (m2 == null)
            {
                ret = 1;
            }
            else
            if (m1.Data == m2.Data)
            {
                ret = 0;
            }
            if (m1.Data == null)
            {
                ret = -1;
            }
            else 
            if (m2.Data == null)
            {
                ret = 1;
            }
            if(m1.Data.Length < m2.Data.Length)
            {
                ret = 1;
            }else
            if (m1.Data.Length > m2.Data.Length)
            {
                ret = -1;
            }
            else
            {
                for (int i = 0; i < m1.Data.Length; ++i)
                {
                    if(m1.Data[i] == m2.Data[i])
                    {
                        continue;
                    }
                    else if(m1.Data[i] < m2.Data[i])
                    {
                        ret = 1;
                        break;
                    }
                    else
                    {
                        ret = -1;
                        break;
                    }
                }
            }

            if (ret != 0 && mode == SortMode.ASCENDING)
            {
                ret *= -1;
            }
            
            return ret;
        }
    }
}