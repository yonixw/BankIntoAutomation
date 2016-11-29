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
        }

        public bool IsValid()
        {
            return isValid;
        }
    }
}
