using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.Validate
{

    class BankMainPageValidate : IValidate
    {
        // Validate if the main page of the bank has info.
        // In this case, just check for id="div3"

        bool valid = false;

        public void Init(HtmlDocument mainDocument)
        {
            if (mainDocument.GetElementById("div3") != null )
            {
                valid = true;
            }
        }

        public bool IsValid()
        {
            return valid;
        }
    }
}
