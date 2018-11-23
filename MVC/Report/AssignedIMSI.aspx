<%@ Page Title="Assigned IMSI" Language="C#" AutoEventWireup="true" CodeBehind="AssignedIMSI.aspx.cs" Inherits="StoreProject.Reports.AssignedIMSI" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">  
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>  
    <div >  
        
        <rsweb:ReportViewer ID="rptViewer" runat="server"   Font-Size="8pt" ProcessingMode="Remote"
                            ShowCredentialPrompts="False" ShowDocumentMapButton="False" ShowExportControls="true"
                            ShowFindControls="False" ShowPageNavigationControls="True" ShowParameterPrompts="False"
                            ShowPrintButton="false" ShowPromptAreaButton="False" ShowRefreshButton="False"
                            ShowToolBar="true" ShowZoomControl="False" EnableTheming="True" AsyncRendering="false"
                            SizeToReportContent="True" BorderWidth="0px"  ShowBackButton="False"></rsweb:ReportViewer>
    </div>  
</form>
</body>
</html>
