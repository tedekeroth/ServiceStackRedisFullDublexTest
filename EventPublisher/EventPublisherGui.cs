using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tWorks.Alfa.AlfaCommons.Actors.Requests;

namespace EventPublisher
{
    public partial class EventPublisherGui : Form
    {
        RedisClient client;

        public EventPublisherGui()
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
            client = new RedisClient(textBox1.Text);
            client.OnServiceHandlerTriggered += Client_OnServiceHandlerTriggered;
        }

        private void Client_OnServiceHandlerTriggered(object sender, string e)
        {
            Log(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.AddMessage(new GetLegitimationsForCustomer(332211, 123));
            textBox2.Text = "Time is " + DateTime.Now.ToString("HH:mm:ss.fff");
            button2.Focus();
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
