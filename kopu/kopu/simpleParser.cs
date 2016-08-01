using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;

namespace kopu
{

    public class SimpleParser
    {

        // Something I wrote to parse my command strings for me.
        // One day maybe I'll learn enough to make a proper one? seems like a lot of work...

        private char[] delimiterChar = { ' ' };
        // private char[] delimiterChars = { ' ', ',', '.'}; // for allowing multiple delimiters
        IKopu myInterface;

        public SimpleParser(IKopu pInterface)
        {
            myInterface = pInterface;
        }

        public void ProcessCommand(string pCommand)
        {
            Type commandListType = typeof(CommandList);

            if (pCommand != null)
            {
                String[] split = pCommand.Split(delimiterChar, 2);
                string command = split[0];
                string commandText = string.Empty;

                if (split.Length > 1)
                {
                    commandText = split[1];
                }

                MethodInfo method = commandListType.GetMethod(command);
                if (method != null) // this is a valid command
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    CommandList myCommandList = new CommandList(myInterface);

                    if (parameters.Length == 0) // if the command takes no parameters
                    {
                        method.Invoke(myCommandList, null);
                    }
                    else
                    {
                        object[] parametersArray = new object[] { commandText };
                        method.Invoke(myCommandList, parametersArray);
                    }

                } else
                {
                    // not a valid command
                    myInterface.Output_NewLine("Unknown command: " + command);
                }
                
            }
            

        }
    }

}