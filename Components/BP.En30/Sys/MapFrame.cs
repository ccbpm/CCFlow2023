﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 框架
    /// </summary>
    public class MapFrameAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// URL
        /// </summary>
        public const string URL = "URL";
        public const string FrameURL = "FrameURL";
        public const string UrlSrcType = "UrlSrcType";
        /// <summary>
        /// 高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 是否可以自适应大小
        /// </summary>
        public const string IsAutoSize = "IsAutoSize";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        public const string Y = "Y";
        public const string X = "X";
        public const string EleType = "EleType";
    }
    /// <summary>
    /// 框架
    /// </summary>
    public class MapFrame : EntityMyPK
    {
        #region 属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin")==true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 是否自适应大小
        /// </summary>
        public bool ItIsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(MapFrameAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(MapFrameAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// EleType
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStrByKey(MapFrameAttr.EleType);
            }
        }
        public void setEleType(string val)
        {
            this.SetValByKey(MapFrameAttr.EleType, val);
        }
        /// <summary>
        /// 连接
        /// </summary>
        public string URL
        {
            get
            {
                string s= this.GetValStrByKey(MapFrameAttr.FrameURL);
                if (DataType.IsNullOrEmpty(s))
                    return "http://ccbpm.cn";
                return s;
            }
            set
            {
                this.SetValByKey(MapFrameAttr.FrameURL, value);
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(MapFrameAttr.H);
            }
            set
            {
                this.SetValByKey(MapFrameAttr.H, value);
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(MapFrameAttr.W);
            }
            set
            {
                this.SetValByKey(MapFrameAttr.W, value);
            }
        }
        public bool ItIsUse = false;
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(MapFrameAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapFrameAttr.FK_MapData, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapFrameAttr.Name);
            }
            set
            {
                this.SetValByKey(MapFrameAttr.Name, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrame()
        {
        }
        /// <summary>
        /// 框架
        /// </summary>
        /// <param name="no"></param>
        public MapFrame(string mypk)
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
                Map map = new Map("Sys_MapFrame", "框架");

                map.AddMyPK();
                map.AddTBString(MapFrameAttr.FK_MapData, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(MapFrameAttr.Name, null, "名称", true, false, 0, 200, 20,true);

                //@0=自定义@1=地图@2=流程轨迹表@3=流程轨迹图.
                map.AddTBInt(MapFrameAttr.UrlSrcType, 0, "URL来源", false, false);

                map.AddTBString(MapFrameAttr.FrameURL, null, "FrameURL", true, false, 0, 3000, 20, true);
                map.AddTBString(MapFrameAttr.URL, null, "URL", false, false, 0, 3000, 20, true);

                //map.AddDDLSysEnum(MapFrameAttr.UrlSrcType, 0, "URL来源", true, true, MapFrameAttr.UrlSrcType,
                //"@0=自定义@1=地图@2=流程轨迹表@3=流程轨迹图");

                map.AddTBString(MapFrameAttr.W, null, "宽度", true, false, 0, 20, 20);
                map.AddTBString(MapFrameAttr.H, null, "高度", true, false, 0, 20, 20);

                map.AddBoolean(MapFrameAttr.IsAutoSize, true, "是否自动设置大小", false, false);

                map.AddTBString(MapFrameAttr.EleType, null, "类型", false, false, 0, 50, 20, true);

               // map.AddTBInt(MapFrameAttr.RowIdx, 99, "位置", false, false);
               // map.AddTBInt(MapFrameAttr.GroupID, 0, "GroupID", false, false);

                map.AddTBString(MapFrameAttr.GUID, null, "GUID", false, false, 0, 128, 20);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 插入之后增加一个分组.
        /// </summary>
        protected override void afterInsert()
        {
            GroupField gf = new GroupField();
            gf.FrmID = this.FrmID;
            gf.CtrlID =  this.MyPK;
            gf.CtrlType = "Frame";
            gf.Lab = this.Name;
            gf.Idx = 0;
            gf.Insert(); //插入.

             
            base.afterInsert();
        }
        /// <summary>
        /// 删除之后的操作
        /// </summary>
        protected override void afterDelete()
        {
            GroupField gf = new GroupField();
            gf.Delete(GroupFieldAttr.CtrlID, this.MyPK);

            base.afterDelete();
        }
        protected override bool beforeInsert()
        {
            //在属性实体集合插入前，clear父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FrmID);
             
            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            GroupField gf = new GroupField();
            gf.Retrieve(GroupFieldAttr.CtrlID, this.MyPK);
            gf.Lab = this.Name;
            gf.Update();

            return base.beforeUpdate();
        }
        #endregion
    }
    /// <summary>
    /// 框架s
    /// </summary>
    public class MapFrames : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 框架s
        /// </summary>
        public MapFrames()
        {
        }
        /// <summary>
        /// 框架s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapFrames(string fk_mapdata)
        {
            this.Retrieve(MapFrameAttr.FK_MapData, fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrame();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrame> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrame>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrame> Tolist()
        {
            System.Collections.Generic.List<MapFrame> list = new System.Collections.Generic.List<MapFrame>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrame)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
