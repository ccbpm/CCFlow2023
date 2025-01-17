﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 图片附件
    /// </summary>
    public class FrmImgAth : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.Name, value);
            }
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string CtrlID
        {
            get
            {
                return this.GetValStringByKey(FrmImgAthAttr.CtrlID);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.CtrlID, value);
            }
        }
       
        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.H);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.H, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmImgAthAttr.W);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.W, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(FrmImgAthAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool ItIsEdit
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsEdit);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.IsEdit, value);
            }
        }
        /// <summary>
        /// 是否必填，2016-11-1
        /// </summary>
        public bool ItIsRequired
        {
            get
            {
                return this.GetValBooleanByKey(FrmImgAthAttr.IsRequired);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.IsRequired, value);
            }
        }
        /// <summary>
        /// 所在的分组
        /// </summary>
        public int GroupID
        {
            get
            {
                string str = this.GetValStringByKey(FrmImgAthAttr.GroupID);
                if (str.Equals("无") || str.Equals(""))
                    return 1;
                return int.Parse(str);
            }
            set
            {
                this.SetValByKey(FrmImgAthAttr.GroupID, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>

        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAth()
        {
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        /// <param name="mypk"></param>
        public FrmImgAth(string mypk)
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
                Map map = new Map("Sys_FrmImgAth", "图片附件");
                map.IndexField = MapAttrAttr.FK_MapData;

                map.AddMyPK();

                map.AddTBString(FrmImgAthAttr.FrmID, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(FrmImgAthAttr.CtrlID, null, "控件ID", true, true, 0, 200, 20);
                map.AddTBString(FrmImgAthAttr.Name, null, "中文名称", true, false, 0, 200, 20);


                map.AddTBFloat(FrmImgAthAttr.H, 200, "H", true, false);
                map.AddTBFloat(FrmImgAthAttr.W, 160, "W", false, false);

                map.AddBoolean(FrmImgAthAttr.IsEdit, true, "是否可编辑", true, true);
                //map.AddTBInt(FrmImgAthAttr.IsEdit, 1, "是否可编辑", true, true);
                map.AddBoolean(FrmImgAthAttr.IsRequired, false, "是否必填项", true, true);
                //显示的分组.
                map.AddDDLSQL(FrmImgAthAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.ColSpan, 0, "单元格数量", false, true);

                //跨单元格
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                //跨行
                map.AddDDLSysEnum(MapAttrAttr.RowSpan, 1, "行数", true, true, "RowSpanAttrString",
                   "@1=跨1个行@2=跨2行@3=跨3行");
                //map.AddTBInt(FrmImgAthAttr.IsRequired, 0, "是否必填项", true, true);
                //map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, true, 0, 128, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //this.setMyPK(this.FrmID + "_" + this.CtrlID;
            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            //在属性实体集合插入前，clear父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FrmID);

            BP.Sys.FrmImgAth imgAth = new BP.Sys.FrmImgAth();
            imgAth.setMyPK(this.MyPK);
            imgAth.RetrieveFromDBSources();
            imgAth.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_" + imgAth.CtrlID);
            if (attr.RetrieveFromDBSources() == 0)
            {
                attr.FrmID =this.FrmID;
                attr.Name = this.Name;
                attr.setKeyOfEn(imgAth.CtrlID);
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.FrmImgAth);
                attr.GroupID = this.GroupID;
                attr.ItIsEnableInAPP = true;
                attr.setUIVisible(true);
                attr.DirectInsert();
            }
            else
            {
                attr.GroupID = this.GroupID;
                attr.DirectUpdate();
            }
           

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //把相关的字段也要删除.
            MapAttrString attr = new MapAttrString();
            attr.setMyPK(this.MyPK);
            attr.FrmID =this.FrmID;
            attr.Delete();
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);
            base.afterDelete();
        }

    }
    /// <summary>
    /// 图片附件s
    /// </summary>
    public class FrmImgAths : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 图片附件s
        /// </summary>
        public FrmImgAths()
        {
        }
        /// <summary>
        /// 图片附件s
        /// </summary>
        /// <param name="frmID">s</param>
        public FrmImgAths(string frmID)
        {
            this.Retrieve(MapAttrAttr.FK_MapData, frmID);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmImgAth();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmImgAth> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmImgAth>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmImgAth> Tolist()
        {
            System.Collections.Generic.List<FrmImgAth> list = new System.Collections.Generic.List<FrmImgAth>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmImgAth)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
