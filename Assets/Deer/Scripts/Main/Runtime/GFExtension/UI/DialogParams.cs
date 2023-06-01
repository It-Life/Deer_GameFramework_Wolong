// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 16-58-34
//修改作者:杜鑫
//修改时间:2022-06-17 16-58-34
//版 本:0.1 
// ===============================================

using System;
using GameFramework;

namespace Main.Runtime 
{
    /// <summary>
    /// 对话框显示数据。
    /// </summary>
    public class DialogParams:IReference
    {
        /// <summary>
        /// 模式，即按钮数量。取值 1、2、3。
        /// </summary>
        public int Mode
        {
            get;
            set;
        }
        /// <summary>
        /// 是否打开关闭按钮
        /// </summary>
        public bool ShowCloseBtn 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 是否SureNoCanCloseView
        /// </summary>
        public bool SureNoCanCloseView 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 是否BgCanCloseView
        /// </summary>
        public bool BgCanCloseView 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 标题。
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public string Message
        {
            get;
            set;
        }
        
        /// <summary>
        /// 确认按钮文本。
        /// </summary>
        public string ConfirmText
        {
            get;
            set;
        }

        /// <summary>
        /// 确定按钮回调。
        /// </summary>
        public Action<object> OnClickConfirm
        {
            get;
            set;
        }

        /// <summary>
        /// 取消按钮文本。
        /// </summary>
        public string CancelText
        {
            get;
            set;
        }

        /// <summary>
        /// 取消按钮回调。
        /// </summary>
        public Action<object> OnClickCancel
        {
            get;
            set;
        }

        /// <summary>
        /// 中立按钮文本。
        /// </summary>
        public string OtherText
        {
            get;
            set;
        }

        /// <summary>
        /// 其它按钮回调。
        /// </summary>
        public Action<object> OnClickOther
        {
            get;
            set;
        }
        /// <summary>
        /// 背景按钮回调。
        /// </summary>
        public Action<object> OnClickBackground
        {
            get;
            set;
        }
        /// <summary>
        /// 背景按钮回调。
        /// </summary>
        public Action<object> OnClickClose
        {
            get;
            set;
        }
        /// <summary>
        /// 用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            set;
        }

        public void Clear()
        {
            Mode = 0;
            ShowCloseBtn = false;
            SureNoCanCloseView = false;
            BgCanCloseView = false;
            Title = "";
            Message = "";
            ConfirmText = "";
            OnClickConfirm = null;
            CancelText = "";
            OnClickCancel = null;
            OtherText = "";
            OnClickOther = null;
            OnClickBackground = null;
            OnClickClose = null;
            UserData = "";
        }
    }
}