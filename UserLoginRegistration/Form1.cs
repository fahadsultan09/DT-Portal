using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using UserLoginRegistration.HelperClasses;
using UserLoginRegistration.Model;

namespace UserLoginRegistration
{
    public partial class Form1 : Form
    {
        readonly string AppUrl = string.Empty;
        readonly List<string> MacAddresses = new List<string>();
        public Form1()
        {
            InitializeComponent();
            AppUrl = ConfigurationManager.AppSettings["AppUrl"];
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> MacAddressList = NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetPhysicalAddress().ToString()).Distinct().ToList();

            foreach (var item in MacAddressList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    MacAddresses.Add(item);
                }
            }
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string GetPath = KnownFolders.GetPath(KnownFolder.Desktop);
            string processorId = string.Empty;
            string HostName = string.Empty;
            string MACAddress = string.Empty;
            int i = 1;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            List<string> MacAddressList = NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetPhysicalAddress().ToString()).Distinct().ToList();

            foreach (ManagementObject mo in moc)
            {
                processorId = mo.Properties["processorID"].Value.ToString();
                break;
            }
            foreach (ManagementObject mo in moc)
            {
                HostName = mo.Properties["SystemName"].Value.ToString();
                break;
            }
            DriveInfo[] driveInfo = DriveInfo.GetDrives();
            char drive = 'c';
            if (driveInfo.Count() > 1)
            {
                drive = driveInfo[1].Name[0];
            }
            else
            {
                drive = driveInfo[0].Name[0];
            }
            //StreamWriter File = new StreamWriter(drive + @":\Registration.txt");
            StreamWriter File = new StreamWriter(GetPath + "\\Registration.txt");
            File.Write("Processor Id: " + processorId + Environment.NewLine);
            File.Write("Host Name: " + HostName + Environment.NewLine);
            foreach (var item in MacAddressList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    File.Write("MAC Address {0}: {1}", i++, item + Environment.NewLine);
                }
            }
            File.Close();
            MessageBox.Show("File created successfully on your desktop " + GetPath + "\\Registration.txt");
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                lblError.Text = string.Empty;
                if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Text))
                {
                    Cursor = Cursors.Default;
                    lblError.Text = "Please enter username and password.";
                    return;
                }
                lblError.Text = string.Empty;
                btnLogin.Enabled = false;
                string ResetPassword = ConfigurationManager.AppSettings["ResetPassword"];
                JsonResponse jsonResponse = Request.Login(txtUserName.Text, txtPassword.Text, MacAddresses, AppUrl);

                if (!jsonResponse.Status && jsonResponse.Message == "Please change default password.")
                {
                    ForgotPassword forgotPasswordForm = new ForgotPassword(jsonResponse, txtUserName.Text, AppUrl);
                    txtUserName.Text = string.Empty;
                    txtPassword.Text = string.Empty;
                    lblError.Text = string.Empty;
                    forgotPasswordForm.ShowDialog();
                }
                if (jsonResponse.Status)
                {
                    User user = new User()
                    {
                        UserName = txtUserName.Text,
                        Password = txtPassword.Text,
                        MacAddresses = string.Join(",", MacAddresses),
                        IsDistributor = true,
                        UpdatedDate = DateTime.Now,
                    };
                    string AccessToken = JsonConvert.SerializeObject(user);
                    OpenBrowser(AppUrl + "Login/Index?AccessToken=" + EncryptDecrypt.Encrypt(AccessToken));
                    this.Close();
                }
                else
                {
                    btnLogin.Enabled = true;
                    lblError.Text = jsonResponse.Message;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The remote name could not be resolved") || ex.Message.Contains("Unable to connect to the remote server"))
                {
                    MessageBox.Show("Your internet connection is not working properly, please check your internet connection");
                }
                else
                {
                    MessageBox.Show("Error occured while login, please try again.");
                }
                btnLogin.Enabled = true;
            }
            Cursor = Cursors.Default;
        }
        private void btnUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private void btnPassword_TextChanged(object sender, EventArgs e)
        {

        }
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start("chrome", url);
            }
            catch
            {
                MessageBox.Show("Please installed chrome browser to run distributor portal.");
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //{
                //    url = url.Replace("&", "^&");
                //    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                //{
                //    Process.Start("xdg-open", url);
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                //{
                //    Process.Start("open", url);
                //}
                //else
                //{
                //    throw;
                //}
            }
        }
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(this, new EventArgs());
            }
        }
    }
}
