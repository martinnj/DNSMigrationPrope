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
        public List<TestEntry> InnerEntries;
        public Monitor(Timer timer, List<TestEntry> entries)
        {
            InitializeComponent();
            InnerTimer = timer;
            InnerEntries = entries;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!InnerTimer.Enabled)
            {
                InnerTimer.Interval = (int) (numericUpDown1.Value*1000);
                numericUpDown1.Enabled = false;
                InnerTimer.Enabled = true;
                button1.Text = "Disable timed queries";
                setLastTime("" + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                foreach (var entry in InnerEntries)
                {
                    entry.Run();
                }
                textBox1.Text = "" + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + Environment.NewLine + GetSummary();
            }
            else
            {
                InnerTimer.Enabled = false;
                numericUpDown1.Enabled = true;
                button1.Text = "Enable timed queries";
            }
        }

        public void setLastTime(string timestamp)
        {
            label2.Invoke((MethodInvoker)(() => label2.Text = "Last check: " + timestamp));
        }

        public string GetSummary()
        {
            int oks = 0;
            int fails = 0;
            int unknowns = 0;
            int pending = 0;
            foreach (var entry in InnerEntries)
            {
                switch (entry.State)
                {
                    case TestEntry.TestState.Fail:
                        fails++;
                        break;
                    case TestEntry.TestState.Ok:
                        oks++;
                        break;
                    case TestEntry.TestState.Pending:
                        pending++;
                        break;
                    case TestEntry.TestState.Unknown:
                        unknowns++;
                        break;
                }
            }
            var sb = new StringBuilder();
            sb.AppendLine("Preliminary test report:");
            sb.AppendLine("");
            sb.AppendLine("Successful tests: " + oks);
            sb.AppendLine("Failed tests: " + fails);
            sb.AppendLine("Unknown results/do manually: " + unknowns);
            sb.AppendLine("Pending tests: " + pending);
            return sb.ToString();
        }
    }
}
