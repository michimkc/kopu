using System;
using System.IO;
using System.Reflection;

public class KopuApp
{
	public KopuApp()
	{
	}

    public void Main(string pApp)
    {
        string appString = pApp;

        if (appString[0] == '-') // this is an internal command
        {
            if (appString[1] == 'a') // add
            {
                // add a string to the file
            }

        }
        else
        {
            myOutputTextbox.AppendText("App: Unknown command: -" + appString[1] + "/r/n");
        }

        // look in the link file for the string link
    }
}
