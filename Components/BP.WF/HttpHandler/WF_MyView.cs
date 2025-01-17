﻿using System;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Template;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 抄送处理类
    /// </summary>
    public class WF_MyView : DirectoryPageBase
    {
       
        /// <summary>
        /// 抄送处理类
        /// </summary>
        public WF_MyView()
        {

        }

        #region 表单查看.
        /// <summary>
        /// 表单数据
        /// </summary>
        /// <returns></returns>
        public string MyFrm_Init_Data()
        {
            string trackID = this.GetRequestVal("TrackID");
            if (DataType.IsNullOrEmpty(trackID) == true)
                return "";
            //根据TrackID从Track表中获取历史数据
            return DBAccess.GetBigTextFromDB("ND" + int.Parse(this.FlowNo) + "Track", "MyPK", trackID, "FrmDB");
        }

        #endregion 表单查看.

        #region  运行变量
        /// <summary>
        /// 从节点.
        /// </summary>
        public string FromNode
        {
            get
            {
                return this.GetRequestVal("FromNode");
            }
        }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool ItIsCC
        {
            get
            {
                string str = this.GetRequestVal("Paras");

                if (DataType.IsNullOrEmpty(str) == false)
                {
                    string myps = str;

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }

                str = this.GetRequestVal("AtPara");
                if (DataType.IsNullOrEmpty(str) == false)
                {
                    if (str.Contains("IsCC=1") == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 轨迹ID
        /// </summary>
        public string TrackID
        {
            get
            {
                return this.GetRequestVal("TrackeID");
            }
        }
        /// <summary>
        /// 到达的节点ID
        /// </summary>
        public int ToNode
        {
            get
            {
                return this.GetRequestValInt("ToNode");
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public new int FK_Node
        {
            get
            {
                string fk_nodeReq = this.GetRequestVal("FK_Node");  //this.Request.Form["FK_Node"];
                if (DataType.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.GetRequestVal("NodeID");// this.Request.Form["NodeID"];

                if (DataType.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.WorkID != 0)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add("WorkID", this.WorkID);
                        _FK_Node = DBAccess.RunSQLReturnValInt(ps, 0);
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FlowNo + "01");
                    }
                }
                return _FK_Node;
            }
        }

        private string _width = "";
        /// <summary>
        /// 表单宽度
        /// </summary>
        public string Width
        {
            get
            {
                return _width;
            }
            set { _width = value; }
        }
        private string _height = "";
        /// <summary>
        /// 表单高度
        /// </summary>
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public string _btnWord = "";
        public string BtnWord
        {
            get { return _btnWord; }
            set { _btnWord = value; }
        }
        private GenerWorkFlow _HisGenerWorkFlow = null;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow == null)
                    _HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                return _HisGenerWorkFlow;
            }
        }
        private Node _currNode = null;
        public Node currND
        {
            get
            {
                if (_currNode == null)
                {
                    if (this.WorkID == 0)
                        throw new Exception("err@WorkID参数没有传入过来.");

                    if (this.NodeID == 0)
                    {
                        _currNode = new Node(this.HisGenerWorkFlow.NodeID);
                        return _currNode;
                    }
                    _currNode = new Node(this.NodeID);
                }
                return _currNode;
            }
        }
        private Flow _currFlow = null;
        public Flow currFlow
        {
            get
            {
                if (_currFlow == null)
                    _currFlow = new Flow(this.FlowNo);
                return _currFlow;
            }
        }
        /// <summary>
        /// 定义跟路径
        /// </summary>
        public string appPath = "/";



        #endregion

        public string InitToolBar()
        {
            DataTable dt = new DataTable("ToolBar");
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Oper");

            BtnLab btnLab = new BtnLab(this.NodeID);
            string tKey = DateTime.Now.ToString("MM-dd-hh:mm:ss");
            string toolbar = "";
            try
            {

                DataRow dr = dt.NewRow();
                /*if (this.IsMobile == false)
                {
                    dr["No"] = "Close";
                    dr["Name"] = "关闭";
                    dr["Oper"] = "Close();";
                    dt.Rows.Add(dr);
                }*/


                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

                Node nd = null;

                if (gwf.WFState == WFState.Runing)
                    nd = new Node(gwf.NodeID);


                #region 根据流程权限控制规则获取可以操作的按钮功能
                string sql = "SELECT A.PowerFlag,A.EmpNo,A.EmpName FROM WF_PowerModel A WHERE PowerCtrlType =1"
                    + " UNION "
                    + "SELECT A.PowerFlag,B." + BP.Sys.Base.Glo.UserNo + ",B.Name FROM WF_PowerModel A, Port_Emp B, Port_Deptempstation C WHERE A.PowerCtrlType = 0 AND B.No=C.FK_Emp AND A.StaNo = C.FK_Station";
                sql = "SELECT PowerFlag From(" + sql + ")D WHERE  D.EmpNo='" + WebUser.No + "'";

                string powers = DBAccess.RunSQLReturnStringIsNull(sql, "");
                switch (gwf.WFState)
                {
                    case WFState.Runing: /* 运行时*/
                        /*删除流程.*/
                        if (powers.Contains("FlowDataDelete") == true || (BP.WF.Dev2Interface.Flow_IsCanDeleteFlowInstance(this.FlowNo, this.WorkID, WebUser.No) == true && btnLab.DeleteEnable != 0))
                        {
                            dr = dt.NewRow();
                            dr["No"] = "Delete";
                            dr["Name"] = btnLab.DeleteLab;
                            dr["Oper"] = "";
                            dt.Rows.Add(dr);
                        }

                        ///*取回审批*/
                        //string para = "";
                        //sql = "SELECT NodeID FROM WF_Node WHERE CheckNodes LIKE '%" + gwf.NodeID + "%'";
                        //int myNode = DBAccess.RunSQLReturnValInt(sql, 0);
                        //if (myNode != 0)
                        //{
                        //    GetTask gt = new GetTask(myNode);
                        //    if (gt.Can_I_Do_It())
                        //    {
                        //        dr = dt.NewRow();
                        //        dr["No"] = "TackBack";
                        //        dr["Name"] = "取回审批";
                        //        dr["Oper"] = "TackBack(" + gwf.NodeID + "," + myNode + ")";
                        //        dt.Rows.Add(dr);

                        //    }
                        //}


                        /*撤销发送*/
                        sql = "SELECT WorkID,FK_Emp,FK_Node,WhoExeIt From WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' AND ((IsPass=0 AND WhoExeIt=1) OR IsPass=1) AND IsEnable=1 AND WorkID=" + this.WorkID;
                        DataTable dtt = DBAccess.RunSQLReturnTable(sql);

                        if (dtt.Rows.Count > 0 || powers.Contains("FlowDataUnSend") == true)
                        {
                            dr = dt.NewRow();
                            dr["No"] = "UnSend";
                            dr["Name"] = "撤销";
                            if(dtt.Rows.Count > 0 && dtt.Rows[0][3].ToString().Equals("1"))
                                dr["Oper"] = "UnSend(1)";
                            else
                                dr["Oper"] = "UnSend()";
                            dt.Rows.Add(dr);
                        }

                        //流程结束
                        if (powers.Contains("FlowDataOver") == true)
                        {
                            dr = dt.NewRow();
                            dr["No"] = "EndFlow";
                            dr["Name"] = btnLab.EndFlowLab;
                            dr["Oper"] = "DoStop('" + btnLab.EndFlowLab + "','" + this.FlowNo + "','" + this.WorkID + "');";
                            dt.Rows.Add(dr);
                        }

                        //催办
                        if (powers.Contains("FlowDataPress") == true ||( gwf.Emps.Contains(WebUser.No) == true && btnLab.PressEnable==true))
                        {
                            dr = dt.NewRow();
                            dr["No"] = "Press";
                            dr["Name"] = btnLab.GetValStringByKey(BtnAttr.PressLab);
                            dr["Oper"] = "Press();";
                            dt.Rows.Add(dr);
                        }
                        break;
                    case WFState.Complete: // 完成.
                    case WFState.Delete:   // 逻辑删除..
                        /*恢复使用流程*/
                        if (WebUser.No.Equals("admin") == true || powers.Contains("FlowDataRollback") == true || (gwf.Emps.Contains(WebUser.No) == true && btnLab.GetValBooleanByKey(BtnAttr.RollbackEnable) == true))
                        {
                            dr = dt.NewRow();
                            dr["No"] = "Rollback";
                            dr["Name"] = btnLab.GetValStringByKey(BtnAttr.RollbackLab);
                            dr["Oper"] = "";
                            dt.Rows.Add(dr);
                        }

                        break;
                    default:
                        break;
                }

                if (btnLab.GetValBooleanByKey(BtnAttr.ShowParentFormEnableMyView) && this.PWorkID != 0)
                {
                    /*如果要查看父流程.*/
                    dr = dt.NewRow();
                    dr["No"] = "ParentForm";
                    dr["Name"] = btnLab.ShowParentFormLab;
                    dr["Oper"] = "";

                    dt.Rows.Add(dr);
                }
                if (btnLab.GetValBooleanByKey(BtnAttr.TrackEnableMyView))
                {
                    dr = dt.NewRow();
                    dr["No"] = "Track";
                    dr["Name"] = btnLab.TrackLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                
                #endregion 根据流程权限控制规则获取可以操作的按钮功能

                #region 加载流程查看器 - 按钮

                /* 判断是否是分合流？ 从而增加子线程按钮.*/
                if (gwf.WFState == WFState.Runing)
                {
                    if (nd.ItIsFLHL == true)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Thread";
                        dr["Name"] = btnLab.ThreadLab;
                        dr["Oper"] = "";
                        dt.Rows.Add(dr);
                    }
                }

                /* 打包下载zip */
                if (btnLab.PrintZipMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_zip";
                    dr["Name"] = btnLab.PrintZipLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载html */
                if (btnLab.PrintHtmlMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_html";
                    dr["Name"] = btnLab.PrintHtmlLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载pdf */
                if (btnLab.PrintPDFMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_pdf";
                    dr["Name"] = btnLab.PrintPDFLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                if (btnLab.PrintDocEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PrintDoc";
                    dr["Name"] = btnLab.PrintDocLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);


                }

                /**数据库版本*/
                if (btnLab.FrmDBVerMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FrmDBVer";
                    dr["Name"] = btnLab.FrmDBVerLab;
                    dr["Oper"] = "FrmDBVer_Init()";
                    dt.Rows.Add(dr);
                }

                //数据批阅
                if (btnLab.GetValIntByKey(BtnAttr.FrmDBRemarkEnableMyView) != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FrmDBRemark";
                    dr["Name"] = btnLab.FrmDBRemarkLab;
                    dr["Oper"] = "FrmDBRemark("+ btnLab.FrmDBRemarkEnable+")";
                    dt.Rows.Add(dr);
                }
                //评论
                if (btnLab.FlowBBSRole != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FlowBBS";
                    dr["Name"] = btnLab.FlowBBSLab;
                    dr["Oper"] = btnLab.FlowBBSRole;
                    dt.Rows.Add(dr);
                }

                /* 公文标签 */
                if (btnLab.OfficeBtnEnable == true && btnLab.OfficeBtnLocal == 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "DocWord";
                    dr["Name"] = btnLab.OfficeBtnLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                #endregion 加载流程查看器 - 按钮

                #region  加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.NodeID, NodeToolbarAttr.IsMyView, 1, NodeToolbarAttr.Idx);
                foreach (NodeToolbar bar in bars)
                {

                    if (bar.ExcType == 1 || (!DataType.IsNullOrEmpty(bar.Target) == false && bar.Target.ToLower() == "javascript"))
                    {
                        dr = dt.NewRow();
                        dr["No"] = "NodeToolBar";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = bar.Url;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.NodeID + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&s=" + tKey;
                        dr = dt.NewRow();
                        dr["No"] = "NodeToolBar";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = "WinOpen('" + urlr3 + "')";
                        dt.Rows.Add(dr);
                    }
                }
                #endregion  //加载自定义的button.

            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex);
                toolbar = "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dt);
        }

        public string MyFrm_InitToolBar()
        {
            DataTable dt = new DataTable("ToolBar");
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Oper");

            BtnLab btnLab = new BtnLab(this.NodeID);
            string tKey = DateTime.Now.ToString("MM-dd-hh:mm:ss");
            string toolbar = "";
            try
            {
                DataRow dr = dt.NewRow();
                dr["No"] = "Close";
                dr["Name"] = "关闭";
                dr["Oper"] = "Close();";
                dt.Rows.Add(dr);

                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

                #region 根据流程权限控制规则获取可以操作的按钮功能

                dr = dt.NewRow();
                dr["No"] = "Track";
                dr["Name"] = "轨迹";
                dr["Oper"] = "";
                dt.Rows.Add(dr);
                #endregion 根据流程权限控制规则获取可以操作的按钮功能

                #region 加载流程查看器 - 按钮

                /* 打包下载zip */
                if (btnLab.PrintZipMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_zip";
                    dr["Name"] = btnLab.PrintZipLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载html */
                if (btnLab.PrintHtmlMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_html";
                    dr["Name"] = btnLab.PrintHtmlLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载pdf */
                if (btnLab.PrintPDFMyView == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_pdf";
                    dr["Name"] = btnLab.PrintPDFLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                /* 公文标签 */
                if (btnLab.OfficeBtnEnable == true && btnLab.OfficeBtnLocal == 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "DocWord";
                    dr["Name"] = btnLab.OfficeBtnLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                #endregion 加载流程查看器 - 按钮
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex);
                toolbar = "err@" + ex.Message;
            }
            return BP.Tools.Json.ToJson(dt);
        }


        /// <summary>
        /// 撤销
        /// </summary>
        /// <returns></returns>
        public string MyView_UnSend()
        {
            bool isUnDelayedSend = this.GetRequestValBoolen("IsUnDelayedSend");
            //是否撤销延期发送
            if(isUnDelayedSend == true)
            {
                GenerWorkerList gwl = new GenerWorkerList();
                int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.NodeID, GenerWorkerListAttr.FK_Emp, WebUser.No);
                if (i != 0)
                {
                    gwl.WhoExeIt = 0;
                    gwl.SetPara("Day",0);
                    gwl.SetPara("hour", 0);
                    gwl.SetPara("Minute", 0);
                    gwl.SetPara("DelayedData", "");
                    gwl.SetPara("ToNodeID", 0);
                    gwl.SetPara("ToEmps", "");
                    gwl.Update();
                }
                return "撤销成功";
            }
            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    currNode = "SELECT FK_Node FROM (SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC ) WHERE rownum=1";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    currNode = "SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC LIMIT 1";
                    break;
                case DBType.MSSQL:
                    currNode = "SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC";
                    break;
                default:
                    currNode = "SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC";
                    break;
            }
            String unSendToNode = DBAccess.RunSQLReturnString(currNode);
            if (DataType.IsNullOrEmpty(unSendToNode) == true)
                return "err@你没有撤销当前流程的权限";
            try
            {
                return BP.WF.Dev2Interface.Flow_DoUnSend(this.FlowNo, this.WorkID, int.Parse(unSendToNode), this.FID);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 是否可以查看工作.
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        public bool IsCanView(GenerWorkFlow gwf)
        {
            //是否可以处理当前工作？
            bool isCanDoCurrWorker = gwf.TodoEmps.Contains(WebUser.No + "," + WebUser.Name + ";");
            //   bool isCanDoCurrWorker = gwf.TodoEmps.Contains(WebUser.No + ",");

            if (isCanDoCurrWorker)
                return true;

            //如果是发起人.
            if (gwf.Starter.Equals(WebUser.No))
                return true;

            //如果是本部门发起的.
            if (gwf.DeptNo.Equals(WebUser.DeptNo))
                return true;

            //是否是工作参与人?
            string emps = gwf.Emps;
            if (DataType.IsNullOrEmpty(emps) == false)
                emps += "@";
            bool isWorker = gwf.Emps.Contains("@" + WebUser.No + "," + WebUser.Name);
            if (isWorker == true || emps.Contains("@" + WebUser.No + "@") == true)
                return true;

            if (WebUser.No.Equals("admin") == true)
                return true;

            if (WebUser.IsAdmin == true && gwf.OrgNo.Equals(WebUser.OrgNo) == true)
                return true;

            //处理流程控制权限.
            TruckViewPower viewEn = new TruckViewPower(gwf.FlowNo);

            #region 基本权限控制.
            //如果任何人可见.
            if (viewEn.PAnyOne == true)
                return true;

            if (viewEn.PSpecDept == true && DataType.IsNullOrEmpty(viewEn.PSpecDeptExt) == false)
            {
                viewEn.PSpecDeptExt += ",";
                if (viewEn.PSpecDeptExt.Equals(WebUser.DeptNo + ",") == true)
                    return true;
            }

            #endregion 基本权限控制.

            #region 按照部门控制.
            //本部门可见.
            if (viewEn.PMyDept == true)
            {
                if (gwf.DeptNo.Equals(WebUser.DeptNo) == true)
                    return true;
            }

            //直属上级部门可看(比如:我是).
            if (viewEn.PPMyDept == true)
            {
                //上级部门可见.
                Dept dept = new Dept(gwf.DeptNo);
                if (dept.ParentNo.Equals(WebUser.DeptNo) == true)
                    return true;
            }

            //上级部门可看
            if (viewEn.PPMyDept == true)
            {
                //上级部门可见.
                Dept dept = new Dept(gwf.DeptNo);
                if (dept.ParentNo.Equals(WebUser.DeptNo) == true)
                    return true;
            }

            //同级部门可见.
            if (viewEn.PSameDept == true)
            {
                //如果发起人的部门，与当前人员的部门是同一级部门.
                Dept dept = new Dept(gwf.DeptNo);
                Dept mydept = new Dept(WebUser.DeptNo);
                if (mydept.ParentNo.Equals(dept.ParentNo) == true)
                    return true;
            }
            #endregion 按照部门控制.
            if (viewEn.PSpecSta == true && DataType.IsNullOrEmpty(viewEn.PSpecStaExt) == false)
            {
                string sql = "Select FK_Station From Port_DeptEmpStation Where FK_Emp='" + WebUser.UserID + "'";
                string stas = DBAccess.RunSQLReturnStringIsNull(sql, "");
                if (DataType.IsNullOrEmpty(stas) == false)
                {
                    viewEn.PSpecStaExt += ",";
                    foreach (string sta in stas.Split(','))
                        if (viewEn.PSpecStaExt.Equals(sta + ",") == true)
                            return true;
                }

            }
            #region 指定角色可见
            #endregion 指定角色可见

            #region 指定人员可见
            if (viewEn.PSpecEmp == true && DataType.IsNullOrEmpty(viewEn.PSpecEmpExt) == false)
            {
                viewEn.PSpecEmpExt += ",";
                if (viewEn.PSpecEmpExt.Equals(WebUser.No + ",") == true)
                    return true;
            }
            #endregion 指定人员可见

            return false;
        }


        /// <summary>
        /// 初始化(处理分发)
        /// </summary>
        /// <returns></returns>
        public string MyView_Init()
        {
            //获得注册实体.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.WFState == WFState.Delete)
                return "err@当前流程已经删除无法查看.";

            if (gwf.WFState == WFState.Blank)
                return "err@当前流程还没有启动.";

            if (gwf.WFState == WFState.Draft)
                return "err@当前流程还是草稿.";

            //是否可以处理当前工作？
            bool isCanDoCurrWorker = false;

            string toDoEmps = ";" + gwf.TodoEmps;
            bool isReadonly = this.GetRequestValBoolen("IsReadonly");
            //当前的流程还是运行中的，并且可以执行当前工作,如果是，就直接转到工作处理器.
            if (gwf.FID != 0)
            {
                Node nd = new Node(gwf.NodeID);
                if (nd.ItIsSubThread ==true && toDoEmps.Contains(";" + WebUser.No + ",") && isReadonly==false)
                {
                    WF_MyFlow handler = new WF_MyFlow();
                    return handler.MyFlow_Init();
                }
            }

            if (gwf.FID == 0 && gwf.WFState != WFState.Complete && toDoEmps.Contains(";" + WebUser.No + ",") && isReadonly == false)
            {
                WF_MyFlow handler = new WF_MyFlow();
                return handler.MyFlow_Init();
            }

            //是否是工作参与人?
            bool isWorker = gwf.Emps.Contains("@" + WebUser.No + "," + WebUser.Name);
            if (isWorker == true || WebUser.No.Equals("admin") == true
                || WebUser.IsAdmin == true)
            {
                //可以查看工作,就执行以后的.
            }
            else
            {
                //判断是否是抄送人员?
                CCList list = new CCList();
                bool isExit = list.IsExit(CCListAttr.WorkID, this.WorkID,
                    CCListAttr.CCTo, WebUser.No);
                //如果是抄送人员.
                if (isExit == true)
                {
                    //是一个抄送人员.
                    WF_MyCC mycc = new WF_MyCC();
                    return mycc.MyCC_Init();
                }
            }

            if (IsCanView(gwf) == false)
            {
                string msg = "err@您无权查看该工作,";
                msg += "\t\n如下情况可以查看该工作.";
                msg += "\t\n1. 该流程发起人, 审批人，抄送人，可以查看.";
                msg += "\t\n2. 默认与发起人是同一个部门的人可以查看.";
                msg += "\t\n3. 二级管理员可以查看本组织的工作.";
                msg += "\t\n4. 超级管理员可以查看.";
                msg += "\t\n5. 流程属性的权限控制设置权限的人可以查看.";
                msg += "\t\n6. 如果该流程的数据，任何人都可以查看，请在流程属性里设置权限控制，任何人可见.";
                return msg;
            }

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree
                 || this.currFlow.FlowDevModel == FlowDevModel.FrmTree)
            {

                #region 开始组合url.
                string toUrl = "";

                if (this.ItIsMobile == true)
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyViewGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyViewGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }
                else
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyViewTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyViewTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FlowNo + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }

                string[] strs = this.RequestParas.Split('&');
                foreach (string str in strs)
                {
                    if (toUrl.Contains(str) == true)
                        continue;
                    if (str.Contains("DoType=") == true)
                        continue;
                    if (str.Contains("DoMethod=") == true)
                        continue;
                    if (str.Contains("HttpHandlerName=") == true)
                        continue;
                    if (str.Contains("IsLoadData=") == true)
                        continue;
                    if (str.Contains("IsCheckGuide=") == true)
                        continue;

                    toUrl += "&" + str;
                }
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (toUrl.Contains(key + "=") == true)
                        continue;
                    toUrl += "&" + key + "=" + HttpContextHelper.RequestParams(key);
                }
                #endregion 开始组合url.

                //增加fk_node
                if (toUrl.Contains("&FK_Node=") == false)
                    toUrl += "&FK_Node=" + this.currND.NodeID;
                return "url@" + toUrl;
            }

            if (this.currND.HisFormType == NodeFormType.SDKForm
                || this.currFlow.FlowDevModel == FlowDevModel.SDKFrmSelfPK
                || this.currFlow.FlowDevModel == FlowDevModel.SDKFrmWorkID)
            {
                string url = currND.FormUrl;
                if (DataType.IsNullOrEmpty(url))
                {
                    return "err@设置读取状流程设计错误态错误,没有设置表单url.";
                }

                //处理连接.
                url = this.MyView_Init_DealUrl(currND, url);

                //sdk表单就让其跳转.
                return "url@" + url;
            }
            #endregion 处理表单类型.

            //求出当前节点frm的类型.
            NodeFormType frmtype = this.currND.HisFormType;
            if (frmtype != NodeFormType.RefOneFrmTree)
            {
                currND.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

                if (this.currND.NodeFrmID.Contains(this.currND.NodeID.ToString()) == false)
                {
                    /*如果当前节点引用的其他节点的表单.*/
                    string nodeFrmID = currND.NodeFrmID;
                    string refNodeID = nodeFrmID.Replace("ND", "");
                    BP.WF.Node nd = new Node(int.Parse(refNodeID));

                    //表单类型.
                    frmtype = nd.HisFormType;
                }
            }

            #region 内置表单类型的判断.
            /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上，为软通动力改造。*/

            if (frmtype == NodeFormType.FoolTruck)
            {
                string url = "MyViewGener.htm";

                //处理连接.
                url = this.MyView_Init_DealUrl(currND, url);
                return "url@" + url;
            }

            if (frmtype == NodeFormType.WebOffice)
            {
                string url = "MyViewWebOffice.htm";

                //处理连接.
                url = this.MyView_Init_DealUrl(currND, url);
                return "url@" + url;
            }

            if (frmtype == NodeFormType.FoolForm && this.ItIsMobile == false)
            {
                string url = "MyViewGener.htm";
                if (this.ItIsMobile)
                    url = "MyViewGener.htm";

                //处理连接.
                url = this.MyView_Init_DealUrl(currND, url);

                url = url.Replace("DoType=MyView_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }

            //自定义表单
            if ((frmtype == NodeFormType.SelfForm || this.currFlow.FlowDevModel == FlowDevModel.SelfFrm) && this.ItIsMobile == false)
            {

                string url = "MyViewSelfForm.htm";

                //处理连接.
                url = this.MyView_Init_DealUrl(currND, url);

                url = url.Replace("DoType=MyView_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }
            #endregion 内置表单类型的判断.

            string myurl = "MyViewGener.htm";
            //MapData md = new MapData(this.currND.NodeFrmID);
            //if (md.HisFrmType == FrmType.ChapterFrm)
             //   myurl = "MyViewTree.htm?NodeFrmType=11";
            //处理连接.
            myurl = this.MyView_Init_DealUrl(currND, myurl);
            myurl = myurl.Replace("DoType=MyView_Init&", "");
            myurl = myurl.Replace("&DoWhat=StartClassic", "");

            return "url@" + myurl;
        }
        private string MyView_Init_DealUrl(BP.WF.Node currND, string url = null)
        {
            if (url == null)
                url = currND.FormUrl;

            string urlExt = this.RequestParas;
            //防止查询不到.
            urlExt = urlExt.Replace("?WorkID=", "&WorkID=");
            if (urlExt.Contains("&WorkID") == false)
            {
                urlExt += "&WorkID=" + this.WorkID;
            }
            else
            {
                urlExt = urlExt.Replace("&WorkID=0", "&WorkID=" + this.WorkID);
                urlExt = urlExt.Replace("&WorkID=&", "&WorkID=" + this.WorkID + "&");
            }

            //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上,珠海驰骋提出. 
            url = url.Replace("@SDKFromServHost", BP.Difference.SystemConfig.AppSettings["SDKFromServHost"]);

            if (urlExt.Contains("&NodeID") == false)
                urlExt += "&NodeID=" + currND.NodeID;

            if (urlExt.Contains("FK_Node") == false)
                urlExt += "&FK_Node=" + currND.NodeID;

            if (urlExt.Contains("&FID") == false)
                urlExt += "&FID=" + this.FID;

            if (urlExt.Contains("&UserNo") == false)
                urlExt += "&UserNo=" + HttpUtility.UrlEncode(WebUser.No);

            if (urlExt.Contains("&Token") == false)
                urlExt += "&Token=" + WebUser.Token;

            if (url.Contains("?") == true)
                url += "&" + urlExt;
            else
                url += "?" + urlExt;

            foreach (string str in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(str) == true || str.ToLower().Equals("t") == true)
                    continue;
                if (url.Contains(str + "=") == true)
                    continue;
                url += "&" + str + "=" + this.GetRequestVal(str);
            }

            url = url.Replace("?&", "?");
            url = url.Replace("&&", "&");
            return url;
        }


        #region 表单树操作
        /// <summary>
        /// 获取表单树数据
        /// </summary>
        /// <returns></returns>
        public string FlowFormTree_Init()
        {
            BP.WF.Template.FlowFormTrees appFlowFormTree = new FlowFormTrees();

            //add root
            BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
            root.No = "1";
            root.ParentNo = "0";
            root.Name = "目录";
            root.NodeType = "root";
            appFlowFormTree.AddEntity(root);

            #region 添加表单及文件夹

            //节点表单
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);

            FrmNodes frmNodes = new FrmNodes();
            frmNodes.Retrieve(FrmNodeAttr.FK_Node, this.NodeID, FrmNodeAttr.Idx);

            //文件夹
            //SysFormTrees formTrees = new SysFormTrees();
            //formTrees.RetrieveAll(SysFormTreeAttr.Name);

            //所有表单集合. 为了优化效率,这部分重置了一下.
            MapDatas mds = new MapDatas();
            if (frmNodes.Count <= 3)
            {
                foreach (FrmNode fn in frmNodes)
                {
                    MapData md = new MapData(fn.FK_Frm);
                    mds.AddEntity(md);
                }
            }
            else
            {
                mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.NodeID);
            }


            string frms = HttpContextHelper.RequestParams("Frms");
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (DataType.IsNullOrEmpty(frms) == true)
            {
                frms = gwf.Paras_Frms;
            }
            else
            {
                gwf.Paras_Frms = frms;
                gwf.Update();
            }

            foreach (FrmNode frmNode in frmNodes)
            {
                #region 增加判断是否启用规则.
                switch (frmNode.FrmEnableRole)
                {
                    case FrmEnableRole.Allways:
                        break;
                    case FrmEnableRole.WhenHaveData: //判断是否有数据.
                        MapData md = mds.GetEntityByKey(frmNode.FK_Frm) as MapData;
                        if (md == null)
                            continue;
                        Int64 pk = this.WorkID;
                        switch (frmNode.WhoIsPK)
                        {
                            case WhoIsPK.FID:
                                pk = this.FID;
                                break;
                            case WhoIsPK.PWorkID:
                                pk = this.PWorkID;
                                break;
                            case WhoIsPK.CWorkID:
                                pk = this.CWorkID;
                                break;
                            case WhoIsPK.OID:
                            default:
                                pk = this.WorkID;
                                break;
                        }
                        if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) as Num FROM " + md.PTable + " WHERE OID=" + pk) == 0)
                            continue;
                        break;
                    case FrmEnableRole.WhenHaveFrmPara: //判断是否有参数.

                        frms = frms.Trim();
                        frms = frms.Replace(" ", "");
                        frms = frms.Replace(" ", "");

                        if (DataType.IsNullOrEmpty(frms) == true)
                        {
                            continue;
                            //return "err@当前表单设置为仅有参数的时候启用,但是没有传递来参数.";
                        }

                        if (frms.Contains(",") == false)
                        {
                            if (frms != frmNode.FK_Frm)
                                continue;
                        }

                        if (frms.Contains(",") == true)
                        {
                            if (frms.Contains(frmNode.FK_Frm + ",") == false)
                                continue;
                        }

                        break;
                    case FrmEnableRole.ByFrmFields:
                        throw new Exception("@这种类型的判断，ByFrmFields 还没有完成。");

                    case FrmEnableRole.BySQL: // 按照SQL的方式.
                        string mysql = frmNode.FrmEnableExp.Clone() as string;

                        if (DataType.IsNullOrEmpty(mysql) == true)
                        {
                            MapData FrmMd = new MapData(frmNode.FK_Frm);
                            return "err@表单" + frmNode.FK_Frm + ",[" + FrmMd.Name + "]在节点[" + frmNode.NodeID + "]启用方式按照sql启用但是您没有给他设置sql表达式.";
                        }


                        mysql = mysql.Replace("@OID", this.WorkID.ToString());
                        mysql = mysql.Replace("@WorkID", this.WorkID.ToString());

                        mysql = mysql.Replace("@NodeID", this.NodeID.ToString());
                        mysql = mysql.Replace("@FK_Node", this.NodeID.ToString());

                        mysql = mysql.Replace("@FK_Flow", this.FlowNo);

                        mysql = mysql.Replace("@WebUser.No", WebUser.No);
                        mysql = mysql.Replace("@WebUser.Name", WebUser.Name);
                        mysql = mysql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);


                        //替换特殊字符.
                        mysql = mysql.Replace("~", "'");

                        if (DBAccess.RunSQLReturnValFloat(mysql) <= 0)
                            continue;
                        break;

                    case FrmEnableRole.ByStation:
                        string exp = frmNode.FrmEnableExp.Clone() as string;
                        string Sql = "SELECT FK_Station FROM Port_DeptEmpStation where FK_Emp='" + WebUser.UserID + "'";
                        string station = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(station) == true)
                            continue;
                        string[] stations = station.Split(';');
                        bool isExit = false;
                        foreach (string s in stations)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;
                        break;


                    case FrmEnableRole.ByDept:
                        exp = frmNode.FrmEnableExp.Clone() as string;
                        Sql = "SELECT FK_Dept FROM Port_DeptEmp where FK_Emp='" + WebUser.No + "'";
                        string dept = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(dept) == true)
                            continue;
                        string[] deptStrs = dept.Split(';');
                        isExit = false;
                        foreach (string s in deptStrs)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;

                        break;
                    case FrmEnableRole.ByEmps:
                        string myexp = frmNode.FrmEnableExp.Clone() as string;
                        if (DataType.IsNullOrEmpty(myexp) == true)
                            throw new Exception("err@按照人员控制表单启用规则，但是您没有设置人员.");

                        myexp = "," + myexp + ",";
                        if (myexp.Contains("," + WebUser.No + ",") == false)
                            continue;

                        break;
                    case FrmEnableRole.Disable: // 如果禁用了，就continue出去..
                        continue;
                    default:
                        throw new Exception("@没有判断的规则." + frmNode.FrmEnableRole);
                }
                #endregion

                #region 检查是否有没有目录的表单?
                bool isHave = false;
                foreach (MapData md in mds)
                {
                    if (md.FormTreeNo == "")
                    {
                        isHave = true;
                        break;
                    }
                }

                string treeNo = "0";
                if (isHave && mds.Count == 1)
                {
                    treeNo = "00";
                }
                else if (isHave == true)
                {
                    foreach (MapData md in mds)
                    {
                        if (md.FormTreeNo != "")
                        {
                            treeNo = md.FormTreeNo;
                            break;
                        }
                    }
                }
                #endregion 检查是否有没有目录的表单?

                foreach (MapData md in mds)
                {
                    if (frmNode.FK_Frm != md.No)
                        continue;

                    if (md.FormTreeNo == "")
                        md.FormTreeNo = treeNo;

                    //给他增加目录.
                    if (appFlowFormTree.Contains("Name", md.FormTreeText) == false)
                    {
                        BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                        nodeFolder.No = md.FormTreeNo;
                        nodeFolder.ParentNo = "1";
                        nodeFolder.Name = md.FormTreeText;
                        nodeFolder.NodeType = "folder";
                        appFlowFormTree.AddEntity(nodeFolder);
                    }

                    //检查必填项.
                    bool IsNotNull = false;
                    FrmFields formFields = new FrmFields();
                    QueryObject obj = new QueryObject(formFields);
                    obj.AddWhere(FrmFieldAttr.FK_Node, this.NodeID);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.FrmID, md.No);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.IsNotNull, 1);
                    obj.DoQuery();
                    if (formFields != null && formFields.Count > 0)
                        IsNotNull = true;

                    BP.WF.Template.FlowFormTree nodeForm = new BP.WF.Template.FlowFormTree();
                    nodeForm.No = md.No;
                    nodeForm.ParentNo = md.FormTreeNo;

                    //设置他的表单显示名字. 2019.09.30
                    string frmName = md.Name;
                    FrmNode fn = frmNodes.GetEntityByKey(FrmNodeAttr.FK_Frm, md.No) as FrmNode;
                    if (fn != null)
                    {
                        string str = fn.FrmNameShow;
                        if (DataType.IsNullOrEmpty(str) == false)
                            frmName = str;
                    }
                    nodeForm.Name = frmName;
                    nodeForm.NodeType = IsNotNull ? "form|1" : "form|0";
                    nodeForm.IsEdit = frmNode.ItIsEditInt.ToString();// Convert.ToString(Convert.ToInt32(frmNode.IsEdit));
                    nodeForm.IsCloseEtcFrm = frmNode.ItIsCloseEtcFrmInt.ToString();
                    appFlowFormTree.AddEntity(nodeForm);
                    break;
                }
            }
            #endregion

            //扩展工具，显示位置为表单树类型. 

            //增加到数据结构上去.
            TansEntitiesToGenerTree(appFlowFormTree, root.No, "");


            return appendMenus.ToString();
        }
        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityTree root = ens.GetEntityByKey(rootNo) as EntityTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");

            //attributes
            BP.WF.Template.FlowFormTree formTree = root as BP.WF.Template.FlowFormTree;
            if (formTree != null)
            {
                string url = formTree.Url == null ? "" : formTree.Url;
                url = url.Replace("/", "|");
                appendMenus.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"IsCloseEtcFrm\":\"" + formTree.IsCloseEtcFrm + "\",\"Url\":\"" + url + "\"}");
            }
            appendMenus.Append(",iconCls:\"icon-Wave\"");
            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens, checkIds);

            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        private void AddChildren(EntityTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");


                //attributes
                BP.WF.Template.FlowFormTree formTree = item as BP.WF.Template.FlowFormTree;
                if (formTree != null)
                {
                    string url = formTree.Url == null ? "" : formTree.Url;
                    string ico = "icon-tree_folder";
                    if (BP.Difference.SystemConfig.SysNo == "YYT")
                    {
                        ico = "icon-boat_16";
                    }
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"IsCloseEtcFrm\":\"" + formTree.IsCloseEtcFrm + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form|0")
                    {
                        ico = "form0";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Wave";
                        }
                    }
                    if (formTree.NodeType == "form|1")
                    {
                        ico = "form1";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Shark_20";
                        }
                    }
                    if (formTree.NodeType.Contains("tools"))
                    {
                        ico = "icon-4";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Wave";
                        }
                    }
                    appendMenuSb.Append(",iconCls:\"");
                    appendMenuSb.Append(ico);
                    appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
        #endregion

        /// <summary>
        /// 产生一个工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            try
            {
                DataSet ds = new DataSet();

                Int64 workID = this.WorkID;
                if (this.currND.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    MapData md = new MapData(this.currND.NodeFrmID);
                    if (md.HisFrmType == FrmType.ChapterFrm)
                    {
                        string url = "Frm.htm?FK_MapData=" + md.No;
                        url = MyView_Init_DealUrl(this.currND, url);
                        return "url@" + url;
                    }

                    //获取绑定的表单
                    FrmNode frmnode = new FrmNode(this.NodeID, this.currND.NodeFrmID);
                    switch (frmnode.WhoIsPK)
                    {
                        case WhoIsPK.FID:
                            workID = this.FID;
                            break;
                        case WhoIsPK.PWorkID:
                            workID = this.PWorkID;
                            break;
                        case WhoIsPK.P2WorkID:
                            GenerWorkFlow gwff = new GenerWorkFlow(this.PWorkID);
                            workID = gwff.PWorkID;
                            break;
                        case WhoIsPK.P3WorkID:
                            string sqlId = "Select PWorkID From WF_GenerWorkFlow Where WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + this.PWorkID + ")";
                            workID = DBAccess.RunSQLReturnValInt(sqlId, 0);
                            break;
                        case WhoIsPK.RootFlowWorkID:
                            workID = BP.WF.Dev2Interface.GetRootWorkIDBySQL(this.WorkID, this.PWorkID);
                            break;
                        default:
                            break;
                    }

                }

                ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FlowNo, this.currND, workID,
                    this.FID, BP.Web.WebUser.No, this.WorkID, "1", true);

                #region 如果是移动应用就考虑多表单的问题.
                if (currND.HisFormType == NodeFormType.SheetTree && this.ItIsMobile == true)
                {
                    /*如果是表单树并且是，移动模式.*/


                    FrmNodes fns = new FrmNodes();
                    QueryObject qo = new QueryObject(fns);

                    qo.AddWhere(FrmNodeAttr.FK_Node, currND.NodeID);
                    qo.addAnd();
                    qo.AddWhere(FrmNodeAttr.FrmEnableRole, "!=", (int)FrmEnableRole.Disable);
                    qo.addOrderBy("Idx");
                    qo.DoQuery();


                    //把节点与表单的关联管理放入到系统.
                    ds.Tables.Add(fns.ToDataTableField("FrmNodes"));
                }
                #endregion 如果是移动应用就考虑多表单的问题.

                if (WebUser.SysLang.Equals("CH") == true)
                    return BP.Tools.Json.ToJson(ds);

                //#region 处理多语言.
                //if (WebUser.SysLang.Equals("CH") == false)
                //{
                //    Langues langs = new Langues();
                //    langs.Retrieve(LangueAttr.Model, LangueModel.CCForm,
                //        LangueAttr.Sort, "Fields", LangueAttr.Langue, WebUser.SysLang); //查询语言.
                //}
                //#endregion 处理多语言.

                return BP.Tools.Json.ToJson(ds);


            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex);
                return "err@" + ex.Message;
            }
        }

    }
}
