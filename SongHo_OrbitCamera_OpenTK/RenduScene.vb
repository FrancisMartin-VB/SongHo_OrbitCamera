Imports OpenTK.Mathematics
Imports OpenTK.WinForms
Imports OpenTK.Windowing

''' <summary> Evenements et procédures du formulaire </summary>
Friend Class RenduScene
    Inherits Form
#Region "Variables"
    'concernant les rendu OpenGL
    Private TailleRendu As Size
    Private Rendu3emePerson, RenduPointDeVue As GLControl
    'pour éviter la réentrance lors de la mise à jour des trackbars
    Private FlagEvenement As Boolean
    'tout ce qui concerne l'affichage des dessins
    Private ReadOnly Scene As SongHo_OrbitCamera
    'stockage des différentes valeurs combinées recues ou envoyées à la scène
    Private Position, Target, Angle As Vector3
#End Region
#Region "Constructeur du formulaire"
    Friend Sub New()
        'partie liée au concepteur du WindowsForms la seule modification concerne 
        'les variables qui sont sans le déclarateur WithEvents
        InitializeComponent()
        'instanciation et initialisation des variables de la scene hors opengl 
        Scene = SongHo_OrbitCamera.Instance
        'récupération des valeurs de la camera2
        UpdateValeursCameraPointDeVue()
        ShowFOV.Checked = Scene.IsFov
        ShowGRID.Checked = Scene.IsGrid
        'ajout des 2 GLControls et initialision de leur contexte Opengl
        'En NET 5.0 chaque controles a son propre contexte y compris pour les objets OpenGL
        AjouterRendusOpenGL()
        'ajouter les évenements du formulaire et des controles dont on aura besoin
        AjouterEvenementsFormulaire()
        Text = "OpenTK & VB & Net Core 5.0 --> " & NomContexteOpengl
    End Sub
#End Region
#Region "procédures interaction pour calculer et obtenir les valeurs de la caméra point de vue"
    ''' <summary> inter-action avec la camera du point de vue concernant les angles </summary>
    Private Sub AnglesBar_ValueChanged(sender As Object, e As EventArgs)
        If FlagEvenement Then
            Dim Bar As TrackBar = CType(sender, TrackBar)
            Dim Index As Integer = CInt(Bar.Tag)
            Angle(Index) = Bar.Value
            'recherche du controle à mettre à jour 
            Dim NomBar As String = Bar.Name(3)
            Dim Lab As Label = CType(CameraAngles.Controls("LabelAngle" & NomBar), Label)
            Lab.Text = Angle(Index).ToString("N0")
            Scene.CameraPointOfViewAngle = Angle
            UpdatePositions()
            UpdateMatrice()
            UpdateQuaternion()
        End If
    End Sub
    ''' <summary> inter-action avec la camera du point de vue concernant la position du point de vue   </summary>
    Private Sub PositionsBar_ValueChanged(sender As Object, e As EventArgs)
        If FlagEvenement Then
            Dim Bar As TrackBar = CType(sender, TrackBar)
            Dim Index As Integer = CInt(Bar.Tag)
            Position(Index) = CSng(Bar.Value / 10)
            Dim NomBar As String = Bar.Name(3)
            'recherche du controle à mettre à jour 
            Dim Lab As Label = CType(CameraPositions.Controls("LabelPosition" & NomBar), Label)
            Lab.Text = Position(Index).ToString("N1")
            Scene.CameraPointOfViewPosition = Position
            UpdateAngles()
            UpdateMatrice()
            UpdateQuaternion()
        End If
    End Sub
    ''' <summary> interaction avec la camera du point de vue concernant la position du point de Visée   </summary>
    Private Sub TargetsBar_ValueChanged(sender As Object, e As EventArgs)
        If FlagEvenement Then
            Dim Bar As TrackBar = CType(sender, TrackBar)
            Dim Index As Integer = CInt(Bar.Tag)
            Target(Index) = CSng(Bar.Value / 10)
            Dim NomBar As String = Bar.Name(3)
            'recherche du controle à mettre à jour 
            Dim Lab As Label = CType(CameraTargets.Controls("LabelTarget" & NomBar), Label)
            Lab.Text = Target(Index).ToString("N1")
            Scene.CameraPointOfViewTarget = Target
            UpdatePositions()
            UpdateMatrice()
            UpdateQuaternion()
        End If
    End Sub
    ''' <summary> remise des valeurs des caméras par defaut </summary>
    Private Sub ResetCameras_Click(sender As Object, e As EventArgs)
        Scene.InitialiserCameras()
        UpdateValeursCameraPointDeVue()
    End Sub
    ''' <summary> demande l'affichage ou non de la grille </summary>
    Private Sub ShowGRID_CheckedChanged(sender As Object, e As EventArgs)
        Scene.IsGrid = ShowGRID.Checked
    End Sub
    ''' <summary> demande l'affichage ou non du cone de visualisation </summary>
    Private Sub ShowFOV_CheckedChanged(sender As Object, e As EventArgs) 'Handles ShowFOV.CheckedChanged
        Scene.IsFov = ShowFOV.Checked
    End Sub
    ''' <summary> interaction avec les cameras concernant le cone de visualistion </summary>
    Private Sub FOV_ValueChanged(sender As Object, e As EventArgs)
        If FlagEvenement Then
            Scene.Fov = Fov.Value
        End If
    End Sub
#End Region
#Region "procédures Evenements formulaire"
    ''' <summary> recalcul la dimesnions des rendus et demande le dessin </summary>
    Private Sub RenduScene_Resize(sender As Object, e As EventArgs) 'Handles MyBase.Resize
        TailleRendu = New Size(ClientSize.Width \ 2, ClientSize.Height - 150)
        Rendu3emePerson.Size = TailleRendu
        RenduPointDeVue.Location = New Point(TailleRendu.Width, 0)
        RenduPointDeVue.Size = TailleRendu
        Scene.SizeRendu = TailleRendu
    End Sub
    ''' <summary> pour quitter ou pour agrandir et normalisée la fenêtre </summary>
    Private Sub RenduScene_KeyDown(sender As Object, e As KeyEventArgs) 'Handles MyBase.KeyDown
        Select Case e.KeyCode
            Case Keys.Escape
                Close()
            Case Keys.F11
                If WindowState = FormWindowState.Maximized Then
                    WindowState = FormWindowState.Normal
                Else
                    WindowState = FormWindowState.Maximized
                End If
            Case Keys.F
                Scene.ChangerRenduPolygones()
        End Select
    End Sub
    ''' <summary> si il faut détruire des ressources non managées </summary>
    Private Sub RenduScene_Closed(sender As Object, e As FormClosedEventArgs) 'Handles MyBase.FormClosed
        SongHo_OrbitCamera.Close()
    End Sub
    ''' <summary> indique la taille des renduGL suite à la fin de l'initialisation du formulaire
    ''' et affiche la scène sur le formulaire qui est ouvert </summary>
    Private Sub RenduScene_Shown(sender As Object, e As EventArgs) 'Handles MyBase.Shown
        Scene.SizeRendu = TailleRendu
    End Sub
#End Region
#Region "Deplacement de la caméra1"
    ''' <summary> interaction avec la souris si les boutons gauche ou droit sont appuyés </summary>
    Private Sub Rendu3emePerson_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left OrElse e.Button = MouseButtons.Right Then
            Scene.MousePosition = e.Location
        End If
    End Sub
    ''' <summary> interaction avec le déplacement de la souris si les boutons gauche ou droit sont appuyés </summary>
    Private Sub Rendu3emePerson_MouseMove(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            Scene.RotateCamera3rdPerson(e.Location)
        ElseIf e.Button = MouseButtons.Right Then
            Scene.ZoomCamera3rdPersonBouton(e.Y)
        End If
    End Sub
    ''' <summary> interaction avec la molette de souris </summary>
    Private Sub Rendu3emePerson_MouseWheel(sender As Object, e As MouseEventArgs)
        Scene.ZoomCamera3rdPersonMolette(e.Delta / 120.0F)
    End Sub
#End Region
#Region "procedures Privées"
    '''<summary> Ajoute les 2 controles GL au formulaire car on ne peut pas le faire directement dans le concepteur de windowsForms
    ''' le choix fait pour ce programme est de ne pas avoir d'évenement autre que ceux de la souris afin de bien marquer la séparation
    ''' entre l'interface utilisateur et la partie dessin </summary>
    Private Sub AjouterRendusOpenGL()
        TailleRendu = New Size(ClientSize.Width \ 2, ClientSize.Height - 150)
        'RenduOpenGL_1. Chaque rendu doit initialiser son contexte
        Rendu3emePerson = New GLControl() With {
            .API = Common.ContextAPI.OpenGL,
            .APIVersion = New Version(3, 3, 0, 0),
            .Flags = Common.ContextFlags.Default,
            .IsEventDriven = True,
            .Profile = Common.ContextProfile.Compatability,
            .Name = "Rendu3emePerson",
            .Location = New Point(0, 0),
            .Size = TailleRendu}
        Scene.InitialiserRenduOpenGL(Rendu3emePerson, ScreenId.R3d, Color.Black)
        Controls.Add(Rendu3emePerson)

        'RenduOpenGL_2. Chaque rendu doit initialiser son contexte
        RenduPointDeVue = New GLControl() With {
            .API = Common.ContextAPI.OpenGL,
            .APIVersion = New Version(3, 3, 0, 0),
            .Flags = Common.ContextFlags.Default,
            .IsEventDriven = True,
            .Profile = Common.ContextProfile.Compatability,
            .Name = "RenduPointDeVue",
            .Location = New Point(TailleRendu.Width, 0),
            .Size = TailleRendu}
        Scene.InitialiserRenduOpenGL(RenduPointDeVue, ScreenId.Pov, Color.Black)
        Controls.Add(RenduPointDeVue)
    End Sub
    ''' <summary> ajoute les évenements du formulaire et des ces controles. Pour les Rendus Opengl juste la souris sur le 3rdPerson </summary>
    Private Sub AjouterEvenementsFormulaire()
        'Evenements du formulaire
        AddHandler Resize, New EventHandler(AddressOf RenduScene_Resize)
        AddHandler Shown, New EventHandler(AddressOf RenduScene_Shown)
        AddHandler KeyDown, New KeyEventHandler(AddressOf RenduScene_KeyDown)
        AddHandler FormClosed, New FormClosedEventHandler(AddressOf RenduScene_Closed)
        'Rendu camera3rdPerson, uniquement la souris
        AddHandler Rendu3emePerson.MouseDown, New MouseEventHandler(AddressOf Rendu3emePerson_MouseDown)
        AddHandler Rendu3emePerson.MouseMove, New MouseEventHandler(AddressOf Rendu3emePerson_MouseMove)
        AddHandler Rendu3emePerson.MouseWheel, New MouseEventHandler(AddressOf Rendu3emePerson_MouseWheel)
        'TrackBar concernant les angles
        AddHandler BarPitch.ValueChanged, New EventHandler(AddressOf AnglesBar_ValueChanged)
        AddHandler BarYaw.ValueChanged, New EventHandler(AddressOf AnglesBar_ValueChanged)
        AddHandler BarRoll.ValueChanged, New EventHandler(AddressOf AnglesBar_ValueChanged)
        'TrackBar concernant la position de la caméra
        AddHandler BarXPosition.ValueChanged, New EventHandler(AddressOf PositionsBar_ValueChanged)
        AddHandler BarYPosition.ValueChanged, New EventHandler(AddressOf PositionsBar_ValueChanged)
        AddHandler BarZPosition.ValueChanged, New EventHandler(AddressOf PositionsBar_ValueChanged)
        'TrackBar concernant la position de la caméra
        AddHandler BarXTarget.ValueChanged, New EventHandler(AddressOf TargetsBar_ValueChanged)
        AddHandler BarYTarget.ValueChanged, New EventHandler(AddressOf TargetsBar_ValueChanged)
        AddHandler BarZTarget.ValueChanged, New EventHandler(AddressOf TargetsBar_ValueChanged)
        'autres controles
        AddHandler ResetCamera.Click, New EventHandler(AddressOf ResetCameras_Click)
        AddHandler ShowGRID.CheckedChanged, New EventHandler(AddressOf ShowGRID_CheckedChanged)
        AddHandler ShowFOV.CheckedChanged, New EventHandler(AddressOf ShowFOV_CheckedChanged)
        AddHandler Fov.ValueChanged, New EventHandler(AddressOf FOV_ValueChanged)
    End Sub
    ''' <summary> initialise les caméras avec leurs valeurs par défaut. 
    ''' Met les controles associés aux valeurs en concordance </summary>
    Private Sub UpdateValeursCameraPointDeVue()
        UpdatePositions()
        UpdateTargets()
        UpdateAngles()
        UpdateFov()
        UpdateMatrice()
        UpdateQuaternion()
    End Sub
    ''' <summary> met à jour l'angle de vue des caméras </summary>
    Private Sub UpdateFov()
        FlagEvenement = False
        Fov.Value = Scene.Fov
        FlagEvenement = True
    End Sub
    ''' <summary> met à jour les 4 valeurs pour le quaternion correspondant à la matrice de rotation du point de vue </summary>
    Private Sub UpdateQuaternion()
        Dim Quaternion = Scene.CameraPointOfViewQuaternion
        QuatS.Text = Quaternion.W.ToString("N3")
        QuatX.Text = Quaternion.X.ToString("N3")
        QuatY.Text = Quaternion.Y.ToString("N3")
        QuatZ.Text = Quaternion.Z.ToString("N3")
    End Sub
    ''' <summary> met à jour les valeurs de la matrice point de vue </summary>
    Private Sub UpdateMatrice()
        Dim Matrice = Scene.CameraPointOfViewMatrix
        Dim Index As Integer = 0
        ' la disposition des labels sur le formulaire est inverse de la disposition interne d'OpenTK 
        ' pour avoir une représentation en colonne majeur
        For Row As Integer = 1 To 4
            For Col As Integer = 1 To 4
                'code fonctionnant dans tous les cas. acces par le nom du controle
                'CameraMatrix.Controls("M" & Row & Col).Text = Matrice(Row - 1, Col - 1).ToString("N3")
                'code fonctionnant pour un ajout des labels sur le groupe CameraMatrix dans l'ordre voulu
                CameraMatrix.Controls(Index).Text = Matrice(Row - 1, Col - 1).ToString("N3")
                Index += 1
            Next
        Next
    End Sub
    ''' <summary> met à jour les 3 tackbar3 de la position du point de vue </summary>
    Private Sub UpdatePositions()
        Position = Scene.CameraPointOfViewPosition
        FlagEvenement = False
        ClampTrackBarValue(Position.X * 10, BarXPosition)
        LabelPositionX.Text = Position.X.ToString("N1")
        ClampTrackBarValue(Position.Y * 10, BarYPosition)
        LabelPositionY.Text = Position.Y.ToString("N1")
        ClampTrackBarValue(Position.Z * 10, BarZPosition)
        LabelPositionZ.Text = Position.Z.ToString("N1")
        FlagEvenement = True
    End Sub
    ''' <summary> met à jour les 3 tackbar3 de la position du point ciblé </summary>
    Private Sub UpdateTargets()
        Target = Scene.CameraPointOfViewTarget
        FlagEvenement = False
        ClampTrackBarValue(Target.X * 10, BarXTarget)
        LabelTargetX.Text = Target.X.ToString("N1")
        ClampTrackBarValue(Target.Y * 10, BarYTarget)
        LabelTargetY.Text = Target.Y.ToString("N1")
        ClampTrackBarValue(Target.Z * 10, BarZTarget)
        LabelTargetZ.Text = Target.Z.ToString("N1")
        FlagEvenement = True
    End Sub
    ''' <summary> met à jour les 3 tackbar3 des angles </summary>
    Private Sub UpdateAngles()
        Angle = Scene.CameraPointOfViewAngle
        FlagEvenement = False
        BarPitch.Value = CInt(Angle.X)
        LabelAngleP.Text = Angle.X.ToString("N0")
        BarYaw.Value = CInt(Angle.Y)
        LabelAngleY.Text = Angle.Y.ToString("N0")
        BarRoll.Value = CInt(Angle.Z)
        LabelAngleR.Text = Angle.Z.ToString("N0")
        FlagEvenement = True
    End Sub
    ''' <summary> Evite les erreurs lorsque les valeurs renvoyées dépassent les limites autorisées du trackbar </summary>
    Private Shared Sub ClampTrackBarValue(Value As Single, T As TrackBar)
        Dim V As Integer = CInt(Math.Truncate(Value))

        If V < T.Minimum Then
            T.Value = T.Minimum
        ElseIf V > T.Maximum Then
            T.Value = T.Maximum
        Else
            T.Value = V
        End If
    End Sub
#End Region
End Class