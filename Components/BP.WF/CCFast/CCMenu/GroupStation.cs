﻿using BP.En;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 权限组角色
    /// </summary>
    public class GroupStationAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 权限组
        /// </summary>
        public const string FK_Group = "FK_Group";
    }
    /// <summary>
    /// 权限组角色
    /// </summary>
    public class GroupStation : EntityMM
    {
        #region 属性
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(GroupStationAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(GroupStationAttr.FK_Station, value);
            }
        }
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey(GroupStationAttr.FK_Group);
            }
            set
            {
                this.SetValByKey(GroupStationAttr.FK_Group, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限组角色
        /// </summary>
        public GroupStation()
        {
        }
        
        /// <summary>
        /// 权限组角色
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_GroupStation", "权限组角色");
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(GroupStationAttr.FK_Group, null, "权限组", false, false, 0, 50, 20);
                map.AddDDLEntitiesPK(GroupStationAttr.FK_Station, null, "角色", new BP.Port.Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 权限组角色s
    /// </summary>
    public class GroupStations : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 权限组s
        /// </summary>
        public GroupStations()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupStation();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupStation>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupStation> Tolist()
        {
            System.Collections.Generic.List<GroupStation> list = new System.Collections.Generic.List<GroupStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupStation)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
