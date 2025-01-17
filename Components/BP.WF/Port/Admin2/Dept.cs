﻿using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;
using BP.Sys;
using BP.GPM;
using BP.WF.Template;

namespace BP.WF.Port.Admin2
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : BP.Port.DeptAttr
    {
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Dept", "部门");

                map.AdjunctType = AdjunctType.None;

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 30, 40);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 60, 200);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 30, 40);
                map.AddTBString(DeptAttr.OrgNo, null, "隶属组织", true, false, 0, 50, 40);
                map.AddTBInt(DeptAttr.Idx, 0, "顺序号", true, false);

                RefMethod rm = new RefMethod();
                rm.Title = "设置为独立组织";
                rm.Warning = "如果当前部门已经是独立组织，系统就会提示错误。";
                rm.ClassMethodName = this.ToString() + ".SetOrg";
                rm.HisAttrs.AddTBString("adminer", null, "组织管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "克隆独立组织";
                rm.Warning = "如果当前部门已经是独立组织，系统就会提示错误。";
                rm.ClassMethodName = this.ToString() + ".DoCloneOrg";
                rm.HisAttrs.AddTBString("adminer", null, "组织管理员编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("coneOrgNo", null, "被克隆的组织编号(14903)", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// 克隆组织
        /// </summary>
        /// <param name="adminer"></param>
        /// <param name="orgNo"></param>
        /// <returns></returns>
        public string DoCloneOrg(string adminer, string cloneOrgNo)
        {
            Org orgClone = new Org(cloneOrgNo);

            if (WebUser.No.Equals("admin") == false)
                return "err@非admin管理员，您无法执行该操作.";

            //检查是否有该用户.
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = adminer;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户编号错误:" + adminer;

            //检查该部门是否是独立组织.
            Org org = new Org();
            org.No = this.No;
            if (org.RetrieveFromDBSources() == 1)
            {
                /* 已经是独立组织了. */
                return "info@当前部门已经是独立组织了,您不能在克隆了.";
            }
            org.Name = this.Name; //把部门名字改为组织名字.

            try
            {
                //设置父级信息.
                BP.Port.Dept parentDept = new BP.Port.Dept();
                if (this.ParentNo.Equals("0") == true)
                    this.ParentNo = this.No;

                parentDept.No = this.ParentNo;
                parentDept.Retrieve();

                org.ParentNo = this.ParentNo;
                org.ParentName = parentDept.Name;

                //设置管理员信息.
                org.Adminer = emp.No;
                org.AdminerName = emp.Name;
                org.DirectInsert();

                //增加到管理员.
                OrgAdminer oa = new OrgAdminer();
                oa.FK_Emp = emp.No;
                oa.OrgNo = this.No;
                oa.DirectDelete();
                oa.DirectInsert();

                //初始化流程树的根节点.
                FlowSort fsRoot = new FlowSort();
                fsRoot.No = this.No;
                fsRoot.ParentNo = "1";
                fsRoot.Name = this.Name;
                fsRoot.OrgNo = this.No;
                fsRoot.DirectUpdate();

                string info = "";

                //执行 clone...

                //查询出来被克隆的流程树.
                BP.WF.Template.FlowSorts sorts = new Template.FlowSorts();
                sorts.Retrieve(FlowSortAttr.OrgNo, cloneOrgNo);
                foreach (FlowSort sort in sorts)
                {
                    if (sort.ParentNo.Equals("1") == true)
                        continue;

                    FlowSort fs = new FlowSort();
                    fs.Copy(sort);
                    fs.Name = sort.Name;
                    fs.ParentNo = this.No;
                    fs.No = DBAccess.GenerGUID();
                    fs.OrgNo = this.No;
                    fs.DirectInsert();

                    //查询出来模版，开始执行clone.
                    Flows fls = new Flows();
                    fls.Retrieve(FlowAttr.FK_FlowSort, sort.No);
                    foreach (Flow fl in fls)
                    {
                        try
                        {
                            string fileName = SystemConfig.PathOfTemp + "" + DBAccess.GenerGUID() + ".xml";
                            DataSet ds = fl.GetFlow(fileName);
                            ds.WriteXml(fileName);

                            var flowNew = BP.WF.Flow.DoLoadFlowTemplate(fs.No, fileName, ImpFlowTempleteModel.AsNewFlow);
                            flowNew.OrgNo = this.No;
                            flowNew.DirectUpdate();
                        }
                        catch (Exception ex)
                        {
                            info += "err@" + ex.Message;
                        }
                    }
                }

                //初始化frmTree的根节点.
                SysFormTree frmRoot = new SysFormTree();
                frmRoot.No = this.No;
                frmRoot.ParentNo = "1";
                frmRoot.Name = this.Name;
                frmRoot.OrgNo = this.No;
                frmRoot.DirectInsert();

                //查询出来被克隆的表单树
                BP.WF.Template.SysFormTrees frmTrees = new Template.SysFormTrees();
                sorts.Retrieve(FlowSortAttr.OrgNo, cloneOrgNo);
                foreach (SysFormTree sort in frmTrees)
                {
                    if (sort.ParentNo.Equals("1") == true)
                        continue;

                    SysFormTree fs = new SysFormTree();
                    fs.Copy(sort);
                    fs.Name = sort.Name;
                    fs.ParentNo = this.No;
                    fs.No = DBAccess.GenerGUID();
                    fs.OrgNo = this.No;
                    fs.DirectInsert();

                    //查询出来模版，开始执行clone.
                    BP.Sys.MapDatas mds = new BP.Sys.MapDatas();
                    mds.Retrieve(BP.Sys.MapDataAttr.FK_FrmSort, sort.No);
                    foreach (BP.Sys.MapData frm in mds)
                    {
                        try
                        {
                            DataSet myds = BP.Sys.CCFormAPI.GenerHisDataSet(frm.No, frm.Name);

                            BP.WF.HttpHandler.WF_Admin_Template en = new HttpHandler.WF_Admin_Template();
                            en.ImpFrm("2", frm.No, frm, myds, sort.No);
                        }
                        catch (Exception ex)
                        {
                            info += "err@" + ex.Message;
                        }
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                string sql = " delete from port_org where adminer = '" + adminer + "' ";
                DBAccess.RunSQL(sql);

                sql = "delete from port_orgAdminer where FK_Emp = '" + adminer + "'";
                DBAccess.RunSQL(sql);

                //删除流程&流程类别.
                FlowSorts fss = new FlowSorts();
                fss.Retrieve("OrgNo", this.No);
                foreach (FlowSort fs in fss)
                {
                    Flows fls = new Flows();
                    fls.Retrieve(FlowAttr.FK_FlowSort, fs.No);

                    //删除流程.
                    foreach (Flow item in fls)
                    {
                        item.DoDelete();
                    }
                    fs.Delete(); //删除.
                }

                //删除表单与表单类别.
                BP.WF.Template.SysFormTrees fts = new BP.WF.Template.SysFormTrees();
                fts.Retrieve("OrgNo", this.No);
                foreach (BP.WF.Template.SysFormTree ft in fts)
                {
                    MapDatas mds = new BP.Sys.MapDatas();
                    mds.Retrieve(MapDataAttr.FK_FormTree, ft.No);

                    //删除流程.
                    foreach (MapData item in mds)
                    {
                        item.Delete();
                    }
                    ft.Delete(); //删除.
                }

                //sql = "delete from wf_flowsort where orgNo = '" + this.No + "'";
                //DBAccess.RunSQL(sql);

                ////SELECT* FROM sys_formtree WHERE OrgNo = ''
                //sql = "delete from sys_formtree where orgNo = '" + this.No + "'";
                //DBAccess.RunSQL(sql);

                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 设置组织
        /// </summary>
        /// <param name="userNo">管理员编号</param>
        /// <returns></returns>
        public string SetOrg(string adminer)
        {

            if (WebUser.No.Equals("admin") == false)
                return "err@非admin管理员，您无法执行该操作.";

            //检查是否有该用户.
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = adminer;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户编号错误:" + adminer;

            //检查该部门是否是独立组织.
            Org org = new Org();
            org.No = this.No;
            if (org.RetrieveFromDBSources() == 1)
            {
                /* 已经是独立组织了. */
                string info1 = org.DoCheck();
                return "info@当前部门已经是独立组织了,检查信息如下:" + info1;
            }
            org.Name = this.Name; //把部门名字改为组织名字.

            //设置父级信息.
            BP.Port.Dept parentDept = new BP.Port.Dept();

            if (this.ParentNo.Equals("0") == true)
                this.ParentNo = this.No;

            parentDept.No = this.ParentNo;
            parentDept.Retrieve();

            org.ParentNo = this.ParentNo;
            org.ParentName = parentDept.Name;

            //设置管理员信息.
            org.Adminer = emp.No;
            org.AdminerName = emp.Name;
            org.Insert();

            //增加到管理员.
            OrgAdminer oa = new OrgAdminer();
            oa.FK_Emp = emp.No;
            oa.OrgNo = this.No;
            oa.Insert();

            //如果不是视图.
            if (DBAccess.IsView("Port_StationType") == false)
            {
                #region 高层岗位.
                StationType st = new StationType();
                st.No = DBAccess.GenerGUID();
                st.Name = "高层岗";
                st.OrgNo = this.No;
                st.DirectInsert();

                Station sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "总经理";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();
                #endregion 高层岗位.

                #region 中层岗.
                st = new StationType();
                st.No = DBAccess.GenerGUID();
                st.Name = "中层岗";
                st.OrgNo = this.No;
                st.DirectInsert();

                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "财务部经理";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();


                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "研发部经理";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();

                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "市场部经理";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();
                #endregion 中层岗.

                #region 基层岗.
                st = new StationType();
                st.No = DBAccess.GenerGUID();
                st.Name = "基层岗";
                st.OrgNo = this.No;
                st.DirectInsert();

                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "会计岗";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();

                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "销售岗";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();

                sta = new Station();
                sta.No = DBAccess.GenerGUID();
                sta.Name = "程序员岗";
                sta.OrgNo = this.No;
                sta.FK_StationType = st.No;
                sta.DirectInsert();

                #endregion 基层岗.
            }

            // 返回他的检查信息，这个方法里，已经包含了自动创建独立组织的，表单树，流程树。
            // 自动他创建，岗位类型，岗位信息.
            string info = org.DoCheck();

            if (info.IndexOf("err@") == 0)
                return info;

            return "设置成功.";


            //初始化表单树，流程树.
            //InitFlowSortTree();
            //return "设置成功,[" + ad.No + "," + ad.Name + "]重新登录就可以看到.";
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
        /// <summary>
        /// 查询全部。
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No.Equals("admin")==true)
                return base.RetrieveAll();

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptAttr.No, " = ", BP.Web.WebUser.FK_Dept);
            qo.addOr();
            qo.AddWhere(DeptAttr.ParentNo, " = ", BP.Web.WebUser.FK_Dept);
            return qo.DoQuery();
        }
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Depts()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
