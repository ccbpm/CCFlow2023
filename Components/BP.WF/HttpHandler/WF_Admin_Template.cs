﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using FtpSupport;

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

        #region  界面 .
        public FtpSupport.FtpConnection GenerFTPConn
        {
            get
            {

                FtpSupport.FtpConnection conn = new FtpSupport.FtpConnection(Glo.TemplateFTPHost, Glo.TemplateFTPUser, Glo.TemplateFTPPassword);
                return conn;
            }
        }
        public string Flow_Init()
        {
            string dirName = this.GetRequestVal("DirName");
            if (DataType.IsNullOrEmpty(dirName) == true)
                dirName = "/";

            FtpSupport.FtpConnection conn = this.GenerFTPConn;

            DataSet ds = new DataSet();

            Win32FindData[] fls = conn.FindFiles();
            DataTable dtDir = new DataTable();
            dtDir.TableName = "Dir";
            dtDir.Columns.Add("FileName", typeof(string));
            dtDir.Columns.Add("FileSize", typeof(string));
            ds.Tables.Add(dtDir);


            //把文件加里面.
            DataTable dtFile = new DataTable();
            dtFile.TableName = "File";
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("FileSize", typeof(string));
            foreach (Win32FindData fl in fls)
            {
                switch(fl.FileAttributes)
                {
                    case System.IO.FileAttributes.Directory:
                        DataRow drDir = dtDir.NewRow(); ;
                        drDir[0] = fl.FileName;
                        drDir[1] = fl.FileSize;
                        dtDir.Rows.Add(drDir);
                        continue;
                    case System.IO.FileAttributes.System:
                    case System.IO.FileAttributes.Hidden:
                        continue;
                    default:
                        break;
                }

                DataRow dr = dtFile.NewRow();
                dr[0]= fl.FileName;
                dr[1] = fl.FileSize;
                dtFile.Rows.Add(dr);
            }
            ds.Tables.Add(dtFile);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 导入文件
        /// </summary>
        /// <returns></returns>
        public string Flow_Imp()
        {
            string fls = this.GetRequestVal("Files");
            string[] strs = fls.Split(';');
            string sortNo = GetRequestVal("SortNo");
            FtpConnection conn = this.GenerFTPConn;
            
            foreach (string str in strs)
            {
                //生成路径.
                string tempfile = BP.Sys.SystemConfig.PathOfTemp + str;
                //下载目录下.

                conn.GetFile(str, tempfile, false, System.IO.FileAttributes.Normal);
                //执行导入.
                Flow.DoLoadFlowTemplate(sortNo, tempfile, ImpFlowTempleteModel.AsNewFlow);
            }
            return "导入成功，请刷新，或者退出重新登录.";
        }
        #endregion 界面方法.

    }


}