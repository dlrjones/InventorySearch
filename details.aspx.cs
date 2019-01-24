using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
        private string dbConnString = "";
        private string item = "";

        protected void Page_Load(object sender, EventArgs e)
        {           
            item = Request.QueryString["item"];
            dbConnString = Session["connect"].ToString();
            string location = Session["locationID"].ToString();
            string rtnResult = "";
            string sql = "";
            string height = Request.QueryString["height"];
            string width = Request.QueryString["width"];
            Page.Title = "Item# " + item.Trim();
            imgItem.ImageUrl = GetImageUrl();
            litItemNumDetails.Text = item.Trim();

            SetDataView(BuildSQL(item));
        }
       
        private void SetDataView(string sql)
        {
            string itemNo = "";
            dbaseConn = new SqlConnection(GetAccess());
            SqlCommand comm = new SqlCommand(sql, dbaseConn);
            dbaseConn.Open();
            SqlDataReader dr = comm.ExecuteReader();
            Boolean stocked = false;
            DataSet dsPackaging = new DataSet();
            DataTable dtPackaging;
            dtPackaging = dsPackaging.Tables.Add();
            dtPackaging.Columns.Add("UM", typeof(string));
            dtPackaging.Columns.Add("UM QTY", typeof(string));
            dtPackaging.Columns.Add("UM PRICE", typeof(string));
            try
            {
                while (dr.Read())
                {
                    itemNo = dr[0].ToString().Trim();
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
                    if (dr[11].ToString().Trim() != "")
                    {
                        lblSurgery.Text = dr[11].ToString().Trim().ToUpper();
                        lblSurgery.Enabled = true;
                    }
                    else
                    {
                        lblSurgery.Text = "<i>NO ALTERNATE DESCRIPTION</i>";
                        lblSurgery.Enabled = false;
                    }
                    if (dr[13].ToString().Trim() != "")
                    {
                        lblGHX.Text = dr[13].ToString().Trim().ToUpper();  //dr[11].ToString().Trim().ToUpper();
                        lblGHX.Enabled = true;
                    }
                    else
                    {
                        lblGHX.Text = "<i>NO ALTERNATE DESCRIPTION</i>";
                        lblGHX.Enabled = false;
                    }
                    lblOUOM.Text = dr[5].ToString().Trim();
                    dtPackaging.Rows.Add(dr[6].ToString().Trim(), dr[7].ToString().Trim(), dr[8].ToString().Trim());
                    stocked = (Boolean)dr[3];
                    if (stocked) { lblStock.Text = "Y"; }
                    else { lblStock.Text = "N"; }
                    lblQTY.Text = dr[12].ToString().Trim();
                    lblMfgName.Text = dr[9].ToString().Trim();
                    lblMfgNum.Text = dr[2].ToString().Trim();
                }
            }
            catch(Exception ex)
            {
                lm.Write("details.SetDataView:  " +  ex.Message);
                divMessage.Visible = true;
                litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }
            gvPackaging.DataSource = dsPackaging;
            gvPackaging.DataBind();
            //next line,and all it leads to, added 12/4/18 dlrjones
            GetUWItemNo(itemNo);            
            dbConnString = "";
            dbaseConn.Close();
        }

        private void GetUWItemNo(string itemNo)
        {
            try
            {                
                dbaseConn = new SqlConnection(GetBIAdminAccess());
                SqlCommand comm = new SqlCommand(GetBiadminSQL(itemNo), dbaseConn);
                dbaseConn.Open();
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read())
                {
                    lblUWMC.Text = dr[0].ToString();
                }
            }
            catch (Exception ex)
            {
                lm.Write("details.GetUWItemNo:  " + ex.Message);
                divMessage.Visible = true;
                litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }
        }

        private string GetBiadminSQL(string itemNo)
        {
            string sql = 
             "SELECT System_ItemNo " +
                   "FROM[uwm_BIAdmin].[dbo].[SC_UWMItemMasterLink] " +
                   "WHERE System_Id = 'UWMC' and UWM_ItemId = " +
                   "(SELECT UWM_ItemId FROM[uwm_BIAdmin].[dbo].[SC_UWMItemMasterLink] " +
                        "WHERE System_Id = 'HMC' and System_ItemNo = '" + itemNo + "')";
            return sql;
        }

        protected void LoadData(string item)
        {
            string sql = BuildSQL(item);
            dbaseConn = new SqlConnection(GetAccess());
            dbaseConn.Open();
            dataAdapter = new SqlDataAdapter(sql, dbaseConn);
            dataAdapter.Fill(dTable);
        }

        private string BuildSQL(string itemNum)
        {
            //Primary Description = ITEM.DESCR
            //Nursing Description = ITEM.DESCR2
            //Surgery Description = ITEM.DESCR1
            /*
                0 - t1.ITEM_NO
                1 - t1.DESCR 
                2 - t1.CTLG_NO
                3 - STOCKED
                4 - t1.DESCR2
                5 - PKG_TO_UM_CD --> use ORDER_UM_CD from Item_Vend table
                6 - PKG_UM_CD
                7 - UM_QTY
                8 - UM_PRICE
                9 - MFR.NAME
                10 - CTLG_NO
                11 - t1.DESCR1
                12 - QTY
                13 - GHX_FullDescr
           */

            return "SELECT t1.ITEM_NO, t1.DESCR, t1.CTLG_NO, " +
     "CONVERT(BIT, isnull((SELECT QTY FROM [h-hemm].dbo.SLOC_ITEM WHERE (LOC_ID IN (" + Session["locationID"] + ")) AND (ITEM_ID = t1.ITEM_ID)),0)) AS STOCKED, " +
    // "CONVERT(BIT, isnull(0,0)) AS STOCKED, " + 
     "t1.DESCR2,  " +
     "SUBSTRING([h-hemm].dbo.ITEM_VEND.ORDER_UM_CD, 7, 2) AS PKG_TO_UM_CD, " +
     // "SUBSTRING(dbo.ITEM_VEND_PKG.TO_UM_CD, 7, 2) AS PKG_TO_UM_CD, " +
     "SUBSTRING([h-hemm].dbo.ITEM_VEND_PKG.UM_CD, 7, 2) AS PKG_UM_CD, [h-hemm].dbo.ITEM_VEND_PKG_FACTOR.TO_QTY AS UM_QTY, " +
     "[h-hemm].dbo.ITEM_VEND_PKG.PRICE AS UM_PRICE, MFR.NAME, t1.CTLG_NO, t1.DESCR1, " +
     "(SELECT QTY FROM [h-hemm].dbo.SLOC_ITEM WHERE (LOC_ID IN (" + Session["locationID"] + ")) AND (ITEM_ID = t1.ITEM_ID)) AS QTY, " +
     "SC_UWMItemMaster.GHX_FullDescr AS DESCR_GHX " +
    "FROM [h-hemm].dbo.ITEM AS t1 " +
    "JOIN [h-hemm].dbo.ITEM_VEND ON ITEM_VEND.ITEM_ID = t1.ITEM_ID " +
    "JOIN [h-hemm].dbo.MFR ON t1.MFR_ID = MFR.MFR_ID " +
    "JOIN [h-hemm].dbo.ITEM_VEND_PKG ON ITEM_VEND.ITEM_VEND_ID = ITEM_VEND_PKG.ITEM_VEND_ID " +
    "JOIN [h-hemm].dbo.ITEM_VEND_PKG_FACTOR ON [h-hemm].dbo.ITEM_VEND_PKG.ITEM_VEND_ID = [h-hemm].dbo.ITEM_VEND_PKG_FACTOR.ITEM_VEND_ID AND " +
                          "[h-hemm].dbo.ITEM_VEND_PKG.ITEM_VEND_IDB = [h-hemm].dbo.ITEM_VEND_PKG_FACTOR.ITEM_VEND_IDB AND " +
                          "[h-hemm].dbo.ITEM_VEND_PKG.UM_CD = [h-hemm].dbo.ITEM_VEND_PKG_FACTOR.UM_CD AND " +
                          "[h-hemm].dbo.ITEM_VEND_PKG.TO_UM_CD = [h-hemm].dbo.ITEM_VEND_PKG_FACTOR.TO_UM_CD " +
    "JOIN [h-hemm].dbo.VEND ON [h-hemm].dbo.ITEM_VEND.VEND_ID = [h-hemm].dbo.VEND.VEND_ID " +
    "LEFT OUTER JOIN dbo.SC_UWMItemMaster ON SC_UWMItemMaster.UWM_ItemId = t1.ITEM_ID " +
    "WHERE (t1.ITEM_NO = '" + itemNum.Trim() + "') " +
    "AND (t1.STAT IN (1, 2)) " +
    "AND ITEM_VEND.SEQ_NO = 1";
        }

        protected string GetImageUrl()     //Eval("[Item #]  object itemNumber
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
                    url = "~/catalog_images/no_image.jpg";
                }
            }
            catch (Exception ex)
            {
                lm.Write("SearchPage:GetUmageUrl      " + Session["username"].ToString() + "  Item# " + item + Environment.NewLine + ex.Message);
            }
            return url;
        }

        private string GetAccess()
        {
            try
            {
                dbConnString = Session["connect"].ToString();   
            }
            catch (Exception ex)
            {
                lm.Write("GetAccess: " + Environment.NewLine + ex.Message);
            }
            return dbConnString;
        }

        private string GetBIAdminAccess()
        {
            try
            {
                dbConnString = Session["BIAdmin"].ToString();
            }
            catch (Exception ex)
            {
                lm.Write("GetBIAdminAccess: " + Environment.NewLine + ex.Message);
            }
            return dbConnString;
        }
        

        protected void lbtnSuggest_Click(object sender, EventArgs e)
        {
            try
            {
                Server.MapPath("~/Suggestion.aspx");
                string query = "Suggestion.aspx?item=" + item;
                string newWin = "window.open('" + query + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "pop", newWin, true);
            }
            catch (Exception ex)
            {
                lm.Write("lbtnSuggest_Click: catalog.aspx.cs - " + ex.Message);
                divMessage.Visible = true;
                litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }
        }
                
    }
}