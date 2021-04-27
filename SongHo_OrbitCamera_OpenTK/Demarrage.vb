Imports System.Globalization
Imports System.Threading

''' <summary> une seule sub en remplacement de l'initialisation de l'application par VB.
''' Correspondance directe avec C# </summary>
Friend Module Demarrage

    '''<summary>GUID unique par application ou nom de l'assembly 
    '''Permet de remplacer l'option Instance unique des projets VB en relation avec l'utilisation des Mutex</summary>
    'Const AppID As String = "91c8bb29-1d5f-43fe-8363-e141eb45c01c"
    Const AppID As String = "SongHo_OrbitCamera_OpenTK"
    ''' <summary> Point d'entrée principal de l'application. </summary>
    <STAThread>
    Friend Sub Main()
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture
        Using mutex As Mutex = New Mutex(False, AppID)
            If mutex.WaitOne(0) Then
                Call Application.EnableVisualStyles()
                Application.SetCompatibleTextRenderingDefault(False)
                Call Application.Run(New RenduScene())
            End If
        End Using
    End Sub

End Module