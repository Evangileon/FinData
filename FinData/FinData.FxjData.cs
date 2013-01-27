using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
//using System.DateTime;



namespace FinData
{
    [ProgId("FinData.FxjData"),ComVisible(true)]
    public class FxjData
    {
        const string ConStr = "server=(local);user id=PRPUser;pwd=61000000;database=PRPStockDataBase";
        public enum DataTypes { dm,hq,hqmb,hq0,hq0_ag,hq1,hq5,cq,cw0,fp,gb,gd,cw,jjjz,jjzh,bk,pj,hqfq};
        private string[,] tableNames = new string[,] {
            #region 表名
            {"dm","代码",""},
            {"hq","行情",""},
            {"hqmb","分笔成交",""},
            {"hq0","动态行情",""},
            {"hq0_ag","上海、深圳A股市场动态行情",""},
            {"hq1","一分钟行情",""},
            {"hq5","五分钟行情",""},
            {"cq","除权数据",""},
            {"cw0","最新财务数据",""},
            {"fp","分红送配",""},
            {"gb","股本结构",""},
            {"gd","十大股东",""},
            {"cw","财务数据",""},
            {"jjjz","基金净值",""},
            {"jjzh","基金投资组合",""},
            {"bk","板块",""},
            {"pj","评级",""},
            {"hqfq","复权行情",""} 

        };//行顺序与Datatype一致，列分别为表名、表中文名、对应文件名（GetTables函数中赋值）
            #endregion

        private string[,] GetCodeList( string Market,string ColumnName_StockCode,string ColumnName_StockName) //读取证券代码列表
        {
           // string ConStr = "server=(local);user id=PRPUser;pwd=61000000;database=PRPStockDataBase";
            string TableName = string.Empty;
            string SqlStr = string.Empty;
            SqlConnection con = new SqlConnection(ConStr);
            con.Open();

            //"Select * From T_PRP_StockCodeList_SH"
            //if (Market=="SH")
            //SqlStr = "Select * From " + T_PRP_StockCodeList_SH;
            switch (Market)
            {
                case "SH":
                    TableName = "T_PRP_StockCodeList_SH";                    
                    break;
                case "SZ":
                    TableName = "T_PRP_StockCodeList_SZ";
                    break;
            }

            SqlStr = "Select * From " + TableName;

            SqlDataAdapter da = new SqlDataAdapter(SqlStr, con);
            DataSet ds = new DataSet();
            SqlCommand cmd1 = new SqlCommand(SqlStr, con);
            da.Fill(ds, TableName);
            string[,] CodeList = new string[ds.Tables[TableName].Columns.Count, ds.Tables[TableName].Rows.Count];

            SqlDataReader dr = cmd1.ExecuteReader();


            int j = 0;
            while (dr.Read())
            {

                if (dr.HasRows)
                {
                    CodeList[0, j] = Market + dr[ColumnName_StockCode].ToString();
                    CodeList[1, j] = dr[ColumnName_StockName].ToString();
                    j++;
                }
            }

            return CodeList;
        }

        private int InsertStockDataToSQLDb(string dataType, string CodeStr, string[,] CodeList, int iRecordCount)
        {


            //cmd.ExecuteNonQuery();
            string[,] dataArray;
            string SqlStr = string.Empty;
            FinData.FxjData fxj = new FinData.FxjData();

            if (CodeStr != "")
            {
                CodeList[0, 0] = CodeStr;
            }
            
            try
            {
                for (int j=0;j < CodeList.GetLength(1);j++)
                {
                    dataArray = fxj.GetData(dataType, CodeList[0,j],iRecordCount);

                    if (fxj.Error == 0 && dataArray.GetLength(0) > 0)
                    {
                        string[,] colname = fxj.GetFields(dataType);
                        try
                        {
                            SqlConnection con = new SqlConnection(ConStr);
                            con.Open();

                            //删除临时表中的原来记录
                            SqlStr = "Delete T_PRP_StockDataTemp From T_PRP_StockdataTemp";
                            SqlCommand cmd = new SqlCommand(SqlStr, con);
                            cmd.ExecuteNonQuery();

                            //向临时表添加数据
                            for (int i = 0; i < dataArray.GetLength(0); i++)
                            {
                                SqlStr = "Insert Into T_PRP_StockDataTemp(FCode,FDateTime,FOpen,FHigh,FLow,FClose,FVolume,FAmount) values('" +
                                dataArray[i, 0] + "','" + dataArray[i, 1] + "','" + dataArray[i, 2] + "','" + dataArray[i, 3] + "','"
                                + dataArray[i, 4] + "','" + dataArray[i, 5] + "','" + dataArray[i, 6] + "','" + dataArray[i, 7] + "')";
                                cmd.CommandText = SqlStr;
                                cmd.ExecuteNonQuery();
                            }

                            //从临时表向正式表添加不重复的记录
                            SqlStr = "Insert Into T_PRP_StockData(FCode,FDateTime,FOpen,FHigh,FLow,FClose,FVolume,FAmount)" +
                            "Select Distinct FCode,FDateTime,FOpen,FHigh,FLow,FClose,FVolume,FAmount From T_PRP_StockDataTemp " +
                            "Where  not exists(Select * From T_PRP_StockData Where (FDateTime=T_PRP_StockDataTemp.FDateTime And FCode=T_PRP_StockDataTemp.FCode))";
                            cmd.CommandText = SqlStr;
                            cmd.ExecuteNonQuery();

                            if (CodeStr != "")
                            {
                                con.Close();
                                return 0;
                            }


                        }
                        catch
                        {
                            //向SQl Server数据库添加数据失败！
                            MessageBox.Show("向SQl Server数据库添加数据失败！"); 
                            return 1;
                        }

                    }
                    else
                    {
                        //没有数据或发生错误 
                        //return 2;
                    }
                }
                }


            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return 3;
            }

            return 0;
        }

        private void InsertStockDataToSQLDb_hq(string Market, int iRecordCount)
        {
            //string[,] CodeList =GetCodeList("T_PRP_StockCodeList_SH", "FStockCode", "FStockName");
            if (InsertStockDataToSQLDb("hq","",GetCodeList(Market,"FStockCode", "FStockName"),iRecordCount)==0)
                MessageBox.Show("成功");
            else
                MessageBox.Show("失败");        
        }


        public FxjData()
        {
            try
            {
                //从注册表中读取分析家数据目录，如c:\fxj\data
                RegistryKey keyFxj;
                RegistryKey keySoftware = Registry.LocalMachine.OpenSubKey("Software");
                keyFxj = keySoftware.OpenSubKey("dzh");
                if (keyFxj == null)
                {
                    keyFxj = keySoftware.OpenSubKey("Huitianqi");
                    if (keyFxj == null)
                    {
                        fxjPath = "";
                        fxjDataPath = "";
                        msg = "没有找到大智慧或分析家安装信息！";
                        return;
                    }
                }
                RegistryKey keySuperstk = keyFxj.OpenSubKey("SUPERSTK");
                if (keySuperstk != null)
                {
                    fxjPath = (string)keySuperstk.GetValue("InstPath");
                    if (fxjPath != "")
                    {
                        fxjPath = fxjPath.ToUpper();
                        if (fxjPath != "" && fxjPath.EndsWith(@"\") == false)
                            fxjPath = fxjPath + @"\";
                        fxjDataPath = fxjPath+ @"DATA\";
                        fxjDataPath = fxjDataPath.ToUpper();
                        RegistryKey keyMarket = keySuperstk.OpenSubKey("Market");
                        if (keyMarket != null)
                        {
                            string[] marketSubKeyNames = keyMarket.GetSubKeyNames();
                            if (marketSubKeyNames.Length > 0)
                            {
                                RegistryKey[] marketSubKey = new RegistryKey[marketSubKeyNames.Length];
                                fxjMarket = new string[marketSubKeyNames.Length, 3];
                                for (int i = 0; i < marketSubKeyNames.Length; i++)
                                {
                                    marketSubKey[i] = keyMarket.OpenSubKey(marketSubKeyNames[i]);
                                    if (marketSubKey[i] != null)
                                    {
                                        fxjMarket[i, 0] = marketSubKeyNames[i];
                                        fxjMarket[i, 1] = (string)marketSubKey[i].GetValue("name");
                                        fxjMarket[i, 2] = (string)marketSubKey[i].GetValue("shortname");
                                    }
                                }
                                for (int i = 0; i < marketSubKeyNames.Length; i++)
                                {
                                    int lastI=marketSubKeyNames.Length-1;
                                    if (fxjMarket[i, 0].ToUpper() == "SH")
                                    {
                                        string[] temp = new string[3];
                                        temp[0] = fxjMarket[0, 0];
                                        temp[1] = fxjMarket[0, 1];
                                        temp[2] = fxjMarket[0, 2];
                                        fxjMarket[0, 0] = fxjMarket[i, 0];
                                        fxjMarket[0, 1] = fxjMarket[i, 1];
                                        fxjMarket[0, 2] = fxjMarket[i, 2];
                                        fxjMarket[i, 0] = temp[0];
                                        fxjMarket[i, 1] = temp[1];
                                        fxjMarket[i, 2] = temp[2];
                                    }
                                    if (fxjMarket[i, 0].ToUpper() == "SZ")
                                    {
                                        string[] temp = new string[3];
                                        temp[0] = fxjMarket[1, 0];
                                        temp[1] = fxjMarket[1, 1];
                                        temp[2] = fxjMarket[1, 2];
                                        fxjMarket[1, 0] = fxjMarket[i, 0];
                                        fxjMarket[1, 1] = fxjMarket[i, 1];
                                        fxjMarket[1, 2] = fxjMarket[i, 2];
                                        fxjMarket[i, 0] = temp[0];
                                        fxjMarket[i, 1] = temp[1];
                                        fxjMarket[i, 2] = temp[2];
                                    }
                                    if (fxjMarket[i, 0].ToUpper() == "$$")
                                    {
                                        string[] temp = new string[3];
                                        temp[0] = fxjMarket[lastI, 0];
                                        temp[1] = fxjMarket[lastI, 1];
                                        temp[2] = fxjMarket[lastI, 2];
                                        fxjMarket[lastI, 0] = fxjMarket[i, 0];
                                        fxjMarket[lastI, 1] = fxjMarket[i, 1];
                                        fxjMarket[lastI, 2] = fxjMarket[i, 2];
                                        fxjMarket[i, 0] = temp[0];
                                        fxjMarket[i, 1] = temp[1];
                                        fxjMarket[i, 2] = temp[2];
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }
        struct stkInfo //主要用于由分时行情换算而来的分钟线
        {
            public string Code;	//股票代码
            public string Name;    // 证券名称
            public System.DateTime Time;//日期时间

            public float Yclose;	//昨日收盘价
            public float Open;		//开盘价
            public float High;		//最高价
            public float Low;		//最低价
            public float Close;	//收盘价
            public float New;//最新价
            public int Volume;	//成交量
            public int Inside;   // 内盘
            public int Outside;  // 外盘
            public float Amount;	//成交金额
            public float BuyPrice1;	// 五个叫买价
            public float BuyPrice2;
            public float BuyPrice3;
            public float BuyPrice4;
            public float BuyPrice5;
            public int BuyVol1;	// 对应五个叫买价的五个买盘
            public int BuyVol2;
            public int BuyVol3;
            public int BuyVol4;
            public int BuyVol5;
            public float SellPrice1;	// 五个叫卖价
            public float SellPrice2;
            public float SellPrice3;
            public float SellPrice4;
            public float SellPrice5;
            public int SellVol1;	// 对应五个叫卖价的五个卖盘
            public int SellVol2;
            public int SellVol3;
            public int SellVol4;
            public int SellVol5;

            public float ltgb; // 流通股本
            public float zgb;  // 总股本
            public float mgsy; // 每股收益
            public float mgjzc;    // 每股净资产
            public float mggjj;    //每股公积金
            public float mgwfp;    //每股未分配

            public float PriceFs;			 //分时价
            public float PriceAve;			 //均线价
            public float PriceZt;   //涨停
            public float PriceDt;   //跌停
            public int SumVol;				 //总成交量
            public int SumAmount;			 //总成交金额
            public int SumVol_hq0;				 //总成交量＿动态行情
            public int SumAmount_hq0;			 //总成交金额＿动态行情

            public int bs;  //笔数
            public int zbs; //总笔数

            public float hsl;				 //相对于流通盘的换手率
            public float zmr5dltp;   //总买入5档占流通盘比率
            public float zmc5dltp;   //总卖出5档占流通盘比率

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
                    #region 代码表STKINFO60.DAT//代码的拼音是乱码，有可能拼音简写未存盘，而是由大智慧软件另行计算。//OK
                    case DataTypes.dm:
                        fileName = "STKINFO60.DAT";
                        //startAddress = 0x845898;
                        startAddress = 0x6d0226;
                        //startAddress = 0x68A8A6;
                        blockSize = 0;
                        //recordSize = 248;//原分析家
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"jc,简称,string,32,1,10,;" +
"py,拼音,string,10,2,42,";
                        break;
                    #endregion
                    #region 分红送配STKINFO60.DAT//除权数据//OK
                    case DataTypes.cq:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x44aa;
                        blockSize = 2227;
                        recordSize = 20;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,日期,date,4,0,0,;" +
"sgbl,送股比例,single,4,1,4,;" +
"pgbl,配股比例,single,4,2,8,;" +
"pgjg,配股价格,single,4,3,12,;" +
"fh,分红,single,4,4,16,";
                        break;
                    #endregion
                    #region 财务数据（简单）STKINFO60.DAT//OK
                    case DataTypes.cw0:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x4c2a;
                        blockSize = 2227;
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,报告期,date,4,0,4,;" +
"gxrq,更新日期,date,4,0,0,;" +
"ssrq,上市日期,date,4,0,8,;" +
"col1,每股收益,single,4,0,12,;" +
"col2,每股净资产,single,4,0,16,;" +
"col3,净资产收益率,single,4,0,20,;" +
"col4,每股经营现金,single,4,0,24,;" +
"col5,每股公积金,single,4,0,28,;" +
"col6,每股未分配,single,4,0,32,;" +
"col7,股东权益比,single,4,0,36,;" +
"col8,净利润同比,single,4,0,40,;" +
"col9,主营收入同比,single,4,0,44,;" +
"col10,销售毛利率,single,4,0,48,;" +
"col11,调整每股净资产,single,4,0,52,;" +
"col12,总资产,single,4,0,56,;" +
"col13,流动资产,single,4,0,60,;" +
"col14,固定资产,single,4,0,64,;" +
"col15,无形资产,single,4,0,68,;" +
"col16,流动负债,single,4,0,72,;" +
"col17,长期负债,single,4,0,76,;" +
"col18,总负债,single,4,0,80,;" +
"col19,股东权益,single,4,0,84,;" +
"col20,资本公积金,single,4,0,88,;" +
"col21,经营现金流量,single,4,0,92,;" +
"col22,投资现金流量,single,4,0,96,;" +
"col23,筹资现金流量,single,4,0,100,;" +
"col24,现金增加额,single,4,0,104,;" +
"col25,主营收入,single,4,0,108,;" +
"col26,主营利润,single,4,0,112,;" +
"col27,营业利润,single,4,0,116,;" +
"col28,投资收益,single,4,0,120,;" +
"col29,营业外收支,single,4,0,124,;" +
"col30,利润总额,single,4,0,128,;" +
"col31,净利润,single,4,0,132,;" +
"col32,未分配利润,single,4,0,136,;" +
"col33,总股本,single,4,0,140,;" +
"col34,无限售股合计,single,4,0,144,;" +
"col35,A股,single,4,0,148,;" +
"col36,B股,single,4,0,152,;" +
"col37,境外上市股,single,4,0,156,;" +
"col38,其他流通股,single,4,0,160,;" +
"col39,限售股合计,single,4,0,164,;" +
"col40,国家持股,single,4,0,168,;" +
"col41,国有法人股,single,4,0,172,;" +
"col42,境内法人股,single,4,0,176,;" +
"col43,境内自然人股,single,4,0,180,;" +
"col44,其他发起人股,single,4,0,184,;" +
"col45,募集法人股,single,4,0,188,;" +
"col46,境外法人股,single,4,0,192,;" +
"col47,境外自然人股,single,4,0,196,;" +
"col48,优先股或其他,single,4,0,200,";     

                         break;
                    #endregion
                    #region 最新行情STKINFO60.DAT//OK
                    case DataTypes.hq0:
                    case DataTypes.hq0_ag:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x6D0226;
                        blockSize = 0;
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"jc,简称,string,32,1,10,;" +
"rq,更新时间,datetime,4,5,60,;" +
"zs,昨收,single,4,7,68,;" +
"kp,今开,single,4,8,72,;" +
"zg,最高,single,4,9,76,;" +
"zd,最低,single,4,10,80,;" +
"sp,最新,single,4,11,84,;" +
"sl,总手数,single,4,12,88,;" +
"je,金额,single,4,13,92,;" +
"xss,现手数,single,4,14,96,;" +
"ztj,涨停价,single,4,27,184,;" +
"dtj,跌停价,single,4,28,188,;" +
"np,内盘,single,4,27,192,;" +
"wp,外盘,single,4,28,196,;" +
"mrjg1,买一价,single,4,15,100,;" +
"mrsl1,买一量,single,4,18,120,;" +
"mrjg2,买二价,single,4,16,104,;" +
"mrsl2,买二量,single,4,19,124,;" +
"mrjg3,买三价,single,4,17,108,;" +
"mrsl3,买三量,single,4,20,128,;" +
"mrjg4,买四价,single,4,32,112,;" +
"mrsl4,买四量,single,4,34,132,;" +
"mrjg5,买五价,single,4,33,116,;" +
"mrsl5,买五量,single,4,35,136,;" +
"mcjg1,卖一价,single,4,21,140,;" +
"mcsl1,卖一量,single,4,24,160,;" +
"mcjg2,卖二价,single,4,22,144,;" +
"mcsl2,卖二量,single,4,25,164,;" +
"mcjg3,卖三价,single,4,23,148,;" +
"mcsl3,卖三量,single,4,26,168,;" +
"mcjg4,卖四价,single,4,36,152,;" +
"mcsl4,卖四量,single,4,38,172,;" +
"mcjg5,卖五价,single,4,37,156,;" +
"mcsl5,卖五量,single,4,39,176,";
                        //"jd,精度,int,4,3,52,;" +
                        //"scbz,删除标志,int,4,4,56,";
                        //"unknown,(未知),int,4,31,164,;" +
                        //",(未知),,48,40,200,;"

                              
                        break;
                    #endregion
                    #region 分笔成交数据文件report.dat（结构同day.dat，但其中一些数据不是直接保存）//OK
                    case DataTypes.hqmb:
                        fileName = "REPORT.DAT";
                        //fileName = "20080926.PRP";
                        startAddress = 0x41000;
                        blockSize = 12272;//52*236=12272
                        recordSize = 52;
                        codeIsLong = false;
                        isIndexDataStruct = false;//不完全等同于day.dat结构，因此单独处理
                        fieldString =
                        "dm,代码,code,10,0,0,;" +
                        "rq,日期,datetime,4,0,0,;" +
                        "zjcj,最近成交价,single,4,1,4,;" +
                        "zss,总手数,single,4,2,8,calc;" +
                        "je,金额,single,4,3,12,;" +
                        "xss,现手数,single,4,2,8,;" +
                        "mm,内外盘,string,2,16,21,;" +
                        "mr1jg,买一价,single,1,10,42,;" +
                        "mr1sl,买一量,single,2,4,22,;" +
                        "mr2jg,买二价,single,1,11,43,;" +
                        "mr2sl,买二量,single,2,5,24,;" +
                        "mr3jg,买三价,single,1,12,44,;" +
                        "mr3sl,买三量,single,2,6,26,;" +
                        "mr4jg,买四价,single,1,12,45,;" +
                        "mr4sl,买四量,single,2,6,28,;" +
                        "mr5jg,买五价,single,1,12,46,;" +
                        "mr5sl,买五量,single,2,6,30,;" +
                        "mc1jg,卖一价,single,1,13,47,;" +
                        "mc1sl,卖一量,single,2,7,32,;" +
                        "mc2jg,卖二价,single,1,14,48,;" +
                        "mc2sl,卖二量,single,2,8,34,;" +
                        "mc3jg,卖三价,single,1,15,49,;" +
                        "mc3sl,卖三量,single,2,9,36,;" +
                        "mc4jg,卖四价,single,1,14,50,;" +
                        "mc4sl,卖四量,single,2,8,38,;" +
                        "mc5jg,卖五价,single,1,14,51,;" +
                        "mc5sl,卖五量,single,2,8,40,;" +
                        "bs,总笔数,int,2,0,16,"
                        ;
                        //以上数据类型不是存储类型，程序中不直接用实际数据类型：买/卖X量为short，买/卖X价为byte
                        //现手数通过当总手数计算而得，应该放在总手数后面
                       
                        break;
                    #endregion
                    #region 日线数据文件day.dat//OK
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
                    #region 1分钟数据文件min1.dat//OK
                    case DataTypes.hq1:
                        fileName = "MIN1.DAT";
                        startAddress = 0x41000;
                        blockSize = 16384;//块大小为：16384/32＝512；原分析家是8192
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
                    #region 5分钟数据文件min.dat//OK
                    case DataTypes.hq5:
                        fileName = "MIN.DAT";
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
                    #region 股本结构STKCapital.fdt
                    case DataTypes.gb:
                        fileName = "STKCAPITAL.FDT";
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
                    #region 财务数据STKFinance.fdt
                    case DataTypes.cw:
                        fileName = "STKFINANCE.FDT";
                        startAddress = 0x41000;
                        blockSize = 14848;
                        recordSize = 464;
                        codeIsLong = true;
                        fieldString =
"dm,代码,code,12,0,0,;" +
"rq,日期,date,4,,460,;" +
"bsdqtzje,短期投资净额,double,8,1,12,;" +
"bsyszkje,应收帐款净额,double,8,2,20,;" +
"bschje,存货净额,double,8,3,28,;" +
"bsldzc,流动资产,double,8,4,36,;" +
"bscqtzje,长期投资净额,double,8,5,44,;" +
"bsgdzc,固定资产,double,8,6,52,;" +
"bswxzc,无形及其他资产,double,8,7,60,;" +
"bszzc,总资产,double,8,8,68,;" +
"bsdqjk,短期借款,double,8,9,76,;" +
"bsyfzk,应付帐款,double,8,10,84,;" +
"bsldfz,流动负债,double,8,11,92,;" +
"bscqfz,长期负债,double,8,12,100,;" +
"bsfz,负债合计,double,8,13,108,;" +
"bsgb,股本,double,8,14,116,;" +
"bsssgdqy,少数股东权益,double,8,15,124,;" +
"bsgdqy,股东权益,double,8,16,132,;" +
"bszbgj,资本公积,double,8,17,140,;" +
"bsyygj,盈余公积,double,8,18,148,;" +
"iszysr,主营业务收入净额,double,8,1,156,;" +
"iszycb,主营业务成本,double,8,2,164,;" +
"iszylr,主营业务利润,double,8,3,172,;" +
"isqtlr,其它业务利润,double,8,4,180,;" +
"isyyfy,营业费用,double,8,5,188,;" +
"isglfy,管理费用,double,8,6,196,;" +
"iscwfy,财务费用,double,8,7,204,;" +
"istzsy,投资收益,double,8,8,212,;" +
"islrze,利润总额,double,8,9,220,;" +
"issds,所得税,double,8,10,228,;" +
"isjlr,净利润,double,8,11,236,;" +
"iskchjlr,扣除经常性损益后的净利润,double,8,12,244,;" +
"iswfplr,未分配利润,double,8,13,252,;" +
"cfjyhdxjlr,经营活动现金流入,double,8,1,260,;" +
"cfjyhdxjlc,经营活动现金流出,double,8,2,268,;" +
"cfjyhdxjje,经营活动现金净额,double,8,3,276,;" +
"cftzxjlr,投资现金流入,double,8,4,284,;" +
"cftzxjlc,投资现金流出,double,8,5,292,;" +
"cftzxjje,投资现金净额,double,8,6,300,;" +
"cfczxjlr,筹措现金流入,double,8,7,308,;" +
"cfczxjlc,筹措现金流出,double,8,8,316,;" +
"cfczxjje,筹措现金净额,double,8,9,324,;" +
"cfxjjze,现金及现金等价物净增额,double,8,10,332,;" +
"cfxsspxj,销售商品收到的现金,double,8,11,340,;" +
"mgsy,每股收益,single,4,1,348,;" +
"mgjzc,每股净资产,single,4,2,352,;" +
"tzmgjzc,调整后每股净资产,single,4,3,356,;" +
"mgzbgjj,每股资本公积金,single,4,4,360,;" +
"mgwfplr,每股未分配利润,single,4,5,364,;" +
"mgjyxjllje,每股经营活动产生的现金流量净额,single,4,6,368,;" +
"mgxjzjje,每股现金及现金等价物增加净额,single,4,7,372,;" +
"mll,毛利率,single,4,8,376,;" +
"zyywlrl,主营业务利润率,single,4,9,380,;" +
"jll,净利率,single,4,10,384,;" +
"zzcbcl,总资产报酬率,single,4,11,388,;" +
"jzcsyl,净资产收益率,single,4,12,392,;" +
"xsxjzb,销售商品收到的现金占主营收入比例,single,4,13,396,;" +
"yszczzl,应收帐款周转率,single,4,14,400,;" +
"chzzl,存货周转率,single,4,15,404,;" +
"gdzczzl,固定资产周转率,single,4,16,408,;" +
"zyywzzl,主营业务增长率,single,4,17,412,;" +
"jlrzzl,净利润增长率,single,4,18,416,;" +
"zzczzl,总资产增长率,single,4,19,420,;" +
"jzczzl,净资产增长率,single,4,20,424,;" +
"ldbl,流动比率,single,4,21,428,;" +
"sdbl,速动比率,single,4,22,432,;" +
"zcfzbl,资产负债比率,single,4,23,436,;" +
"fzbl,负债比率,single,4,24,440,;" +
"gdqybl,股东权益比率,single,4,25,444,;" +
"gdzcbl,固定资产比率,single,4,26,448,;" +
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
                    case DataTypes.jjjz:
                        //fileName = "FUNDWEEK.FDT";
                        fileName = "FUNDINFO.fdt";
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
                        fileName = "FUNDINVEST.fdt";
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
                    #region 板块userdata\block
                    case DataTypes.bk:
                        fileName = "BLOCK.DEF";
                        startAddress = 0;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"lb,类别,string,20,0,0,;" +
"bk,板块,string,20,1,10,;" +
"dm,证券代码,string,10,2,42,";
                        break;
                     #endregion
                    #region 评级
                    case DataTypes.pj:
                        fileName = "评级.str";
                        //fileName = "SIMU.DAT";
                        startAddress = 0;
                        blockSize = 256;
                        recordSize = 256;
                        codeIsLong = true;
                        isIndexDataStruct = false;
                        fieldString =
"dm,证券代码,string,12,0,0,;" +
"pj,评级,string,2,2,0,;" +
"sm,说明,string,244,2,0,";
                        break;
                    #endregion
                    #region 复权行情，计算而得//OK
                    case DataTypes.hqfq:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,代码,code,10,0,0,;" +
"rq,日期,date,4,1,0,;" +
"kp,开盘复权价,single,4,2,4,B;" +
"zg,最高复权价,single,4,3,8,B;" +
"zd,最低复权价,single,4,4,12,B;" +
"sp,收盘复权价,single,4,5,16,B;" +
"sl,复权成交数量,single,4,6,20,A;" +
"je,成交金额,single,4,7,24,;"+
"spsyl,收盘收益率,single,4,0,0,";
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
        private string fxjPath = "";
        private string fxjDataPath="";
        private string[,] fxjMarket;
        private string msg = "";
        private DateTime date19700101 = new DateTime(1970, 1, 1);
        private FileStream fs; private BinaryReader br;
        private void checkFileStream(string fxjFileName)
        {
            if (this.fs == null || (this.fs != null && this.fs.Name.ToUpper() != fxjFileName))
            {
                if (this.fs != null)
                {
                    fs.Close();
                    br.Close();
                }
                fs = new FileStream(fxjFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                br = new BinaryReader(fs);
            }
        }
        public void FileClose()
        {
            if (this.fs != null)
            {
                fs.Close();
                br.Close();
            }   
        }
        
        public string Version
        {
            get
            {
                return ("1.0.0.0");
            }
        }
        public int Error
        {
            get
            {
                if (msg != "") return 1;
                else return 0;
            }
        }
        public string Msg
        {
            get { return (msg); }
        }
        public string FxjPath   //属性FxjPath
        {
            get
            {
                return (fxjPath);
            }
            set
            {
                fxjPath = value;
                fxjPath = fxjPath.Trim().ToUpper();
                if (fxjPath != "" && !fxjPath.EndsWith(@"\"))
                {
                    fxjPath += @"\";
                }
                fxjPath = fxjPath.ToUpper();
            }
        }
        public string FxjDataPath   //属性FxjDataPath
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

        public void ShowAboutBox()
        {
            FinData.AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
        public void ShowFxjConverter()
        {
            FxjConverter fxjConverter = new FxjConverter();
            fxjConverter.ShowDialog();
        }
        public void ShowFxjReader()
        {
            FxjReader fxjReader = new FxjReader();
            fxjReader.ShowDialog();
        }

        public string[,] GetMarkets()
        {
                return (fxjMarket);
        }
        public string[,] GetTables()
        {
            if (tableNames[0, 2] == "")
            {
                for (int i = 0; i < tableNames.GetLength(0); i++)
                {
                    DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), tableNames[i,0].ToLower());
                    fileStruct fxjFileStruct = new fileStruct(d);
                    tableNames[i, 2] = fxjFileStruct.fileName;
                }

            }

            return tableNames;
        }
        public string GetTableDef(string dataType, string descDataType, bool delOldTable)
        {
            dataType = dataType.Trim(); descDataType = descDataType.Trim();
            string result = "";
            fileStruct fxjFileStruct = new fileStruct((DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower()));
            switch (descDataType.ToUpper())
            {
                case "SAS":
                    for (int i = 0; i < fxjFileStruct.fields.GetLength(0); i++)
                    {
                        if (result != "") result += ",";
                        result += fxjFileStruct.fields[i, 0];//字段
                        if ("  ,code,string".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " char(" + fxjFileStruct.fields[i, 3] + ") format=$" + fxjFileStruct.fields[i, 3] + "."; //字符串
                        }
                        else if ("  ,int,single,double".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num "; //数值类型
                        }
                        else if ("  ,date".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num format=YYMMDD10."; //date类型
                        }
                        else if ("  ,datetime".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num format=datetime."; //datetime类型
                        }
                        result += " label='" + fxjFileStruct.fields[i, 1] + "'";//标签

                    }
                    result = "create table FinData." + dataType + "(" + result + ");";
                    if (delOldTable == true)
                    {
                        result = "drop table FinData." + dataType + ";" + result;
                    }
                    result = "proc sql;" + result + "quit;";
                    break;
                case "SASINPUT"://用于SAS直接读取数据时所用的INPUT语句，需进一步修改
                    for (int i = 0; i < fxjFileStruct.fields.GetLength(0); i++)
                    {
                        if ("  ,code,string".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " $" + fxjFileStruct.fields[i, 3] + "."; //字符串
                        }
                        else if ("  ,int,date,datetime".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " ib" + fxjFileStruct.fields[i, 3] + "."; //数值类型
                        }
                        else if ("  ,single".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " float" + fxjFileStruct.fields[i, 3] + "."; //数值类型
                        }
                        else if ("  ,double".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " rb" + fxjFileStruct.fields[i, 3] + "."; //数值类型
                        }
                    }
                    break;
                case "FIELDS"://列出字段名称
                    for (int i = 0; i < fxjFileStruct.fields.GetLength(0); i++)
                    {
                        result += " " + fxjFileStruct.fields[i, 0] ;
                    }
                    break;

                default:
                    result = "";
                    break;
            }
            return result;

        }
        public string GetCodeType(string code)
        {
            code = code.Trim().ToUpper();
            if (Regex.IsMatch(code, @"(SH000300)") == true)
            {
                return "zs";
            }
            if (Regex.IsMatch(code, @"(SH60[0-8]\d{3})|(SH90\d{4})|(SZ00[01256789]\d{3})|(SZ20\d{4})|(SZ4[02]\d{4})") == true)
            {
                return "gp";
            }
            else if (Regex.IsMatch(code, @"(SH00000\d)|(SH00001[0-6])") == true)
            {
                return "zs";
            }
            else if (Regex.IsMatch(code, @"(SH[012]\d{5})|(SZ1[0123]\d{4})") == true && Regex.IsMatch(code, @"(SH181\d{3})") == false && Regex.IsMatch(code, @"(SH190\d{3})") == false)
            {
                return "zq";
            }
            else if (Regex.IsMatch(code, @"(SH5[01]\d{4})|(SZ184\d{3})|(SZ1[56]\d{4})") == true)
            {
                return "jj";
            }
            else if (Regex.IsMatch(code, @"(SH58\d{4})|(SZ03\d{4})") == true)
            {
                return "qz";
            }
            else if (Regex.IsMatch(code, @"(SH000\d{3})|(SZ399\d{3})|(SH8[013]\d{4})") == true)
            {
                return "zs";
            }
            return "";
        }
        private string [] GetCodes(string Market)   //读取Day.dat中的代码
        {
            //沪市指数代码转换表,分析家同时保存沪市两类代码
            string[,] codesRename = new string[,] 
            {
            {"SH1A0001","SH000001"},
            {"SH1A0002","SH000002"},
            {"SH1A0003","SH000003"},
            {"SH1B0001","SH000004"},
            {"SH1B0002","SH000005"},
            {"SH1B0004","SH000006"},
            {"SH1B0005","SH000007"},
            {"SH1B0006","SH000008"},
            {"SH1B0007","SH000010"},
            {"SH1B0008","SH000011"},
            {"SH1B0009","SH000012"},
            {"SH1B0010","SH000013"},
            {"SH1C0003","SH000016"}         
            };
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
                this.checkFileStream(FxjFile);
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
                            code = code.ToUpper();
                            for (int icode = 0; icode < codesRename.GetLength(0); icode++)
                            {
                                code = code.Replace(codesRename[icode, 0], codesRename[icode, 1]);
                            }
                            codes[i] = code;
                        }
                    }
                    //fs.Close();
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
        private string[,] GetFields(DataTypes dataType)
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

        public string[,] GetData(string dataType, string code,int iRecordCount)
        {
            return GetData(dataType, code, "", iRecordCount);
        }
        public string[,] GetData(string dataType, string code, string newFileName, int iRecordCount) //newFileName 历史分时数据
        {
            try
            {
                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());

                if (d == DataTypes.hq0_ag ) return GetAG0(iRecordCount);
                
                return GetData(d, code, newFileName, iRecordCount);
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
        private string[,] GetData(DataTypes dataType,string code,string newFileName,int iRecordCount) //读取数据，重载
        {
            if (dataType == DataTypes.bk) return GetBK(code);
            if (dataType == DataTypes.pj) return GetPJ(code);
            if (dataType == DataTypes.hqfq) return GetHqfq(dataType,code,newFileName,iRecordCount );
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
            System.Globalization.CultureInfo cnCultureInfo = new System.Globalization.CultureInfo("zh-CN");
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
            FxjFile = FxjFile.ToUpper();
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
                #region 处理DAY.DAT等结构（索引/数据）的数据，目前包括分时数据、1分钟数据、5分钟数据、日数据...
                try
                {
                    this.checkFileStream(FxjFile);
                    int secCounts = 0;//文件中证券总数
                    string code0 = "";
                    len = fs.Length;
                    fs.Position = 12;
                    secCounts = br.ReadInt32();
                    bool codeRead = false;
                    for (int i = 0; i < secCounts && codeRead==false; i++)
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
                                codeRead = true;
                            }
                        }
                    }
                    int iRecord = 1;//记录
                    int iBlock = 0;//第iBlock块
                    int fieldCounts = fxjFileStruct.fields.GetLength(0);

                    while (iBlock < 25 && blocks[iBlock] != -1)
                    {
                        int r = 0;
                        ////long tempAddress = 0;
                        ////long tempAddress1 = 0;
                        ////long tempResult = 0;
                        ////int tempPos = 0;
                         //while (iRecord < 20000)   //test
                       while (iRecord < recordCounts + 1 && r < fxjFileStruct.blockSize / fxjFileStruct.recordSize)
                        {
                            string[] record = new string[fieldCounts];
                            //pos = fxjFileStruct.startAddress + r * fxjFileStruct.recordSize;//test，用来推测单个blocks数据块的大小
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
                                    case "datetime":

                                        ////tempAddress1 = tempAddress; //lps
                                        ////tempAddress = fs.Position; //lps

                                        ////if (iRecord > 1) //lps
                                        ////    tempResult = tempAddress - tempAddress1;  //lps

                                        ////intField = br.ReadInt32();
                                        ////if (intField >= 14148.625 * 86400)//test
                                        ////    Console.WriteLine(tempAddress.ToString());

                                        ////record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        ////record[iField] = record[iField] + "|地址：" + tempAddress.ToString() + "(" + tempResult.ToString() + ")";  //test


                                        intField = br.ReadInt32();
                                        record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        break;
                                    case "int":
                                        intField = br.ReadInt32();
                                        record[iField] = intField.ToString("D", cnCultureInfo);
                                        break;
                                    case "single":
                                        //floatField = br.ReadSingle();
                                        //if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                        //record[iField] = floatField.ToString("G", cnCultureInfo);
                                        doubleField =(double) br.ReadSingle();
                                        if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") doubleField *= 100;
                                        record[iField] = doubleField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F", cnCultureInfo); 
                                        break;
                                    case "double":
                                        doubleField = br.ReadDouble();
                                        record[iField] = doubleField.ToString("F", cnCultureInfo);
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

                    if (iRecordCount == 0 || iRecordCount > recordList.Count) iRecordCount = recordList.Count;
                    string[,] records = new string[iRecordCount, fieldCounts];
                    for (int i = 0; i < iRecordCount; i++)
                    {
                        string[] record0 = (string[])recordList[recordList.Count - 1 - i];
                        //string[] record0 = (string[])recordList[i];
                            for (int j = 0; j < fieldCounts; j++)
                            {
                                records[i, j] = record0[j];
                            }  
                    }

                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
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
                        #region 代码表（处理STKINFO60.DAT等结构的数据）
                        try
                        {
                            this.checkFileStream(FxjFile);
                            string[,] codesRename = new string[,] 
                                    {
                                    {"SH1A0001","SH000001"},
                                    {"SH1A0002","SH000002"},
                                    {"SH1A0003","SH000003"},
                                    {"SH1B0001","SH000004"},
                                    {"SH1B0002","SH000005"},
                                    {"SH1B0004","SH000006"},
                                    {"SH1B0005","SH000007"},
                                    {"SH1B0006","SH000008"},
                                    {"SH1B0007","SH000010"},
                                    {"SH1B0008","SH000011"},
                                    {"SH1B0009","SH000012"},
                                    {"SH1B0010","SH000013"},
                                    {"SH1C0003","SH000016"}         
                                    };
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
                                if (Regex.IsMatch(code0, @"(1[ABC]00\d\d)") == false)
                                {
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
                                                for (int icode = 0; icode < codesRename.GetLength(0); icode++)
                                                {
                                                    record[iField] = record[iField].Replace(codesRename[icode, 0], codesRename[icode, 1]);
                                                }
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
                                                record[iField] = intField.ToString("D");
                                                break;
                                            case "single":
                                                floatField = br.ReadSingle();
                                                if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                record[iField] = floatField.ToString("F");
                                                break;
                                            case "double":
                                                doubleField = br.ReadDouble();
                                                record[iField] = doubleField.ToString("F");
                                                break;
                                            case "string":
                                                record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                break;
                                        }

                                    }
                                    if (code0 != "") 
                                        recordList.Add(record);
                                }                             
                            }

                            if (iRecordCount == 0 || iRecordCount > recordList.Count) iRecordCount = recordList.Count;
                            string[,] records = new string[iRecordCount, fieldCounts];
                            for (int i = 0; i < iRecordCount; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }                                                       


                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.hq0:
                        #region 最新行情（处理STKINFO60.DAT等结构的数据）
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                                if (fxjFileStruct.codeIsLong == false && code == market + code0 || fxjFileStruct.codeIsLong == true && code == code0 || code0 != "" && code.Length ==2)
                                {
                                    

                                    if (code0 != "" && code.Length ==2)
                                        hasCode = false;
                                    else 
                                        hasCode = true;

                                    string[] record = new string[fieldCounts];
                                    double doubleTemp=0;
                                    for (int iField = 0; iField < fieldCounts ; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        doubleTemp = fs.Position;
                                        switch (fxjFileStruct.fields[iField, 2].ToLower())
                                        {
                                            case "code":
                                                record[iField] = code;
                                                if (code0 != "" && code.Length == 2)
                                                    record[iField] = market + code0;
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
                                                record[iField] = intField.ToString("D");
                                                break;
                                            case "single":
                                                doubleField = (double)br.ReadSingle();
                                                if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") doubleField *= 100;
                                                record[iField] = doubleField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F", cnCultureInfo); 
                                                break;
                                            case "double":
                                                doubleField = br.ReadDouble();
                                                record[iField] = Math.Round(doubleField, 2).ToString("F");
                                                break;
                                            case "string":
                                                record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                break;
                                        }

                                        ////Console.WriteLine((doubleTemp.ToString())+":" + record[iField]);
                                    }
                                    if (code0 != "")
                                        recordList.Add(record);
                                }
                            }

                            if (iRecordCount == 0 || iRecordCount > recordList.Count) iRecordCount = recordList.Count;
                            string[,] records = new string[iRecordCount, fieldCounts];
                            for (int i = 0; i < iRecordCount; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cq:
                        #region 分红送配（处理STKINFO60.DAT等结构的数据）//除权
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                                                    record[iField] = intField.ToString("D");
                                                    break;
                                                case "single":
                                                    floatField = br.ReadSingle();
                                                    if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                    record[iField] = Math.Round(floatField, 2).ToString("F");
                                                    break;
                                                case "double":
                                                    doubleField = br.ReadDouble();
                                                    record[iField] = Math.Round(doubleField, 2).ToString("F");
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

                            if (iRecordCount == 0 || iRecordCount > recordList.Count) iRecordCount = recordList.Count;
                            string[,] records = new string[iRecordCount, fieldCounts];
                            for (int i = 0; i < iRecordCount; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cw0:
                        #region 财务数据--简单（处理STKINFO60.DAT等结构的数据）
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                                                record[iField] = intField.ToString("D");
                                                break;
                                            case "single":
                                                floatField = br.ReadSingle();
                                                if (fxjFileStruct.fields[iField, 6].ToUpper() == "A") floatField *= 100;
                                                record[iField] = Math.Round(floatField, 3).ToString("F3");
                                                break;
                                            case "double":
                                                doubleField = br.ReadDouble();
                                                record[iField] = Math.Round(doubleField, 3).ToString("F3");
                                                break;
                                            case "string":
                                                record[iField] = System.Text.Encoding.Default.GetString(br.ReadBytes(Convert.ToInt32(fxjFileStruct.fields[iField, 3]))).Replace("\0", "");
                                                break;
                                        }

                                    }
                                    recordList.Add(record);
                                }

                            }

                            if (iRecordCount == 0 || iRecordCount > recordList.Count) iRecordCount = recordList.Count;
                            string[,] records = new string[iRecordCount, fieldCounts];
                            for (int i = 0; i < iRecordCount; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }
                            if (records.GetLength(0) == 0) msg = "没有读到数据!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.hqmb:
                        #region 处理Report.DAT数据（结构类似DAY.DAT，但有些数值需要进一步计算而来）
                        try
                        {
                            this.checkFileStream(FxjFile);
                            int secCounts = 0;//文件中证券总数
                            string code0 = "";
                            len = fs.Length;
                            fs.Position = 12;
                            secCounts = br.ReadInt32();
                            bool codeRead = false;
                            for (int i = 0; i < secCounts && codeRead==false; i++)
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
                                        codeRead = true;
                                    }
                                }
                            }
                            int iRecord = 1;//记录
                            int iBlock = 0;//第iBlock块
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            while (iBlock < 25 && blocks[iBlock] != -1)
                            {
                                int r = 0;
                                //long tempAddress = 0;
                                //long tempAddress1 = 0;
                                //long tempResult = 0;
                                //int tempPos = 0;
                                UInt16 curValue_bs, preValue_bs = 0;
                                while (iRecord < recordCounts + 1 && r < fxjFileStruct.blockSize / fxjFileStruct.recordSize)   //12272/52=236条记录
                                {

                                    string[] record = new string[fieldCounts];
                                    pos = fxjFileStruct.startAddress + blocks[iBlock] * fxjFileStruct.blockSize + r * fxjFileStruct.recordSize;

                                    #region 调试数据结构
                                    /*
                                //tempPos = 0x41000 + 0x0047 * 0x2FF0;
                                //tempPos = 0x41000 + 0x0427 * 0x2FF0;
                                //tempPos = 0x41000 + 0x0573 * 0x2FF0;
                                tempPos = 0x41000 + 0x06CC * 0x2FF0;
                                //tempPos = 0x41000 + 0x0836 * 0x2FF0;

                                while (iRecord < 250 + 1 && r < 250)  
                                {
                                    string[] record = new string[fieldCounts];
                                    pos = fxjFileStruct.startAddress + blocks[iBlock] * fxjFileStruct.blockSize + r * fxjFileStruct.recordSize;


                                   
                                    //for (int iField = 0; iField < recordCounts ; iField ++)
                                    //{

                                    record[0] = code;

                                    fs.Position = tempPos;
                                    tempAddress1 = tempAddress;
                                    tempAddress = fs.Position; //lps
                                        intField = br.ReadInt32();
                                        record[1] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        tempPos = tempPos + 52;


                                        if (iRecord > 1)
                                            tempResult = tempAddress - tempAddress1;

                                        floatField = tempAddress;
                                        record[4] = "地址：" + floatField.ToString() + "(" + tempResult.ToString() + ")"; 

                                    //}
                                */
                                    #endregion

                                    for (int iField = 0; iField < fieldCounts; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        switch (fxjFileStruct.fields[iField, 0].ToLower()) //这里与读取DAY.DAT用法不同，判断的是代码而不是类型
                                        {
                                            case "dm":
                                                record[iField] = code;
                                                break;
                                            case "rq":
                                                //tempAddress1 = tempAddress; //lps
                                                //tempAddress = fs.Position; //lps

                                                //if (iRecord > 1) //lps
                                                //    tempResult = tempAddress - tempAddress1;  //lps
                                                
                                                intField = br.ReadInt32();
                                                //if (intField >= 14148.625*86400)//test
                                                //    intField = intField;
                                            
                                                record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                                ////record[iField] = record[iField] + "|地址：" + tempAddress.ToString() + "(" + tempResult.ToString() + ")";  //test                                                
                                                break;
                                            case "zjcj":
                                            case "zss":
                                            case "je":
                                                floatField = br.ReadSingle();
                                                record[iField] = floatField.ToString("_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F");                                              
                                                break;
                                            case "mr1sl":
                                            case "mr2sl":
                                            case "mr3sl":
                                            case "mr4sl":
                                            case "mr5sl":
                                            case "mc1sl":
                                            case "mc2sl":
                                            case "mc3sl":
                                            case "mc4sl":
                                            case "mc5sl":
                                                record[iField] = br.ReadUInt16().ToString("D");
                                                //if (iField==26)
                                                //    record[iField] = br.ReadUInt16().ToString("D");
                                                break;
                                            case "mr1jg":
                                            case "mr2jg":
                                            case "mr3jg":
                                            case "mr4jg":
                                            case "mr5jg":
                                            case "mc1jg":
                                            case "mc2jg":
                                            case "mc3jg":
                                            case "mc4jg":
                                            case "mc5jg":
                                                //int temp = 0;//test
                                                //if (fxjFileStruct.fields[iField, 0].ToLower() == "mc5jg")
                                                //     temp =1;

                                                float jg=br.ReadSByte();
                                                if ("_jj_qz".IndexOf(this.GetCodeType(code)) > 0)
                                                {
                                                    jg = Convert.ToSingle(record[2]) + jg / 1000;
                                                    record[iField] = jg.ToString("F3");
                                                }
                                                else
                                                {
                                                    jg = Convert.ToSingle(record[2]) + jg / 100;
                                                    record[iField] = jg.ToString("F");
                                                }
                                                break;
                                            case "xss":
                                                record[iField] = "";//现手数在下面计算
                                                break;
                                            case "mm":
                                                int mm = br.ReadSByte();
                                                record[iField] = "";
                                                if (mm == -128) record[iField] = "内盘"; //-128 = 0x80
                                                if (mm == -64) record[iField] = "外盘";  //-64 = 0xC0
                                                break;
                                            case "bs":                                                
                                                curValue_bs = br.ReadUInt16();
                                                record[iField] = (curValue_bs - preValue_bs).ToString("D");//当前笔数＝当前总笔数减上一次笔数
                                                preValue_bs = curValue_bs;
                                                break;
                                        }
                                        
                                    }
                                    recordList.Add(record);
                                    
                                    r = r + 1;
                                    iRecord = iRecord + 1;
                                }
                                iBlock = iBlock + 1;
                            }

                            float zssSaved = 0;
                            string[,] records = new string[recordList.Count, fieldCounts];
                            for (int i = 0; i < recordList.Count; i++)
                            {
                                string[] record0 = (string[])recordList[i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    if (j == 5)  //现手数
                                    {
                                        record0[j] = (Convert.ToSingle(record0[3]) - zssSaved).ToString();
                                        zssSaved = Convert.ToSingle(record0[3]);
                                    }
                                    records[i, j] = record0[j];
                                }
                            }
                            
                            if (iRecordCount == 0 || iRecordCount >recordList.Count ) iRecordCount = recordList.Count;
                            records = new string[iRecordCount, fieldCounts];
                            for (int i = 0; i<iRecordCount; i++)
                            {
                                string[] record0 = (string[])recordList[recordList.Count - 1 - i];
                                for (int j = 0; j < fieldCounts; j++)
                                {
                                    records[i, j] = record0[j];
                                }
                            }

                            if (records.GetLength(0) == 0) msg = "没有读到数据!";

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
        private string[,] GetBK(string code)//板块定义数据
        {
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(DataTypes.bk);
            if (code == null) code = "";
            code = code.Trim().ToUpper();
            ArrayList recordList = new ArrayList();
            if (this.FxjDataPath == "")
            {
                msg = @"无法在注册表中到分析家数据文件目录，请自行将属性 FxjDataPath设置为有效路径，如c:\fxj\data\。";
                return new string[1, 1] { { null } };
            }
            string FxjBlockPath = fxjDataPath;
            FxjBlockPath = FxjBlockPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\BLOCK\\") ; //假设目录中含有data文字
            string FxjFile = FxjBlockPath + fxjFileStruct.fileName;
            
            msg = "";
            if (!File.Exists(FxjFile))
            {
                msg = "板块文件无法找到。";
                return new string[1, 1] { { null } };
            }
            try
            {
                this.checkFileStream(FxjFile);
                string bklines="";string lb="";string bk="";
                string bkFile = ""; string dmLines = ""; int n = -1;
                bklines=System.Text.Encoding.Default.GetString(br.ReadBytes((int)fs.Length));
                string[] bks = bklines.Replace("\r\n","\n").Split(new Char[] { '\n' });
                for(int i =0 ; i<bks.Length;i++)
                {
                    if (bks[i] != "")
                    {
                        bks[i] = bks[i].Trim();
                        if (bks[i].StartsWith("[") && bks[i].EndsWith("]"))
                        {
                            lb = bks[i].Replace("[","").Replace("]","");
                        }
                        else
                        {
                            bk = bks[i];
                            if (bk != "")
                            {
                                if (code == "" || (code!="" && bk.ToUpper()==code) )
                                {
                                    bkFile = FxjBlockPath + bk + ".blk";
                                    if (File.Exists(bkFile))
                                    {
                                        StreamReader bkReader = new StreamReader(bkFile);
                                        bkReader.Read(); bkReader.Read();
                                        dmLines = bkReader.ReadToEnd();
                                        dmLines = dmLines.Replace("\x05", "\0").Replace("\0Z00", "\0\0\0\0").Replace("\0\0\0\0", ",");
                                        string[] dms = dmLines.Split(',');
                                        string[,] record = new string[dms.Length, 3];
                                        for (int r = 0; r < dms.Length; r++)
                                        {
                                            if (dms[r] != "")
                                            {
                                                n = n + 1;
                                                record[r, 0] = lb;
                                                record[r, 1] = bk;
                                                record[r, 2] = dms[r];
                                            }
                                        }
                                        recordList.Add(record);
                                    }
                                }
                            }
                        }
                    }
                }
                if (n > 0)
                {
                    string[,] records = new string[n+1, 3];
                    int rr = 0;
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        string[,] record0 = (string[,])recordList[i];
                        for (int j = 0; j < record0.GetLength(0); j++)
                        {
                            if (record0[j, 0] != null && record0[j, 1] != null && record0[j,2]!=null)
                            {
                                records[rr, 0] = record0[j, 0];
                                records[rr, 1] = record0[j, 1];
                                records[rr, 2] = record0[j, 2];
                                rr = rr + 1;
                            }
                        }
                    }
                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
                    return records;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new string[1, 1] { { null } };

        }
        private string[,] GetPJ(string code)//评级数据
        {
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(DataTypes.pj);
            code = code.Trim().ToUpper();
            ArrayList recordList = new ArrayList();
            if (this.FxjDataPath == "")
            {
                msg = @"无法在注册表中到分析家数据文件目录，请自行将属性 FxjDataPath设置为有效路径，如c:\fxj\data\。";
                return new string[1, 1] { { null } };
            }
            string fxjSubPath = fxjDataPath;
            fxjSubPath = fxjSubPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\SelfData\\"); //假设目录中含有data文字
            string FxjFile = fxjSubPath + fxjFileStruct.fileName;

            msg = "";
            if (!File.Exists(FxjFile))
            {
                msg = fxjFileStruct.fileName +"无法找到。";
                return new string[1, 1] { { null } };
            }
            try
            {
                this.checkFileStream(FxjFile);
                int n = 0;
                int pos =fxjFileStruct.startAddress + n * fxjFileStruct.recordSize;
                fs.Position = pos;
                while (br.PeekChar()!=-1)
                {
                    string[] record = new string[3];
                    pos = fxjFileStruct.startAddress + n * fxjFileStruct.recordSize;
                    fs.Position = pos;
                    record[0]=System.Text.Encoding.Default.GetString(br.ReadBytes(8));//dm 
                    if (code==""||(code!="" && code==record[0]))
                    {
                        fs.Position = pos+12;
                        record[2] = System.Text.Encoding.Default.GetString(br.ReadBytes(244));
                        record[1] = record[2].Substring(0,2).Trim();
                        record[2] = record[2].Replace("\0","").Trim();
                        if(record[0]!="") recordList.Add(record);
                    }
                    n = n + 1;
                }

                if (n > 0)
                {
                    string[,] records = new string[recordList.Count, 3];
                    for (int i = 0; i < recordList.Count; i++)
                    {
                        string[] record0 = (string[])recordList[i];
                        if (record0[0] != null)
                        {
                            records[i, 0] = record0[0];
                            records[i, 1] = record0[1];
                            records[i, 2] = record0[2];
                        }
                    }
                    if (records.GetLength(0) == 0) msg = "没有读到数据!";
                    return records;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new string[1, 1] { { null } };

        }
        private string[,] GetAG0(int iRecordCount)//A股股票的动态行情
        {
            string[,] records_sh = GetData("hq0", "sh", 0);
            string[,] records_sz = GetData("hq0", "sz", 0);
            int iHJSL = records_sh.GetLength(0) + records_sz.GetLength(0);//合计数量
            int iField = records_sh.GetLength(1); //字段数量 
            if (iRecordCount == 0 || iRecordCount > iHJSL) iRecordCount = iHJSL;
            string[,] records = new string[iRecordCount, iField];
            
            int iSHSL=records_sh .GetLength(0);//上海记录数量
            int iSZSL = records_sz.GetLength(0);//深圳记录数量



            for (int i = 0; i < iRecordCount; i++)
            {
                if (i < iSHSL)
                    for (int j = 0; j < iField; j++)
                    {
                        records[i, j] = records_sh[i,j];
                    }
                else
                    for (int j = 0; j < iField; j++)
                    {
                        records[i, j] = records_sz[i - iSHSL, j];
                    }
            }
            if (records.GetLength(0) == 0) msg = "没有读到数据!";

            return records;
        }
        public ArrayList GetStkInfo(string stkType)
        {
            ArrayList recordList = new ArrayList();
            string[,] records_dm=new string[1,1];
            switch (stkType)
            {
                case "sh":
                    records_dm = GetData("dm", "sh", 0);
                    break;
                case "sz":
                    records_dm = GetData("dm", "sz", 0);
                    break;
            }
            
            //string[,] records = GetAG0(0);
            int iZQSL = records_dm.GetLength(0);//证券数量
            for (int i = 0; i < iZQSL; i++)
            {
                
                switch (records_dm[i,0].Substring(0,4).ToLower())
                {
                case "sh60":
                        stkInfo AstkInfo = new stkInfo();
                        string[,] records_mb = GetData("hqmb", records_dm[i, 0], 0);
                        string[,] records_hq0 = GetData("hq0", records_dm[i, 0], 0);
                        int irecords_mb = records_mb.GetLength(0);

                        string[,] records_min1 = new string[241, 60];//分钟线记录数组

                        System.TimeSpan ND = Convert.ToDateTime(records_mb[0, 1]) - Convert.ToDateTime(System.DateTime.Today.ToString("yyyy-MM-dd") + " 9:15:00");
                        
                        int iMinCount = Convert.ToInt32(ND.TotalMinutes);
                        for (int j = 0; j < iMinCount; j++)
                        {
                            int iMinute = 0;
                            int iJYCS = 0;
                            for (int k = 0; k < irecords_mb; k++)
                            {

                                AstkInfo.Code = records_mb[k, 0];
                                AstkInfo.Name = records_dm[i, 1];
                                AstkInfo.Yclose = Convert.ToSingle(records_hq0[0, 3]);
                                AstkInfo.Open = Convert.ToSingle(records_hq0[0, 4]);
                                AstkInfo.High = Convert.ToSingle(records_hq0[0, 5]);
                                AstkInfo.Low = Convert.ToSingle(records_hq0[0, 6]);
                                AstkInfo.New = Convert.ToSingle(records_hq0[0, 7]);
                                //AstkInfo.SumVol_hq0 = Convert.ToInt32(records_hq0[0, 8]);//
                                //AstkInfo.SumAmount_hq0 = Convert.ToInt32(records_hq0[0, 9]);//
                                AstkInfo.PriceZt = Convert.ToSingle(records_hq0[0, 11]);
                                AstkInfo.PriceDt = Convert.ToSingle(records_hq0[0, 12]);
                                //AstkInfo.Inside = Convert.ToInt32(records_hq0[0, 13]);//
                                //AstkInfo.Outside = Convert.ToInt32(records_hq0[0, 14]);//

                                AstkInfo.Time = Convert.ToDateTime(records_mb[k, 1]);
                                //DateTime tTimeTemp;
                                

                                ////if (k = irecords_mb - 1)
                                ////{
                                ////    tTimeTemp = Convert.ToDateTime(records_mb[k, 1]);
                                ////}
                                ////else
                                ////{
                                ////    tTimeTemp = Convert.ToDateTime(records_mb[k + 1, 1]);
                                ////}

                                
                                if (AstkInfo.Time.Minute == iMinute)
                                {
                                    iJYCS = iJYCS + 1;

                                    AstkInfo.Close = AstkInfo.Close + Convert.ToSingle(records_mb[k, 2]);//
                                    AstkInfo.Volume = AstkInfo.Volume + Convert.ToInt32(records_mb[k, 5]);//
                                }
                                else
                                {
                                    AstkInfo.Close = AstkInfo.Close / iJYCS;
                                    //AstkInfo.SumVol = Convert.ToInt32(records_mb[k, 3]);
                                    //AstkInfo.SumAmount = Convert.ToInt32(records_mb[k, 4]);
                                    recordList.Add(AstkInfo);
                                    iJYCS = 0;

                                    iJYCS = iJYCS + 1;

                                    AstkInfo.Close = AstkInfo.Close + Convert.ToSingle(records_mb[k, 2]);//
                                    AstkInfo.Volume = AstkInfo.Volume + Convert.ToInt32(records_mb[k, 5]);//
                                }
                                iMinute = AstkInfo.Time.Minute;


                               
                            }

                        }                      

                        
                	break;
                case "sz00":
                    
                    records_mb = GetData("hqmb", records_dm[i, 0], 0);

                    break;
                }

            }
                return recordList;

        }
        private string[,] GetHqfq(DataTypes dataType, string code, string newFileName, int iRecordCount)//复权价格,分红再投资,向前复权法
        {
            FxjData fxj = new FxjData();
            string[,] hq = fxj.GetData("hq", code, newFileName, iRecordCount);
            if (fxj.Error != 0 || hq.GetLength(1)<4 ) return new string[1, 1] { { null } };
            string[,] x = new string[hq.GetLength(0),9];
            string[,] cq = fxj.GetData("cq", code, newFileName, iRecordCount);
            string fmt = "_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F";
            if (fxj.Error != 0 || cq.GetLength(1) < 4 || cq.GetLength(0)==0) //没有除权信息
            {
                for (int i = 0; i < hq.GetLength(0); i++)
                {
                    for (int j = 0; j < hq.GetLength(1); j++)
                    {
                        x[i, j] = hq[i, j];
                    }
                    if (i == 0)
                    {
                        x[i, hq.GetLength(1)] = "0.00000";
                    }
                    else
                    {
                        x[i, hq.GetLength(1)] = (Single.Parse(hq[i, 5]) / Single.Parse(hq[i - 1, 5]) - 1).ToString("0.00000");
                    }
                }
            }
            else  //有除权信息
            {
                DateTime[] cqdt = new DateTime[cq.GetLength(0)];
                for (int j = 0; j < cq.GetLength(0); j++) cqdt[j] = new DateTime(int.Parse(cq[j, 1].Split('-')[0]), int.Parse(cq[j, 1].Split('-')[1]), int.Parse(cq[j, 1].Split('-')[2]));
                int i0 = hq.GetLength(0) - 1;
                DateTime hqdt_1,hqdt;
                double kp_1,zg_1,zd_1,sp_1,kp,zg,zd,sp,kpx,zgx,zdx,spx,sgbl,kpsyl,zgsyl,zdsyl,spsyl, pgbl, pgjg, fh;
                for (int k = 0; k < 8; k++) x[i0, k] = hq[i0, k];  //最后一条记录
                x[0, 8] = "0.00000";
                kpx = double.Parse(x[i0, 2]);
                zgx = double.Parse(x[i0, 3]);
                zdx = double.Parse(x[i0, 4]);
                spx = double.Parse(x[i0, 5]);
                for (int i = i0; i > 0; i--)
                {
                    sgbl = 0; pgbl = 0; pgjg = 0; fh = 0;
                    hqdt_1 = new DateTime(int.Parse(hq[i - 1, 1].Split('-')[0]), int.Parse(hq[i - 1, 1].Split('-')[1]), int.Parse(hq[i - 1, 1].Split('-')[2]));
                    hqdt = new DateTime(int.Parse(hq[i, 1].Split('-')[0]), int.Parse(hq[i, 1].Split('-')[1]), int.Parse(hq[i, 1].Split('-')[2]));
                    for (int j = 0; j < cq.GetLength(0); j++)
                    {
                        if (hqdt_1 < cqdt[j] && cqdt[j] <= hqdt)
                        {
                            sgbl = double.Parse(cq[j, 2]);
                            pgbl = double.Parse(cq[j, 3]);
                            pgjg = double.Parse(cq[j, 4]);
                            fh = double.Parse(cq[j, 5]);
                        }
                    }
                    x[i-1, 0] = hq[i-1, 0];//dm
                    x[i-1, 1] = hq[i-1, 1];//rq
                    //syl=1+第t日收益率 =( t日收盘价*(1+送股比例+配股比例)+分红金额-配股价格*配股比例)/(t-1日收盘价)
                    kp = double.Parse(hq[i, 2]);
                    zg = double.Parse(hq[i, 3]);
                    zd = double.Parse(hq[i, 4]);
                    sp = double.Parse(hq[i, 5]);
                    kp_1 = double.Parse(hq[i-1, 2]);
                    zg_1 = double.Parse(hq[i-1, 3]);
                    zd_1 = double.Parse(hq[i-1, 4]);
                    sp_1 = double.Parse(hq[i-1, 5]);
                    kpsyl = (kp * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / kp_1; 
                    zgsyl = (zg * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / zg_1; 
                    zdsyl = (zd * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / zd_1; 
                    spsyl = (sp * (1 + sgbl + pgbl) + fh - pgjg * pgbl) / sp_1;
                    kpx = kpx / kpsyl;
                    zgx = zgx / zgsyl;
                    zdx = zdx / zdsyl;
                    spx = spx / spsyl;
                    x[i - 1, 2] = kpx.ToString(fmt);
                    x[i - 1, 3] = zgx.ToString(fmt);
                    x[i - 1, 4] = zdx.ToString(fmt);
                    x[i - 1, 5] = spx.ToString(fmt);
                    x[i - 1, 6] = hq[i - 1, 6];//sl 成交量未复权
                    x[i - 1, 7] = hq[i - 1, 7];//je
                    x[i, 8] = (spsyl - 1).ToString("0.00000");//spsyl 收盘价收益率
                    
                }

            }

            return x;

        }
    }
}
