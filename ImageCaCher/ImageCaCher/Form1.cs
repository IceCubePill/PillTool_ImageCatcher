using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using  CCWin;


namespace ImageCaCher
{  
    public partial class Form1 : CCSkinMain
    {
        public Form1()
        {
            InitializeComponent();
        }
    private void Form1_Load(object sender, EventArgs e)
    {
            conectcheck();
         
    }


        // 控件事件
        #region

        private void skinButton1_Click(object sender, EventArgs e)
        {
            conectcheck();
            string s_URL=skinTextBox1.Text;
            if (CheckURL(ref s_URL))
            {
                Uri uri=new Uri(s_URL);
                string html=GetHtmlFile(uri);
                string s_savepath=CreatFolder();
                PiceImage(html, s_savepath);
            }
        }

     

        #endregion
        //自定义事件
        #region
        /// <summary>
        /// 检测输入的url是否合法，如果不带http：//则尝试添加之，不然则提示输入不合法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool CheckURL(ref string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return true;
            }
            else
            {
                try
                {
                    url = "http://" + url;
                    Uri ur=new Uri(url);
                    WebRequest hwrq= WebRequest.Create(ur);
                    WebResponse hwrs = (HttpWebResponse) hwrq.GetResponse();
                    if (hwrs.ContentLength <= 0)
                    {
                        throw new Exception();
                    }

                }
                catch (Exception)
                {

                    MessageBox.Show("请输入正确的网站");
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// 链接网站，下载html文件
        /// </summary>
        private string GetHtmlFile(Uri uri)
        {
            ////早起版本
            WebRequest wRequest = WebRequest.Create(uri);
            WebResponse wResponse = wRequest.GetResponse();
            Stream DataStream = wResponse.GetResponseStream();//得到回应的文件流
            StreamReader DS_Reader = new StreamReader(DataStream);
            string html = DS_Reader.ReadToEnd();

            DS_Reader.Close();
            DataStream.Close();
            wResponse.Close();
            return html;


        }
        /// <summary>
        /// 创建文件夹地址
        /// </summary>
        private string CreatFolder()
        {
            //获取保存地址
            string s_SavePath;
            FolderBrowserDialog fb_dialog=new FolderBrowserDialog();
            fb_dialog.SelectedPath = "C:/";
            //创建文件夹    
            if (fb_dialog.ShowDialog() == DialogResult.OK)
                s_SavePath = fb_dialog.SelectedPath;
            else s_SavePath = System.Environment.CurrentDirectory;
            int i_temp = 0;
            while (true)
            {
             
                if (!Directory.Exists(s_SavePath + "/OutPut" + i_temp))
                {
                    Directory.CreateDirectory(s_SavePath = s_SavePath+"/OutPut" + i_temp);
                    break;
                }
                else
                    i_temp++;
            }
            skinLabel2.Text = s_SavePath;
            return s_SavePath;
        }
        /// <summary>
        /// 拾取图片
        /// </summary>
        /// <param name="html"></param>
        private void  PiceImage(string html,string s_savepath)
        {
            Regex m_hvtRegImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串 
            MatchCollection matches = m_hvtRegImg.Matches(html);
            int m_i = 0;
            string[] sUrlList = new string[matches.Count];
            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[m_i++] = match.Groups["imgUrl"].Value;

            int i_temp = 0;
            foreach (string imagehttp in sUrlList)
            {
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(imagehttp, s_savepath + "/" + i_temp + ".jpeg");
                    i_temp++;
                }
                catch 
                {
                }
            }
            MessageBox.Show("提取完成");
        }
        /// <summary>
        /// 链接检测
        /// </summary>
        void conectcheck()
        {
            Ping ping = new Ping();
            PingReply pingreply = ping.Send(new IPAddress(new byte[] { 202, 108, 22, 5 }));//百度
            if (pingreply.Status != IPStatus.Success)
            {
                MessageBox.Show("请检查网络配置");
                return;
            }
            else
                netLink_Stext.Text = "网络已连接";
        }


        #endregion
        //暂时不用的代码
        #region
        //public  bool  GetURL(out string S)
        // {

        //     if (skinTextBox1.Text == "" || !skinTextBox1.Text.Contains('.'))
        //     {
        //         MessageBox.Show("请输入正确的网站");
        //         S = "";
        //         return false;
        //     }
        //    if (skinTextBox1.Text.Contains("http://"))
        //     {
        //         string s_temp = skinTextBox1.Text;
        //         s_temp = s_temp.Replace("http://","");
        //         s_temp = s_temp.Remove(s_temp.IndexOf('/'));

        //         // 调试
        //         skinTextBox1.Text = s_temp;
        //         S = s_temp;
        //         return true;
        //     }
        //     else if (skinTextBox1.Text.Contains("https://"))
        //     {
        //         string s_temp = skinTextBox1.Text;
        //         s_temp = s_temp.Replace("https://", "");
        //         s_temp = s_temp.Remove(s_temp.IndexOf('/'));

        //         // 调试
        //         skinTextBox1.Text = s_temp;
        //         S = s_temp;
        //         return true;
        //     }
        //     S = skinTextBox1.Text;
        //         return true;
        // }
        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            skinAnimator1.Hide(this);
        }
    }
}
