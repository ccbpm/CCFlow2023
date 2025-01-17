﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 表单对应组织属性
    /// </summary>
    public class FrmOrgAttr
    {
        /// <summary>
        /// 表单
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 表单对应组织
    /// </summary>
    public class FrmOrg : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        ///表单
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmOrgAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmOrgAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 组织
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(FrmOrgAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FrmOrgAttr.OrgNo, value);
            }
        }
        public string OrgNoT
        {
            get
            {
                return this.GetValRefTextByKey(FrmOrgAttr.OrgNo);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单对应组织
        /// </summary>
        public FrmOrg()
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

                Map map = new Map("WF_FrmOrg", "表单对应组织");
                map.IndexField = FrmOrgAttr.FrmID;

                map.AddMyPK(false);

                map.AddTBString(FrmOrgAttr.FrmID, null, "表单", true, true, 1, 100, 100);
                map.AddTBString(FlowOrgAttr.OrgNo, null, "到组织", true, true, 1, 150, 150);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 表单对应组织
    /// </summary>
    public class FrmOrgs : EntitiesMyPK
    {
        /// <summary>
        /// 表单对应组织
        /// </summary>
        public FrmOrgs() { }
        /// <summary>
        /// 表单对应组织
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public FrmOrgs(string orgNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmOrgAttr.OrgNo, orgNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmOrg();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmOrg> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmOrg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmOrg> Tolist()
        {
            System.Collections.Generic.List<FrmOrg> list = new System.Collections.Generic.List<FrmOrg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmOrg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
