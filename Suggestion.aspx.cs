using System;
using System.Web.UI;
using System.Net.Mail;

namespace InventorySearch
{
    public partial class Suggestion : System.Web.UI.Page
    {
        private LogManager lm = LogManager.GetInstance();
        protected void Page_Load(object sender, EventArgs e)
        {
            lblItemNum.Text = Request.QueryString["item"].Trim();
            Page.Title = "New Descripition for Item# " + lblItemNum.Text;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string toAddress, fromAddress, fromName, subject, content, phone, description, department;

                if (txtPhone.Text == "") { phone = ""; } else { phone = txtPhone.Text.Trim(); }

                toAddress = "dlrjones@uw.edu";
                fromAddress = txtEmail.Text.Trim();
                fromName = txtName.Text.Trim();
                department = txtDepartment.Text.Trim();
                description = txtDescription.Text.Trim();

                if (fromAddress.Length > 0 && fromName.Length > 0 && department.Length > 0 && description.Length > 0)
                {
                    subject = "Web Item Catalog - Item# " + lblItemNum.Text;
                    content = "Alternate Description Request" + Environment.NewLine + "from:  " + fromName + Environment.NewLine + Environment.NewLine +
                               "Department:  " + department + Environment.NewLine + Environment.NewLine + "Phone:  " + phone + Environment.NewLine +
                             Environment.NewLine +
                              "Item#:  " + lblItemNum.Text + Environment.NewLine + Environment.NewLine + 
                              "Add this as a new description: " + Environment.NewLine + description;
                    MailMessage mail = new MailMessage(fromAddress, toAddress, subject, content);
                    SmtpClient smtp = new SmtpClient("mcis.washington.edu");
                    smtp.Send(mail);
                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtDepartment.Text = "";
                    txtPhone.Text = "";
                    txtDescription.Text = "";
                }
            }
            catch (Exception ex)
            {
                lm.Write("Suggestion.btnSubmit_Click: " + ex.Message);
            }
        }
    }
}