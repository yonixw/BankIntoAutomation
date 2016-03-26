using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonyDataMacro.InfoMine
{
    public interface IInfoMine
    {
        void Mine(HtmlDocument mainDocument);
        T GetInfo<T>();
    }
}
