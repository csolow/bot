using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.IO.Ports;
using System.Xml;
using System.IO;

namespace bot
{
    public partial class Form1 : Form
    {

        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;
        SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        String temp;
        String condition;
        String high;
        String low;
        Choices list = new Choices();
        public Form1()
        {

            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();


            list.Add(new String[] {"Hello", "how are you", "what time is it", "what is today", "open google", "thanks", "wake", "sleep", "restart", "update", "open hearts of iron 4", "what's the weather like", "what's the temperature",
                "what is lowest point today", "what is higest point today", "minimize", "maximize", "play", "pause", "spotify", "next", "last", "test" });

            Grammar gr = new Grammar(new GrammarBuilder(list));


            try
            {

                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeachRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch { return; }

            s.SelectVoiceByHints(VoiceGender.Female);

            s.Speak("hello, my name is nanna");

            InitializeComponent();
        }
        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='copenhagen, state')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query);
            }
            catch
            {
                MessageBox.Show("No internet connection");
                return "No internet";
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                int rawtemp = int.Parse(channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value);
                temp = (rawtemp - 32) * 5/9 + "";
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                high = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["high"].Value;
                low = channel.SelectSingleNode("item").SelectSingleNode("yweather:forecast", manager).Attributes["low"].Value;
                if (input == "temp")
                {
                    return temp; 
                }
                if (input == "high")
                {
                    return high;
                }
                if (input == "low")
                {
                    return low;
                }
                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }

        public void restart()
        {
            Process.Start(@"C:\Users\bot\bot.exe");
            Environment.Exit(0);
        }

        public void say(String h)
        {
            s.Speak(h);
            wake = true;
            textBox2.AppendText(h + "\n");
        }

        private void rec_SpeachRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String r = e.Result.Text;

            if (r == "wake")
            {
                wake = true;
                label3.Text = "State:Awake";
                say("wake mode is now active");
            }


            if (r == "sleep")
            {
                wake = false;
                label3.Text = "State:Sleep mode";
                say("sleep mode is now active");
            }

        

            if (wake == true)
            {

                if (r == "open hearts of iron 4")
                {
                    Process.Start(@"C:\Program Files (x86)\Steam\steamapps\common\Hearts of Iron IV\hoi4");
                    say("for the people, everything");
                }

                if (r == "spotify")
                {
                    Process.Start(@"C:\Users\cswag\Desktop\Spotify");
                    say("somebody once told me the world was gonna roll me i aint the sharpest tool in the shed, she was looking kind of dumb with her finger and her thumb in the shape of an L on her forehead");
                }

                if (r == "next")
                {
                    SendKeys.Send("^{RIGHT}");
                }

                if (r == "last")
                {
                    SendKeys.Send("^{LEFT}");
                }

                if (r == "play" || r == "pause")
                {
                    SendKeys.Send(" ");
                }

                if (r == "restart" || r == "update")
                {
                    restart();
                }

                if (r == "what is today")
                {
                    say(DateTime.Now.ToString("d/M/yyyy"));
                }

                if (r == "test")
                {
                    say("1,2,3 1,2,3");
                }

                if (r == "Hello")
                {
                    say("hi");
                }

                if (r == "what's the weather like")
                {
                    say("The sky is " + GetWeather("cond") + ",");
                }

                if (r == "what's the temperature")
                {
                    say("it is " + GetWeather("temp") + "degrees");
                }

                if (r == "what is higest point today")
                {
                    say("the higest point today is " + GetWeather("high") + "degrees");
                }

                if (r == "what is lowest point today")
                {
                    say("the lowest point today is " + GetWeather("low") + "degrees");
                }

                if (r == "how are you")
                {
                    say("great, and you?");
                }

                if (r == "what time is it")
                {
                    say(DateTime.Now.ToString("h:m tt"));
                }

                if (r == "what is today")
                {
                    say(DateTime.Now.ToString("d/M/yyyy"));
                }

                if (r == "minimize")
                {
                    this.WindowState = FormWindowState.Minimized;
                }

                if (r == "maximize")
                {
                    this.WindowState = FormWindowState.Normal;
                }

                if (r == "open google")
                {
                    Process.Start("https://google.com");
                }

                if (r == "thanks")
                {
                    say("you're welcome.");
                }
            }
            textBox1.AppendText(r + "\n");
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
