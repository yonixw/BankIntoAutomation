using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.Validate
{
    class CreditListValidate : IValidate
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
        }

        public bool IsValid()
        {
            return isValid;
        }
    }
}
