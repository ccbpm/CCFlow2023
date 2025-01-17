﻿using BP.En;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 人员菜单功能
    /// </summary>
    public class EmpMenuAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 菜单功能
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 是否选中.
        /// </summary>
        public const string IsChecked = "IsChecked";
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
    }
    /// <summary>
    /// 人员菜单功能
    /// </summary>
    public class EmpMenu : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 人员
        /// </summary>
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(EmpMenuAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 菜单
        /// </summary>
        public string FK_Menu
        {
            get
            {
                return this.GetValStringByKey(EmpMenuAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.FK_Menu, value);
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool ItIsChecked
        {
            get
            {
                return this.GetValBooleanByKey(EmpMenuAttr.IsChecked);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.IsChecked, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 人员菜单功能
        /// </summary>
        public EmpMenu()
        {
        }
        /// <summary>
        /// 人员菜单功能
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("GPM_EmpMenu", "人员菜单对应");

                map.AddMyPK();

                // map.AddTBStringPK(EmpMenuAttr.FK_Emp, null, "操作员", true, false, 0, 3900, 20);
                map.AddTBString(EmpMenuAttr.FK_Menu, null, "菜单", false, false, 0, 50, 20);
                map.AddDDLEntities(EmpMenuAttr.FK_Emp, null, "人员", new BP.Port.Emps(), true);
                map.AddTBString(EmpMenuAttr.FK_App, null, "系统编号", false, false, 0, 50, 20);
                map.AddBoolean(EmpMenuAttr.IsChecked, true, "是否选中", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            //@wwh,代码转换.
            this.MyPK = this.FK_Menu + "_" + this.EmpNo;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 人员菜单功能s
    /// </summary>
    public class EmpMenus : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 菜单s
        /// </summary>
        public EmpMenus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpMenu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpMenu> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpMenu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpMenu> Tolist()
        {
            System.Collections.Generic.List<EmpMenu> list = new System.Collections.Generic.List<EmpMenu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpMenu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
