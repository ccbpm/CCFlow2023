﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 抄送状态
    /// </summary>
    public enum CCSta
    {
        /// <summary>
        /// 未读
        /// </summary>
        UnRead,
        /// <summary>
        /// 已读
        /// </summary>
        Read,
        /// <summary>
        /// 已回复
        /// </summary>
        CheckOver,
        /// <summary>
        /// 已删除
        /// </summary>
        Del
    }
    /// <summary>
    /// 所有子流程结束，父流程处理规则
    /// </summary>
    public enum AllSubFlowOverRole
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 发送父流程到下一个节点
        /// </summary>
        SendParentFlowToNextStep,
        /// <summary>
        /// 结束父流程
        /// </summary>
        OverParentFlow
    }
    /// <summary>
    /// 根据子流程运行模式控制父流程的运行到下一个节点/结束
    /// </summary>
    public enum SubFlowRunModel
    {
        /// <summary>
        /// 无，不设置
        /// </summary>
        None,
        /// <summary>
        /// 子流程结束
        /// </summary>
        FlowOver,
        /// <summary>
        /// 子流程运行到指定节点
        /// </summary>
        SpecifiedNodes

    }
    /// <summary>
    /// 子流程数据反填父流程数据规则
    /// </summary>
    public enum BackCopyRole
    {
        /// <summary>
        /// 不反填
        /// </summary>
        None,
        /// <summary>
        /// 字段自动匹配
        /// </summary>
        AutoFieldMatch,
        /// <summary>
        /// 按照设置的格式匹配
        /// </summary>
        FollowSetFormat,
        /// <summary>
        /// 混合模式
        /// </summary>
        MixedMode
    }
    /// <summary>
    /// 会签模式
    /// </summary>
    public enum HuiQianRole
    {
        None,
        /// <summary>
        /// 队列(按照顺序处理，有最后一个人发送到下一个节点)
        /// </summary>
        Teamup = 1,
        /// <summary>
        /// 协作组长模式
        /// </summary>
        TeamupGroupLeader = 4
    }
    /// <summary>
    /// 组长会签规则
    /// </summary>
    public enum HuiQianLeaderRole
    {
        /// <summary>
        /// 仅有一个组长
        /// </summary>
        OnlyOne = 0,
        /// <summary>
        /// 最后一个组长为主
        /// </summary>
        LastOneMain = 1,
        /// <summary>
        /// 任意组长为主
        /// </summary>
        EveryOneMain = 2
    }
    /// <summary>
    /// 方向条件控制规则
    /// </summary>
    public enum DirCondModel
    {
        /// <summary>
        /// 由连接线控制
        /// </summary>
        ByLineCond = 0,
        /// <summary>
        /// 发送后手工选择到达节点与接受人（用于子线程节点）
        /// </summary>
        ByPopSelect = 1,
        /// <summary>
        /// 下拉框模式
        /// </summary>
        ByDDLSelected = 2,
        /// <summary>
        /// 按照按钮选择
        /// </summary>
        ByButtonSelected = 3

    }
    /// <summary>
    /// 抢办发送后执行规则
    /// </summary>
    public enum QiangBanSendAfterRole
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None = 0,
        /// <summary>
        /// 抄送给其他人
        /// </summary>
        CCToEtcEmps = 1,
        /// <summary>
        /// 发送消息给其他人
        /// </summary>
        SendMsgToEtcEmps = 2
    }
    /// <summary>
    /// 待办工作超时处理方式
    /// </summary>
    public enum OutTimeDeal
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None = 0,
        /// <summary>
        /// 自动的转向下一步骤
        /// </summary>
        AutoTurntoNextStep = 1,
        /// <summary>
        /// 自动跳转到指定的点
        /// </summary>
        AutoJumpToSpecNode = 2,
        /// <summary>
        /// 自动移交到指定的人员
        /// </summary>
        AutoShiftToSpecUser = 3,
        /// <summary>
        /// 向指定的人员发送消息
        /// </summary>
        SendMsgToSpecUser = 4,
        /// <summary>
        /// 删除流程
        /// </summary>
        DeleteFlow = 5,
        /// <summary>
        /// 执行SQL
        /// </summary>
        RunSQL = 6
    }
    public enum SelectorModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        Station,
        /// <summary>
        /// 部门
        /// </summary>
        Dept,
        /// <summary>
        /// 操作员
        /// </summary>
        Emp,
        /// <summary>
        /// SQL
        /// </summary>
        SQL,
        /// <summary>
        /// SQL模版计算
        /// </summary>
        SQLTemplate,
        /// <summary>
        /// 通用的人员选择器.
        /// </summary>
        GenerUserSelecter,
        /// <summary>
        /// 按部门与角色的交集
        /// </summary>
        DeptAndStation,
        /// <summary>
        /// 自定义链接
        /// </summary>
        Url,
        /// <summary>
        /// 通用部门角色人员选择器
        /// </summary>
        AccepterOfDeptStationEmp,
        /// <summary>
        /// 按角色智能计算(操作员所在部门)
        /// </summary>
        AccepterOfDeptStationOfCurrentOper,
        /// <summary>
        /// 按本组织用户计算.
        /// </summary>
        TeamOrgOnly = 10,
        /// <summary>
        /// 按全组织用户计算
        /// </summary>
        TeamOnly = 11,
        /// <summary>
        /// 按本组织部门用户计算
        /// </summary>
        TeamDeptOnly = 12,
        /// <summary>
        /// 按照角色智能计算
        /// </summary>
        ByStationAI = 13,
        /// <summary>
        /// 按照webapi接口计算
        /// </summary>
        ByWebAPI=14,
        /// <summary>
        /// 按照webapi接口计算
        /// </summary>
        ByMyDeptEmps = 15
    }
    
    /// <summary>
    /// 运行平台
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// CCFlow .net平台.
        /// </summary>
        CCFlow,
        /// <summary>
        /// JFlow java 平台.
        /// </summary>
        JFlow
    }
    /// <summary>
    /// 加签模式
    /// </summary>
    public enum AskforHelpSta
    {
        /// <summary>
        /// 加签后直接发送
        /// </summary>
        AfterDealSend = 5,
        /// <summary>
        /// 加签后由我直接发送
        /// </summary>
        AfterDealSendByWorker = 6
    }
    /// <summary>
    /// 删除流程规则
    /// @0=不能删除
    /// @1=逻辑删除
    /// @2=记录日志方式删除: 数据删除后，记录到WF_DeleteWorkFlow中。
    /// @3=彻底删除：
    /// @4=让用户决定删除方式
    /// </summary>
    public enum DelWorkFlowRole
    {
        /// <summary>
        /// 不能删除
        /// </summary>
        None,
        /// <summary>
        /// 按照标记删除(需要交互,填写删除原因)
        /// </summary>
        DeleteByFlag,
        /// <summary>
        /// 删除到日志库(需要交互,填写删除原因)
        /// </summary>
        DeleteAndWriteToLog,
        /// <summary>
        /// 彻底的删除(不需要交互，直接干净彻底的删除)
        /// </summary>
        DeleteReal,
        /// <summary>
        /// 让用户决定删除方式(需要交互)
        /// </summary>
        ByUser
    }
    /// <summary>
    /// 导入流程的模式
    /// </summary>
    public enum ImpFlowTempleteModel
    {
        /// <summary>
        /// 按新的流程导入
        /// </summary>
        AsNewFlow,
        /// <summary>
        /// 按模版的流程编号
        /// </summary>
        AsTempleteFlowNo,
        /// <summary>
        /// 覆盖当前的流程
        /// </summary>
        OvrewaiteCurrFlowNo,
        /// <summary>
        /// 按指定的流程编号导入
        /// </summary>
        AsSpecFlowNo
    }
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 发起
        /// </summary>
        Start = 0,
        /// <summary>
        /// 前进(发送)
        /// </summary>
        Forward = 1,
        /// <summary>
        /// 退回
        /// </summary>
        Return = 2,
        /// <summary>
        /// 退回并原路返回.
        /// </summary>
        ReturnAndBackWay = 201,
        /// <summary>
        /// 移交
        /// </summary>
        Shift = 3,
        /// <summary>
        /// 撤消移交
        /// </summary>
        UnShift = 4,
        /// <summary>
        /// 撤消发送
        /// </summary>
        UnSend = 5,
        /// <summary>
        /// 分流前进
        /// </summary>
        ForwardFL = 6,
        /// <summary>
        /// 合流前进
        /// </summary>
        ForwardHL = 7,
        /// <summary>
        /// 流程正常结束
        /// </summary>
        FlowOver = 8,
        /// <summary>
        /// 调用起子流程
        /// </summary>
        CallChildenFlow = 9,
        /// <summary>
        /// 启动子流程
        /// </summary>
        StartChildenFlow = 10,
        /// <summary>
        /// 子线程前进
        /// </summary>
        SubThreadForward = 11,
        /// <summary>
        /// 取回
        /// </summary>
        Tackback = 12,
        /// <summary>
        /// 恢复已完成的流程
        /// </summary>
        RebackOverFlow = 13,
        /// <summary>
        /// 强制终止流程 For lijian:2012-10-24.
        /// </summary>
        FlowOverByCoercion = 14,
        /// <summary>
        /// 挂起
        /// </summary>
        Hungup = 15,
        /// <summary>
        /// 取消挂起
        /// </summary>
        UnHungup = 16,
        /// <summary>
        /// 强制移交
        /// </summary>
        ShiftByCoercion = 17,
        /// <summary>
        /// 催办
        /// </summary>
        Press = 18,
        /// <summary>
        /// 逻辑删除流程(撤销流程)
        /// </summary>
        DeleteFlowByFlag = 19,
        /// <summary>
        /// 恢复删除流程(撤销流程)
        /// </summary>
        UnDeleteFlowByFlag = 20,
        /// <summary>
        /// 抄送
        /// </summary>
        CC = 21,
        /// <summary>
        /// 工作审核(日志)
        /// </summary>
        WorkCheck = 22,
        /// <summary>
        /// 删除子线程
        /// </summary>
        DeleteSubThread = 23,
        /// <summary>
        /// 请求加签
        /// </summary>
        AskforHelp = 24,
        /// <summary>
        /// 加签向下发送
        /// </summary>
        ForwardAskfor = 25,
        /// <summary>
        /// 自动条转的方式向下发送
        /// </summary>
        Skip = 26,
        /// <summary>
        /// 队列发送
        /// </summary>
        Order = 27,
        /// <summary>
        /// 协作发送
        /// </summary>
        TeampUp = 28,
        /// <summary>
        /// 流程评论
        /// </summary>
        FlowBBS = 29,
        /// <summary>
        /// 执行会签
        /// </summary>
        HuiQian = 30,
        /// <summary>
        /// 调整流程
        /// </summary>
        Adjust = 31,
        /// <summary>
        /// 路由节点
        /// </summary>
        Route = 32,
        /// <summary>
        /// 信息
        /// </summary>
        Info = 100
    }
    /// <summary>
    /// 挂起方式
    /// </summary>
    public enum HungupWay
    {
        /// <summary>
        /// 永久挂起
        /// </summary>
        Forever,
        /// <summary>
        /// 在指定的日期解除
        /// </summary>
        SpecDataRel
    }
    /// <summary>
    /// 自动跳转规则
    /// </summary>
    public enum AutoJumpRole
    {
        /// <summary>
        /// 处理人就是提交人
        /// </summary>
        DealerIsDealer,
        /// <summary>
        /// 处理人已经出现过
        /// </summary>
        DealerIsInWorkerList,
        /// <summary>
        /// 处理人与上一步相同
        /// </summary>
        DealerAsNextStepWorker
    }
    /// <summary>
    /// 抄送到角色计算方式
    /// </summary>
    public enum CCStaWay
    {
        /// <summary>
        /// 仅按角色计算
        /// </summary>
        StationOnly,
        /// <summary>
        /// 按角色智能计算(当前节点的人员角色)
        /// </summary>
        StationSmartCurrNodeWorker,
        /// <summary>
        /// 按角色智能计算(接受节点的人员角色)
        /// </summary>
        StationSmartNextNodeWorker,
        /// <summary>
        /// 按角色与部门的交集
        /// </summary>
        StationAndDept,
        /// <summary>
        /// 按直线部门找角色下的人员(当前节点)
        /// </summary>
        StationDeptUpLevelCurrNodeWorker,
        /// <summary>
        /// 按直线部门找角色下的人员
        /// </summary>
        StationDeptUpLevelNextNodeWorker
    }
    /// <summary>
    /// 抄送数据写入规则
    /// </summary>
    public enum CCWriteTo
    {
        /// <summary>
        /// 抄送列表
        /// </summary>
        CCList,
        /// <summary>
        /// 待办列表
        /// </summary>
        Todolist,
        /// <summary>
        /// 抄送与待办列表
        /// </summary>
        All,
    }
    /// <summary>
    /// 组长确认规则
    /// </summary>
    public enum TeamLeaderConfirmRole
    {
        /// <summary>
        /// 按照部门表的字段 Leader 模式计算.
        /// </summary>
        ByDeptFieldLeader,
        /// <summary>
        /// 按照SQL计算.
        /// </summary>
        BySQL,
        /// <summary>
        /// 会签时主持人计算
        /// </summary>
        HuiQianLeader
    }
    /// <summary>
    /// 普通工作节点处理模式
    /// </summary>
    public enum TodolistModel
    {
        /// <summary>
        /// 抢办(谁抢到谁来办理,办理完后其他人就不能办理.)
        /// </summary>
        QiangBan = 0,
        /// <summary>
        /// 协作(没有处理顺序，接受的人都要去处理,由最后一个人发送到下一个节点)
        /// </summary>
        Teamup = 1,
        /// <summary>
        /// 队列(按照顺序处理，有最后一个人发送到下一个节点)
        /// </summary>
        Order = 2,
        /// <summary>
        /// 共享模式(需要申请，申请后才能执行)
        /// </summary>
        Sharing = 3,
        /// <summary>
        /// 协作组长模式
        /// </summary>
        TeamupGroupLeader = 4
    }
    /// <summary>
    /// 阻塞模式
    /// </summary>
    public enum BlockModel
    {
        /// <summary>
        /// 不阻塞
        /// </summary>
        None = 0,
        /// <summary>
        /// 当前节点的有未完成的子线程
        /// </summary>
        CurrNodeAll = 1,
        /// <summary>
        /// 按照约定的格式阻塞.
        /// </summary>
        SpecSubFlow = 2,
        /// <summary>
        /// 按照配置的sql阻塞,返回大于等于1表示阻塞,否则不阻塞.
        /// </summary>
        BySQL = 3,
        /// <summary>
        /// 按照表达式阻塞，表达式类似方向条件的表达式.
        /// </summary>
        ByExp = 4,
        /// <summary>
        /// 为父流程时，指定的子流程未运行到指定节点，则阻塞
        /// </summary>
        SpecSubFlowNode = 5,
        /// <summary>
        /// 为平级子流程时，指定的子流程未运行到指定节点，则阻塞
        /// </summary>
        SameLevelSubFlow = 6
    }
    /// <summary>
    /// 节点工作批处理
    /// </summary>
    public enum BatchRole
    {
        /// <summary>
        /// 不可以
        /// </summary>
        None,
        /// <summary>
        /// 批量审批
        /// </summary>
        WorkCheckModel,
        /// <summary>
        /// 分组批量审核
        /// </summary>
        Group
    }
    /// <summary>
    /// 子线程删除规则
    /// </summary>
    public enum ThreadKillRole
    {
        /// <summary>
        /// 不能删除，不许等到全部完成才可以向下运动。
        /// </summary>
        None,
        /// <summary>
        /// 需要手工的删除才可以向下运动。
        /// </summary>
        ByHand,
        /// <summary>
        /// 自动删除未完成的子线程。
        /// </summary>
        ByAuto
    }
    
    
    /// <summary>
    /// 撤销规则
    /// </summary>
    public enum CancelRole
    {
        /// <summary>
        /// 仅上一步
        /// </summary>
        OnlyNextStep,
        /// <summary>
        /// 不能撤销
        /// </summary>
        None,
        /// <summary>
        /// 上一步与开始节点.
        /// </summary>
        NextStepAndStartNode,
        /// <summary>
        /// 可以撤销指定的节点
        /// </summary>
        SpecNodes
    }
    /// <summary>
    /// 抄送方式
    /// </summary>
    public enum CCWay
    {
        /// <summary>
        /// 按照信息发送
        /// </summary>
        ByMsg,
        /// <summary>
        /// 按照e-mail
        /// </summary>
        ByEmail,
        /// <summary>
        /// 按照电话
        /// </summary>
        ByPhone,
        /// <summary>
        /// 按照数据库功能
        /// </summary>
        ByDBFunc
    }
    /// <summary>
    /// 抄送类型
    /// </summary>
    public enum CCType
    {
        /// <summary>
        /// 不抄送
        /// </summary>
        None,
        /// <summary>
        /// 按人员
        /// </summary>
        AsEmps,
        /// <summary>
        /// 按角色
        /// </summary>
        AsStation,
        /// <summary>
        /// 按节点
        /// </summary>
        AsNode,
        /// <summary>
        /// 按部门
        /// </summary>
        AsDept,
        /// <summary>
        /// 按照部门与角色
        /// </summary>
        AsDeptAndStation
    }
    /// <summary>
    /// 流程类型
    /// </summary>
    public enum FlowType_del
    {
        /// <summary>
        /// 平面流程
        /// </summary>
        Panel,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL
    }
    /// <summary>
    /// 流程发起限制
    /// </summary>
    public enum StartLimitRole
    {
        /// <summary>
        /// 不限制
        /// </summary>
        None = 0,
        /// <summary>
        /// 一人一天一次
        /// </summary>
        Day = 1,
        /// <summary>
        /// 一人一周一次
        /// </summary>
        Week = 2,
        /// <summary>
        /// 一人一月一次
        /// </summary>
        Month = 3,
        /// <summary>
        /// 一人一季度一次
        /// </summary>
        JD = 4,
        /// <summary>
        /// 一人一年一次
        /// </summary>
        Year = 5,
        /// <summary>
        /// 发起的列不能重复,(多个列可以用逗号分开)
        /// </summary>
        ColNotExit = 6,
        /// <summary>
        /// 设置的SQL数据源为空,或者返回结果为零时可以启动.
        /// </summary>
        ResultIsZero = 7,
        /// <summary>
        /// 设置的SQL数据源为空,或者返回结果为零时不可以启动.
        /// </summary>
        ResultIsNotZero = 8,
        /// <summary>
        /// 为子流程时仅仅只能被调用1次.
        /// </summary>
        OnlyOneSubFlow = 9
    }
    /// <summary>
    /// 装在前提示
    /// </summary>
    public enum StartLimitWhen
    {
        /// <summary>
        /// 表单装载后
        /// </summary>
        StartFlow,
        /// <summary>
        /// 发送前检查
        /// </summary>
        SendWhen
    }
    /// <summary>
    /// 流程启动类型
    /// </summary>
    public enum FlowRunWay
    {
        /// <summary>
        /// 手工启动
        /// </summary>
        HandWork=0,
        /// <summary>
        /// 指定人员按时启动
        /// </summary>
        SpecEmp=1,
        /// <summary>
        /// 数据集按时启动
        /// </summary>
        SelectSQLModel=2,
        /// <summary>
        /// 触发式启动
        /// </summary>
        WF_TaskTableInsertModel=3,
        /// <summary>
        /// 指定人员按时启动高级模式
        /// </summary>
        SpecEmpAdv=4,
        /// <summary>
        ///  让管理员启动流程发送到指定
        /// </summary>
        LetAdminSendSpecEmp=5

    }
    /// <summary>
    /// 保存模式
    /// </summary>
    public enum SaveModel
    {
        /// <summary>
        /// 仅节点表.
        /// </summary>
        NDOnly,
        /// <summary>
        /// 节点表与Rpt表.
        /// </summary>
        NDAndRpt
    }
    /// <summary>
    /// 节点完成转向处理
    /// </summary>
    public enum TurnToDeal
    {
        /// <summary>
        /// 按系统默认的提示
        /// </summary>
        CCFlowMsg,
        /// <summary>
        /// 指定消息
        /// </summary>
        SpecMsg,
        /// <summary>
        /// 指定Url
        /// </summary>
        SpecUrl,
        /// <summary>
        /// 发送后关闭
        /// </summary>
        TurntoClose,
        /// <summary>
        /// 按条件转向
        /// </summary>
        TurnToByCond

    }
    /// <summary>
    /// 投递方式
    /// </summary>
    public enum DeliveryWay
    {
        /// <summary>
        /// 按角色(以部门为纬度)
        /// </summary>
        ByStation = 0,
        /// <summary>
        /// 本部门内的人员
        /// </summary>
        FindSpecDeptEmpsInStationlist = 19,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept = 1,
        /// <summary>
        /// 按SQL
        /// </summary>
        BySQL = 2,
        /// <summary>
        /// 按本节点绑定的人员
        /// </summary>
        ByBindEmp = 3,
        /// <summary>
        /// 由上一步发送人选择
        /// </summary>
        BySelected = 4,
        /// <summary>
        /// 所有人员都可以发起
        /// </summary>
        BySelected_1 = 41,
        /// <summary>
        /// 固定范围的选择
        /// </summary>
        BySelected_2 = 60,
        /// <summary>
        /// 按表单选择人员
        /// </summary>
        ByPreviousNodeFormEmpsField = 5, //字段是主表.
        ByPreviousNodeFormEmpsFrmDtl = 501, //字段是从表.
        ByPreviousNodeFormEmpsTeam = 502, // 字段是权限组.
        ByPreviousNodeFormDepts = 503, //字段是部门编号
        /// <summary>
        /// 按照角色计算
        /// </summary>
        ByPreviousNodeFormStationsAI = 53,
        /// <summary>
        /// 智能计算
        /// </summary>
        ByPreviousNodeFormStationsOnly = 54,
        /// <summary>
        /// 与上一节点的人员相同
        /// </summary>
        ByPreviousNodeEmp = 6,
        /// <summary>
        /// 与开始节点的人员相同
        /// </summary>
        ByStarter = 7,
        /// <summary>
        /// 与指定节点的人员相同
        /// </summary>
        BySpecNodeEmp = 8,
        /// <summary>
        /// 按角色与部门交集计算
        /// </summary>
        ByDeptAndStation = 9,
        /// <summary>
        /// 按角色计算(以部门集合为纬度)
        /// </summary>
        //ByStationAndEmpDept = 10,
        /// <summary>
        /// 按指定节点的人员或者指定字段作为人员的角色计算
        /// </summary>
        BySpecNodeEmpStation = 11,
        /// <summary>
        /// 按SQL确定子线程接受人与数据源.
        /// </summary>
        BySQLAsSubThreadEmpsAndData = 12,
        /// <summary>
        /// 按明细表确定子线程接受人.
        /// </summary>
        ByDtlAsSubThreadEmps = 13,
        /// <summary>
        /// 仅按角色计算
        /// </summary>
        ByStationOnly = 14,
        /// <summary>
        /// FEE计算.
        /// </summary>
        ByFEE = 15,
        /// <summary>
        /// 按绑定部门计算,该部门一人处理标识该工作结束(子线程).
        /// </summary>
        BySetDeptAsSubthread = 16,
        /// <summary>
        /// 按SQL模版计算
        /// </summary>
        BySQLTemplate = 17,
        /// <summary>
        /// 从人员到人员
        /// </summary>
        ByFromEmpToEmp = 18,
        /// <summary>
        /// 按照角色计算-范围内的
        /// </summary>
        ByStationForPrj = 20,
        /// <summary>
        /// 按照选择模式计算.
        /// </summary>
        BySelectedForPrj = 21,
        /// <summary>
        /// 按照设置的组织计算
        /// </summary>
        BySelectedOrgs = 22,
        /// <summary>
        /// 按照部门领导计算
        /// </summary>
        ByDeptLeader = 23,
        /// <summary>
        /// 按照部门分管领导计算
        /// </summary>
        ByDeptShipLeader = 28,
        /// <summary>
        /// 找自己的直属领导.
        /// </summary>
        ByEmpLeader = 50,
        /// <summary>
        /// 按照用户组计算(本组织范围内)
        /// </summary>
        ByTeamOrgOnly = 24,
        /// <summary>
        /// 按照用户组计算(全集团)
        /// </summary>
        ByTeamOnly = 25,
        /// <summary>
        /// 按照用户组计算(本部门范围内)
        /// </summary>
        ByTeamDeptOnly = 26,
        /// <summary>
        /// 按照绑定角色的用户组人员
        /// </summary>
        ByBindTeamEmp = 27,
        /// <summary>
        /// 按照组织模式人员选择器
        /// </summary>
        BySelectedEmpsOrgModel = 43,
        /// <summary>
        /// 按照自定义Url模式的人员选择器
        /// </summary>
        BySelfUrl = 44,
        /// <summary>
        /// 按照设置的WebAPI接口获取的数据计算
        /// </summary>
        ByAPIUrl = 45,
        /// <summary>
        /// 发送人的上级部门的负责人
        /// 就是找上级领导主管.
        /// </summary>
        BySenderParentDeptLeader = 46,
        /// <summary>
        /// 发送人上级部门指定的角色
        /// </summary>
        BySenderParentDeptStations = 47,
        /// <summary>
        /// 外部用户
        /// </summary>
        ByGuest = 51,
     
        /// <summary>
        /// 选择其他组织的联络员
        /// </summary>
        BySelectEmpByOfficer = 55,
        /// <summary>
        /// 绑定字典表
        /// </summary>
        BySFTable = 52,
        /// <summary>
        /// 按指定的部门集合与设置的岗位交集
        /// </summary>
        ByStationSpecDepts = 56,
        /// <summary>
        /// 按指定的角色集合与设置的部门交集
        /// </summary>
        ByStationSpecStas = 57,
        /// <summary>
        /// 按照ccflow的BPM模式处理
        /// </summary>
        ByCCFlowBPM = 100
    }
    /// <summary>
    /// 自动发起
    /// </summary>
    public enum AutoStart
    {
        /// <summary>
        /// 手工启动（默认）
        /// </summary>
         None = 0,
        /// <summary>
        /// 按照指定的人员
        /// </summary>
         ByDesignee = 1,
        /// <summary>
        /// 数据集按时启动
        /// </summary>
         ByTineData = 2,
        /// <summary>
        /// 触发试启动
        /// </summary>
         ByTrigger = 3
    }
 

    /// <summary>
    /// 节点工作退回规则
    /// </summary>
    public enum JumpWay
    {
        /// <summary>
        /// 不能跳转
        /// </summary>
        CanNotJump,
        /// <summary>
        /// 向后跳转
        /// </summary>
        Next,
        /// <summary>
        /// 向前跳转
        /// </summary>
        Previous,
        /// <summary>
        /// 任何节点
        /// </summary>
        AnyNode,
        /// <summary>
        /// 任意点
        /// </summary>
        JumpSpecifiedNodes
    }
    /// <summary>
    /// 节点工作退回规则
    /// </summary>
    public enum ReturnRole
    {
        /// <summary>
        /// 不能退回
        /// </summary>
        CanNotReturn,
        /// <summary>
        /// 只能退回上一个节点
        /// </summary>
        ReturnPreviousNode,
        /// <summary>
        /// 可退回以前任意节点(默认)
        /// </summary>
        ReturnAnyNodes,
        /// <summary>
        /// 可退回指定的节点
        /// </summary>
        ReturnSpecifiedNodes,
        /// <summary>
        /// 由流程图设计的退回路线来决定
        /// </summary>
        ByReturnLine
    }
    /// <summary>
    /// 附件开放类型
    /// </summary>
    public enum FJOpen
    {
        /// <summary>
        /// 不开放
        /// </summary>
        None,
        /// <summary>
        /// 对操作员开放
        /// </summary>
        ForEmp,
        /// <summary>
        /// 对工作ID开放
        /// </summary>
        ForWorkID,
        /// <summary>
        /// 对流程ID开放
        /// </summary>
        ForFID
    }
    /// <summary>
    /// 分流规则
    /// </summary>
    public enum FLRole
    {
        /// <summary>
        /// 按照接受人
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按照部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按照角色
        /// </summary>
        ByStation
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL = 3,
        /// <summary>
        /// 同表单子线程
        /// </summary>
        SubThreadSameWorkID = 4,
        /// <summary>
        /// 异表单子线程
        /// </summary>
        SubThreadUnSameWorkID = 5
    }

    public enum NodeType
    {
        /// <summary>
        /// 用户节点
        /// </summary>
        UserNode=0,
        /// <summary>
        /// 路由节点
        /// </summary>
        RouteNode=1,
        /// <summary>
        /// 抄送节点
        /// </summary>
        CCNode = 2,
        /// <summary>
        /// 子流程节点
        /// </summary>
        SubFlowNode = 3,
    }
    /// <summary>
    /// 流程状态(详)
    /// ccflow根据是否启用草稿分两种工作模式,它的设置是在web.config 是 IsEnableDraft 节点来配置的.
    /// 1, 不启用草稿  IsEnableDraft = 0.
    ///    这种模式下，就没有草稿状态, 一个用户进入工作界面后就生成一个Blank, 用户保存时，也是存储blank状态。
    /// 2, 启用草稿.
    /// </summary>
    public enum WFState
    {
        /// <summary>
        /// 空白
        /// </summary>
        Blank = 0,
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        Runing = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Complete = 3,
        /// <summary>
        /// 挂起
        /// </summary>
        Hungup = 4,
        /// <summary>
        /// 退回
        /// </summary>
        ReturnSta = 5,
        /// <summary>
        /// 转发(移交)
        /// </summary>
        Shift = 6,
        /// <summary>
        /// 删除(逻辑删除状态)
        /// </summary>
        Delete = 7,
        /// <summary>
        /// 加签
        /// </summary>
        Askfor = 8,
        /// <summary>
        /// 冻结
        /// </summary>
        Fix = 9,
        /// <summary>
        /// 加签回复状态
        /// </summary>
        AskForReplay = 10
    }
    /// <summary>
    /// 节点工作类型
    /// </summary>
    public enum NodeWorkType
    {
        Work = 0,
        /// <summary>
        /// 开始节点
        /// </summary>
        StartWork = 1,
        /// <summary>
        /// 开始节点分流
        /// </summary>
        StartWorkFL = 2,
        /// <summary>
        /// 合流节点
        /// </summary>
        WorkHL = 3,
        /// <summary>
        /// 分流节点
        /// </summary>
        WorkFL = 4,
        /// <summary>
        /// 分合流
        /// </summary>
        WorkFHL = 5,
        /// <summary>
        /// 子流程
        /// </summary>
        SubThreadWork = 6
    }
    /// <summary>
    /// 抄送规则
    /// </summary>
    public enum CCRoleEnum
    {
        /// <summary>
        /// 不能抄送
        /// </summary>
        UnCC,
        /// <summary>
        /// 手工抄送
        /// </summary>
        HandCC,
        /// <summary>
        /// 自动抄送
        /// </summary>
        AutoCC,
        /// <summary>
        /// 手工与自动并存
        /// </summary>
        HandAndAuto,
        /// <summary>
        /// 按字段
        /// </summary>
        BySysCCEmps,
        /// <summary>
        /// 在发送前打开
        /// </summary>
        WhenSend
    }
    /// <summary>
    /// 谁执行它
    /// </summary>
    public enum WhoDoIt
    {
        /// <summary>
        /// 操作员
        /// </summary>
        Operator,
        /// <summary>
        /// 机器
        /// </summary>
        MachtionOnly,
        /// <summary>
        /// 混合
        /// </summary>
        Mux
    }
    /// <summary>
    /// 位置类型
    /// </summary>
    public enum NodePosType
    {
        /// <summary>
        /// 开始
        /// </summary>
        Start,
        /// <summary>
        /// 中间
        /// </summary>
        Mid,
        /// <summary>
        /// 结束
        /// </summary>
        End
    }
    /// <summary>
    /// 节点表单类型
    /// </summary>
    public enum NodeFormType
    {
        /// <summary>
        /// 傻瓜表单.
        /// </summary>
        FoolForm = 0,
         
        /// <summary>
        /// 嵌入式表单.
        /// </summary>
        SelfForm = 2,
        /// <summary>
        /// SDKForm
        /// </summary>
        SDKForm = 3,
        /// <summary>
        /// 表单树
        /// </summary>
        SheetTree = 5,
        /// <summary>
        /// 动态表单树
        /// </summary>
        SheetAutoTree = 6,
        /// <summary>
        /// 公文表单
        /// </summary>
        WebOffice = 7,
        /// <summary>
        /// Excel表单
        /// </summary>
        ExcelForm = 8,
        /// <summary>
        /// Word表单
        /// </summary>
        WordForm = 9,
        /// <summary>
        /// 累加表单
        /// </summary>
        FoolTruck = 10,
        /// <summary>
        /// 节点单表单
        /// </summary>
        RefOneFrmTree = 11,
        /// <summary>
        /// 开发者表单
        /// </summary>
        Develop = 12,
        /// <summary>
        /// 开发者表单
        /// </summary>
        ChapterFrm = 13,
        /// <summary>
        /// 引用指定节点上的表单
        /// </summary>
        RefNodeFrm = 14,
        /// <summary>
        /// 禁用(对多表单流程有效)
        /// </summary>
        DisableIt = 100,

    }
    /// <summary>
    /// 工作类型
    /// </summary>
    public enum WorkType
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Ordinary,
        /// <summary>
        /// 自动的
        /// </summary>
        Auto
    }
    /// <summary>
    /// 已读回执类型
    /// </summary>
    public enum ReadReceipts
    {
        /// <summary>
        /// 不回执
        /// </summary>
        None,
        /// <summary>
        /// 自动回执
        /// </summary>
        Auto,
        /// <summary>
        /// 由系统字段决定
        /// </summary>
        BySysField,
        /// <summary>
        /// 按开发者参数
        /// </summary>
        BySDKPara
    }
    /// <summary>
    /// 打印方式
    /// @0=不打印@1=打印网页@2=打印RTF模板
    /// </summary>
    public enum PrintDocEnable
    {
        /// <summary>
        /// 不打印
        /// </summary>
        None,
        /// <summary>
        /// 打印网页
        /// </summary>
        PrintHtml,
        /// <summary>
        /// 打印RTF模板
        /// </summary>
        PrintRTF,
        /// <summary>
        /// 打印word
        /// </summary>
        PrintWord
    }
    /// <summary>
    /// 考核规则
    /// </summary>
    public enum CHWay
    {
        /// <summary>
        /// 不考核
        /// </summary>
        None,
        /// <summary>
        /// 按照时效考核
        /// </summary>
        ByTime,
        /// <summary>
        /// 按照工作量考核
        /// </summary>
        ByWorkNum,
        /// <summary>
        /// 是否是考核质量点
        /// </summary>
        IsQuality
    }

    public enum GuestFlowRole
    {
        /// <summary>
        /// 不参与
        /// </summary>
        None,
        /// <summary>
        /// 开始节点参与
        /// </summary>
        StartNodeJoin,
        /// <summary>
        /// 中间节点参与
        /// </summary>
        MiddleNodeJoin
    }

    /// <summary>
    /// 工作提醒规则
    /// </summary>
    public enum CHAlertRole
    {
        /// <summary>
        /// 不提醒
        /// </summary>
        None,
        /// <summary>
        /// 一天一次
        /// </summary>
        OneDayOneTime,
        /// <summary>
        /// 一天两次
        /// </summary>
        OneDayTowTime
    }
    /// <summary>
    /// 工作提醒方式
    /// </summary>
    public enum CHAlertWay
    {
        /// <summary>
        /// 邮件
        /// </summary>
        ByEmail,
        /// <summary>
        /// 短消息
        /// </summary>
        BySMS,
        /// <summary>
        /// 即时通讯
        /// </summary>
        ByCCIM
    }

    /// <summary>
    /// 运行平台
    /// </summary>
    public enum Plant
    {
        CCFlow,
        JFlow
    }
    /// <summary>
    /// 周末休息类型
    /// </summary>
    public enum WeekResetType
    {
        /// <summary>
        /// 双休
        /// </summary>
        Double,
        /// <summary>
        /// 单休
        /// </summary>
        Single,
        /// <summary>
        /// 不
        /// </summary>
        None
    }
    /// <summary>
    /// 用户信息显示格式
    /// </summary>
    public enum UserInfoShowModel
    {
        /// <summary>
        /// 用户ID,用户名
        /// </summary>
        UserIDUserName = 0,
        /// <summary>
        /// 用户ID
        /// </summary>
        UserIDOnly = 1,
        /// <summary>
        /// 用户名
        /// </summary>
        UserNameOnly = 2
    }
}
