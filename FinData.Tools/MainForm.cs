using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using FinData;
namespace FinData
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ���������ݶ�ȡ��FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form fxjReader = new FinData.Tools.FxjReaderForm();
            //fxjReader.ShowDialog();

            FinData.FxjData fxjReader = new FinData.FxjData();
            fxjReader.ShowFxjReader();
        }

        private void ����AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FxjData fxj = new FxjData();
            fxj.ShowAboutBox();
        }

        private void �˳�XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void �����Խ���ZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com";
            process.Start();
        }

        private void ���߰���HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com/findata";
            process.Start();
        }

        private void ��������ϢFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //��ע����ж�ȡ��������Ϣ
            string fxjInfo = "";
            RegistryKey keyFxj;
            RegistryKey keySoftware = Registry.LocalMachine.OpenSubKey("Software");
            keyFxj = keySoftware.OpenSubKey("FXJ");
            if (keyFxj == null)
            {
                keyFxj = keySoftware.OpenSubKey("Huitianqi");
                if (keyFxj == null)
                {
                    fxjInfo = "û���ҵ������ң�";
                    MessageBox.Show(fxjInfo, "������");
                    return;
                }
            }
            RegistryKey keySuperstk = keyFxj.OpenSubKey("SUPERSTK");
            if (keySuperstk != null)
            {
                fxjInfo = "��װ�汾�� " + (string)keySuperstk.GetValue("VERSION");
                fxjInfo += "\n\n��װĿ¼�� " + (string)keySuperstk.GetValue("InstPath");
                FinData.FxjData fxj = new FxjData();
                fxjInfo += "\n\n����Ŀ¼�� " + fxj.FxjDataPath;
                MessageBox.Show(fxjInfo, "������");
            }
        }

        private void finData����汾VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            MessageBox.Show(fxj.Version, "����汾");
        }

        private void ���߰�װλ��LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("�����߰�װλ�ã�" + System.Reflection.Assembly.GetExecutingAssembly().Location, "��Ϣ");

        }

        private void ����ת����TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            fxj.ShowFxjConverter();
        }

        private void ����TToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ���������ݶ�ȡ��FToolStripMenuItem_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ����ת����TToolStripMenuItem_Click(sender, e);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            string[,] x = fxj.GetData("hqfq", "SZ000001",0);
            MessageBox.Show(x[1, 1]);
        }
    }
}