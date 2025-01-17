﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.Base;

namespace BP.Sys
{
    /// <summary>
    /// 导入模式
    /// </summary>
    public enum ImpModel
    {
        /// <summary>
        /// 不执行导入
        /// </summary>
        None = 0,
        /// <summary>
        /// 表格模式
        /// </summary>
        Table = 1,
        /// <summary>
        /// 按照Excel文件模式
        /// </summary>
        ExcelFile = 2,
        /// <summary>
        /// 单据模式
        /// </summary>
        BillModel = 3
    }
    /// <summary>
    /// 明细
    /// </summary>
    public class MapDtlAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 行Idx
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string Model = "Model";
        /// <summary>
        /// 使用的版本
        /// </summary>
        public const string DtlVer = "DtlVer";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 别名
        /// </summary>
        public const string Alias = "Alias";
        /// <summary>
        /// PTable
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// DtlOpenType
        /// </summary>
        public const string DtlOpenType = "DtlOpenType";
        /// <summary>
        /// 行数量
        /// </summary>
        public const string RowsOfList = "RowsOfList";
        /// <summary>
        /// 是否显示合计
        /// </summary>
        public const string IsShowSum = "IsShowSum";
        /// <summary>
        /// 是否显示idx
        /// </summary>
        public const string IsShowIdx = "IsShowIdx";
        /// <summary>
        /// 是否允许copy数据
        /// </summary>
        public const string IsCopyNDData = "IsCopyNDData";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string IsReadonly = "IsReadonly";
        /// <summary>
        /// WhenOverSize
        /// </summary>
        public const string WhenOverSize = "WhenOverSize";
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDelete = "IsDelete";
        /// <summary>
        /// 是否可以插入
        /// </summary>
        public const string IsInsert = "IsInsert";
        /// <summary>
        /// 是否可以更新
        /// </summary>
        public const string IsUpdate = "IsUpdate";
        /// <summary>
        /// 是否可以批量更新
        /// </summary>
        public const string IsBatchUpdate = "IsBatchUpdate";
        /// <summary>
        /// 是否启用通过
        /// </summary>
        public const string IsEnablePass = "IsEnablePass";
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public const string IsHLDtl = "IsHLDtl";
        /// <summary>
        /// 是否是分流
        /// </summary>
        public const string IsFLDtl = "IsFLDtl";
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public const string IsShowTitle = "IsShowTitle";
        /// <summary>
        /// 列表显示格式
        /// </summary>
        public const string ListShowModel = "ListShowModel";
        /// <summary>
        /// 行数据显示格式
        /// </summary>
        public const string EditModel = "EditModel";
        /// <summary>
        /// 自定义url。
        /// </summary>
        public const string UrlDtl = "UrlDtl";
        /// <summary>
        /// 移动端显示方式
        /// </summary>
        public const string MobileShowModel = "MobileShowModel";
        /// <summary>
        /// 移动端列表展示时显示的字段
        /// </summary>
        public const string MobileShowField = "MobileShowField";
        /// <summary>
        /// 过滤的SQL 表达式.
        /// </summary>
        public const string FilterSQLExp = "FilterSQLExp";
        /// <summary>
        /// 排序表达式.
        /// </summary>
        public const string OrderBySQLExp = "OrderBySQLExp";
        /// <summary>
        /// 列自动计算表达式
        /// </summary>
        public const string ColAutoExp = "ColAutoExp";
        /// <summary>
        /// 显示列
        /// </summary>
        public const string ShowCols = "ShowCols";
        /// <summary>
        /// 是否可见
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// H高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string FrmW = "FrmW";
        /// <summary>
        /// 高度
        /// </summary>
        public const string FrmH = "FrmH";
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public const string IsEnableAthM = "IsEnableAthM";
        /// <summary>
        /// 多表头列
        /// </summary>
        //public const string MTR = "MTR";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupField = "GroupField";
        /// <summary>
        /// 关联主键
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        public const string IsEnableGroupField = "IsEnableGroupField";
        /// <summary>
        /// 节点(用于多表单的权限控制)
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 映射的实体事件类
        /// </summary>
        public const string FEBD = "FEBD";
        /// <summary>
        /// 导入模式.
        /// </summary>
        public const string ImpModel = "ImpModel";

        #region 参数属性.
        public const string IsEnableLink = "IsEnableLink";
        public const string LinkLabel = "LinkLabel";
        public const string ExcType = "ExcType";
        public const string LinkUrl = "LinkUrl";
        public const string LinkTarget = "LinkTarget";



        public const string IsEnableLink2 = "IsEnableLink2";
        public const string LinkLabel2 = "LinkLabel2";
        public const string ExcType2 = "ExcType2";
        public const string LinkUrl2 = "LinkUrl2";
        public const string LinkTarget2 = "LinkTarget2";

        /// <summary>
        /// 从表存盘方式(失去焦点自动存盘，手工存盘)
        /// </summary>
        public const string DtlSaveModel = "DtlSaveModel";
        /// <summary>
        /// 明细表追加模式
        /// </summary>
        public const string DtlAddRecModel = "DtlAddRecModel";
        #endregion 参数属性.

        #region 参数属性.
        /// <summary>
        /// 是否启用锁定
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 子线程处理人字段
        /// </summary>
        public const string SubThreadWorker = "SubThreadWorker";
        /// <summary>
        /// 子线程分组字段.
        /// </summary>
        public const string SubThreadGroupMark = "SubThreadGroupMark";
        #endregion 参数属性.

        #region 导入导出属性.
        /// <summary>
        /// 是否可以导入
        /// </summary>
        public const string IsImp = "IsImp";
        /// <summary>
        /// 是否可以导出
        /// </summary>
        public const string IsExp = "IsExp";
        /// <summary>
        /// 查询sql
        /// </summary>
        public const string ImpSQLSearch = "ImpSQLSearch";
        /// <summary>
        /// 选择sql
        /// </summary>
        public const string ImpSQLInit = "ImpSQLInit";
        /// <summary>
        /// 填充数据一行数据
        /// </summary>
        public const string ImpSQLFullOneRow = "ImpSQLFullOneRow";
        /// <summary>
        /// 列的中文名称
        /// </summary>
        public const string ImpSQLNames = "ImpSQLNames";
        /// <summary>
        /// 从表最小集合
        /// </summary>
        public const string NumOfDtl = "NumOfDtl";
        /// <summary>
        /// 是否拷贝第一条数据
        /// </summary>
        public const string IsCopyFirstData = "IsCopyFirstData";
        /// <summary>
        /// 行数据初始化字段
        /// </summary>
        public const string InitDBAttrs = "InitDBAttrs";
        #endregion
    }
    /// <summary>
    /// 明细
    /// </summary>
    public class MapDtl : EntityNoName
    {
        #region 导入导出属性.
        /// <summary>
        /// 关联主键
        /// </summary>
        public string RefPK
        {
            get
            {
                string str = this.GetValStrByKey(MapDtlAttr.RefPK);
                if (DataType.IsNullOrEmpty(str))
                    return "RefPK";
                return str;
            }
            set
            {
                this.SetValByKey(MapDtlAttr.RefPK, value);
            }
        }
        /// <summary>
        /// Rowid
        /// </summary>
        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(MapDtlAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.RowIdx, value);
            }
        }
        /// <summary>
        /// 是否可以导入
        /// </summary>
        public bool ItIsImp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsImp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsImp, value);
            }
        }
        /// <summary>
        /// 是否可以导出
        /// </summary>
        public bool ItIsExp
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsExp);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsExp, value);
            }
        }
        /// <summary>
        /// 执行的类
        /// </summary>
        public string FEBD
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.FEBD);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FEBD, value);
            }
        }
        /// <summary>
        /// 导入模式
        /// </summary>
        public int ImpModel
        {
            get
            {
                return this.GetValIntByKey(MapDtlAttr.ImpModel);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpModel, value);
            }
        }
        /// <summary>
        /// 查询sql
        /// </summary>
        public string ImpSQLInit
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLInit).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLInit, value);
            }
        }
        /// <summary>
        /// 搜索sql
        /// </summary>
        public string ImpSQLSearch
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLSearch).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLSearch, value);
            }
        }
        /// <summary>
        /// 填充数据
        /// </summary>
        public string ImpSQLFullOneRow
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.ImpSQLFullOneRow).Replace("~", "'");
            }
            set
            {
                this.SetValByKey(MapDtlAttr.ImpSQLFullOneRow, value);
            }
        }
        #endregion

        #region 基本设置
        /// <summary>
        /// 工作模式
        /// </summary>
        public DtlModel DtlModel
        {
            get
            {
                return (DtlModel)this.GetValIntByKey(MapDtlAttr.Model);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.Model, (int)value);
            }
        }
        /// <summary>
        /// 是否启用行锁定.
        /// </summary>
        public bool ItIsRowLock
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsRowLock, value);
            }
        }
        #endregion 基本设置

        #region 参数属性
        /// <summary>
        /// 记录增加模式
        /// </summary>
        public DtlAddRecModel DtlAddRecModel
        {
            get
            {
                return (DtlAddRecModel)this.GetParaInt(MapDtlAttr.DtlAddRecModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlAddRecModel, (int)value);
            }
        }
        /// <summary>
        /// 保存方式
        /// </summary>
        public DtlSaveModel DtlSaveModel
        {
            get
            {
                return (DtlSaveModel)this.GetParaInt(MapDtlAttr.DtlSaveModel);
            }
            set
            {
                this.SetPara(MapDtlAttr.DtlSaveModel, (int)value);
            }
        }
        /// <summary>
        /// 是否启用Link,在记录的右边.
        /// </summary>
        public bool ItIsEnableLink
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsEnableLink, false);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsEnableLink, value);
            }
        }
        public string LinkLabel
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.LinkLabel);
                if (DataType.IsNullOrEmpty(s))
                    return "详细";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.LinkLabel, value);
            }
        }
        public string LinkUrl
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.LinkUrl);
                if (DataType.IsNullOrEmpty(s))
                    return "http://ccport.org";
                return s;
            }
            set
            {
                this.SetValByKey(MapDtlAttr.LinkUrl, value);
                //string val = value;
                //val = val.Replace("@", "*");
                //this.SetPara(MapDtlAttr.LinkUrl, val);
            }
        }
        public string LinkTarget
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.LinkTarget);
                if (DataType.IsNullOrEmpty(s))
                    return "_blank";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.LinkTarget, value);
            }
        }
        /// <summary>
        /// 子线程处理人字段(用于分流节点的明细表分配子线程任务).
        /// </summary>
        public string SubThreadWorker
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadWorker);
                if (DataType.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadWorker, value);
            }
        }
        /// <summary>
        /// 子线程分组字段(用于分流节点的明细表分配子线程任务)
        /// </summary>
        public string SubThreadGroupMark
        {
            get
            {
                string s = this.GetParaString(MapDtlAttr.SubThreadGroupMark);
                if (DataType.IsNullOrEmpty(s))
                    return "";
                return s;
            }
            set
            {
                this.SetPara(MapDtlAttr.SubThreadGroupMark, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(MapDtlAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_Node, value);
            }
        }

        public string InitDBAttrs
        {
            get
            {
                return this.GetValStringByKey(MapDtlAttr.InitDBAttrs);
            }
        }
        #endregion 参数属性

        #region 外键属性
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrames MapFrames
        {
            get
            {
                MapFrames obj = this.GetRefObject("MapFrames") as MapFrames;
                if (obj == null)
                {
                    obj = new MapFrames(this.No);
                    this.SetRefObject("MapFrames", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 逻辑扩展
        /// </summary>
        public MapExts MapExts
        {
            get
            {
                MapExts obj = this.GetRefObject("MapExts") as MapExts;
                if (obj == null)
                {
                    obj = new MapExts(this.No);
                    this.SetRefObject("MapExts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 事件:
        /// 1.该事件与Node,Flow,MapDtl,MapData一样的算法.
        /// 2.如果一个业务逻辑有变化，其他的也要变化.
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                //判断内存是否有？.
                FrmEvents objs = this.GetRefObject("FrmEvents") as FrmEvents;
                if (objs != null)
                    return objs; //如果缓存有值，就直接返回.

                int count = this.GetParaInt("FrmEventsNum", -1);
                if (count == -1)
                {
                    objs = new FrmEvents();
                    objs.Retrieve(FrmEventAttr.FrmID, this.No);

                    this.SetPara("FrmEventsNum", objs.Count); //设置他的数量.
                    this.DirectUpdate();
                    this.SetRefObject("FrmEvents", objs);
                    return objs;
                }

                if (count == 0)
                {
                    objs = new FrmEvents();
                    this.SetRefObject("FrmEvents", objs);
                    return objs;
                }
                else
                {
                    objs = new FrmEvents();
                    objs.Retrieve(FrmEventAttr.FrmID, this.No);
                    this.SetPara("FrmEventsNum", objs.Count); //设置他的数量.
                    this.SetRefObject("FrmEvents", objs);
                }
                return objs;
            }
        }
        /// <summary>
        /// 从表
        /// </summary>
        public MapDtls MapDtls
        {
            get
            {
                MapDtls obj = this.GetRefObject("MapDtls") as MapDtls;
                if (obj == null)
                {
                    obj = new MapDtls(this.No);
                    this.SetRefObject("MapDtls", obj);
                }
                return obj;
            }
        }

        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachments FrmAttachments
        {
            get
            {
                FrmAttachments obj = this.GetRefObject("FrmAttachments") as FrmAttachments;
                if (obj == null)
                {
                    obj = new FrmAttachments(this.No);
                    this.SetRefObject("FrmAttachments", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAths FrmImgAths
        {
            get
            {
                FrmImgAths obj = this.GetRefObject("FrmImgAths") as FrmImgAths;
                if (obj == null)
                {
                    obj = new FrmImgAths(this.No);
                    this.SetRefObject("FrmImgAths", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 单选按钮
        /// </summary>
        public FrmRBs FrmRBs
        {
            get
            {
                FrmRBs obj = this.GetRefObject("FrmRBs") as FrmRBs;
                if (obj == null)
                {
                    obj = new FrmRBs(this.No);
                    this.SetRefObject("FrmRBs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public MapAttrs MapAttrs
        {
            get
            {
                MapAttrs obj = this.GetRefObject("MapAttrs") as MapAttrs;
                if (obj == null)
                {
                    obj = new MapAttrs(this.No);
                    this.SetRefObject("MapAttrs", obj);
                }
                return obj;
            }
        }
        #endregion

        #region 属性
        public GEDtls HisGEDtls_temp = null;
        public EditModel HisEditModel
        {
            get
            {
                return (EditModel)this.GetValIntByKey(MapDtlAttr.EditModel);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.EditModel, (int)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public WhenOverSize HisWhenOverSize
        {
            get
            {
                return (WhenOverSize)this.GetValIntByKey(MapDtlAttr.WhenOverSize);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.WhenOverSize, (int)value);
            }
        }
        /// <summary>
        /// 是否显示数量
        /// </summary>
        public bool ItIsShowSum
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowSum);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowSum, value);
            }
        }
        /// <summary>
        /// 是否显示Idx
        /// </summary>
        public bool ItIsShowIdx
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowIdx);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowIdx, value);
            }
        }
        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool ItIsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是合流汇总数据
        /// </summary>
        public bool ItIsHLDtl
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsHLDtl);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsHLDtl, value);
            }
        }
        /// <summary>
        /// 是否是分流
        /// </summary>
        public bool ItIsFLDtl
        {
            get
            {
                return this.GetParaBoolen(MapDtlAttr.IsFLDtl);
            }
            set
            {
                this.SetPara(MapDtlAttr.IsFLDtl, value);
            }
        }
        /// <summary>
        /// 是否只读？
        /// </summary>
        public bool ItIsReadonly
        {
            get
            {

                if (this.ItIsInsert == false && this.ItIsUpdate == false && this.ItIsDelete == false)
                    return true;

                return this.GetValBooleanByKey(MapDtlAttr.IsReadonly);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsReadonly, value);
            }
        }
        /// <summary>
        /// 是否可以删除？
        /// </summary>
        public bool ItIsDelete
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsDelete, value);
            }
        }
        /// <summary>
        /// 是否可以新增？
        /// </summary>
        public bool ItIsInsert
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsInsert);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsInsert, value);
            }
        }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool ItIsView
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsView);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsView, value);
            }
        }
        /// <summary>
        /// 是否可以更新？
        /// </summary>
        public bool ItIsUpdate
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsUpdate);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsUpdate, value);
            }
        }
        /// <summary>
        /// 是否启用多附件
        /// </summary>
        public bool ItIsEnableAthM
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableAthM);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableAthM, value);
            }
        }
        /// <summary>
        /// 是否启用分组字段
        /// </summary>
        public bool ItIsEnableGroupField
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnableGroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnableGroupField, value);
            }
        }
        /// <summary>
        /// 是否起用审核连接
        /// </summary>
        public bool ItIsEnablePass
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsEnablePass);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsEnablePass, value);
            }
        }

        /// <summary>
        /// 是否copy数据？
        /// </summary>
        public bool ItIsCopyNDData
        {
            get
            {
                return this.GetValBooleanByKey(MapDtlAttr.IsCopyNDData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.IsCopyNDData, value);
            }
        }


        public bool ItIsUse = false;
        /// <summary>
        /// 是否检查人员的权限
        /// </summary>
        public DtlOpenType DtlOpenType
        {
            get
            {
                return (DtlOpenType)this.GetValIntByKey(MapDtlAttr.DtlOpenType);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.DtlOpenType, (int)value);
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupField
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.GroupField);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.GroupField, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 从表的模式
        /// </summary>
        public int PTableModel
        {
            get
            {
                return this.GetParaInt("PTableModel");
            }
            set
            {
                this.SetPara("PTableModel", value);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int RowsOfList
        {
            get
            {
                //如果不可以插入，就让其返回0.
                if (this.ItIsInsert == false)
                    return 0;

                return this.GetValIntByKey(MapDtlAttr.RowsOfList);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.RowsOfList, value);
            }
        }
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.PTable);
                if (DataType.IsNullOrEmpty(s) == true)
                {

                    s = this.No;
                    if (s.Substring(0, 1).Equals("0"))
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
                else
                {
                    if (s.Substring(0, 1).Equals("0"))
                    {
                        return "T" + this.No;
                    }
                    else
                        return s;
                }
            }
            set
            {
                this.SetValByKey(MapDtlAttr.PTable, value);
            }
        }
        /// <summary>
        /// 过滤的SQL表达式.
        /// </summary>
        public string FilterSQLExp
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.FilterSQLExp);
                if (DataType.IsNullOrEmpty(s) == true)
                    return "";
                s = s.Replace("~", "'");
                return s.Trim();
            }
            set
            {
                this.SetValByKey(MapDtlAttr.FilterSQLExp, value);
            }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBySQLExp
        {
            get
            {
                string s = this.GetValStrByKey(MapDtlAttr.OrderBySQLExp);
                if (DataType.IsNullOrEmpty(s) == true)
                    return "";
                s = s.Replace("~", "'");
                return s.Trim();
            }
            set
            {
                this.SetValByKey(MapDtlAttr.OrderBySQLExp, value);
            }
        }
        /// <summary>
        /// 多表头
        /// </summary>
        //public string MTR
        //{
        //    get
        //    {
        //        string s = this.GetValStrByKey(MapDtlAttr.MTR);
        //        s = s.Replace("《", "<");
        //        s = s.Replace("》", ">");
        //        s = s.Replace("‘", "'");
        //        return s;
        //    }
        //    set
        //    {
        //        string s = value;
        //        s = s.Replace("<", "《");
        //        s = s.Replace(">", "》");
        //        s = s.Replace("'", "‘");
        //        this.SetValByKey(MapDtlAttr.MTR, value);
        //    }
        //}
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias
        {
            get
            {
                return this.GetValStrByKey(MapDtlAttr.Alias);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.Alias, value);
            }
        }
        #endregion

        #region 构造方法
        public Map GenerMap()
        {
            bool isdebug = BP.Difference.SystemConfig.isDebug;

            if (isdebug == false)
            {
                Map m = Cache.GetMap(this.No);
                if (m != null)
                    return m;
            }

            MapAttrs mapAttrs = this.MapAttrs;
            Map map = new Map(this.PTable, this.Name);

            Attrs attrs = new Attrs();
            foreach (MapAttr mapAttr in mapAttrs)
                map.AddAttr(mapAttr.HisAttr);

            Cache.SetMap(this.No, map);
            return map;
        }
        public GEDtl HisGEDtl
        {
            get
            {
                GEDtl dtl = new GEDtl(this.No);
                return dtl;
            }
        }
        public GEEntity GenerGEMainEntity(string mainPK)
        {
            GEEntity en = new GEEntity(this.FrmID, mainPK);
            return en;
        }
        /// <summary>
        /// 明细
        /// </summary>
        public MapDtl()
        {
        }
        public MapDtl(string no)
        {
            this.No = no;
            //   this.IsReadonly = 2;
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

                Map map = new Map("Sys_MapDtl", "明细");
                map.IndexField = MapDtlAttr.FK_MapData;

                map.AddTBStringPK(MapDtlAttr.No, null, "编号", true, false, 1, 100, 20);
                map.AddTBString(MapDtlAttr.Name, null, "描述", true, false, 1, 200, 20);
                map.AddTBString(MapDtlAttr.Alias, null, "别名", true, false, 1, 200, 20);
                map.AddTBString(MapDtlAttr.FK_MapData, null, "主表", true, false, 0, 100, 20);
                map.AddTBString(MapDtlAttr.PTable, null, "物理表", true, false, 0, 200, 20);

                map.AddTBString(MapDtlAttr.GroupField, null, "分组字段", true, false, 0, 300, 20);
                map.AddTBString(MapDtlAttr.RefPK, null, "关联的主键", true, false, 0, 100, 20);

                // 为明细表初始化事件类.
                map.AddTBString(MapDtlAttr.FEBD, null, "映射的事件实体类", true, false, 0, 100, 20);

                // @0=普通@1=固定行
                map.AddTBInt(MapDtlAttr.Model, 0, "工作模式", false, false);
                //map.AddDDLSysEnum(MapDtlAttr.Model, 0, "工作模式", true, true,
                //MapDtlAttr.Model, "@0=普通@1=固定行");

                map.AddTBInt(MapDtlAttr.DtlVer, 0, "使用版本", false, false);
                // map.AddDDLSysEnum(MapDtlAttr.DtlVer, 0, "使用版本", true, true, MapDtlAttr.DtlVer, "@0=2017传统版@1=2019EasyUI版本");


                map.AddTBInt(MapDtlAttr.RowsOfList, 0, "初始化行数", false, false);

                map.AddBoolean(MapDtlAttr.IsEnableGroupField, false, "是否启用分组字段", false, false);

                map.AddBoolean(MapDtlAttr.IsShowSum, true, "是否显示合计？", false, false);
                map.AddBoolean(MapDtlAttr.IsShowIdx, true, "是否显示序号？", false, false);
                map.AddBoolean(MapDtlAttr.IsCopyNDData, true, "是否允许Copy数据", false, false);
                map.AddBoolean(MapDtlAttr.IsHLDtl, false, "是否是合流汇总", false, false);

                map.AddBoolean(MapDtlAttr.IsReadonly, false, "是否只读？", false, false);
                map.AddBoolean(MapDtlAttr.IsShowTitle, true, "是否显示标题？", false, false);
                map.AddBoolean(MapDtlAttr.IsView, true, "是否可见", false, false);

                map.AddBoolean(MapDtlAttr.IsInsert, true, "是否可以插入行？", false, false);
                map.AddBoolean(MapDtlAttr.IsDelete, true, "是否可以删除行", false, false);
                map.AddBoolean(MapDtlAttr.IsUpdate, true, "是否可以更新？", false, false);
                map.AddBoolean(MapDtlAttr.IsBatchUpdate, true, "是否可以批量更新？", false, false);
                map.AddBoolean(MapDtlAttr.IsEnablePass, false, "是否启用通过审核功能?", false, false);
                map.AddBoolean(MapDtlAttr.IsEnableAthM, false, "是否启用多附件", false, false);

                map.AddBoolean(MapDtlAttr.IsCopyFirstData, false, "是否复制第一行数据？", false, false);
                map.AddTBString(MapDtlAttr.InitDBAttrs, null, "行初始化字段", true, false, 0, 40, 20, false);
                // 超出行数
                map.AddTBInt(MapDtlAttr.WhenOverSize, 0, "列表数据显示格式", false, false);

                //数据开放类型 .
                map.AddTBInt(MapDtlAttr.DtlOpenType, 1, "数据开放类型", false, false);

                map.AddTBInt(MapDtlAttr.ListShowModel, 0, "列表数据显示格式", false, false);
                map.AddTBInt(MapDtlAttr.EditModel, 0, "行数据显示格式", false, false);
                map.AddTBString(MapDtlAttr.UrlDtl, null, "自定义Url", true, false, 0, 200, 20, true);

                map.AddTBString(MapDtlAttr.ColAutoExp, null, "列字段计算", true, false, 0, 200, 20, true);
                map.AddTBInt(MapDtlAttr.MobileShowModel, 0, "移动端数据显示格式", false, false);
                map.AddTBString(MapDtlAttr.MobileShowField, null, "移动端列表显示字段", true, false, 0, 100, 20);

                map.AddTBFloat(MapDtlAttr.H, 150, "高度", true, false);

                map.AddTBFloat(MapDtlAttr.FrmW, 900, "表单宽度", true, true);
                map.AddTBFloat(MapDtlAttr.FrmH, 1200, "表单高度", true, true);
                map.AddTBInt(MapDtlAttr.NumOfDtl, 0, "最小从表集合", true, false);
                //MTR 多表头列.
                //map.AddTBString(MapDtlAttr.MTR, null, "多表头列", true, false, 0, 3000, 20);
                #region 超链接.
                map.AddBoolean(MapDtlAttr.IsEnableLink, false, "是否启用超链接", true, true);
                map.AddTBString(MapDtlAttr.LinkLabel, "", "超连接标签", true, false, 0, 50, 100);
                map.AddTBString(MapDtlAttr.LinkTarget, null, "连接目标", true, false, 0, 10, 100);
                map.AddTBString(MapDtlAttr.LinkUrl, null, "连接URL", true, false, 0, 200, 200, true);
                #endregion 超链接.

                //SQL过滤表达式.
                map.AddTBString(MapDtlAttr.FilterSQLExp, null, "过滤SQL表达式", true, false, 0, 70, 20, true);
                map.AddTBString(MapDtlAttr.OrderBySQLExp, null, "排序字段", true, false, 0, 70, 20, true);

                //add 2014-02-21.
                map.AddTBInt(MapDtlAttr.FK_Node, 0, "节点(用户独立表单权限控制)", false, false);

                //要显示的列.
                map.AddTBString(MapDtlAttr.ShowCols, null, "显示的列", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDtlAttr.ShowCols, "默认为空,全部显示,如果配置了就按照配置的计算,格式为:field1,field2");

                map.AddTBString("Btns", null, "头部按钮", true, false, 0, 200, 20, true);
                #region 导入导出填充.
                // 2014-07-17 for xinchang bank.
                map.AddBoolean(MapDtlAttr.IsExp, true, "IsExp", false, false);
                map.AddTBInt(MapDtlAttr.ImpModel, 0, "导入规则", false, false);

                // map.AddBoolean(MapDtlAttr.IsImp, true, "IsImp", false, false);
                // map.AddBoolean(MapDtlAttr.IsEnableSelectImp, false, "是否启用选择数据导入?", false, false);
                map.AddTBString(MapDtlAttr.ImpSQLSearch, null, "查询SQL", true, false, 0, 500, 20);
                map.AddTBString(MapDtlAttr.ImpSQLInit, null, "初始化SQL", true, false, 0, 500, 20);
                map.AddTBString(MapDtlAttr.ImpSQLFullOneRow, null, "数据填充SQL", true, false, 0, 500, 20);
                map.AddTBString(MapDtlAttr.ImpSQLNames, null, "字段中文名", true, false, 0, 900, 20);
                map.AddBoolean(MapDtlAttr.IsImp, false, "IsImp", true, true);
                #endregion 导入导出填充.

                map.AddTBString(MapDtlAttr.GUID, null, "GUID", false, false, 0, 128, 20);

                //参数.
                map.AddTBAtParas(300);

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 基本属性.


        public float H
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.H);
            }
            set
            {
                this.SetValByKey(MapDtlAttr.H, value);
            }
        }
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmW);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDtlAttr.FrmH);
            }
        }
        #endregion 基本属性.

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <param name="fk_val"></param>
        /// <returns></returns>
        public int GetCountByFK(int workID)
        {
            return DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE WorkID=" + workID);
        }
        public int GetCountByFK(string field, string val)
        {
            return DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "='" + val + "'");
        }
        public int GetCountByFK(string field, Int64 val)
        {
            return DBAccess.RunSQLReturnValInt("select COUNT(OID) from " + this.PTable + " WHERE " + field + "=" + val);
        }
        public int GetCountByFK(string f1, Int64 val1, string f2, string val2)
        {
            return DBAccess.RunSQLReturnValInt("SELECT COUNT(OID) from " + this.PTable + " WHERE " + f1 + "=" + val1 + " AND " + f2 + "='" + val2 + "'");
        }
        #endregion

        public void IntMapAttrs()
        {
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = this.No;
            if (md.RetrieveFromDBSources() == 0)
            {
                //@hongyan. 
                md.Name = this.Name;
              //  md.PTable = this.PTable;
                md.DirectInsert();
            }

            MapAttrs attrs = new MapAttrs(this.No);
            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (attrs.Contains(MapAttrAttr.KeyOfEn, "OID") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.setEditType(EditType.Readonly);

                attr.setKeyOfEn("OID");
                attr.setName("主键");
                attr.setMyDataType(DataType.AppInt);

                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "RefPK") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.setEditType(EditType.Readonly);

                attr.setKeyOfEn("RefPK");
                attr.setName("关联ID");
                attr.setMyDataType(DataType.AppString);

                attr.setMyDataType(DataType.AppString);

                attr.setMyDataType(DataType.AppString);

                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "FID") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.setEditType(EditType.Readonly);

                attr.setKeyOfEn("FID");
                attr.setName("FID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "RDT") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.HisEditType = EditType.UnDel;

                attr.setKeyOfEn("RDT");
                attr.setName("记录时间");
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "Rec") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.setEditType(EditType.Readonly);

                attr.setKeyOfEn("Rec");
                attr.setName("记录人");
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMaxLen(20);
                attr.setMinLen(0);
                attr.DefVal = "@WebUser.No";
                attr.Tag = "@WebUser.No";
                attr.Insert();
            }
            if (attrs.Contains(MapAttrAttr.KeyOfEn, "Idx") == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FrmID =this.No;
                attr.setEditType(EditType.Readonly);

                attr.setKeyOfEn("Idx");
                attr.setName("Idx");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.Insert();
            }

        }
        private void InitExtMembers()
        {
            /* 如果启用了多附件*/
            if (this.ItIsEnableAthM == true)
            {
                BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
                athDesc.setMyPK(this.No + "_AthMDtl");
                if (athDesc.RetrieveFromDBSources() == 0)
                {
                    athDesc.FrmID =this.No;
                    athDesc.NoOfObj = "AthMDtl";
                    athDesc.Name = this.Name;
                    athDesc.DirectInsert();
                    //增加分组
                    GroupField group = new GroupField();
                    group.Lab = athDesc.Name;
                    group.FrmID = this.No;
                    group.CtrlType = "Ath";
                    group.CtrlID = athDesc.MyPK;
                    group.Idx = 10;
                    group.Insert();
                }
            }

        }

        public string ChangeMapAttrIdx(string mypkArr)
        {
            string[] mypks = mypkArr.Split(',');
            for (int i = 0; i < mypks.Length; i++)
            {
                string mypk = mypks[i];
                if (mypk == null || mypk.Equals(""))
                    continue;

                string sql = "UPDATE Sys_MapAttr SET Idx=" + i + " WHERE MyPK='" + mypk + "' ";
                DBAccess.RunSQL(sql);
            }
            return "移动成功..";
        }

        protected override bool beforeInsert()
        {
            //在属性实体集合插入前，clear父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FrmID);


            GroupField gf = new GroupField();
            if (gf.IsExit(GroupFieldAttr.CtrlID, this.No) == false)
            {
                gf.FrmID = this.FrmID;
                gf.CtrlID = this.No;
                gf.CtrlType = "Dtl";
                gf.Lab = this.Name;
                gf.Idx = 0;
                gf.Insert(); //插入.
            }

            if (DataType.IsNullOrEmpty(this.PTable) == true
                && this.No.Contains("ND") == true)
            {
                if (this.No.Contains("01Dtl") == false)
                {
                    string ptable = this.No.Substring(0, this.No.IndexOf("01Dtl")) + this.No.Substring(this.No.IndexOf("01Dtl"));
                    this.PTable = ptable;
                }
            }


            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.InitExtMembers();

            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = this.No;
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = this.Name;
                md.Insert();
            }

            if (this.ItIsRowLock == true)
            {
                /*检查是否启用了行锁定.*/
                MapAttrs attrs = new MapAttrs(this.No);
                if (attrs.Contains(MapAttrAttr.KeyOfEn, "IsRowLock") == false)
                    throw new Exception("您启用了从表单(" + this.Name + ")行数据锁定功能，但是该从表里没IsRowLock字段，请参考帮助文档。");
            }

            if (this.ItIsEnablePass)
            {
                /*判断是否有IsPass 字段。*/
                MapAttrs attrs = new MapAttrs(this.No);
                if (attrs.Contains(MapAttrAttr.KeyOfEn, "IsPass") == false)
                    throw new Exception("您启用了从表单(" + this.Name + ")条数据审核选项，但是该从表里没IsPass字段，请参考帮助文档。");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeUpdate()
        {
            if (this.No.Equals(this.FrmID) == true)
                throw new Exception("err@从表的No不能与FK_MapData字段相等." + this.No);

            int listShowModel = this.GetValIntByKey(MapDtlAttr.ListShowModel);
            //判断二维、三维是否增加了IsEnable的字段
            bool isExist = false;
            MapAttrs mattrs = new MapAttrs(this.No);
            foreach (MapAttr attr in mattrs)
            {
                if (attr.KeyOfEn.Equals("IsEnable"))
                {
                    isExist = true;
                    break;
                }
            }
            if(isExist == false && (listShowModel == 3 || listShowModel == 4 || listShowModel == 5))
            {
                MapAttr attr = mattrs[0] as MapAttr;
                attr.MyPK = attr.FrmID + "_IsEnable";
                attr.setKeyOfEn("IsEnable");
                attr.Name = "是否可用";
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.CheckBok;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.Insert();

            }
            this.InitExtMembers();

            //更新MapData中的名称
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = this.No;

            //获得事件实体.
            FormEventBaseDtl febd = BP.Sys.Base.Glo.GetFormDtlEventBaseByEnName(this.No);
            if (febd == null)
                this.FEBD = "";
            else
                this.FEBD = febd.ToString();

            if (this.PTable.Length == 0)
                this.PTable = this.No;

            if (md.RetrieveFromDBSources() == 1)
            {
                md.Name = this.Name;
                md.PTable = this.PTable;
                //避免在表单库中显示
                md.FormTreeNo = "";
                md.Update(); //需要更新到缓存.
            }

            return base.beforeUpdate();
        }
        protected override bool beforeDelete()
        {
            string sql = "";
            sql += "@DELETE FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmImgAth WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmRB WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_MapFrame WHERE FK_MapData='" + this.No + "'";

            if (this.No.Contains("BP.") == false)
                sql += "@DELETE FROM Sys_MapExt WHERE FK_MapData='" + this.No + "'";

            sql += "@DELETE FROM Sys_MapAttr WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_MapData WHERE No='" + this.No + "'";
            sql += "@DELETE FROM Sys_GroupField WHERE FrmID='" + this.No + "'";
            sql += "@DELETE FROM Sys_GroupField WHERE CtrlID='" + this.No + "'";
            DBAccess.RunSQLs(sql);

            if (DBAccess.IsExitsObject(this.PTable) && this.PTable.IndexOf("ND") == 0)
            {
                //如果其他表单引用了该表，就不能删除它. 
                sql = "SELECT COUNT(No) AS NUM  FROM Sys_MapData WHERE PTable='" + this.PTable + "' OR ( PTable='' AND No='" + this.PTable + "')";
                int i = DBAccess.RunSQLReturnValInt(sql, 0);

                sql = "SELECT COUNT(No) AS NUM  FROM Sys_MapDtl WHERE PTable='" + this.PTable + "' OR ( PTable='' AND No='" + this.PTable + "')";
                i += DBAccess.RunSQLReturnValInt(sql, 0);
                if (i >= 1)
                {
                    /* 说明有多个表单在引用.就不删除物理*/
                }
                else
                {
                    // edit by zhoupeng 误删已经有数据的表. 
                    if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + this.PTable + " WHERE 1=1 ") == 0)
                        DBAccess.RunSQL("DROP TABLE " + this.PTable);
                }
            }

            //执行清空缓存到的AutoNum.
            MapData md = new MapData(this.FrmID);
            md.ClearAutoNumCache(true); //更新缓存.
            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 明细s
    /// </summary>
    public class MapDtls : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 明细s
        /// </summary>
        public MapDtls()
        {
        }
        /// <summary>
        /// 明细s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapDtls(string fk_mapdata)
        {
            if (fk_mapdata == null)
                throw new Exception("fk_mapdata 传的值为空,不能查询.");

            //zhoupeng 注销掉，为了这样多的过滤条件？
            // this.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata, MapDtlAttr.FK_Node, 0, MapDtlAttr.No);
            this.Retrieve(MapDtlAttr.FK_MapData, fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDtl();
            }
        }

        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDtl> Tolist()
        {
            System.Collections.Generic.List<MapDtl> list = new System.Collections.Generic.List<MapDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
