Imports System.Net.NetworkInformation, Microsoft.Win32
Public Class trMon
    Dim g As Graphics
    Dim newRK As RegistryKey
    Dim trs As Long = 0, trsOldL As Long
    Dim trsOld As String
    Private Sub tmr_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr.Tick
        loadData()
    End Sub
    Sub loadData()
        g = pb.CreateGraphics()
        Dim ipv4Stats As IPv4InterfaceStatistics
        Dim strUnit As String = "B"

        For Each network As NetworkInterface _
            In NetworkInterface.GetAllNetworkInterfaces()

            If network.Name.Contains("Wireless") And Not network.Name.Contains("2") Then
                ipv4Stats = network.GetIPv4Statistics()
                trs = ipv4Stats.BytesReceived + ipv4Stats.BytesSent + trsOldL

                If trs > 1024 Then
                    trs = trs / 1024
                    strUnit = "KB"
                    If trs > 1024 Then
                        trs = trs / 1024
                        strUnit = "MB"
                        If trs > 20480 Then
                            trs = trs / 1024
                            strUnit = "GB"
                        End If
                    End If
                End If
                g.Clear(Color.Brown)
                g.DrawString("Total Data: " + trs.ToString + strUnit, _
                             New Font("Arial", 12), Brushes.Black, 5, 5)
                newRK.SetValue("Data", trs.ToString() + strUnit)
            End If
        Next

    End Sub
   
    Private Sub trMon_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim tempKey As RegistryKey
        trsOldL = 0


        tempKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, _
                                              "").OpenSubKey("Software", True)
        Try
            newRK = tempKey.OpenSubKey("WIFINetworkMonitor", True)
            trsOld = newRK.GetValue("Data").ToString()
        Catch ex As Exception
            newRK = tempKey.CreateSubKey("WIFINetworkMonitor")
        End Try

        Try
            Select Case trsOld.Substring(trsOld.Length - 2)
                Case "GB"
                    Try
                        trsOldL = Integer.Parse(trsOld.Substring(0, _
                                trsOld.Length - 2)) * 1024 * 1024 * 1024
                    Catch ex As Exception
                        MsgBox("Your data bundle has exeeded 100GB. Resetting configuration")
                        trsOldL = 0
                    End Try

                Case "MB"
                    Try
                        trsOldL = Integer.Parse(trsOld.Substring(0, _
                                trsOld.Length - 2)) * 1024 * 1024
                    Catch ex As Exception
                        MsgBox("Your data bundle has exeeded 100GB. Resetting configuration")
                        trsOldL = 0
                    End Try
                Case "KB"
                    trsOldL = Integer.Parse(trsOld.Substring(0, _
                            trsOld.Length - 2)) * 1024
                Case "KB"
                    trsOldL = Integer.Parse(trsOld.Substring(0, _
                            trsOld.Length - 2))
            End Select

        Catch ex As Exception

        End Try

        tmr.Interval = 10000
        tmr.Enabled = True
    End Sub

    Private Sub pb_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles pb.DoubleClick
        Me.Close()
    End Sub

    Private Sub pb_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pb.Click
        Me.SetDesktopLocation(0, 0)
    End Sub
End Class
