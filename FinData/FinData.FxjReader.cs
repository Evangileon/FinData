using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using FinData;
using System.Data.SqlClient;

namespace FinData
{

    internal partial class FxjReader : Form
    {
       //const string ConStr = "server=(local);user id=PRPUser;pwd=61000000;database=PRPStockDataBase";
        public FxjReader()
        {
            InitializeComponent();
        }
        string[,] dataArray;
        private void button1_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new  FinData.FxjData();
            if (dataType.Text.Trim() == "")
            {
                MessageBox.Show("请选择数据类型!", "信息");
                return;
            }
            try
            {
                string callText;
                string dt = dataType.Text.Split('-')[0];
                if (newFileName.Text == "")
                {
                    dataArray = fxj.GetData(dt, code.Text,Convert.ToInt32(cboRecordCount.Text.Trim()));
                    callText = "fxjdata.GetData(\"" + dt + "\",\"" + code.Text.Trim() + "\",\""+cboRecordCount.Text.Trim()+"\")";
                }
                else
                {
                    dataArray = fxj.GetData(dt, code.Text, newFileName.Text,Convert.ToInt32(cboRecordCount.Text.Trim()));
                    callText = "fxjdata.GetData(\"" + dt + "\",\"" + code.Text.Trim() + "\",\"" + newFileName.Text.Trim() + "\",\"" + cboRecordCount.Text.Trim() + "\")";
                }
                //statusStrip1.Text = fxj.Msg;
                if (fxj.Error == 0 && dataArray.GetLength(0) > 0)
                {
                    string[,] colname = fxj.GetFields(dt);
                    dataGridView.DataSource = new ArrayDataView(dataArray);
                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        dataGridView.Rows[i].HeaderCell.Value = (i+1).ToString().Trim();
                    }
                    if (colname.GetLength(0) == dataGridView.Columns.Count)
                    {
                        for (int i = 0; i < colname.GetLength(0); i++)
                        {
                            dataGridView.Columns[i].HeaderText = colname[i, 0] + "(" + colname[i, 1] + ")";
                        }
                    }
                    methodText.Text = callText;
                }
                else
                {
                    methodText.Text = "";
                    MessageBox.Show("没有数据或发生错误。" + fxj.Msg);
                }
            }
            catch (Exception ex)
            {
                methodText.Text = "";
                MessageBox.Show(ex.Message);
            }
            
        }

        private void FxjReader_Load(object sender, EventArgs e)
        {
            FxjData fxj = new FxjData();
            //string[] dataTypeNames = fxj.GetTables("");
            string[,] tableNames = fxj.GetTables();
            for (int i = 0; i < tableNames.GetLength(0); i++)
            {
                dataType.Items.Add(tableNames[i,0]+"-"+tableNames[i,1]);
            }
        }
        private void dataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (dataType.Text.Split('-')[0].ToLower())
            {
                case "dm":
                    codeDesc.Text = "（请输入市场代码，沪市为SH，深市为SZ等。）";
                    code.Text = "SH";
                    newFileNameDesc.Text = "（一般为空）";
                    newFileName.Text = "";
                    break;
                case "hq0":
                    codeDesc.Text = "（必填，由市场+证券代码组成，如SZ000001、SH000001、SH600000等，仅输入市场代码则返回整个市场的动态行情，如，沪市为SH，深市为SZ等）";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "（一般为空）";
                    newFileName.Text = ""; 
                    break;
               case "hq0_ag":
                    codeDesc.Text = "（将返回上海、深圳A股市场动态行情。）";
                    code.Text = "";
                    newFileNameDesc.Text = "（一般为空）";
                    newFileName.Text = "";
                    break;
                case "hqmb":
                    codeDesc.Text = "（请输入市场代码，沪市为SH，深市为SZ等。）";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "（若要读最新分笔成交不必填写；若要读历史分笔成交，请输入文件名，\n如“20060324.PRP”表示2006年3月24日的分笔成交）";
                    newFileName.Text = ""; 
                    break;
                case "bk":
                    codeDesc.Text = "（若读取所有板块请清空；若读取某个板块请输入板块名称，如“A股板块”等。）";
                    code.Text = "";
                    newFileNameDesc.Text = "（空，未使用。）";
                    newFileName.Text = ""; 
                    break;
                case "pj":
                    codeDesc.Text = "（若读取所有评级请清空；若读取某只证券的评级请输入证券代码，如“SZ000001”等。）";
                    code.Text = "";
                    newFileNameDesc.Text = "（空，未使用。）";
                    newFileName.Text = ""; 
                    break;
                default:
                    codeDesc.Text = "（必填，由市场+证券代码组成，如SZ000001、SH000001、SH600000等）";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "（一般为空）";
                    newFileName.Text = ""; 
                    break;                    
            }
            methodText.Text = "";
        }



        private void readFieldNames_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FinData.FxjData();
            //string[,] s;
            if (dataType.Text.Trim() == "")
            {
                MessageBox.Show("请选择数据类型!","信息");
                return;
            }
            try
            {
                string callText;
                string dt = dataType.Text.Split('-')[0];
                dataArray = fxj.GetFields(dt);
                callText = "fxjdata.GetFields(\"" + dt + "\")";

                if (fxj.Error == 0 && dataArray.GetLength(0) > 0)
                {
                    dataGridView.DataSource = new ArrayDataView(dataArray);
                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString().Trim();
                    }
                    dataGridView.Columns[0].HeaderText ="字段名称";
                    dataGridView.Columns[1].HeaderText = "字段说明";
                    dataGridView.Columns[2].HeaderText = "字段类型";
                    methodText.Text = callText;
                }
                else
                {
                    methodText.Text = "";
                    MessageBox.Show("没有数据或发生错误 " + fxj.Msg);
                }
            }
            catch (Exception ex)
            {
                methodText.Text = "";
                MessageBox.Show(ex.Message);
            }
            

        }

        private void url_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com/";
            process.Start();
        }

        private void exportData_Click(object sender, EventArgs e)
        {
            if (dataArray == null)
            {
                MessageBox.Show("没有数据！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.CheckFileExists = false;
            fileDialog.CheckPathExists = true;
            fileDialog.FileName = methodText.Text.ToLower().IndexOf("getfields") > 0 ? dataType.Text.Split('-')[0] + "_fields" : dataType.Text.Split('-')[0] + "_" + code.Text.Trim() + (newFileName.Text.ToLower().IndexOf(".prp") > 0 ? "_" + newFileName.Text.Substring(0,6): "");
            fileDialog.DefaultExt = "txt";
            fileDialog.Filter = "文本(*.txt)|*.txt";
            fileDialog.OverwritePrompt = true;
            fileDialog.RestoreDirectory = true;
            DialogResult result = fileDialog.ShowDialog();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileDialog.FileName);
            if (result == DialogResult.OK)
            {
                try
                {

                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        sw.Write(dataGridView.Columns[i].HeaderText + "  ,  ");
                    }
                    sw.WriteLine();
                    for (int ii = 0; ii < dataArray.GetLength(0); ii++)
                    {
                        string s = "";
                        for (int jj = 0; jj < dataArray.GetLength(1); jj++)
                            s += dataArray[ii, jj] + "  ,  ";
                        sw.WriteLine(s);
                    }
                    //sw.Close();

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FinData.FxjData fxj = new FinData.FxjData();
            //fxj.InsertStockDataToSQLDb_hq("SH",Convert.ToInt32(cboRecordCount.Text.Trim()));            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //FinData.FxjData fxj = new FinData.FxjData();
            //fxj.InsertStockDataToSQLDb_hq("SZ",Convert.ToInt32(cboRecordCount.Text.Trim()));   
        }

        private void btnCurrentDay_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FinData.FxjData();
            fxj.GetStkInfo("sh");
        }
 

    }
}