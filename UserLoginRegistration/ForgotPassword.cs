using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using UserLoginRegistration.HelperClasses;
using UserLoginRegistration.Model;

namespace UserLoginRegistration
{
    public partial class ForgotPassword : Form
    {
        string AppUrl = string.Empty;
        List<string> MacAddresses = new List<string>();
        public ForgotPassword()
        {
            InitializeComponent();
            AppUrl = ConfigurationManager.AppSettings["AppUrl"];
        }
        public ForgotPassword(JsonResponse jsonResponse, string txtUserName,string appUrl)
        {
            InitializeComponent();
            lblResponseError.Text = jsonResponse.Message;
            lblUserName.Text = txtUserName;
            AppUrl = appUrl;
        }
        private void ForgotPassword_Load(object sender, EventArgs e)
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
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                lblError.Text = "Please enter password and confirm password.";
                return;
            }
            if (txtPassword.Text.Trim() == "Sami@1234" && txtConfirmPassword.Text.Trim() == "Sami@1234")
            {
                lblError.Text = "Password and confirm password should be match.";
                return;
            }
            JsonResponse jsonResponse = Request.ChangePassword(lblUserName.Text, txtPassword.Text, MacAddresses.ToList(), AppUrl);
            if (jsonResponse.Status)
            {
                MessageBox.Show("Password changed successfully");
                this.Close(); 
            }
        }
    }
}
