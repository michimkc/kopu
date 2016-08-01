using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

/*
 * To Do
 * 
 * Add an interface for Scripts
 *  They take over the input and output
 *  Only one script can run at a time
 *  When they close they give back the resources they took
 *  The script object is deleted.
 * 
 * */

namespace kopu
{
    public interface IKopu
    {
        void Output_NewLine(string pLine);
        void Output_Clear();

        void StartDragEvent(Action<string> pListener);
        void StopDragEvent();

        void Input_TakeOver(Action<KeyEventArgs> pListener);
        void Input_GiveBack();

        void ProcessDone(string ScriptName);

        TextBox GetInputBox();
    }

    public partial class Kopu : Form, IKopu
    {
        
        SimpleParser myParser;
        public Action<string> dragDropListener= null;
        public Action<KeyEventArgs> InputListener = null;
        bool configSingleRun = true; // bool to determine whether kopu closes after every command.

        public Kopu()
        {
            InitializeComponent();
            myParser = new kopu.SimpleParser(this);
            dragDropListener = null;
            InputListener = null;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
             
        }

        public void Output_NewLine(string pLine)
        {
            // add a new line to the history textbox
            historyText.AppendText(pLine + "\r\n");
        }

        public void Input_TakeOver(Action<KeyEventArgs> pListener)
        {
            if (InputListener != null)
            {
                Output_NewLine("Kopu: Error in Input_TakeOver. Tried to assign listener, but a listener already exists.");
                Output_NewLine("Kopu: New listener replaces old one.");
            }

            InputListener = pListener;

        }

        public void Input_GiveBack()
        {
            InputListener = null;
        }

        public TextBox GetInputBox()
        {
            return txtBox;
        }

        public void StartDragEvent(Action<string> pListener)
        {
            if(dragDropListener != null)
            {
                // someone is already listening
                Output_NewLine("Kopu: Error in StartDragEvent. Tried to assign listener, but a listener already exists.");
                Output_NewLine("Kopu: New listener replaces old one.");
            }
            dragDropListener = pListener;
            historyText.AllowDrop = true;
        }

        public void StopDragEvent()
        {
            dragDropListener = null;
            historyText.AllowDrop = false;
        }

        public void ProcessDone(string sProcess)
        {
            // called by processes when they finish.
            if (configSingleRun)
            {
                Application.Exit();
            }
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (InputListener == null) // if someone else isn't listening
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string inputText = txtBox.Text;
                    txtBox.Text = string.Empty;
                    Output_NewLine("> " + inputText);

                    // make sure there actually was input
                    if (!string.IsNullOrWhiteSpace(inputText))
                    {
                        myParser.ProcessCommand(inputText);
                    }

                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Application.Exit();
                }
            }
            else
            {
                // someone is listening. Pass the info through.
                InputListener(e);
            }
            
        }

        private void historyText_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }


        private void HistoryText_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (dragDropListener != null)
            {
                dragDropListener(FileList[0]);
            }
            
            
        }

        private void HistoryText_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public void Output_Clear()
        {
            historyText.Text = string.Empty;
        }

    }


}
