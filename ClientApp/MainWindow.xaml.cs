using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkStream ns = null;
        IPEndPoint serverEndPoint;
        TcpClient tcpClient ;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        StreamReader sr = null;
        StreamWriter sw = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            tcpClient = new TcpClient();
            string serverAddress = ConfigurationManager.AppSettings["ServerAddress"]!;
            short serverPort = short.Parse( ConfigurationManager.AppSettings["ServerPort"]!);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);  
        }

        private void SendBtnClick(object sender, RoutedEventArgs e)
        {            
            string message = msgTextBox.Text;
            sw.WriteLine(message);
            sw.Flush(); 
        }

        private void ConnectionBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                tcpClient.Connect(serverEndPoint);
                ns = tcpClient.GetStream();
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                Listen();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }        
        }
 
        private async void Listen()
        {
            while (true)
            {
                string? message = await sr.ReadLineAsync();             
                messages.Add(new MessageInfo(message));
            }           
        }
        private void DisconnectBtnClick(object sender, RoutedEventArgs e)
        {
            sw.Close();
            sr.Close();
            ns.Close(); 
            tcpClient.Close();  
        }
    }
    public class MessageInfo
    {
        public string Message { get; set; } 
        public DateTime Time { get; set; }
        public MessageInfo(string? text)
        {
            Message = text ??""; 
            Time = DateTime.Now;
        }
        public override string ToString()
        {
            return $" {Message} : {Time.ToShortTimeString()}";
        }
    }
}
