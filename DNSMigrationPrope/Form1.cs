using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DnsDig;
using Heijden.DNS;

namespace DNSMigrationPrope
{
    public partial class Form1 : Form
    {
        // Global variables
        private Dig dig;
        public Form1()
        {
            InitializeComponent();
            dig = new Dig();
            Console.SetOut(new FeedbackWriter(this.textBox3));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dig.resolver.Recursion = true;
            dig.resolver.UseCache = false;
            dig.resolver.DnsServer = textBox1.Text;
            dig.resolver.TimeOut = 10;
            dig.resolver.Retries = 3;
            dig.resolver.TransportType = TransportType.Tcp;
            dig.BeginDigIt(textBox2.Text, QType.NS, QClass.ANY);
        }
    }
}
