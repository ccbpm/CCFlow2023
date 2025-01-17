﻿using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.En;

namespace BP.WF
{
    /// <summary>
    /// 普通工作
    /// </summary>
    public class GEWorkAttr : WorkAttr
    {
    }
    /// <summary>
    /// 普通工作
    /// </summary>
    public class GEWork : Work
    {
        #region 与_SQLCache 操作有关
        private SQLCache _SQLCache = null;
        public override SQLCache SQLCache
        {
            get
            {
                if (_SQLCache == null)
                {
                    _SQLCache = Cache.GetSQL(this.NodeFrmID.ToString());
                    if (_SQLCache == null)
                    {
                        _SQLCache = new SQLCache(this);
                        Cache.SetSQL(this.NodeFrmID.ToString(), _SQLCache);
                    }
                }
                return _SQLCache;
            }
            set
            {
                _SQLCache = value;
            }
        }
        #endregion

        #region 构造函数        
        /// <summary>
        /// 普通工作
        /// </summary>
        public GEWork()
        {
        }
        /// <summary>
        /// 普通工作
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEWork(int nodeid, string nodeFrmID)
        {
            this.NodeFrmID = nodeFrmID;
            this.NodeID = nodeid;
            this.SQLCache = null;
        }
        /// <summary>
        /// 普通工作
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEWork(int nodeid, string nodeFrmID, Int64 _oid)
        {
            this.NodeFrmID = nodeFrmID;
            this.NodeID = nodeid;
            this.OID = _oid;
            this.SQLCache = null;
        }
        #endregion

        #region Map
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                //if (this._enMap == null)
                this._enMap = BP.Sys.MapData.GenerHisMap(this.NodeFrmID);
                return this._enMap;
            }
        }
        /// <summary>
        /// GEWorks
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this.NodeID == 0)
                    return new GEWorks();
                return new GEWorks(this.NodeID, this.NodeFrmID);
            }
        }
        #endregion

        /// <summary>
        /// 重写tostring 返回fromID.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.NodeFrmID;
        }
    }
    /// <summary>
    /// 普通工作s
    /// </summary>
    public class GEWorks : Works
    {
        #region 重载基类方法
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID = 0;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                if (this.NodeID == 0)
                    return new GEWork();
                return new GEWork(this.NodeID, this.NodeFrmID);
            }
        }
        /// <summary>
        /// 普通工作ID
        /// </summary>
        public GEWorks()
        {
        }
        /// <summary>
        /// 普通工作ID
        /// </summary>
        /// <param name="nodeid"></param>
        public GEWorks(int nodeid, string nodeFrmID)
        {
            this.NodeID = nodeid;
            this.NodeFrmID = nodeFrmID;
        }
        public string NodeFrmID = "";
        #endregion
    }
}
