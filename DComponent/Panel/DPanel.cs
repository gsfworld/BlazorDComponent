using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DComponent
{
    public partial class DPanel: IDComponent
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public RenderFragment HeaderContent { get; set; }
        [Parameter]
        public RenderFragment BodyContent { get; set; }
        [Parameter]
        public RenderFragment FooterContent { get; set; }
        [Inject]
        private IJSRuntime js { get; set; }
        private int height;
        private string headerId;
        private string footerId;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Id))
            {
                height = 0;
                Id = $"DC{Guid.NewGuid().ToString().Replace("-", "")}";
                headerId = $"header{Id}";
                footerId = $"footer{Id}";
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();            
        }

        public async Task Refresh()
        {
            var bodyElements = await DcomponentJsInterop.Select(js, $"#{Id}");
            if (bodyElements != null && bodyElements.Count > 0)
            {
                var elementHeight = bodyElements[0].OffsetHeight;
                var headerElements = await DcomponentJsInterop.Select(js, $"#{headerId}");
                if (headerElements != null && headerElements.Count > 0)
                    elementHeight = elementHeight - headerElements[0].OffsetHeight;
                var footerElements = await DcomponentJsInterop.Select(js, $"#{footerId}");
                if (footerElements != null && footerElements.Count > 0)
                    elementHeight = elementHeight - footerElements[0].OffsetHeight;
                height = elementHeight;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Refresh();
            }
            await base.OnAfterRenderAsync(firstRender);
        }        
    }
}
