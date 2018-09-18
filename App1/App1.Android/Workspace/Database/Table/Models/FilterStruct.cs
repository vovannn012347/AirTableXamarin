namespace App1.Droid.Table.Models
{
    public class FilterStruct
    {
        public enum FilterMode { NONE, IS, NOT_IS, CONTAINS, NOT_CONTAINS, ONE_OF, NOT_ONE_OF, EMPTY, NOT_EMPTY}

        string columnId;
        FilterMode mode;
        string predicate;

        public string ColumnId { get => columnId; set => columnId = value; }
        public FilterMode Mode { get => mode; set => mode = value; }
        public string Condition { get => predicate; set => predicate = value; }

        public FilterStruct(string columnId, FilterMode mode, string condition)
        {
            this.ColumnId = columnId;
            this.Mode = mode;
            this.Condition = condition;
        }
        
        public bool Filtered(RowModel model)
        {
            if (string.IsNullOrEmpty(Condition)) return true;

            CellModel m = model.Cells.Find(V => V.ParentColumn.ColumnId == ColumnId);
            switch (Mode)
            {
                case FilterMode.IS:
                    if (m == null)
                    {
                        return false;
                    }
                    return m.Data == Condition;
                case FilterMode.NOT_IS:
                    if (m == null)
                    {
                        return true;
                    }
                    return m.Data != Condition;
                case FilterMode.CONTAINS:
                    if (m == null)
                    {
                        return false;
                    }
                    return m.Data.Contains(Condition);
                case FilterMode.NOT_CONTAINS:
                    if (m == null)
                    {
                        return true;
                    }
                    return !m.Data.Contains(Condition);
                case FilterMode.ONE_OF:
                    if (m == null)
                    {
                        return false;
                    }
                    return predicate.Contains(m.Data);
                case FilterMode.NOT_ONE_OF:
                    if (m == null)
                    {
                        return true;
                    }
                    return !predicate.Contains(m.Data);
                case FilterMode.EMPTY:
                    if (m == null)
                    {
                        return true;
                    }
                    return string.IsNullOrEmpty(m.Data);
                case FilterMode.NOT_EMPTY:
                    if (m == null)
                    {
                        return false;
                    }
                    return !string.IsNullOrEmpty(m.Data);
            }

            return true;
        }
    }
}