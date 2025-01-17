﻿using System;
using BP.DA;
using BP.En;


namespace BP.WF.Data
{
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCListExt : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 状态
        /// </summary>
        public CCSta HisSta
        {
            get
            {
                return (CCSta)this.GetValIntByKey(CCListAttr.Sta);
            }
            set
            {
                //@sly 这里去掉了业务逻辑.
                if (value == CCSta.Read)
                    this.ReadDT = DataType.CurrentDateTime;
                this.SetValByKey(CCListAttr.Sta, (int)value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                if (BP.Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 域
        /// </summary>
        public string Domain
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Domain);
            }
            set
            {
                this.SetValByKey(CCListAttr.Domain, value);
            }
        }
        /// <summary>
        /// 抄送给
        /// </summary>
        public string CCTo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CCTo);
            }
            set
            {
                this.SetValByKey(CCListAttr.CCTo, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 抄送给Name
        /// </summary>
        public string CCToName
        {
            get
            {
                string s = this.GetValStringByKey(CCListAttr.CCToName);
                if (DataType.IsNullOrEmpty(s))
                    s = this.CCTo;
                return s;
            }
            set
            {
                this.SetValByKey(CCListAttr.CCToName, value);
            }
        }
        /// <summary>
        /// 读取时间
        /// </summary>
        public string CDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.CDT, value);
            }
        }
        /// <summary>
        /// 抄送人所在的节点编号
        /// </summary>
        public int NodeIDCC
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.NodeIDCC);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeIDCC, value);
            }
        }

        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.WorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.WorkID, value);
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.FID);
            }
            set
            {
                this.SetValByKey(CCListAttr.FID, value);
            }
        }
        /// <summary>
        /// 父流程工作ID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程编号
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.PFlowNo, value);
            }
        }
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FlowName);
            }
            set
            {
                this.SetValByKey(CCListAttr.FlowName, value);
            }
        }
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.NodeName);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 抄送标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Title);
            }
            set
            {
                this.SetValByKey(CCListAttr.Title, value);
            }
        }
        /// <summary>
        /// 抄送内容
        /// </summary>
        public string Doc
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Doc);
            }
            set
            {
                this.SetValByKey(CCListAttr.Doc, value);
            }
        }
        public string DocHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(CCListAttr.Doc);
            }
        }
        /// <summary>
        /// 抄送对象
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.FlowNo, value);
            }
        }
        public string RecEmpNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RecEmpNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.RecEmpNo, value);
            }
        }
        /// <summary>
        /// 读取日期
        /// </summary>
        public string ReadDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.ReadDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.ReadDT, value);
            }
        }
        /// <summary>
        /// 写入日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.RDT, value);
            }
        }
        /// <summary>
        /// 是否加入待办列表
        /// </summary>
	    public bool InEmpWorks
        {
            get { return this.GetValBooleanByKey(CCListAttr.InEmpWorks); }
            set { this.SetValByKey(CCListAttr.InEmpWorks, value); }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// CCListExt
        /// </summary>
        public CCListExt()
        {
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

                Map map = new Map("WF_CCList", "抄送");

                map.AddMyPK(); //组合主键 WorkID+"_"+FK_Node+"_"+FK_Emp 
                map.AddTBInt(CCListAttr.WorkID, 0, "工作ID", false, true);
                map.AddTBInt(CCListAttr.NodeIDWork, 0, "节点", false, false);
                map.AddTBInt(CCListAttr.FID, 0, "FID", false, false);
                map.AddTBString(CCListAttr.FlowNo, null, "流程编号", false, false, 0, 5, 10, true);

                map.AddTBString(CCListAttr.Title, null, "标题", true, true, 0, 500, 10, true);
                map.AddDDLSysEnum(CCListAttr.Sta, 0, "状态", true, false, "CCSta", "@0=未读@1=已读@2=已回复@3=删除");

                map.AddTBString(CCListAttr.FlowName, null, "流程", true, true, 0, 200, 10, true);
                map.AddTBString(CCListAttr.NodeName, null, "节点", true, true, 0, 500, 10, true);
                map.AddTBString(CCListAttr.RecEmpNo, null, "抄送人", false, false, 0, 50, 10, false);
                map.AddTBString(CCListAttr.RecEmpName, null, "抄送人", true, false, 0, 50, 10, true);
                map.AddTBDateTime(CCListAttr.RDT, null, "抄送日期", true, true);

                map.AddTBString(CCListAttr.CCTo, null, "抄送给", false, false, 0, 50, 10, true);
                map.AddTBString(CCListAttr.CCToName, null, "抄送给(人员名称)", false, false, 0, 50, 10, true);

                map.AddTBString(CCListAttr.OrgNo, null, "组织", false, false, 0, 50, 10, true);
                map.AddTBDateTime(CCListAttr.CDT, null, "打开时间", true, true);
                map.AddTBDateTime(CCListAttr.ReadDT, null, "阅读时间", true, true);

                //add by zhoupeng  
                map.AddTBString(CCListAttr.Domain, null, "Domain", false, true, 0, 50, 10, true);
                map.AddTBString(CCListAttr.OrgNo, null, "OrgNo", false, true, 0, 50, 10, true);

                #region 查询条件.
                map.DTSearchLabel = "抄送日期";
                map.DTSearchKey = CCListAttr.RDT;
                map.DTSearchWay = BP.Sys.DTSearchWay.ByDate;

                map.AddSearchAttr(CCListAttr.Sta); //按状态.

                //增加隐藏条件.
                if (BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.Single
                    || BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.GroupInc)
                {
                    map.AddHidden(CCListAttr.CCTo, "=", "@WebUser.No");
                }
                else
                {
                    map.AddHidden(CCListAttr.OrgNo, "=", "@WebUser.OrgNo");
                    map.AddHidden(CCListAttr.CCTo, "=", "@WebUser.No");
                }
                #endregion 查询条件.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCListExts : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCListExt();
            }
        }
        /// <summary>
        /// 抄送
        /// </summary>
        public CCListExts() { }


        /// <summary>
        /// 查询出来所有的抄送信息
        /// </summary>
        /// <param name="fk_node"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        public CCListExts(int fk_node, Int64 workid, Int64 fid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCListAttr.NodeIDWork, fk_node);
            qo.addAnd();
            if (fid != 0)
                qo.AddWhereIn(CCListAttr.WorkID, "(" + workid + "," + fid + ")");
            else
                qo.AddWhere(CCListAttr.WorkID, workid);
            qo.DoQuery();
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCListExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCListExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCListExt> Tolist()
        {
            System.Collections.Generic.List<CCListExt> list = new System.Collections.Generic.List<CCListExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCListExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
