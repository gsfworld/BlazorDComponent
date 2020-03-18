using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DComponent
{
    public partial class DFormLayout
    {
        private DFormLayoutHandler _dFormLayout;
        [Parameter]
        public string Header { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
        private RenderFragment CustomComponent() => builder =>
        {            
            builder.OpenElement(0, "form");
            builder.AddAttribute(1, "class", "form-inline formlayout");
            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "row");
            foreach ((string id, FormLayoutItem item) in _dFormLayout.LayoutElements)
            {
                builder.OpenElement(4, "div");
                builder.AddAttribute(5, "class", $"col-{item.ColSpan}");     
                
                builder.OpenElement(6, "div");
                  builder.AddAttribute(7, "class", "form-group");     
                
                  builder.OpenElement(8, "lable");
                  builder.AddAttribute(9, "class", "control-label");
                  builder.AddContent(11, $"{item.Caption}:");
                  builder.CloseElement();

                  builder.OpenElement(8, "div");
                  builder.AddAttribute(9, "class", "");
                  builder.AddContent(10, item.Template);               
                  builder.CloseElement();  
                
                 builder.CloseElement();
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();
        };

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _dFormLayout = new DFormLayoutHandler();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender) StateHasChanged();
        }       

    }
}
