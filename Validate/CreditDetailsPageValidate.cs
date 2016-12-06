using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.Validate
{
    class CreditDetailsPageValidate : IValidate
    {
        bool isValid = false;
        public void Init(HtmlDocument mainDocument)
        {
            // Not trival but not complex validation.
            foreach (HtmlElement row in mainDocument.GetElementsByTagName("tr"))
            {
                if (row.GetAttribute("className") == "DetailsTDParameterHeaderHigh")
                {
                    isValid = true;
                    break;
                }
            }

            // Note: Dont use card list <select> element, it doesn't exit with one card

            Utils.AddScript(MonyDataMacro.Properties.Resources.CreditListJSCode, mainDocument);
            HtmlElement cardList = mainDocument.GetElementById("CardIndex");
            if (cardList != null) { 
                if (!(bool)mainDocument.InvokeScript("__isIndexInbound")) {
                    // If list exists and we our out of bound, stop and sy not valid.
                    Console.WriteLine("Card list exists but out of bound");
                    isValid = false;
                }
                else if (!isValid) {
                    // If no purchase, no table.
                    // But still need to go to next one.
                    // So allow it if index is in bound, and IMine will ignore due to not finding <tr>
                    isValid = true; 
                }
            }

            
        }

        public bool IsValid()
        {
            return isValid;
        }
    }
}
