using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Heijden.DNS;

namespace DNSMigrationPrope
{
    public partial class MainForm : Form
    {
        private readonly string _testsfile;
        private SummaryPage _summary;
        private Monitor _monitor;
        private List<TestEntry> _entries;

        public MainForm()
        {
            InitializeComponent();
            _testsfile = "tests.xml";
            _entries = new List<TestEntry>();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            buildSuites();
        }

        void buildSuites()
        {
            tabControl1.TabPages.Clear();
            _entries = new List<TestEntry>();

            var doc = new XmlDocument();
            try
            {
                doc.Load(_testsfile);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("File not found: " + ex.Message);
                return;
            }


            var suites = doc.SelectNodes("tests/suite");
            if (suites == null)
            {
                MessageBox.Show("No tests found in file.");
                return;
            }

            // Setup and start tests.
            foreach (XmlNode suite in suites)
            {
                if (suite.Attributes == null)
                {
                    break;
                }

                var name = suite.Attributes["name"].Value;
                var server = suite.Attributes["server"].Value;
                var tab = new TabPage { Text = "" + name + " - " + server };
                var tests = suite.SelectNodes("test");
                if (tests != null)
                {
                    var page = new Suitepage();
                    page.Dock = DockStyle.Fill;
                    foreach (XmlNode test in tests)
                    {
                        var text = test.SelectSingleNode("text").InnerText;
                        var query = test.SelectSingleNode("query").InnerText;
                        var type = test.SelectSingleNode("type").InnerText;
                        var value = test.SelectSingleNode("value").InnerText;
                        var entry = new TestEntry(text, server, query, str2qt(type), value);
                        page.flowPanel.Controls.Add(entry);
                        _entries.Add(entry);
                    }
                    tab.Controls.Add(page);
                    tab.Controls.Add(page);
                }
                tabControl1.TabPages.Add(tab);
            }
        }

        TabPage ConstructSummaryPage()
        {
            var page = new TabPage("Summary");
            _summary = new SummaryPage {Dock = DockStyle.Fill};
            page.Controls.Add(_summary);
            return page;
        }

        QType str2qt(string type)
        {
            switch (type.ToUpper())
            {
                case "A":
                    return QType.A;
                case "CNAME":
                    return QType.CNAME;
                case "MX":
                    return QType.MX;
                case "NS":
                    return QType.NS;
                case "TXT":
                    return QType.TXT;
            }
            QType val;
            Enum.TryParse(type, out val);
            return val;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var entry in _entries)
            {
                entry.Run();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            buildSuites();
        }

        private void getSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oks = 0;
            int fails = 0;
            int unknowns = 0;
            int pending = 0;
            foreach (var entry in _entries)
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
            MessageBox.Show(sb.ToString());
        }

        private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var page = new TabPage {Text = "Monitor/scheduler"};
            _monitor = new Monitor(timer1) {Dock = DockStyle.Fill};
            page.Controls.Add(_monitor);
            tabControl1.TabPages.Add(page);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _monitor.setLastTime("" + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
            foreach (var entry in _entries)
            {
                entry.Run();
            }
        }
    }
}
