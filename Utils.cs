using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro
{
    class Utils
    {
        

        public static void ErrorBox(Exception ex, string titleRow = "Error Occured")
        {
            MessageBox.Show(titleRow + "\n\n" + ex.Message + "\n\n" + ex.StackTrace.ToString());
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            //int X = Cursor.Position.X;
            //int Y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
        }

        public static Rectangle findElemPosition(WebBrowser wbMain, HtmlElement elem, string frameId = "")
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
                return new Rectangle(xoff - scrollBarXPosition, yoff - scrollBarYPosition, elemSize.Width, elemSize.Height);
            }
            else
            {
                return new Rectangle(-1, -1, 0, 0);
            }
        }
    }
}
