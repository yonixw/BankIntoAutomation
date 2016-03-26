using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.Validate
{
    public interface IValidate
    {
        void Init(HtmlDocument mainDocument);
        bool IsValid();
    }
}
