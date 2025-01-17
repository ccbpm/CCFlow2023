﻿using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using System.IO;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_FoolFormDesigner_MapExt : DirectoryPageBase
    {
        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到.");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner_MapExt()
        {
        }
        #endregion 执行父类的重写方法.

        #region AutoFullDtlField 自动计算 a*b  功能界面 .
        /// <summary>
        /// 保存(自动计算: @单价*@数量 模式.)
        /// </summary>
        /// <returns></returns>
        public string AutoFullDtlField_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FrmID = this.FrmID;
            me.AttrOfOper = this.KeyOfEn;
            me.Doc = this.GetValFromFrmByKey("DDL_Dtl") + "." + this.GetValFromFrmByKey("DDL_Field") + "." + this.GetValFromFrmByKey("DDL_JSFS"); //要执行的表达式.

            me.ExtType = MapExtXmlList.AutoFullDtlField;

            me.Tag1 = this.GetValFromFrmByKey("TB_Tag1");
            me.Tag2 = this.GetValFromFrmByKey("TB_Tag2");

            string Tag = "0";
            try
            {
                Tag = this.GetValFromFrmByKey("CB_Tag");
                if (Tag == "on")
                    Tag = "1";
            }
            catch (Exception e)
            {
                Tag = "0";
            }


            me.Tag = Tag;

            string Tag3 = "0";
            try
            {
                Tag3 = this.GetValFromFrmByKey("CB_Tag3");
                if (Tag3 == "on")
                    Tag3 = "1";
            }
            catch (Exception e)
            {
                Tag3 = "0";
            }
            me.Tag3 = Tag3;

            me.Tag4 = this.GetValFromFrmByKey("DDL_Fileds");

            //执行保存.
            me.setMyPK(MapExtXmlList.AutoFullDtlField + "_" + me.FrmID + "_" + me.AttrOfOper);
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string AutoFullDtlField_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string AutoFullDtlField_Init()
        {
            DataSet ds = new DataSet();

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDtlField,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (me.DBSrcNo == "")
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            //把从表放入里面.
            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, this.FrmID, MapDtlAttr.FK_Node, 0);
            ds.Tables.Add(dtls.ToDataTableField("Dtls"));

            //把从表的字段放入.
            foreach (MapDtl dtl in dtls)
            {
                string sql = "SELECT KeyOfEn as \"No\",Name as \"Name\" FROM Sys_MapAttr WHERE FK_MapData='" + dtl.No + "' AND (MyDataType=2 OR MyDataType=3 OR MyDataType=5 OR MyDataType=8)  ";
                sql += " AND KeyOfEn !='OID' AND KeyOfEn!='FID' AND KeyOfEn!='RefPK' ";

                //把从表增加里面去.
                DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                mydt.TableName = dtl.No;
                ds.Tables.Add(mydt);
            }

            //把主表的字段放入
            string mainsql = "SELECT KeyOfEn as \"No\",Name as \"Name\" FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "' AND MyDataType=1 AND UIIsEnable = 0 ";
            mainsql += " AND KeyOfEn !='OID' AND KeyOfEn!='FID' AND KeyOfEn!='WorkID' AND KeyOfEn!='NodeID' AND KeyOfEn!='RefPK'  AND KeyOfEn!='RDT' AND KeyOfEn!='Rec' ";

            //把从表增加里面去.
            DataTable maindt = DBAccess.RunSQLReturnTable(mainsql);
            maindt.TableName = "main_Attr";
            ds.Tables.Add(maindt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion AutoFullDtlField  功能界面.

        #region AutoFull 自动计算 a*b  功能界面 .
        /// <summary>
        /// 保存(自动计算: @单价*@数量 模式.)
        /// </summary>
        /// <returns></returns>
        public string AutoFull_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFull,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FrmID = this.FrmID;
            me.AttrOfOper = this.KeyOfEn;
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的表达式.

            me.ExtType = MapExtXmlList.AutoFull;

            //执行保存.
            me.setMyPK(MapExtXmlList.AutoFull + "_" + me.FrmID + "_" + me.AttrOfOper);
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }

        public string AutoFull_Init()
        {
            DataSet ds = new DataSet();

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFull,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (me.DBSrcNo == "")
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion ActiveDDL 功能界面.

        #region TBFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string TBFullCtrl_Save()
        {
            try
            {
                MapExt me = new MapExt();
                int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                    MapExtAttr.FK_MapData, this.FrmID,
                    MapExtAttr.AttrOfOper, this.KeyOfEn);

                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = this.GetValFromFrmByKey("FK_DBSrc");
                me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

                me.ExtType = MapExtXmlList.TBFullCtrl;

                //执行保存.
                me.InitPK();

                if (me.Update() == 0)
                    me.Insert();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string TBFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string TBFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.TBFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            //这个属性没有用.
            me.W = i;  //用于标记该数据是否保存?  从而不现实填充从表，填充下拉框.按钮是否可以用.
            if (me.DBSrcNo == "")
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 填充从表
        /// </summary>
        /// <returns></returns>
        public string TBFullCtrlDtl_Init()
        {
            MapExt me = new MapExt(this.MyPK);

            string[] strs = me.Tag1.Split('$');
            // 格式为: $ND101Dtl2:SQL.

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, me.FrmID);
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) || str.Contains(":") == false)
                    continue;

                string[] kvs = str.Split(':');
                string fk_mapdtl = kvs[0];
                string sql = kvs[1];

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.No != fk_mapdtl)
                        continue;
                    //dtl.MTR = sql.Trim();
                }
            }

            foreach (MapDtl dtl in dtls)
            {
                string cols = "";
                MapAttrs mattrs = new MapAttrs(dtl.No);
                foreach (MapAttr item in mattrs)
                {
                    if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal" || item.KeyOfEn == "RefPK")
                        continue;

                    cols += item.KeyOfEn + ",";
                }
                dtl.Alias = cols; //把ptable作为一个数据参数.
            }
            return dtls.ToJson();
        }

        public string TBFullCtrlDtl_Save()
        {
            MapDtls dtls = new MapDtls(this.FrmID);
            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapDtl dtl in dtls)
            {
                string sql = this.GetRequestVal("TB_" + dtl.No);
                sql = sql.Trim();
                if (DataType.IsNullOrEmpty(sql) == true)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + dtl.No + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + dtl.No + ":" + sql;
            }
            me.Tag1 = str;
            me.Update();

            return "保存成功.";
        }

        public string TBFullCtrlDDL_Init()
        {
            MapExt myme = new MapExt();
            myme.setMyPK(this.MyPK);
            myme.RetrieveFromDBSources();
            MapAttrs mattrs = new MapAttrs(myme.FrmID);
            mattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            string[] strs = myme.Tag.Split('$');
            foreach (MapAttr attr in mattrs)
            {
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) == true)
                        continue;
                    if (s.Contains(attr.KeyOfEn + ":") == false)
                        continue;

                    attr.DefVal = s.Replace(attr.KeyOfEn + ":", ""); //使用这个字段作为对应设置的sql.
                }
            }

            return mattrs.ToJson();
        }
        public string TBFullCtrlDDL_Save()
        {
            MapExt me = new MapExt();
            me.setMyPK(this.MyPK);
            if (me.RetrieveFromDBSources() == 0)
            {
                me.setMyPK(this.MyPK);
                me.AttrOfOper = GetRequestVal("AttrOfOper");
                me.FrmID = this.FrmID;
                me.ExtType = MapExtXmlList.FullData;
                //me.DoWay = this.GetRequestVal("DDL_" + attr.KeyOfEn);
                me.Insert();
            }
            string tag6 = this.GetRequestVal("Tag6");
            string tag4 = this.GetRequestVal("Tag4");
            if (DataType.IsNullOrEmpty(tag4) == true)
            {
                tag4 = "";
            }
            me.SetValByKey("Tag4", tag4);
            me.SetValByKey("Tag6", tag6);
            if (tag6.Equals("1") == true)
                me.Tag = this.GetRequestVal("Tag");
            else
            {
                MapAttrs attrs = new MapAttrs(me.FrmID);
                attrs.Retrieve(MapAttrAttr.FK_MapData, me.FrmID,
                    MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);
                MapAttr mapAttr = new MapAttr(this.FrmID + "_" + me.AttrOfOper);
                string str = "";
                foreach (MapAttr attr in attrs)
                {

                    string sql = this.GetRequestVal("TB_" + attr.KeyOfEn);
                    if (DataType.IsNullOrEmpty(sql) == true)
                        continue;
                    sql = sql.Trim();

                    if (sql.Contains("@Key") == false && (int)mapAttr.UIContralType != 18)
                        return "err@在配置从表:" + attr.KeyOfEn + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                    str += "$" + attr.KeyOfEn + ":" + sql;
                }
                me.Tag = str;
            }
            me.AttrOfOper = GetRequestVal("AttrOfOper");
            me.Update();

            return "保存成功.";
        }
        #endregion TBFullCtrl 功能界面.

        #region AutoFullDLL 功能界面 .

        public string AutoFullDLL_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (DataType.IsNullOrEmpty(me.DBSrcNo) == true)
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 查询条件的自动填充
        /// </summary>
        /// <returns></returns>
        public string AutoFullDLL_Init_SearchCond()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLLSearchCond,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (DataType.IsNullOrEmpty(me.DBSrcNo) == true)
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion AutoFullDLL 功能界面.

        #region DDLFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string DDLFullCtrl_Save()
        {
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            me.FrmID = this.FrmID;
            me.AttrOfOper = this.KeyOfEn;
            me.DBSrcNo = this.GetValFromFrmByKey("FK_DBSrc");
            me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

            me.ExtType = MapExtXmlList.DDLFullCtrl;

            //执行保存.
            me.InitPK();
            if (me.Update() == 0)
                me.Insert();

            return "保存成功.";
        }
        public string DDLFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string DDLFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            me.W = i;

            if (DataType.IsNullOrEmpty(me.DBSrcNo))
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion DDLFullCtrl 功能界面.

        #region ActiveDDL 功能界面 .

        public string ActiveDDL_Init()
        {
            DataSet ds = new DataSet();

            //加载外键字段.
            Paras ps = new Paras();
            ps.SQL = "SELECT KeyOfEn AS No, Name FROM Sys_MapAttr WHERE UIContralType=1 AND FK_MapData=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_MapData AND KeyOfEn!=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "KeyOfEn";
            ps.Add("FK_MapData", this.FrmID);
            ps.Add("KeyOfEn", this.KeyOfEn);
            //string sql = "SELECT KeyOfEn AS No, Name FROM Sys_MapAttr WHERE UIContralType=1 AND FK_MapData='" + this.FrmID + "' AND KeyOfEn!='" + this.KeyOfEn + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Sys_MapAttr";

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            ds.Tables.Add(dt);

            if (dt.Rows.Count == 0)
                return "err@表单中没有要级联的下拉框.";

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (DataType.IsNullOrEmpty(me.DBSrcNo))
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string ActiveDDL_Init_SearchCond()
        {
            DataSet ds = new DataSet();

            //加载外键字段.
            Paras ps = new Paras();
            ps.SQL = "SELECT KeyOfEn AS No, Name FROM Sys_MapAttr WHERE UIContralType=1 AND FK_MapData=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_MapData AND KeyOfEn!=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "KeyOfEn";
            ps.Add("FK_MapData", this.FrmID);
            ps.Add("KeyOfEn", this.KeyOfEn);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Sys_MapAttr";

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            ds.Tables.Add(dt);

            if (dt.Rows.Count == 0)
                return "err@表单中没有要级联的下拉框.";

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.ActiveDDLSearchCond,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);
            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            if (me.DBSrcNo == "")
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion ActiveDDL 功能界面.

        #region 配置自动计算日期天数
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string LoadRDTClo_Init()
        {
            DataSet ds = new DataSet();
            string FK_MapData = GetRequestVal("FK_MapData");
            string KeyOfEn = GetRequestVal("KeyOfEn");
            string sql = "";

            sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE (MyDataType='6' OR MyDataType='7') AND FK_MapData='" + FK_MapData + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        public string LoadRDTClo_Save()
        {
            string KeyOfEn = GetRequestVal("KeyOfEn");
            string StarRDT = GetRequestVal("DDL_StarRDT");//开始日期
            string EndRDT = GetRequestVal("DDL_EndRDT");//结束日期
            string RDTRadio = GetRequestVal("RDTRadio");//是否包含节假日 

            MapExt mapExt = new MapExt();
            mapExt.setMyPK("ReqDays_" + this.FrmID + "_" + KeyOfEn);
            if (mapExt.RetrieveFromDBSources() == 0)
            {
                mapExt.FrmID = this.FrmID;
                mapExt.ExtType = "ReqDays";
                mapExt.AttrOfOper = KeyOfEn;
                mapExt.Tag1 = StarRDT;
                mapExt.Tag2 = EndRDT;
                mapExt.Tag3 = RDTRadio;
                mapExt.Insert();
            }
            else
            {
                mapExt.FrmID = this.FrmID;
                mapExt.ExtType = "ReqDays";
                mapExt.AttrOfOper = KeyOfEn;
                mapExt.Tag1 = StarRDT;
                mapExt.Tag2 = EndRDT;
                mapExt.Tag3 = RDTRadio;
                mapExt.Update();
            }
            return "保存成功！！";
        }
        #endregion
        #region 单选按钮事件
        /// <summary>
        /// 返回信息。
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Init()
        {
            DataSet ds = new DataSet();

            //放入表单字段.
            MapAttrs attrs = new MapAttrs(this.FrmID);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            //属性.
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_" + this.KeyOfEn);
            attr.Retrieve();

            //加入从表组件
            MapDtls mapDtls = new MapDtls(this.FrmID);
            ds.Tables.Add(mapDtls.ToDataTableField("MapDtls"));

            //加入多附件组件
            FrmAttachments frmAttachments = new FrmAttachments(this.FrmID);
            ds.Tables.Add(frmAttachments.ToDataTableField("FrmAttachments"));

            //把分组加入里面.
            GroupFields gfs = new GroupFields(this.FrmID);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupFields"));

            //获取外键值
            DataTable dt = BP.Pub.PubClass.GetDataTableByUIBineKey(attr.UIBindKey);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                string columnName = "";
                foreach (DataColumn col in dt.Columns)
                {
                    columnName = col.ColumnName.ToUpper();
                    switch (columnName)
                    {
                        case "NO":
                            col.ColumnName = "No";
                            break;
                        case "NAME":
                            col.ColumnName = "Name";
                            break;
                        default: break;
                    }
                }
            }


            //字段值.
            FrmRBs rbs = new FrmRBs();
            rbs.Retrieve(FrmRBAttr.FrmID, this.FrmID, FrmRBAttr.KeyOfEn, this.KeyOfEn,FrmRBAttr.IntKey);
            if (rbs.Count == 0)
            {
                //如果是枚举类型
                if (attr.LGType == FieldTypeS.Enum)
                {
                    /*初始枚举值变化.
                     */
                    FrmRB rb = new FrmRB();
                    rb.FrmID = this.FrmID;
                    rb.setKeyOfEn(this.KeyOfEn);
                    rb.setIntKey("-1");
                    rb.setLab("--无(不选择)--");
                    rb.setEnumKey(attr.UIBindKey);
                    rb.Insert(); //插入数据.

                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    foreach (SysEnum se in ses)
                    {
                        rb = new FrmRB();
                        rb.FrmID = this.FrmID;
                        rb.setKeyOfEn(this.KeyOfEn);
                        if(DataType.IsNullOrEmpty(se.StrKey)==false)
                            rb.setIntKey(se.StrKey);
                        else
                            rb.setIntKey(se.IntKey.ToString());
                        rb.setLab(se.Lab);
                        rb.setEnumKey(attr.UIBindKey);
                        rb.Insert(); //插入数据.
                    }
                }
                //如果是外键类型
                if (attr.LGType == FieldTypeS.FK)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        FrmRB rb = new FrmRB();
                        rb.FrmID = this.FrmID;
                        rb.setKeyOfEn(this.KeyOfEn);
                        rb.setIntKey(row["No"].ToString());
                        rb.setLab(row["Name"].ToString());
                        rb.setEnumKey(attr.UIBindKey);
                        rb.Insert(); //插入数据.
                    }
                }

                //如果是复选框
                if (attr.MyDataType == DataType.AppBoolean && attr.UIContralType == UIContralType.CheckBok)
                {
                    FrmRB rb = new FrmRB();
                    rb.FrmID = this.FrmID;
                    rb.setKeyOfEn(this.KeyOfEn);
                    rb.setIntKey("0");
                    rb.setLab("否");
                    rb.setEnumKey(attr.UIBindKey);
                    rb.Insert(); //插入数据.

                    rb = new FrmRB();
                    rb.FrmID = this.FrmID;
                    rb.setKeyOfEn(this.KeyOfEn);
                    rb.setIntKey("1");
                    rb.setLab("是");
                    rb.setEnumKey(attr.UIBindKey);
                    rb.Insert(); //插入数据.

                }

                rbs.Retrieve(FrmRBAttr.FrmID, this.FrmID, FrmRBAttr.KeyOfEn, this.KeyOfEn, FrmRBAttr.IntKey);
            }
            //枚举值的情况
            if (rbs.Count != 0)
            {
                if (attr.LGType == FieldTypeS.Enum)
                {
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    if (rbs.Count < ses.Count)
                    {
                        foreach(SysEnum se in ses)
                        {
                            string intKey = se.IntKey.ToString();
                            if (DataType.IsNullOrEmpty(se.StrKey) == false)
                                intKey = se.StrKey;

                            FrmRB rb = rbs.GetEntityByKey(this.FrmID + "_" + this.KeyOfEn + "_" + intKey) as FrmRB;
                            if(rb == null)
                            {
                                rb = new FrmRB();
                                rb.FrmID = this.FrmID;
                                rb.setKeyOfEn(this.KeyOfEn);
                                rb.setIntKey(intKey);
                                rb.setLab(se.Lab);
                                rb.setEnumKey(attr.UIBindKey);
                                rb.Insert(); //插入数据.

                            }
                        }
                        rbs.Retrieve(FrmRBAttr.FrmID, this.FrmID, FrmRBAttr.KeyOfEn, this.KeyOfEn, FrmRBAttr.IntKey);
                    }
                }
            }

            //加入单选按钮.
            ds.Tables.Add(rbs.ToDataTableField("Sys_FrmRB"));
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 复选框选择事件
        /// </summary>
        /// <returns></returns>
        public string CheckBoxs_Init()
        {
            DataSet ds = new DataSet();

            //放入表单字段.
            MapAttrs attrs = new MapAttrs(this.FrmID);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            //属性.
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_" + this.KeyOfEn);
            attr.Retrieve();

            //把分组加入里面.
            GroupFields gfs = new GroupFields(this.FrmID);
            ds.Tables.Add(gfs.ToDataTableField("Sys_GroupFields"));

            FrmRBs rbs = new FrmRBs();
            rbs.Retrieve(FrmRBAttr.FrmID, this.FrmID, FrmRBAttr.KeyOfEn, this.KeyOfEn);
            //加入单选按钮.
            ds.Tables.Add(rbs.ToDataTableField("Sys_FrmRB"));


            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Save()
        {
            //string json = context.Request.Form["data"];
            //if (DataType.IsNullOrEmpty(json))
            string json = GetRequestVal("data");
            DataTable dt = null;

            try
            {
                dt = BP.Tools.Json.ToDataTable(json);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
                //  return json;
            }

            foreach (DataRow dr in dt.Rows)
            {
                FrmRB rb = new FrmRB();
                rb.setMyPK(dr["MyPK"].ToString());
                rb.Retrieve();

                rb.Script = dr["Script"].ToString();
                rb.FieldsCfg = dr["FieldsCfg"].ToString(); //格式为 @字段名1=1@字段名2=0
                rb.Tip = dr["Tip"].ToString(); //提示信息

                rb.SetVal = dr["SetVal"].ToString(); //设置值.

                rb.DirectUpdate();
            }

            return "保存成功.";
        }
        #endregion

        #region xxx 界面
        /// <summary>
        /// 初始化正则表达式界面
        /// </summary>
        /// <returns></returns>
        public string RegularExpression_Init()
        {
            DataSet ds = new DataSet();

            MapExts mapExts = new MapExts();
            mapExts.Retrieve(MapExtAttr.AttrOfOper, this.KeyOfEn, MapExtAttr.FK_MapData, this.FrmID);
            ds.Tables.Add(mapExts.ToDataTableField("Sys_MapExt"));

            BP.Sys.XML.RegularExpressions res = new BP.Sys.XML.RegularExpressions();
            res.Retrieve("ForCtrl", "TB");

            DataTable myDT = res.ToDataTable();
            myDT.TableName = "RE";
            ds.Tables.Add(myDT);


            BP.Sys.XML.RegularExpressionDtls dtls = new BP.Sys.XML.RegularExpressionDtls();
            dtls.RetrieveAll();
            DataTable myDTDtls = dtls.ToDataTable();
            myDTDtls.TableName = "REDtl";
            ds.Tables.Add(myDTDtls);

            return BP.Tools.Json.ToJson(ds);
        }
        public string RegularExpressionNum_Init()
        {
            DataSet ds = new DataSet();

            MapExts mes = new MapExts();
            mes.Retrieve("AttrOfOper", this.KeyOfEn, "FK_MapData", this.FrmID);
            ds.Tables.Add(mes.ToDataTableField("Sys_MapExt"));

            BP.Sys.XML.RegularExpressions res = new BP.Sys.XML.RegularExpressions();
            res.Retrieve("ForCtrl", "TBNum");

            DataTable myDT = res.ToDataTable();
            myDT.TableName = "RE";
            ds.Tables.Add(myDT);


            BP.Sys.XML.RegularExpressionDtls dtls = new BP.Sys.XML.RegularExpressionDtls();
            dtls.RetrieveAll();
            DataTable myDTDtls = dtls.ToDataTable();
            myDTDtls.TableName = "REDtl";
            ds.Tables.Add(myDTDtls);

            return BP.Tools.Json.ToJson(ds);
        }
        private void RegularExpression_Save_Tag(string tagID)
        {
            string val = this.GetValFromFrmByKey("TB_Doc_" + tagID);
            if (DataType.IsNullOrEmpty(val))
                return;

            MapExt me = new MapExt();
            me.setMyPK(MapExtXmlList.TBFullCtrl + "_" + this.FrmID + "_" + this.KeyOfEn + "_" + tagID);
            me.FrmID = this.FrmID;
            me.AttrOfOper = this.KeyOfEn;
            me.ExtType = "RegularExpression";
            me.Tag = tagID;
            me.Doc = val;
            me.Tag1 = this.GetValFromFrmByKey("TB_Tag1_" + tagID);
            me.Save();
        }


        /// <summary>
        /// 执行 保存.
        /// </summary>
        /// <returns></returns>
        public string RegularExpression_Save()
        {
            //删除该字段的全部扩展设置. 
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.ExtType, MapExtXmlList.RegularExpression,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            //执行存盘.
            RegularExpression_Save_Tag("onblur");
            RegularExpression_Save_Tag("onchange");
            RegularExpression_Save_Tag("onclick");
            RegularExpression_Save_Tag("ondblclick");
            RegularExpression_Save_Tag("onkeypress");
            RegularExpression_Save_Tag("onkeyup");
            RegularExpression_Save_Tag("onsubmit");

            return "保存成功...";
        }


        string no;
        string name;
        string fk_dept;
        string oid;
        string kvs;
        public string DealSQL(string sql, string key)
        {
            sql = sql.Replace("@Key", key);
            sql = sql.Replace("@key", key);
            sql = sql.Replace("@Val", key);
            sql = sql.Replace("@val", key);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
            if (oid != null)
                sql = sql.Replace("@OID", oid);

            if (DataType.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s)
                        || s.Contains("=") == false)
                        continue;
                    string[] mykv = s.Split('=');
                    sql = sql.Replace("@" + mykv[0], mykv[1]);

                    if (sql.Contains("@") == false)
                        break;
                }
            }
            return sql;
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string PopVal_Init()
        {
            MapExt ext = new MapExt();
            ext.setMyPK(this.MyPK);
            if (ext.RetrieveFromDBSources() == 0)
            {
                // throw new Exception("err@主键=" + ext.MyPK + "的配置数据丢失");
                ext.PopValSelectModel = PopValSelectModel.One;
                ext.PopValWorkModel = PopValWorkModel.TableOnly;
            }

            // ext.SetValByKey
            return ext.PopValToJson();
        }
        /// <summary>
        /// 保存设置.
        /// </summary>
        /// <returns></returns>
        public string PopVal_Save()
        {
            try
            {
                MapExt me = new MapExt();
                me.setMyPK(this.FK_MapExt);
                me.FrmID = this.FrmID;
                me.ExtType = "PopVal";
                me.AttrOfOper = this.KeyOfEn;
                me.RetrieveFromDBSources();

                string valWorkModel = this.GetValFromFrmByKey("Model");

                switch (valWorkModel)
                {
                    case "None":
                        me.PopValWorkModel = PopValWorkModel.None;
                        break;
                    case "SelfUrl": //URL模式.
                        me.PopValWorkModel = PopValWorkModel.SelfUrl;
                        me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "TableOnly": //表格模式.
                        me.PopValWorkModel = PopValWorkModel.TableOnly;
                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_Table_SQL");
                        break;
                    case "TablePage": //分页模式.
                        me.PopValWorkModel = PopValWorkModel.TablePage;
                        me.PopValTablePageSQL = this.GetValFromFrmByKey("TB_TablePage_SQL");
                        me.PopValTablePageSQLCount = this.GetValFromFrmByKey("TB_TablePage_SQLCount");
                        break;
                    case "Group": //分组模式.
                        me.PopValWorkModel = PopValWorkModel.Group;

                        me.PopValGroupSQL = this.GetValFromFrmByKey("TB_GroupModel_Group");
                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_GroupModel_Entity");

                        //me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "Tree": //单实体树.
                        me.PopValWorkModel = PopValWorkModel.Tree;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_TreeSQL");
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_TreeParentNo");
                        break;
                    case "TreeDouble": //双实体树.
                        me.PopValWorkModel = PopValWorkModel.TreeDouble;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_DoubleTreeSQL");// 树SQL
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_DoubleTreeParentNo");

                        me.PopValDoubleTreeEntitySQL = this.GetValFromFrmByKey("TB_DoubleTreeEntitySQL"); //实体SQL
                        break;
                    default:
                        break;
                }

                //高级属性.
                me.W = int.Parse(this.GetValFromFrmByKey("TB_Width"));
                me.H = int.Parse(this.GetValFromFrmByKey("TB_Height"));
                me.PopValColNames = this.GetValFromFrmByKey("TB_ColNames"); //中文列名的对应.
                me.PopValTitle = this.GetValFromFrmByKey("TB_Title"); //标题.
                me.PopValSearchTip = this.GetValFromFrmByKey("TB_PopValSearchTip"); //关键字提示.
                me.PopValSearchCond = this.GetValFromFrmByKey("TB_PopValSearchCond"); //查询条件.


                //数据返回格式.
                string popValFormat = this.GetValFromFrmByKey("PopValFormat");
                switch (popValFormat)
                {
                    case "OnlyNo":
                        me.PopValFormat = PopValFormat.OnlyNo;
                        break;
                    case "OnlyName":
                        me.PopValFormat = PopValFormat.OnlyName;
                        break;
                    case "NoName":
                        me.PopValFormat = PopValFormat.NoName;
                        break;
                    default:
                        break;
                }

                //选择模式.
                string seleModel = this.GetValFromFrmByKey("PopValSelectModel");
                if (seleModel == "One")
                    me.PopValSelectModel = PopValSelectModel.One;
                else
                    me.PopValSelectModel = PopValSelectModel.More;

                me.Save();
                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "@保存失败:" + ex.Message;
            }
        }
        #endregion xxx 界面方法.

        #region PopFullCtrl 功能界面 .
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string PopFullCtrl_Save()
        {
            try
            {
                MapExt me = new MapExt();
                int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                    MapExtAttr.FK_MapData, this.FrmID,
                    MapExtAttr.AttrOfOper, this.KeyOfEn);

                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = this.GetValFromFrmByKey("FK_DBSrc");
                me.Doc = this.GetValFromFrmByKey("TB_Doc"); //要执行的SQL.

                me.ExtType = MapExtXmlList.PopFullCtrl;

                //执行保存.
                me.InitPK();

                if (me.Update() == 0)
                    me.Insert();

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string PopFullCtrl_Delete()
        {
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            return "删除成功.";
        }
        public string PopFullCtrl_Init()
        {
            DataSet ds = new DataSet();

            //加载数据源.
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            DataTable dtSrc = srcs.ToDataTableField();
            dtSrc.TableName = "Sys_SFDBSrc";
            ds.Tables.Add(dtSrc);

            // 加载 mapext 数据.
            MapExt me = new MapExt();
            int i = me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PopFullCtrl,
                MapExtAttr.FK_MapData, this.FrmID,
                MapExtAttr.AttrOfOper, this.KeyOfEn);

            if (i == 0)
            {
                me.FrmID = this.FrmID;
                me.AttrOfOper = this.KeyOfEn;
                me.DBSrcNo = "local";
            }

            //这个属性没有用.
            me.W = i;  //用于标记该数据是否保存?  从而不现实填充从表，填充下拉框.按钮是否可以用.
            if (me.DBSrcNo.Equals(""))
                me.DBSrcNo = "local";

            //去掉 ' 号.
            me.SetValByKey("Doc", me.Doc);

            DataTable dt = me.ToDataTableField();
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 填充从表
        /// </summary>
        /// <returns></returns>
        public string PopFullCtrlDtl_Init()
        {
            MapExt me = new MapExt(this.MyPK);

            string[] strs = me.Tag1.Split('$');
            // 格式为: $ND101Dtl2:SQL.

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, me.FrmID);
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) || str.Contains(":") == false)
                    continue;

                string[] kvs = str.Split(':');
                string fk_mapdtl = kvs[0];
                string sql = kvs[1];

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.No != fk_mapdtl)
                        continue;
                    //dtl.MTR = sql.Trim();//多表头去掉了
                }
            }

            foreach (MapDtl dtl in dtls)
            {
                string cols = "";
                MapAttrs mattrs = new MapAttrs(dtl.No);
                foreach (MapAttr item in mattrs)
                {
                    if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal" || item.KeyOfEn == "RefPK")
                        continue;

                    cols += item.KeyOfEn + ",";
                }
                dtl.Alias = cols; //把ptable作为一个数据参数.
            }
            return dtls.ToJson();
        }

        public string PopFullCtrlDtl_Save()
        {
            MapDtls dtls = new MapDtls(this.FrmID);
            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapDtl dtl in dtls)
            {
                string sql = this.GetRequestVal("TB_" + dtl.No);
                sql = sql.Trim();
                if (DataType.IsNullOrEmpty(sql) == true)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + dtl.No + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + dtl.No + ":" + sql;
            }
            me.Tag1 = str;
            me.Update();

            return "保存成功.";
        }

        public string PopFullCtrlDDL_Init()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapAttrs mattrs = new MapAttrs(myme.FrmID);
            mattrs.Retrieve(MapAttrAttr.FK_MapData, myme.FrmID,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            string[] strs = myme.Tag.Split('$');
            foreach (MapAttr attr in mattrs)
            {
                foreach (string s in strs)
                {
                    if (s == null)
                        continue;
                    if (s.Contains(attr.KeyOfEn + ":") == false)
                        continue;

                    string[] ss = s.Split(':');
                    attr.DefVal = ss[1]; //使用这个字段作为对应设置的sql.
                }
            }

            return mattrs.ToJson();
        }
        public string PopFullCtrlDDL_Save()
        {
            MapExt myme = new MapExt(this.MyPK);

            MapAttrs mattrs = new MapAttrs(myme.FrmID);
            mattrs.Retrieve(MapAttrAttr.FK_MapData, myme.FrmID,
                MapAttrAttr.UIIsEnable, 1, MapAttrAttr.UIContralType, (int)UIContralType.DDL);

            MapExt me = new MapExt(this.MyPK);

            string str = "";
            foreach (MapAttr attr in mattrs)
            {

                string sql = this.GetRequestVal("TB_" + attr.KeyOfEn);
                sql = sql.Trim();
                if (DataType.IsNullOrEmpty(sql) == true)
                    continue;

                if (sql.Contains("@Key") == false)
                    return "err@在配置从表:" + attr.KeyOfEn + " sql填写错误, 必须包含@Key列, @Key就是当前文本框输入的值. ";

                str += "$" + attr.KeyOfEn + ":" + sql;
            }
            me.Tag = str;
            me.Update();

            return "保存成功.";
        }
        #endregion PopFullCtrl 功能界面.


        #region 杨玉慧  表单设计--表单属性   JS编程 
        public string InitScript_Init()
        {
            try
            {
                //2019-07-26 zyt改造
                //String webPath = HttpRuntime.AppDomainAppPath.Replace("\\", "/");
                String webPath = BP.Difference.SystemConfig.PathOfWebApp.Replace("\\", "/");
                String filePath = webPath + @"DataUser/JSLibData/" + this.FrmID + "_Self.js";
                String content = "";
                if (!File.Exists(filePath))
                {
                    content = "";
                }
                else
                {
                    //content = File.ReadAllText(filePath);
                    content = DataType.ReadTextFile(filePath);
                }
                return content;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public string InitScript_Save()
        {
            try
            {
                //2019-07-26 zyt改造
                //String webPath = HttpRuntime.AppDomainAppPath.Replace("\\", "/");
                String webPath = BP.Difference.SystemConfig.PathOfWebApp.Replace("\\", "/");
                String filePath = webPath + @"DataUser/JSLibData/" + this.FrmID + "_Self.js";
                String content = HttpContextHelper.RequestParams("JSDoc"); // this.context.Request.Params["JSDoc"];

                //在应用程序当前目录下的File1.txt文件中追加文件内容，如果文件不存在就创建，默认编码
                //File.WriteAllText(filePath, content);
                DataType.WriteFile(filePath, content);
                return "保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

        }

        public string InitScript_Delete()
        {
            try
            {
                //2019-07-26 zyt改造
                //String webPath = HttpRuntime.AppDomainAppPath.Replace("\\", "/");
                String webPath = BP.Difference.SystemConfig.PathOfWebApp.Replace("\\", "/");
                String filePath = webPath + @"DataUser/JSLibData/" + this.FrmID + "_Self.js";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        //public string NRCMaterielDtlSave()
        //{
        //    string fk_Template = this.GetRequestVal("FK_Template");
        //    string workid = this.GetRequestVal("WorkId");
        //    string sql = "SELECT * FROM STARCO_TemplateNRCMaterielDtl WHERE FK_Template='" + fk_Template + "'";
        //    DataTable dt = new DataTable();
        //    dt = DBAccess.RunSQLReturnTable(sql);
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        //string sql1 = "SELECT * FROM ND105Dtl1 WHERE RefPK='" + workid + "'";
        //        //DataTable dt1 = new DataTable();
        //        //dt1 = DBAccess.RunSQLReturnTable(sql1);
        //        //if (dt1 != null && dt1.Rows.Count > 0)
        //        //{

        //        //}

        //        string delSql = "DELETE FROM ND105Dtl1 WHERE RefPK='" + workid + "'";
        //        DBAccess.RunSQLReturnString(delSql);

        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            GEDtl dtl = new GEDtl("ND105Dtl1");

        //            dtl.SetValByKey("MingChen", dt.Rows[i]["Name"].ToString());
        //            dtl.SetValByKey("JianHao", dt.Rows[i]["PartNumber"].ToString());
        //            dtl.SetValByKey("RefPK", dt.Rows[i]["Qty"].ToString());
        //            dtl.SetValByKey("ShuLiang", dt.Rows[i]["PCH"].ToString());
        //            dtl.SetValByKey("PiCiHao", dt.Rows[i]["Name"].ToString());
        //            dtl.SetValByKey("RDT", dt.Rows[i]["Name"].ToString());
        //            dtl.SetValByKey("Rec", dt.Rows[i]["Name"].ToString());

        //            string name = dt.Rows[i]["Name"].ToString();
        //            string jianHao = dt.Rows[i]["PartNumber"].ToString();
        //            string workId = workid;
        //            string shuLiang = dt.Rows[i]["Qty"].ToString();
        //            string piCiHao = dt.Rows[i]["PCH"].ToString();
        //            string rdt = DateTime.Now.ToString();
        //            string userNo = WebUser.No;

        //            string sql2 = "INSERT INTO ND105Dtl1(MingChen,JianHao,RefPK,ShuLiang,PiCiHao,RDT,Rec) VALUES('" + name + "','" + jianHao + "','" + workId + "','" + shuLiang + "','" + piCiHao + "','" + rdt + "','" + userNo + "')";
        //            string result = DBAccess.RunSQLReturnString(sql2);
        //        }

        //    }

        //    return "ok";
        //}

    }
}
