﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys
{
    public class MapExtXmlList
    {
        /// <summary>
        /// 转载数据
        /// </summary>
        public const string FullData = "FullData";
        public const string FullDataDtl = "FullDataDtl";
        /// <summary>
        /// 求和
        /// </summary>
        public const string AutoFull = "AutoFull";
        /// <summary>
        /// 对从表列求和
        /// </summary>
        public const string AutoFullDtlField = "AutoFullDtlField";
        /// <summary>
        /// 自动填充
        /// </summary>
        public const string ActiveDDL = "ActiveDDL";
        /// <summary>
        /// 查询条件级联关系.
        /// </summary>
        public const string ActiveDDLSearchCond = "ActiveDDLSearchCond";
        /// <summary>
        /// 输入验证
        /// </summary>
        public const string InputCheck = "InputCheck";
        /// <summary>
        /// 文本框自动填充
        /// </summary>
        public const string TBFullCtrl = "TBFullCtrl";
        /// <summary>
        /// Pop返回值
        /// </summary>
        public const string PopVal = "PopVal";
        /// <summary>
        /// Func
        /// </summary>
        public const string Func = "Func";
        /// <summary>
        /// (动态的)填充下拉框
        /// </summary>
        public const string AutoFullDLL = "AutoFullDLL";
        /// <summary>
        /// 查询条件的自动填充
        /// </summary>
        public const string AutoFullDLLSearchCond = "AutoFullDLLSearchCond";
        /// <summary>
        /// 下拉框自动填充
        /// </summary>
        public const string DDLFullCtrl = "DDLFullCtrl";
        /// <summary>
        /// 表单装载填充
        /// </summary>
        public const string PageLoadFull = "PageLoadFull";
        /// <summary>
        /// 主表的装载填充
        /// </summary>
        public const string PageLoadFullMainTable = "PageLoadFullMainTable";
        /// <summary>
        /// 从表的装载填充
        /// </summary>
        public const string PageLoadFullDtl = "PageLoadFullDtl";
        /// <summary>
        /// 下拉框的装载填充
        /// </summary>
        public const string PageLoadFullDDL = "PageLoadFullDDL";
        /// <summary>
        /// 发起流程
        /// </summary>
        public const string StartFlow = "StartFlow";
        /// <summary>
        /// 超链接.
        /// </summary>
        public const string Link = "Link";
        /// <summary>
        /// 自动生成编号
        /// </summary>
        public const string AotuGenerNo = "AotuGenerNo";
        /// <summary>
        /// 正则表达式
        /// </summary>
        public const string RegularExpression = "RegularExpression";
        /// <summary>
        /// 绑定函数
        /// </summary>
        public const string BindFunction = "BindFunction";
        /// <summary>
        /// WordFrm
        /// </summary>
        public const string WordFrm = "WordFrm";
        /// <summary>
        /// ExcelFrm
        /// </summary>
        public const string ExcelFrm = "ExcelFrm";
        /// <summary>
        /// 特别字段特殊用户权限
        /// </summary>
        public const string SepcFiledsSepcUsers = "SepcFiledsSepcUsers";
        /// <summary>
        /// 特别附件特别权限
        /// </summary>
        public const string SepcAthSepcUsers = "SepcAthSepcUsers";
        /// <summary>
        /// pop填充其他控件
        /// </summary>
        public const string PopFullCtrl = "PopFullCtrl";
    }
}
