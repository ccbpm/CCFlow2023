using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// ѧ�� ����
    /// </summary>
    public class StudentAttr : EntityNoNameAttr
    {
        #region ��������
        /// <summary>
        /// �Ա�
        /// </summary>
        public const string XB = "XB";
        /// <summary>
        /// ��ַ
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public const string PWD = "PWD";
        /// <summary>
        /// �༶
        /// </summary>
        public const string FK_BanJi = "FK_BanJi";
        /// <summary>
        /// ����
        /// </summary>
        public const string Age = "Age";
        /// <summary>
        /// �ʼ�
        /// </summary>
        public const string Email = "Email";
        /// <summary>
        /// �绰
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// ע��ʱ��
        /// </summary>
        public const string RegDate = "RegDate";
        /// <summary>
        /// ��ע
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// �Ƿ���������
        /// </summary>
        public const string IsTeKunSheng = "IsTeKunSheng";
        /// <summary>
        /// �Ƿ����ش󼲲�ʷ��
        /// </summary>
        public const string IsJiBing = "IsJiBing";
        /// <summary>
        /// �Ƿ�ƫԶɽ����
        /// </summary>
        public const string IsPianYuanShanQu = "IsPianYuanShanQu";
        /// <summary>
        /// �Ƿ������
        /// </summary>
        public const string IsDuShengZi = "IsDuShengZi";
        /// <summary>
        /// ������ò
        /// </summary>
        public const string ZZMM = "ZZMM";
        /// <summary>
        /// ¼����
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// ¼��������
        /// </summary>
        public const string RecName = "RecName";
        #endregion

        /// <summary>
        /// Ƭ��
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// ʡ��
        /// </summary>
        public const string FK_SF = "FK_SF";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_City = "FK_City";
    }
    /// <summary>
    /// ѧ��
    /// </summary>
    public class Student : BP.En.EntityNoName
    {
        #region ����
        /// <summary>
        /// ��¼ϵͳ����
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.PWD);
            }
            set
            {
                this.SetValByKey(StudentAttr.PWD, value);
            }
        }

        /// <summary>
        /// ��ַ
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Addr);
            }
            set
            {
                this.SetValByKey(StudentAttr.Addr, value);
            }
        }
        /// <summary>
        /// �Ա�
        /// </summary>
        public int XB
        {
            get
            {
                return this.GetValIntByKey(StudentAttr.XB);
            }
            set
            {
                this.SetValByKey(StudentAttr.XB, value);
            }
        }
        /// <summary>
        /// �Ա�����
        /// </summary>
        public string XBText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.XB);
            }
        }
        /// <summary>
        /// �༶���
        /// </summary>
        public string FK_BanJi
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.FK_BanJi);
            }
            set
            {
                this.SetValByKey(StudentAttr.FK_BanJi, value);
            }
        }
        /// <summary>
        /// �༶����
        /// </summary>
        public string FK_BanJiText
        {
            get
            {
                return this.GetValRefTextByKey(StudentAttr.FK_BanJi);
            }
        }
        /// <summary>
        /// �ʼ�
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Email);
            }
            set
            {
                this.SetValByKey(StudentAttr.Email, value);
            }
        }
        /// <summary>
        /// �绰
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.Tel);
            }
            set
            {
                this.SetValByKey(StudentAttr.Tel, value);
            }
        }
        /// <summary>
        /// ע������
        /// </summary>
        public string RegDate
        {
            get
            {
                return this.GetValStringByKey(StudentAttr.RegDate);
            }
            set
            {
                this.SetValByKey(StudentAttr.RegDate, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ʵ���Ȩ�޿���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                //  uac.LoadRightFromCCGPM(this); //��GPM����װ��.
                // return uac;
                //uac.OpenAllForStation("001,002");
                //return uac;

                if (BP.Web.WebUser.No.Equals("admin") == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    uac.IsView = true;
                }
                else
                {
                    uac.IsView = true;
                }
                uac.IsImp = true;
                return uac;
            }
        }
        /// <summary>
        /// ѧ��
        /// </summary>
        public Student()
        {
        }
        /// <summary>
        /// ѧ��
        /// </summary>
        /// <param name="no"></param>
        public Student(string no)
            : base(no)
        {
        }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_Student", "ѧ��");
                //������Ϣ.
                map.ItIsAllowRepeatName = true; //�Ƿ����������ظ�.
                map.ItIsAutoGenerNo = true; //�Ƿ��Զ����ɱ��.
                map.setCodeStruct( "4"); // 4λ���ı�ţ��� 0001 ��ʼ���� 9999. 
                map.DepositaryOfEntity = Depositary.None; //���λ��.None=���ݱ��� Application=����.

                #region �ֶ�ӳ�� - ��ͨ�ֶ�.
                map.AddGroupAttr("��ͨ�ֶ�");
                map.AddTBStringPK(StudentAttr.No, null, "ѧ��", true, true, 4, 4, 90); // ��������Զ�����ֶα�����ֻ����.
                map.AddTBString(StudentAttr.Name, null, "����", true, false, 0, 200, 70);
                map.AddTBString(StudentAttr.PWD, null, "����", true, false, 0, 200, 50);
                map.AddTBInt(StudentAttr.Age, 18, "����", true, false);
                map.AddTBString(StudentAttr.Addr, null, "��ַ", true, false, 0, 200, 100, true);
                map.AddTBString(StudentAttr.Tel, null, "�绰", true, false, 0, 200, 100);
                map.AddTBString(StudentAttr.Email, null, "�ʼ�", true, false, 0, 200, 100);
                map.AddTBDateTime(StudentAttr.RegDate, null, "ע������", true, true);
                map.AddBoolean(StudentAttr.IsDuShengZi, false, "�Ƿ��Ƕ����ӣ�", true, true, true);
                map.AddBoolean(StudentAttr.IsJiBing, false, "�Ƿ����ش󼲲���", true, true, true);
                map.AddBoolean(StudentAttr.IsPianYuanShanQu, false, "�Ƿ�ƫԶɽ����", true, true);
                map.AddBoolean(StudentAttr.IsTeKunSheng, false, "�Ƿ�����������", true, true);
                map.AddTBStringDoc(ResumeAttr.BeiZhu, null, "��ע", true, false);

                map.AddTBString(StudentAttr.RecNo, null, "¼���˱��", true, true, 0, 200, 100); //��������.
                map.AddTBString(StudentAttr.RecName, null, "¼��������", false, false, 0, 200, 100);//��������.
                map.AddTBAtParas(2000);
                #endregion �ֶ�ӳ�� - ��ͨ�ֶ�.

                #region ���ö���ֶ�.
                map.AddGroupAttr("���ö���ֶ�");
                map.AddDDLSysEnum(StudentAttr.ZZMM, 0, "������ò", true, true, StudentAttr.ZZMM,
                    "@0=���ȶ�Ա@1=��Ա@2=��Ա@3=����");
                map.AddRadioBtnSysEnum(StudentAttr.XB, 0, "�Ա�", true, true, StudentAttr.XB, "@0=Ů@1=��");
                //����ֶ�.
                map.AddDDLEntities(StudentAttr.FK_BanJi, null, "�༶", new BP.Demo.BanJis(), true);
                //string sql = "SELECT No,Name FROM CN_SF "; //���sql������֧�ֱ���ʽ @WebUser.* , Ҳ�����Ǳ�ʵ����������Ʊ��� @No.
                //map.AddDDLSQL(StudentAttr.FK_SF, null, "ʡ��", sql, true);

                //map.AddDDLEntities(StudentAttr.FK_PQ, null, "Ƭ��",new BP.CN.PQs(),true);
                //map.AddDDLEntities(StudentAttr.FK_SF, null, "ʡ��",new BP.CN.SFs(),true);
                //map.AddDDLEntities(StudentAttr.FK_City, null, "����",new BP.CN.Citys(),true);
                #endregion ���ö���ֶ�.

                map.AddMyFileS("����");//�ϴ�������

                #region ���ò�ѯ������
                //   map.ItIsShowSearchKey = false; //�Ƿ���ʾ�ؼ��ֲ�ѯ��Ĭ����ʾ���ؼ��ֲ�ѯƥ���κ��С�
                //String�ֶ����͵�ģ����ѯ�����巽ʽmap.SearchFields,�丳ֵ��ʽ��@����=�ֶ�Ӣ����
                //��������ø��ֶ�����йؼ��ֲ�ѯ������string�ֶε�ģ����ѯ
                // map.SearchFields = "@����=Name@��ַ=Addr@�绰=Tel";
                //��ֵ���ֶβ�ѯ�����巽ʽmap.SearchFieldsOfNum���丳ֵ��ʽ��@����=�ֶ�Ӣ����
                //��ѯ��ʽ�Ǵ�Age1��Age2�׶β�ѯ��
                //�����Age1��ֵ��Age2��ֵ�����ѯ���ڵ���Age1�Ľ����
                //�����Age1��ֵ��Age2��ֵ�����ѯС�ڵ���Age2�Ľ����
                //�����Age1��ֵ��Age2��ֵ�����ѯ���ڵ���Age1С�ڵ���Age2�Ľ����

                //��ֵ��Χ��ѯ
                map.SearchFieldsOfNum = "@����=Age";
                //���ڲ�ѯ.
                map.DTSearchKey = "RegDate";
                map.DTSearchLabel = "ע������";
                map.DTSearchWay = Sys.DTSearchWay.ByYearMonth;
                //����Search.htmҳ���ѯ�������еĹ��������ӵĲ�ѯ�ֶεĿ��ȳ���4000������
                map.AddSearchAttr(StudentAttr.XB);
                map.AddSearchAttr(StudentAttr.ZZMM);
                map.AddSearchAttr(StudentAttr.FK_BanJi);
                //���������Ĳ�ѯ: ������ѯ��¼���.
                //  map.AddHidden(StudentAttr.RecNo, " = ", "@WebUser.No");
                #endregion ���ò�ѯ����

                #region �������� - ����.
                map.AddGroupMethod("��������");
                ////��Զ��ӳ��.
                //map.AttrsOfOneVSM.Add(new StudentKeMus(), new KeMus(), StudentKeMuAttr.FK_Student,
                //  StudentKeMuAttr.FK_KeMu, KeMuAttr.Name, KeMuAttr.No, "ѡ�޵Ŀ�Ŀ");
                //��ѯģʽ.
                map.AddDtl(new Resumes(), ResumeAttr.StudentNo, null, DtlEditerModel.DtlSearch, "icon-drop");
                //�����༭ģʽ
                map.AddDtl(new Resumes(), ResumeAttr.StudentNo, null, DtlEditerModel.DtlBatch, "icon-drop");
                //���в����ķ���.
                RefMethod rm = new RefMethod();
                rm.Title = "���ɰ��";
                rm.HisAttrs.AddTBDecimal("JinE", 100, "���ɽ��", true, false);
                rm.HisAttrs.AddTBString("Note", null, "��ע", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoJiaoNaBanFei";
                //  rm.ItIsCanBatch = false; //�Ƿ������������
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "ע��ѧ��";
                rm.Warning = "��ȷ��Ҫע����";
                rm.ClassMethodName = this.ToString() + ".DoZhuXiao";
                rm.ItIsForEns = true;
                rm.ItIsCanBatch = true; //�Ƿ������������
                map.AddRefMethod(rm);
                #endregion �������� - ����.

                #region �߼����� - ����.
                map.AddGroupMethod("�߼�����");
                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "����Ȱ������";
                rm.ClassMethodName = this.ToString() + ".DoStartFlow";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ItIsCanBatch = false; //�Ƿ������������
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "��ӡѧ��֤";
                rm.ClassMethodName = this.ToString() + ".DoPrintStuLicence";
                rm.ItIsCanBatch = true; //�Ƿ������������
                map.AddRefMethod(rm);

                //�����в����ķ���.
                rm = new RefMethod();
                rm.Title = "������ҳ����ʾ";
                rm.ClassMethodName = this.ToString() + ".DoOpenit";
                rm.ItIsCanBatch = true; //�Ƿ������������
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion �߼����� - ����.

                ////�����в����ķ���.
                //rm = new RefMethod();
                //rm.Title = "������ӡѧ��֤";
                //rm.ClassMethodName = this.ToString() + ".EnsMothed";
                ////rm.ItIsForEns = true; //�Ƿ������������
                //rm.RefMethodType = RefMethodType.FuncBacthEntities; //�Ƿ������������
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoOpenit()
        {
            return "/WebForm1.aspx?No=" + this.No;
        }
        /// <summary>
        /// ��д����ķ���.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            //�ڲ���֮ǰ����ע��ʱ��.
            this.RegDate = DataType.CurrentDateTime;
            this.SetValByKey(StudentAttr.RecNo, BP.Web.WebUser.No); //���ü�¼��.
            this.SetValByKey(StudentAttr.RecName, BP.Web.WebUser.Name); //���ü�¼��.
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (this.Email.Length == 0)
                throw new Exception("@email ����Ϊ��.");

            return base.beforeUpdateInsertAction();
        }

        #endregion ��д���෽��

        #region ����
        public string DoPrintStuLicence()
        {
            BP.Pub.RTFEngine en = new BP.Pub.RTFEngine();
            Student stu = new Student(this.No);
            en.HisGEEntity = stu; //��ǰ��ʵ��.
            //���Ӵӱ�.
            BP.Demo.Resumes dtls = new Resumes();
            dtls.Retrieve(ResumeAttr.StudentNo, stu.No);
            en.AddDtlEns(dtls);

            string saveTo = BP.Difference.SystemConfig.PathOfTemp; // \\DataUser\\Temp\\
            string billFileName = this.No + "StuTest.doc";

            //Ҫ���ɵ�����.
            en.MakeDoc(BP.Difference.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\StudentDemo.rtf", saveTo, billFileName);

            string url = "/DataUser/Temp/" + billFileName;

            string info = "�������ɳɹ�:<a href='" + url + "' >��ӡ</a>��<a href='/SDKFlowDemo/App/PrintJoin.aspx'>ƴ�Ӵ�ӡ</a>";
            return info;
        }
        public string DoStartFlow()
        {
            return "/WF/MyFlow.htm?FK_Flow=045&XH=" + this.No + "&XM=" + this.Name;
        }
        /// <summary>
        /// ���в����ķ���:���ɰ��
        /// ˵������Ҫ����string����.
        /// </summary>
        /// <returns></returns>
        public string DoJiaoNaBanFei(decimal jine, string note)
        {
            return "ѧ��:" + this.No + ",����:" + this.Name + ",������:" + jine + "Ԫ,˵��:" + note;
        }
        /// <summary>
        /// �޲����ķ���:ע��ѧ��
        /// ˵������Ҫ����string����.
        /// </summary>
        /// <returns></returns>
        public string DoZhuXiao()
        {
            //    DBAccess.RunSQL("DELETE RR");
            //   DataTable DT=    DBAccess.RunSQLReturnTable("elect * from ");
            return "ѧ��:" + this.No + ",����:" + this.Name + ",�Ѿ�ע��.";
        }
        /// <summary>
        /// У������
        /// </summary>
        /// <param name="pass">ԭʼ����</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool CheckPass(string pass)
        {
            return this.PWD.Equals(pass);
        }
        #endregion

        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }

    }
    /// <summary>
    /// ѧ��s
    /// </summary>
    public class Students : BP.En.EntitiesNoName
    {
        #region ����
        /// <summary>
        /// ѧ��s
        /// </summary>
        public Students() { }
        #endregion

        #region ��д���෽��
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Student();
            }
        }
        #endregion ��д���෽��

        #region ���Է���.
        public string EnsMothed()
        {
            return "EnsMothed@ִ�гɹ�.";
        }
        public string EnsMothedParas(string para1, string para2)
        {
            return "EnsMothedParas@ִ�гɹ�." + para1 + " - " + para2;
        }
        #endregion

    }
}