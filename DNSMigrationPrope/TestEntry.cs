using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heijden.DNS;

namespace DNSMigrationPrope
{
    public partial class TestEntry : UserControl
    {
        private readonly Resolver _resolver;
        public new string Text;
        public string Server;
        public string Subject;
        public string Expected;
        public QType Qtype;
        public TestState State;

        public enum TestState
        {
            Ok,
            Fail,
            Pending,
            Unknown
        };
        public TestEntry(string text, string server, string query, QType qtype, string expected)
        {
            InitializeComponent();
            Text = text;
            label1.Text = text;
            pictureBox1.Image = new Bitmap("img/pending.png");
            _resolver = new Resolver(){
                            Recursion = true,
                            UseCache = false,
                            TimeOut = 1000,
                            Retries = 3,
                            TransportType = TransportType.Tcp,
                            DnsServer = server
            };
            Server = server;
            Subject = query;
            Qtype = qtype;
            Expected = expected;
            State = TestState.Pending;
        }

        void SetText(string text)
        {
            label1.Text = text;
        }

        void SetState(TestState state)
        {
            State = state;
            switch (state)
            {
                case TestState.Ok:
                    pictureBox1.Image = new Bitmap("img/ok.png");
                    break;
                case TestState.Fail:
                    pictureBox1.Image = new Bitmap("img/error.png");
                    break;
                case TestState.Pending:
                    pictureBox1.Image = new Bitmap("img/pending.png");
                    break;
                case TestState.Unknown:
                    pictureBox1.Image = new Bitmap("img/unknown.png");
                    break;
            }
        }

        public void Run()
        {
            SetState(TestState.Pending);
            var workerThread = new Thread(worker);
            workerThread.Start();
        }

        private void worker()
        {
            label1.Invoke((MethodInvoker) (() => label1.Text = "Sending query."));
            Response response = _resolver.Query(Subject, Qtype, QClass.ANY);
            label1.Invoke((MethodInvoker) (() => label1.Text = "Got it, parsing."));
            if (response.Error != "")
            {
                label1.Invoke((MethodInvoker) (() => label1.Text = Text + " >> " + response.Error));
                pictureBox1.Image = new Bitmap("img/error.png");
                return;
            }
            label1.Invoke((MethodInvoker) (() => label1.Text = "No errors so far."));
            if (response.header.ANCOUNT > 0)
            {
                var res = false;
                foreach (AnswerRR answerRR in response.Answers)
                {
                    var temp = answerRR.RECORD.ToString().Equals(Expected);
                    if (!temp && Qtype != QType.NS)
                    {
                        //TODO: Lets get rid if these NS specific things. euck.
                        label1.Invoke(
                            (MethodInvoker) (() => label1.Text = Text + " >> Expected: " + Expected + " but got: " +
                                                                 answerRR.RECORD.ToString()));
                    }
                    res = res || temp;

                }
                if (!res)
                {
                    label1.Invoke((MethodInvoker) (() => label1.Text = Text + " >> Incorrect or no records."));
                    SetState(TestState.Fail);
                    return;
                }
                label1.Invoke((MethodInvoker) (() => label1.Text = Text));
                SetState(TestState.Ok);
                return;
            }
            else
            {
                label1.Invoke((MethodInvoker) (() => label1.Text = Text + " >> Got no useable reply from server."));
                SetState(TestState.Fail);
                MessageBox.Show(parseStuff(response));
                return;
            }
        }

        string parseStuff(Response response)
        {
            var sb = new StringBuilder();
            sb.AppendLine(";; Got answer:");

            sb.AppendLine(";; ->>HEADER<<- opcode: " +response.header.OPCODE + ", status: " +response.header.RCODE + ", id: " +response.header.ID + "");
            sb.AppendLine(";; flags: " +(response.header.QR ? " qr" : "") + "" + (response.header.AA ? " aa" : "")+ "" + (response.header.RD ? " rd" : "")+ "" +(response.header.RA ? " ra" : "") +
                "; QUERY: "+ response.header.QDCOUNT +", ANSWER: " + response.header.ANCOUNT +", AUTHORITY: " + response.header.NSCOUNT +", ADDITIONAL: " + response.header.ARCOUNT);
            sb.AppendLine("");

            if (response.header.QDCOUNT > 0)
            {
                sb.AppendLine(";; QUESTION SECTION:");
                foreach (Question question in response.Questions)
                    sb.AppendLine(";" + question);
                sb.AppendLine("");
            }

            if (response.header.ANCOUNT > 0)
            {
                sb.AppendLine(";; ANSWER SECTION:");
                foreach (AnswerRR answerRR in response.Answers)
                    sb.AppendLine(answerRR.ToString());
                sb.AppendLine("");
            }

            if (response.header.NSCOUNT > 0)
            {
                sb.AppendLine(";; AUTHORITY SECTION:");
                foreach (AuthorityRR authorityRR in response.Authorities)
                    sb.AppendLine(authorityRR.ToString());
                sb.AppendLine("");
            }

            if (response.header.ARCOUNT > 0)
            {
                sb.AppendLine(";; ADDITIONAL SECTION:");
                foreach (AdditionalRR additionalRR in response.Additionals)
                    sb.AppendLine(additionalRR.ToString());
                sb.AppendLine("");
            }

            //sb.AppendLine(";; Query time: " + sw.ElapsedMilliseconds + " msec");
            sb.AppendLine(";; SERVER: " + response.Server.Address + "#" + response.Server.Port + "(" + response.Server.Address + ")");
            sb.AppendLine(";; WHEN: " + response.TimeStamp.ToString("ddd MMM dd HH:mm:ss yyyy", new System.Globalization.CultureInfo("en-US")));
            sb.AppendLine(";; MSG SIZE rcvd: " + response.MessageSize);
            return sb.ToString();
        }
    }
}
