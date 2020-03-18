using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DComponent
{
    public partial class DTree<TItem>
    {
        private List<TItem> _Data;
        [Parameter]
        public List<TItem> Data
        {
            get => _Data;
            set
            {
                _Data = value;
                //Refresh();
            }
        }
        [Parameter]
        public Expression<Func<object, string>> TextExpression { get; set; }
        [Parameter]
        public Expression<Func<object, string>> IdExpression { get; set; }
        [Parameter]
        public Expression<Func<object, string>> ParentIdExpression { get; set; }
        [Parameter]
        public Expression<Func<object, IEnumerable<object>>> ChildrenExpression { get; set; }
        [Parameter]
        public TItem SingleSelectedData { get; set; }
        [Parameter]
        public EventCallback<TItem> SingleSelectedDataChanged { get; set; }
        [Parameter]
        public RenderFragment<object> NodeContext { get; set; }
        [Parameter]
        public bool ShowLine { get; set; } = false;
        [Parameter]
        public bool AutoExpandAll { get; set; } = false;
        [Parameter]
        public SelectMode SelectMode { get; set; } = SelectMode.S;
        [Parameter]
        public string IdField { get; set; }
        [Parameter]
        public string ParentField { get; set; }
        [Parameter]
        public string TextField { get; set; }
        [Parameter]
        public string SelectedField { get; set; }
        [Parameter]
        public string RootValue { get; set; } = string.Empty;
        [Parameter]
        public List<TItem> SelectedData { get; set; } = new List<TItem>();
        [Parameter]
        public EventCallback<List<TItem>> SelectedDataChanged { get; set; }
        private DTreeHandler _dTree;


        public List<TItem> GetSelectedData()
        {
            return _dTree.TreeNodeData.Where(p => p.Selected).Select(p => (TItem)p.Tag).ToList();
        }

        public void CollapseToNode(string id)
        {
            _dTree.CollapseToNode(id);
        }
        public void CollapseAll()
        {
            _dTree.CollapseAll();
        }
        public void ExpandAll()
        {
            _dTree.ExpandAll();
        }
        public void ExpandToNode(string id)
        {
            _dTree.ExpandToNode(id);
        }
        public void UpdateNodeSelect(string id, bool isSelected)
        {
            if (SelectMode == SelectMode.S)
                _dTree.UpdateNodeSelect(id, isSelected);
        }
        public void UpdateNodeCheck(string id, bool isChecked)
        {
            if (SelectMode == SelectMode.M)
                _dTree.UpdateNodeCheck(id, isChecked);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            _dTree.UpdateData(Data as IEnumerable<object>);
            Refresh();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            //Refresh();
            _dTree = new DTreeHandler(Data as IEnumerable<object>, StateHasChanged, RootValue, ParentField, TextField, IdField, SelectedField, SelectMode, IdExpression, TextExpression, ChildrenExpression);
        }

        private void Refresh()
        {
            _dTree.RefreshTree();
            if (AutoExpandAll)
                _dTree.ExpandAll();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }
    }
}
