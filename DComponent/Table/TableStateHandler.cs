using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DComponent
{
    public sealed class TableStateHandler
    {
        private IEnumerable<object> _data;
        private readonly Type _dataType;
        private readonly StateDelegate.StateHasChanged _stateUpdater;
        private EventCallback<List<object>> _OnSelectRowsChanged;
        public readonly PageStateHandler Paginate;
        public bool AllRowChecked = false;//行全部选中        
        public TableStateHandler(
            IEnumerable<object> data,
            StateDelegate.StateHasChanged stateHasChanged,
            EventCallback<List<object>> OnSelectRowsChanged,
            int paginationRange = 3,
            int defaultPageSize = 15
        )
        {
            _data = data;
            _stateUpdater = stateHasChanged;
            _dataType = data.GetType().GetGenericArguments()[0];
            DataProps = _dataType.GetProperties();
            Paginate = new PageStateHandler(_stateUpdater, paginationRange, defaultPageSize);
            _OnSelectRowsChanged = OnSelectRowsChanged;
        }
        public Type DataType { get => _dataType; }
        public Dictionary<string, Column> Columns { get; } = new Dictionary<string, Column>();
        public Dictionary<object, bool> SelectRows { get; } = new Dictionary<object, bool>();
        public PropertyInfo[] DataProps { get; }
        private Dictionary<string, bool> DetailExpandRows { get; } = new Dictionary<string, bool>();
        private Dictionary<string, bool> GroupExpandRows { get; } = new Dictionary<string, bool>();

        public void UpdateData(IEnumerable<object> udata)
        {
            _data = udata;
        }
        public void InitColumn(Column column)
        {
            var name = column.Property.Name;
            if (Columns.ContainsKey(name)) return;
            if (column.CType != ColumnType.O)
            {
                if (DataProps.All(v => v.Name != name)) return;
            }
            //throw new ArgumentException(
            //    $"Field name '{name}' does not exist in type '{_dataType.Name}'");
            Columns.Add(name, column);
            //_columnData[name] = new Column
            //{
            //    Property = _props.First(v => v.Name == name)
            //};
        }
        public string DetailExpandRowDir(string name)
        {
            if (DetailExpandRows.ContainsKey(name) && DetailExpandRows[name])
                return "fa fa-arrow-down";
            return "fa fa-arrow-right";
        }
        public bool GetDetailExpandRow(string name)
        {
            if (DetailExpandRows.ContainsKey(name))
                return DetailExpandRows[name];
            return false;

        }
        public void UpdateDetailExpandRow(string name, bool expanded)
        {
            //DetailExpandRows.Clear();//一行展开
            if (DetailExpandRows.ContainsKey(name))
            {
                DetailExpandRows[name] = expanded;
                //if (!expanded)
                //{
                //    DetailExpandRows.Remove(name);                    
                //}
            }
            else
            {
                DetailExpandRows.Add(name, true);
            }
            //_stateUpdater.Invoke();
        }
        public void UpdateDetailExpandRowNoRender(string name, bool expanded)
        {
            //DetailExpandRows.Clear();//一行展开
            if (DetailExpandRows.ContainsKey(name))
            {
                DetailExpandRows[name] = expanded;
                //if (!expanded)
                //{
                //    DetailExpandRows.Remove(name);                    
                //}
            }
            else
            {
                DetailExpandRows.Add(name, true);
            }
        }

        public string GroupExpandRowDir(string name)
        {
            if (GroupExpandRows.ContainsKey(name) && GroupExpandRows[name])
                return "fa fa-arrow-down";
            return "fa fa-arrow-right";
        }
        public bool GetGroupExpandRow(string name)
        {
            if (GroupExpandRows.ContainsKey(name))
                return GroupExpandRows[name];
            return false;
        }
        public void UpdateGroupExpandRow(string name, bool expanded)
        {
            //GroupExpandRows.Clear();//一组
            if (GroupExpandRows.ContainsKey(name))
            {
                GroupExpandRows[name] = expanded;
                //if (!expanded)
                //{
                //    GroupExpandRows.Remove(name);
                //}
            }
            else
            {
                GroupExpandRows.Add(name, true);
            }
            //_stateUpdater.Invoke();
        }

        public void UpdateGroupExpandRowNoRender(string name, bool expanded)
        {
            //GroupExpandRows.Clear();//一组
            if (GroupExpandRows.ContainsKey(name))
            {
                GroupExpandRows[name] = expanded;
                //if (!expanded)
                //{
                //    GroupExpandRows.Remove(name);
                //}
            }
            else
            {
                GroupExpandRows.Add(name, true);
            }
        }

        public bool GetRowChecked(object row)
        {
            if (SelectRows.ContainsKey(row))
                return SelectRows[row];
            return false;
        }

        public void UpdateRowChecked(object row, bool checkValue, SelectMode selectMode)
        {
            if (selectMode == SelectMode.S)
                SelectRows.Clear();
            if (SelectRows.ContainsKey(row))
            {
                if (!checkValue)
                {
                    SelectRows.Remove(row);
                    AllRowChecked = false;
                }
            }
            else
            {
                if (checkValue)
                {
                    SelectRows.Add(row, true);
                    AllRowChecked = SelectRows.Count == _data.Count();
                }
            }
            if (_OnSelectRowsChanged.HasDelegate)
                _OnSelectRowsChanged.InvokeAsync(SelectRows.Keys.ToList());
            //_stateUpdater.Invoke();
        }
        public void UpdateRowAllChecked(ChangeEventArgs e)
        {
            if (e.Value.ToString() == "True")
            {
                foreach (var row in _data)
                {
                    if (!SelectRows.ContainsKey(row))
                    {
                        SelectRows.Add(row, true);
                    }
                }
                AllRowChecked = true;
            }
            else
            {
                SelectRows.Clear();
                AllRowChecked = false;
            }
            if (_OnSelectRowsChanged.HasDelegate)
                _OnSelectRowsChanged.InvokeAsync(SelectRows.Keys.ToList());
            //_stateUpdater.Invoke();
        }

        public void UpdateColumnValue(ChangeEventArgs args, string name)
        {
            Columns[name].FilterValue = (string)args.Value == "" ? null : (string)args.Value;
            _stateUpdater.Invoke();
        }
        public void UpdateColumnFilterContidion(string e, string name)
        {
            Columns[name].FCondition = e == "" ? ColumnFilterCondition.E : (ColumnFilterCondition)Enum.Parse(typeof(ColumnFilterCondition), e);
            _stateUpdater.Invoke();
        }

        public string ColumnValue(string name)
        {
            return Columns[name].FilterValue;
        }

        public void UpdateSort(string name)
        {
            foreach ((string key, Column _) in Columns.Where(v => v.Key != name))
                Columns[key].SortDir = null;
            if (Columns[name].SortDir == null)
                Columns[name].SortDir = 'a';
            else
                Columns[name].SortDir = Columns[name].SortDir == 'a' ? 'd' : 'a';
            //_stateUpdater.Invoke();
        }

        public void ResetSorting()
        {
            foreach ((string key, Column _) in Columns)
            {
                Columns[key].SortDir = null;
            }
            //_stateUpdater.Invoke();
        }

        public void ResetFilterValue()
        {
            foreach ((string key, Column _) in Columns)
            {
                Columns[key].FilterValue = null;
            }
            //_stateUpdater.Invoke();
        }

        public string SortDir(string name)
        {
            if (Columns[name].SortDir == null)
                return "fa fa-list";
            return Columns[name].SortDir == 'a' ? "fa fa-arrow-up" : "fa fa-arrow-down";
        }



        public void UpdatePageSize(ChangeEventArgs args)
        {
            Paginate.PageSize = int.Parse((string)args.Value);
            _stateUpdater.Invoke();
        }

        //        string.Compare(str1, str2, true);
        //        返回值：
        //1 ： str1大于str2
        //0 ： str1等于str2
        //-1 ： str1小于str2
        private static bool Match(Column column, object row)
        {
            var str = column.Property.GetValue(row)?.ToString();
            //lookupEdit需要转换值
            if (column.CType == ColumnType.L)
            {
                var convertRow = column.CData.FirstOrDefault(p => p.GetType().GetProperty(column.ValueField).GetValue(p).ToString() == str);
                if (convertRow != null)
                    str = convertRow.GetType().GetProperty(column.DisplayField).GetValue(convertRow).ToString();
            }
            switch (column.FCondition)
            {
                case ColumnFilterCondition.L:
                    return string.Compare(str, column.FilterValue, true) == -1;
                case ColumnFilterCondition.M:
                    return string.Compare(str, column.FilterValue, true) == 1;
                case ColumnFilterCondition.E:
                    return string.Compare(str, column.FilterValue, true) == 0;
                case ColumnFilterCondition.C:
                    return str?.IndexOf(column.FilterValue, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        public IEnumerable<object> PageData()
        {
            IEnumerable<object> data = _data;
            data = data.Where(v =>
            {
                foreach ((string _, Column c) in Columns)
                {
                    if (c.FilterValue == null) continue;
                    bool matches = Match(c, v);
                    if (!matches) return false;
                }
                return true;
            }).ToList();

            foreach ((string s, Column value) in Columns)
                if (value.SortDir != null)
                {
                    PropertyInfo prop = DataProps.First(v => v.Name == s);
                    if (value.SortDir == 'a')
                        Sort(ref data, prop, false);
                    else if (value.SortDir == 'd')
                        Sort(ref data, prop, true);
                    break;
                }
            IEnumerable<object> enumerable = data.ToArray();
            Paginate.RowCount = enumerable.Count();
            if (Paginate.PageSize != 0)
                data = enumerable.Skip(Paginate.Skip).Take(Paginate.PageSize).ToList();
            return data;
        }

        public IEnumerable<object> GroupPageData()
        {
            IEnumerable<object> data = _data;
            data = data.Where(v =>
            {
                foreach ((string _, Column c) in Columns)
                {
                    if (c.FilterValue == null) continue;
                    bool matches = Match(c, v);
                    if (!matches) return false;
                }
                return true;
            }).ToList();
            if (Columns.Any(p => p.Value.IsGroup))
            {
                var groupColumn = Columns.FirstOrDefault(p => p.Value.IsGroup);
                if (groupColumn.Value.SortDir != null)
                {
                    if (groupColumn.Value.SortDir == 'a')
                        Sort(ref data, groupColumn.Value.Property, false);
                    else if (groupColumn.Value.SortDir == 'd')
                        Sort(ref data, groupColumn.Value.Property, true);
                }
            }
            IEnumerable<object> enumerable = data.ToArray();
            Paginate.RowCount = enumerable.Count();
            if (Paginate.PageSize != 0)
                data = enumerable.Skip(Paginate.Skip).Take(Paginate.PageSize).ToList();
            return data;
        }

        private static void Sort(ref IEnumerable<object> data, PropertyInfo prop, bool desc)
        {
            IEnumerable<object> enumerable = data as object[] ?? data.ToArray();
            if (!enumerable.Any()) return;
            bool isSortable = enumerable.First() is IComparable;
            if (!desc)
                if (isSortable)
                    data = enumerable.OrderBy(v => prop.GetValue(v)).ToList();
                else
                    data = enumerable.OrderBy(v => prop.GetValue(v)?.ToString());
            else if (isSortable)
                data = enumerable.OrderByDescending(v => prop.GetValue(v)).ToList();
            else
                data = enumerable.OrderByDescending(v => prop.GetValue(v)?.ToString());
        }

    }

}
