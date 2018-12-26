﻿using Battery.DAL.Sys;
using Battery.Framework.Core.Domain;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Battery.Framework.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class InjectionWxJsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null 
                && filterContext.HttpContext != null 
                && filterContext.IsChildAction == false 
                && filterContext.HttpContext.Request.IsAjaxRequest() == false)
            {
                var dicts = DictDAL.GetDicts("Share");
                filterContext.Controller.ViewData["ShareTitle"] = dicts.Where(m => m.ConfigCode == "Slogan").Select(m => m.ConfigValue).FirstOrDefault() ?? "";
                filterContext.Controller.ViewData["ShareDesc"] = dicts.Where(m => m.ConfigCode == "Desc").Select(m => m.ConfigValue).FirstOrDefault() ?? "";
                filterContext.Controller.ViewData["ShareImg"] = dicts.Where(m => m.ConfigCode == "Image").Select(m => m.ConfigValue).FirstOrDefault() ?? "";

                InjectionWxJs(filterContext);
            }
            base.OnActionExecuting(filterContext);
        }

        public void InjectionWxJs(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            filterContext.Controller.ViewData["AppId"] = string.Empty;
            filterContext.Controller.ViewData["Timestamp"] = string.Empty;
            filterContext.Controller.ViewData["NonceStr"] = string.Empty;
            filterContext.Controller.ViewData["Signature"] = string.Empty;
            filterContext.Controller.ViewData["JsTicket"] = string.Empty;
            filterContext.Controller.ViewData["RedirectUrl"] = string.Empty;

            //获取时间戳
            var timestamp = JSSDKHelper.GetTimestamp();
            //获取随机码
            var nonceStr = JSSDKHelper.GetNoncestr();
            string ticket = AccessTokenContainer.TryGetJsApiTicket(WxConfig.AppId, WxConfig.SecretKey);
            //获取签名
            string redirectUrl = request.Url.AbsoluteUri.ToString();
            redirectUrl = Regex.Replace(redirectUrl, @"(:(\d)+)", "");
            var signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, redirectUrl);

            filterContext.Controller.ViewData["AppId"] = WxConfig.AppId;
            filterContext.Controller.ViewData["Timestamp"] = timestamp;
            filterContext.Controller.ViewData["NonceStr"] = nonceStr;
            filterContext.Controller.ViewData["Signature"] = signature;

            filterContext.Controller.ViewData["JsTicket"] = ticket;
            filterContext.Controller.ViewData["RedirectUrl"] = redirectUrl;
        }
    }
}