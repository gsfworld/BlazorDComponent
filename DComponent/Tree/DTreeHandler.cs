using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DComponent
{
    public class DTreeHandler
    {
        private IEnumerable<object> _data;
        private readonly StateDelegate.StateHasChanged _stateUpdater;
        public string _rootValue;
        public string _parentField;
        public string _textFiled;
        public string _idField;
        public string _selectedField;
        public DTreeFieldMode _fieldMode;
        public Expression<Func<object, string>> _textExpression { get; set; }
        public Expression<Func<object, string>> _IdExpression { get; set; }        
        public Expression<Func<object, IEnumerable<object>>> _childrenExpression { get; set; }
        public SelectMode _selectMode;
        public PropertyInfo[] _dataProps { get; set; }
        public List<DTreeNode> TreeNodeData { get; set; }
        public DTreeHandler(IEnumerable<object> data,
            StateDelegate.StateHasChanged stateHasChanged,
            string rootValue,
            string parentField,
            string textFiled,
            string idField,
            string selectedField,
            SelectMode selectMode,
            Expression<Func<object, string>> IdExpression,            
            Expression<Func<object, string>> TextExpression,
            Expression<Func<object, IEnumerable<object>>> ChildrenExpression
            )
        {
            _data = data;
            _stateUpdater = stateHasChanged;
            _rootValue = rootValue;
            _parentField = parentField;
            _textFiled = textFiled;
            _idField = idField;
            _selectedField = selectedField;            
            _selectMode = selectMode;
            _dataProps = data.GetType().GetGenericArguments()[0].GetProperties();
            _IdExpression = IdExpression;
            _textExpression = TextExpression;
            _childrenExpression = ChildrenExpression;
            RefreshTree();
        }

        public void RefreshTree()
        {
            TreeNodeData = new List<DTreeNode>();
            if (_IdExpression != null && _textExpression != null && _childrenExpression != null)
            {
                BuildTreeExpression(_data);
                _fieldMode = DTreeFieldMode.E;
            }

            if (!string.IsNullOrEmpty(_idField) && !string.IsNullOrEmpty(_textFiled) && !string.IsNullOrEmpty(_parentField))
            {
                BuildTree(_data);
                _fieldMode = DTreeFieldMode.F;
            }
            //_stateUpdater.Invoke();
        }
        public void UpdateData(IEnumerable<object> udata)
        {
            if (udata == null) return;
            _data = udata;
            //RefreshTree();            
            //_stateUpdater.Invoke();
        }

        //只有单选调用
        public void UpdateNodeSelect(string id, bool isSelected)
        {
            //if (_selectMode == SelectMode.M) return;
            var selectNode = TreeNodeData.First(p => p.Id == id);
            if (isSelected)
                ExpandToNodeNoRender(selectNode.Id);
            selectNode.Selected = isSelected;
            SetTagSelectedValue(selectNode, isSelected);
            foreach (var node in TreeNodeData)
            {
                if (node.Id != id)
                {
                    node.Selected = false;
                    SetTagSelectedValue(node, false);
                }
            }
            //_stateUpdater.Invoke();
        }

        private void SetTagSelectedValue(DTreeNode node, bool isSelected)
        {
            if (!string.IsNullOrEmpty(_selectedField))
                node.Tag.GetType().GetProperty(_selectedField).SetValue(node.Tag, isSelected);
        }

        public void UpdateNodeExpand(string id, bool isExpanded)
        {
            var expandNode = TreeNodeData.First(p => p.Id == id);
            if (isExpanded)
                ExpandToNodeNoRender(expandNode.Id);
            else
                CollapseToNodeNoRender(expandNode.Id);
            //_stateUpdater.Invoke();
        }

        public void ExpandToNode(string id)
        {
            ExpandToNodeNoRender(id);
            //_stateUpdater.Invoke();
        }

        private void ExpandToNodeNoRender(string id)
        {
            var expandNode = TreeNodeData.First(p => p.Id == id);
            expandNode.Expanded = true;
            expandNode.IsVisible = true;
            var updateNodes = SearchNodeParents(expandNode);
            foreach (var updateNode in updateNodes)
            {
                updateNode.Expanded = true;
                updateNode.IsVisible = true;
            }
            if (expandNode.ChirdenNodes != null)
            {
                foreach (var childNode in expandNode.ChirdenNodes)
                    childNode.IsVisible = true;
            }
        }

        public void ExpandAll()
        {
            foreach (var updateNode in TreeNodeData)
            {
                updateNode.Expanded = true;
                updateNode.IsVisible = true;
            }
            //_stateUpdater.Invoke();
        }

        public void CollapseAll()
        {
            foreach (var updateNode in TreeNodeData)
            {
                updateNode.Expanded = false;
                if (updateNode.Level != 0)
                    updateNode.IsVisible = false;
            }

            //_stateUpdater.Invoke();
        }

        public void CollapseToNode(string id)
        {
            CollapseToNodeNoRender(id);
            //_stateUpdater.Invoke();
        }

        private void CollapseToNodeNoRender(string id)
        {
            var collapseNode = TreeNodeData.First(p => p.Id == id);
            collapseNode.Expanded = false;
            var updateNodes = SearchNodeChildren(collapseNode);
            foreach (var updateNode in updateNodes)
            {
                updateNode.Expanded = false;
                updateNode.IsVisible = false;
            }
        }

        private void UpdateNodeCheckAllNoRender(string id, bool isChecked)
        {
            if (_selectMode == SelectMode.S) return;
            var checkNode = TreeNodeData.First(p => p.Id == id);
            SetTagSelectedValue(checkNode, isChecked);
            var updateNodes = SearchNodeChildren(checkNode);
            foreach (var updateNode in updateNodes)
            {
                updateNode.Selected = isChecked;
                SetTagSelectedValue(updateNode, isChecked);
            }
        }

        public void UpdateNodeCheckAll(string id, bool isChecked)
        {
            UpdateNodeCheckAllNoRender(id, isChecked);
            //_stateUpdater.Invoke();            
        }

        public void UpdateNodeCheck(string id, bool isChecked)
        {
            UpdateNodeCheckNoRender(id, isChecked);
            //_stateUpdater.Invoke();
        }
        public void UpdateNodeCheckNoRender(string id, bool isChecked)
        {
            if (_selectMode == SelectMode.S) return;
            UpdateNodeCheckAllNoRender(id, isChecked);
            var checkNode = TreeNodeData.First(p => p.Id == id);
            SetTagSelectedValue(checkNode, isChecked);
            checkNode.Selected = isChecked;
            if (isChecked) ExpandToNodeNoRender(id);
            else CollapseToNodeNoRender(id);
            var checkNodeParent = TreeNodeData.FirstOrDefault(p => p.Id == checkNode.ParentId);
            if (checkNodeParent != null)
            {
                if (isChecked)
                {
                    checkNodeParent.Selected = true;
                }
                else
                {
                    checkNodeParent.Selected = !checkNodeParent.ChirdenNodes.All(p => !p.Selected);
                }
                SetTagSelectedValue(checkNodeParent, checkNodeParent.Selected);
            }
            //if (_OnSelectNodesChanged.HasDelegate && isChecked)
            //{
            //    var selectedData = new List<object>();
            //    selectedData.AddRange(TreeNodeData.Where(p => p.Selected).Select(p => p.Tag));
            //    _OnSelectNodesChanged.InvokeAsync(selectedData);
            //}

        }


        public List<DTreeNode> SearchNodeChildren(DTreeNode searchNode)
        {
            var result = new List<DTreeNode>();
            //result.Add(searchNode);//包含自身
            SearchNodeChild(searchNode, result);
            return result;
        }
        private void SearchNodeChild(DTreeNode searchNode, List<DTreeNode> searchNodeResult)
        {
            if (searchNode.ChirdenNodes != null)
            {
                foreach (var node in searchNode.ChirdenNodes)
                {
                    searchNodeResult.Add(node);
                    SearchNodeChild(node, searchNodeResult);
                }
            }
        }

        public List<DTreeNode> SearchNodeParents(DTreeNode searchNode)
        {
            var result = new List<DTreeNode>();
            result.Add(searchNode);//包含自身
            SearchNodeParent(searchNode, result);
            return result;
        }

        private void SearchNodeParent(DTreeNode searchNode, List<DTreeNode> searchNodeResult)
        {
            if (searchNode.ParentId != null || searchNode.ParentId == _rootValue)
            {
                var parent = TreeNodeData.FirstOrDefault(p => p.Id == searchNode.ParentId);
                if (parent!=null)
                {
                    searchNodeResult.Add(parent);
                    SearchNodeChild(parent, searchNodeResult);
                }                
            }
        }

        private void BuildTree(IEnumerable<object> _data)
        {
            if (_dataProps.All(v => v.Name != _idField)) return;
            if (_dataProps.All(v => v.Name != _parentField)) return;
            if (_dataProps.All(v => v.Name != _textFiled)) return;
            //if (_dataProps.All(v => v.Name != _selectedField)) return;
            IEnumerable<object> rootNodeDatas;
            if (!string.IsNullOrEmpty(_rootValue))
                rootNodeDatas = _data.Where(p => p.GetType().GetProperty(_idField).GetValue(p)?.ToString() == _rootValue);
            else
                rootNodeDatas = _data.Where(p => p.GetType().GetProperty(_parentField).GetValue(p) == null);
            foreach (var rootNodeData in rootNodeDatas)
            {
                var rootTreeNode = new DTreeNode
                {
                    Id = _dataProps.First(p => p.Name == _idField).GetValue(rootNodeData)?.ToString(),
                    Text = _dataProps.First(p => p.Name == _textFiled).GetValue(rootNodeData)?.ToString(),
                    Selected = string.IsNullOrEmpty(_selectedField) ? false : (bool)_dataProps.First(p => p.Name == _selectedField).GetValue(rootNodeData),
                    Level = 0,
                    ParentId = _dataProps.First(p => p.Name == _parentField).GetValue(rootNodeData)?.ToString(),
                    Tag = rootNodeData,
                    Expanded = false,
                    IsVisible = true
                };
                TreeNodeData.Add(rootTreeNode);
                BuidTreeChildren(rootTreeNode, _data);
            }
        }
        private void BuidTreeChildren(DTreeNode parentNode, IEnumerable<object> _data)
        {
            var childNodeData = _data.Where(p => p.GetType().GetProperty(_parentField).GetValue(p)?.ToString() == parentNode.Id);
            if (childNodeData.Count() > 0)
            {
                parentNode.ChirdenNodes = new List<DTreeNode>();
                foreach (var childNode in childNodeData)
                {
                    var childTreeNode = new DTreeNode
                    {
                        Id = _dataProps.First(p => p.Name == _idField).GetValue(childNode)?.ToString(),
                        Text = _dataProps.First(p => p.Name == _textFiled).GetValue(childNode)?.ToString(),
                        Selected = string.IsNullOrEmpty(_selectedField) ? false : (bool)_dataProps.First(p => p.Name == _selectedField).GetValue(childNode),
                        Level = parentNode.Level + 1,
                        ParentId = _dataProps.First(p => p.Name == _parentField).GetValue(childNode)?.ToString(),
                        Tag = childNode,
                        Expanded = false,
                        IsVisible = false
                    };
                    parentNode.ChirdenNodes.Add(childTreeNode);
                    TreeNodeData.Add(childTreeNode);
                    BuidTreeChildren(childTreeNode, _data);
                }
            }
        }

        private void BuildTreeExpression(IEnumerable<object> _data)
        {
            foreach (var rootData in _data)
            {
                var rootNode = new DTreeNode
                {
                    Id = _IdExpression?.Compile()(rootData),
                    Text = _textExpression?.Compile()(rootData),
                    Selected = string.IsNullOrEmpty(_selectedField) ? false : (bool)_dataProps.First(p => p.Name == _selectedField).GetValue(rootData),
                    Level = 0,
                    ParentId =_rootValue,
                    Tag = rootData,
                    Expanded = false,
                    IsVisible = true
                };
                TreeNodeData.Add(rootNode);
                BuildTreeExpressionChildren(rootNode);
            }
        }

        private void BuildTreeExpressionChildren(DTreeNode parentNode)
        {
            var childNodeData = _childrenExpression?.Compile()(parentNode.Tag);
            if (childNodeData != null && childNodeData.Count() > 0)
            {
                parentNode.ChirdenNodes = new List<DTreeNode>();
                foreach (var childNode in childNodeData)
                {
                    var childTreeNode = new DTreeNode
                    {
                        Id = _IdExpression?.Compile()(childNode),
                        Text = _textExpression?.Compile()(childNode),
                        Selected = string.IsNullOrEmpty(_selectedField) ? false : (bool)_dataProps.First(p => p.Name == _selectedField).GetValue(childNode),
                        Level = parentNode.Level + 1,
                        ParentId = _IdExpression?.Compile()(parentNode.Tag),
                        Tag = childNode,
                        Expanded = false,
                        IsVisible = false
                    };
                    parentNode.ChirdenNodes.Add(childTreeNode);
                    TreeNodeData.Add(childTreeNode);
                    BuildTreeExpressionChildren(childTreeNode);
                }
            }
        }

    }
}
