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
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

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
            if (news == FSM.CREDIT_MINED)
            {
                button2.Enabled = true;
                FSM.INFO_SAVED.Set();
            }

            if (news == FSM.INFO_SAVED)
            {
                PushBulletUpdate.sendDataToPushbullet(totalInfo);
                FSM.INFO_MAILED.Set();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            State.StateChanged += on_statechange;
            FSM.InitMachine();
            wbMain.ScriptErrorsSuppressed = true; // Supress credit card page js errors.
            wbMain.Navigate(new Uri(MonyDataMacro.Properties.Settings.Default.banksite));
        }

        private void wbMain_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            groupBox1.Enabled = true;

            // Url checkin is here so logging should be here
            Log(e.Url.AbsoluteUri.ToString());
            txtUrl.Text = e.Url.AbsoluteUri.ToString(); 

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
                }
                else
                {
                    Log("Main page is not valid, stopping.");
                    FSM.IDLE.Set();
                }
            }
            else if ( wbMain.Url.AbsoluteUri.StartsWith(PrivateData.Default.creditSiteAfterNavigation) && FSM.State == FSM.CREDIT_NAVIGATED )
            {
                Validiation.IValidate creditMain = new Validiation.CreditListValidate();
                creditMain.Init(wbMain.Document);
                if (creditMain.IsValid())
                {
                    Log("Credit page is valid");
                    Mining.IInfoMine creditMineInfo = new Mining.CreditListInfoMine();
                    creditMineInfo.Mine(wbMain.Document);

                    totalInfo += creditMineInfo.GetInfo<string>();

                    FSM.CREDIT_MINED.Set();
                }
                else
                {
                    Log("Credit page is not valid, stopping.");
                    FSM.IDLE.Set();
                }
            }

        }


        #region First Bank Page

        private void button1_Click(object sender, EventArgs e)
        {
            // Start Everything with main site of bank
            // Click on user and then start the timer

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

                        Rectangle pos = Utils.findElemPosition(wbMain, elem, "LoginIframeTag");
                        Log("Rectangle of label: (" + pos.X + "," + pos.Y + " " + pos.Width + "," + pos.Height + ")");

                        if (!pos.Size.IsEmpty)
                        {
                            Point ScreenPosition = wbMain.PointToScreen(new Point(pos.X + pos.Width / 2, pos.Y + 3));
                            Cursor.Position = ScreenPosition;
                            Log("Moved curser to position.");

                            Log("Clicking");
                            Utils.DoMouseClick();

                            FSM.USERNAME_CLICK.Set();

                            labelFound = true;
                        }
                        else
                        {
                            Log("Client rea of login username is empty.");
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
                else if (FSM.State == FSM.PASS_CLICK)
                {
                    Log("Typing Password");
                    // Press enter after pass to submit
                    SendKeys.SendWait(MonyDataMacro.Properties.Settings.Default.password + "{ENTER}"); 

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
                        if (elem != null)
                        {

                            Log("Found pass label");
                            Rectangle size = elem.ClientRectangle;

                            Rectangle pos = Utils.findElemPosition(wbMain, elem, "LoginIframeTag");
                            Log("Rectangle of label: (" + pos.X + "," + pos.Y + " " + pos.Width + "," + pos.Height + ")");

                            if (!pos.Size.IsEmpty)
                            {
                                Point ScreenPosition = wbMain.PointToScreen(new Point(pos.X + pos.Width / 2, pos.Y + 3));
                                Cursor.Position = ScreenPosition;
                                Log("Moved curser to position.");

                                Log("Clicking");
                                Utils.DoMouseClick();

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

        #endregion


        #region GUI CODE

        public void Log(string s)
        {
            lstLog.Items.Add(s);
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
                Utils.ErrorBox(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(totalInfo);
        }
        
        #endregion
    }
}
