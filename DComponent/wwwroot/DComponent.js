window.DComponent = {
    SelectElement: (Id) => {
        var element;
        if (Id == "window")
            element = $(window);;
        if (Id == "document")
            element = $(document);
        if (!element)
            element = $(Id);
        if (element) {
            var data = element.map(function (n, i) {
                return {
                    ClassName: $(this).attr('class'),
                    Id: $(this).attr('id'),
                    Name: $(this).attr('name'),
                    Value: $(this).attr('value'),
                    Text: $(this).text(),
                    ClientWidth: $(this).innerWidth(),
                    ClientHeight: $(this).innerHeight(),
                    OffsetWidth: $(this).outerWidth(),
                    OffsetHeight: $(this).outerHeight()
                };
            }).get();
            return JSON.stringify(data);
        }
        return "";
    },
    LayoutCheckIsChild: (layoutId) => {
        return $(layoutId).parents('.layout').length > 0;        
    },
    LayoutParentRender: function (layoutId, dotnetHelper) {
        var layoutParent = $(layoutId).parent();
        var interval = setInterval(function () {
            var layoutParentHeight = $(layoutParent).height();
            if (layoutParentHeight > 0) {
                clearInterval(interval);
                dotnetHelper.invokeMethodAsync('LayoutParentRenderHandler', $(layoutParent).width(), $(layoutParent).height());
            }
        }, 100);
        
    },
    InitLayout: (layoutId, dotnetHelper)=> {
        window.onresize = function () {
            dotnetHelper.invokeMethodAsync('LayoutResizeHandler');        
        };
        setTimeout(function () {
            dotnetHelper.invokeMethodAsync('LayoutReadyHandler');   
        }, 200);
    },
}