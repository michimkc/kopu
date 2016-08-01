using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms; // for TextBox
using System.Diagnostics; // for Process

namespace kopu
{
    public class KopuApp : IScript
    {
        private string myScriptName = "KopuApp";
        private string fileKeywordsPath = "app-links.txt";
        private readonly IKopu myInterface;

        private struct keyword
        {
            public string shortcutName;
            public string linkPath;
            public string friendlyName;
        }

        public KopuApp(IKopu pInterface)
        {
            myInterface = pInterface;
        }

        public string GetScriptName()
        {
            return myScriptName;
        }

        public void CloseScript()
        {

        }
        
        string[] helpString =
        {
                "Help - Kopu App",
                ".SYNOPSIS",
                "Shortcut app for calling programs. Shortcuts aliases are user defined, shortcut paths are entered by drag-and-dropping a shortcut file or exe onto the kopu main window.",
                ".USAGE",
                "\"App <shortcut>\" - Runs the pre-defined shortcut.",
                "\"App -a <shortcut>\" - Adds a new shortcut. The program will prompt the user to drag a shortcut or exe onto the main window, which will assign the link.",
                "\"App -l\" - Lists all the shortcuts currently defined.",
                "\"App -l -v\" - (Verbose) Lists all the shortcuts, including their paths.",
                "\"App -d <shortcut>\" - Deletes the specified shortcut. The program will prompt the user to confirm that they want to delete it."
                //"\"App -r <shortcut>\" - Replaces the link to an existing shortcut. The program will prompt the user to drag the new link in to reassing it."
        };

        keyword curKeyword;
        bool bAssigningName = false; // flag to determine when the input is listening for friendly name assignment. 

        public void Main(string pApp)
        {
            string appString = pApp;
            string commandString = string.Empty;

            bAssigningName = false;
            curKeyword = new keyword();
            ClearKeyword();

            if (!string.IsNullOrWhiteSpace(appString))
            {
                
                if (appString[0] != '-')
                {
                    string appLink = "";

                    //go into the link file and try to find the line with the shortcut.
                    if (!File.Exists(fileKeywordsPath))
                    {
                        myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                        return;
                    }

                    using (StreamReader sr = new StreamReader(fileKeywordsPath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            if (sr.Peek() == '#') // ignore comment lines
                            {
                                sr.ReadLine();
                            }
                            else
                            {
                                string nextLine = sr.ReadLine();
                                if (!string.IsNullOrEmpty(nextLine))
                                {
                                    string[] splitLine = nextLine.Split(new char[] { ',' });
                                    if (splitLine.Length == 3 && String.Equals(splitLine[0], appString, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // the shortcut exists
                                        appLink = splitLine[2];
                                        string stringShortcut = splitLine[1];
                                        if (string.IsNullOrEmpty(stringShortcut))
                                            stringShortcut = splitLine[0];
                                        myInterface.Output_NewLine("App: Running shortcut - " + stringShortcut);
                                        RunApp(appLink);
                                        return;
                                    }
                                }

                            }
                        }


                    }

                    // if after reading through the whole keyword file, we didn't find the shortcut
                    // it means it doesnt exist.
                    myInterface.Output_NewLine("App: Error - Shortcut \'" + appString + "\' is not defined.");
                    return;
                }

                // okay, so it wasn't a regular shortcut command. Let's see what kind of command it is.

                String[] split = appString.Split(new char[] { ' ' }, 2);

                // separate the first item
                commandString = split[0];
                if (commandString == "-a") // add
                {
                    if (split.Length == 2) // the command came as "app -a <string>" 
                    {
                        string shortcutString = split[1];
                        if (shortcutString.Contains(","))
                        {
                            myInterface.Output_NewLine("App: Illegal character , in command.");
                            return;
                        } else if (shortcutString.Contains(" "))
                        {
                            myInterface.Output_NewLine("App: Illegal character <space> in command.");
                            return;
                        }

                        if (ShortcutExists(shortcutString))
                        {
                            myInterface.Output_NewLine("App: Error - Shortcut \'" + shortcutString + "\' already exists.");
                            return;
                        }

                        myInterface.Output_NewLine("App: Please drag the file or shortcut into this screen that you want to link.");
                        myInterface.Input_TakeOver(delegate (KeyEventArgs e) { InputWrapper(e); });
                        curKeyword.shortcutName = shortcutString; // set the shortcut string.
                        // turn on the drag event 
                        myInterface.StartDragEvent(delegate (string s) { DragDrop_HistoryTxt(s); });

                    } else // there were not enough or more than one string after -a
                    {
                        myInterface.Output_NewLine("App: Invalid syntax. Please enter a shortcut name with no spaces. (ie. App -a MyShortcut)");
                        return;
                    }

                } else if (commandString == "-l")
                {
                    
                    if (split.Length > 1 && split[1] == "-v") // verbose
                    {
                        ListShortcuts(1);
                    }
                    else
                    {
                        // list things
                        ListShortcuts();
                    }
                } else if (commandString == "-d")
                {
                    if (split.Length == 2) // the command came as "app -d <string>" 
                    {
                        string shortcutString = split[1];
                        if (shortcutString.Contains(","))
                        {
                            myInterface.Output_NewLine("App: Illegal character ',' in command.");
                            return;
                        }

                        DeleteKeyword(shortcutString);

                    }
                    else // there were not enough or more than one string after -d
                    {
                        myInterface.Output_NewLine("App: Invalid syntax. Please enter a shortcut name with no spaces. (ie. App -d MyShortcut)");
                        return;
                    }
                }
                else
                {
                    myInterface.Output_NewLine("App: Unknown command: -" + appString[1]);
                    return;
                }
                
            }
            else
            {
                // no entry
                MainHelp();
            }

            
        }
        
        private void RunApp(string pPath)
        {
            if (!File.Exists(pPath))
            {
                myInterface.Output_NewLine("App: Error trying to find the file " + pPath + ". Please double check that the file exists.");
                return;
            }

            Process proc = new Process();
            proc.StartInfo.FileName = pPath;
            proc.Start();
            myInterface.ProcessDone(GetScriptName());
        }

        public void DragDrop_HistoryTxt(string pLink)
        {
            // This occurs after a "-a" command.
            // KopuApp takes over the input box so the user cannot enter anything else.

            // when the user drops a link into the historybox, this method is called.

            if (!KeywordValid(1, "DragDrop_HistoryTxt")) // first check if a shortcut name has been assigned.
            {
                return;
            }

            // add the link to the file with the string determined in the previous step
            curKeyword.linkPath = pLink;

            // turn off drag and dropping
            myInterface.StopDragEvent();
            // then call assign name to let the user add a friendly name to the shortcut.
            AssignName();
        }

        private void DeleteKeyword(string pShortcut)
        {
            //go into the link file and try to find the line with the shortcut.
            if (!File.Exists(fileKeywordsPath))
            {
                myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                return;
            }

            if (string.IsNullOrEmpty(pShortcut))
            {
                myInterface.Output_NewLine("App: Error: Please enter the shortcut that you want to delete. (ie. app -d myShortcut)");
                return;
            }
            string fileTempPath = fileKeywordsPath + ".temp";
            bool foundString = false;
            using (StreamReader sr = new StreamReader(fileKeywordsPath))
            {
                using (StreamWriter sw = new StreamWriter(fileTempPath))
                {
                    
                    while (sr.Peek() >= 0)
                    {
                        
                        string nextLine = sr.ReadLine();
                        string[] splitLine = nextLine.Split(new char[] { ',' });
                        if (!foundString && splitLine.Length == 3 && String.Equals(splitLine[0], pShortcut, StringComparison.OrdinalIgnoreCase))
                        {
                            // the shortcut exists skip this line
                            foundString = true;
                        } else
                        {
                            sw.WriteLine(nextLine);
                        }
                                                
                    }

                }
            }
            if (!foundString) // we didnt find the requested command
            {
                myInterface.Output_NewLine("App: Error deleting shortcut. \'" + pShortcut + "\' is not currently a shortcut.");
                File.Delete(fileTempPath);
            } else
            {
                File.Delete(fileKeywordsPath);
                File.Move(fileTempPath, fileKeywordsPath);
                myInterface.Output_NewLine("App: Shortcut \'" + pShortcut + "\' deleted successfully.");
            }

        }

        private void AssignName()
        {
            if (!KeywordValid(2, "AssignName"))
            {
                return;
            }

            // this occurs after DragDrop_HistoryTxt()

            myInterface.Output_NewLine("App: (Optional) Please input a short name for this command. For example, \"League of Legends\".");
            bAssigningName = true;
        }

        public void InputWrapper(KeyEventArgs e)
        {
            // delegate for when KopuApp takes over the console.

            // if we are assigning a name, we let the user type in a friendly name and then wait for enter key.
            if (bAssigningName)
            {
                if (!KeywordValid(2, "InputWrapper"))
                {
                    return;
                }


                if (e.KeyCode == Keys.Enter)
                {
                    string inputText = myInterface.GetInputBox().Text;
                    myInterface.GetInputBox().Text = string.Empty;

                    if (inputText.Contains(","))
                    {
                        myInterface.Output_NewLine("App: Illegal character ','. Please input another name.");
                        return;
                    }

                    myInterface.Output_NewLine("> " + inputText);

                    // make sure there actually was input
                    if (string.IsNullOrWhiteSpace(inputText))
                    {
                        myInterface.Output_NewLine("App: No friendly name assigned.");
                        bAssigningName = false;
                        CreateShortcut(curKeyword);
                        ResetApp();
                    } else
                    {
                        // assign the friendly name
                        curKeyword.friendlyName = inputText;
                        bAssigningName = false;
                        CreateShortcut(curKeyword);
                        ResetApp();
                    }

                }
            }

            if (e.KeyCode == Keys.Escape)
            {
                ResetApp();
                myInterface.Output_NewLine("App: Command cancelled.");
            }
        }
            

        private void ClearKeyword()
        {
            curKeyword.shortcutName = string.Empty;
            curKeyword.friendlyName = string.Empty;
            curKeyword.linkPath = string.Empty;
        }

        public void MainHelp()
        {
            for (int i=0;i<helpString.Length-1;i++)
            {
                myInterface.Output_NewLine(helpString[i]);
            }

        }

        private bool KeywordValid(int pPhase, string callerName)
        {
            bool error = false;
            if (pPhase == 1) // check if shortcut string is assigned
            {
                if (string.IsNullOrEmpty(curKeyword.shortcutName))
                {
                    error = true;
                }
            } else if (pPhase == 2) // check if shortcut and path are assigned
            {
                if (string.IsNullOrEmpty(curKeyword.shortcutName) || string.IsNullOrEmpty(curKeyword.linkPath))
                {
                    error = true;
                }
            } else if (pPhase == 3) // check if all three values are assigned
            {
                if (string.IsNullOrEmpty(curKeyword.shortcutName) || string.IsNullOrEmpty(curKeyword.linkPath) || string.IsNullOrEmpty(curKeyword.friendlyName))
                {
                    error = true;
                }
            } else
            {
                myInterface.Output_NewLine("App: Error in KeywordValid. Illegal phase: " + pPhase);
                return false;
            }
            
            if (error)
            {
                // we should only be getting to assign a name after the first X steps. if these were bypassed for whatever
                // inane reason, we should throw an error and leave.
                myInterface.Output_NewLine("App: Error occured in " + callerName + ". Keyword is not assigned. Cannot complete command.");
                ResetApp();
                return false;
            }
            return true;
        }

        private void ResetApp()
        {
            // resets all values to default. Relinquishes all controls.
            ClearKeyword();
            myInterface.Input_GiveBack();
            myInterface.StopDragEvent();
            bAssigningName = false;
        }

        private void CreateShortcut(keyword pKeyword)
        {
            // input the keyword into the file.
            if (!File.Exists(fileKeywordsPath))
            {
                myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                return;
            }
            else
            {
                using (FileStream fs = new FileStream(fileKeywordsPath, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(pKeyword.shortcutName + "," + pKeyword.friendlyName + "," + pKeyword.linkPath + "\r\n");
                }

                string stringFriendly = string.Empty;
                if (!string.IsNullOrEmpty(pKeyword.friendlyName))
                    stringFriendly = " for " + pKeyword.friendlyName;

                myInterface.Output_NewLine("App: Shortcut \'" + pKeyword.shortcutName + "\' created" + stringFriendly + ".");
            }
        }


        private void ListShortcuts(int pFlags = 0)
        {
            // called by -l parameter.

            if (pFlags == 0) // no options
            {
                if (!File.Exists(fileKeywordsPath))
                {
                    myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                    return;
                }

                myInterface.Output_NewLine("Shortcut - Name");
                using (StreamReader sr = new StreamReader(fileKeywordsPath))
                {
                    while (sr.Peek() >= 0)
                    {

                        if (sr.Peek() == '#')
                        {
                            sr.ReadLine();
                        }
                        else
                        {
                            string nextLine = sr.ReadLine();
                            if (!string.IsNullOrEmpty(nextLine))
                            {
                                string[] splitLine = nextLine.Split(new char[] { ',' });
                                if (splitLine.Length != 3)
                                {
                                    myInterface.Output_NewLine("Invalid Line: \"" + nextLine + "\".");
                                }
                                else
                                {
                                    string splitTemp = splitLine[1];
                                    if (string.IsNullOrEmpty(splitTemp))
                                        splitTemp = "(undefined)";
                                    myInterface.Output_NewLine(splitLine[0] + " - " + splitTemp);
                                }
                            }
                        }
                    }
                }
            }
            else if (pFlags == 1)
            {
                // verbose
                if (!File.Exists(fileKeywordsPath))
                {
                    myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                    return;
                }

                using (StreamReader sr = new StreamReader(fileKeywordsPath))
                {
                    while (sr.Peek() >= 0)
                    {

                        if (sr.Peek() == '#')
                        {
                            sr.ReadLine();
                        }
                        else
                        {
                            string nextLine = sr.ReadLine();
                            if (!string.IsNullOrEmpty(nextLine))
                            {
                                string[] splitLine = nextLine.Split(new char[] { ',' });
                                if (splitLine.Length != 3)
                                {
                                    myInterface.Output_NewLine("App: Error reading keyword file.");
                                    myInterface.Output_NewLine("App: \"" + nextLine + "\" has invalid formatting.");
                                    myInterface.Output_NewLine("App: Skipping this line.");
                                }
                                else
                                {
                                    string splitTemp = splitLine[1];
                                    if (string.IsNullOrEmpty(splitTemp))
                                        splitTemp = "(undefined)";
                                    myInterface.Output_NewLine(splitLine[0] + ", " + splitLine[1] + ", " + splitLine[2]);
                                }
                            }
                        }
                    }
                }
                     
            } else // invalid 
            {
                myInterface.Output_NewLine("App: Invalid option for List Shortcuts");
            }

            
        }

        private bool ShortcutExists(string pShortcut)
        {
            //go into the link file and try to find the line with the shortcut.
            if (!File.Exists(fileKeywordsPath))
            {
                myInterface.Output_NewLine("App: Error: Couldn't find keyword file.");
                return false;
            }

            if (string.IsNullOrEmpty(pShortcut))
            {
                myInterface.Output_NewLine("App: Error: Please enter the shortcut that you want to delete. (ie. app -d myShortcut)");
                return false;
            }
            string fileTempPath = fileKeywordsPath + ".temp";
            bool foundString = false;
            using (StreamReader sr = new StreamReader(fileKeywordsPath))
            {
                

                    while (sr.Peek() >= 0)
                    {

                        string nextLine = sr.ReadLine();
                        string[] splitLine = nextLine.Split(new char[] { ',' });
                        if (!foundString && splitLine.Length == 3 && String.Equals(splitLine[0], pShortcut, StringComparison.OrdinalIgnoreCase))
                        {
                            // the shortcut exists skip this line
                            foundString = true;
                        }
                        
                    }

                
            }

            if (foundString) // we found the shortcut.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
