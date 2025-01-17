﻿using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Template.SFlow;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点表单组件
    /// </summary>
    public class FrmNodeComponent : Entity
    {
        #region 公共属性
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public FrmWorkCheckSta FWCSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeWorkCheckAttr.FWCSta, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCSta, (int)value);
            }
        }
        /// <summary>
        /// 节点属性.
        /// </summary>
        public string No
        {
            get
            {
                return "ND" + this.NodeID;
            }
            set
            {
                string nodeID = value.Replace("ND", "");
                this.NodeID = int.Parse(nodeID);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        #endregion 

        #region 构造方法
        /// <summary>
        /// 控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 节点表单组件
        /// </summary>
        public FrmNodeComponent()
        {
        }
        /// <summary>
        /// 节点表单组件
        /// </summary>
        /// <param name="no"></param>
        public FrmNodeComponent(string mapData)
        {
            string mapdata = mapData.Replace("ND", "");
            if (DataType.IsNumStr(mapdata) == false)
            {
                //  this.HisFrmNodeComponentSta = FrmNodeComponentSta.Disable;
                return;
            }

            try
            {
                this.NodeID = int.Parse(mapdata);
            }
            catch
            {
                return;
            }
            this.Retrieve();
        }
        /// <summary>
        /// 节点表单组件
        /// </summary>
        /// <param name="no"></param>
        public FrmNodeComponent(int nodeID)
        {
            this.NodeID = nodeID;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "节点表单组件");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddGroupAttr("基本属性");
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10);

                map.AddGroupAttr("审核组件");
                //审核组件.
                NodeWorkCheck fwc = new NodeWorkCheck();
                map.AddAttrs(fwc.EnMap.Attrs);

                //父子流程.
                map.AddGroupAttr("父子流程");
                FrmSubFlow subflow = new FrmSubFlow();
                map.AddAttrs(subflow.EnMap.Attrs);

                //轨迹组件.
                map.AddGroupAttr("轨迹组件");
                FrmTrack track = new FrmTrack();
                map.AddAttrs(track.EnMap.Attrs);

                //流转自定义组件.
                map.AddGroupAttr("流转自定义组件");
                FrmTransferCustom ftt = new FrmTransferCustom();
                map.AddAttrs(ftt.EnMap.Attrs);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 初始化GroupField
        /// </summary>
        public void InitGroupField()
        {
            GroupField gf = new GroupField();

            #region 审核组件.
            NodeWorkCheck fwc = new NodeWorkCheck(this.NodeID);
            fwc.Copy(this);

            if (fwc.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
            {
                gf.Delete(GroupFieldAttr.FrmID, this.No, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.FrmID, this.No, GroupFieldAttr.CtrlType, GroupCtrlType.FWC) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "审核组件";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 审核组件.

            #region 父子流程组件.
            FrmSubFlow subflow = new FrmSubFlow(this.NodeID);
            subflow.Copy(this);

            if (subflow.HisFrmSubFlowSta == FrmSubFlowSta.Disable)
            {
                gf.Delete(GroupFieldAttr.CtrlID, "SubFlow" + this.No);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlID, "SubFlow" + this.No) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlID = "SubFlow" + this.No;
                    gf.CtrlType = GroupCtrlType.SubFlow;
                    gf.Lab = "父子流程组件";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 父子流程组件.

            #region 处理轨迹组件.
            FrmTrack track = new FrmTrack(this.NodeID);
            track.Copy(this);
            if (track.FrmTrackSta == FrmTrackSta.Disable)
            {
                gf.Delete(GroupFieldAttr.CtrlID, "FrmTrack" + this.No);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlID, "FrmTrack" + this.No) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlID = "FrmTrack" + this.No;
                    gf.CtrlType = GroupCtrlType.Track;
                    gf.Lab = "轨迹";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 处理轨迹组件.


            #region 流转自定义组件.
            FrmTransferCustom ftc = new FrmTransferCustom(this.NodeID);
            ftc.Copy(this);

            if (ftc.FTCSta == FTCSta.Disable)
            {
                gf.Delete(GroupFieldAttr.CtrlID, "FrmFTC" + this.No);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlID, "FrmFTC" + this.No) == false)
                {
                    gf = new GroupField();
                    gf.FrmID = "ND" + this.NodeID;
                    gf.CtrlID = "FrmFTC" + this.No;
                    gf.CtrlType = GroupCtrlType.FTC;
                    gf.Lab = "流转自定义";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 流转自定义组件.

        }

        protected override bool beforeUpdate()
        {

            this.InitGroupField();

            return base.beforeUpdate();
        }
        #endregion

        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            fl.Update();

            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 节点表单组件s
    /// </summary>
    public class FrmNodeComponents : Entities
    {
        #region 构造
        /// <summary>
        /// 节点表单组件s
        /// </summary>
        public FrmNodeComponents()
        {
        }
        /// <summary>
        /// 节点表单组件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmNodeComponents(string fk_mapdata)
        {
            if (BP.Difference.SystemConfig.isDebug)
                this.Retrieve("No", fk_mapdata);
            else
                this.RetrieveFromCache("No", (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNodeComponent();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmNodeComponent> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmNodeComponent>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmNodeComponent> Tolist()
        {
            System.Collections.Generic.List<FrmNodeComponent> list = new System.Collections.Generic.List<FrmNodeComponent>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmNodeComponent)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
