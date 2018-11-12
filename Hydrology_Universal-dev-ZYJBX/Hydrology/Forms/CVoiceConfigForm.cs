using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.Utils;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace Hydrology.Forms
{
    public partial class CVoiceConfigForm : Form
    {
        private OpenFileDialog m_openFileDialog;
        /// <summary>
        /// 当前文件的完整路径
        /// </summary>
        private String m_strCurrentPathAndFileName;
        public CVoiceConfigForm()
        {
            InitializeComponent();
            m_openFileDialog = new OpenFileDialog()
            {
                Title = "选择音频文件",
                Filter = "音频文件(*.wav)|*.wav|(*.mp3)|*.mp3"
            };

            string path = string.Empty;
            bool enabled = false;

            VoiceXmlHelper.ReadFromXML(out path, out enabled);
            this.textBox_FileName.Text = Path.GetFileName(m_strCurrentPathAndFileName);
            this.chkEnabled.Checked = enabled;
            m_strCurrentPathAndFileName = path;

            // 是否正在播放音频文件
            this.btnStop.Enabled = CVoicePlayer.Instance.IsCurrentPlay();

            UpdateFileAndPathTextBox();

            // 添加事件响应
            this.FormClosing += new FormClosingEventHandler(EHFormClosing);
        }

        /// <summary>
        /// 关闭窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EHFormClosing(object sender, FormClosingEventArgs e)
        {
            //如果当前正在播放音乐，并且是试听的话 ，就停止播放
            if ((!CVoicePlayer.Instance.BIsCurrentFromWarningInfo) && CVoicePlayer.Instance.IsCurrentPlay())
            {
                CVoicePlayer.Instance.Stop();
            }
        }

        private void btn_OpenFile_Click(object sender, EventArgs e)
        {
            // 浏览本地dll文件
            m_openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();// 设置起始目录


            //string dir = this.textBox_FileName.Text;
            if (m_strCurrentPathAndFileName != null && !m_strCurrentPathAndFileName.Equals(""))
            {
                //dir = dir.Substring(0, dir.LastIndexOf('\\'));
                m_openFileDialog.InitialDirectory = Path.GetDirectoryName(m_strCurrentPathAndFileName);
            }
            DialogResult result = m_openFileDialog.ShowDialog();

            if (result == DialogResult.OK && (!m_openFileDialog.FileName.ToString().Equals("")))
            {
                // 显示路径
                // this.textBox_FileName.Text = m_openFileDialog.FileName;
                m_strCurrentPathAndFileName = m_openFileDialog.FileName;
                UpdateFileAndPathTextBox();
            }
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            // 播放一次
            CVoicePlayer.Instance.Play(m_strCurrentPathAndFileName);
            btnStop.Enabled = true; //启用停止菜单
            //VoicePlayer.Instance.Play();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //string path = this.textBox_FileName.Text.Trim();
            string path = m_strCurrentPathAndFileName;
            bool enabled = this.chkEnabled.Checked;

            if (String.IsNullOrEmpty(path))
            {
                MessageBox.Show(String.Format("音频文件名不能为空!"));
                return;
            }
            if (!File.Exists(path))
            {
                MessageBox.Show(String.Format("音频文件{0}不存在!", path));
                return;
            }

            if (VoiceXmlHelper.WriteToXML(path, enabled))
            {
                MessageBox.Show("保存成功！");
            }
        }
        private void UpdateFileAndPathTextBox()
        {
            if (m_strCurrentPathAndFileName != null && !m_strCurrentPathAndFileName.Equals(""))
            {
                textBox_FileName.Text = Path.GetFileName(m_strCurrentPathAndFileName);
                textBox_FilePath.Text = Path.GetDirectoryName(m_strCurrentPathAndFileName);
            }
        }

        /// <summary>
        /// 停止按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            CVoicePlayer.Instance.Stop();
            btnStop.Enabled = false;
        }
    }

    internal class CVoicePlayer
    {
        #region 单例模式
        public static CVoicePlayer m_instance = null;
        private CVoicePlayer()
        {
            m_currentPlayer = new ZPlay();
        }

        public static CVoicePlayer Instance
        {
            get 
            {
                if (null == m_instance)
                {
                    m_instance = new CVoicePlayer();
                }
                return m_instance;
            }
        }
        #endregion 单例模式

        #region 数据成员
        /// <summary>
        /// 当前的播放实例
        /// </summary>
        private ZPlay m_currentPlayer = null;

        private bool m_bIsCurrentFromWarningInfo = false;

        /// <summary>
        /// 当前播放的音乐是否是报警信息
        /// </summary>
        public bool BIsCurrentFromWarningInfo
        {
            get { return m_bIsCurrentFromWarningInfo; }
        }

        #endregion 数据成员

        /// <summary>
        /// 循环播放当前配置文件中的音频文件,告警信息时候调用
        /// </summary>
        public void Play()
        {
            try
            {
                string source = string.Empty;
                bool enabled = false;
                VoiceXmlHelper.ReadFromXML(out source, out enabled);

                if (File.Exists(source) && enabled)
                {
                    m_currentPlayer.OpenFile(source, TStreamFormat.sfAutodetect);
                    // 循环播放
                    TStreamInfo info = new TStreamInfo();
                    m_currentPlayer.GetStreamInfo(ref info);
                    
                    TStreamTime time = new TStreamTime();
                    time.sec = 0;
                    TStreamTime timeEnd = new TStreamTime();
                    timeEnd.sec = info.Length.sec;
                    m_currentPlayer.PlayLoop(TTimeFormat.tfSecond, ref time, TTimeFormat.tfSecond, ref timeEnd, Int32.MaxValue, true);
                    m_bIsCurrentFromWarningInfo = true;
                    //Play(source);
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 试听，会传入一个文件路径
        /// </summary>
        /// <param name="path"></param>
        public void Play(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show(string.Format("音频文件{0}不存在！", path));
                    return;
                }
                m_bIsCurrentFromWarningInfo = false;
                if (m_currentPlayer.OpenFile(path, TStreamFormat.sfAutodetect))
                    m_currentPlayer.StartPlayback();
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 停止播放当前的音频文件
        /// </summary>
        public void Stop()
        {
            try
            {
                if (null != m_currentPlayer)
                {
                    m_currentPlayer.StopPlayback();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 判断当前是否正在播放音频文件
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentPlay()
        {
            TStreamStatus status = new TStreamStatus();
            m_currentPlayer.GetStatus(ref status);
            return status.fPlay;
        }

    }

    internal class VoiceXmlHelper
    {
        public static bool WriteToXML(string path, bool enabled)
        {
            try
            {
                if (!Directory.Exists("Config"))
                {
                    Directory.CreateDirectory("Config");
                }
                XElement xElement =
                    new XElement("sound",
                        new XElement("enable", enabled),
                        new XElement("source", path)
                    );
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Encoding = new UTF8Encoding(false),
                    Indent = true
                };
                XmlWriter xw = XmlWriter.Create("Config/sounds.xml", settings);
                xElement.Save(xw);
                xw.Flush();
                xw.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
        public static void ReadFromXML(out string path, out bool enabled)
        {
            string filename = "Config/sounds.xml";
            path = string.Empty;
            enabled = false;
            if (File.Exists(filename))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                var root = xmlDoc.DocumentElement;

                XmlNode singleChild = xmlDoc.SelectSingleNode("sound/enable");
                enabled = Boolean.Parse(singleChild.InnerText);

                singleChild = xmlDoc.SelectSingleNode("sound/source");
                path = singleChild.InnerText.ToString();
            }
            else
            {
                VoiceXmlHelper.WriteToXML("default.wav", true);
                ReadFromXML(out path, out enabled);
            }
        }
    }
}
