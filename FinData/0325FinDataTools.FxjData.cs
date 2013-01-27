using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;


namespace FinData
{
    public class FxjData
    {
        public int Error
        {
            get
            {
                if (msg != "") return 1;
                else return 0;
            }
        }
        public enum DataTypes {dm,zxhq,hq,min,fp,gb,cw,gd,jjzb,jjzh,cq,cw0,fbcj};
        public FxjData()
        {

            //��ע����ж�ȡ����������Ŀ¼����c:\fxj\data
            RegistryKey keyFxj;
            RegistryKey keySoftware = Registry.LocalMachine.OpenSubKey("Software");
            keyFxj = keySoftware.OpenSubKey("FXJ");
            if (keyFxj == null)
            {
                keyFxj = keySoftware.OpenSubKey("Huitianqi");
                if (keyFxj == null)
                {
                    fxjDataPath = "";
                    return;
                }
            }
            RegistryKey keySuperstk = keyFxj.OpenSubKey("SUPERSTK");
            if (keySuperstk != null)
            {
                fxjDataPath = (string)keySuperstk.GetValue("InstPath");
                if (fxjDataPath != "")
                {
                    fxjDataPath += @"\DATA\";
                    fxjDataPath = fxjDataPath.ToUpper();
                    return;
                }
            }
        }
        public string Version
        {
            get
            {
                return ("0.5");
            }
        }
        public void ShowAbout()
        {
            MessageBox.Show("FinDataTools�����������ݶ�ȡ��� V" + Version+"\n\n        www.zhangwenzhang.com","����",MessageBoxButtons.OK);
        }
        struct fileStruct
        {
            public string fileName;//�ļ���
            public int startAddress,blockSize,recordSize;//��ʼ��ַ��ÿ�鳤�ȣ���¼����
            public bool codeIsLong, isIndexDataStruct;   //codeIsLong�����еĴ���������г�����SH��SZ��;isIndexDataStruct��Day.Dat�����Ľṹ��������+�������; 
            public string[,] fields;//�ֶ�
            public fileStruct(DataTypes fileType)
            {
                fileName = "";
                startAddress = 0;
                blockSize = 0;
                recordSize = 0;
                codeIsLong = false;
                isIndexDataStruct = true;
                string fieldString = ""; //�ֶ������ֶα�ǩ�����ͣ������ֶΣ��洢˳��ƫ����
                switch (fileType)
                {

                    #region �����stkinfo51.dat
                    case DataTypes.dm:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x845898;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"jc,���,string,32,1,10,;" +
"py,ƴ��,string,10,2,42,";
                        break;
                    #endregion
                    #region �ֺ�����stkinfo51.dat
                    case DataTypes.cq:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x14;
                        blockSize = 2116;
                        recordSize = 20;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,����,date,4,0,0,;" +
"sgsl,�͹�,single,4,1,4,;"+
"pgsl,�������,single,4,2,8,;"+
"pgjg,��ɼ۸�,single,4,3,12,;"+
"fh,�ֺ�,single,4,4,16,";
                        break;
                    #endregion
                    #region �������ݣ��򵥣�stkinfo51.dat
                    case DataTypes.cw0:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x794;
                        blockSize = 2116;
                        recordSize = 196;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,��������,date,4,38,192,;" +
"zgb,�ܹɱ�,single,4,0,0,;" +
"gjg,���ҹ�,single,4,1,4,;"+
"fqrg,�����˷��˹�,single,4,2,8,;"+
"frg,���˹�,single,4,3,12,;"+
"b,B��,single,4,4,16,;"+
"h,H��,single,4,5,20,;"+
"a,��ͨA��,single,4,6,24,;"+
"zgg,ְ����,single,4,7,28,;"+
"a2zpg,A2ת���,single,4,8,32,;"+
"bszzc,���ʲ�,single,4,9,36,;"+
"bsldzc,�����ʲ�,single,4,10,40,;"+
"bsgdzc,�̶��ʲ�,single,4,11,44,;"+
"bswxzc,�����ʲ�,single,4,12,48,;"+
"bscqtz,����Ͷ��,single,4,13,52,;"+
"bsldfz,������ծ,single,4,14,56,;"+
"bscqfz,���ڸ�ծ,single,4,15,60,;"+
"bszbgj,�ʱ�������,single,4,16,64,;"+
"mggjj,ÿ�ɹ�����,single,4,17,68,;"+
"bsgdqy,�ɶ�Ȩ��,single,4,18,72,;"+
"iszysl,��Ӫ����,single,4,19,76,;"+
"iszylr,��Ӫ����,single,4,20,80,;"+
"isqtlr,��������,single,4,21,84,;"+
"isyylr,Ӫҵ����,single,4,22,88,;"+
"istzsy,Ͷ������,single,4,23,92,;"+
"isbtsr,��������,single,4,24,96,;"+
"isyywsz,Ӫҵ����֧,single,4,25,100,;"+
"issytz,�����������,single,4,26,104,;"+
"islrze,�����ܶ�,single,4,27,108,;"+
"isshlr,˰������,single,4,28,112,;"+
"isjlr,������,single,4,29,116,;"+
"iswfplr,δ��������,single,4,30,120,;"+
"mgwfplr,ÿ��δ��������,single,4,31,124,;"+
"mgsy,ÿ������,single,4,32,128,;"+
"mgjzc,ÿ�ɾ��ʲ�,single,4,33,132,;"+
"tzmgjzc,����ÿ�ɾ��ʲ�,single,4,34,136,;"+
"gdqybl,�ɶ�Ȩ�����,single,4,35,140,;"+
"jzcsyl,���ʲ�������,single,4,36,144,";
                        //"unknown,(δ֪),string,44,37,148,;"

                        break;
                    #endregion
                    #region ��������stkinfo51.dat
                    case DataTypes.zxhq:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x845898;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"jc,���,string,32,1,10,;" +
"py,ƴ��,string,10,2,42,;" +
"gxsj,����ʱ��,datetime,4,5,60,;" +
"wrjl,���վ���,single,4,6,64,;" +
"zs,����,single,4,7,68,;" +
"jk,��,single,4,8,72,;" +
"zg,���,single,4,9,76,;" +
"zd,���,single,4,10,80,;" +
"zx,����,single,4,11,84,;" +
"zss,������,single,4,12,88,;" +
"je,���,single,4,13,92,;" +
"xss,������,single,4,14,96,;" +
"sbcj,�ϱʳɽ���,single,4,29,156,;" +
"dbcj,���ʳɽ���,single,4,30,160,;" +
"np,����,single,4,27,148,;" +
"wp,����,single,4,28,152,;" +
"mr1jg,��һ��,single,4,15,100,;" +
"mr1sl,��һ��,single,4,18,112,;" +
"mr2jg,�����,single,4,16,104,;" +
"mr2sl,�����,single,4,19,116,;" +
"mr3jg,������,single,4,17,108,;" +
"mr3sl,������,single,4,20,120,;" +
"mr4jg,���ļ�,single,4,32,168,;" +
"mr4sl,������,single,4,34,176,;" +
"mr5jg,�����,single,4,33,172,;" +
"mr5sl,������,single,4,35,180,;" +
"mc1jg,��һ��,single,4,21,124,;" +
"mc1sl,��һ��,single,4,24,136,;" +
"mc2jg,������,single,4,22,128,;" +
"mc2sl,������,single,4,25,140,;" +
"mc3jg,������,single,4,23,132,;" +
"mc3sl,������,single,4,26,144,;" +
"mc4jg,���ļ�,single,4,36,184,;" +
"mc4sl,������,single,4,38,192,;" +
"mc5jg,�����,single,4,37,188,;" +
"mc5sl,������,single,4,39,196,";
//"jd,����,int,4,3,52,;" +
//"scbz,ɾ����־,int,4,4,56,";
 //"unknown,(δ֪),int,4,31,164,;" +
//",(δ֪),,48,40,200,;"
                        break;
                    #endregion
                    #region �ֱʳɽ������ļ�report.dat���ṹͬday.dat��������һЩ���ݲ���ֱ�ӱ��棩
                    case DataTypes.fbcj:
                        fileName = "REPORT.DAT"; 
                        startAddress = 0x41000;
                        blockSize = 4068;
                        recordSize = 36;
                        codeIsLong = false;
                        isIndexDataStruct = false;//����ȫ��ͬ��day.dat�ṹ����˵�������
                        fieldString =
"dm,����,code,4,0,0,;" +
"rq,����,datetime,4,0,0,;" +
"zjcj,����ɽ���,single,4,1,4,;" +
"zss,������,single,4,2,8,calc;" +
"je,���,single,4,3,12,;" +
"xss,������,single,4,2,8,;" +
"mm,�����,string,2,16,35,;" +
"mr1sl,��һ��,single,2,4,16,;" +
"mr2sl,�����,single,2,5,18,;" +
"mr3sl,������,single,2,6,20,;" +
"mc1sl,��һ��,single,2,7,22,;" +
"mc2sl,������,single,2,8,24,;" +
"mc3sl,������,single,2,9,26,;" +
"mr1jg,��һ��,single,1,10,28,;" +
"mr2jg,�����,single,1,11,29,;" +
"mr3jg,������,single,1,12,30,;" +
"mc1jg,��һ��,single,1,13,31,;" +
"mc2jg,������,single,1,14,32,;" +
"mc3jg,������,single,1,15,33,";
                        //�����������Ͳ��Ǵ洢���ͣ������в�ֱ����ʵ���������ͣ���/��X��Ϊshort����/��X��Ϊbyte
                        //������ͨ����������������ã�Ӧ�÷�������������
                        break;
                    #endregion
                    #region ���������ļ�day.dat
                    case DataTypes.hq:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,����,date,4,1,0,;" +
"kp,����,single,4,2,4,B;" +
"zg,���,single,4,3,8,B;" +
"zd,���,single,4,4,12,B;" +
"sp,����,single,4,5,16,B;" +
"sl,�ɽ�����,single,4,6,20,A;"+
"je,�ɽ����,single,4,7,24,";
                        break;
                    #endregion
                    #region 5���������ļ�min.dat
                    case DataTypes.min:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,����,datetime,4,1,0,;" +
"kp,����,single,4,2,4,B;" +
"zg,���,single,4,3,8,B;" +
"zd,���,single,4,4,12,B;" +
"sp,����,single,4,5,16,B;" +
"sl,�ɽ�����,single,4,6,20,A;"+
"je,�ɽ����,single,4,7,24,";
                        break;
                    #endregion
                    #region �ֺ����������ļ�exprof.fdt
                    case DataTypes.fp:
                        fileName = "EXPROF.FDT";
                        startAddress = 0x41000;
                        blockSize = 3776;
                        recordSize = 236;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;"+
"cqrq,��Ȩ����,date,4,23,176,;" +
"sgbl,�͹ɱ���,double,8,1,12,;" +
"sgdjr,�͹ɹ�Ȩ�Ǽ���,date,4,2,20,;"+
"sgcqr,�͹ɳ�Ȩ��,date,4,3,24,;"+
"sgssr,���������,date,4,4,28,;"+
"zzbl,ת������,double,8,5,32,;"+
"zzdjr,ת����Ȩ�Ǽ���,date,4,6,40,;"+
"zzcqr,ת����Ȩ��,date,4,7,44,;"+
"zzssr,ת��������,date,4,8,48,;"+
"fhbl,�ֺ����,double,8,9,52,;"+
"fhdjr,�ֺ��Ȩ�Ǽ���,date,4,10,60,;" +
"fhcxr,�ֺ��Ϣ��,date,4,11,64,;" +
"fhpxr,�ֺ���Ϣ��,date,4,12,68,;" +
"pgbl,��ɱ���,double,8,13,72,;"+
"pgdjr,��ɹ�Ȩ�Ǽ���,date,4,14,80,;"+
"pgcqr,��ɳ�Ȩ��׼��,date,4,15,84,;"+
"pgjkqsr,��ɽɿ���ʼ��,date,4,16,88,;"+
"pgjkzzr,��ɽɿ���ֹ��,date,4,17,92,;"+
"pgssr,��ɿ���ͨ������,date,4,18,96,;"+
"pgjg,��ɼ۸�,single,4,19,100,;"+
"frgpgbl,���ڹ����÷��˹���ɱ���,double,8,20,104,;"+
"frgmgzrf,�Ϲ����˹����ÿ��ת�÷�,single,4,21,112,;"+
"pgzcxs,�����������,string,60,22,116,;"+
"bgrq,��������,date,4,24,180,;"+
"dshrq,���»�����,date,4,25,184,;"+
"gdhrq,�ɶ�������,date,4,26,188,;"+
"fhggrq,�ֺ칫������,date,4,27,192,;"+
"zgbjs,�ܹɱ�����,double,8,28,196,;"+
"sgsl,�͹�����,double,8,29,204,;"+
"zzsl,ת������,double,8,30,212,;"+
"sjpgs,ʵ���������,double,8,31,220,;"+
"cqhzgb,��Ȩ���ܹɱ�,double,8,32,228";

                        break;
                    #endregion
                    #region �ɱ��ṹCapital.fdt
                    case DataTypes.gb:
                        fileName = "CAPITAL.FDT";
                        startAddress = 0x41000;
                        blockSize = 3488;
                        recordSize = 218;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0;" +
"rq,����,date,4,17,214;"+
"zgb,�ܹɱ�,double,8,1,12;" +
"gjg,���ҹ�,double,8,2,20;" +
"fqrg,�����˹�,double,8,3,28;" +
"frg,���˹�,double,8,4,36;" +
"ybfrps,һ�㷨������,double,8,5,44;" +
"zgg,�ڲ�ְ����,double,8,6,52;" +
"a,��ͨA��,double,8,7,60;" +
"zltzag,ս��Ͷ��A��,double,8,8,68;" +
"zpg,ת���,double,8,9,76;" +
"jjps,��������,double,8,10,84;" +
"h,H��,double,8,11,92;" +
"b,B��,double,8,12,100;" +
"yxg,���ȹ�,double,8,13,108;" +
"ggcg,�߼�������Ա�ֹ�,double,8,14,116;" +
"gbbdyy,�ɱ��䶯ԭ��,string,56,15,124;" +
"gbbdyylb,�ɱ��䶯ԭ�����,string,34,16,180";

                        break;
                    #endregion 
                    #region ��������Finance.fdt
                    case DataTypes.cw:
                        fileName = "FINANCE.FDT";
                        startAddress = 0x41000;
                        blockSize = 14848;
                        recordSize = 464;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;"+
"rq,����,date,4,,460,;"+
"bsdqtzje,����Ͷ�ʾ���,double,8,1,12,;"+
"bsyszkje,Ӧ���ʿ��,double,8,2,20,;"+
"bschje,�������,double,8,3,28,;"+
"bsldzc,�����ʲ�,double,8,4,36,;"+
"bscqtzje,����Ͷ�ʾ���,double,8,5,44,;"+
"bsgdzc,�̶��ʲ�,double,8,6,52,;"+
"bswxzc,���μ������ʲ�,double,8,7,60,;"+
"bszzc,���ʲ�,double,8,8,68,;"+
"bsdqjk,���ڽ��,double,8,9,76,;"+
"bsyfzk,Ӧ���ʿ�,double,8,10,84,;"+
"bsldfz,������ծ,double,8,11,92,;"+
"bscqfz,���ڸ�ծ,double,8,12,100,;"+
"bsfz,��ծ�ϼ�,double,8,13,108,;"+
"bsgb,�ɱ�,double,8,14,116,;"+
"bsssgdqy,�����ɶ�Ȩ��,double,8,15,124,;"+
"bsgdqy,�ɶ�Ȩ��,double,8,16,132,;"+
"bszbgj,�ʱ�����,double,8,17,140,;"+
"bsyygj,ӯ�๫��,double,8,18,148,;"+
"iszysr,��Ӫҵ�����뾻��,double,8,1,156,;"+
"iszycb,��Ӫҵ��ɱ�,double,8,2,164,;"+
"iszylr,��Ӫҵ������,double,8,3,172,;"+
"isqtlr,����ҵ������,double,8,4,180,;"+
"isyyfy,Ӫҵ����,double,8,5,188,;"+
"isglfy,�������,double,8,6,196,;"+
"iscwfy,�������,double,8,7,204,;"+
"istzsy,Ͷ������,double,8,8,212,;"+
"islrze,�����ܶ�,double,8,9,220,;"+
"issds,����˰,double,8,10,228,;"+
"isjlr,������,double,8,11,236,;"+
"iskchjlr,�۳������������ľ�����,double,8,12,244,;"+
"iswfplr,δ��������,double,8,13,252,;"+
"cfjyhdxjlr,��Ӫ��ֽ�����,double,8,1,260,;"+
"cfjyhdxjlc,��Ӫ��ֽ�����,double,8,2,268,;"+
"cfjyhdxjje,��Ӫ��ֽ𾻶�,double,8,3,276,;"+
"cftzxjlr,Ͷ���ֽ�����,double,8,4,284,;"+
"cftzxjlc,Ͷ���ֽ�����,double,8,5,292,;"+
"cftzxjje,Ͷ���ֽ𾻶�,double,8,6,300,;"+
"cfczxjlr,����ֽ�����,double,8,7,308,;"+
"cfczxjlc,����ֽ�����,double,8,8,316,;"+
"cfczxjje,����ֽ𾻶�,double,8,9,324,;"+
"cfxjjze,�ֽ��ֽ�ȼ��ﾻ����,double,8,10,332,;"+
"cfxsspxj,������Ʒ�յ����ֽ�,double,8,11,340,;"+
"mgsy,ÿ������,single,4,1,348,;"+
"mgjzc,ÿ�ɾ��ʲ�,single,4,2,352,;"+
"tzmgjzc,������ÿ�ɾ��ʲ�,single,4,3,356,;"+
"mgzbgjj,ÿ���ʱ�������,single,4,4,360,;"+
"mgwfplr,ÿ��δ��������,single,4,5,364,;"+
"mgjyxjllje,ÿ�ɾ�Ӫ��������ֽ���������,single,4,6,368,;"+
"mgxjzjje,ÿ���ֽ��ֽ�ȼ������Ӿ���,single,4,7,372,;"+
"mll,ë����,single,4,8,376,;"+
"zyywlrl,��Ӫҵ��������,single,4,9,380,;"+
"jll,������,single,4,10,384,;"+
"zzcbcl,���ʲ�������,single,4,11,388,;"+
"jzcsyl,���ʲ�������,single,4,12,392,;"+
"xsxjzb,������Ʒ�յ����ֽ�ռ��Ӫ�������,single,4,13,396,;"+
"yszczzl,Ӧ���ʿ���ת��,single,4,14,400,;"+
"chzzl,�����ת��,single,4,15,404,;"+
"gdzczzl,�̶��ʲ���ת��,single,4,16,408,;"+
"zyywzzl,��Ӫҵ��������,single,4,17,412,;"+
"jlrzzl,������������,single,4,18,416,;"+
"zzczzl,���ʲ�������,single,4,19,420,;"+
"jzczzl,���ʲ�������,single,4,20,424,;"+
"ldbl,��������,single,4,21,428,;"+
"sdbl,�ٶ�����,single,4,22,432,;"+
"zcfzbl,�ʲ���ծ����,single,4,23,436,;"+
"fzbl,��ծ����,single,4,24,440,;"+
"gdqybl,�ɶ�Ȩ�����,single,4,25,444,;"+
"gdzcbl,�̶��ʲ�����,single,4,26,448,;"+
"kchmgjlr,�۳������������ÿ�ɾ�����,single,4,27,452,";

                        break;
                    #endregion
                    #region ʮ��ɶ�stkhold.fdt
                    case DataTypes.gd:
                        fileName = "STKHOLD.FDT";
                        startAddress = 0x41000;
                        blockSize = 17568;
                        recordSize = 2196;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;"+
"rq,����,date,4,66,2192,;" +
"gd1mc,�ɶ�1����,string,160,1,12,;"+
"gd1cgsl,�ɶ�1�ֹ�����,double,8,2,172,;"+
"gd1cgbl,�ɶ�1�ֹɱ���,single,4,3,180,;"+
"gd1bz,�ɶ�1��ע,string,20,4,184,;"+
"gd1fr,�ɶ�1����,string,8,5,204,;"+
"gd1jyfw,�ɶ�1��Ӫ��Χ,string,16,6,212,;"+
"gd2mc,�ɶ�2����,string,160,7,228,;"+
"gd2cgsl,�ɶ�2�ֹ�����,double,8,8,388,;"+
"gd2cgbl,�ɶ�2�ֹɱ���,single,4,9,396,;"+
"gd2bz,�ɶ�2��ע,string,20,10,400,;"+
"gd2fr,�ɶ�2����,string,8,11,420,;"+
"gd2jyfw,�ɶ�2��Ӫ��Χ,string,16,12,428,;"+
"gd3mc,�ɶ�3����,string,160,13,444,;"+
"gd3cgsl,�ɶ�3�ֹ�����,double,8,14,604,;"+
"gd3cgbl,�ɶ�3�ֹɱ���,single,4,15,612,;"+
"gd3bz,�ɶ�3��ע,string,20,16,616,;"+
"gd3fr,�ɶ�3����,string,8,17,636,;"+
"gd3jyfw,�ɶ�3��Ӫ��Χ,string,16,18,644,;"+
"gd4mc,�ɶ�4����,string,160,19,660,;"+
"gd4cgsl,�ɶ�4�ֹ�����,double,8,20,820,;"+
"gd4cgbl,�ɶ�4�ֹɱ���,single,4,21,828,;"+
"gd4bz,�ɶ�4��ע,string,20,22,832,;"+
"gd4fr,�ɶ�4����,string,8,23,852,;"+
"gd4jyfw,�ɶ�4��Ӫ��Χ,string,16,24,860,;"+
"gd5mc,�ɶ�5����,string,160,25,876,;"+
"gd5cgsl,�ɶ�5�ֹ�����,double,8,26,1036,;"+
"gd5cgbl,�ɶ�5�ֹɱ���,single,4,27,1044,;"+
"gd5bz,�ɶ�5��ע,string,20,28,1048,;"+
"gd5fr,�ɶ�5����,string,8,29,1068,;"+
"gd5jyfw,�ɶ�5��Ӫ��Χ,string,16,30,1076,;"+
"gd6mc,�ɶ�6����,string,160,31,1092,;"+
"gd6cgsl,�ɶ�6�ֹ�����,double,8,32,1252,;"+
"gd6cgbl,�ɶ�6�ֹɱ���,single,4,33,1260,;"+
"gd6bz,�ɶ�6��ע,string,20,34,1264,;"+
"gd6fr,�ɶ�6����,string,8,35,1284,;"+
"gd6jyfw,�ɶ�6��Ӫ��Χ,string,16,36,1292,;"+
"gd7mc,�ɶ�7����,string,160,37,1308,;"+
"gd7cgsl,�ɶ�7�ֹ�����,double,8,38,1468,;"+
"gd7cgbl,�ɶ�7�ֹɱ���,single,4,39,1476,;"+
"gd7bz,�ɶ�7��ע,string,20,40,1480,;"+
"gd7fr,�ɶ�7����,string,8,41,1500,;"+
"gd7jyfw,�ɶ�7��Ӫ��Χ,string,16,42,1508,;"+
"gd8mc,�ɶ�8����,string,160,43,1524,;"+
"gd8cgsl,�ɶ�8�ֹ�����,double,8,44,1684,;"+
"gd8cgbl,�ɶ�8�ֹɱ���,single,4,45,1692,;"+
"gd8bz,�ɶ�8��ע,string,20,46,1696,;"+
"gd8fr,�ɶ�8����,string,8,47,1716,;"+
"gd8jyfw,�ɶ�8��Ӫ��Χ,string,16,48,1724,;"+
"gd9mc,�ɶ�9����,string,160,49,1740,;"+
"gd9cgsl,�ɶ�9�ֹ�����,double,8,50,1900,;"+
"gd9cgbl,�ɶ�9�ֹɱ���,single,4,51,1908,;"+
"gd9bz,�ɶ�9��ע,string,20,52,1912,;"+
"gd9fr,�ɶ�9����,string,8,53,1932,;"+
"gd9jyfw,�ɶ�9��Ӫ��Χ,string,16,54,1940,;"+
"gd10mc,�ɶ�10����,string,160,55,1956,;"+
"gd10cgsl,�ɶ�10�ֹ�����,double,8,56,2116,;"+
"gd10cgbl,�ɶ�10�ֹɱ���,single,4,57,2124,;"+
"gd10bz,�ɶ�10��ע,string,20,58,2128,;"+
"gd10fr,�ɶ�10����,string,8,59,2148,;"+
"gd10jyfw,�ɶ�10��Ӫ��Χ,string,16,60,2156,;"+
"gdzs,�ɶ�����,int,4,61,2172,;"+
"gjgfrggds,���ҹɷ��˹ɹɶ���,int,4,62,2176,;"+
"aggds,��ͨ��A�ɹɶ���,int,4,63,2180,;"+
"bggds,��ͨ��B�ɹɶ���,int,4,64,2184,";

                        break;
                    #endregion
                    #region �����ܱ�fundweek.fdt
                    case DataTypes.jjzb:
                        fileName = "FUNDWEEK.FDT";
                        startAddress = 0x41000;
                        blockSize = 12032;
                        recordSize = 188;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;"+
"rq,����,date,4,13,184,;"+
"dwjz,����λ��ֵ,single,4,6,152,;" +
"jjze,����ֵ�ܶ�,double,8,5,144,;" +
"gm,�����ģ,double,8,4,136,;" +
"dwcz,����λ��ֵ,single,4,7,156,;"+
"tzhjz,���������ֵ,single,4,8,160,;"+
"tzhcz,����������ֵ,single,4,9,164,;"+
"zzl,����������(%),double,8,10,168,;"+
"ljjz,�����ۼƾ�ֵ,single,4,11,176,;"+
"slrq,������������,date,4,1,12,;"+
"glr,���������,string,60,2,16,;"+
"tgr,�����й���,string,60,3,76,"
;//12Ϊ�����ֶ�

                        break;
                    #endregion
                    #region ����Ͷ�����funddiv.fdt
                    case DataTypes.jjzh:
                        fileName = "FUNDDIV.FDT";
                        startAddress = 0x41000;
                        blockSize = 8320;
                        recordSize = 260;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;" +
"bgrq,��������,date,4,31,252,;" +
"zzrq,��ֹ����,date,4,32,256,;" +
"dm1,֤ȯ1����,string,12,1,12,;" +
"sz1,֤ȯ1��ֵ,double,8,2,24,;" +
"bl1,֤ȯ1ռ��ֵ����(%),single,4,3,32,;" +
"dm2,֤ȯ2����,string,12,4,36,;" +
"sz2,֤ȯ2��ֵ,double,8,5,48,;" +
"bl2,֤ȯ2ռ��ֵ����(%),single,4,6,56,;" +
"dm3,֤ȯ3����,string,12,7,60,;" +
"sz3,֤ȯ3��ֵ,double,8,8,72,;" +
"bl3,֤ȯ3ռ��ֵ����(%),single,4,9,80,;" +
"dm4,֤ȯ4����,string,12,10,84,;" +
"sz4,֤ȯ4��ֵ,double,8,11,96,;" +
"bl4,֤ȯ4ռ��ֵ����(%),single,4,12,104,;" +
"dm5,֤ȯ5����,string,12,13,108,;" +
"sz5,֤ȯ5��ֵ,double,8,14,120,;" +
"bl5,֤ȯ5ռ��ֵ����(%),single,4,15,128,;" +
"dm6,֤ȯ6����,string,12,16,132,;" +
"sz6,֤ȯ6��ֵ,double,8,17,144,;" +
"bl6,֤ȯ6ռ��ֵ����(%),single,4,18,152,;" +
"dm7,֤ȯ7����,string,12,19,156,;" +
"sz7,֤ȯ7��ֵ,double,8,20,168,;" +
"bl7,֤ȯ7ռ��ֵ����(%),single,4,21,176,;" +
"dm8,֤ȯ8����,string,12,22,180,;" +
"sz8,֤ȯ8��ֵ,double,8,23,192,;" +
"bl8,֤ȯ8ռ��ֵ����(%),single,4,24,200,;" +
"dm9,֤ȯ9����,string,12,25,204,;" +
"sz9,֤ȯ9��ֵ,double,8,26,216,;" +
"bl9,֤ȯ9ռ��ֵ����(%),single,4,27,224,;" +
"dm10,֤ȯ10����,string,12,28,228,;" +
"sz10,֤ȯ10��ֵ,double,8,29,240,;" +
"bl10,֤ȯ10ռ��ֵ����(%),single,4,30,248,";


                        break;
                    #endregion
 
                }
                string[] fieldLine = fieldString.Split(new char[] { ';' });
                fields = new string[fieldLine.Length, 7];
                for (int i = 0; i < fieldLine.Length; i++)
                {
                    string[] field = fieldLine[i].Split(new char[]{','} ,7 );
                    for(int j=0;j<field.Length;j++)
                    {
                        fields[i,j]=field[j];
                    }
                }
            }

        }
        private string fxjDataPath="";
        private string msg = "";
        private DateTime date19700101 = new DateTime(1970, 1, 1);

        public string  FxjDataPath   //����FxjDataPath
        {
            get
            { 
                return (fxjDataPath); 
            }
            set
            { 
                fxjDataPath = value;
                fxjDataPath = fxjDataPath.Trim().ToUpper();
                if (fxjDataPath!="" && !fxjDataPath.EndsWith(@"\"))
                {
                    fxjDataPath += @"\";
                }
                fxjDataPath=fxjDataPath.ToUpper(); 
            }
        }
        public string Msg
        {
            get { return (msg); }
        }
        private string [] GetCodes(string Market)   //��ȡDay.dat�еĴ���
        {
            long len = -1;
            long pos = 0;
            int flag;
            if (FxjDataPath == "")
            {
                msg = @"�޷���ע����е������������ļ�Ŀ¼�������н����� FxjDataPath����Ϊ��Ч·������c:\fxj\data\��";
                return new string[1] { null };
            }
            Market = Market.Trim().ToUpper();
            if (Market == "")
            {
                msg = "Market����ֻ�����г���ƣ��绦��ΪSH������ΪSZ�����ΪHK�ȡ�";
                return null;
            }
            string FxjFile = fxjDataPath + Market+@"\DAY.DAT";
            msg="";
            if (!File.Exists(FxjFile))  //DAY.DAT�ļ�������
            {
                msg = FxjFile + "�����ڣ�";
                return new string[1] { null };
            }
            try
            {
                FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                int secCounts = 0;//�ļ���֤ȯ����
                string code = "";
                len = fs.Length;
                fs.Position=0;
                flag = br.ReadInt32();
                if (flag == -65823756)   //0xFC139BF4
                {
                    fs.Position = 12;
                    secCounts=br.ReadInt32();
                    string[] codes = new string[secCounts];
                    for (int i = 0; i < secCounts; i++)
                    {
                        pos = 24 + 64*i;
                        if (pos <= len)
                        {
                            fs.Position = pos;
                            code = new string(br.ReadChars(10));//��������10���ֽڱ�����룬һ����6���ֽ�
                            code = Market + code.Replace("\0", "");
                            code = code.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                            codes[i] = code;
                        }
                    }
                    fs.Close();
                    msg = "";
                    return codes;
                }
            }
            catch(Exception e)
            {
                 msg=e.Message;
                 
            }
            return new string[1] { null };

        }
        public string[,] GetFields(string dataType)
        {
            msg = "";
            try
            {
                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
                return GetFields(d);
            }
            catch
            {
                msg = @"����Ĳ������󡣲���ֻ����:";
                foreach (string s in Enum.GetNames(typeof(DataTypes)))
                    msg += " \"" + s + "\"";
                msg += @" ���� ";
                foreach (int i in Enum.GetValues(typeof(DataTypes)))
                    msg += " " + i.ToString();

                return new string[1, 1] { { null } };
            }

        }
        public string[,] GetFields(DataTypes dataType)
        {
            msg = "";
            try
            {
                fileStruct fxjFileStruct = new fileStruct(dataType);
                string[,] fields = new string[fxjFileStruct.fields.GetLength(0), 3];
                //fields[0, 0] = "<�ֶ���>"; fields[0, 1] = "<����>"; fields[0, 2] = "<����>";
                for (int i = 0; i < fxjFileStruct.fields.GetLength(0); i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        fields[i, j] = fxjFileStruct.fields[i, j];
                    }
                }

                return fields;
            }
            catch
            {
                msg = "����"; return new string[1, 1] { { null } };
            }

        }
        public string[,] GetData(string dataType, string code)
        {
            return GetData(dataType, code, "");
        }
        public string[,] GetData(DataTypes dataType, string code)
        {
            return GetData(dataType, code, "");
        }
        public string[,] GetData(string dataType, string code, string newFileName) //��ȡ���ݣ�����
        {
            msg = "";
            try
            {
                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
                return GetData(d, code,newFileName);
            }
            catch
            {
                msg = @"����Ĳ������󡣵�һ������ֻ����:";
                foreach (string s in Enum.GetNames(typeof(DataTypes)))
                    msg += " \"" + s + "\"";
                msg += @" ���� ";
                foreach (int i in Enum.GetValues(typeof(DataTypes)))
                    msg += " " + i.ToString() ;

                return new string[1, 1] { { null } };
            }
        }
        public string[,] GetData(DataTypes dataType,string code,string newFileName) //��ȡ���ݣ�����
        {
            #region ��ȡ����ǰ��ʼ��
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(dataType);
            if (newFileName != "") fxjFileStruct.fileName = newFileName; //����û�����ָ�����ļ���
            code = code.Trim().ToUpper();
            if (code == "")
            {
                msg = @"CODE��������Ϊ�ա����ṩ֤ȯ���룬��SZ000001��";
                return new string[1, 1] { { null } };
            }
            ArrayList recordList = new ArrayList();
            int intField; float floatField; double doubleField; //string stringField;
            string market = code.Substring(0, 2);
            int recordCounts = 0;
            short[] blocks = new short[25];
            long len = -1;
            long pos = 0;
            if (this.FxjDataPath == "")
            {
                msg = @"�޷���ע����е������������ļ�Ŀ¼�������н����� FxjDataPath����Ϊ��Ч·������c:\fxj\data\��";
                return new string[1, 1] { { null } };
            }
            string FxjFile = fxjDataPath + fxjFileStruct.fileName;
            if (!File.Exists(FxjFile))
            {
                FxjFile = fxjDataPath + market +@"\" +fxjFileStruct.fileName;
            }
            msg = "";
            if (!File.Exists(FxjFile)) 
            {
                msg = fxjFileStruct.fileName + "û���ҵ���";
                return new string[1, 1] { { null } };
            }
            #endregion
            if (fxjFileStruct.isIndexDataStruct == true)
            {
                #region ����DAY.DAT�Ƚṹ������/���ݣ�������
                try
                {
                    FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    BinaryReader br = new BinaryReader(fs);
                    int secCounts = 0;//�ļ���֤ȯ����
                    string code0 = "";
                    len = fs.Length;
                    fs.Position = 12;
                    secCounts = br.ReadInt32();
                    for (int i = 0; i < secCounts; i++)
                    {
                        pos = 24 + 64 * i;
                        if (pos <= len)
                        {
                            fs.Position = pos;
                            //code0 = new string(br.ReadChars(10));//��������10���ֽڱ�����룬һ����8���ֽ�
                            code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                            code0 = code0.Replace("\0", "");
                            code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                            if (fxjFileStruct.codeIsLong == false && code == market + code0 || fxjFileStruct.codeIsLong == true && code == code0)
                            {
                                recordCounts = br.ReadInt32();
                                for (int j = 0; j < 25; j++)
                                {
                                    blocks[j] = br.ReadInt16();
                                }
                            }
                        }
                    }
                    int iRecord = 1;//��¼
                    int iBlock = 0;//��iBlock��
                    int fieldCounts = fxjFileStruct.fields.GetLength(0);
                    while (iBlock < 25 && blocks[iBlock] != -1)
                    {
                        int r = 0;
                        while (iRecord < recordCounts + 1 && r < fxjFileStruct.blockSize / fxjFileStruct.recordSize)   //16=3776/236
                        {
                            string[] record = new string[fieldCounts];
                            pos = fxjFileStruct.startAddress + blocks[iBlock] * fxjFileStruct.blockSize + r * fxjFileStruct.recordSize;
                            for (int iField = 0; iField < fieldCounts; iField++)
                            {
                                fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                switch (fxjFileStruct.fields[iField, 2].ToLower())
                                {
                                    case "code":
                                        //code0 = new string(br.ReadChars(8));//��12λ��ʵ������8λ����9-12λһ��Ϊ\0����ʱ�Ǵ����ֽڣ���Ϊֻ��8λ
                                        //code0 = code0.Replace("\0", "");
                                        record[iField] = code;
                                        break;
                                    case "date":
                                        intField = br.ReadInt32();
                                        record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
                                        break;
                                    case "int":
                                        intField = br.ReadInt32();
                                        record[iField] = intField.ToString();
                                        break;
                                    case "single":
                                        floatField = br.ReadSingle();
                                        if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                        record[iField] = floatField.ToString();
                                        break;
                                    case "double":
                                        doubleField = br.ReadDouble();
                                        record[iField] = doubleField.ToString();
                                        break;
                                    case "string":
                                        record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                        break;
                                }
                            }


                            recordList.Add(record);

                            r = r + 1;
                            iRecord = iRecord + 1;
                        }
                        iBlock = iBlock + 1;
                    }

                    fs.Close();
                    string[,] records = new string[recordList.Count, fieldCounts];
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        string[] record0 = (string[])recordList[i];
                        for (int j = 0; j < fieldCounts; j++)
                        {
                            records[i, j] = record0[j];
                        }
                    }
                    return records;
                }
                catch (Exception e)
                {
                    msg = e.Message;
                }
                #endregion
            }
            else
            {
                switch (dataType)
                {
                    case DataTypes.dm:
                        #region ���������STKINFO51.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//�ļ���֤ȯ����
                            string code0 = "";
                            fs.Position = 8;
                            secCounts = br.ReadInt32();
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            for (int i = 0; i < secCounts; i++)
                            {
                                pos = fxjFileStruct.startAddress + i * fxjFileStruct.recordSize;
                                fs.Position = pos;
                                code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                code0 = code0.Replace("\0", "");
                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                                string[] recordFieldName = new string[fieldCounts];
                                string[] record = new string[fieldCounts];
                                for (int iField = 0; iField < fieldCounts; iField++)
                                {
                                    fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                    switch (fxjFileStruct.fields[iField, 2].ToLower())
                                    {
                                        case "code":
                                            record[iField] = fxjFileStruct.codeIsLong == true ? code0 : market + code0;
                                            record[iField] = record[iField].Replace("HKHK", "HK");
                                            break;
                                        case "date":
                                            intField = br.ReadInt32();
                                            record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
                                            break;
                                        case "datetime":
                                            intField = br.ReadInt32();
                                            record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                            break;
                                        case "int":
                                            intField = br.ReadInt32();
                                            record[iField] = intField.ToString();
                                            break;
                                        case "single":
                                            floatField = br.ReadSingle();
                                            if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                            record[iField] = floatField.ToString();
                                            break;
                                        case "double":
                                            doubleField = br.ReadDouble();
                                            record[iField] = doubleField.ToString();
                                            break;
                                        case "string":
                                            record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                            break;
                                    }

                                }
                                recordList.Add(record);

                                

                            }

                            fs.Close();
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.zxhq:
                        #region �������飨����STKINFO51.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//�ļ���֤ȯ����
                            string code0 = "";
                            fs.Position = 8;
                            secCounts = br.ReadInt32();
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            bool hasCode = false;
                            for (int i = 0; i < secCounts && hasCode==false; i++)
                            {
                                pos = fxjFileStruct.startAddress + i * fxjFileStruct.recordSize;
                                fs.Position = pos;
                                code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                code0 = code0.Replace("\0", "");
                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                                if (fxjFileStruct.codeIsLong == false && code == market + code0 || fxjFileStruct.codeIsLong == true && code == code0)
                                {
                                    hasCode = true;
                                    string[] record = new string[fieldCounts];
                                    for (int iField = 0; iField < fieldCounts; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        switch (fxjFileStruct.fields[iField, 2].ToLower())
                                        {
                                            case "code":
                                                record[iField] = code;
                                                break;
                                            case "date":
                                                intField = br.ReadInt32();
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
                                                break;
                                            case "datetime":
                                                intField = br.ReadInt32();
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                                break;
                                            case "int":
                                                intField = br.ReadInt32();
                                                record[iField] = intField.ToString();
                                                break;
                                            case "single":
                                                floatField = br.ReadSingle();
                                                if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                record[iField] = Math.Round(floatField,2).ToString();
                                                break;
                                            case "double":
                                                doubleField = br.ReadDouble();
                                                record[iField] = Math.Round(doubleField,2).ToString();
                                                break;
                                            case "string":
                                                record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                break;
                                        }

                                    }
                                    recordList.Add(record);

                                }

                            }

                            fs.Close();
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cq:
                        #region �ֺ����䣨����STKINFO51.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//�ļ���֤ȯ����
                            string code0 = "";
                            fileStruct fxjdmStruct = new fileStruct(DataTypes.dm);//    ����Ľṹ
                            int dmpos=0;
                            fs.Position = 8;
                            secCounts = br.ReadInt32();
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            bool hasCode = false;
                            for (int i = 0; i < secCounts && hasCode==false; i++)
                            {
                                dmpos = fxjdmStruct.startAddress + i * fxjdmStruct.recordSize;
                                fs.Position = dmpos;
                                code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                code0 = code0.Replace("\0", "");
                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                                if (fxjdmStruct.codeIsLong == false && code == market + code0 || fxjdmStruct.codeIsLong == true && code == code0)
                                {
                                    hasCode = true;
                                    int iRecord=0;
                                    pos = fxjFileStruct.startAddress + i * fxjFileStruct.blockSize + iRecord * fxjFileStruct.recordSize;
                                    fs.Position=pos;
                                    while (br.ReadInt32() != 0)
                                    {
                                        string[] record = new string[fieldCounts];
                                        for (int iField = 0; iField < fieldCounts; iField++)
                                        {
                                            fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                            switch (fxjFileStruct.fields[iField, 2].ToLower())
                                            {
                                                case "code":
                                                    record[iField] = code;
                                                    break;
                                                case "date":
                                                    intField = br.ReadInt32();
                                                    record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
                                                    break;
                                                case "datetime":
                                                    intField = br.ReadInt32();
                                                    record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                                    break;
                                                case "int":
                                                    intField = br.ReadInt32();
                                                    record[iField] = intField.ToString();
                                                    break;
                                                case "single":
                                                    floatField = br.ReadSingle();
                                                    if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                    record[iField] = Math.Round(floatField,2).ToString();
                                                    break;
                                                case "double":
                                                    doubleField = br.ReadDouble();
                                                    record[iField] = Math.Round(doubleField,2).ToString();
                                                    break;
                                                case "string":
                                                    record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                    break;
                                            }

                                        }
                                        recordList.Add(record);
                                        iRecord = iRecord + 1;
                                        pos = fxjFileStruct.startAddress + i * fxjFileStruct.blockSize + iRecord * fxjFileStruct.recordSize;
                                        fs.Position = pos;

                                    }

                                }

                            }

                            fs.Close();
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cw0:
                        #region ��������--�򵥣�����STKINFO51.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//�ļ���֤ȯ����
                            string code0 = "";
                            fileStruct fxjdmStruct = new fileStruct(DataTypes.dm);//    ����Ľṹ
                            int dmpos = 0;
                            fs.Position = 8;
                            secCounts = br.ReadInt32();
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            bool hasCode = false;
                            for (int i = 0; i < secCounts && hasCode == false; i++)
                            {
                                dmpos = fxjdmStruct.startAddress + i * fxjdmStruct.recordSize;
                                fs.Position = dmpos;
                                code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                code0 = code0.Replace("\0", "");
                                code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                                if (fxjdmStruct.codeIsLong == false && code == market + code0 || fxjdmStruct.codeIsLong == true && code == code0)
                                {
                                    hasCode = true;
                                    int iRecord = 0;
                                    pos = fxjFileStruct.startAddress + i * fxjFileStruct.blockSize + iRecord * fxjFileStruct.recordSize;
                                    fs.Position = pos;
                                    string[] record = new string[fieldCounts];
                                    for (int iField = 0; iField < fieldCounts; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        switch (fxjFileStruct.fields[iField, 2].ToLower())
                                        {
                                            case "code":
                                                record[iField] = code;
                                                break;
                                            case "date":
                                                intField = br.ReadInt32();
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddDays(intField / 86400)).ToString("yyyy-MM-dd"));
                                                break;
                                            case "datetime":
                                                intField = br.ReadInt32();
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                                break;
                                            case "int":
                                                intField = br.ReadInt32();
                                                record[iField] = intField.ToString();
                                                break;
                                            case "single":
                                                floatField = br.ReadSingle();
                                                if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                record[iField] = Math.Round(floatField,2).ToString();
                                                break;
                                            case "double":
                                                doubleField = br.ReadDouble();
                                                record[iField] = Math.Round(doubleField,2).ToString();
                                                break;
                                            case "string":
                                                record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                break;
                                        }

                                    }
                                    recordList.Add(record);
                                }

                            }

                            fs.Close();
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.fbcj:
                        #region ����Report.DAT���ݣ��ṹ����DAY.DAT������Щ��ֵ��Ҫ��һ�����������
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//�ļ���֤ȯ����
                            string code0 = "";
                            len = fs.Length;
                            fs.Position = 12;
                            secCounts = br.ReadInt32();
                            for (int i = 0; i < secCounts; i++)
                            {
                                pos = 24 + 64 * i;
                                if (pos <= len)
                                {
                                    fs.Position = pos;
                                    //code0 = new string(br.ReadChars(10));//��������10���ֽڱ�����룬һ����8���ֽ�
                                    code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                    code0 = code0.Replace("\0", "");
                                    code0 = code0.Replace("HKHK", "HK");   //���֤ȯ���뱾����ΪHKxxxx
                                    if (fxjFileStruct.codeIsLong == false && code == market + code0 || fxjFileStruct.codeIsLong == true && code == code0)
                                    {
                                        recordCounts = br.ReadInt32();
                                        for (int j = 0; j < 25; j++)
                                        {
                                            blocks[j] = br.ReadInt16();
                                        }
                                    }
                                }
                            }
                            int iRecord = 1;//��¼
                            int iBlock = 0;//��iBlock��
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            while (iBlock < 25 && blocks[iBlock] != -1)
                            {
                                int r = 0;
                                while (iRecord < recordCounts + 1 && r < fxjFileStruct.blockSize / fxjFileStruct.recordSize)   //16=3776/236
                                {
                                    string[] record = new string[fieldCounts];
                                    pos = fxjFileStruct.startAddress + blocks[iBlock] * fxjFileStruct.blockSize + r * fxjFileStruct.recordSize;
                                    for (int iField = 0; iField < fieldCounts; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        switch (fxjFileStruct.fields[iField, 0].ToLower()) //�������ȡDAY.DAT�÷���ͬ���жϵ��Ǵ������������
                                        {
                                            case "dm":
                                                record[iField] = code;
                                                break;
                                            case "rq":
                                                intField = br.ReadInt32();
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                                break;
                                            case "zjcj":
                                            case "zss":
                                            case "je":
                                                floatField = br.ReadSingle();
                                                record[iField] = floatField.ToString();
                                                break;
                                            case "mr1sl":
                                            case "mr2sl":
                                            case "mr3sl":
                                            case "mc1sl":
                                            case "mc2sl":
                                            case "mc3sl":
                                                record[iField] = br.ReadInt16().ToString();
                                                break;
                                            case "mr1jg":
                                            case "mr2jg":
                                            case "mr3jg":
                                            case "mc1jg":
                                            case "mc2jg":
                                            case "mc3jg":
                                                float jg=br.ReadSByte();
                                                jg = Convert.ToSingle(record[2])  + jg / 100;
                                                record[iField] = jg.ToString();
                                                break;
                                            case "xss":
                                                record[iField] = "";//���������������
                                                break;
                                            case "mm":
                                                int mm = br.ReadSByte();
                                                record[iField] = "";
                                                if (mm == -128) record[iField] = "��"; //-128 = 0x80
                                                if (mm == -64) record[iField] = "��";  //-64 = 0xC0
                                                break;
                                        }
                                        
                                    }


                                    recordList.Add(record);
                                    
                                    r = r + 1;
                                    iRecord = iRecord + 1;
                                }
                                iBlock = iBlock + 1;
                            }

                            fs.Close();
                            float zssSaved = 0; 
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    if (j == 5)  //������
                                    {
                                        record0[j]= (Convert.ToSingle(record0[3]) - zssSaved).ToString();
                                        zssSaved = Convert.ToSingle(record0[3]);
                                    }
                                    records[i, j] = record0[j];
                                }
                            }
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion

                        break;
                }

            }
            msg = "���ؿ����顣";
            return new string[1, 1] { { null } };

        }
        
      
    }
}
