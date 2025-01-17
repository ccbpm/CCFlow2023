﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 抄送部门属性	  
	/// </summary>
	public class CCDeptAttr
	{
		/// <summary>
		/// 节点
		/// </summary>
		public const string FK_Node="FK_Node";
		/// <summary>
		/// 工作部门
		/// </summary>
		public const string FK_Dept="FK_Dept";
	}
	/// <summary>
	/// 抄送部门
	/// 节点的工作部门有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class CCDept :EntityMM
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
		public int  FK_Node
		{
			get
			{
				return this.GetValIntByKey(CCDeptAttr.FK_Node);
			}
			set
			{
				this.SetValByKey(CCDeptAttr.FK_Node,value);
			}
		}
		/// <summary>
		/// 工作部门
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(CCDeptAttr.FK_Dept);
			}
			set
			{
				this.SetValByKey(CCDeptAttr.FK_Dept,value);
			}
		}
		#endregion 

		#region 构造方法
		/// <summary>
		/// 抄送部门
		/// </summary>
		public CCDept(){}
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;

                Map map = new Map("WF_CCDept", "抄送部门");				 

				map.AddDDLEntitiesPK(CCDeptAttr.FK_Node,0,DataType.AppInt,"节点",new Nodes(),NodeAttr.NodeID,NodeAttr.Name,true);
				map.AddDDLEntitiesPK( CCDeptAttr.FK_Dept,null,"部门",new BP.Port.Depts(),true);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion
	}
	/// <summary>
	/// 抄送部门
	/// </summary>
    public class CCDepts : EntitiesMM
    {
      
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (CCDept ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        /// <summary>
        /// 抄送部门
        /// </summary>
        public CCDepts() { }
        /// <summary>
        /// 抄送部门
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public CCDepts(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCDeptAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 抄送部门
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public CCDepts(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCDeptAttr.FK_Dept, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCDept();
            }
        }
        /// <summary>
        /// 取到一个工作部门集合能够访问到的节点s
        /// </summary>
        /// <param name="sts">工作部门集合</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Stations sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Station st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
       
        /// <summary>
        /// 工作部门对应的节点
        /// </summary>
        /// <param name="stationNo">工作部门编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string stationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCDeptAttr.FK_Dept, stationNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (CCDept en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCDept> Tolist()
        {
            System.Collections.Generic.List<CCDept> list = new System.Collections.Generic.List<CCDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
