using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Image = System.Drawing.Image;



namespace InventorySearch
{
    public partial class SearchPage : Page
    {
        #region Class Variables
        private SqlConnection dbaseConn;       
        private SqlDataAdapter dataAdapter = new SqlDataAdapter();
        private static NameValueCollection ConfigData = null;
        protected HttpSessionStateBase thisSession = null;
        private bool searchClick = false;
        private DataSet dTable = new DataSet();
        private string dbaseConnStr = "";
        private string archivePath = "";
        private string locationID = "0";
        private int totalItemCount = 0;
        private int pageCount = 20;
        private LogManager lm = LogManager.GetInstance();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigData = (NameValueCollection)WebConfigurationManager.GetSection("appSettings");
                gvItemList.RowStyle.Height = 40;
                LoadData();
            }
        }

        private void ResizeGrid()
        {
            for (int i = 0; i < gvItemList.Columns.Count; i++)
            {
                int width;
                switch (i)
                {
                    case 0:                 //item#
                        width = 65;         
                        break;
                    case 1:                 //descr
                        width = 100;         
                        break;
                    case 2:                 //ctlg#
                        width = 60;         
                        break;
                    case 3:                 //price
                        width = 50;
                        break;
                    case 4:                 //location id
                        width = 65;
                        break;
                    case 5:                 //image
                        width = 75;
                        break;
                    case 6:                 //stocked
                        width = 65;
                        break;
                    default:
                        width = 60;
                        break;
                }
                gvItemList.Columns[i].ItemStyle.Width = width;
            }
        }

        protected void LoadData()
        {
            string sql = BuildSQL();
            SqlDataSource1.SelectCommand = sql;
            int indx = 0;
            int col = 0;
            try
            {
                litResults.Text = "0";
                dbaseConn = new SqlConnection(GetAccess());
                dbaseConn.Open();
                dataAdapter = new SqlDataAdapter(sql, dbaseConn);
                dataAdapter.Fill(dTable);
                ParseDataSet(dTable);
                if (dTable.Tables[0].Rows.Count > 0)
                {
                    litResults.Text = dTable.Tables[0].Rows.Count.ToString();
                    try
                    {
                        gvItemList.PageSize = Convert.ToInt16(DropDownList1.SelectedValue);
                        BindGrid();
                    }
                    catch (Exception ex)
                    {
                        //CommonCode error = new CommonCode();
                        lm.Write("searchpage.aspx.cs--ddlPageCount_SelectedIndexChanged:      " + Session["username"].ToString() + Environment.NewLine + ex.Message);
                        divMessage.Visible = true;
                        litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
                    }
                }
                dbaseConnStr = "";
                dbaseConn.Close();
            }
            catch (Exception ex)
            {
                lm.Write("LoadData: " + Environment.NewLine + ex.Message);
            }
        }

        private void BindGrid()
        {
            gvItemList.DataBind();
        }

        private void ParseDataSet(DataSet dsTemp)
        {
            totalItemCount = dsTemp.Tables[0].Rows.Count;
            gvItemList.VirtualItemCount = totalItemCount;
            dTable = dsTemp;
        }

        private string BuildSQL()
        {
            FindLocationID(ddlLocation.SelectedIndex);            
            string pageCount = DropDownList1.SelectedValue;
            string where = "WHERE SLOC_ITEM.LOC_ID IN (" + locationID + ") ";
            string sql = "SELECT  t1.ITEM_NO AS [ITEM #], t1.DESCR AS DESCRIPTION , t1.CTLG_NO AS [CATALOG #], " +
            "CAST(QTY AS BIT) AS STOCKED, " +
            " (SELECT CASE SLOC_ITEM.LOC_ID " +
             "         WHEN '1000' THEN 'WHSE' " +
             "         WHEN '1001' THEN 'OR' " +
             "         WHEN '1002' THEN 'MED STORES' " +
             "         WHEN '1003' THEN 'IMPLANTS' " +
             "         WHEN '2001' THEN 'ANGIO' " +
             "         ELSE 'STANDARDS' " +
             "         END) AS LOCATION, " +
             " '' AS IMAGE, " +
             "'$' + CONVERT(VARCHAR(10), CONVERT(MONEY, IVP.PRICE)) AS PRICE " +
            "FROM dbo.ITEM AS t1 " +
            "JOIN dbo.SLOC_ITEM ON t1.ITEM_ID = SLOC_ITEM.ITEM_ID " +
            "JOIN dbo.ITEM_VEND_PKG IVP ON IVP.ITEM_VEND_ID = SLOC_ITEM.ITEM_VEND_ID ";
              
            if (searchClick)
                sql = BuildSearch(sql);
            else
            {
                sql += where +
                    "AND IVP.SEQ_NO = (SELECT MAX(SEQ_NO) FROM dbo.ITEM_VEND_PKG WHERE ITEM_VEND_ID = SLOC_ITEM.ITEM_VEND_ID) " +
                    "ORDER BY ITEM_NO";
            }
            return sql;                
        }

        private string BuildSearch(string sql)
        {
            string searchText = txtSearch.Text.Trim().ToUpper();
            string filter = ddlSearchFilter.Text;
            string searchBy = ddlSearchBy.Text;

            try
            {
                if (locationID.Equals("9999"))
                {
                    sql = "SELECT distinct ITEM_NO, DESCR, t1.CTLG_NO, '$' + CONVERT(VARCHAR(10), CONVERT(MONEY, IVP.PRICE)) AS PRICE," +
                            "CONVERT(BIT,0) AS STOCKED " +
                            "FROM dbo.ITEM AS t1 " +
                            "JOIN dbo.ITEM_VEND ON t1.ITEM_ID = ITEM_VEND.ITEM_ID " +
                            "JOIN dbo.ITEM_VEND_PKG IVP ON IVP.ITEM_VEND_ID = ITEM_VEND.ITEM_VEND_ID " +
                            "WHERE t1.ITEM_ID IN " +
                            "(SELECT ITEM_ID " +
                            "FROM dbo.ITEM WHERE ITEM.STAT = 1 " +
                            "AND ITEM.CTLG_ITEM_IND = 'Y' " +
                            "EXCEPT " +
                            "SELECT ITEM_ID " +
                            "FROM dbo.SLOC_ITEM " +
                            "WHERE SLOC_ITEM.STAT = 1) " +
                            "AND t1.STAT = 1 " +
                            "AND IVP.SEQ_NO = (SELECT MAX (SEQ_NO) FROM dbo.ITEM_VEND_PKG WHERE ITEM_VEND_ID = ITEM_VEND.ITEM_VEND_ID) " +
                            "AND ITEM_VEND.SEQ_NO = (SELECT MIN (SEQ_NO) FROM dbo.ITEM_VEND WHERE ITEM_VEND.ITEM_ID = t1.ITEM_ID) ";
                }
                else
                {
                    if (searchBy.Equals("MFR.NAME"))
                        sql += "JOIN MFR ON MFR.MFR_ID=t1.MFR_ID ";

                    sql += "WHERE SLOC_ITEM.STAT IN (1,2) " +
                     "AND SLOC_ITEM.LOC_ID IN (" + locationID + ") " +
                     "AND IVP.SEQ_NO = (SELECT MAX (SEQ_NO) FROM dbo.ITEM_VEND_PKG WHERE ITEM_VEND_ID = SLOC_ITEM.ITEM_VEND_ID) ";
                }

                ArrayList arrSearch = new ArrayList();
                arrSearch.AddRange(searchText.Split(' '));

                switch (filter)
                {
                    case "Partial":
                        sql += "AND " + searchBy + " Like '%" + searchText + "%' ";
                        break;
                    case "Exact":
                        sql += "AND " + searchBy + " = '" + searchText + "' ";
                        if (searchBy == "t1.DESCR")
                        {
                            sql += "OR t1.DESCR = '" + searchText + "' ";
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
                                sql += "OR t1.DESCR LIKE '%" + arrSearch[j] + "%' ";
                            }

                            for (int k = 0; k < arrSearch.Count; k++)
                            {
                                sql += "OR t1.DESCR LIKE '%" + arrSearch[k] + "%' ";
                            }
                        }
                        sql += "OR t1.DESCR LIKE '%" + searchText + "%') ";
                        break;
                    case "All":
                        sql += "AND " + searchBy + " LIKE '%" + searchText.Replace(" ", "%") + "%' ";
                        break;
                }

                if (filter != "Exact" && filter != "Any" && searchBy == "t1.DESCR")
                {
                    sql += "OR t1.DESCR LIKE '%" + searchText + "%' ";
                }

                sql += "GROUP BY t1.ITEM_NO, t1.DESCR, t1.CTLG_NO,QTY,SLOC_ITEM.LOC_ID, IVP.PRICE, t1.ITEM_ID ";
                sql += "ORDER BY t1.ITEM_NO";

            }
            catch (Exception ex)
            {
                divMessage.Visible = true;
                litMessage.Text = ex.Message;    //"Attn: An error occured and has been reported to the web administrator.";
            }
            return sql;
        }

        private string GetAccess()
        {
            try
            {
                dbaseConnStr = ConfigurationManager.ConnectionStrings["amc_userConnectionString"].ConnectionString;
                return dbaseConnStr;
            }
            catch (Exception ex)
            {
                lm.Write("GetAccess: " + Environment.NewLine + ex.Message);               
            }
            return dbaseConnStr;
        }

        protected string GetImageUrl(object itemNumber)
        {
            string url = null;
            string[] item = itemNumber.ToString().Split(';');
            string itemNo = item[0].Trim();
            string location = "";
            if (item.Length > 1)
                location = item[1].Trim();
            try
            {
                if (File.Exists(Server.MapPath("~/catalog_images/" + itemNo + ".jpg")))
                {
                    url = "~/catalog_images/" + itemNo + ".jpg";
                    //itemNumber.ToString().Trim()
                }
                else
                {
                    url = "~/catalog_images/no_image_thumb.jpg";
                }
            }
            catch (Exception ex)
            {
                lm.Write("SearchPage:GetUmageUrl      " + Session["username"].ToString() + Environment.NewLine + ex.Message);
                divMessage.Visible = true;
                litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }
            return url;
        }

        //////////protected void Button1_Click(object sender, EventArgs e)
        //////////{
        //////////    lm.Write("@ button click");
        //////////}

        private void FindLocationID(int locIndx)
        {
            switch (locIndx)
            {
                case 0:
                    locationID = "0";           //All
                    break;
                case 1:
                    locationID = "1002";        //MedStores
                    break;
                case 2:
                    locationID = "2001";        //Angio    (INV TOUCHSCAN ESI)
                    break;
                case 3:
                    locationID = "1001";        //OR
                    break;
                case 4:
                    locationID = "1003";        //OR Implants
                    break;
                case 5:
                    locationID = "1000";        //Warehouse
                    break;
                case 6:
                    locationID = "2539";        //Standards
                    break;
                default: break;
            }
            if (locationID == "0")
                locationID = "'1000','1001','1002','1003','2001','2539'";
        }    

        protected void ibtnSearch_Click(object sender, ImageClickEventArgs e)
        {
            searchClick = true;
            FindLocationID(ddlLocation.SelectedIndex);
            LoadData();
            searchClick = false;
        }
       
        

        protected void ibtnClear_Click(object sender, ImageClickEventArgs e)
        {
            txtSearch.Text = "";
            ddlLocation.SelectedIndex = 0;
            ddlSearchFilter.SelectedIndex = 2;
            ddlSearchBy.SelectedIndex = 0;
            ibtnSearch_Click(sender, e);
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gvItemList.PageSize = Convert.ToInt32(DropDownList1.SelectedValue);
                LoadData();
            }
            catch (Exception ex)
            {
                //CommonCode error = new CommonCode();
                lm.Write("searchpage.aspx.cs--ddlPageCount_SelectedIndexChanged:      " + Session["username"].ToString() + Environment.NewLine + ex.Message);
                divMessage.Visible = true;
                litMessage.Text = "Attn: An error occured and has been reported to the web administrator.";
            }

        }

        protected void gvItemList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvItemList.PageIndex = e.NewPageIndex;
            LoadData();
        }       

        protected void lbtnItemNum_Click(object sender, EventArgs e)
        {            
            Server.MapPath("~/details.aspx");
            string[] arg = new string[2];
            arg = ((LinkButton)sender).CommandArgument.ToString().Split(';');

            string item = arg[0].ToString().Trim();
            Session["locationID"] = GetLocationCode(arg[1]);
            Session["connect"] = GetAccess();
            string query = "details.aspx?item=" + item;            
            string newWin = "window.open('" + query + "','_blank');";
            //Session["locationID"] = GetLocationCode();
            ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenWindow", newWin, true);
        }

        private string GetLocationCode(string loc)
        {
           // string location = ddlLocation.Text;
            switch (loc)
            {
                case "MED STORES":
                    loc = "1002";
                    break;
                case "ANGIO":
                    loc = "2001"; //INV_TOUCHSCAN
                    break;
                case "OR":
                    loc = "1001";
                    break;
                case "IMPLANTS":
                    loc = "1003";
                    break;
                case "WHSE":
                    loc = "1000";
                    break;
                case "STANDARDS":
                    loc = "2539";
                    break;
                default:
                    loc = "1000,1001,1002,1003,2001,2539";
                    break;
            }
            return loc;
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = 0;
            x++;
            switch (ddlLocation.SelectedIndex)
            {
                case 0:
                    locationID = "0";           //All
                    break;
                case 1:
                    locationID = "1002";        //MedStores
                    break;
                case 2:
                    locationID = "2001";        //Angio    (INV TOUCHSCAN ESI)
                    break;
                case 3:
                    locationID = "1001";        //OR
                    break;
                case 4:
                    locationID = "1003";        //OR Implants
                    break;
                case 5:
                    locationID = "1000";        //Warehouse
                    break;
                case 6:
                    locationID = "2539";        //Standards
                    break;
                default:
                    locationID = "0";
                    ddlLocation.SelectedIndex = 0; //defaults to All
                    break;
            }
            if (locationID == "0")
                locationID = "'1000','1001','1002','1003','2001','2539'";

            Session["locationID"] = locationID; // lID.ToString();
            //ViewState["sql"] = null;   //used to tell PageLoad() not to use the cached ViewState["sql"], if there is one, because the user has selected a new location
            //                           //the ViewState is set when the user clicks the Search button (see BuildSearch below) and ViewState["sql"] is the query for the last location.
            //Page_Load(sender, e);
            LoadData();
        }
    }
}