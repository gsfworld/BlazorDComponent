using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DComponent
{
    public class DcomponentJsInterop
    {
        public static async Task<List<DomElement>> Select(IJSRuntime JSRuntime, string id)
        {
            var data = await JSRuntime.InvokeAsync<string>("DComponent.SelectElement",id);
            if (!string.IsNullOrEmpty(data))
                return JsonSerializer.Deserialize<List<DomElement>>(data);
            return new List<DomElement>();
        }
    }

    public class DomElement
    {
        public string Id { get; set; }
        public string ClassName { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public int ClientWidth { get; set; }
        public int ClientHeight { get; set; }
        public int OffsetWidth { get; set; }
        public int OffsetHeight { get; set; }
    }
}
