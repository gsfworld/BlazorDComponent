using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DComponent
{
    public class DLayoutHandler
    {
        private readonly StateDelegate.StateHasChanged _stateUpdater;       
        public Dictionary<string, LayoutElement> LayoutElements { get; } = new Dictionary<string, LayoutElement>();

        public DLayoutHandler(StateDelegate.StateHasChanged stateHasChanged)
        {
            _stateUpdater = stateHasChanged;          
        }

        public void UpdatePWdithHeight(int widthp, int heightp)
        {
            //固定值处理
            var fixHeight = LayoutElements.Where(p => p.Value.Height > 0).Sum(p => p.Value.Height);
            heightp -= fixHeight;
            var fixWidth = LayoutElements.Where(p => p.Value.Width > 0).Sum(p => p.Value.Width);
            widthp -= fixWidth;
            //百分比折算px
            var leftH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.L).Sum(p => p.Value.UpdateHeightP);
            var rightH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.R).Sum(p => p.Value.UpdateHeightP);
            var centerH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.C).Sum(p => p.Value.UpdateHeightP);
            var topH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.U).Sum(p => p.Value.UpdateHeightP);
            var downH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.D).Sum(p => p.Value.UpdateHeightP);
            var leftW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.L).Sum(p => p.Value.UpdateWidthP);
            var rightW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.R).Sum(p => p.Value.UpdateWidthP);
            var centerW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.C).Sum(p => p.Value.UpdateWidthP);
            var topW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.U).Sum(p => p.Value.UpdateWidthP);
            var downW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.D).Sum(p => p.Value.UpdateWidthP);
            foreach ((string id, LayoutElement element) in LayoutElements)
            {                
                switch (element.ElementType)
                {
                    case LayoutElementType.U:                        
                        element.WidthP = widthp;
                        element.HeightP = element.Height > 0 ? element.Height : element.UpdateHeightP * heightp / 100;
                        break;
                    case LayoutElementType.D:
                        element.WidthP = widthp;
                        element.HeightP = element.Height > 0 ? element.Height : element.UpdateHeightP * heightp / 100;
                        break;
                    case LayoutElementType.C:
                        element.WidthP = (100 - leftW - rightW) * widthp / 100;
                        element.HeightP = (100 - topH - downH) * heightp / 100;
                        break;
                    case LayoutElementType.L:
                        element.WidthP = element.Width > 0 ? element.Width : element.UpdateWidthP * widthp / 100;
                        element.HeightP = (100 - topH - downH) * heightp / 100;
                        break;
                    case LayoutElementType.R:
                        element.WidthP = element.Width > 0 ? element.Width : element.UpdateWidthP * widthp / 100;
                        element.HeightP = (100 - topH - downH) * heightp / 100;
                        break;
                }
            }
            //实际px尺寸 计算顺序u,l,r,c,d
            leftH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.L).Sum(p => p.Value.HeightP);
            rightH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.R).Sum(p => p.Value.HeightP);
            centerH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.C).Sum(p => p.Value.HeightP);
            topH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.U).Sum(p => p.Value.HeightP);
            downH = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.D).Sum(p => p.Value.HeightP);
            leftW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.L).Sum(p => p.Value.WidthP);
            rightW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.R).Sum(p => p.Value.WidthP);
            centerW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.C).Sum(p => p.Value.WidthP);
            topW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.U).Sum(p => p.Value.WidthP);
            downW = LayoutElements.Where(p => p.Value.ElementType == LayoutElementType.D).Sum(p => p.Value.WidthP);
            foreach ((string id, LayoutElement element) in LayoutElements.OrderBy(p => p.Value.ElementType))
            {

                switch (element.ElementType)
                {
                    case LayoutElementType.U:
                        element.Left = 0;
                        element.Top = 0;
                        break;
                    case LayoutElementType.D:
                        element.Left = 0;
                        element.Top = topH + centerH;
                        break;
                    case LayoutElementType.C:
                        element.Left = leftW;
                        element.Top = topH;
                        break;
                    case LayoutElementType.L:
                        element.Left = 0;
                        element.Top = topH;
                        break;
                    case LayoutElementType.R:
                        element.Left = leftW + centerW;
                        element.Top = topH;
                        break;
                }

            }
            _stateUpdater.Invoke();
        }

        public void UpdateElementWdithHeight(string id, int widthP, int heightP)
        {
            if (!LayoutElements.ContainsKey(id)) return;
            LayoutElements[id].UpdateWidthP = widthP;
            LayoutElements[id].UpdateHeightP = heightP;
            _stateUpdater.Invoke();
        }
        public void InitElement(LayoutElement element)
        {
            if (LayoutElements.ContainsKey(element.Id)) return;
            element.SourceWidthP = element.WidthP;
            element.SourceHeightP = element.HeightP;
            element.UpdateWidthP = element.WidthP;
            element.UpdateHeightP = element.HeightP;
            LayoutElements.Add(element.Id, element);
        }        

    }




}
