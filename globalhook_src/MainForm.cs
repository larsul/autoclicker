using gma.System.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalHookDemo
{
    class MainForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelMousePosition;
        private Button button1;
        private TextBox txtCount;
        private System.Windows.Forms.TextBox textBox;

        public MainForm()
        {
            InitializeComponent();
        }

        // THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
        // DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
        void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.labelMousePosition = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtCount = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.textBox.Location = new System.Drawing.Point(4, 55);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(427, 340);
            this.textBox.TabIndex = 3;
            // 
            // labelMousePosition
            // 
            this.labelMousePosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMousePosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelMousePosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelMousePosition.Location = new System.Drawing.Point(4, 29);
            this.labelMousePosition.Name = "labelMousePosition";
            this.labelMousePosition.Size = new System.Drawing.Size(427, 23);
            this.labelMousePosition.TabIndex = 2;
            this.labelMousePosition.Text = "labelMousePosition";
            this.labelMousePosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonStop
            // 
            this.buttonStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStop.Location = new System.Drawing.Point(85, 3);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.ButtonStopClick);
            // 
            // buttonStart
            // 
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.Location = new System.Drawing.Point(4, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.Click += new System.EventHandler(this.ButtonStartClick);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(166, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Test";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtCount
            // 
            this.txtCount.Location = new System.Drawing.Point(232, 5);
            this.txtCount.Name = "txtCount";
            this.txtCount.Size = new System.Drawing.Size(100, 20);
            this.txtCount.TabIndex = 5;
            this.txtCount.Text = "300000";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(433, 398);
            this.Controls.Add(this.txtCount);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.labelMousePosition);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "MainForm";
            this.Text = "This application captures keystrokes";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }

        void ButtonStartClick(object sender, System.EventArgs e)
        {
            actHook.Start();
        }

        void ButtonStopClick(object sender, System.EventArgs e)
        {
            actHook.Stop();
        }


        UserActivityHook actHook;
        void MainFormLoad(object sender, System.EventArgs e)
        {
            actHook = new UserActivityHook(); // crate an instance with global hooks
                                              // hang on events
            actHook.OnMouseActivity += new MouseEventHandler(MouseMoved);
            actHook.KeyDown += new KeyEventHandler(MyKeyDown);
            actHook.KeyPress += new KeyPressEventHandler(MyKeyPress);
            actHook.KeyUp += new KeyEventHandler(MyKeyUp);
        }

        public void MouseMoved(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = String.Format("x={0}  y={1} wheel={2}", e.X, e.Y, e.Delta);
            if (e.Clicks > 0) LogWrite("MouseButton 	- " + e.Button.ToString());
        }

        public static bool stop = false;
        List<string> keypressList = new List<string>();
        CancellationTokenSource cts = new CancellationTokenSource();
        Task runner = null;
        public void MyKeyDown(object sender, KeyEventArgs e)
        {
            LogWrite("KeyDown 	- " + e.KeyData.ToString());
            keypressList.Add(e.KeyData.ToString());
            var last5 = keypressList.TakeLast(5);
            var fiveSame = last5.Distinct().Count();
            Debug.WriteLine(last5.Count());
            Debug.WriteLine(fiveSame);
            //if (last5.Count() >= 5 && fiveSame == 1)
            //{
            //    var parsed = int.TryParse(txtCount.Text, out int result);
            //    if (!parsed)
            //    {
            //        LogWrite("Your input for number is not valid.");
            //        return;
            //    }
            //    else if (result > 500000)
            //    {
            //        LogWrite("Your input for number is too large, need to be maximum 500000.");
            //        return;
            //    }
            //    Task.Factory.StartNew(() =>
            //    {
            //        Enumerable.Range(1, 300000).ToList().ForEach(_ =>
            //        {
            //            if (stop)
            //                return;

            //            uint X = Convert.ToUInt32(Cursor.Position.X);
            //            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            //            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, new UIntPtr());
            //        });
            //    }, cts.Token);
            //}
            if (e.KeyData == Keys.F11)
            {
                LogWrite("Loop started by F11");
                if (cts.IsCancellationRequested || cts == null)
                    cts = new CancellationTokenSource();
                runner = Task.Factory.StartNew(async () =>
                {
                    while (!cts.IsCancellationRequested)
                    {

                        uint X = Convert.ToUInt32(Cursor.Position.X);
                        uint Y = Convert.ToUInt32(Cursor.Position.Y);
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, new UIntPtr());
                    }
                }, cts.Token);
            }
            else if (e.KeyData == Keys.F12)
            {
                cts.Cancel();
                runner = null;
                stop = true;
                LogWrite("Loop stopped by F12");
            }

        }

        public void MyKeyPress(object sender, KeyPressEventArgs e)
        {
            LogWrite("KeyPress 	- " + e.KeyChar);
        }

        public void MyKeyUp(object sender, KeyEventArgs e)
        {
            LogWrite("KeyUp 		- " + e.KeyData.ToString());
        }

        private void LogWrite(string txt)
        {
            textBox.AppendText($"[{DateTime.Now.ToLongTimeString()}]{txt}{Environment.NewLine}");
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoMouseClick();
        }


        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        //1829, 8
        public void DoMouseClick()
        {      //Call the imported function with the cursor's current position            
            //uint X = Convert.ToUInt32(Cursor.Position.X);
            //uint Y = Convert.ToUInt32(Cursor.Position.Y);
            //Cursor.Position = new Point(-1982, 171);
            //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, new UIntPtr());
            //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, new UIntPtr());
            Thread.Sleep(2000);
            Enumerable.Range(1, 20).ToList().ForEach(_ =>
            {
                uint X = Convert.ToUInt32(Cursor.Position.X);
                uint Y = Convert.ToUInt32(Cursor.Position.Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, new UIntPtr());
            });
        }

    }
    public static class MiscExtensions
    {
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}
