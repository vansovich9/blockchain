using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Diagnostics;
using SQLite;
using SQLite.Net;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioController controller;
        GpioPin ch1;
        GpioPin ch2;
        Stopwatch stopWatch = new Stopwatch();
        DispatcherTimer timer1 = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();
        SQLiteConnection conn; 
        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
        }

        [DataContract]
        private class check_values
        {
            [DataMember]
            public TimeSpan time { get; set; }
            [DataMember]
            public double value { get; set; }
            [DataMember]
            public int device { get; set; }
        }

        private void InitGPIO()
        {
            timer1.Interval = new TimeSpan(0,0,5);
            timer1.Tick += Timer_Tick1;
            timer1.Start();
            timer2.Interval = new TimeSpan(0, 0, 5);
            timer2.Tick += Timer_Tick2;
            timer2.Start();
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "water_checker_db.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            //if (conn.Table<check_values>() == null)
                conn.CreateTable<check_values>();

            controller = GpioController.GetDefault();
            ch1 = controller.OpenPin(26);
            ch1.ValueChanged += Ch_ValueChanged;
            ch2 = controller.OpenPin(13);
            ch2.ValueChanged += Ch_ValueChanged;
            stopWatch.Start();
        }

        private async void Timer_Tick1(object sender, object e)
        {
            SQLiteCommand cmd = conn.CreateCommand("select time, value, device from check_values where device = 26", "");
            List<check_values> arr_awk = cmd.ExecuteQuery<check_values>();
            listBox.Items.Clear();
            double valc = 0;
            TimeSpan tsc = new TimeSpan();
            foreach (check_values chv in arr_awk)
            {
                valc += chv.value;
                tsc += chv.time;
                string it = chv.time.ToString() + " - " + chv.value.ToString();
                listBox.Items.Add(it);
            }
            textBlock.Text = "Итого : " + valc + " за " + tsc;
            if (tsc.TotalMinutes >= 1)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<check_values>));
                MemoryStream stream1 = new MemoryStream();
                ser.WriteObject(stream1, arr_awk);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string json_str_arr = sr.ReadToEnd();

                ser = new DataContractJsonSerializer(typeof(TimeSpan));
                stream1 = new MemoryStream();
                ser.WriteObject(stream1, stopWatch.Elapsed);
                stopWatch.Restart();
                stream1.Position = 0;
                sr = new StreamReader(stream1);
                string json_str_ts = sr.ReadToEnd();

                ser = new DataContractJsonSerializer(typeof(int));
                stream1 = new MemoryStream();
                ser.WriteObject(stream1, 26);
                stopWatch.Restart();
                stream1.Position = 0;
                sr = new StreamReader(stream1);
                string json_str_dv = sr.ReadToEnd();

                string str_url = "http://woterchecker.azurewebsites.net/api/values";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str_url);

                request.Method = "POST";
                request.Accept = "application/json";
                
                try
                {
                    string postData = json_str_arr+ "\r\n" + json_str_ts+"\r\n" + json_str_dv;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Set the ContentLength property of the WebRequest.
                    Stream dataStream = await request.GetRequestStreamAsync();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Dispose();

                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
                    {
                        SQLiteCommand cmd_del = conn.CreateCommand("delete from check_values where device = 26", "");
                        cmd_del.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        private async void Timer_Tick2(object sender, object e)
        {
            SQLiteCommand cmd = conn.CreateCommand("select time, value, device from check_values where device = 13", "");
            List<check_values> arr_awk = cmd.ExecuteQuery<check_values>();
            listBox.Items.Clear();
            double valc = 0;
            TimeSpan tsc = new TimeSpan();
            foreach (check_values chv in arr_awk)
            {
                valc += chv.value;
                tsc += chv.time;
                string it = chv.time.ToString() + " - " + chv.value.ToString();
                listBox.Items.Add(it);
            }
            textBlock.Text = "Итого : " + valc + " за " + tsc;
            if (tsc.TotalMinutes >= 1)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<check_values>));
                MemoryStream stream1 = new MemoryStream();
                ser.WriteObject(stream1, arr_awk);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);
                string json_str_arr = sr.ReadToEnd();

                ser = new DataContractJsonSerializer(typeof(TimeSpan));
                stream1 = new MemoryStream();
                ser.WriteObject(stream1, stopWatch.Elapsed);
                stopWatch.Restart();
                stream1.Position = 0;
                sr = new StreamReader(stream1);
                string json_str_ts = sr.ReadToEnd();

                ser = new DataContractJsonSerializer(typeof(int));
                stream1 = new MemoryStream();
                ser.WriteObject(stream1, 13);
                stopWatch.Restart();
                stream1.Position = 0;
                sr = new StreamReader(stream1);
                string json_str_dv = sr.ReadToEnd();

                string str_url = "http://woterchecker.azurewebsites.net/api/values";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str_url);

                request.Method = "POST";
                request.Accept = "application/json";

                try
                {
                    string postData = json_str_arr + "\r\n" + json_str_ts + "\r\n" + json_str_dv;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Set the ContentLength property of the WebRequest.
                    Stream dataStream = await request.GetRequestStreamAsync();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Dispose();

                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                    if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK)
                    {
                        SQLiteCommand cmd_del = conn.CreateCommand("delete from check_values where device = 13", "");
                        cmd_del.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        private void Ch_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                //Отправляем показания в AZURE
                check_values awc = new check_values();
                awc.time = stopWatch.Elapsed;
                awc.value = 0.001;
                SQLiteCommand cmd = conn.CreateCommand("insert into check_values (time, value, device) values (?,?,?)", awc.time, awc.value,sender.PinNumber);
                cmd.ExecuteNonQuery();
                stopWatch.Restart();
                conn.Commit();

            }
        }
        
    }
}
