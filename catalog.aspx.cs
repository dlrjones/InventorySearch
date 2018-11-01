using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data.SqlClient;

public partial class catalog : System.Web.UI.Page
{
    protected static string pmmConnStr = ConfigurationManager.ConnectionStrings["pmm_devConnectionString"].ConnectionString;
    protected SqlConnection pmmConn = new SqlConnection(pmmConnStr);
    protected static string amcConnStr = ConfigurationManager.ConnectionStrings["amc_userConnectionString"].ConnectionString;
    protected SqlConnection amcConn = new SqlConnection(amcConnStr);
    public int xx = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            sdsItems.SelectCommand = "SELECT  DISTINCT t1.ITEM_NO, t1.ITEM_ID, t1.DESCR, t1.CTLG_NO, CONVERT(BIT,(SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) AS STOCKED, '$' + CONVERT(VARCHAR(11), CONVERT(MONEY, IVP.PRICE)) AS PRICE FROM ITEM AS t1 INNER JOIN ITEM_VEND IV ON t1.ITEM_ID = IV.ITEM_ID INNER JOIN ITEM_VEND_PKG IVP ON IV.ITEM_VEND_ID = IVP.ITEM_VEND_ID WHERE t1.STAT IN (1, 2) AND IV.CORP_ID = 1000 AND IV.SEQ_NO = 1 AND IVP.SEQ_NO = (SELECT MAX(SEQ_NO) MAX_SEQ_NO FROM ITEM_VEND_PKG WHERE IV.ITEM_VEND_ID = ITEM_VEND_ID) ORDER BY t1.ITEM_NO ";
            if (Request.QueryString["itemnum"] != null)
            {
                mpeItemInfo.Show();
                ViewState["itemNum"] = Request.QueryString["itemnum"].ToString();
                ViewState["view"] = "details";
                LoadItemInfo();
            }
            txtSearch.Focus();
            //Session["username"] = Request.ServerVariables["HTTP_PUBCOOKIE_USER"].Trim();
            Session["username"] = Request.ServerVariables["HTTP_PUBCOOKIE_USER"] != null;
        }
        else
        {
            if (ViewState["sql"] != null)
            {
                sdsItems.SelectCommand = ViewState["sql"].ToString();
            }
            else
            {
                sdsItems.SelectCommand = "SELECT  DISTINCT t1.ITEM_NO, t1.ITEM_ID, t1.DESCR, t1.CTLG_NO, CONVERT(BIT,(SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) AS STOCKED, '$' + CONVERT(VARCHAR(11), CONVERT(MONEY, IVP.PRICE)) AS PRICE FROM ITEM AS t1 INNER JOIN ITEM_VEND IV ON t1.ITEM_ID = IV.ITEM_ID INNER JOIN ITEM_VEND_PKG IVP ON IV.ITEM_VEND_ID = IVP.ITEM_VEND_ID WHERE t1.STAT IN (1, 2) AND IV.CORP_ID = 1000 AND IV.SEQ_NO = 1 AND IVP.SEQ_NO = (SELECT MAX(SEQ_NO) MAX_SEQ_NO FROM ITEM_VEND_PKG WHERE IV.ITEM_VEND_ID = ITEM_VEND_ID) ORDER BY t1.ITEM_NO ";
            }
            //if (ddlOption.SelectedIndex == 0)
            //{
            //    gvItems.DataSourceID = "sdsItems";
            //    gvItems.DataBind();
            //}
        }
        this.Form.DefaultButton = this.ibtnSearch.UniqueID.ToString();
    }

    //Sets the item page count based on user selection
    protected void ddlPageCount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gvItems.PageSize = Convert.ToInt32(ddlPageCount.SelectedValue);
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "ddlPageCount_SelectedIndexChanged: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Determines the item result count
    protected void sdsItems_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        try
        {
            litResults.Text = e.AffectedRows.ToString();
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "sdsItems_Selected: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Sets the items imageurl based on the Item Number
    protected string GetImageUrl(object itemNumber)
    {
        string url = null;
        try
        {
            if (File.Exists(Server.MapPath("~/catalog_images/" + itemNumber.ToString().Trim() + ".jpg")))
            {
                url = "~/catalog_images/" + itemNumber.ToString().Trim() + ".jpg";
            }
            else
            {
                url = "~/catalog_images/no_image_thumb.jpg";
            }
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "GetImageUrl: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
        return url;
    }

    //Calls the LoadItemInfo procedure
    protected void lbtnItemNum_Click(object sender, EventArgs e)
    {
        mpeItemInfo.Show();
        LinkButton ibtnItemNum = (LinkButton)sender;
        ViewState["itemNum"] = ibtnItemNum.CommandArgument.Trim();
        ViewState["view"] = "details";
        LoadItemInfo();
    }

    //Calls the LoadItemInfo procedure
    protected void imgItem_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton ibtnItemNum = (ImageButton)sender;
        ViewState["itemNum"] = ibtnItemNum.CommandArgument.Trim();
        ViewState["view"] = "photo";
        LoadItemInfo();
        mpeItemInfo.Show();
    }

    //Calls the BuildSearch procedure
    protected void ibtnSearch_Click(object sender, ImageClickEventArgs e)
    {
        BuildSearch();
        txtSearch.Focus();
    }

    //Builds the sql search statement based on user input
    protected void BuildSearch()
    {
        try
        {
            string sql = @"SELECT DISTINCT t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, CONVERT(BIT,
                     (SELECT COUNT(ITEM_ID) AS STOCK_COUNT
                     FROM ITEM_CC_ACCT_EXCP AS t2
                     WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) AS STOCKED, 
                     t1.DESCR2, ITEM_CORP_ACCT.EXP_ACCT_NO AS ACCTCTG, MFR.NAME, '$' + CONVERT(VARCHAR(11), CONVERT(MONEY, IVP.PRICE)) AS PRICE 
                     FROM ITEM AS t1 
                     INNER JOIN ITEM_CORP_ACCT ON t1.ITEM_ID = ITEM_CORP_ACCT.ITEM_ID AND t1.ITEM_IDB = ITEM_CORP_ACCT.ITEM_IDB 
                     INNER JOIN MFR ON t1.MFR_ID = MFR.MFR_ID AND t1.MFR_IDB = MFR.MFR_IDB
                     INNER JOIN ITEM_VEND IV ON t1.ITEM_ID = IV.ITEM_ID 
                     INNER JOIN ITEM_VEND_PKG IVP ON IV.ITEM_VEND_ID = IVP.ITEM_VEND_ID 
                     
                     WHERE t1.STAT IN (1, 2) AND IV.CORP_ID = 1000 AND IV.SEQ_NO = 1 
                     AND IVP.SEQ_NO = (SELECT MAX(SEQ_NO) MAX_SEQ_NO FROM ITEM_VEND_PKG WHERE IV.ITEM_VEND_ID = ITEM_VEND_ID)";

            string searchText = txtSearch.Text.Trim().ToUpper();
            string searchFilter = ddlSearchFilter.SelectedValue.ToString();
            string searchBy = ddlSearchBy.SelectedValue.ToString();
            string searchIn = ddlSearchIn.SelectedValue.ToString();
            ArrayList arrSearch = new ArrayList();
            arrSearch.AddRange(searchText.Split(' '));

            if (searchIn == "Stock")
            {
                sql += "AND ((SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID)) > 0) ";
            }
            else if (searchIn == "Non-Stock")
            {
                sql += "AND ((SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID)) = 0) ";
            }

            switch (searchFilter)
            {
                case "Partial":
                    sql += "AND " + searchBy + " Like '%" + searchText + "%' ";
                    break;
                case "Exact":
                    sql += "AND " + searchBy + " = '" + searchText + "' ";
                    if (searchBy == "t1.DESCR")
                    {
                        sql += "OR t1.DESCR1 = '" + searchText + "' OR t1.DESCR2 = '" + searchText + "' ";
                    }
                    break;
                case "Any":
                    sql += "AND (" + searchBy + " LIKE '%" + arrSearch[0] + "%' ";
                    if (arrSearch.Count > 1)
                    {
                        for (int i = 0; i < arrSearch.Count; i++)
                        {
                            sql += "OR " + searchBy + " LIKE '%" + arrSearch[i] + "%' ";
                        }

                        for (int j = 0; j < arrSearch.Count; j++)
                        {
                            sql += "OR t1.DESCR1 LIKE '%" + arrSearch[j] + "%' ";
                        }

                        for (int k = 0; k < arrSearch.Count; k++)
                        {
                            sql += "OR t1.DESCR2 LIKE '%" + arrSearch[k] + "%' ";
                        }
                    }
                    sql += "OR t1.DESCR1 LIKE '%" + searchText + "%' OR t1.DESCR2 LIKE '%" + searchText + "%') ";
                    break;
                case "All":
                    sql += "AND " + searchBy + " LIKE '%" + searchText.Replace(" ", "%") + "%' ";
                    break;
            }

            if (searchFilter != "Exact" && searchFilter != "Any" && searchBy == "t1.DESCR")
            {
                sql += "OR t1.DESCR1 LIKE '%" + searchText + "%' OR t1.DESCR2 LIKE '%" + searchText + "%' ";
            }

            sql += "GROUP BY t1.ITEM_ID, t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, t1.DESCR2, ITEM_CORP_ACCT.EXP_ACCT_NO, MFR.NAME, IVP.PRICE ";
            sql += "ORDER BY t1.DESCR";
            ViewState["sql"] = sql;
            sdsItems.SelectCommand = sql;
            gvItems.DataBind();
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "BuildSearch: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Switches the tab view to item details
    protected void ibtnDetails_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["view"] = "details";
        mvItemInfo.ActiveViewIndex = 0;
        DetailsSwitch();
    }

    //Switches the tab view to item photo
    protected void ibtnPhoto_Click(object sender, ImageClickEventArgs e)
    {
        ViewState["view"] = "photo";
        mvItemInfo.ActiveViewIndex = 1;
        PhotoSwitch();
    }

    //Closes the item info window
    protected void lbtnClose_Click(object sender, EventArgs e)
    {
        mpeItemInfo.Hide();
        gvItems.DataBind();
    }

    //Switches the item info view to the suggestion form
    protected void lbtnSuggest_Click(object sender, EventArgs e)
    {
        try
        {
            mvItemInfo.ActiveViewIndex = 2;
            //string sql = @"SELECT firstName, lastName, userEmail FROM tblUserData WHERE userID = '" + Session["username"].ToString() + "'";
            //SqlCommand comm = new SqlCommand(sql, amcConn);
            //amcConn.Open();
           // SqlDataReader dr = comm.ExecuteReader();
            //while (dr.Read())
            //{
            //    lblName.Text = string.Format("{0} {1}", dr[0].ToString().Trim(), dr[1].ToString().Trim());
            //    lblEmail.Text = dr[2].ToString().Trim();
            //}
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "lbtnSuggest_Click: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Deletes an items photo
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            string itemNum = ViewState["itemNum"].ToString();
            File.Delete(Server.MapPath("~/catalog_images/" + itemNum.ToString().Trim() + ".jpg"));
            divOptions.Visible = true;
            btnUploadNew.Visible = true;
            LoadItemInfo();
            gvItems.DataBind();
            mpeItemInfo.Show();
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "btnDelete_Click: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    protected void btnUploadNew_Click(object sender, EventArgs e)
    {
        divUpload.Visible = true;
        divOptions.Visible = false;
        mpeItemInfo.Show();
    }

    //Uploads an items photo
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            string itemNum = ViewState["itemNum"].ToString();
            if (fuItemPhoto.FileName != null)
            {
                //if (fuItemPhoto.PostedFile.ContentType == "image/jpg" || fuItemPhoto.PostedFile.ContentType == "image/jpeg")
                //{
                fuItemPhoto.SaveAs(Server.MapPath("~/catalog_images/" + itemNum.Trim() + ".jpg"));
                divOptions.Visible = true;
                LoadItemInfo();
                mpeItemInfo.Show();
                //}
            }
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "btnUpload_Click: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Emails a description suggestion 
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            string toAddress, fromAddress, subject, body, name; //, phone;

            //if (txtPhone.Text == "") { phone = "&nbsp;"; } else { phone = txtPhone.Text.Trim(); }
           
            toAddress = "uwmcmm@u.washington.edu";
            //toAddress = "mijung@u.washington.edu";
            fromAddress = txtEmail.Text;
            subject = "Alternate Description Suggestion";
            body = string.Format(@"&lt;html&gt;&lt;head&gt;
                &lt;/head&gt;&lt;body style='font-family:Arial;font-size:.8em;color:#5e5e5e;'&gt;
                &lt;table style='border: 1px solid #7d7898;width:500px;font-size:.9em;' cellpadding='0' cellspacing='0'&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td colspan='2' style='padding:0;'&gt;
                            &lt;div style='background-color:#7d7898;color:White;padding:10px;width:500px;'&gt;
                                &lt;h4 style='margin:0;'&gt;Alternate Description Suggestion&lt;/h4&gt;
                            &lt;/div&gt;
                        &lt;/td&gt;
                    &lt;/tr&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;background-color:#F7F6F3;font-weight:bold;'&gt;
                            Name:
                        &lt;/td&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;'&gt;
                            {0}
                        &lt;/td&gt;
                    &lt;/tr&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;background-color:#F7F6F3;font-weight:bold;'&gt;
                            Email:
                        &lt;/td&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;'&gt;
                            {1}
                        &lt;/td&gt;
                    &lt;/tr&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;background-color:#F7F6F3;font-weight:bold;'&gt;
                            Department:
                        &lt;/td&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;'&gt;
                            {2}
                        &lt;/td&gt;
                    &lt;/tr&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;background-color:#F7F6F3;font-weight:bold;'&gt;
                            Item Num:
                        &lt;/td&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;'&gt;
                            {3}
                        &lt;/td&gt;
                    &lt;/tr&gt;
                    &lt;tr style='vertical-align:top;'&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;background-color:#F7F6F3;font-weight:bold;'&gt;
                            Suggestion:
                        &lt;/td&gt;
                        &lt;td style='padding:10px 0 10px 10px;border-bottom: 1px dashed #cccccc;'&gt;
                            {4}
                        &lt;/td&gt;
                    &lt;/tr&gt;
                &lt;/table&gt;&lt;br/&gt;&lt;a id='hypItem' &lt;/a&gt;&lt;/body&gt;&lt;/html&gt;", txtName.Text.Trim(), txtEmail.Text.Trim(), txtDepartment.Text.Trim(), lblItemNum.Text, txtDescription.Text.Trim());

            CommonCode email = new CommonCode();
            email.SendEmail(toAddress, fromAddress, subject, body, Session["username"].ToString());
            txtName.Text = "";
            txtEmail.Text = "";
            txtDepartment.Text = "";
            txtDescription.Text = "";
            mvItemInfo.ActiveViewIndex = 0;
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "btnSubmit_Click: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
    }

    //Cancels out of the suggestion form
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        txtName.Text = "";
        txtEmail.Text = "";
        txtDepartment.Text = "";
        txtDescription.Text = "";
        mvItemInfo.ActiveViewIndex = 0;
    }

    //Loads the item information when an item is selected from the results
    protected void LoadItemInfo()
    {
        try
        {
            if (ViewState["view"].ToString() == "details")
            { mvItemInfo.ActiveViewIndex = 0; DetailsSwitch(); }
            else if (ViewState["view"].ToString() == "photo")
            { mvItemInfo.ActiveViewIndex = 1; PhotoSwitch(); }
            string itemNum = ViewState["itemNum"].ToString();
            litItemNumDetails.Text = itemNum;
            litItemNumPhoto.Text = itemNum;
            lblItemNum.Text = itemNum;
            string sql = @"SELECT ITEM_NO, DESCR, CTLG_NO, STOCKED, DESCR2, ACCTCTG, PKG_UM_CD, UM_QTY, UM_PRICE, MFR_NAME, DESCR1, LOC
FROM (SELECT ROW_NUMBER() OVER ( PARTITION BY t1.ITEM_NO, v_uwmcmm_ItemMaster.PKG_UM_CD, v_uwmcmm_ItemMaster.UM_QTY, v_uwmcmm_ItemMaster.UM_PRICE ORDER BY t1.ITEM_NO, V.LOC_ID DESC ) AS 'RowNumber', t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, 
CONVERT(BIT,(SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) AS STOCKED, 
t1.DESCR2, ITEM_CORP_ACCT.EXP_ACCT_NO AS ACCTCTG, v_uwmcmm_ItemMaster.PKG_UM_CD, v_uwmcmm_ItemMaster.UM_QTY, v_uwmcmm_ItemMaster.UM_PRICE, v_uwmcmm_ItemMaster.MFR_NAME, t1.DESCR1, 
CASE WHEN CONVERT(BIT,(SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) = 1 AND V.LOC_ID <> 1007 AND V.LOC_ID IN (1004, 1006) THEN '(Stocked at Sand Point Only)' ELSE '' END AS LOC
FROM ITEM AS t1 INNER JOIN ITEM_CORP_ACCT ON t1.ITEM_ID = ITEM_CORP_ACCT.ITEM_ID AND t1.ITEM_IDB = ITEM_CORP_ACCT.ITEM_IDB INNER JOIN v_uwmcmm_ItemMaster ON t1.ITEM_ID = v_uwmcmm_ItemMaster.ITEM_ID LEFT JOIN v_uwmcmm_SupplyLocationItems V ON V.ITEM_NO = t1.ITEM_NO AND V.LOC_ID IN (1004, 1006, 1007) WHERE (t1.ITEM_NO = '" + itemNum + "') AND (ITEM_CORP_ACCT.CORP_ID = 1000) AND (t1.STAT IN (1, 2)) AND (v_uwmcmm_ItemMaster.VEND_SEQ = 1)) DT WHERE RowNumber = 1 ORDER BY ITEM_NO";
            
            SqlCommand comm = new SqlCommand(sql, pmmConn);
            pmmConn.Open();
            SqlDataReader dr = comm.ExecuteReader();
            Boolean stocked = false;
            DataSet dsPackaging = new DataSet();
            DataTable dtPackaging;
            dtPackaging = dsPackaging.Tables.Add();
            dtPackaging.Columns.Add("UM", typeof(string));
            dtPackaging.Columns.Add("UM QTY", typeof(string));
            dtPackaging.Columns.Add("UM PRICE", typeof(string));
            while (dr.Read())
            {                
                lblDescription.Text = dr[1].ToString().Trim();
                if (dr[4].ToString().Trim() != "")
                {
                    lblNursing.Text = dr[4].ToString().Trim().ToUpper();
                    lblNursing.Enabled = false;
                }
                else
                {
                    lblNursing.Text = "<i>NO NURSING DESCRIPTION</i>";
                    lblNursing.Enabled = false;
                }
                if (dr[10].ToString().Trim() != "")
                {
                    lblAlternate.Text = dr[10].ToString().Trim().ToUpper();
                    lblAlternate.Enabled = true;
                }
                else
                {
                    lblAlternate.Text = "<i>NO ALTERNATE DESCRIPTION</i>";
                    lblAlternate.Enabled = false;
                }                     

                lblAcctCatg.Text = dr[5].ToString().Trim();
                dtPackaging.Rows.Add(dr[6].ToString().Trim(), dr[7].ToString().Trim(), dr[8].ToString().Trim());
                stocked = (Boolean)dr[3];
                if (stocked) { lblStock.Text = "Y"; }
                else { lblStock.Text = "N"; }
                lblLocation.Text = dr[11].ToString().Trim();
                lblMfgName.Text = dr[9].ToString().Trim();
                lblMfgNum.Text = dr[2].ToString().Trim();                              
                
            }
            gvPackaging.DataSource = dsPackaging;
            gvPackaging.DataBind();

            if (File.Exists(Server.MapPath("~/catalog_images/" + itemNum.Trim() + ".jpg")))
            {
                imgItemPhoto.ImageUrl = "~/catalog_images/" + itemNum.Trim() + ".jpg";

                System.Drawing.Image objImage = System.Drawing.Image.FromFile(Server.MapPath("~/catalog_images/" + itemNum.Trim() + ".jpg"));

                float h = objImage.Height;
                float w = objImage.Width;
                float wRatio = w / h;
                float hRatio = h / w;

                if (objImage.Height > 450)
                {
                    int newWidth = Convert.ToInt32(wRatio * 400);
                    imgItemPhoto.Width = newWidth;
                    if (newWidth > 480)
                    {
                        imgItemPhoto.Width = 480;
                        imgItemPhoto.Height = Convert.ToInt32(hRatio * 480);
                    }
                    else
                    {
                        imgItemPhoto.Height = 400;
                    }
                }
                else
                {
                    //imgItemPhoto.Width = 480;
                    //imgItemPhoto.Height = Convert.ToInt32(hRatio * 480);
                }
                if (Session["catalogAdmin"] != null)
                {
                    divUpload.Visible = false;
                    divOptions.Visible = true;
                    btnDelete.Visible = true;
                    btnUploadNew.Visible = false;
                }
            }
            else
            {
                if (Session["catalogAdmin"] != null)
                {
                    divUpload.Visible = false;
                    divOptions.Visible = true;
                    btnDelete.Visible = false;
                }
                imgItemPhoto.Width = 480;
                imgItemPhoto.ImageUrl = "~/catalog_images/no_image.jpg";
            }
            if (!Page.IsPostBack) { lbtnClose.PostBackUrl = Request.UrlReferrer.AbsoluteUri; }
        }
        catch (Exception ex)
        {
            CommonCode error = new CommonCode();
            error.ReportError(ex.Message, "sdsItems_Selected: catalog.aspx.cs", Session["username"].ToString());
            divMessage.Visible = true;
            litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
        }
        finally
        {
            pmmConn.Close();
        }
    }

    //Switches the tab image to item details
    protected void DetailsSwitch()
    {
        ibtnDetails.ImageUrl = "~/images/btn_details_up.gif";
        ibtnPhoto.ImageUrl = "~/images/btn_photo_down.gif";
    }

    //Switches the tab image to item photo
    protected void PhotoSwitch()
    {
        ibtnDetails.ImageUrl = "~/images/btn_details_down.gif";
        ibtnPhoto.ImageUrl = "~/images/btn_photo_up.gif";
    }

    protected void GetItemPhotos()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ITEM_NO", typeof(System.String));
        dt.Columns.Add("DESCR", typeof(System.String));
        dt.Columns.Add("CTLG_NO", typeof(System.String));
        dt.Columns.Add("STOCKED", typeof(System.String));
        DataRow row;

        string sql = "SELECT t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, CONVERT(BIT, (SELECT COUNT(ITEM_ID) AS STOCK_COUNT FROM ITEM_CC_ACCT_EXCP AS t2 WHERE (CC_ID IN (1001, 1002, 1003)) AND (ITEM_ID = t1.ITEM_ID))) AS STOCKED, t1.DESCR2, ITEM_CORP_ACCT.EXP_ACCT_NO AS ACCTCTG, MFR.NAME FROM ITEM AS t1 INNER JOIN ITEM_CORP_ACCT ON t1.ITEM_ID = ITEM_CORP_ACCT.ITEM_ID AND t1.ITEM_IDB = ITEM_CORP_ACCT.ITEM_IDB INNER JOIN MFR ON t1.MFR_ID = MFR.MFR_ID AND t1.MFR_IDB = MFR.MFR_IDB WHERE (ITEM_CORP_ACCT.CORP_ID = 1000) AND (t1.STAT IN (1, 2))";
        SqlCommand comm = new SqlCommand(sql, pmmConn);
        pmmConn.Open();
        SqlDataReader dr = comm.ExecuteReader();
        while (dr.Read())
        {
            if (!File.Exists(Server.MapPath("~/catalog_images/" + dr[0].ToString().Trim() + ".jpg")))
            {
                row = dt.NewRow();
                row["ITEM_NO"] = dr[0].ToString().Trim();
                row["DESCR"] = dr[1].ToString().Trim();
                row["CTLG_NO"] = dr[2].ToString().Trim();
                row["STOCKED"] = dr[3].ToString().Trim();
                dt.Rows.Add(row);
            }
        }
        gvItems.DataSourceID = null;
        gvItems.DataSource = dt;
        gvItems.DataBind();

        string x = dt.Rows.Count.ToString();

        dr.Dispose();
        comm.Dispose();
        pmmConn.Close();
    }

    protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (ddlOption.SelectedIndex == 1)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        LinkButton lbtnItemNum = (LinkButton)e.Row.FindControl("lbtnItemNum");
        //        string itemNumber = lbtnItemNum.CommandArgument.Trim();
        //        if (File.Exists(Server.MapPath("~/catalog_images/" + itemNumber + ".jpg")))
        //        {
        //            e.Row.Visible = false;
        //        }
        //    }
        //    ++xx;
        //    litResults.Text = xx.ToString();
        //}
    }

    //Calls a databind for the items gridview
    protected void ibtnView_Click(object sender, ImageClickEventArgs e)
    {
        //if (ddlOption.SelectedIndex == 1)
        //{
        //    gvItems.AllowPaging = false;
        //    gvItems.DataBind();
        //}
    }

    //Scrolls to the top of the page after a page change
    protected void gvItems_PageIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "ScrollPage", "ResetScrollPosition();", true);
    }

    protected void gvItems_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}