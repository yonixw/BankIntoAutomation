using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.InfoMine
{
    class BankMainPageItem
    {
        public string Category;
        
        public string ActivityType;
        public string MoneyLeft;
        public string Date;

        public override string ToString()
        {
            return  ActivityType + ", " + Date + ", " + MoneyLeft;
        }
    }


    class BankMainPageInfoMain : IInfoMine
    {
        List<BankMainPageItem> items = new List<BankMainPageItem>();

        public T GetInfo<T>()
        {
            Type ReturnType = typeof(T);

            if (ReturnType == typeof(string))
            {
                return (T)Convert.ChangeType(GetInfoAsString(), typeof(T));
            }
            else if(ReturnType == typeof(List<BankMainPageItem>))
            {
                return (T)Convert.ChangeType(GetInfoAsList(), typeof(T));
            }

            throw new NotImplementedException("Cannot return this type for BankMainPageItem");
        }

        private string GetInfoAsString() // If T is string
        {
            //string result = "";
            //foreach (BankMainPageItem item in items)
            //{
            //    result += item.ToString();
            //}
            //return result;

            if (items.Count > 1)
                return items[0].ToString();

            return "";
        }

        private List<BankMainPageItem> GetInfoAsList()
        {
            return items;
        }

        public void Mine(HtmlDocument mainDocument)
        {
            // Support multi-time mining by clearing old results.
            items.Clear();

            HtmlElement table = mainDocument.GetElementById("div3")
                .GetElementsByTagName("table")[0]
                .GetElementsByTagName("tbody")[0]
                ;

            string currentCategory = "";
            HtmlElementCollection rows = table.GetElementsByTagName("tr");

            foreach (HtmlElement row in rows)
            {
                HtmlElementCollection cells = row.GetElementsByTagName("td");

                if (cells[0].GetAttribute("colspan") == "4")
                {
                    // It's a title of the category
                    currentCategory = row.InnerText;
                }
                else if (cells.Count == 2 && cells[1].GetAttribute("colspan") == "3")
                {
                    // It is a sum of category
                    items.Add(
                        new BankMainPageItem()
                        {
                            Category = currentCategory,
                            ActivityType = cells[0].InnerText,
                            MoneyLeft = cells[1].InnerText
                        }
                        );
                }
                else if(cells.Count == 4 && row.GetAttribute("className") != "table_header")
                {
                    // A normal item
                    items.Add(
                        new BankMainPageItem()
                        {
                            Category = currentCategory,
                            ActivityType = cells[0].InnerText,
                            MoneyLeft = cells[1].InnerText,
                            Date = cells[2].InnerText,
                            // Last cell is availible operations.
                        }
                        );
                }
            }

        }
    }
}
