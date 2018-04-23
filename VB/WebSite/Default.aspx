﻿<%@ Page Language="vb" AutoEventWireup="true"  CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v9.2, Version=9.2.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPopupControl" TagPrefix="dxpc" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v9.2, Version=9.2.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.v9.2, Version=9.2.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRoundPanel" TagPrefix="dxed" %>
<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v9.2, Version=9.2.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxScheduler" TagPrefix="dxwschs" %>
<%@ Register assembly="DevExpress.XtraScheduler.v9.2.Core, Version=9.2.2.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraScheduler" TagPrefix="cc1" %>

<%@ Register Src="~/DefaultObjectDataSource.ascx" TagName="DataSource" TagPrefix="dd" %>
<%@ Register Src="~/UserForms/ScriptAppointmentForm.ascx" TagName="ScriptAppointmentForm" TagPrefix="form"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<script type="text/javascript"><!--
	function OnMenuItemClick(s, e) {
		e.handled = true;
		switch(e.itemName) {
			case SchedulerMenuItemId.NewAppointment:
				scheduler.ShowLoadingPanel();
				NewAppointment(scheduler);
				break;
			case SchedulerMenuItemId.NewRecurringAppointment:
				scheduler.ShowLoadingPanel();
				NewRecurringAppointment(scheduler);
				break;
			case SchedulerMenuItemId.NewAllDayEvent:
				scheduler.ShowLoadingPanel();
				NewAllDayEvent(scheduler);
				break;
			case SchedulerMenuItemId.NewRecurringEvent:
				scheduler.ShowLoadingPanel();
				NewRecurringEvent(scheduler);
				break;
			case SchedulerMenuItemId.OpenAppointment:
				scheduler.ShowLoadingPanel();
				OpenAppointment(scheduler);
				break;
			case SchedulerMenuItemId.EditSeries:
				scheduler.ShowLoadingPanel();
				EditSeries(scheduler);
				break;
			default:
				e.handled = false;    
		}
	}
	function OpenAppointment(scheduler) {
		var apt = GetSelectedAppointment(scheduler);
		scheduler.RefreshClientAppointmentProperties(apt, AppointmentPropertyNames.Normal, OnAppointmentRefresh);
	}
	function EditSeries(scheduler) {
		var apt = GetSelectedAppointment(scheduler);
		scheduler.RefreshClientAppointmentProperties(apt, AppointmentPropertyNames.Pattern, OnAppointmentEditSeriesRefresh);                        
	}
	function OnAppointmentRefresh(apt) {
		ShowAppointmentForm(apt);
	}
	function OnAppointmentEditSeriesRefresh(apt) {
		if (apt.GetRecurrencePattern()) {
			ShowAppointmentForm(apt.GetRecurrencePattern());
		}
	}
	function NewAppointment(scheduler) {
		var apt = CreateAppointment(scheduler)
		ShowAppointmentForm(apt);
	}
	function NewRecurringAppointment(scheduler) {
		var apt = CreateRecurringAppointment(scheduler);
		ShowAppointmentForm(apt);
	}
	function NewRecurringEvent(scheduler) {
		var apt = CreateRecurringEvent(scheduler);
		ShowAppointmentForm(apt);
	}
	function NewAllDayEvent(scheduler) {
		var apt = CreateAllDayEvent(scheduler);
		ShowAppointmentForm(apt);
	}            
	function ShowAppointmentForm(apt) {
		scheduler.HideLoadingPanel();
		MyScriptForm.Clear();
		MyScriptForm.Update(apt);
		if (apt.GetSubject() != "") 
			myFormPopup.SetHeaderText(apt.GetSubject() +" - Appointment");
		else
			myFormPopup.SetHeaderText("Untitled - Appointment");
		myFormPopup.Show();
	}
	function CloseAppointmentForm() {
		myFormPopup.Hide();
	}
	function CreateAppointment(scheduler) {
		var apt = new ASPxClientAppointment();
		var selectedInterval = scheduler.GetSelectedInterval();
		apt.SetStart(selectedInterval.GetStart());
		apt.SetEnd(selectedInterval.GetEnd());
		apt.AddResource(scheduler.GetSelectedResource());
		apt.SetLabelId(0);
		apt.SetStatusId(0);
		return apt;
	}
	function CreateRecurringAppointment(scheduler) {
		var apt = CreateAppointment(scheduler);
		apt.SetAppointmentType(ASPxAppointmentType.Pattern);
		var recurrenceInfo = new ASPxClientRecurrenceInfo();
		recurrenceInfo.SetStart(apt.GetStart());
		recurrenceInfo.SetEnd(apt.GetEnd());
		apt.SetRecurrenceInfo(recurrenceInfo);
		return apt;
	}
	function CreateAllDayEvent(scheduler) {
		var apt = CreateAppointment(scheduler);
		var start = apt.interval.start;
		var today = new Date(start.getFullYear(), start.getMonth(), start.getDate());
		apt.SetStart(today);
		apt.SetDuration(24 * 60 * 60 * 1000);
		apt.SetAllDay(true);
		return apt;
	}
	function CreateRecurringEvent(scheduler) {
		var apt = CreateAllDayEvent(scheduler);
		apt.SetAppointmentType(ASPxAppointmentType.Pattern);
		var recurrenceInfo = new ASPxClientRecurrenceInfo();
		recurrenceInfo.SetStart(apt.GetStart());
		recurrenceInfo.SetEnd(apt.GetEnd());
		apt.SetRecurrenceInfo(recurrenceInfo);
		return apt;
	}
	function GetSelectedAppointment(scheduler) {
		var aptIds = scheduler.GetSelectedAppointmentIds();
		if (aptIds.length == 0)
			return;
		var apt = scheduler.GetAppointmentById(aptIds[0]);
		return apt;
	}
//--></script>
	<form id="form1" runat="server">
	<div>
		<dd:DataSource runat="server" id="dataSource" UniqueSessionPrefix="ResourceSharing" ResourceSharing="true" />

		<dxwschs:ASPxScheduler id="ASPxScheduler1" runat="server" Width="100%" ActiveViewType="WorkWeek" Start="2008-07-13" ClientInstanceName="scheduler" GroupType="Resource">
		<Views>
			<DayView ResourcesPerPage="2">
				<TimeRulers><cc1:TimeRuler /></TimeRulers>
				<DayViewStyles ScrollAreaHeight="200px" />                
			</DayView>
			<WorkWeekView ResourcesPerPage="2">
				<TimeRulers><cc1:TimeRuler /></TimeRulers>
				<WorkWeekViewStyles ScrollAreaHeight="200px" />                
			</WorkWeekView>
			<WeekView ResourcesPerPage="2">
			  <WeekViewStyles>
				<DateCellBody Height="50px" />                
			  </WeekViewStyles>
			</WeekView>
			<MonthView ResourcesPerPage="2">
			  <MonthViewStyles>
				<DateCellBody Height="50px" />
			  </MonthViewStyles>
			</MonthView>
			<TimelineView ResourcesPerPage="2">
			  <TimelineViewStyles>
				<TimelineCellBody Height="250px" />                
			  </TimelineViewStyles>
			</TimelineView>
		</Views>
		<ClientSideEvents MenuItemClicked="function(s, e) { OnMenuItemClick(s,e); }" />
		<Storage EnableReminders="False">
			<Appointments ResourceSharing="True">
				<CustomFieldMappings>
					<dxwschs:ASPxAppointmentCustomFieldMapping Member="ContactInfo" Name="ContactInfo" />
					<dxwschs:ASPxAppointmentCustomFieldMapping Member="Price" Name="Price" />
				</CustomFieldMappings>
			</Appointments>
		</Storage>
	</dxwschs:ASPxScheduler>
	<%-- EndRegion --%>
	<dxpc:ASPxPopupControl ID="ASPxPopupControl1" runat="server" ClientInstanceName="myFormPopup" AllowDragging="true" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" EnableAnimation="false" Width="0px" Height="0px" Modal="true" CloseAction="CloseButton">
		<ClientSideEvents Shown="function(s,e) { s.SetSize(10,10); } " />
		<ContentCollection>
			<dxpc:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
				<form:ScriptAppointmentForm runat="server"  ID="AppointmentForm" SchedulerId="ASPxScheduler1" ClientInstanceName="MyScriptForm" ClientSideEvents-FormClosed="function(s, e) { CloseAppointmentForm();}" />
			</dxpc:PopupControlContentControl>
		</ContentCollection>
	</dxpc:ASPxPopupControl>
	</div>
	</form>
</body>
</html>