﻿using Battery.Framework.Core.Attributes;

namespace Battery.Framework.Core.Controllers
{
    [WeixinInternalRequest("该页面只能在微信中查看", Order = 0)]//只能在微信中查看
    [InjectionWxJsAttribute(Order=40)]
    public class WxBaseController : ContextController
    {
        public WxBaseController()
        {   
        }
    }
}
