﻿using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Difference;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.Template.SFlow;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace BP.WF
{
    /// <summary>
    /// WF 的摘要说明.
    /// 工作流.
    /// 这里包含了两个方面
    /// 工作的信息．
    /// 流程的信息.
    /// </summary>
    public class WorkNode
    {
        #region 身份.
        private WebUserCopy _webUserCopy = null;
        public WebUserCopy WebUser
        {
            get
            {
                if (_webUserCopy == null)
                {
                    _webUserCopy = new WebUserCopy();
                    _webUserCopy.LoadWebUser();
                }
                return _webUserCopy;
            }
            set
            {
                _webUserCopy = value;
            }
        }
        private GuestUserCopy _guestUserCopy = null;
        public GuestUserCopy GuestUser
        {
            get
            {
                if (_guestUserCopy == null)
                {
                    _guestUserCopy = new GuestUserCopy();
                    _guestUserCopy.LoadWebUser();
                }
                return _guestUserCopy;
            }
            set
            {
                _guestUserCopy = value;
            }
        }

        #endregion 身份.


        #region 权限判断
        /// <summary>
        /// 判断一个人能不能对这个工作节点进行操作。
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        private bool IsCanOpenCurrentWorkNode(string empId)
        {
            WFState stat = this.HisGenerWorkFlow.WFState;
            if (stat == WFState.Runing)
            {
                if (this.HisNode.ItIsStartNode)
                {
                    /*如果是开始工作节点，从工作角色判断他有没有工作的权限。*/
                    return WorkFlow.IsCanDoWorkCheckByEmpStation(this.HisNode.NodeID, empId);
                }
                else
                {
                    /* 如果是初始化阶段,判断他的初始化节点 */
                    GenerWorkerList wl = new GenerWorkerList();
                    wl.WorkID = this.HisWork.OID;
                    wl.EmpNo = empId;

                    Emp myEmp = new Emp(empId);
                    wl.EmpName = myEmp.Name;

                    wl.NodeID = this.HisNode.NodeID;
                    wl.NodeName = this.HisNode.Name;
                    return wl.IsExits;
                }
            }
            else
            {
                /* 如果是初始化阶段 */
                return false;
            }
        }
        #endregion

        #region 属性/变量.
        /// <summary>
        /// 子线程是否有分组标志.
        /// </summary>
        public bool ItIsHaveSubThreadGroupMark = false;

        /// <summary>
        /// 执行人
        /// </summary>
        private string _execer11 = null;
        /// <summary>
        /// 实际执行人，执行工作发送时，有时候当前 WebUser.No 并非实际的执行人。
        /// </summary>
        public string Execer
        {
            get
            {
                if (_execer11 == null || _execer11 == "")
                {
                    if (WebUser.IsAuthorize == true)
                        _execer11 = WebUser.Auth;
                    else
                        _execer11 = WebUser.No;
                }
                return _execer11;
            }
            set
            {
                _execer11 = value;
            }
        }
        private string _execerName = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerName
        {
            get
            {
                if (_execerName == null || _execerName == "")
                {
                    if (WebUser.IsAuthorize == true)
                        _execerName = WebUser.AuthName;
                    else
                        _execerName = WebUser.Name;
                }
                return _execerName;
            }
            set
            {
                _execerName = value;
            }
        }
        private string _execerDeptName = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerDeptName
        {
            get
            {
                if (_execerDeptName == null)
                    _execerDeptName = WebUser.DeptName;
                return _execerDeptName;
            }
            set
            {
                _execerDeptName = value;
            }
        }
        private string _execerDeptNo = null;
        /// <summary>
        /// 实际执行人名称(请参考实际执行人)
        /// </summary>
        public string ExecerDeptNo
        {
            get
            {
                if (_execerDeptNo == null)
                    _execerDeptNo = WebUser.DeptNo;
                return _execerDeptNo;
            }
            set
            {
                _execerDeptNo = value;
            }
        }
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        private string _VirPath = null;
        /// <summary>
        /// 虚拟目录的路径 
        /// </summary>
        public string VirPath
        {
            get
            {
                if (_VirPath == null && BP.Difference.SystemConfig.isBSsystem)
                    _VirPath = Glo.CCFlowAppPath;//BP.Sys.Base.Glo.Request.ApplicationPath;
                return _VirPath;
            }
        }
        private string _AppType = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string AppType
        {
            get
            {
                if (BP.Difference.SystemConfig.isBSsystem == false)
                {
                    return "CCFlow";
                }

                if (_AppType == null && BP.Difference.SystemConfig.isBSsystem)
                {
                    _AppType = "WF";
                }
                return _AppType;
            }
        }
        private string nextStationName = "";
        public WorkNode town = null;
        private bool IsFindWorker = false;
        public bool ItIsSubFlowWorkNode
        {
            get
            {
                if (this.HisWork.FID == 0)
                    return false;
                else
                    return true;
            }
        }
        #endregion 属性/变量.

        #region GenerWorkerList 相关方法.
        //查询出每个节点表里的接收人集合（Emps）。
        public string GenerEmps(Node nd)
        {
            string str = "";
            foreach (GenerWorkerList wl in this.HisWorkerLists)
                str = wl.EmpNo + ",";
            return str;
        }
        /// <summary>
        /// 产生它的工作者
        /// </summary>
        /// <param name="town">WorkNode</param>
        /// <returns>产生的工作人员</returns>
        public GenerWorkerLists Func_GenerWorkerLists(WorkNode town)
        {
            this.town = town;
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            // 如果指定特定的人员处理。
            if (DataType.IsNullOrEmpty(JumpToEmp) == false)
            {
                string[] myEmpStrs = JumpToEmp.Split(',');
                foreach (string emp in myEmpStrs)
                {
                    if (DataType.IsNullOrEmpty(emp))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = emp;
                    dt.Rows.Add(dr);
                }

                /*如果是抢办或者共享.*/

                // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误。
                ps = new Paras();
                ps.Add("WorkID", this.HisWork.OID);
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
                DBAccess.RunSQL(ps);

                return InitWorkerLists(town, dt);
            }

            // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误,
            ps = new Paras();
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
            DBAccess.RunSQL(ps);

            //开始找人.
            FindWorker fw = new FindWorker();
            fw.currWn = this;
            Node toNode = town.HisNode;
            if ((this.TodolistModel == TodolistModel.Teamup || this.TodolistModel == TodolistModel.TeamupGroupLeader)
                && (toNode.HisDeliveryWay == DeliveryWay.ByStation || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptLeader || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptStations))
                return Teamup_InitWorkerLists(fw, town);

            dt = fw.DoIt(this.HisFlow, this, town);
            if (dt == null)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人.", "WorkNode", "not_found_receiver"));

            return InitWorkerLists(town, dt);
        }

        /// <summary>
        /// 子线程获取下一个节点的处理人
        /// </summary>
        /// <param name="town"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GenerWorkerLists Func_GenerWorkerLists_Thread(WorkNode town)
        {
            this.town = town;
            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            // 如果指定特定的人员处理。
            if (DataType.IsNullOrEmpty(JumpToEmp) == false)
            {
                string[] myEmpStrs = JumpToEmp.Split(',');
                foreach (string emp in myEmpStrs)
                {
                    if (DataType.IsNullOrEmpty(emp))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = emp;
                    dt.Rows.Add(dr);
                }

                /*如果是抢办或者共享.*/

                // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误。
                ps = new Paras();
                ps.Add("WorkID", this.HisWork.OID);
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
                DBAccess.RunSQL(ps);

                return InitWorkerLists(town, dt);
            }

            // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误,
            ps = new Paras();
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
            DBAccess.RunSQL(ps);

            /*如果设置了安ccbpm的BPM模式*/
            FindWorker fw = new FindWorker();
            fw.currWn = this;
            //如果是协作模式且下一个节点的接收人和身份相关的处理 
            Node toNode = town.HisNode;
            if ((this.TodolistModel == TodolistModel.Teamup || this.TodolistModel == TodolistModel.TeamupGroupLeader)
                && (toNode.HisDeliveryWay == DeliveryWay.ByStation || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptLeader || toNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptStations))
                return Teamup_InitWorkerLists(fw, town);

            dt = fw.DoIt(this.HisFlow, this, town);
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return InitWorkerLists(town, dt);
        }

        /// <summary>
        /// 协作模式下处理下一个节点的接收人
        /// </summary>
        /// <param name="fw"></param>
        /// <param name="town"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private GenerWorkerLists Teamup_InitWorkerLists(FindWorker fw, WorkNode town)
        {
            string currEmpNo = WebUser.No;
            string currDeptNo = WebUser.DeptNo;
            try
            {
                //下一个节点的接收人集合
                DataTable empDt = new DataTable();
                empDt.Columns.Add("No");

                //获取处理当前节点业务所有人所在部门集合
                string sql = "SELECT FK_Emp,FK_Dept From WF_GenerWorkerlist Where WorkID=" + this.WorkID + " AND FK_Node=" + this.HisNode.NodeID + " AND (IsPass=1 OR IsPass=0)";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@不可能出现的错误");
                if (dt.Rows.Count == 1)
                {
                    dt = fw.DoIt(this.HisFlow, this, town);
                    if (dt == null)
                        throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人.", "WorkNode", "not_found_receiver"));
                    return InitWorkerLists(town, dt);
                }
                string deptNos = ",";
                string deptNo = "";
                string empNo = "";
                foreach (DataRow dr in dt.Rows)
                {
                    empNo = dr[0].ToString();
                    deptNo = dr[1].ToString();
                    if (deptNos.Contains("," + deptNo + ",") == true)
                        continue;
                    if (empNo.Equals(WebUser.No) == true)
                    {
                        DataTable ddt = fw.DoIt(this.HisFlow, this, town);
                        if (ddt != null && ddt.Rows.Count > 0)
                            empDt.Merge(ddt, true);
                    }
                    else
                    {
                        WebUser.No = empNo;
                        WebUser.DeptNo = deptNo;
                        DataTable ddt = fw.DoIt(this.HisFlow, this, town);
                        if (ddt != null && ddt.Rows.Count > 0)
                            empDt.Merge(ddt, true);
                    }
                }
                if (empDt.Rows.Count == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人.", "WorkNode", "not_found_receiver"));
                return InitWorkerLists(town, empDt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                WebUser.No = currEmpNo;
                WebUser.DeptNo = currDeptNo;

            }
        }
        #endregion GenerWorkerList 相关方法.

        string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
        public Paras ps = new Paras();
        /// <summary>
        /// 递归删除两个节点之间的数据
        /// </summary>
        /// <param name="nds">到达的节点集合</param>
        public void DeleteToNodesData(Nodes nds)
        {
            return;
        }

        #region 根据工作角色生成工作者
        private Node _ndFrom = null;
        private Node ndFrom
        {
            get
            {
                if (_ndFrom == null)
                    _ndFrom = this.HisNode;
                return _ndFrom;
            }
            set
            {
                _ndFrom = value;
            }
        }
        /// <summary>
        /// 初始化工作人员
        /// </summary>
        /// <param name="town">到达的wn</param>
        /// <param name="dt">数据源</param>
        /// <param name="fid">FID</param>
        /// <returns>GenerWorkerLists</returns>
        private GenerWorkerLists InitWorkerLists(WorkNode town, DataTable dt, Int64 fid = 0)
        {
            if (dt.Rows.Count == 0)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人,InitWorkerLists.", "WorkNode", "not_found_receiver")); // 接收人员列表为空

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                string orgNo = BP.Web.WebUser.OrgNo;
                foreach (DataRow dr in dt.Rows)
                {
                    string str = dr[0] as string;
                    if (str == null)
                        continue;
                    str = str.Replace(orgNo + "_", "");
                    dr[0] = str;
                }
            }

            this.HisGenerWorkFlow.TodoEmpsNum = -1;

            #region 判断发送的类型，处理相关的FID.
            // 定义下一个节点的接收人的 FID 与 WorkID.
            Int64 nextUsersWorkID = this.WorkID;
            Int64 nextUsersFID = this.HisWork.FID;

            // 是否是分流到子线程。
            bool isFenLiuToSubThread = false;
            if (this.HisNode.ItIsFLHL == true
                && town.HisNode.ItIsSubThread == true)
            {
                isFenLiuToSubThread = true;
                nextUsersWorkID = 0;
                nextUsersFID = this.HisWork.OID;
            }


            // 子线程 到 合流点or 分合流点.
            bool isSubThreadToFenLiu = false;
            if (this.HisNode.ItIsSubThread == true
                && town.HisNode.ItIsFLHL == true)
            {
                nextUsersWorkID = this.HisWork.FID;
                nextUsersFID = 0;
                isSubThreadToFenLiu = true;
            }

            // 子线程到子线程.
            bool isSubthread2Subthread = false;
            if (this.HisNode.ItIsSubThread == true && town.HisNode.ItIsSubThread == true)
            {
                nextUsersWorkID = this.HisWork.OID;
                nextUsersFID = this.HisWork.FID;
                isSubthread2Subthread = true;
            }
            #endregion 判断发送的类型，处理相关的FID.

            int toNodeId = town.HisNode.NodeID;
            this.HisWorkerLists = new GenerWorkerLists();
            this.HisWorkerLists.Clear();

            #region 限期时间  town.HisNode.TSpan-1

            DateTime dtOfShould = DateTime.Now;

            if (town.HisNode.HisCHWay == CHWay.ByTime)
            {
                CHNode chNode = new CHNode();
                chNode.setMyPK(this.HisGenerWorkFlow.WorkID + "_" + this.town.HisNode.NodeID);
                if (chNode.RetrieveFromDBSources() != 0)
                    dtOfShould = DateTime.Parse(chNode.EndDT);
                else
                {
                    //按天、小时考核
                    if (town.HisNode.GetParaInt("CHWayOfTimeRole") == 0)
                    {
                        //增加天数. 考虑到了节假日. 
                        //判断是修改了节点期限的天数
                        int timeLimit = this.town.HisNode.TimeLimit;
                        dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, timeLimit,
                        this.town.HisNode.TimeLimitHH, this.town.HisNode.TimeLimitMM, this.town.HisNode.TWay);
                    }
                    //按照节点字段设置
                    if (town.HisNode.GetParaInt("CHWayOfTimeRole") == 1)
                    {
                        //获取设置的字段、
                        string keyOfEn = town.HisNode.GetParaString("CHWayOfTimeRoleField");
                        if (DataType.IsNullOrEmpty(keyOfEn) == true)
                            town.HisNode.HisCHWay = CHWay.None;
                        else
                            dtOfShould = DataType.ParseSysDateTime2DateTime(this.HisWork.GetValByKey(keyOfEn).ToString());

                    }
                }

                //流转自定义的流程并且考核规则按照流转自定义设置
                //if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByWorkerSet
                //    && town.HisNode.GetParaInt("CHWayOfTimeRole") == 2)
                //{
                //    //获取当前节点的流转自定义时间
                //    TransferCustom tf = new TransferCustom();
                //    tf.setMyPK(town.HisNode.NodeID + "_" + this.WorkID;
                //    if (tf.RetrieveFromDBSources() != 0)
                //    {
                //        if (DataType.IsNullOrEmpty(tf.PlanDT) == true)
                //            throw new Exception("err@在流转自定义期间，没有设置计划完成日期。");
                //        dtOfShould = DataType.ParseSysDateTime2DateTime(tf.PlanDT);
                //    }
                //}
            }
            else if (town.HisNode.HisCHWay == CHWay.None)
            {
                //添加默认应处理时间
                dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, 2,
                        0, 0, BP.DA.TWay.Holiday);
            }

            //求警告日期.
            DateTime dtOfWarning = DateTime.Now;
            if (this.town.HisNode.WarningDay == 0)
            {
                dtOfWarning = dtOfShould;
            }
            else
            {
                //计算警告日期.
                //增加小时数. 考虑到了节假日.
                dtOfWarning = Glo.AddDayHoursSpan(DateTime.Now, (int)this.town.HisNode.WarningDay, 0, 0, this.town.HisNode.TWay);
            }

            switch (this.HisNode.HisNodeWorkType)
            {
                case NodeWorkType.StartWorkFL:
                case NodeWorkType.WorkFHL:
                case NodeWorkType.WorkFL:
                case NodeWorkType.WorkHL:
                    break;
                default:
                    this.HisGenerWorkFlow.NodeID = town.HisNode.NodeID;
                    this.HisGenerWorkFlow.SDTOfNode = DataType.SysDateTimeFormat(dtOfShould);
                    //暂时注释掉，忘记使用情况
                    //this.HisGenerWorkFlow.SetPara("CH" + this.town.HisNode.NodeID, this.HisGenerWorkFlow.SDTOfNode);
                    //this.HisGenerWorkFlow.SDTOfFlow = dtOfFlow.ToString(DataType.SysDateTimeFormat);
                    //this.HisGenerWorkFlow.SDTOfFlowWarning = dtOfFlowWarning.ToString(DataType.SysDateTimeFormat);
                    this.HisGenerWorkFlow.TodoEmpsNum = dt.Rows.Count;
                    break;
            }
            #endregion 限期时间  town.HisNode.TSpan-1

            #region 处理 人员列表 数据源。
            // 定义是否有分组mark. 如果有三列，就说明该集合中有分组 mark. 就是要处理一个人多个子线程的情况.

            this.ItIsHaveSubThreadGroupMark = true;

            if (dt.Columns.Count <= 2)
                this.ItIsHaveSubThreadGroupMark = false;

            if (dt.Rows.Count == 1)
            {
                /* 如果只有一个人员 */
                GenerWorkerList wl = new GenerWorkerList();
                if (isFenLiuToSubThread)
                {
                    /*  说明这是分流点向下发送
                     *  在这里产生临时的workid.
                     */
                    wl.WorkID = DBAccess.GenerOIDByGUID();
                }
                else
                {
                    wl.WorkID = nextUsersWorkID;
                }
                wl.FID = nextUsersFID;
                wl.NodeID = toNodeId;
                wl.NodeName = town.HisNode.Name;
                wl.EmpNo = dt.Rows[0][0].ToString();
                Emp emp = new Emp();
                emp.UserID = wl.EmpNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    string[] para = new string[1];
                    para[0] = wl.EmpNo;
                    string str = BP.WF.Glo.multilingual("@设置接收人规则错误, 接收人[" + wl.EmpNo + "][{0}]不存在或者被停用。", "WorkNode", "invalid_setting_receiver", para);
                    throw new Exception("err@" + str);
                }

                wl.EmpName = emp.Name;
                if (dt.Rows[0].Table.Columns.Contains("FK_Dept"))
                {
                    wl.DeptNo = dt.Rows[0][1].ToString();
                    Dept dept = new Dept(wl.DeptNo);
                    wl.DeptName = dept.Name;
                    if (dept.RetrieveFromDBSources() == 0)
                    {
                        wl.DeptNo = emp.DeptNo;
                        wl.DeptName = emp.DeptText;
                    }
                }
                else
                {
                    wl.DeptNo = emp.DeptNo;
                    wl.DeptName = emp.DeptText;
                }
                wl.WhoExeIt = town.HisNode.WhoExeIt; //设置谁执行它.

                //应完成日期.
                if (town.HisNode.HisCHWay == CHWay.None)
                    wl.SDT = "无";
                else
                    wl.SDT = DataType.SysDateTimeFormat(dtOfShould);

                //警告日期.
                wl.DTOfWarning = DataType.SysDateTimeFormat(dtOfWarning);

                wl.FlowNo = town.HisNode.FlowNo;

                // and 2015-01-14 , 如果有三列，就约定为最后一列是分组标志， 有标志就把它放入标志里 .
                if (this.ItIsHaveSubThreadGroupMark == true)
                {
                    wl.GroupMark = dt.Rows[0][2].ToString(); //第3个列是分组标记.
                    if (DataType.IsNullOrEmpty(wl.GroupMark))
                    {
                        string[] para = new string[1];
                        para[0] = wl.EmpNo;
                        BP.WF.Glo.multilingual("@[{0}]分组标记中没有值,会导致无法按照分组标记去生成子线程,请检查配置的信息是否正确.", "WorkNode", "no_value_in_group_tags", para);
                    }
                }

                //设置发送人.
                if (BP.Web.WebUser.No == "Guest")
                {
                    wl.Sender = GuestUser.No + "," + GuestUser.Name;
                    wl.GuestNo = GuestUser.No;
                    wl.GuestName = GuestUser.Name;
                }
                else
                {
                    wl.Sender = WebUser.No + "," + WebUser.Name;
                }

                //判断下一个节点是否是外部用户处理人节点？ 
                if (town.HisNode.ItIsGuestNode)
                {
                    if (this.HisGenerWorkFlow.GuestNo != "")
                    {
                        wl.GuestNo = this.HisGenerWorkFlow.GuestNo;
                        wl.GuestName = this.HisGenerWorkFlow.GuestName;
                    }
                    else
                    {
                        /*这种情况是，不是外部用户发起的流程。*/
                        if (town.HisNode.HisDeliveryWay == DeliveryWay.BySQL)
                        {
                            string mysql = town.HisNode.DeliveryParas.Clone() as string;
                            DataTable mydt = DBAccess.RunSQLReturnTable(Glo.DealExp(mysql, this.rptGe));

                            wl.GuestNo = mydt.Rows[0][0].ToString();
                            wl.GuestName = mydt.Rows[0][1].ToString();

                            this.HisGenerWorkFlow.GuestNo = wl.GuestNo;
                            this.HisGenerWorkFlow.GuestName = wl.GuestName;
                        }
                        else if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField)
                        {
                            wl.GuestNo = this.HisWork.GetValStrByKey(town.HisNode.DeliveryParas);
                            wl.GuestName = "外部用户";
                            this.HisGenerWorkFlow.GuestNo = wl.GuestNo;
                            this.HisGenerWorkFlow.GuestName = wl.GuestName;
                        }
                        else
                        {
                            string[] para = new string[1];
                            para[0] = this.town.HisNode.Name;
                            BP.WF.Glo.multilingual("@当前节点[{0}]是中间节点，并且是外部用户处理节点，您需要正确的设置这个外部用户接收人规则。", "WorkNode", "invalid_setting_external_receiver", para);
                        }
                    }
                }

                wl.Insert();
                this.HisWorkerLists.AddEntity(wl);

                RememberMe rm = new RememberMe(); // this.GetHisRememberMe(town.HisNode);
                rm.Objs = "@" + wl.EmpNo + "@";
                rm.ObjsExt += BP.WF.Glo.DealUserInfoShowModel(wl.EmpNo, wl.EmpName);
                rm.Emps = "@" + wl.EmpNo + "@";
                rm.EmpsExt = BP.WF.Glo.DealUserInfoShowModel(wl.EmpNo, wl.EmpName);
                this.HisRememberMe = rm;
            }

            //为了缓解代码量，与处理效率问题。
            if (dt.Rows.Count > 1)
            {
                WorkNodePlus.InitWorkerList_Ext(this, town, dt, toNodeId, dtOfShould, dtOfWarning, nextUsersFID, isFenLiuToSubThread, nextUsersWorkID);
            }

            if (this.HisWorkerLists.Count == 0)
            {
                string[] para = new string[3];
                para[0] = this.town.HisNode.HisRunModel.ToString();
                para[1] = this.town.HisNode.Name;
                para[2] = this.HisWorkFlow.HisFlow.Name;
                BP.WF.Glo.multilingual("@根据部门[{0}]产生工作人员出现错误，流程[{1}]中节点[{2}]定义错误,没有找到接收此工作的工作人员.", "WorkNode", "generate_receiver_error_by_depart", para);
            }
            #endregion 处理 人员列表 数据源。

            #region 设置流程数量,其他的信息为任务池提供数据。
            string hisEmps = "";
            int num = 0;
            foreach (GenerWorkerList wl in this.HisWorkerLists)
            {
                if (wl.PassInt == 0 && wl.ItIsEnable == true)
                {
                    num++;
                    hisEmps += wl.EmpNo + "," + wl.EmpName + ";";
                }
            }

            if (num == 0)
                throw new Exception("@不应该产生的结果错误,没有找到接受人.");

            this.HisGenerWorkFlow.TodoEmpsNum = num;
            this.HisGenerWorkFlow.TodoEmps = hisEmps;
            #endregion

            #region  求出日志类型，并加入变量中。
            ActionType at = ActionType.Forward;
            switch (town.HisNode.HisNodeWorkType)
            {
                case NodeWorkType.StartWork:
                case NodeWorkType.StartWorkFL:
                    at = ActionType.Start;
                    break;
                case NodeWorkType.Work:
                    if (this.HisNode.HisNodeWorkType == NodeWorkType.WorkFL
                        || this.HisNode.HisNodeWorkType == NodeWorkType.WorkFHL)
                        at = ActionType.ForwardFL;
                    else
                        at = ActionType.Forward;
                    break;
                case NodeWorkType.WorkHL:
                    at = ActionType.ForwardHL;
                    break;
                case NodeWorkType.SubThreadWork:

                    switch (this.HisNode.HisNodeWorkType)
                    {
                        case NodeWorkType.StartWorkFL:
                        case NodeWorkType.WorkFL:
                        case NodeWorkType.WorkFHL:
                            at = ActionType.ForwardFL;
                            break;
                        case NodeWorkType.WorkHL:
                            throw new Exception(BP.WF.Glo.multilingual("err@流程设计错误: 当前节点是合流节点，而到达的节点是子线程。", "WorkNode", "workflow_error_1", new string[0]));
                            break;
                        case NodeWorkType.Work:
                            throw new Exception(BP.WF.Glo.multilingual("err@流程设计错误: 当前节点是合流节点，而到达的节点是子线程。", "WorkNode", "workflow_error_2", new string[0]));
                            break;
                        default:
                            at = ActionType.Forward;
                            break;
                    }

                    break;
                default:
                    break;
            }
            #endregion  求出日志类型，并加入变量中。

            #region 如果是子线城前进.
            if (at == ActionType.SubThreadForward)
            {
                string emps = "";
                foreach (GenerWorkerList wl in this.HisWorkerLists)
                {
                    this.AddToTrack(at, wl, BP.WF.Glo.multilingual("子线程", "WorkNode", "sub_thread", new string[0]), this.town.HisWork.OID, this.HisNode);
                }
                //写入到日志.
            }
            #endregion 如果是子线城前进.

            #region 如果是非子线城前进.
            if (at != ActionType.SubThreadForward)
            {
                if (this.HisWorkerLists.Count == 1)
                {
                    GenerWorkerList wl = this.HisWorkerLists[0] as GenerWorkerList;
                    this.AddToTrack(at, wl.EmpNo, wl.EmpName, wl.NodeID, wl.NodeName, null, this.ndFrom, null);
                }
                else
                {
                    string[] para = new string[1];
                    para[0] = this.HisWorkerLists.Count.ToString();
                    string info = BP.WF.Glo.multilingual("共({0})人接收:", "WorkNode", "total_receivers", para);

                    string empNos = "";
                    string empNames = "";
                    foreach (GenerWorkerList wl in this.HisWorkerLists)
                    {
                        info += BP.WF.Glo.DealUserInfoShowModel(wl.DeptName, wl.EmpName) + ":";

                        empNos += wl.EmpNo + ",";
                        empNames += wl.EmpName + ",";
                    }

                    //写入到日志.
                    this.AddToTrack(at, empNos, empNames, town.HisNode.NodeID, town.HisNode.Name, BP.WF.Glo.multilingual("多人接收(见信息栏)", "WorkNode", "multiple_receivers", new string[0]), this.ndFrom, info);
                }
            }
            #endregion 如果是非子线城前进.

            #region 把数据加入变量中.
            string ids = "";
            string names = "";
            string idNames = "";
            if (this.HisWorkerLists.Count == 1)
            {
                GenerWorkerList gwl = (GenerWorkerList)this.HisWorkerLists[0];
                ids = gwl.EmpNo;
                names = gwl.EmpName;

                //设置状态。
                this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            }
            else
            {
                foreach (GenerWorkerList gwl in this.HisWorkerLists)
                {
                    ids += gwl.EmpNo + ",";
                    names += gwl.EmpName + ",";
                }

                //设置状态, 如果该流程使用了启用共享任务池。
                if (town.HisNode.ItIsEnableTaskPool && this.HisNode.HisRunModel == RunModel.Ordinary)
                    this.HisGenerWorkFlow.TaskSta = TaskSta.Sharing;
                else
                    this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            }

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, ids, ids, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, names, names, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersNID, idNames, idNames, SendReturnMsgType.SystemMsg);
            #endregion

            return this.HisWorkerLists;
        }
        #endregion

        #region 条件
        private Conds _HisNodeCompleteConditions = null;
        /// <summary>
        /// 节点完成任务的条件
        /// 条件与条件之间是or 的关系, 就是说,如果任何一个条件满足,这个工作人员在这个节点上的任务就完成了.
        /// </summary>
        public Conds HisNodeCompleteConditions
        {
            get
            {
                if (this._HisNodeCompleteConditions == null)
                {
                    _HisNodeCompleteConditions = new Conds(CondType.Node, this.HisNode.NodeID, this.WorkID, this.rptGe);
                    return _HisNodeCompleteConditions;
                }
                return _HisNodeCompleteConditions;
            }
        }
        #endregion

        #region 关于质量考核
        ///// <summary>
        ///// 得到以前的已经完成的工作节点.
        ///// </summary>
        ///// <returns></returns>
        //public WorkNodes GetHadCompleteWorkNodes()
        //{
        //    WorkNodes mywns = new WorkNodes();
        //    WorkNodes wns = new WorkNodes(this.HisNode.HisFlow, this.HisWork.OID);
        //    foreach (WorkNode wn in wns)
        //    {
        //        if (wn.IsComplete)
        //            mywns.Add(wn);
        //    }
        //    return mywns;
        //}
        #endregion

        #region 流程公共方法
        private Flow _HisFlow = null;
        public Flow HisFlow
        {
            get
            {
                if (_HisFlow == null)
                    _HisFlow = this.HisNode.HisFlow;
                return _HisFlow;
            }
        }
        public Node JumpToNode = null;
        public string JumpToEmp = null;

        #region NodeSend 的附属功能.
        public Node NodeSend_GenerNextStepNode(bool IsFullSA = false)
        {
            Node node = NodeSend_GenerNextStepNodeExt(IsFullSA);
            if (node.HisBatchRole == BatchRole.None)
                this.HisGenerWorkFlow.ItIsCanBatch = false;
            else
                this.HisGenerWorkFlow.ItIsCanBatch = true;

            return node;
        }
        /// <summary>
        /// 获得下一个节点.
        /// </summary>
        /// <returns></returns>
        private Node NodeSend_GenerNextStepNodeExt(bool isFullSA = false)
        {
            //如果要是跳转到的节点，自动跳转规则规则就会失效.
            if (this.JumpToNode != null)
                return this.JumpToNode;

            //如果是自动计算未来接受人.
            if (this.HisFlow.ItIsFullSA == true && this.HisNode.ItIsStartNode == false && isFullSA == true)
            {
                //判断是否需要重新计算？根据版本号。
                if (this.HisGenerWorkFlow.GetParaString("SADataVer").Equals(this.HisFlow.GetParaString("SADataVer")) == false)
                {
                    FullSA sa = new FullSA();
                    sa.DoIt2023(this); //设置重新计算接收人方向.
                    this.HisGenerWorkFlow.SetPara("SADataVer", this.HisGenerWorkFlow.GetParaString("SADataVer"));
                }

                string sql = "SELECT A.ToNode,FK_Emp FROM WF_Direction A, WF_SelectAccper B WHERE A.Node=" + this.HisNode.NodeID + " AND A.ToNode=B.FK_Node AND B.WorkID=" + this.WorkID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@当前节点[" + this.HisNode.NodeID + "," + this.HisNode.Name + "]没有找到下一步节点." + sql);
                string nodeID = dt.Rows[0][0].ToString();

                string strs = ""; //生成下一步节点.
                foreach (DataRow dr in dt.Rows)
                {
                    strs += dr[1].ToString() + ",";
                }

                this.JumpToNode = new Node(int.Parse(nodeID));
                this.JumpToEmp = strs;
                return this.JumpToNode;
            }

            // 被zhoupeng注释，因为有可能遇到跳转.
            //if (this.HisNode.HisToNodes.Count == 1)
            //    return (Node)this.HisNode.HisToNodes[0];

            // 判断是否有用户选择的节点.
            if (this.HisNode.CondModel == DirCondModel.ByPopSelect)
            {
                // 获取用户选择的节点.
                string nodes = this.HisGenerWorkFlow.Paras_ToNodes;
                if (DataType.IsNullOrEmpty(nodes) || nodes.Equals(",") == true)
                {
                    if (this.HisNode.HisToNodes.Count == 1)
                        return this.HisNode.HisToNodes[0] as Node;

                    throw new Exception(BP.WF.Glo.multilingual("@用户没有选择发送到的节点.", "WorkNode", "no_choice_of_target_node", new string[0]));
                }

                string[] mynodes = nodes.Split(',');
                foreach (string item in mynodes)
                {
                    if (DataType.IsNullOrEmpty(item))
                        continue;

                    //排除到达自身节点.
                    if (this.HisNode.NodeID.ToString() == item)
                        continue;

                    return new Node(int.Parse(item));
                }

                //设置他为空,以防止下一次发送出现错误.
                this.HisGenerWorkFlow.Paras_ToNodes = "";
            }

            Node nd = NodeSend_GenerNextStepNode_Ext1();
            return nd;
        }
        /// <summary>
        /// 知否执行了跳转.
        /// </summary>
        public bool ItIsSkip = false;
        /// <summary>
        /// 获取下一步骤的工作节点.
        /// </summary>
        /// <returns></returns>
        public Node NodeSend_GenerNextStepNode_Ext1()
        {
            //如果要是跳转到的节点，自动跳转规则规则就会失效。
            if (this.JumpToNode != null)
                return this.JumpToNode;

            Node mynd = this.HisNode;
            Work mywork = this.HisWork;
            int beforeSkipNodeID = 0;   //added by liuxc,2015-7-13,标识自动跳转之前的节点ID


            #region (最后)判断是否有延续流程.
            if (this.HisNode.SubFlowYanXuNum > 0)
            {
                SubFlowYanXus ygflows = new SubFlowYanXus(this.HisNode.NodeID);
                if (ygflows.Count != 0 && 1 == 2)
                {
                    foreach (SubFlowYanXu item in ygflows)
                    {
                        bool isPass = false;

                        if (item.ExpType == ConnDataFrom.Paras)
                            isPass = BP.WF.Glo.CondExpPara(item.CondExp, this.rptGe.Row, this.WorkID);

                        if (item.ExpType == ConnDataFrom.SQL)
                            isPass = BP.WF.Glo.CondExpSQL(item.CondExp, this.rptGe.Row, this.WorkID);

                        if (isPass == true)
                            return new Node(int.Parse(item.SubFlowNo + "01"));
                    }
                }
            }
            #endregion (最后)判断是否有延续流程.

            #region 计算到达的节点.
            this.ndFrom = this.HisNode;
            string Executor = "";//实际执行人
            string ExecutorName = "";//实际执行人名称
            while (true)
            {
                //上一步的工作节点.
                int prvNodeID = mynd.NodeID;
                if (mynd.ItIsEndNode)
                {
                    /*如果是最后一个节点了,仍然找不到下一步节点...*/
                    if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                    {
                        this.HisWorkFlow.HisGenerWorkFlow.NodeID = mynd.NodeID;
                        this.HisWorkFlow.HisGenerWorkFlow.NodeName = mynd.Name;
                        this.HisGenerWorkFlow.NodeID = mynd.NodeID;
                        this.HisGenerWorkFlow.NodeName = mynd.Name;
                        this.HisGenerWorkFlow.Update();
                        if (DataType.IsNullOrEmpty(Executor) == false)
                        {
                            // this.Execer = Executor;
                            this.ExecerName = ExecutorName;
                        }
                        String msg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "流程已经走到最后一个节点，流程成功结束。",
                                mynd, this.rptGe, 0, Executor, ExecutorName);
                        this.addMsg(SendReturnMsgFlag.End, msg);
                        this.addMsg(SendReturnMsgFlag.IsStopFlow, "1", BP.WF.Glo.multilingual("流程已经结束", "WorkNode", "wf_end_success"), SendReturnMsgType.Info);

                        this.IsStopFlow = true;
                    }

                    return mynd;
                }

                // 获取它的下一步节点.
                Node nd = this.NodeSend_GenerNextStepNode_Ext(mynd);
                nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

                mynd = nd;
                Work skipWork = null;
                if (mywork.NodeFrmID != nd.NodeFrmID)
                {
                    /* 跳过去的节点也要写入数据，不然会造成签名错误。*/
                    skipWork = nd.HisWork;

                    if (skipWork.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
                    {
                        skipWork.Copy(this.rptGe);
                        skipWork.Copy(mywork);

                        skipWork.OID = this.WorkID;
                        if (nd.ItIsSubThread == true)
                            skipWork.FID = mywork.FID;

                        skipWork.Rec = this.Execer;

                        skipWork.ResetDefaultVal();

                        // 把里面的默认值也copy报表里面去.
                        rptGe.Copy(skipWork);

                        //如果存在就修改
                        if (skipWork.IsExit(skipWork.PK, this.WorkID) == true)
                        {
                            int count = skipWork.RetrieveFromDBSources();
                            if (count == 1)
                                skipWork.DirectUpdate();
                            else
                                skipWork.DirectInsert();
                        }
                        else
                            skipWork.InsertAsOID(this.WorkID);
                    }

                    ItIsSkip = true;
                    mywork = skipWork;
                }

                if (DataType.IsNullOrEmpty(Executor) == false)
                {
                    this.Execer = Executor;
                    this.ExecerName = ExecutorName;
                }

                //判断是否是跳转时间 0=发送的时候检测。. 1=打开的时候检测.
                if (nd.SkipTime == 1)
                    return nd;

                //如果没有设置跳转规则，就返回他们.
                if (nd.AutoJumpRole0 == false && nd.AutoJumpRole1 == false && nd.AutoJumpRole2 == false && nd.AutoJumpRole3 == false && nd.HisWhenNoWorker == false)
                    return nd;

                DataTable dt = null;
                FindWorker fw = new FindWorker();
                WorkNode toWn = new WorkNode(this.WorkID, nd.NodeID);
                if (skipWork == null)
                    skipWork = toWn.HisWork;

                dt = fw.DoIt(this.HisFlow, this, toWn); // 找到下一步骤的接收人.
                Executor = "";//实际执行人
                ExecutorName = "";//  实际执行人名称
                Emp emp = new Emp();
                if (dt == null || dt.Rows.Count == 0)
                {
                    if (nd.HisWhenNoWorker == true)
                    {
                        this.AddToTrack(ActionType.Skip, this.Execer, this.ExecerName,
                            nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(启用跳转规则,当没有找到处理人时)", "WorkNode", "system_error_jump_automatically_1", new string[0]), ndFrom);
                        ndFrom = nd;
                        continue;
                    }
                    else
                    {
                        //抛出异常.
                        string[] para = new string[1];
                        para[0] = nd.Name;
                        throw new Exception(BP.WF.Glo.multilingual("@没有找到节点[{0}]的处理人", "WorkNode", "system_error_not_found_operator", para));
                    }
                }

                #region 处理人就是发起人
                if (nd.AutoJumpRole0)
                {
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 如果出现了 处理人就是发起人的情况.
                        if (dr[0].ToString() == this.HisGenerWorkFlow.Starter)
                        {
                            #region 处理签名，让签名的人是发起人。


                            Attrs attrs = skipWork.EnMap.Attrs;
                            bool isUpdate = false;
                            foreach (Attr attr in attrs)
                            {
                                if (attr.UIIsReadonly && attr.UIVisible == true
                                    && DataType.IsNullOrEmpty(attr.DefaultValOfReal) == false)
                                {
                                    if (attr.DefaultValOfReal == "@WebUser.No")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.Starter);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.Name")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.StarterName);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.DeptNo")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.DeptNo);
                                        isUpdate = true;
                                    }
                                    if (attr.DefaultValOfReal == "@WebUser.DeptName")
                                    {
                                        skipWork.SetValByKey(attr.Key, this.HisGenerWorkFlow.DeptName);
                                        isUpdate = true;
                                    }
                                }
                            }
                            if (isUpdate)
                                skipWork.DirectUpdate();
                            #endregion 处理签名，让签名的人是发起人。

                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        /*如果发现了，当前人员包含处理人集合. */
                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(处理人就是发起人)", "WorkNode", "system_error_jump_automatically_2", new string[0]), ndFrom);
                        //增加当前节点的处理人的GenerWorkerList
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = this.WorkID;
                        gwl.FID = this.HisGenerWorkFlow.FID;
                        gwl.NodeID = nd.NodeID;
                        gwl.NodeName = nd.Name;
                        gwl.EmpNo = Executor;
                        gwl.EmpName = ExecutorName;
                        gwl.DeptNo = emp.DeptNo;
                        gwl.DeptName = emp.DeptText;
                        gwl.WhoExeIt = nd.WhoExeIt;
                        gwl.SDT = DataType.CurrentDateTime;
                        gwl.DTOfWarning = DataType.CurrentDateTime;
                        gwl.CDT = DataType.CurrentDateTime;
                        gwl.FlowNo = nd.FlowNo;
                        gwl.Sender = WebUser.No + "," + WebUser.Name;
                        gwl.PassInt = 1;
                        gwl.ItIsRead = true;
                        gwl.Insert();
                        ExecEvent.DoNode(EventListNode.SendWhen, nd, skipWork, null);

                        ndFrom = nd;
                        ExecEvent.DoNode(EventListNode.SendSuccess, mynd, skipWork, this.HisMsgObjs, null);

                        CC(nd);//执行抄送
                        continue;
                    }

                    //如果没有跳转,判断是否,其他两个条件是否设置.
                    if (nd.AutoJumpRole1 == false && nd.AutoJumpRole2 == false && nd.AutoJumpRole3 == false)
                        return nd;
                }
                #endregion

                #region 处理人已经出现过
                if (nd.AutoJumpRole1 == true)
                {
                    if (this.HisFlow.ItIsFullSA == false)
                        return nd;
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 如果出现了处理人就是提交人的情况.
                        string sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Emp='" + dr[0].ToString() + "' AND WorkID=" + this.WorkID;
                        if (DBAccess.RunSQLReturnValInt(sql) == 1)
                        {
                            /*这里不处理签名.*/
                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(操作人已经完成)", "WorkNode", "system_error_jump_automatically_3", new string[0]), ndFrom);
                        //增加当前节点的处理人的GenerWorkerList
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = this.WorkID;
                        gwl.FID = this.HisGenerWorkFlow.FID;
                        gwl.NodeID = nd.NodeID;
                        gwl.NodeName = nd.Name;
                        gwl.EmpNo = Executor;
                        gwl.EmpName = ExecutorName;
                        if (dt.Rows[0].Table.Columns.Contains("FK_Dept"))
                        {
                            gwl.DeptNo = dt.Rows[0][1].ToString();
                            Dept dept = new Dept(gwl.DeptNo);
                            gwl.DeptName = dept.Name;
                            if (dept.RetrieveFromDBSources() == 0)
                            {
                                gwl.DeptNo = emp.DeptNo;
                                gwl.DeptName = emp.DeptText;
                            }
                        }
                        else
                        {
                            gwl.DeptNo = emp.DeptNo;
                            gwl.DeptName = emp.DeptText;
                        }
                        gwl.WhoExeIt = nd.WhoExeIt;
                        gwl.SDT = DataType.CurrentDateTime;
                        gwl.DTOfWarning = DataType.CurrentDateTime;
                        gwl.CDT = DataType.CurrentDateTime;
                        gwl.FlowNo = nd.FlowNo;
                        gwl.Sender = WebUser.No + "," + WebUser.Name;
                        gwl.PassInt = 1;
                        gwl.ItIsRead = true;
                        gwl.Insert();
                        ExecEvent.DoNode(EventListNode.SendWhen, nd, skipWork, null);
                        ndFrom = nd;
                        ExecEvent.DoNode(EventListNode.SendSuccess, mynd, skipWork, this.HisMsgObjs, null);
                        CC(nd);//执行抄送
                        continue;
                    }

                    //如果没有跳转,判断是否,其他两个条件是否设置.
                    if (nd.AutoJumpRole2 == false && nd.AutoJumpRole3 == false)
                        return nd;
                }
                #endregion 处理人已经出现过

                #region 处理人与上一步相同 
                if (nd.AutoJumpRole2)
                {
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Emp='" + dr[0] +
                                     "' AND WorkID=" + this.WorkID + " AND FK_Node=" +
                                     (beforeSkipNodeID > 0 ? beforeSkipNodeID : prvNodeID); //edited by liuxc,2015-7-13
                        if (DBAccess.RunSQLReturnValInt(sql) == 1)
                        {
                            /*这里不处理签名.*/
                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        //added by liuxc,2015-7-13,生成跳过的节点数据
                        //记录最开始相同处理人的节点ID，用来上面查找SQL判断
                        if (beforeSkipNodeID == 0)
                            beforeSkipNodeID = prvNodeID;

                        Work wk = nd.HisWork;
                        wk.Copy(mywork);
                        //存储在相同的表中，不需要拷贝
                        if (wk.NodeFrmID != nd.NodeFrmID)
                        {
                            if (wk.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
                                wk.Copy(this.rptGe);
                            wk.Rec = WebUser.No;

                            wk.OID = this.WorkID;
                            wk.ResetDefaultVal();
                        }

                        //执行表单填充，如果有，修改新昌方面同时修改本版本，added by liuxc,2015-10-16
                        MapExt item = nd.MapData.MapExts.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
                        BP.WF.CCFormAPI.DealPageLoadFull(wk, item, nd.MapData.MapAttrs, nd.MapData.MapDtls);

                        wk.DirectSave();

                        //added by liuxc,2015-10-16
                        #region  //此处时，跳转的节点如果有签章，则签章路径会计算错误，需要重新计算一下签章路径，暂时没找到好法子，将UCEn.ascx.cs中的计算签章的逻辑挪过来使用
                        FrmImgs imgs = new FrmImgs();
                        imgs.Retrieve(FrmImgAttr.FrmID, "ND" + nd.NodeID, FrmImgAttr.ImgAppType, 1, FrmImgAttr.IsEdit, 1);

                        foreach (FrmImg img in imgs)
                        {
                            //获取登录人角色
                            string stationNo = "";
                            //签章对应部门
                            string fk_dept = WebUser.DeptNo;
                            //部门来源类别
                            string sealType = "0";
                            //签章对应角色
                            string fk_station = img.Tag0;
                            //表单字段
                            string sealField = "";
                            string imgSrc = "";
                            string sql = "";

                            //如果设置了部门与角色的集合进行拆分
                            if (!DataType.IsNullOrEmpty(img.Tag0) && img.Tag0.Contains("^") && img.Tag0.Split('^').Length == 4)
                            {
                                fk_dept = img.Tag0.Split('^')[0];
                                fk_station = img.Tag0.Split('^')[1];
                                sealType = img.Tag0.Split('^')[2];
                                sealField = img.Tag0.Split('^')[3];
                                //如果部门没有设定，就获取部门来源
                                if (fk_dept == "all")
                                {
                                    //发起人
                                    if (sealType == "1")
                                    {
                                        sql = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + wk.OID;
                                        fk_dept = DBAccess.RunSQLReturnString(sql);
                                    }
                                    //表单字段
                                    if (sealType == "2" && !DataType.IsNullOrEmpty(sealField))
                                    {
                                        //判断字段是否存在
                                        foreach (MapAttr attr in nd.MapData.MapAttrs)
                                        {
                                            if (attr.KeyOfEn == sealField)
                                            {
                                                fk_dept = wk.GetValStrByKey(sealField);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            sql = string.Format(" select FK_Station from Port_DeptStation where FK_Dept ='{0}' and FK_Station in (select FK_Station from  Port_DeptEmpStation where FK_Emp='{1}')", fk_dept, WebUser.No);
                            dt = DBAccess.RunSQLReturnTable(sql);
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (fk_station.Contains(dr[0] + ","))
                                {
                                    stationNo = dr[0].ToString();
                                    break;
                                }
                            }

                            try
                            {
                                imgSrc = BP.WF.Glo.CCFlowAppPath + "DataUser/Seal/" + fk_dept + "_" + stationNo + ".jpg";
                                //设置主键
                                string myPK = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                myPK = myPK + "_" + wk.OID + "_" + img.MyPK;

                                FrmEleDB imgDb = new FrmEleDB(myPK);
                                //判断是否存在
                                if (imgDb != null && !DataType.IsNullOrEmpty(imgDb.FrmID))
                                {
                                    imgDb.FrmID = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                    imgDb.EleID = wk.OID.ToString();
                                    imgDb.RefPKVal = img.MyPK;
                                    imgDb.Tag1 = imgSrc;
                                    imgDb.Update();
                                }
                                else
                                {
                                    imgDb.FrmID = DataType.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                                    imgDb.EleID = wk.OID.ToString();
                                    imgDb.RefPKVal = img.MyPK;
                                    imgDb.Tag1 = imgSrc;
                                    imgDb.Insert();
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        #endregion

                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(操作人与上一步相同)", "WorkNode", "system_error_jump_automatically_2", new string[0]), ndFrom);
                        //增加当前节点的处理人的GenerWorkerList
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = this.WorkID;
                        gwl.FID = this.HisGenerWorkFlow.FID;
                        gwl.NodeID = nd.NodeID;
                        gwl.NodeName = nd.Name;
                        gwl.EmpNo = Executor;
                        gwl.EmpName = ExecutorName;

                        if (dt.Rows[0].Table.Columns.Contains("FK_Dept"))
                        {
                            gwl.DeptNo = dt.Rows[0][1].ToString();
                            Dept dept = new Dept(gwl.DeptNo);
                            gwl.DeptName = dept.Name;
                            if (dept.RetrieveFromDBSources() == 0)
                            {
                                gwl.DeptNo = emp.DeptNo;
                                gwl.DeptName = emp.DeptText;
                            }
                        }
                        else
                        {
                            gwl.DeptNo = emp.DeptNo;
                            gwl.DeptName = emp.DeptText;
                        }
                        gwl.WhoExeIt = nd.WhoExeIt;
                        gwl.SDT = DataType.CurrentDateTime;
                        gwl.DTOfWarning = DataType.CurrentDateTime;
                        gwl.CDT = DataType.CurrentDateTime;
                        gwl.FlowNo = nd.FlowNo;
                        gwl.Sender = WebUser.No + "," + WebUser.Name;
                        gwl.PassInt = 1;
                        gwl.ItIsRead = true;
                        gwl.Insert();
                        ExecEvent.DoNode(EventListNode.SendWhen, nd, wk, null);
                        ndFrom = nd;
                        ExecEvent.DoNode(EventListNode.SendSuccess, mynd, wk);
                        CC(nd);//执行抄送
                        continue;
                    }
                    //没有跳出转的条件，就返回本身.
                    if (nd.AutoJumpRole3 == false)
                        return nd;
                }
                #endregion 处理人与上一步相同 

                #region 未来节点处理人已经出现过
                if (nd.AutoJumpRole3 == true)
                {
                    bool isHave = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 如果出现了处理人就是提交人的情况.
                        string sql = "SELECT COUNT(*) FROM WF_SelectAccper WHERE FK_Emp='" + dr[0].ToString() + "' AND WorkID=" + this.WorkID + " AND FK_Node NOT IN(SELECT DISTINCT(FK_Node) FROM WF_GenerWorkerlist WHERE  WorkID=" + this.WorkID + ") AND FK_Node !=" + nd.NodeID;
                        if (DBAccess.RunSQLReturnValInt(sql) == 1)
                        {
                            /*这里不处理签名.*/
                            isHave = true;
                            Executor = dr[0].ToString();
                            emp = new Emp(Executor);
                            ExecutorName = emp.Name;
                            break;
                        }
                    }

                    if (isHave == true)
                    {
                        this.AddToTrack(ActionType.Skip, Executor, ExecutorName, nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(操作人已经完成)", "WorkNode", "system_error_jump_automatically_3", new string[0]), ndFrom);
                        //增加当前节点的处理人的GenerWorkerList
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = this.WorkID;
                        gwl.FID = this.HisGenerWorkFlow.FID;
                        gwl.NodeID = nd.NodeID;
                        gwl.NodeName = nd.Name;
                        gwl.EmpNo = Executor;
                        gwl.EmpName = ExecutorName;
                        if (dt.Rows[0].Table.Columns.Contains("FK_Dept"))
                        {
                            gwl.DeptNo = dt.Rows[0][1].ToString();
                            Dept dept = new Dept(gwl.DeptNo);
                            gwl.DeptName = dept.Name;
                            if (dept.RetrieveFromDBSources() == 0)
                            {
                                gwl.DeptNo = emp.DeptNo;
                                gwl.DeptName = emp.DeptText;
                            }
                        }
                        else
                        {
                            gwl.DeptNo = emp.DeptNo;
                            gwl.DeptName = emp.DeptText;
                        }
                        gwl.WhoExeIt = nd.WhoExeIt;
                        gwl.SDT = DataType.CurrentDateTime;
                        gwl.DTOfWarning = DataType.CurrentDateTime;
                        gwl.CDT = DataType.CurrentDateTime;
                        gwl.FlowNo = nd.FlowNo;
                        gwl.Sender = WebUser.No + "," + WebUser.Name;
                        gwl.PassInt = 1;
                        gwl.ItIsRead = true;
                        gwl.Insert();
                        ExecEvent.DoNode(EventListNode.SendWhen, nd, skipWork, null);
                        ndFrom = nd;
                        ExecEvent.DoNode(EventListNode.SendSuccess, mynd, skipWork, this.HisMsgObjs, null);
                        CC(nd);//执行抄送
                        continue;
                    }
                    return nd;
                }
                #endregion 处理人已经出现过

                #region 按照 表达式 处理. yln
                if (DataType.IsNullOrEmpty(nd.AutoJumpExp) == false)
                {
                    string exp = nd.AutoJumpExp.Clone() as string;
                    try
                    {
                        exp = BP.WF.Glo.DealExp(exp, this.rptGe, null);

                        if (exp.ToLower().Contains("select") == true)
                        {
                            float val = DBAccess.RunSQLReturnValFloat(exp, 0);
                            if (val >= 0)
                                return nd;
                        }
                        else
                        {
                            if (exp.Contains("?") == true)
                                exp += "&";
                            else
                                exp += "?";

                            exp += "WorkID=" + rptGe.OID + "&Token=" + WebUser.Token + "&UserNo=" + WebUser.No;

                            string str = DataType.ReadURLContext(exp, 10000);
                            float val = float.Parse(str);
                            if (val <= 0)
                                return nd;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("err@判断exp跳转错误，表达式:" + exp + " 信息:" + ex.Message);
                    }
                }
                #endregion 按照 表达式 处理.


                if (nd.HisWhenNoWorker == true)
                    return nd;

                mynd = nd;
                ndFrom = nd;
            }//结束循环。

            #endregion 计算到达的节点.


            throw new Exception(BP.WF.Glo.multilingual("@找到下一步节点.", "WorkNode", "found_next_node", new string[0]));
        }

        private void CC(Node node)
        {
            WorkCC cc = new WorkCC(this, this.WebUser);
            cc.DoCC("WorkNode"); //执行抄送动作.
            if (node.HisCCRole == CCRoleEnum.HandCC)
            {
                WorkOpt workOpt = new WorkOpt();
                workOpt.setMyPK(this.WebUser.No + "_" + node.NodeID + "_" + this.WorkID);
                if (workOpt.RetrieveFromDBSources() == 1)
                {
                    String emps = workOpt.CCEmps;
                    emps += "," + BP.Port.Glo.GenerEmpNosByDeptNos(workOpt.CCDepts);
                    emps += "," + BP.Port.Glo.GenerEmpNosByStationNos(workOpt.CCStations);
                    String ccMsg = Dev2Interface.Node_CCToEmps(workOpt.NodeID, workOpt.WorkID, emps, "来自" + WebUser.Name + "的抄送", workOpt.CCNote);
                    if (DataType.IsNullOrEmpty(ccMsg) == false)
                        this.addMsg("HandlerCC", ccMsg);
                }
            }
            //string ccRole = node.GetParaString("CCRoleNum");
            //if (DataType.IsNullOrEmpty(ccRole) == true)
            //{
            //    //for vue3版本.
            //    CCRoles ens = new CCRoles();
            //    ens.Retrieve(CCRoleAttr.NodeID, node.NodeID);
            //    node.SetPara("CCRoleNum", ens.Count);
            //    node.Update();

            //    // string msg = WorkFlowBuessRole.DoCCAuto(node, this.rptGe, this.WorkID, this.HisWork.FID);
            //   // this.addMsg("CC", BP.WF.Glo.multilingual("@自动抄送给:{0}.", "WorkNode", "cc", msg));
            //    return;
            //}
            //if (DataType.IsNullOrEmpty(ccRole) == false && ccRole.Equals("0") == false)
            //{
            //    CCRoles ens = new CCRoles();
            //    ens.Retrieve(CCRoleAttr.NodeID, node.NodeID);

            //    // string msg = WorkFlowBuessRole.DoCCAuto(node, this.rptGe, this.WorkID, this.HisWork.FID);
            //   // this.addMsg("CC", BP.WF.Glo.multilingual("@自动抄送给:{0}.", "WorkNode", "cc", msg));
            //    return;
            //}
        }
        /// <summary>
        /// 处理OrderTeamup退回模式
        /// </summary>
        public void DealReturnOrderTeamup()
        {
            /*如果协作，顺序方式.*/
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.EmpNo = WebUser.No;
            gwl.NodeID = this.HisNode.NodeID;
            gwl.WorkID = this.WorkID;
            if (gwl.RetrieveFromDBSources() == 0)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到自己期望的数据.", "WorkNode", "not_found_my_expected_data", new string[0]));

            gwl.ItIsPass = true;
            gwl.Update();

            gwl.EmpNo = this.JumpToEmp;
            gwl.NodeID = this.JumpToNode.NodeID;
            gwl.WorkID = this.WorkID;
            if (gwl.RetrieveFromDBSources() == 0)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到接收人期望的数据，在协作模式的按照顺序退回的时候.", "WorkNode", "not_found_receiver_expected_data", new string[0]));

            #region 要计算当前人员的应完成日期
            // 计算出来 退回到节点的应完成时间. 
            DateTime dtOfShould;

            //增加天数. 考虑到了节假日.             
            dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.HisNode.TimeLimit,
                this.HisNode.TimeLimitHH, this.HisNode.TimeLimitMM, this.HisNode.TWay);

            // 应完成日期.
            string sdt = DataType.SysDateTimeFormat(dtOfShould);
            #endregion

            //更新日期，为了考核. 
            if (this.HisNode.HisCHWay == CHWay.None)
                gwl.SDT = "无";
            else
                gwl.SDT = sdt;

            gwl.ItIsPass = false;
            gwl.Update();

            GenerWorkerLists ens = new GenerWorkerLists();
            ens.AddEntity(gwl);
            this.HisWorkerLists = ens;

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, gwl.EmpNo, gwl.EmpNo, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, gwl.EmpName, gwl.EmpName, SendReturnMsgType.SystemMsg);
            string[] para = new string[2];
            para[0] = gwl.EmpNo;
            para[1] = gwl.EmpName;
            string str = BP.WF.Glo.multilingual("@当前工作已经发送给退回人({0},{1}).", "WorkNode", "current_work_send_to_returner", para);

            this.addMsg(SendReturnMsgFlag.OverCurr, str, null, SendReturnMsgType.Info);

            this.HisGenerWorkFlow.WFState = WFState.Runing;
            this.HisGenerWorkFlow.NodeID = gwl.NodeID;
            this.HisGenerWorkFlow.NodeName = gwl.NodeName;

            this.HisGenerWorkFlow.TodoEmps = gwl.EmpNo + "," + gwl.EmpName + ";";
            this.HisGenerWorkFlow.TodoEmpsNum = 0;
            this.HisGenerWorkFlow.TaskSta = TaskSta.None;
            this.HisGenerWorkFlow.Update();
        }
        /// <summary>
        /// 获取下一步骤的工作节点
        /// </summary>
        /// <returns></returns>
        private Node NodeSend_GenerNextStepNode_Ext(Node currNode)
        {
            Nodes nds = currNode.HisToNodes;
            if (nds.Count == 1)
            {
                Node toND = (Node)nds[0];
                if (toND.HisNodeType == NodeType.UserNode)
                    return toND;
                AddToTrack(ActionType.Route, "", "", toND.NodeID, toND.Name, "节点" + currNode.Name + "经过路由" + toND.Name);
                return NodeSend_GenerNextStepNode_Ext(toND);
            }
            if (nds.Count == 0)
                throw new Exception(BP.WF.Glo.multilingual("@没找到下一步节点.", "WorkNode", "not_found_next_node", new string[0]));
            //获得所有的方向,按照优先级, 按照条件处理方向，如果成立就返回.
            Directions dirs = new Directions(currNode.NodeID);
            //@yln
            Node nd = null;
            if (dirs.Count == 1)
            {
                nd = new Node(dirs[0].GetValIntByKey(DirectionAttr.ToNode));
                if (nd.HisNodeType == NodeType.UserNode)
                    return nd;
                AddToTrack(ActionType.Route, "", "", nd.NodeID, nd.Name, "节点[" + currNode.Name + "]经过路由" + nd.Name);
                return NodeSend_GenerNextStepNode_Ext(nd);
            }

            //定义没有条件的节点集合.
            Directions dirs0Cond = new Directions();
            if (this.SendHTOfTemp != null)
            {
                foreach (string key in this.SendHTOfTemp.Keys)
                {
                    if (rptGe.Row.ContainsKey(key) == true)
                        this.rptGe.Row[key] = this.SendHTOfTemp[key] as string;
                    else
                        this.rptGe.Row.Add(key, this.SendHTOfTemp[key] as string);
                }
            }
            foreach (Direction dir in dirs)
            {
                //查询出来他的条件.
                Conds conds = new Conds();
                conds.Retrieve(CondAttr.FK_Node, currNode.NodeID,
                    CondAttr.ToNodeID, dir.ToNode, CondAttr.CondType,
                    (int)CondType.Dir,
                    CondAttr.Idx);

                //可以到达的节点.
                if (conds.Count == 0)
                {
                    dirs0Cond.AddEntity(dir); //把他加入到里面.
                    continue;
                }

                //按条件计算.
                if (conds.GenerResult(this.rptGe, this.WebUser, this.HisNode) == true)
                {
                    nd = new Node(dir.ToNode);
                    if (nd.HisNodeType == NodeType.UserNode)
                        return nd;
                    AddToTrack(ActionType.Route, "", "", nd.NodeID, nd.Name, "节点" + currNode.Name + "经过路由" + nd.Name);
                    return NodeSend_GenerNextStepNode_Ext(nd);
                }
            }

            if (dirs0Cond.Count == 0)
                throw new Exception("err@流程设计错误:[" + currNode.NodeID + "," + currNode.Name + "]到达的节点，所有的条件都不成立.");

            if (dirs0Cond.Count != 1)
                throw new Exception("err@流程设计错误:当前节点[" + currNode.Name + "]的到达的节点，超过1个节点，没有设置方向条件.");

            int toNodeID = dirs0Cond[0].GetValIntByKey(DirectionAttr.ToNode);
            //@yln
            nd = new Node(toNodeID);
            if (nd.HisNodeType == NodeType.UserNode)
                return nd;
            AddToTrack(ActionType.Route, "", "", nd.NodeID, nd.Name, "节点" + currNode.Name + "经过路由" + nd.Name);
            return NodeSend_GenerNextStepNode_Ext(nd);
        }
        /// <summary>
        /// 获取下一步骤的节点集合
        /// </summary>
        /// <returns></returns>
        public Nodes Func_GenerNextStepNodes()
        {
            //如果跳转节点已经有了变量.
            if (this.JumpToNode != null)
            {
                Nodes myNodesTo = new Nodes();
                myNodesTo.AddEntity(this.JumpToNode);
                return myNodesTo;
            }

            if (this.HisNode.HisToNodes.Count == 1)
                return this.HisNode.HisToNodes;


            #region 如果使用户选择的.
            if (this.HisNode.CondModel == DirCondModel.ByDDLSelected || this.HisNode.CondModel == DirCondModel.ByPopSelect
            || this.HisNode.CondModel == DirCondModel.ByButtonSelected)
            {
                // 获取用户选择的节点.
                string nodes = this.HisGenerWorkFlow.Paras_ToNodes;
                if (DataType.IsNullOrEmpty(nodes))
                    throw new Exception(BP.WF.Glo.multilingual("@用户没有选择发送到的节点", "WorkNode", "no_choice_of_target_node", new string[0]));

                Nodes nds = new Nodes();
                string[] mynodes = nodes.Split(',');
                foreach (string item in mynodes)
                {
                    if (DataType.IsNullOrEmpty(item))
                        continue;
                    nds.AddEntity(new Node(int.Parse(item)));
                }
                return nds;
            }
            #endregion 如果使用户选择的.


            Nodes toNodes = this.HisNode.HisToNodes;
            // 如果只有一个转向节点, 就不用判断条件了,直接转向他.
            if (toNodes.Count == 1)
                return toNodes;

            //dcsAll.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.Idx);

            #region 获取能够通过的节点集合，如果没有设置方向条件就默认通过.
            Nodes myNodes = new Nodes();
            int toNodeId = 0;
            int numOfWay = 0;

            foreach (Node nd in toNodes)
            {
                Conds dcs = new Conds();
                dcs.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID,
                    CondAttr.ToNodeID, nd.NodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

                if (dcs.Count == 0)
                {
                    myNodes.AddEntity(nd);
                    continue;
                }

                if (dcs.GenerResult(this.rptGe, this.WebUser) == true)
                {
                    myNodes.AddEntity(nd);
                    continue;
                }

            }
            #endregion 获取能够通过的节点集合，如果没有设置方向条件就默认通过.

            #region 走到最后，发现一个条件都不符合，就找没有设置方向条件的节点. （@杜翻译）
            if (myNodes.Count == 0)
            {
                /*如果没有找到其他节点，就找没有设置方向条件的节点.*/
                foreach (Node nd in toNodes)
                {
                    Conds conds = new Conds();
                    int i = conds.Retrieve(CondAttr.FK_Node, nd.NodeID, CondAttr.CondType, 2);
                    if (i == 0)
                        continue;

                    //增加到节点集合.
                    myNodes.AddEntity(nd);
                }

                //如果没有设置方向条件的节点有多个，就清除在后面抛出异常.
                if (myNodes.Count != 1)
                    myNodes.Clear();
            }
            #endregion 走到最后，发现一个条件都不符合，就找没有设置方向条件的节点.


            if (myNodes.Count == 0)
            {
                string[] para = new string[3];
                para[0] = this.ExecerName;
                para[1] = this.HisNode.NodeID.ToString();
                para[2] = this.HisNode.Name;
                throw new Exception(BP.WF.Glo.multilingual("@当前用户({0})定义节点的方向条件错误:从节点({1}-{2})到其它所有节点的转向条件都不成立.", "WorkNode", "error_node_jump_condition", para));
            }

            return myNodes;
        }
        /// <summary>
        /// 检查一下流程完成条件.
        /// </summary>
        /// <returns></returns>
        private void Func_CheckCompleteCondition()
        {
            if (this.HisNode.ItIsSubThread == true)
                throw new Exception(BP.WF.Glo.multilingual("@流程设计错误：不允许在子线程上设置流程完成条件。", "WorkNode", "error_sub_thread", new string[0]));

            this.IsStopFlow = false;
            string[] para = new string[1];
            para[0] = this.HisNode.Name;
            this.addMsg("CurrWorkOver", BP.WF.Glo.multilingual("@当前节点工作[{0}]已经完成。", "WorkNode", "current_node_completed", para));

            #region 判断流程条件.
            try
            {
                string matched_str = BP.WF.Glo.multilingual("符合流程完成条件", "WorkNode", "match_workflow_completed", new string[0]);
                if (this.HisNode.HisToNodes.Count == 0 && this.HisNode.ItIsStartNode)
                {
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, matched_str, this.HisNode, this.rptGe);

                    if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                        this.IsStopFlow = true;

                    this.addMsg("OneNodeFlowOver", BP.WF.Glo.multilingual("@工作已经成功处理(一个节点的流程)。", "WorkNode", "node_completed_success", new string[0]));
                }

                if (this.HisFlow.CondsOfFlowComplete.Count >= 1
                    && this.HisFlow.CondsOfFlowComplete.GenerResult(this.rptGe, this.WebUser))
                {
                    /*如果有流程完成条件，并且流程完成条件是通过的。*/
                    string stopMsg = this.HisFlow.CondsOfFlowComplete.ConditionDesc;
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, matched_str + ":" + stopMsg, this.HisNode, this.rptGe);
                    this.IsStopFlow = true;

                    // string path = BP.Sys.Base.Glo.Request.ApplicationPath;
                    string mymsg = "@" + matched_str + " " + stopMsg + " " + overMsg;
                    this.addMsg(SendReturnMsgFlag.FlowOver, mymsg, mymsg, SendReturnMsgType.Info);
                }
            }
            catch (Exception ex)
            {
                string str = BP.WF.Glo.multilingual("@判断流程({0})完成条件出现错误:{1}.",
                    "WorkNode",
                    "error_workflow_complete_condition", ex.StackTrace, this.HisNode.Name);
                throw new Exception(str);
            }
            #endregion
        }
        /// <summary>
        /// 设置当前工作已经完成.
        /// </summary>
        /// <returns></returns>
        private string Func_DoSetThisWorkOver()
        {
            //设置结束人.  
            this.rptGe.SetValByKey(GERptAttr.FK_Dept, this.HisGenerWorkFlow.DeptNo); //此值不能变化.
            this.rptGe.SetValByKey(GERptAttr.FlowEnder, this.Execer);
            this.rptGe.SetValByKey(GERptAttr.FlowEnderRDT, DataType.CurrentDateTime);
            if (this.town == null)
                this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.HisNode.NodeID);
            else
            {
                if (this.HisNode.HisRunModel == RunModel.FL || this.HisNode.HisRunModel == RunModel.FHL)
                    this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.HisNode.NodeID);
                else
                    this.rptGe.SetValByKey(GERptAttr.FlowEndNode, this.town.HisNode.NodeID);
            }

            //有可能日期是空的。
            if (rptGe.FlowStartRDT.Length >= 8)
            {
                this.rptGe.SetValByKey(GERptAttr.FlowDaySpan,
                    DataType.GeTimeLimits(rptGe.FlowStartRDT, DataType.CurrentDateTime));
            }

            //如果两个物理表不想等.
            if (this.HisWork.EnMap.PhysicsTable.Equals(this.rptGe.EnMap.PhysicsTable) == false)
            {
                // 更新状态。
                this.HisWork.SetValByKey("CDT", DataType.CurrentDateTime);
                this.HisWork.Rec = this.Execer;

                //判断是不是MD5流程？
                if (this.HisFlow.ItIsMD5)
                    this.HisWork.SetValByKey("MD5", Glo.GenerMD5(this.HisWork));

                if (this.HisNode.ItIsStartNode)
                    this.HisWork.SetValByKey(GERptAttr.Title, this.HisGenerWorkFlow.Title);

                this.HisWork.DirectUpdate();
            }

            #region 2014-08-02 删除了其他人员的待办，增加了 IsPass=0 参数.
            if (this.town != null && this.town.HisNode.NodeID == this.HisNode.NodeID)
            {
                // 清除其他的工作者.
                //ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp <> " + dbStr + "FK_Emp";
                //ps.Clear();
                //ps.Add("FK_Node", this.HisNode.NodeID);
                //ps.Add("WorkID", this.WorkID);
                //ps.Add("FK_Emp", this.Execer);
                //DBAccess.RunSQL(ps);
            }
            else
            {
                // 清除其他的工作者.
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp <> " + dbStr + "FK_Emp";
                ps.Clear();
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Emp", this.Execer);
                DBAccess.RunSQL(ps);
            }
            #endregion 2014-08-02 删除了其他人员的待办，增加了 IsPass=0 参数.

            if (this.town != null && this.town.HisNode.NodeID == this.HisNode.NodeID)
            {
                /*如果是当前节点发给当前节点，就更新上一个节点全部完成。 */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp AND IsPass=0";
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                ps.Add("FK_Emp", this.Execer);
                DBAccess.RunSQL(ps);
            }
            else
            {
                /*如果不是当前节点发给当前节点，就更新上一个节点全部完成。 */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND IsPass=0";
                ps.Add("FK_Node", this.HisNode.NodeID);
                ps.Add("WorkID", this.WorkID);
                DBAccess.RunSQL(ps);
            }

            // 给generworkflow赋值。
            if (this.IsStopFlow == true)
                this.HisGenerWorkFlow.WFState = WFState.Complete;
            else
                this.HisGenerWorkFlow.WFState = WFState.Runing;

            // 流程应完成时间。
            if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfFlow))
                this.HisGenerWorkFlow.SDTOfFlow = this.HisWork.GetValStrByKey(WorkSysFieldAttr.SysSDTOfFlow);

            // 下一个节点应完成时间。
            if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfNode))
                this.HisGenerWorkFlow.SDTOfNode = this.HisWork.GetValStrByKey(WorkSysFieldAttr.SysSDTOfNode);

            //执行更新。
            if (this.IsStopFlow == false)
                this.HisGenerWorkFlow.Update();
            return BP.WF.Glo.multilingual("@流程已经完成.", "WorkNode", "workflow_completed");
        }
        #endregion 附属功能
        /// <summary>
        /// 普通节点到普通节点
        /// </summary>
        /// <param name="toND">要到达的下一个节点</param>
        /// <returns>执行消息</returns>
        private void NodeSend_11(Node toND)
        {
            string sql = "";
            string errMsg = "";
            Work toWK = toND.HisWork;
            toWK.OID = this.WorkID;
            toWK.FID = this.HisWork.FID;

            // 如果执行了跳转.
            if (this.ItIsSkip == true)
                toWK.RetrieveFromDBSources(); //有可能是跳转.

            #region 执行数据初始化
            // town.
            WorkNode town = new WorkNode(toWK, toND);

            errMsg = BP.WF.Glo.multilingual("@初试化他们的工作人员 - 期间出现错误.", "WorkNode", "error_initialize_workflow_operator");

            // 初试化他们的工作人员．
            current_gwls = this.Func_GenerWorkerLists(town);
            if (town.HisNode.HuiQianRole == HuiQianRole.TeamupGroupLeader && town.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader && town.HisNode.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne && current_gwls.Count > 1)
                throw new Exception(BP.WF.Glo.multilingual("@接收人出错! 详情:{0}.", "WorkNode", "error_sendToemps_data", "@节点" + town.HisNode.NodeID + "是组长会签模式，接受人只能选择一人"));

            if (town.HisNode.TodolistModel == TodolistModel.Order && current_gwls.Count > 1)
            {
                /*如果到达的节点是队列流程节点，就要设置他们的队列顺序.*/
                int idx = 0;
                foreach (GenerWorkerList gwl in current_gwls)
                {
                    idx++;
                    if (idx == 1)
                        continue;
                    gwl.PassInt = idx + 100;
                    gwl.Update();
                }
            }

            if ((town.HisNode.TodolistModel == TodolistModel.Teamup || town.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader) && current_gwls.Count > 1)
            {
                /*如果是协作模式 */
                if (town.HisNode.FWCOrderModel == 1)
                {
                    /* 如果是协作模式，并且显示排序按照官职大小排序. */
                    DateTime dt = DateTime.Now;
                    foreach (GenerWorkerList gwl in current_gwls)
                    {
                        dt = dt.AddMinutes(5);
                        string rdt = dt.ToString("yyyy-MM-dd HH:mm:ss");

                        BP.WF.Dev2Interface.WriteTrack(this.HisFlow.No, town.HisNode.NodeID, town.HisNode.Name, this.WorkID, town.HisWork.FID, "",
                            ActionType.WorkCheck, null, null, null, gwl.EmpNo, gwl.EmpName, gwl.EmpNo, gwl.EmpName, rdt);
                    }
                }
            }


            #region 保存目标节点数据.
            if (this.HisWork.EnMap.PhysicsTable != toWK.EnMap.PhysicsTable)
            {
                errMsg = BP.WF.Glo.multilingual("@保存目标节点数据 - 期间出现错误.", "WorkNode", "error_saving_target_node_data");

                //为下一步骤初始化数据.
                GenerWorkerList gwl = current_gwls[0] as GenerWorkerList;
                toWK.Rec = gwl.EmpNo;
                string emps = gwl.EmpNo;
                if (current_gwls.Count != 1)
                {
                    foreach (GenerWorkerList item in current_gwls)
                        emps += item.EmpNo + ",";
                }
                //toWK.Emps = emps;

                try
                {

                    int count = toWK.RetrieveFromDBSources();
                    if (count > 0)
                        toWK.DirectUpdate(); // 如果执行了跳转.
                    else
                        toWK.DirectInsert();

                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError(BP.WF.Glo.multilingual("@出现SQL异常! 可能是没有修复表或者重复发送.详情:{0}.", "WorkNode", "sql_exception_1", ex.Message));
                    try
                    {
                        toWK.CheckPhysicsTable();
                        toWK.DirectUpdate();
                    }
                    catch (Exception ex1)
                    {
                        BP.DA.Log.DebugWriteError(BP.WF.Glo.multilingual("@保存工作出错! 详情:{0}.", "WorkNode", "error_saving_data", ex1.Message));
                        throw new Exception(BP.WF.Glo.multilingual("@保存工作出错! 详情:{0}.", "WorkNode", "error_saving_data", toWK.EnDesc + ex1.Message));
                    }
                }
            }

            #endregion 保存目标节点数据.

            //@加入消息集合里。
            this.SendMsgToThem(current_gwls);

            if (toND.ItIsGuestNode == true)
            {
                string htmlInfo = BP.WF.Glo.multilingual("@发送给如下{0}位处理人{1}.", "WorkNode", "send_to_the_following_operators", this.HisRememberMe.NumOfObjs.ToString(), this.HisGenerWorkFlow.GuestNo + " " + this.HisGenerWorkFlow.GuestName);

                string textInfo = BP.WF.Glo.multilingual("@发送给如下{0}位处理人{1}.", "WorkNode", "send_to_the_following_operators", this.HisRememberMe.NumOfObjs.ToString(), this.HisGenerWorkFlow.GuestName);

                this.addMsg(SendReturnMsgFlag.ToEmps, textInfo, htmlInfo);
            }
            else
            {
                string htmlInfo = BP.WF.Glo.multilingual("@发送给如下{0}位处理人{1}.", "WorkNode", "send_to_the_following_operators", this.HisRememberMe.NumOfObjs.ToString(), this.HisRememberMe.EmpsExt);

                string textInfo = BP.WF.Glo.multilingual("@发送给如下{0}位处理人{1}.", "WorkNode", "send_to_the_following_operators", this.HisRememberMe.NumOfObjs.ToString(), this.HisRememberMe.ObjsExt);

                this.addMsg(SendReturnMsgFlag.ToEmps, textInfo, htmlInfo);

            }

            #region 处理审核问题,更新审核组件插入的审核意见中的 到节点，到人员。
            Paras ps = new Paras();
            try
            {
                ps.SQL = "UPDATE ND" + int.Parse(toND.FlowNo) + "Track SET NDTo=" + dbStr + "NDTo,NDToT=" + dbStr + "NDToT,EmpTo=" + dbStr + "EmpTo,EmpToT=" + dbStr + "EmpToT WHERE NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND WorkID=" + dbStr + "WorkID AND ActionType=" + (int)ActionType.WorkCheck;

                ps.Add(TrackAttr.NDTo, this.HisNode.NodeID);
                ps.Add(TrackAttr.NDToT, this.HisNode.Name);

                ps.Add(TrackAttr.EmpTo, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.EmpToT, this.HisRememberMe.EmpsExt);
                ps.Add(TrackAttr.NDFrom, this.HisNode.NodeID);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);
                ps.Add(TrackAttr.WorkID, this.WorkID);
                DBAccess.RunSQL(ps);
            }
            catch (Exception ex)
            {

                try
                {
                    #region  如果更新失败，可能是由于数据字段大小引起。
                    Flow flow = new Flow(toND.FlowNo);

                    string updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FlowNo) + "Track", "EmpFromT");
                    DBAccess.RunSQL(updateLengthSql);

                    updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FlowNo) + "Track", "EmpFrom");
                    DBAccess.RunSQL(updateLengthSql);

                    updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FlowNo) + "Track", "EmpTo");
                    DBAccess.RunSQL(updateLengthSql);
                    updateLengthSql = string.Format("  alter table {0} alter column {1} varchar(2000) ", "ND" + int.Parse(toND.FlowNo) + "Track", "EmpToT");
                    DBAccess.RunSQL(updateLengthSql);

                    DBAccess.RunSQL(ps);
                    #endregion
                }
                catch (Exception myex)
                {
                    throw new Exception("err@处理track表出现错误:" + myex.Message);
                }
            }
            #endregion 处理审核问题.

            //string htmlInfo = string.Format("@任务自动发送给{0}如下处理人{1}.", this.nextStationName,this._RememberMe.EmpsExt);
            //string textInfo = string.Format("@任务自动发送给{0}如下处理人{1}.", this.nextStationName,this._RememberMe.ObjsExt);


            if (this.HisWorkerLists.Count >= 2 && this.HisNode.ItIsTask)
            {
                //Img 的路径问题.
                this.addMsg(SendReturnMsgFlag.AllotTask, null, "<a href='./WorkOpt/AllotTask.htm?WorkID=" + this.WorkID + "&FK_Node=" + toND.NodeID + "&FK_Flow=" + toND.FlowNo + "'  target=_self><img src='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/Img/AllotTask.gif' border=0/>指定特定的处理人处理</a>。", SendReturnMsgType.Info);
            }


            if (this.HisNode.HisFormType != NodeFormType.SDKForm && this.HisNode.HisCancelRole != CancelRole.None)
            {
                if (this.HisNode.ItIsStartNode)
                    this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&Token=" + WebUser.Token + "&WorkID=" + this.HisWork.OID + "&FK_Flow=" + toND.FlowNo + "' ><img src='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/Img/Action/UnSend.png' border=0/>撤销本次发送</a>", SendReturnMsgType.Info);
                else
                    this.addMsg(SendReturnMsgFlag.ToEmpExt, null, "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&Token=" + WebUser.Token + "&WorkID=" + this.HisWork.OID + "&FK_Flow=" + toND.FlowNo + "' ><img src='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/Img/Action/UnSend.png' border=0 />撤销本次发送</a> ", SendReturnMsgType.Info);
            }

            this.HisGenerWorkFlow.NodeID = toND.NodeID;
            this.HisGenerWorkFlow.NodeName = toND.Name;
            this.HisGenerWorkFlow.SetPara("ThreadCount", 0);
            string str1 = BP.WF.Glo.multilingual("@下一步工作[{0}]成功启动.", "WorkNode", "start_next_node_work_success", toND.Name);
            string str2 = BP.WF.Glo.multilingual("@下一步工作<font color=blue>[{0}]</font>成功启动.", "WorkNode", "start_next_node_work_success", toND.Name);

            this.addMsg(SendReturnMsgFlag.WorkStartNode, str1, str2);

            //this.addMsg(SendReturnMsgFlag.WorkStartNode, Glo.Multilingual("WorkNode","WorkStartNode",toND.Name), "WorkStartNode1");

            #endregion

            #region  初始化发起的工作节点。
            if (this.HisWork.EnMap.PhysicsTable.Equals(toWK.EnMap.PhysicsTable) == false)
                /* 如果两个数据源不想等，就执行copy。 */
                this.CopyData(toWK, toND, false);

            #endregion 初始化发起的工作节点.

            #region 判断是否是质量评价。
            if (toND.ItIsEval)
            {
                /*如果是质量评价流程*/
                toWK.SetValByKey(WorkSysFieldAttr.EvalEmpNo, this.Execer);
                toWK.SetValByKey(WorkSysFieldAttr.EvalEmpName, this.ExecerName);
                toWK.SetValByKey(WorkSysFieldAttr.EvalCent, 0);
                toWK.SetValByKey(WorkSysFieldAttr.EvalNote, "");
            }
            #endregion

        }

        /// <summary>
        /// 处理分流点向下发送 to 异表单.
        /// </summary>
        /// <returns></returns>
        private void NodeSend_24_UnSameSheet(Nodes toNDs)
        {
            //NodeSend_2X_GenerFH();

            /*分别启动每个节点的信息.*/
            string msg = "";

            //定义系统变量.
            string workIDs = "";
            string empIDs = "";
            string empNames = "";
            string toNodeIDs = "";

            /*
             * for:计算中心.
             * 1. 首先要查询出来到达的节点是否有历史数据?
             * 2. 产生历史数据的有 子线程的退回，撤销发送.
             */

            GenerWorkFlows gwfThreads = new GenerWorkFlows();
            gwfThreads.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);

            string msg_str = "";
            foreach (Node nd in toNDs)
            {
                //删除垃圾数据.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FK_Node=" + nd.NodeID + " AND FID=" + this.WorkID);


                Int64 workIDSubThread = 0;

                FrmNodes fns = new FrmNodes();
                fns.Retrieve(FrmNodeAttr.FK_Node, nd.NodeID);

                #region 如果生成workid的规则模式为0,  异表单子线程WorkID生成规则  UnSameSheetWorkIDModel  0= 仅生成一个WorkID,  1=按接受人生成WorkID,
                if (nd.USSWorkIDRole == 0)
                {
                    bool isNew = true;
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    foreach (GenerWorkFlow gwfT in gwfThreads)
                    {
                        if (gwfT.NodeID == nd.NodeID)
                        {
                            gwf = gwfT; //设置.
                            workIDSubThread = gwfT.WorkID;
                            isNew = false;
                            break;
                        }
                    }
                    if (workIDSubThread == 0)
                    {
                        if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                            workIDSubThread = DBAccess.GenerOID("WorkID");
                        else
                            workIDSubThread = DBAccess.GenerOIDByGUID();
                    }


                    //产生一个工作信息。
                    Work wk = nd.HisWork;
                    wk.Copy(this.rptGe);

                    wk.FID = this.HisWork.OID;
                    wk.OID = workIDSubThread;
                    if (isNew == true)
                        wk.DirectInsert(); //执行插入.
                    else
                        wk.DirectUpdate(); //执行保存.

                    #region 检查子线程是否有绑定的表单? 如果有就给它主表的数据.
                    foreach (FrmNode fn in fns)
                    {
                        //不是始终启用，就不给数据.
                        if (fn.FrmEnableRole != FrmEnableRole.Allways)
                            continue;
                        if (DataType.IsNullOrEmpty(fn.FK_Frm)==true)
                        {
                            fn.Delete();
                            continue;
                        }

                        GEEntity ge = new GEEntity(fn.FK_Frm);
                        if (fn.WhoIsPK != WhoIsPK.FID)
                        {
                            ge.OID = this.HisWork.OID;
                            ge.Copy(this.HisWork);
                            if (ge.IsExits == false)
                                ge.InsertAsOID(ge.OID);
                            else
                                ge.Update();
                        }
                    }
                    #endregion 检查子线程是否有绑定的表单? 如果有就给它主表的数据.

                    //获得它的工作者。
                    WorkNode town = new WorkNode(wk, nd);
                    current_gwls = this.Func_GenerWorkerLists_Thread(town);
                    if (current_gwls == null)
                    {
                        //@yln 判断该节点是否存在跳转
                        if (nd.HisWhenNoWorker == true)
                        {

                            //记录跳转信息
                            this.AddToTrack(ActionType.Skip, WebUser.No, WebUser.Name,
                                    nd.NodeID, nd.Name, BP.WF.Glo.multilingual("自动跳转(启用跳转规则,当没有找到处理人时)", "WorkNode", "system_error_jump_automatically_1", new String[0]), nd);
                            //没有找到人就跳转
                            Node nextNode = this.NodeSend_GenerNextStepNode_Ext(nd);
                            //判断当前节点是不是合流点
                            if (nextNode.HisRunModel == RunModel.HL || nextNode.HisRunModel == RunModel.FHL)
                            {
                                //处理业务逻辑
                                NodeSend_ToGenerWorkFlow(gwf, wk, nd, isNew, WebUser.DeptNo, WebUser.DeptName, WebUser.No + "," + WebUser.Name);
                                gwf.SetPara("FLNodeID", this.HisNode.NodeID);
                                gwf.SetPara("ThreadCount", toNDs.Count);
                                gwf.Update();
                                //发送到分流点
                                SendReturnObjs returnObj = BP.WF.Dev2Interface.Node_SendWork(gwf.FlowNo, gwf.WorkID);
                                this.addMsg("Send", returnObj.ToMsgOfHtml());
                                GenerWorkFlow maingwf = new GenerWorkFlow(this.WorkID);
                                this.HisGenerWorkFlow = maingwf;
                                continue;
                            }
                            town = new WorkNode(wk, nextNode);
                            current_gwls = this.Func_GenerWorkerLists(town);
                        }
                        else
                        {
                            msg += BP.WF.Glo.multilingual("@没有找到节点[{0}]的处理人员,所以此节点无法成功启动.", "WorkNode", "not_found_node_operator", nd.Name);
                            continue;
                        }

                    }

                    #region 生成待办.
                    string operators = "";
                    int i = 0;
                    GenerWorkerList oneGWL = null; //获得这个变量，在gwf中使用.
                    string todoEmps = "";
                    foreach (GenerWorkerList wl in current_gwls)
                    {
                        oneGWL = wl; //获得这个变量，在gwf中使用.

                        i += 1;
                        operators += wl.EmpNo + ", " + wl.EmpName + ";";
                        empIDs += wl.EmpNo + ",";
                        empNames += wl.EmpName + ",";

                        ps = new Paras();
                        ps.SQL = "UPDATE WF_GenerWorkerlist SET WorkID=" + dbStr + "WorkID1,FID=" + dbStr + "FID WHERE FK_Emp=" + dbStr + "FK_Emp AND WorkID=" + dbStr + "WorkID2 AND FK_Node=" + dbStr + "FK_Node ";
                        ps.Add("WorkID1", wk.OID);
                        ps.Add("FID", this.WorkID);

                        ps.Add("FK_Emp", wl.EmpNo);
                        ps.Add("WorkID2", wl.WorkID);
                        ps.Add("FK_Node", wl.NodeID);
                        DBAccess.RunSQL(ps);

                        //设置当前的workid.
                        wl.WorkID = wk.OID;


                        //更新工作信息.
                        wk.Rec = wl.EmpNo;
                        //wk.Emps = "@" + wl.EmpNo;
                        //wk.RDT = DataType.CurrentDateTimess;
                        wk.DirectUpdate();

                        //为子线程产生分流节点的发送副本。
                        wl.FID = this.WorkID;
                        wl.EmpNo = WebUser.No;
                        wl.EmpName = WebUser.Name;
                        wl.PassInt = -2;
                        wl.ItIsRead = true;
                        wl.NodeID = this.HisNode.NodeID;
                        wl.NodeName = this.HisNode.Name;
                        wl.DeptNo = WebUser.DeptNo;
                        wl.DeptName = WebUser.DeptName;
                        if (wl.IsExits == false)
                            wl.Insert();
                    }
                    #endregion 生成待办.

                    #region 生成子线程的GWF.
                    gwf.WorkID = wk.OID;
                    //干流、子线程关联字段
                    gwf.FID = this.WorkID;

                    //父流程关联字段
                    gwf.PWorkID = this.HisGenerWorkFlow.PWorkID;
                    gwf.PFlowNo = this.HisGenerWorkFlow.PFlowNo;
                    gwf.PNodeID = this.HisGenerWorkFlow.PNodeID;

                    //工程类项目关联字段
                    gwf.PrjNo = this.HisGenerWorkFlow.PrjNo;
                    gwf.PrjName = this.HisGenerWorkFlow.PrjName;

                    //#warning 需要修改成标题生成规则。
                    //#warning 让子流程的Titlte与父流程的一样.

                    gwf.Title = this.HisGenerWorkFlow.Title; // WorkNode.GenerTitle(this.rptGe);
                    gwf.WFState = WFState.Runing;
                    gwf.RDT = DataType.CurrentDateTime;
                    gwf.Starter = this.HisGenerWorkFlow.Starter;
                    gwf.StarterName = this.HisGenerWorkFlow.StarterName;
                    gwf.FlowNo = nd.FlowNo;
                    gwf.FlowName = nd.FlowName;
                    gwf.FlowSortNo = this.HisNode.HisFlow.FlowSortNo;
                    gwf.SysType = this.HisNode.HisFlow.SysType;

                    gwf.NodeID = nd.NodeID;
                    gwf.NodeName = nd.Name;
                    gwf.DeptNo = oneGWL.DeptNo;
                    gwf.DeptName = oneGWL.DeptName;
                    gwf.TodoEmps = operators;
                    gwf.Domain = this.HisGenerWorkFlow.Domain; //域.
                    gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
                    if (DataType.IsNullOrEmpty(this.HisFlow.BuessFields) == false)
                    {
                        //存储到表里atPara  @BuessFields=电话^Tel^18992323232;地址^Addr^山东成都;
                        string[] expFields = this.HisFlow.BuessFields.Split(',');
                        string exp = "";
                        Attrs attrs = this.rptGe.EnMap.Attrs;
                        foreach (string item in expFields)
                        {
                            if (DataType.IsNullOrEmpty(item) == true)
                                continue;
                            if (attrs.Contains(item) == false)
                                continue;

                            Attr attr = attrs.GetAttrByKey(item);
                            exp += attr.Desc + "^" + attr.Key + "^" + this.rptGe.GetValStrByKey(item);
                        }
                        gwf.BuessFields = exp; //表达式字段.
                    }

                    if (isNew == true)
                        gwf.DirectInsert();
                    else
                        gwf.DirectUpdate();
                    #endregion 生成子线程的GWF.
                    msg += BP.WF.Glo.multilingual("@节点[{0}]成功启动, 发送给{1}位处理人:{2}.", "WorkNode", "found_node_operator", nd.Name, i.ToString(), operators);
                }
                #endregion 如果生成workid的规则模式为0,  异表单子线程WorkID生成规则  UnSameSheetWorkIDModel  0= 仅生成一个WorkID,  1=按接受人生成WorkID,


                #region 如果生成workid的规则模式为 1 ,  异表单子线程WorkID生成规则  UnSameSheetWorkIDModel  0= 仅生成一个WorkID,  1=按接受人生成WorkID,
                if (nd.USSWorkIDRole == 1)
                {
                    //产生一个工作信息。
                    Work wk = nd.HisWork;
                    //  wk.Copy(this.HisWork);
                    wk.Copy(this.rptGe); //
                    wk.FID = this.HisWork.OID;

                    //获得它的工作者。
                    WorkNode town = new WorkNode(wk, nd);
                    current_gwls = this.Func_GenerWorkerLists(town); //获得数量.
                    if (current_gwls.Count == 0)
                    {
                        msg += BP.WF.Glo.multilingual("@没有找到节点[{0}]的处理人员,所以此节点无法成功启动.", "WorkNode", "not_found_node_operator", nd.Name);
                        // wk.Delete();
                        continue;
                    }

                    #region 检查子线程是否有绑定的表单? 如果有就给它主表的数据.
                    foreach (FrmNode fn in fns)
                    {
                        //不是始终启用，就不给数据.
                        if (fn.FrmEnableRole != FrmEnableRole.Allways)
                            continue;

                        GEEntity ge = new GEEntity(fn.FK_Frm);
                        if (fn.WhoIsPK != WhoIsPK.FID)
                        {
                            ge.OID = this.HisWork.OID;
                            ge.Copy(this.HisWork);
                            if (ge.IsExits == false)
                                ge.InsertAsOID(ge.OID);
                            else
                                ge.Update();
                        }
                    }
                    #endregion 检查子线程是否有绑定的表单? 如果有就给它主表的数据.

                    #region 生成待办.
                    string operators = "";
                    int i = 0;
                    GenerWorkerList oneGWL = null; //获得这个变量，在gwf中使用.
                    string todoEmps = "";
                    foreach (GenerWorkerList wl in current_gwls)
                    {
                        if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                            workIDSubThread = DBAccess.GenerOID("WorkID");
                        else
                            workIDSubThread = DBAccess.GenerOIDByGUID();

                        GenerWorkFlow gwf = new GenerWorkFlow();
                        gwf.WorkID = workIDSubThread;
                        gwf.FID = this.WorkID;

                        wk.OID = workIDSubThread;
                        wk.DirectInsert(); //执行插入.

                        oneGWL = wl; //获得这个变量，在gwf中使用.

                        i += 1;
                        operators += wl.EmpNo + ", " + wl.EmpName + ";";
                        empIDs += wl.EmpNo + ",";
                        empNames += wl.EmpName + ",";

                        ps = new Paras();
                        ps.SQL = "UPDATE WF_GenerWorkerlist SET WorkID=" + dbStr + "WorkID1,FID=" + dbStr + "FID WHERE FK_Emp=" + dbStr + "FK_Emp AND WorkID=" + dbStr + "WorkID2 AND FK_Node=" + dbStr + "FK_Node ";
                        ps.Add("WorkID1", wk.OID);
                        ps.Add("FID", this.WorkID);

                        ps.Add("FK_Emp", wl.EmpNo);
                        ps.Add("WorkID2", wl.WorkID);
                        ps.Add("FK_Node", wl.NodeID);
                        DBAccess.RunSQL(ps);

                        //设置当前的workid.
                        wl.WorkID = wk.OID;

                        //更新工作信息.
                        wk.Rec = wl.EmpNo;

                        //为子线程产生分流节点的发送副本。
                        wl.FID = this.WorkID;
                        wl.EmpNo = WebUser.No;
                        wl.EmpName = WebUser.Name;
                        wl.PassInt = -2;
                        wl.ItIsRead = true;
                        wl.NodeID = this.HisNode.NodeID;
                        wl.NodeName = this.HisNode.Name;
                        wl.DeptNo = WebUser.DeptNo;
                        wl.DeptName = WebUser.DeptName;
                        if (wl.IsExits == false)
                            wl.Insert();

                        gwf.WorkID = wk.OID;
                        //干流、子线程关联字段
                        gwf.FID = this.WorkID;

                        //父流程关联字段
                        gwf.PWorkID = this.HisGenerWorkFlow.PWorkID;
                        gwf.PFlowNo = this.HisGenerWorkFlow.PFlowNo;
                        gwf.PNodeID = this.HisGenerWorkFlow.PNodeID;

                        //工程类项目关联字段
                        gwf.PrjNo = this.HisGenerWorkFlow.PrjNo;
                        gwf.PrjName = this.HisGenerWorkFlow.PrjName;

                        //#warning 需要修改成标题生成规则。
                        //#warning 让子流程的Titlte与父流程的一样.

                        gwf.Title = this.HisGenerWorkFlow.Title; // WorkNode.GenerTitle(this.rptGe);
                        gwf.WFState = WFState.Runing;
                        gwf.RDT = DataType.CurrentDateTime;
                        gwf.Starter = this.HisGenerWorkFlow.Starter;
                        gwf.StarterName = this.HisGenerWorkFlow.StarterName;
                        gwf.FlowNo = nd.FlowNo;
                        gwf.FlowName = nd.FlowName;
                        gwf.FlowSortNo = this.HisNode.HisFlow.FlowSortNo;
                        gwf.SysType = this.HisNode.HisFlow.SysType;

                        gwf.NodeID = nd.NodeID;
                        gwf.NodeName = nd.Name;
                        gwf.DeptNo = oneGWL.DeptNo;
                        gwf.DeptName = oneGWL.DeptName;
                        gwf.TodoEmps = wl.EmpNo + ", " + wl.EmpName + ";";
                        gwf.Domain = this.HisGenerWorkFlow.Domain; //域.
                        gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
                        if (DataType.IsNullOrEmpty(this.HisFlow.BuessFields) == false)
                        {
                            //存储到表里atPara  @BuessFields=电话^Tel^18992323232;地址^Addr^山东成都;
                            string[] expFields = this.HisFlow.BuessFields.Split(',');
                            string exp = "";
                            Attrs attrs = this.rptGe.EnMap.Attrs;
                            foreach (string item in expFields)
                            {
                                if (DataType.IsNullOrEmpty(item) == true)
                                    continue;
                                if (attrs.Contains(item) == false)
                                    continue;

                                Attr attr = attrs.GetAttrByKey(item);
                                exp += attr.Desc + "^" + attr.Key + "^" + this.rptGe.GetValStrByKey(item);
                            }
                            gwf.BuessFields = exp; //表达式字段.
                        }

                        gwf.Insert(); //插入数据.
                    }
                    #endregion 生成待办.

                    msg += BP.WF.Glo.multilingual("@节点[{0}]成功启动, 发送给{1}位处理人:{2}.", "WorkNode", "found_node_operator", nd.Name, i.ToString(), operators);
                }
                #endregion 如果生成workid的规则模式为0,  异表单子线程WorkID生成规则  UnSameSheetWorkIDModel  0= 仅生成一个WorkID,  1=按接受人生成WorkID,
            }

            //加入分流异表单，提示信息。
            this.addMsg("FenLiuUnSameSheet", msg);

            //加入变量：异表单子线程, 一般来说，到达的节点IDs, 与接收人员的IDs是一一对应关系.
            this.addMsg(SendReturnMsgFlag.VarTreadWorkIDs, workIDs, workIDs, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, empIDs, empIDs, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, empNames, empNames, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarToNodeIDs, toNodeIDs, toNodeIDs, SendReturnMsgType.SystemMsg);

            //写入日志. 
            if (this.HisNode.ItIsStartNode == true)
                this.AddToTrack(ActionType.Start, empIDs, empNames, this.HisNode.NodeID, this.HisNode.Name, msg);
            else
                this.AddToTrack(ActionType.Forward, empIDs, empNames, this.HisNode.NodeID, this.HisNode.Name, msg);

        }

        private void NodeSend_ToGenerWorkFlow(GenerWorkFlow gwf, Work wk, Node nd, bool isNew, String deptNo, String deptName, String todoEmps)
        {
            gwf.WorkID = wk.OID;
            //干流、子线程关联字段
            gwf.FID = this.WorkID;

            //父流程关联字段
            gwf.PWorkID = this.HisGenerWorkFlow.PWorkID;
            gwf.PFlowNo = this.HisGenerWorkFlow.PFlowNo;
            gwf.PNodeID = this.HisGenerWorkFlow.PNodeID;

            //工程类项目关联字段
            gwf.PrjNo = this.HisGenerWorkFlow.PrjNo;
            gwf.PrjName = this.HisGenerWorkFlow.PrjName;

            ///#warning 需要修改成标题生成规则。
            ///#warning 让子流程的Titlte与父流程的一样.

            gwf.Title = this.HisGenerWorkFlow.Title; // WorkNode.GenerTitle(this.rptGe);
            gwf.WFState = WFState.Runing;
            gwf.RDT = DataType.CurrentDateTime;
            gwf.Starter = this.HisGenerWorkFlow.Starter;
            gwf.StarterName = this.HisGenerWorkFlow.StarterName;
            gwf.FlowNo = nd.FlowNo;
            gwf.FlowName = nd.FlowName;
            gwf.FlowSortNo = this.HisNode.HisFlow.FlowSortNo;
            gwf.SysType = this.HisNode.HisFlow.SysType;

            gwf.NodeID = nd.NodeID;
            gwf.NodeName = nd.Name;
            gwf.DeptNo = deptNo;
            gwf.DeptName = deptName;
            gwf.TodoEmps = todoEmps;
            gwf.Domain = this.HisGenerWorkFlow.Domain; //域.
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            if (DataType.IsNullOrEmpty(this.HisFlow.BuessFields) == false)

            {
                //存储到表里atPara  @BuessFields=电话^Tel^18992323232;地址^Addr^山东成都;
                string[] expFields = this.HisFlow.BuessFields.Split(',');
                string exp = "";
                Attrs attrs = this.rptGe.EnMap.Attrs;
                foreach (String item in expFields)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                    {
                        continue;
                    }
                    if (attrs.Contains(item) == false)
                    {
                        continue;
                    }

                    Attr attr = attrs.GetAttrByKey(item);
                    exp += attr.Desc + "^" + attr.Key + "^" + this.rptGe.GetValStrByKey(item);
                }
                gwf.BuessFields = exp; //表达式字段.
            }

            if (isNew == true)
            {
                gwf.DirectInsert();
            }
            else
            {
                gwf.DirectUpdate();
            }

        }

        /// <summary>
        /// 产生分流点
        /// </summary>
        /// <param name="toWN"></param>
        /// <returns></returns>
        private GenerWorkerLists NodeSend_24_SameSheet_GenerWorkerList(WorkNode toWN)
        {
            return null;
        }
        /// <summary>
        /// 当前产生的接收人员列表.
        /// </summary>
        private GenerWorkerLists current_gwls = null;
        /// <summary>
        /// 处理分流点向下发送 to 同表单.
        /// </summary>
        /// <param name="toNode">到达的分流节点</param>
        private void NodeSend_24_SameSheet(Node toNode)
        {
            if (this.HisGenerWorkFlow.Title == BP.WF.Glo.multilingual("未生成", "WorkNode", "not_generated"))
                this.HisGenerWorkFlow.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork);


            #region 产生下一步骤的工作人员
            // 发起.
            Work wk = toNode.HisWork;
            wk.Copy(this.rptGe);
            wk.Copy(this.HisWork);  //复制过来主表基础信息。
            wk.FID = this.HisWork.OID; // 把该工作FID设置成干流程上的工作ID.

            // 到达的节点.
            town = new WorkNode(wk, toNode);

            // 产生下一步骤要执行的人员.
            current_gwls = this.Func_GenerWorkerLists(town);

            //删除当前工作人员给每个子线程增加的GenerWorkerlist数据
            //current_gwls.Delete(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.FID, this.WorkID); //首先清除.

            //判断当前节点是否存在分合流信息（退回的原因）。
            bool IsHaveFH = false;
            ps = new Paras();
            ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node";
            ps.Add("OID", this.HisWork.OID);
            ps.Add("FK_Node", toNode.NodeID);
            if (DBAccess.RunSQLReturnValInt(ps) != 0)
                IsHaveFH = true;

            #endregion 产生下一步骤的工作人员

            #region 复制数据.
            MapDtls dtlsFrom = new MapDtls("ND" + this.HisNode.NodeID);

            ///定义系统变量.
            string workIDs = "";

            DataTable dtWork = null;
            if (toNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {
                /*如果是按照查询SQL，确定明细表的接收人与子线程的数据。*/
                string sql = toNode.DeliveryParas;
                sql = Glo.DealExp(sql, this.HisWork);
                dtWork = DBAccess.RunSQLReturnTable(sql);
            }
            if (toNode.HisDeliveryWay == DeliveryWay.ByDtlAsSubThreadEmps)
            {
                /*如果是按照明细表，确定明细表的接收人与子线程的数据。*/
                foreach (MapDtl dtl in dtlsFrom)
                {
                    //加上顺序，防止变化，人员编号变化，处理明细表中接收人重复的问题。
                    string sql = "SELECT * FROM " + dtl.PTable + " WHERE RefPK=" + this.WorkID + " ORDER BY OID";
                    dtWork = DBAccess.RunSQLReturnTable(sql);
                    if (dtWork.Columns.Contains("UserNo") || dtWork.Columns.Contains(toNode.DeliveryParas))
                        break;
                    else
                        dtWork = null;
                }
            }

            string groupMark = "";
            int idx = -1;
            foreach (GenerWorkerList wl in current_gwls)
            {
                if (this.ItIsHaveSubThreadGroupMark == true)
                {
                    /*如果启用了批次处理,子线程的问题..*/
                    if (groupMark.Contains("@" + wl.EmpNo + "," + wl.GroupMark) == false)
                        groupMark += "@" + wl.EmpNo + "," + wl.GroupMark;
                    else
                    {
                        wl.Delete(); //删除该条垃圾数据.
                        continue;
                    }
                }

                idx++;
                Work mywk = toNode.HisWork;

                mywk.Copy(this.rptGe);
                #region 复制从表数据到子线程主表中
                //拷贝SQL查询字段的结果数据
                if (dtWork != null)
                {
                    /*用IDX处理是为了解决，人员重复出现在数据源并且还能根据索引对应的上。*/
                    DataRow dr = dtWork.Rows[idx];
                    if ((dtWork.Columns.Contains("UserNo") && dr["UserNo"].ToString().Equals(wl.EmpNo))
                        || (dtWork.Columns.Contains("No") && dr["No"].ToString().Equals(wl.EmpNo))
                        || dtWork.Columns.Contains(toNode.DeliveryParas) && dr[toNode.DeliveryParas].ToString().Equals(wl.EmpNo))
                        mywk.Copy(dr);
                }
                #endregion 复制从表数据到子线程主表中



                //是否是分组工作流程, 定义变量是为了，不让其在重复插入work数据。
                bool isGroupMarkWorklist = false;
                bool isHaveEmp = false;
                if (IsHaveFH)
                {
                    /* 如果曾经走过分流合流，就找到同一个人员同一个FID下的OID ，做这当前线程的ID。*/
                    ps = new Paras();
                    ps.SQL = "SELECT WorkID,FK_Node FROM WF_GenerWorkerlist WHERE FK_Node=" + dbStr + "FK_Node AND FID=" + dbStr + "FID AND FK_Emp=" + dbStr + "FK_Emp AND WorkID!=" + dbStr + "WorkID ORDER BY RDT DESC";
                    ps.Add("FK_Node", toNode.NodeID);
                    ps.Add("FID", this.WorkID);
                    ps.Add("FK_Emp", wl.EmpNo);
                    ps.Add("WorkID", wl.WorkID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /*没有发现，就说明以前分流节点中没有这个人的分流信息. */
                        if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                            mywk.OID = DBAccess.GenerOID("WorkID");
                        else
                            mywk.OID = DBAccess.GenerOIDByGUID();
                    }
                    else
                    {
                        int workid_old = (int)dt.Rows[0][0];
                        int fk_Node_nearly = (int)dt.Rows[0][1];
                        Node nd_nearly = new Node(fk_Node_nearly);
                        Work nd_nearly_work = nd_nearly.HisWork;
                        nd_nearly_work.OID = workid_old;
                        if (nd_nearly_work.RetrieveFromDBSources() != 0)
                        {
                            mywk.Copy(nd_nearly_work);
                            mywk.OID = workid_old;
                            isHaveEmp = true;
                        }
                        else
                        {
                            if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                                mywk.OID = DBAccess.GenerOID("WorkID");
                            else
                                mywk.OID = DBAccess.GenerOIDByGUID();
                        }

                    }
                }
                else
                {
                    //为子线程产生WorkID.
                    /* edit by zhoupeng 2015.12.24 平安夜. 处理国机的需求，判断是否有分组的情况，如果有就要找到分组的workid
                     * 让其同一个分组，只能生成一个workid。 
                     * */
                    if (this.ItIsHaveSubThreadGroupMark == true)
                    {
                        //查询该GroupMark 是否已经注册到流程引擎主表里了.
                        string sql = "SELECT WorkID FROM WF_GenerWorkFlow WHERE AtPara LIKE '%GroupMark=" + wl.GroupMark + "%' AND FID=" + this.WorkID;
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                                mywk.OID = DBAccess.GenerOID("WorkID");
                            else
                                mywk.OID = DBAccess.GenerOIDByGUID();
                        }
                        else
                        {
                            mywk.OID = Int64.Parse(dt.Rows[0][0].ToString()); //使用该分组下的，已经注册过的分组的WorkID，而非产生一个新的WorkID。
                            isGroupMarkWorklist = true; //是分组数据，让其work 就不要在重复插入了.
                        }
                    }
                    else
                    {
                        if (SystemConfig.GetValByKeyInt("GenerWorkIDModel", 0) == 0)
                            mywk.OID = DBAccess.GenerOID("WorkID");
                        else
                            mywk.OID = DBAccess.GenerOIDByGUID(); //DBAccess.GenerOID();
                    }
                }

                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    if (this.HisWork.FID == 0)
                        mywk.FID = this.HisWork.OID;

                    mywk.Rec = wl.EmpNo;

                    //判断是不是MD5流程？
                    if (this.HisFlow.ItIsMD5)
                        mywk.SetValByKey("MD5", Glo.GenerMD5(mywk));
                }


                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    //保存主表数据.
                    if (isHaveEmp == true)
                        mywk.Update();
                    else
                        mywk.InsertAsOID(mywk.OID);
                    //给系统变量赋值，放在发送后返回对象里.
                    workIDs += mywk.OID + ",";

                    //复制数据
                    SendToSameSheet_CopyData(toNode, mywk, dtlsFrom);
                }

                #region (循环最后处理)产生工作的信息
                // 产生工作的信息。
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = mywk.OID;
                if (gwf.RetrieveFromDBSources() == 0)
                {
                    gwf.FID = this.WorkID;
                    gwf.NodeID = toNode.NodeID;

                    if (this.HisNode.ItIsStartNode)
                        gwf.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork) + "(" + wl.EmpName + ")";
                    else
                        gwf.Title = this.HisGenerWorkFlow.Title + "(" + wl.EmpName + ")";

                    gwf.WFState = WFState.Runing;
                    gwf.RDT = DataType.CurrentDateTime;
                    gwf.Starter = this.Execer;
                    gwf.StarterName = this.ExecerName;
                    gwf.FlowNo = toNode.FlowNo;
                    gwf.FlowName = toNode.FlowName;

                    //干流、子线程关联字段
                    gwf.FID = this.WorkID;

                    //父流程关联字段
                    gwf.PWorkID = this.HisGenerWorkFlow.PWorkID;
                    gwf.PFlowNo = this.HisGenerWorkFlow.PFlowNo;
                    gwf.PNodeID = this.HisGenerWorkFlow.PNodeID;

                    //域.
                    gwf.Domain = this.HisGenerWorkFlow.Domain;


                    //工程类项目关联字段
                    gwf.PrjNo = this.HisGenerWorkFlow.PrjNo;
                    gwf.PrjName = this.HisGenerWorkFlow.PrjName;

                    gwf.FlowSortNo = toNode.HisFlow.FlowSortNo;
                    gwf.NodeName = toNode.Name;
                    gwf.DeptNo = wl.DeptNo;
                    gwf.DeptName = wl.DeptName;
                    gwf.TodoEmps = wl.EmpNo + "," + wl.EmpName + ";";
                    if (wl.GroupMark != "")
                        gwf.Paras_GroupMark = wl.GroupMark;

                    gwf.Sender = WebUser.No + "," + WebUser.Name + ";";

                    if (DataType.IsNullOrEmpty(this.HisFlow.BuessFields) == false)
                    {
                        //存储到表里atPara  @BuessFields=电话^Tel^18992323232;地址^Addr^山东成都;
                        string[] expFields = this.HisFlow.BuessFields.Split(',');
                        string exp = "";
                        Attrs attrs = this.rptGe.EnMap.Attrs;
                        foreach (string item in expFields)
                        {
                            if (DataType.IsNullOrEmpty(item) == true)
                                continue;
                            if (attrs.Contains(item) == false)
                                continue;

                            Attr attr = attrs.GetAttrByKey(item);
                            exp += attr.Desc + "^" + attr.Key + "^" + this.rptGe.GetValStrByKey(item);
                        }
                        gwf.BuessFields = exp;
                    }

                    gwf.DirectInsert();
                }
                else
                {
                    if (wl.GroupMark != "")
                        gwf.Paras_GroupMark = wl.GroupMark;

                    gwf.WFState = WFState.Runing;
                    gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
                    gwf.NodeID = toNode.NodeID;
                    gwf.NodeName = toNode.Name;
                    gwf.Update();
                }


                // 插入当前分流节点的处理人员,让其可以在在途里看到工作.
                //非分组工作人员.
                if (isGroupMarkWorklist == false)
                {
                    GenerWorkerList flGwl = new GenerWorkerList();
                    flGwl.Copy(wl);
                    if (isHaveEmp == true)
                        flGwl.WorkID = mywk.OID;
                    flGwl.EmpNo = WebUser.No;
                    flGwl.EmpName = WebUser.Name;
                    flGwl.NodeID = this.HisNode.NodeID;
                    flGwl.Sender = WebUser.No + "," + WebUser.Name;
                    //flGwl.DeptNo = WebUser.DeptNo;
                    //flGwl.DeptName = WebUser.DeptName;
                    flGwl.PassInt = -2; // -2; //标志该节点是干流程人员处理的节点.
                    //  wl.FID = 0; //如果是干流，
                    flGwl.Save();
                }

                //把临时的workid 更新到.
                if (isHaveEmp == false)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET WorkID=" + dbStr + "WorkID1 WHERE WorkID=" + dbStr + "WorkID2";
                    ps.Add("WorkID1", mywk.OID);
                    ps.Add("WorkID2", wl.WorkID); //临时的ID,更新最新的workid.
                    int num = DBAccess.RunSQL(ps);
                    if (num == 0)
                        throw new Exception("@不应该更新不到它。");
                }
                else
                {
                    //修改状态，不是0或者1改成0
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp AND FK_Node=" + dbStr + "FK_Node AND IsPass NOT IN(0,1)";
                    ps.Add("WorkID", mywk.OID);
                    ps.Add("FK_Emp", wl.EmpNo);
                    ps.Add("FK_Node", wl.NodeID);
                    int num = DBAccess.RunSQL(ps);
                    wl.Delete();
                }


                //设置当前的workid. 临时的id有变化.
                wl.WorkID = mywk.OID;
                #endregion 产生工作的信息.
            }
            #endregion 复制数据.

            #region 处理消息提示
            string info = BP.WF.Glo.multilingual("@分流节点[{0}]成功启动, 发送给{1}位处理人:{2}.", "WorkNode", "found_node_operator", toNode.Name, this.HisRememberMe.NumOfObjs.ToString(), this.HisRememberMe.EmpsExt);

            this.addMsg("FenLiuInfo", info);

            //把子线程的 WorkIDs 加入系统变量.
            this.addMsg(SendReturnMsgFlag.VarTreadWorkIDs, workIDs, workIDs, SendReturnMsgType.SystemMsg);

            // 如果是开始节点，就可以允许选择接收人。
            if (this.HisNode.ItIsStartNode && current_gwls.Count >= 2 && this.HisNode.ItIsTask)
                this.addMsg("AllotTask", "@<img src='./Img/AllotTask.gif' border=0 /><a href='./WorkOpt/AllotTask.htm?WorkID=" + this.WorkID + "&FID=" + this.WorkID + "&NodeID=" + toNode.NodeID + "' target=_self >修改接收对象</a>.");

            if (this.HisNode.HisCancelRole != CancelRole.None)
            {
                if (this.HisNode.ItIsStartNode)
                    this.addMsg("UnDo", "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&Token=" + WebUser.Token + "&WorkID=" + this.WorkID + "&FK_Flow=" + toNode.FlowNo + "' ><img src='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/Img/Action/UnSend.png' border=0/>撤销本次发送</a>.");
                else
                    this.addMsg("UnDo", "@<a href='./WorkOpt/UnSend.htm?DoType=UnSend&UserNo=" + WebUser.No + "&Token=" + WebUser.Token + "&WorkID=" + this.WorkID + "&FK_Flow=" + toNode.FlowNo + "' ><img src='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/Img/Action/UnSend.png' border=0/>撤销本次发送</a>.");
            }


            #endregion 处理消息提示
        }
        /// <summary>
        /// 发送给同表单子线程时干流程数据拷贝到子线程
        /// </summary>
        /// <param name="toNode"></param>
        /// <param name="mywk"></param>
        /// <param name="dtlsFrom"></param>
        private void SendToSameSheet_CopyData(Node toNode, Work mywk, MapDtls dtlsFrom)
        {

            if (dtlsFrom.Count > 1)
            {
                foreach (MapDtl d in dtlsFrom)
                    d.HisGEDtls_temp = null;
            }
            MapDtls dtlsTo = null;
            if (dtlsFrom.Count >= 1)
                dtlsTo = new MapDtls("ND" + toNode.NodeID);

            #region  复制附件信息
            //获得当前流程数节点数据.
            FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID,
                                            this.WorkID.ToString());
            if (athDBs.Count > 0)
            {
                /* 说明当前节点有附件数据 */
                athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toNode.NodeID,
                    FrmAttachmentDBAttr.RefPKVal, mywk.OID);

                foreach (FrmAttachmentDB athDB in athDBs)
                {
                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                    athDB_N.Copy(athDB);
                    athDB_N.FrmID = "ND" + toNode.NodeID;
                    athDB_N.RefPKVal = mywk.OID.ToString();
                    athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                      "ND" + toNode.NodeID);

                    if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
                    {
                        //注意如果是单附件主键的命名规则不能变化，否则会导致与前台约定获取数据错误。
                        athDB_N.setMyPK(athDB_N.FK_FrmAttachment + "_" + mywk.OID);
                        try
                        {
                            athDB_N.DirectInsert();
                        }
                        catch
                        {
                            athDB_N.setMyPK(DBAccess.GenerGUID());
                            athDB_N.Insert();
                        }
                    }
                    else
                    {
                        try
                        {
                            // 多附件就是: FK_MapData+序列号的方式, 替换主键让其可以保存,不会重复.
                            athDB_N.setMyPK(athDB_N.UploadGUID + "_" + athDB_N.FrmID + "_" + athDB_N.RefPKVal);
                            athDB_N.DirectInsert();
                        }
                        catch
                        {
                            athDB_N.setMyPK(DBAccess.GenerGUID());
                            athDB_N.Insert();
                        }
                    }
                }
            }
            #endregion  复制附件信息

            #region  复制签名信息
            if (this.HisNode.MapData.FrmImgs.Count > 0)
            {
                foreach (FrmImg img in this.HisNode.MapData.FrmImgs)
                {
                    //排除图片
                    if (img.HisImgAppType == ImgAppType.Img)
                        continue;
                    //获取数据
                    FrmEleDBs eleDBs = new FrmEleDBs(img.MyPK, this.WorkID.ToString());
                    if (eleDBs.Count > 0)
                    {
                        eleDBs.Delete(FrmEleDBAttr.FK_MapData, img.MyPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID)
                            , FrmEleDBAttr.EleID, this.WorkID);

                        /*说明当前节点有附件数据*/
                        foreach (FrmEleDB eleDB in eleDBs)
                        {
                            FrmEleDB eleDB_N = new FrmEleDB();
                            eleDB_N.Copy(eleDB);
                            eleDB_N.FrmID = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                            eleDB_N.RefPKVal = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                            eleDB_N.EleID = mywk.OID.ToString();
                            eleDB_N.GenerPKVal();
                            eleDB_N.Save();
                        }
                    }
                }
            }
            #endregion  复制附件信息

            #region 复制图片上传附件。
            if (this.HisNode.MapData.FrmImgAths.Count > 0)
            {
                FrmImgAthDBs frmImgAthDBs = new FrmImgAthDBs("ND" + this.HisNode.NodeID,
                      this.WorkID.ToString());
                if (frmImgAthDBs.Count > 0)
                {
                    frmImgAthDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toNode.NodeID,
                        FrmAttachmentDBAttr.RefPKVal, mywk.OID);

                    /*说明当前节点有附件数据*/
                    foreach (FrmImgAthDB imgAthDB in frmImgAthDBs)
                    {
                        FrmImgAthDB imgAthDB_N = new FrmImgAthDB();
                        imgAthDB_N.Copy(imgAthDB);
                        imgAthDB_N.FrmID = "ND" + toNode.NodeID;
                        imgAthDB_N.RefPKVal = mywk.OID.ToString();
                        imgAthDB_N.FK_FrmImgAth = imgAthDB_N.FK_FrmImgAth.Replace("ND" + this.HisNode.NodeID, "ND" + toNode.NodeID);
                        imgAthDB_N.Save();
                    }
                }
            }
            #endregion 复制图片上传附件。

            #region  复制从表信息.
            if (dtlsFrom.Count > 0 && dtlsTo.Count > 0)
            {
                int i = -1;
                foreach (BP.Sys.MapDtl dtl in dtlsFrom)
                {
                    i++;
                    if (dtlsTo.Count <= i)
                        continue;

                    MapDtl toDtl = (BP.Sys.MapDtl)dtlsTo[i];
                    if (toDtl.ItIsCopyNDData == false)
                        continue;

                    if (toDtl.PTable == dtl.PTable)
                        continue;

                    //获取明细数据。
                    GEDtls gedtls = null;
                    if (dtl.HisGEDtls_temp == null)
                    {
                        gedtls = new GEDtls(dtl.No);
                        QueryObject qo = null;
                        qo = new QueryObject(gedtls);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:
                                qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                break;
                            case DtlOpenType.ForWorkID:
                                qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                                break;
                            case DtlOpenType.ForFID:
                                qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                                break;
                        }
                        qo.DoQuery();
                        dtl.HisGEDtls_temp = gedtls;
                    }
                    gedtls = dtl.HisGEDtls_temp;

                    int unPass = 0;
                    DBAccess.RunSQL("DELETE FROM " + toDtl.PTable + " WHERE RefPK=" + dbStr + "RefPK", "RefPK", mywk.OID);
                    foreach (GEDtl gedtl in gedtls)
                    {
                        BP.Sys.GEDtl dtCopy = new GEDtl(toDtl.No);
                        dtCopy.Copy(gedtl);
                        dtCopy.MapDtlNo = toDtl.No;
                        dtCopy.RefPK = mywk.OID.ToString();
                        dtCopy.OID = 0;
                        dtCopy.Insert();

                        #region  复制从表单条 - 附件信息 - M2M- M2MM
                        if (toDtl.ItIsEnableAthM)
                        {
                            /*如果启用了多附件,就复制这条明细数据的附件信息。*/
                            athDBs = new FrmAttachmentDBs(dtl.No, gedtl.OID.ToString());
                            if (athDBs.Count > 0)
                            {
                                i = 0;
                                foreach (FrmAttachmentDB athDB in athDBs)
                                {
                                    i++;
                                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                                    athDB_N.Copy(athDB);
                                    athDB_N.FrmID = toDtl.No;
                                    athDB_N.setMyPK(toDtl.No + "_" + dtCopy.OID + "_" + i.ToString());
                                    athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
                                        "ND" + toNode.NodeID);
                                    athDB_N.RefPKVal = dtCopy.OID.ToString();
                                    athDB_N.DirectInsert();
                                }
                            }
                        }
                        #endregion  复制从表单条 - 附件信息
                    }
                }
            }
            #endregion  复制附件信息
        }
        /// <summary>
        /// 合流点到普通点发送
        /// 1. 首先要检查完成率.
        /// 2, 按普通节点向普通节点发送.
        /// </summary>
        /// <returns></returns>
        private void NodeSend_31(Node nd)
        {
            //检查完成率.

            // 与1-1一样的逻辑处理.
            this.NodeSend_11(nd);
        }
        /// <summary>
        /// 子线程向下发送
        /// </summary>
        /// <returns></returns>
        private string NodeSend_4x()
        {
            return null;
        }
        /// <summary>
        /// 子线程向合流点
        /// </summary>
        /// <returns></returns>
        private void NodeSend_53_SameSheet_To_HeLiu(Node toNode)
        {
            Work toNodeWK = toNode.HisWork;
            toNodeWK.Copy(this.HisWork);
            toNodeWK.OID = this.HisWork.FID;
            toNodeWK.FID = 0;
            this.town = new WorkNode(toNodeWK, toNode);

            // 获取到达当前合流节点上 与上一个分流点之间的子线程节点的集合。
            string spanNodes = this.SpanSubTheadNodes(toNode);

            #region 处理FID.
            Int64 fid = this.HisWork.FID;
            if (fid == 0)
            {
                if (this.HisNode.ItIsSubThread == false)
                    throw new Exception(BP.WF.Glo.multilingual("@当前节点非子线程节点.", "WorkNode", "not_sub_thread"));
                fid = this.HisGenerWorkFlow.FID;
                if (fid == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@丢失FID信息.", "WorkNode", "missing_FID"));

                this.HisWork.FID = fid;
            }
            #endregion FID

            // 先查询一下是否有人员，在合流节点上，如果没有就让其初始化人员. 
            current_gwls = new GenerWorkerLists();
            current_gwls.Retrieve(GenerWorkerListAttr.WorkID, this.HisWork.FID, GenerWorkerListAttr.FK_Node, toNode.NodeID);

            if (current_gwls.Count == 0)
                current_gwls = this.Func_GenerWorkerLists(this.town);// 初试化他们的工作人员．


            string toEmpsStr = "";
            string emps = "";
            foreach (GenerWorkerList wl in current_gwls)
            {
                toEmpsStr += BP.WF.Glo.DealUserInfoShowModel(wl.EmpNo, wl.EmpName);

                if (current_gwls.Count == 1)
                    emps = wl.EmpNo;
                else
                    emps += "@" + wl.EmpNo;
            }

            //写入日志, 2020.07.26 by zhoupeng.
            ActionType at = ActionType.SubThreadForward;
            this.AddToTrack(at, emps, toEmpsStr, toNode.NodeID, toNode.Name, BP.WF.Glo.multilingual("子线程", "WorkNode", "sub_thread"), this.HisNode);

            //增加变量.
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, emps.Replace("@", ","), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersName, toEmpsStr, SendReturnMsgType.SystemMsg);

            /* 
            * 更新它的节点 worklist 信息, 说明当前节点已经完成了.
            * 不让当前的操作员能看到自己的工作。
            */

            #region 设置父流程状态 设置当前的节点为:
            //根据Node判断该节点是否绑定表单库的表单
            bool isCopyData = true;
            //分流节点和子线程的节点绑定的表单相同
            if (toNode.HisFormType == NodeFormType.RefOneFrmTree && toNode.NodeFrmID.Equals(this.HisNode.NodeFrmID) == true)
                isCopyData = false;
            if (isCopyData == true)
            {
                Work mainWK = town.HisWork;
                mainWK.OID = this.HisWork.FID;
                mainWK.RetrieveFromDBSources();

                // 复制报表上面的数据到合流点上去。
                DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM " + this.HisFlow.PTable + " WHERE OID=" + dbStr + "OID",
                    "OID", this.HisWork.FID);
                foreach (DataColumn dc in dt.Columns)
                    mainWK.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);

                mainWK.Rec = WebUser.No;
                mainWK.OID = this.HisWork.FID;
                mainWK.Save();
            }


            // 产生合流汇总从表数据.
            this.GenerHieLiuHuiZhongDtlData_2013(toNode);

            //设置当前子线程已经通过.
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1  WHERE WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND IsPass=0";
            ps.Add("WorkID", this.WorkID);
            ps.Add("FID", this.HisWork.FID);
            DBAccess.RunSQL(ps);


            //合流节点上的工作处理者。
            GenerWorkerLists gwls = new GenerWorkerLists(this.HisWork.FID, toNode.NodeID);
            current_gwls = gwls;

            /* 合流点需要等待各个分流点全部处理完后才能看到它。*/
            string mysql = "";

#warning 对于多个分合流点可能会有问题。
            mysql = "SELECT COUNT(distinct WorkID) AS Num FROM WF_GenerWorkerlist WHERE IsEnable=1 AND FID=" + this.HisWork.FID + " AND FK_Node IN (" + spanNodes + ")";
            decimal numAll = (decimal)DBAccess.RunSQLReturnValInt(mysql);

            GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.FID);
            //记录子线程到达合流节点数
            int count = gwf.GetParaInt("ThreadCount");
            gwf.SetPara("ThreadCount", count + 1);
            gwf.TodoEmps = toEmpsStr;
            gwf.Update();


            decimal numPassed = gwf.GetParaInt("ThreadCount");

            decimal passRate = numPassed / numAll * 100;
            if (toNode.PassRate <= passRate)
            {
                /* 这时已经通过,可以让主线程看到待办. */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它.");

                gwf.Emps = gwf.Emps + "@" + this.HisGenerWorkFlow.Emps;
                //gwf.Para("ThreadCount", 0);
                gwf.Update();

            }
            else
            {
#warning 为了不让其显示在途的工作需要， =3 不是正常的处理模式。
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=3 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", toNode.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它.");

                gwf.Emps = gwf.Emps + "@" + this.HisGenerWorkFlow.Emps;
                gwf.Update();
            }


            this.HisGenerWorkFlow.NodeID = toNode.NodeID;
            this.HisGenerWorkFlow.NodeName = toNode.Name;

            //改变当前流程的当前节点.
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + ",  FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE WorkID=" + dbStr + "WorkID";
            ps.Add("FK_Node", toNode.NodeID);
            ps.Add("NodeName", toNode.Name);
            ps.Add("WorkID", this.HisWork.FID);
            DBAccess.RunSQL(ps);


            #endregion 设置父流程状态

            this.addMsg("InfoToHeLiu", BP.WF.Glo.multilingual("@流程已经运行到合流节点[{0}]. @您的工作已经发送给如下人员[{1}]. @您是第{2}个到达此节点的处理人.", "WorkNode", "first_node_person", toNode.Name, toEmpsStr, (count + 1).ToString()));

            #region 处理国机的需求, 把最后一个子线程的主表数据同步到合流节点的Rpt里面去.(不是很合理) 2015.12.30
            Work towk = town.HisWork;
            towk.OID = this.HisWork.FID;
            towk.RetrieveFromDBSources();
            towk.Copy(this.HisWork);
            towk.DirectUpdate();
            #endregion 处理国机的需求, 把最后一个子线程的主表数据同步到合流节点的Rpt里面去.

        }

        /// <summary>
        /// 节点向下运动
        /// </summary>
        private void NodeSend_Send_5_5()
        {
            //执行设置当前人员的完成时间. for: anhua 2013-12-18.
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET CDT=" + dbstr + "CDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add(GenerWorkerListAttr.CDT, DataType.CurrentDateTimess);
            ps.Add(GenerWorkerListAttr.WorkID, this.WorkID);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);
            ps.Add(GenerWorkerListAttr.FK_Emp, this.Execer);
            DBAccess.RunSQL(ps);

            switch (this.HisNode.HisRunModel)
            {
                case RunModel.Ordinary: /* 1： 普通节点向下发送的*/
                    Node toND = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    if (this.HisNode.FlowNo.Equals(toND.FlowNo) == false)
                    {
                        NodeSendToYGFlow(toND, JumpToEmp);
                        return;
                    }

                    //写入到达信息.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND.NodeID.ToString(), toND.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND.Name, toND.Name, SendReturnMsgType.SystemMsg);
                    switch (toND.HisRunModel)
                    {
                        case RunModel.Ordinary:   /*1-1 普通节to普通节点 */
                            this.NodeSend_11(toND);
                            break;
                        case RunModel.FL:  /* 1-2 普通节to分流点 */
                            this.NodeSend_11(toND);
                            break;
                        case RunModel.HL:  /*1-3 普通节to合流点   */
                            this.NodeSend_11(toND);
                            // throw new Exception("@流程设计错误:请检查流程获取详细信息, 普通节点下面不能连接合流节点(" + toND.Name + ").");
                            break;
                        case RunModel.FHL: /*1-4 普通节点to分合流点 */
                            this.NodeSend_11(toND);
                            break;
                        // throw new Exception("@流程设计错误:请检查流程获取详细信息, 普通节点下面不能连接分合流节点(" + toND.Name + ").");
                        case RunModel.SubThreadSameWorkID: /*1-5 普通节to子线程点 */
                        case RunModel.SubThreadUnSameWorkID: /*1-5 普通节to子线程点 */

                            throw new Exception(BP.WF.Glo.multilingual("@流程设计错误: 普通节点下面[" + this.HisNode.Name + "]不能连接子线程节点{0}", "WorkNode", "workflow_error_3", toND.Name));
                        default:
                            throw new Exception(BP.WF.Glo.multilingual("@没有判断的节点类型({0}).", "WorkNode", "node_type_does_not_exist", toND.Name));
                            break;
                    }
                    break;
                case RunModel.FL: /* 2: 分流节点向下发送的*/
                    Nodes toNDs = this.Func_GenerNextStepNodes();
                    if (toNDs.Count == 1)
                    {
                        Node toND2 = toNDs[0] as Node;
                        //加入系统变量.
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, toND2.NodeID.ToString(), toND2.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, toND2.Name, toND2.Name, SendReturnMsgType.SystemMsg);

                        switch (toND2.HisRunModel)
                        {
                            case RunModel.Ordinary:    /*2.1 分流点to普通节点 */
                                this.NodeSend_11(toND2); /* 按普通节点到普通节点处理. */
                                break;
                            case RunModel.FL:  /*2.2 分流点to分流点  */
                            //  throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接分流节点(" + toND2.Name + ").");
                            case RunModel.HL:  /*2.3 分流点to合流点,分合流点. */
                            case RunModel.FHL:
                                this.NodeSend_11(toND2); /* 按普通节点到普通节点处理. */
                                break;
                            // throw new Exception("@流程设计错误:请检查流程获取详细信息, 分流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + toND2.Name + ").");
                            case RunModel.SubThreadSameWorkID: /* 2.4 分流点to子线程点   */
                            case RunModel.SubThreadUnSameWorkID: /* 2.4 分流点to子线程点   */

                                if (toND2.HisRunModel == RunModel.SubThreadSameWorkID)
                                {
                                    this.HisGenerWorkFlow.NodeName += "," + toND2.Name;
                                    this.HisGenerWorkFlow.SetPara("ThreadCount", 0);
                                    this.HisGenerWorkFlow.DirectUpdate();

                                    NodeSend_24_SameSheet(toND2);
                                }
                                else
                                {
                                    //为计算中心：执行更新.
                                    string names = "";
                                    foreach (Node mynd in toNDs)
                                    {
                                        names += "," + mynd.Name;
                                    }
                                    this.HisGenerWorkFlow.NodeName += names;
                                    this.HisGenerWorkFlow.SetPara("ThreadCount", 0); //子线程增加方向条件，出现跳转的修改
                                    this.HisGenerWorkFlow.DirectUpdate();
                                    NodeSend_24_UnSameSheet(toNDs); /*可能是只发送1个异表单*/



                                }
                                break;
                            default:
                                throw new Exception(BP.WF.Glo.multilingual("@没有判断的节点类型({0}).", "WorkNode", "node_type_does_not_exist", toND2.Name));
                                break;
                        }
                    }
                    else
                    {
                        /* 如果有多个节点，检查一下它们必定是子线程节点否则，就是设计错误。*/
                        bool isHaveSameSheet = false;
                        bool isHaveUnSameSheet = false;
                        foreach (Node nd in toNDs)
                        {
                            switch (nd.HisRunModel)
                            {
                                case RunModel.Ordinary:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                case RunModel.FL:
                                case RunModel.FHL:
                                case RunModel.HL:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                default:
                                    break;
                            }
                            if (nd.HisRunModel == RunModel.SubThreadSameWorkID)
                                isHaveSameSheet = true;

                            if (nd.HisRunModel == RunModel.SubThreadUnSameWorkID)
                                isHaveUnSameSheet = true;
                        }
                        if (isHaveSameSheet == false && isHaveUnSameSheet == false)
                            throw new Exception(BP.WF.Glo.multilingual("@不支持流程模式: 分流节点同时启动了多个线性节点.", "WorkNode", "workflow_error_5"));

                        if (isHaveUnSameSheet && isHaveSameSheet)
                            throw new Exception(BP.WF.Glo.multilingual("@不支持流程模式: 分流节点同时启动了同表单的子线程与异表单的子线程.", "WorkNode", "workflow_error_4"));

                        if (isHaveSameSheet == true)
                            throw new Exception(BP.WF.Glo.multilingual("@不支持流程模式: 分流节点同时启动了多个同表单的子线程.", "WorkNode", "workflow_error_5"));

                        this.HisGenerWorkFlow.SetPara("ThreadCount", 0); //子线程增加方向条件，出现跳转的修改
                        this.HisGenerWorkFlow.DirectUpdate();
                        //启动多个异表单子线程节点.
                        this.NodeSend_24_UnSameSheet(toNDs);

                        //为计算中心：执行更新.
                        string names = "";
                        foreach (Node mynd in toNDs)
                        {
                            names += "," + mynd.Name;
                        }
                        this.HisGenerWorkFlow.NodeName += names;
                        this.HisGenerWorkFlow.DirectUpdate();

                    }
                    break;
                case RunModel.HL:  /* 3: 合流节点向下发送 */
                    Node toND3 = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    //加入系统变量.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND3.NodeID.ToString(), toND3.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND3.Name, toND3.Name, SendReturnMsgType.SystemMsg);

                    switch (toND3.HisRunModel)
                    {
                        case RunModel.Ordinary: /*3.1 普通工作节点 */
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        case RunModel.FL: /*3.2 分流点 */
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        case RunModel.HL: /*3.3 合流点 */
                        case RunModel.FHL:
                            this.NodeSend_31(toND3); /* 让它与普通点点普通点一样的逻辑. */
                            break;
                        //throw new Exception("@流程设计错误:请检查流程获取详细信息, 合流点(" + this.HisNode.Name + ")下面不能连接合流节点(" + toND3.Name + ").");
                        case RunModel.SubThreadUnSameWorkID: /*3.4 子线程*/
                        case RunModel.SubThreadSameWorkID: /*3.4 子线程*/

                            throw new Exception(BP.WF.Glo.multilingual("@流程设计错误: 合流节点({0})下面不能连接子线程节点({1})", "WorkNode", "workflow_error_6", this.HisNode.Name, toND3.Name));
                        default:
                            throw new Exception(BP.WF.Glo.multilingual("@没有判断的节点类型({0}).", "WorkNode", "node_type_does_not_exist", toND3.Name));
                    }
                    break;
                case RunModel.FHL:  /* 4: 分流节点向下发送的 */
                    if (this.IsStopFlow)
                        return;
                    Nodes toND4s = this.Func_GenerNextStepNodes();
                    if (toND4s.Count == 1)
                    {
                        Node toND4 = toND4s[0] as Node;
                        //加入系统变量.
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, toND4.NodeID.ToString(), toND4.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, toND4.Name, toND4.Name, SendReturnMsgType.SystemMsg);

                        switch (toND4.HisRunModel)
                        {
                            case RunModel.Ordinary: /*4.1 普通工作节点 */
                                this.NodeSend_11(toND4); /* 让它与普通点点普通点一样的逻辑. */
                                break;
                            case RunModel.FL: /*4.2 分流点 */
                                throw new Exception(BP.WF.Glo.multilingual("@流程设计错误: 合流节点({0})下面不能连接分流节点({1})", "WorkNode", "workflow_error_7", this.HisNode.Name, toND4.Name));
                            case RunModel.HL: /*4.3 合流点 */
                            case RunModel.FHL:
                                this.NodeSend_11(toND4); /* 让它与普通点点普通点一样的逻辑. */
                                break;
                            case RunModel.SubThreadSameWorkID:/*4.5 子线程*/
                            case RunModel.SubThreadUnSameWorkID:/*4.5 子线程*/

                                if (toND4.HisRunModel == RunModel.SubThreadSameWorkID)
                                {

                                    this.HisGenerWorkFlow.SetPara("ThreadCount", 0);
                                    this.HisGenerWorkFlow.DirectUpdate();
                                    NodeSend_24_SameSheet(toND4);

                                    // 为广西计算中心.
                                    this.HisGenerWorkFlow.NodeName += "," + toND4.Name;

                                    this.HisGenerWorkFlow.DirectUpdate();
                                }
                                else
                                {
                                    Nodes toNDs4 = this.Func_GenerNextStepNodes();
                                    this.HisGenerWorkFlow.SetPara("ThreadCount", 0);
                                    this.HisGenerWorkFlow.DirectUpdate();
                                    NodeSend_24_UnSameSheet(toNDs4); /*可能是只发送1个异表单*/

                                    //为计算中心：执行更新.
                                    string names = "";
                                    foreach (Node mynd in toNDs4)
                                    {
                                        names += "," + mynd.Name;
                                    }
                                    this.HisGenerWorkFlow.NodeName += names;
                                    this.HisGenerWorkFlow.DirectUpdate();
                                }
                                break;
                            default:
                                throw new Exception(BP.WF.Glo.multilingual("@没有判断的节点类型({0}).", "WorkNode", "node_type_does_not_exist", toND4.Name));
                        }
                    }
                    else
                    {
                        /* 如果有多个节点，检查一下它们必定是子线程节点否则，就是设计错误。*/
                        bool isHaveSameSheet = false;
                        bool isHaveUnSameSheet = false;
                        foreach (Node nd in toND4s)
                        {
                            switch (nd.HisRunModel)
                            {
                                case RunModel.Ordinary:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                case RunModel.FL:
                                case RunModel.FHL:
                                case RunModel.HL:
                                    NodeSend_11(nd); /*按普通节点到普通节点处理.*/
                                    break;
                                default:
                                    break;
                            }
                            if (nd.HisRunModel == RunModel.SubThreadSameWorkID)
                                isHaveSameSheet = true;

                            if (nd.HisRunModel == RunModel.SubThreadUnSameWorkID)
                                isHaveUnSameSheet = true;
                        }

                        if (isHaveUnSameSheet && isHaveSameSheet)
                            throw new Exception(BP.WF.Glo.multilingual("@不支持流程模式: 分流节点同时启动了同表单的子线程与异表单的子线程.", "WorkNode", "workflow_error_4"));

                        if (isHaveSameSheet == true)
                            throw new Exception(BP.WF.Glo.multilingual("@不支持流程模式: 分流节点同时启动了多个同表单的子线程.", "WorkNode", "workflow_error_5"));

                        this.HisGenerWorkFlow.SetPara("ThreadCount", 0);
                        this.HisGenerWorkFlow.DirectUpdate();
                        //启动多个异表单子线程节点.
                        this.NodeSend_24_UnSameSheet(toND4s);

                        //为计算中心：执行更新.
                        string names = "";
                        foreach (Node mynd in toND4s)
                        {
                            names += "," + mynd.Name;
                        }
                        this.HisGenerWorkFlow.NodeName += names;
                        this.HisGenerWorkFlow.DirectUpdate();

                    }
                    break;
                case RunModel.SubThreadSameWorkID: /* 5: 子线程节点向下发送的 */
                case RunModel.SubThreadUnSameWorkID:
                    Node toND5 = this.NodeSend_GenerNextStepNode();
                    if (this.IsStopFlow)
                        return;

                    //加入系统变量.
                    this.addMsg(SendReturnMsgFlag.VarToNodeID, toND5.NodeID.ToString(), toND5.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarToNodeName, toND5.Name, toND5.Name, SendReturnMsgType.SystemMsg);

                    switch (toND5.HisRunModel)
                    {
                        case RunModel.Ordinary: /*5.1 普通工作节点 */
                            throw new Exception(BP.WF.Glo.multilingual("@流程设计错误: 子线程节点({0})下面不能连接普通节点({1})", "WorkNode", "workflow_error_8", this.HisNode.Name, toND5.Name));
                            break;
                        case RunModel.FL: /*5.2 分流点 */
                            throw new Exception(BP.WF.Glo.multilingual("@流程设计错误: 子线程节点({0})下面不能连接分流节点({1})", "WorkNode", "workflow_error_9", this.HisNode.Name, toND5.Name));
                        case RunModel.HL: /*5.3 合流点 */
                        case RunModel.FHL: /*5.4 分合流点 */
                            if (this.HisNode.HisRunModel == RunModel.SubThreadSameWorkID)
                                this.NodeSend_53_SameSheet_To_HeLiu(toND5);
                            else
                                this.NodeSend_53_UnSameSheet_To_HeLiu(toND5);

                            //把合流点设置未读.
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsRead=0 WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND  FK_Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Node";
                            ps.Add("WorkID", this.HisWork.FID);
                            ps.Add("FK_Node", toND5.NodeID);
                            DBAccess.RunSQL(ps);
                            break;
                        case RunModel.SubThreadSameWorkID: /* 5.5 子线程   */
                        case RunModel.SubThreadUnSameWorkID:

                            //为计算中心增加,子线程停留节点.
                            GenerWorkFlow gwfZhuGan = new GenerWorkFlow(this.HisGenerWorkFlow.FID);
                            if (gwfZhuGan.NodeName.Contains("," + toND5.Name) == false)
                            {
                                gwfZhuGan.NodeName = gwfZhuGan.NodeName.Replace("," + this.HisNode.Name, "," + toND5.Name);
                                if (gwfZhuGan.NodeName.Contains("," + toND5.Name) == false)
                                    gwfZhuGan.NodeName += "," + toND5.Name;
                                gwfZhuGan.DirectUpdate(); //执行更新.
                            }


                            if (toND5.HisRunModel == this.HisNode.HisRunModel)
                            {
                                #region 删除到达节点的子线程如果有，防止退回信息垃圾数据问题,如果退回处理了这个部分就不需要处理了.
                                ps = new Paras();
                                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE FID=" + dbStr + "FID  AND FK_Node=" + dbStr + "FK_Node";
                                ps.Add("FID", this.HisWork.FID);
                                ps.Add("FK_Node", toND5.NodeID);
                                #endregion 删除到达节点的子线程如果有，防止退回信息垃圾数据问题，如果退回处理了这个部分就不需要处理了.

                                this.NodeSend_11(toND5); /*与普通节点一样.*/
                            }
                            else
                                throw new Exception(BP.WF.Glo.multilingual("@流程设计错误：两个连续子线程的子线程模式不一样(从节点{0}到节点{1}).", "WorkNode", "workflow_error_10", this.HisNode.Name, toND5.Name));
                            break;
                        default:
                            throw new Exception(BP.WF.Glo.multilingual("@没有判断的节点类型({0}).", "WorkNode", "node_type_does_not_exist", toND5.Name));
                    }
                    break;
                default:
                    throw new Exception(BP.WF.Glo.multilingual("@没有判断的执行节点类型({0}).", "WorkNode", "node_type_does_not_exist", this.HisNode.HisRunModelT));
            }
        }

        #region 执行数据copy.
        public void CopyData(Work toWK, Node toND, bool isSamePTable)
        {
            //如果存储模式为, 合并模式.
            if (toND.ItIsSubThread == false)
                return;

            string errMsg = "如果两个数据源不想等，就执行 copy - 期间出现错误.";
            if (isSamePTable == true)
                return;

            #region 主表数据copy.
            if (isSamePTable == false)
            {
                toWK.SetValByKey("OID", this.HisWork.OID); //设定它的ID.
                if (this.HisNode.ItIsStartNode == false)
                    toWK.Copy(this.rptGe);

                toWK.Copy(this.HisWork); // 执行 copy 上一个节点的数据。
                toWK.Rec = this.Execer;

                //要考虑FID的问题.
                if (this.HisNode.ItIsSubThread == true
                    && toND.ItIsSubThread == true)
                    toWK.FID = this.HisWork.FID;

                try
                {
                    //判断是不是MD5流程？
                    if (this.HisFlow.ItIsMD5)
                        toWK.SetValByKey("MD5", Glo.GenerMD5(toWK));

                    if (toWK.IsExits)
                        toWK.Update();
                    else
                        toWK.Insert();
                }
                catch (Exception ex)
                {
                    toWK.CheckPhysicsTable();
                    try
                    {
                        toWK.Copy(this.HisWork); // 执行 copy 上一个节点的数据。
                        toWK.Rec = this.Execer;
                        toWK.SaveAsOID(toWK.OID);
                    }
                    catch (Exception ex11)
                    {
                        if (toWK.Update() == 0)
                            throw new Exception(ex.Message + " == " + ex11.Message);
                    }
                }
            }
            #endregion 主表数据copy.

            //            #region 复制附件。
            //            if (this.HisNode.MapData.FrmAttachments.Count > 0)
            //            {
            //                删除上一个节点可能有的数据，有可能是发送退回来的产生的垃圾数据.
            //                Paras ps = new Paras();
            //                ps.SQL = "DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData=" + dbStr + "FK_MapData AND RefPKVal=" + dbStr + "RefPKVal";
            //                ps.Add(FrmAttachmentDBAttr.FK_MapData, "ND" + toND.NodeID);
            //                ps.Add(FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());
            //                DBAccess.RunSQL(ps);

            //                FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + this.HisNode.NodeID,
            //                      this.WorkID.ToString());

            //                int idx = 0;
            //                if (athDBs.Count > 0)
            //                {
            //                    /*说明当前节点有附件数据*/
            //                    foreach (FrmAttachmentDB athDB in athDBs)
            //                    {
            //                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
            //                        athDB_N.Copy(athDB);
            //                        athDB_N.FrmID ="ND" + toND.NodeID);
            //                        athDB_N.RefPKVal = this.HisWork.OID.ToString();
            //                        athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
            //                          "ND" + toND.NodeID);

            //                        if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
            //                        {
            //                            /*如果是单附件.*/
            //                            athDB_N.setMyPK(athDB_N.FK_FrmAttachment + "_" + this.HisWork.OID);
            //                            if (athDB_N.IsExits == true)
            //                                continue; /*说明上一个节点或者子线程已经copy过了, 但是还有子线程向合流点传递数据的可能，所以不能用break.*/
            //                            try
            //                            {
            //                                athDB_N.Insert();
            //                            }
            //                            catch
            //                            {
            //                                athDB_N.setMyPK(DBAccess.GenerGUID());
            //                                athDB_N.Insert();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //判断这个guid 的上传文件是否被其他的线程copy过去了？
            //                            if (athDB_N.IsExit(FrmAttachmentDBAttr.UploadGUID, athDB_N.UploadGUID,
            //                                FrmAttachmentDBAttr.FK_MapData, athDB_N.FrmID) == true)
            //                                continue; /*如果是就不要copy了.*/

            //                            athDB_N.setMyPK(athDB_N.UploadGUID + "_" + athDB_N.FrmID + "_" + toWK.OID);
            //                            try
            //                            {
            //                                athDB_N.Insert();
            //                            }
            //                            catch
            //                            {
            //                                athDB_N.setMyPK(DBAccess.GenerGUID());
            //                                athDB_N.Insert();
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            #endregion 复制附件。

            //            #region 复制图片上传附件。
            //            if (this.HisNode.MapData.FrmImgAths.Count > 0)
            //            {
            //                FrmImgAthDBs athDBs = new FrmImgAthDBs("ND" + this.HisNode.NodeID,
            //                      this.WorkID.ToString());
            //                int idx = 0;
            //                if (athDBs.Count > 0)
            //                {
            //                    athDBs.Delete(FrmAttachmentDBAttr.FK_MapData, "ND" + toND.NodeID,
            //                        FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());

            //                    /*说明当前节点有附件数据*/
            //                    foreach (FrmImgAthDB athDB in athDBs)
            //                    {
            //                        idx++;
            //                        FrmImgAthDB athDB_N = new FrmImgAthDB();
            //                        athDB_N.Copy(athDB);
            //                        athDB_N.FrmID ="ND" + toND.NodeID);
            //                        athDB_N.RefPKVal = this.WorkID.ToString();
            //                        athDB_N.setMyPK(this.WorkID + "_" + idx + "_" + athDB_N.FrmID);
            //                        athDB_N.FK_FrmImgAth = athDB_N.FK_FrmImgAth.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID);
            //                        athDB_N.Save();
            //                    }
            //                }
            //            }
            //            #endregion 复制图片上传附件。

            //            #region 复制Ele
            //            if (this.HisNode.MapData.FrmImgs.Count > 0)
            //            {
            //                foreach (FrmImg img in this.HisNode.MapData.FrmImgs)
            //                {
            //                    排除图片
            //                    if (img.HisImgAppType == ImgAppType.Img)
            //                        continue;
            //                    获取数据
            //                    FrmEleDBs eleDBs = new FrmEleDBs(img.EnPK, this.WorkID.ToString());
            //                    if (eleDBs.Count > 0)
            //                    {
            //                        eleDBs.Delete(FrmEleDBAttr.FK_MapData, img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID)
            //                            , FrmEleDBAttr.EleID, this.WorkID);

            //                        /*说明当前节点有附件数据*/
            //                        foreach (FrmEleDB eleDB in eleDBs)
            //                        {
            //                            FrmEleDB eleDB_N = new FrmEleDB();
            //                            eleDB_N.Copy(eleDB);
            //                            eleDB_N.FrmID =img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID));
            //                            eleDB_N.RefPKVal = img.EnPK.Replace("ND" + this.HisNode.NodeID, "ND" + toND.NodeID);
            //                            eleDB_N.GenerPKVal();
            //                            eleDB_N.Save();
            //                        }
            //                    }
            //                }
            //            }
            //            #endregion 复制Ele

            //            #region 复制明细数据
            //            int deBugDtlCount =
            //           Sys.MapDtls dtls = this.HisNode.MapData.MapDtls;
            //            string[] para = new string[3];
            //            para[0] = this.HisNode.NodeID.ToString();
            //            para[1] = this.WorkID.ToString();
            //            para[2] = toND.NodeID.ToString();
            //            string recDtlLog = BP.WF.Glo.multilingual("@记录测试明细表Copy过程,从节点ID:{0}, WorkID:{1}, 到节点ID:{2}", "WorkNode", "log_copy", para);

            //            if (dtls.Count > 0)
            //            {
            //                Sys.MapDtls toDtls = toND.MapData.MapDtls;
            //                recDtlLog += BP.WF.Glo.multilingual("@到节点明细表数量是{0}个", "WorkNode", "count_of_detail_table", dtls.Count.ToString());

            //                Sys.MapDtls startDtls = null;
            //                bool isEnablePass = false; /*是否有明细表的审批.*/
            //                foreach (MapDtl dtl in dtls)
            //                {
            //                    if (dtl.IsEnablePass)
            //                        isEnablePass = true;
            //                }

            //                if (isEnablePass) /* 如果有就建立它开始节点表数据 */
            //                    startDtls = new BP.Sys.MapDtls("ND" + int.Parse(toND.FlowNo) + "01");

            //                recDtlLog += BP.WF.Glo.multilingual("@进入循环开始执行逐个明细表copy:", "WorkNode", "start_copy_detail_tables");
            //                int i = -1;

            //                foreach (BP.Sys.MapDtl dtl in dtls)
            //                {
            //                    recDtlLog += BP.WF.Glo.multilingual("@进入循环开始执行明细表({0})copy:", "WorkNode", "start_copy_detail_table", dtl.No);

            //                    如果当前的明细表，不需要copy.
            //                    if (dtl.IsCopyNDData == false)
            //                        continue;

            //                    i++;
            //                    if (toDtls.Count <= i)
            //                        continue;
            //                    Sys.MapDtl toDtl = (BP.Sys.MapDtl)toDtls[i];

            //                    i++;
            //                    if (toDtls.Count <= i)
            //                        continue;
            //                    Sys.MapDtl toDtl = null;
            //                    foreach (MapDtl todtl in toDtls)
            //                    {
            //                        if (todtl.PTable == dtl.PTable)
            //                            continue;

            //                        string toDtlName = "";
            //                        string dtlName = "";
            //                        try
            //                        {
            //                            toDtlName = todtl.HisGEDtl.FK_MapDtl.Substring(todtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"), todtl.HisGEDtl.FK_MapDtl.Length - todtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"));
            //                            dtlName = dtl.HisGEDtl.FK_MapDtl.Substring(dtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"), dtl.HisGEDtl.FK_MapDtl.Length - dtl.HisGEDtl.FK_MapDtl.IndexOf("Dtl"));
            //                        }
            //                        catch
            //                        {
            //                            continue;
            //                        }

            //                        if (toDtlName == dtlName)
            //                        {
            //                            toDtl = todtl;
            //                            break;
            //                        }
            //                    }

            //                    if (dtl.IsEnablePass == true)
            //                    {
            //                        /*如果启用了是否明细表的审核通过机制,就允许copy节点数据。*/
            //                        toDtl.IsCopyNDData = true;
            //                    }

            //                    if (toDtl == null || toDtl.IsCopyNDData == false)
            //                        continue;

            //                    if (dtl.PTable == toDtl.PTable)
            //                        continue;


            //                    获取明细数据。
            //                    GEDtls gedtls = new GEDtls(dtl.No);
            //                    QueryObject qo = null;
            //                    qo = new QueryObject(gedtls);
            //                    switch (dtl.DtlOpenType)
            //                    {
            //                        case DtlOpenType.ForEmp:
            //                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
            //                            break;
            //                        case DtlOpenType.ForWorkID:
            //                            qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
            //                            break;
            //                        case DtlOpenType.ForFID:
            //                            qo.AddWhere(GEDtlAttr.FID, this.WorkID);
            //                            break;
            //                    }
            //                    qo.DoQuery();

            //                    recDtlLog += BP.WF.Glo.multilingual("@从明细表({0})查询数据一共{1}条.", "WorkNode", "log_detail_table_1", dtl.No, gedtls.Count.ToString());

            //                    int unPass = 0;
            //                    是否启用审核机制。
            //                    isEnablePass = dtl.IsEnablePass;
            //                    if (isEnablePass && this.HisNode.ItIsStartNode == false)
            //                        isEnablePass = true;
            //                    else
            //                        isEnablePass = false;

            //                    if (isEnablePass == true)
            //                    {
            //                        /*判断当前节点该明细表上是否有，isPass 审核字段，如果没有抛出异常信息。*/
            //                        if (gedtls.Count != 0)
            //                        {
            //                            GEDtl dtl1 = gedtls[0] as GEDtl;
            //                            if (dtl1.EnMap.Attrs.Contains("IsPass") == false)
            //                                isEnablePass = false;
            //                        }
            //                    }

            //                    recDtlLog += BP.WF.Glo.multilingual("@数据删除到达明细表:{0},并开始遍历明细表,执行一行行的copy.", "WorkNode", "log_detail_table_2", dtl.No);

            //                    if (DBAccess.IsExitsObject(toDtl.PTable))
            //                        DBAccess.RunSQL("DELETE FROM " + toDtl.PTable + " WHERE RefPK=" + dbStr + "RefPK", "RefPK", this.WorkID.ToString());

            //                    copy数量.
            //                    int deBugNumCopy = 0;
            //                    foreach (GEDtl gedtl in gedtls)
            //                    {
            //                        if (isEnablePass)
            //                        {
            //                            if (gedtl.GetValBooleanByKey("IsPass") == false)
            //                            {
            //                                /*没有审核通过的就 continue 它们，仅复制已经审批通过的.*/
            //                                continue;
            //                            }
            //                        }

            //                        BP.Sys.GEDtl dtCopy = new GEDtl(toDtl.No);
            //                        dtCopy.Copy(gedtl);
            //                        dtCopy.FK_MapDtl = toDtl.No;
            //                        dtCopy.RefPK = this.WorkID.ToString();
            //                        dtCopy.InsertAsOID(dtCopy.OID);
            //                        dtCopy.RefPKInt64 = this.WorkID;
            //                        deBugNumCopy++;

            //                        #region  复制明细表单条 - 附件信息
            //                        if (toDtl.IsEnableAthM)
            //                        {
            //                            /*如果启用了多附件,就复制这条明细数据的附件信息。*/
            //                            FrmAttachmentDBs athDBs = new FrmAttachmentDBs(dtl.No, gedtl.OID.ToString());
            //                            if (athDBs.Count > 0)
            //                            {
            //                                i = 0;
            //                                foreach (FrmAttachmentDB athDB in athDBs)
            //                                {
            //                                    i++;
            //                                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
            //                                    athDB_N.Copy(athDB);
            //                                    athDB_N.FrmID =toDtl.No);
            //                                    athDB_N.setMyPK(athDB.MyPK + "_" + dtCopy.OID + "_" + i.ToString());
            //                                    athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + this.HisNode.NodeID,
            //                                        "ND" + toND.NodeID);
            //                                    athDB_N.RefPKVal = dtCopy.OID.ToString();
            //                                    try
            //                                    {
            //                                        athDB_N.DirectInsert();
            //                                    }
            //                                    catch
            //                                    {
            //                                        athDB_N.DirectUpdate();
            //                                    }

            //                                }
            //                            }
            //                        }
            //                        #endregion  复制明细表单条 - 附件信息

            //                    }
            //#warning 记录日志.
            //                    if (gedtls.Count != deBugNumCopy)
            //                    {
            //                        recDtlLog += BP.WF.Glo.multilingual("@从明细表({0})查询数据一共{1}条.", "WorkNode", "log_detail_table_1", dtl.No, gedtls.Count.ToString());

            //                        记录日志.
            //                        BP.DA.Log.DebugWriteError(recDtlLog);
            //                        throw new Exception(BP.WF.Glo.multilingual("@系统出现错误,请将如下信息反馈给管理员,谢谢。技术信息:{0}.", "WorkNode", "system_error", recDtlLog));

            //                    }

            //                    #region 如果启用了审核机制
            //                    if (isEnablePass)
            //                    {
            //                        /* 如果启用了审核通过机制，就把未审核的数据copy到第一个节点上去 
            //                         * 1, 找到对应的明细点.
            //                         * 2, 把未审核通过的数据复制到开始明细表里.
            //                         */
            //                        string fk_mapdata = "ND" + int.Parse(toND.FlowNo) + "01";
            //                        MapData md = new MapData(fk_mapdata);
            //                        string startUser = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
            //                        startUser = DBAccess.RunSQLReturnString(startUser);

            //                        MapDtl startDtl = (MapDtl)startDtls[i];
            //                        foreach (GEDtl gedtl in gedtls)
            //                        {
            //                            if (gedtl.GetValBooleanByKey("IsPass"))
            //                                continue; /* 排除审核通过的 */

            //                            BP.Sys.GEDtl dtCopy = new GEDtl(startDtl.No);
            //                            dtCopy.Copy(gedtl);
            //                            dtCopy.OID = 0;
            //                            dtCopy.FK_MapDtl = startDtl.No;
            //                            dtCopy.RefPK = gedtl.OID.ToString(); //this.WorkID.ToString();
            //                            dtCopy.SetValByKey("BatchID", this.WorkID);
            //                            dtCopy.SetValByKey("IsPass", 0);
            //                            dtCopy.SetValByKey("Rec", startUser);
            //                            dtCopy.SetValByKey("Checker", this.ExecerName);
            //                            dtCopy.RefPKInt64 = this.WorkID;
            //                            dtCopy.SaveAsOID(gedtl.OID);
            //                        }
            //                        DBAccess.RunSQL("UPDATE " + startDtl.PTable + " SET Rec='" + startUser + "',Checker='" + this.Execer + "' WHERE BatchID=" + this.WorkID + " AND Rec='" + this.Execer + "'");
            //                    }
            //                    #endregion 如果启用了审核机制
            //                }
            //            }
            //            #endregion 复制明细数据
        }
        #endregion

        #region 返回对象处理.
        public SendReturnObjs HisMsgObjs = null;
        public void addMsg(string flag, string msg)
        {
            addMsg(flag, msg, null, SendReturnMsgType.Info);
        }
        public void addMsg(string flag, string msg, SendReturnMsgType msgType)
        {
            addMsg(flag, msg, null, msgType);
        }
        public void addMsg(string flag, string msg, string msgofHtml, SendReturnMsgType msgType)
        {
            if (HisMsgObjs == null)
                HisMsgObjs = new SendReturnObjs();
            this.HisMsgObjs.AddMsg(flag, msg, msgofHtml, msgType);
        }
        public void addMsg(string flag, string msg, string msgofHtml)
        {
            addMsg(flag, msg, msgofHtml, SendReturnMsgType.Info);
        }
        #endregion 返回对象处理.

        #region 方法
        /// <summary>
        /// 发送失败是撤消数据。
        /// </summary>
        public void DealEvalUn()
        {

            //数据发送。
            BP.WF.Data.Eval eval = new Eval();
            if (this.HisNode.ItIsFLHL == false)
            {
                eval.setMyPK(this.WorkID + "_" + this.HisNode.NodeID);
                eval.Delete();
            }

            // 分合流的情况，它是明细表产生的质量评价。
            MapDtls dtls = this.HisNode.MapData.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.ItIsHLDtl == false)
                    continue;

                //获取明细数据。
                GEDtls gedtls = new GEDtls(dtl.No);
                QueryObject qo = null;
                qo = new QueryObject(gedtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForFID:
                        qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                        break;
                }
                qo.DoQuery();

                foreach (GEDtl gedtl in gedtls)
                {
                    eval = new Eval();
                    eval.setMyPK(gedtl.OID + "_" + gedtl.Rec);
                    eval.Delete();
                }
            }
        }
        /// <summary>
        /// 处理质量考核
        /// </summary>
        public void DealEval()
        {
            if (this.HisNode.ItIsEval == false)
                return;

            BP.WF.Data.Eval eval = new Eval();

            if (this.HisNode.ItIsFLHL == false)
            {
                eval.setMyPK(this.WorkID + "_" + this.HisNode.NodeID);
                eval.Delete();

                eval.Title = this.HisGenerWorkFlow.Title;

                eval.WorkID = this.WorkID;
                eval.NodeID = this.HisNode.NodeID;
                eval.NodeName = this.HisNode.Name;

                eval.FlowNo = this.HisNode.FlowNo;
                eval.FlowName = this.HisNode.FlowName;

                eval.DeptNo = this.ExecerDeptNo;
                eval.DeptName = this.ExecerDeptName;

                eval.Rec = this.Execer;
                eval.RecName = this.ExecerName;

                eval.RDT = DataType.CurrentDateTime;
                eval.NY = DataType.CurrentYearMonth;

                eval.EvalEmpNo = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalEmpNo);
                eval.EvalEmpName = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalEmpName);
                eval.EvalCent = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalCent);
                eval.EvalNote = this.HisWork.GetValStringByKey(WorkSysFieldAttr.EvalNote);

                eval.Insert();
                return;
            }

            // 分合流的情况，它是明细表产生的质量评价。
            Sys.MapDtls dtls = this.HisNode.MapData.MapDtls;
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.ItIsHLDtl == false)
                    continue;

                //获取明细数据。
                GEDtls gedtls = new GEDtls(dtl.No);
                QueryObject qo = null;
                qo = new QueryObject(gedtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, this.WorkID);
                        break;
                    case DtlOpenType.ForFID:
                        qo.AddWhere(GEDtlAttr.FID, this.WorkID);
                        break;
                }
                qo.DoQuery();

                foreach (GEDtl gedtl in gedtls)
                {
                    eval = new Eval();
                    eval.setMyPK(gedtl.OID + "_" + gedtl.Rec);
                    eval.Delete();

                    eval.Title = this.HisGenerWorkFlow.Title;

                    eval.WorkID = this.WorkID;
                    eval.NodeID = this.HisNode.NodeID;
                    eval.NodeName = this.HisNode.Name;

                    eval.FlowNo = this.HisNode.FlowNo;
                    eval.FlowName = this.HisNode.FlowName;

                    eval.DeptNo = this.ExecerDeptNo;
                    eval.DeptName = this.ExecerDeptName;

                    eval.Rec = this.Execer;
                    eval.RecName = this.ExecerName;

                    eval.RDT = DataType.CurrentDateTime;
                    eval.NY = DataType.CurrentYearMonth;

                    eval.EvalEmpNo = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalEmpNo);
                    eval.EvalEmpName = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalEmpName);
                    eval.EvalCent = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalCent);
                    eval.EvalNote = gedtl.GetValStringByKey(WorkSysFieldAttr.EvalNote);
                    eval.Insert();
                }
            }
        }

        #endregion


        /// <summary>
        /// 工作流发送业务处理
        /// </summary>
        public SendReturnObjs NodeSend()
        {
            SendReturnObjs sendObj = NodeSend(null, null, false);
            return sendObj;
        }

        /// <summary>
        /// 1变N,用于分流节点，向子线程copy数据。
        /// </summary>
        /// <returns></returns>
        public void CheckFrm1ToN()
        {
            //只有分流，合流才能执行1ToN.
            if (this.HisNode.HisRunModel == RunModel.Ordinary
                || this.HisNode.HisRunModel == RunModel.HL
                || this.HisNode.ItIsSubThread == true)
            {
                return;
            }

            //初始化变量.
            if (frmNDs == null)
                frmNDs = new FrmNodes(this.HisNode.FlowNo, this.HisNode.NodeID);

            foreach (FrmNode fn in frmNDs)
            {
                if (fn.ItIs1ToN == false)
                    continue;

                #region 获得实体主键.
                // 处理主键.
                long pk = 0;// this.WorkID;
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        if (this.rptGe == null)
                            this.rptGe = new GERpt("ND" + int.Parse(this.HisFlow.No) + "Rpt", this.WorkID);
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception(BP.WF.Glo.multilingual("@未判断的类型:{0}.", "WorkNode", "not_found_value", fn.WhoIsPK.ToString()));
                }

                if (pk == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@未能获取表单主键.", "WorkNode", "not_found_form_primary_key"));
                #endregion 获得实体主键.

                //初始化这个实体.
                GEEntity geEn = new GEEntity(fn.FK_Frm, pk);

                //首先删除垃圾数据.
                geEn.Delete("FID", this.WorkID);

                //循环子线程，然后插入数据.
                foreach (GenerWorkerList item in current_gwls)
                {
                    geEn.PKVal = item.WorkID; //子线程的WorkID作为.
                    geEn.SetValByKey("FID", this.WorkID);

                    #region 处理默认变量.
                    //foreach (Attr attr in geEn.EnMap.Attrs)
                    //{
                    //    if (attr.DefaultValOfReal == "@RDT")
                    //    {
                    //        geEn.SetValByKey(attr.Key, DataType.CurrentDateTime);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.No")
                    //    {
                    //        geEn.SetValByKey(attr.Key, item.FK_Emp);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.Name")
                    //    {
                    //        geEn.SetValByKey(attr.Key, item.EmpNam);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.DeptNo")
                    //    {
                    //        Emp emp = new Emp(item.EmpNo);
                    //        geEn.SetValByKey(attr.Key, emp.DeptNo);
                    //        continue;
                    //    }

                    //    if (attr.DefaultValOfReal == "@WebUser.DeptName")
                    //    {
                    //        Emp emp = new Emp(item.EmpNo);
                    //        geEn.SetValByKey(attr.Key, emp.DeptText);
                    //        continue;
                    //    }
                    //}
                    #endregion 处理默认变量.

                    geEn.DirectInsert();
                }
            }
        }
        /// <summary>
        /// 当前节点的-表单绑定.
        /// </summary>
        private BP.WF.Template.FrmNodes frmNDs = null;
        /// <summary>
        /// 汇总子线程的表单到合流节点上去
        /// </summary>
        /// <returns></returns>
        public void CheckFrmHuiZongToDtl()
        {
            //只有分流，合流才能执行1ToN.
            if (this.HisNode.ItIsSubThread == false)
                return;

            //初始化变量.
            if (frmNDs == null)
                frmNDs = new FrmNodes(this.HisNode.FlowNo, this.HisNode.NodeID);

            foreach (FrmNode fn in frmNDs)
            {
                //如果该表单不需要汇总，就不处理他.
                if (fn.HuiZong == "0" || fn.HuiZong == "")
                    continue;

                #region 获得实体主键.
                // 处理主键.
                long pk = 0;// this.WorkID;
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception(BP.WF.Glo.multilingual("@未判断的类型:{0}.", "WorkNode", "not_found_value", fn.WhoIsPK.ToString()));
                }

                if (pk == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@未能获取表单主键.", "WorkNode", "not_found_form_primary_key"));
                #endregion 获得实体主键.


                //初始化这个实体,获得这个实体的数据.
                GEEntity rpt = new GEEntity(fn.FK_Frm, pk);
                //
                string[] strs = fn.HuiZong.Trim().Split('@');

                //实例化这个数据.
                MapDtl dtl = new MapDtl(strs[1].ToString());

                //把数据汇总到指定的表里.
                GEDtl dtlEn = dtl.HisGEDtl;
                dtlEn.OID = (int)this.WorkID;
                int i = dtlEn.RetrieveFromDBSources();
                dtlEn.Copy(rpt);

                dtlEn.OID = (int)this.WorkID;
                dtlEn.RDT = DataType.CurrentDateTime;
                dtlEn.Rec = BP.Web.WebUser.No;

                dtlEn.RefPK = this.HisWork.FID.ToString();
                dtlEn.FID = 0;

                if (i == 0)
                    dtlEn.SaveAsOID((int)this.WorkID);
                else
                    dtlEn.Update();
            }
        }
        /// <summary>
        /// 检查是否填写审核意见
        /// </summary>
        /// <returns></returns>
        private bool CheckFrmIsFullCheckNote()
        {
            //检查是否写入了审核意见.
            if (this.HisNode.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
            {
                /*检查审核意见 */
                string sql = "SELECT Msg \"Msg\",EmpToT \"EmpToT\" FROM ND" + int.Parse(this.HisNode.FlowNo) + "Track WHERE  EmpFrom='" + WebUser.No + "' AND NDFrom=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID + " AND ActionType=" + (int)ActionType.WorkCheck;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count <= 0)
                    throw new Exception("err@请为[" + this.HisNode.Name + "]填写审核意见.");

                if (DataType.IsNullOrEmpty(dt.Rows[0][0].ToString()) == true)
                    throw new Exception("err@节点[" + this.HisNode.Name + "]审核意见不能为空.");
            }
            return true;
        }
        /// <summary>
        /// 检查独立表单上必须填写的项目.
        /// </summary>
        /// <returns></returns>
        public bool CheckFrmIsNotNull()
        {
            //if (this.HisNode.HisFormType != NodeFormType.SheetTree)
            //    return true;
            //判断绑定的树形表单
            //增加节点表单的必填项判断.
            string err = "";
            if (this.HisNode.HisFormType == NodeFormType.SheetTree)
            {
                //获取绑定的表单.
                string frms = this.HisGenerWorkFlow.Paras_Frms;
                FrmNodes nds = null;
                if (DataType.IsNullOrEmpty(frms) == false)
                {
                    //设置前置导航，选择表单的操作
                    frms = "'" + frms.Replace(",", "','") + "'";
                    nds = new FrmNodes();
                    QueryObject qury = new QueryObject(nds);
                    qury.AddWhere(FrmNodeAttr.FK_Flow, this.HisNode.FlowNo);
                    qury.addAnd();
                    qury.AddWhere(FrmNodeAttr.FK_Node, this.HisNode.NodeID);
                    qury.addAnd();
                    qury.AddWhere(FrmNodeAttr.FK_Frm, "In", "(" + frms + ")");
                    qury.addOrderBy(FrmNodeAttr.Idx);
                    qury.DoQuery();

                }
                else
                {
                    nds = new FrmNodes(this.HisNode.FlowNo, this.HisNode.NodeID);
                }

                foreach (FrmNode item in nds)
                {
                    if (item.FrmEnableRole == FrmEnableRole.Disable)
                        continue;

                    if (item.HisFrmType != FrmType.FoolForm && item.HisFrmType != FrmType.Develop)
                        continue;

                    if (item.FrmSln == FrmSln.Readonly)
                        continue;

                    MapData md = new MapData();
                    md.No = item.FK_Frm;
                    md.Retrieve();
                    if (md.HisFrmType != FrmType.FoolForm && md.HisFrmType != FrmType.Develop)
                        continue;

                    //判断WhoIsPK
                    long pkVal = this.WorkID;
                    if (item.WhoIsPK == WhoIsPK.FID)
                        pkVal = this.HisGenerWorkFlow.FID;
                    if (item.WhoIsPK == WhoIsPK.PWorkID)
                        pkVal = this.HisGenerWorkFlow.PWorkID;
                    if (item.WhoIsPK == WhoIsPK.P2WorkID)
                    {
                        GenerWorkFlow gwf = new GenerWorkFlow(this.HisGenerWorkFlow.PWorkID);
                        if (gwf != null && gwf.PWorkID != 0)
                            pkVal = gwf.PWorkID;
                    }
                    if (item.WhoIsPK == WhoIsPK.P3WorkID)
                    {
                        string sql = "SELECT PWorkID FROM WF_GenerWorkFlow Where WorkID=(SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.HisGenerWorkFlow.PWorkID + ")";
                        pkVal = DBAccess.RunSQLReturnValInt(sql, 0);
                    }


                    MapAttrs mapAttrs = md.MapAttrs;
                    //主表实体.
                    GEEntity en = new GEEntity(item.FK_Frm);
                    en.OID = pkVal;
                    int i = en.RetrieveFromDBSources();
                    if (i == 0)
                        continue;

                    Row row = en.Row;
                    if (item.FrmSln == FrmSln.Self)
                    {
                        // 查询出来自定义的数据.
                        FrmFields ffs1 = new FrmFields();
                        ffs1.Retrieve(FrmFieldAttr.FK_Node, this.HisNode.NodeID, FrmFieldAttr.FrmID, md.No);
                        //获取整合后的mapAttrs
                        foreach (FrmField frmField in ffs1)
                        {
                            foreach (MapAttr mapAttr in mapAttrs)
                            {
                                if (frmField.KeyOfEn.Equals(mapAttr.KeyOfEn))
                                {
                                    mapAttr.UIIsInput = frmField.ItIsNotNull;
                                    break;
                                }
                            }
                        }
                    }

                    //string frmErr = "";
                    foreach (MapAttr mapAttr in mapAttrs)
                    {
                        if (mapAttr.UIIsInput == false)
                            continue;

                        string str = row[mapAttr.KeyOfEn] == null ? string.Empty : row[mapAttr.KeyOfEn].ToString();
                        /*如果是检查不能为空 */
                        if (str == null || DataType.IsNullOrEmpty(str) == true || str.Trim() == "")
                            err += BP.WF.Glo.multilingual("@表单【{0}】字段{1},【{2}】不能为空.", "WorkNode", "form_field_must_not_be_null_1", md.Name, mapAttr.KeyOfEn, mapAttr.Name);
                    }
                    //  if (DataType.IsNullOrEmpty(frmErr)==false)
                    //    err+=" @表单："+md.Name
                }

                if (!err.Equals(""))
                    throw new Exception(BP.WF.Glo.multilingual("err@提交前检查到如下必填字段填写不完整:{0}.", "WorkNode", "detected_error", err));

                return true;
            }

            if (this.HisNode.ItIsNodeFrm == true)
            {
                MapAttrs attrs = this.HisNode.MapData.MapAttrs;
                Row row = this.HisWork.Row;
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIIsInput == false)
                        continue;

                    object val = row[attr.KeyOfEn];
                    string str = null;
                    if (val != null)
                        str = val.ToString();


                    /*如果是检查不能为空 */
                    if (DataType.IsNullOrEmpty(str) == true)
                        err += BP.WF.Glo.multilingual("@字段{0},{1}不能为空.", "WorkNode", "form_field_must_not_be_null_2", attr.KeyOfEn, attr.Name);
                }

                #region 检查附件个数的完整性. - 该部分代码稳定后，移动到独立表单的检查上去。
                foreach (FrmAttachment ath in this.HisWork.HisFrmAttachments)
                {
                    #region 增加阅读规则. @祝梦娟.
                    if (ath.ReadRole != 0)
                    {
                        //查询出来当前的数据.
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                            GenerWorkerListAttr.FK_Emp, WebUser.No, GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

                        //获得已经下载或者读取的数据. 格式为: a2e06fbf-2bae-44fb-9176-9a0047751e83,a2e06fbf-we-44fb-9176-9a0047751e83
                        string ids = gwl.GetParaString(ath.NoOfObj);
                        if (ids.Contains("ALL") == false)
                        {
                            //获得当前节点的上传附件.
                            FrmAttachmentDBs dbs = BP.WF.CCFormAPI.GenerFrmAttachmentDBs(ath, this.WorkID.ToString(), ath.MyPK, this.WorkID, 0, 0, false);

                            //string sql = "SELECT MyPK,FileName FROM Sys_FrmAttachmentDB WHERE RefPKVal=" + this.WorkID + " AND FK_FrmAttachment='" + ath.MyPK + "' AND Rec!='" + BP.Web.WebUser.No + "'";
                            //DataTable dt = DBAccess.RunSQLReturnTable(sql);
                            string errFileUnRead = "";
                            foreach (FrmAttachmentDB db in dbs)
                            {
                                string guid = db.MyPK;
                                if (ids.Contains(guid) == false)
                                    errFileUnRead += BP.WF.Glo.multilingual("@文件({0})未阅读.", "WorkNode", "document_not_read", db.FileName);

                            }

                            //如果有未阅读的文件.
                            if (DataType.IsNullOrEmpty(errFileUnRead) == false)
                            {
                                //未阅读不让其发送.
                                if (ath.ReadRole == 1)
                                    throw new Exception("err" + BP.WF.Glo.multilingual("@您还有如下文件没有阅读:{0}.", "WorkNode", "you_have_document_not_read", errFileUnRead));

                                //未阅读记录日志并让其发送.
                                if (ath.ReadRole == 2)
                                {
                                    //AthUnReadLog log = new AthUnReadLog();
                                    //log.setMyPK(this.WorkID + "_" + this.HisNode.NodeID + "_" + WebUser.No);
                                    //log.Delete();

                                    //log.FK_Emp = WebUser.No;
                                    //log.FK_EmpDept = WebUser.DeptNo;
                                    //log.FK_EmpDeptName = WebUser.DeptName;
                                    //log.FlowNo = this.HisNode.FlowNo;
                                    //log.FlowName = this.HisFlow.Name;

                                    //log.NodeID = this.HisNode.NodeID;
                                    //log.FlowName = this.HisFlow.Name;
                                    //log.SendDT = DataType.CurrentDateTime;
                                    //log.WorkID = this.WorkID;

                                    //log.Insert(); //插入到数据库.

                                }
                            }
                        }
                    }
                    #endregion 增加阅读规则.

                    if (ath.UploadFileNumCheck == UploadFileNumCheck.None)
                        continue;

                    Int64 pkval = this.WorkID;
                    if (ath.HisCtrlWay == AthCtrlWay.FID)
                        pkval = this.HisGenerWorkFlow.FID;
                    if (ath.HisCtrlWay == AthCtrlWay.PWorkID)
                        pkval = this.HisGenerWorkFlow.PWorkID;
                    if (ath.HisCtrlWay == AthCtrlWay.PWorkID)
                        pkval = DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.HisGenerWorkFlow.PWorkID, 0);
                    if (ath.HisCtrlWay == AthCtrlWay.P3WorkID)
                        pkval = DBAccess.RunSQLReturnValInt("Select PWorkID From WF_GenerWorkFlow Where WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + this.HisGenerWorkFlow.PWorkID + ")", 0);

                    if (ath.UploadFileNumCheck == UploadFileNumCheck.NotEmpty)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT COUNT(MyPK) as Num FROM Sys_FrmAttachmentDB WHERE NoOfObj=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "NoOfObj AND RefPKVal=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "RefPKVal";
                        ps.Add("NoOfObj", ath.NoOfObj);
                        ps.Add("RefPKVal", pkval);
                        int count = DBAccess.RunSQLReturnValInt(ps);
                        if (count == 0)
                            err += BP.WF.Glo.multilingual("@您没有上传附件:{0}.", "WorkNode", "not_upload_attachment", ath.Name);

                        if (ath.NumOfUpload > count)
                            err += BP.WF.Glo.multilingual("@您上传的附件数量小于最低上传数量要求.", "WorkNode", "attachment_less_than_required");
                    }

                    if (ath.UploadFileNumCheck == UploadFileNumCheck.EverySortNoteEmpty)
                    {


                        Paras ps = new Paras();
                        ps.SQL = "SELECT COUNT(MyPK) as Num, Sort FROM Sys_FrmAttachmentDB WHERE  NoOfObj=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "NoOfObj AND RefPKVal=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "RefPKVal Group BY Sort";
                        ps.Add("NoOfObj", ath.NoOfObj);
                        ps.Add("RefPKVal", pkval);

                        DataTable dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 0)
                            err += BP.WF.Glo.multilingual("@您没有上传附件:{0}.", "WorkNode", "not_upload_attachment", ath.Name);


                        string sort = ath.Sort.Replace(";", ",");
                        string[] strs = sort.Split(',');
                        foreach (string str in strs)
                        {
                            bool isHave = false;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr[1].ToString() == str)
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                                err += BP.WF.Glo.multilingual("@您没有上传附件:{0}.", "WorkNode", "not_upload_attachment", str);
                        }
                    }
                }
                #endregion 检查附件个数的完整性.


                #region 检查图片附件的必填，added by liuxc,2016-11-1
                foreach (FrmImgAth imgAth in this.HisNode.MapData.FrmImgAths)
                {
                    if (!imgAth.ItIsRequired)
                        continue;

                    Paras ps = new Paras();
                    ps.SQL = "SELECT COUNT(MyPK) as Num FROM Sys_FrmImgAthDB WHERE FK_MapData=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_MapData AND FK_FrmImgAth=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_FrmImgAth AND RefPKVal=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "RefPKVal";
                    ps.Add("FK_MapData", "ND" + this.HisNode.NodeID);
                    ps.Add("FK_FrmImgAth", imgAth.MyPK);
                    ps.Add("RefPKVal", this.WorkID);
                    if (DBAccess.RunSQLReturnValInt(ps) == 0)
                        err += BP.WF.Glo.multilingual("@您没有上传图片附件:{0}.", "WorkNode", "not_upload_attachment", imgAth.CtrlID.ToString());

                }
                #endregion 检查图片附件的必填，added by liuxc,2016-11-1

                if (err != "")
                    throw new Exception(BP.WF.Glo.multilingual("err@提交前检查到如下必填字段填写不完整:{0}.", "WorkNode", "detected_error", err));

                CheckFrmIsFullCheckNote();
            }

            //查询出来所有的设置。
            FrmFields ffs = new FrmFields();

            QueryObject qo = new QueryObject(ffs);
            qo.AddWhere(FrmFieldAttr.FK_Node, this.HisNode.NodeID);
            qo.addAnd();
            qo.AddWhere(FrmFieldAttr.IsNotNull, 1);
            qo.DoQuery();

            if (ffs.Count == 0)
                return true;

            BP.WF.Template.FrmNodes frmNDs = new FrmNodes(this.HisNode.FlowNo, this.HisNode.NodeID);
            err = "";
            foreach (FrmNode item in frmNDs)
            {
                MapData md = new MapData(item.FK_Frm);

                //可能是url.
                if (md.HisFrmType == FrmType.Url)
                    continue;

                //如果使用默认方案,就return出去.
                if (item.FrmSln == 0)
                    continue;

                //检查是否有？
                bool isHave = false;
                foreach (FrmField myff in ffs)
                {
                    if (myff.FrmID != item.FK_Frm)
                        continue;
                    isHave = true;
                    break;
                }
                if (isHave == false)
                    continue;

                // 处理主键.
                long pk = 0;// this.WorkID;

                switch (item.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        pk = this.rptGe.PWorkID;
                        break;
                    default:
                        throw new Exception(BP.WF.Glo.multilingual("@未判断的类型:{0}.", "WorkNode", "not_found_value", item.WhoIsPK.ToString()));
                }

                if (pk == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@未能获取表单主键.", "WorkNode", "not_found_form_primary_key"));

                //获取表单值
                GEEntity en = new GEEntity(md.No);
                en.OID = pk;
                if (en.RetrieveFromDBSources() == 0)
                {
                    err += BP.WF.Glo.multilingual("@表单{0}没有输入数据.", "WorkNode", "not_found_value", md.Name);
                    continue;
                }
                //循环判断数据是否在Work中存在
                foreach(string keyOfEn in en.Row.Keys)
                {
                    if (this.HisWork.Row.Contains(keyOfEn) == true)
                        this.HisWork.SetValByKey(keyOfEn, en.GetValStringByKey(keyOfEn));
                }
                // 检查数据是否完整.
                foreach (FrmField ff in ffs)
                {
                    if (ff.FrmID != item.FK_Frm)
                        continue;

                    //获得数据.
                    string val = string.Empty;
                    val = en.GetValStringByKey(ff.KeyOfEn); //dt.Rows[0][ff.KeyOfEn].ToString();

                    if (ff.ItIsNotNull == true)
                    {
                        /*如果是检查不能为空 */
                        if (DataType.IsNullOrEmpty(val) == true || val.Trim() == "")
                            err += BP.WF.Glo.multilingual("@表单{0}字段{1},{2}不能为空.", "WorkNode", "form_field_must_not_be_null_1", md.Name, ff.KeyOfEn, ff.Name);

                    }

                    ////判断是否需要写入流程数据表.
                    //if (ff.ItIsWriteToFlowTable == true)
                    //{
                    //    this.HisWork.SetValByKey(ff.KeyOfEn, val);
                    //    //this.rptGe.SetValByKey(ff.KeyOfEn, val);
                    //}
                }
            }
            if (err != "")
                throw new Exception(BP.WF.Glo.multilingual("@提交前检查到如下必填字段填写不完整({0}).", "WorkNode", "not_found_value", err));

            return true;
        }
        /// <summary>
        /// copy表单树的数据
        /// </summary>
        /// <returns></returns>
        public Work CopySheetTree()
        {
            if (this.HisNode.HisFormType != NodeFormType.SheetTree && this.HisNode.HisFormType != NodeFormType.RefOneFrmTree)
                return null;

            //查询出来所有的设置。
            FrmFields ffs = new FrmFields();
            QueryObject qo = new QueryObject(ffs);
            qo.AddWhere(FrmFieldAttr.FK_Node, this.HisNode.NodeID);
            qo.DoQuery();
            if (ffs.Count == 0)
                return null;
            BP.WF.Template.FrmNodes frmNDs = new FrmNodes(this.HisNode.FlowNo, this.HisNode.NodeID);
            string err = "";
            foreach (FrmNode item in frmNDs)
            {
                MapData md = new MapData(item.FK_Frm);

                //可能是url.
                if (md.HisFrmType == FrmType.Url)
                    continue;

                //检查是否有？
                bool isHave = false;
                foreach (FrmField myff in ffs)
                {
                    if (myff.FrmID != item.FK_Frm)
                        continue;
                    isHave = true;
                    break;
                }

                if (isHave == false)
                    continue;

                // 处理主键.
                long pk = 0;// this.WorkID;

                switch (item.WhoIsPK)
                {
                    case WhoIsPK.FID:
                        pk = this.HisWork.FID;
                        break;
                    case WhoIsPK.OID:
                        pk = this.HisWork.OID;
                        break;
                    case WhoIsPK.PWorkID:
                        if (this.rptGe == null)
                            this.rptGe = new GERpt("ND" + int.Parse(this.HisFlow.No) + "Rpt", this.WorkID);
                        pk = this.rptGe.PWorkID;
                        break;
                    case WhoIsPK.P2WorkID:
                        //获取P2WorkID
                        GenerWorkFlow gwf = new GenerWorkFlow(this.HisGenerWorkFlow.PWorkID);
                        if (gwf != null && gwf.PWorkID != 0)
                            pk = gwf.PWorkID;
                        break;
                    case WhoIsPK.P3WorkID:
                        string sql = "Select PWorkID From WF_GenerWorkFlow Where WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + this.HisGenerWorkFlow.PWorkID + ")";
                        pk = DBAccess.RunSQLReturnValInt(sql, 0);


                        break;
                    default:
                        throw new Exception(BP.WF.Glo.multilingual("@未判断的类型:{0}.", "WorkNode", "not_found_value", item.WhoIsPK.ToString()));
                }

                if (pk == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@未能获取表单主键.", "WorkNode", "not_found_form_primary_key"));

                //获取表单值
                ps = new Paras();
                ps.SQL = "SELECT * FROM " + md.PTable + " WHERE OID=" + ps.DBStr + "OID";
                ps.Add(WorkAttr.OID, pk);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    continue;

                // 检查数据是否完整.
                foreach (FrmField ff in ffs)
                {
                    if (ff.FrmID != item.FK_Frm)
                        continue;

                    if (dt.Columns.Contains(ff.KeyOfEn) == false)
                        continue;

                    //获得数据.
                    string val = string.Empty;
                    val = dt.Rows[0][ff.KeyOfEn].ToString();
                    this.HisWork.SetValByKey(ff.KeyOfEn, val);
                }
            }

            return this.HisWork;
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        public void DoCC()
        {
        }
        /// <summary>
        /// 通知主持人
        /// </summary>
        /// <returns></returns>
        private string DealAlertZhuChiRen(string huiQianZhuChiRen)
        {
            /*有两个待办，就说明当前人员是最后一个会签人，就要把主持人的状态设置为 0 */
            //获得主持人信息.
            GenerWorkerList gwl = new GenerWorkerList();
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Emp, huiQianZhuChiRen, GenerWorkerListAttr.IsPass, 90);
            if (i != 1)
                return BP.WF.Glo.multilingual("@您已经会签完毕.", "WorkNode", "you_have_finished");

            gwl.PassInt = 0; //从会签列表里移动到待办.
            gwl.ItIsRead = false; //设置为未读.

            string str1 = BP.WF.Glo.multilingual("@工作会签完毕.", "WorkNode", "you_have_finished");
            string str2 = BP.WF.Glo.multilingual("@{0}工作已经完成,请到待办列表查看.", "WorkNode", "you_have_finished_todo", this.HisGenerWorkFlow.Title);
            BP.WF.Dev2Interface.Port_SendMsg(gwl.EmpNo,
               str1, str2,
                "HuiQian" + this.WorkID + "_" + WebUser.No, "HuiQian", HisGenerWorkFlow.FlowNo, this.HisGenerWorkFlow.NodeID, this.WorkID, 0);

            //设置为未读.
            BP.WF.Dev2Interface.Node_SetWorkUnRead(this.HisGenerWorkFlow.WorkID);

            //设置最后处理人.
            this.HisGenerWorkFlow.TodoEmps = gwl.EmpNo + "," + gwl.EmpName + ";";
            this.HisGenerWorkFlow.Update();

            #region 处理天业集团对主持人的考核.
            /*
             * 对于会签人的时间计算
             * 1, 从主持人接收工作时间点起，到最后一个一次分配会签人止，作为第一时间段。
             * 2，所有会签人会签完毕后到会签人执行发送时间点止作为第2个时间段。
             * 3，第1个时间端+第2个时间段为主持人所处理该工作的时间，时效考核的内容按照这个两个时间段开始计算。
             */
            if (this.HisNode.HisCHWay == CHWay.ByTime)
            {
                /*如果是按照时效考核.*/

                //获得最后一次执行会签的时间点.
                string sql = "SELECT RDT FROM ND" + int.Parse(this.HisNode.FlowNo) + "TRACK WHERE WorkID=" + this.WorkID + " AND ActionType=30 ORDER BY RDT";
                string lastDTOfHuiQian = DBAccess.RunSQLReturnStringIsNull(sql, null);

                //取出来下达给主持人的时间点.
                string dtOfToZhuChiRen = gwl.RDT;

                //获得两个时间间隔.
                DateTime t_lastDTOfHuiQian = DataType.ParseSysDate2DateTime(lastDTOfHuiQian);
                DateTime t_dtOfToZhuChiRen = DataType.ParseSysDate2DateTime(dtOfToZhuChiRen);

                TimeSpan ts = t_lastDTOfHuiQian - t_dtOfToZhuChiRen;

                //生成该节点设定的 时间范围.
                int hour = this.HisNode.TimeLimit * 24 + this.HisNode.TimeLimitHH;
                // int.Parse(this.HisNode.TSpanHour.ToString());
                TimeSpan tsLimt = new TimeSpan(hour, this.HisNode.TimeLimitMM, 0);

                //获得剩余的时间范围.
                TimeSpan myLeftTS = tsLimt - ts;

                //计算应该完成的日期.
                DateTime dtNow = DateTime.Now;
                dtNow = dtNow.AddHours(myLeftTS.TotalHours);

                //设置应该按成的日期.
                if (this.HisNode.HisCHWay == CHWay.None)
                    gwl.SDT = "无";
                else
                    gwl.SDT = DataType.SysDateTimeFormat(dtNow);

                //设置预警日期, 为了方便提前1天预警.
                dtNow = dtNow.AddDays(-1);
                gwl.DTOfWarning = DataType.SysDateTimeFormat(dtNow);
            }
            #endregion 处理天业集团对会签人的考核.

            gwl.Update();

            return BP.WF.Glo.multilingual("您是最后一个会签该工作的处理人，已经提醒主持人({0}, {1})处理当前工作.", "WorkNode", "you_are_the_last_operator", gwl.EmpNo, gwl.EmpName);
        }
        /// <summary>
        /// 如果是协作.
        /// </summary>
        /// <returns>是否执行到最后一个人？</returns>
        public bool DealTeamUpNode()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人,就没有顺序的问题.*/

            //查看是否我是最后一个？
            int num = 0;
            string todoEmps = ""; //记录没有处理的人.
            string todoNos = "";
            foreach (GenerWorkerList item in gwls)
            {
                if (item.PassInt == 0 || item.PassInt == 90)
                {
                    if (item.EmpNo.Equals(WebUser.No) == false)
                    {
                        todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + " ";
                        todoNos += item.EmpNo + ";";
                    }

                    num++;
                }
            }

            if (num == 1)
            {
                if (this.HisGenerWorkFlow.HuiQianTaskSta == HuiQianTaskSta.None)
                {
                    this.HisGenerWorkFlow.Sender = WebUser.No + "," + WebUser.Name + ";";
                    this.HisGenerWorkFlow.TodoEmpsNum = 1;
                    this.HisGenerWorkFlow.TodoEmps = WebUser.Name + ";";
                }
                else
                {
                    string huiqianNo = this.HisGenerWorkFlow.HuiQianZhuChiRen;
                    string huiqianName = this.HisGenerWorkFlow.HuiQianZhuChiRenName;

                    this.HisGenerWorkFlow.Sender = huiqianNo + "," + huiqianName + ";";
                    this.HisGenerWorkFlow.TodoEmpsNum = 1;
                    this.HisGenerWorkFlow.TodoEmps = WebUser.Name + ";";
                    this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                }

                return false; /*只有一个待办,说明自己就是最后的一个人.*/
            }

            //把当前的待办设置已办，并且提示未处理的人。
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.EmpNo.Equals(WebUser.No) == false)
                    continue;

                //设置当前不可以用.
                gwl.PassInt = 1;
                gwl.Update();

                // 检查完成条件。
                if (this.HisNode.ItIsEndNode == false)
                    this.CheckCompleteCondition();

                //写入日志.
                if (this.HisGenerWorkFlow.HuiQianTaskSta != HuiQianTaskSta.None)
                    this.AddToTrack(ActionType.TeampUp, todoNos, todoEmps, this.HisNode.NodeID, this.HisNode.Name, BP.WF.Glo.multilingual("会签", "WorkNode", "cross_signing"));
                else
                    this.AddToTrack(ActionType.TeampUp, todoNos, todoEmps, this.HisNode.NodeID, this.HisNode.Name, BP.WF.Glo.multilingual("协作发送", "WorkNode", "cross_signing"));

                //替换人员信息.
                string emps = this.HisGenerWorkFlow.TodoEmps;

                emps = emps.Replace(WebUser.No + "," + WebUser.Name + ";", "");
                emps = emps.Replace(WebUser.No + "," + WebUser.Name, "");

                this.HisGenerWorkFlow.TodoEmps = emps;

                //处理会签问题
                this.addMsg(SendReturnMsgFlag.OverCurr, BP.WF.Glo.multilingual("@您已经完成签完工作. 当前未处理会签工作的人还有:{0}.", "WorkNode", "you_have_finished_1", todoEmps), null, SendReturnMsgType.Info);

                return true;
            }

            throw new Exception("@不应该运行到这里，DealTeamUpNode。当前登录人员[" + WebUser.No + "," + WebUser.Name + "],请确认人员信息.WorkID=" + this.WorkID);
        }
        /// <summary>
        /// 如果是协作
        /// </summary>
        public bool DealTeamupGroupLeader()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.IsPass);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人,就没有顺序的问题.*/

            #region  判断自己是否是组长？如果是组长，就让返回false, 让其运动到最后一个节点，因为组长同意了，就全部同意了。
            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.ByDeptFieldLeader)
            {
                string sql = "SELECT COUNT(No) AS num FROM Port_Dept WHERE Leader='" + WebUser.No + "'";
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 1)
                    return false;
            }

            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.BySQL)
            {
                string sql = this.HisNode.TeamLeaderConfirmDoc;
                sql = Glo.DealExp(sql, this.HisWork);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("@WorkID", this.WorkID.ToString());
                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                string userNo = WebUser.No;
                foreach (DataRow dr in dt.Rows)
                {
                    string str = dr[0] as string;
                    if (str == userNo)
                        return false;
                }
                //获取未处理的待办人员
                string todoEmpNo = "";
                string todoEmpName = "";
                foreach (GenerWorkerList gwl in gwls)
                {
                    if (gwl.EmpNo.Equals(WebUser.No) == false)
                        continue;
                    if ((gwl.PassInt == 0 || gwl.PassInt == 90) && (gwl.EmpNo.Equals(WebUser.No) == false))
                    {
                        todoEmpName += BP.WF.Glo.DealUserInfoShowModel(gwl.EmpNo, gwl.EmpName) + ";";
                        todoEmpNo += gwl.EmpNo + ";";
                    }
                }
                //把当前的待办设置已办
                foreach (GenerWorkerList gwl in gwls)
                {
                    if (gwl.EmpNo.Equals(WebUser.No) == false)
                        continue;

                    //设置当前已经完成.
                    gwl.PassInt = 1;
                    gwl.Update();

                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs, null);
                    this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                    //执行时效考核.
                    if (this.rptGe == null)
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title, gwl);
                    else
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title, gwl);

                    this.AddToTrack(ActionType.TeampUp, todoEmpNo, todoEmpName, this.HisNode.NodeID, this.HisNode.Name, "多人处理规则：协作组长模式");
                }
                this.addMsg(SendReturnMsgFlag.CondInfo, BP.WF.Glo.multilingual("@当前工作未处理的人还有: {0},所以不能发送到下一步.", "WorkNode", "you_have_finished_1", todoEmpName), null, SendReturnMsgType.Info);
                return true;
            }

            if (this.HisNode.TeamLeaderConfirmRole == TeamLeaderConfirmRole.HuiQianLeader)
            {
                //当前人员的流程处理信息
                GenerWorkerList gwlOfMe = new GenerWorkerList();
                gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                            GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);
                string myhqzcr = gwlOfMe.GetParaString("HuiQianZhuChiRen");
                string myhqType = gwlOfMe.GetParaString("HuiQianType");
                myhqType = DataType.IsNullOrEmpty(myhqType) == true ? "" : myhqType;

                //只有一个组长的模式
                if (this.HisNode.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne)
                {
                    /* 当前人是组长，检查是否可以可以发送,检查自己是否是最后一个人 ？ */
                    if (this.HisGenerWorkFlow.TodoEmps.Contains(WebUser.No + ",") == true && DataType.IsNullOrEmpty(myhqzcr) == true)
                    {
                        String todoEmps = ""; // 记录没有处理的人.
                        int num = 0;
                        foreach (GenerWorkerList item in gwls)
                        {
                            if (item.PassInt == 0 || item.PassInt == 90)
                            {
                                if (item.EmpNo.Equals(WebUser.No) == false)
                                    todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + " ";
                                num++;
                            }
                        }

                        if (num == 1)
                        {
                            this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name) + ";";
                            this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                            this.HisGenerWorkFlow.HuiQianZhuChiRen = "";
                            this.HisGenerWorkFlow.HuiQianZhuChiRenName = "";
                            return false; /* 只有一个待办,说明自己就是最后的一个人. */
                        }

                        this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todoEmps + ",您不能执行发送.", null,
                                SendReturnMsgType.Info);
                        return true;
                    }
                }
                //任意组长都可以发发送，只要该组长加签的人已经处理完，他点击发送其他人的待办消失
                if (this.HisNode.HuiQianLeaderRole == HuiQianLeaderRole.EveryOneMain)
                {
                    /* 当前人是组长，检查是否可以可以发送,检查自己是不是最后一个待办处理人 ？*/
                    if (this.HisGenerWorkFlow.TodoEmps.Contains(WebUser.No + ",") == true
                        && (DataType.IsNullOrEmpty(myhqzcr) == true || myhqType.Equals("AddLeader") == true))
                    {
                        String todoEmps = ""; // 记录没有处理的人.
                        String hqzcr = "";
                        String hqType = "";
                        int num = 0;
                        foreach (GenerWorkerList item in gwls)
                        {
                            //主持人
                            hqzcr = item.GetParaString("HuiQianZhuChiRen");
                            hqzcr = DataType.IsNullOrEmpty(hqzcr) == true ? "" : hqzcr;
                            //加签的类型 普通人，主持人
                            hqType = item.GetParaString("HuiQianType");
                            hqType = DataType.IsNullOrEmpty(hqType) == true ? "" : hqType;

                            if ((item.PassInt == 0 || item.PassInt == 90)
                                    && (item.EmpNo.Equals(WebUser.No) || hqzcr.Equals(WebUser.No) && hqType.Equals("AddLeader") == false))
                            {
                                if (item.EmpNo.Equals(WebUser.No) == false)
                                    todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + " ";
                                num++;
                            }
                        }
                        //说明当前自己加签的人员已经处理完成，自己是最后一个人
                        if (num == 1)
                        {
                            //删除其他人的待办信息
                            String sql = "UPDATE  WF_GenerWorkerlist  Set IsPass=1 WHERE WorkID=" + this.WorkID
                                    + " AND FK_Node=" + this.HisNode.NodeID + " AND IsPass=0 AND FK_Emp!='" + WebUser.No + "'";
                            DBAccess.RunSQL(sql);
                            this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name) + ";";
                            this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                            this.HisGenerWorkFlow.HuiQianZhuChiRen = "";
                            this.HisGenerWorkFlow.HuiQianZhuChiRenName = "";
                            return false;
                        }

                        this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todoEmps + ",您不能执行发送.", null,
                                SendReturnMsgType.Info);
                        return true;
                    }
                }

                //最后一个组长可以发发送
                if (this.HisNode.HuiQianLeaderRole == HuiQianLeaderRole.LastOneMain)
                {
                    /* 当前人是组长，检查是否可以可以发送,检查自己加签的人是否都已经处理完成 ？*/
                    if (this.HisGenerWorkFlow.TodoEmps.Contains(WebUser.No + ",") == true
                            && (DataType.IsNullOrEmpty(myhqzcr) == true || myhqType.Equals("AddLeader") == true))
                    {
                        string todoEmps = ""; // 记录没有处理的人.
                        string todoNos = "";
                        string todohqzcrEmps = "";//记录未处理的主持人
                        string hqzcr = "";
                        string hqType = "";
                        int num = 0;//自己及自己加签人的待办
                        int othernum = 0;//其它组长的待办

                        foreach (GenerWorkerList item in gwls)
                        {
                            //主持人
                            hqzcr = item.GetParaString("HuiQianZhuChiRen");
                            hqzcr = DataType.IsNullOrEmpty(hqzcr) == true ? "" : hqzcr;
                            //加签的类型 普通人，主持人
                            hqType = item.GetParaString("HuiQianType");
                            hqType = DataType.IsNullOrEmpty(hqType) == true ? "" : hqType;

                            if (item.PassInt == 0 || item.PassInt == 90)
                            {
                                if (item.EmpNo.Equals(WebUser.No) || hqzcr.Equals(WebUser.No) && hqType.Equals("AddLeader") == false)
                                {
                                    if (item.EmpNo.Equals(WebUser.No) == false)
                                    {
                                        todoEmps += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + ";";
                                        todoNos += item.EmpNo + ";";
                                    }

                                    num++;
                                }
                                if (item.EmpNo.Equals(WebUser.No) == false && (DataType.IsNullOrEmpty(hqzcr) == true || hqType.Equals("AddLeader") == true))
                                {
                                    todohqzcrEmps += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + " ";
                                    othernum++;
                                }

                            }
                        }
                        //说明当前自己加签的人员已经处理完成，并且是最后一个组长未处理待办
                        if (num == 1 && othernum == 0)
                        {
                            //删除其他人的待办信息
                            String sql = "UPDATE  WF_GenerWorkerlist  Set IsPass=1 WHERE WorkID=" + this.WorkID
                                    + " AND FK_Node=" + this.HisNode.NodeID + " AND IsPass=0 AND FK_Emp!='" + WebUser.No + "'";
                            DBAccess.RunSQL(sql);
                            this.HisGenerWorkFlow.Sender = BP.WF.Glo.DealUserInfoShowModel(WebUser.No, WebUser.Name) + ";";
                            this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                            this.HisGenerWorkFlow.HuiQianZhuChiRen = "";
                            this.HisGenerWorkFlow.HuiQianZhuChiRenName = "";
                            return false;
                        }
                        //当前组长加签的人员已经处理完，自己的待办可以结束
                        if (num == 1 && othernum != 0)
                        {
                            // 设置当前已经完成.
                            gwlOfMe.PassInt = 1;
                            gwlOfMe.Update();

                            // 检查完成条件。
                            if (this.HisNode.ItIsEndNode == false)
                            {
                                this.CheckCompleteCondition();
                            }
                            // 调用发送成功事件.
                            String sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this);
                            this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                            // 执行时效考核.
                            if (this.rptGe == null)
                            {
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID,
                                        this.rptGe.Title, gwlOfMe);
                            }
                            else
                            {
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0,
                                        this.HisGenerWorkFlow.Title, gwlOfMe);
                            }

                            this.AddToTrack(ActionType.TeampUp, todoNos, todoEmps, this.HisNode.NodeID,
                                    this.HisNode.Name, "协作发送");
                            String emps = this.HisGenerWorkFlow.TodoEmps;
                            emps = emps.Replace(WebUser.Name + ";", "");
                            this.HisGenerWorkFlow.TodoEmps = emps;
                            this.HisGenerWorkFlow.DirectUpdate();
                            this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todohqzcrEmps, null,
                                    SendReturnMsgType.Info);
                            return true;
                        }
                        if (DataType.IsNullOrEmpty(todohqzcrEmps) == false)
                            this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todoEmps + ",组长有：" + todohqzcrEmps + ",您不能执行发送.", null,
                                SendReturnMsgType.Info);
                        else
                            this.addMsg(SendReturnMsgFlag.CondInfo, "@当前工作未处理的会签人有: " + todoEmps + ",您不能执行发送.", null,
                                    SendReturnMsgType.Info);
                        return true;
                    }
                }

                #region 加签人的处理
                //查看是否我是最后一个？ 主持人必须是相同的人
                int mynum = 0;
                int cnum = 0;//当前加签人所属主持人下的待办数
                string todoEmps1 = ""; //记录没有处理的人.
                string todoEmpNos = "";
                foreach (GenerWorkerList item in gwls)
                {
                    //主持人
                    string hqzcr = item.GetParaString("HuiQianZhuChiRen");
                    hqzcr = DataType.IsNullOrEmpty(hqzcr) == true ? "" : hqzcr;
                    if (item.PassInt == 0 || item.PassInt == 90)
                    {
                        if (item.EmpNo.Equals(Execer) == false)
                        {
                            todoEmps1 += BP.WF.Glo.DealUserInfoShowModel(item.EmpNo, item.EmpName) + " ";
                            todoEmpNos += item.EmpNo + ";";
                        }

                        if (myhqzcr.Equals(hqzcr) || item.EmpNo.Equals(myhqzcr))
                            cnum++;
                        mynum++;
                    }

                }

                if (mynum == 1)
                {
                    this.HisGenerWorkFlow.Sender = WebUser.No + "," + WebUser.Name + ";";
                    this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                    this.HisGenerWorkFlow.HuiQianZhuChiRen = "";
                    this.HisGenerWorkFlow.HuiQianZhuChiRenName = "";
                    return false; /*只有一个待办,说明自己就是最后的一个人.*/
                }

                //把当前的待办设置已办，并且提示未处理的人。
                foreach (GenerWorkerList gwl in gwls)
                {
                    if (gwl.EmpNo.Equals(Execer) == false)
                        continue;

                    //设置当前已经完成.
                    gwl.PassInt = 1;
                    gwl.Update();

                    // 检查完成条件。
                    if (this.HisNode.ItIsEndNode == false)
                        this.CheckCompleteCondition();

                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs, null);

                    this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                    //执行时效考核.
                    if (this.rptGe == null)
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title, gwl);
                    else
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title, gwl);

                    this.AddToTrack(ActionType.TeampUp, todoEmpNos, todoEmps1, this.HisNode.NodeID, this.HisNode.Name, "协作发送");

                    //cut 当前的人员.
                    string emps = this.HisGenerWorkFlow.TodoEmps;
                    emps = emps.Replace(WebUser.No + "," + WebUser.Name + ";", "");
                    emps = emps.Replace(WebUser.Name + ";", "");
                    emps = emps.Replace(WebUser.Name, "");

                    this.HisGenerWorkFlow.TodoEmps = emps;
                    this.HisGenerWorkFlow.DirectUpdate();

                    //处理会签问题，
                    if (cnum == 2)
                    {
                        string msg = this.DealAlertZhuChiRen(myhqzcr);
                        this.addMsg(SendReturnMsgFlag.OverCurr, msg, null, SendReturnMsgType.Info);
                    }
                    else
                    {
                        this.addMsg(SendReturnMsgFlag.OverCurr, BP.WF.Glo.multilingual("@您已经完成签完工作. 当前未处理会签工作的人还有:{0}.", "WorkNode", "you_have_finished_1", todoEmps1), null, SendReturnMsgType.Info);
                    }
                    return true;

                    #endregion 加签人的处理

                }
                #endregion
            }
            throw new Exception("@不应该运行到这里。");
        }
        /// <summary>
        /// 处理队列节点
        /// </summary>
        /// <returns>是否可以向下发送?</returns>
        public bool DealOradeNode()
        {
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.HisNode.NodeID, GenerWorkerListAttr.IsPass);

            if (gwls.Count == 1)
                return false; /*让其向下执行,因为只有一个人。就没有顺序的问题.*/

            int idx = -100;
            foreach (GenerWorkerList gwl in gwls)
            {
                idx++;
                if (gwl.EmpNo != WebUser.No)
                    continue;

                //设置当前不可以用. //审核组件显示有问题IsPass设置成1审核通过
                gwl.PassInt = 1;
                gwl.Update();
            }

            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.PassInt > 10)
                {
                    /*就开始发到这个人身上. */
                    gwl.PassInt = 0;
                    gwl.Update();

                    // 检查完成条件。
                    if (this.HisNode.ItIsEndNode == false)
                    {
                        this.CheckCompleteCondition();
                    }
                    //写入日志.
                    this.AddToTrack(ActionType.Order, gwl.EmpNo, gwl.EmpName, this.HisNode.NodeID,
                        this.HisNode.Name, BP.WF.Glo.multilingual("队列发送", "WorkNode", "queue_transferred"));

                    this.addMsg(SendReturnMsgFlag.VarAcceptersID, gwl.EmpNo, gwl.EmpNo, SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.VarAcceptersName, gwl.EmpName, gwl.EmpName, SendReturnMsgType.SystemMsg);
                    this.addMsg(SendReturnMsgFlag.OverCurr, BP.WF.Glo.multilingual("@当前工作已经发送给({0},{1}).", "WorkNode", "send_to_the_operator", gwl.EmpNo, gwl.EmpName), null, SendReturnMsgType.Info);

                    //执行更新.
                    if (this.HisGenerWorkFlow.Emps.Contains("@" + WebUser.No + "," + WebUser.Name + "@") == false || this.HisGenerWorkFlow.Emps.Contains("@" + WebUser.No + "@") == false)
                        this.HisGenerWorkFlow.Emps = this.HisGenerWorkFlow.Emps + WebUser.No + "," + WebUser.Name + "@";

                    this.rptGe.FlowEmps = this.HisGenerWorkFlow.Emps;
                    this.rptGe.WFState = WFState.Runing;

                    this.rptGe.Update(GERptAttr.FlowEmps, this.rptGe.FlowEmps, GERptAttr.WFState, (int)WFState.Runing);


                    this.HisGenerWorkFlow.WFState = WFState.Runing;
                    this.HisGenerWorkFlow.Update();
                    return true;
                }
            }

            // 如果是最后一个，就要他向下发送。
            return false;
        }
        /// <summary>
        /// 检查阻塞模式
        /// </summary>
        private void CheckBlockModel()
        {
            if (this.HisNode.BlockModel == BlockModel.None)
                return;

            try
            {
                string blockMsg = this.HisNode.BlockAlert;

                if (DataType.IsNullOrEmpty(this.HisNode.BlockAlert))
                    blockMsg = BP.WF.Glo.multilingual("@符合发送阻塞规则，不能向下发送.", "WorkNode", "cannot_send_to_next");

                if (this.HisNode.BlockModel == BlockModel.CurrNodeAll)
                {
                    /*如果设置检查是否子流程结束.*/
                    GenerWorkFlows gwls = new GenerWorkFlows();
                    if (this.HisNode.ItIsSubThread == true)
                    {
                        /*如果是子流程,仅仅检查自己子流程上发起的workid.*/
                        QueryObject qo = new QueryObject(gwls);
                        qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PNodeID, this.HisNode.NodeID);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                        qo.DoQuery();
                        if (gwls.Count == 0)
                            return;
                    }
                    else
                    {
                        /*检查，以前的子线程是否发起过流程 与以前的分子线程节点是否发起过子流程。 */
                        QueryObject qo = new QueryObject(gwls);

                        qo.addLeftBracket();
                        qo.AddWhere(GenerWorkFlowAttr.PFID, this.WorkID);
                        qo.addOr();
                        qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                        qo.addRightBracket();

                        qo.addAnd();

                        qo.addLeftBracket();
                        qo.AddWhere(GenerWorkFlowAttr.PNodeID, this.HisNode.NodeID);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                        qo.addRightBracket();

                        qo.DoQuery();
                        if (gwls.Count == 0)
                            return;
                    }

                    string err = "";
                    err += BP.WF.Glo.multilingual("@如下子流程没有完成，你不能向下发送。@---------------------------------", "WorkNode", "cannot_send_to_next_1");
                    string wf_id = BP.WF.Glo.multilingual("@流程ID:", "WorkNode", "workflow_id");
                    string wf_title = BP.WF.Glo.multilingual(",标题:", "WorkNode", "workflow_title");
                    string wf_operator = BP.WF.Glo.multilingual(",当前执行人:", "WorkNode", "current_operator");
                    string wf_step = BP.WF.Glo.multilingual(",运行到节点:", "WorkNode", "current_step");
                    foreach (GenerWorkFlow gwf in gwls)
                        err += wf_id + gwf.WorkID + wf_title + gwf.Title + wf_operator + gwf.TodoEmps + wf_step + gwf.NodeName;

                    err = Glo.DealExp(blockMsg, this.rptGe) + err;
                    throw new Exception(err);
                }

                if (this.HisNode.BlockModel == BlockModel.SpecSubFlow)
                {
                    /*如果按照特定的格式判断阻塞*/
                    string exp = this.HisNode.BlockExp;
                    if (exp.Contains("@") == false)
                        throw new Exception(BP.WF.Glo.multilingual("@设置错误，该节点的阻塞配置格式({0})错误，请参考帮助来解决。", "WorkNode", "error_in_param_setting", exp));

                    string[] strs = exp.Split('@');
                    string err = "";
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        if (str.Contains("=") == false)
                            throw new Exception(BP.WF.Glo.multilingual("@阻塞设置的格式不正确:{0}.", "WorkNode", "error_in_param_setting", str));

                        string[] nodeFlow = str.Split('=');
                        int nodeid = int.Parse(nodeFlow[0]); //启动子流程的节点.
                        string subFlowNo = nodeFlow[1];

                        GenerWorkFlows gwls = new GenerWorkFlows();

                        if (this.HisNode.ItIsSubThread == true)
                        {
                            /* 如果是子线程，就不需要管，主干节点的问题。*/
                            QueryObject qo = new QueryObject(gwls);
                            qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subFlowNo);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);

                            qo.DoQuery();
                            if (gwls.Count == 0)
                                continue;
                        }
                        else
                        {
                            /* 非子线程，就需要考虑，从该节点上，发起的子线程的 ，主干节点的问题。*/
                            QueryObject qo = new QueryObject(gwls);

                            qo.addLeftBracket();
                            qo.AddWhere(GenerWorkFlowAttr.PFID, this.WorkID);
                            qo.addOr();
                            qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                            qo.addRightBracket();

                            qo.addAnd();

                            qo.addLeftBracket();
                            qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                            //qo.addAnd();
                            //qo.AddWhere(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subFlowNo);
                            qo.addRightBracket();

                            qo.DoQuery();
                            if (gwls.Count != 0 && (gwls[0] as GenerWorkFlow).WFSta == WFSta.Complete)
                                continue;
                        }

                        err += BP.WF.Glo.multilingual("@如下子流程没有完成，你不能向下发送。@---------------------------------", "WorkNode", "cannot_send_to_next_1");
                        string sub_wf_id = BP.WF.Glo.multilingual("@子流程ID:", "WorkNode", "sub_workflow_id");
                        string sub_wf_name = BP.WF.Glo.multilingual(",子流程名称:", "WorkNode", "sub_workflow_title");
                        string sub_wf_title = BP.WF.Glo.multilingual(",子流程标题:", "WorkNode", "sub_workflow_title");
                        string sub_wf_operator = BP.WF.Glo.multilingual(",当前执行人:", "WorkNode", "current_operator");
                        string sub_wf_step = BP.WF.Glo.multilingual(",运行到节点:", "WorkNode", "current_step");

                        foreach (GenerWorkFlow gwf in gwls)
                            err += BP.WF.Glo.multilingual("@子流程ID:{0}", "WorkNode", "sub_workflow_id", gwf.WorkID.ToString()) + "\n" + BP.WF.Glo.multilingual(",子流程名称:{0}", "WorkNode", "sub_workflow_title", gwf.FlowName)
                                                       + "\n" + BP.WF.Glo.multilingual(",子流程标题:{0}", "WorkNode", "sub_workflow_title", gwf.Title) + "\n" + BP.WF.Glo.multilingual(",当前执行人:{0}", "WorkNode", "current_operator", gwf.TodoEmps)
                                                       + "\n" + BP.WF.Glo.multilingual(",运行到节点:{0}", "WorkNode", "current_step", gwf.NodeName);
                    }

                    if (DataType.IsNullOrEmpty(err) == true)
                        return;

                    err = Glo.DealExp(blockMsg, this.rptGe) + err;
                    throw new Exception(err);
                }

                if (this.HisNode.BlockModel == BlockModel.BySQL)
                {

                    string sql = this.HisNode.BlockExp;
                    sql = Glo.DealExp(sql, this.rptGe);

                    sql = sql.Replace("@WorkID", this.WorkID.ToString());
                    sql = sql.Replace("@OID", this.WorkID.ToString());

                    /*按 sql 判断阻塞*/
                    decimal d = DBAccess.RunSQLReturnValDecimal(Glo.DealExp(sql, this.rptGe), 0, 1);
                    //如果值大于0进行阻塞
                    if (d > 0)
                        throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));
                    return;
                }

                if (this.HisNode.BlockModel == BlockModel.ByExp)
                {
                    /*按表达式阻塞. 格式为: @ABC=123 */
                    //this.MsgOfCond = "@以表单值判断方向，值 " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") 操作符:(" + this.FK_Operator + ") 判断值:(" + this.OperatorValue.ToString() + ")";
                    string exp = this.HisNode.BlockExp;
                    string[] strs = exp.Trim().Split(' ');

                    string key = strs[0].Trim().TrimStart('@');
                    string oper = strs[1].Trim();
                    string val = strs[2].Trim();
                    val = val.Replace("'", "");
                    val = val.Replace("%", "");
                    val = val.Replace("~", "");
                    BP.En.Row row = this.rptGe.Row;
                    string valPara = null;
                    if (row.ContainsKey(key) == false)
                    {
                        try
                        {
                            bool isHave = false;
                            if (BP.Difference.SystemConfig.isBSsystem == true)
                            {
                                foreach (string param in HttpContextHelper.RequestParamKeys)
                                {
                                    if (string.IsNullOrEmpty(param) || param.Equals(key) == false)
                                        continue;
                                    valPara = HttpContextHelper.RequestParams(key);
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                            {
                                string expression = exp + " Key=(" + key + ") oper=(" + oper + ")Val=(" + val + ")";
                                throw new Exception(BP.WF.Glo.multilingual("@判断条件时错误,请确认参数是否拼写错误,没有找到对应的表达式:{0}.", "WorkNode", "expression_setting_error", expression));
                            }
                        }
                        catch
                        {
                            //有可能是常量. 
                            valPara = key;
                        }
                    }
                    else
                    {
                        valPara = row[key].ToString().Trim();
                    }

                    #region 开始执行判断.
                    if (oper == "=")
                    {
                        //如果表达式成立，就阻塞.
                        if (valPara.Equals(val) == true)
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));
                        return;
                    }

                    if (oper.ToUpper() == "LIKE")
                    {
                        if (valPara.Contains(val) == true)
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));
                        return;
                    }

                    if (oper == ">")
                    {
                        if (float.Parse(valPara) > float.Parse(val))
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));

                        return;
                    }

                    if (oper == ">=")
                    {
                        if (float.Parse(valPara) >= float.Parse(val))
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));

                        return;
                    }

                    if (oper == "<")
                    {
                        if (float.Parse(valPara) < float.Parse(val))
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));

                        return;
                    }

                    if (oper == "<=")
                    {
                        if (float.Parse(valPara) <= float.Parse(val))
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));

                        return;
                    }

                    if (oper == "!=")
                    {
                        if (float.Parse(valPara) != float.Parse(val))
                            throw new Exception("@" + Glo.DealExp(blockMsg, this.rptGe));

                        return;
                    }

                    string expression1 = exp + " Key=" + key + " oper=" + oper + " Val=" + val + ")";
                    throw new Exception(BP.WF.Glo.multilingual("@阻塞模式参数配置格式错误:{0}.", "WorkNode", "error_in_param_setting", expression1));
                    #endregion 开始执行判断.
                }

                //为父流程时，指定的子流程未运行到指定节点，则阻塞
                if (this.HisNode.BlockModel == BlockModel.SpecSubFlowNode)
                {
                    /*如果按照特定的格式判断阻塞*/
                    string exp = this.HisNode.BlockExp;
                    if (exp.Contains("@") == false)
                        throw new Exception(BP.WF.Glo.multilingual("@设置错误，该节点的阻塞配置格式({0})错误，请参考帮助来解决。", "WorkNode", "error_in_param_setting", exp));


                    string[] strs = exp.Split('@');
                    string err = "";
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        if (str.Contains("=") == false)
                            throw new Exception(BP.WF.Glo.multilingual("@阻塞设置的格式不正确:{0}.", "WorkNode", "error_in_param_setting", str));


                        string[] nodeFlow = str.Split('=');
                        int nodeid = int.Parse(nodeFlow[0]); //启动子流程的节点.
                        int subFlowNode = int.Parse(nodeFlow[1]); //子流程的节点
                        Node subNode = new Node(subFlowNode);
                        GenerWorkFlows gwfs = new GenerWorkFlows();
                        GenerWorkerLists gwls = new GenerWorkerLists();

                        if (this.HisNode.ItIsSubThread == true)
                        {
                            /* 如果是子线程，就不需要管，主干节点的问题。*/
                            QueryObject qo = new QueryObject(gwfs);
                            qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subNode.FlowNo);
                            qo.DoQuery();
                            //该子流程已经运行
                            if (gwfs.Count != 0)
                            {
                                GenerWorkFlow gwf = (GenerWorkFlow)gwfs[0];
                                if (gwf.WFState == WFState.Complete) //子流程结束
                                    continue;

                                //判断是否运行到指定的节点
                                gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID, GenerWorkerListAttr.FK_Node, subFlowNode, GenerWorkerListAttr.IsPass, 1);
                                if (gwls.Count != 0)
                                    continue;

                                gwls.Retrieve(GenerWorkerListAttr.FID, gwf.WorkID, GenerWorkerListAttr.FK_Node, subFlowNode, GenerWorkerListAttr.IsPass, 1);
                                if (gwls.Count != 0)
                                    continue;
                            }

                        }
                        else
                        {
                            /* 非子线程，就需要考虑，从该节点上，发起的子线程的 ，主干节点的问题。*/
                            QueryObject qo = new QueryObject(gwfs);

                            qo.addLeftBracket();
                            qo.AddWhere(GenerWorkFlowAttr.PFID, this.WorkID);
                            qo.addOr();
                            qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.WorkID);
                            qo.addRightBracket();

                            qo.addAnd();

                            qo.addLeftBracket();
                            qo.AddWhere(GenerWorkFlowAttr.PNodeID, nodeid);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisFlow.No);
                            qo.addAnd();
                            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subNode.FlowNo);
                            qo.addRightBracket();

                            qo.DoQuery();
                            //该子流程已经运行
                            if (gwfs.Count != 0)
                            {
                                GenerWorkFlow gwf = (GenerWorkFlow)gwfs[0];
                                if (gwf.WFState == WFState.Complete) //子流程结束
                                    continue;

                                //判断是否运行到指定的节点
                                string sql = "";
                                if (gwf.FID == 0)
                                    sql = "SELECT count(*) as Num FROM ND" + int.Parse(gwf.FlowNo) + "Track WHERE WorkID=" + gwf.WorkID + " AND (NDFrom=" + subFlowNode + " or NDTo=" + subFlowNode + " )";
                                else
                                    sql = "SELECT count(*) as Num FROM ND" + int.Parse(gwf.FlowNo) + "Track WHERE FID=" + gwf.WorkID + " AND (NDFrom=" + subFlowNode + " or NDTo=" + subFlowNode + " )";

                                if (DBAccess.RunSQLReturnValInt(sql) != 0)
                                    continue;

                                //做第2次判断.
                                if (gwf.FID == 0)
                                    sql = "SELECT count(*) as Num FROM WF_GenerWorkerlist WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + subFlowNode;
                                else
                                    sql = "SELECT count(*) as Num FROM WF_GenerWorkerlist  WHERE WorkID=" + gwf.FID + " AND FK_Node=" + subFlowNode;

                                if (DBAccess.RunSQLReturnValInt(sql) != 0)
                                    continue;

                            }
                        }

                        err += BP.WF.Glo.multilingual("@如下子流程没有完成，你不能向下发送。@---------------------------------", "WorkNode", "cannot_send_to_next_1");
                        string sub_wf_id = BP.WF.Glo.multilingual("@子流程ID:", "WorkNode", "sub_workflow_id");
                        string sub_wf_name = BP.WF.Glo.multilingual(",子流程名称:", "WorkNode", "sub_workflow_title");
                        string sub_wf_title = BP.WF.Glo.multilingual(",子流程标题:", "WorkNode", "sub_workflow_title");
                        string sub_wf_operator = BP.WF.Glo.multilingual(",当前执行人:", "WorkNode", "current_operator");
                        string sub_wf_step = BP.WF.Glo.multilingual(",运行到节点:", "WorkNode", "current_step");

                        foreach (GenerWorkFlow gwf in gwfs)
                            err += BP.WF.Glo.multilingual("@子流程ID:{0}", "WorkNode", "sub_workflow_id", gwf.WorkID.ToString()) + "\n" + BP.WF.Glo.multilingual(",子流程名称:{0}", "WorkNode", "sub_workflow_title", gwf.FlowName)
                                                       + "\n" + BP.WF.Glo.multilingual(",子流程标题:{0}", "WorkNode", "sub_workflow_title", gwf.Title) + "\n" + BP.WF.Glo.multilingual(",当前执行人:{0}", "WorkNode", "current_operator", gwf.TodoEmps)
                                                       + "\n" + BP.WF.Glo.multilingual(",运行到节点:{0}", "WorkNode", "current_step", gwf.NodeName);
                    }

                    if (DataType.IsNullOrEmpty(err) == true)
                        return;

                    err = Glo.DealExp(blockMsg, this.rptGe) + err;
                    throw new Exception(err);
                }

                //为平级流程时，指定的子流程未运行到指定节点，则阻塞
                if (this.HisNode.BlockModel == BlockModel.SameLevelSubFlow)
                {
                    /*如果按照特定的格式判断阻塞*/
                    string exp = this.HisNode.BlockExp;

                    string[] strs = exp.Split(',');
                    string err = "";
                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        int nodeid = int.Parse(str); //平级子流程的节点
                        Node subNode = new Node(nodeid);
                        GenerWorkFlows gwfs = new GenerWorkFlows();
                        GenerWorkerLists gwls = new GenerWorkerLists();


                        QueryObject qo = new QueryObject(gwfs);
                        qo.AddWhere(GenerWorkFlowAttr.PWorkID, this.HisGenerWorkFlow.PWorkID);
                        //qo.addAnd(); 
                        //qo.AddWhere(GenerWorkFlowAttr.PNodeID, this.HisGenerWorkFlow.PNodeID);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.PFlowNo, this.HisGenerWorkFlow.PFlowNo);
                        qo.addAnd();
                        qo.AddWhere(GenerWorkFlowAttr.FK_Flow, subNode.FlowNo);
                        qo.DoQuery();
                        //该子流程已经运行
                        if (gwfs.Count != 0)
                        {
                            GenerWorkFlow gwf = (GenerWorkFlow)gwfs[0];
                            if (gwf.WFState == WFState.Complete) //子流程结束
                                continue;

                            //判断是否运行到指定的节点
                            long workId = gwf.WorkID;
                            gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID, GenerWorkerListAttr.FK_Node, nodeid, GenerWorkerListAttr.IsPass, 1);
                            if (gwls.Count != 0)
                                continue;
                        }
                        err += BP.WF.Glo.multilingual("@如下子流程没有完成，你不能向下发送。@---------------------------------", "WorkNode", "cannot_send_to_next_1");
                        string sub_wf_id = BP.WF.Glo.multilingual("@子流程ID:", "WorkNode", "sub_workflow_id");
                        string sub_wf_name = BP.WF.Glo.multilingual(",子流程名称:", "WorkNode", "sub_workflow_title");
                        string sub_wf_title = BP.WF.Glo.multilingual(",子流程标题:", "WorkNode", "sub_workflow_title");
                        string sub_wf_operator = BP.WF.Glo.multilingual(",当前执行人:", "WorkNode", "current_operator");
                        string sub_wf_step = BP.WF.Glo.multilingual(",运行到节点:", "WorkNode", "current_step");

                        foreach (GenerWorkFlow gwf in gwfs)
                            err += BP.WF.Glo.multilingual("@子流程ID:{0}", "WorkNode", "sub_workflow_id", gwf.WorkID.ToString()) + "\n" + BP.WF.Glo.multilingual(",子流程名称:{0}", "WorkNode", "sub_workflow_title", gwf.FlowName)
                                                      + "\n" + BP.WF.Glo.multilingual(",子流程标题:{0}", "WorkNode", "sub_workflow_title", gwf.Title) + "\n" + BP.WF.Glo.multilingual(",当前执行人:{0}", "WorkNode", "current_operator", gwf.TodoEmps)
                                                      + "\n" + BP.WF.Glo.multilingual(",运行到节点:{0}", "WorkNode", "current_step", gwf.NodeName);
                    }

                    if (DataType.IsNullOrEmpty(err) == true)
                        return;

                    err = Glo.DealExp(blockMsg, this.rptGe) + err;
                    throw new Exception(err);
                }
                throw new Exception("@该阻塞模式没有实现...");
            }
            catch (Exception ex)
            {

                //  throw ex;

                //提示：宜昌的友好提示 211102
                if (this.HisNode.BlockModel == BlockModel.BySQL)
                    throw new Exception("阻塞原因：" + this.HisNode.BlockAlert);


                //正确的提示: 宜昌的需要这样的明确的提示信息.
                throw new Exception("设置的阻塞规则错误:" + this.HisNode.BlockModel + ",exp:" + this.HisNode.BlockExp + "异常信息" + ex.Message);

            }
        }
        /// <summary>
        /// 发送到延续子流程.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="toEmps"></param>
        /// <returns></returns>
        private SendReturnObjs NodeSendToYGFlow(Node node, string toEmpIDs)
        {
            SubFlowYanXu subFlow = new SubFlowYanXu();
            subFlow.setMyPK(this.HisNode.NodeID + "_" + node.FlowNo + "_" + 2);
            if (subFlow.RetrieveFromDBSources() == 0)
                throw new Exception(BP.WF.Glo.multilingual("@延续子流程配置信息丢失，请联系管理员.", "WorkNode", "not_found_receiver"));

            string sql = "";
            if (DataType.IsNullOrEmpty(toEmpIDs))
            {
                toEmpIDs = "";

                DataTable dt = null;

                #region 按照人员选择
                if (node.HisDeliveryWay == DeliveryWay.BySelected)
                {
                    sql = "SELECT FK_Emp AS No, EmpName AS Name FROM WF_SelectAccper WHERE FK_Node=" + node.NodeID + " AND WorkID=" + this.WorkID + " AND AccType=0";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        throw new Exception(BP.WF.Glo.multilingual("@没有为延续子流程设置接收人.", "WorkNode", "not_found_receiver"));
                }
                #endregion 按照人员选择.

                #region 按照角色与部门的交集.
                if (node.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
                {
                    sql = "SELECT pdes.fk_emp AS No"
                     + " FROM   Port_DeptEmpStation pdes"
                     + "        INNER JOIN WF_NodeDept wnd"
                     + "             ON  wnd.fk_dept = pdes.fk_dept"
                     + "             AND wnd.fk_node = " + node.NodeID
                     + "        INNER JOIN WF_NodeStation wns"
                     + "             ON  wns.FK_Station = pdes.fk_station"
                     + "             AND wnd.fk_node =" + node.NodeID
                     + " ORDER BY"
                     + "        pdes.fk_emp";
                    dt = DBAccess.RunSQLReturnTable(sql);


                    if (dt.Rows.Count == 0)
                    {
                        string[] para = new string[4];
                        para[0] = node.HisDeliveryWay.ToString();
                        para[1] = node.NodeID.ToString();
                        para[2] = node.Name;
                        para[3] = sql;

                        throw new Exception(BP.WF.Glo.multilingual("@节点访问规则({0})错误:节点({1},{2}), 按照角色与部门的交集确定接收人的范围错误，没有找到人员:SQL={3}.", "WorkNode", "error_in_access_rules_setting", para));
                    }
                }
                #endregion 按照角色与部门的交集

                #region 仅按角色计算
                if (node.HisDeliveryWay == DeliveryWay.ByStationOnly)
                {
                    ps = new Paras();


                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    {
                        sql = "SELECT A.FK_Emp No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                        ps.Add("FK_Node", node.NodeID);
                        ps.SQL = sql;
                    }
                    else
                    {
                        sql = "SELECT A.FK_Emp No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND A.OrgNo=" + dbStr + "OrgNo AND B.FK_Node=" + dbStr + "FK_Node  ORDER BY A.FK_Emp";
                        ps.Add("OrgNo", BP.Web.WebUser.OrgNo);
                        ps.Add("FK_Node", node.NodeID);
                        ps.SQL = sql;
                    }

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        string[] para2 = new string[3];
                        para2[0] = node.HisDeliveryWay.ToString();
                        para2[1] = node.NodeID.ToString();
                        para2[2] = node.Name;
                        // para2[3] = ps.SQLNoPara;
                        throw new Exception(BP.WF.Glo.multilingual("@节点访问规则{0}错误:节点({1},{2}), 仅按角色计算，没有找到人员.", "WorkNode", "error_in_access_rules_setting", para2));
                    }
                }
                #endregion

                #region 仅按用户组计算 
                if (node.HisDeliveryWay == DeliveryWay.ByTeamOnly)
                {
                    sql = "SELECT A.FK_Emp No FROM Port_TeamEmp A, WF_NodeTeam B WHERE A.FK_Team=B.FK_Team AND B.FK_Node=" + node.NodeID;
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        string[] para2 = new string[4];
                        para2[0] = node.HisDeliveryWay.ToString();
                        para2[1] = node.NodeID.ToString();
                        para2[2] = node.Name;
                        para2[3] = sql;
                        throw new Exception(BP.WF.Glo.multilingual("@节点访问规则{0}错误:节点({1},{2}), 仅按用户组计算，没有找到人员:SQL={3}.", "WorkNode", "error_in_access_rules_setting", para2));
                    }
                }
                #endregion

                #region 按用户组计算（本部门）
                if (node.HisDeliveryWay == DeliveryWay.ByTeamDeptOnly)
                {
                    sql = "SELECT A.FK_Emp No FROM Port_TeamEmp A, WF_NodeTeam B, Port_DeptEmp C WHERE A.FK_Emp=C.FK_Emp AND A.FK_Team=B.FK_Team AND B.FK_Node=" + node.NodeID + " AND C.FK_Dept='" + WebUser.DeptNo + "' ORDER BY A.FK_Emp";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        string[] para2 = new string[4];
                        para2[0] = node.HisDeliveryWay.ToString();
                        para2[1] = node.NodeID.ToString();
                        para2[2] = node.Name;
                        para2[3] = sql;
                        throw new Exception(BP.WF.Glo.multilingual("@节点访问规则{0}错误:节点({1},{2}), 仅按用户组计算，没有找到人员:SQL={3}.", "WorkNode", "error_in_access_rules_setting", para2));
                    }
                }
                #endregion

                #region 按用户组计算(本组织) 
                if (node.HisDeliveryWay == DeliveryWay.ByTeamOrgOnly)
                {
                    sql = "SELECT A.FK_Emp No FROM Port_TeamEmp A, WF_NodeTeam B, Port_Emp C WHERE A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node AND C.OrgNo=" + dbStr + "OrgNo ORDER BY A.FK_Emp";
                    ps = new Paras();
                    ps.Add("FK_Node", node.NodeID);
                    ps.Add("OrgNo", WebUser.OrgNo);

                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        string[] para2 = new string[4];
                        para2[0] = node.HisDeliveryWay.ToString();
                        para2[1] = node.NodeID.ToString();
                        para2[2] = node.Name;
                        para2[3] = ps.SQLNoPara;
                        throw new Exception(BP.WF.Glo.multilingual("@节点访问规则{0}错误:节点({1},{2}), 仅按用户组计算，没有找到人员:SQL={3}.", "WorkNode", "error_in_access_rules_setting", para2));
                    }
                }
                #endregion

                #region 按绑定的人计算
                if (node.HisDeliveryWay == DeliveryWay.ByBindEmp)
                {
                    ps = new Paras();
                    ps.Add("FK_Node", node.NodeID);
                    ps.SQL = "SELECT FK_Emp AS No FROM WF_NodeEmp WHERE FK_Node=" + dbStr + "FK_Node ORDER BY FK_Emp";
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        throw new Exception(BP.WF.Glo.multilingual("@流程设计错误:没找到下一个节点(" + town.HisNode.Name + ")的接收人.", "WorkNode", "system_error_not_found_operator", town.HisNode.Name));
                }
                #endregion

                if (dt == null)
                    throw new Exception(BP.WF.Glo.multilingual("err@您启动的子流程或者延续流程开始节点没有明确的设置接收人.", "WorkNode", "not_found_receiver"));

                if (dt.Rows.Count == 0)
                    throw new Exception("err@请选择接受人.");

                if (dt.Rows.Count != 1)
                    throw new Exception("err@必须选择一个接受人.");

                toEmpIDs = dt.Rows[0]["No"].ToString();
                ////组装到达的人员. 延续子流程的第一个节点的发起人只有一个人 @lizhen.
                //foreach (DataRow dr in dt.Rows)
                //    toEmpIDs =  dr["No"].ToString();
            }

            if (DataType.IsNullOrEmpty(toEmpIDs) == true)
                throw new Exception(BP.WF.Glo.multilingual("@延续子流程目前仅仅支持选择接收人方式.", "WorkNode", "not_found_receiver"));

            string starter = toEmpIDs;
            Int64 workid = 0;
            bool IsSendToStartNode = true;
            //if (subFlow.YanXuToNode != int.Parse(int.Parse(subFlow.SubFlowNo) + "01"))
            //    IsSendToStartNode = false;

            if (node.NodeID != int.Parse(int.Parse(subFlow.SubFlowNo) + "01"))
                IsSendToStartNode = false;

            if (IsSendToStartNode == false)
                starter = WebUser.No;

            if (subFlow.HisSubFlowModel == SubFlowModel.SubLevel)//下级子流程
            {
                workid = BP.WF.Dev2Interface.Node_CreateBlankWork(node.FlowNo, null, null,
                starter, null, this.WorkID, 0, this.HisNode.FlowNo, this.HisNode.NodeID, BP.Web.WebUser.No, 0, null);
            }
            else if (subFlow.HisSubFlowModel == SubFlowModel.SameLevel)//平级子流程
            {
                workid = BP.WF.Dev2Interface.Node_CreateBlankWork(node.FlowNo, null, null,
               starter, null, this.HisGenerWorkFlow.PWorkID, 0, this.HisGenerWorkFlow.PFlowNo, this.HisGenerWorkFlow.PNodeID, this.HisGenerWorkFlow.PEmp, 0, null);
                //存储同级子流程的信息
                GenerWorkFlow subYXGWF = new GenerWorkFlow(workid);
                subYXGWF.SetPara("SLWorkID", this.WorkID);
                subYXGWF.SetPara("SLFlowNo", this.HisNode.FlowNo);
                subYXGWF.SetPara("SLNodeID", this.HisNode.NodeID);
                subYXGWF.SetPara("SLEmp", BP.Web.WebUser.No);
                subYXGWF.Update();
            }

            //复制当前信息.
            Work wk = node.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            wk.Copy(this.HisWork);
            wk.Update();

            //为接收人显示待办.
            //if (subFlow.YanXuToNode == int.Parse(int.Parse(subFlow.SubFlowNo) + "01"))
            if (node.NodeID == int.Parse(int.Parse(subFlow.SubFlowNo) + "01"))
            {
                // 产生工作列表. 
                GenerWorkerList gwl = new GenerWorkerList();
                int count = gwl.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, node.NodeID);
                if (count == 0)
                {
                    Emp emp = new Emp(toEmpIDs);
                    gwl.WorkID = workid;
                    gwl.EmpNo = toEmpIDs;
                    gwl.EmpName = emp.Name;

                    gwl.NodeID = node.NodeID;
                    gwl.NodeName = node.Name;
                    gwl.FID = 0;

                    gwl.FlowNo = node.FlowNo;
                    gwl.DeptNo = emp.DeptNo;
                    gwl.DeptName = emp.DeptText;

                    gwl.SDT = "无";
                    gwl.DTOfWarning = DataType.CurrentDateTimess;
                    gwl.ItIsEnable = true;

                    gwl.ItIsPass = false;
                    gwl.Save();

                }
                BP.WF.Dev2Interface.Node_SetDraft2Todolist(workid);
            }
            else
            {
                //执行发送到下一个环节..
                //SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(subFlow.SubFlowNo, workid, subFlow.YanXuToNode, toEmpIDs);
                SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(subFlow.SubFlowNo, workid, node.NodeID, toEmpIDs);
            }

            //设置变量.
            this.addMsg(SendReturnMsgFlag.VarToNodeID, node.NodeID.ToString(), workid.ToString(), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarAcceptersID, toEmpIDs, toEmpIDs, SendReturnMsgType.SystemMsg);

            //设置消息.
            this.addMsg("Msg1", BP.WF.Glo.multilingual("子流程({0})已经启动,发送给({1})处理人.", "WorkNode", "sub_wf_started", node.FlowName, toEmpIDs));
            if (BP.Difference.SystemConfig.CustomerNo.Equals("ASSET") == false)
                this.addMsg("Msg2", BP.WF.Glo.multilingual("当前您的待办不可见,需要等待子流程完成后您的待办才能显示,您可以从在途里查看工作进度.", "WorkNode", "to_do_list_invisible"));


            //设置当前工作操作员不可见.
            sql = "UPDATE WF_GenerWorkerlist SET IsPass=80 WHERE WorkID=" + this.WorkID + " AND IsPass=0";
            DBAccess.RunSQL(sql);

            return HisMsgObjs;
        }

        /// <summary>
        /// 工作流发送业务处理.
        /// 升级日期:2012-11-11.
        /// 升级原因:代码逻辑性不清晰,有遗漏的处理模式.
        /// 修改人:zhoupeng.
        /// 修改地点:厦门.
        /// ----------------------------------- 说明 -----------------------------
        /// 1，方法体分为三大部分: 发送前检查\5*5算法\发送后的业务处理.
        /// 2, 详细请参考代码体上的说明.
        /// 3, 发送后可以直接获取它的
        /// </summary>
        /// <param name="jumpToNode">要跳转的节点,可以为空.</param>
        /// <param name="jumpToEmp">要跳转的人,可以为空.</param>
        /// <returns>返回执行结果</returns>
        public SendReturnObjs NodeSend(Node jumpToNode, string jumpToEmp, bool IsReturnNode = false)
        {
            //判断 guest 节点.
            if (this.HisNode.ItIsGuestNode)
                if (this.Execer.Equals("Guest") == false)
                    throw new Exception(BP.WF.Glo.multilingual("@当前节点({0})是客户执行节点,所以当前登录人员应当是Guest,现在是:{1}.", "WorkNode", "should_gust", this.HisNode.Name, this.Execer));

            #region 第1: 安全性检查.
            //   第1: 检查是否可以处理当前的工作.
            if ((this.HisNode.ItIsStartNode == false ||
                (this.HisNode.ItIsStartNode == true && this.HisGenerWorkFlow.WFState == WFState.ReturnSta))
                && this.HisGenerWorkFlow.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, this.Execer) == false)
                    throw new Exception("@当前工作{" + this.HisFlow.No + " - WorkID=" + this.WorkID + "} 您({" + this.Execer + "} {" + this.ExecerName + "})没有处理权限.");
            }
            #endregion 安全性检查.

            #region 第2: 调用发起前的事件接口,处理用户定义的业务逻辑.
            string sendWhen = ExecEvent.DoNode(EventListNode.SendWhen, this);

            //返回格式. @Info=xxxx@ToNodeID=xxxx@ToEmps=xxxx@IsStopFlow=0
            if (sendWhen != null && sendWhen.IndexOf("@") >= 0)
            {
                AtPara ap = new AtPara(sendWhen);
                int nodeid = ap.GetValIntByKey("ToNodeID", 0);
                if (nodeid != 0)
                    jumpToNode = new Node(nodeid);

                //监测是否有停止流程的标志？
                this.IsStopFlow = ap.GetValBoolenByKey("IsStopFlow", false);

                string toEmps = ap.GetValStrByKey("ToEmps");
                if (DataType.IsNullOrEmpty(toEmps) == false)
                    jumpToEmp = toEmps;

                //处理str信息.
                sendWhen = sendWhen.Replace("@Info=", "");
                sendWhen = sendWhen.Replace("@IsStopFlow=1", "");
                sendWhen = sendWhen.Replace("@ToNodeID=" + nodeid.ToString(), "");
                sendWhen = sendWhen.Replace("@ToEmps=" + toEmps, "");
            }

            if (sendWhen != null)
            {
                /*说明有事件要执行,把执行后的数据查询到实体里*/
                this.HisWork.RetrieveFromDBSources();
                this.HisWork.ResetDefaultVal();
                this.HisWork.Rec = this.Execer;
                if (DataType.IsNullOrEmpty(sendWhen) == false)
                {
                    sendWhen = System.Web.HttpUtility.UrlDecode(sendWhen);
                    if (sendWhen.StartsWith("false") || sendWhen.StartsWith("False") || sendWhen.StartsWith("error") || sendWhen.StartsWith("Error"))
                    {
                        this.addMsg(SendReturnMsgFlag.SendWhen, sendWhen);
                        sendWhen = sendWhen.Replace("false", "");
                        sendWhen = sendWhen.Replace("False", "");
                        throw new Exception(BP.WF.Glo.multilingual("@执行发送前事件失败:{0}.", "WorkNode", "error_send", sendWhen));
                    }
                }

                //把发送sendWhen 消息提示给用户.
                if (sendWhen.Equals("null") == true)
                    sendWhen = "";
                this.addMsg("SendWhen", sendWhen, sendWhen, SendReturnMsgType.Info);
            }

            //加入系统变量.
            this.addMsg(SendReturnMsgFlag.VarCurrNodeID, this.HisNode.NodeID.ToString(), this.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarCurrNodeName, this.HisNode.Name, this.HisNode.Name, SendReturnMsgType.SystemMsg);
            this.addMsg(SendReturnMsgFlag.VarWorkID, this.WorkID.ToString(), this.WorkID.ToString(), SendReturnMsgType.SystemMsg);

            if (this.IsStopFlow == true)
            {
                /*在检查完后，反馈来的标志流程已经停止了。*/

                //查询出来当前节点的工作报表.
                this.rptGe = this.HisFlow.HisGERpt;
                this.rptGe.SetValByKey("OID", this.WorkID);
                this.rptGe.RetrieveFromDBSources();

                this.Func_DoSetThisWorkOver(); //设置工作完成.

                this.rptGe.WFState = WFState.Complete;
                this.rptGe.Update();
                this.HisGenerWorkFlow.Update(); //added by liuxc,2016-10-24,最后节点更新Sender字段

                //执行考核.
                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title);

                //判断当前流程是否子流程，是否启用该流程结束后，主流程自动运行到下一节点 
                string msg = WorkNodePlus.SubFlowEvent(this);
                if (DataType.IsNullOrEmpty(msg) == false)
                    this.HisMsgObjs.AddMsg("info", msg, msg, SendReturnMsgType.Info);

                CC(this.HisNode); //抄送到其他节点.
                return this.HisMsgObjs;
            }
            #endregion 处理发送前事件.

            //设置跳转节点，如果有可以为null.
            this.JumpToNode = jumpToNode;
            this.JumpToEmp = jumpToEmp;

            // 为广西计算中心增加自动返回的节点, 发送之后，让其自动返回给发送人.
            if (this.HisNode.ItIsSendBackNode == true)
            {
                WorkNode wn = WorkNodePlus.IsSendBackNode(this);
                this.JumpToEmp = wn.JumpToEmp;
                this.JumpToNode = wn.JumpToNode; //计算要到达的人.
            }

            //定义变量.
            //string sql = null;
            //DateTime dt = DateTime.Now;
            this.HisWork.Rec = this.Execer;
            // this.WorkID = this.HisWork.OID;

            #region 第一步: 检查当前操作员是否可以发送: 共分如下 3 个步骤.
            //第1.2.1: 如果是开始节点，就要检查发起流程限制条件.
            if (this.HisNode.ItIsStartNode == true)
            {
                if (WorkNodePlus.CheckIsCanStartFlow_SendStartFlow(this.HisFlow, this.HisWork) == false)
                {
                    string er = Glo.DealExp(this.HisFlow.StartLimitAlert, this.HisWork);
                    throw new Exception(BP.WF.Glo.multilingual("@违反了流程发起限制条件:{0}.", "WorkNode", "error_send", er));
                }
            }

            // 第1.3: 判断当前流程状态,如果是加签状态, 处理.
            if (this.HisNode.ItIsStartNode == false
            && this.HisGenerWorkFlow.WFState == WFState.Askfor)
            {
                SendReturnObjs objs = WorkNodePlus.DealAskForState(this);
                if (objs != null)
                    return objs;
            }

            // 第3: 如果是是合流点，有子线程未完成的情况.(不能删除或手工删除需要抛异常，自动删除则直接删除子流程)
            if (this.HisNode.ItIsHL || this.HisNode.HisRunModel == RunModel.FHL)
                WorkNodePlus.DealHeLiuState(this);

            #endregion 第一步: 检查当前操作员是否可以发送

            //查询出来当前节点的工作报表.
            if (this.rptGe == null || this.rptGe.OID == 0)
            {
                this.rptGe = this.HisFlow.HisGERpt;
                if (this.HisNode.ItIsSubThread == true)
                    this.rptGe.SetValByKey("OID", this.HisGenerWorkFlow.FID);
                else
                    this.rptGe.SetValByKey("OID", this.WorkID);

                int i = this.rptGe.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("err@系统错误，不应该查询不出来." + this.rptGe.EnMap.PhysicsTable + " WorkID=" + this.rptGe.OID);
            }

            //检查阻塞模式.
            this.CheckBlockModel();

            // 检查FormTree必填项目,如果有一些项目没有填写就抛出异常.
            this.CheckFrmIsNotNull();

            // 处理自动运行 - 预先设置未来的运行节点.
            // this.DealAutoRunEnable();

            //把数据更新到数据库里.
            this.HisWork.DirectUpdate();
            if (this.HisWork.EnMap.PhysicsTable != this.rptGe.EnMap.PhysicsTable)
            {
                // 有可能外部参数传递过来导致，rpt表数据没有发生变化。
                this.rptGe.Copy(this.HisWork);
                //首先执行保存，不然会影响条件的判断 by dgq 2016-1-14
                this.rptGe.Update();
            }

            //如果是队列节点, 就判断当前的队列人员是否走完。
            if (this.TodolistModel == TodolistModel.Order)
            {
                if (this.DealOradeNode() == true)
                {
                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this);

                    this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                    //执行时效考核.
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);

                    this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                    //设置当前的流程所有的用时.
                    this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                    this.rptGe.Update();
                    return this.HisMsgObjs;
                }
            }

            //如果是协作模式节点, 就判断当前的队列人员是否走完.
            if (this.TodolistModel == TodolistModel.Teamup)
            {
                //判断删除其他人员待办的规则.
                if (this.HisNode.GenerWorkerListDelRole != 0)
                {
                    WorkNodePlus.GenerWorkerListDelRole(this.HisNode, this.HisGenerWorkFlow);
                }

                //,增加了此部分.
                string todoEmps = this.HisGenerWorkFlow.TodoEmps;
                todoEmps = todoEmps.Replace(WebUser.No + "," + WebUser.Name + ";", "");
                todoEmps = todoEmps.Replace(WebUser.No + "," + WebUser.Name, "");
                // 追加当前操作人
                string emps = this.HisGenerWorkFlow.Emps;
                if (emps.Contains("@" + WebUser.No + "@") == false)
                {
                    emps = emps + WebUser.No + "@";
                }
                this.HisGenerWorkFlow.Emps = emps;
                this.HisGenerWorkFlow.TodoEmps = todoEmps;
                this.HisGenerWorkFlow.Update(GenerWorkFlowAttr.TodoEmps, todoEmps, GenerWorkFlowAttr.Emps, emps);

                /* 如果是协作*/
                if (this.DealTeamUpNode() == true)
                {
                    /*
                     * 1. 判断是否传递过来到达节点，到达人员信息，如果传递过来，就可能是主持人在会签之后执行的发送.
                     * 2. 会签之后执行的发送，就要把到达节点，到达人员存储到数据表里.
                     */

                    if (jumpToNode != null)
                    {
                        /*如果是就记录下来发送到达的节点ID,到达的人员ID.*/
                        this.HisGenerWorkFlow.HuiQianSendToNodeIDStr = this.HisNode.NodeID + "," + jumpToNode.NodeID;
                        if (jumpToEmp == null)
                            this.HisGenerWorkFlow.HuiQianSendToEmps = "";
                        else
                            this.HisGenerWorkFlow.HuiQianSendToEmps = jumpToEmp;

                        this.HisGenerWorkFlow.Update();
                    }

                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs, null);
                    this.HisMsgObjs.AddMsg("info1", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                    //执行时效考核.
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);

                    this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                    //设置当前的流程所有的用时.
                    this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                    this.rptGe.Update();

                    return this.HisMsgObjs;
                }
                this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
                //取出来已经存储的到达节点，节点人员信息. 在tempUp模式的会签时，主持人发送会把发送到节点，发送给人员的信息
                // 存储到wf_generworkflow里面.
                if (this.JumpToNode == null)
                {
                    /* 如果是就记录下来发送到达的节点ID,到达的人员ID.*/
                    string strs = this.HisGenerWorkFlow.HuiQianSendToNodeIDStr;

                    if (strs.Contains(",") == true)
                    {
                        string[] ndStrs = strs.Split(',');
                        int fromNodeID = int.Parse(ndStrs[0]);
                        int toNodeID = int.Parse(ndStrs[1]);
                        if (fromNodeID == this.HisNode.NodeID)
                        {
                            JumpToNode = new Node(toNodeID);
                            JumpToEmp = this.HisGenerWorkFlow.HuiQianSendToEmps;
                        }
                    }
                }
            }

            //如果是协作组长模式节点, 就判断当前的队列人员是否走完.
            if (this.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                /* 如果是协作组长模式.*/
                if (this.DealTeamupGroupLeader() == true)
                {
                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs);
                    this.HisMsgObjs.AddMsg("info1", sendSuccess, sendSuccess, SendReturnMsgType.Info);
                    this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                    //设置当前的流程所有的用时.
                    this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                    this.rptGe.Update();
                    return this.HisMsgObjs;
                }
                this.HisGenerWorkFlow.HuiQianTaskSta = HuiQianTaskSta.None;
            }

            //如果当前节点是子线程，如果合流节点是退回状态，就要冻结子线程的发送动作。
            if (this.HisNode.HisNodeWorkType == NodeWorkType.SubThreadWork)
            {
                GenerWorkFlow gwfMain = new GenerWorkFlow(this.HisGenerWorkFlow.FID);
                if (gwfMain.WFState == WFState.ReturnSta)
                    throw new Exception(BP.WF.Glo.multilingual("err@发送错误:当前流程已经被退回，您不能执行发送操作。技术信息:当前工作节点是子线程状态，主线程是退回状态。", "WorkNode", "send_error_1"));
            }

            //为台州处理 抢办模式下发送后提示给其他人信息.
            if (this.HisNode.TodolistModel == TodolistModel.QiangBan
                && this.HisNode.QiangBanSendAfterRole != QiangBanSendAfterRole.None
                && this.HisGenerWorkFlow.TodoEmpsNum > 1)
            {
                //查询出来当前节点的人员.
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                    GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

                string emps = "";
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.EmpNo.Equals(this.Execer) == true)
                        continue; //如果当前人员，就排除掉. 

                    //要抄送给其他人.
                    if (this.HisNode.QiangBanSendAfterRole == QiangBanSendAfterRole.CCToEtcEmps)
                        emps += item.EmpNo + ",";

                    //要发送消息给其他人.
                    if (this.HisNode.QiangBanSendAfterRole == QiangBanSendAfterRole.SendMsgToEtcEmps)
                        Dev2Interface.Port_SendMsg(item.EmpNo, this.HisGenerWorkFlow.Title + "(被[" + WebUser.Name + "]抢办)", "", "QiangBan");
                }

                if (this.HisNode.QiangBanSendAfterRole == QiangBanSendAfterRole.CCToEtcEmps)
                    Dev2Interface.Node_CCTo(this.WorkID, emps);
            }

            // 启动事务,这里没有实现,在后面做的代码补偿.
            DBAccess.DoTransactionBegin();
            try
            {
                if (this.HisNode.ItIsStartNode)
                    InitStartWorkDataV2(); // 初始化开始节点数据, 如果当前节点是开始节点.

                //处理发送人，把发送人的信息放入wf_generworkflow 2015-01-14. 原来放入WF_GenerWorkerlist.
                if (this.HisGenerWorkFlow.Sender.Contains(",") == false)
                    oldSender = this.HisGenerWorkFlow.Sender; //旧发送人,在回滚的时候把该发送人赋值给他.
                else
                    oldSender = this.HisGenerWorkFlow.Sender.Split(',')[0];
                this.HisGenerWorkFlow.Sender = WebUser.No + "," + WebUser.Name + ";";

                #region 处理退回的情况.
                if (this.HisGenerWorkFlow.WFState == WFState.ReturnSta)
                {
                    #region 当前节点是分流节点但是是子线程退回的节点,需要直接发送给子线程
                    if ((this.HisNode.HisRunModel == RunModel.FL || this.HisNode.HisRunModel == RunModel.FHL) && this.HisGenerWorkFlow.FID != 0 && this.JumpToNode == null)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT NDFrom,EmpFrom,EmpFromT FROM ND" + Int32.Parse(this.HisNode.FlowNo) + "Track WHERE ActionType IN(2,201) AND WorkID=" + dbStr + "WorkID  ORDER BY RDT DESC";
                        ps.Add(TrackAttr.WorkID, this.WorkID);
                        DataTable mydt11 = DBAccess.RunSQLReturnTable(ps);
                        if (mydt11.Rows.Count == 0)
                            throw new Exception(BP.WF.Glo.multilingual("@没有找到退回流程的记录.", "WorkNode", "not_found_my_expected_data", new string[0]));

                        this.JumpToNode = new Node(int.Parse(mydt11.Rows[0][0].ToString()));
                        this.JumpToEmp = mydt11.Rows[0][1].ToString();
                        string toEmpName = mydt11.Rows[0][2].ToString();

                        /**处理发送的数据*/
                        GenerWorkerList myGwl = new GenerWorkerList();
                        myGwl.EmpNo = WebUser.No;
                        myGwl.NodeID = this.HisNode.NodeID;
                        myGwl.WorkID = this.WorkID;
                        if (myGwl.RetrieveFromDBSources() == 0)
                            throw new Exception(BP.WF.Glo.multilingual("@没有找到自己期望的数据，再退回并发送的时候.", "WorkNode", "not_found_my_expected_data", new string[0]));
                        myGwl.ItIsPass = false;
                        myGwl.PassInt = -2;
                        myGwl.Update();

                        GenerWorkerLists gwls = new GenerWorkerLists();
                        gwls.Retrieve(GenerWorkerListAttr.WorkID, this.HisGenerWorkFlow.WorkID,
                            GenerWorkerListAttr.FK_Node, this.JumpToNode.NodeID, GenerWorkerListAttr.IsPass, 5);
                        if (gwls.Count == 0)
                            throw new Exception(BP.WF.Glo.multilingual("@没有找到退回节点的工作人员列表数据.[WorkID=" + this.HisGenerWorkFlow.WorkID + "]", "WorkNode", "not_found_receiver_expected_data", new string[0]));

                        GenerWorkerList gwl = gwls[0] as GenerWorkerList;

                        #region 要计算当前人员的应完成日期
                        // 计算出来 退回到节点的应完成时间. 
                        DateTime dtOfShould;

                        //增加天数. 考虑到了节假日.             
                        dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.HisNode.TimeLimit,
                            this.HisNode.TimeLimitHH, this.HisNode.TimeLimitMM, this.HisNode.TWay);

                        // 应完成日期.
                        string sdt = DataType.SysDateTimeFormat(dtOfShould);
                        #endregion

                        //更新日期，为了考核. 
                        if (this.HisNode.HisCHWay == CHWay.None)
                            gwl.SDT = "无";
                        else
                            gwl.SDT = sdt;

                        gwl.PassInt = 0;
                        gwl.ItIsPass = false;
                        gwl.Update();

                        GenerWorkerLists ens = new GenerWorkerLists();
                        ens.AddEntity(gwl);
                        this.HisWorkerLists = ens;

                        this.addMsg(SendReturnMsgFlag.VarAcceptersID, gwl.EmpNo, gwl.EmpNo, SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarAcceptersName, gwl.EmpName, gwl.EmpName, SendReturnMsgType.SystemMsg);
                        string[] para = new string[2];
                        para[0] = gwl.EmpNo;
                        para[1] = gwl.EmpName;
                        string str = BP.WF.Glo.multilingual("@当前工作已经发送给退回人({0},{1}).", "WorkNode", "current_work_send_to_returner", para);

                        this.addMsg(SendReturnMsgFlag.OverCurr, str, null, SendReturnMsgType.Info);

                        this.HisGenerWorkFlow.WFState = WFState.Runing;
                        this.HisGenerWorkFlow.NodeID = gwl.NodeID;
                        this.HisGenerWorkFlow.NodeName = gwl.NodeName;

                        this.HisGenerWorkFlow.TodoEmps = gwl.EmpNo + "," + gwl.EmpName + ";";
                        this.HisGenerWorkFlow.TodoEmpsNum = 0;
                        this.HisGenerWorkFlow.TaskSta = TaskSta.None;
                        this.HisGenerWorkFlow.Update();

                        //写入track.
                        this.AddToTrack(ActionType.Forward, this.JumpToEmp, gwl.EmpNo, this.JumpToNode.NodeID, this.JumpToNode.Name, BP.WF.Glo.multilingual("退回后发送", "WorkNode", "send_error_2"));

                        //调用发送成功事件.
                        string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs);

                        this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                        //执行时效考核.
                        Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                        this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                        //设置当前的流程所有的用时.
                        this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                        this.rptGe.Update();

                        return this.HisMsgObjs;
                    }
                    #endregion 当前节点是分流节点但是是子线程退回的节点

                    /* 检查该退回是否是原路返回 ? */
                    ps = new Paras();
                    ps.SQL = "SELECT NDFrom,EmpFrom,EmpFromT FROM ND" + Int32.Parse(this.HisNode.FlowNo) + "Track WHERE ActionType IN(2,201) AND WorkID=" + dbStr + "WorkID  AND NDTo=" + this.HisGenerWorkFlow.NodeID + "   ORDER BY RDT DESC";

                    ps.Add(TrackAttr.WorkID, this.WorkID);
                    DataTable mydt = DBAccess.RunSQLReturnTable(ps);

                    bool isBackTracking = this.HisGenerWorkFlow.GetParaBoolen("IsBackTracking");
                    int returnNodeID = 0;
                    if (mydt.Rows.Count != 0)
                    {
                        returnNodeID = int.Parse(mydt.Rows[0][0].ToString());
                        //isBackTracking = int.Parse(mydt.Rows[0][3].ToString());
                    }
                    if (mydt.Rows.Count != 0 && isBackTracking == true
                        && (this.JumpToNode == null || this.JumpToNode.NodeID == returnNodeID))
                    {
                        //有可能查询出来多个，因为按时间排序了，只取出最后一次退回的，看看是否有退回并原路返回的信息。

                        /*确认这次退回，是退回并原路返回 ,  在这里初始化它的工作人员, 与将要发送的节点. */
                        this.JumpToNode = new Node(int.Parse(mydt.Rows[0][0].ToString()));

                        this.JumpToEmp = mydt.Rows[0][1].ToString();
                        string toEmpName = mydt.Rows[0][2].ToString();

                        #region 如果当前是退回, 并且当前的运行模式是按照流程图运行.
                        if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                        {
                            if (this.JumpToNode.TodolistModel == TodolistModel.Order
                                || this.JumpToNode.TodolistModel == TodolistModel.TeamupGroupLeader
                                || this.JumpToNode.TodolistModel == TodolistModel.Teamup)
                            {
                                /*如果是多人处理节点.*/
                                this.DealReturnOrderTeamup();

                                //写入track.
                                this.AddToTrack(ActionType.Forward, this.JumpToEmp, toEmpName, this.JumpToNode.NodeID, this.JumpToNode.Name, BP.WF.Glo.multilingual("退回后发送", "WorkNode", "send_error_2"));

                                //调用发送成功事件.
                                string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs);

                                this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                                //执行时效考核.
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                                this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                                //设置当前的流程所有的用时.
                                this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                                this.rptGe.Update();
                                return this.HisMsgObjs;
                            }
                        }
                        #endregion 如果当前是退回, 并且当前的运行模式是按照流程图运行.*/

                        #region  如果当前是退回. 并且当前的运行模式按照自由流程设置方式运行
                        if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByWorkerSet)
                        {
                            if (this.HisGenerWorkFlow.TodolistModel == TodolistModel.Order
                                || this.JumpToNode.TodolistModel == TodolistModel.TeamupGroupLeader
                                || this.HisGenerWorkFlow.TodolistModel == TodolistModel.Teamup)
                            {
                                /*如果是多人处理节点.*/
                                this.DealReturnOrderTeamup();

                                //写入track.
                                this.AddToTrack(ActionType.Forward, this.JumpToEmp, toEmpName, this.JumpToNode.NodeID, this.JumpToNode.Name, BP.WF.Glo.multilingual("退回后发送(按照自定义运行模式)", "WorkNode", "send_error_2"));

                                //调用发送成功事件.
                                string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this, this.HisMsgObjs);
                                this.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                                //执行时效考核.
                                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);
                                this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                                //设置当前的流程所有的用时.
                                this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                                this.rptGe.Update();
                                return this.HisMsgObjs;
                            }
                        }
                        #endregion  如果当前是退回. 并且当前的运行模式按照自由流程设置方式运行

                        #region 当前节点不是队列，协助组长，协作模式的处理
                        if (this.JumpToNode.ItIsBackResetAccepter == true)
                        {
                            //重新计算处理人
                            try
                            {
                                WorkNode town = new WorkNode(this.WorkID, this.JumpToNode.NodeID);
                                FindWorker fw = new FindWorker();
                                DataTable empdt = fw.DoIt(this.HisFlow, this, town);
                                string empNos = "";
                                string empName = "";
                                if (empdt != null)
                                {
                                    foreach (DataRow dr in empdt.Rows)
                                    {
                                        empNos += dr["No"].ToString() + ",";
                                    }
                                    this.JumpToEmp = empNos;
                                }

                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                                if (msg.IndexOf("url@") != -1)
                                {
                                    if (DataType.IsNullOrEmpty(jumpToEmp) == false)
                                    {
                                        this.JumpToEmp = jumpToEmp;
                                    }
                                    else
                                    {
                                        throw new Exception(msg);
                                    }
                                }
                                else
                                    throw new Exception(msg);
                            }

                        }
                        # endregion 当前节点不是队列，协助组长，协作模式的处理
                    }

                }
                #endregion 处理退回的情况.

                //做了不可能性的判断.
                if (this.HisGenerWorkFlow.NodeID != this.HisNode.NodeID)
                {
                    /*
                    // 2020-05-21 在计算中心出现一次错误. 节点, 当前的节点，与FK_Flow不再一个流程里面。
                    // 没有找到原因.
                    */
                    string[] para = new string[5];
                    para[0] = this.WorkID.ToString();
                    para[1] = this.HisGenerWorkFlow.NodeID.ToString();
                    para[2] = this.HisGenerWorkFlow.NodeName;
                    para[3] = this.HisNode.NodeID.ToString();
                    para[4] = this.HisNode.Name;
                    throw new Exception(BP.WF.Glo.multilingual("@流程出现错误:工作ID={0},当前活动点({1} {2})与发送点({3} {4})不一致.", "WorkNode", "send_error_3", para));
                }

                // 检查完成条件。
                if (jumpToNode != null && this.HisNode.ItIsEndNode == true)
                {
                    /* 是跳转的情况，并且是最后的节点，就不检查流程完成条件。*/
                }
                else
                {
                    //检查流程完成条件.
                    this.CheckCompleteCondition();
                }

                #region  处理自由流程. add by zhoupeng. 2014-11-23.
                if (jumpToNode == null && this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByWorkerSet)
                {
                    if (this.HisNode.GetParaBoolen(NodeAttr.IsYouLiTai) == true)
                    {
                        // 如果没有指定要跳转到的节点，并且当前处理手工干预的运行状态.
                        _transferCustom = TransferCustom.GetNextTransferCustom(this.WorkID, this.HisNode.NodeID);
                        if (_transferCustom == null)
                        {
                            /* 表示执行到这里结束流程. */
                            this.IsStopFlow = true;

                            this.HisGenerWorkFlow.WFState = WFState.Complete;
                            this.rptGe.WFState = WFState.Complete;
                            string msg1 = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver,
                                BP.WF.Glo.multilingual("流程已经按照设置的步骤成功结束", "WorkNode", "wf_end_success"), this.HisNode, this.rptGe);
                            this.addMsg(SendReturnMsgFlag.End, msg1);
                        }
                        else
                        {
                            this.JumpToNode = new Node(_transferCustom.NodeID);
                            this.JumpToEmp = _transferCustom.Worker;
                            this.HisGenerWorkFlow.TodolistModel = _transferCustom.TodolistModel;
                        }
                    }
                    else
                    {
                        //当前为自由流程，需要先判断它的下一个节点是否为固定节点，为固定节点需要发送给固定节点，为游离态则运行自定义的节点
                        Nodes nds = new Directions().GetHisToNodes(this.HisNode.NodeID, false);
                        if (nds.Count == 0)
                        {
                            /* 表示执行到这里结束流程. */
                            this.IsStopFlow = true;

                            this.HisGenerWorkFlow.WFState = WFState.Complete;
                            this.rptGe.WFState = WFState.Complete;
                            string msg1 = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver,
                                BP.WF.Glo.multilingual("流程已经按照设置的步骤成功结束", "WorkNode", "wf_end_success"), this.HisNode, this.rptGe);
                            this.addMsg(SendReturnMsgFlag.End, msg1);
                        }
                        if (nds.Count == 1)
                        {
                            Node toND = (Node)nds[0];
                            if (toND.GetParaBoolen(NodeAttr.IsYouLiTai) == true)
                            {
                                // 如果没有指定要跳转到的节点，并且当前处理手工干预的运行状态.
                                _transferCustom = TransferCustom.GetNextTransferCustom(this.WorkID, this.HisNode.NodeID);
                                this.JumpToNode = new Node(_transferCustom.NodeID);
                                this.JumpToEmp = _transferCustom.Worker;
                                this.HisGenerWorkFlow.TodolistModel = _transferCustom.TodolistModel;
                            }
                            else
                            {
                                this.JumpToNode = toND;
                            }
                        }
                        if (nds.Count > 1)
                        {
                            //如果都是游离态就按照自由流程运行，否则抛异常
                            foreach (Node nd in nds)
                            {
                                if (nd.GetParaBoolen(NodeAttr.IsYouLiTai) == false)
                                    throw new Exception("err@该流程运行是自由流程，" + this.HisNode.Name + "需要设置方向条件，或者把此节点转向的所有节点设置为游离态");
                            }
                            // 如果没有指定要跳转到的节点，并且当前处理手工干预的运行状态.
                            _transferCustom = TransferCustom.GetNextTransferCustom(this.WorkID, this.HisNode.NodeID);
                            this.JumpToNode = new Node(_transferCustom.NodeID);
                            this.JumpToEmp = _transferCustom.Worker;
                            this.HisGenerWorkFlow.TodolistModel = _transferCustom.TodolistModel;
                        }
                    }
                }
                #endregion  处理自由流程. add by zhoupeng. 2014-11-23.

                // 处理质量考核，在发送前。
                this.DealEval();

                // 加入系统变量.
                if (this.IsStopFlow)
                    this.addMsg(SendReturnMsgFlag.IsStopFlow, "1", BP.WF.Glo.multilingual("流程已经结束", "WorkNode", "wf_end_success"), SendReturnMsgType.Info);
                else
                    this.addMsg(SendReturnMsgFlag.IsStopFlow, "0", BP.WF.Glo.multilingual("流程未结束", "WorkNode", "wf_end_success"), SendReturnMsgType.SystemMsg);

                if (this.IsStopFlow == true)
                {
                    //设置缓存中的流程状态
                    this.HisGenerWorkFlow.WFState = WFState.Complete;
                    this.rptGe.WFState = WFState.Complete;
                    // 执行 自动 启动子流程.
                    CallAutoSubFlow(this.HisNode, 0); //启动本节点上的.

                    //执行考核
                    Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, 0, this.HisGenerWorkFlow.Title);
                    this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

                    //设置当前的流程所有的用时.
                    this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);
                    this.rptGe.Update();

                    //执行抄送. 2020-04-28 修改只要启动抄送规则就执行抄送 
                    CC(this.HisNode);

                    //判断当前流程是否子流程，是否启用该流程结束后，主流程自动运行到下一节点
                    string msg = WorkNodePlus.SubFlowEvent(this);
                    if (DataType.IsNullOrEmpty(msg) == false)
                        this.HisMsgObjs.AddMsg("info", msg, msg, SendReturnMsgType.Info);

                    return HisMsgObjs;
                }

                //@增加发送到子流程的判断.
                if (jumpToNode != null && this.HisNode.FlowNo.Equals(jumpToNode.FlowNo) == false)
                {
                    /*判断是否是延续子流程. */
                    return NodeSendToYGFlow(jumpToNode, jumpToEmp);
                }

                #region 2019-09-25 计算未来处理人.
                if (this.HisNode.ItIsStartNode == true && this.HisFlow.ItIsFullSA == true)
                {

                    FullSA fa = new FullSA();
                    fa.DoIt2023(this); //自动计算接受人.

                    //设置版本号.
                    this.HisGenerWorkFlow.SetPara("SADataVer", this.HisFlow.GetParaString("SADataVer"));
                }
                #endregion 计算未来处理人.

                #region 2019-09-25 计算业务字段存储到 wf_generworkflow atpara字段里，用于显示待办信息.
                if (this.HisNode.ItIsStartNode && DataType.IsNullOrEmpty(this.HisFlow.BuessFields) == false)
                {
                    //存储到表里atPara  @BuessFields=电话^Tel^18992323232;地址^Addr^山东成都;
                    string[] expFields = this.HisFlow.BuessFields.Split(',');
                    string exp = "";
                    Attrs attrs = this.rptGe.EnMap.Attrs;
                    foreach (string item in expFields)
                    {
                        if (DataType.IsNullOrEmpty(item) == true)
                            continue;
                        if (attrs.Contains(item) == false)
                            continue;

                        Attr attr = attrs.GetAttrByKey(item);
                        exp += attr.Desc + "^" + attr.Key + "^" + this.rptGe.GetValStrByKey(item);
                    }
                    this.HisGenerWorkFlow.BuessFields = exp;
                }
                #endregion 计算业务字段存储到 wf_generworkflow atpara字段里，用于显示待办信息.

                #region 第二步: 进入核心的流程运转计算区域. 5*5 的方式处理不同的发送情况.
                // 执行节点向下发送的25种情况的判断.
                this.NodeSend_Send_5_5();

                //通过 55 之后要判断是否要结束流程，如果结束流程就执行相关的更新。
                if (this.IsStopFlow)
                {
                    this.rptGe.WFState = WFState.Complete;
                    this.Func_DoSetThisWorkOver();

                    this.HisGenerWorkFlow.WFState = WFState.Complete;
                    this.HisGenerWorkFlow.Update(); //added by liuxc,2016-10=24,最后节点更新Sender字段
                    //判断当前流程是否子流程，是否启用该流程结束后，主流程自动运行到下一节点
                    string msg = WorkNodePlus.SubFlowEvent(this);
                    if (DataType.IsNullOrEmpty(msg) == false)
                        this.HisMsgObjs.AddMsg("info", msg, msg, SendReturnMsgType.Info);
                    //this.HisMsgObjs.AddMsg("info", msg, msg, SendReturnMsgType.Info);
                }

                if (this.IsStopFlow == false)
                {
                    //如果是退回状态，就把是否原路返回的轨迹去掉.
                    if (this.HisGenerWorkFlow.WFState == WFState.ReturnSta)
                        this.HisGenerWorkFlow.SetPara("IsBackTracking", 0);

                    this.Func_DoSetThisWorkOver();

                    //判断当前流程是子流程，并且启用运行到该节点时主流程自动运行到下一个节点
                    string msg = WorkNodePlus.SubFlowEvent(this);
                    if (DataType.IsNullOrEmpty(msg) == false)
                        this.HisMsgObjs.AddMsg("info", msg, msg, SendReturnMsgType.Info);


                    if (town != null && town.HisNode.HisBatchRole == BatchRole.Group)
                    {
                        this.HisGenerWorkFlow.ItIsCanBatch = true;
                        this.HisGenerWorkFlow.Update();
                    }
                }

                //计算从发送到现在的天数.
                this.rptGe.FlowDaySpan = DataType.GeTimeLimits(this.HisGenerWorkFlow.RDT);
                this.rptGe.FlowEndNode = this.HisGenerWorkFlow.NodeID;
                Int64 fid = this.rptGe.FID;
                this.rptGe.Update();
                #endregion 第二步: 5*5 的方式处理不同的发送情况.

                #region 第三步: 处理发送之后的业务逻辑.
                //把当前节点表单数据copy的流程数据表里.
                this.DoCopyCurrentWorkDataToRpt();

                //处理合理节点的1变N的问题.
                this.CheckFrm1ToN();

                //处理子线程的独立表单向合流节点的独立表单明细表的数据汇总.
                this.CheckFrmHuiZongToDtl();
                #endregion 第三步: 处理发送之后的业务逻辑.

                #region 执行抄送.
                //执行抄送.
                if (this.HisNode.ItIsEndNode == false)
                {
                    //执行抄送
                    CC(this.HisNode);
                }

                DBAccess.DoTransactionCommit(); //提交事务.
                #endregion 处理主要业务逻辑.

                #region 执行 自动 启动子流程.
                CallAutoSubFlow(this.HisNode, 0); //启动本节点上的.
                if (this.town != null)
                {
                    CallAutoSubFlow(this.town.HisNode, 1);
                }
                #endregion 执行启动子流程.

                #region 处理流程数据与业务表的数据同步.
                if (this.HisFlow.DTSWay != DataDTSWay.None)
                    WorkNodePlus.DTSData(this.HisFlow, this.HisGenerWorkFlow, this.rptGe, this.HisNode, this.IsStopFlow);
                #endregion 处理流程数据与业务表的数据同步.

                #region 处理发送成功后的消息提示
                if (this.HisNode.HisTurnToDeal == TurnToDeal.SpecMsg)
                {
                    string htmlInfo = "";
                    string textInfo = "";

                    #region 判断当前处理人员，可否处理下一步工作.
                    if (this.town != null
                        && this.HisRememberMe != null
                        && this.HisRememberMe.Emps.Contains("@" + WebUser.No + "@") == true)
                    {
                        string url = "MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + town.HisNode.NodeID + "&FID=" + this.rptGe.FID;
                        //   htmlInfo = "@<a href='" + url + "' >下一步工作您仍然可以处理，点击这里现在处理。</a>.";
                        textInfo = BP.WF.Glo.multilingual("@下一步工作您仍然可以处理。", "WorkNode", "have_permission_next");
                        this.addMsg(SendReturnMsgFlag.MsgOfText, textInfo, null);
                    }
                    #endregion 判断当前处理人员，可否处理下一步工作.

                    string msgOfSend = this.HisNode.TurnToDealDoc;
                    if (msgOfSend.Contains("@"))
                    {
                        Attrs attrs = this.HisWork.EnMap.Attrs;
                        foreach (Attr attr in attrs)
                        {
                            if (msgOfSend.Contains("@") == false)
                                continue;
                            msgOfSend = msgOfSend.Replace("@" + attr.Key, this.HisWork.GetValStrByKey(attr.Key));
                        }
                    }

                    if (msgOfSend.Contains("@") == true)
                    {
                        /*说明有一些变量在系统运行里面.*/
                        string msgOfSendText = msgOfSend.Clone() as string;
                        foreach (SendReturnObj item in this.HisMsgObjs)
                        {
                            if (DataType.IsNullOrEmpty(item.MsgFlag))
                                continue;

                            if (msgOfSend.Contains("@") == false)
                                break;

                            msgOfSendText = msgOfSendText.Replace("@" + item.MsgFlag, item.MsgOfText);

                            if (item.MsgOfHtml != null)
                                msgOfSend = msgOfSend.Replace("@" + item.MsgFlag, item.MsgOfHtml);
                            else
                                msgOfSend = msgOfSend.Replace("@" + item.MsgFlag, item.MsgOfText);
                        }

                        this.HisMsgObjs.OutMessageHtml = msgOfSend + htmlInfo;
                        this.HisMsgObjs.OutMessageText = msgOfSendText + textInfo;
                    }
                    else
                    {
                        this.HisMsgObjs.OutMessageHtml = msgOfSend;
                        this.HisMsgObjs.OutMessageText = msgOfSend;
                    }

                    //return msgOfSend;
                }
                #endregion 处理发送成功后事件.

                #region 如果需要跳转.
                if (town != null)
                {
                    if (this.town.HisNode.ItIsSubThread == true && this.town.HisNode.ItIsSubThread == true)
                    {
                        this.addMsg(SendReturnMsgFlag.VarToNodeID, town.HisNode.NodeID.ToString(), town.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                        this.addMsg(SendReturnMsgFlag.VarToNodeName, town.HisNode.Name, town.HisNode.Name, SendReturnMsgType.SystemMsg);
                    }

#warning 如果这里设置了自动跳转，现在去掉了. 2014-11-07.
                    //if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousOperSkip)
                    //{
                    //    town.NodeSend();
                    //    this.HisMsgObjs = town.HisMsgObjs;
                    //}
                }
                #endregion 如果需要跳转.

                #region 设置流程的标记.
                if (this.HisNode.ItIsStartNode)
                {
                    if (this.rptGe.PWorkID != 0 && this.HisGenerWorkFlow.PWorkID == 0)
                    {
                        BP.WF.Dev2Interface.SetParentInfo(this.HisFlow.No, this.WorkID, this.rptGe.PWorkID);

                        //写入track, 调用了父流程.
                        Node pND = new Node(rptGe.PNodeID);
                        fid = 0;
                        if (pND.HisNodeWorkType == NodeWorkType.SubThreadWork)
                        {
                            GenerWorkFlow gwf = new GenerWorkFlow(this.rptGe.PWorkID);
                            fid = gwf.FID;
                        }

                        string paras = "@CFlowNo=" + this.HisFlow.No + "@CWorkID=" + this.WorkID;

                        Glo.AddToTrack(ActionType.StartChildenFlow, rptGe.PFlowNo, rptGe.PWorkID, fid, pND.NodeID, pND.Name,
                            WebUser.No, WebUser.Name,
                            pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                            "<a href='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/WFRpt.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "' target=_blank >打开子流程</a>", paras);
                    }
                    else if (BP.Difference.SystemConfig.isBSsystem == true)
                    {
                        /*如果是BS系统*/
                        string pflowNo = HttpContextHelper.RequestParams("PFlowNo");
                        if (DataType.IsNullOrEmpty(pflowNo) == false)
                        {
                            string pWorkID = HttpContextHelper.RequestParams("PWorkID");// BP.Sys.Base.Glo.Request.QueryString["PWorkID"];
                            string pNodeID = HttpContextHelper.RequestParams("PNodeID");// BP.Sys.Base.Glo.Request.QueryString["PNodeID"];
                            string pEmp = HttpContextHelper.RequestParams("PEmp");// BP.Sys.Base.Glo.Request.QueryString["PEmp"];

                            // 设置成父流程关系.
                            BP.WF.Dev2Interface.SetParentInfo(this.HisFlow.No, this.WorkID, Int64.Parse(pWorkID));

                            //写入track, 调用了父流程.
                            Node pND = new Node(pNodeID);
                            fid = 0;
                            if (pND.HisNodeWorkType == NodeWorkType.SubThreadWork)
                            {
                                GenerWorkFlow gwf = new GenerWorkFlow(Int64.Parse(pWorkID));
                                fid = gwf.FID;
                            }
                            string paras = "@CFlowNo=" + this.HisFlow.No + "@CWorkID=" + this.WorkID;
                            Glo.AddToTrack(ActionType.StartChildenFlow, pflowNo, Int64.Parse(pWorkID), Int64.Parse(fid.ToString()), pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                                pND.NodeID, pND.Name, WebUser.No, WebUser.Name,
                                "<a href='" + BP.Difference.SystemConfig.HostURLOfBS + "/WF/WFRpt.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "' target=_blank >" + BP.WF.Glo.multilingual("打开子流程", "WorkNode", "open_sub_wf") + "</a>", paras);
                        }
                    }
                }
                #endregion 设置流程的标记.

                //执行时效考核.
                Glo.InitCH(this.HisFlow, this.HisNode, this.WorkID, this.rptGe.FID, this.rptGe.Title);

                #region 触发下一个节点的自动发送, 处理国机的需求.  （去掉:2019-05-05）
                if (this.HisMsgObjs.VarToNodeID != null
                    && this.town != null
                    && 1 == 2
                    && this.town.HisNode.WhoExeIt != 0)
                {
                    string currUser = BP.Web.WebUser.No;
                    string[] myEmpStrs = this.HisMsgObjs.VarAcceptersID.Split(',');
                    foreach (string emp in myEmpStrs)
                    {
                        if (DataType.IsNullOrEmpty(emp))
                            continue;

                        try
                        {
                            //让这个人登录.
                            BP.Port.Emp empEn = new Emp(emp);
                            BP.WF.Dev2Interface.Port_Login(emp);
                            if (this.HisNode.ItIsSubThread == true
                                && this.town.HisNode.ItIsSubThread == false)
                            {
                                /*如果当前的节点是子线程，并且发送到的节点非子线程。
                                 * 就是子线程发送到非子线程的情况。
                                 */
                                this.HisMsgObjs = BP.WF.Dev2Interface.Node_SendWork(this.HisNode.FlowNo, this.HisWork.FID);
                            }
                            else
                            {
                                this.HisMsgObjs = BP.WF.Dev2Interface.Node_SendWork(this.HisNode.FlowNo, this.HisWork.OID);
                            }
                        }
                        catch
                        {
                            // 可能是正常的阻挡发送，操作不必提示。
                            //this.HisMsgObjs.AddMsg("Auto"
                        }
                        BP.WF.Dev2Interface.Port_Login(currUser);
                        //使用一个人处理就可以了.
                        break;
                    }
                }
                #endregion 触发下一个节点的自动发送。

                #region 判断当前处理人员，可否处理下一步工作.
                if (this.IsStopFlow == false && this.town != null
                    && this.HisRememberMe != null
                    && this.HisRememberMe.Emps.Contains("@" + WebUser.No + "@") == true)
                {
                    string url = "MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + town.HisNode.NodeID + "&FID=" + this.rptGe.FID;
                    //    string htmlInfo = "@<a href='" + url + "' >下一步工作您仍然可以处理，点击这里现在处理。</a>.";
                    string textInfo = BP.WF.Glo.multilingual("@下一步工作您仍然可以处理。", "WorkNode", "have_permission_next");
                }
                #endregion 判断当前处理人员，可否处理下一步工作.


                string userNo = BP.Web.WebUser.No;
                HttpContext ctx = HttpContextHelper.Current;
                new Thread(() =>
                {
                    try
                    {
                        HttpContext.Current = ctx;
                        Stopwatch eventSW = new Stopwatch();
                        eventSW.Start();
                        this.Deal_Event();
                        eventSW.Stop();
                        BP.DA.Log.DebugWriteInfo("Deal Event 执行时间：" + eventSW.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DebugWriteError("Deal Event 出现错误：" + ex.Message);
                    }
                }).Start();


                //返回这个对象.
                return this.HisMsgObjs;
            }
            catch (Exception ex)
            {
                //当下一个节点的接收人规则为上一个人员选择的时候，就会抛出人员选择器链接,让其选择人员.
                if (ex.Message.IndexOf("url@") == 0)
                    throw new Exception(ex.Message);

                this.WhenTranscactionRollbackError(ex);

                DBAccess.DoTransactionRollback();

                BP.DA.Log.DebugWriteError(ex.StackTrace);

                throw new Exception(ex.Message);
                //throw new Exception(ex.Message + "  tech@info:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 自动启动子流程
        /// </summary>
        public void CallAutoSubFlow(Node nd, int invokeTime)
        {
            //自动发起流程的数量.
            //if (nd.SubFlowAutoNum == 0)
            //    return;

            SubFlowAutos subs = new SubFlowAutos(nd.NodeID);
            if (subs.Count == 0)
                return;

            foreach (SubFlowAuto sub in subs)
            {
                if (sub.InvokeTime != invokeTime)
                    continue;

                //启动下级子流程.
                if (sub.HisSubFlowModel == SubFlowModel.SubLevel)
                {
                    #region 判断启动权限.
                    if (sub.StartOnceOnly == true)
                    {
                        /* 如果仅仅被启动一次.*/
                        string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.WorkID + " AND FK_Flow='" + sub.SubFlowNo + "'";
                        if (DBAccess.RunSQLReturnValInt(sql) > 0)
                            continue; //已经启动了，就不启动了。
                    }

                    if (sub.CompleteReStart == true)
                    {
                        /* 该子流程启动的流程运行结束后才可以启动.*/
                        string sql = "SELECT Starter, RDT,WFState FROM WF_GenerWorkFlow WHERE PWorkID=" + this.WorkID + " AND FK_Flow='" + sub.SubFlowNo + "' AND WFSta !=" + (int)WFSta.Complete;
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 1 && Int32.Parse(dt.Rows[0]["WFState"].ToString()) != 0)
                            continue;//已经启动的流程运行没有结束了，就不启动了。 WFState 是草稿
                    }
                    //指定的流程启动后,才能启动该子流程。
                    if (sub.ItIsEnableSpecFlowStart == true)
                    {
                        string[] fls = sub.SpecFlowStart.Split(',');
                        bool isHave = false;
                        foreach (string fl in fls)
                        {
                            if (DataType.IsNullOrEmpty(fl) == true)
                                continue;

                            string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.WorkID + " AND FK_Flow='" + fl + "'";
                            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                            {
                                isHave = true;
                                break; //还没有启动过.
                            }
                        }
                        if (isHave == true)
                            continue; //就不能启动该子流程.
                    }

                    //指定的流程结束后,才能启动该子流程。
                    if (sub.ItIsEnableSpecFlowOver == true)
                    {
                        string[] fls = sub.SpecFlowOver.Split(',');
                        bool isHave = false;
                        foreach (string fl in fls)
                        {
                            if (DataType.IsNullOrEmpty(fl) == true)
                                continue;

                            string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.WorkID + " AND FK_Flow='" + fl + "' AND WFState=3";
                            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                            {
                                isHave = true;
                                break; //还没有启动过/或者没有完成.
                            }
                        }
                        if (isHave == true)
                            continue; //就不能启动该子流程.
                    }

                    if (sub.ItIsEnableSQL == true)
                    {
                        string sql = sub.SpecSQL;
                        if (DataType.IsNullOrEmpty(sql) == true)
                            continue;

                        sql = BP.WF.Glo.DealExp(sql, this.rptGe);
                        if (DBAccess.RunSQLReturnValInt(sql) == 0) //不能执行子流程
                            continue;
                    }

                    //按指定子流程节点
                    if (sub.ItIsEnableSameLevelNode == true)
                        throw new Exception("配置错误，按指定平级子流程节点只使用触发平级子流程，不能触发下级子流程");
                    #endregion

                    #region 判断数据源类型.0.仅仅发起一次，使用当前表单数据源.
                    if (sub.DBSrcType == 0)
                    {
                        #region 检查 SendModel.
                        // 设置开始节点待办.
                        if (sub.SendModel == 0)
                        {
                            //创建workid.
                            Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                            //设置父子关系.
                            BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.WorkID, WebUser.No, nd.NodeID);

                            //执行保存.
                            BP.WF.Dev2Interface.Node_SaveWork(subWorkID, this.rptGe.Row);

                            //为开始节点设置待办.
                            BP.WF.Dev2Interface.Node_AddTodolist(subWorkID, WebUser.No);

                            BP.WF.Dev2Interface.Flow_ReSetFlowTitle(subWorkID);

                            //写入消息.
                            this.addMsg("SubFlow" + sub.SubFlowNo, "流程[" + sub.FlowName + "]启动成功.");
                        }

                        //发送到下一个环节去.
                        if (sub.SendModel == 1)
                        {
                            //创建workid.
                            Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                            //设置父子关系.
                            BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.WorkID, null, 0, true);

                            //执行发送到下一个环节..
                            SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(sub.SubFlowNo, subWorkID, this.rptGe.Row, null);
                            this.addMsg("SubFlow" + sub.SubFlowNo, sendObjs.ToMsgOfHtml());
                        }

                        if (sub.SubFlowHidTodolist == true)
                        {
                            //发送子流程后不显示父流程待办，设置父流程已经的待办已经处理 100
                            int nodeID = 0;
                            if (nd.NodeID == this.town.HisNode.NodeID)
                                nodeID = nd.NodeID;
                            else
                                nodeID = this.town.HisNode.NodeID;
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=100 Where WorkID=" + this.HisGenerWorkFlow.WorkID + " AND FK_Node=" + nodeID);
                        }
                        #endregion 检查sendModel.
                    }
                    #endregion 判断数据源类型.0.仅仅发起一次，使用当前表单数据源.

                    #region 判断数据源类型.1.使用SQL数据源发起流程.
                    if (sub.DBSrcType == 1)
                    {
                        string sql = sub.DBSrcDoc;
                        if (DataType.IsNullOrEmpty(sql) == true)
                            continue;

                        sql = BP.WF.Glo.DealExp(sql, this.rptGe);
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        Hashtable ht = this.rptGe.Row;

                        //遍历数据源.
                        foreach (DataRow dr in dt.Rows)
                        {
                            foreach (DataColumn dc in dt.Columns)
                            {
                                if (dt.Columns.Contains(dc.ColumnName))
                                    ht[dc.ColumnName] = dr[dc.ColumnName];
                                else
                                    ht.Add(dc.ColumnName, dr[dc.ColumnName]);
                            }

                            #region 检查 SendModel.
                            // 设置开始节点待办.
                            if (sub.SendModel == 0)
                            {
                                //创建workid.
                                Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                                //设置父子关系.
                                BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.WorkID, WebUser.No, nd.NodeID);

                                //执行保存.
                                BP.WF.Dev2Interface.Node_SaveWork(subWorkID, ht);

                                //为开始节点设置待办.
                                BP.WF.Dev2Interface.Node_AddTodolist(subWorkID, WebUser.No);

                                BP.WF.Dev2Interface.Flow_ReSetFlowTitle(subWorkID);

                                //写入消息.
                                this.addMsg("SubFlow" + sub.SubFlowNo, "流程[" + sub.FlowName + "]启动成功.");
                            }

                            //发送到下一个环节去.
                            if (sub.SendModel == 1)
                            {
                                //创建workid.
                                Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                                //设置父子关系.
                                BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.WorkID, null, 0, true);

                                //执行发送到下一个环节..
                                SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(sub.SubFlowNo, subWorkID, ht, null);
                                this.addMsg("SubFlow" + sub.SubFlowNo, sendObjs.ToMsgOfHtml());
                            }

                            if (sub.SubFlowHidTodolist == true)
                            {
                                //发送子流程后不显示父流程待办，设置父流程已经的待办已经处理 100
                                int nodeID = 0;
                                if (nd.NodeID == this.town.HisNode.NodeID)
                                    nodeID = nd.NodeID;
                                else
                                    nodeID = this.town.HisNode.NodeID;
                                DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=100 Where WorkID=" + this.HisGenerWorkFlow.WorkID + " AND FK_Node=" + nodeID);
                            }
                            #endregion 检查sendModel.

                        } //结束循环.

                    }
                    #endregion 判断数据源类型.1.使用SQL数据源发起流程

                }

                //如果要自动启动平级的子流程，就需要判断当前是是否是子流程，如果不是子流程，就不能启动。
                if (sub.HisSubFlowModel == SubFlowModel.SameLevel && this.HisGenerWorkFlow.PWorkID != 0)
                {
                    #region 判断启动权限.
                    if (sub.StartOnceOnly == true)
                    {
                        /* 如果仅仅被启动一次.*/
                        string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.HisGenerWorkFlow.PWorkID + " AND FK_Flow='" + sub.SubFlowNo + "'";
                        if (DBAccess.RunSQLReturnValInt(sql) > 0)
                            continue; //已经启动了，就不启动了。
                    }

                    if (sub.CompleteReStart == true)
                    {
                        /* 该子流程启动的流程运行结束后才可以启动.*/
                        string sql = "SELECT Starter, RDT,WFState FROM WF_GenerWorkFlow WHERE PWorkID=" + this.HisGenerWorkFlow.PWorkID + " AND FK_Flow='" + sub.SubFlowNo + "' AND WFSta !=" + (int)WFSta.Complete;
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 1 && Int32.Parse(dt.Rows[0]["WFState"].ToString()) != 0)
                            continue;//已经启动的流程运行没有结束了，就不启动了。 WFState 0是草稿可以发起
                    }


                    //指定的流程启动后,才能启动该子流程。
                    if (sub.ItIsEnableSpecFlowStart == true)
                    {
                        string[] fls = sub.SpecFlowStart.Split(',');
                        bool isHave = false;
                        foreach (string fl in fls)
                        {
                            if (DataType.IsNullOrEmpty(fl) == true)
                                continue;

                            string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.HisGenerWorkFlow.PWorkID + " AND FK_Flow='" + fl + "'";
                            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                            {
                                isHave = true;
                                break; //还没有启动过.
                            }
                        }
                        if (isHave == true)
                            continue; //就不能启动该子流程.
                    }

                    if (sub.ItIsEnableSpecFlowOver == true)
                    {
                        string[] fls = sub.SpecFlowOver.Split(',');
                        bool isHave = false;
                        foreach (string fl in fls)
                        {
                            if (DataType.IsNullOrEmpty(fl) == true)
                                continue;

                            string sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + this.HisGenerWorkFlow.PWorkID + " AND FK_Flow='" + fl + "' AND WFState=3";
                            if (DBAccess.RunSQLReturnValInt(sql) == 0)
                            {
                                isHave = true;
                                break; //还没有启动过.
                            }
                        }
                        if (isHave == true)
                            continue; //就不能启动该子流程.
                    }
                    //按指定的SQL配置，如果结果值是>=1就执行
                    if (sub.ItIsEnableSQL == true)
                    {
                        string sql = sub.SpecSQL;
                        if (DataType.IsNullOrEmpty(sql) == true)
                            continue;

                        sql = BP.WF.Glo.DealExp(sql, this.rptGe);
                        if (DBAccess.RunSQLReturnValInt(sql) == 0) //不能执行子流程
                            continue;
                    }

                    //按指定子流程节点
                    if (sub.ItIsEnableSameLevelNode == true)
                    {
                        string levelNodes = sub.SameLevelNode;
                        if (DataType.IsNullOrEmpty(levelNodes) == true)
                            continue;

                        string[] nodes = levelNodes.Split(';');
                        bool isHave = false;
                        foreach (string val in nodes)
                        {
                            string[] flowNode = val.Split(',');
                            if (flowNode.Length != 2)
                            {
                                isHave = true;
                                break; //不能启动.
                            }


                            GenerWorkFlow gwfSub = new GenerWorkFlow();
                            int count = gwfSub.Retrieve(GenerWorkFlowAttr.PWorkID, this.HisGenerWorkFlow.PWorkID, GenerWorkFlowAttr.FK_Flow, flowNode[0]);
                            if (count == 0)
                            {
                                isHave = true;
                                break; //不能启动.
                            }
                            if (gwfSub.WFSta != WFSta.Complete)
                            {
                                //判断该节点是不是子线程
                                Node subNode = new Node(int.Parse(flowNode[1]));
                                string sql = "";
                                if (subNode.ItIsSubThread == true)
                                    sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkerlist WHERE FID=" + gwfSub.WorkID + " AND FK_Flow='" + flowNode[0] + "' AND FK_Node=" + int.Parse(flowNode[1]) + " AND IsEnable=1 AND IsPass=1";
                                else
                                    sql = "SELECT COUNT(*) as Num FROM WF_GenerWorkerlist WHERE WorkID=" + gwfSub.WorkID + " AND FK_Flow='" + flowNode[0] + "' AND FK_Node=" + int.Parse(flowNode[1]) + " AND IsEnable=1 AND IsPass=1";
                                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                                {
                                    isHave = true;
                                    break; //不能启动.
                                }
                            }

                        }
                        if (isHave == true)
                            continue;

                    }
                    #endregion

                    #region 检查sendModel.
                    // 设置开始节点待办.
                    if (sub.SendModel == 0)
                    {
                        //创建workid.
                        Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                        //设置父子关系.
                        BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.PWorkID, WebUser.No, nd.NodeID, true);

                        //执行保存.
                        BP.WF.Dev2Interface.Node_SaveWork(subWorkID, this.rptGe.Row);

                        //为开始节点设置待办.
                        BP.WF.Dev2Interface.Node_AddTodolist(subWorkID, WebUser.No);

                        BP.WF.Dev2Interface.Flow_ReSetFlowTitle(subWorkID);


                        //增加启动该子流程的同级子流程信息
                        GenerWorkFlow gwf = new GenerWorkFlow(subWorkID);
                        gwf.SetPara("SLFlowNo", this.HisNode.FlowNo);
                        gwf.SetPara("SLNodeID", this.HisNode.NodeID);
                        gwf.SetPara("SLWorkID", this.HisGenerWorkFlow.WorkID);
                        gwf.Update();

                        //写入消息.
                        this.addMsg("SubFlow" + sub.SubFlowNo, "流程[" + sub.FlowName + "]启动成功.");
                    }

                    //发送到下一个环节去.
                    if (sub.SendModel == 1)
                    {
                        //创建workid.
                        Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(sub.SubFlowNo, WebUser.No);

                        //设置父子关系.
                        BP.WF.Dev2Interface.SetParentInfo(sub.SubFlowNo, subWorkID, this.HisGenerWorkFlow.PWorkID, WebUser.No, nd.NodeID);

                        //增加启动该子流程的同级子流程信息
                        GenerWorkFlow gwf = new GenerWorkFlow(subWorkID);
                        gwf.SetPara("SLFlowNo", this.HisNode.FlowNo);
                        gwf.SetPara("SLNodeID", this.HisNode.NodeID);
                        gwf.SetPara("SLWorkID", this.HisGenerWorkFlow.WorkID);
                        gwf.Update();


                        //执行发送到下一个环节..
                        SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(sub.SubFlowNo, subWorkID, this.rptGe.Row, null);

                        this.addMsg("SubFlow" + sub.SubFlowNo, sendObjs.ToMsgOfHtml());
                    }
                    #endregion 检查sendModel.

                }
            }

            return;
        }
        /// <summary>
        /// 处理事件
        /// </summary>
        private void Deal_Event()
        {
            #region 处理节点到达事件..
            //执行发送到达事件.
            if (this.IsStopFlow == false && this.town != null)
                ExecEvent.DoNode(EventListNode.WorkArrive, this.town);
            #endregion 处理节点到达事件.

            #region 处理发送成功后事件.
            try
            {
                //调起发送成功后的事件，把参数传入进去。
                if (this.SendHTOfTemp != null)
                {
                    foreach (string key in this.SendHTOfTemp.Keys)
                    {
                        if (rptGe.Row.ContainsKey(key) == true)
                            this.rptGe.Row[key] = this.SendHTOfTemp[key] as string;
                        else
                            this.rptGe.Row.Add(key, this.SendHTOfTemp[key] as string);
                    }
                }

                //执行发送.
                string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, this);
                if (sendSuccess != null)
                    this.addMsg(SendReturnMsgFlag.SendSuccessMsg, sendSuccess);
            }
            catch (Exception ex)
            {
                this.addMsg(SendReturnMsgFlag.SendSuccessMsgErr, "err@执行事件出现SendSuccessMsgErr：" + ex.Message);
            }
            #endregion 处理发送成功后事件.
        }
        /// <summary>
        /// 手工的回滚提交失败信息，补偿没有事务的缺陷。
        /// </summary>
        /// <param name="ex"></param>
        private void WhenTranscactionRollbackError(Exception ex)
        {
            /*在提交错误的情况下，回滚数据。*/

            #region 如果是分流点下同表单发送失败再次发送就出现错误.
            if (this.town != null
                && this.town.HisNode.HisNodeWorkType == NodeWorkType.SubThreadWork
                && this.town.HisNode.HisRunModel == RunModel.SubThreadSameWorkID)
            {
                /*如果是子线程*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.WorkID + " AND FK_Node=" + this.town.HisNode.NodeID);
                //删除子线程数据.
                if (DBAccess.IsExitsObject(this.town.HisWork.EnMap.PhysicsTable) == true)
                    DBAccess.RunSQL("DELETE FROM " + this.town.HisWork.EnMap.PhysicsTable + " WHERE FID=" + this.WorkID);
            }
            #endregion 如果是分流点下同表单发送失败再次发送就出现错误.

            try
            {
                //有可能删除之前的日志，即退回又运行到该节点，处理的办法是求出轨迹运行的最后处理时间.
                string maxDT = DBAccess.RunSQLReturnStringIsNull("Select Max(RDT) FROM ND" + int.Parse(this.HisFlow.No) + "Track WHERE WorkID=" + this.WorkID, null);
                if (maxDT != null)
                {
                    //删除发生的日志.
                    DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisFlow.No) + "Track WHERE WorkID=" + this.WorkID +
                                    " AND NDFrom=" + this.HisNode.NodeID + " AND ActionType=" + (int)ActionType.Forward +
                                    " AND RDT='" + maxDT + "'");
                }

                // 删除考核信息。
                this.DealEvalUn();

                // 把工作的状态设置回来。
                if (this.HisNode.ItIsStartNode)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Runing + " WHERE OID=" +
                             dbStr + "OID ";
                    ps.Add(GERptAttr.OID, this.WorkID);
                    DBAccess.RunSQL(ps);
                    //  this.HisWork.Update(GERptAttr.WFState, (int)WFState.Runing);
                }

                // 把流程的状态设置回来。
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 0)
                    return;
                //还原WF_GenerWorkList
                if (gwf.WFState == WFState.Complete)
                {
                    string ndTrack = "ND" + int.Parse(this.HisFlow.No) + "Track";
                    string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
                    string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + this.WorkID + " ORDER BY RDT DESC, NDFrom ";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        throw new Exception("@工作ID为:" + this.WorkID + "的数据不存在.");

                    string starter = "";
                    bool isMeetSpecNode = false;
                    GenerWorkerList currWl = new GenerWorkerList();
                    foreach (DataRow dr in dt.Rows)
                    {
                        int ndFrom = int.Parse(dr["NDFrom"].ToString());
                        Node nd = new Node(ndFrom);

                        string ndFromT = dr["NDFromT"].ToString();
                        string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                        string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                        // 增加上 工作人员的信息.
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = this.WorkID;
                        gwl.FlowNo = this.HisFlow.No;

                        gwl.NodeID = ndFrom;
                        gwl.NodeName = ndFromT;

                        if (gwl.NodeID == this.HisNode.NodeID)
                        {
                            gwl.ItIsPass = false;
                            currWl = gwl;
                        }
                        else
                            gwl.ItIsPass = true;

                        gwl.EmpNo = EmpFrom;
                        gwl.EmpName = EmpFromT;
                        if (gwl.IsExits)
                            continue; /*有可能是反复退回的情况.*/

                        Emp emp = new Emp(gwl.EmpNo);
                        gwl.DeptNo = emp.DeptNo;

                        gwl.SDT = dr["RDT"].ToString();
                        gwl.DTOfWarning = gwf.SDTOfNode;

                        gwl.ItIsEnable = true;
                        gwl.WhoExeIt = nd.WhoExeIt;
                        gwl.Insert();
                    }
                }
                else
                {
                    //执行数据.
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE FK_Emp=" + dbStr + "FK_Emp AND WorkID=" + dbStr +
                             "WorkID AND FK_Node=" + dbStr + "FK_Node ";
                    //ps.AddFK_Emp();
                    ps.Add("FK_Emp", WebUser.No);
                    ps.Add("WorkID", this.WorkID);
                    ps.Add("FK_Node", this.HisNode.NodeID);
                    DBAccess.RunSQL(ps);
                }


                if (gwf.WFState != 0 || gwf.NodeID != this.HisNode.NodeID)
                {
                    /* 如果这两项其中有一项有变化。*/
                    gwf.NodeID = this.HisNode.NodeID;
                    gwf.NodeName = this.HisNode.Name;
                    gwf.WFState = WFState.Runing;

                    //设置他的旧发送人.
                    if (DataType.IsNullOrEmpty(oldSender) == false)
                    {
                        Emp emp = new Emp(oldSender);
                        this.HisGenerWorkFlow.Sender = emp.UserID + "," + emp.Name + ";";
                    }
                    gwf.Update();
                }

                // Node startND = this.HisNode.HisFlow.HisStartNode;

                Nodes nds = this.HisNode.HisToNodes;
                foreach (Node nd in nds)
                {
                    if (nd.NodeID == this.HisNode.NodeID)
                        continue;

                    Work mwk = nd.HisWork;
                    if (mwk.EnMap.PhysicsTable == this.HisFlow.PTable
                        || mwk.EnMap.PhysicsTable == this.HisWork.EnMap.PhysicsTable)
                        continue;

                    mwk.OID = this.WorkID;
                    try
                    {
                        mwk.DirectDelete();
                    }
                    catch
                    {
                        mwk.CheckPhysicsTable();
                        mwk.DirectDelete();
                    }
                }

                //执行发送失败事件，让开发人员回滚相关数据.
                ExecEvent.DoNode(EventListNode.SendError, this);

            }
            catch (Exception ex1)
            {
                if (this.town != null && this.town.HisWork != null)
                    this.town.HisWork.CheckPhysicsTable();

                if (this.rptGe != null)
                    this.rptGe.CheckPhysicsTable();
                string er1 = BP.WF.Glo.multilingual("@发送失败后,回滚发送失败数据出现错误:{0}.", "WorkNode", "wf_eng_error_4", ex1.StackTrace);
                string er2 = BP.WF.Glo.multilingual("@回滚发送失败数据出现错误:{0}.", "WorkNode", "wf_eng_error_3");
                throw new Exception(ex.Message + er1 + er2);
            }
        }
        #endregion

        #region 用户到的变量
        public GenerWorkerLists HisWorkerLists = null;
        private GenerWorkFlow _HisGenerWorkFlow;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow == null && this.WorkID != 0)
                {

                    _HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);


                    SendNodeWFState = _HisGenerWorkFlow.WFState; //设置发送前的节点状态。
                }
                return _HisGenerWorkFlow;
            }
            set
            {
                _HisGenerWorkFlow = value;
            }
        }
        private Int64 _WorkID = 0;
        /// <summary>
        /// 工作ID.
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// 原来的发送人.
        /// </summary>
        private string oldSender = null;
        #endregion


        public GERpt rptGe = null;
        private void InitStartWorkDataV2()
        {
            //判断开始节点是不是流程发起的节点，有可能是退回或者其他流程流转过来的节点数据
            bool isStart = true;
            if (DataType.IsNullOrEmpty(this.HisGenerWorkFlow.Sender) == false)
                isStart = false;
            if (isStart == false)
                return;
            if (isStart == true)
                this.rptGe.SetValByKey("RDT", DataType.CurrentDateTimess);

            /*如果是开始流程判断是不是被吊起的流程，如果是就要向父流程写日志。*/
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                string fk_nodeFrom = HttpContextHelper.RequestParams("FromNode");// BP.Sys.Base.Glo.Request.QueryString["FromNode"];
                if (DataType.IsNullOrEmpty(fk_nodeFrom) == false)
                {
                    Node ndFrom = new Node(int.Parse(fk_nodeFrom));
                    string PWorkID = HttpContextHelper.RequestParams("PWorkID");
                    if (DataType.IsNullOrEmpty(PWorkID))
                        PWorkID = HttpContextHelper.RequestParams("PWorkID");//BP.Sys.Base.Glo.Request.QueryString["PWorkID"];

                    string pTitle = DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM  ND" + int.Parse(ndFrom.FlowNo) + "01 WHERE OID=" + PWorkID, "");

                    ////记录当前流程被调起。
                    //  this.AddToTrack(ActionType.StartSubFlow, WebUser.No,
                    //  WebUser.Name, ndFrom.NodeID, ndFrom.FlowName + "\t\n" + ndFrom.FlowName, "被父流程(" + ndFrom.FlowName + ":" + pTitle + ")调起.");

                    //记录父流程被调起。
                    string st1 = BP.WF.Glo.multilingual("{0}发起工作流{1}", "WorkNode", "start_wf", this.ExecerName, ndFrom.FlowName);
                    string st2 = BP.WF.Glo.multilingual("发起子流程:{0}", "WorkNode", "start_sub_wf", this.HisFlow.Name);
                    BP.WF.Dev2Interface.WriteTrack(this.HisFlow.No, this.HisNode.NodeID, this.HisNode.Name, this.WorkID, 0,
                        st1, ActionType.CallChildenFlow, "@PWorkID=" + PWorkID + "@PFlowNo=" + ndFrom.HisFlow.No, st2, null);
                }
            }

            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET CDT='" + DataType.CurrentDateTime + "',SDT='' , Sender='" + WebUser.No + "," + WebUser.Name + "',IsPass=1,IsRead=1 WHERE FK_Emp='" + WebUser.No + "' AND FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID);


            // 再一次生成单据编号.
            if (DataType.IsNullOrEmpty(this.HisFlow.BillNoFormat) == false)
            {
                this.HisGenerWorkFlow.BillNo = WorkFlowBuessRole.GenerBillNo(this.HisFlow.BillNoFormat, this.WorkID, this.rptGe, this.HisFlow.PTable);
                this.rptGe.BillNo = this.HisGenerWorkFlow.BillNo;
                this.HisWork.SetValByKey("BillNo", this.HisGenerWorkFlow.BillNo);
                if (DataType.IsNullOrEmpty(this.HisFlow.TitleRole) == false && this.HisFlow.TitleRole.Contains("@BillNo"))
                {
                    string title = WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork);
                    if (title.Contains("@") == true)
                        title = WorkFlowBuessRole.GenerTitle(this.HisFlow, this.rptGe);
                    this.HisGenerWorkFlow.Title = title;
                    this.rptGe.Title = title;
                }
            }
            /* 产生开始工作流程记录. */
            #region 设置流程标题.
            if (this.HisGenerWorkFlow.Title == null)
                this.HisGenerWorkFlow.Title = BP.WF.WorkFlowBuessRole.GenerTitle(this.HisFlow, this.HisWork);

            //流程标题.
            this.rptGe.Title = this.HisGenerWorkFlow.Title;
            #endregion 设置流程标题.

            this.HisWork.SetValByKey("Title", this.HisGenerWorkFlow.Title);
            if (isStart == true)
                this.HisGenerWorkFlow.RDT = DataType.CurrentDateTimess;  // this.HisWork.RDT;
            if (this.HisGenerWorkFlow.WFState == WFState.Runing)
            {
                this.HisGenerWorkFlow.Starter = this.Execer;
                this.HisGenerWorkFlow.StarterName = this.ExecerName;
            }
            this.HisGenerWorkFlow.FlowNo = this.HisNode.FlowNo;
            this.HisGenerWorkFlow.FlowName = this.HisNode.FlowName;
            this.HisGenerWorkFlow.FlowSortNo = this.HisNode.HisFlow.FlowSortNo;
            this.HisGenerWorkFlow.SysType = this.HisNode.HisFlow.SysType;
            this.HisGenerWorkFlow.NodeID = this.HisNode.NodeID;
            this.HisGenerWorkFlow.NodeName = this.HisNode.Name;
            this.HisGenerWorkFlow.DeptNo = WebUser.DeptNo;
            this.HisGenerWorkFlow.DeptName = WebUser.DeptName;

            //按照指定的字段计算
            if (this.HisFlow.SDTOfFlowRole == SDTOfFlowRole.BySpecDateField)
            {
                try
                {
                    this.HisGenerWorkFlow.SDTOfFlow = this.HisWork.GetValStrByKey(this.HisFlow.GetParaString("SDTOfFlowRole_DateField"));
                    this.HisGenerWorkFlow.RDTOfSetting = this.HisWork.GetValStrByKey(this.HisFlow.GetParaString("SDTOfFlowRole_StartDateField"));
                }
                catch (Exception ex)
                {
                    string err1 = BP.WF.Glo.multilingual("可能是流程设计错误,获取开始节点[" + this.HisGenerWorkFlow.Title + "]的整体流程应完成时间有错误,是否包含SysSDTOfFlow字段? 异常信息:{0}.", "WorkNode", "wf_eng_error_5", ex.Message);
                    BP.DA.Log.DebugWriteError(err1);
                    /*获取开始节点的整体流程应完成时间有错误,是否包含SysSDTOfFlow字段? .*/
                    if (this.HisWork.EnMap.Attrs.Contains(WorkSysFieldAttr.SysSDTOfFlow) == false)
                    {
                        string err2 = BP.WF.Glo.multilingual("流程设计错误，您设置的流程时效属性是按开始节点表单SysSDTOfFlow字段计算,但是开始节点表单不包含字段 SysSDTOfFlow ,系统错误信息:{0}.", "WorkNode", "wf_eng_error_5", ex.Message);
                        throw new Exception(err2);
                    }
                    throw new Exception(BP.WF.Glo.multilingual("@初始化开始节点数据错误:{0}.", "WorkNode", "wf_eng_error_5", ex.Message));
                }
            }
            //按照指定的SQL计算
            if (this.HisFlow.SDTOfFlowRole == SDTOfFlowRole.BySQL)
            {
                string sql = this.HisFlow.SDTOfFlowRoleSQL;
                //配置的SQL为空
                if (DataType.IsNullOrEmpty(sql) == false)
                    throw new Exception(BP.WF.Glo.multilingual("@计算流程应完成时间错误,初始化开始节点数据错误:{0}.", "WorkNode", "wf_eng_error_5", "配置的SQL为空"));

                //替换SQL中的参数
                sql = Glo.DealExp(sql, this.HisWork);
                string sdtOfFlow = DBAccess.RunSQLReturnString(sql);
                if (DataType.IsNullOrEmpty(sdtOfFlow) == false)
                    this.HisGenerWorkFlow.SDTOfFlow = sdtOfFlow;
                else
                    throw new Exception(BP.WF.Glo.multilingual("@计算流程应完成时间错误,初始化开始节点数据错误:{0}.", "WorkNode", "wf_eng_error_5", "根据SQL配置查询的结果为空"));
            }

            //按照所有节点之和,
            if (this.HisFlow.SDTOfFlowRole == SDTOfFlowRole.ByAllNodes)
            {
                //获取流程的所有节点
                Nodes nds = new Nodes(this.HisFlow.No);
                DateTime sdtOfFlow = DateTime.Now;
                foreach (Node nd in nds)
                {
                    if (nd.ItIsStartNode == true)
                        continue;
                    if (nd.HisCHWay == CHWay.ByTime && nd.GetParaInt("CHWayOfTimeRole") == 0)
                    {//按天、小时考核
                     //增加天数. 考虑到了节假日. 
                     //判断是修改了节点期限的天数
                        int timeLimit = nd.TimeLimit;
                        sdtOfFlow = Glo.AddDayHoursSpan(sdtOfFlow, timeLimit,
                            nd.TimeLimitHH, nd.TimeLimitMM, nd.TWay);
                    }
                }
                this.HisGenerWorkFlow.SDTOfFlow = DataType.SysDateTimeFormat(sdtOfFlow);
            }
            //按照设置的天数
            if (this.HisFlow.SDTOfFlowRole == SDTOfFlowRole.ByDays)
            {
                //获取设置的天数
                int day = this.HisFlow.GetParaInt("SDTOfFlowRole_Days");
                if (day == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@初始化开始节点数据错误:{0}.", "WorkNode", "wf_eng_error_5", "设置流程完成时间不能为0天"));
                this.HisGenerWorkFlow.SDTOfFlow = DataType.SysDateTimeFormat(DateTime.Now.AddDays(day));
            }
            //加入两个参数. 2013-02-17
            if (this.HisGenerWorkFlow.PWorkID != 0)
            {
                this.rptGe.PWorkID = this.HisGenerWorkFlow.PWorkID;
                this.rptGe.PFlowNo = this.HisGenerWorkFlow.PFlowNo;
                this.rptGe.PNodeID = this.HisGenerWorkFlow.PNodeID;
                this.rptGe.PEmp = this.HisGenerWorkFlow.PEmp;
            }

            if (isStart == true)
                this.rptGe.FlowStartRDT = DataType.CurrentDateTimess;
            this.rptGe.FlowEnderRDT = DataType.CurrentDateTimess;
            //设置发起时间
            if (isStart == true)
                this.rptGe.SetValByKey("RDT", DataType.CurrentDateTimess);
        }
        /// <summary>
        /// 执行将当前工作节点的数据copy到Rpt里面去.
        /// </summary>
        public void DoCopyCurrentWorkDataToRpt()
        {
            /* 如果两个表一致就返回..*/
            // 把当前的工作人员增加里面去.
            string str = rptGe.GetValStrByKey(GERptAttr.FlowEmps);
            if (DataType.IsNullOrEmpty(str) == true)
                str = "@";

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDOnly)
            {
                if (str.Contains("@" + this.Execer + "@") == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + this.Execer + "@");
            }

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
            {
                if (str.Contains("@" + WebUser.Name + "@") == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + this.ExecerName + "@");
            }

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
            {
                if (str.Contains("@" + this.Execer + "," + this.ExecerName) == false)
                    rptGe.SetValByKey(GERptAttr.FlowEmps, str + this.Execer + "," + this.ExecerName + "@");
            }

            rptGe.FlowEnder = this.Execer;
            rptGe.FlowEnderRDT = DataType.CurrentDateTimess;

            //设置当前的流程所有的用时.
            rptGe.FlowDaySpan = DataType.GeTimeLimits(this.rptGe.GetValStringByKey(GERptAttr.FlowStartRDT), DataType.CurrentDateTime);

            if (this.HisNode.ItIsEndNode || this.IsStopFlow)
                rptGe.WFState = WFState.Complete;
            else
                rptGe.WFState = WFState.Runing;

            if (this.HisWork.EnMap.PhysicsTable.Equals(this.HisFlow.PTable) == false)
            {
                /*将当前的属性复制到rpt表里面去.*/
                DoCopyWorkToRpt(this.HisWork);
            }
            rptGe.DirectUpdate();
        }
        /// <summary>
        /// 执行数据copy.
        /// </summary>
        /// <param name="fromWK"></param>
        public void DoCopyWorkToRpt(Work fromWK)
        {
            foreach (Attr attr in fromWK.EnMap.Attrs)
            {
                switch (attr.Key)
                {
                    case BP.WF.GERptAttr.FK_NY:
                    case BP.WF.GERptAttr.FK_Dept:
                    case BP.WF.GERptAttr.FlowDaySpan:
                    case BP.WF.GERptAttr.FlowEmps:
                    case BP.WF.GERptAttr.FlowEnder:
                    case BP.WF.GERptAttr.FlowEnderRDT:
                    case BP.WF.GERptAttr.FlowEndNode:
                    case BP.WF.GERptAttr.FlowStarter:
                    case BP.WF.GERptAttr.Title:
                    case BP.WF.GERptAttr.WFSta:
                        continue;
                    default:
                        break;
                }

                object obj = fromWK.GetValByKey(attr.Key);
                if (obj == null)
                    continue;
                this.rptGe.SetValByKey(attr.Key, obj);
            }
            if (this.HisNode.ItIsStartNode)
                this.rptGe.SetValByKey("Title", fromWK.GetValByKey("Title"));
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg)
        {
            AddToTrack(at, toEmp, toEmpName, toNDid, toNDName, msg, this.HisNode);
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at"></param>
        /// <param name="gwl"></param>
        /// <param name="msg"></param>
        public void AddToTrack(ActionType at, GenerWorkerList gwl, string msg, Int64 subTreadWorkID, Node nd)
        {
            Track t = new Track();

            if (this.HisGenerWorkFlow.FID == 0)
            {
                t.WorkID = subTreadWorkID;
                t.FID = this.HisWork.OID;
            }
            else
            {
                t.WorkID = this.HisWork.OID;
                t.FID = this.HisGenerWorkFlow.FID;
            }

            t.RDT = DataType.CurrentDateTimess;
            t.HisActionType = at;

            t.NDFrom = ndFrom.NodeID;
            t.NDFromT = ndFrom.Name;

            t.EmpFrom = this.Execer;
            t.EmpFromT = this.ExecerName;
            t.FlowNo = this.HisNode.FlowNo;

            t.NDTo = gwl.NodeID;
            t.NDToT = gwl.NodeName;

            t.EmpTo = gwl.EmpNo;
            t.EmpToT = gwl.EmpName;
            t.Msg = msg;
            t.NodeData = "@DeptNo=" + WebUser.DeptNo + "@DeptName=" + WebUser.DeptName;
            //t.FrmDB = frmDBJson; //表单数据Json.

            switch (at)
            {
                case ActionType.Forward:
                case ActionType.ForwardAskfor:
                case ActionType.Start:
                case ActionType.UnSend:
                case ActionType.ForwardFL:
                case ActionType.ForwardHL:
                case ActionType.TeampUp:
                case ActionType.Order:
                case ActionType.SubThreadForward:
                case ActionType.FlowOver:
                case ActionType.DeleteFlowByFlag:
                    //判断是否有焦点字段，如果有就把它记录到日志里。
                    if (this.HisNode.FocusField.Length > 1)
                    {
                        string exp = this.HisNode.FocusField;
                        if (this.rptGe != null)
                            exp = Glo.DealExp(exp, this.rptGe);
                        else
                            exp = Glo.DealExp(exp, this.HisWork);

                        t.Msg += exp;
                        if (t.Msg.Contains("@"))
                        {
                            string[] para = new string[4];
                            para[0] = this.HisNode.NodeID.ToString();
                            para[1] = this.HisNode.Name;
                            para[2] = this.HisNode.FocusField;
                            para[3] = t.Msg;
                            BP.DA.Log.DebugWriteError(BP.WF.Glo.multilingual("@在节点({0}, {1})焦点字段被删除了,表达式为:{2}替换的结果为:{3}.", "WorkNode", "delete_focus_field", para));
                        }

                    }

                    //判断是否有审核组件，把审核信息存储在Msg中 
                    if (this.HisNode.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                    {
                        //获取审核组件信息
                        string sql = "SELECT Msg,MyPK From ND" + int.Parse(this.HisNode.FlowNo) + "Track Where WorkID=" + t.WorkID + " AND FID=" + t.FID + " AND ActionType=" + (int)ActionType.WorkCheck + " AND NDFrom=" + this.HisNode.NodeID + " AND EmpFrom='" + WebUser.No + "' ORDER BY RDT DESC";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            t.Msg += "WorkCheck@" + dt.Rows[0][0].ToString();
                            t.WriteDB = dt.Rows[0][1].ToString();
                        }
                        else
                        {
                            t.Msg += "WorkCheck@";
                        }

                        //把审核组件的立场信息保存在track表中
                        string checkTag = Dev2Interface.GetCheckTag(this.HisNode.FlowNo, this.WorkID, this.HisNode.NodeID, WebUser.No);
                        string[] strs = checkTag.Split('@');
                        foreach (string str in strs)
                        {
                            if (str.Contains("FWCView") == true)
                            {
                                t.Tag = t.Tag + "@" + str;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            try
            {
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
            }

            #region 处理数据版本.
            if (at == ActionType.SubThreadForward
                || at == ActionType.StartChildenFlow
                || at == ActionType.Start
                || at == ActionType.Forward
                || at == ActionType.SubThreadForward
                || at == ActionType.ForwardHL
                || at == ActionType.FlowOver)
            {
                if (this.HisNode.ItIsFL)
                    at = ActionType.ForwardFL;

                //写入数据轨迹.
                WorkNodePlus.AddNodeFrmTrackDB(this.HisFlow, this.HisNode, t, this.HisWork);
                //t.FrmDB11 = this.HisWork.ToJson();
            }
            #endregion 处理数据版本.

            if (at == ActionType.SubThreadForward
              || at == ActionType.StartChildenFlow
              || at == ActionType.Start
              || at == ActionType.Forward
              || at == ActionType.SubThreadForward
              || at == ActionType.ForwardHL
              || at == ActionType.FlowOver)
            {
                this.HisGenerWorkFlow.Paras_LastSendTruckID = t.MyPK;
            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg, Node ndFrom, string tag = null)
        {
            Track t = new Track();

            t.WorkID = this.HisWork.OID;
            t.FID = this.HisWork.FID;

            t.RDT = DataType.CurrentDateTimess;

            t.HisActionType = at;

            t.NDFrom = ndFrom.NodeID;
            t.NDFromT = ndFrom.Name;

            t.EmpFrom = this.Execer;
            t.EmpFromT = this.ExecerName;
            t.FlowNo = this.HisNode.FlowNo;
            t.Tag = tag + "@SendNode=" + this.HisNode.NodeID;

            if (toNDid == 0)
            {
                toNDid = this.HisNode.NodeID;
                toNDName = this.HisNode.Name;
            }

            t.NDTo = toNDid;
            t.NDToT = toNDName;

            t.EmpTo = toEmp;
            t.EmpToT = toEmpName;
            t.Msg = msg;
            t.NodeData = "@DeptNo=" + WebUser.DeptNo + "@DeptName=" + WebUser.DeptName;

            switch (at)
            {
                case ActionType.Forward:
                case ActionType.ForwardAskfor:
                case ActionType.Start:
                case ActionType.UnSend:
                case ActionType.ForwardFL:
                case ActionType.ForwardHL:
                case ActionType.TeampUp:
                case ActionType.Order:
                case ActionType.SubThreadForward:
                case ActionType.FlowOver:
                case ActionType.DeleteFlowByFlag:
                    //判断是否有焦点字段，如果有就把它记录到日志里。
                    if (this.HisNode.FocusField.Length > 1)
                    {
                        string exp = this.HisNode.FocusField;
                        if (this.rptGe != null)
                            exp = Glo.DealExp(exp, this.rptGe);
                        else
                            exp = Glo.DealExp(exp, this.HisWork);

                        t.Msg += exp;
                        if (t.Msg.Contains("@"))
                        {
                            string[] para = new string[4];
                            para[0] = this.HisNode.NodeID.ToString();
                            para[1] = this.HisNode.Name;
                            para[2] = this.HisNode.FocusField;
                            para[3] = t.Msg;
                            //BP.DA.Log.DebugWriteError(BP.WF.Glo.multilingual("@在节点({0}, {1})焦点字段被删除了,表达式为:{2}替换的结果为:{3}.", "WorkNode", "delete_focus_field", para));
                        }
                    }
                    //判断是否有审核组件，把审核信息存储在Msg中 
                    if (this.HisNode.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                    {
                        //获取审核组件信息 
                        string sql = "SELECT Msg,MyPK From ND" + int.Parse(this.HisNode.FlowNo) + "Track Where WorkID=" + t.WorkID + " AND FID=" + t.FID + " AND ActionType=" + (int)ActionType.WorkCheck + " AND NDFrom=" + this.HisNode.NodeID + " AND EmpFrom='" + WebUser.No + "' ORDER BY RDT DESC";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            t.Msg += "WorkCheck@" + dt.Rows[0][0].ToString();
                            t.WriteDB = dt.Rows[0][1].ToString();
                        }
                        else
                        {
                            t.Msg += "WorkCheck@";
                        }

                        //string sql = "SELECT Msg From ND" + int.Parse(this.HisNode.FlowNo) + "Track Where WorkID=" + t.WorkID + " AND FID=" + t.FID + " AND ActionType=" + (int)ActionType.WorkCheck + " AND NDFrom=" + this.HisNode.NodeID + " AND EmpFrom='" + WebUser.No + "'";
                        //t.Msg += "WorkCheck@" + DBAccess.RunSQLReturnStringIsNull(sql, "");
                        //把审核组件的立场信息保存在track表中
                        string checkTag = Dev2Interface.GetCheckTag(this.HisNode.FlowNo, this.WorkID, this.HisNode.NodeID, WebUser.No);
                        string[] strs = checkTag.Split('@');
                        foreach (string str in strs)
                        {
                            if (str.Contains("FWCView") == true)
                            {
                                t.Tag = t.Tag + "@" + str;
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            try
            {
                // t.setMyPK(t.WorkID + "_" + t.FID + "_"  + t.NDFrom + "_" + t.NDTo +"_"+t.EmpFrom+"_"+t.EmpTo+"_"+ DateTime.Now.ToString("yyMMddHHmmss");
                t.Insert();
            }
            catch
            {
                Track.CreateOrRepairTrackTable(t.FlowNo);
                t.Insert();
            }

            #region 增加,日志.
            if (at == ActionType.SubThreadForward
              || at == ActionType.StartChildenFlow
              || at == ActionType.Start
              || at == ActionType.Forward
              || at == ActionType.SubThreadForward
              || at == ActionType.ForwardHL
              || at == ActionType.FlowOver)
            {
                if (this.HisNode.ItIsFL)
                    at = ActionType.ForwardFL;

                WorkNodePlus.AddNodeFrmTrackDB(this.HisFlow, this.HisNode, t, this.HisWork);
                //t.FrmDB = this.HisWork.ToJson();
            }
            #endregion 增加.


            if (at == ActionType.SubThreadForward
              || at == ActionType.StartChildenFlow
              || at == ActionType.Start
              || at == ActionType.Forward
              || at == ActionType.SubThreadForward
              || at == ActionType.ForwardHL
              || at == ActionType.FlowOver)
            {
                this.HisGenerWorkFlow.Paras_LastSendTruckID = t.MyPK;
            }
            this.HisGenerWorkFlow.SendDT = DataType.CurrentDateTime;
            this.HisGenerWorkFlow.Update();
            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET CDT='" + DataType.CurrentDateTimess + "' WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.HisNode.NodeID + " AND FK_Emp='" + BP.Web.WebUser.No + "'");
        }
        /// <summary>
        /// 向他们发送消息
        /// </summary>
        /// <param name="gwls">接收人</param>
        public void SendMsgToThem(GenerWorkerLists gwls)
        {
            //if (BP.WF.Glo.IsEnableSysMessage == false)
            //    return;
            //求到达人员的IDs
            string toEmps = "";
            foreach (GenerWorkerList gwl in gwls)
            {
                toEmps += gwl.EmpNo + ",";
            }

            //处理工作到达事件.
            PushMsgs pms = this.town.HisNode.HisPushMsgs;
            foreach (PushMsg pm in pms)
            {
                if (pm.FK_Event != EventListNode.WorkArrive)
                    continue;

                string msg = pm.DoSendMessage(this.town.HisNode, this.town.HisWork, null, null, null, toEmps);

                this.addMsg("alert" + pm.MyPK, msg, msg, SendReturnMsgType.Info);
                // this.addMsg(SendReturnMsgFlag.SendSuccessMsg, "已经转给，加签的发起人(" + item.EmpNo + "," + item.EmpName + ")", SendReturnMsgType.Info);
            }
            return;
        }
        /// <summary>
        /// 发送前的流程状态。
        /// </summary>
        private WFState SendNodeWFState = WFState.Blank;
        /// <summary>
        /// 合流节点是否全部完成？
        /// </summary>
        private bool IsOverMGECheckStand = false;
        private bool _IsStopFlow = false;
        private bool IsStopFlow
        {
            get
            {
                return _IsStopFlow;
            }
            set
            {
                _IsStopFlow = value;
                if (_IsStopFlow == true)
                {
                    if (this.rptGe != null)
                    {
                        this.rptGe.WFState = WFState.Complete;
                        this.rptGe.Update("WFState", (int)WFState.Complete);
                    }
                }
            }
        }
        /// <summary>
        /// 检查
        /// </summary>
        private void CheckCompleteCondition_IntCompleteEmps()
        {
            string sql = "SELECT FK_Emp,EmpName FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND IsPass=1";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string emps = "@";
            string flowEmps = "@";
            foreach (DataRow dr in dt.Rows)
            {
                if (emps.Contains("@" + dr[0].ToString() + "@") || emps.Contains("@" + dr[0].ToString() + "," + dr[1].ToString() + "@"))
                    continue;

                emps = emps + dr[0].ToString() + "," + dr[1].ToString() + "@";
                flowEmps = flowEmps + dr[0].ToString() + "," + dr[1].ToString() + "@";
            }
            //追加当前操作人
            if (emps.Contains("@" + WebUser.No + ",") == false)
            {
                emps = emps + WebUser.No + "," + WebUser.Name + "@";
                flowEmps = flowEmps + WebUser.No + "," + WebUser.Name + "@";
            }
            // 给他们赋值.
            this.rptGe.FlowEmps = flowEmps;
            this.HisGenerWorkFlow.Emps = emps;
        }
        /// <summary>
        /// 检查流程、节点的完成条件
        /// </summary>
        /// <returns></returns>
        private void CheckCompleteCondition()
        {
            // 执行初始化人员.
            this.CheckCompleteCondition_IntCompleteEmps();

            // 如果结束流程，就增加如下信息 翻译.
            this.HisGenerWorkFlow.Sender = WebUser.No + "," + WebUser.Name + ";";
            this.HisGenerWorkFlow.SendDT = DataType.CurrentDateTime;

            this.rptGe.FlowEnder = BP.Web.WebUser.No;
            this.rptGe.FlowEnderRDT = DataType.CurrentDateTime;

            this.IsStopFlow = false;
            if (this.HisNode.ItIsEndNode)
            {
                /* 如果流程完成 */
                //   CCWork cc = new CCWork(this);
                // 在流程完成锁前处理消息收听，否则WF_GenerWorkerlist就删除了。

                if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                {
                    this.IsStopFlow = true;
                    this.HisGenerWorkFlow.WFState = WFState.Complete;
                    this.rptGe.WFState = WFState.Complete;

                    string msg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, "流程已经走到最后一个节点，流程成功结束。", this.HisNode, this.rptGe);
                    this.addMsg(SendReturnMsgFlag.End, msg);
                }
                return;
            }

            this.addMsg(SendReturnMsgFlag.OverCurr, BP.WF.Glo.multilingual("当前工作[{0}]已经完成", "WorkNode", "current_work_completed_para", this.HisNode.Name));

            #region 判断流程条件.
            try
            {
                string str = BP.WF.Glo.multilingual("符合流程完成条件", "WorkNode", "match_workflow_completed");
                if (this.HisNode.HisToNodes.Count == 0 && this.HisNode.ItIsStartNode)
                {
                    // 在流程完成锁前处理消息收听，否则WF_GenerWorkerlist就删除了。

                    /* 如果流程完成 */

                    this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, str, this.HisNode, this.rptGe);
                    this.IsStopFlow = true;
                    string str1 = BP.WF.Glo.multilingual("工作已经成功处理(一个节点的流程)。", "WorkNode", "match_workflow_completed");
                    string str2 = BP.WF.Glo.multilingual("工作已经成功处理(一个节点的流程)。 @查看<img src='./Img/Btn/PrintWorkRpt.gif' >", "WorkNode", "match_wf_completed_condition");
                    this.addMsg(SendReturnMsgFlag.OneNodeSheetver, str1, str2, SendReturnMsgType.Info);
                    return;
                }

                if (this.HisNode.CondsOfFlowComplete.Count >= 1
                    && this.HisNode.CondsOfFlowComplete.GenerResult(this.rptGe, this.WebUser))
                {
                    string stopMsg = this.HisFlow.CondsOfFlowComplete.ConditionDesc;
                    /* 如果流程完成 */
                    string overMsg = this.HisWorkFlow.DoFlowOver(ActionType.FlowOver, str + ": " + stopMsg, this.HisNode, this.rptGe);
                    this.IsStopFlow = true;

                    // string path = BP.Sys.Base.Glo.Request.ApplicationPath;
                    this.addMsg(SendReturnMsgFlag.MacthFlowOver, "@" + str + stopMsg + "" + overMsg,
                       "@" + str + stopMsg + "" + overMsg, SendReturnMsgType.Info);
                    return;
                }
            }
            catch (Exception ex)
            {
                string str = BP.WF.Glo.multilingual("@判断流程({0})完成条件出现错误:{1}.",
                    "WorkNode",
                    "error_workflow_complete_condition", ex.StackTrace, this.HisNode.Name);

                throw new Exception(str);
            }
            #endregion

        }

        #region 启动多个节点
        /// <summary>
        /// 生成为什么发送给他们
        /// </summary>
        /// <param name="fNodeID"></param>
        /// <param name="toNodeID"></param>
        /// <returns></returns>
        public string GenerWhySendToThem(int fNodeID, int toNodeID)
        {
            return "";
            //return "@<a href='WhySendToThem.aspx?NodeID=" + fNodeID + "&ToNodeID=" + toNodeID + "&WorkID=" + this.WorkID + "' target=_blank >" + this.ToE("WN20", "为什么要发送给他们？") + "</a>";
        }
        /// <summary>
        /// 工作流程ID
        /// </summary>
        public static Int64 FID = 0;
        /// <summary>
        /// 没有FID
        /// </summary>
        /// <param name="nd"></param>
        /// <returns></returns>
        private string StartNextWorkNodeHeLiu_WithOutFID(Node nd)
        {
            throw new Exception("未完成:StartNextWorkNodeHeLiu_WithOutFID");
        }
        /// <summary>
        /// 异表单子线程向合流点运动
        /// </summary>
        /// <param name="nd"></param>
        private void NodeSend_53_UnSameSheet_To_HeLiu(Node nd)
        {

            Work heLiuWK = nd.HisWork;

            #region 处理FID.
            Int64 fid = this.HisWork.FID;
            if (fid == 0)
            {
                if (this.HisNode.ItIsSubThread == false)
                    throw new Exception(BP.WF.Glo.multilingual("@当前节点非子线程节点.", "WorkNode", "not_sub_thread"));
                fid = this.HisGenerWorkFlow.FID;
                if (fid == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@丢失FID信息.", "WorkNode", "missing_FID"));

                this.HisWork.FID = fid;
                this.HisWork.Update();
            }
            #endregion FID

            heLiuWK.OID = this.HisWork.FID;
            if (heLiuWK.RetrieveFromDBSources() == 0) //查询出来数据.
                heLiuWK.DirectInsert();

            //根据Node判断该节点是否绑定表单库的表单
            bool isCopyData = true;
            //分流节点和子线程的节点绑定的表单相同
            if (nd.HisFormType == NodeFormType.RefOneFrmTree && nd.NodeFrmID.Equals(this.HisNode.NodeFrmID) == true)
                isCopyData = false;

            if (isCopyData == true)
                heLiuWK.Copy(this.HisWork); // 执行copy.

            heLiuWK.OID = this.HisWork.FID;
            heLiuWK.FID = 0;

            this.town = new WorkNode(heLiuWK, nd);

            //合流节点上的工作处理者。
            GenerWorkerLists gwls = new GenerWorkerLists(this.HisWork.FID, nd.NodeID);
            current_gwls = gwls;

            GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.FID);
            if (gwls.Count == 0)
            {
                // 说明第一次到达河流节点。
                current_gwls = this.Func_GenerWorkerLists(this.town);
                gwls = current_gwls;

                gwf.NodeID = nd.NodeID;
                gwf.NodeName = nd.Name;
                gwf.TodoEmpsNum = gwls.Count;

                string todoEmps = "";
                foreach (GenerWorkerList item in gwls)
                    todoEmps += item.EmpNo + "," + item.EmpName + ";";

                gwf.TodoEmps = todoEmps;
                gwf.WFState = WFState.Runing;
                //第一次到达设计Gen
                gwf.Update();
            }

            //记录子线程到达合流节点数
            int count = gwf.GetParaInt("ThreadCount");
            gwf.SetPara("ThreadCount", count + 1);
            gwf.Update();

            string FK_Emp = "";
            string toEmpsStr = "";
            string emps = "";
            string empNos = "";
            foreach (GenerWorkerList wl in gwls)
            {
                empNos += wl.EmpNo + ",";
                toEmpsStr += wl.EmpName + ",";
                if (gwls.Count == 1)
                    emps = wl.EmpNo + "," + wl.EmpName;
                else
                    emps += "@" + wl.EmpNo + "," + wl.EmpName;
            }

            ActionType at = ActionType.SubThreadForward;
            this.AddToTrack(at, empNos, toEmpsStr, nd.NodeID, nd.Name, BP.WF.Glo.multilingual("子线程向合流节点发送", "WorkNode", "sub_thread"), this.HisNode);

            /* 
            * 更新它的节点 worklist 信息, 说明当前节点已经完成了.
            * 不让当前的操作员能看到自己的工作。
            */
            #region 处理合流节点表单数据。


            #region 复制主表数据. edit 2014-11-20 向合流点汇总数据.
            //复制当前节点表单数据.
            heLiuWK.FID = 0;
            heLiuWK.Rec = FK_Emp;
            //heLiuWK.Emps = emps;
            heLiuWK.OID = this.HisWork.FID;
            heLiuWK.DirectUpdate(); //在更新一次.

            /* 把数据复制到rpt数据表里. */
            this.rptGe.OID = this.HisWork.FID;
            this.rptGe.RetrieveFromDBSources();
            this.rptGe.Copy(this.HisWork);
            this.rptGe.DirectUpdate();
            #endregion 复制主表数据.

            #endregion 处理合流节点表单数据

            //设置当前子线程已经通过.
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1  WHERE WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND IsPass=0";
            ps.Add("WorkID", this.WorkID);
            ps.Add("FID", this.HisWork.FID);
            DBAccess.RunSQL(ps);

            if (this.HisNode.TodolistModel == BP.WF.TodolistModel.QiangBan)
            {
                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND FK_Emp!=" + dbStr + "FK_Emp AND IsPass=0";
                ps.Add("WorkID", this.WorkID);
                ps.Add("FID", this.HisWork.FID);
                ps.Add("FK_Emp", WebUser.No);
                DBAccess.RunSQL(ps);
            }

            string info = "";

            /* 合流点需要等待各个分流点全部处理完后才能看到它。*/
            string sql1 = "";
#warning 对于多个分合流点可能会有问题。
            ps = new Paras();
            ps.SQL = "SELECT COUNT(distinct WorkID) AS Num FROM WF_GenerWorkerlist WHERE  FID=" + dbStr + "FID AND FK_Node IN (" + this.SpanSubTheadNodes(nd) + ")";
            ps.Add("FID", this.HisWork.FID);
            decimal numAll1 = (decimal)DBAccess.RunSQLReturnValInt(ps);
            //说明出现跳转情况，计算出合流点发送的子线程
            int hlNodeID = this.HisGenerWorkFlow.GetParaInt("FLNodeID");
            if (hlNodeID != 0)
                numAll1 = (decimal)this.HisGenerWorkFlow.GetParaInt("ThreadCount");
            decimal numPassed = gwf.GetParaInt("ThreadCount");

            decimal passRate1 = numPassed / numAll1 * 100;
            if (nd.PassRate <= passRate1)
            {
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0,FID=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                DBAccess.RunSQL(ps);

                //ps = new Paras();
                //ps.SQL = "UPDATE WF_GenerWorkFlow SET FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE WorkID=" + dbStr + "WorkID";
                //ps.Add("FK_Node", nd.NodeID);
                //ps.Add("NodeName", nd.Name);
                //ps.Add("WorkID", this.HisWork.FID);
                //DBAccess.RunSQL(ps);

                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE  FID=" + dbStr + "FID  AND IsPass=-2";
                ps.Add("FID", this.HisWork.FID);
                DBAccess.RunSQL(ps);

                gwf.NodeID = nd.NodeID;
                gwf.NodeName = nd.Name;
                gwf.Emps = gwf.Emps + "@" + this.HisGenerWorkFlow.Emps;
                //gwf.Para("ThreadCount", 0);
                gwf.Update();
                info = BP.WF.Glo.multilingual("@下一步合流节点[{0}]工作成功启动.", "WorkNode", "start_next_combined_node_work_success", nd.Name);
            }
            else
            {
#warning 为了不让其显示在途的工作需要， =3 不是正常的处理模式。
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=3,FID=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("WorkID", this.HisWork.FID);
                DBAccess.RunSQL(ps);

                gwf.Emps = gwf.Emps + "@" + this.HisGenerWorkFlow.Emps;
                gwf.Update();
            }

            this.HisGenerWorkFlow.NodeID = nd.NodeID;
            this.HisGenerWorkFlow.NodeName = nd.Name;

            // 产生合流汇总从表数据.
            this.GenerHieLiuHuiZhongDtlData_2013(nd);

            this.addMsg(SendReturnMsgFlag.VarAcceptersID, emps, SendReturnMsgType.SystemMsg);

            this.addMsg("HeLiuInfo", BP.WF.Glo.multilingual("@下一步的工作处理人[{0}]", "WorkNode", "next_node_operator", emps) + info, SendReturnMsgType.Info);
        }
        /// <summary>
        /// 产生合流汇总数据
        /// 把子线程的子表主表数据放到合流点的从表上去
        /// </summary>
        /// <param name="nd"></param>
        private void GenerHieLiuHuiZhongDtlData_2013(Node ndOfHeLiu)
        {
            #region 汇总明细表.
            MapDtls mydtls = ndOfHeLiu.HisWork.HisMapDtls;
            foreach (MapDtl dtl in mydtls)
            {
                if (dtl.ItIsHLDtl == false)
                    continue;

                GEDtl geDtl = dtl.HisGEDtl;
                geDtl.Copy(this.HisWork);
                geDtl.RefPK = this.HisWork.FID.ToString(); // RefPK 就是当前子线程的FID.
                geDtl.Rec = this.Execer;
                geDtl.RDT = DataType.CurrentDateTime;

                #region 判断是否是质量评价
                if (ndOfHeLiu.ItIsEval)
                {
                    /*如果是质量评价流程*/
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalEmpNo, this.Execer);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalEmpName, this.ExecerName);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalCent, 0);
                    geDtl.SetValByKey(WorkSysFieldAttr.EvalNote, "");
                }
                #endregion

                #region 执行插入数据.
                try
                {
                    geDtl.InsertAsOID(this.HisWork.OID);
                }
                catch
                {
                    geDtl.Update();
                }
                #endregion 执行插入数据.


                #region 还要处理附件的 copy 汇总. 如果子线程上有附件组件.
                if (dtl.ItIsEnableAthM == true)
                {
                    /*如果启用了多附件。*/
                    //取出来所有的上个节点的数据集合.
                    FrmAttachments athSLs = this.HisWork.HisFrmAttachments;
                    if (athSLs.Count == 0)
                        break; /*子线程上没有附件组件.*/

                    //求子线程的汇总附件集合 (处理如果子线程上有多个附件，其中一部分附件需要汇总另外一部分不需要汇总的模式)
                    string strs = "";
                    foreach (FrmAttachment item in athSLs)
                    {
                        if (item.ItIsToHeLiuHZ == true)
                            strs += "," + item.MyPK + ",";
                    }

                    //如果没有找到，并且附件集合只有1个，就设置他为子线程的汇总附件，可能是设计人员忘记了设计.
                    if (strs == "" && athSLs.Count == 1)
                    {
                        FrmAttachment athT = athSLs[0] as FrmAttachment;
                        athT.ItIsToHeLiuHZ = true;
                        athT.Update();
                        strs = "," + athT.MyPK + ",";
                    }

                    // 没有找到要执行的附件.
                    if (strs == "")
                        break;

                    //取出来所有的上个节点的数据集合.
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs();
                    athDBs.Retrieve(FrmAttachmentDBAttr.FK_MapData, this.HisWork.NodeFrmID,
                        FrmAttachmentDBAttr.RefPKVal, this.HisWork.OID);

                    if (athDBs.Count == 0)
                        break; /*子线程没有上传附件.*/


                    /*说明当前节点有附件数据*/
                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        if (strs.Contains("," + athDB.FK_FrmAttachment + ",") == false)
                            continue;

                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.FrmID = dtl.No;
                        athDB_N.RefPKVal = geDtl.OID.ToString();
                        athDB_N.FK_FrmAttachment = dtl.No + "_AthMDtl";
                        athDB_N.UploadGUID = "";
                        athDB_N.FID = this.HisWork.FID;

                        //生成新的GUID.
                        athDB_N.setMyPK(DBAccess.GenerGUID());
                        athDB_N.Insert();
                    }

                }
                #endregion 还要处理附件的copy 汇总.
                break;
            }
            #endregion 汇总明细表.

            #region 复制附件。
            //合流点附件的集合
            FrmAttachments aths = ndOfHeLiu.HisWork.HisFrmAttachments;  // new FrmAttachments("ND" + this.HisNode.NodeID);
            if (aths.Count == 0)
                return;
            foreach (FrmAttachment ath in aths)
            {
                //合流的汇总的多附件数据。
                if (ath.ItIsHeLiuHuiZong == false)
                    continue;
                //附件标识
                string noOfObj = ath.NoOfObj;

                //如果附件标识相同的附件数据汇总
                FrmAttachments athSLs = this.HisWork.HisFrmAttachments;
                if (athSLs.Count == 0)
                    break; /*子线程上没有附件组件.*/

                //求子线程的汇总附件集合NoOfObj相同才可以汇总一起
                string strs = "";
                foreach (FrmAttachment item in athSLs)
                {
                    if (item.ItIsToHeLiuHZ == true && item.NoOfObj.Equals(noOfObj) == true)
                        strs += "," + item.NoOfObj + ",";
                }

                //如果没有找到，并且附件集合只有1个，就设置他为子线程的汇总附件，可能是设计人员忘记了设计.
                if (strs == "" && athSLs.Count == 1)
                {
                    FrmAttachment athT = athSLs[0] as FrmAttachment;
                    athT.ItIsToHeLiuHZ = true;
                    athT.Update();
                    strs = "," + athT.MyPK + ",";
                    noOfObj = athT.NoOfObj;
                }

                // 没有找到要执行的附件.
                if (strs == "")
                    break;

                //取出来所有的上个节点的数据集合.
                FrmAttachmentDBs athDBs = new FrmAttachmentDBs();

                athDBs.Retrieve(FrmAttachmentDBAttr.NoOfObj, noOfObj, FrmAttachmentDBAttr.RefPKVal, this.HisWork.OID);

                if (athDBs.Count == 0)
                    break; /*子线程没有上传附件.*/

                /*说明当前节点有附件数据*/
                foreach (FrmAttachmentDB athDB in athDBs)
                {
                    //判断是否已经存在附件，避免重复上传
                    FrmAttachmentDB athNDB = new FrmAttachmentDB();
                    int num = athNDB.Retrieve(FrmAttachmentDBAttr.FK_MapData, "ND" + ndOfHeLiu.NodeID, FrmAttachmentDBAttr.RefPKVal, this.HisWork.FID.ToString(), FrmAttachmentDBAttr.UploadGUID, athDB.UploadGUID);
                    if (num > 0)
                        continue;

                    FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                    athDB_N.Copy(athDB);
                    athDB_N.FrmID = "ND" + ndOfHeLiu.NodeID;
                    athDB_N.RefPKVal = this.HisWork.FID.ToString();
                    athDB_N.FK_FrmAttachment = ath.MyPK;

                    //生成新的GUID.
                    athDB_N.setMyPK(DBAccess.GenerGUID());
                    athDB_N.Insert();
                }
                break;
            }
            #endregion 复制附件。

            #region 复制Ele。
            FrmEleDBs eleDBs = new FrmEleDBs("ND" + this.HisNode.NodeID,
                  this.WorkID.ToString());
            if (eleDBs.Count > 0)
            {
                /*说明当前节点有附件数据*/
                int idx = 0;
                foreach (FrmEleDB eleDB in eleDBs)
                {
                    idx++;
                    FrmEleDB eleDB_N = new FrmEleDB();
                    eleDB_N.Copy(eleDB);
                    eleDB_N.FrmID = "ND" + ndOfHeLiu.NodeID;
                    eleDB_N.setMyPK(eleDB_N.MyPK.Replace("ND" + this.HisNode.NodeID, "ND" + ndOfHeLiu.NodeID));
                    eleDB_N.RefPKVal = this.HisWork.FID.ToString();
                    eleDB_N.Save();
                }
            }
            #endregion 复制Ele。


        }
        /// <summary>
        /// 子线程节点
        /// </summary>
        private string _SpanSubTheadNodes = null;
        /// <summary>
        /// 获取分流与合流之间的子线程节点集合.
        /// </summary>
        /// <param name="toNode"></param>
        /// <returns></returns>
        private string SpanSubTheadNodes(Node toHLNode)
        {
            _SpanSubTheadNodes = "";
            SpanSubTheadNodes_DiGui(toHLNode.FromNodes);
            if (_SpanSubTheadNodes == "")
                throw new Exception(BP.WF.Glo.multilingual("获取分合流之间的子线程节点集合为空，请检查流程设计，在分合流之间的节点必须设置为子线程节点。", "WorkNode", "wf_eng_error_6"));

            _SpanSubTheadNodes = _SpanSubTheadNodes.Substring(1);
            return _SpanSubTheadNodes;

        }
        private void SpanSubTheadNodes_DiGui(Nodes subNDs)
        {
            foreach (Node nd in subNDs)
            {
                if (nd.HisNodeWorkType == NodeWorkType.SubThreadWork)
                {
                    //判断是否已经包含，不然可能死循环
                    if (_SpanSubTheadNodes.Contains("," + nd.NodeID))
                        continue;

                    _SpanSubTheadNodes += "," + nd.NodeID;
                    SpanSubTheadNodes_DiGui(nd.FromNodes);
                }
            }
        }


        #endregion

        #region 基本属性
        /// <summary>
        /// 工作
        /// </summary>
        private Work _HisWork = null;
        /// <summary>
        /// 工作
        /// </summary>
        public Work HisWork
        {
            get
            {
                return this._HisWork;
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        private Node _HisNode = null;
        /// <summary>
        /// 节点
        /// </summary>
        public Node HisNode
        {
            get
            {
                return this._HisNode;
            }
        }
        public RememberMe HisRememberMe = null;
        public RememberMe GetHisRememberMe(Node nd)
        {
            if (HisRememberMe == null || HisRememberMe.NodeID != nd.NodeID)
            {
                HisRememberMe = new RememberMe();
                HisRememberMe.EmpNo = this.Execer;
                HisRememberMe.NodeID = nd.NodeID;
                HisRememberMe.RetrieveFromDBSources();
            }
            return this.HisRememberMe;
        }
        private WorkFlow _HisWorkFlow = null;
        /// <summary>
        /// 工作流程
        /// </summary>
        public WorkFlow HisWorkFlow
        {
            get
            {
                if (_HisWorkFlow == null)
                    _HisWorkFlow = new WorkFlow(this.HisNode.HisFlow, this.HisWork.OID, this.HisWork.FID);
                return _HisWorkFlow;
            }
        }
        /// <summary>
        /// 当前节点的工作是不是完成。
        /// </summary>
        public bool ItIsComplete
        {
            get
            {
                if (this.HisGenerWorkFlow.WFState == WFState.Complete)
                    return true;
                else
                    return false;
            }
        }
        public TransferCustom _transferCustom = null;
        public TodolistModel TodolistModel
        {
            get
            {
                //如果当前的节点是按照ccbpm定义的方式运行的，就返回当前节点的多人待办模式，否则就返回自定义的模式。
                ///if (this.HisGenerWorkFlow.TransferCustomType == TransferCustomType.ByCCBPMDefine)
                return this.HisNode.TodolistModel;
                //return this.HisGenerWorkFlow.TodolistModel;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 建立一个工作节点事例.
        /// </summary>
        /// <param name="workId">工作ID</param>
        /// <param name="nodeId">节点ID</param>
        public WorkNode(Int64 workId, int nodeId)
        {

            this.WorkID = workId;
            Node nd = new Node(nodeId);
            Work wk = nd.HisWork;
            wk.OID = workId;
            int i = wk.RetrieveFromDBSources();
            if (i == 0)
            {
                this.rptGe = nd.HisFlow.HisGERpt;
                if (wk.FID != 0)
                    this.rptGe.OID = wk.FID;
                else
                    this.rptGe.OID = this.WorkID;

                this.rptGe.RetrieveFromDBSources();
                wk.Row = rptGe.Row;
            }
            this._HisWork = wk;
            this._HisNode = nd;
        }
        public Hashtable SendHTOfTemp = null;
        public string title = null;
        /// <summary>
        /// 建立一个工作节点事例
        /// </summary>
        /// <param name="wk">工作</param>
        /// <param name="nd">节点</param>
        public WorkNode(Work wk, Node nd)
        {
            this.WorkID = wk.OID;
            this._HisWork = wk;
            this._HisNode = nd;
        }
        #endregion

        #region 运算属性
        private void Repair()
        {
        }
        public WorkNode GetPreviousWorkNode_FHL(Int64 workid)
        {
            Nodes nds = this.HisNode.FromNodes;
            foreach (Node nd in nds)
            {
                if (nd.ItIsSubThread == true)
                {
                    Work wk = nd.HisWork;
                    wk.OID = workid;
                    if (wk.RetrieveFromDBSources() != 0)
                    {
                        WorkNode wn = new WorkNode(wk, nd);
                        return wn;
                    }
                }
            }
            return null;
        }
        public WorkNodes GetPreviousWorkNodes_FHL()
        {
            // 如果没有找到转向他的节点,就返回,当前的工作.
            if (this.HisNode.ItIsStartNode)
                throw new Exception(BP.WF.Glo.multilingual("@此节点是开始节点,没有上一步工作.", "WorkNode", "not_found_pre_node_1"));
            //此节点是开始节点,没有上一步工作.

            if (this.HisNode.HisNodeWorkType == NodeWorkType.WorkHL
               || this.HisNode.HisNodeWorkType == NodeWorkType.WorkFHL)
            {
            }
            else
            {
                throw new Exception(BP.WF.Glo.multilingual("@当前工作节点不是分合流节点。", "WorkNode", "current_node_not_separate"));
            }

            WorkNodes wns = new WorkNodes();
            Nodes nds = this.HisNode.FromNodes;
            foreach (Node nd in nds)
            {
                Works wks = (Works)nd.HisWorks;
                wks.Retrieve(WorkAttr.FID, this.HisWork.OID);

                if (wks.Count == 0)
                    continue;

                foreach (Work wk in wks)
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    wns.Add(wn);
                }
            }
            return wns;
        }
        /// <summary>
        /// 得当他的上一步工作
        /// 1, 从当前的找到他的上一步工作的节点集合.		 
        /// 如果没有找到转向他的节点,就返回,当前的工作.
        /// </summary>
        /// <returns>得当他的上一步工作</returns>
        public WorkNode GetPreviousWorkNode()
        {
            // 如果没有找到转向他的节点,就返回,当前的工作.
            if (this.HisNode.ItIsStartNode)
                throw new Exception(BP.WF.Glo.multilingual("@此节点是开始节点,没有上一步工作.", "WorkNode", "not_found_pre_node_1")); //此节点是开始节点,没有上一步工作.

            string sql = "";
            int nodeid = 0;
            string truckTable = "ND" + int.Parse(this.HisNode.FlowNo) + "Track";
            sql = "SELECT NDFrom,Tag FROM " + truckTable + " WHERE WorkID=" + this.WorkID + " AND NDTo='" + this.HisNode.NodeID + "' AND ";
            sql += " (ActionType=1 OR ActionType=" + (int)ActionType.Skip + "  OR ActionType=" + (int)ActionType.ForwardFL + " ";
            sql += "  OR  ActionType=" + (int)ActionType.ForwardHL + " "; //合流.
            sql += "  OR  ActionType=" + (int)ActionType.ForwardAskfor + " "; //会签.
            sql += "   )";
            sql += " ORDER BY RDT DESC";

            //首先获取实际发送节点，不存在时再使用from节点.
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                nodeid = int.Parse(dt.Rows[0]["NDFrom"].ToString());
                if (dt.Rows[0]["Tag"] != null && dt.Rows[0]["Tag"].ToString().Contains("SendNode=") == true)
                {
                    string tag = dt.Rows[0]["Tag"].ToString();
                    string[] strs = tag.Split('@');
                    foreach (string str in strs)
                    {
                        if (str == null || str == "" || str.Contains("SendNode=") == false)
                            continue;
                        string[] mystr = str.Split('=');
                        if (mystr.Length == 2)
                        {
                            string sendNode = mystr[1];
                            if (string.IsNullOrEmpty(sendNode) == false && sendNode.Equals("0") == false)
                            {
                                nodeid = int.Parse(sendNode);
                            }
                        }
                    }
                }
            }

            if (nodeid == 0)
            {
                switch (this.HisNode.HisRunModel)
                {
                    case RunModel.HL:
                    case RunModel.FHL:
                        sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.WorkID
                                                                                       + " ORDER BY RDT DESC";
                        break;
                    case RunModel.SubThreadSameWorkID:
                    case RunModel.SubThreadUnSameWorkID:
                        sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.WorkID
                                                                                       + " AND NDTo=" + this.HisNode.NodeID + " "
                                                                                       + " AND ( ActionType=" + (int)ActionType.SubThreadForward + " OR  ActionType=" + (int)ActionType.ForwardFL + ")  ORDER BY RDT DESC";
                        if (DBAccess.RunSQLReturnCOUNT(sql) == 0)
                            sql = "SELECT NDFrom FROM " + truckTable + " WHERE WorkID=" + this.HisWork.FID
                                                                                      + " AND NDTo=" + this.HisNode.NodeID + " "
                                                                                      + " AND (ActionType=" + (int)ActionType.SubThreadForward + " OR  ActionType=" + (int)ActionType.ForwardFL + ") ORDER BY RDT DESC";

                        break;
                    default:
                        sql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node!='" + this.HisNode.NodeID + "' ORDER BY RDT,FK_Node ";
                        //throw new Exception("err@没有判断的类型:"+this.HisNode.HisRunModel);
                        //根据当前节点获取上一个节点，不用管那个人发送的
                        break;
                }
                nodeid = DBAccess.RunSQLReturnValInt(sql, 0);
            }
            if (nodeid == 0)
                throw new Exception(BP.WF.Glo.multilingual("@没有找到上一步节点", "WorkNode", "not_found_pre_node_2") + ":" + sql);

            Node nd = new Node(nodeid);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();

            WorkNode wn = new WorkNode(wk, nd);
            return wn;
        }
        #endregion
    }
    /// <summary>
    /// 工作节点集合.
    /// </summary>
    public class WorkNodes : CollectionBase
    {
        #region 构造
        /// <summary>
        /// 他的工作s
        /// </summary> 
        public Works GetWorks
        {
            get
            {
                if (this.Count == 0)
                    throw new Exception(BP.WF.Glo.multilingual("@初始化失败，没有找到任何节点。", "WorkNode", "not_found_pre_node_3"));

                Works ens = this[0].HisNode.HisWorks;
                ens.Clear();

                foreach (WorkNode wn in this)
                {
                    ens.AddEntity(wn.HisWork);
                }
                return ens;
            }
        }
        /// <summary>
        /// 工作节点集合
        /// </summary>
        public WorkNodes()
        {
        }

        public int GenerByFID(Flow flow, Int64 fid)
        {
            this.Clear();

            Nodes nds = flow.HisNodes;
            foreach (Node nd in nds)
            {
                if (nd.ItIsSubThread == true)
                    continue;

                Work wk = nd.GetWork(fid);
                if (wk == null)
                    continue;


                this.Add(new WorkNode(wk, nd));
            }
            return this.Count;
        }

        public int GenerByWorkID(Flow flow, Int64 oid)
        {
            /*退回 ,需要判断跳转的情况，如果是跳转的需要退回到他开始执行的节点
		    * 跳转的节点在WF_GenerWorkerlist中不存在该信息
		    */
            string table = "ND" + int.Parse(flow.No) + "Track";

            string actionSQL = "SELECT EmpFrom,EmpFromT,RDT,NDFrom FROM " + table + " WHERE WorkID=" + oid
                          + " AND (ActionType=" + (int)ActionType.Start
                          + " OR ActionType=" + (int)ActionType.Forward
                          + " OR ActionType=" + (int)ActionType.ForwardFL
                          + " OR ActionType=" + (int)ActionType.ForwardHL
                          + " OR ActionType=" + (int)ActionType.SubThreadForward
                          + " OR ActionType=" + (int)ActionType.Skip
                          + " )"
                          + " AND NDFrom IN(SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + oid + ")"
                          + " ORDER BY RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(actionSQL);

            string nds = "";
            foreach (DataRow dr in dt.Rows)
            {
                Node nd = new Node(int.Parse(dr["NDFrom"].ToString()));
                Work wk = nd.GetWork(oid);
                if (wk == null)
                    wk = nd.HisWork;

                // 处理重复的问题.
                if (nds.Contains(nd.NodeID.ToString() + ",") == true)
                    continue;
                nds += nd.NodeID.ToString() + ",";


                wk.Rec = dr["EmpFrom"].ToString();
                //   wk.RecText = dr["EmpFromT"].ToString();
                wk.SetValByKey("RDT", dr["RDT"].ToString());
                this.Add(new WorkNode(wk, nd));
            }
            return this.Count;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 增加一个WorkNode
        /// </summary>
        /// <param name="wn">工作 节点</param>
        public void Add(WorkNode wn)
        {
            this.InnerList.Add(wn);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public WorkNode this[int index]
        {
            get
            {
                return (WorkNode)this.InnerList[index];
            }
        }
        #endregion
    }
}
