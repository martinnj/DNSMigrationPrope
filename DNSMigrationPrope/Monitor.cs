using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNSMigrationPrope
{
    public partial class Monitor : UserControl
    {
        public Timer InnerTimer;
        public Monitor(Timer timer)
        {
            InitializeComponent();
            InnerTimer = timer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!InnerTimer.Enabled)
            {
                InnerTimer.Interval = (int) (numericUpDown1.Value*1000);
                InnerTimer.Enabled = true;
                button1.Text = "Disable timed queries";
            }
            else
            {
                InnerTimer.Enabled = false;
                button1.Text = "Enable timed queries";
            }
        }

        public void setLastTime(string timestamp)
        {
            label2.Invoke((MethodInvoker)(() => label2.Text = "Last check: " + timestamp));
        }
    }
}
