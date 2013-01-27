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

            //从注册表中读取分析家数据目录，如c:\fxj\data
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
            MessageBox.Show("FinDataTools—分析家数据读取组件 V" + Version+"\n\n        www.zhangwenzhang.com","关于",MessageBoxButtons.OK);
        }
        struct fileStruct
        {
            public string fileName;//文件名
            public int startAddress,blockSize,recordSize;//起始地址，每块长度，记录长度
            public bool codeIsLong, isIndexDataStruct;   //codeIsLong索引中的代码包含有市场代码SH、SZ等;isIndexDataStruct象Day.Dat那样的结构即由索引+数据组成; 
            public string[,] fields;//字段
            public fileStruct(DataTypes fileType)
            {
                fileName = "";
                startAddress = 0;
                blockSize = 0;
                recordSize = 0;
                codeIsLong = false;
                isIndexDataStruct = true;
                string fieldString = ""; //字段名，字段标签，类型，长度字段，存储顺序，偏移量
                switch (fileType)
                {

                    #region 代码表stkinfo51.dat
                    case DataTypes.dm:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x845898;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"jc,简称,string,32,1,10,;" +
"py,拼音,string,10,2,42,";
                        break;
                    #endregion
                    #region 分红送配stkinfo51.dat
                    case DataTypes.cq:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x14;
                        blockSize = 2116;
                        recordSize = 20;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,日期,date,4,0,0,;" +
"sgsl,送股,single,4,1,4,;"+
"pgsl,配股数量,single,4,2,8,;"+
"pgjg,配股价格,single,4,3,12,;"+
"fh,分红,single,4,4,16,";
                        break;
                    #endregion
                    #region 财务数据（简单）stkinfo51.dat
                    case DataTypes.cw0:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x794;
                        blockSize = 2116;
                        recordSize = 196;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,更新日期,date,4,38,192,;" +
"zgb,总股本,single,4,0,0,;" +
"gjg,国家股,single,4,1,4,;"+
"fqrg,发起人法人股,single,4,2,8,;"+
"frg,法人股,single,4,3,12,;"+
"b,B股,single,4,4,16,;"+
"h,H股,single,4,5,20,;"+
"a,流通A股,single,4,6,24,;"+
"zgg,职工股,single,4,7,28,;"+
"a2zpg,A2转配股,single,4,8,32,;"+
"bszzc,总资产,single,4,9,36,;"+
"bsldzc,流动资产,single,4,10,40,;"+
"bsgdzc,固定资产,single,4,11,44,;"+
"bswxzc,无形资产,single,4,12,48,;"+
"bscqtz,长期投资,single,4,13,52,;"+
"bsldfz,流动负债,single,4,14,56,;"+
"bscqfz,长期负债,single,4,15,60,;"+
"bszbgj,资本公积金,single,4,16,64,;"+
"mggjj,每股公积金,single,4,17,68,;"+
"bsgdqy,股东权益,single,4,18,72,;"+
"iszysl,主营收入,single,4,19,76,;"+
"iszylr,主营利润,single,4,20,80,;"+
"isqtlr,其他利润,single,4,21,84,;"+
"isyylr,营业利润,single,4,22,88,;"+
"istzsy,投资收益,single,4,23,92,;"+
"isbtsr,补贴收入,single,4,24,96,;"+
"isyywsz,营业外收支,single,4,25,100,;"+
"issytz,上年损益调整,single,4,26,104,;"+
"islrze,利润总额,single,4,27,108,;"+
"isshlr,税后利润,single,4,28,112,;"+
"isjlr,净利润,single,4,29,116,;"+
"iswfplr,未分配利润,single,4,30,120,;"+
"mgwfplr,每股未分配利润,single,4,31,124,;"+
"mgsy,每股收益,single,4,32,128,;"+
"mgjzc,每股净资产,single,4,33,132,;"+
"tzmgjzc,调整每股净资产,single,4,34,136,;"+
"gdqybl,股东权益比率,single,4,35,140,;"+
"jzcsyl,净资产收益率,single,4,36,144,";
                        //"unknown,(未知),string,44,37,148,;"

                        break;
                    #endregion
                    #region 最新行情stkinfo51.dat
                    case DataTypes.zxhq:
                        fileName = "STKINFO51.DAT";
                        startAddress = 0x845898;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"jc,简称,string,32,1,10,;" +
"py,拼音,string,10,2,42,;" +
"gxsj,更新时间,datetime,4,5,60,;" +
"wrjl,五日均量,single,4,6,64,;" +
"zs,昨收,single,4,7,68,;" +
"jk,今开,single,4,8,72,;" +
"zg,最高,single,4,9,76,;" +
"zd,最低,single,4,10,80,;" +
"zx,最新,single,4,11,84,;" +
"zss,总手数,single,4,12,88,;" +
"je,金额,single,4,13,92,;" +
"xss,现手数,single,4,14,96,;" +
"sbcj,上笔成交价,single,4,29,156,;" +
"dbcj,当笔成交价,single,4,30,160,;" +
"np,内盘,single,4,27,148,;" +
"wp,外盘,single,4,28,152,;" +
"mr1jg,买一价,single,4,15,100,;" +
"mr1sl,买一量,single,4,18,112,;" +
"mr2jg,买二价,single,4,16,104,;" +
"mr2sl,买二量,single,4,19,116,;" +
"mr3jg,买三价,single,4,17,108,;" +
"mr3sl,买三量,single,4,20,120,;" +
"mr4jg,买四价,single,4,32,168,;" +
"mr4sl,买四量,single,4,34,176,;" +
"mr5jg,买五价,single,4,33,172,;" +
"mr5sl,买五量,single,4,35,180,;" +
"mc1jg,卖一价,single,4,21,124,;" +
"mc1sl,卖一量,single,4,24,136,;" +
"mc2jg,卖二价,single,4,22,128,;" +
"mc2sl,卖二量,single,4,25,140,;" +
"mc3jg,卖三价,single,4,23,132,;" +
"mc3sl,卖三量,single,4,26,144,;" +
"mc4jg,卖四价,single,4,36,184,;" +
"mc4sl,卖四量,single,4,38,192,;" +
"mc5jg,卖五价,single,4,37,188,;" +
"mc5sl,卖五量,single,4,39,196,";
//"jd,精度,int,4,3,52,;" +
//"scbz,删除标志,int,4,4,56,";
 //"unknown,(未知),int,4,31,164,;" +
//",(未知),,48,40,200,;"
                        break;
                    #endregion
                    #region 分笔成交数据文件report.dat（结构同day.dat，但其中一些数据不是直接保存）
                    case DataTypes.fbcj:
                        fileName = "REPORT.DAT"; 
                        startAddress = 0x41000;
                        blockSize = 4068;
                        recordSize = 36;
                        codeIsLong = false;
                        isIndexDataStruct = false;//不完全等同于day.dat结构，因此单独处理
                        fieldString =
"dm,代码,code,4,0,0,;" +
"rq,日期,datetime,4,0,0,;" +
"zjcj,最近成交价,single,4,1,4,;" +
"zss,总手数,single,4,2,8,calc;" +
"je,金额,single,4,3,12,;" +
"xss,现手数,single,4,2,8,;" +
"mm,买或卖,string,2,16,35,;" +
"mr1sl,买一量,single,2,4,16,;" +
"mr2sl,买二量,single,2,5,18,;" +
"mr3sl,买三量,single,2,6,20,;" +
"mc1sl,卖一量,single,2,7,22,;" +
"mc2sl,卖二量,single,2,8,24,;" +
"mc3sl,卖三量,single,2,9,26,;" +
"mr1jg,买一价,single,1,10,28,;" +
"mr2jg,买二价,single,1,11,29,;" +
"mr3jg,买三价,single,1,12,30,;" +
"mc1jg,卖一价,single,1,13,31,;" +
"mc2jg,卖二价,single,1,14,32,;" +
"mc3jg,卖三价,single,1,15,33,";
                        //以类数据类型不是存储类型，程序中不直接用实际数据类型：买/卖X量为short，买/卖X价为byte
                        //现手数通过当总手数计算而得，应该放在总手数后面
                        break;
                    #endregion
                    #region 日线数据文件day.dat
                    case DataTypes.hq:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,日期,date,4,1,0,;" +
"kp,开盘,single,4,2,4,B;" +
"zg,最高,single,4,3,8,B;" +
"zd,最低,single,4,4,12,B;" +
"sp,收盘,single,4,5,16,B;" +
"sl,成交数量,single,4,6,20,A;"+
"je,成交金额,single,4,7,24,";
                        break;
                    #endregion
                    #region 5分钟数据文件min.dat
                    case DataTypes.min:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,日期,datetime,4,1,0,;" +
"kp,开盘,single,4,2,4,B;" +
"zg,最高,single,4,3,8,B;" +
"zd,最低,single,4,4,12,B;" +
"sp,收盘,single,4,5,16,B;" +
"sl,成交数量,single,4,6,20,A;"+
"je,成交金额,single,4,7,24,";
                        break;
                    #endregion
                    #region 分红送配数据文件exprof.fdt
                    case DataTypes.fp:
                        fileName = "EXPROF.FDT";
                        startAddress = 0x41000;
                        blockSize = 3776;
                        recordSize = 236;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;"+
"cqrq,除权日期,date,4,23,176,;" +
"sgbl,送股比例,double,8,1,12,;" +
"sgdjr,送股股权登记日,date,4,2,20,;"+
"sgcqr,送股除权日,date,4,3,24,;"+
"sgssr,红股上市日,date,4,4,28,;"+
"zzbl,转增比例,double,8,5,32,;"+
"zzdjr,转增股权登记日,date,4,6,40,;"+
"zzcqr,转增除权日,date,4,7,44,;"+
"zzssr,转增上市日,date,4,8,48,;"+
"fhbl,分红比例,double,8,9,52,;"+
"fhdjr,分红股权登记日,date,4,10,60,;" +
"fhcxr,分红除息日,date,4,11,64,;" +
"fhpxr,分红派息日,date,4,12,68,;" +
"pgbl,配股比例,double,8,13,72,;"+
"pgdjr,配股股权登记日,date,4,14,80,;"+
"pgcqr,配股除权基准日,date,4,15,84,;"+
"pgjkqsr,配股缴款起始日,date,4,16,88,;"+
"pgjkzzr,配股缴款终止日,date,4,17,92,;"+
"pgssr,配股可流通上市日,date,4,18,96,;"+
"pgjg,配股价格,single,4,19,100,;"+
"frgpgbl,公众股受让法人股配股比例,double,8,20,104,;"+
"frgmgzrf,认购法人股配股每股转让费,single,4,21,112,;"+
"pgzcxs,配股主承销商,string,60,22,116,;"+
"bgrq,报告日期,date,4,24,180,;"+
"dshrq,董事会日期,date,4,25,184,;"+
"gdhrq,股东会日期,date,4,26,188,;"+
"fhggrq,分红公告日期,date,4,27,192,;"+
"zgbjs,总股本基数,double,8,28,196,;"+
"sgsl,送股数量,double,8,29,204,;"+
"zzsl,转增数量,double,8,30,212,;"+
"sjpgs,实际配股总数,double,8,31,220,;"+
"cqhzgb,除权后总股本,double,8,32,228";

                        break;
                    #endregion
                    #region 股本结构Capital.fdt
                    case DataTypes.gb:
                        fileName = "CAPITAL.FDT";
                        startAddress = 0x41000;
                        blockSize = 3488;
                        recordSize = 218;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0;" +
"rq,日期,date,4,17,214;"+
"zgb,总股本,double,8,1,12;" +
"gjg,国家股,double,8,2,20;" +
"fqrg,发起人股,double,8,3,28;" +
"frg,法人股,double,8,4,36;" +
"ybfrps,一般法人配售,double,8,5,44;" +
"zgg,内部职工股,double,8,6,52;" +
"a,流通A股,double,8,7,60;" +
"zltzag,战略投资A股,double,8,8,68;" +
"zpg,转配股,double,8,9,76;" +
"jjps,基金配售,double,8,10,84;" +
"h,H股,double,8,11,92;" +
"b,B股,double,8,12,100;" +
"yxg,优先股,double,8,13,108;" +
"ggcg,高级管理人员持股,double,8,14,116;" +
"gbbdyy,股本变动原因,string,56,15,124;" +
"gbbdyylb,股本变动原因类别,string,34,16,180";

                        break;
                    #endregion 
                    #region 财务数据Finance.fdt
                    case DataTypes.cw:
                        fileName = "FINANCE.FDT";
                        startAddress = 0x41000;
                        blockSize = 14848;
                        recordSize = 464;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;"+
"rq,日期,date,4,,460,;"+
"bsdqtzje,短期投资净额,double,8,1,12,;"+
"bsyszkje,应收帐款净额,double,8,2,20,;"+
"bschje,存货净额,double,8,3,28,;"+
"bsldzc,流动资产,double,8,4,36,;"+
"bscqtzje,长期投资净额,double,8,5,44,;"+
"bsgdzc,固定资产,double,8,6,52,;"+
"bswxzc,无形及其他资产,double,8,7,60,;"+
"bszzc,总资产,double,8,8,68,;"+
"bsdqjk,短期借款,double,8,9,76,;"+
"bsyfzk,应付帐款,double,8,10,84,;"+
"bsldfz,流动负债,double,8,11,92,;"+
"bscqfz,长期负债,double,8,12,100,;"+
"bsfz,负债合计,double,8,13,108,;"+
"bsgb,股本,double,8,14,116,;"+
"bsssgdqy,少数股东权益,double,8,15,124,;"+
"bsgdqy,股东权益,double,8,16,132,;"+
"bszbgj,资本公积,double,8,17,140,;"+
"bsyygj,盈余公积,double,8,18,148,;"+
"iszysr,主营业务收入净额,double,8,1,156,;"+
"iszycb,主营业务成本,double,8,2,164,;"+
"iszylr,主营业务利润,double,8,3,172,;"+
"isqtlr,其它业务利润,double,8,4,180,;"+
"isyyfy,营业费用,double,8,5,188,;"+
"isglfy,管理费用,double,8,6,196,;"+
"iscwfy,财务费用,double,8,7,204,;"+
"istzsy,投资收益,double,8,8,212,;"+
"islrze,利润总额,double,8,9,220,;"+
"issds,所得税,double,8,10,228,;"+
"isjlr,净利润,double,8,11,236,;"+
"iskchjlr,扣除经常性损益后的净利润,double,8,12,244,;"+
"iswfplr,未分配利润,double,8,13,252,;"+
"cfjyhdxjlr,经营活动现金流入,double,8,1,260,;"+
"cfjyhdxjlc,经营活动现金流出,double,8,2,268,;"+
"cfjyhdxjje,经营活动现金净额,double,8,3,276,;"+
"cftzxjlr,投资现金流入,double,8,4,284,;"+
"cftzxjlc,投资现金流出,double,8,5,292,;"+
"cftzxjje,投资现金净额,double,8,6,300,;"+
"cfczxjlr,筹措现金流入,double,8,7,308,;"+
"cfczxjlc,筹措现金流出,double,8,8,316,;"+
"cfczxjje,筹措现金净额,double,8,9,324,;"+
"cfxjjze,现金及现金等价物净增额,double,8,10,332,;"+
"cfxsspxj,销售商品收到的现金,double,8,11,340,;"+
"mgsy,每股收益,single,4,1,348,;"+
"mgjzc,每股净资产,single,4,2,352,;"+
"tzmgjzc,调整后每股净资产,single,4,3,356,;"+
"mgzbgjj,每股资本公积金,single,4,4,360,;"+
"mgwfplr,每股未分配利润,single,4,5,364,;"+
"mgjyxjllje,每股经营活动产生的现金流量净额,single,4,6,368,;"+
"mgxjzjje,每股现金及现金等价物增加净额,single,4,7,372,;"+
"mll,毛利率,single,4,8,376,;"+
"zyywlrl,主营业务利润率,single,4,9,380,;"+
"jll,净利率,single,4,10,384,;"+
"zzcbcl,总资产报酬率,single,4,11,388,;"+
"jzcsyl,净资产收益率,single,4,12,392,;"+
"xsxjzb,销售商品收到的现金占主营收入比例,single,4,13,396,;"+
"yszczzl,应收帐款周转率,single,4,14,400,;"+
"chzzl,存货周转率,single,4,15,404,;"+
"gdzczzl,固定资产周转率,single,4,16,408,;"+
"zyywzzl,主营业务增长率,single,4,17,412,;"+
"jlrzzl,净利润增长率,single,4,18,416,;"+
"zzczzl,总资产增长率,single,4,19,420,;"+
"jzczzl,净资产增长率,single,4,20,424,;"+
"ldbl,流动比率,single,4,21,428,;"+
"sdbl,速动比率,single,4,22,432,;"+
"zcfzbl,资产负债比率,single,4,23,436,;"+
"fzbl,负债比率,single,4,24,440,;"+
"gdqybl,股东权益比率,single,4,25,444,;"+
"gdzcbl,固定资产比率,single,4,26,448,;"+
"kchmgjlr,扣除经常性损益后每股净利润,single,4,27,452,";

                        break;
                    #endregion
                    #region 十大股东stkhold.fdt
                    case DataTypes.gd:
                        fileName = "STKHOLD.FDT";
                        startAddress = 0x41000;
                        blockSize = 17568;
                        recordSize = 2196;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;"+
"rq,日期,date,4,66,2192,;" +
"gd1mc,股东1名称,string,160,1,12,;"+
"gd1cgsl,股东1持股数量,double,8,2,172,;"+
"gd1cgbl,股东1持股比例,single,4,3,180,;"+
"gd1bz,股东1备注,string,20,4,184,;"+
"gd1fr,股东1法人,string,8,5,204,;"+
"gd1jyfw,股东1经营范围,string,16,6,212,;"+
"gd2mc,股东2名称,string,160,7,228,;"+
"gd2cgsl,股东2持股数量,double,8,8,388,;"+
"gd2cgbl,股东2持股比例,single,4,9,396,;"+
"gd2bz,股东2备注,string,20,10,400,;"+
"gd2fr,股东2法人,string,8,11,420,;"+
"gd2jyfw,股东2经营范围,string,16,12,428,;"+
"gd3mc,股东3名称,string,160,13,444,;"+
"gd3cgsl,股东3持股数量,double,8,14,604,;"+
"gd3cgbl,股东3持股比例,single,4,15,612,;"+
"gd3bz,股东3备注,string,20,16,616,;"+
"gd3fr,股东3法人,string,8,17,636,;"+
"gd3jyfw,股东3经营范围,string,16,18,644,;"+
"gd4mc,股东4名称,string,160,19,660,;"+
"gd4cgsl,股东4持股数量,double,8,20,820,;"+
"gd4cgbl,股东4持股比例,single,4,21,828,;"+
"gd4bz,股东4备注,string,20,22,832,;"+
"gd4fr,股东4法人,string,8,23,852,;"+
"gd4jyfw,股东4经营范围,string,16,24,860,;"+
"gd5mc,股东5名称,string,160,25,876,;"+
"gd5cgsl,股东5持股数量,double,8,26,1036,;"+
"gd5cgbl,股东5持股比例,single,4,27,1044,;"+
"gd5bz,股东5备注,string,20,28,1048,;"+
"gd5fr,股东5法人,string,8,29,1068,;"+
"gd5jyfw,股东5经营范围,string,16,30,1076,;"+
"gd6mc,股东6名称,string,160,31,1092,;"+
"gd6cgsl,股东6持股数量,double,8,32,1252,;"+
"gd6cgbl,股东6持股比例,single,4,33,1260,;"+
"gd6bz,股东6备注,string,20,34,1264,;"+
"gd6fr,股东6法人,string,8,35,1284,;"+
"gd6jyfw,股东6经营范围,string,16,36,1292,;"+
"gd7mc,股东7名称,string,160,37,1308,;"+
"gd7cgsl,股东7持股数量,double,8,38,1468,;"+
"gd7cgbl,股东7持股比例,single,4,39,1476,;"+
"gd7bz,股东7备注,string,20,40,1480,;"+
"gd7fr,股东7法人,string,8,41,1500,;"+
"gd7jyfw,股东7经营范围,string,16,42,1508,;"+
"gd8mc,股东8名称,string,160,43,1524,;"+
"gd8cgsl,股东8持股数量,double,8,44,1684,;"+
"gd8cgbl,股东8持股比例,single,4,45,1692,;"+
"gd8bz,股东8备注,string,20,46,1696,;"+
"gd8fr,股东8法人,string,8,47,1716,;"+
"gd8jyfw,股东8经营范围,string,16,48,1724,;"+
"gd9mc,股东9名称,string,160,49,1740,;"+
"gd9cgsl,股东9持股数量,double,8,50,1900,;"+
"gd9cgbl,股东9持股比例,single,4,51,1908,;"+
"gd9bz,股东9备注,string,20,52,1912,;"+
"gd9fr,股东9法人,string,8,53,1932,;"+
"gd9jyfw,股东9经营范围,string,16,54,1940,;"+
"gd10mc,股东10名称,string,160,55,1956,;"+
"gd10cgsl,股东10持股数量,double,8,56,2116,;"+
"gd10cgbl,股东10持股比例,single,4,57,2124,;"+
"gd10bz,股东10备注,string,20,58,2128,;"+
"gd10fr,股东10法人,string,8,59,2148,;"+
"gd10jyfw,股东10经营范围,string,16,60,2156,;"+
"gdzs,股东总数,int,4,61,2172,;"+
"gjgfrggds,国家股法人股股东数,int,4,62,2176,;"+
"aggds,流通股A股股东数,int,4,63,2180,;"+
"bggds,流通股B股股东数,int,4,64,2184,";

                        break;
                    #endregion
                    #region 基金周报fundweek.fdt
                    case DataTypes.jjzb:
                        fileName = "FUNDWEEK.FDT";
                        startAddress = 0x41000;
                        blockSize = 12032;
                        recordSize = 188;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;"+
"rq,日期,date,4,13,184,;"+
"dwjz,基金单位净值,single,4,6,152,;" +
"jjze,基金净值总额,double,8,5,144,;" +
"gm,基金规模,double,8,4,136,;" +
"dwcz,基金单位初值,single,4,7,156,;"+
"tzhjz,基金调整后净值,single,4,8,160,;"+
"tzhcz,基金调整后初值,single,4,9,164,;"+
"zzl,基金增长率(%),double,8,10,168,;"+
"ljjz,基金累计净值,single,4,11,176,;"+
"slrq,基金设立日期,date,4,1,12,;"+
"glr,基金管理人,string,60,2,16,;"+
"tgr,基金托管人,string,60,3,76,"
;//12为保留字段

                        break;
                    #endregion
                    #region 基金投资组合funddiv.fdt
                    case DataTypes.jjzh:
                        fileName = "FUNDDIV.FDT";
                        startAddress = 0x41000;
                        blockSize = 8320;
                        recordSize = 260;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;" +
"bgrq,报告日期,date,4,31,252,;" +
"zzrq,截止日期,date,4,32,256,;" +
"dm1,证券1代码,string,12,1,12,;" +
"sz1,证券1市值,double,8,2,24,;" +
"bl1,证券1占净值比例(%),single,4,3,32,;" +
"dm2,证券2代码,string,12,4,36,;" +
"sz2,证券2市值,double,8,5,48,;" +
"bl2,证券2占净值比例(%),single,4,6,56,;" +
"dm3,证券3代码,string,12,7,60,;" +
"sz3,证券3市值,double,8,8,72,;" +
"bl3,证券3占净值比例(%),single,4,9,80,;" +
"dm4,证券4代码,string,12,10,84,;" +
"sz4,证券4市值,double,8,11,96,;" +
"bl4,证券4占净值比例(%),single,4,12,104,;" +
"dm5,证券5代码,string,12,13,108,;" +
"sz5,证券5市值,double,8,14,120,;" +
"bl5,证券5占净值比例(%),single,4,15,128,;" +
"dm6,证券6代码,string,12,16,132,;" +
"sz6,证券6市值,double,8,17,144,;" +
"bl6,证券6占净值比例(%),single,4,18,152,;" +
"dm7,证券7代码,string,12,19,156,;" +
"sz7,证券7市值,double,8,20,168,;" +
"bl7,证券7占净值比例(%),single,4,21,176,;" +
"dm8,证券8代码,string,12,22,180,;" +
"sz8,证券8市值,double,8,23,192,;" +
"bl8,证券8占净值比例(%),single,4,24,200,;" +
"dm9,证券9代码,string,12,25,204,;" +
"sz9,证券9市值,double,8,26,216,;" +
"bl9,证券9占净值比例(%),single,4,27,224,;" +
"dm10,证券10代码,string,12,28,228,;" +
"sz10,证券10市值,double,8,29,240,;" +
"bl10,证券10占净值比例(%),single,4,30,248,";


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

        public string  FxjDataPath   //属性FxjDataPath
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
        private string [] GetCodes(string Market)   //读取Day.dat中的代码
        {
            long len = -1;
            long pos = 0;
            int flag;
            if (FxjDataPath == "")
            {
                msg = @"无法在注册表中到分析家数据文件目录，请自行将属性 FxjDataPath设置为有效路径，如c:\fxj\data\。";
                return new string[1] { null };
            }
            Market = Market.Trim().ToUpper();
            if (Market == "")
            {
                msg = "Market参数只能是市场简称，如沪市为SH，深市为SZ，香港为HK等。";
                return null;
            }
            string FxjFile = fxjDataPath + Market+@"\DAY.DAT";
            msg="";
            if (!File.Exists(FxjFile))  //DAY.DAT文件不存在
            {
                msg = FxjFile + "不存在！";
                return new string[1] { null };
            }
            try
            {
                FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                int secCounts = 0;//文件中证券总数
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
                            code = new string(br.ReadChars(10));//分析家用10个字节保存代码，一般用6个字节
                            code = Market + code.Replace("\0", "");
                            code = code.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                msg = @"输入的参数有误。参数只能是:";
                foreach (string s in Enum.GetNames(typeof(DataTypes)))
                    msg += " \"" + s + "\"";
                msg += @" 或者 ";
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
                //fields[0, 0] = "<字段名>"; fields[0, 1] = "<含义>"; fields[0, 2] = "<类型>";
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
                msg = "错误"; return new string[1, 1] { { null } };
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
        public string[,] GetData(string dataType, string code, string newFileName) //读取数据，重载
        {
            msg = "";
            try
            {
                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());
                return GetData(d, code,newFileName);
            }
            catch
            {
                msg = @"输入的参数有误。第一个参数只能是:";
                foreach (string s in Enum.GetNames(typeof(DataTypes)))
                    msg += " \"" + s + "\"";
                msg += @" 或者 ";
                foreach (int i in Enum.GetValues(typeof(DataTypes)))
                    msg += " " + i.ToString() ;

                return new string[1, 1] { { null } };
            }
        }
        public string[,] GetData(DataTypes dataType,string code,string newFileName) //读取数据，重载
        {
            #region 读取数据前初始化
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(dataType);
            if (newFileName != "") fxjFileStruct.fileName = newFileName; //如果用户重新指定了文件名
            code = code.Trim().ToUpper();
            if (code == "")
            {
                msg = @"CODE参数不可为空。请提供证券代码，如SZ000001。";
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
                msg = @"无法在注册表中到分析家数据文件目录，请自行将属性 FxjDataPath设置为有效路径，如c:\fxj\data\。";
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
                msg = fxjFileStruct.fileName + "没有找到！";
                return new string[1, 1] { { null } };
            }
            #endregion
            if (fxjFileStruct.isIndexDataStruct == true)
            {
                #region 处理DAY.DAT等结构（索引/数据）的数据
                try
                {
                    FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    BinaryReader br = new BinaryReader(fs);
                    int secCounts = 0;//文件中证券总数
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
                            //code0 = new string(br.ReadChars(10));//分析家用10个字节保存代码，一般用8个字节
                            code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                            code0 = code0.Replace("\0", "");
                            code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                    int iRecord = 1;//记录
                    int iBlock = 0;//第iBlock块
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
                                        //code0 = new string(br.ReadChars(8));//有12位，实际用了8位，第9-12位一般为\0，有时是错误字节，因为只读8位
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
                        #region 代码表（处理STKINFO51.DAT等结构的数据）
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//文件中证券总数
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
                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                        #region 最新行情（处理STKINFO51.DAT等结构的数据）
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//文件中证券总数
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
                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                        #region 分红送配（处理STKINFO51.DAT等结构的数据）
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//文件中证券总数
                            string code0 = "";
                            fileStruct fxjdmStruct = new fileStruct(DataTypes.dm);//    代码的结构
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
                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                        #region 财务数据--简单（处理STKINFO51.DAT等结构的数据）
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//文件中证券总数
                            string code0 = "";
                            fileStruct fxjdmStruct = new fileStruct(DataTypes.dm);//    代码的结构
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
                                code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                        #region 处理Report.DAT数据（结构类似DAY.DAT，但有些数值需要进一步计算而来）
                        try
                        {
                            FileStream fs = new FileStream(FxjFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            BinaryReader br = new BinaryReader(fs);
                            int secCounts = 0;//文件中证券总数
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
                                    //code0 = new string(br.ReadChars(10));//分析家用10个字节保存代码，一般用8个字节
                                    code0 = System.Text.Encoding.Default.GetString(br.ReadBytes(10));
                                    code0 = code0.Replace("\0", "");
                                    code0 = code0.Replace("HKHK", "HK");   //香港证券代码本身保存为HKxxxx
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
                            int iRecord = 1;//记录
                            int iBlock = 0;//第iBlock块
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
                                        switch (fxjFileStruct.fields[iField, 0].ToLower()) //这里与读取DAY.DAT用法不同，判断的是代码而不是类型
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
                                                record[iField] = "";//现手数在下面计算
                                                break;
                                            case "mm":
                                                int mm = br.ReadSByte();
                                                record[iField] = "";
                                                if (mm == -128) record[iField] = "买"; //-128 = 0x80
                                                if (mm == -64) record[iField] = "卖";  //-64 = 0xC0
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
                                    if (j == 5)  //现手数
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
            msg = "返回空数组。";
            return new string[1, 1] { { null } };

        }
        
      
    }
}
