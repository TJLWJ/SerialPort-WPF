using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace PORT_WPF
{
    class Host
    {
        SerialPort sp = new SerialPort();
        //定义接收数据的委托事件，便于将数据传递给其他类的成员使用
        public delegate void ShowData(string str);
        public event ShowData DataCallBack;
        /// <summary>
        /// 串口参数设置
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="baud"></param>
        public void SetProperty(string Num, string baud)
        {
            sp.PortName = Num;
            sp.BaudRate = Convert.ToInt32(baud);
            sp.DataBits = 8;
            sp.StopBits = StopBits.One;
            sp.Parity = Parity.None;
            sp.ReadTimeout = -1;
            sp.RtsEnable = true;

            //为串口数据接收事件添加委托函数
            sp.DataReceived += new SerialDataReceivedEventHandler(SP_DataRecieved);
            sp.Open();
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            sp.Close();
        }

        /// <summary>
        /// 串口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void SP_DataRecieved(object sender, SerialDataReceivedEventArgs arg)
        {
            Thread.Sleep(100);
            int length = sp.BytesToRead;
            byte[] buffer = new byte[length];
            sp.Read(buffer, 0, length);
            string data = Encoding.ASCII.GetString(buffer);
            //调用委托事件，触发绑定的回调函数
            DataCallBack(data);
        }
        /// <summary>
        /// 串口发送数据
        /// </summary>
        /// <param name="buffer"></param>
        public void SendMsg(byte[] buffer)
        {
            sp.Write(buffer, 0, buffer.Length);
        }

    }
}
