﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
      
    /// <summary>
    /// 外键字段
    /// </summary>
    public class MapAttrSFTable : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData); 
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 绑定的枚举ID
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIBindKey, value);
            }
        }
        #endregion
        
        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 外键字段
        /// </summary>
        public MapAttrSFTable()
        {
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

                Map map = new Map("Sys_MapAttr", "外键字段");
                map.IndexField = MapAttrAttr.FK_MapData;

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20,true);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20, true);

                //默认值.
                map.AddDDLSysEnum(MapAttrAttr.LGType, 4, "类型", true, false);
                map.AddTBString(MapAttrAttr.UIBindKey, null, "外键SFTable", true, true, 0, 100, 20,true);

                //map.AddTBString(MapAttrAttr.DefVal, null, "默认值", true, false, 0, 300, 20);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                //map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, true);


                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见", true, false);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);

                // map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                // map.AddBoolean("IsEnableJS", false, "是否启用JS高级设置？", true, true); //参数字段.
                #endregion 基本信息.

                #region 傻瓜表单。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT",
                   "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文
                 
                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设置联动";
                rm.ClassMethodName = this.ToString() + ".DoActiveDDL()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置显示过滤";
                rm.ClassMethodName = this.ToString() + ".DoAutoFullDLL()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

            
                rm = new RefMethod();
                rm.Title = "填充其他控件";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "外键表属性";
                rm.ClassMethodName = this.ToString() + ".DoSFTable()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.GroupName = "高级";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "高级JS设置";
                rm.ClassMethodName = this.ToString() + ".DoRadioBtns()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
             
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);


            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);
            base.afterDelete();
        }

        #region 方法执行.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn + "&T=" + DateTime.Now.ToString();
        }
        /// <summary>
        /// 外键表属性
        /// </summary>
        /// <returns></returns>
        public string DoSFTable()
        {
            return "../../Comm/En.htm?EnName=BP.Sys.SFTable&No=" + this.UIBindKey;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoRadioBtns()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FrmID + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置填充其他下拉框
        /// </summary>
        /// <returns></returns>

        public string DoDDLFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl2019.htm?FK_MapData=" + this.FrmID + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }

        /// <summary>
        /// 设置下拉框显示过滤
        /// </summary>
        /// <returns></returns>
        public string DoAutoFullDLL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDLL.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ActiveDDL.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrSFTables : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrSFTables()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrSFTable();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrSFTable> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrSFTable>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrSFTable> Tolist()
        {
            System.Collections.Generic.List<MapAttrSFTable> list = new System.Collections.Generic.List<MapAttrSFTable>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrSFTable)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
