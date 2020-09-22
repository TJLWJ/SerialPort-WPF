using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PORT_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConnected = false;
        Host m_host = new Host();
        public MainWindow()
        {
            InitializeComponent();
            DetectPort();

            //为m_host实例的委托事件绑定回调函数
            m_host.DataCallBack += new Host.ShowData(Data2Win);
        }
        /// <summary>
        /// 建立/断开通信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                string portNo = Combo_PortNo.Text;
                string baud = Combo_Baud.Text;
                m_host.SetProperty(portNo, baud);
                isConnected = true;
                Combo_PortNo.IsEnabled = false;
                Combo_Baud.IsEnabled = false;
                Button_Connect.Content = "断开通信";
                Button_Connect.Background = Brushes.LightGreen;
            }
            else
            {
                m_host.ClosePort();
                isConnected = false;
                Combo_PortNo.IsEnabled = true;
                Combo_Baud.IsEnabled = true;
                Button_Connect.Content = "建立通信";
                Button_Connect.Background = Brushes.LightGoldenrodYellow;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                string msg = TextBox_Send.Text;
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                m_host.SendMsg(buffer);
            }
            else
            {
                MessageBox.Show("串口未打开", "错误");
            }
        }
        /// <summary>
        /// 检测可用串口
        /// </summary>
        /// <returns></returns>
        private int DetectPort()
        {
            int port = 0;
            SerialPort SP = new SerialPort();
            for (int i = 0; i < 10; i++)
            {
                SP.PortName = "COM" + (i + 1).ToString();
                try
                {
                    SP.Open();
                    SP.Close();
                    Combo_PortNo.Items.Add("COM" + (i + 1).ToString());
                    port += 1;

                }
                catch
                {
                    continue;
                }              
            }
            if (port > 0)
            {
                Combo_PortNo.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("没有可用串口");
            }
            return port;
        }

        private void Data2Win(string data)
        {
            data += "\r\n";
            this.Dispatcher.Invoke(() =>
            {
                TextBox_Recieved.AppendText(data);
                TextBox_Recieved.ScrollToEnd();
            });
        }
    }
}
