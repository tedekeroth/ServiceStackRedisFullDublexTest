using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventConsumer
{
    public partial class Gui : Form
    {
        private ServerAppHost serverAppHost;
        public Gui()
        {
            InitializeComponent();
            string a = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                textBox1.Text += a[r.Next(0, a.Length)];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            serverAppHost = new ServerAppHost(textBox1.Text);
            serverAppHost.OnMessageReceived += ServerAppHost_OnMessageReceived;
            serverAppHost.Init();
            serverAppHost.Start($"http://localhost:{new Random().Next(1000, 10000)}/");
            Log("Server running");
        }

        private void ServerAppHost_OnMessageReceived(object sender, string e)
        {
            Log(e);
        }

        private void Log(string message)
        {
            Invoke(new Action(() => 
            {
                richTextBox1.Text = $"{message}{Environment.NewLine}{richTextBox1.Text}";
            }));
        }

    }
}
