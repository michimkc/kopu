using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kopu
{
    public interface IScript
    {
        string GetScriptName(); // the name of the script

        void Main(string pParams); // this is run when the script is called
        void MainHelp(); // this prints the help text
        void CloseScript(); // closes the script.
        void InputWrapper(KeyEventArgs e); // this is the delegate for taking control of the input box
        void DragDrop_HistoryTxt(string pLink); // this is the delegate for dragging and dropping files onto the history textbox

    }
}
