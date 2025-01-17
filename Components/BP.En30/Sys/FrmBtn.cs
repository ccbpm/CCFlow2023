﻿using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Pub;

namespace BP.Sys
{
    /// <summary>
    /// 按钮事件类型 - 与sl 中设置的要相同。
    /// </summary>
    public enum BtnEventType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 运行存储过程
        /// </summary>
        RunSP = 1,
        /// <summary>
        /// 运行sql
        /// </summary>
        RunSQL = 2,
        /// <summary>
        /// 执行URL
        /// </summary>
        RunURL = 3,
        /// <summary>
        /// 运行webservices
        /// </summary>
        RunWS = 4,
        /// <summary>
        /// 运行Exe文件.
        /// </summary>
        RunExe = 5,
        /// <summary>
        /// 运行JS
        /// </summary>
        RunJS =6
    }    
    /// <summary>
    /// 按钮访问
    /// </summary>
    public enum BtnUAC
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按角色
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按sql
        /// </summary>
        BySQL
    }
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum BtnType
    {
        /// <summary>
        /// 保存
        /// </summary>
        Save=0,
        /// <summary>
        /// 打印
        /// </summary>
        Print=1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete=2,
        /// <summary>
        /// 增加
        /// </summary>
        Add=3,
        /// <summary>
        /// 自定义
        /// </summary>
        Self=100
    }
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtnAttr : EntityMyPKAttr
    {
        /// <summary>
        /// Text
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string BtnType = "BtnType";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// 风格
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 字体风格
        /// </summary>
        public const string EventContext = "EventContext";
        /// <summary>
        /// 字体
        /// </summary>
        public const string UACContext = "UACContext";
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string EventType = "EventType";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string UAC = "UAC";
        /// <summary>
        /// MsgOK
        /// </summary>
        public const string MsgOK = "MsgOK";
        /// <summary>
        /// MsgErr
        /// </summary>
        public const string MsgErr = "MsgErr";
        /// <summary>
        /// 按钮ID
        /// </summary>
        public const string BtnID = "BtnID";
        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupID = "GroupID";
    }
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtn : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 所在的分组
        /// </summary>
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(FrmBtnAttr.GroupID);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.GroupID, value);
            }
        }
        public string MsgOK
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.MsgOK);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.MsgOK, value);
            }
        }
        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.MsgErr, value);
            }
        }
        /// <summary>
        /// EventContext
        /// </summary>
        public string EventContext
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.EventContext).Replace("#", "@");
                //return this.GetValStringByKey(FrmBtnAttr.EventContext);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.EventContext, value);
            }
        }
        
        public string UACContext
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.UACContext);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.UACContext, value);
            }
        }
        /// <summary>
        /// IsEnable
        /// </summary>
        public bool ItIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmBtnAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmBtnAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmBtnAttr.X);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.X, value);
            }
        }
        public BtnEventType HisBtnEventType
        {
            get
            {
                return (BtnEventType)this.GetValIntByKey(FrmBtnAttr.EventType);
            }
        }
        /// <summary>
        /// BtnType
        /// </summary>
        public int EventType
        {
            get
            {
                return this.GetValIntByKey(FrmBtnAttr.EventType);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.EventType, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Lab
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.Lab);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.Lab, value);
            }
        }
        public string TextHtml
        {
            get
            {
                //if (this.EventType)
                //    return "<b>" + this.GetValStrByKey(FrmBtnAttr.Text).Replace("@","<br>") + "</b>";
                //else
                    return this.GetValStrByKey(FrmBtnAttr.Lab).Replace("@", "<br>");
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtn()
        {
        }
        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="mypk"></param>
        public FrmBtn(string mypk)
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

                Map map = new Map("Sys_FrmBtn", "按钮");
                map.IndexField = FrmBtnAttr.FK_MapData;

                map.AddMyPK();
                map.AddTBString(FrmBtnAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmBtnAttr.Lab, null, "标签", true, false, 0, 3900, 20);

                map.AddTBFloat(FrmBtnAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmBtnAttr.Y, 5, "Y", false, false);

                map.AddTBInt(FrmBtnAttr.IsView, 0, "是否可见", false, false);
                map.AddTBInt(FrmBtnAttr.IsEnable, 0, "是否起用", false, false);

                //map.AddTBInt(FrmBtnAttr.BtnType, 0, "类型", false, false);

                map.AddTBInt(FrmBtnAttr.UAC, 0, "控制类型", false, false);
                map.AddTBString(FrmBtnAttr.UACContext, null, "控制内容", true, false, 0, 3900, 20);

                map.AddTBInt(FrmBtnAttr.EventType, 0, "事件类型", false, false);
                map.AddTBString(FrmBtnAttr.EventContext, null, "事件内容", true, false, 0, 3900, 20);

                map.AddTBString(FrmBtnAttr.MsgOK, null, "运行成功提示", true, false, 0, 500, 20);
                map.AddTBString(FrmBtnAttr.MsgErr, null, "运行失败提示", true, false, 0, 500, 20);

                map.AddTBString(FrmBtnAttr.BtnID, null, "按钮ID", true, false, 0, 128, 20);

                map.AddTBInt(FrmBtnAttr.GroupID, 0, "所在分组", false, false);

             
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 按钮s
    /// </summary>
    public class FrmBtns : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 按钮s
        /// </summary>
        public FrmBtns()
        {
        }
        /// <summary>
        /// 按钮s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmBtns(string fk_mapdata)
        {
            this.Retrieve(FrmBtnAttr.FK_MapData, fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBtn();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBtn> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBtn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBtn> Tolist()
        {
            System.Collections.Generic.List<FrmBtn> list = new System.Collections.Generic.List<FrmBtn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBtn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
