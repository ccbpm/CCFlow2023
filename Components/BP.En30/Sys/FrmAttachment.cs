﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachmentAttr : EntityMyPKAttr
    {
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 运行模式
        /// </summary>
        public const string AthRunModel = "AthRunModel";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 要求上传的格式
        /// </summary>
        public const string Exts = "Exts";
        /// <summary>
        /// 附件编号
        /// </summary>
        public const string NoOfObj = "NoOfObj";
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public const string IsUpload = "IsUpload";
        /// <summary>
        /// 是否是合流汇总
        /// </summary>
        public const string IsHeLiuHuiZong = "IsHeLiuHuiZong";
        /// <summary>
        /// 是否汇总到合流节点上去？
        /// </summary>
        public const string IsToHeLiuHZ = "IsToHeLiuHZ";
        /// <summary>
        /// 是否增加
        /// </summary>
        public const string IsNote = "IsNote";
        /// <summary>
        /// 是否启用扩展列
        /// </summary>
        public const string IsExpCol = "IsExpCol";
        /// <summary>
        /// 是否显示标题列
        /// </summary>
        public const string IsShowTitle = "IsShowTitle";
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public const string IsDownload = "IsDownload";
        /// <summary>
        /// 是否可以排序
        /// </summary>
        public const string IsOrder11 = "IsOrder";
        /// <summary>
        /// 数据存储方式
        /// </summary>
        public const string AthSaveWay = "AthSaveWay";
        /// <summary>
        /// 单附件模板使用规则
        /// </summary>
        public const string AthSingleRole = "AthSingleRole";
        /// <summary>
        /// 单附件编辑模式
        /// </summary>
        public const string AthEditModel = "AthEditModel";
        /// <summary>
        /// 是否排序？
        /// </summary>
        public const string IsIdx = "IsIdx";
        /// <summary>
        /// 是否要转换成html，方便在线浏览.
        /// </summary>
        public const string IsTurn2Html = "IsTurn2Html";
        /// <summary>
        /// 类别
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 上传类型
        /// </summary>
        public const string UploadType = "UploadType";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// RowIdx
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public const string IsAutoSize = "IsAutoSize";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 数据控制方式(对父子流程有效果)
        /// </summary>
        public const string CtrlWay = "CtrlWay";
        /// <summary>
        /// 上传方式(对父子流程有效果)
        /// </summary>
        public const string AthUploadWay = "AthUploadWay";
        /// <summary>
        /// 文件展现方式
        /// </summary>
        public const string FileShowWay = "FileShowWay";
        /// <summary>
        /// 上传方式
        /// 0，批量上传。
        /// 1，单个上传。
        /// </summary>
        public const string UploadCtrl = "UploadCtrl";
        /// <summary>
        /// 上传校验
        /// 0=不校验.
        /// 1=不能为空.
        /// 2=每个类别下不能为空.
        /// </summary>
        public const string UploadFileNumCheck = "UploadFileNumCheck";
        /// <summary>
        /// 上传最小数量
        /// </summary>
        public const string NumOfUpload = "NumOfUpload";
        /// <summary>
        /// 上传最大数量
        /// </summary>
        public const string TopNumOfUpload = "TopNumOfUpload";
        /// <summary>
        /// 附件最大限制
        /// </summary>
        public const string FileMaxSize = "FileMaxSize";
        /// <summary>
        /// 是否可见？
        /// </summary>
        public const string IsVisable = "IsVisable";
        /// <summary>
        /// 附件类型 0 普通附件 1 图片附件
        /// </summary>
        public const string FileType = "FileType";
        /// <summary>
        /// 移动端图片附件上传的方式
        /// </summary>
        public const string PicUploadType = "PicUploadType";
        /// <summary>
        /// 是否启用模板？
        /// </summary>
        public const string IsEnableTemplate = "IsEnableTemplate";
        /// <summary>
        /// 附件删除方式
        /// </summary>
        public const string DeleteWay = "DeleteWay";

        #region 数据引用.
        /// <summary>
        /// 数据引用
        /// </summary>
        public const string DataRefNoOfObj = "DataRefNoOfObj";
        /// <summary>
        /// 阅读规则
        /// </summary>
        public const string ReadRole = "ReadRole";
        #endregion 数据引用.

        #region 快捷键.
        /// <summary>
        /// 是否启用快捷键
        /// </summary>
        public const string FastKeyIsEnable = "FastKeyIsEnable";
        /// <summary>
        /// 快捷键生成规则
        /// </summary>
        public const string FastKeyGenerRole = "FastKeyGenerRole";
        #endregion
    }
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachment : EntityMyPK
    {
        #region 参数属性.
        /// <summary>
        /// 是否可见？
        /// </summary>
        public bool ItIsVisable
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsVisable, true);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsVisable, value);
            }
        }
        public int DeleteWay
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.DeleteWay, 0);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DeleteWay, value);
            }
        }
        /// <summary>
        /// 使用上传附件的 - 控件类型
        /// 0=批量.
        /// 1=单个。
        /// </summary>
        public int UploadCtrl
        {
            get
            {
                return this.GetParaInt(FrmAttachmentAttr.UploadCtrl);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadCtrl, value);
            }
        }

        /// <summary>
        /// 最低上传数量
        /// </summary>
        public int NumOfUpload
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.NumOfUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.NumOfUpload, value);
            }
        }
        /// <summary>
        /// 最大上传数量
        /// </summary>
        public int TopNumOfUpload
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.TopNumOfUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.TopNumOfUpload, value);
            }
        }
        /// <summary>
        /// 附件最大限制
        /// </summary>
        public int FileMaxSize
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FileMaxSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FileMaxSize, value);
            }
        }
        /// <summary>
        /// 上传校验
        /// 0=不校验.
        /// 1=不能为空.
        /// 2=每个类别下不能为空.
        /// </summary>
        public UploadFileNumCheck UploadFileNumCheck
        {
            get
            {
                return (UploadFileNumCheck)this.GetValIntByKey(FrmAttachmentAttr.UploadFileNumCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadFileNumCheck, (int)value);
            }
        }
        /// <summary>
        /// 保存方式
        /// 0 =文件方式保存。
        /// 1 = 保存到数据库.
        /// 2 = ftp服务器.
        /// </summary>
        public AthSaveWay AthSaveWay
        {
            get
            {
                return (AthSaveWay)this.GetValIntByKey(FrmAttachmentAttr.AthSaveWay);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.AthSaveWay, (int)value);
            }
        }
        #endregion 参数属性.

        #region 属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 运行模式？
        /// </summary>
        public AthRunModel AthRunModel
        {
            get
            {
                return (AthRunModel)this.GetValIntByKey(FrmAttachmentAttr.AthRunModel);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.AthRunModel, (int)value);
            }
        }
        /// <summary>
        /// 上传类型（单个的，多个，指定的）
        /// </summary>
        public AttachmentUploadType UploadType
        {
            get
            {
                return (AttachmentUploadType)this.GetValIntByKey(FrmAttachmentAttr.UploadType);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.UploadType, (int)value);
            }
        }
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public bool ItIsUpload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsUpload, value);
            }
        }
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public bool ItIsDownload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsDownload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsDownload, value);
            }
        }

        /// <summary>
        /// 附件删除方式
        /// </summary>
        public AthDeleteWay HisDeleteWay
        {
            get
            {
                return (AthDeleteWay)this.GetValIntByKey(FrmAttachmentAttr.DeleteWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DeleteWay, (int)value);
            }
        }
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public bool ItIsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// IsShowTitle
        /// </summary>
        public bool ItIsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 备注列
        /// </summary>
        public bool ItIsNote
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsNote, value);
            }
        }

        /// <summary>
        /// 是否启用扩张列
        /// </summary>
        public bool ItIsExpCol
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsExpCol);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsExpCol, value);
            }
        }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentAttr.Name);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "未命名";
                return str;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Name, value);
            }
        }
        public void setName(string val)
        {
            this.SetValByKey(FrmAttachmentAttr.Name, val);
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Sort, value);
            }
        }
        /// <summary>
        /// 要求的格式
        /// </summary>
        public string Exts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Exts);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Exts, value);
            }
        }

        public int FileType
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FileType);
            }
        }
        /// <summary>
        /// 保存到
        /// </summary>
        public string SaveTo
        {
            get
            {
                if (this.AthSaveWay == BP.Sys.AthSaveWay.IISServer)
                {
                    return BP.Difference.SystemConfig.PathOfDataUser + @"/UploadFile/" + this.FrmID + "/";
                }

                if (this.AthSaveWay == BP.Sys.AthSaveWay.FTPServer)
                {
                    return @"//" + this.FrmID + "//";
                }

                return this.FrmID;
            }
        }
        /// <summary>
        /// 数据关联组件ID
        /// </summary>
        public string DataRefNoOfObj
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentAttr.DataRefNoOfObj);
                if (str.Equals(""))
                    str = this.NoOfObj;
                return str;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DataRefNoOfObj, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.NoOfObj);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.NoOfObj, value);
            }
        }


        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.H);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.H, value);
            }
        }
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.GroupID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.GroupID, value);
            }
        }
        /// <summary>
        /// 阅读规则:@0=不控制@1=未阅读阻止发送@2=未阅读做记录
        /// </summary>
        public int ReadRole
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.ReadRole);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.ReadRole, value);
            }
        }


        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.RowIdx, value);
            }
        }
        /// <summary>
        /// 数据控制方式
        /// </summary>
        public AthCtrlWay HisCtrlWay
        {
            get
            {
                return (AthCtrlWay)this.GetValIntByKey(FrmAttachmentAttr.CtrlWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.CtrlWay, (int)value);
            }
        }
        /// <summary>
        /// 是否是合流汇总多附件？
        /// </summary>
        public bool ItIsHeLiuHuiZong
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsHeLiuHuiZong);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsHeLiuHuiZong, value);
            }
        }
        /// <summary>
        /// 该附件是否汇总到合流节点上去？
        /// </summary>
        public bool ItIsToHeLiuHZ
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsToHeLiuHZ);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsToHeLiuHZ, value);
            }
        }
        /// <summary>
        /// 文件展现方式
        /// </summary>
        public FileShowWay FileShowWay
        {
            get
            {
                return (FileShowWay)this.GetParaInt(FrmAttachmentAttr.FileShowWay);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FileShowWay, (int)value);
            }
        }
        /// <summary>
        /// 上传方式（对于父子流程有效）
        /// </summary>
        public AthUploadWay AthUploadWay
        {
            get
            {
                return (AthUploadWay)this.GetValIntByKey(FrmAttachmentAttr.AthUploadWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.AthUploadWay, (int)value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(FrmAttachmentAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_MapData, value);
            }

        }
        #endregion

        #region 快捷键
        /// <summary>
        /// 是否启用快捷键
        /// </summary>
        public bool FastKeyIsEnable
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.FastKeyIsEnable);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyIsEnable, value);
            }
        }
        /// <summary>
        /// 启用规则
        /// </summary>
        public string FastKeyGenerRole
        {
            get
            {
                return this.GetParaString(FrmAttachmentAttr.FastKeyGenerRole);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyGenerRole, value);
            }
        }
        #endregion 快捷键

        #region 构造方法
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachment()
        {
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="mypk"></param>
        public FrmAttachment(string mypk)
        {
            this.setMyPK(mypk);
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

                Map map = new Map("Sys_FrmAttachment", "附件");
                map.IndexField = FrmAttachmentAttr.FK_MapData;
                map.AddMyPK();

                map.AddTBString(FrmAttachmentAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "附件编号", true, false, 0, 50, 20);
                map.AddTBInt(FrmAttachmentAttr.FK_Node, 0, "节点控制(对sln有效)", false, false);

                //for渔业厅增加.
                map.AddTBInt(FrmAttachmentAttr.AthRunModel, 0, "运行模式", false, false);
                //for oppein欧派 BP.Difference.SystemConfig.AthSaveWayDefault
                map.AddTBInt(FrmAttachmentAttr.AthSaveWay, BP.Difference.SystemConfig.AthSaveWayDefault, "保存方式", false, false);

                map.AddTBString(FrmAttachmentAttr.Name, null, "名称", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentAttr.Exts, null, "要求上传的格式", true, false, 0, 200, 20);
                map.AddTBInt(FrmAttachmentAttr.NumOfUpload, 0, "最小上传数量", true, false);
                map.AddTBInt(FrmAttachmentAttr.TopNumOfUpload, 99, "最大上传数量", true, false);
                map.AddTBInt(FrmAttachmentAttr.FileMaxSize, 10240, "附件最大限制(KB)", true, false);
                map.AddTBInt(FrmAttachmentAttr.UploadFileNumCheck, 0, "上传校验方式", true, false);

                map.AddTBString(FrmAttachmentAttr.Sort, null, "类别(可为空)", true, false, 0, 500, 20);

                map.AddTBFloat(FrmAttachmentAttr.H, 150, "H", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "是否可以上传", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsVisable, true, "是否可见", false, false);
                //  map.AddTBInt(FrmAttachmentAttr.IsDelete, 1, "附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的)", false, false);
                map.AddTBInt(FrmAttachmentAttr.FileType, 0, "附件类型", false, false);
                map.AddTBInt(FrmAttachmentAttr.ReadRole, 0, "阅读规则", true, true);
                map.AddTBInt(FrmAttachmentAttr.PicUploadType, 0, "图片附件上传方式", true, true);

                //hzm新增列
                map.AddTBInt(FrmAttachmentAttr.DeleteWay, 1, "附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "是否可以下载", false, false);


                map.AddBoolean(FrmAttachmentAttr.IsAutoSize, true, "自动控制大小", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "是否增加备注", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsExpCol, false, "是否启用扩展列", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsShowTitle, true, "是否显示标题列", false, false);
                map.AddTBInt(FrmAttachmentAttr.UploadType, 0, "上传类型0单个1多个2指定", false, false);

                map.AddTBInt(FrmAttachmentAttr.IsIdx, 0, "是否排序", false, false);


                // map.AddBoolean(FrmAttachmentAttr.IsIdx, false, "是否排序?", true, true);

                #region 流程属性.
                //对于父子流程有效.
                map.AddTBInt(FrmAttachmentAttr.CtrlWay, 4, "控制呈现控制方式0=PK,1=FID,2=ParentID", false, false);
                map.AddTBInt(FrmAttachmentAttr.AthUploadWay, 0, "控制上传控制方式0=继承模式,1=协作模式.", false, false);
                map.AddTBInt(FrmAttachmentAttr.ReadRole, 0, "阅读规则", true, true);

                //数据引用，如果为空就引用当前的.
                map.AddTBString(FrmAttachmentAttr.DataRefNoOfObj, null, "数据引用组件ID", true, false, 0, 150, 20, true, null);
                #endregion 流程属性.


                //参数属性.
                map.AddTBAtParas(3000);

                //  map.AddTBInt(FrmAttachmentAttr.RowIdx, 0, "RowIdx", false, false);
                map.AddTBInt(FrmAttachmentAttr.GroupID, 0, "GroupID", false, false);
                map.AddTBString(FrmAttachmentAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                map.AddTBInt(FrmAttachmentAttr.IsEnableTemplate, 0, "是否启用模板下载?", false, false);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public bool ItIsUse = false;
        protected override bool beforeUpdateInsertAction()
        {
            if (this.NodeID == 0)
            {
                //适应设计器新的规则 by dgq 
                if (!DataType.IsNullOrEmpty(this.NoOfObj) && this.NoOfObj.Contains(this.FrmID))
                    this.setMyPK(this.NoOfObj);
                else
                    this.setMyPK(this.FrmID + "_" + this.NoOfObj);
            }
            else
                this.setMyPK(this.FrmID + "_" + this.NoOfObj + "_" + this.NodeID);

            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeInsert()
        {
            //在属性实体集合插入前，clear父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FrmID);

            if (this.NodeID == 0)
                this.setMyPK(this.FrmID + "_" + this.NoOfObj);
            else
                this.setMyPK(this.FrmID + "_" + this.NoOfObj + "_" + this.NodeID);

            //对于流程类的多附件，默认按照WorkID控制. add 2017.08.03  by zhoupeng.
            if (this.NodeID != 0 && this.HisCtrlWay == AthCtrlWay.PK)
                this.HisCtrlWay = AthCtrlWay.WorkID;

            return base.beforeInsert();
        }
        /// <summary>
        /// 插入之后
        /// </summary>
        protected override void afterInsert()
        {
            GroupField gf = new GroupField();
            if (this.NodeID == 0 && gf.IsExit(GroupFieldAttr.CtrlID, this.MyPK) == false)
            {
                if (this.GetParaBoolen("IsFieldAth") == true)
                    gf.SetPara("IsFieldAth", 1);
                gf.FrmID = this.FrmID;
                gf.CtrlID = this.MyPK;
                gf.CtrlType = "Ath";
                gf.Lab = this.Name;
                gf.Idx = 0;
                gf.Insert(); //插入.
            }
            base.afterInsert();
        }

        /// <summary>
        /// 删除之后.
        /// </summary>
        protected override void afterDelete()
        {
            GroupField gf = new GroupField();
            gf.Delete(GroupFieldAttr.CtrlID, this.MyPK);

            base.afterDelete();
        }
    }
    /// <summary>
    /// 附件s
    /// </summary>
    public class FrmAttachments : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件s
        /// </summary>
        public FrmAttachments()
        {
        }
        /// <summary>
        /// 附件s
        /// </summary>
        /// <param name="frmID">s</param>
        public FrmAttachments(string frmID)
        {
            this.Retrieve(FrmAttachmentAttr.FK_MapData, frmID, FrmAttachmentAttr.FK_Node, 0);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachment();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachment> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachment>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachment> Tolist()
        {
            System.Collections.Generic.List<FrmAttachment> list = new System.Collections.Generic.List<FrmAttachment>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachment)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
