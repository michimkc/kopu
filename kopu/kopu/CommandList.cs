using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;


namespace kopu
{

    public class CommandList
    {
        private readonly IKopu myInterface;

        public CommandList(IKopu pInterface)
        {
            myInterface = pInterface;
        }

        public void g(string pSearchString)
        {
            string searchString = pSearchString;
            searchString = Regex.Replace(searchString, "\\s+", "+");
            System.Diagnostics.Process.Start("http://google.com#q=" + searchString);
            myInterface.ProcessDone("google");
        }

        public void app(string pApp)
        {

            KopuApp newApp = new KopuApp(myInterface);
            newApp.Main(pApp);

        }

        public void clear()
        {
            myInterface.Output_Clear();
        }

        public void quickupdate()
        {
            // copies the kopu.exe from my visual studio folder to my projects folder
            string vsFolder = "C:\\Users\\may\\Documents\\Visual Studio 2015\\Projects\\kopu\\kopu\\kopu\\bin\\Debug\\kopu.exe";
            string projFolder = "C:\\Users\\may\\Documents\\PROJECTS\\SCRIPTING\\Kopu Project\\kopu.exe";
            if (File.Exists(vsFolder))
            {
                File.Copy(vsFolder, projFolder, true);
                myInterface.Output_NewLine("quickupdate: Update complete");
                myInterface.ProcessDone("quickupdate");
            } else
            {
                myInterface.Output_NewLine("quickupdate: Error - File kopu.exe does not exist in default location");
            }
        }
    }

}