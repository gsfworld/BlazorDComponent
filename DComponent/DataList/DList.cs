using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DComponent
{
    public partial class DList<TItem>
    {
        [Parameter]
        public string Id { get; set; }
        private List<ListData> _Data;
        [Parameter]
        public List<TItem> Data { get; set; }
        [Parameter]
        public TItem SingleSelectedData { get; set; }
        [Parameter]
        public EventCallback<TItem> SingleSelectedDataChanged { get; set; }
        [Parameter]
        public RenderFragment<object> ChildContent { get; set; }
        [Parameter]
        public SelectMode SelectMode { get; set; } = SelectMode.S;
        [Parameter]
        public string IdField { get; set; }
        [Parameter]
        public string TextField { get; set; }
        [Parameter]
        public List<TItem> SelectedData { get; set; }
        [Parameter]
        public EventCallback<List<TItem>> SelectedDataChanged { get; set; }
        private PropertyInfo[] _dataProps { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Refresh();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Id))
            {
                Id = $"DC{Guid.NewGuid().ToString().Replace("-", "")}";
                SelectedData = new List<TItem>();
                _dataProps = Data.GetType().GetGenericArguments()[0].GetProperties();
            }            
        }

        private void Refresh()
        {
            if (_dataProps.All(v => v.Name != IdField)) return;
            if (_dataProps.All(v => v.Name != TextField)) return;
            _Data = Data.Select(p => new ListData
            {
                id = _dataProps.First(d => d.Name == IdField).GetValue(p)?.ToString(),
                isSelected = GetSelected(p),
                tag = p,
                text = p.GetType().GetProperty(TextField).GetValue(p)?.ToString()
            }).ToList();
            //StateHasChanged();
        }
        private bool GetSelected(TItem item)
        {
            var value = _Data?.FirstOrDefault(s => s.id == _dataProps.First(d => d.Name == IdField).GetValue(item).ToString());
            if (value != null)
            {
                return value.isSelected;
            }
            return false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        internal class ListData
        {
            public string id;
            public string text;
            public object tag;
            public bool isSelected = false;
        }
    }
}
