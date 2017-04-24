﻿Imports BoincRpc

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        My.Settings.MySQLServer = TextBox4.Text
        My.Settings.MySQLPort = TextBox5.Text
        My.Settings.MySQLDatabase = TextBox6.Text
        My.Settings.MySQLUsername = TextBox7.Text
        My.Settings.MySQLPassword = TextBox8.Text
        My.Settings.TimeToWait = NumericUpDown1.Value
        My.Settings.Save()
        If NumericUpDown1.Value > 0 Then
            Timer1.Interval = NumericUpDown1.Value * 60 * 1000
            Timer1.Start()
            Dim NumberOfHosts As Integer = ListBox1.Items.Count
            StatusLog("Starting MySQL Database Update")
            StatusLog("Number of hosts: " & NumberOfHosts)
            StatusLog("Starting MySQL Database Update")
            TruncateTables(My.Settings.MySQLServer, My.Settings.MySQLPort, My.Settings.MySQLDatabase, My.Settings.MySQLUsername, My.Settings.MySQLPassword)
            If NumberOfHosts > 0 Then
                For i = 0 To NumberOfHosts - 1
                    GetHostTasks(My.Settings.PCName.Item(i), My.Settings.PCIPAddress.Item(i), My.Settings.PCPort.Item(i), My.Settings.PCPassword.Item(i), My.Settings.MySQLServer, My.Settings.MySQLPort, My.Settings.MySQLDatabase, My.Settings.MySQLUsername, My.Settings.MySQLPassword)
                Next
            End If
        End If

    End Sub

    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim BOINCClient As New RpcClient
        Try
            Await BOINCClient.ConnectAsync(TextBox2.Text, TextBox9.Text)
            Dim Authorized = Await BOINCClient.AuthorizeAsync(TextBox3.Text)
            If Authorized = True Then
                My.Settings.PCName.Add(TextBox1.Text)
                My.Settings.PCIPAddress.Add(TextBox2.Text)
                My.Settings.PCPassword.Add(TextBox3.Text)
                My.Settings.PCPort.Add(TextBox9.Text)
                ListBox1.Items.Add(TextBox1.Text)
                My.Settings.Save()
            Else
                MsgBox("Could not connect. Please check the PC details and try again")
            End If
        Catch ex As Exception
            MsgBox("Could not connect. Please check the PC details and try again")
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.PCName Is Nothing Then
            My.Settings.PCName = New Specialized.StringCollection
        End If
        If My.Settings.PCIPAddress Is Nothing Then
            My.Settings.PCIPAddress = New Specialized.StringCollection
        End If
        If My.Settings.PCPassword Is Nothing Then
            My.Settings.PCPassword = New Specialized.StringCollection
        End If
        If My.Settings.PCPort Is Nothing Then
            My.Settings.PCPort = New Specialized.StringCollection
        End If
        If My.Settings.PCName.Count > 0 Then
            For Each item In My.Settings.PCName
                ListBox1.Items.Add(item)
            Next
        End If
        TextBox4.Text = My.Settings.MySQLServer
        TextBox5.Text = My.Settings.MySQLPort
        TextBox6.Text = My.Settings.MySQLDatabase
        TextBox7.Text = My.Settings.MySQLUsername
        TextBox8.Text = My.Settings.MySQLPassword
        NumericUpDown1.Value = My.Settings.TimeToWait
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Try
            TextBox1.Text = ListBox1.SelectedItem
            TextBox2.Text = My.Settings.PCIPAddress.Item(ListBox1.SelectedIndex)
            TextBox3.Text = My.Settings.PCPassword.Item(ListBox1.SelectedIndex)
            TextBox9.Text = My.Settings.PCPort.Item(ListBox1.SelectedIndex)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        My.Settings.PCName.RemoveAt(ListBox1.SelectedIndex)
        My.Settings.PCIPAddress.RemoveAt(ListBox1.SelectedIndex)
        My.Settings.PCPassword.RemoveAt(ListBox1.SelectedIndex)
        My.Settings.PCPort.RemoveAt(ListBox1.SelectedIndex)
        ListBox1.Items.RemoveAt(ListBox1.SelectedIndex)
        My.Settings.Save()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim NumberOfHosts As Integer = ListBox1.Items.Count
        StatusLog("Starting MySQL Database Update")
        TruncateTables(My.Settings.MySQLServer, My.Settings.MySQLPort, My.Settings.MySQLDatabase, My.Settings.MySQLUsername, My.Settings.MySQLPassword)
        StatusLog("Number of hosts: " & NumberOfHosts)
        If NumberOfHosts > 0 Then
            For i = 0 To NumberOfHosts - 1
                GetHostTasks(My.Settings.PCName.Item(i), My.Settings.PCIPAddress.Item(i), My.Settings.PCPort.Item(i), My.Settings.PCPassword.Item(i), My.Settings.MySQLServer, My.Settings.MySQLPort, My.Settings.MySQLDatabase, My.Settings.MySQLUsername, My.Settings.MySQLPassword)
            Next
        End If
    End Sub
    Private Sub StatusLog(text As String)
        RichTextBox1.Text += Date.Now & " || " & text & vbNewLine
    End Sub
    Private Sub TruncateTables(MySQLServer As String, MySQLPort As Integer, MySQLDatabase As String, MySQLUsername As String, MySQLPassword As String)
        StatusLog("Truncating Table")
        Dim MySQLConnString = "server=" & MySQLServer & ";Port=" & MySQLPort & ";Database=" & MySQLDatabase & ";Uid=" & MySQLUsername & ";Pwd=" & MySQLPassword & ";Check Parameters=false;default command timeout=999;Connection Timeout=999;Pooling=false;allow user variables=true;"
        Dim truncateSQL = "TRUNCATE TABLE tasks"
        Dim SQLConnection = New MySql.Data.MySqlClient.MySqlConnection(MySQLConnString)
        SQLConnection.Open()
        Dim SQLCommand1 As New MySql.Data.MySqlClient.MySqlCommand(truncateSQL, SQLConnection)
        SQLCommand1.ExecuteNonQuery()
        StatusLog("Table truncated")
    End Sub
    Private Async Sub GetHostTasks(host As String, ip As String, port As Integer, password As String, MySQLServer As String, MySQLPort As Integer, MySQLDatabase As String, MySQLUsername As String, MySQLPassword As String)
        StatusLog("Getting Tasks for host " & host)
        Dim MySQLConnString = "server=" & MySQLServer & ";Port=" & MySQLPort & ";Database=" & MySQLDatabase & ";Uid=" & MySQLUsername & ";Pwd=" & MySQLPassword & ";Check Parameters=false;default command timeout=999;Connection Timeout=999;Pooling=false;allow user variables=true;"
        Dim BOINCClient As New RpcClient
        Try
            Await BOINCClient.ConnectAsync(ip, port)
            Dim Authorized As Boolean = Await BOINCClient.AuthorizeAsync(password)
            Dim SQLInsert As String = ""
            If Authorized Then
                For Each result In Await BOINCClient.GetResultsAsync()
                    Dim Percent As Double
                    Dim Status As String = ""
                    If result.ActiveTask = True And result.ReadyToReport = False Then
                        If String.IsNullOrEmpty(result.PlanClass) Then
                            Status = "Running"
                        Else
                            Status = "Running (" & result.PlanClass & ")"
                        End If
                        Percent = result.FractionDone * 100
                    ElseIf result.ActiveTask = False And result.ReadyToReport = False Then
                        If String.IsNullOrEmpty(result.PlanClass) Then
                            Status = "Ready to start"
                        Else
                            Status = "Ready to start (" & result.PlanClass & ")"
                        End If
                        Percent = 0
                    ElseIf result.ActiveTask = False And result.ReadyToReport = True Then
                        If String.IsNullOrEmpty(result.PlanClass) Then
                            Status = "Ready to report"
                        Else
                            Status = "Ready to report (" & result.PlanClass & ")"
                        End If
                        Percent = 100
                    End If
                    Dim ElapsedTime As TimeSpan
                    If result.ElapsedTime.TotalMilliseconds = 0 Then
                        ElapsedTime = result.FinalElapsedTime
                    Else
                        ElapsedTime = result.ElapsedTime
                    End If

                    Dim RemainingTime As TimeSpan = result.EstimatedCpuTimeRemaining
                    Dim Project As String = ""
                    If result.ProjectUrl = "http://www.worldcommunitygrid.org/" Or result.ProjectUrl = "https://www.worldcommunitygrid.org/" Then
                        Project = "World Community Grid"
                    ElseIf result.ProjectUrl = "https://boinc.thesonntags.com/collatz/" Or result.ProjectUrl = "http://boinc.thesonntags.com/collatz/" Then
                        Project = "Collatz Conjecture"
                    ElseIf result.ProjectUrl = "https://wuprop.boinc-af.org/" Or result.ProjectUrl = "http://wuprop.boinc-af.org/" Then
                        Project = "WUProp@Home"
                    ElseIf result.ProjectUrl = "https://moowrap.net/" Or result.ProjectUrl = "http://moowrap.net/" Then
                        Project = "Moo! Wrapper"
                    ElseIf result.ProjectUrl = "http://www.bitcoinutopia.net/bitcoinutopia/" Or result.ProjectUrl = "http://bitcoinutopia.net/bitcoinutopia/" Then
                        Project = "Bitcoin Utopia"
                    ElseIf result.ProjectUrl = "http://setiathome.berkeley.edu/" Then
                        Project = "SETI@Home"
                    ElseIf result.ProjectUrl = "http://asteroidsathome.net/boinc/" Then
                        Project = "Asteroids@Home"
                    ElseIf result.ProjectUrl = "http://goofyxgridathome.net/" Then
                        Project = "GoofyxGrid@Home"
                    ElseIf result.ProjectUrl = "http://cpu.goofyxgridathome.net/" Then
                        Project = "GoofyxGrid@Home CPU"
                    ElseIf result.ProjectUrl = "http://finance.gridcoin.us/finance/" Then
                        Project = "Gridcoin Finance"
                    Else
                        Project = result.ProjectUrl
                    End If
                    SQLInsert += "INSERT INTO tasks (TaskName, Project, PercentDone, Status, PCName, ElapsedTime, RemainingTime, ReportDeadline) VALUES ('" & result.WorkunitName & "', '" & Project & "', '" & Percent & "', '" & Status & "', '" & host & "', '" & String.Format("{0}:{1:mm}:{1:ss}", CInt(Math.Truncate(ElapsedTime.TotalHours)), ElapsedTime) & "', '" & String.Format("{0}:{1:mm}:{1:ss}", CInt(Math.Truncate(RemainingTime.TotalHours)), RemainingTime) & "', '" & result.ReportDeadline.ToString("MM/dd/yyyy hh:mm:ss tt") & " UTC');"
                Next
            End If
            Dim SQLConnection = New MySql.Data.MySqlClient.MySqlConnection(MySQLConnString)
            SQLConnection.Open()
            Dim SQLCommand As New MySql.Data.MySqlClient.MySqlCommand(SQLInsert, SQLConnection)
            SQLCommand.ExecuteNonQuery()
            StatusLog("Finished getting tasks for host " & host)
        Catch ex As Exception
            StatusLog("Failed getting tasks for host " & host)
        End Try
    End Sub
End Class