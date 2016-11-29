using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.Validate
{
    class CreditListLandingPageValidate : IValidate
    {
        bool isValid = false;
        public void Init(HtmlDocument mainDocument)
        {
            // Not trival but not complex validation.
            foreach (HtmlElement row in mainDocument.GetElementsByTagName("ul"))
            {
                if (row.GetAttribute("className") == "RightMenu") // If right menu exists
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
