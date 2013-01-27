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
            #region ����
            {"dm","����",""},
            {"hq","����",""},
            {"hqmb","�ֱʳɽ�",""},
            {"hq0","��̬����",""},
            {"hq0_ag","�Ϻ�������A���г���̬����",""},
            {"hq1","һ��������",""},
            {"hq5","���������",""},
            {"cq","��Ȩ����",""},
            {"cw0","���²�������",""},
            {"fp","�ֺ�����",""},
            {"gb","�ɱ��ṹ",""},
            {"gd","ʮ��ɶ�",""},
            {"cw","��������",""},
            {"jjjz","����ֵ",""},
            {"jjzh","����Ͷ�����",""},
            {"bk","���",""},
            {"pj","����",""},
            {"hqfq","��Ȩ����",""} 

        };//��˳����Datatypeһ�£��зֱ�Ϊ������������������Ӧ�ļ�����GetTables�����и�ֵ��
            #endregion

        private string[,] GetCodeList( string Market,string ColumnName_StockCode,string ColumnName_StockName) //��ȡ֤ȯ�����б�
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

                            //ɾ����ʱ���е�ԭ����¼
                            SqlStr = "Delete T_PRP_StockDataTemp From T_PRP_StockdataTemp";
                            SqlCommand cmd = new SqlCommand(SqlStr, con);
                            cmd.ExecuteNonQuery();

                            //����ʱ���������
                            for (int i = 0; i < dataArray.GetLength(0); i++)
                            {
                                SqlStr = "Insert Into T_PRP_StockDataTemp(FCode,FDateTime,FOpen,FHigh,FLow,FClose,FVolume,FAmount) values('" +
                                dataArray[i, 0] + "','" + dataArray[i, 1] + "','" + dataArray[i, 2] + "','" + dataArray[i, 3] + "','"
                                + dataArray[i, 4] + "','" + dataArray[i, 5] + "','" + dataArray[i, 6] + "','" + dataArray[i, 7] + "')";
                                cmd.CommandText = SqlStr;
                                cmd.ExecuteNonQuery();
                            }

                            //����ʱ������ʽ����Ӳ��ظ��ļ�¼
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
                            //��SQl Server���ݿ��������ʧ�ܣ�
                            MessageBox.Show("��SQl Server���ݿ��������ʧ�ܣ�"); 
                            return 1;
                        }

                    }
                    else
                    {
                        //û�����ݻ������� 
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
                MessageBox.Show("�ɹ�");
            else
                MessageBox.Show("ʧ��");        
        }


        public FxjData()
        {
            try
            {
                //��ע����ж�ȡ����������Ŀ¼����c:\fxj\data
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
                        msg = "û���ҵ����ǻۻ�����Ұ�װ��Ϣ��";
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
        struct stkInfo //��Ҫ�����ɷ�ʱ���黻������ķ�����
        {
            public string Code;	//��Ʊ����
            public string Name;    // ֤ȯ����
            public System.DateTime Time;//����ʱ��

            public float Yclose;	//�������̼�
            public float Open;		//���̼�
            public float High;		//��߼�
            public float Low;		//��ͼ�
            public float Close;	//���̼�
            public float New;//���¼�
            public int Volume;	//�ɽ���
            public int Inside;   // ����
            public int Outside;  // ����
            public float Amount;	//�ɽ����
            public float BuyPrice1;	// ��������
            public float BuyPrice2;
            public float BuyPrice3;
            public float BuyPrice4;
            public float BuyPrice5;
            public int BuyVol1;	// ��Ӧ�������۵��������
            public int BuyVol2;
            public int BuyVol3;
            public int BuyVol4;
            public int BuyVol5;
            public float SellPrice1;	// ���������
            public float SellPrice2;
            public float SellPrice3;
            public float SellPrice4;
            public float SellPrice5;
            public int SellVol1;	// ��Ӧ��������۵��������
            public int SellVol2;
            public int SellVol3;
            public int SellVol4;
            public int SellVol5;

            public float ltgb; // ��ͨ�ɱ�
            public float zgb;  // �ܹɱ�
            public float mgsy; // ÿ������
            public float mgjzc;    // ÿ�ɾ��ʲ�
            public float mggjj;    //ÿ�ɹ�����
            public float mgwfp;    //ÿ��δ����

            public float PriceFs;			 //��ʱ��
            public float PriceAve;			 //���߼�
            public float PriceZt;   //��ͣ
            public float PriceDt;   //��ͣ
            public int SumVol;				 //�ܳɽ���
            public int SumAmount;			 //�ܳɽ����
            public int SumVol_hq0;				 //�ܳɽ����߶�̬����
            public int SumAmount_hq0;			 //�ܳɽ����߶�̬����

            public int bs;  //����
            public int zbs; //�ܱ���

            public float hsl;				 //�������ͨ�̵Ļ�����
            public float zmr5dltp;   //������5��ռ��ͨ�̱���
            public float zmc5dltp;   //������5��ռ��ͨ�̱���

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
                    #region �����STKINFO60.DAT//�����ƴ�������룬�п���ƴ����дδ���̣������ɴ��ǻ�������м��㡣//OK
                    case DataTypes.dm:
                        fileName = "STKINFO60.DAT";
                        //startAddress = 0x845898;
                        startAddress = 0x6d0226;
                        //startAddress = 0x68A8A6;
                        blockSize = 0;
                        //recordSize = 248;//ԭ������
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"jc,���,string,32,1,10,;" +
"py,ƴ��,string,10,2,42,";
                        break;
                    #endregion
                    #region �ֺ�����STKINFO60.DAT//��Ȩ����//OK
                    case DataTypes.cq:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x44aa;
                        blockSize = 2227;
                        recordSize = 20;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,����,date,4,0,0,;" +
"sgbl,�͹ɱ���,single,4,1,4,;" +
"pgbl,��ɱ���,single,4,2,8,;" +
"pgjg,��ɼ۸�,single,4,3,12,;" +
"fh,�ֺ�,single,4,4,16,";
                        break;
                    #endregion
                    #region �������ݣ��򵥣�STKINFO60.DAT//OK
                    case DataTypes.cw0:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x4c2a;
                        blockSize = 2227;
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,������,date,4,0,4,;" +
"gxrq,��������,date,4,0,0,;" +
"ssrq,��������,date,4,0,8,;" +
"col1,ÿ������,single,4,0,12,;" +
"col2,ÿ�ɾ��ʲ�,single,4,0,16,;" +
"col3,���ʲ�������,single,4,0,20,;" +
"col4,ÿ�ɾ�Ӫ�ֽ�,single,4,0,24,;" +
"col5,ÿ�ɹ�����,single,4,0,28,;" +
"col6,ÿ��δ����,single,4,0,32,;" +
"col7,�ɶ�Ȩ���,single,4,0,36,;" +
"col8,������ͬ��,single,4,0,40,;" +
"col9,��Ӫ����ͬ��,single,4,0,44,;" +
"col10,����ë����,single,4,0,48,;" +
"col11,����ÿ�ɾ��ʲ�,single,4,0,52,;" +
"col12,���ʲ�,single,4,0,56,;" +
"col13,�����ʲ�,single,4,0,60,;" +
"col14,�̶��ʲ�,single,4,0,64,;" +
"col15,�����ʲ�,single,4,0,68,;" +
"col16,������ծ,single,4,0,72,;" +
"col17,���ڸ�ծ,single,4,0,76,;" +
"col18,�ܸ�ծ,single,4,0,80,;" +
"col19,�ɶ�Ȩ��,single,4,0,84,;" +
"col20,�ʱ�������,single,4,0,88,;" +
"col21,��Ӫ�ֽ�����,single,4,0,92,;" +
"col22,Ͷ���ֽ�����,single,4,0,96,;" +
"col23,�����ֽ�����,single,4,0,100,;" +
"col24,�ֽ����Ӷ�,single,4,0,104,;" +
"col25,��Ӫ����,single,4,0,108,;" +
"col26,��Ӫ����,single,4,0,112,;" +
"col27,Ӫҵ����,single,4,0,116,;" +
"col28,Ͷ������,single,4,0,120,;" +
"col29,Ӫҵ����֧,single,4,0,124,;" +
"col30,�����ܶ�,single,4,0,128,;" +
"col31,������,single,4,0,132,;" +
"col32,δ��������,single,4,0,136,;" +
"col33,�ܹɱ�,single,4,0,140,;" +
"col34,�����۹ɺϼ�,single,4,0,144,;" +
"col35,A��,single,4,0,148,;" +
"col36,B��,single,4,0,152,;" +
"col37,�������й�,single,4,0,156,;" +
"col38,������ͨ��,single,4,0,160,;" +
"col39,���۹ɺϼ�,single,4,0,164,;" +
"col40,���ҳֹ�,single,4,0,168,;" +
"col41,���з��˹�,single,4,0,172,;" +
"col42,���ڷ��˹�,single,4,0,176,;" +
"col43,������Ȼ�˹�,single,4,0,180,;" +
"col44,���������˹�,single,4,0,184,;" +
"col45,ļ�����˹�,single,4,0,188,;" +
"col46,���ⷨ�˹�,single,4,0,192,;" +
"col47,������Ȼ�˹�,single,4,0,196,;" +
"col48,���ȹɻ�����,single,4,0,200,";     

                         break;
                    #endregion
                    #region ��������STKINFO60.DAT//OK
                    case DataTypes.hq0:
                    case DataTypes.hq0_ag:
                        fileName = "STKINFO60.DAT";
                        startAddress = 0x6D0226;
                        blockSize = 0;
                        recordSize = 273;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"jc,���,string,32,1,10,;" +
"rq,����ʱ��,datetime,4,5,60,;" +
"zs,����,single,4,7,68,;" +
"kp,��,single,4,8,72,;" +
"zg,���,single,4,9,76,;" +
"zd,���,single,4,10,80,;" +
"sp,����,single,4,11,84,;" +
"sl,������,single,4,12,88,;" +
"je,���,single,4,13,92,;" +
"xss,������,single,4,14,96,;" +
"ztj,��ͣ��,single,4,27,184,;" +
"dtj,��ͣ��,single,4,28,188,;" +
"np,����,single,4,27,192,;" +
"wp,����,single,4,28,196,;" +
"mrjg1,��һ��,single,4,15,100,;" +
"mrsl1,��һ��,single,4,18,120,;" +
"mrjg2,�����,single,4,16,104,;" +
"mrsl2,�����,single,4,19,124,;" +
"mrjg3,������,single,4,17,108,;" +
"mrsl3,������,single,4,20,128,;" +
"mrjg4,���ļ�,single,4,32,112,;" +
"mrsl4,������,single,4,34,132,;" +
"mrjg5,�����,single,4,33,116,;" +
"mrsl5,������,single,4,35,136,;" +
"mcjg1,��һ��,single,4,21,140,;" +
"mcsl1,��һ��,single,4,24,160,;" +
"mcjg2,������,single,4,22,144,;" +
"mcsl2,������,single,4,25,164,;" +
"mcjg3,������,single,4,23,148,;" +
"mcsl3,������,single,4,26,168,;" +
"mcjg4,���ļ�,single,4,36,152,;" +
"mcsl4,������,single,4,38,172,;" +
"mcjg5,�����,single,4,37,156,;" +
"mcsl5,������,single,4,39,176,";
                        //"jd,����,int,4,3,52,;" +
                        //"scbz,ɾ����־,int,4,4,56,";
                        //"unknown,(δ֪),int,4,31,164,;" +
                        //",(δ֪),,48,40,200,;"

                              
                        break;
                    #endregion
                    #region �ֱʳɽ������ļ�report.dat���ṹͬday.dat��������һЩ���ݲ���ֱ�ӱ��棩//OK
                    case DataTypes.hqmb:
                        fileName = "REPORT.DAT";
                        //fileName = "20080926.PRP";
                        startAddress = 0x41000;
                        blockSize = 12272;//52*236=12272
                        recordSize = 52;
                        codeIsLong = false;
                        isIndexDataStruct = false;//����ȫ��ͬ��day.dat�ṹ����˵�������
                        fieldString =
                        "dm,����,code,10,0,0,;" +
                        "rq,����,datetime,4,0,0,;" +
                        "zjcj,����ɽ���,single,4,1,4,;" +
                        "zss,������,single,4,2,8,calc;" +
                        "je,���,single,4,3,12,;" +
                        "xss,������,single,4,2,8,;" +
                        "mm,������,string,2,16,21,;" +
                        "mr1jg,��һ��,single,1,10,42,;" +
                        "mr1sl,��һ��,single,2,4,22,;" +
                        "mr2jg,�����,single,1,11,43,;" +
                        "mr2sl,�����,single,2,5,24,;" +
                        "mr3jg,������,single,1,12,44,;" +
                        "mr3sl,������,single,2,6,26,;" +
                        "mr4jg,���ļ�,single,1,12,45,;" +
                        "mr4sl,������,single,2,6,28,;" +
                        "mr5jg,�����,single,1,12,46,;" +
                        "mr5sl,������,single,2,6,30,;" +
                        "mc1jg,��һ��,single,1,13,47,;" +
                        "mc1sl,��һ��,single,2,7,32,;" +
                        "mc2jg,������,single,1,14,48,;" +
                        "mc2sl,������,single,2,8,34,;" +
                        "mc3jg,������,single,1,15,49,;" +
                        "mc3sl,������,single,2,9,36,;" +
                        "mc4jg,���ļ�,single,1,14,50,;" +
                        "mc4sl,������,single,2,8,38,;" +
                        "mc5jg,�����,single,1,14,51,;" +
                        "mc5sl,������,single,2,8,40,;" +
                        "bs,�ܱ���,int,2,0,16,"
                        ;
                        //�����������Ͳ��Ǵ洢���ͣ������в�ֱ����ʵ���������ͣ���/��X��Ϊshort����/��X��Ϊbyte
                        //������ͨ����������������ã�Ӧ�÷�������������
                       
                        break;
                    #endregion
                    #region ���������ļ�day.dat//OK
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
                    #region 1���������ļ�min1.dat//OK
                    case DataTypes.hq1:
                        fileName = "MIN1.DAT";
                        startAddress = 0x41000;
                        blockSize = 16384;//���СΪ��16384/32��512��ԭ��������8192
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
                    #region 5���������ļ�min.dat//OK
                    case DataTypes.hq5:
                        fileName = "MIN.DAT";
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
                    #region �ɱ��ṹSTKCapital.fdt
                    case DataTypes.gb:
                        fileName = "STKCAPITAL.FDT";
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
                    #region ��������STKFinance.fdt
                    case DataTypes.cw:
                        fileName = "STKFINANCE.FDT";
                        startAddress = 0x41000;
                        blockSize = 14848;
                        recordSize = 464;
                        codeIsLong = true;
                        fieldString =
"dm,����,code,12,0,0,;" +
"rq,����,date,4,,460,;" +
"bsdqtzje,����Ͷ�ʾ���,double,8,1,12,;" +
"bsyszkje,Ӧ���ʿ��,double,8,2,20,;" +
"bschje,�������,double,8,3,28,;" +
"bsldzc,�����ʲ�,double,8,4,36,;" +
"bscqtzje,����Ͷ�ʾ���,double,8,5,44,;" +
"bsgdzc,�̶��ʲ�,double,8,6,52,;" +
"bswxzc,���μ������ʲ�,double,8,7,60,;" +
"bszzc,���ʲ�,double,8,8,68,;" +
"bsdqjk,���ڽ��,double,8,9,76,;" +
"bsyfzk,Ӧ���ʿ�,double,8,10,84,;" +
"bsldfz,������ծ,double,8,11,92,;" +
"bscqfz,���ڸ�ծ,double,8,12,100,;" +
"bsfz,��ծ�ϼ�,double,8,13,108,;" +
"bsgb,�ɱ�,double,8,14,116,;" +
"bsssgdqy,�����ɶ�Ȩ��,double,8,15,124,;" +
"bsgdqy,�ɶ�Ȩ��,double,8,16,132,;" +
"bszbgj,�ʱ�����,double,8,17,140,;" +
"bsyygj,ӯ�๫��,double,8,18,148,;" +
"iszysr,��Ӫҵ�����뾻��,double,8,1,156,;" +
"iszycb,��Ӫҵ��ɱ�,double,8,2,164,;" +
"iszylr,��Ӫҵ������,double,8,3,172,;" +
"isqtlr,����ҵ������,double,8,4,180,;" +
"isyyfy,Ӫҵ����,double,8,5,188,;" +
"isglfy,�������,double,8,6,196,;" +
"iscwfy,�������,double,8,7,204,;" +
"istzsy,Ͷ������,double,8,8,212,;" +
"islrze,�����ܶ�,double,8,9,220,;" +
"issds,����˰,double,8,10,228,;" +
"isjlr,������,double,8,11,236,;" +
"iskchjlr,�۳������������ľ�����,double,8,12,244,;" +
"iswfplr,δ��������,double,8,13,252,;" +
"cfjyhdxjlr,��Ӫ��ֽ�����,double,8,1,260,;" +
"cfjyhdxjlc,��Ӫ��ֽ�����,double,8,2,268,;" +
"cfjyhdxjje,��Ӫ��ֽ𾻶�,double,8,3,276,;" +
"cftzxjlr,Ͷ���ֽ�����,double,8,4,284,;" +
"cftzxjlc,Ͷ���ֽ�����,double,8,5,292,;" +
"cftzxjje,Ͷ���ֽ𾻶�,double,8,6,300,;" +
"cfczxjlr,����ֽ�����,double,8,7,308,;" +
"cfczxjlc,����ֽ�����,double,8,8,316,;" +
"cfczxjje,����ֽ𾻶�,double,8,9,324,;" +
"cfxjjze,�ֽ��ֽ�ȼ��ﾻ����,double,8,10,332,;" +
"cfxsspxj,������Ʒ�յ����ֽ�,double,8,11,340,;" +
"mgsy,ÿ������,single,4,1,348,;" +
"mgjzc,ÿ�ɾ��ʲ�,single,4,2,352,;" +
"tzmgjzc,������ÿ�ɾ��ʲ�,single,4,3,356,;" +
"mgzbgjj,ÿ���ʱ�������,single,4,4,360,;" +
"mgwfplr,ÿ��δ��������,single,4,5,364,;" +
"mgjyxjllje,ÿ�ɾ�Ӫ��������ֽ���������,single,4,6,368,;" +
"mgxjzjje,ÿ���ֽ��ֽ�ȼ������Ӿ���,single,4,7,372,;" +
"mll,ë����,single,4,8,376,;" +
"zyywlrl,��Ӫҵ��������,single,4,9,380,;" +
"jll,������,single,4,10,384,;" +
"zzcbcl,���ʲ�������,single,4,11,388,;" +
"jzcsyl,���ʲ�������,single,4,12,392,;" +
"xsxjzb,������Ʒ�յ����ֽ�ռ��Ӫ�������,single,4,13,396,;" +
"yszczzl,Ӧ���ʿ���ת��,single,4,14,400,;" +
"chzzl,�����ת��,single,4,15,404,;" +
"gdzczzl,�̶��ʲ���ת��,single,4,16,408,;" +
"zyywzzl,��Ӫҵ��������,single,4,17,412,;" +
"jlrzzl,������������,single,4,18,416,;" +
"zzczzl,���ʲ�������,single,4,19,420,;" +
"jzczzl,���ʲ�������,single,4,20,424,;" +
"ldbl,��������,single,4,21,428,;" +
"sdbl,�ٶ�����,single,4,22,432,;" +
"zcfzbl,�ʲ���ծ����,single,4,23,436,;" +
"fzbl,��ծ����,single,4,24,440,;" +
"gdqybl,�ɶ�Ȩ�����,single,4,25,444,;" +
"gdzcbl,�̶��ʲ�����,single,4,26,448,;" +
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
                    case DataTypes.jjjz:
                        //fileName = "FUNDWEEK.FDT";
                        fileName = "FUNDINFO.fdt";
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
                        fileName = "FUNDINVEST.fdt";
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
                    #region ���userdata\block
                    case DataTypes.bk:
                        fileName = "BLOCK.DEF";
                        startAddress = 0;
                        blockSize = 0;
                        recordSize = 248;
                        codeIsLong = false;
                        isIndexDataStruct = false;
                        fieldString =
"lb,���,string,20,0,0,;" +
"bk,���,string,20,1,10,;" +
"dm,֤ȯ����,string,10,2,42,";
                        break;
                     #endregion
                    #region ����
                    case DataTypes.pj:
                        fileName = "����.str";
                        //fileName = "SIMU.DAT";
                        startAddress = 0;
                        blockSize = 256;
                        recordSize = 256;
                        codeIsLong = true;
                        isIndexDataStruct = false;
                        fieldString =
"dm,֤ȯ����,string,12,0,0,;" +
"pj,����,string,2,2,0,;" +
"sm,˵��,string,244,2,0,";
                        break;
                    #endregion
                    #region ��Ȩ���飬�������//OK
                    case DataTypes.hqfq:
                        fileName = "DAY.DAT";
                        startAddress = 0x41000;
                        blockSize = 8192;
                        recordSize = 32;
                        codeIsLong = false;
                        fieldString =
"dm,����,code,10,0,0,;" +
"rq,����,date,4,1,0,;" +
"kp,���̸�Ȩ��,single,4,2,4,B;" +
"zg,��߸�Ȩ��,single,4,3,8,B;" +
"zd,��͸�Ȩ��,single,4,4,12,B;" +
"sp,���̸�Ȩ��,single,4,5,16,B;" +
"sl,��Ȩ�ɽ�����,single,4,6,20,A;" +
"je,�ɽ����,single,4,7,24,;"+
"spsyl,����������,single,4,0,0,";
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
        public string FxjPath   //����FxjPath
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
        public string FxjDataPath   //����FxjDataPath
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
                        result += fxjFileStruct.fields[i, 0];//�ֶ�
                        if ("  ,code,string".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " char(" + fxjFileStruct.fields[i, 3] + ") format=$" + fxjFileStruct.fields[i, 3] + "."; //�ַ���
                        }
                        else if ("  ,int,single,double".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num "; //��ֵ����
                        }
                        else if ("  ,date".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num format=YYMMDD10."; //date����
                        }
                        else if ("  ,datetime".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " num format=datetime."; //datetime����
                        }
                        result += " label='" + fxjFileStruct.fields[i, 1] + "'";//��ǩ

                    }
                    result = "create table FinData." + dataType + "(" + result + ");";
                    if (delOldTable == true)
                    {
                        result = "drop table FinData." + dataType + ";" + result;
                    }
                    result = "proc sql;" + result + "quit;";
                    break;
                case "SASINPUT"://����SASֱ�Ӷ�ȡ����ʱ���õ�INPUT��䣬���һ���޸�
                    for (int i = 0; i < fxjFileStruct.fields.GetLength(0); i++)
                    {
                        if ("  ,code,string".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " $" + fxjFileStruct.fields[i, 3] + "."; //�ַ���
                        }
                        else if ("  ,int,date,datetime".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " ib" + fxjFileStruct.fields[i, 3] + "."; //��ֵ����
                        }
                        else if ("  ,single".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " float" + fxjFileStruct.fields[i, 3] + "."; //��ֵ����
                        }
                        else if ("  ,double".IndexOf(fxjFileStruct.fields[i, 2]) > 0)
                        {
                            result += " @(p+" + fxjFileStruct.fields[i, 5] + ") " + fxjFileStruct.fields[i, 0] + " rb" + fxjFileStruct.fields[i, 3] + "."; //��ֵ����
                        }
                    }
                    break;
                case "FIELDS"://�г��ֶ�����
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
        private string [] GetCodes(string Market)   //��ȡDay.dat�еĴ���
        {
            //����ָ������ת����,������ͬʱ���滦���������
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
                this.checkFileStream(FxjFile);
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
                msg = @"����Ĳ������󡣲���ֻ����:";
                foreach (string s in Enum.GetNames(typeof(DataTypes)))
                    msg += " \"" + s + "\"";
                msg += @" ���� ";
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

        public string[,] GetData(string dataType, string code,int iRecordCount)
        {
            return GetData(dataType, code, "", iRecordCount);
        }
        public string[,] GetData(string dataType, string code, string newFileName, int iRecordCount) //newFileName ��ʷ��ʱ����
        {
            try
            {
                DataTypes d = (DataTypes)Enum.Parse(typeof(DataTypes), dataType.ToLower());

                if (d == DataTypes.hq0_ag ) return GetAG0(iRecordCount);
                
                return GetData(d, code, newFileName, iRecordCount);
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
        private string[,] GetData(DataTypes dataType,string code,string newFileName,int iRecordCount) //��ȡ���ݣ�����
        {
            if (dataType == DataTypes.bk) return GetBK(code);
            if (dataType == DataTypes.pj) return GetPJ(code);
            if (dataType == DataTypes.hqfq) return GetHqfq(dataType,code,newFileName,iRecordCount );
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
            System.Globalization.CultureInfo cnCultureInfo = new System.Globalization.CultureInfo("zh-CN");
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
            FxjFile = FxjFile.ToUpper();
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
                #region ����DAY.DAT�Ƚṹ������/���ݣ������ݣ�Ŀǰ������ʱ���ݡ�1�������ݡ�5�������ݡ�������...
                try
                {
                    this.checkFileStream(FxjFile);
                    int secCounts = 0;//�ļ���֤ȯ����
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
                                codeRead = true;
                            }
                        }
                    }
                    int iRecord = 1;//��¼
                    int iBlock = 0;//��iBlock��
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
                            //pos = fxjFileStruct.startAddress + r * fxjFileStruct.recordSize;//test�������Ʋⵥ��blocks���ݿ�Ĵ�С
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
                                    case "datetime":

                                        ////tempAddress1 = tempAddress; //lps
                                        ////tempAddress = fs.Position; //lps

                                        ////if (iRecord > 1) //lps
                                        ////    tempResult = tempAddress - tempAddress1;  //lps

                                        ////intField = br.ReadInt32();
                                        ////if (intField >= 14148.625 * 86400)//test
                                        ////    Console.WriteLine(tempAddress.ToString());

                                        ////record[iField] = (intField == 0 ? "" : (date19700101.AddSeconds(intField)).ToString("yyyy-MM-dd HH:mm:ss"));
                                        ////record[iField] = record[iField] + "|��ַ��" + tempAddress.ToString() + "(" + tempResult.ToString() + ")";  //test


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

                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
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
                        #region ���������STKINFO60.DAT�Ƚṹ�����ݣ�
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


                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.hq0:
                        #region �������飨����STKINFO60.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cq:
                        #region �ֺ����䣨����STKINFO60.DAT�Ƚṹ�����ݣ�//��Ȩ
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.cw0:
                        #region ��������--�򵥣�����STKINFO60.DAT�Ƚṹ�����ݣ�
                        try
                        {
                            this.checkFileStream(FxjFile);
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
                            if (records.GetLength(0) == 0) msg = "û�ж�������!";
                            return records;
                        }
                        catch (Exception e)
                        {
                            msg = e.Message;
                        }
                        #endregion
                        break;
                    case DataTypes.hqmb:
                        #region ����Report.DAT���ݣ��ṹ����DAY.DAT������Щ��ֵ��Ҫ��һ�����������
                        try
                        {
                            this.checkFileStream(FxjFile);
                            int secCounts = 0;//�ļ���֤ȯ����
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
                                        codeRead = true;
                                    }
                                }
                            }
                            int iRecord = 1;//��¼
                            int iBlock = 0;//��iBlock��
                            int fieldCounts = fxjFileStruct.fields.GetLength(0);
                            while (iBlock < 25 && blocks[iBlock] != -1)
                            {
                                int r = 0;
                                //long tempAddress = 0;
                                //long tempAddress1 = 0;
                                //long tempResult = 0;
                                //int tempPos = 0;
                                UInt16 curValue_bs, preValue_bs = 0;
                                while (iRecord < recordCounts + 1 && r < fxjFileStruct.blockSize / fxjFileStruct.recordSize)   //12272/52=236����¼
                                {

                                    string[] record = new string[fieldCounts];
                                    pos = fxjFileStruct.startAddress + blocks[iBlock] * fxjFileStruct.blockSize + r * fxjFileStruct.recordSize;

                                    #region �������ݽṹ
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
                                        record[4] = "��ַ��" + floatField.ToString() + "(" + tempResult.ToString() + ")"; 

                                    //}
                                */
                                    #endregion

                                    for (int iField = 0; iField < fieldCounts; iField++)
                                    {
                                        fs.Position = pos + Convert.ToInt64(fxjFileStruct.fields[iField, 5]);
                                        switch (fxjFileStruct.fields[iField, 0].ToLower()) //�������ȡDAY.DAT�÷���ͬ���жϵ��Ǵ������������
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
                                                ////record[iField] = record[iField] + "|��ַ��" + tempAddress.ToString() + "(" + tempResult.ToString() + ")";  //test                                                
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
                                                record[iField] = "";//���������������
                                                break;
                                            case "mm":
                                                int mm = br.ReadSByte();
                                                record[iField] = "";
                                                if (mm == -128) record[iField] = "����"; //-128 = 0x80
                                                if (mm == -64) record[iField] = "����";  //-64 = 0xC0
                                                break;
                                            case "bs":                                                
                                                curValue_bs = br.ReadUInt16();
                                                record[iField] = (curValue_bs - preValue_bs).ToString("D");//��ǰ��������ǰ�ܱ�������һ�α���
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
                                    if (j == 5)  //������
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

                            if (records.GetLength(0) == 0) msg = "û�ж�������!";

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
        private string[,] GetBK(string code)//��鶨������
        {
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(DataTypes.bk);
            if (code == null) code = "";
            code = code.Trim().ToUpper();
            ArrayList recordList = new ArrayList();
            if (this.FxjDataPath == "")
            {
                msg = @"�޷���ע����е������������ļ�Ŀ¼�������н����� FxjDataPath����Ϊ��Ч·������c:\fxj\data\��";
                return new string[1, 1] { { null } };
            }
            string FxjBlockPath = fxjDataPath;
            FxjBlockPath = FxjBlockPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\BLOCK\\") ; //����Ŀ¼�к���data����
            string FxjFile = FxjBlockPath + fxjFileStruct.fileName;
            
            msg = "";
            if (!File.Exists(FxjFile))
            {
                msg = "����ļ��޷��ҵ���";
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
                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
                    return records;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new string[1, 1] { { null } };

        }
        private string[,] GetPJ(string code)//��������
        {
            msg = "";
            fileStruct fxjFileStruct = new fileStruct(DataTypes.pj);
            code = code.Trim().ToUpper();
            ArrayList recordList = new ArrayList();
            if (this.FxjDataPath == "")
            {
                msg = @"�޷���ע����е������������ļ�Ŀ¼�������н����� FxjDataPath����Ϊ��Ч·������c:\fxj\data\��";
                return new string[1, 1] { { null } };
            }
            string fxjSubPath = fxjDataPath;
            fxjSubPath = fxjSubPath.ToUpper().Replace("\\DATA\\", "\\USERDATA\\SelfData\\"); //����Ŀ¼�к���data����
            string FxjFile = fxjSubPath + fxjFileStruct.fileName;

            msg = "";
            if (!File.Exists(FxjFile))
            {
                msg = fxjFileStruct.fileName +"�޷��ҵ���";
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
                    if (records.GetLength(0) == 0) msg = "û�ж�������!";
                    return records;
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return new string[1, 1] { { null } };

        }
        private string[,] GetAG0(int iRecordCount)//A�ɹ�Ʊ�Ķ�̬����
        {
            string[,] records_sh = GetData("hq0", "sh", 0);
            string[,] records_sz = GetData("hq0", "sz", 0);
            int iHJSL = records_sh.GetLength(0) + records_sz.GetLength(0);//�ϼ�����
            int iField = records_sh.GetLength(1); //�ֶ����� 
            if (iRecordCount == 0 || iRecordCount > iHJSL) iRecordCount = iHJSL;
            string[,] records = new string[iRecordCount, iField];
            
            int iSHSL=records_sh .GetLength(0);//�Ϻ���¼����
            int iSZSL = records_sz.GetLength(0);//���ڼ�¼����



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
            if (records.GetLength(0) == 0) msg = "û�ж�������!";

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
            int iZQSL = records_dm.GetLength(0);//֤ȯ����
            for (int i = 0; i < iZQSL; i++)
            {
                
                switch (records_dm[i,0].Substring(0,4).ToLower())
                {
                case "sh60":
                        stkInfo AstkInfo = new stkInfo();
                        string[,] records_mb = GetData("hqmb", records_dm[i, 0], 0);
                        string[,] records_hq0 = GetData("hq0", records_dm[i, 0], 0);
                        int irecords_mb = records_mb.GetLength(0);

                        string[,] records_min1 = new string[241, 60];//�����߼�¼����

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
        private string[,] GetHqfq(DataTypes dataType, string code, string newFileName, int iRecordCount)//��Ȩ�۸�,�ֺ���Ͷ��,��ǰ��Ȩ��
        {
            FxjData fxj = new FxjData();
            string[,] hq = fxj.GetData("hq", code, newFileName, iRecordCount);
            if (fxj.Error != 0 || hq.GetLength(1)<4 ) return new string[1, 1] { { null } };
            string[,] x = new string[hq.GetLength(0),9];
            string[,] cq = fxj.GetData("cq", code, newFileName, iRecordCount);
            string fmt = "_jj_qz".IndexOf(this.GetCodeType(code)) > 0 ? "F3" : "F";
            if (fxj.Error != 0 || cq.GetLength(1) < 4 || cq.GetLength(0)==0) //û�г�Ȩ��Ϣ
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
            else  //�г�Ȩ��Ϣ
            {
                DateTime[] cqdt = new DateTime[cq.GetLength(0)];
                for (int j = 0; j < cq.GetLength(0); j++) cqdt[j] = new DateTime(int.Parse(cq[j, 1].Split('-')[0]), int.Parse(cq[j, 1].Split('-')[1]), int.Parse(cq[j, 1].Split('-')[2]));
                int i0 = hq.GetLength(0) - 1;
                DateTime hqdt_1,hqdt;
                double kp_1,zg_1,zd_1,sp_1,kp,zg,zd,sp,kpx,zgx,zdx,spx,sgbl,kpsyl,zgsyl,zdsyl,spsyl, pgbl, pgjg, fh;
                for (int k = 0; k < 8; k++) x[i0, k] = hq[i0, k];  //���һ����¼
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
                    //syl=1+��t�������� =( t�����̼�*(1+�͹ɱ���+��ɱ���)+�ֺ���-��ɼ۸�*��ɱ���)/(t-1�����̼�)
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
                    x[i - 1, 6] = hq[i - 1, 6];//sl �ɽ���δ��Ȩ
                    x[i - 1, 7] = hq[i - 1, 7];//je
                    x[i, 8] = (spsyl - 1).ToString("0.00000");//spsyl ���̼�������
                    
                }

            }

            return x;

        }
    }
}
