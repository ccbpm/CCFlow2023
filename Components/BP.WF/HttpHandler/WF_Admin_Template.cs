﻿using System;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Web;
using FluentFTP;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Template : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Template()
        {

        }
        /// <summary>
        /// 导入本机模版 
        /// 负责人：lizhen.
        /// </summary>
        /// <returns></returns>
        public string ImpFrmLocal_Done()
        {

            ///表单类型.
            string frmSort = this.GetRequestVal("FrmSort");

            //创建临时文件.
            string temp =  BP.Difference.SystemConfig.PathOfTemp + "" + Guid.NewGuid() + ".xml";
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), temp);

            //获得数据类型.
            DataSet ds = new DataSet();
            ds.ReadXml(temp);

            MapData md = new MapData();

            //获得frmID.
            string frmID = null;

            #region 检查模版是否正确.
            //检查模版是否正确.
            string errMsg = "";
            if (ds.Tables.Contains("WF_Flow") == true)
                return "err@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                return "err@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                return "err@缺少表:Sys_MapData";


            frmID = ds.Tables["Sys_MapData"].Rows[0]["No"].ToString();
            #endregion 检查模版是否正确.

            string impType = this.GetRequestVal("RB_ImpType");

            //执行导入.
            return ImpFrm(impType, frmID, md, ds, frmSort);
        }

        public string ImpFrm(string impType, string frmID, MapData md, DataSet ds, string frmSort)
        {
            //导入模式:按照模版的表单编号导入,如果该编号已经存在就提示错误
            if (impType == "0")
            {
                md.No = frmID;
                if (md.RetrieveFromDBSources() == 1)
                    return "err@该表单ID【" + frmID + "】已经存在数据库中,您不能导入.";
                md = BP.Sys.CCFormAPI.Template_LoadXmlTemplateAsNewFrm(ds, frmSort);
            }

            //导入模式:按照模版的表单编号导入,如果该编号已经存在就直接覆盖.
            if (impType == "1")
            {
                md.No = frmID;
                if (md.RetrieveFromDBSources() == 1)
                    md.Delete(); //直接删除.
                md = BP.Sys.CCFormAPI.Template_LoadXmlTemplateAsNewFrm(ds, frmSort);  // MapData.ImpMapData(ds);
            }

            //导入模式:按照模版的表单编号导入,如果该编号已经存在就增加@WebUser.OrgNo(组织编号)导入.
            if (impType == "2")
            {
                md.No = frmID;
                if (md.RetrieveFromDBSources() == 1)
                {
                    md.No = frmID + WebUser.OrgNo;
                    if (md.RetrieveFromDBSources() == 1)
                        return "err@表单编号为:" + md.No + "已存在.";
                    frmID = frmID + "" + WebUser.OrgNo;
                    md.No = frmID;
                }
                md = BP.Sys.CCFormAPI.Template_LoadXmlTemplateAsSpecFrmID(frmID, ds, frmSort);  // MapData.ImpMapData(ds);
            }

            //导入模式:按照指定的模版ID导入.
            if (impType == "3")
            {
                frmID = this.GetRequestVal("TB_SpecFrmID");
                md.No = frmID;
                if (md.RetrieveFromDBSources() == 1)
                    return "err@您输入的表单编号为:" + md.No + "已存在.";
                md = BP.Sys.CCFormAPI.Template_LoadXmlTemplateAsSpecFrmID(frmID, ds, frmSort);  // MapData.ImpMapData(ds);
            }
            if (impType == "3ftp")
            {
                md.No = frmID;
                if (md.RetrieveFromDBSources() == 1)
                    return "err@您输入的表单编号为:" + md.No + "已存在.";
                md = BP.Sys.CCFormAPI.Template_LoadXmlTemplateAsSpecFrmID(frmID, ds, frmSort);  // MapData.ImpMapData(ds);
            }

            return "执行成功.";
        }

        #region  界面 .
        public FtpClient GenerFTPConn
        {
            get
            {
                FtpClient conn = new FtpClient(Glo.TemplateFTPHost, Glo.TemplateFTPPort, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
                conn.Encoding = Encoding.GetEncoding("GB2312");
                //FtpClient conn = new FtpClient(Glo.TemplateFTPHost, Glo.TemplateFTPPort, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
                return conn;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Flow_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/Flow/";
            if (dirName.IndexOf("/Flow/") == -1)
                dirName = "/Flow/" + dirName;
            FtpClient conn = this.GenerFTPConn;
            DataSet ds = new DataSet();
            FtpListItem[] fls;
            try
            {
                fls = conn.GetListing(dirName);
            }
            catch
            {
                //System.Windows.Forms.MessageBox.Show("该目录无文件");
                return "err@该目录无文件";
            }
            DataTable dtDir = new DataTable();
            dtDir.TableName = "Dir";
            dtDir.Columns.Add("FileName", typeof(string));
            dtDir.Columns.Add("RDT", typeof(string));
            dtDir.Columns.Add("Path", typeof(string));
            ds.Tables.Add(dtDir);

            //把文件加里面.
            DataTable dtFile = new DataTable();
            dtFile.TableName = "File";
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("RDT", typeof(string));
            dtFile.Columns.Add("Path", typeof(string));
            foreach (FtpListItem fl in fls)
            {

                switch (fl.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        {
                            DataRow drDir = dtDir.NewRow();
                            drDir[0] = fl.Name;
                            drDir[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                            drDir[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                            dtDir.Rows.Add(drDir);
                            continue;
                        }
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0] = fl.Name;
                dr[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                dr[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/Form/";
            if (dirName.IndexOf("/Form/") == -1)
                dirName = "/Form/" + dirName;
            FtpClient conn = this.GenerFTPConn;
            DataSet ds = new DataSet();
            FtpListItem[] fls;
            try
            {
                fls = conn.GetListing(dirName);
            }
            catch
            {

                //System.Windows.Forms.MessageBox.Show("该目录无文件");
                return "err@该目录无文件";
            }

            DataTable dtDir = new DataTable();
            dtDir.TableName = "Dir";
            dtDir.Columns.Add("FileName", typeof(string));
            dtDir.Columns.Add("RDT", typeof(string));
            dtDir.Columns.Add("Path", typeof(string));
            ds.Tables.Add(dtDir);

            //把文件加里面.
            DataTable dtFile = new DataTable();
            dtFile.TableName = "File";
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("RDT", typeof(string));
            dtFile.Columns.Add("Path", typeof(string));
            foreach (FtpListItem fl in fls)
            {

                switch (fl.Type)
                {
                    case FtpFileSystemObjectType.Directory:
                        {
                            DataRow drDir = dtDir.NewRow();
                            drDir[0] = fl.Name;
                            drDir[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                            drDir[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                            dtDir.Rows.Add(drDir);
                            continue;
                        }
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0] = fl.Name;
                dr[1] = fl.Created.ToString("yyyy-MM-dd HH:mm");
                dr[2] = conn.GetWorkingDirectory() + "/" + fl.Name;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 导入流程模板
        /// </summary>
        /// <returns></returns>
        public string Flow_Imp()
        {
            //构造返回数据.
            DataTable dtInfo = new DataTable();
            dtInfo.Columns.Add("Name");   //文件名.
            dtInfo.Columns.Add("Info");   //导入信息。
            dtInfo.Columns.Add("Result"); //执行结果.

            //获得下载的文件名.
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');

            string sortNo = GetRequestVal("SortNo");//流程类别.
            string dirName = GetRequestVal("DirName"); //目录名称.
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";

            FtpClient conn = this.GenerFTPConn;
            string remotePath = conn.GetWorkingDirectory() + dirName;

            string err = "";
            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;

                #region 下载文件.
                //设置要到的路径.
                string tempfile =  BP.Difference.SystemConfig.PathOfTemp +  str;
                FtpStatus fs;
                try
                {
                    //下载目录下.
                    fs = conn.DownloadFile(tempfile, "/Flow" + remotePath + "/" + str, FtpLocalExists.Overwrite);
                }
                catch (Exception ex)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, ex.Message, "失败.");
                    continue;
                }

                if (fs.ToString().Equals("Success") == false)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, "模板未下载成", "失败.");
                    continue;
                }
                #endregion 下载文件.

                #region 执行导入.
                BP.WF.Flow flow = new BP.WF.Flow();
                try
                {
                    //执行导入.
                    flow = BP.WF.Template.TemplateGlo.LoadFlowTemplate(sortNo, tempfile, ImpFlowTempleteModel.AsNewFlow);
                    flow.DoCheck(); //要执行一次检查.

                    dtInfo = this.ImpAddInfo(dtInfo, str, "执行成功:新流程编号:" + flow.No + " - " + flow.Name, "成功.");
                }
                catch (Exception ex)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, ex.Message, "导入失败.");
                }
                #endregion 执行导入.
            }

            return BP.Tools.Json.ToJson(dtInfo);
        }
        public DataTable ImpAddInfo(DataTable dtInfo, string fileName, string info, string result)
        {
            DataRow dr = dtInfo.NewRow();
            dr[0] = fileName;
            dr[1] = info;
            dr[2] = result;
            dtInfo.Rows.Add(dr);
            return dtInfo;
        }
        /// <summary>
        /// 导入表单模板
        /// </summary>
        /// <returns></returns>
        public string Form_Step1()
        {
            //构造返回数据.
            DataTable dtInfo = new DataTable();
            dtInfo.Columns.Add("Name");   //文件名.
            dtInfo.Columns.Add("Info");   //导入信息.
            dtInfo.Columns.Add("Result"); //执行结果.

            //获得变量.
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');
            string sortNo = GetRequestVal("SortNo");
            string dirName = GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";

            FtpClient conn = this.GenerFTPConn;
            string remotePath = conn.GetWorkingDirectory() + dirName;

            MapData md = new MapData();
            ///遍历选择的文件.
            foreach (string str in strs)
            {
                if (str == "" || str.IndexOf(".xml") == -1)
                    continue;

                string[] def = str.Split(',');
                string fileName = def[0]; //文件名
                string model = def[1]; //模式. 3=按照指定的表单ID进行导入.
                string frmID = def[2]; //指定表单的ID.

                if (model == "3" && DataType.IsNullOrEmpty(frmID) == true)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, fileName, "您需要指定表单ID", "导入失败");
                    continue;
                }

                //设置要到的路径.
                string tempfile =  BP.Difference.SystemConfig.PathOfTemp + "" + fileName;

                //下载目录下
                FtpStatus fs = conn.DownloadFile(tempfile, "/Form" + remotePath + "/" + fileName, FtpLocalExists.Overwrite);
                if (fs.ToString().Equals("Success") == false)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, fileName, "文件下载失败", "导入失败");
                    continue;
                }

                //读取文件.
                DataSet ds = new DataSet();
                ds.ReadXml(tempfile);

                if (ds.Tables.Contains("Sys_MapData") == false)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, "模版不存在Sys_MapData表,非法的表单.", "导入失败");
                    continue;
                }


                try
                {
                    if (model == "3")
                        model += "ftp";
                    string info = this.ImpFrm(model, frmID, md, ds, sortNo);

                    if (info.Contains("err@"))
                        dtInfo = this.ImpAddInfo(dtInfo, fileName, info, "导入失败");
                    else
                        dtInfo = this.ImpAddInfo(dtInfo, fileName, info, "导入成功");
                }
                catch (Exception ex)
                {
                    dtInfo = this.ImpAddInfo(dtInfo, str, ex.Message, "导入失败");
                }
            }

            //返回执行结果.
            return BP.Tools.Json.ToJson(dtInfo);
        }
        #endregion 界面方法.

    }


}
