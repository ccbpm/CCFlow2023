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
    /// 实体属性
    /// </summary>
    public class MapAttrCard : EntityMyPK
    {
        #region 文本字段参数属性.
       
  
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
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
        /// 最大长度
        /// </summary>
        public int MaxLen
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MaxLen);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MaxLen, value);
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
        /// 控件类型
        /// </summary>
        public UIContralType UIContralType
        {
            get
            {
                return (UIContralType)this.GetValIntByKey(MapAttrAttr.UIContralType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIContralType, (int)value);
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
        /// 实体属性
        /// </summary>
        public MapAttrCard()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrCard(string myPK)
        {
            this.MyPK = myPK;
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

                Map map = new Map("Sys_MapAttr", "证件字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20, true);

                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 50, "最大长度", true, false);
                map.SetHelperAlert(MapAttrAttr.MaxLen, "定义该字段的字节长度.");


                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.SetHelperAlert(MapAttrAttr.UIWidth, "对自由表单,从表有效,显示文本框的宽度.");

               
                map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", true, false);

                /**map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIVisible, "对于不可见的字段可以在隐藏功能的栏目里找到这些字段进行编辑或者删除.");

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIIsEnable, "不可编辑,让该字段设置为只读.");

                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean(MapAttrAttr.IsRichText, false, "是否富文本？", true, true);
                map.SetHelperAlert(MapAttrAttr.IsRichText, "以html编辑器呈现或者编写字段.");
                map.AddBoolean(MapAttrAttr.IsSecret, false, "是否保密？", true, true);

                map.AddBoolean(MapAttrAttr.IsSupperText, false, "是否大块文本？(是否该字段存放的超长字节字段)", true, true, true);
                map.SetHelperAlert(MapAttrAttr.IsSupperText, "大块文本存储字节比较长，超过4000个字符.");

                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);
                map.SetHelperAlert(MapAttrAttr.Tip, "在文本框输入的时候显示在文本框背景的提示文字,也就是文本框的 placeholder 的值.");
                //CCS样式
                */
                map.AddDDLSQL(MapAttrAttr.CSS, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 基本字段信息.

                #region 傻瓜表单
                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@0=跨0个单元格@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.TextColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");


                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(MapAttrAttr.Idx, "对傻瓜表单有效:用于调整字段在同一个分组中的顺序.");

                #endregion 傻瓜表单

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "字段重命名";
                rm.ClassMethodName = this.ToString() + ".DoRenameField()";
                rm.HisAttrs.AddTBString("key1", "@KeyOfEn", "字段重命名为?", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "如果是节点表单，系统就会把该流程上的所有同名的字段都会重命名，包括NDxxxRpt表单。";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "填充其他控件";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 填充其他控件
        /// </summary>
        /// <returns></returns>
        public string DoDDLFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl2019.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 字段分组查询语句
        /// </summary>
        public static string SQLOfGroupAttr
        {
            get
            {
                return "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'  AND (CtrlType IS NULL OR CtrlType='')  ";
            }
        }
     
        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            

            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn + "T' OR KeyOfEn='" + this.KeyOfEn+"')";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterDelete();
        }


        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        #endregion

        public string DoRenameField(string newField)
        {
            string sql = "";
            if (this.FK_MapData.IndexOf("ND") == 0)
            {
                string strs = this.FK_MapData.Replace("ND", "");
                strs = strs.Substring(0, strs.Length - 2);

                string rptTable = "ND" + strs + "Rpt";
                MapDatas mds = new MapDatas();
                mds.Retrieve(MapDataAttr.PTable, rptTable);

                foreach (MapData item in mds)
                {
                    sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "',  MyPK='" + item.No  + "_" + newField + "' WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + item.No + "'";
                    DBAccess.RunSQL(sql);
                }
            }
            else
            {
                sql = "UPDATE Sys_MapAttr SET KeyOfEn='" + newField + "', MyPK='" + this.FK_MapData + "_" +  newField + "'  WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'";
                DBAccess.RunSQL(sql);
            }

            return "重名称成功,如果是自由表单，请关闭表单设计器重新打开.";
        }
       

        #region 重载.
        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            #region 自动扩展字段长度. 需要翻译.
            if (attr.MaxLen < this.MaxLen )
            {
                attr.MaxLen = this.MaxLen;

                string sql = "";
                MapData md = new MapData();
                md.No = this.FK_MapData;
                if (md.RetrieveFromDBSources() == 1)
                {
                    if (DBAccess.IsExitsTableCol(md.PTable, this.KeyOfEn) == true)
                    {
                        if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                            sql = "ALTER TABLE " + md.PTable + " ALTER column " + this.KeyOfEn + " NVARCHAR(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.MySQL)
                            sql = "ALTER table " + md.PTable + " modify " + attr.Field + " NVARCHAR(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.Oracle
                            || SystemConfig.AppCenterDBType == DBType.DM )
                            sql = "ALTER table " + md.PTable + " modify " + attr.Field + " NVARCHAR2(" + attr.MaxLen + ")";

                        if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                            sql = "ALTER table " + md.PTable + " alter " + attr.Field + " type character varying(" + attr.MaxLen + ")";

                        DBAccess.RunSQL(sql); //如果是oracle如果有nvarchar与varchar类型，就会出错.
                    }
                }
            }
            #endregion 自动扩展字段长度.


            //默认值.
            string defval = this.GetValStrByKey("ExtDefVal");
            if (defval == "" || defval == "0")
            {
                string defVal = this.GetValStrByKey("DefVal");
                if (defval.Contains("@") == true)
                    this.SetValByKey("DefVal", "");
            }
            else
            {
                this.SetValByKey("DefVal", this.GetValByKey("ExtDefVal"));
            }

            //执行保存.
            attr.Save();

            if (this.GetValStrByKey("GroupID") == "无")
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrMapAttrCards : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrMapAttrCards()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrCard();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrCard> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrCard>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrCard> Tolist()
        {
            System.Collections.Generic.List<MapAttrCard> list = new System.Collections.Generic.List<MapAttrCard>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrCard)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}