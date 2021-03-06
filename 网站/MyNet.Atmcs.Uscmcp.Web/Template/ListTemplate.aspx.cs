﻿// ***********************************************************************
// Assembly         : MyNet.Atmcs.Uscmcp.Web
// Author           : zlsyl
// Created          : 06-24-2016
//
// Last Modified By : zlsyl
// Last Modified On : 08-15-2016
// ***********************************************************************
// <copyright file="ListTemplate.aspx.cs" company="ZKLT">
//     Copyright (c) ZKLT. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Ext.Net;
using MyNet.Atmcs.Uscmcp.Bll;
using MyNet.Common.Log;
using MyNet.Atmcs.Uscmcp.Model;

/// <summary>
/// The Web namespace.
/// </summary>
namespace MyNet.Atmcs.Uscmcp.Web
{
    /// <summary>
    /// Class ListTemplate.
    /// </summary>
    public partial class ListTemplate : System.Web.UI.Page
    {
        #region 成员变量

        /// <summary>
        /// The setting manager
        /// </summary>
        private SettingManager settingManager = new SettingManager();
        private MyNet.Atmcs.Uscmcp.Bll.LogManager logManager = new Bll.LogManager(); 
        /// <summary>
        /// The user login
        /// </summary>
        private UserLogin userLogin = new UserLogin();

        #endregion 成员变量

        #region 事件

        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = Request.QueryString["username"]; if (!userLogin.CheckLogin(username)) { string js = "alert('您没有登录或操作超时，请重新登录!');window.top.location.href='" +StaticInfo.LoginPage + "'"; System.Web.HttpContext.Current.Response.Write("<script type='text/javascript'>" + js + "</script>"); return; }
            if (!X.IsAjaxRequest)
            {
                InitForm();
            }
        }

        /// <summary>
        /// 设定首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void TemplateGroup_Event(object sender, DirectEventArgs e)
        {
            try
            {
                RadioGroup r = sender as RadioGroup;
                foreach (Radio item in r.Items)
                {
                    if (item.Checked)
                    {
                        string TemplateCode = Bll.Common.GetTemplateCode(item.InputValue);

                        UserInfo userinfo = Session["userinfo"] as UserInfo;
                        int res = settingManager.UpdateTemplatePageInfo("FirstTemplate", TemplateCode, userinfo.UserCode);
                        if (res > 0)
                        {
                            userinfo.FirstTemplate = TemplateCode;
                            Session["userinfo"] = null;
                            Session["userinfo"] = userinfo;
                            X.Msg.Alert("提示", "首页修改已完成，将在下次登录时生效！").Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.WriteErrorLog(ex);
                logManager.InsertLogError("ListTemplate.aspx-TemplateGroup_Event", ex.Message+"；"+ex.StackTrace, "TemplateGroup_Event has an exception");
            }
        }

        #endregion 事件

        #region 私有方法

        /// <summary>
        /// 加载绑定的模版块
        /// </summary>
        /// <returns></returns>
        protected void InitForm()
        {
            try
            {
                UserInfo userinfo = Session["userinfo"] as UserInfo;
                if (userinfo != null)
                {
                    string mark = "AutoLoadPanel('ListTemplate1','" + ConvertUrl(userinfo.ListTemplate1) + "');";
                    this.ResourceManager1.RegisterAfterClientInitScript(mark);
                    mark = "AutoLoadPanel('ListTemplate2','" + ConvertUrl(userinfo.ListTemplate2) + "');";
                    this.ResourceManager1.RegisterAfterClientInitScript(mark);
                    mark = "AutoLoadPanel('ListTemplate3','" + ConvertUrl(userinfo.ListTemplate3) + "');";
                    this.ResourceManager1.RegisterAfterClientInitScript(mark);
                    if (!string.IsNullOrEmpty(userinfo.FirstTemplate))
                    {
                        foreach (Radio item in this.RadioGroupTemplate.Items)
                        {
                            if (item.InputValue.Equals(Bll.Common.GetTemplateMs(userinfo.FirstTemplate)))
                            {
                                item.Checked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.WriteErrorLog(ex);
                logManager.InsertLogError("ListTemplate.aspx-InitForm", ex.Message+"；"+ex.StackTrace, "InitForm has an exception");
            }
        }

        /// <summary>
        /// 选择模版保存事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void ButOKClick(object sender, DirectEventArgs e)
        {
            try
            {
                if (Session["templatepage"] != null)
                {
                    TemplateInfo templateInfo = Session["templatepage"] as TemplateInfo;
                    UserInfo userinfo = Session["userinfo"] as UserInfo;
                    int res = settingManager.UpdateTemplatePageInfo(ButData.Value.ToString(), templateInfo.TemplateId, userinfo.UserCode);
                    if (res > 0)
                    {
                        switch (ButData.Value.ToString())
                        {
                            case "ListTemplate1":
                                userinfo.ListTemplate1 = templateInfo.TemplatePage;
                                break;

                            case "ListTemplate2":
                                userinfo.ListTemplate2 = templateInfo.TemplatePage;
                                break;

                            case "ListTemplate3":
                                userinfo.ListTemplate3 = templateInfo.TemplatePage;
                                break;
                        }
                        Session["userinfo"] = null;
                        Session["userinfo"] = userinfo;
                        Window1.Hide();
                        InitForm();
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.WriteErrorLog(ex);
                logManager.InsertLogError("ListTemplate.aspx-ButOKClick", ex.Message+"；"+ex.StackTrace, "ButOKClick has an exception");
            }
        }

        /// <summary>
        /// 转换 url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string ConvertUrl(string url)
        {
            if (url.IndexOf("pages/collect") >= 0)
            {
                try
                {
                    string itgsurl = System.Configuration.ConfigurationManager.AppSettings["itgs"].ToString();
                    if (!String.IsNullOrEmpty(itgsurl))
                    {
                        if (!itgsurl.Substring(itgsurl.Length - 1).Equals("/"))
                        {
                            itgsurl = itgsurl + "/";
                        }
                        if (url.IndexOf(itgsurl) >= 0)
                        {
                            return url;
                        }
                        if (url.Substring(0, 1).Equals("/"))
                        {
                            url = url.Substring(1);
                        }
                        return itgsurl + url;
                    }
                    return url;
                }
                catch (Exception ex)
                {
                    ILog.WriteErrorLog(ex);
                    logManager.InsertLogError("ListTemplate.aspx-ConvertUrl", ex.Message+"；"+ex.StackTrace, "ConvertUrl has an exception");
                    return url;
                }
            }
            return url;
        }

        #endregion 私有方法
    }
}