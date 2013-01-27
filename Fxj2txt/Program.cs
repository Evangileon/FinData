using System;
using System.Collections.Generic;
using System.Text;

namespace Fxj2txt
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) return;
            Console.Title = "正在读取数据，请不要手工关闭...";
            string sepChar = ",";
            if (args[0].ToLower() == "getdata" && args.Length>=3)
            {
                string dataType,code,newFileName;
                dataType=args[1];
                code = args[2];
                newFileName = "";
                if (args.Length == 4) newFileName = args[3];
                FinData.FxjData fxj = new FinData.FxjData();
                string[,] x = fxj.GetData(dataType, code, newFileName,0);
                if (fxj.Error == 0)
                {
                    for (int ii = 0; ii < x.GetLength(0); ii++)
                    {
                        for (int jj = 0; jj < x.GetLength(1); jj++)
                        {
                            Console.Write(x[ii, jj]);
                            if (jj < x.GetLength(1) - 1) Console.Write(sepChar);
                        }
                        Console.WriteLine();
                    }
                }
            }
            else if (args[0].ToLower() == "getfields")
            {
                string dataType;
                dataType = args[1];
                FinData.FxjData fxj = new FinData.FxjData();
                string[,] x = fxj.GetFields(dataType);
                if (fxj.Error == 0)
                {
                    for (int ii = 0; ii < x.GetLength(0); ii++)
                    {
                        for (int jj = 0; jj < x.GetLength(1); jj++)
                        {
                            Console.Write(x[ii, jj]);
                            if (jj < x.GetLength(1) - 1) Console.Write(sepChar);
                        }
                        Console.WriteLine();
                    }
                }
            }
            else if (args[0].ToLower() == "gettabledef")
            {

                if (args.Length >= 3)
                {
                    string dataType, descDataType;
                    dataType = args[1];
                    descDataType = args[2];
                    FinData.FxjData fxj = new FinData.FxjData();
                    if (fxj.Error == 0)
                    {
                        string x = fxj.GetTableDef(dataType, descDataType, false);
                        Console.WriteLine(x);
                    }
                }
            }
            else if (args[0].ToLower() == "gettables")
            {
                FinData.FxjData fxj = new FinData.FxjData();
                string[,] x = fxj.GetTables();
                if (fxj.Error == 0)
                {
                    for (int ii = 0; ii < x.GetLength(0); ii++)
                    {
                        for (int jj = 0; jj < x.GetLength(1); jj++)
                        {
                            Console.Write(x[ii, jj]);
                            if (jj < x.GetLength(1) - 1) Console.Write(sepChar);
                        }
                        Console.WriteLine();
                    }
                }
            }
            else if (args[0].ToLower() == "getmarkets")
            {
                FinData.FxjData fxj = new FinData.FxjData();
                string[,] x = fxj.GetMarkets();
                if (fxj.Error == 0)
                {
                    for (int ii = 0; ii < x.GetLength(0); ii++)
                    {
                        for (int jj = 0; jj < x.GetLength(1); jj++)
                        {
                            Console.Write(x[ii, jj]);
                            if (jj < x.GetLength(1) - 1) Console.Write(sepChar);
                        }
                        Console.WriteLine();
                    }
                }
            }
            else if (args[0].ToLower() == "getcodetype")
            {
                if (args.Length >= 2)
                {
                    string code;
                    code = args[1];
                    FinData.FxjData fxj = new FinData.FxjData();
                    if (fxj.Error == 0)
                    {
                        string x = fxj.GetCodeType(code);
                        Console.WriteLine(x);
                    }
                }
            }


        }
    }
}
