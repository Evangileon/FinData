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
                MessageBox.Show("��ѡ����������!", "��Ϣ");
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
                    MessageBox.Show("û�����ݻ�������" + fxj.Msg);
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
                    codeDesc.Text = "���������г����룬����ΪSH������ΪSZ�ȡ���";
                    code.Text = "SH";
                    newFileNameDesc.Text = "��һ��Ϊ�գ�";
                    newFileName.Text = "";
                    break;
                case "hq0":
                    codeDesc.Text = "��������г�+֤ȯ������ɣ���SZ000001��SH000001��SH600000�ȣ��������г������򷵻������г��Ķ�̬���飬�磬����ΪSH������ΪSZ�ȣ�";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "��һ��Ϊ�գ�";
                    newFileName.Text = ""; 
                    break;
               case "hq0_ag":
                    codeDesc.Text = "���������Ϻ�������A���г���̬���顣��";
                    code.Text = "";
                    newFileNameDesc.Text = "��һ��Ϊ�գ�";
                    newFileName.Text = "";
                    break;
                case "hqmb":
                    codeDesc.Text = "���������г����룬����ΪSH������ΪSZ�ȡ���";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "����Ҫ�����·ֱʳɽ�������д����Ҫ����ʷ�ֱʳɽ����������ļ�����\n�硰20060324.PRP����ʾ2006��3��24�յķֱʳɽ���";
                    newFileName.Text = ""; 
                    break;
                case "bk":
                    codeDesc.Text = "������ȡ���а������գ�����ȡĳ����������������ƣ��硰A�ɰ�顱�ȡ���";
                    code.Text = "";
                    newFileNameDesc.Text = "���գ�δʹ�á���";
                    newFileName.Text = ""; 
                    break;
                case "pj":
                    codeDesc.Text = "������ȡ������������գ�����ȡĳֻ֤ȯ������������֤ȯ���룬�硰SZ000001���ȡ���";
                    code.Text = "";
                    newFileNameDesc.Text = "���գ�δʹ�á���";
                    newFileName.Text = ""; 
                    break;
                default:
                    codeDesc.Text = "��������г�+֤ȯ������ɣ���SZ000001��SH000001��SH600000�ȣ�";
                    code.Text = "SH600000";
                    newFileNameDesc.Text = "��һ��Ϊ�գ�";
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
                MessageBox.Show("��ѡ����������!","��Ϣ");
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
                    dataGridView.Columns[0].HeaderText ="�ֶ�����";
                    dataGridView.Columns[1].HeaderText = "�ֶ�˵��";
                    dataGridView.Columns[2].HeaderText = "�ֶ�����";
                    methodText.Text = callText;
                }
                else
                {
                    methodText.Text = "";
                    MessageBox.Show("û�����ݻ������� " + fxj.Msg);
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
                MessageBox.Show("û�����ݣ�", "����", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.AddExtension = true;
            fileDialog.CheckFileExists = false;
            fileDialog.CheckPathExists = true;
            fileDialog.FileName = methodText.Text.ToLower().IndexOf("getfields") > 0 ? dataType.Text.Split('-')[0] + "_fields" : dataType.Text.Split('-')[0] + "_" + code.Text.Trim() + (newFileName.Text.ToLower().IndexOf(".prp") > 0 ? "_" + newFileName.Text.Substring(0,6): "");
            fileDialog.DefaultExt = "txt";
            fileDialog.Filter = "�ı�(*.txt)|*.txt";
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