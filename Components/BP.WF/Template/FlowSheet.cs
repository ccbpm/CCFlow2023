﻿using System;
using System.Data;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowSheet : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 流程事件实体
        /// </summary>
        public string FlowEventEntity
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FlowEventEntity);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowEventEntity, value);
            }
        }
        /// <summary>
        /// 流程标记
        /// </summary>
        public string FlowMark
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.FlowMark);
                if (str == "")
                    return this.No;
                return str;
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowMark, value);
            }
        }

        #region   前置导航
        /// <summary>
        /// 前置导航方式
        /// </summary>
        public StartGuideWay StartGuideWay
        {
            get
            {
                return (StartGuideWay)this.GetValIntByKey(FlowAttr.StartGuideWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuideWay, (int)value);
            }

        }
        /// <summary>
        /// 前置导航参数1
        /// </summary>
        public string StartGuidePara1
        {
            get
            {
                string str = this.GetValStringByKey(FlowAttr.StartGuidePara1);
                return str.Replace("~", "'");
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara1, value);
            }

        }
        /// <summary>
        /// 前置导航参数2
        /// </summary>
        public string StartGuidePara2
        {

            get
            {
                string str = this.GetValStringByKey(FlowAttr.StartGuidePara2);
                return str.Replace("~", "'");
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara2, value);
            }

        }
        /// <summary>
        /// 前置导航参数3
        /// </summary>
        public string StartGuidePara3
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.StartGuidePara3);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartGuidePara3, value);
            }

        }

        /// <summary>
        /// 启动方式
        /// </summary>
        public FlowRunWay FlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);

            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }

        }

        /// <summary>
        /// 运行内容
        /// </summary>
        public string RunObj
        {

            get
            {
                return this.GetValStringByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }

        }

        /// <summary>
        /// 是否启用开始节点数据重置按钮
        /// </summary>
        public bool ItIsResetData
        {

            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsResetData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsResetData, value);
            }
        }

        /// <summary>
        /// 是否自动装载上一笔数据
        /// </summary>
        public bool ItIsLoadPriData

        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsLoadPriData);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsLoadPriData, value);
            }
        }
        #endregion        
        
       
        /// <summary>
        /// 编号生成格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.BillNoFormat);
            }
            set
            {
                this.SetValByKey(FlowAttr.BillNoFormat, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true )
                {
                    uac.IsUpdate = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowSheet()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowSheet(string _No)
        {
            this.No = _No;
            if (BP.Difference.SystemConfig.isDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("流程编号不存在");
            }
            else
            {
                this.Retrieve();
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "流程");
                map.CodeStruct = "3";

                #region 基本属性。
                map.AddGroupAttr("基本属性");
                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 10, 3);
                map.SetHelperUrl(FlowAttr.No, "http://ccbpm.mydoc.io/?v=5404&t=17023"); //使用alert的方式显示帮助信息.

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), true);

                map.SetHelperUrl(FlowAttr.FK_FlowSort, "http://ccbpm.mydoc.io/?v=5404&t=17024");
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 50, 10, true);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowMark, null, "流程标记", true, false, 0, 150, 10);
                map.AddTBString(FlowAttr.FlowEventEntity, null, "流程事件实体", true, true, 0, 150, 10);
                map.SetHelperUrl(FlowAttr.FlowMark, "http://ccbpm.mydoc.io/?v=5404&t=16847");
                map.SetHelperUrl(FlowAttr.FlowEventEntity, "http://ccbpm.mydoc.io/?v=5404&t=17026");

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);
                map.SetHelperUrl(FlowAttr.TitleRole, "http://ccbpm.mydoc.io/?v=5404&t=17040");

                //add  2013-08-30.
                map.AddTBString(FlowAttr.BillNoFormat, null, "单据编号格式", true, false, 0, 50, 10, false);
                map.SetHelperUrl(FlowAttr.BillNoFormat, "http://ccbpm.mydoc.io/?v=5404&t=17041");

                // add 2014-10-19.
                map.AddDDLSysEnum(FlowAttr.ChartType, (int)FlowChartType.Icon, "节点图形类型", true, true,
                    "ChartType", "@0=几何图形@1=肖像图片");

                map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？(独立启动的流程可以显示在发起流程列表里)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsCanStart, "http://ccbpm.mydoc.io/?v=5404&t=17027");


                map.AddBoolean(FlowAttr.IsMD5, false, "是否是数据加密流程(MD5数据加密防篡改)", true, true, true);
                map.SetHelperUrl(FlowAttr.IsMD5, "http://ccbpm.mydoc.io/?v=5404&t=17028");

                map.AddBoolean(FlowAttr.IsFullSA, false, "是否自动计算未来的处理人？", true, true, true);
                map.SetHelperUrl(FlowAttr.IsFullSA, "http://ccbpm.mydoc.io/?v=5404&t=17034");



                map.AddBoolean(FlowAttr.GuestFlowRole, false, "是否外部用户参与流程(非组织结构人员参与的流程)", true, true, false);
                map.AddDDLSysEnum(FlowAttr.GuestFlowRole, (int)GuestFlowRole.None, "外部用户参与流程规则",
                 true, true, "GuestFlowRole", "@0=不参与@1=开始节点参与@2=中间节点参与");

                //批量发起 add 2013-12-27. 
                map.AddBoolean(FlowAttr.IsBatchStart, false, "是否可以批量发起流程？(如果是就要设置发起的需要填写的字段,多个用逗号分开)", true, true, true);
                map.AddTBString(FlowAttr.BatchStartFields, null, "发起字段s", true, false, 0, 500, 10, true);
                map.SetHelperUrl(FlowAttr.IsBatchStart, "http://ccbpm.mydoc.io/?v=5404&t=17047");
              

                // 草稿
                map.AddDDLSysEnum(FlowAttr.Draft, (int)DraftRole.None, "草稿规则",
               true, true, FlowAttr.Draft, "@0=无(不设草稿)@1=保存到待办@2=保存到草稿箱");
                map.SetHelperUrl(FlowAttr.Draft, "http://ccbpm.mydoc.io/?v=5404&t=17037");
                 
 
                #endregion 基本属性。

                //查询条件.
                map.AddSearchAttr(FlowAttr.FK_FlowSort);
                //   map.AddSearchAttr(FlowAttr.TimelineRole);

                //绑定组织.
                map.AttrsOfOneVSM.Add(new FlowOrgs(), new BP.WF.Port.Admin2Group.Orgs(),
                    FlowOrgAttr.FlowNo,
                  FlowOrgAttr.OrgNo, FlowAttr.Name, FlowAttr.No, "可以发起的组织");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrNode/Action.htm?NodeID=0&FK_Flow=" + this.No + "&tk=" + new Random().NextDouble();
        }
        public string DoDBSrc()
        {
            return "../../Comm/Sys/SFDBSrcNewGuide.htm";
        }


        public string DoBindFlowSheet()
        {
            return "../../Admin/Sln/BindFrms.htm?s=d34&ShowType=FlowFrms&FK_Node=0&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 批量发起字段
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrFlow/BatchStartFields.htm?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 执行流程数据表与业务表数据手工同步
        /// </summary>
        /// <returns></returns>
        public string DoBTableDTS()
        {
            Flow fl = new Flow(this.No);
            return fl.DoBTableDTS();

        }
        /// <summary>
        /// 回滚已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// </summary>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (DataType.IsNullOrEmpty(note) == true)
                return "请填写恢复已完成的流程原因.";
            if (note.Length <= 2)
                return "填写回滚原因不能少于三个字符.";

            Flow fl = new Flow(this.No);
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@错误，流程数据丢失。");

            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // 最后一个节点.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            try
            {
                #region 创建流程引擎主表数据.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 0)
                    return "err@丢失了 GenerWorkFlow 数据,无法回滚.";

                if (gwf.WFState != WFState.Complete)
                    return "err@仅仅能对已经完成的流程才能回滚,当前流程走到了["+gwf.NodeName+"]工作人员["+gwf.TodoEmps+"].";

                gwf.FlowNo = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.PNodeID = rpt.PNodeID;
                gwf.PEmp = rpt.PEmp;


                gwf.NodeID = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FlowSortNo = fl.FlowSortNo;
                gwf.SysType = fl.SysType;
                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /* 设置为退回的状态 */
                gwf.DeptNo = rpt.DeptNo;

                Dept dept = new Dept(empStarter.DeptNo);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd HH:mm:ss");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd HH:mm:ss");


                #endregion 创建流程引擎主表数据

                string ndTrack = "ND" + int.Parse(this.No) + "Track";
                string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@工作ID为:" + workid + "的数据不存在.");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                string todoEmps = "";
                int num = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();
                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // 增加上 工作人员的信息.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FlowNo = this.No;

                    gwl.NodeID = ndFrom;
                    gwl.NodeName = ndFromT;
                    gwl.ItIsPass = true;
                    if (gwl.NodeID == backToNodeID)
                    {
                        gwl.ItIsPass = false;
                        currWl = gwl;
                    }

                    gwl.EmpNo = EmpFrom;
                    gwl.EmpName= EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.EmpNo);
                    gwl.DeptNo = emp.DeptNo;
                    gwl.DeptName = emp.DeptText;


                    todoEmps += emp.UserID + "," + emp.Name + ";";
                    num++;

                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;
                    //gwl.WarningHour = nd.WarningHour;
                    gwl.ItIsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                //设置当前处理人员.
                gwf.SetValByKey(GenerWorkFlowAttr.TodoEmps, todoEmps);
                gwf.TodoEmpsNum = num;

                gwf.Update();
                //更新流程表的状态.
                rpt.FlowEnder = currWl.EmpNo;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.NodeID;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMsg(currWl.EmpNo, "工作恢复:" + gwf.Title, "工作被:" + WebUser.No + " 恢复." + note, "ReBack" + workid, BP.WF.SMSMsgType.SendSuccess, this.No, int.Parse(this.No + "01"), workid, 0);

                //写入该日志.
                WorkNode wn = new WorkNode(workid, currWl.NodeID);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.EmpNo, currWl.EmpName, currWl.NodeID, currWl.NodeName, note);

                return "@已经还原成功,现在的流程已经复原到(" + currWl.NodeName + "). @当前工作处理人为(" + currWl.EmpNo + " , " + currWl.EmpName + ")  @请通知他处理工作.";
            }
            catch (Exception ex)
            {
                //此表的记录删除已取消
                //gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>会滚期间出现错误</font><hr>" + ex.Message;
            }
        }
        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerFlowEmps()
        {
            if (WebUser.No != "admin")
                return "非admin用户不能执行。";

            Flow fl = new Flow(this.No);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.No);

            foreach (GenerWorkFlow gwf in gwfs)
            {
                string emps = "";
                string sql = "SELECT EmpFrom FROM ND" + int.Parse(this.No) + "Track  WHERE WorkID=" + gwf.WorkID;

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (emps.Contains("," + dr[0].ToString() + ","))
                        continue;
                }

                sql = "UPDATE " + fl.PTable + " SET FlowEmps='" + emps + "' WHERE OID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_GenerWorkFlow SET Emps='" + emps + "' WHERE WorkID=" + gwf.WorkID;
                DBAccess.RunSQL(sql);
            }

            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {
                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);


            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }

        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.No != "admin")
                return "非admin用户不能执行。";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);


            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);

            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }
        /// <summary>
        /// 流程监控
        /// </summary>
        /// <returns></returns>
        public string DoDataManger()
        {
            //PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Rpt/OneFlow.htm?FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=", 700, 500);
            return "../../Comm/Search.htm?s=d34&EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo=";
        }

        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="sd"></param>
        /// <returns></returns>
        public string DoDelDataOne(int workid, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(workid, true);
                return "删除成功 workid=" + workid + "  理由:" + note;
            }
            catch (Exception ex)
            {
                return "删除失败:" + ex.Message;
            }
        }


        /// <summary>
        /// 执行运行
        /// </summary>
        /// <returns></returns>
        public string DoRunIt()
        {
            return "../../Admin/TestFlow.htm?FK_Flow=" + this.No + "&Lang=CH";
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            //Flow fl = new Flow();
            //fl.No = this.No;
            //fl.RetrieveFromDBSources();

            return "/WF/Admin/AttrFlow/CheckFlow.htm?FK_Flow=" + this.No;

            //return fl.DoCheck();
        }

        /// <summary>
        /// 删除数据.
        /// </summary>
        /// <returns></returns>
        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }
        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.No);
            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            //同步流程数据表.
            string ndxxRpt = "ND" + int.Parse(this.No) + "Rpt";
            Flow fl = new Flow(this.No);
            if (fl.PTable != "ND" + int.Parse(this.No) + "Rpt")
            {
                BP.Sys.MapData md = new Sys.MapData(ndxxRpt);
                if (md.PTable != fl.PTable)
                    md.Update();
            }
            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowSheets : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.Template.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.Template.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowSheets() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowSheets(string fk_sort)
        {
            this.Retrieve(BP.WF.Template.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSheet();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowSheet> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowSheet>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowSheet> Tolist()
        {
            System.Collections.Generic.List<FlowSheet> list = new System.Collections.Generic.List<FlowSheet>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowSheet)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

