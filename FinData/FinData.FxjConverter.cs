using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using FinData;
namespace FinData
{
    internal partial class FxjConverter : Form
    {
        public FxjConverter()
        {
            InitializeComponent();
        }

        private void changeFxjPath_Click(object sender, EventArgs e)
        {

        }
        private bool stopConverting=false;
        private void FxjConverter_Load(object sender, EventArgs e)
        {
            FxjData fxj = new FxjData();
            if (fxj.FxjPath == "")
            {
                fxjPath.Text = "û���ҵ������ң�";
                fxjPath.ForeColor = Color.Red;
            }
            else
            {
                this.fxjPath.Text = fxj.FxjPath;
                this.ver.Text = fxj.Version;
                if (fxj.GetMarkets().GetLength(0) > 0)
                {
                    TreeNode[] marketNode = new TreeNode[fxj.GetMarkets().GetLength(0) + 1];
                    treeView1.ExpandAll();
                    for (int i = 0; i < fxj.GetMarkets().GetLength(0); i++)
                    {
                        DirectoryInfo dir = new DirectoryInfo(fxj.FxjPath + @"\data\" + fxj.GetMarkets()[i, 0].Trim());
                        if (dir.Exists)
                        {
                            marketNode[i] = treeView1.Nodes.Add(fxj.GetMarkets()[i, 1]);
                            string[,] tables = fxj.GetTables();
                            for (int j = 0; j < tables.GetLength(0); j++)
                            {
                                if ("..SH,SZ,".IndexOf(fxj.GetMarkets()[i, 0].ToUpper()) > 0 && tables[j, 0].ToLower() != "bk")  //�
                                {
                                    if (File.Exists(fxj.FxjPath + @"data\" + fxj.GetMarkets()[i, 0] + @"\" + tables[j, 2]) ||
                                        ",,,fp,gb,gd,cw,jjjz,jjzh".IndexOf(tables[j, 0].ToLower()) > 0 && File.Exists(fxj.FxjPath + @"data\" + tables[j, 2]))
                                    {
                                        marketNode[i].Nodes.Add(tables[j, 1] + "|" + fxj.GetMarkets()[i, 0].ToLower() + "|" + tables[j, 0] + "|" + fxj.GetMarkets()[i, 0].ToLower() + tables[j, 0] + ".txt|" + tables[j, 2]);
                                    }



                                }
                                else   //�����г�
                                {
                                    if (File.Exists(fxj.FxjPath + @"data\" + fxj.GetMarkets()[i, 0] + @"\" + tables[j, 2]))
                                    {
                                        if (",,,fp,gb,gd,cw,jjjz,jjzh".IndexOf(tables[j, 0].ToLower()) <= 0 && tables[j, 0].ToLower() != "bk")
                                        {
                                            marketNode[i].Nodes.Add(tables[j, 1] + "|" + fxj.GetMarkets()[i, 0].ToLower() + "|" + tables[j, 0] + "|" + fxj.GetMarkets()[i, 0].ToLower() + tables[j, 0] + ".txt|" + tables[j, 2]);
                                        }
                                    }

                                }
                            }

                            foreach (FileInfo file in dir.GetFiles(@"*.PRP"))
                            {
                                marketNode[i].Nodes.Add("��ʷ�ֱʳɽ�|" + fxj.GetMarkets()[i, 0].ToLower() + "|hqmb|"+ fxj.GetMarkets()[i, 0].ToLower()+"hqmb" + file.Name.Substring(0, file.Name.IndexOf(".")) + ".txt|" + file.Name);
                            }
                        }
                    }
                    marketNode[fxj.GetMarkets().GetLength(0)] = treeView1.Nodes.Add("��������");
                    marketNode[fxj.GetMarkets().GetLength(0)].Nodes.Add("���|ss|bk|bk.txt|block.def");
                    marketNode[fxj.GetMarkets().GetLength(0)].Nodes.Add("����|ss|pj|pj.txt|����.str");
                }
            }

        }
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }
        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "ȡ��(&C)")
            {
                if (MessageBox.Show("ȷ��Ҫ�ж�����ת����", "ȷ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    stopConverting = true;
                    outputBox.Enabled = true;
                    treeView1.Enabled = true;
                    status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":�ж�ת��!\n";
                }
                return;
            }

            if (treeView1.Nodes.Count < 1)
            {
                MessageBox.Show("û���ҵ����ݣ�", "����");
                return;
            }
            if (descPath.Text.Trim() == "")
            {
                MessageBox.Show("��ѡ��Ŀ���ļ��У�", "����");
                descPath.Focus();
                return;
            }
            if(Directory.Exists(descPath.Text)==false)
            {
                MessageBox.Show("�ļ��С�" + descPath.Text +"�������ڣ�������ѡ��򴴽���", "����",MessageBoxButtons.OK);
                descPath.Focus();
                return;
            }
            if (MessageBox.Show("ת������ʱ����������ѡ���ڡ�" + descPath.Text.Trim() + "���д�������ļ���ͬ���ļ��������ǣ�\n\nȷ����ʼת����","ȷ��",MessageBoxButtons.OKCancel,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)!=DialogResult.OK)
            {
                return;
            }

            //ת������
            int errorCounts = 0;
            button1.Text = "ȡ��(&C)";
            status.Text = "";
            status2.Text = "";
            status3.Text = DateTime.Now.ToString("HH:mm:ss") + ":��ʼת��...\n";
            outputBox.Enabled = false;
            treeView1.Enabled = false;
            try
            {
                FxjData fxj = new FxjData();
                string[,] x;
                string s="";
                string sepChar = seperateChar.Text;
                for (int i = 0; i < treeView1.Nodes.Count ; i++)
                {
                    if (treeView1.Nodes[i].Nodes.Count > 0)
                    {
                        string[,] codes = fxj.GetData("dm", treeView1.Nodes[i].Nodes[0].Text.Split('|')[1],0);
                        foreach (TreeNode node in treeView1.Nodes[i].Nodes)
                        {

                            if (node.Checked)
                            {
                                status.Text = node.Text.Split('|')[1] + node.Text.Split('|')[2];
                                status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":��ʼת��" + status.Text + " \n";
                                Application.DoEvents();
                                switch (node.Text.Split('|')[2])
                                {
                                    case "dm":
                                        StreamWriter swdm = new StreamWriter(descPath.Text + @"\" + node.Text.Split('|')[3], false);
                                        x = fxj.GetFields("dm");
                                        for (int ii = 0; ii < x.GetLength(0); ii++)
                                        {
                                            swdm.Write(x[ii, 0] + "(" + x[ii, 1] + ")" + sepChar);

                                        }
                                        swdm.WriteLine();
                                        x = fxj.GetData("dm", node.Text.Split('|')[1],0);
                                        if (fxj.Error == 1 && errorCounts < 10) { status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":" + node.Text.Split('|')[1] + "," + fxj.Msg + (errorCounts == 9 ? "\n ������������" : "") + " \n"; errorCounts++; }
                                        for (int ii = 0; ii < x.GetLength(0); ii++)
                                        {
                                            status2.Text = x[ii, 0];
                                            Application.DoEvents();
                                            if (stopConverting) { swdm.Close(); return; }
                                            for (int jj = 0; jj < x.GetLength(1); jj++)
                                            {
                                                swdm.Write(x[ii, jj] + sepChar);
                                            }
                                            swdm.WriteLine();
                                        }
                                        swdm.Close();
                                        status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":����" + node.Text.Split('|')[3] + " \n";
                                        break;
                                    case "bk":
                                    case "pj":
                                        StreamWriter sw0 = new StreamWriter(descPath.Text + @"\" + node.Text.Split('|')[3], false);
                                        x = fxj.GetFields(node.Text.Split('|')[2]);
                                        for (int ii = 0; ii < x.GetLength(0); ii++)
                                        {
                                            sw0.Write(x[ii, 0] + "(" + x[ii, 1] + ")" + sepChar);

                                        }
                                        sw0.WriteLine();
                                        x = fxj.GetData(node.Text.Split('|')[2], "",0);
                                        if (fxj.Error == 1 && errorCounts < 10) { status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":" + node.Text.Split('|')[2] + "," + fxj.Msg + (errorCounts == 9 ? "\n ������������" : "") + " \n"; errorCounts++; }
                                        for (int ii = 0; ii < x.GetLength(0); ii++)
                                        {
                                            status2.Text = x[ii, 0];
                                            Application.DoEvents();
                                            if (stopConverting) { sw0.Close(); return; }
                                            for (int jj = 0; jj < x.GetLength(1); jj++)
                                            {
                                                sw0.Write(x[ii, jj] + sepChar);
                                            }
                                            sw0.WriteLine();
                                        }
                                        sw0.Close();
                                        status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":����" + node.Text.Split('|')[3] + " \n";
                                        break;
                                    default:
                                        StreamWriter sw = new StreamWriter(descPath.Text + @"\" + node.Text.Split('|')[3], false);
                                        x = fxj.GetFields(node.Text.Split('|')[2]);
                                        for (int ii = 0; ii < x.GetLength(0); ii++)
                                        {
                                            sw.Write(x[ii, 0] + "(" + x[ii, 1] + ")" + sepChar);

                                        }
                                        sw.WriteLine();
                                        for (int c = 0; c < codes.GetLength(0); c++)
                                        {
                                            if (fxj.GetCodeType(codes[c, 0]) != "")
                                            {
                                                status2.Text = codes[c, 0];
                                                Application.DoEvents();

                                                x = fxj.GetData(node.Text.Split('|')[2], codes[c, 0], node.Text.Split('|')[4],0);
                                                if (fxj.Error == 1 && errorCounts < 10) { status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":" + codes[c, 0] + "," + fxj.Msg + (errorCounts==9?"\n ������������":"") + " \n"; errorCounts++; }
                                                for (int ii = 0; ii < x.GetLength(0); ii++)
                                                {
                                                    if (stopConverting) { sw.Close(); return; }
                                                    s = "";
                                                    for (int jj = 0; jj < x.GetLength(1); jj++)
                                                    {
                                                        s += x[ii, jj] + sepChar;
                                                    }
                                                    sw.WriteLine(s);
                                                }
                                            }
                                           
                                        }
                                        status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":����" + node.Text.Split('|')[3] + " \n";
                                        sw.Close();
                                        break;
                                }
                            }

                        }

                    }

                }
            }//try
            catch (Exception ex)
            {
                status3.Text += "��������" + ex.Message+"\n";
            }
            finally
            {
                button1.Text = "ת��(&T)";
                stopConverting = false;
                outputBox.Enabled = true;
                treeView1.Enabled = true;
                status3.Text += DateTime.Now.ToString("HH:mm:ss") + ":ת������.\n";
            }


        }

        private void browseFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = descPath.Text;
            folderBrowserDialog1.ShowDialog();
            descPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void url_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com/";
            process.Start();
        }




    }
}