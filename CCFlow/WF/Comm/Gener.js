﻿/**
 * 异步加载（layui.open）
 * @param {any} msg 加载时的提示信息
 * @returns
 */
var asyncLoad = (msg) => new Promise((resolve, _) => {
    const index = layer.open({
        content: msg,
        shade: [0.2, '#000'],
        title: '',
        btn: [],
        closeBtn: 0,
        icon: 16,
    })
    setTimeout(() => {
        resolve(index);
    }, 16)

})

function deleteUrlParam(targetUrl, targetKey) {
    if (typeof targetUrl !== 'string') {
        return targetUrl
    }

    if (!targetUrl.includes('?')) {
        return targetUrl
    }

    const queries = targetUrl.split('?')
    let resultUrl = ''
    if (queries.length > 1) {

        const search = queries[1].split('&');
        if (search.length > 0) {
            resultUrl += queries[0] + '?';
            for (const kvStr of search) {
                const [key, val] = kvStr.split('=')
                if (targetKey === key) continue;
                else resultUrl += `${key}=${val}&`
            }
            return resultUrl.substring(0, resultUrl.length - 1);
        }
    }
    return '';
}

// 从指定字符串中获取参数
function getQueryFromTargetStr(targetStr, targetKey) {
    if (typeof targetStr !== 'string') {
        return null
    }
    const queries = targetStr.split('?')
    if (queries.length > 1) {
        const search = queries[1].split('&');
        for (const kvStr of search) {
            const [key, val] = kvStr.split('=')
            if (targetKey === key) return val
        }
    }
    return null
}

function promptGener(msg, defVal) {
    var val = window.prompt(msg, defVal);
    return val;
}
function Reload() {
    Reload();
}

function GetHrefUrl() {
    return window.location.href;
    //return GetHrefUrl();
}
function SetHref(url) {

    var a = 1;
    var b = 2;
    if (a + a == b) {
        window.location.href = filterXSS(url);
    }
}


function setTimeoutGener(callback, time) {
    if (typeof callback != "function")
        return;
    else
        setTimeout(callback, time);
}
/**
 * 处理文本，方式被污染，用于安全检查
 * @param {any} text
 */
function DealText(text) {

    //if (text.toUpperCase().indexOf('SCRIPT') ==-1 )
    //    return text;

    if (/^[\d\-\+]*$/.test(text))
        return text;
    var a = 12;
    var b = 24;
    if (a + a == b || b == a * 2 || /^[\d\-\+]*$/.test(text))
        return text;
}

/**
 * 把表达式计算或者转化为json对象.
 * 
 * @param {表达式} str
 */
function cceval(exp) {
    if (exp == undefined) {
        alert('表达式错误:');
        return;
    }

    if (/^[\d\-\+]*$/.test(exp))
        return eval(exp);

    var a = 1;
    var b = 2;
    if (a + a == b)
        return eval(exp);
    // alert("非法的表达式：" + exp);
}

/**
 * 让用户重新登录.
 * 1. 系统登录成功后: 就转入了 http://localhost:2296/WF/Portal/Default.htm?Tokencfb65cbd-e1df-4e8e-b394-bd4f5ea89e1d&UserNo=admin
 * 2. 在调试系统的时候经常切换用户, 为了让用户不用来回登录，需要使用sid登录。
 * 3. 这个公共的方法
 * */
function ReLoginByToken() {
    //获得参数,获取不到就return. userNo,SID，当前url没有，就判断parent的url.
    var userNo = GetQueryString("UserNo");
    if (userNo == null || userNo == undefined)
        userNo = window.parent.GetQueryString("UserNo");

    if (userNo == null || userNo == undefined)
        return;

    //获得参数,获取不到就return. userNo,SID，当前url没有，就判断parent的url.
    var sid = localStorage.getItem('Token');
    if (sid == null || sid == undefined)
        sid = window.parent.localStorage.getItem('Token');
    if (sid == null || sid == undefined || sid == 'null')
        return;

    //var webUser = new WebUser();
    //if (webUser.No == userNo)
    //    return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner");
    handler.AddPara("Token", sid);
    var data = handler.DoMethodReturnString("LetAdminLoginByToken");
    if (data.indexOf('err@') == 0) {
        //  alert(data);
        return true;
    }
    return true;
}

//检查字段,从表名,附件ID,输入是否合法.
function CheckID(val) {

    //首位可以是字母以及下划线。 
    //首位之后可以是字母，数字以及下划线。下划线后不能接下划线

    var flag = false; //用来判断
    var reg = /(^_([a-zA-Z0-9]_?)*$)|(^[a-zA-Z](_?[a-zA-Z0-9])*_?$)/;

    flag = reg.test(val);
    return flag;
}


//去左空格;
function ltrim(s) {
    return s.replace(/(^\s*)/g, "");
}
//去右空格;
function rtrim(s) {
    return s.replace(/(\s*$)/g, "");
}
//去左右空格;
function trim(s) {
    return s.replace(/(^\s*)|(\s*$)/g, "");
}

// script 标签过滤
function filterXSS(str) {
    if (typeof str !== 'string') {
        return str
    }
    str = str.trim()
    //.replace(/[(]/g, '')
    return str.replace(/<\/?[^>]+>/gi, '')
        .replace(/->/g, '_')
}

/* 把一个 @XB=1@Age=25 转化成一个js对象.  */
function AtParaToJson(json) {
    var jsObj = {};
    if (json) {
        var atParamArr = json.split('@');
        $.each(atParamArr, function (i, atParam) {
            if (atParam != '') {
                var atParamKeyValue = atParam.split('=');
                if (atParamKeyValue.length == 2) {
                    jsObj[atParamKeyValue[0]] = filterXSS(atParamKeyValue[1]);
                }
            }
        });
    }
    return jsObj;
}

function GetPKVal() {

    var val = this.GetQueryString("OID");

    if (val == undefined || val == "")
        val = GetQueryString("No");

    if (val == undefined || val == "")
        val = GetQueryString("WorkID");

    if (val == undefined || val == "")
        val = GetQueryString("NodeID");

    if (val == undefined || val == "")
        val = GetQueryString("MyPK");

    if (val == undefined || val == "")
        val = GetQueryString("PKVal");

    if (val == undefined || val == "")
        val = GetQueryString("PK");

    if (val == "null" || val == "" || val == undefined)
        return null;

    return val;
}

//处理url，删除无效的参数.
function DearUrlParas(urlParam) {

    //如何获得全部的参数？ &FK_Node=120&FK_Flow=222 放入到url里面去？
    //var href = GetHrefUrl();
    //var urlParam = href.substring(href.indexOf('?') + 1, href.length);

    if (urlParam == null || urlParam == undefined)
        urlParam = window.location.search.substring(1);

    var params = {};
    if (urlParam == "" && urlParam.length == 0) {
        urlParam = "1=1"
    } else {
        $.each(urlParam.split("&"), function (i, o) {
            if (o) {
                var param = o.split("=");
                if (param.length == 2) {
                    var key = param[0];
                    var value = param[1];

                    if (key == "DoType" || key == "DoMethod" || key == "HttpHandlerName")
                        return true;

                    if (value == "null" || typeof value == "undefined")
                        return true;

                    if (value != null && typeof value != "undefined"
                        && value != "null"
                        && value != "undefined") {

                        //  value = value.trim();

                        if (value != "" && value.length > 0) {
                            if (typeof params[key] == "undefined") {
                                params[key] = value;
                            }
                        }
                    }
                }
            }
        });
    }
    urlParam = "";
    $.each(params, function (i, o) {
        urlParam += i + "=" + o + "&";
    });

    urlParam = urlParam.replace("&&", "&");
    urlParam = urlParam.replace("&&", "&");
    urlParam = urlParam.replace("&&", "&");
    return urlParam;
}

function GetRadioValue(groupName) {
    var obj;
    obj = document.getElementsByName(groupName);
    if (obj != null) {
        var i;
        for (i = 0; i < obj.length; i++) {
            if (obj[i].checked) {
                return obj[i].value;
            }
        }
    }
    return null;
}

//获得所有的checkbox 的id组成一个string用逗号分开, 以方便后台接受的值保存.
function GenerCheckIDs() {

    var checkBoxIDs = "";
    var arrObj = document.all;

    for (var i = 0; i < arrObj.length; i++) {

        if (arrObj[i].type != 'checkbox')
            continue;

        var cid = arrObj[i].name;
        if (cid == null || cid == "" || cid == '')
            continue;

        checkBoxIDs += arrObj[i].id + ',';
    }
    return checkBoxIDs;
}

function GenerCheckNames() {

    var checkBoxIDs = "";
    var arrObj = document.all;

    for (var i = 0; i < arrObj.length; i++) {

        if (arrObj[i].type != 'checkbox')
            continue;

        var cid = arrObj[i].name;
        if (cid == null || cid == "" || cid == '')
            continue;
        if (checkBoxIDs.indexOf(arrObj[i].name) == -1)
            checkBoxIDs += arrObj[i].name + ',';
    }
    return checkBoxIDs;
}

//填充下拉框.
function GenerBindDDL(ddlCtrlID, data, noCol, nameCol, selectVal, filterKey1, filterVal1) {
    if (noCol == null)
        noCol = "No";

    if (nameCol == null)
        nameCol = "Name";

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;

    // 清空默认值, 写一个循环把数据给值.
    $("#" + ddlCtrlID).empty();
    $("#" + ddlCtrlID).append("<option value=''>- 请选择 -</option>");

    //如果他的数量==0，就return.
    if (json.length == 0)
        return;

    if (data[0].length == 1)
        json = data[0];

    if (json[0][noCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，No列名' + noCol + '不存在,无法行程期望的下拉框value . ');
        return;
    }

    if (json[0][nameCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，Name列名' + nameCol + '不存在,无法行程期望的下拉框value. ');
        return;
    }

    for (var i = 0; i < json.length; i++) {

        if (filterKey1 != undefined) {
            if (json[i][filterKey1] != filterVal1)
                continue;
        }

        // var no = json[i][noCol].toString();
        //   var no = json[i][nameCol].toString();

        if (json[i][noCol] == undefined)
            $("#" + ddlCtrlID).append("<option value='" + json[i][0] + "'>" + json[i][1] + "</option>");
        else
            $("#" + ddlCtrlID).append("<option value='" + json[i][noCol] + "'>" + json[i][nameCol] + "</option>");
    }

    //设置选中的值.
    if (selectVal != undefined) {

        var v = $("#" + ddlCtrlID)[0].options.length;
        if (v == 0)
            return;

        $("#" + ddlCtrlID).val(selectVal);

        var v = $("#" + ddlCtrlID).val();
        if (v == null) {
            $("#" + ddlCtrlID)[0].options[0].selected = true;
        }
    }
}

function GenerBindDDLAppend(ddlCtrlID, data, noCol, nameCol) {

    if (noCol == null)
        noCol = "No";

    if (nameCol == null)
        nameCol = "Name";

    $("#" + ddlCtrlID).html(''); //("<option value='" + json[i][0] + "'>" + json[i][1] + "</option>");


    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;

    //如果他的数量==0，就return.
    if (json.length == 0)
        return;

    if (data[0].length == 1)
        json = data[0];

    if (json[0][noCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，No列名' + noCol + '不存在,无法行程期望的下拉框value . ');
        return;
    }

    if (json[0][nameCol] == undefined) {
        alert('@在绑定[' + ddlCtrlID + ']错误，Name列名' + nameCol + '不存在,无法行程期望的下拉框value. ');
        return;
    }

    for (var i = 0; i < json.length; i++) {

        if (json[i][noCol] == undefined)
            $("#" + ddlCtrlID).append("<option value='" + json[i][0] + "'>" + json[i][1] + "</option>");
        else
            $("#" + ddlCtrlID).append("<option value='" + json[i][noCol] + "'>" + json[i][nameCol] + "</option>");
    }
}


/*绑定枚举值.*/
function GenerBindEnumKey(ctrlDDLId, enumKey, selectVal) {
    if (dynamicHandler == "")
        return;

    $.ajax({

        type: 'post',
        async: false,
        url: dynamicHandler + "?DoType=EnumList&EnumKey=" + enumKey + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {


            data = JSON.parse(data);

            if (data.length == 0) {
                alert('没有找到枚举值:' + enumKey);
                return;
            }



            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "IntKey", "Lab", selectVal);
            return;
        }
    });
}


/* 绑定枚举值外键表.*/
function GenerBindEntities(ctrlDDLId, ensName, selectVal, filter) {
    if (dynamicHandler == "")
        return;
    $.ajax({
        type: 'post',
        async: true,
        url: dynamicHandler + "?DoType=EnsData&EnsName=" + ensName + "&Filter=" + filter + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            data = JSON.parse(data);
            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "No", "Name", selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindEntities,错误:参数:EnsName" + ensName + " , 异常信息 responseText:" + jqXHR.responseText + "; status:" + jqXHR.status + "; statusText:" + jqXHR.statusText + "; \t\n textStatus=" + textStatus + ";errorThrown=" + errorThrown);
        }
    });
}


/*
绑定外键表.
*/
function GenerBindSFTable(ctrlDDLId, sfTable, selectVal) {
    if (dynamicHandler == "")
        return;
    $.ajax({
        type: 'post',
        async: true,
        url: dynamicHandler + "?DoType=SFTable&SFTable=" + sfTable + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            data = JSON.parse(data);
            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, "No", "Name", selectVal);
            return;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /*错误信息处理*/
            alert("GenerBindSFTable,错误:参数:EnsName" + ensName + " , 异常信息 responseText:" + jqXHR.responseText + "; status:" + jqXHR.status + "; statusText:" + jqXHR.statusText + "; \t\n textStatus=" + textStatus + ";errorThrown=" + errorThrown);
        }
    });
}

/* 绑定SQL.
1. 调用这个方法，需要在 SQLList.xml 配置一个SQL , sqlKey 就是该sql的标记.
2, paras 就是向这个sql传递的参数, 比如： @FK_Mapdata=BAC@KeyOfEn=MyFild  .
*/
function GenerBindSQL(ctrlDDLId, sqlKey, paras, colNo, colName, selectVal) {
    if (dynamicHandler == "")
        return;
    if (colNo == null)
        colNo = "NO";
    if (colName == null)
        colName = "NAME";

    $.ajax({
        type: 'post',
        async: true,
        url: dynamicHandler + "?DoType=SQLList&SQLKey=" + sqlKey + "&Paras=" + paras + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert(data);
            }

            data = JSON.parse(data);

            //绑定枚举值.
            GenerBindDDL(ctrlDDLId, data, colNo, colName, selectVal);

            return;
        }
    });
}

/*为页面的所有字段属性赋值. */
function GenerChangeParentValue(data) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data.length == 1)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值

        var div = window.parent.document.getElementById(attr);
        if (div != null) {
            div.innerHTML = val;
            continue;
        }
    }
}

/* 为页面的所有字段属性赋值.
 * 1. 列的字段控件使用 TB_,CB_,DDL,RB_
 * 2. 参数字段控件使用 TBPara_,CBPara_,DDLPara,RBPara_
 * */
function GenerFullAllCtrlsVal(data) {

    //  console.log(data);

    if (data == null)
        return;

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if ($.isArray(data) && data.length > 0)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值
        if (attr == 'enName' || attr == 'pkval' || attr === "" || attr == null)
            continue;

        var div = document.getElementById(attr);
        if (div != null) {
            div.innerHTML = val;
            continue;
        }

        if (attr == 'PAnyOne') {
            var aa = 1;
        }

        // textbox
        var tb = document.getElementById('TB_' + attr);
        if (tb != null) {
            if (val != null && '' != val && !isNaN(val) && !(/^\+?[1-9][0-9]*$/.test(val + '')))
                val = val.replace(new RegExp("~", "gm"), "'");
            //val = val.replace( /~/g,  "'");   //替换掉特殊字符,设置的sql语句的引号.

            if (tb.tagName.toLowerCase() != "input") {
                tb.innerHTML = val;
            }
            else {
                tb.value = val;
            }
            continue;
        }

        //checkbox.
        var cb = document.getElementById('CB_' + attr);
        if (cb != null) {
            if (val == "1" || val == 1)
                cb.checked = true;
            else
                cb.checked = false;
            continue;
        }

        //下拉框.
        var ddl = document.getElementById('DDL_' + attr);
        if (ddl != null) {

            if (ddl.options.length == 0)
                continue;

            $("#DDL_" + attr).val(val); // 操作权限.
            continue;
        }

        // RadioButton. 单选按钮.
        var rb = document.getElementById('RB_' + attr + "_" + val);
        // alert('RB_' + attr + "_" + val);
        if (rb != null) {
            rb.checked = true;
            continue;
        }

        // 处理参数字段.....................
        if (attr == "AtPara") {

            //val=@Title=1@SelectType=0@SearchTip=2@RootTreeNo=0
            $.each(val.split("@"), function (i, o) {
                if (o == "") {
                    return true;
                }
                var kv = o.split("=");
                if (kv.length == 2) {

                    json[kv[0]] = kv[1];
                    var suffix = kv[0];
                    var val = kv[1];

                    // textbox
                    tb = document.getElementById('TBPara_' + suffix);
                    if (tb == null)
                        tb = document.getElementById('TB_' + suffix);

                    if (tb != null) {

                        val = val.replace(new RegExp("~", "gm"), "'");
                        tb.value = val;
                        return true;
                    }

                    //下拉框.
                    ddl = document.getElementById('DDLPara_' + suffix);
                    if (ddl == null)
                        ddl = document.getElementById('DDL_' + suffix);

                    if (ddl != null) {

                        if (ddl.options.length == 0)
                            return true;

                        // console.log(suffix + "_before_" + val);
                        //$("#DDLPara_" + suffix).val(""); // 操作权限.

                        $("#DDLPara_" + suffix).val(val); // 操作权限.

                        //   window.setTimeout(function () { $("#DDLPara_" + suffix).val(row.districtCode); }, 1200); 
                        //  json[kv[0]] = kv[1];
                        //   $("#DDLPara_" + suffix).val("2"); // 操作权限.
                        //console.log(suffix + "_" + val);

                        return true;
                    }

                    //checkbox.
                    cb = document.getElementById('CBPara_' + suffix);
                    if (cb == null)
                        cb = document.getElementById('CB_' + suffix);

                    if (cb != null) {
                        if (val == "1" || val == 1)
                            cb.checked = true;
                        else
                            cb.checked = false;
                        return true;
                    }

                    // RadioButton. 单选按钮.
                    rb = document.getElementById('RBPara_' + suffix + "_" + val);
                    if (rb == null)
                        rb = document.getElementById('RB_' + suffix + "_" + val);

                    if (rb != null) {
                        rb.checked = true;
                        return true;
                    }
                }
            });
        }
        unSetCtrl += "@" + attr + " = " + val;
    }
}


/*为页面的所有 div 属性赋值. */
function GenerFullAllDivVal(data) {

    //判断data是否是一个数组，如果是一个数组，就取第1个对象.
    var json = data;
    if (data.length == 1)
        json = data[0];

    var unSetCtrl = "";
    for (var attr in json) {

        var val = json[attr]; //值

        var div = document.getElementById(attr);
        if (div != null) {
            div.innerHTML = val;
            continue;
        }
    }

    // alert('没有找到的控件类型:' + unSetCtrl);
}

function DoCheckboxValue(frmData, cbId) {
    if (frmData.indexOf(cbId + "=") == -1) {
        frmData += "&" + cbId + "=0";
    }
    else {
        frmData.replace(cbId + '=on', cbId + '=1');
    }

    return frmData;
}


/*隐藏与显示.*/
function ShowHidden(ctrlID) {

    var ctrl = document.getElementById(ctrlID);
    if (ctrl.style.display == "block") {
        ctrl.style.display = 'none';
    } else {
        ctrl.style.display = 'block';
    }
}

function OpenDialogAndCloseRefresh(url, dlgTitle, dlgWidth, dlgHeight, dlgIcon, fnClosed) {
    ///<summary>使用EasyUiDialog打开一个页面，页面中嵌入iframe【id="eudlgframe"】</summary>
    ///<param name="url" type="String">页面链接</param>
    ///<param name="dlgTitle" type="String">Dialog标题</param>
    ///<param name="dlgWidth" type="int">Dialog宽度</param>
    ///<param name="dlgHeight" type="int">Dialog高度</param>
    ///<param name="dlgIcon" type="String">Dialog图标，必须是一个样式class</param>
    ///<param name="fnClosed" type="Function">窗体关闭调用的方法（注意：此方法中可以调用dialog中页面的内容；如此方法启用，则关闭窗体时的自动刷新功能会失效）</param>

    var dlg = $('#eudlg');
    var iframeId = "eudlgframe";

    if (dlg.length == 0) {
        var divDom = document.createElement('div');
        divDom.setAttribute('id', 'eudlg');
        document.body.appendChild(divDom);
        dlg = $('#eudlg');
        dlg.append("<iframe frameborder='0' src='' scrolling='auto' id='" + iframeId + "' style='width:100%;height:100%'></iframe>");
    }

    dlg.dialog({
        title: dlgTitle,
        left: document.body.clientWidth > dlgWidth ? (document.body.clientWidth - dlgWidth) / 2 : 0,
        top: document.body.clientHeight > dlgHeight ? (document.body.clientHeight - dlgHeight) / 2 : 0,
        width: dlgWidth,
        height: dlgHeight,
        iconCls: dlgIcon,
        resizable: true,
        modal: true,
        onClose: function () {
            if (fnClosed) {
                fnClosed();
                return;
            }

            Reload();
        },
        cache: false
    });

    dlg.dialog('open');
    $('#' + iframeId).attr('src', url);
}

function Reload() {
    ///<summary>重新加载当前页面</summary>
    var newurl = "";
    var urls = GetHrefUrl().split('?');
    var params;

    if (urls.length == 1) {
        SetHref(GetHrefUrl() + "?t=" + Math.random());
    }

    newurl = urls[0] + '?1=1';
    params = urls[1].split('&');

    for (var i = 0; i < params.length; i++) {
        if (params[i].indexOf("1=1") != -1 || params[i].toLowerCase().indexOf("t=") != -1) {
            continue;
        }

        newurl += "&" + params[i];
    }

    SetHref(newurl + "&t=" + Math.random());
}

function ConvertDataTableFieldCase(dt, isLower) {
    ///<summary>转换datatable的json对象中的属性名称的大小写形式</summary>
    ///<param name="dt" type="Array">datatable json化后的[]数组</param>
    ///<param name="isLower" type="Boolean">是否转换成小写模式，默认转换成大写</param>
    if (!dt || !IsArray(dt)) {
        return dt;
    }

    if (dt.length == 0 || IsObject(dt[0]) == false) {
        return dt;
    }

    var newArr = [];
    var obj;

    for (var i = 0; i < dt.length; i++) {
        obj = {};

        for (var field in dt[i]) {
            obj[isLower ? field.toLowerCase() : field.toUpperCase()] = dt[i][field];
        }

        newArr.push(obj);
    }

    return newArr;
}

//通用的aj访问与处理工具.
function AjaxServiceGener(param, myUrl, callback, scope) {

    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "html", //返回json格式的数据
        contentType: "text/plain; charset=utf-8",
        url: Handler + myUrl, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (data) { //msg为返回的数据，在这里做数据绑定
            callback(data, scope);
        }
    });
}

function IsArray(obj) {
    ///<summary>判断是否是数组</summary>
    ///<param name="obj" type="All Type">要判断的对象</param>
    return Object.prototype.toString.call(obj) == "[object Array]";
}

function IsObject(obj) {
    ///<summary>判断是否是Object对象</summary>
    ///<param name="obj" type="All Type">要判断的对象</param>
    return typeof obj != "undefined" && obj.constructor == Object;
}

function To(url) {
    //window.location.href = filterXSS(url);
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {

    var newWindow = window.open(url, winName, 'width=800,height=550,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

function WinOpenFull(url, winName) {
    var newWindow = window.open(url, winName, 'width=' + window.screen.availWidth + ',height=' + window.screen.availHeight + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return newWindow;
}

// document绑定esc键的keyup事件, 关闭弹出窗
function closeWhileEscUp() {
    $(document).bind("keyup", function (e) {
        e = e || window.event;
        var key = e.keyCode || e.which || e.charCode;
        if (key == 27) {
            // 可能需要调整if判断的顺序
            if (parent && typeof parent.doCloseDialog === 'function') {
                parent.doCloseDialog.call();
            } else if (typeof doCloseDialog === 'function') {
                doCloseDialog.call();
            } else if (parent && parent.parent && typeof parent.parent.doCloseDialog === "function") {
                parent.parent.doCloseDialog.call();
            } else {
                window.close();
            }
        }
    });
}

/* 关于实体的类
GEEntity_Init
var pkval="Demo_DtlExpImpDtl1";  
var EnName="BP.WF.Template.Frm.MapDtlExt";
GEntity en=new GEEntity(EnName,pkval);
var strs=  en.ImpSQLNames;
// var strss=en.GetValByKey('ImpSQLNames');
en.ImpSQLNames=aaa;
en.Updata();
*/

var Entity = (function () {

    var jsonString;

    var Entity = function (enName, pkval) {

        if (enName == null || enName == "" || enName == undefined) {
            alert('enName不能为空');
            throw Error('enName不能为空');
            return;
        }

        if (pkval === "undefined") {
            alert(' pkval 不能为 undefined ');
            throw Error('pkval 不能为 undefined ');
        }

        this.enName = enName;

        if (pkval != null && typeof pkval === "object") {
            jsonString = {};
            this.CopyJSON(pkval);
        } else {
            this.pkval = pkval || "";
            this.loadData();
        }

    };

    function setData(self) {
        if (typeof jsonString !== "undefined") {
            $.each(jsonString, function (n, o) {
                // 需要判断属性名与当前对象属性名是否相同
                if (typeof self[n] !== "function") {
                    self[n] = o;
                }
            });
        }
    }

    function getParams(self) {
        var params = {};
        $.each(self, function (n, o) {
            if (typeof self[n] !== "function" && n != "enName" && n != "ensName") {
                params[n] = encodeURIComponent(self[n]);
            }
        });
        return params;
    }

    function getParams1(self) {

        var params = ["t=" + new Date().getTime()];
        $.each(jsonString, function (n, o) {

            if (typeof self[n] !== "function" && (self[n] != o || true)) {

                if (self[n] != undefined && self[n].toString().indexOf('<script') != -1)
                    params.push(n + "=aa");
                else
                    params.push(n + "=" + self[n]);

            }
        });
        return params.join("&");
    }

    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";

    Entity.prototype = {

        constructor: Entity,

        loadData: function () {
            var self = this;
            var pkval = self.pkval;
            if (dynamicHandler == "")
                return;
            var token = GetQueryString("Token");
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Init&EnName=" + self.enName + "&PKVal=" + encodeURIComponent(pkval) + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                success: function (data) {

                    if (data.indexOf("err@") != -1) {
                        data = data.replace('@@', '@');
                        alert(data);
                        throw new Error(data);
                        return;
                    }

                    if (data == "")
                        return;

                    try {
                        //处理特殊字符，字段中的值含有双引号
                        data = data.replace(/\\\\"/g, '\\"');
                        jsonString = JSON.parse(data);
                        setData(self);
                    } catch (e) {
                        alert("解析错误: " + data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Entity_Init 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState + " enName=" + self.enName + " pkval=" + self.pkval);
                }
            });
        },

        SetValByKey: function (key, value) {
            this[key] = value;
        },

        GetValByKey: function (key) {
            return this[key];
        },

        Insert: function () {
            if (dynamicHandler == "")
                return;

            var self = this;
            var params = getParams(self);

            var token = GetQueryString("Token");
            if (params.length == 0)
                params = getParams1(self);
            else {
                if (params.hasOwnProperty("Token") == false)
                    params["Token"] = token;
            }

            var result = "";

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Insert&EnName=" + self.enName + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: params,
                success: function (data) {

                    result = data;
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return 0; //插入失败.
                    }


                    data = JSON.parse(data);
                    result = data;

                    //alert(result.No);
                    //alert(data.No);
                    // setData(result);
                    // return;

                    //var self = this;
                    $.each(data, function (n, o) {
                        if (typeof self[n] !== "function") {
                            jsonString[n] = o;
                            self[n] = o;
                        }
                    });

                    //alert(result.No);
                    //alert(data.No);
                    //alert(" self "+self.No);

                    //alert(result.No);
                    //alert(this.No);

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
            return result;
        },
        DirectInsert: function () {
            if (dynamicHandler == "")
                return;

            var self = this;
            var params = getParams(self);
            var token = GetQueryString("Token");
            if (params.length == 0)
                params = getParams1(self);
            else {
                if (params.hasOwnProperty("Token") == false)
                    params["Token"] = token;
            }

            var result = "";

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_DirectInsert&EnName=" + self.enName + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: params,
                success: function (data) {

                    result = data;
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return 0; //插入失败.
                    }

                    data = JSON.parse(data);
                    result = data;

                    var self = this;
                    $.each(data, function (n, o) {
                        if (typeof self[n] !== "function") {
                            jsonString[n] = o;
                            self[n] = o;
                        }
                    });

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
            return result;
        },

        Update: function () {
            if (dynamicHandler == "")
                return;

            var self = this;
            var params = getParams(self);

            var token = GetQueryString("Token");

            if (params.hasOwnProperty("Token") == false)
                params["Token"] = token;


            var result;

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Update&EnName=" + self.enName + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: params,
                success: function (data) {
                    result = data;
                    if (data.indexOf("err@") != -1) {
                        var err = data.replace('err@', '');
                        alert('更新异常:' + err);
                        return 0;
                    }

                    $.each(params, function (n, o) {
                        jsonString[n] = o;
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Entity Update系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
            return result;
        },

        Save: function () {
            if (dynamicHandler == "")
                return;

            var self = this;
            var params = getParams(self);

            var token = GetQueryString("Token");
            if (params.hasOwnProperty("Token") == false)
                params["Token"] = token;

            var result;

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Save&EnName=" + self.enName + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: params,
                success: function (data) {
                    result = data;
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }
                    $.each(params, function (n, o) {
                        jsonString[n] = o;
                    });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Save 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
            return result;
        },

        Delete: function (key1, val1, key2, val2) {
            if (dynamicHandler == "")
                return;
            var self = this;
            //var params = getParams(self);
            var params = getParams1(this);
            var token = GetQueryString("Token");
            var result;

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Delete&EnName=" + self.enName + "&PKVal=" + this.GetPKVal() + "&Key1=" + key1 + "&Val1=" + val1 + "&Key2=" + key2 + "&Val2=" + val2 + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: params,
                success: function (data) {
                    result = data;
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }
                    //这个位置暂时去掉，保持删除Entity后信息仍然保留
                    /* $.each(jsonString, function (n, o) {
                         jsonString[n] = undefined;
                     });
                     setData(self);*/
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Delete 系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
                }
            });
            return result;
        },

        Retrieve: function () {
            if (dynamicHandler == "")
                return;

            var self = this;
            var params = getParams1(this);
            var token = GetQueryString("Token");
            var result;
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_Retrieve&EnName=" + self.enName + "&Token=" + token + "&" + params,
                dataType: 'html',
                success: function (data) {
                    result = data;
                    if (data.indexOf("err@") == 0) {
                        alert('查询失败:' + self.enName + "请联系管理员:\t\n" + data.replace('err@', ''));
                        return;
                    }

                    try {
                        //处理特殊字符，字段中的值含有双引号
                        data = data.replace(/\\\\"/g, '\\"');
                        jsonString = JSON.parse(data);
                        setData(self);
                        result = jsonString.Retrieve;

                    } catch (e) {
                        result = "err@解析错误: " + data;
                        alert(result);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var url = dynamicHandler + "?DoType=Entity_Retrieve&EnName=" + self.enName + "&" + params;
                    ThrowMakeErrInfo("Retrieve-" + self.enName + " pkval=" + pkavl, textStatus, url);
                }
            });
            return result;
        },
        SetPKVal: function (pkVal) {

            self.pkval = pkVal;
            this["MyPK"] = pkval;
            this["OID"] = pkval;
            this["WorkID"] = pkval;
            this["NodeID"] = pkval;
            this["No"] = pkval;

            if (jsonString != null) {
                jsonString["MyPK"] = pkval;
                jsonString["OID"] = pkval;
                jsonString["WorkID"] = pkval;
                jsonString["NodeID"] = pkval;
                jsonString["No"] = pkval;
            }

        },
        GetPKVal: function () {

            var val = null;
            var self = this;

            if (jsonString != null) {
                val = jsonString["MyPK"];
                if (val == undefined || val == "")
                    val = jsonString["OID"];
                if (val == undefined || val == "")
                    val = jsonString["WorkID"];
                if (val == undefined || val == "")
                    val = jsonString["NodeID"];
                if (val == undefined || val == "")
                    val = jsonString["No"];
                if (val == undefined || val == "")
                    val = this.pkval;

                if (val == undefined || val == "" || val == null) {
                } else {
                    return val;
                }
            }

            if (self != null) {
                val = self["MyPK"];
                if (val == undefined || val == "")
                    val = self["OID"];
                if (val == undefined || val == "")
                    val = self["WorkID"];
                if (val == undefined || val == "")
                    val = self["NodeID"];
                if (val == undefined || val == "")
                    val = self["No"];
                if (val == undefined || val == "")
                    val = this.pkval;

                if (val == undefined || val == "" || val == null) {
                } else {
                    return val;
                }
            }

            if (val == undefined || val == "")
                val = this["MyPK"];
            if (val == undefined || val == "")
                val = this["OID"];
            if (val == undefined || val == "")
                val = this["WorkID"];
            if (val == undefined || val == "")
                val = this["NodeID"];
            if (val == undefined || val == "")
                val = this["No"];
            if (val == undefined || val == "")
                val = this.pkval;

            return val;
        },
        RetrieveFromDBSources: function () {
            if (dynamicHandler == "")
                return;
            var self = this;
            // var params = getParams1(this); //查询的时候不需要把参数传入里面去.

            var pkavl = this.GetPKVal();

            if (pkavl == null || pkavl == "") {
                alert('[' + this.enName + ']没有给主键赋值无法执行查询.');
                return;
            }

            //  alert(self.GetPKVal()); 
            var token = GetQueryString("Token");
            var result;
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_RetrieveFromDBSources&EnName=" + self.enName + "&PKVal=" + pkavl + "&Token=" + token,
                dataType: 'html',
                success: function (data) {
                    result = data;
                    if (data.indexOf("err@") == 0) {
                        alert(data);
                        //var str = "查询:" + self.enName + " pk=" + self.pkval + " 错误.\t\n" + data.replace('err@', '');
                        //alert('查询:' + str);
                        return;
                    }
                    if (data == "")
                        return 0;
                    try {
                        //处理特殊字符，字段中的值含有双引号
                        data = data.replace(/\\\\"/g, '\\"');
                        jsonString = JSON.parse(data);
                        setData(self);
                        result = jsonString.RetrieveFromDBSources;

                    } catch (e) {
                        result = "err@解析错误: " + data;
                        alert(result);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var url = dynamicHandler + "?DoType=Entity_RetrieveFromDBSources&EnName=" + self.enName + "&PKVal=" + pkavl;
                    ThrowMakeErrInfo("Entity_RetrieveFromDBSources-" + self.enName + " pkval=" + pkavl, textStatus, url);

                    //alert(JSON.stringify(XMLHttpRequest));
                    //result = "RetrieveFromDBSources err@系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState;
                    //alert(result);
                }
            });
            return result;
        },

        IsExits: function () {
            if (dynamicHandler == "")
                return;
            var self = this;
            var result;

            var data = getParams1(self);
            var token = GetQueryString("Token");
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_IsExits&EnName=" + self.enName + "&" + getParams1(self) + "&Token=" + token,
                dataType: 'html',
                success: function (data) {

                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }

                    if (data == "1")
                        result = true;
                    else
                        result = false;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ThrowMakeErrInfo("Entity_IsExits-" + self.enName, textStatus);
                }
            });
            return result;
        },   //一个参数直接传递,  多个参数，参数之间使用 ~隔开， 比如: zhangsna~123~1~山东济南.
        DoMethodReturnString: function (methodName, myparams) {
            methodName = filterXSS(methodName)
            if (dynamicHandler == "")
                return;
            var params = "";
            if (myparams == null || myparams == undefined)
                myparams = "";

            $.each(arguments, function (i, o) {
                if (o == null) o = "";
                if (i != 0) {
                    if (!!o && o.toString().indexOf('~') != -1)
                        o = o.replace(/~/g, '`');
                    params += o + "~";
                }
            });
            if (params.lastIndexOf("~") == params.length - 1)
                params = params.substr(0, params.length - 1);
            arguments["paras"] = params;


            var pkval = this.GetPKVal();
            if (pkval == null || pkval == "") {
                alert('[' + this.enName + ']没有给主键赋值无法执行查询.');
                return;
            }
            var token = GetQueryString("Token");

            var self = this;
            var string;
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entity_DoMethodReturnString&EnName=" + self.enName + "&PKVal=" + encodeURIComponent(pkval) + "&MethodName=" + methodName + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                data: arguments,
                success: function (data) {
                    string = data;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var url = dynamicHandler + "?DoType=Entity_DoMethodReturnString&EnName=" + self.enName + "&PKVal=" + pkval + "&MethodName=" + methodName + "&t=" + new Date().getTime();
                    ThrowMakeErrInfo("Entity_DoMethodReturnString-" + self.enName + " pkval=" + pkval + " MethodName=" + methodName, textStatus,
                        url, XMLHttpRequest, errorThrown);

                    //    string = "Entity.DoMethodReturnString err@系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState;
                    //  alert(string);
                }
            });

            return string;

        },

        DoMethodReturnJSON: function (methodName, params) {

            var jsonString = this.DoMethodReturnString(methodName, params);

            if (jsonString.indexOf("err@") != -1) {
                alert(jsonString);
                return jsonString;
            }

            try {

                jsonString = ToJson(jsonString);

                //jsonString = JSON.parse(jsonString);
            } catch (e) {
                jsonString = "err@json解析错误: " + jsonString;
                alert(jsonString);
            }
            return jsonString;
        },

        toString: function () {
            return JSON.stringify(this);
        },

        GetPara: function (key, isNullAsVal="") {
            var atPara = this.AtPara;
            if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
                return isNullAsVal;
            }
            var reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
            var results = atPara.match(reg);
            if (results != null) {
                return unescape(results[2]);
            }
            return isNullAsVal;
        },

        SetPara: function (key, value) {
            var atPara = this.AtPara;
            if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
                return;
            }

            var m = "@" + key + "=";
            var index = atPara.indexOf(m);
            if (index == -1) {
                this.AtPara += "@" + key + "=" + value;
                return;
            }

            var p = atPara.substring(0, index + m.length);
            var s = atPara.substring(index + m.length, atPara.length);
            var i = s.indexOf("@");
            if (i == -1) {
                this.AtPara = p + value;
            } else {
                this.AtPara = p + value + s.substring(i, s.length);
            }

        },

        CopyURL: function () {
            var self = this;
            $.each(self, function (n, o) {
                if (typeof o !== "function") {
                    var value = GetQueryString(n);
                    if (value != null && typeof value !== "undefined" && $.trim(value) != "") {
                        self[n] = value;
                        jsonString[n] = value;
                    }
                }
            });
        },

        CopyForm: function () {

            $("input,select").each(function (i, e) {
                if (typeof $(e).attr("name") === "undefined" || $(e).attr("name") == "") {
                    $(e).attr("name", $(e).attr("id"));
                }
            });

            // 新版本20180107 2130
            var self = this;
            // 普通属性
            $("[name^=TB_],[name^=CB_],[name^=RB_],[name^=DDL_]").each(function (i, o) {
                var target = $(this);
                var name = target.attr("name");
                var key = name.replace(/^TB_|CB_|RB_|DDL_/, "");
                if (typeof self[key] === "function") {
                    return true;
                }
                if (name.match(/^TB_/)) {
                    self[key] = target.val();
                } else if (name.match(/^DDL_/)) {
                    self[key] = target.val();
                } else if (name.match(/^CB_/)) {
                    if (target.length == 1) {	// 仅一个复选框
                        if (target.is(":checked")) {
                            // 已选
                            self[key] = "1";
                        } else {
                            // 未选
                            self[key] = "0";
                        }
                    } else if (target.length > 1) {	// 多个复选框(待扩展)
                        // ?
                    }
                } else if (name.match(/^RB_/)) {

                    if (target.is(":checked")) {
                        // 已选
                        self[key] = "1";
                    } else {
                        // 未选
                        self[key] = "0";
                    }
                }
            });
            //获取树形结构的表单值
            var combotrees = $(".easyui-combotree");
            $.each(combotrees, function (i, combotree) {
                var name = $(combotree).attr('id');
                var tree = $('#' + name).combotree('tree');
                //获取当前选中的节点
                var data = tree.tree('getSelected');
                if (data != null) {
                    self[name.replace("DDL_", "")] = data.id;
                    self[name.replace("DDL_", "") + "T"] = data.text;
                }
            });
            // 参数属性
            $("[name^=TBPara_],[name^=CBPara_],[name^=RBPara_],[name^=DDLPara_]").each(function (i, o) {
                var target = $(this);
                var name = target.attr("name");
                var value;
                if (name.match(/^TBPara_/)) {
                    value = target.val();
                    // value = value.replace('@', ''); //替换掉@符号.
                } else if (name.match(/^DDLPara_/)) {
                    value = target.val();
                    //value = value.replace('@', ''); //替换掉@符号.
                } else if (name.match(/^CBPara_/)) {
                    if (target.length == 1) {	// 仅一个复选框
                        if (target.is(":checked")) {
                            // 已选
                            value = "1";
                        } else {
                            // 未选
                            value = "0";
                        }
                    } else if (target.length > 1) {	// 多个复选框(待扩展)
                        // ?
                    }
                } else if (name.match(/^RBPara_/)) {
                    if (target.is(":checked")) {
                        // 已选
                        value = "1";
                    } else {
                        // 未选
                        value = "0";
                    }
                }
                var key = name.replace(/^TBPara_|CBPara_|RBPara_|DDLPara_/, "");
                self.SetPara(key, value);
            });
        },

        CopyJSON: function (json) {
            var count = 0;
            if (json) {
                var self = this;
                $.each(json, function (n, o) {
                    if (typeof self[n] !== "function") {

                        if (n == 'enName' || n == 'MyPK')
                            return;

                        self[n] = o;
                        jsonString[n] = o;
                        count++;
                    }
                });
            }
            return count;
        },

        ToJsonWithParas: function () {
            var json = {};
            $.each(this, function (n, o) {
                if (typeof o !== "undefined") {
                    json[n] = o;
                }
            });
            if (typeof this.AtPara == "string") {
                $.each(this.AtPara.split("@"), function (i, o) {
                    if (o == "") {
                        return true;
                    }
                    var kv = o.split("=");
                    if (kv.length == 2) {
                        json[kv[0]] = kv[1];
                    }
                });
            }
            return json;
        }

    };

    return Entity;

})();


//// 外键，外部数据源数据实体类.
//// 1. 通过传入 SFNo 对应定义的数据表.
//// 2. xxxx
//var SFTableDBs = (function (sfNo) {
//    var en = new Entity("BP.Sys.SFTable", sfNo);
//    var data = en.DoMethodReturnJSON("GenerHisJson");

//}


var Entities = (function () {

    var jsonString;
    var parameters = {};
    var Entities = function () {
        this.ensName = arguments[0];
        this.Paras = getParameters(arguments);
        if (arguments.length >= 3) {
            this.loadData();
        }
    };

    function getParameters(args, divisor) {
        var params = "";
        var length;
        var orderBy;
        if (divisor == null || divisor == undefined)
            divisor = 2;

        if (divisor == 2) {
            if (args.length % 2 == 0) {
                orderBy = args[args.length - 1];
                length = args.length - 1;
            } else {
                length = args.length;
            }
            for (var i = 1; i < length; i += 2) {
                params += "@" + args[i] + "=" + args[i + 1];
            }
            if (typeof orderBy !== "undefined") {
                params += "@OrderBy=" + orderBy;
            }
            return params;
        }

        if (divisor == 3) {
            if ((args.length - 1) % divisor != 0) {
                orderBy = args[args.length - 1];
                length = args.length - 1;
            } else {
                length = args.length;
            }
            for (var i = 1; i < length; i += 3) { //args[i+1]是操作符
                params += "@" + args[i] + "|" + args[i + 1] + "|" + args[i + 2].replace(/%/g, '[%]');
            }
            if (typeof orderBy !== "undefined") {
                params += "@OrderBy||" + orderBy;
            }
            return params;
        }

    }

    Entities.prototype = {
        constructor: Entities,
        loadData: function () {
            if (dynamicHandler == "")
                return;
            var self = this;

            if (self.ensName == null || self.ensName == "" || self.ensName == "") {
                alert("在初始化实体期间EnsName没有赋值");
                return;
            }
            var token = GetQueryString("Token");

            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entities_Init&EnsName=" + self.ensName + "&Paras=" + self.Paras + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                success: function (data) {

                    if (data.indexOf("err@") != -1) {
                        data = data.replace('err@', '');
                        data += "\t\n参数信息:";
                        data += "\t\nDoType=Entities_Init";
                        data += "\t\EnsName=" + self.ensName;
                        data += "\t\Paras=" + self.Paras;
                        alert(data);
                        return;
                    }

                    try {
                        jsonString = JSON.parse(data);
                        if ($.isArray(jsonString)) {
                            self.length = jsonString.length;
                            $.extend(self, jsonString);
                        } else {
                            alert("解析失败, 返回值不是集合");
                        }
                    } catch (e) {
                        alert("json解析错误: " + data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ThrowMakeErrInfo("Entities_Init-" + self.ensName, textStatus);

                }
            });
        },
        deleteIt: function () {
            if (dynamicHandler == "")
                return;
            var self = this;
            if (self.ensName == null || self.ensName == "" || self.ensName == "") {
                alert("在初始化实体期间EnsName没有赋值");
                return;
            }
            var token = GetQueryString("Token");
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entities_Delete&EnsName=" + self.ensName + "&Paras=" + self.Paras + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                success: function (data) {
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }


                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {

                    ThrowMakeErrInfo("Entities_Delte-" + self.ensName, textStatus);

                }
            });
        },
        TurnToArry: function () {

            var ens = this;
            delete ens.Paras;
            delete ens.ensName;
            delete ens.length;
            var arr = [];
            for (var key in ens) {
                if (Object.hasOwnProperty.call(ens, key)) {
                    var en = ens[key];
                    arr.push(en);
                }
            }
            return arr
        },
        Retrieve: function () {
            var args = [""];
            $.each(arguments, function (i, o) {
                args.push(o);
            });
            this.Paras = getParameters(args);
            this.loadData();
        },
        RetrieveCond: function () {
            if (dynamicHandler == "")
                return;
            var args = [""];
            $.each(arguments, function (i, o) {
                args.push(o);
            });
            this.Paras = getParameters(args, 3);
            var self = this;

            if (self.ensName == null || self.ensName == "" || self.ensName == "") {
                alert("在初始化实体期间EnsName没有赋值");
                return;
            }
            var token = GetQueryString("Token");
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entities_RetrieveCond&EnsName=" + self.ensName + "&Token=" + token + "&t=" + new Date().getTime(),
                data: { "Paras": self.Paras },
                dataType: 'html',
                success: function (data) {

                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }

                    try {
                        jsonString = JSON.parse(data);
                        if ($.isArray(jsonString)) {
                            self.length = jsonString.length;
                            $.extend(self, jsonString);
                        } else {
                            alert("解析失败, 返回值不是集合");
                        }
                    } catch (e) {
                        alert("json解析错误: " + data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {

                    ThrowMakeErrInfo("Entities_RetrieveCond-" + self.ensName, textStatus);
                }
            });

        },
        Delete: function () {
            var args = [""];
            $.each(arguments, function (i, o) {
                args.push(o);
            });
            this.Paras = getParameters(args);

            this.deleteIt();
        },
        DoMethodReturnString: function (methodName) {
            methodName = filterXSS(methodName)
            if (dynamicHandler == "")
                return;
            var params = "";
            $.each(arguments, function (i, o) {
                if (o == null) o = "";
                if (i != 0)
                    params += o + "~";
            });

            params = params.substr(0, params.length - 1);
            var atPara = "";
            for (var key in parameters) {
                atPara += "@" + key + "=" + parameters[key];
            }
            var token = GetQueryString("Token");
            var self = this;
            var string;
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=Entities_DoMethodReturnString&EnsName=" + self.ensName + "&MethodName=" + methodName + "&paras=" + params + "&Token=" + token + "&t=" + new Date().getTime(),
                data: atPara = "" ? {} : { atPara: atPara },
                dataType: 'html',
                success: function (data) {
                    string = data;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ThrowMakeErrInfo("Entities_DoMethodReturnString-" + methodName, textStatus);
                }
            });

            return string;

        },
        GetEns: function () {
            // { data: [{}, {}, {}], length: 3, name: 'xxx' };
            var result = [];
            for (var key in this) {
                if (typeof this[key] === 'object') {
                    result.push(this[key]);
                }
            }
            this.data = result;
            return this;
        },

        DoMethodReturnJSON: function (methodName) {
            methodName = filterXSS(methodName)
            if (dynamicHandler == "")
                return;
            var params = "";
            $.each(arguments, function (i, o) {
                if (i != 0)
                    params += o + "~";
            });
            params = params.substr(0, params.length - 1);
            var jsonString = this.DoMethodReturnString(methodName, params);

            if (jsonString.indexOf("err@") != -1) {
                alert(jsonString);
                return jsonString;
            }

            try {
                jsonString = ToJson(jsonString);
            } catch (e) {
                jsonString = "err@json解析错误: " + jsonString;
                alert(jsonString);
            }
            return jsonString;
        },
        RetrieveAll: function () {
            if (dynamicHandler == "")
                return;
            var pathRe = "";
            if (plant == "JFlow" && (basePath == null || basePath == '')) {
                var rowUrl = GetHrefUrl();
                pathRe = rowUrl.substring(0, rowUrl.indexOf('/SDKFlowDemo') + 1);
            }
            var token = GetQueryString("Token");
            var self = this;
            $.ajax({
                type: 'post',
                async: false,
                url: pathRe + dynamicHandler + "?DoType=Entities_RetrieveAll&EnsName=" + self.ensName + "&Token=" + token + "&t=" + new Date().getTime(),
                dataType: 'html',
                success: function (data) {
                    if (data.indexOf("err@") != -1) {
                        alert(data);
                        return;
                    }
                    try {

                        jsonString = ToJson(data);

                        if ($.isArray(jsonString)) {
                            self.length = jsonString.length;
                            $.extend(self, jsonString);
                        } else {
                            alert("解析失败, 返回值不是集合");
                        }
                    } catch (e) {
                        alert("json解析错误: " + data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {

                    ThrowMakeErrInfo("Entities_RetrieveAll-", textStatus);

                }
            });
        },
        AddPara: function (key, value) {
            parameters[key] = filterXSS(value);
        },
        CopyForm: function () {

            $("input,select").each(function (i, e) {
                if (typeof $(e).attr("name") === "undefined" || $(e).attr("name") == "") {
                    $(e).attr("name", $(e).attr("id"));
                }
            });

            // 普通属性
            $("[name^=TB_],[name^=CB_],[name^=RB_],[name^=DDL_]").each(function (i, o) {
                var target = $(this);
                var name = target.attr("name");
                var key = name.replace(/^TB_|CB_|RB_|DDL_/, "");
                if (typeof parameters[key] === "function") {
                    return true;
                }
                if (name.match(/^TB_/)) {
                    parameters[key] = target.val();
                } else if (name.match(/^DDL_/)) {
                    parameters[key] = target.val();
                } else if (name.match(/^CB_/)) {
                    if (target.length == 1) {	// 仅一个复选框
                        if (target.is(":checked")) {
                            // 已选
                            parameters[key] = "1";
                        } else {
                            // 未选
                            parameters[key] = "0";
                        }
                    } else if (target.length > 1) {	// 多个复选框(待扩展)
                        // ?
                    }
                } else if (name.match(/^RB_/)) {

                    if (target.is(":checked")) {
                        // 已选
                        parameters[key] = "1";
                    } else {
                        // 未选
                        parameters[key] = "0";
                    }
                }
            });
            //获取树形结构的表单值
            var combotrees = $(".easyui-combotree");
            $.each(combotrees, function (i, combotree) {
                var name = $(combotree).attr('id');
                var tree = $('#' + name).combotree('tree');
                //获取当前选中的节点
                var data = tree.tree('getSelected');
                if (data != null) {
                    parameters[name.replace("DDL_", "")] = data.id;
                    parameters[name.replace("DDL_", "") + "T"] = data.text;
                }
            });
            // 参数属性
            $("[name^=TBPara_],[name^=CBPara_],[name^=RBPara_],[name^=DDLPara_]").each(function (i, o) {
                var target = $(this);
                var name = target.attr("name");
                var value;
                if (name.match(/^TBPara_/)) {
                    value = target.val();
                    // value = value.replace('@', ''); //替换掉@符号.
                } else if (name.match(/^DDLPara_/)) {
                    value = target.val();
                    //value = value.replace('@', ''); //替换掉@符号.
                } else if (name.match(/^CBPara_/)) {
                    if (target.length == 1) {	// 仅一个复选框
                        if (target.is(":checked")) {
                            // 已选
                            value = "1";
                        } else {
                            // 未选
                            value = "0";
                        }
                    } else if (target.length > 1) {	// 多个复选框(待扩展)
                        // ?
                    }
                } else if (name.match(/^RBPara_/)) {
                    if (target.is(":checked")) {
                        // 已选
                        value = "1";
                    } else {
                        // 未选
                        value = "0";
                    }
                }
                var key = name.replace(/^TBPara_|CBPara_|RBPara_|DDLPara_/, "");
                parameters[key] = value;
            });
        },
        Clear: function () {
            parameters = {};
        }

    };

    return Entities;

})();


function ToJson(data) {

    try {
        data = JSON.parse(data);
        return data;
    } catch (e) {
        return cceval(data);
    }
}


var DBAccess = (function () {

    function DBAccess() {
    }

    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";

    DBAccess.RunSQL = function (sql, dbSrc) {
        if (dynamicHandler == "")
            return;
        var count = 0;
        sql = sql.replace(/'/g, '~');
        $.ajax({
            type: 'post',
            async: false,
            url: dynamicHandler + "?DoType=DBAccess_RunSQL&t=" + new Date().getTime(),
            dataType: 'html',
            data: { "SQL": encodeURIComponent(sql), "DBSrc": dbSrc },
            success: function (data) {
                count = parseInt(data);
                if (isNaN(count)) {
                    count = -1;
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ThrowMakeErrInfo("DBAccess_RunSQL-", textStatus);
            }
        });

        return count;

    };
    //执行数据源返回json.
    DBAccess.RunDBSrc = function (dbSrc, dbType, dbSource) {

        if (dbSrc == "" || dbSrc == null || dbSrc == undefined) {
            alert("数据源为空..");
            return;
        }

        if (dbType == undefined) {
            dbType = 0; //默认为sql.

            if (dbSrc.length <= 20) {
                dbType = 2; //可能是一个方法名称.
            }

            if (dbSrc.indexOf('/') != -1) {
                dbType = 1; //是一个url.
            }
        }

        /*if (dbSrc.indexOf('@') != -1) {
        //val = val.replace(/~/g, "'"); //替换掉特殊字符,设置的sql语句的引号.
        var alt = "如果关键字有多个，可以使用.  /myword/g 作为通配符替换。  ";
        alert("数据源参数没有替换" + dbSrc + " \t\n" + alt);
        return;
        }*/


        //执行的SQL
        if (dbType == 0) {
            return DBAccess.RunSQLReturnTable(dbSrc, dbSource);
        }

        //执行URL
        if (dbType == 1 || dbType == "1") {
            return DBAccess.RunUrlReturnJSON(dbSrc);
        }

        //执行方法名称返回json.
        if (dbType == 2 || dbType == "2") {

            var str = DBAccess.RunFunctionReturnStr(dbSrc);
            if (str == null || str == undefined || str == "")
                return null;

            return JSON.parse(str);
        }
        //@谢 如何执行一个方法,
        //   alert("@没有处理执行方法。"); RunFunctionReturnJSON
    };

    //执行方法名返回str.
    DBAccess.RunFunctionReturnStr = function (funcName) {

        try {
            funcName = funcName.replace(/~/g, "'");
            if (funcName.indexOf('(') == -1)
                return cceval(funcName + "()");
            else
                return cceval(funcName);

        } catch (e) {
            if (e.message)
                alert("执行方法[" + funcName + "]错误:" + e.message);
        }
    };

    //执行方法名返回str.
    DBAccess.RunSQLReturnVal = function (sql, dbSrc) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Comm");
        handler.AddPara("SQL", sql);
        handler.AddPara("DBSrc", dbSrc);
        var dt = handler.DoMethodReturnString("RunSQL_Init");
        if (dt.length == 0)
            return null;
        var firItem = dt[0];
        var firAttr = "";
        for (var k in firItem) {
            firAttr = k;
            break;
        }
        return firItem[firAttr];
    };

    DBAccess.RunSQLReturnTable = function (sql, dbSrc) {
        if (dynamicHandler == "")
            return;

        sql = sql.replace(/~/g, "'");
        sql = sql.replace(/[+]/g, "/#");
        //sql = sql.replace(/-/g, '/$');



        var jsonString;

        $.ajax({
            type: 'post',
            async: false,
            url: dynamicHandler + "?DoType=DBAccess_RunSQLReturnTable" + "&t=" + new Date().getTime(),
            dataType: 'html',
            data: { "SQL": encodeURIComponent(sql), "DBSrc": dbSrc },
            success: function (data) {
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }
                try {
                    jsonString = JSON.parse(data);
                } catch (e) {
                    alert("json解析错误: " + data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ThrowMakeErrInfo("DBAccess_RunSQLReturnTable-", textStatus);
            }
        });
        return jsonString;
    };

    DBAccess.RunUrlReturnString = function (url) {
        if (dynamicHandler == "")
            return;
        if (url == null || typeof url === "undefined") {
            alert("err@url无效");
            return;
        }

        if (url.match(/^http:\/\//) == null) {
            url = basePath + url;
        }

        var string;

        $.ajax({
            type: 'post',
            async: false,
            url: dynamicHandler + "?DoType=RunUrlCrossReturnString&t=" + new Date().getTime(),
            data: { urlExt: url },
            dataType: 'html',
            success: function (data) {
                if (data.indexOf("err@") != -1) {
                    alert(data);
                    return;
                }
                string = data;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                alert(url);
                ThrowMakeErrInfo("HttpHandler-RunUrlCrossReturnString-", textStatus);

            }
        });

        return string;
    };

    DBAccess.RunUrlReturnJSON = function (url) {

        var jsonString = DBAccess.RunUrlReturnString(url);
        if (typeof jsonString === "undefined") {
            alert("执行错误:\t\n URL:" + url);
            return;
        }

        if (jsonString.indexOf("err@") != -1) {
            alert(jsonString + "\t\n URL:" + url);
            return jsonString;
        }

        try {

            jsonString = JSON.parse(jsonString);

        } catch (e) {
            jsonString = "err@json,RunUrlReturnJSON解析错误:" + jsonString;
            alert(jsonString);
        }
        return jsonString;
    };

    return DBAccess;

})();

var HttpHandler = (function () {

    var parameters;


    if (IsIELower10 == true)
        parameters = {};
    else
        parameters = new FormData();

    var formData;
    var params = "&";

    function HttpHandler(handlerName) {
        this.handlerName = handlerName;
        if (IsIELower10 == true)
            parameters = {};
        else
            parameters = new FormData();

        formData = undefined;
        params = "&";
    }

    function validate(s) {
        if (s == null || typeof s === "undefined") {
            return false;
        }
        s = s.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
        if (s == "" || s == "null" || s == "undefined") {
            return false;
        }
        return true;
    }

    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";

    HttpHandler.prototype = {

        constructor: HttpHandler,
        AddUrlData: function (url) {
            var queryString = url;
            if (url == null || url == undefined || url == "")
                queryString = document.location.search.substr(1);
            queryString = decodeURI(queryString);
            var self = this;
            $.each(queryString.split("&"), function (i, o) {
                var param = o.split("=");
                if (param.length == 2 && validate(param[1])) {

                    (function (key, value) {

                        if (key == "DoType" || key == "DoMethod" || key == "HttpHandlerName")
                            return;

                        self.AddPara(key, filterXSS(value));

                    })(param[0], param[1]);
                }
            });

        },

        AddFormData: function () {
            if ($("form").length == 0)
                throw Error('必须是Form表单才可以使用该方法');

            formData = $("form").serialize();
            //序列化时把空格转成+，+转义成％２Ｂ，在保存时需要把+转成空格  
            formData = formData.replace(/\+/g, " ");
            //form表单序列化时调用了encodeURLComponent方法将数据编码了
            // formData = decodeURIComponent(formData, true);
            if (formData.length > 0) {
                var self = this;
                $.each(formData.split("&"), function (i, o) {
                    var param = o.split("=");
                    if (param.length == 2 && validate(param[1])) {
                        (function (key, value) {
                            self.AddPara(key, filterXSS(decodeURIComponent(value, true)));
                        })(param[0], param[1]);
                    }
                });
            }
            //获取form表单中disabled的表单字段
            var disabledEles = $('form :disabled');
            $.each(disabledEles, function (i, disabledEle) {
                var name = $(disabledEle).attr('name');
                if (name == null || name == undefined || name == "")
                    return true;
                switch (disabledEle.tagName.toUpperCase()) {
                    case "INPUT":
                        switch (disabledEle.type.toUpperCase()) {
                            case "CHECKBOX": //复选框
                                self.AddPara(name, encodeURIComponent($(disabledEle).is(':checked') ? 1 : 0));
                                break;
                            case "TEXT": //文本框
                            case "NUMBER":
                                self.AddPara(name, encodeURIComponent($(disabledEle).val()));
                                break;
                            case "RADIO": //单选钮
                                self.AddPara(name, $('[name="' + name + ':checked"]').val());
                                break;
                        }
                        break;
                    //下拉框
                    case "SELECT":
                        self.AddPara(name, $(disabledEle).children('option:checked').val());
                        break;
                    case "TEXTAREA":
                        self.AddPara(name, encodeURIComponent($(disabledEle).val()));
                        break;
                }
            });
        },
        AddFileData: function () {
            var files = $("input[type=file]");
            for (var i = 0; i < files.length; i++) {
                var fileObj = files[i].files[0]; // js 获取文件对象
                if (typeof (fileObj) == "undefined") {
                    alert("请选择上传的文件.");
                    return;
                }
                if (fileObj.size == 0) {
                    alert("上传的文件大小为0KB,请查看内容后再重新上传");
                    return;
                }
                parameters.append("file", fileObj)
            }
        },
        AddPara: function (key, value) {
            if (params.indexOf("&" + key + "=") == -1) {
                if (value == undefined)
                    value = "";
                if (IsIELower10 == true)
                    parameters[key] = value;
                else
                    parameters.append(key, value);
                params += key + "=" + value + "&";
            }

        },

        AddJson: function (json) {

            for (var key in json) {
                this.AddPara(key, filterXSS(json[key]));
            }
        },

        Clear: function () {
            if (IsIELower10 == true)
                parameters = {};
            else
                parameters = new FormData();
            formData = undefined;
            params = "&";
        },

        getParams: function () {
            //    var params = [];
            //   /* $.each(parameters, function (key, value) {

            //        if (value.indexOf('<script') != -1)
            //            value = '';

            //        params.push(key + "=" + value);

            //    });
            //*/

            //    for (let [name, value] of formData) {
            //        alert(`${name} = ${value}`); // key1=value1，然后是 key2=value2
            //        if (value.indexOf('<script') != -1)
            //            value = '';
            //        params.push(name + "=" + value);
            //    }

            //    //for (var key of parameters.keys()) {
            //    //    var val = formData.get(key);
            //    //    if (val.indexOf('<script') != -1)
            //    //        val = '';
            //    //    params.push(key + "=" + val);

            //    //}


            return params;
        },

        customRequest: function (methodName) {
            methodName = filterXSS(methodName)
            if (dynamicHandler == "")
                return;
            var self = this;
            var jsonString;
            // 如果没有携带token， 自动补上
            if (!parameters.has('Token')) {
                parameters.append('Token', GetQueryString('Token'))
            }
            $.ajax({
                type: 'post',
                async: false,
                url: dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random(),
                data: new FormData(),
                dataType: 'html',
                contentType: false,
                processData: false,
                success: function (data) {
                    if (methodName === 'Login_Submit' || methodName === 'TestFlow2020_StartIt') {
                        localStorage.setItem('Token', getQueryFromTargetStr(data, 'Token'))
                        jsonString = deleteUrlParam(data, 'Token')
                    } else {
                        jsonString = data;
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random();
                    ThrowMakeErrInfo("HttpHandler-DoMethodReturnString-" + methodName, textStatus, url);


                }
            });
            return jsonString;
        },

        DoMethodReturnString: function (methodName) {
            methodName = filterXSS(methodName)
            if (dynamicHandler == "")
                dynamicHandler = basePath + "/WF/Comm/ProcessRequest";
            var self = this;
            var jsonString;
            // 如果没有携带token， 自动补上
            if (!parameters.has('Token')) {
                parameters.append('Token', GetQueryString('Token'))
            }
            if (methodName === 'Login_Submit' || methodName === 'Login_AdminOnlySaas' ||  methodName === 'ChangePassword_Submit') {
                var isEncrypt = this.customRequest("CheckEncryptEnable")
                var key = "TB_PW"
                if (isEncrypt === '0') {
                    var encryptStr = encodeURIComponent(parameters.get(key));
                    parameters.delete(key);
                    parameters.append(key, encryptStr)
                } else if (isEncrypt === '1') {
                    var encryptStr = md5(parameters.get(key)).toUpperCase();
                    parameters.delete(key);
                    parameters.append(key, encryptStr)
                }
            }
            if (IsIELower10 == false)
                $.ajax({
                    type: 'post',
                    async: false,
                    url: dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random(),
                    data: parameters,
                    dataType: 'html',
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        if (methodName === 'Login_Submit' || methodName === 'TestFlow2020_StartIt') {
                            localStorage.setItem('Token', getQueryFromTargetStr(data, 'Token'))
                            jsonString = deleteUrlParam(data, 'Token')
                        } else {
                            jsonString = data;
                        }

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random();
                        ThrowMakeErrInfo("HttpHandler-DoMethodReturnString-" + methodName, textStatus, url);


                    }
                });
            else
                $.ajax({
                    type: 'post',
                    async: false,
                    url: dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random(),
                    data: parameters,
                    dataType: 'html',
                    success: function (data) {
                        if (methodName === 'Login_Submit') {
                            localStorage.setItem('Token', getQueryFromTargetStr(data, 'Token'))
                            jsonString = deleteUrlParam(data, 'Token')
                        } else {
                            jsonString = data;
                        }

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        var url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + methodName + "&HttpHandlerName=" + self.handlerName + "&t=" + Math.random();
                        ThrowMakeErrInfo("HttpHandler-DoMethodReturnString-" + methodName, textStatus, url);


                    }
                });
            return jsonString;

        },

        DoMethodReturnJSON: function (methodName) {

            var jsonString = this.DoMethodReturnString(methodName);

            if (jsonString.indexOf("err@") == 0) {
                alert(jsonString);

                //alert('请查看控制台(DoMethodReturnJSON):' + jsonString);
                console.log(jsonString);
                return jsonString;
            }

            try {

                jsonString = ToJson(jsonString);

                //jsonString = JSON.parse(jsonString);
            } catch (e) {
                jsonString = "err@json解析错误: " + jsonString;
                alert(jsonString);
                //  console.log(jsonString);
            }
            return jsonString;
        }
    }
    return HttpHandler;

})();

var webUserJsonString = null;
var WebUser = function () {
    if (dynamicHandler == "")
        return;
    if (webUserJsonString != null) {
        var self = this;
        $.each(webUserJsonString, function (n, o) {
            self[n] = filterXSS(o);
        });
        return;
    }
    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";
    //获得页面上的token. 在登录信息丢失的时候，用token重新登录.
    var token = GetQueryString('Token');
    $.ajax({
        type: 'post',
        async: false,
        url: dynamicHandler + "?DoType=WebUser_Init&Token=" + token + "&t=" + new Date().getTime(),
        dataType: 'html',
        success: function (data) {

            if (data.indexOf("err@") != -1) {
                if (data.indexOf('登录信息丢失') != -1) {
                    alert("登录信息丢失，请重新登录。");
                } else {
                    alert(data);
                }
                try {
                    if (!!window.top && !!window.top.vm)
                        window.top.vm.logoutExt();
                    else {
                        if (GetHrefUrl().indexOf("Portal/Standard/") != -1)
                            SetHref(basePath + "/Portal/Standard/Login.htm");
                    }
                } catch (e) {
                    //可能出现跨域
                    //SetHref(basePath + "/Portal/Standard/Login.htm");
                }

                return;
            }

            try {
                webUserJsonString = JSON.parse(filterXSS(data));
                localStorage.setItem('Token', webUserJsonString.Token);

            } catch (e) {
                alert("json解析错误: " + data);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var url = dynamicHandler + "?DoType=WebUser_Init&t=" + new Date().getTime();
            ThrowMakeErrInfo("WebUser-WebUser_Init", textStatus, url);
        }
    });
    var self = this;
    if (webUserJsonString != null)
        $.each(webUserJsonString, function (n, o) {
            self[n] = filterXSS(o);
        });
};

var guestUserJsonString = null;
var GuestUser = function () {
    if (dynamicHandler == "")
        return;
    if (guestUserJsonString != null) {
        var self = this;
        $.each(guestUserJsonString, function (n, o) {
            self[n] = o;
        });
        return;
    }
    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";
    $.ajax({
        type: 'post',
        async: false,
        url: dynamicHandler + "?DoType=GuestUser_Init&t=" + new Date().getTime(),
        dataType: 'html',
        success: function (data) {

            if (data.indexOf("err@") != -1) {
                if (data.indexOf('登录信息丢失') != -1) {
                    alert("登录信息丢失，请重新登录。");
                } else {
                    alert(data);
                }
                return;
            }

            try {
                guestUserJsonString = JSON.parse(data);

            } catch (e) {
                alert("json解析错误: " + data);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var url = dynamicHandler + "?DoType=GuestUser_Init&t=" + new Date().getTime();
            ThrowMakeErrInfo("GuestUser_Init", textStatus, url);
        }
    });
    var self = this;
    $.each(guestUserJsonString, function (n, o) {
        self[n] = o;
    });

};

function ThrowMakeErrInfo(funcName, textStatus, url, XMLHttpRequest, errorThrown) {
    if (url == undefined || url == null) url = '';
    var msg = "1. " + funcName + " err@系统发生异常.";
    msg += "\t\n2.检查请求的URL连接是否错误：" + url;
    msg += "\t\n3.估计是数据库连接错误或者是系统环境问题. ";
    msg += "\t\n4.技术信息";

    if (textStatus != null && textStatus != undefined)
        msg += " \t\m  textStatus: " + JSON.stringify(textStatus);

    if (errorThrown != null && errorThrown != undefined)
        msg += " \t\mt  errorThrown: " + JSON.stringify(errorThrown);

    if (XMLHttpRequest != null && XMLHttpRequest != undefined)
        msg += " \t\m  XMLHttpRequest: " + JSON.stringify(XMLHttpRequest);

    msg += "\t\n5.您要打开执行的handler查看错误吗？ ";
    // msg += "\t\n5 您可以执行一下http://127.0.0.1/WF/Default.aspx/jsp/php 测试一下，动态文件是否可以被执行。";

    if (url.indexOf('WF/WF/') != -1)
        msg += "您没有配置项目名称,请仔细阅读配置连接";

    if (url != null) {
        if (window.confirm(msg) == true) {
            WinOpen(url);
            return;
        }
        return;
    }
    alert(msg);
}

//替换全部.
function replaceAll(str, oldKey, newKey) {

    if (str == null || str == undefined) {
        alert("要替换的原始字符串为空.");
        return str;
    }
    str = str.replace(new RegExp(oldKey, "gm"), newKey);
    return str;
}


//date = new Date();
//date = FormatDate(date, "yyyy-MM-dd");
function FormatDate(now, mask) {
    var d = now;
    var zeroize = function (value, length) {
        if (!length) length = 2;
        value = String(value);
        for (var i = 0, zeros = ''; i < (length - value.length); i++) {
            zeros += '0';
        }
        return zeros + value;
    }

    return mask.replace(/"[^"]*"|'[^']*'|\b(?:d{1,4}|m{1,4}|yy(?:yy)?|([hHMstT])\1?|[lLZ])\b/g, function ($0) {
        switch ($0) {
            case 'd': return d.getDate();
            case 'dd': return zeroize(d.getDate());
            case 'ddd': return ['Sun', 'Mon', 'Tue', 'Wed', 'Thr', 'Fri', 'Sat'][d.getDay()];
            case 'dddd': return ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'][d.getDay()];
            case 'M': return d.getMonth() + 1;
            case 'MM': return zeroize(d.getMonth() + 1);
            case 'MMM': return ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'][d.getMonth()];
            case 'MMMM': return ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'][d.getMonth()];
            case 'yy': return String(d.getFullYear()).substr(2);
            case 'yyyy': return d.getFullYear();
            case 'h': return d.getHours() % 12 || 12;
            case 'hh': return zeroize(d.getHours() % 12 || 12);
            case 'H': return d.getHours();
            case 'HH': return zeroize(d.getHours());
            case 'm': return d.getMinutes();
            case 'mm': return zeroize(d.getMinutes());
            case 's': return d.getSeconds();
            case 'ss': return zeroize(d.getSeconds());
            case 'l': return zeroize(d.getMilliseconds(), 3);
            case 'L': var m = d.getMilliseconds();
                if (m > 99) m = Math.round(m / 10);
                return zeroize(m);
            case 'tt': return d.getHours() < 12 ? 'am' : 'pm';
            case 'TT': return d.getHours() < 12 ? 'AM' : 'PM';
            case 'Z': return d.toUTCString().match(/[A-Z]+$/);
            // Return quoted strings with the surrounding quotes removed
            default: return $0.substr(1, $0.length - 2);
        }
    });
}

//表达式的替换.
function DealExp(expStr, webUser, isDtlField) {

    if (expStr.indexOf('@') == -1)
        return expStr;

    if (webUser == null || webUser == undefined)
        webUser = new WebUser();

    //替换表达式常用的用户信息
    expStr = expStr.replace('@WebUser.No', webUser.No);
    expStr = expStr.replace('@WebUser.Name', webUser.Name);
    expStr = expStr.replace("@WebUser.FK_DeptNameOfFull", webUser.FK_DeptNameOfFull);
    expStr = expStr.replace('@WebUser.FK_DeptName', webUser.FK_DeptName);
    expStr = expStr.replace('@WebUser.FK_Dept', webUser.FK_Dept);
    expStr = expStr.replace('@WebUser.OrgNo', webUser.OrgNo);
    expStr = expStr.replace('@WebUser.OrgName', webUser.OrgName);
    if (expStr.indexOf('@') == -1)
        return expStr;

    var objs = document.all;
    if (isDtlField != undefined && isDtlField == true)
        objs = window.parent.document.all;
    var length1;
    for (var i = 0; i < objs.length; i++) {

        if (expStr.indexOf('@') == -1)
            return expStr;

        var obj = objs[i].tagName;
        if (obj == null)
            continue;


        //把标签名转换为小写
        obj = obj.toLowerCase();
        if (obj != "input" && obj != "select")
            continue;
        //获取节点的ID 和值
        var NodeID = objs[i].getAttribute("id");
        if (NodeID == null)
            continue;
        var NodeType = objs[i].getAttribute("type");
        var NodeValue = objs[i].value;
        if (obj != "input" && (NodeType == "text" || NodeType == "radio" || NodeType == "checkbox")) {
            NodeValue = objs[i].value;
            if (NodeType == "checkbox") {
                NodeValue = 0;
                var isChecked = NodeID.is(":checked");
                if (isChecked == true)
                    NodeValue = 1;
            }
            if (NodeType == "radio") {
                var nodeName = objs[i].getAttribute("name");
                NodeValue = $("input:radio[name='" + nodeName + "']:checked").val();
            }

        } else if (obj == "select") {
            NodeValue = decodeURI(objs[i].value);
        }
        var key = "@" + NodeID.substring(NodeID.indexOf("_") + 1);
        expStr = expStr.replace(new RegExp(key, 'g'), NodeValue);
    }

    return expStr;
}

function DealJsonExp(json, expStr, webUser) {
    if (webUser == null || webUser == undefined)
        webUser = new WebUser();

    //替换表达式常用的用户信息
    expStr = expStr.replace('@WebUser.No', webUser.No);
    expStr = expStr.replace('@WebUser.Name', webUser.Name);
    expStr = expStr.replace("@WebUser.FK_DeptNameOfFull", webUser.FK_DeptNameOfFull);
    expStr = expStr.replace('@WebUser.FK_DeptName', webUser.FK_DeptName);
    expStr = expStr.replace('@WebUser.FK_Dept', webUser.FK_Dept);
    expStr = expStr.replace('@WebUser.OrgNo', webUser.OrgNo);
    expStr = expStr.replace('@WebUser.OrgName', webUser.OrgName);


    if (expStr.indexOf('@') == -1)
        return expStr;
    if (json == null)
        return expStr;
    $.each(json, function (n, val) {
        if (expStr.indexOf("@") == -1)
            return;
        //(str, oldKey, newKey)
        expStr = replaceAll(expStr, "@" + n, val);
    });
    return expStr;
}

//根据AtPara例如AtPara=@Helpurl=XXX@Count=XXX,获取HelpUrl的值
function GetPara(atPara, key) {
    if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
        return undefined;
    }
    var reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
    var results = atPara.match(reg);
    if (results != null) {
        return unescape(results[2]);
    }
    return undefined;

}

//用户处理日志
function UserLogInsert(logType, logMsg, userNo) {

    if (userNo == null || userNo == undefined) {
        if (loadWebUser == null)
            loadWebUser = new WebUser();
        userNo = loadWebUser.No;
    }
    var userLog = new Entity("BP.Sys.UserLog");
    userLog.FK_Emp = userNo;
    userLog.LogFlag = logType;
    userLog.Docs = logMsg;
    userLog.Insert();

}

function SFTaleHandler(url) {
    //获取当前网址，如： http://localhost:80/jflow-web/index.jsp  
    var curPath = GetHrefUrl();
    //获取主机地址之后的目录，如： jflow-web/index.jsp  
    var pathName = window.document.location.pathname;
    var pos = curPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:80  
    var localhostPaht = curPath.substring(0, pos);
    //获取带"/"的项目名，如：/jflow-web
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);

    var localpath = localhostPaht + projectName;
    if (plant == "CCFlow") {
        // CCFlow
        dynamicHandler = localhostPaht + "/DataUser/SFTableHandler.ashx";
    } else {
        // JFlow
        dynamicHandler = localpath + "/DataUser/SFTableHandler/";
    }
    var jsonString = "";

    if (url.indexOf("?") == -1)
        url = url + "?1=1";

    url = dynamicHandler + url + "&t=" + new Date().getTime();
    $.ajax({
        type: 'post',
        async: false,
        url: url,
        dataType: 'html',
        success: function (data) {
            if (data.indexOf("err@") != -1) {
                alert(data);
                jsonString = "false";
            }

            jsonString = data;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(URL + "err@系统发生异常, status: " + XMLHttpRequest.status + " readyState: " + XMLHttpRequest.readyState);
        }
    });

    return jsonString;
}

function validate(s) {
    if (s == null || typeof s === "undefined") {
        return false;
    }
    s = s.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
    if (s == "" || s == "null" || s == "undefined") {
        return false;
    }
    return true;
}

var loadWebUser = null;

//初始化页面
$(function () {
    var ver = IEVersion();
    if (ver == 6 || ver == 7 || ver == 8 || ver == 9) {
        jQuery.getScript(basePath + "/WF/Scripts/jquery.XDomainRequest.js")
    }
    dynamicHandler = basePath + "/WF/Comm/ProcessRequest";
    var url = GetHrefUrl().toLowerCase();
    var pageName = window.document.location.pathname.toLowerCase();
    pageName = pageName.substring(pageName.lastIndexOf("/") + 1);

    //不需要权限信息
    var listPage = ['login.htm', 'selectoneorg.htm', 'dbinstall.htm', 'scanguide.htm', 'qrcodescan.htm', 'index.htm', 'gotourl.htm', 'invited.htm', 'registerbywebsite.htm', 'reqpassword.htm', 'reguser.htm', 'port.htm', 'ccbpm.cn/', 'loginwebsite.htm', 'goto.htm', 'do.htm'];
    if (listPage.includes(pageName) || url == basePath) {
        localStorage.setItem('Token', '');
        return;
    }

    loadWebUser = new WebUser();

    if (loadWebUser != null && (loadWebUser.No == "" || loadWebUser.No == undefined || loadWebUser.No == null)) {
        dynamicHandler = "";
        alert("登录信息丢失,请重新登录.");
        return;
    }
    //要排除的目录.
    if (url.indexOf("/admin/TestingContainer/") == -1)
        return;
    //如果进入了管理员目录.
    if (url.indexOf("/admin/") != -1 && loadWebUser.IsAdmin != 1) {
        dynamicHandler = "";
        alert("管理员登录信息丢失,请重新登录,当前用户[" + loadWebUser.No + "]不能操作管理员目录功能.");
        return;
    }

});

/**
 * 子页面跨域调用父页面方法
 * @param {any} info
 * @param {any} action
 */
function ChildrenPostMessage(info, action) {
    //获取当前子页面的URL
    var curPath = window.location.href;
    var pathName = window.document.location.pathname;
    var pos = curPath.indexOf(pathName);
    var localhostPath = curPath.substring(0, pos);
    window.postMessage({ action: action, info: info }, localhostPath);
}

/**
 * 按照MapAttrs的规范去，处理jsonDT的大小写.
 * @param {数据集合} jsonDT
 * @param {属性集合} mapAttrs
 */
function DealDataTableColName(jsonDT, mapAttrs) {

    var data = {};
    //遍历数据源的列.
    for (colName in jsonDT) {

        var val = jsonDT[colName];

        // alert("colName:[" + colName + "] val:[" + val + "]");
        //找到。
        var isHave = false;
        for (var i = 0; i < mapAttrs.length; i++) {

            var mapAttr = mapAttrs[i];

            if (mapAttr.KeyOfEn.toUpperCase() == colName.toUpperCase()) {

                if (val == null || val == "" || val == " ") {

                    //如果是数值类型的就让其为 0. 不然会填充错误，保存错误。
                    if (mapAttr.MyDataType == 2 //int 
                        || mapAttr.MyDataType == 3 //AppFloat
                        || mapAttr.MyDataType == 4 // boolen
                        || mapAttr.MyDataType == 5) { //AppDouble
                        val = 0;
                    }

                    if (mapAttr.MyDataType == 8) //AppMoney
                        val = "0.00";
                }
                data[mapAttr.KeyOfEn] = val; //jsonDT[colName];
                isHave = true;
                break;
            } else {
                data[colName] = val;
            }
        }

        //if (isHave == false) {
        //   alert("数据源字段名[" + colName + "]没有匹配到表单字段.");
        //}
    }
    return data;
}

/**
 * 通用配置的获取
 * @param {any} key  变量
 * @param {any} defVal 默认值
 */
function getConfigByKey(key, defVal) {

    if (typeof CommonConfig == "undefined") {
        CommonConfig = {};
        CommonConfig[key] = defVal;
        return defVal;
    }
    if (CommonConfig[key] == undefined)
        CommonConfig[key] = defVal;
    var val = CommonConfig[key];
    if (typeof val == 'string' && val.indexOf("@") != -1)
        val = DealJsonExp(null, val);
    return val;
}
/**
 * 对象数组分组
 * @param {any} array
 * @param {any} f
 */
function groupBy(array, f) {
    const groups = {};
    $.each(array, function (i, o) {
        const group = f(o);
        groups[group] = groups[group] || [];
        groups[group].push(o);
    });
    return groups;
}

//文件下载
function downLoadExcel(url) {
    var link = document.createElement('a');
    link.setAttribute("download", "");
    link.href = url;
    link.click();
}

