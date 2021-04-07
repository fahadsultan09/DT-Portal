using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

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
    }
}
