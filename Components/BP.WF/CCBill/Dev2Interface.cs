﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Sys;
using BP.WF.Template;
using Aliyun.OSS;

namespace BP.CCBill
{
    /// <summary>
    /// 接口调用
    /// </summary>
    public class Dev2Interface
    {
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at"></param>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static void Dict_AddTrack(string frmID, string frmWorkID, string at, string msg, string paras = null,
            string flowNo = null, string flowName = null, int nodeID = 0, Int64 workIDOfFlow = 0, string frmName = "")
        {
            BP.CCBill.Track tk = new BP.CCBill.Track();
            tk.WorkID = frmWorkID;
            tk.FrmID = frmID;
            tk.FrmName = frmName;
            tk.ActionType = at;

            switch (at)
            {
                case FrmActionType.BBS:
                    tk.ActionTypeText = "评论";
                    break;
                case FrmActionType.Create:
                    tk.ActionTypeText = "创建";
                    break;
                case FrmActionType.DataVerReback:
                    tk.ActionTypeText = "数据版本";
                    break;
                case FrmActionType.Save:
                    tk.ActionTypeText = "保存";
                    break;
                case FrmActionType.StartFlow:
                    tk.ActionTypeText = "发起流程";
                    break;
                default:
                    tk.ActionTypeText = "其他";
                    break;
            }

            tk.Rec = WebUser.No;
            tk.RecName = WebUser.Name;
            tk.DeptNo = WebUser.DeptNo;
            tk.DeptName = WebUser.DeptName;

            // 流程信息。
            tk.WorkIDOfFlow = workIDOfFlow;
            tk.NodeID = nodeID;
            if (flowName != null)
                tk.FlowName = flowName;
            if (flowNo != null)
                tk.FlowNo = flowNo;

            //tk.setMyPK(tk.FrmID + "_" + tk.WorkID + "_" + tk.Rec + "_" + (int)BP.CCBill.FrmActionType.BBS;
            tk.Msg = msg;
            tk.RDT = DataType.CurrentDateTime;

            ////流程信息.
            //tk.NodeID = nodeID;
            //tk.NodeName = nodeName;
            //tk.FlowNo = flowNo;
            //tk.FlowName = flowName;
            //tk.FID = fid;
            tk.Insert();
        }


        /// <summary>
        /// 创建单据的WorkID
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="userNo"></param>
        /// <param name="htParas"></param>
        /// <param name="billNo"></param>
        /// <param name="pDictID"></param>
        /// <param name="pDictWorkID"></param>
        /// <returns></returns>
        public static Int64 CreateBlankBillID(string frmID, string userNo = null, Hashtable htParas = null, string pDictFrmID = null,
            Int64 pDictWorkID = 0)
        {
            if (userNo == null)
                userNo = WebUser.No;

            GenerBill gb = new GenerBill();
            int i = gb.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, userNo, GenerBillAttr.BillState, 0);
            if (i == 1)
            {
                GERpt rpt1 = new GERpt(frmID);
                rpt1.OID = gb.WorkID;
                int count = rpt1.RetrieveFromDBSources();
               
                if (htParas != null)
                    rpt1.Copy(htParas);

                rpt1.SetValByKey("BillState", 0);
                rpt1.SetValByKey("Starter", gb.Starter);
                rpt1.SetValByKey("StarterName", gb.StarterName);
                rpt1.SetValByKey("FK_Dept", WebUser.DeptNo);
                rpt1.SetValByKey("RDT", gb.RDT);
                rpt1.SetValByKey("Title", gb.Title);
                rpt1.SetValByKey("BillNo", gb.BillNo);
                if (pDictFrmID != null)
                {
                    rpt1.SetValByKey("PWorkID", pDictWorkID);
                    rpt1.SetValByKey("PFrmID", pDictFrmID);
                }
                if(count == 0)
                    rpt1.InsertAsOID(gb.WorkID);
                else
                    rpt1.Update();
                return gb.WorkID;
            }


            FrmBill fb = new FrmBill(frmID);
            gb.WorkID = DBAccess.GenerOID("WorkID");
            gb.BillState = BillState.None; //初始化状态.
            gb.Starter = BP.Web.WebUser.No;
            gb.StarterName = BP.Web.WebUser.Name;
            gb.FrmName = fb.Name; //单据名称.
            gb.FrmID = fb.No; //单据ID

            //if (DataType.IsNullOrEmpty(billNo) == false)
            //    gb.BillNo = billNo; //BillNo
            gb.DeptNo = BP.Web.WebUser.DeptNo;
            gb.DeptName = BP.Web.WebUser.DeptName;
            gb.FrmTreeNo = fb.FormTreeNo; //单据类别.
            gb.RDT = DataType.CurrentDateTime;
            gb.NDStep = 1;
            gb.NDStepName = "启动";

            //父字典信息.
            if (pDictFrmID != null)
            {
                gb.PFrmID = pDictFrmID;
                gb.PWorkID = pDictWorkID;
            }


            //创建rpt.
            BP.WF.GERpt rpt = new BP.WF.GERpt(frmID);

            //设置标题.
            if (fb.EntityType == EntityType.FrmBill)
            {
                gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
                //if (DataType.IsNullOrEmpty(billNo) == false)
                //    gb.BillNo = billNo;
                //else
                gb.BillNo = BP.CCBill.Dev2Interface.GenerBillNo(fb.BillNoFormat, gb.WorkID, null, frmID);
            }

            if (fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict)
            {
                rpt.EnMap.CodeStruct = fb.EnMap.CodeStruct;
                //if (DataType.IsNullOrEmpty(billNo) == false)
                //    gb.BillNo = billNo;
                //else
                gb.BillNo = rpt.GenerNewNoByKey("BillNo");
                // BP.CCBill.Dev2Interface.GenerBillNo(fb.BillNoFormat, gb.WorkID, null, frmID);
                gb.Title = "";
            }

            gb.DirectInsert(); //执行插入.

            //如果.
            if (htParas != null)
                rpt.Copy(htParas);

            //更新基础的数据到表单表.
            // rpt = new BP.WF.GERpt(frmID);
            rpt.SetValByKey("BillState", (int)gb.BillState);
            rpt.SetValByKey("Starter", gb.Starter);
            rpt.SetValByKey("StarterName", gb.StarterName);
            rpt.SetValByKey("FK_Dept", WebUser.DeptNo);
            rpt.SetValByKey("RDT", gb.RDT);
            rpt.SetValByKey("Title", gb.Title);
            rpt.SetValByKey("BillNo", gb.BillNo);
            if (pDictFrmID != null)
            {
                rpt.SetValByKey("PWorkID", pDictWorkID);
                rpt.SetValByKey("PFrmID", pDictFrmID);
            }

            rpt.OID = gb.WorkID;
            rpt.InsertAsOID(gb.WorkID);

            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, rpt.OID.ToString(), FrmActionType.Create, "创建记录");
            return gb.WorkID;
        }
        /// <summary>
        /// 创建一个实体ID
        /// </summary>
        /// <param name="frmID">实体ID</param>
        /// <param name="userNo">用户编号</param>
        /// <param name="htParas">参数</param>
        /// <returns>一个实例的workid</returns>
        public static Int64 CreateBlankDictID(string frmID, string userNo, Hashtable htParas)
        {
            if (userNo == null)
                userNo = WebUser.No;

            // 创建一个实体, 先检查一下是否有空白的数据.
            GERpt rpt = new GERpt(frmID);
            int i = rpt.Retrieve("Starter", userNo, "BillState", 0);
            if (i >= 1)
            {
                if (htParas != null)
                    rpt.Copy(htParas);

                rpt.SetValByKey("RDT", DataType.CurrentDate);
                rpt.Update();
                return rpt.OID; //如果有空白的数据，就返回给他.
            }


            //执行copy数据.
            if (htParas != null)
                rpt.Copy(htParas);

            FrmDict fb = new FrmDict(frmID);


            //更新基础的数据到表单表.
            rpt.SetValByKey("BillState", 0);
            rpt.SetValByKey("Starter", WebUser.No);
            rpt.SetValByKey("StarterName", WebUser.Name);
            rpt.SetValByKey("FK_Dept", WebUser.DeptNo);
            rpt.SetValByKey("RDT", DataType.CurrentDate);

            //设置编号生成规则.
            rpt.EnMap.CodeStruct = "4";//  fb.BillNoFormat;

            //rpt.SetValByKey("Title", gb.Title);
            rpt.SetValByKey("BillNo", rpt.GenerNewNoByKey("BillNo"));
            rpt.OID = DBAccess.GenerOID("WorkID");
            rpt.ResetDefaultVal();
            rpt.InsertAsOID(rpt.OID);


            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, rpt.OID.ToString(), FrmActionType.Create, "创建记录");

            return rpt.OID;
        }
        /// <summary>
        /// 保存实体数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="htParas">参数数据</param>
        /// <returns></returns>
        public static void SaveDictWork(string frmID, Int64 workid, Hashtable htParas)
        {
            // 创建一个实体, 先检查一下是否有空白的数据.
            GERpt rpt = new GERpt(frmID);
            rpt.OID = workid;
            if (rpt.RetrieveFromDBSources() == 0)
            {
                if (htParas != null)
                    rpt.Copy(htParas);

                //设置编号生成规则.
                FrmBill fb = new FrmBill(frmID);
                rpt.EnMap.CodeStruct = fb.BillNoFormat;
                rpt.SetValByKey("BillNo", rpt.GenerNewNoByKey("BillNo"));
                rpt.InsertAsOID(workid);
            }
            else
            {
                //执行copy数据.
                if (htParas != null)
                    rpt.Copy(htParas);
            }

            //更新基础的数据到表单表.
            rpt.SetValByKey("BillState", 100);
            rpt.SetValByKey("Starter", WebUser.No);
            rpt.SetValByKey("StarterName", WebUser.Name);
            rpt.SetValByKey("FK_Dept", WebUser.DeptNo);
            rpt.SetValByKey("RDT", DataType.CurrentDate);
            rpt.Update();

            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, workid.ToString(), FrmActionType.Save, "执行保存");

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveBillWork(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);

            GenerBill gb = new GenerBill();
            gb.WorkID = workID;
            int i = gb.RetrieveFromDBSources();
            if (i == 0)
                return "";

            gb.BillState = BillState.Editing;

            //创建rpt.
            BP.WF.GERpt rpt = new BP.WF.GERpt(gb.FrmID, workID);

            if (fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict)
            {

                gb.Title = rpt.Title;
                gb.Update();
                return "保存成功...";
            }

            //单据编号.
            if (DataType.IsNullOrEmpty(gb.BillNo) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.BillNo = BP.CCBill.Dev2Interface.GenerBillNo(fb.BillNoFormat, workID, null, fb.PTable);
                //更新单据里面的billNo字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "BillNo") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET BillNo='" + gb.BillNo + "' WHERE OID=" + workID);
            }

            //标题.
            if (DataType.IsNullOrEmpty(gb.Title) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
                //更新单据里面的 Title 字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "Title") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET Title='" + gb.Title + "' WHERE OID=" + workID);
            }

            gb.Update();

            //把通用的字段更新到数据库.
            rpt.Title = gb.Title;
            rpt.BillNo = gb.BillNo;
            rpt.Update();

            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, rpt.OID.ToString(), FrmActionType.Save, "保存");

            return "保存成功...";
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SubmitWork(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);

            GenerBill gb = new GenerBill();
            gb.WorkID = workID;
            int i = gb.RetrieveFromDBSources();
            if (i == 0)
                return "";

            //设置为归档状态.
            gb.BillState = BillState.Over;

            //创建rpt.
            BP.WF.GERpt rpt = new BP.WF.GERpt(gb.FrmID, workID);

            if (fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict)
            {
                gb.Title = rpt.Title;
                gb.Update();
                return "提交成功...";
            }

            //单据编号.
            if (DataType.IsNullOrEmpty(gb.BillNo) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.BillNo = BP.CCBill.Dev2Interface.GenerBillNo(fb.BillNoFormat, workID, null, fb.PTable);
                //更新单据里面的billNo字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "BillNo") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET BillNo='" + gb.BillNo + "' WHERE OID=" + workID);
            }

            //标题.
            if (DataType.IsNullOrEmpty(gb.Title) == true && !(fb.EntityType == EntityType.EntityTree || fb.EntityType == EntityType.FrmDict))
            {
                gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
                //更新单据里面的 Title 字段.
                if (DBAccess.IsExitsTableCol(fb.PTable, "Title") == true)
                    DBAccess.RunSQL("UPDATE " + fb.PTable + " SET Title='" + gb.Title + "' WHERE OID=" + workID);
            }

            gb.Update();

            //把通用的字段更新到数据库.
            rpt.Title = gb.Title;
            rpt.BillNo = gb.BillNo;
            rpt.Update();

            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, workID.ToString(), FrmActionType.Submit, "执行提交.");


            return "提交成功...";
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存结果</returns>
        public static string SaveAsDraft(string frmID, Int64 workID)
        {
            GenerBill gb = new GenerBill(workID);
            if (gb.BillState != BillState.None)
                return "err@只有在None的模式下才能保存草稿。";

            if (gb.BillState != BillState.Editing)
            {
                gb.BillState = BillState.Editing;
                gb.Update();
            }
            return "保存成功...";
        }
        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyBill_Delete(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);
            string sqls = "DELETE FROM Frm_GenerBill WHERE WorkID=" + workID;
            sqls += "@DELETE FROM " + fb.PTable + " WHERE OID=" + workID;
            DBAccess.RunSQLs(sqls);
            return "删除成功.";
        }
        public static string MyBill_DeleteBills(string frmID, string workIds)
        {
            FrmBill fb = new FrmBill(frmID);
            string sqls = "DELETE FROM Frm_GenerBill WHERE WorkID in (" + workIds + ")";
            sqls += "@DELETE FROM " + fb.PTable + " WHERE OID in (" + workIds + ")";
            DBAccess.RunSQLs(sqls);
            return "删除成功.";
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyDict_Delete(string frmID, Int64 workID)
        {
            FrmBill fb = new FrmBill(frmID);
            string sql = "@DELETE FROM " + fb.PTable + " WHERE OID=" + workID;
            DBAccess.RunSQLs(sql);
            return "删除成功.";
        }


        /// <summary>
        /// 删除实体单据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyDict_DeleteDicts(string frmID, string workIds)
        {
            FrmBill fb = new FrmBill(frmID);
            string sql = "DELETE FROM " + fb.PTable + " WHERE OID in (" + workIds + ")";
            DBAccess.RunSQLs(sql);
            return "删除成功.";
        }
        /// <summary>
        /// 删除树形结构的实体表单
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="billNo"></param>
        /// <returns></returns>
        public static string MyEntityTree_Delete(string frmID, string billNo)
        {
            FrmBill fb = new FrmBill(frmID);
            string sql = "DELETE FROM " + fb.PTable + " WHERE BillNo='" + billNo + "' OR ParentNo='" + billNo + "'";
            DBAccess.RunSQLs(sql);
            return "删除成功.";
        }

        /// <summary>
        /// 复制单据数据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public static string MyBill_Copy(string frmID, Int64 workID)
        {
            //获取单据的属性
            FrmBill fb = new FrmBill(frmID);

            GenerBill gb = new GenerBill();
            gb.WorkID = DBAccess.GenerOID("WorkID");
            gb.BillState = BillState.Editing; //初始化状态.
            gb.Starter = BP.Web.WebUser.No;
            gb.StarterName = BP.Web.WebUser.Name;
            gb.FrmName = fb.Name; //单据名称.
            gb.FrmID = fb.No; //单据ID

            gb.FrmTreeNo = fb.FormTreeNo; //单据类别.
            gb.RDT = DataType.CurrentDateTime;
            gb.NDStep = 1;
            gb.NDStepName = "启动";

            //创建rpt.
            BP.WF.GERpt rpt = new BP.WF.GERpt(frmID, workID);

            //设置标题.
            gb.Title = Dev2Interface.GenerTitle(fb.TitleRole, rpt);
            gb.BillNo = BP.CCBill.Dev2Interface.GenerBillNo(fb.BillNoFormat, gb.WorkID, null, frmID);

            gb.DirectInsert(); //执行插入.

            //更新基础的数据到表单表.
            rpt.SetValByKey("BillState", (int)gb.BillState);
            rpt.SetValByKey("Starter", gb.Starter);
            rpt.SetValByKey("StarterName", gb.StarterName);
            rpt.SetValByKey("RDT", gb.RDT);
            rpt.SetValByKey("Title", gb.Title);
            rpt.SetValByKey("BillNo", gb.BillNo);
            rpt.OID = gb.WorkID;
            rpt.InsertAsOID(gb.WorkID);

            #region 复制其他数据.

            //复制明细。
            MapDtls dtls = new MapDtls(frmID);
            if (dtls.Count > 0)
            {
                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.ItIsCopyNDData == false)
                        continue;

                    //new 一个实例.
                    GEDtl dtlData = new GEDtl(dtl.No);

                    GEDtls dtlsFromData = new GEDtls(dtl.No);
                    dtlsFromData.Retrieve(GEDtlAttr.RefPK, workID);
                    foreach (GEDtl geDtlFromData in dtlsFromData)
                    {
                        //是否启用多附件
                        FrmAttachmentDBs dbs = null;
                        if (dtl.ItIsEnableAthM == true)
                        {
                            //根据从表的OID 获取附件信息
                            dbs = new FrmAttachmentDBs();
                            dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, geDtlFromData.OID);
                        }

                        dtlData.Copy(geDtlFromData);
                        dtlData.RefPK = rpt.OID.ToString();
                        dtlData.InsertAsNew();
                        if (dbs != null && dbs.Count != 0)
                        {
                            //复制附件信息
                            FrmAttachmentDB newDB = new FrmAttachmentDB();
                            foreach (FrmAttachmentDB db in dbs)
                            {
                                newDB.Copy(db);
                                newDB.RefPKVal = dtlData.OID.ToString();
                                newDB.FID = dtlData.OID;
                                newDB.setMyPK(DBAccess.GenerGUID());
                                newDB.Insert();
                            }
                        }

                    }
                }

            }

            //获取附件组件、
            FrmAttachments athDecs = new FrmAttachments(frmID);
            //复制附件数据。
            if (athDecs.Count > 0)
            {
                foreach (FrmAttachment athDec in athDecs)
                {
                    FrmAttachmentDBs aths = new FrmAttachmentDBs();
                    aths.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, athDec.MyPK, FrmAttachmentDBAttr.RefPKVal, workID);
                    foreach (FrmAttachmentDB athDB in aths)
                    {
                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.RefPKVal = rpt.OID.ToString();
                        athDB_N.setMyPK(DBAccess.GenerGUID());
                        athDB_N.Insert();
                    }
                }
            }
            #endregion 复制表单其他数据.

            BP.CCBill.Dev2Interface.Dict_AddTrack(frmID, workID.ToString(), "复制", "执行复制");
            return "复制成功.";
        }

        /// <summary>
        /// 获得发起列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataSet DB_StartBills(string empID)
        {
            //定义容器.
            DataSet ds = new DataSet();

            //单据类别.
            SysFormTrees ens = new SysFormTrees();
            ens.RetrieveAll();

            DataTable dtSort = ens.ToDataTableField("Sort");
            dtSort.TableName = "Sort";
            ds.Tables.Add(dtSort);

            //查询出来单据运行模式的.
            FrmBills bills = new FrmBills();
            bills.RetrieveAll();

            //bills.Retrieve(FrmBillAttr.EntityType, 0); //实体类型.

            DataTable dtStart = bills.ToDataTableField();
            dtStart.TableName = "Start";
            ds.Tables.Add(dtStart);
            return ds;

        }
        /// <summary>
        /// 获得待办列表
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public static DataTable DB_Todolist(string empID)
        {
            return new DataTable();
        }
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <param name="frmID">单据ID</param>
        /// <param name="empID">操作员</param>
        /// <returns></returns>
        public static DataTable DB_Draft(string frmID, string empID)
        {
            if (DataType.IsNullOrEmpty(empID) == true)
                empID = BP.Web.WebUser.No;

            GenerBills bills = new GenerBills();
            bills.Retrieve(GenerBillAttr.FrmID, frmID, GenerBillAttr.Starter, empID);

            return bills.ToDataTableField();
        }

        public static string GenerTitle(string titleRole, Entity wk)
        {
            if (DataType.IsNullOrEmpty(titleRole))
            {
                // 为了保持与ccflow4.5的兼容,从开始节点属性里获取.
                Attr myattr = wk.EnMap.Attrs.GetAttrByKey("Title");
                if (myattr == null)
                    myattr = wk.EnMap.Attrs.GetAttrByKey("Title");

                if (myattr != null)
                    titleRole = myattr.DefaultVal.ToString();

                if (DataType.IsNullOrEmpty(titleRole) || titleRole.Contains("@") == false)
                    titleRole = "@WebUser.FK_DeptName-@WebUser.No,@WebUser.Name在@RDT发起.";
            }

            if (titleRole == "@OutPara" || DataType.IsNullOrEmpty(titleRole) == true)
                titleRole = "@WebUser.FK_DeptName-@WebUser.No,@WebUser.Name在@RDT发起.";


            titleRole = titleRole.Replace("@WebUser.No", WebUser.No);
            titleRole = titleRole.Replace("@WebUser.Name", WebUser.Name);
            titleRole = titleRole.Replace("@WebUser.FK_DeptNameOfFull", WebUser.DeptNameOfFull);
            titleRole = titleRole.Replace("@WebUser.FK_DeptName", WebUser.DeptName);
            titleRole = titleRole.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
            titleRole = titleRole.Replace("@RDT", DataType.CurrentDateByFormart("yy年MM月dd日HH时mm分"));
            if (titleRole.Contains("@"))
            {
                Attrs attrs = wk.EnMap.Attrs;

                // 优先考虑外键的替换,因为外键文本的字段的长度相对较长。
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;
                    if (attr.ItIsRefAttr == false)
                        continue;
                    titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                }

                //在考虑其它的字段替换.
                foreach (Attr attr in attrs)
                {
                    if (titleRole.Contains("@") == false)
                        break;

                    if (attr.ItIsRefAttr == true)
                        continue;
                    titleRole = titleRole.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                }
            }
            titleRole = titleRole.Replace('~', '-');
            titleRole = titleRole.Replace("'", "”");

            // 为当前的工作设置title.
            wk.SetValByKey("Title", titleRole);
            return titleRole;
        }
        /// <summary>
        /// 生成单据编号
        /// </summary>
        /// <param name="billNo">单据编号规则</param>
        /// <param name="workid">工作ID</param>
        /// <param name="en">实体类</param>
        /// <param name="frmID">表单ID</param>
        /// <returns>生成的单据编号</returns>
        public static string GenerBillNo(string billNo, Int64 workid, Entity en, string frmID)
        {
            if (DataType.IsNullOrEmpty(billNo))
                billNo = "3";

            if (billNo.Contains("@"))
                billNo = BP.WF.Glo.DealExp(billNo, en, null);

            /*如果，Bill 有规则 */
            billNo = billNo.Replace("{YYYY}", DateTime.Now.ToString("yyyy"));
            billNo = billNo.Replace("{yyyy}", DateTime.Now.ToString("yyyy"));

            billNo = billNo.Replace("{yy}", DateTime.Now.ToString("yy"));
            billNo = billNo.Replace("{YY}", DateTime.Now.ToString("yy"));

            billNo = billNo.Replace("{MM}", DateTime.Now.ToString("MM"));
            billNo = billNo.Replace("{mm}", DateTime.Now.ToString("MM"));

            billNo = billNo.Replace("{DD}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{dd}", DateTime.Now.ToString("dd"));
            billNo = billNo.Replace("{HH}", DateTime.Now.ToString("HH"));
            billNo = billNo.Replace("{hh}", DateTime.Now.ToString("HH"));

            billNo = billNo.Replace("{LSH}", workid.ToString());
            billNo = billNo.Replace("{WorkID}", workid.ToString());
            billNo = billNo.Replace("{OID}", workid.ToString());

            if (billNo.Contains("@WebUser.DeptZi"))
            {
                string val = DBAccess.RunSQLReturnStringIsNull("SELECT Zi FROM Port_Dept WHERE No='" + WebUser.DeptNo + "'", "");
                billNo = billNo.Replace("@WebUser.DeptZi", val.ToString());
            }

            string sql = "";
            int num = 0;
            string supposeBillNo = billNo;  //假设单据号，长度与真实单据号一致
            List<KeyValuePair<int, int>> loc = new List<KeyValuePair<int, int>>();  //流水号位置，流水号位数
            string lsh; //流水号设置码
            int lshIdx = -1;    //流水号设置码所在位置

            for (int i = 2; i < 9; i++)
            {
                lsh = "{LSH" + i + "}";

                if (!supposeBillNo.Contains(lsh))
                    continue;

                while (supposeBillNo.Contains(lsh))
                {
                    //查找流水号所在位置
                    lshIdx = supposeBillNo.IndexOf(lsh);
                    //将找到的流水号码替换成假设的流水号
                    supposeBillNo = (lshIdx == 0 ? "" : supposeBillNo.Substring(0, lshIdx))
                                    + string.Empty.PadLeft(i, '_')
                                    +
                                    (lshIdx + 6 < supposeBillNo.Length
                                         ? supposeBillNo.Substring(lshIdx + 6)
                                         : "");
                    //保存当前流程号所处位置，及流程号长度，以便之后使用替换成正确的流水号
                    loc.Add(new KeyValuePair<int, int>(lshIdx, i));
                }
            }

            //数据库中查找符合的单据号集合,NOTE:此处需要注意，在LIKE中带有左广方括号时，要使用一对广播号将其转义
            sql = "SELECT BillNo FROM Frm_GenerBill WHERE BillNo LIKE '" + supposeBillNo.Replace("[", "[[]") + "'"
                + " AND WorkID <> " + workid
                + " AND FrmID ='" + frmID + "' "
                + " ORDER BY BillNo DESC ";

            string maxBillNo = DBAccess.RunSQLReturnString(sql);
            int ilsh = 0;

            if (DataType.IsNullOrEmpty(maxBillNo))
            {
                //没有数据，则所有流水号都从1开始
                foreach (KeyValuePair<int, int> kv in loc)
                {
                    supposeBillNo = (kv.Key == 0 ? "" : supposeBillNo.Substring(0, kv.Key))
                                    + "1".PadLeft(kv.Value, '0')
                                    +
                                    (kv.Key + kv.Value < supposeBillNo.Length
                                         ? supposeBillNo.Substring(kv.Key + kv.Value)
                                         : "");
                }
            }
            else
            {
                //有数据，则从右向左开始判断流水号，当右侧的流水号达到最大值，则左侧的流水号自动加1
                Dictionary<int, int> mlsh = new Dictionary<int, int>();
                int plus1idx = -1;

                for (int i = loc.Count - 1; i >= 0; i--)
                {
                    //获取单据号中当前位的流水码数
                    ilsh = Convert.ToInt32(maxBillNo.Substring(loc[i].Key, loc[i].Value));

                    if (plus1idx >= 0)
                    {
                        //如果当前码位被置为+1，则+1，同时将标识置为-1
                        ilsh++;
                        plus1idx = -1;
                    }
                    else
                    {
                        mlsh.Add(loc[i].Key, i == loc.Count - 1 ? ilsh + 1 : ilsh);
                        continue;
                    }

                    if (ilsh >= Convert.ToInt32(string.Empty.PadLeft(loc[i].Value, '9')))
                    {
                        //右侧已经达到最大值
                        if (i > 0)
                        {
                            //记录前位的码
                            mlsh.Add(loc[i].Key, 1);
                        }
                        else
                        {
                            supposeBillNo = "单据号超出范围";
                            break;
                        }

                        //则将前一个流水码位，标记为+1
                        plus1idx = i - 1;
                    }
                    else
                    {
                        mlsh.Add(loc[i].Key, ilsh + 1);
                    }
                }

                if (supposeBillNo == "单据号超出范围")
                    return supposeBillNo;

                //拼接单据号
                foreach (KeyValuePair<int, int> kv in loc)
                {
                    supposeBillNo = (kv.Key == 0 ? "" : supposeBillNo.Substring(0, kv.Key))
                                    + mlsh[kv.Key].ToString().PadLeft(kv.Value, '0')
                                    +
                                    (kv.Key + kv.Value < supposeBillNo.Length
                                         ? supposeBillNo.Substring(kv.Key + kv.Value)
                                         : "");
                }
            }

            billNo = supposeBillNo;

            return billNo;
        }
    }
}
