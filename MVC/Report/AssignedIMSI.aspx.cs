using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;

namespace StoreProject.Reports
{
  
    public partial class AssignedIMSI : System.Web.UI.Page
    {
        public ICredentials NetworkCredentials { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!Page.IsPostBack)
                {
                    ShowReport(User.Identity.GetUserId());
                }
            }
            
          
        }

        private void ShowReport(string userid)
        {
            try
            {
                ReportParameter[] parm = new ReportParameter[4];
                parm[0] = new ReportParameter("NombreEmpresa", "Vagwireless");
                parm[1] = new ReportParameter("Server", "SQL5031.site4now.net");
               // parm[1] = new ReportParameter("Server", "SERVERCODEAS");
                parm[2] = new ReportParameter("basedDatos", "DB_A266CC_StoreProject");
               // parm[2] = new ReportParameter("basedDatos", "aspnet-StoreProject-20180328122119");
                parm[3] = new ReportParameter("userId", userid);

                rptViewer.ProcessingMode = ProcessingMode.Remote;
                rptViewer.SizeToReportContent = true;
                rptViewer.ServerReport.ReportServerCredentials = new CredencialesReporting("vagwireless-001", "vagw2018", "ifc");  

                rptViewer.ServerReport.ReportServerUrl = new Uri("http://sql5030.site4now.net/ReportServer");
                rptViewer.ServerReport.ReportPath = "/vagwireless-001/assignedimsi";
                rptViewer.ServerReport.SetParameters(parm);


             
                rptViewer.ServerReport.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
    }
}