using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace DComponent
{
    public partial class DLookupEdit<TItem>
    {
        [Parameter]
        public List<TItem> Data { get; set; }
        [Parameter]
        public string DisplayField { get; set; }
        [Parameter]
        public string ValueField { get; set; }
        [Parameter]
        public string SelectedValue { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public string Width { get; set; } = "100%";
        [Parameter]
        public string Class { get; set; }
        [Parameter]
        public string style { get; set; }
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public EventCallback<string> SelectedValueChanged { get; set; }
        public string _SelectedValue
        {
            get => SelectedValue;
            set
            {
                SelectedValue = value;
                if (value == "0") return;
                if (SelectedValueChanged.HasDelegate)
                    SelectedValueChanged.InvokeAsync(value);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Id))
                Id = $"DC{Guid.NewGuid().ToString().Replace("-", "")}";
        }
    }
}
