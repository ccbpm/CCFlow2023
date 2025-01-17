﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Sys
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum CCBPMRunModel
    {
        /// <summary>
        /// 单机版
        /// </summary>
        Single,
        /// <summary>
        /// 集团模式
        /// </summary>
        GroupInc,
        /// <summary>
        /// 多租户模式
        /// </summary>
        SAAS
    }
    /// <summary>
    /// 组织结构类型
    /// </summary>
    public enum OSModel
    {
        /// <summary>
        /// 一个人一个部门模式.
        /// </summary>
        OneOne,
        /// <summary>
        /// 一个人多个部门模式
        /// </summary>
        OneMore
    }
    
    /// <summary>
    /// 运行平台
    /// </summary>
    public enum Plant
    {
        /// <summary>
        /// 默认不打开.
        /// </summary>
        CSharp,
        /// <summary>
        /// 打开
        /// </summary>
        Java
    }
    /// <summary>
    /// 按日期查询方式
    /// </summary>
    public enum DTSearchWay
    {
        /// <summary>
        /// 不设置
        /// </summary>
        None,
        /// <summary>
        /// 按日期
        /// </summary>
        ByDate,
        /// <summary>
        /// 按日期时间
        /// </summary>
        ByDateTime,
        /// <summary>
        /// 按照年月
        /// </summary>
        ByYearMonth,
        /// <summary>
        /// 按照年份
        /// </summary>
        ByYear,
    }
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 独立表单
        /// </summary>
        Application = 0,
        /// <summary>
        /// 节点表单
        /// </summary>
        Node = 1
    }
    public enum FrmFrom
    {
        Flow,
        Node,
        Dtl
    }
    /// <summary>
    /// 表单类型 @0=傻瓜表单@1=自由表单@3=嵌入式表单@4=Word表单@5=Excel表单@6=VSTOForExcel@7=Entity
    /// </summary>
    public enum FrmType
    {
        /// <summary>
        /// 傻瓜表单
        /// </summary>
        FoolForm = 0,
        /// <summary>
        /// URL表单(自定义)
        /// </summary>
        Url = 3,
        /// <summary>
        /// Word类型表单
        /// </summary>
        WordFrm = 4,
        /// <summary>
        /// Excel表单
        /// </summary>
        ExcelFrm = 5,
        /// <summary>
        /// VSTOExccel模式.
        /// </summary>
        VSTOForExcel = 6,
        /// <summary>
        /// 实体类
        /// </summary>
        Entity = 7,
        /// <summary>
        /// 开发者表单
        /// </summary>
        Develop = 8,
        /// <summary>
        /// WPS表单
        /// </summary>
        WPSFrm = 9,
        /// <summary>
        /// 外部数据源列表
        /// </summary>
        DBList = 100,
        /// <summary>
        /// 章节表单
        /// </summary>
        ChapterFrm= 10
    }
}
