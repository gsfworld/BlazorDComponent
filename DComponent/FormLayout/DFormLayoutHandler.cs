using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace DComponent
{
    public class DFormLayoutHandler
    {
        public Dictionary<string, FormLayoutItem> LayoutElements { get; } = new Dictionary<string, FormLayoutItem>();
        public DFormLayoutHandler()
        {

        }
    }

    public class FormLayoutItem
    {
        public string Caption { get; set; }
        public int ColSpan { get; set; }
        public int Height { get; set; }
        public RenderFragment Template { get; set; }
    }
}
