﻿Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Xml
Imports System.Collections
Imports System.Data
Imports System.Drawing

Partial Public Class DefaultObjectDataSource
    Inherits System.Web.UI.UserControl


    Private initAppointments_Renamed As Boolean = True

    Private uniqueSessionPrefix_Renamed As String = String.Empty

    Private resourceSharing_Renamed As Boolean = False

    Public Property InitAppointments() As Boolean
        Get
            Return initAppointments_Renamed
        End Get
        Set(ByVal value As Boolean)
            initAppointments_Renamed = value
        End Set
    End Property
    Public Property UniqueSessionPrefix() As String
        Get
            Return uniqueSessionPrefix_Renamed
        End Get
        Set(ByVal value As String)
            uniqueSessionPrefix_Renamed = value
        End Set
    End Property
    Private ReadOnly Property CustomEventsSessionName() As String
        Get
            Return UniqueSessionPrefix & "CustomEvents"
        End Get
    End Property
    Private ReadOnly Property CustomResourcesSessionName() As String
        Get
            Return UniqueSessionPrefix & "CustomResources"
        End Get
    End Property
    Public Property ResourceSharing() As Boolean
        Get
            Return resourceSharing_Renamed
        End Get
        Set(ByVal value As Boolean)
            resourceSharing_Renamed = value
        End Set
    End Property

    Public ReadOnly Property AppointmentDataSource() As DataSourceControl
        Get
            Return temporaryAppointmentDataSource
        End Get
    End Property
    Public ReadOnly Property ResourceDataSource() As DataSourceControl
        Get
            Return temporaryResourceDataSource
        End Get
    End Property

    Public Sub AttachTo(ByVal control As ASPxScheduler)
        control.ResourceDataSource = Me.ResourceDataSource
        control.AppointmentDataSource = Me.AppointmentDataSource
        control.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Protected Sub temporaryAppointmentDataSource_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)

        Dim events_Renamed As CustomEventList = GetCustomEvents()
        e.ObjectInstance = New CustomEventDataSource(events_Renamed)
    End Sub
    Protected Sub temporaryResourceDataSource_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)
        Dim resources As CustomResourceList = GetCustomResources()
        e.ObjectInstance = New CustomResourceDataSource(resources)
    End Sub
    Public Function GetCustomEvents() As CustomEventList

        Dim events_Renamed As CustomEventList = TryCast(Session(CustomEventsSessionName), CustomEventList)
        If events_Renamed IsNot Nothing Then
            Return events_Renamed
        End If

        events_Renamed = If(InitAppointments, DemoUtils.CreateCustomEvents(ResourceSharing), New CustomEventList())
        Session(CustomEventsSessionName) = events_Renamed
        Return events_Renamed
    End Function
    Protected Function GetCustomResources() As CustomResourceList
        Dim resources As CustomResourceList = TryCast(Session(CustomResourcesSessionName), CustomResourceList)
        If resources IsNot Nothing Then
            Return resources
        End If

        resources = DemoUtils.CreateCustomResurces()
        Session(CustomResourcesSessionName) = resources
        Return resources
    End Function
End Class
