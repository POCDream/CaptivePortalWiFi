'! This is NOT production code and is simply a proof of concept 
'Requires additinal thread to keep checking for 
Imports System.Runtime.InteropServices
Public Class CaptiveWiFi
    'Attempts to download txt file from MS to check for external internet access if this succeeds then close the app else fire up the browser
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Threading.Thread.Sleep(3000)
        RefreshIESettings()
        Me.Size = New Size(1024, 768)
        Dim webClient As New System.Net.WebClient
        Dim result As String = ""
        Try
            result = webClient.DownloadString("http://www.msftncsi.com/ncsi.txt")
        Catch ex As Exception
            result = "Fail"
        End Try
        If result = "Microsoft NCSI" Then
            Me.Close()
        Else

            Navigate("http://www.msftconnecttest.com/redirect")
        End If
    End Sub

    'Reference: https://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.url(v=vs.110).aspx
    Private Sub Navigate(ByVal address As String)
        If String.IsNullOrEmpty(address) Then Return
        If address.Equals("about:blank") Then Return
        If Not address.StartsWith("http://") And
            Not address.StartsWith("https://") Then
            address = "http://" & address
        End If
        Try
            WebBrowser1.Navigate(New Uri(address))
        Catch ex As System.UriFormatException
            Return
        End Try

    End Sub
    'Close if redirected to MSN.com as this is where http://www.msftconnecttest.com/redirect directs to when it has an internet connection
    Private Sub webBrowser1_Navigated(ByVal sender As Object,
    ByVal e As WebBrowserNavigatedEventArgs) _
    Handles WebBrowser1.Navigated
        Dim RedirectUrl = WebBrowser1.Url.ToString()
        If RedirectUrl Like "*msn.com*" Then
            Me.Close()
        End If
    End Sub
    'Clear Proxy
    'Source: https://stackoverflow.com/questions/11954326/how-to-change-the-proxy-of-the-webbrowser
    <Runtime.InteropServices.DllImport("wininet.dll", SetLastError:=True)>
    Private Shared Function InternetSetOption(ByVal hInternet As IntPtr, ByVal dwOption As Integer, ByVal lpBuffer As IntPtr, ByVal lpdwBufferLength As Integer) As Boolean
    End Function
    Public Structure Struct_INTERNET_PROXY_INFO
        Public dwAccessType As Integer
        Public proxy As IntPtr
        Public proxyBypass As IntPtr
    End Structure
    Private Sub RefreshIESettings()
        Const INTERNET_OPTION_PROXY As Integer = 38
        Const INTERNET_OPEN_TYPE_DIRECT As Integer = 1
        Dim struct_IPI As Struct_INTERNET_PROXY_INFO
        struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT
        struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local")
        Dim intptrStruct As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI))
        Marshal.StructureToPtr(struct_IPI, intptrStruct, True)
        Dim iReturn As Boolean = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, System.Runtime.InteropServices.Marshal.SizeOf(struct_IPI))
    End Sub

End Class
