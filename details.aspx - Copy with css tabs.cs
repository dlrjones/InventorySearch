using System;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.IO;

namespace InventorySearch
{
    public partial class details : System.Web.UI.Page
    {
        private System.Data.SqlClient.SqlConnection dbaseConn;
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private static NameValueCollection ConfigData = null;
        private DataSet dTable = new DataSet();
        private LogManager lm = LogManager.GetInstance();
        string dbConnString = "";
        string item = "";

        protected void Page_Load(object sender, EventArgs e)
        {           
            item = Request.QueryString["item"];
            string height = Request.QueryString["height"];
            string width = Request.QueryString["width"];
            dbConnString = Request.QueryString["connect"];
            Page.Title = "Item# " + item.Trim();

            TextBox1.Text = item;            
            //Label1.Text = "<h3>Item #" + item + " Details</h3>";
            LoadData(item);    
        }
       
        protected void LoadData(string item)
        {
            string sql = BuildSQL(item);
          //  SqlDataSource1.SelectCommand = sql;
            dbaseConn = new SqlConnection(dbConnString);
            dbaseConn.Open();
            dataAdapter = new SqlDataAdapter(sql, dbaseConn);
            dataAdapter.Fill(dTable);
        }

        private string BuildSQL(string itemNum)
        {
            return "SELECT t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, " +
     "CONVERT(BIT, isnull((SELECT QTY FROM dbo.SLOC_ITEM WHERE (LOC_ID IN (" + Session["locationID"] + ")) AND (ITEM_ID = t1.ITEM_ID)),0)) AS STOCKED, t1.DESCR2,  " +
     "SUBSTRING(dbo.ITEM_VEND_PKG.TO_UM_CD, 7, 2) AS PKG_TO_UM_CD, " +
     "SUBSTRING(dbo.ITEM_VEND_PKG.UM_CD, 7, 2) AS PKG_UM_CD, dbo.ITEM_VEND_PKG_FACTOR.TO_QTY AS UM_QTY, " +
     "ITEM_VEND_PKG.PRICE AS UM_PRICE, MFR.NAME, t1.DESCR1 " +
    "FROM ITEM AS t1 " +
    "JOIN ITEM_VEND ON ITEM_VEND.ITEM_ID = t1.ITEM_ID " +
    "JOIN MFR ON t1.MFR_ID = MFR.MFR_ID " +
    "JOIN ITEM_VEND_PKG ON ITEM_VEND.ITEM_VEND_ID = ITEM_VEND_PKG.ITEM_VEND_ID " +
    "JOIN ITEM_VEND_PKG_FACTOR ON dbo.ITEM_VEND_PKG.ITEM_VEND_ID = dbo.ITEM_VEND_PKG_FACTOR.ITEM_VEND_ID AND " +
                          "dbo.ITEM_VEND_PKG.ITEM_VEND_IDB = dbo.ITEM_VEND_PKG_FACTOR.ITEM_VEND_IDB AND " +
                          "dbo.ITEM_VEND_PKG.UM_CD = dbo.ITEM_VEND_PKG_FACTOR.UM_CD AND " +
                          "dbo.ITEM_VEND_PKG.TO_UM_CD = dbo.ITEM_VEND_PKG_FACTOR.TO_UM_CD " +
    "JOIN VEND ON dbo.ITEM_VEND.VEND_ID = dbo.VEND.VEND_ID " +
    "WHERE (t1.ITEM_NO = '" + itemNum.Trim() + "') " +
    "AND (t1.STAT IN (1, 2)) " +
    "AND ITEM_VEND.SEQ_NO = 1";
        }

        protected string GetImageUrl(object itemNumber)     //Eval("[Item #]  object itemNumber
        {
            string url = null;
            try
            {
                if (File.Exists(Server.MapPath("~/catalog_images/" + item.Trim() + ".jpg")))
                {
                    url = "~/catalog_images/" + item.Trim() + ".jpg";
                }
                else
                {
                    url = "~/catalog_images/no_image_thumb.jpg";
                }
                imgItem.ImageUrl = url;
            }
            catch (Exception ex)
            {
                lm.Write("SearchPage:GetUmageUrl      " + Session["username"].ToString() + " Item# " + item + Environment.NewLine + ex.Message);
                //divMessage.Visible = true;
                //litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }
            return url;
        }

        protected void gvItemDetail_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}