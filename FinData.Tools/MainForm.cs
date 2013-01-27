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

        private void 分析家数据读取器FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form fxjReader = new FinData.Tools.FxjReaderForm();
            //fxjReader.ShowDialog();

            FinData.FxjData fxjReader = new FinData.FxjData();
            fxjReader.ShowFxjReader();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FxjData fxj = new FxjData();
            fxj.ShowAboutBox();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 数海淘金网ZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com";
            process.Start();
        }

        private void 在线帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "iexplore.exe";
            process.StartInfo.Arguments = "http://www.zhangwenzhang.com/findata";
            process.Start();
        }

        private void 分析家信息FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //从注册表中读取分析家信息
            string fxjInfo = "";
            RegistryKey keyFxj;
            RegistryKey keySoftware = Registry.LocalMachine.OpenSubKey("Software");
            keyFxj = keySoftware.OpenSubKey("FXJ");
            if (keyFxj == null)
            {
                keyFxj = keySoftware.OpenSubKey("Huitianqi");
                if (keyFxj == null)
                {
                    fxjInfo = "没有找到分析家！";
                    MessageBox.Show(fxjInfo, "分析家");
                    return;
                }
            }
            RegistryKey keySuperstk = keyFxj.OpenSubKey("SUPERSTK");
            if (keySuperstk != null)
            {
                fxjInfo = "安装版本： " + (string)keySuperstk.GetValue("VERSION");
                fxjInfo += "\n\n安装目录： " + (string)keySuperstk.GetValue("InstPath");
                FinData.FxjData fxj = new FxjData();
                fxjInfo += "\n\n数据目录： " + fxj.FxjDataPath;
                MessageBox.Show(fxjInfo, "分析家");
            }
        }

        private void finData组件版本VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            MessageBox.Show(fxj.Version, "组件版本");
        }

        private void 工具安装位置LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本工具安装位置：" + System.Reflection.Assembly.GetExecutingAssembly().Location, "信息");

        }

        private void 数据转换器TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            fxj.ShowFxjConverter();
        }

        private void 工具TToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            分析家数据读取器FToolStripMenuItem_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            数据转换器TToolStripMenuItem_Click(sender, e);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FinData.FxjData fxj = new FxjData();
            string[,] x = fxj.GetData("hqfq", "SZ000001",0);
            MessageBox.Show(x[1, 1]);
        }
    }
}