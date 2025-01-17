﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.FrmUI;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程进度图
    /// </summary>
    public class ExtJobSchedule : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 目标
        /// </summary>
        public string Target
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag1);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag1, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string URL
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.Tag2).Replace("#", "@");
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag2, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.Name);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Name, value);
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                if (BP.Web.WebUser.No.Equals("admin")==true)
                {
                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 流程进度图
        /// </summary>
        public ExtJobSchedule()
        {
        }
        /// <summary>
        /// 流程进度图
        /// </summary>
        /// <param name="mypk"></param>
        public ExtJobSchedule(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_MapAttr", "流程进度图");
                map.DepositaryOfEntity= Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                

                #region 通用的属性.
                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段", true, true, 1, 100, 20);
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                //map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                //    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                //map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                map.AddTBFloat(MapAttrAttr.UIHeight, 1, "高度", true, false);
                map.AddTBFloat(MapAttrAttr.UIWidth, 1, "宽度", true, false);

                map.AddTBString(MapAttrAttr.Name, null, "名称", true, false, 0, 500, 20, true);
                #endregion 通用的属性.

                #region 个性化属性.
               // map.AddTBString(MapAttrAttr.Tag1, "_blank", "连接目标(_blank,_parent,_self)", true, false, 0, 20, 20);
               // map.AddTBString(MapAttrAttr.Tag2, null, "URL", true, false, 0, 500, 20, true);
                #endregion 个性化属性.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程进度图s
    /// </summary>
    public class ExtJobSchedules : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 流程进度图s
        /// </summary>
        public ExtJobSchedules()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ExtJobSchedule();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtJobSchedule> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtJobSchedule>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtJobSchedule> Tolist()
        {
            System.Collections.Generic.List<ExtJobSchedule> list = new System.Collections.Generic.List<ExtJobSchedule>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtJobSchedule)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
