using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DComponent
{
    public partial class DLayout
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string Class { get; set; }
        [Parameter]
        public string Id { get; set; }
        private DLayoutHandler _dLayout;        
        private bool _renderFinish;
        private bool _isLayoutChild;
        [Inject]
        private IJSRuntime js { get; set; }
        protected override void OnInitialized()
        {           
            base.OnInitialized();
            _renderFinish = false;
            _isLayoutChild = false;
            Id = $"DC{Guid.NewGuid().ToString().Replace("-", "")}";
            _dLayout = new DLayoutHandler(StateHasChanged);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                _isLayoutChild = await js.InvokeAsync<bool>("DComponent.LayoutCheckIsChild", $"#{Id}");
                if (!_isLayoutChild)
                    InitLayout();      
                else
                {                    
                    dotNetObjectRef = dotNetObjectRef ?? DotNetObjectReference.Create(this);
                    await js.InvokeVoidAsync("DComponent.LayoutParentRender", $"#{Id}", dotNetObjectRef);
                }
            }
        }

        private DotNetObjectReference<DLayout> dotNetObjectRef;
        public void InitLayout()
        {
            dotNetObjectRef = dotNetObjectRef ?? DotNetObjectReference.Create(this);
            js.InvokeVoidAsync("DComponent.InitLayout", $"#{Id}", dotNetObjectRef);
        }

        private async Task UpdateLayout(bool firstRender)
        {
            if (_dLayout.LayoutElements.Count > 0)
            {
                var layoutElements = await DcomponentJsInterop.Select(js, $"#{Id}");
                if (layoutElements.Count > 0)
                {
                    var heightp = layoutElements.First().ClientHeight;
                    //if (!_isLayoutChild)
                    //    heightp = heightp - 30;
                    var widthp = layoutElements.First().ClientWidth;
                    _dLayout.UpdatePWdithHeight(widthp-1, heightp-1);
                }
            }
        }

        private async Task UpdateChildLayout(int _parentWidth,int _parentHeight)
        {
            if (_dLayout.LayoutElements.Count > 0)
            {
                _dLayout.UpdatePWdithHeight(_parentWidth - 1, _parentHeight - 1);
            }

        }

        //内嵌不会调用
        [JSInvokable]
        public async Task LayoutResizeHandler()
        {
            await UpdateLayout(false);
        }
        //内嵌不会调用
        [JSInvokable]
        public async Task LayoutReadyHandler()
        {
            _renderFinish = true;
            await UpdateLayout(true);
        }
        [JSInvokable]
        public async Task LayoutParentRenderHandler(int parentWidth,int parentHeight)
        {
            if (_isLayoutChild)
            {
                _renderFinish = true;
                await UpdateChildLayout(parentWidth, parentHeight);
            }
               
        }
        protected override bool ShouldRender()
        {
           return _renderFinish;
        }
    }
}
