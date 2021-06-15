using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserLoginRegistration.HelperClasses
{

    public class BrowserDetector
    {
        private readonly Dictionary<string, string> browsers = new Dictionary<string, string>
                                                  {
                                                      {
                                                          "firefox", "Mozilla Firefox"
                                                      },
                                                      {
                                                          "chrome", "Google Chrome"
                                                      },
                                                      {
                                                          "iexplore", "Internet Explorer"
                                                      },
                                                      {
                                                          "MicrosoftEdgeCP", "Microsoft Edge"
                                                      }
                                                      // add other browsers
                                                  };

        public bool BrowserIsOpen()
        {
            return Process.GetProcesses().Any(this.IsBrowserWithWindow);
        }

        private bool IsBrowserWithWindow(Process process)
        {
            return this.browsers.TryGetValue(process.ProcessName, out var browserTitle) && process.MainWindowTitle.Contains(browserTitle);
        }
        internal string GetSystemDefaultBrowser()
        {
            string name = string.Empty;
            RegistryKey regKey = null;

            try
            {
                //set the registry key we want to open
                regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

                //get rid of the enclosing quotes
                name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

                //check to see if the value ends with .exe (this way we can remove any command line arguments)
                if (!name.EndsWith("exe"))
                    //get rid of all command line arguments (anything after the .exe must go)
                    name = name.Substring(0, name.LastIndexOf(".exe") + 4);

            }
            catch (Exception ex)
            {
                name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, this.GetType());
            }
            finally
            {
                //check and see if the key is still open, if so
                //then close it
                if (regKey != null)
                    regKey.Close();
            }
            //return the value
            return name;

        }
    }
}
