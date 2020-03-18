using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DComponent
{
    public static class StateDelegate
    {
        public delegate void StateHasChanged();
    }

    public class Column
    {
        public PropertyInfo Property;
        public char? SortDir;//排序值 asc:a desc:d
        public string FilterValue;//过滤值
        public ColumnType CType;//显示组件类型
        public string Caption;
        public string CFormate;  //日期类型显示格式，默认yyyy-MM-dd HH:mi:ss      
        public string DisplayField;
        public string ValueField;
        public string CWidth;
        public string CheckValue;//checkbox为checked值，只支持Y
        public string UnCheckValue;//checkbox为unchecked值，只支持N
        public IEnumerable<object> CData;//lookupEdit数据列表
        public Type DataType;//lookupEdit原始对象类型
        public ColumnFilterCondition FCondition = ColumnFilterCondition.C;//过滤条件
        public RenderFragment HerderContext;
        public RenderFragment<object> CommandContext;
        public bool ShowFilter;//列显示过滤，默认lookupedit列显示
        public string FooterMark;
        public ColumnStateType FooterStateType;
        public bool IsGroup;//
        public string GroupMark;
        public string GroupStateFieldName;
        public ColumnStateType GroupStateType;
        public RenderFragment<object> CustomContext;
        public FieldEditMode EditMode = FieldEditMode.Normal;
    }

    public class FilterCondition
    {
        public static List<FCondition> FConditionData = new List<FCondition>
        {
            new FCondition{FName="小于",FValue=ColumnFilterCondition.L},
            new FCondition{FName="大于",FValue=ColumnFilterCondition.M},
            new FCondition{FName="等于",FValue=ColumnFilterCondition.E},
            new FCondition{FName="包含",FValue=ColumnFilterCondition.C},
        };
    }
    public class FCondition
    {
        public string FName { get; set; }
        public ColumnFilterCondition FValue { get; set; }
    }

    public enum ColumnStateType : int
    {
        S = 0,//sum       
        C = 1 //count
    }
    public enum ColumnFilterCondition : int
    {
        L = 0,//小于
        M = 1,//大于
        E = 2,//等于
        C = 3 //包含
    }
    public enum ColumnType : int
    {
        T = 0,//text
        D = 1,//datetime
        C = 2,//checkbox
        L = 3, //lookup
        O = 4, //command
        U = 5, //Custom
        S = 5 //Spin
    }

    public enum SelectMode : int
    {
        S = 0,//单选
        M = 1//多选
    }

    public class LayoutElement
    {
        public LayoutElementType ElementType;
        public string Id;
        public int WidthP;
        public int HeightP;
        public int Height;
        public int Width;
        public int SourceWidthP;
        public int SourceHeightP;
        public int UpdateWidthP;
        public int UpdateHeightP;
        public int Left;
        public int Top;
        public bool Spliter;
        public string Header;
        public RenderFragment ElementContext;
    }

    public enum LayoutElementType : int
    {
        U = 0,//up
        L = 1,//left
        R = 2,//right
        C = 3, //center
        D = 4, //down
    }

    public class DTreeNode
    {
        public string Id;
        public string Text;
        public bool Selected;
        public int Level;
        public string ParentId;
        public object Tag;
        public bool Expanded;
        public bool IsVisible;
        public List<DTreeNode> ChirdenNodes;
    }

    public enum DTreeFieldMode : int
    {
        F = 0,//field
        E = 1,//expression
        O = 2, //none
    }

    public enum DDateType : int
    {
        D = 0,//date
        DT = 1,//dattime        
    }

    public enum TabType
    {
        Normal = 0,
        Card = 1,
        BorderCard = 2
    }

    public enum TabPosition
    {
        Top,
        Left,
        Right,
        Bottom
    }

    public enum FieldEditMode
    {
        Normal = 0,
        ReadOnly = 1,
        No = 2
    }

    public enum RowEditMode
    {
        New = 0,
        Edit = 1,
        None = 3
    }

    public class SpinValueType
    {
        public static string sZh = @"[^\u4e00-\u9fa5]";
        public static string sEn = @"[^a-zA-Z]";
        public static string sInt = @"/\D";
        public static string sEnInt = @"[^\d|chun]";
        public static string sEnZh = @"[\d]";
        public static string sDecimal = @"^(0|[1-9]\d{0,17})(\.\d{0,4})?$";
    }
}
