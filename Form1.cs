using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Validiation = MonyDataMacro.Validate;
using Mining = MonyDataMacro.InfoMine;
using PrivateData = MonyDataMacro.Properties.Settings;

namespace MonyDataMacro
{

    public partial class Form1 : Form
    {
        string totalInfo = "";

        public Form1()
        {
            InitializeComponent();
        }

        WebFSM FSM = new WebFSM();

        void on_statechange(State news)
        {
            Log("Changerd to state: " + news.Description);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FSM.InitMachine();
            State.StateChanged += on_statechange;
            wbMain.Navigate(new Uri(MonyDataMacro.Properties.Settings.Default.banksite));
        }

        private void wbMain_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            groupBox1.Enabled = true;

            if (wbMain.Url.AbsoluteUri == PrivateData.Default.portalSite && FSM.State == FSM.MAIN_BANK_NAVIGATED)
            {
                Validiation.IValidate bankMain = new Validiation.BankMainPageValidate();
                bankMain.Init(wbMain.Document);
                if (bankMain.IsValid())
                {
                    Log("Main page is valid");
                    Mining.IInfoMine bankMaininfo = new Mining.BankMainPageInfoMain();
                    bankMaininfo.Mine(wbMain.Document);

                    totalInfo +=  bankMaininfo.GetInfo<string>();

                    FSM.MAIN_BANK_MINED.Set() ;
                    FSM.CREDIT_NAVIGATED.Set();

                    wbMain.Navigate(PrivateData.Default.creditExternSite);
                    //TODO: Make it ie11 and velidate and mine Credit site
                }
                else
                {
                    Log("Main page is not valid, stopping.");
                    FSM.IDLE.Set();
                }
            }
            else if ( wbMain.Url.AbsoluteUri == PrivateData.Default.creditSite && FSM.State == FSM.CREDIT_NAVIGATED )
            {

            }

        }

        void Log(string s)
        {
            lstLog.Items.Add(s);
        }

        void ErrorBox(Exception ex, string titleRow = "Error Occured")
        {
            MessageBox.Show(titleRow + "\n\n" + ex.Message + "\n\n" + ex.StackTrace.ToString());
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            //int X = Cursor.Position.X;
            //int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
        }

        Rectangle findElemPosition(HtmlElement elem,string frameId = "")
        {
            // Calculate the offset of the element, all the way up through the parent nodes
            var parent = elem.OffsetParent;
            int xoff = elem.OffsetRectangle.X;
            int yoff = elem.OffsetRectangle.Y;
            Rectangle elemSize = elem.ClientRectangle;

            while (parent != null)
            {
                xoff += parent.OffsetRectangle.X;
                yoff += parent.OffsetRectangle.Y;
                parent = parent.OffsetParent;
            }

            if (frameId != "")
            {
                parent = wbMain.Document.GetElementById(frameId).OffsetParent;
                while (parent != null)
                {
                    xoff += parent.OffsetRectangle.X;
                    yoff += parent.OffsetRectangle.Y;
                    parent = parent.OffsetParent;
                }
            }


            // Get the scrollbar offsets
            int scrollBarYPosition = wbMain.Document.GetElementsByTagName("HTML")[0].ScrollTop;
            int scrollBarXPosition = wbMain.Document.GetElementsByTagName("HTML")[0].ScrollLeft;

            // Calculate the visible page space
            Rectangle visibleWindow = new Rectangle(scrollBarXPosition, scrollBarYPosition, wbMain.Width, wbMain.Height);

            // Calculate the visible area of the element
            Rectangle elementWindow = new Rectangle(xoff, yoff, wbMain.ClientRectangle.Width, wbMain.ClientRectangle.Height);

            Log("Scroll positon: " + scrollBarXPosition + "," + scrollBarYPosition);
            Log("Scroll positon: " + xoff + "," + yoff);

            if (visibleWindow.IntersectsWith(elementWindow))
            {
                return new Rectangle(xoff - scrollBarXPosition, yoff - scrollBarYPosition,elemSize.Width, elemSize.Height);
            }
            else
            {
                return new Rectangle(-1, -1,0,0);
            }
        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            totalInfo = "";

            try
            {
                var frame = wbMain.Document.Window.Frames["LoginIframeTag"];
                if (frame != null)
                {
                    bool labelFound = false;
                    var allLabels = frame.Document.GetElementsByTagName("label");
                    var elem = frame.Document.GetElementById("username");
                    //foreach (HtmlElement elem in allLabels)
                    //{
                    //if (elem.InnerText == "קוד משתמש")
                    if (elem != null)
                    {

                        Log("Found username label");
                        Rectangle size = elem.ClientRectangle;

                        Rectangle pos = findElemPosition(elem, "LoginIframeTag");
                        Log("Rectangle of label: (" + pos.X + "," + pos.Y + " " + pos.Width + "," + pos.Height + ")");

                        if (!pos.Size.IsEmpty)
                        {
                            Point ScreenPosition = wbMain.PointToScreen(new Point(pos.X + pos.Width / 2, pos.Y+3));
                            Cursor.Position = ScreenPosition;
                            Log("Moved curser to position.");

                            Log("Clicking");
                            DoMouseClick();

                            FSM.USERNAME_CLICK.Set();

                            labelFound = true;
                        }
                        else
                        {
                            Log("Client area of login username is empty.");
                        }

                        //break;
                    }
                    //}

                    // Only if found username go on to password
                    if (!labelFound)
                    {
                        FSM.IDLE.Set();
                        Log("Cant find username lable in frame.");
                    }

                }
                else
                {
                    Log("Cant find login frame 'LoginIframeTag'");
                }
            }
            catch (Exception ex)
            {
                Log("Exception:");
                Log(ex.Message);
                Log(ex.StackTrace);
                FSM.IDLE.Set(); // Return to no state;
            }
        }

        bool busy = false;
        private void tmrPos_Tick(object sender, EventArgs e)
        {
            if (busy)
                return;
            busy = true;

            try
            {

                if (FSM.State == FSM.USERNAME_CLICK)
                {
                    Log("Typing username");
                    SendKeys.SendWait(MonyDataMacro.Properties.Settings.Default.username);

                    FSM.USERNAME_CLICKED.Set();
                }
                else if(FSM.State == FSM.PASS_CLICK){
                    Log("Typing Password");
                    SendKeys.SendWait(MonyDataMacro.Properties.Settings.Default.password);

                    FSM.MAIN_BANK_NAVIGATED.Set();
                }
                else if (FSM.State == FSM.USERNAME_CLICKED)
                {
                    // Now click password:
                    var frame = wbMain.Document.Window.Frames["LoginIframeTag"];

                    if (frame != null)
                    {
                        bool labelFound = false;
                        var allLabels = frame.Document.GetElementsByTagName("label");
                        var elem = frame.Document.GetElementById("password");
                        //foreach (HtmlElement elem in allLabels)
                        //{
                        //    if (elem.InnerText == "סיסמא")
                        if(elem != null)
                            {

                                Log("Found pass label");
                                Rectangle size = elem.ClientRectangle;

                                Rectangle pos = findElemPosition(elem, "LoginIframeTag");
                                Log("Rectangle of label: (" + pos.X + "," + pos.Y + " " + pos.Width + "," + pos.Height + ")");

                                if (!pos.Size.IsEmpty)
                                {
                                    Point ScreenPosition = wbMain.PointToScreen(new Point(pos.X + pos.Width / 2, pos.Y+3));
                                    Cursor.Position = ScreenPosition;
                                    Log("Moved curser to position.");

                                    Log("Clicking");
                                    DoMouseClick();

                                    FSM.PASS_CLICK.Set();

                                    labelFound = true;
                                }
                                else
                                {
                                    Log("Client area of login pass is empty.");
                                }

                                //break;
                            }
                        //}

                        // Only if found username go on to password
                        if (!labelFound)
                        {
                            FSM.IDLE.Set();
                            Log("Cant find username lable in frame.");
                        }
                    }
                    else
                    {
                        Log("Cant open login frame for password.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Exception:");
                Log(ex.Message);
                Log(ex.StackTrace);
                FSM.IDLE.Set(); // Return to no state;
            }

            busy = false;
        }

        private void lstLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLog.SelectedItem != null)
            {
                txtCopy.Text = lstLog.SelectedItem.ToString();
            }
        }

        private void wbMain_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Log(e.Url.AbsoluteUri.ToString());
            txtUrl.Text = e.Url.AbsoluteUri.ToString();
        }

        private void btnHtmlSource_Click(object sender, EventArgs e)
        {
            try
            {
                var htmlElem = wbMain.Document.GetElementsByTagName("html")[0];
                if (htmlElem != null)
                {
                    Clipboard.SetText(htmlElem.OuterHtml); // Copy to clipboard
                }
            }
            catch (Exception ex)
            {
                ErrorBox(ex);
            }
        }
    }
}
