﻿using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.En;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public class GenerWorkFlowAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 工作流
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFSta = "WFSta";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string Starter = "Starter";
        /// <summary>
        /// 产生时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 完成时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 挂起时间
        /// </summary>
        public const string HungupTime = "HungupTime";
        /// <summary>
        /// 得分
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// 当前工作到的节点.
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 当前工作角色
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 发起人名称
        /// </summary>
        public const string StarterName = "StarterName";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 系统类别
        /// </summary>
        public const string SysType = "SysType";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public const string SDTOfFlow = "SDTOfFlow";
        /// <summary>
        /// 流程预警时间
        /// </summary>
        public const string SDTOfFlowWarning = "SDTOfFlowWarning";
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public const string SDTOfNode = "SDTOfNode";
        /// <summary>
        /// 父流程ID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父亲流程的FID
        /// </summary>
        public const string PFID = "PFID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 父流程节点
        /// </summary>
        public const string PNodeID = "PNodeID";
        /// <summary>
        /// 子流程的调用人.
        /// </summary>
        public const string PEmp = "PEmp";
        /// <summary>
        /// 客户编号(对于客户发起的流程有效)
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 客户名称
        /// </summary>
        public const string GuestName = "GuestName";
        /// <summary>
        /// 单据编号
        /// </summary>
        public const string BillNo = "BillNo";
        /// <summary>
        /// 待办人员
        /// </summary>
        public const string TodoEmps = "TodoEmps";
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public const string TodoEmpsNum = "TodoEmpsNum";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// 临时存放的参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 参与人
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 周次
        /// </summary>
        public const string WeekNum = "WeekNum";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// 发送日期
        /// </summary>
        public const string SendDT = "SendDT";
        /// <summary>
        /// 时间范围
        /// </summary>
        public const string TSpan = "TSpan";
        /// <summary>
        /// 待办状态(0=待办中,1=预警中,2=逾期中,3=按期完成,4=逾期完成)
        /// </summary>
        public const string TodoSta = "TodoSta";
        /// <summary>
        /// 会签状态
        /// </summary>
        public const string HuiQianTaskSta = "HuiQianTaskSta";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string Domain = "Domain";

        public const string PrjNo = "PrjNo";
        public const string PrjName = "PrjName";

        public const string OrgNo = "OrgNo";
        public const string FlowNote = "FlowNote";
        /// <summary>
        /// 耗时
        /// </summary>
        public const string LostTimeHH = "LostTimeHH";

        #endregion
    }
    /// <summary>
    /// 流程实例
    /// </summary>
    public class GenerWorkFlow : Entity
    {
        #region 基本属性
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return GenerWorkFlowAttr.WorkID;
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.OrgNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 所在的域
        /// </summary>
        public string Domain
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Domain);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Domain, value);
            }
        }

        public string BuessFields
        {
            get
            {
                return this.GetParaString("BuessFields");
            }
            set
            {
                this.SetPara("BuessFields", value);
            }
        }

        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_Flow);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.BillNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 最后的发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Sender);
            }
            set
            {
                //检查数据正确性.
                if (DataType.IsNullOrEmpty(value) == true)
                    throw new Exception("err@设置的人员不能为空.");

                if (value.Contains(";") == false)
                    value = value + ";";

                //检查数据正确性.
                if (value.Contains(",") == false || value.Contains(";") == false)
                    throw new Exception("err@设置的Sender人员格式不正确，请联系管理员,格式为:No,Name; 您设置的值为:" + value);

                //发送人.
                this.SetValByKey(GenerWorkFlowAttr.Sender, value);

                //当前日期.
                this.SetValByKey(GenerWorkFlowAttr.SendDT, DataType.CurrentDateTime);
            }
        }
        /// <summary>
        /// 发送日期
        /// </summary>
        public string SendDT
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SendDT);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SendDT, value);
            }
        }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FlowName);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.PRI);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.TodoEmps);
            }
            set
            {
                string str = value;
                str = str.Replace(" ", "");
                //TodoEmps在会签完去掉人员此判断去不掉，暂时注释掉
                //string val = this.GetValStrByKey(GenerWorkFlowAttr.TodoEmps);
                //if (val.Contains(str) == true)
                //    return;

                SetValByKey(GenerWorkFlowAttr.TodoEmps, str);
            }
        }

        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Emps);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.Emps, value);
            }
        }
        /// <summary>
        /// 会签状态
        /// </summary>
        public HuiQianTaskSta HuiQianTaskSta
        {
            get
            {
                //如果有方向信息，并且方向不包含到达的节点.
                if (this.HuiQianSendToNodeIDStr.Length > 3 && this.HuiQianSendToNodeIDStr.Contains(this.NodeID + ",") == false)
                    return WF.HuiQianTaskSta.None;

                return (HuiQianTaskSta)this.GetParaInt(GenerWorkFlowAttr.HuiQianTaskSta, 0);
            }
            set
            {
                SetPara(GenerWorkFlowAttr.HuiQianTaskSta, (int)value);
            }
        }
        /// <summary>
        /// 共享任务池状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(GenerWorkFlowAttr.TaskSta);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FlowSortNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 系统类别
        /// </summary>
        public string SysType
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SysType);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SysType, value);
            }
        }
        /// <summary>
        /// 发起人部门
        /// </summary>
		public string DeptNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_Dept);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Title);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Title, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GuestNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GuestName);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string NY
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.FK_NY);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 实际开始时间
        /// </summary>
        public string RDT
        {
            get
            {
                //string rdt = this.GetParaString("");
                return this.GetValStrByKey(GenerWorkFlowAttr.RDT);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.RDT, value);
                this.NY = value.Substring(0, 7);
            }
        }
        public string HungupTime
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.HungupTime);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.HungupTime, value);
            }
        }
        /// <summary>
        /// 计划开始时间
        /// SDTOfFlow 就是计划完成日期.
        /// </summary>
        public string RDTOfSetting
        {
            get
            {
                string str = this.GetParaString("RDTOfSetting");
                if (DataType.IsNullOrEmpty(str) == true)
                    return this.RDT;
                return str;
            }
            set
            {
                this.SetPara("RDTOfSetting", value);
            }
        }
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// RDTOfSetting 是计划开始日期，如果为空就是发起日期.
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SDTOfFlow, value);
            }
        }
        /// <summary>
        /// 流程预警时间时间
        /// </summary>
        public string SDTOfFlowWarning
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.SDTOfFlowWarning);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.SDTOfFlowWarning, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.WorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.FID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.PWorkID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PWorkID, value);
            }
        }
        public Int64 PFID
        {
            get
            {
                return this.GetValInt64ByKey(GenerWorkFlowAttr.PFID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PFID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.PNodeID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PFlowNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 项目编号
        /// </summary>
        public string PrjNo
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PrjNo);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PrjNo, value);
            }
        }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string PrjName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PrjName);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PrjName, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.PEmp);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.Starter);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.StarterName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.DeptName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.NodeName);
            }
            set
            {
                this.SetValByKey(GenerWorkFlowAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.FK_Node, value);

                //设置耗时. 
                TimeSpan ts = DateTime.Now - this.GetValDate(this.RDT);
                this.SetValByKey(GenerWorkFlowAttr.LostTimeHH, ts.TotalHours.ToString("0.00"));
            }
        }
        /// <summary>
		/// 工作流程状态
		/// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(GenerWorkFlowAttr.WFState);
            }
            set
            {
                if (value == BP.WF.WFState.Complete)
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Complete);
                else if (value == BP.WF.WFState.Delete || value == BP.WF.WFState.Blank)
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(GenerWorkFlowAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(GenerWorkFlowAttr.WFState, (int)value);

                //设置耗时.
                TimeSpan ts = DateTime.Now - this.GetValDate(this.RDT);
                this.SetValByKey(GenerWorkFlowAttr.LostTimeHH, ts.TotalHours.ToString("0.00"));
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(GenerWorkFlowAttr.WFSta);
            }
        }
        /// <summary>
        /// 是否可以批处理？
        /// </summary>
        public bool ItIsCanBatch
        {
            get
            {
                return this.GetParaBoolen("IsCanBatch");
            }
            set
            {
                this.SetPara("IsCanBatch", value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public string WFStateText
        {
            get
            {
                BP.WF.WFState ws = (WFState)this.WFState;
                switch (ws)
                {
                    case WF.WFState.Complete:
                        return "已完成";
                    case WF.WFState.Runing:
                        return "在运行";
                    case WF.WFState.Hungup:
                        return "挂起";
                    case WF.WFState.Askfor:
                        return "加签";
                    case WF.WFState.Draft:
                        return "草稿";
                    case WF.WFState.ReturnSta:
                        return "退回";
                    default:
                        return "其他" + ws.ToString();
                }
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(GenerWorkFlowAttr.GUID);
            }
            set
            {
                SetValByKey(GenerWorkFlowAttr.GUID, value);
            }
        }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 它的子流程
        /// </summary>
        public GenerWorkFlows HisSubFlowGenerWorkFlows
        {
            get
            {
                GenerWorkFlows ens = new GenerWorkFlows();
                ens.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
                return ens;
            }
        }
        /// <summary>
        /// 0=待办中,1=预警中,2=逾期中,3=按期完成,4=逾期完成
        /// </summary>
        public int TodoSta
        {
            get
            {
                return this.GetValIntByKey(GenerWorkFlowAttr.TodoSta);
            }
        }
        #endregion 扩展属性

        #region 参数属性.
        /// <summary>
        /// 是否是流程模版?
        /// </summary>
        public bool Paras_DBTemplate
        {
            get
            {
                return this.GetParaBoolen("DBTemplate");
            }
            set
            {
                this.SetPara("DBTemplate", value);
            }
        }
        /// <summary>
        /// 模版名称
        /// </summary>
        public string Paras_DBTemplateName
        {
            get
            {
                return this.GetParaString("DBTemplateName");
            }
            set
            {
                this.SetPara("DBTemplateName", value);
            }
        }
        /// <summary>
        /// 选择的表单(用于子流程列表里，打开草稿，记录当初选择的表单.)
        /// </summary>
        public string Paras_Frms
        {
            get
            {
                return this.GetParaString("Frms");
            }
            set
            {
                this.SetPara("Frms", value);
            }
        }
        /// <summary>
        /// 到达的节点
        /// </summary>
        public string Paras_ToNodes
        {
            get
            {
                return this.GetParaString("ToNodes");
            }
            set
            {
                this.SetPara("ToNodes", value);
            }
        }
        /// <summary>
        /// 关注&取消关注
        /// </summary>
        public bool Paras_Focus
        {
            get
            {
                return this.GetParaBoolen("F_" + BP.Web.WebUser.No, false);
            }
            set
            {
                this.SetPara("F_" + BP.Web.WebUser.No, value);
            }
        }
        /// <summary>
        /// 确认与取消确认
        /// </summary>
        public bool Paras_Confirm
        {
            get
            {
                return this.GetParaBoolen("C_" + BP.Web.WebUser.No, false);
            }
            set
            {
                this.SetPara("C_" + BP.Web.WebUser.No, value);
            }
        }
        /// <summary>
        /// 最后一个执行发送动作的ID.
        /// </summary>
        public string Paras_LastSendTruckID
        {
            get
            {
                string str = this.GetParaString("LastTruckID");
                if (str == "")
                    str = this.WorkID.ToString();
                return str;
            }
            set
            {
                this.SetPara("LastTruckID", value);
            }
        }

        /// <summary>
        /// 加签信息
        /// </summary>
        public string Paras_AskForReply
        {
            get
            {
                return this.GetParaString("AskForReply");
            }
            set
            {
                this.SetPara("AskForReply", value);
            }
        }
        /// <summary>
        /// 是否是退回并原路返回.
        /// </summary>
        public bool Paras_IsTrackBack
        {

            get
            {
                return this.GetParaBoolen("IsTrackBack");
            }
            set
            {
                this.SetPara("IsTrackBack", value);
            }
        }
        /// <summary>
        /// 分组Mark
        /// </summary>
        public string Paras_GroupMark
        {
            get
            {
                return this.GetParaString(GenerWorkerListAttr.GroupMark);
            }
            set
            {
                this.SetPara(GenerWorkerListAttr.GroupMark, value);
            }
        }
        /// <summary>
        /// 是否是自动运行
        /// 0=自动运行(默认,无需人工干涉). 1=手工运行(按照手工设置的模式运行,人工干涉模式).
        /// 用于自由流程中.
        /// </summary>
        public TransferCustomType TransferCustomType
        {
            get
            {
                return (TransferCustomType)this.GetParaInt("IsAutoRun");
            }
            set
            {
                this.SetPara("IsAutoRun", (int)value);
            }
        }
        /// <summary>
        /// 多人待办处理模式
        /// </summary>
        public TodolistModel TodolistModel
        {
            get
            {
                return (TodolistModel)this.GetParaInt("TodolistModel");
            }
            set
            {
                this.SetPara("TodolistModel", (int)value);
            }
        }
        /// <summary>
        /// 会签到达人员
        /// </summary>
        public string HuiQianSendToEmps
        {
            get
            {
                return this.GetParaString("HuiQianSendToEmps");
            }
            set
            {
                this.SetPara("HuiQianSendToEmps", value);
            }
        }
        /// <summary>
        /// 会签到达节点: 101@102
        /// </summary>
        public string HuiQianSendToNodeIDStr
        {
            get
            {
                return this.GetParaString("HuiQianSendToNodeID");
            }
            set
            {
                this.SetPara("HuiQianSendToNodeID", value);
            }
        }
        /// <summary>
        /// 会签主持人
        /// </summary>
        public string HuiQianZhuChiRen
        {
            get
            {
                return this.GetParaString("HuiQianZhuChiRen");
            }
            set
            {
                this.SetPara("HuiQianZhuChiRen", value);
            }
        }
        /// <summary>
        /// 会签主持人名称
        /// </summary>
        public string HuiQianZhuChiRenName
        {
            get
            {
                return this.GetParaString("HuiQianZhuChiRenName");
            }
            set
            {
                this.SetPara("HuiQianZhuChiRenName", value);
            }
        }

        public int ScripNodeID
        {
            get
            {
                return this.GetParaInt("ScripNodeID");
            }
            set
            {
                this.SetPara("ScripNodeID", value);
            }
        }
        public string ScripMsg
        {
            set
            {
                this.SetPara("ScripMsg", value);
            }
        }

        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public GenerWorkFlow()
        {
        }
        /// <summary>
        /// 按照WorkID查询.
        /// </summary>
        /// <param name="workId"></param>
        public GenerWorkFlow(Int64 workId)
        {
            //this.WorkID = workId
            //this.Retrieve();
            if (workId == 0)
                throw new Exception("工作 GenerWorkFlow 查询参数错误,WorkID不能为 0 .");

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkFlowAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 GenerWorkFlow [" + workId + "]不存在。");
        }
        /// <summary>
        /// 按照GUID查询.
        /// </summary>
        /// <param name="guid"></param>
        public GenerWorkFlow(string guid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GenerWorkFlowAttr.GUID, guid);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 GenerWorkFlow [" + guid + "]不存在。");
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

                Map map = new Map("WF_GenerWorkFlow", "流程实例");

                map.AddTBIntPK(GenerWorkFlowAttr.WorkID, 0, "WorkID", true, true); //主键.
                map.AddTBInt(GenerWorkFlowAttr.FID, 0, "流程ID", true, true);

                map.AddTBString(GenerWorkFlowAttr.FK_FlowSort, null, "流程类别", true, false, 0, 10, 10);

                //等于流程类别的Domain字段值.
                map.AddTBString(GenerWorkFlowAttr.SysType, null, "系统类别", true, false, 0, 10, 10);

                map.AddTBString(GenerWorkFlowAttr.FK_Flow, null, "流程", true, false, 0, 5, 10);
                map.AddTBString(GenerWorkFlowAttr.FlowName, null, "流程名称", true, false, 0, 100, 10);
                map.AddTBString(GenerWorkFlowAttr.Title, null, "标题", true, false, 0, 300, 10);

                //两个状态，在不同的情况下使用. WFState状态 可以查询到SELECT  * FROM Sys_Enum WHERE EnumKey='WFState'
                // WFState 的状态  @0=空白@1=草稿@2=运行中@3=已经完成@4=挂起@5=退回.
                map.AddTBInt(GenerWorkFlowAttr.WFSta, 0, "状态", true, false);
                map.AddTBInt(GenerWorkFlowAttr.WFState, 0, "状态", true, false);

                //  map.AddDDLSysEnum(GenerWorkFlowAttr.WFSta, 0, "状态", true, false, GenerWorkFlowAttr.WFSta, "@0=运行中@1=已完成@2=其他");
                //  map.AddDDLSysEnum(GenerWorkFlowAttr.WFState, 0, "流程状态", true, false, GenerWorkFlowAttr.WFState);

                map.AddTBString(GenerWorkFlowAttr.Starter, null, "发起人", true, false, 0, 200, 10);
                map.AddTBString(GenerWorkFlowAttr.StarterName, null, "发起人名称", true, false, 0, 200, 10);
                map.AddTBString(GenerWorkFlowAttr.Sender, null, "发送人", true, false, 0, 200, 10);

                map.AddTBDateTime(GenerWorkFlowAttr.RDT, "记录日期", true, true);
                map.AddTBString(GenerWorkFlowAttr.HungupTime, null, "挂起日期", true, false, 0, 50, 10);
                map.AddTBDateTime(GenerWorkFlowAttr.SendDT, "流程活动时间", true, true);
                map.AddTBInt(GenerWorkFlowAttr.FK_Node, 0, "节点", true, false);
                map.AddTBString(GenerWorkFlowAttr.NodeName, null, "节点名称", true, false, 0, 100, 10);

                map.AddTBString(GenerWorkFlowAttr.FK_Dept, null, "部门", true, false, 0, 100, 10);
                map.AddTBString(GenerWorkFlowAttr.DeptName, null, "部门名称", true, false, 0, 100, 10);
                map.AddTBInt(GenerWorkFlowAttr.PRI, 1, "优先级", true, true);

                map.AddTBDateTime(GenerWorkFlowAttr.SDTOfNode, "节点应完成时间", true, true);
                map.AddTBDateTime(GenerWorkFlowAttr.SDTOfFlow, null, "流程应完成时间", true, true);
                map.AddTBDateTime(GenerWorkFlowAttr.SDTOfFlowWarning, null, "流程预警时间", true, true);

                //父子流程信息.
                map.AddTBString(GenerWorkFlowAttr.PFlowNo, null, "父流程编号", true, false, 0, 100, 10);
                map.AddTBInt(GenerWorkFlowAttr.PWorkID, 0, "父流程ID", true, true);
                map.AddTBInt(GenerWorkFlowAttr.PNodeID, 0, "父流程调用节点", true, true);
                map.AddTBInt(GenerWorkFlowAttr.PFID, 0, "父流程调用的PFID", true, true);
                map.AddTBString(GenerWorkFlowAttr.PEmp, null, "子流程的调用人", true, false, 0, 100, 10);

                //客户流程信息.
                map.AddTBString(GenerWorkFlowAttr.GuestNo, null, "客户编号", true, false, 0, 100, 10);
                map.AddTBString(GenerWorkFlowAttr.GuestName, null, "客户名称", true, false, 0, 100, 10);

                map.AddTBString(GenerWorkFlowAttr.BillNo, null, "单据编号", true, false, 0, 100, 10);

                //任务池相关。
                map.AddTBString(GenerWorkFlowAttr.TodoEmps, null, "待办人员", true, false, 0, 4000, 10);
                map.AddTBInt(GenerWorkFlowAttr.TodoEmpsNum, 0, "待办人员数量", true, true);
                map.AddTBInt(GenerWorkFlowAttr.TaskSta, 0, "共享状态", true, true);

                //参数. (流程运行设置临时存储的参数)
                map.AddTBString(GenerWorkFlowAttr.AtPara, null, "参数", true, false, 0, 2000, 10);

                //(格式:@zhangshan,张三@lishi,李四)
                map.AddTBString(GenerWorkFlowAttr.Emps, null, "参与人", true, false, 0, 4000, 10);
                map.AddTBString(GenerWorkFlowAttr.GUID, null, "GUID", false, false, 0, 36, 10);
                map.AddTBString(GenerWorkFlowAttr.FK_NY, null, "年月", false, false, 0, 7, 7);
                map.AddTBInt(GenerWorkFlowAttr.WeekNum, 0, "周次", true, true);
                map.AddTBInt(GenerWorkFlowAttr.TSpan, 0, "时间间隔", true, true);

                //待办状态(0=待办中,1=预警中,2=逾期中,3=按期完成,4=逾期完成) 
                map.AddTBInt(GenerWorkFlowAttr.TodoSta, 0, "待办状态", true, true);

                map.AddTBString(GenerWorkFlowAttr.Domain, null, "域/系统编号", true, false, 0, 100, 30);
                //map.SetHelperAlert(GenerWorkFlowAttr.Domain, "用于区分不同系统的流程,比如:一个集团有多个子系统每个子系统都有自己的流程,就需要标记那些流程是那个子系统的.");

                map.AddTBString(GenerWorkFlowAttr.PrjNo, null, "PrjNo", true, false, 0, 100, 10);
                map.AddTBString(GenerWorkFlowAttr.PrjName, null, "PrjNo", true, false, 0, 100, 10);

                //隶属组织.
                map.AddTBString(GenerWorkFlowAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 10);

                // 审核组件，签批组件最后一个人的意见填写到这里.
                map.AddTBString(GenerWorkFlowAttr.FlowNote, null, "流程备注", true, false, 0, 500, 200);
                //  map.AddTBString(GenerWorkFlowAttr.LostTimeHH, null, "流程备注", true, false, 0, 500, 200);
                //  map.AddTBFloat(GenerWorkFlowAttr.LostTimeHH, 0, "耗时", true, true);

                RefMethod rm = new RefMethod();
                rm.Title = "工作轨迹";  // "工作报告";
                rm.ClassMethodName = this.ToString() + ".DoRpt";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程自检"; // "流程自检";
                rm.ClassMethodName = this.ToString() + ".DoSelfTestInfo";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程自检并修复";
                rm.ClassMethodName = this.ToString() + ".DoRepare";
                rm.Warning = "您确定要执行此功能吗？ \t\n 1)如果是断流程，并且停留在第一个节点上，系统为执行删除它。\t\n 2)如果是非地第一个节点，系统会返回到上次发起的位置。";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 业务属性.
        /// <summary>
        /// 执行移交.
        /// </summary>
        /// <param name="emps">移交给</param>
        /// <param name="note">移交原因</param>
        /// <returns>执行信息.</returns>
        public string Shift(string emps, string note)
        {

            try
            {
                return BP.WF.Dev2Interface.Node_Shift(this.WorkID, emps, note);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

        }
        /// <summary>
        /// 增加处理人
        /// </summary>
        /// <param name="emps"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string AddEmps(string emps, string note)
        {

            try
            {
                BP.WF.Dev2Interface.Node_AddTodolist(this.WorkID, emps);
                return "增加成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

        }
        public string RemoveEmps(string empNo, string note)
        {
            try
            {
                BP.WF.Dev2Interface.Node_AddTodolist(this.WorkID, empNo);
                return "移除成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string GenerTrackForReback()
        {
            string sql = "SELECT * FROM ND" + int.Parse(this.FlowNo) + "Track WHERE WorkID=" + this.WorkID + "  AND ActionType IN("+ ActionType.Start + ","+ActionType.Forward + ") ORDER BY RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string Reback(int nodeID, string msg)
        {
            try
            {
                return BP.WF.Dev2Interface.Flow_DoRebackWorkFlow(this.FlowNo, this.WorkID, nodeID, msg);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 业务属性.


        #region 重载基类方法
        /// <summary>
        /// 删除后,需要把工作者列表也要删除.
        /// </summary>
        protected override void afterDelete()
        {
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID in  ( select WorkID from WF_GenerWorkerlist WHERE WorkID not in (select WorkID from WF_GenerWorkFlow) )");
                    break;
                case DBType.MySQL:
                    DBAccess.RunSQL("DELETE A FROM WF_GenerWorkerlist A, WF_GenerWorkerlist B WHERE A.WorkID = B.WorkID And B.WorkID Not IN(select WorkID from WF_GenerWorkFlow)");
                    break;
                case DBType.PostgreSQL:
                case DBType.UX:
                case DBType.HGDB:
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist A USING WF_GenerWorkerlist B WHERE A.WorkID = B.WorkID And B.WorkID Not IN(select WorkID from WF_GenerWorkFlow)");
                    break;
                default: break;

            }

            WorkFlow wf = new WorkFlow(new Flow(this.FlowNo), this.WorkID, this.FID);
            wf.DoDeleteWorkFlowByReal(true); /* 删除下面的工作。*/
            base.afterDelete();
        }

        protected override bool beforeInsert()
        {
            if (this.Starter == "Guest")
            {
                this.StarterName = BP.Web.GuestUser.Name;
                this.GuestName = this.StarterName;
                this.GuestNo = BP.Web.GuestUser.No;
            }

            //加入组织no.
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            //生成GUID.
            this.GUID = BP.DA.DBAccess.GenerGUID();

            return base.beforeInsert();
        }
        #endregion

        #region 执行诊断

        /// <summary>
        /// 终止流程
        /// </summary>
        /// <param name="msg">终止的信息</param>
        /// <returns>终止结果</returns>
        public string DoFix(string msg)
        {
            return BP.WF.Dev2Interface.Flow_DoFix(this.WorkID, true, msg);
        }

        public string DoRpt()
        {
            return "WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FlowNo;
        }
        /// <summary>
        /// 增加子线程
        /// </summary>
        /// <param name="empStrs">要增加的人员多个用都好分开.</param>
        /// <returns></returns>
        public string DoSubFlowAddEmps(string empStrs, int toNodeID)
        {
            //获得当前的干流程的gwf.
            long workID = this.FID;
            if (workID == 0)
                workID = this.WorkID;
            return BP.WF.Dev2Interface.Node_FHL_AddSubThread(workID, empStrs, toNodeID);
        }
        /// <summary>
        /// 执行修复
        /// </summary>
        /// <returns></returns>
        public string DoRepare()
        {
            if (this.DoSelfTestInfo() == "没有发现异常。")
                return "没有发现异常。";

            string sql = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WORKID='" + this.WorkID + "' ORDER BY FK_Node desc";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                /*如果是开始工作节点，就删除它。*/
                WorkFlow wf = new WorkFlow(new Flow(this.FlowNo), this.WorkID, this.FID);
                wf.DoDeleteWorkFlowByReal(true);
                return "此流程是因为发起工作失败被系统删除。";
            }

            int FK_Node = int.Parse(dt.Rows[0][0].ToString());

            Node nd = new Node(FK_Node);
            if (nd.ItIsStartNode)
            {
                /*如果是开始工作节点，就删除它。*/
                WorkFlow wf = new WorkFlow(new Flow(this.FlowNo), this.WorkID, this.FID);
                wf.DoDeleteWorkFlowByReal(true);
                return "此流程是因为发起工作失败被系统删除。";
            }

            this.NodeID = nd.NodeID;
            this.NodeName = nd.Name;
            this.Update();

            string str = "";
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.FK_Node, FK_Node, GenerWorkerListAttr.WorkID, this.WorkID);
            foreach (GenerWorkerList wl in wls)
            {
                str += wl.EmpNo + wl.EmpName + ",";
            }

            return "此流程是因为[" + nd.Name + "]工作发送失败被回滚到当前位置，请转告[" + str + "]流程修复成功。";
        }
        public string DoSelfTestInfo()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.FlowNo);

            #region  查看一下当前的节点是否开始工作节点。
            Node nd = new Node(this.NodeID);
            if (nd.ItIsStartNode)
            {
                /* 判断是否是退回的节点 */
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.Retrieve();
            }
            #endregion


            #region  查看一下是否有当前的工作节点信息。
            bool isHave = false;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.NodeID == this.NodeID)
                    isHave = true;
            }

            if (isHave == false)
            {
                /*  */
                return "已经不存在当前的工作节点信息，造成此流程的原因可能是没有捕获的系统异常，建议删除此流程或者交给系统自动修复它。";
            }
            #endregion

            return "没有发现异常。";
        }
        #endregion
    }
    /// <summary>
    /// 流程实例s
    /// </summary>
    public class GenerWorkFlows : Entities
    {
        /// <summary>
        /// 根据工作流程,工作人员 ID 查询出来他当前的能做的工作.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="empId">工作人员ID</param>
        /// <returns></returns>
        public static DataTable QuByFlowAndEmp(string flowNo, int empId)
        {
            string sql = "SELECT a.WorkID FROM WF_GenerWorkFlow a, WF_GenerWorkerlist b WHERE a.WorkID=b.WorkID   AND b.FK_Node=a.FK_Node  AND b.FK_Emp='" + empId.ToString() + "' AND a.FK_Flow='" + flowNo + "'";
            return DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 根据流程编号，标题模糊查询
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="likeKey"></param>
        /// <returns></returns>
        public string QueryByLike(string flowNo, string likeKey)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere("FK_Flow", flowNo);
            if (DataType.IsNullOrEmpty(likeKey) == false)
            {
                qo.addAnd();
                if (BP.Difference.SystemConfig.AppCenterDBVarStr == "@" || BP.Difference.SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere("Title", " LIKE ", BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title" + ",'%')") : (" '%'+" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title" + "+'%'"));
                else
                    qo.AddWhere("Title", " LIKE ", " '%'||" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Title" + "||'%'");
                qo.MyParas.Add("Title", likeKey);
            }

            qo.addOrderBy("WorkID");
            qo.DoQuery();
            return BP.Tools.Json.ToJson(this.ToDataTableField("WF_GenerWorkFlow"));
        }

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GenerWorkFlow();
            }
        }
        /// <summary>
        /// 流程实例集合
        /// </summary>
        public GenerWorkFlows() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List  
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GenerWorkFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<GenerWorkFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GenerWorkFlow> Tolist()
        {
            System.Collections.Generic.List<BP.WF.GenerWorkFlow> list = new System.Collections.Generic.List<BP.WF.GenerWorkFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BP.WF.GenerWorkFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
