using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ricompany.ControlValueSaver
{
    public interface IControl
    {
        string Text { get; set; }
        string Name { get; set; }

    }
}
