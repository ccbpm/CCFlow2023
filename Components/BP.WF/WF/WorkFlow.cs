﻿using System;
using BP.En;
using BP.Web;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
using BP.Difference;


namespace BP.WF
{
    public enum HungupSta
    {
        /// <summary>
        /// 申请
        /// </summary>
        Apply,
        /// <summary>
        /// 同意
        /// </summary>
        Agree,
        /// <summary>
        /// 拒绝
        /// </summary>
        Reject
    }
    /// <summary>
    /// WF 的摘要说明。
    /// 工作流
    /// 这里包含了两个方面
    /// 工作的信息．
    /// 流程的信息．
    /// </summary>
    public class WorkFlow
    {
        #region 当前工作统计信息
        /// <summary>
        /// 正常范围的运行的个数。
        /// </summary>
        public static int NumOfRuning(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=0";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 进入警告期限的个数
        /// </summary>
        public static int NumOfAlert(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=1";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 逾期
        /// </summary>
        public static int NumOfTimeout(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=2";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        #endregion

        #region  权限管理
        /// <summary>
        /// 是不是能够作当前的工作。
        /// </summary>
        /// <param name="empId">工作人员ID</param>
        /// <returns>是不是能够作当前的工作</returns>
        public bool ItIsCanDoCurrentWork(string empId)
        {
            WorkNode wn = this.GetCurrentWorkNode();
            return BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(wn.WorkID, empId);
            #region 使用dev2InterFace 中的算法
            //return true;
            // 找到当前的工作节点

            // 判断是不是开始工作节点..
            if (wn.HisNode.ItIsStartNode)
            {
                // 从物理上判断是不是有这个权限。
                // return WorkFlow.IsCanDoWorkCheckByEmpStation(wn.HisNode.NodeID, empId);
                return true;
            }

            // 判断他的工作生成的工作者.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID, wn.HisNode.NodeID);
            if (gwls.Count == 0)
            {
                //return true;
                //throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:工作ID="+this.WorkID+",节点ID="+wn.HisNode.NodeID );
                throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:WorkID=" + this.WorkID + ",NodeID=" + wn.HisNode.NodeID);
            }

            foreach (GenerWorkerList en in gwls)
            {
                if (en.EmpNo == empId)
                    return true;
            }
            return false;
            #endregion
        }
        #endregion

        #region 流程公共方法
        /// <summary>
        /// 执行驳回
        /// 应用场景:子流程向分合点驳回时
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="fk_node">被驳回的节点</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string DoHungupReject(Int64 fid, int fk_node, string msg)
        {
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, fk_node);

            //if (i == 0)
            //    throw new Exception("系统错误，没有找到应该找到的数据。");

            i = wl.Delete();
            //if (i == 0)
            //    throw new Exception("系统错误，没有删除应该删除的数据。");

            wl = new GenerWorkerList();
            i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.IsPass, 3);

            //if (i == 0)
            //    throw new Exception("系统错误，想找到退回的原始起点没有找到。");

            Node nd = new Node(fk_node);
            // 更新当前流程管理表的设置当前的节点。
            DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET FK_Node=" + fk_node + ", NodeName='" + nd.Name + "' WHERE WorkID=" + this.WorkID);

            wl.ItIsPass = false;
            wl.Update();

            return "工作已经驳回到(" + wl.EmpNo + " , " + wl.EmpName + ")";
            // wl.HisNode
        }
        /// <summary>
        /// 逻辑删除流程
        /// </summary>
        /// <param name="msg">逻辑删除流程原因，可以为空。</param>
        public void DoDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

                BP.WF.Node nd = new Node(gwf.NodeID);
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.RetrieveFromDBSources();

                //定义workNode.
                WorkNode wn = new WorkNode(wk, nd);

                //调用结束前事件.
                ExecEvent.DoFlow(EventListFlow.BeforeFlowDel, wn, null);

                //记录日志 感谢 itdos and 888 , 提出了这个问题..
                wn.AddToTrack(ActionType.DeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);

                //更新-流程数据表的状态. 
                string sql = "UPDATE  " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Delete + " WHERE OID=" + this.WorkID;
                DBAccess.RunSQL(sql);

                //删除他的工作者，不让其有待办.
                sql = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID;
                DBAccess.RunSQL(sql);

                //设置产生的工作流程为.
                gwf.WFState = BP.WF.WFState.Delete;
                gwf.Update();

                //调用结束后事件.
                ExecEvent.DoFlow(EventListFlow.AfterFlowDel, wn, null);

            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 恢复逻辑删除流程
        /// </summary>
        /// <param name="msg">回复原因,可以为空.</param>
        public void DoUnDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + " WHERE  WorkID=" + this.WorkID);

                //设置产生的工作流程为.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

                //回复数据.
                BP.WF.Dev2Interface.Flow_DoRebackWorkFlow(gwf.FlowNo, gwf.WorkID, gwf.NodeID, msg);


                WorkNode wn = new WorkNode(WorkID, gwf.NodeID);
                wn.AddToTrack(ActionType.UnDeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 删除已经完成的流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <param name="note">删除原因</param>
        /// <returns>删除信息</returns>
        public static string DoDeleteWorkFlowAlreadyComplete(string flowNo, Int64 workID, bool isDelSubFlow, string note)
        {
            BP.DA.Log.DebugWriteInfo("开始删除流程:流程编号:" + flowNo + "-WorkID:" + workID + "-" + ". 是否要删除子流程:" + isDelSubFlow + ";删除原因:" + note);

            Flow fl = new Flow(flowNo);

            #region 记录流程删除日志
            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt");
            rpt.SetValByKey(GERptAttr.OID, workID);
            rpt.Retrieve();
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = workID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDateTime;
                log.OperDept = WebUser.DeptNo;
                log.OperDeptName = WebUser.DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = note;
                log.OID = workID;
                log.FlowNo = flowNo;
                log.FlowSortNo = fl.FlowSortNo;
                log.InsertAsOID(log.OID);
            }
            catch (Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                return ex.StackTrace;
            }
            #endregion 记录流程删除日志

            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workID);
            DBAccess.RunSQL("DELETE FROM " + fl.PTable + " WHERE OID=" + workID);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + workID); // 删除质量考核数据。

            string info = "";

            #region 正常的删除信息.
            string msg = "";
            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workID);

                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + workID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");

                //删除所有节点上的数据.
                Nodes nds = fl.HisNodes;
                foreach (Node nd in nds)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + workID + " OR FID=" + workID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }
                }
                if (msg != "")
                {
                    BP.DA.Log.DebugWriteInfo(msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程 Err " + ex.TargetSite;
                BP.DA.Log.DebugWriteError(err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.

            BP.DA.Log.DebugWriteInfo("@[" + fl.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + workID + "]。");
            return "已经完成的流程被您删除成功.";
        }
        /// <summary>
        /// 执行驳回
        /// 应用场景:子流程向分合点驳回时
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="fk_node">被驳回的节点</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string DoReject(Int64 fid, int fk_node, string msg)
        {
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, fk_node);

            //if (i == 0)
            //    throw new Exception("系统错误，没有找到应该找到的数据。");

            i = wl.Delete();
            //if (i == 0)
            //    throw new Exception("系统错误，没有删除应该删除的数据。");

            wl = new GenerWorkerList();
            i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.IsPass, 3);

            //if (i == 0)
            //    throw new Exception("系统错误，想找到退回的原始起点没有找到。");

            Node nd = new Node(fk_node);
            // 更新当前流程管理表的设置当前的节点。
            DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET FK_Node=" + fk_node + ", NodeName='" + nd.Name + "' WHERE WorkID=" + this.WorkID);

            wl.ItIsPass = false;
            wl.Update();

            return "工作已经驳回到(" + wl.EmpNo + " , " + wl.EmpName + ")";
            // wl.HisNode
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns>返回删除结果.</returns>
        private string DoDeleteSubThread()
        {
            WorkNode wn = this.GetCurrentWorkNode();
            Emp empOfWorker = new Emp(WebUser.No);

            #region 正常的删除信息.
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除质量考核信息.
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE WorkID=" + this.WorkID); // 删除质量考核数据。

                // 删除抄送信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");

                if (msg != "")
                    BP.DA.Log.DebugWriteInfo(msg);
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisGenerWorkFlow.WorkID + "," + this.HisGenerWorkFlow.Title + "] Err " + ex.Message;
                BP.DA.Log.DebugWriteError(err);
                throw new Exception(err);
            }
            string info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (1 == 2)
            {
                /* 目前还没有必要，因为在分流点,才有计算完成率的需求. */
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */
                sql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /*说明全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0  WHERE IsPass=3  AND WorkID=" + this.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.NodeID);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            ///* 解决删除最后一个子流程时要把干流程也要删除。*/
                            //sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" +this.HisGenerWorkFlow +" AND FID=" + this.FID;
                            //int num = DBAccess.RunSQLReturnValInt(sql);
                            //if (num == 0)
                            //{
                            //    /*说明没有子进程，就要把这个流程执行完成。*/
                            //    WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            //    info += "@所有的子线程已经结束。";
                            //    info += "@结束主流程信息。";
                            //    info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束");
                            //}
                            break;
                    }
                }
            }
            #endregion

            #region 写入删除日志.
            wn.AddToTrack(ActionType.DeleteSubThread, empOfWorker.UserID, empOfWorker.Name,
             wn.HisNode.NodeID,
             wn.HisNode.Name, "子线程被:" + BP.Web.WebUser.Name + "删除.");
            #endregion 写入删除日志.

            return "子线程被删除成功.";
        }
        /// <summary>
        /// 删除已经完成的流程
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="isDelSubFlow">是否删除子流程</param>
        /// <returns>删除错误会抛出异常</returns>
        public static void DeleteFlowByReal(Int64 workid, bool isDelSubFlow)
        {
            //检查流程是否完成，如果没有完成就调用workflow流程删除.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            int i = gwf.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("err@错误：该流程应不存在");

            BP.WF.Flow fl = new Flow(gwf.FlowNo);
            string toEmps = gwf.Emps.Replace('@', ',');//流程的所有处理人
            if (i != 0)
            {
                if (gwf.WFState != WFState.Complete)
                {
                    WorkFlow wf = new WorkFlow(workid);
                    //发送退回消息 
                    PushMsgs pms1 = new PushMsgs();
                    pms1.Retrieve(PushMsgAttr.FK_Node, gwf.NodeID, PushMsgAttr.FK_Event, EventListFlow.AfterFlowDel);
                    Node node = new Node(gwf.NodeID);
                    foreach (PushMsg pm in pms1)
                    {
                        Work work = node.HisWork;
                        work.OID = gwf.WorkID;
                        work.NodeID = node.NodeID;
                        work.SetValByKey("FK_Dept", BP.Web.WebUser.DeptNo);
                        pm.DoSendMessage(node, work, null, null, null, toEmps);
                    }

                    wf.DoDeleteWorkFlowByReal(isDelSubFlow);
                    return;
                }
            }

            #region 删除独立表单的数据.
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, gwf.FlowNo);
            string strs = "";
            foreach (FrmNode frmNode in fns)
            {
                if (strs.Contains("@" + frmNode.FK_Frm) == true)
                    continue;

                strs += "@" + frmNode.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(frmNode.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID=" + workid);
                }
                catch
                {

                }
            }
            #endregion 删除独立表单的数据.

            //删除流程数据.
            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(gwf.FlowNo) + "Track WHERE WorkID=" + workid);
            DBAccess.RunSQL("DELETE FROM " + fl.PTable + " WHERE OID=" + workid);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + workid); // 删除质量考核数据。

            #region 正常的删除信息.
            BP.DA.Log.DebugWriteInfo("@[" + fl.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + workid + "]。");
            string msg = "";

            // 删除单据信息.
            DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workid);

            //发送退回消息 
            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, gwf.NodeID, PushMsgAttr.FK_Event, EventListFlow.AfterFlowDel);
            Node pnd = new Node(gwf.NodeID);
            foreach (PushMsg pm in pms)
            {
                Work work = pnd.HisWork;
                work.OID = gwf.WorkID;
                work.NodeID = pnd.NodeID;
                work.SetValByKey("FK_Dept", BP.Web.WebUser.DeptNo);

                pm.DoSendMessage(pnd, work, null, null, null, toEmps);
            }

            //删除它的工作.
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + workid + " OR FID=" + workid + " ) AND FK_Flow='" + gwf.FlowNo + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + workid + " OR FID=" + workid + " )");

            //删除所有节点上的数据.
            Nodes nodes = new Nodes(gwf.FlowNo); // this.HisFlow.HisNodes;
            foreach (Node node in nodes)
            {
                try
                {
                    if (DBAccess.IsExitsObject("ND" + node.NodeID) == false)
                        continue;

                    DBAccess.RunSQL("DELETE FROM ND" + node.NodeID + " WHERE OID=" + workid + " OR FID=" + workid);
                }
                catch (Exception ex)
                {
                    msg += "@ delete data error " + ex.Message;
                }

                MapDtls dtls = new MapDtls("ND" + node.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                    }
                    catch
                    {
                    }
                }
            }

            MapDtls mydtls = new MapDtls("ND" + int.Parse(gwf.FlowNo) + "Rpt");
            foreach (MapDtl dtl in mydtls)
            {
                try
                {
                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                }
                catch
                {
                }
            }

            if (msg != "")
            {
                BP.DA.Log.DebugWriteInfo(msg);
            }

            #endregion 正常的删除信息.
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns>删除的消息</returns>
        public string DoDeleteSubThread2015()
        {
            if (this.FID == 0)
                throw new Exception("@该流程非子线程流程实例，不能执行该方法。");

            #region 正常的删除信息.
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除质量考核信息.
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE WorkID=" + this.WorkID); // 删除质量考核数据。

                // 删除抄送信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID);

                if (msg != "")
                    BP.DA.Log.DebugWriteInfo(msg);
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisGenerWorkFlow.WorkID + "," + this.HisGenerWorkFlow.Title + "] Err " + ex.Message;
                BP.DA.Log.DebugWriteError(err);
                throw new Exception(err);
            }
            string info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (1 == 2)
            {
                /*
                 * 开发说明：
                 * 1，当前是删除子线程操作,当前的节点就是子线程节点.
                 * 2, 删除子线程的动作，1，合流点。2，分流点。
                 * 3，这里要解决合流节点的完成率的问题.
                 */

#warning 应该删除一个子线程后，就需要计算完成率的问题。但是目前应用到该场景极少,因为。能够看到河流点信息，说明已经到达了完成率了。

                /* 目前还没有必要，因为在分流点,才有计算完成率的需求. */
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */

                sql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + this.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /* 说明: 全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0  WHERE IsPass=3  AND WorkID=" + this.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.NodeID);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            ///* 解决删除最后一个子流程时要把干流程也要删除。*/
                            //sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" +this.HisGenerWorkFlow +" AND FID=" + this.FID;
                            //int num = DBAccess.RunSQLReturnValInt(sql);
                            //if (num == 0)
                            //{
                            //    /*说明没有子进程，就要把这个流程执行完成。*/
                            //    WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            //    info += "@所有的子线程已经结束。";
                            //    info += "@结束主流程信息。";
                            //    info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束");
                            //}
                            break;
                    }
                }
            }
            #endregion



            //检查是否是最后一个子线程被删除了？如果是，就需要当分流节点产生待办.
            GenerWorkFlow gwfMain = new GenerWorkFlow(this.FID);

            /*说明仅仅停留在分流节点,还没有到合流节点上去.
             * 删除子线程的时候，判断是否是最后一个子线程,如果是，就要把他设置为待办状态。
             * 1.首先要找到.
             * 2.xxxx.
             */
            //  string sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Node=";
            string mysql = "SELECT COUNT(*)  as Num FROM WF_GenerWorkerlist WHERE IsPass=0 AND FID=" + this.FID;
            int num = DBAccess.RunSQLReturnValInt(mysql);
            if (num == 0)
            {
                /* 说明当前主流程上是分流节点，但是已经没有子线程的待办了。
                 * 就是说，删除子流程的时候，删除到最后已经没有活动或者已经完成的子线程了.
                 * */

                GenerWorkerList gwl = new GenerWorkerList();
                int i = gwl.Retrieve(GenerWorkerListAttr.FK_Node, gwfMain.NodeID, GenerWorkerListAttr.WorkID, gwfMain.WorkID,
                    GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
                if (i == 0)
                {
                    Node ndMain = new Node(gwfMain.NodeID);
                    if (ndMain.ItIsHL == true)
                    {
                        /* 有可能是当前节点已经到了合流节点上去了, 要判断合流节点是否有代办？如果没有代办，就撤销到分流节点上去.
                         * 
                         * 就要检查他是否有代办.
                         */
                        mysql = "SELECT COUNT(*)  as Num FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Node=" + gwfMain.NodeID;
                        num = DBAccess.RunSQLReturnValInt(mysql);
                        if (num == 0)
                        {
                            /*如果没有待办，就说明，当前节点已经运行到合流节点，但是不符合合流节点的完成率，导致合流节点上的人员看不到待办. 
                             * 这种情况，就需要让当前分流节点产生待办.
                             */

                            mysql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE FID=0 AND WorkID=" + gwfMain.WorkID + " ORDER BY RDT DESC ";
                            int fenLiuNodeID = DBAccess.RunSQLReturnValInt(mysql);

                            Node nd = new Node(fenLiuNodeID);
                            if (nd.ItIsFL == false)
                                throw new Exception("@程序错误，没有找到最近的一个分流节点.");

                            GenerWorkerLists gwls = new GenerWorkerLists();
                            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, fenLiuNodeID);
                            foreach (GenerWorkerList item in gwls)
                            {
                                item.ItIsRead = false;
                                item.PassInt = 0;
                                item.SDT = DataType.CurrentDateTimess;
                                item.Update();
                            }
                        }
                    }
                }
                else
                {
                    gwl.ItIsRead = false;
                    gwl.PassInt = 0;
                    gwl.SDT = DataType.CurrentDateTimess;
                    gwl.Update();
                    return "子线程被删除成功,这是最后一个删除的子线程已经为您在{" + gwfMain.NodeName + "}产生了待办,<a href='/WF/MyFlow.htm?WorkID=" + gwfMain.WorkID + "&FK_Flow=" + gwfMain.FlowNo + "'>点击处理工作</a>.";

                }
            }
            return "子线程被删除成功.";
        }

        /// <summary>
        /// 彻底的删除流程
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns>删除的消息</returns>
        public string DoDeleteWorkFlowByReal(bool isDelSubFlow)
        {
            if (this.FID != 0)
                return DoDeleteSubThread2015();

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
                return "删除成功.";

            string info = "";
            WorkNode wn = this.GetCurrentWorkNode();

            // 处理删除前事件。
            ExecEvent.DoFlow(EventListFlow.BeforeFlowDel, wn, null);

            #region 删除独立表单的数据.
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.HisFlow.No);
            string strs = "";
            foreach (FrmNode nd in fns)
            {
                if (strs.Contains("@" + nd.FK_Frm) == true)
                    continue;

                strs += "@" + nd.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(nd.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID=" + this.WorkID);
                }
                catch
                {
                }
            }
            #endregion 删除独立表单的数据.

            //删除流程数据.
            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisFlow.No) + "Track WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM " + this.HisFlow.PTable + " WHERE OID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + this.WorkID); // 删除质量考核数据。

            #region 正常的删除信息.
            BP.DA.Log.DebugWriteInfo("@[" + this.HisFlow.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + this.WorkID + "]。");
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");

                //删除所有节点上的数据.
                Nodes nds = this.HisFlow.HisNodes;
                foreach (Node nd in nds)
                {
                    MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                    foreach (MapDtl dtl in dtls)
                    {
                        try
                        {
                            DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPk = " + this.WorkID);
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        if (DBAccess.IsExitsObject("ND" + nd.NodeID) == false)
                            continue;

                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + this.WorkID + " OR FID=" + this.WorkID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }


                }
                if (msg != "")
                {
                    BP.DA.Log.DebugWriteInfo(msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisGenerWorkFlow.WorkID + "," + this.HisGenerWorkFlow.Title + "] Err " + ex.Message;
                BP.DA.Log.DebugWriteError(err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (this.FID != 0)
            {
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */
                sql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + wn.HisWork.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /*说明全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0  WHERE IsPass=3  AND WorkID=" + wn.HisWork.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.NodeID);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            /* 解决删除最后一个子流程时要把干流程也要删除。*/
                            sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE FK_Node=" + wn.HisNode.NodeID + " AND FID=" + wn.HisWork.FID;
                            int num = DBAccess.RunSQLReturnValInt(sql);
                            if (num == 0)
                            {
                                /*说明没有子进程，就要把这个流程执行完成。*/
                                WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                                info += "@所有的子线程已经结束。";
                                info += "@结束主流程信息。";
                                info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束", null, null);
                            }
                            break;
                    }
                }
            }
            #endregion

            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.

            // 处理删除hou事件。
            ExecEvent.DoFlow(EventListFlow.AfterFlowDel, wn, null);

            return info;
        }

        /// <summary>
        /// 删除工作流程记录日志，并保留运动轨迹.
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns></returns>
        public string DoDeleteWorkFlowByWriteLog(string info, bool isDelSubFlow)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(this.HisFlow.No) + "Rpt", this.WorkID);
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = this.WorkID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDateTime;
                log.OperDept = WebUser.DeptNo;
                log.OperDeptName = WebUser.DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = info;
                log.OID = this.WorkID;
                log.FlowNo = this.HisFlow.No;
                log.InsertAsOID(log.OID);
                return DoDeleteWorkFlowByReal(isDelSubFlow);
            }
            catch (Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                throw new Exception(ex.StackTrace);
            }
        }

        #region 流程的强制终止\删除 或者恢复使用流程,
        /// <summary>
        /// 恢复流程.
        /// </summary>
        /// <param name="msg">回复流程的原因</param>
        public void DoComeBackWorkFlow(string msg)
        {
            try
            {
                //设置产生的工作流程为
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = WFState.Runing;
                gwf.DirectUpdate();

                // 增加消息 
                WorkNode wn = this.GetCurrentWorkNode();
                GenerWorkerLists wls = new GenerWorkerLists(wn.HisWork.OID, wn.HisNode.NodeID);
                if (wls.Count == 0)
                    throw new Exception("@恢复流程出现错误,产生的工作者列表");

                foreach (GenerWorkerList item in wls)
                    BP.WF.Dev2Interface.Port_SendMsg(item.EmpNo, "流程恢复通知:" + gwf.Title, "该流程[" + gwf.Title + "]，请打开待办处理.", "rback");
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError("@恢复流程出现错误." + ex.Message);
                throw new Exception("@恢复流程出现错误." + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 得到当前的进行中的工作。
        /// </summary>
        /// <returns></returns>		 
        public WorkNode GetCurrentWorkNode()
        {
            int currNodeID = 0;
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                this.DoFlowOver(ActionType.FlowOver, "非正常结束，没有找到当前的流程记录。", null, null);
                throw new Exception("@" + string.Format("工作流程{0}已经完成。", this.HisGenerWorkFlow.Title));
            }

            Node nd = new Node(gwf.NodeID);
            Work work = nd.HisWork;
            work.OID = this.WorkID;
            work.NodeID = nd.NodeID;
            work.SetValByKey("FK_Dept", BP.Web.WebUser.DeptNo);
            if (work.RetrieveFromDBSources() == 0)
            {
                BP.DA.Log.DebugWriteError("@WorkID=" + this.WorkID + ",FK_Node=" + gwf.NodeID + ".不应该出现查询不出来工作."); // 没有找到当前的工作节点的数据，流程出现未知的异常。
                work.Rec = BP.Web.WebUser.No;
                try
                {
                    work.Insert();
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError("@没有找到当前的工作节点的数据，流程出现未知的异常" + ex.Message + ",不应该出现"); // 没有找到当前的工作节点的数据
                }
            }
            work.FID = gwf.FID;

            WorkNode wn = new WorkNode(work, nd);
            return wn;
        }
        /// <summary>
        /// 结束分流的节点
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string DoFlowOverFeiLiu(GenerWorkFlow gwf)
        {
            // 查询出来有少没有完成的流程。
            int i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE FID=" + gwf.FID + " AND WFState!=1");
            switch (i)
            {
                case 0:
                    throw new Exception("@不应该的错误。");
                case 1:
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow  WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);

                    Work wk = this.HisFlow.HisStartNode.HisWork;
                    wk.OID = gwf.FID;
                    wk.Update();

                    return "@当前的工作已经完成，该流程上所有的工作都已经完成。";
                default:
                    DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=1 WHERE WorkID=" + this.WorkID);
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + this.WorkID);
                    return "@当前的工作已经完成。";
            }
        }
        /// <summary>
        /// 处理子线程完成.
        /// </summary>
        /// <returns></returns>
        public string DoFlowThreadOver()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Node nd = new Node(gwf.NodeID);

            //DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow   WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID);

            string sql = "SELECT count(*) FROM WF_GenerWorkerlist WHERE  FID=" + this.FID;
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
            {
                /*说明这是最后一个*/
                WorkFlow wf = new WorkFlow(this.FID);
                wf.DoFlowOver(ActionType.FlowOver, "子线程结束", null, null);
                return "@当前子线程已完成，干流程已完成。";
            }
            else
            {
                return "@当前子线程已完成，干流程还有(" + num + ")个子线程未完成。";
            }
        }


        /// <summary>
        /// 执行流程完成
        /// </summary>
        /// <param name="at"></param>
        /// <param name="stopMsg"></param>
        /// <param name="currNode"></param>
        /// <param name="rpt"></param>
        /// <param name="stopFlowType">结束类型:自定义参数</param>
        /// <param name="empNo"></param>
        /// <param name="empName"></param>
        /// <returns></returns>
        public string DoFlowOver(ActionType at, string stopMsg, Node currNode, GERpt rpt, int stopFlowType = 1, string empNo = "", string empName = "")
        {
            if (null == currNode)
                return "err@当前节点为空..";

            if (DataType.IsNullOrEmpty(stopMsg))
                stopMsg += "流程结束";

            //获得当前的节点.
            WorkNode wn = this.GetCurrentWorkNode();
            wn.rptGe = rpt;

            //调用结束前事件.
            string mymsg = ExecEvent.DoFlow(EventListFlow.FlowOverBefore, wn, null);
            //string mymsg = this.HisFlow.DoFlowEventEntity(EventListFlow.FlowOverBefore, currNode, rpt, null);
            if (mymsg != null)
                stopMsg += "@" + mymsg;

            string exp = currNode.FocusField;
            if (DataType.IsNullOrEmpty(exp) == false && exp.Length > 1)
            {
                if (rpt != null)
                    stopMsg += Glo.DealExp(exp, rpt, null);
            }

            //IsMainFlow== false 这个位置是子线程
            if (this.ItIsMainFlow == false)
            {
                /* 处理子线程完成*/
                stopMsg += this.DoFlowThreadOver();
            }

            #region 处理明细表的汇总.
            this._IsComplete = 1;
            #endregion 处理明细表的汇总.

            #region 处理后续的业务.

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (1 == 2)
            {
                // 是否删除流程注册表的数据？
                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
                ps.Add("WorkID1", this.WorkID);
                ps.Add("WorkID2", this.WorkID);
                DBAccess.RunSQL(ps);
            }


            // 删除子线程产生的 流程注册信息.
            if (this.FID == 0)
            {
                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbstr + "WorkID";
                ps.Add("WorkID", this.WorkID);
                DBAccess.RunSQL(ps);
            }

            // 清除工作者.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            //把当前的人员字符串加入到参与人里面去,以方便查询.
            string emps = WebUser.No + "," + WebUser.Name + "@";

            // 设置流程完成状态.
            ps = new Paras();
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6 || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET  FlowEmps= FlowEmps ||'" + emps + "', WFState=:WFState,WFSta=:WFSta WHERE OID=" + dbstr + "OID";
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET FlowEmps= CONCAT(FlowEmps ,'" + emps + "'), WFState=" + dbstr + "WFState,WFSta=" + dbstr + "WFSta WHERE OID=" + dbstr + "OID";
            else
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET FlowEmps= FlowEmps + '" + emps + "', WFState=" + dbstr + "WFState,WFSta=" + dbstr + "WFSta WHERE OID=" + dbstr + "OID";

            ps.Add("WFState", (int)WFState.Complete);
            ps.Add("WFSta", (int)WFSta.Complete);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            //加入轨迹.
            if (DataType.IsNullOrEmpty(empNo) == true)
            {
                empNo = WebUser.No;
                empName = WebUser.Name;
            }
            wn.AddToTrack(at, empNo, empName, wn.HisNode.NodeID, wn.HisNode.Name, stopMsg);

            //执行流程结束.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            //增加参与的人员
            if (gwf.Emps.Contains("@" + WebUser.No + ",") == false)
                gwf.Emps += "@" + WebUser.No + "," + WebUser.Name;

            gwf.WFState = WFState.Complete;
            gwf.SetPara("StopFlowType", stopFlowType); //结束流程类型.
            gwf.Update();

            //生成关键字.
            this.GenerSKeyWords(gwf, wn.rptGe);
            //流程发送成功事件
            string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, wn);
               
            //调用结束后事件.
            string result = ExecEvent.DoFlow(EventListFlow.FlowOverAfter, wn, null);
            if (result != null)
                stopMsg += result;
           #endregion 处理后续的业务.

                //执行最后一个子流程发送后的检查，不管是否成功，都要结束该流程。
            stopMsg += WorkNodePlus.SubFlowEvent(wn);

            //string dbstr =  BP.Difference.SystemConfig.AppCenterDBVarStr;

            #region 处理审核问题,更新审核组件插入的审核意见中的 到节点，到人员。
            ps = new Paras();
            ps.SQL = "UPDATE ND" + int.Parse(currNode.FlowNo) + "Track SET NDTo=" + dbstr + "NDTo,NDToT=" + dbstr + "NDToT,EmpTo=" + dbstr + "EmpTo,EmpToT=" + dbstr + "EmpToT WHERE NDFrom=" + dbstr + "NDFrom AND EmpFrom=" + dbstr + "EmpFrom AND WorkID=" + dbstr + "WorkID AND ActionType=" + (int)ActionType.WorkCheck;
            ps.Add(TrackAttr.NDTo, currNode.NodeID);
            ps.Add(TrackAttr.NDToT, "");
            ps.Add(TrackAttr.EmpTo, "");
            ps.Add(TrackAttr.EmpToT, "");

            ps.Add(TrackAttr.NDFrom, currNode.NodeID);
            ps.Add(TrackAttr.EmpFrom, WebUser.No);
            ps.Add(TrackAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);
            #endregion 处理审核问题.

            //如果存在 BillState列，执行更新, 让其可见.
            if (rpt.EnMap.Attrs.Contains("BillState") == true)
            {
                rpt.SetValByKey("BillState", 100);
            }
            else
            {
                string ptable = "ND" + int.Parse(gwf.FlowNo) + "Rpt";
                if (rpt.EnMap.PhysicsTable.Equals(ptable) == false && DBAccess.IsExitsTableCol(rpt.EnMap.PhysicsTable, "BillState") == true)
                    DBAccess.RunSQL("UPDATE " + rpt.EnMap.PhysicsTable + " SET BillState=100 WHERE OID=" + this.WorkID);
            }
            if (sendSuccess != null)
                return "@"+sendSuccess+stopMsg;

            return stopMsg;
        }
        /// <summary>
        /// 归档关键字查询
        /// </summary>
        /// <param name="gwf"></param>
        /// <param name="en"></param>
        public void GenerSKeyWords(GenerWorkFlow gwf, Entity en)
        {
            //获取WF_GenerWorkFlow的关键字.
            string keyworkd = gwf.Title + gwf.TodoEmps + "," + gwf.FlowName;
            foreach (Attr item in en.EnMap.Attrs)
            {
                if (item.UIContralType == UIContralType.DDL)
                    continue;

                if (item.MyDataType == DataType.AppString && item.MaxLength <= 100)
                {
                    keyworkd += en.GetValStrByKey(item.Key)+",";
                    continue;
                }
                keyworkd += en.GetValStrByKey(item.Key)+",";
            }

            if (DBAccess.IsExitsTableCol("WF_GenerWorkFlow", "SKeyWords") == false)
                return;

            Paras pa = new Paras();
            pa.SQL = "UPDATE WF_GenerWorkFlow SET SKeyWords=" + SystemConfig.AppCenterDBVarStr + "SKeyWords WHERE WorkID=" + this.WorkID;
            pa.Add("SKeyWords", keyworkd);
            DBAccess.RunSQL(pa);
        }
        public string GenerFHStartWorkInfo()
        {
            string msg = "";
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT Title,RDT,Rec,OID FROM ND" + this.StartNodeID + " WHERE FID=" + this.FID);
            switch (dt.Rows.Count)
            {
                case 0:
                    Node nd = new Node(this.StartNodeID);
                    throw new Exception("@没有找到他们开始节点的数据，流程异常。FID=" + this.FID + "，节点：" + nd.Name + "节点ID：" + nd.NodeID);
                case 1:
                    msg = string.Format("@发起人： {0}  日期：{1} 发起的流程 标题：{2} ，已经成功完成。",
                        dt.Rows[0]["Rec"].ToString(), dt.Rows[0]["RDT"].ToString(), dt.Rows[0]["Title"].ToString());
                    break;
                default:
                    msg = "@下列(" + dt.Rows.Count + ")位人员发起的流程已经完成。";
                    foreach (DataRow dr in dt.Rows)
                    {
                        msg += "<br>发起人：" + dr["Rec"] + " 发起日期：" + dr["RDT"] + " 标题：" + dr["Title"] + "<a href='./../../WF/WFRpt.htm?WorkID=" + dr["OID"] + "&FK_Flow=" + this.HisFlow.No + "' target=_blank>详细...</a>";
                    }
                    break;
            }
            return msg;
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.HisFlow.No + "01");
            }
        }

        /// <summary>
        /// 执行冻结
        /// </summary>
        /// <param name="msg">冻结原因</param>
        public string DoFix(string fixMsg)
        {
            if (this.HisGenerWorkFlow.WFState == WFState.Fix)
                throw new Exception("@当前已经是冻结的状态您不能执行再冻结.");

            if (DataType.IsNullOrEmpty(fixMsg))
                fixMsg = "无";


            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            string emps = "";

            foreach (GenerWorkerList wl in wls)
            {
                if (wl.ItIsEnable == false)
                    continue; //不发送给禁用的人。
                emps += wl.EmpNo + "," + wl.EmpName + ";";
                //写入消息。
                BP.WF.Dev2Interface.Port_SendMsg(wl.EmpNo, this.HisGenerWorkFlow.Title, fixMsg, "Fix" + wl.WorkID, "Fix", wl.FlowNo, wl.NodeID, wl.WorkID, wl.FID);
            }

            /* 执行 WF_GenerWorkFlow 冻结. */
            int sta = (int)WFState.Fix;
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=" + dbstr + "IsPass WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + this.HisGenerWorkFlow.NodeID;
            ps.Add(GenerWorkerListAttr.IsPass, 9);
            ps.Add(GenerWorkerListAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);


            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            //WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            //wn.AddToTrack(ActionType.Info, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, fixMsg,);

            return this.WorkID + "-" + this.HisFlow.Name + "已经成功执行冻结";
        }
        /// <summary>
        /// 执行解除冻结
        /// </summary>
        /// <param name="msg">冻结原因</param>
        public string DoUnFix(string unFixMsg)
        {
            if (this.HisGenerWorkFlow.WFState != WFState.Fix)
                throw new Exception("@当前非冻结的状态您不能执行解除冻结.");

            if (DataType.IsNullOrEmpty(unFixMsg))
                unFixMsg = "无";


            ///* 获取它的工作者，向他们发送消息。*/
            //GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);

            //string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.NodeID;
            //string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            //string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "冻结" + unFixMsg;
            //string emps = "";
            //foreach (GenerWorkerList wl in wls)
            //{
            //    if (wl.ItIsEnable == false)
            //        continue; //不发送给禁用的人。

            //    emps += wl.EmpNo + "," + wl.EmpName + ";";

            //    //写入消息。
            //    BP.WF.Dev2Interface.Port_SendMsg(wl.EmpNo, title, mailDoc, "Fix" + wl.WorkID, BP.Sys.SMSMsgType.Self, wl.FK_Flow, wl.NodeID, wl.WorkID, wl.FID);
            //}

            /* 执行 WF_GenerWorkFlow 冻结. */
            int sta = (int)WFState.Runing;
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            //wn.AddToTrack(ActionType.Info, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, unFixMsg);

            return "已经成功执行解除冻结:";
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 他的节点
        /// </summary>
        private Nodes _HisNodes = null;
        /// <summary>
        /// 节点s
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                if (this._HisNodes == null)
                    this._HisNodes = this.HisFlow.HisNodes;
                return this._HisNodes;
            }
        }
        /// <summary>
        /// 工作节点s(普通的工作节点)
        /// </summary>
        private WorkNodes _HisWorkNodesOfWorkID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfWorkID
        {
            get
            {
                if (this._HisWorkNodesOfWorkID == null)
                {
                    this._HisWorkNodesOfWorkID = new WorkNodes();
                    this._HisWorkNodesOfWorkID.GenerByWorkID(this.HisFlow, this.WorkID);
                }
                return this._HisWorkNodesOfWorkID;
            }
        }
        /// <summary>
        /// 工作节点s
        /// </summary>
        private WorkNodes _HisWorkNodesOfFID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfFID
        {
            get
            {
                if (this._HisWorkNodesOfFID == null)
                {
                    this._HisWorkNodesOfFID = new WorkNodes();
                    this._HisWorkNodesOfFID.GenerByFID(this.HisFlow, this.FID);
                }
                return this._HisWorkNodesOfFID;
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        private Flow _HisFlow = null;
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                return this._HisFlow;
            }
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
            set
            {
                _HisGenerWorkFlow = value;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _WorkID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this._WorkID;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _FID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this._FID;
            }
            set
            {
                this._FID = value;
            }
        }
        /// <summary>
        /// 是否是干流
        /// </summary>
        public bool ItIsMainFlow
        {
            get
            {
                if (this.FID != 0 && this.FID != this.WorkID)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        #region 构造方法
        public WorkFlow(Int64 wkid)
        {
            this.HisGenerWorkFlow = new GenerWorkFlow();
            this.HisGenerWorkFlow.RetrieveByAttr(GenerWorkerListAttr.WorkID, wkid);
            this._FID = this.HisGenerWorkFlow.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");

            Flow flow = new Flow(this.HisGenerWorkFlow.FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;

        }

        public WorkFlow(Flow flow, Int64 wkid)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wkid;
            gwf.RetrieveFromDBSources();

            this._FID = gwf.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        /// <summary>
        /// 建立一个工作流事例
        /// </summary>
        /// <param name="flow">流程No</param>
        /// <param name="wkid">工作ID</param>
        public WorkFlow(Flow flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        public WorkFlow(string FK_flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;

            Flow flow = new Flow(FK_flow);
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        #endregion

        #region 运算属性
        public int _IsComplete = -1;
        /// <summary>
        /// 是不是完成
        /// </summary>
        public bool ItIsComplete
        {
            get
            {

                //  bool s = !DBAccess.IsExits("select workid from WF_GenerWorkFlow WHERE WorkID=" + this.WorkID + " AND FK_Flow='" + this.HisFlow.No + "'");

                GenerWorkFlow generWorkFlow = new GenerWorkFlow(this.WorkID);
                if (generWorkFlow.WFState == WFState.Complete)
                    return true;
                else
                    return false;

            }
        }
        /// <summary>
        /// 是不是完成
        /// </summary>
        public string IsCompleteStr
        {
            get
            {
                if (this.ItIsComplete)
                    return "已";
                else
                    return "未";
            }
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="empId">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpStation(int nodeId, string empId)
        {
            bool isCan = false;
            // 判断角色对应关系是不是能够执行.
            string sql = "SELECT a.FK_Node FROM WF_NodeStation a,  Port_DeptEmpStation b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " AND b.FK_Emp='" + empId + "' )";
            isCan = DBAccess.IsExits(sql);
            if (isCan)
                return true;
            // 判断他的主要工作角色能不能执行它.
            sql = "select FK_Node from WF_NodeStation WHERE FK_Node=" + nodeId + " AND ( FK_Station in (select FK_Station from Port_DeptEmpStation WHERE FK_Emp='" + empId + "') ) ";
            return DBAccess.IsExits(sql);
        }
        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="dutyNo">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpDuty(int nodeId, string dutyNo)
        {
            string sql = "SELECT a.FK_Node FROM WF_NodeDuty  a,  Port_EmpDuty b WHERE (a.FK_Duty=b.FK_Duty) AND (a.FK_Node=" + nodeId + " AND b.FK_Duty=" + dutyNo + ")";
            if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 在物理上能构作这项工作的人员。
        /// </summary>
        /// <param name="nodeId">节点ID</param>		 
        /// <returns></returns>
        public static DataTable CanDoWorkEmps(int nodeId)
        {
            string sql = "select a.FK_Node, b.EmpID from WF_NodeStation  a,  Port_DeptEmpStation b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " )";
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// GetEmpsBy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Emps GetEmpsBy(DataTable dt)
        {
            // 形成能够处理这件事情的用户几何。
            Emps emps = new Emps();
            foreach (DataRow dr in dt.Rows)
            {
                emps.AddEntity(new Emp(dr["EmpID"].ToString()));
            }
            return emps;
        }

        #endregion

        #region 流程方法

        private string _AppType = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string AppType
        {
            get
            {
                if (_AppType == null)
                {
                    if (BP.Difference.SystemConfig.isBSsystem == false)
                    {
                        _AppType = "WF";
                    }
                    else
                    {


                        _AppType = "WF";

                    }
                }
                return _AppType;
            }
        }
        private string _VirPath = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string VirPath
        {
            get
            {
                if (_VirPath == null)
                {
                    if (BP.Difference.SystemConfig.isBSsystem)
                        _VirPath = HttpContextHelper.RequestApplicationPath; // _VirPath = BP.Sys.Base.Glo.Request.ApplicationPath;
                    else
                        _VirPath = "";
                }
                return _VirPath;
            }
        }
        /// <summary>
        /// 撤销挂起
        /// </summary>
        /// <returns></returns>
        public string DoHungupWork_Un()
        {

            string checker = this.HisGenerWorkFlow.GetParaString("HungupChecker");

            //删除领导审核的数据.
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FK_Node=" + this.HisGenerWorkFlow.NodeID + " AND FK_Emp='" + checker + "' AND WorkID=" + this.HisGenerWorkFlow.WorkID);

            this.HisGenerWorkFlow.HungupTime = DataType.CurrentDateTime;
            this.HisGenerWorkFlow.WFState = WFState.Runing;
            this.HisGenerWorkFlow.Update();

            string ptable = this.HisFlow.PTable;




            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            wn.AddToTrack(ActionType.UnHungup, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "撤销挂起");


            return "成功撤销.";
        }
        /// <summary>
        /// 执行挂起
        /// </summary>
        /// <param name="way">挂起方式</param>
        /// <param name="relData">释放日期</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns></returns>
        public string DoHungup(HungupWay way, string relData, string hungNote)
        {
            if (this.HisGenerWorkFlow.WFState == WFState.Hungup)
                return "err@当前已经是挂起的状态您不能执行在挂起.";

            if (DataType.IsNullOrEmpty(hungNote) == true)
                hungNote = "无";

            if (way == HungupWay.SpecDataRel)
            {
                try
                {
                    DateTime d = DataType.ParseSysDate2DateTime(relData);
                }
                catch (Exception ex)
                {
                    throw new Exception("err@解除挂起的日期[" + relData + "]不正确" + ex.Message);
                }
            }
            if (relData == null)
                relData = "";

            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            //string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "挂起" + hungNote;
            string emps = "";

            GenerWorkerList gwl = null;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.ItIsEnable == false)
                    continue; //不发送给禁用的人。

                //BP.WF.Port.WFEmp emp = new BP.Port.WFEmp(wl.EmpNo);
                emps += wl.EmpNo + "," + wl.EmpName + ";";

                gwl = wl;

                //写入消息。
                BP.WF.Dev2Interface.Port_SendMsg(wl.EmpNo, title, title, "Hungup" + wl.WorkID, BP.WF.SMSMsgType.Hungup, wl.FlowNo, wl.NodeID, wl.WorkID, wl.FID);
            }

            /* 执行 WF_GenerWorkFlow 挂起. */
            int hungSta = (int)WFState.Hungup;
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;


            //发送人.
            string[] sender = this.HisGenerWorkFlow.Sender.Split(',');
            gwl.EmpNo = sender[0];
            gwl.EmpName= sender[1];
            gwl.WorkID = this.HisGenerWorkFlow.WorkID;
            gwl.NodeID = this.HisGenerWorkFlow.NodeID;
            gwl.PassInt = 0;
            gwl.ItIsRead = false;
            gwl.SDT = "2030-01-01 10:00";
            gwl.RDT = DataType.CurrentDateTimess;
            //   gwl.FID = this.HisGenerWorkFlow.FID;
            gwl.Save();

            //更新挂起状态.
            this.HisGenerWorkFlow.WFState = WFState.Hungup;
            this.HisGenerWorkFlow.SetPara("Hunguper", BP.Web.WebUser.No);
            this.HisGenerWorkFlow.SetPara("HunguperName", BP.Web.WebUser.Name);
            this.HisGenerWorkFlow.SetPara("HungupWay", (int)way);
            this.HisGenerWorkFlow.SetPara("HungupRelDate", relData);
            this.HisGenerWorkFlow.SetPara("HungupNote", hungNote);
            this.HisGenerWorkFlow.SetPara("HungupSta", (int)HungupSta.Apply); //设置申请状态.
            this.HisGenerWorkFlow.SetPara("HungupChecker", gwl.EmpNo); //要审批的人.

            this.HisGenerWorkFlow.HungupTime = DataType.CurrentDateTime;
            this.HisGenerWorkFlow.Update();

            // 更新流程报表的状态。 
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, hungSta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            wn.AddToTrack(ActionType.Hungup, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, hungNote);

            return "已经成功执行挂起,并且已经通知给:" + emps;
        }
        /// <summary>
        /// 同意挂起
        /// </summary>
        /// <returns></returns>
        public string HungupWorkAgree()
        {
            if (this.HisGenerWorkFlow.WFState != WFState.Hungup)
                throw new Exception("@非挂起状态,您不能解除挂起.");

            this.HisGenerWorkFlow.SetPara("HungupSta", (int)HungupSta.Agree); //同意.
            this.HisGenerWorkFlow.SetPara("HungupChecker", BP.Web.WebUser.No);
            this.HisGenerWorkFlow.SetPara("HungupCheckerName", BP.Web.WebUser.Name);
            this.HisGenerWorkFlow.SetPara("HungupCheckRDT", DataType.CurrentDateTime);
            this.HisGenerWorkFlow.Update();

            //如果是按照指定的日期解除挂起.
            int way = this.HisGenerWorkFlow.GetParaInt("HungupWay");
            if (way == 1)
            {
                string relDT = this.HisGenerWorkFlow.GetParaString("HungupRelDate");
                DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET SDT='" + relDT + "' WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.HisGenerWorkFlow.NodeID);
            }
            //删除当前的待办.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node,
                this.HisGenerWorkFlow.NodeID, GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);

            //更新业务表的状态.
            DBAccess.RunSQL("UPDATE " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Hungup + " WHERE OID=" + this.WorkID);

            return "已经同意挂起.";
        }

        /// <summary>
        /// 新增取消挂起的API
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string CancelHungupWork()
        {
            if (this.HisGenerWorkFlow.WFState != WFState.Hungup)
                throw new Exception("@非挂起状态,您不能取消挂起.");

            /* 执行解除挂起. */
            this.HisGenerWorkFlow.WFState = WFState.Runing; //这里仅仅更新大的状态就好，不在处理AtPara的参数状态.
            this.HisGenerWorkFlow.Update();

            // 更新流程报表的状态。 
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=2 WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET  DTOfUnHungup=" + dbstr + "DTOfUnHungup WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfUnHungup, DataType.CurrentDateTime);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.NodeID);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            wn.AddToTrack(ActionType.UnHungup, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "");
            return "您已经取消挂起，该流程实例已经进入了正常运行状态.";
        }
        /// <summary>
        /// 拒绝挂起 @wwh
        /// </summary>
        /// <returns></returns>
        public string HungupWorkReject(string msg)
        {
            if (this.HisGenerWorkFlow.WFState != WFState.Hungup)
                throw new Exception("@非挂起状态,您不能解除挂起.");

            /* 执行解除挂起. */
            this.HisGenerWorkFlow.WFState = WFState.Hungup;

            this.HisGenerWorkFlow.SetPara("HungupSta", (int)HungupSta.Reject); //不同意.
            this.HisGenerWorkFlow.SetPara("HungupNodeID", this.HisGenerWorkFlow.NodeID); //不同意.
            this.HisGenerWorkFlow.SetPara("HungupCheckMsg", msg); //拒绝原因.
            this.HisGenerWorkFlow.Update();

            // 更新流程报表的状态。 
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=4 WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET  DTOfUnHungup=" + dbstr + "DTOfUnHungup WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfUnHungup, DataType.CurrentDateTime);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.NodeID);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);


            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID);
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.NodeID);

            //string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.NodeID;
            //string mailDoc = "详细信息:<A href='" + url + "'>打开流程</A>.";
            //mailDoc += " 拒绝原因:" + msg;
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "拒绝挂起.";
            string emps = "";
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.ItIsEnable == false)
                    continue; //不发送给禁用的人。

                emps += wl.EmpNo + "," + wl.EmpName + ";";

                //写入消息。
                BP.WF.Dev2Interface.Port_SendMsg(wl.EmpNo, title, msg,
                    "RejectHungup" + wl.NodeID + this.WorkID, BP.WF.SMSMsgType.RejectHungup, HisGenerWorkFlow.FlowNo, HisGenerWorkFlow.NodeID, this.WorkID, this.FID);
            }

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.NodeID);
            wn.AddToTrack(ActionType.UnHungup, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "拒绝挂起，通知给:" + emps);

            //删除当前的待办(当前人是，审核人.)
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node,
                this.HisGenerWorkFlow.NodeID, GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);


            //更新业务表的状态.
            DBAccess.RunSQL("UPDATE " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Runing + " WHERE OID=" + this.WorkID);

            return "您不同意对方挂起，该流程实例已经进入了正常运行状态.";
        }
        #endregion
    }
    /// <summary>
    /// 工作流程集合.
    /// </summary>
    public class WorkFlows : CollectionBase
    {
        #region 构造
        /// <summary>
        /// 工作流程集合
        /// </summary>
        public WorkFlows()
        {
        }
        /// <summary>
        /// 工作流程集合
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="flowState">工作ID</param> 
        public WorkFlows(Flow flow, int flowState)
        {
            //StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            //QueryObject qo = new QueryObject(ens);
            //qo.AddWhere(GERptAttr.WFState, flowState);
            //qo.DoQuery();
            //foreach (StartWork sw in ens)
            //{
            //    this.Add(new WorkFlow(flow, sw.OID, sw.FID));
            //}
        }
        #endregion

        #region 方法
        /// <summary>
        /// 增加一个工作流程
        /// </summary>
        /// <param name="wn">工作流程</param>
        public void Add(WorkFlow wn)
        {
            this.InnerList.Add(wn);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public WorkFlow this[int index]
        {
            get
            {
                return (WorkFlow)this.InnerList[index];
            }
        }
        #endregion

        #region 关于调度的自动方法
        /// <summary>
        /// 清除死节点。
        /// 死节点的产生，就是用户非法的操作，或者系统出现存储故障，造成的流程中的当前工作节点没有工作人员，从而不能正常的运行下去。
        /// 清除死节点，就是把他们放到死节点工作集合里面。
        /// </summary>
        /// <returns></returns>
        public static string ClearBadWorkNode()
        {
            string infoMsg = "清除死节点的信息：";
            string errMsg = "清除死节点的错误信息：";
            return infoMsg + errMsg;
        }
        #endregion
    }
}
