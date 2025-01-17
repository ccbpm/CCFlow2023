﻿
namespace BP.CCBill
{
    /// <summary>
    /// 实体表单 - Attr
    /// </summary>
    public class FrmAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string FrmDictWorkModel = "FrmDictWorkModel";
        /// <summary>
        /// 实体类型
        /// </summary>
        public const string EntityType = "EntityType";
        /// <summary>
        /// 展示模式
        /// </summary>
        public const string EntityShowModel = "EntityShowModel";
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public const string BillNoFormat = "BillNoFormat";
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public const string TitleRole = "TitleRole";
        /// <summary>
        /// 排序字段
        /// </summary>
        public const string SortColumns = "SortColumns";
        /// <summary>
        /// 字段颜色设置
        /// </summary>
        public const string ColorSet = "ColorSet";
        /// <summary>
        /// 按照指定字段的颜色显示表格行的颜色
        /// </summary>
        public const string RowColorSet = "RowColorSet";
        /// <summary>
        /// 字段求和求平均设置
        /// </summary>
        public const string FieldSet = "FieldSet";
        /// <summary>
        /// 关联单据
        /// </summary>
        public const string RefBill = "RefBill";
        #endregion

        #region 隐藏属性.
        /// <summary>
        /// 要显示的列
        /// </summary>
        public const string ShowCols = "ShowCols";
        #endregion 隐藏属性

        #region 按钮信息.
        /// <summary>
        /// 按钮New标签
        /// </summary>
        public const string BtnNewLable = "BtnNewLable";
        /// <summary>
        /// 按钮New启用规则
        /// </summary>
        public const string BtnNewModel = "BtnNewModel";
        /// <summary>
        /// 按钮Save标签
        /// </summary>
        public const string BtnSaveLable = "BtnSaveLable";
        /// <summary>
        /// 按钮save启用规则
        /// </summary>
        public const string BtnSaveEnable = "BtnSaveEnable";

        public const string BtnSubmitLable = "BtnSubmitLable";
        public const string BtnSubmitEnable = "BtnSubmitEnable";
        

        /// <summary>
        /// 保存andclose
        /// </summary>
        public const string BtnSaveAndCloseLable = "BtnSaveAndCloseLable";
        /// <summary>
        /// 保存并关闭.
        /// </summary>
        public const string BtnSaveAndCloseEnable = "BtnSaveAndCloseEnable";

        /// <summary>
        /// 按钮del标签
        /// </summary>
        public const string BtnDelLable = "BtnDelLable";
        /// <summary>
        /// 数据版本
        /// </summary>
        public const string BtnDataVer = "BtnDataVer";
        /// <summary>
        /// 按钮del启用规则
        /// </summary>
        public const string BtnDelEnable = "BtnDelEnable";
        /// <summary>
        /// 按钮del标签
        /// </summary>
        public const string BtnStartFlowLable = "BtnStartFlowLable";
        /// <summary>
        /// 按钮del启用规则
        /// </summary>
        public const string BtnStartFlowEnable = "BtnStartFlowEnable";
        /// <summary>
        /// 查询
        /// </summary>
        public const string BtnSearchLabel = "BtnSearchLabel";
        /// <summary>
        /// 查询
        /// </summary>
        public const string BtnSearchEnable = "BtnSearchEnable";
        /// <summary>
        /// 分析
        /// </summary>
        public const string BtnGroupLabel = "BtnGroupLabel";
        /// <summary>
        /// 分析
        /// </summary>
        public const string BtnGroupEnable = "BtnGroupEnable";
        #endregion

        #region 打印
        public const string BtnPrintHtml = "BtnPrintHtml";
        public const string BtnPrintHtmlEnable = "BtnPrintHtmlEnable";

        public const string BtnPrintPDF = "BtnPrintPDF";
        public const string BtnPrintPDFEnable = "BtnPrintPDFEnable";

        public const string BtnPrintRTF = "BtnPrintRTF";
        public const string BtnPrintRTFEnable = "BtnPrintRTFEnable";

        public const string BtnPrintCCWord = "BtnPrintCCWord";
        public const string BtnPrintCCWordEnable = "BtnPrintCCWordEnable";
        #endregion

        #region 按钮.
        /// <summary>
        /// 导出zip文件
        /// </summary>
        public const string BtnExpZip = "BtnExpZip";
        /// <summary>
        /// 是否可以启用?
        /// </summary>
        public const string BtnExpZipEnable = "BtnExpZipEnable";
        /// <summary>
        /// 关联单据
        /// </summary>
        public const string BtnRefBill = "BtnRefBill";
        /// <summary>
        /// 关联单据是否可用
        /// </summary>
        public const string RefBillRole = "RefBillRole";
        #endregion 按钮.

        #region 集合的操作.
        /// <summary>
        /// 导入Excel
        /// </summary>
        public const string BtnImpExcel = "BtnImpExcel";
        /// <summary>
        /// 是否启用导入
        /// </summary>
        public const string BtnImpExcelEnable = "BtnImpExcelEnable";
        /// <summary>
        /// 导出Excel
        /// </summary>
        public const string BtnExpExcel = "BtnExpExcel";
        /// <summary>
        /// 导出excel
        /// </summary>
        public const string BtnExpExcelEnable = "BtnExpExcelEnable";
        #endregion 集合的操作.

        /// <summary>
        /// 行打开模式
        /// </summary>
        public const string RowOpenModel = "RowOpenModel";
        public const string PopHeight = "PopHeight";
        public const string PopWidth = "PopWidth";
        public const string Tag0 = "Tag0";
        public const string Tag1 = "Tag1";
        public const string Tag2 = "Tag2";
        /// <summary>
        /// 实体编辑模式
        /// </summary>
        public const string EntityEditModel = "EntityEditModel";

        public const string SearchDictOpenType = "SearchDictOpenType";

    }
}
