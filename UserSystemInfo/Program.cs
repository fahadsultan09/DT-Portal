using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace UserSystemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            string processorID = string.Empty;
            string HostName = string.Empty;
            string MACAddress = string.Empty;
            int i = 1;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            List<string> MacAddressList = NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetPhysicalAddress().ToString()).Distinct().ToList();

            foreach (ManagementObject mo in moc)
            {
                processorID = mo.Properties["processorID"].Value.ToString();
                break;
            }
            foreach (ManagementObject mo in moc)
            {
                HostName = mo.Properties["SystemName"].Value.ToString();
                break;
            }

            Console.WriteLine("Processor Id: " + processorID);
            Console.WriteLine("host Name: " + HostName);
            foreach (var item in MacAddressList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    Console.WriteLine("Adapter MAC Address {0}: {1}", i++, item);
                }
            }
            Console.ReadLine();
        }
        public static string GetMacAddressUsedByIp(string ipAddress)
        {
            var ips = new List<string>();
            string output;

            try
            {
                // Start the child process.
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;

                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "ipconfig";
                p.StartInfo.Arguments = "/all";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

            }
            catch
            {
                return null;
            }

            // pattern to get all connections
            var pattern = @"(?xis) 
            (?<Header>
                 (\r|\n) [^\r]+ :  \r\n\r\n
            )
            (?<content>
                .+? (?= ( (\r\n\r\n)|($)) )
            )";

            List<Match> matches = new List<Match>();

            foreach (Match m in Regex.Matches(output, pattern))
                matches.Add(m);

            var connection = matches.Select(m => new
            {
                containsIp = m.Value.Contains(ipAddress),
                containsPhysicalAddress = Regex.Match(m.Value, @"(?ix)Physical \s Address").Success,
                content = m.Value
            }).Where(x => x.containsIp && x.containsPhysicalAddress)
            .Select(m => Regex.Match(m.content, @"(?ix)  Physical \s address [^:]+ : \s* (?<Mac>[^\s]+)").Groups["Mac"].Value).FirstOrDefault();

            return connection;
        }
    }
}
