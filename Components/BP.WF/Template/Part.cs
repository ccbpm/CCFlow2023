﻿using BP.DA;
using BP.En;
using BP.Sys;
using BP.Tools;
using System;
/// <summary>
/// 先阶段主要用于流程属性中的时限规则
/// </summary>
namespace BP.WF.Template
{
    /// <summary>
    /// 配件类型
    /// </summary>
    public class PartType
    {
        /// <summary>
        /// 前置导航的父子流程关系
        /// </summary>
        public const string ParentSubFlowGuide = "ParentSubFlowGuide";
        /// <summary>
        /// 流程时限设置
        /// </summary>
        public const string DeadLineRole = "DeadLineRole";
    }
    /// <summary>
    /// 配件属性
    /// </summary>
    public class PartAttr : BP.En.EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 前置导航的父子流程关系
        /// </summary>
        public const string PartType = "PartType";
        /// <summary>
        /// 字段存储0
        /// </summary>
        public const string Tag0 = "Tag0";
        /// <summary>
        /// 字段存储1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 字段存储2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// 字段存储3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// 字段存储4
        /// </summary>
        public const string Tag4 = "Tag4";
        /// <summary>
        /// 字段存储5
        /// </summary>
        public const string Tag5 = "Tag5";
        /// <summary>
        /// 字段存储6
        /// </summary>
        public const string Tag6 = "Tag6";
        /// <summary>
        /// 字段存储7
        /// </summary>
        public const string Tag7 = "Tag7";
        /// <summary>
        /// 字段存储8
        /// </summary>
        public const string Tag8 = "Tag8";
        /// <summary>
        /// 字段存储9
        /// </summary>
        public const string Tag9 = "Tag9";
        #endregion
    }
    /// <summary>
    /// 配件.	 
    /// </summary>
    public class Part : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 配件的事务编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(PartAttr.FlowNo);
            }
            set
            {
                SetValByKey(PartAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string PartType
        {
            get
            {
                return this.GetValStringByKey(PartAttr.PartType);
            }
            set
            {
                SetValByKey(PartAttr.PartType, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(PartAttr.NodeID);
            }
            set
            {
                SetValByKey(PartAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 字段存储0
        /// </summary>
        public string Tag0
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag0);
            }
            set
            {
                SetValByKey(PartAttr.Tag0, value);
            }
        }
        /// <summary>
        /// 字段存储1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag1);
            }
            set
            {
                SetValByKey(PartAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 字段存储2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag2);
            }
            set
            {
                SetValByKey(PartAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 字段存储3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag3);
            }
            set
            {
                SetValByKey(PartAttr.Tag3, value);
            }
        }
        /// <summary>
        /// 字段存储4
        /// </summary>
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag4);
            }
            set
            {
                SetValByKey(PartAttr.Tag4, value);
            }
        }
        /// <summary>
        /// 字段存储5
        /// </summary>
        public string Tag5
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag5);
            }
            set
            {
                SetValByKey(PartAttr.Tag5, value);
            }
        }
        /// <summary>
        /// 字段存储6
        /// </summary>
        public string Tag6
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag6);
            }
            set
            {
                SetValByKey(PartAttr.Tag6, value);
            }
        }
        /// <summary>
        /// 字段存储7
        /// </summary>
        public string Tag7
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag7);
            }
            set
            {
                SetValByKey(PartAttr.Tag7, value);
            }
        }
        /// <summary>
        /// 字段存储8
        /// </summary>
        public string Tag8
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag8);
            }
            set
            {
                SetValByKey(PartAttr.Tag8, value);
            }
        }
        /// <summary>
        /// 字段存储9
        /// </summary>
        public string Tag9
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag9);
            }
            set
            {
                SetValByKey(PartAttr.Tag9, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 配件
        /// </summary>
        public Part() { }
        /// <summary>
        /// 配件
        /// </summary>
        /// <param name="mypk">配件ID</param>	
        public Part(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
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

                Map map = new Map("WF_Part", "配件");

                map.AddMyPK();

                map.AddTBString(PartAttr.FlowNo, null, "流程编号", false, true, 0, 5, 10);
                map.AddTBInt(PartAttr.NodeID, 0, "节点ID", false, false);
                map.AddTBString(PartAttr.PartType, null, "类型", false, true, 0, 100, 10);

                map.AddTBString(PartAttr.Tag0, null, "Tag0", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag1, null, "Tag1", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag2, null, "Tag2", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag3, null, "Tag3", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag4, null, "Tag4", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag5, null, "Tag5", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag6, null, "Tag6", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag7, null, "Tag7", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag8, null, "Tag8", false, true, 0, 200, 10);
                map.AddTBString(PartAttr.Tag9, null, "Tag9", false, true, 0, 200, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 执行测试.
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string DoTestARWebApi(string paras)
        {
            if (paras.Contains("@WorkID") == false || paras.Contains("@OID") == false)
                return "err@参数模式是表单全量模式，您没有传入workid参数.";

            //获得参数.
            AtPara ap = new AtPara(paras);
            int workID = 0;
            if (ap.HisHT.ContainsKey("OID") == true)
                workID = ap.GetValIntByKey("OID");
            else
                workID = ap.GetValIntByKey("WorkID");

            string url = this.Tag0; //url. 
            string urlUodel = this.Tag1; //模式. Post,Get
            string paraMode = this.Tag2; //参数模式. 0=自定义模式， 1=全量模式.
            string pdocs = this.Tag3; //参数内容.  对自定义模式有效.

            //处理url里的参数.
            foreach (string item in ap.HisHT.Keys)
                url = url.Replace("@" + item, ap.GetValStrByKey(item));

            //全量参数模式. 
            if (paraMode.Equals("1") == true)
            {
                GEEntity geEntity = new GEEntity("ND" + int.Parse(this.FlowNo) + "Rpt", workID);
                pdocs = geEntity.ToJson(false);
            }
            else
            {
                pdocs = pdocs.Replace("`", "\"");
                //自定义参数模式.
                pdocs = Glo.DealExp(pdocs, null);
                foreach (string item in ap.HisHT.Keys)
                    pdocs = pdocs.Replace("@" + item, ap.GetValStrByKey(item));

                if (pdocs.Contains("@") == true)
                    return "err@TestAPI参数不完整:" + pdocs;
            }

            //判断提交模式.
            if (urlUodel.ToLower().Equals("get") == true)
                return DataType.ReadURLContext(url, 9000); //返回字符串.

            try
            {
                string doc = PubGlo.HttpPostConnect(url, pdocs);
                return doc;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message+" - " +url;
            }
        }
        public string ARWebApi(string paras)
        {
            if (paras.Contains("@WorkID") == false || paras.Contains("@OID") == false)
                return "err@参数模式是表单全量模式，您没有传入workid参数.";

            //获得参数.
            AtPara ap = new AtPara(paras);
            int workID = 0;
            if (ap.HisHT.ContainsKey("OID") == true)
                workID = ap.GetValIntByKey("OID");
            else
                workID = ap.GetValIntByKey("WorkID");

            GEEntity geEntity = new GEEntity("ND" + int.Parse(this.FlowNo) + "Rpt", workID);

            string url = this.Tag0; //url. 
            url = Glo.DealExp(url, geEntity);

            string urlUodel = this.Tag1; //模式. Post,Get
            string paraMode = this.Tag2; //参数模式. 0=自定义模式， 1=全量模式.
            string pdocs = this.Tag3; //参数内容.  对自定义模式有效.

            //全量参数模式. 
            if (paraMode.Equals("1") == true)
            {
                pdocs = geEntity.ToJson(false);
            }
            else
            {
                pdocs = pdocs.Replace("~", "\"");
                pdocs = Glo.DealExp(pdocs, geEntity);
                if (pdocs.Contains("@") == true)
                    return "err@参数不完整:" + pdocs;
                pdocs = pdocs.Replace("'", "\"");
            }

            //判断提交模式.
            if (urlUodel.ToLower().Equals("get") == true)
                return DataType.ReadURLContext(url, 9000); //返回字符串.

            bool isJson = false;
            if (this.Tag4.Trim().Equals("1") == true)
                isJson = true;

            string doc = PubGlo.HttpPostConnect(url, pdocs,"POST", isJson);
            return doc;
        }
    }
    /// <summary>
    /// 配件s
    /// </summary>
    public class Parts : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Part();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 配件集合
        /// </summary>
        public Parts()
        {
        }
        /// <summary>
        /// 配件集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public Parts(string fk_flow)
        {
            this.Retrieve(PartAttr.FlowNo, fk_flow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Part> ToJavaList()
        {
            return (System.Collections.Generic.IList<Part>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Part> Tolist()
        {
            System.Collections.Generic.List<Part> list = new System.Collections.Generic.List<Part>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Part)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
