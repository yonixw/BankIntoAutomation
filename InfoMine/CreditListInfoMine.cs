﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.InfoMine
{
    class CreditPurchase
    {
        public string purchaseDate;
        public string supplierName;
        public string dealSum;
        public string paymentSum;

    }

    public class CreditListInfoMine : IInfoMine
    {
        private string GetInfoAsString()
        {
            string result = "";
            for (int i = Math.Max(0, items.Count - 10); i< items.Count; i++)
            {
                CreditPurchase item = items[i];
                result +=
                    "[" + i + "]\n" +
                    titlepurchaseDate + ": " + item.purchaseDate + "\n" +
                    titlesupplierName + ": " + item.supplierName + "\n" +
                    titledealSum + ": " + item.dealSum + "\n" +
                    titlepaymentSum + ": " + item.paymentSum + "\n\n" 
                    ;
            }
            return result;
        }

        public T GetInfo<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(GetInfoAsString(), typeof(T));
            }

            throw new NotImplementedException("Cant convert CreditPurchases to object");
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

        public void Mine(HtmlDocument mainDocument)
        {
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
