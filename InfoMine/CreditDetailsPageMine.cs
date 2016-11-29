using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrivateData = MonyDataMacro.Properties.Settings;

namespace MonyDataMacro.InfoMine
{
    class CreditPurchase
    {
        public string purchaseDate;
        public string supplierName;
        public string dealSum;
        public string paymentSum;

        public override string ToString()
        {
            return this.purchaseDate + ", " + this.supplierName + "," + this.paymentSum;
        }
    }

    public class CreditDetailsPageMine : IInfoMine
    {
        public static int MaxRows = PrivateData.Default.creditInfoMaxRows;
        private string GetInfoAsString()
        {
            CreditPurchase item;
            string result = "";

            for (int i = Math.Max(0, items.Count - MaxRows); i< items.Count - 1; i++)
            {
                item = items[i];
                //result +=
                //    "[" + i + "]\n" +
                //    titlepurchaseDate + ": " + item.purchaseDate + "\n" +
                //    titlesupplierName + ": " + item.supplierName + "\n" +
                //    titledealSum + ": " + item.dealSum + "\n" +
                //    titlepaymentSum + ": " + item.paymentSum + "\n\n" 
                //    ;

                result += "(" + i + ") " + item.ToString() + '\n';
            }

            // Last row has sum and date (not to logical order);
            item = items.Last();
            if (item != null)
                result += "(סה\"כ) " + item.purchaseDate + ", " + item.supplierName + "," + item.dealSum + "," + item.paymentSum +  '\n';

            return result;
        }

        public T GetInfo<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(
                    GetInfoAsString()
                    , typeof(T));
            }
            else if (typeof(T) == typeof(int)) {
                return (T)Convert.ChangeType(
                    items.Count
                    ,typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(
                    CreditCardValid
                    , typeof(T));
            }

            throw new NotImplementedException("Cant convert CreditPurchases to object " + typeof(T).ToString());
        }

        // I only care about the four cells at the start of each row.
        const int importantcells = 4;

        string titlepurchaseDate;
        string titlesupplierName;
        string titledealSum;
        string titlepaymentSum;
        List<CreditPurchase> items = new List<CreditPurchase>();

        private string reverseString(string str)
        {
            string result = "";
            foreach (char c in str)
                result = c + result;
            return result;
        }

        bool CreditCardValid; // Exist and in the range
        public void Mine(HtmlDocument mainDocument)
        {
            CreditCardValid = (bool)mainDocument.InvokeScript("__isIndexInbound");

            // Start new instance
            titlepurchaseDate = "";
            titlesupplierName   = "";
            titledealSum        = "";
            titlepaymentSum     = "";
            items.Clear();

            // First we find the title row:
            HtmlElement row = null;
            foreach (HtmlElement tablerow in mainDocument.GetElementsByTagName("tr"))
            {
                if (tablerow.GetAttribute("className") == "DetailsTDParameterHeaderHigh")
                {
                    row = tablerow;
                    break;
                }
            }

            // Get the titles
            if (row != null)
            {
                HtmlElementCollection cells = row.GetElementsByTagName("th"); // th for header
                // Title

                titlepurchaseDate = cells[0].InnerText;
                titlesupplierName = cells[1].InnerText;
                titledealSum = cells[2].InnerText;
                titlepaymentSum = cells[3].InnerText;

                row = row.NextSibling;
            }

            // Move to the next row while you can
            while (row != null)
            {
                HtmlElementCollection cells = row.GetElementsByTagName("td"); // td for data

                items.Add(new CreditPurchase()
                {
                    purchaseDate =      cells[0].InnerText,
                    supplierName  =     reverseString(cells[1].InnerText),
                    dealSum =           cells[2].InnerText,
                    paymentSum =        cells[3].InnerText
                });

                row = row.NextSibling;
            }

        }
    }
}
