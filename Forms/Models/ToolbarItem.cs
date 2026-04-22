using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forms.Models
{
    //esta clase no se esta usando
    public class ToolbarItem
    {
        public string Text { get; set; }
        public ICommand command { get; set; }
        public string IconImageSource { get; set; }
    }
}
