﻿using BP.DA;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 扩充Doc字段的长度 的摘要说明
    /// </summary>
    public class DocFieldAddLengthTo1000 : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DocFieldAddLengthTo1000()
        {
            this.Title = "扩充Doc字段的长度";
            this.Help = "为doc类型的字段扩充长度，低于1000的字符扩充为1000.";
            this.Help += "<br>减少因为实施的原因忽略了字符长度导致的界面报错。";
            this.GroupName = "系统维护";

        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
       
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No.Equals("admin")==true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string strs = "开始执行....";
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.MyDataType, DataType.AppString, MapAttrAttr.FK_MapData);
            strs += "<br>@如下字段受到了影响。";
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIHeightInt > 50 && attr.MaxLen < 1000 )
                {
                    strs += " @ 类:" + attr.FrmID + " 字段:" + attr.KeyOfEn + " , " + attr.Name + " "; 
                    attr.setMaxLen(1000);
                    attr.Update();
                }
            }
            return "执行成功..."+strs;
        }
    }
}
