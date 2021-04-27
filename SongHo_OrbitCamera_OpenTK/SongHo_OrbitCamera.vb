Imports OpenTK.WinForms
Imports OpenTK.Mathematics
Imports OpenTK.Graphics.OpenGL
Imports SongHo_OrbitCamera_OpenTK.Texte 'si on ne veut pas de DLL supplémentaire on intègre le code 
'Imports OpenTK.Texte ' si on ne veut pas trop de code et un peu plus rapide

''' <summary> Classe singleton. On pourrait la remplacer par un module mais en VB
''' le module ne peut pas hériter ou implémenter une interface. 
''' On ne peut instancier la class qu'une seule fois à travers la propriété static readonly Instance </summary>
Friend Class SongHo_OrbitCamera
    Implements IDisposable
#Region "Procédures publiques"
    ''' <summary> initialisation du context OpenGL, 1 fois par GLcontrol </summary>
    ''' <param name="ControlGL"> ControlGL servant à l'affichage d'une des 2 vues </param>
    ''' <param name="BgColor"> Couleur de fond de l'affichage </param>
    ''' <param name="ScreenId"> type d'affichage associé au control </param>
    Friend Sub InitialiserRenduOpenGL(ControlGL As GLControl, ScreenId As ScreenId, BgColor As Color)
        If Rendus(ScreenId) IsNot Nothing Then
            Throw New Exception($"Le rendu de la caméra {ScreenId} a déjà été affecté")
        End If
        Rendus(ScreenId) = ControlGL
        Rendus(ScreenId).MakeCurrent()
        VerifierVersionOpenGL()
        InitialiserEtatsOpenGL(BgColor)
        InitialiserLumieres()
        InitialiserObjetsOpenGL(ScreenId)
    End Sub
    ''' <summary> Création et initialisation des variables des caméras avec des valeurs par défaut </summary>
    Friend Sub InitialiserCameras()
        ' vertices for FOV
        ReDim fovVertices(4)
        ReDim fovNormals(3)
        ' les 2 cameras avec leurs positions respective dans le repère global
        Camera3rdPerson = New OrbitCamera(New Vector3(CAM_DIST * 2, CAM_DIST * 1.5F, CAM_DIST * 2), New Vector3(0, 0, 0))
        CameraPointOfView = New OrbitCamera(New Vector3(0, 0, CAM_DIST), New Vector3(0, 0, 0))
        'rendu des polygones pour l'affichage Point de vue
        RenduPolygone = PolygonMode.Fill
        ' fov
        _fov = FOV_Y
        CalculerFovVertices(_fov)
        'Si la taille du rendu est >0 cela indique qu'openGL est initialisé
        'et que l'on a demandé la réinitialisation donc on dessine la scène avec
        'les valeurs par défaut
        If RenduSize.Width > 0 Then
            CalculerMatriceProjection()
        End If
    End Sub
    ''' <summary> change le rendu des polygone par OpenGL pour l'afichage point de vue </summary>
    Friend Sub ChangerRenduPolygones()
        If RenduPolygone = PolygonMode.Fill Then
            RenduPolygone = PolygonMode.Line
        ElseIf RenduPolygone = PolygonMode.Line Then
            RenduPolygone = PolygonMode.Point
        Else
            RenduPolygone = PolygonMode.Fill
        End If
        DessinerScene(Affichage.PoV)
    End Sub
    ''' <summary> ne sert pas ? </summary>
    Friend Shared Sub Close()
        _Instance.Dispose()
        _Instance = Nothing
    End Sub
#End Region
#Region "Propriétés publiques"
    ''' <summary> renvoie la seule instance possible de la classe </summary>
    Friend Shared ReadOnly Property Instance As SongHo_OrbitCamera
        Get
            If _Instance Is Nothing Then
                _Instance = New SongHo_OrbitCamera()
            End If
            Return _Instance
        End Get
    End Property
    ''' <summary> calcul les 2 matrices de projections, indique à OpenGL la taille 
    ''' des affichages et demande le dessin </summary>
    ''' <param name="Taille"> taille des affichages en pixel </param>
    Friend WriteOnly Property SizeRendu As Size
        Set(value As Size)
            RenduSize = value
            CalculerMatriceProjection()
        End Set
    End Property
#Region "Echange information Caméra du point de vue"
    'Maj de Position
    Friend Property CameraPointOfViewPosition As Vector3
        Get
            Return CameraPointOfView.Position
        End Get
        Set(value As Vector3)
            CameraPointOfView.Position = value
            DessinerScene(Affichage.All)
        End Set
    End Property
    'Maj de Target
    Friend Property CameraPointOfViewTarget As Vector3
        Get
            Return CameraPointOfView.Target
        End Get
        Set(value As Vector3)
            CameraPointOfView.Target = value
            DessinerScene(Affichage.All)
        End Set
    End Property
    'Maj de Angle
    Friend Property CameraPointOfViewAngle() As Vector3
        Get
            Return CameraPointOfView.Angle
        End Get
        Set(value As Vector3)
            CameraPointOfView.Angle = value
            DessinerScene(Affichage.All)
        End Set
    End Property
    ' for FOV
    Friend Property Fov As Single
        Get
            Return _fov
        End Get
        Set(value As Single)
            _fov = value
            CalculerFovVertices(_fov)
            CalculerMatriceProjection()
        End Set
    End Property
    Friend Property IsFov As Boolean
        Get
            Return _IsFov
        End Get
        Set(value As Boolean)
            _IsFov = value
            DessinerScene(Affichage.R3d)
        End Set
    End Property
    ' for grid
    Friend Property IsGrid As Boolean
        Get
            Return _IsGrid
        End Get
        Set(value As Boolean)
            _IsGrid = value
            DessinerScene(Affichage.All)
        End Set
    End Property
    'for quaternion
    Friend ReadOnly Property CameraPointOfViewQuaternion() As Quaternion
        Get
            Return CameraPointOfView.Quaternion
        End Get
    End Property
    'for matrice caméra
    Friend ReadOnly Property CameraPointOfViewMatrix() As Matrix4
        Get
            Return CameraPointOfView.MatrixView
        End Get
    End Property
#End Region
#Region "Echange information Caméra 3rd Person uniquement avec la souris "
    ''' <summary> Boutons Appuyé. Marque la position de la souris au début du déplacement </summary>
    Friend WriteOnly Property MousePosition As Point
        Set(value As Point)
            Mouse = value
        End Set
    End Property
    ''' <summary> deplacement de la souris avec bouton gauche appuyé </summary>
    ''' <param name="location"> position de la souris </param>
    Friend Sub RotateCamera3rdPerson(location As Point)
        Const ANGLE_SCALE As Single = 0.2F
        Dim angle As Vector3 = Camera3rdPerson.Angle()
        angle.Y -= (location.X - Mouse.X) * ANGLE_SCALE
        angle.X += (location.Y - Mouse.Y) * ANGLE_SCALE
        Mouse = location

        ' constrain x angle -90 < x < 90
        If angle.X < -90.0F Then
            angle.X = -90.0F
        ElseIf angle.X > 90.0F Then
            angle.X = 90.0F
        End If

        Camera3rdPerson.Angle = angle
        DessinerScene(Affichage.R3d)
    End Sub
    ''' <summary> deplacement de la souris avec bouton droit appuyé </summary>
    ''' <param name="location"> position de la souris </param>
    Friend Sub ZoomCamera3rdPersonBouton(Y As Integer)
        Dim delta As Single = CSng(Y) - Mouse.Y
        Mouse.Y = Y
        ZoomCamera3rdPersonMolette(delta)
    End Sub
    ''' <summary> rotation de la molette de la souris </summary>
    ''' <param name="delta"> nb de cran de la molette </param>
    Friend Sub ZoomCamera3rdPersonMolette(delta As Single)
        Const ZOOM_SCALE As Single = 0.5F
        Const MIN_DIST As Single = 1.0F
        Const MAX_DIST As Single = 30.0F
        Dim distance As Single = Camera3rdPerson.Distance()
        distance -= (delta * ZOOM_SCALE)

        ' constrain min and max
        If distance < MIN_DIST Then
            distance = MIN_DIST
        ElseIf distance > MAX_DIST Then
            distance = MAX_DIST
        End If

        Camera3rdPerson.Distance = distance
        DessinerScene(Affichage.R3d)
    End Sub
#End Region
#End Region
#Region "Procédures internes"
    <Flags>
    Private Enum Affichage As Byte
        Aucun = 0
        R3d = 1
        PoV = 2
        All = 3
    End Enum
    ''' <summary> initialise toutes les variables qui ne dépendent pas d'un contexte OpenGL </summary>
    Private Sub New()
        _IsFov = True
        _IsGrid = True
        gridSize = GRID_SIZE
        gridStep = GRID_STEP
        'nous avons 2 affichages, et chaque affichage a son propre context Opengl et ses propres objets
        'le canard sera affiché sur les 2 affichages donc il faut prévoir 2 VBOs, IBOs disctincts
        objModel = New ObjModel(2)
        If Not objModel.Read(OBJ_MODEL) Then
            Throw New Exception("Erreur de lecture de l'objet Canard : " & objModel.ErrorMessage)
        End If
        'la camera ne sera affiché que sur l'affichage 3rd donc il faut prévoir 1 seul VBO, IBO
        objCam = New ObjModel(1)
        If Not objCam.Read(OBJ_CAM) Then
            Throw New Exception("Erreur de lecture de l'objet Caméra : " & objCam.ErrorMessage)
        End If
        ' couleur principale de l'objet
        ReDim Ambient(1)
        Ambient(ScreenId.R3d) = New Color4(0.8F, 0.6F, 0.5F, 0.5F)
        Ambient(ScreenId.Pov) = New Color4(0.7F, 0.5F, 0.2F, 0.0F)
        ReDim Diffuse(1)
        Diffuse(ScreenId.R3d) = New Color4(1.0F, 0.9F, 0.2F, 1.0F)
        Diffuse(ScreenId.Pov) = New Color4(0.9F, 0.9F, 0.9F, 1.0F)
        'refléchissant ou absorbant
        ReDim Specular(1)
        Specular(ScreenId.R3d) = New Color4(1.0F, 1.0F, 1.0F, 1.0F)
        Specular(ScreenId.Pov) = New Color4(0.5F, 0.5F, 0.5F, 1.0F)
        'intensité de la lumière
        ReDim Shininess(1)
        Shininess(ScreenId.R3d) = 128.0F
        Shininess(ScreenId.Pov) = 256
        'polices des informations
        font = New Font("Segoe UI", 18, FontStyle.Regular)
        'textes de informations
        ReDim Information(1)
        Information(ScreenId.R3d) = "3rd" & CrLf & "Person" & CrLf & "View" '"3rd Person View" '
        Information(ScreenId.Pov) = "Point" & CrLf & "of" & CrLf & "View" '"Point of View" '
        'variables qui ont besoin d'un contexte opengl pour être initialisées
        ReDim Rendus(1)
        ReDim ProgId(1)
        ReDim Text(1)
        ReDim TailleInformation(1)
        'initialise les caméras avec leurs valeurs par défaut
        InitialiserCameras()
    End Sub
    ''' <summary> libère les ressources non managée si il y en a </summary>
    Private Sub Dispose() Implements IDisposable.Dispose
    End Sub
    ''' <summary> calcul les 2 matrices de projection et redessine la scène </summary>
    Private Sub CalculerMatriceProjection()
        matrixProjection = Matrix4.CreatePerspectiveFieldOfView(_fov * DegToRad, CSng(RenduSize.Width / RenduSize.Height), NEAR_PLANE, FAR_PLANE)
        DessinerScene(Affichage.All, True)
    End Sub
    ''' <summary> permet de choisir le rendu à dessiner </summary>
    ''' <param name="Affichage"> Camera à dessiner </param>
    Private Sub DessinerScene(Affichage As Affichage, Optional InitViewPort As Boolean = False)
        If Affichage.HasFlag(Affichage.R3d) Then
            DessinerRendu3rdPerson(InitViewPort)
        End If
        If Affichage.HasFlag(Affichage.PoV) Then
            DessinerRenduPointOfView(InitViewPort)
        End If
    End Sub
    ''' <summary> calcule les sommets de la pyramide de visualisation </summary>
    ''' <param name="fov"> angle de visualisation en degré </param>
    Private Sub CalculerFovVertices(fov As Single)
        Const DIST As Single = 11.0F
        Const ratio As Single = 1.0F
        Dim halfFov As Single = fov * 0.5F * DegToRad
        Dim TanFovDist As Single = CSng(Math.Tan(halfFov)) * DIST
        Dim TanFovRatioDist As Single = CSng(Math.Tan(halfFov * ratio)) * DIST

        ' compute 5 vertices of the fov
        ' origin
        fovVertices(0).X = 0
        fovVertices(0).Y = 0
        fovVertices(0).Z = 0

        ' top-left
        fovVertices(1).X = TanFovRatioDist
        fovVertices(1).Y = TanFovDist
        fovVertices(1).Z = DIST

        ' top-right
        fovVertices(2).X = -TanFovRatioDist
        fovVertices(2).Y = TanFovDist
        fovVertices(2).Z = DIST

        ' bottom-left
        fovVertices(3).X = TanFovRatioDist
        fovVertices(3).Y = -TanFovDist
        fovVertices(3).Z = DIST

        ' bottom-right
        fovVertices(4).X = -TanFovRatioDist
        fovVertices(4).Y = -TanFovDist
        fovVertices(4).Z = DIST

        ' compute normals
        ' top
        fovNormals(0) = Vector3.Cross(fovVertices(2) - fovVertices(0), fovVertices(1) - fovVertices(0))
        fovNormals(0).Normalize()
        ' bottom
        fovNormals(1) = Vector3.Cross(fovVertices(3) - fovVertices(0), fovVertices(4) - fovVertices(0))
        fovNormals(1).Normalize()
        ' left
        fovNormals(2) = Vector3.Cross(fovVertices(1) - fovVertices(0), fovVertices(3) - fovVertices(0))
        fovNormals(2).Normalize()
        ' right
        fovNormals(3) = Vector3.Cross(fovVertices(4) - fovVertices(0), fovVertices(2) - fovVertices(0))
        fovNormals(3).Normalize()
    End Sub
    ''' <summary> Initialise tous les variables qui dépendent d'opengl et sont associées à un contexte OpenGL </summary>
    Private Sub InitialiserObjetsOpenGL(ScreenId As ScreenId)
        'permet l'écriture des information sur l'affichage
        Text(ScreenId) = New TexteGL(TextQuality.High, Rendus(ScreenId).Context)
        'pas obligatoire mais permet le placement du texte où l'on veut
        TailleInformation(ScreenId) = Text(ScreenId).Mesurer(Information(ScreenId), font, New RectangleF(New PointF(5, 0), SizeF.Empty)).BoundingBox
        Dim MessageErreur As String = ""
        'les 2 affichages dessine des VBOs
        If Not CreerShader(ProgId(ScreenId), MessageErreur) Then
            Throw New Exception("Erreur d'initialisation Shader : " & MessageErreur)
        End If
        'le canard est dessiné sur chaque affichage
        If Not CreerVBO(objModel, ScreenId) Then
            Throw New Exception("Erreur d'initialisation Objet Canard : " & objModel.ErrorMessage)
        End If
        'la caméra n'est dessinée que sur le rendu 3ème person
        If ScreenId = ScreenId.R3d Then
            If Not CreerVBO(objCam, ScreenId) Then
                Throw New Exception("Erreur d'initialisation Objet Camera " & objCam.ErrorMessage)
            End If
        End If
    End Sub
    ''' <summary> cherche les procedures opengl exposées par le contexte </summary>
    Private Shared Sub VerifierVersionOpenGL()
        Dim vboSupported = IsSupported("GL_ARB_vertex_buffer_object")
        Dim glslSupported = IsSupported("GL_ARB_shader_objects")
        If Not vboSupported OrElse Not glslSupported Then
            Throw New Exception("Votre carte graphique est trop ancienne pour ce programme")
        End If
    End Sub
#End Region
#Region "Variables"
    ' interaction members pour Viewport et camera3rd
    Private RenduSize As Size, Mouse As Point
    ' obj
    Private ReadOnly objModel, objCam As ObjModel
    ' cameras et rendu
    Private Camera3rdPerson, CameraPointOfView As OrbitCamera ' for rendus
    Private ReadOnly Rendus() As GLControl
    ' 4x4 projection matrices
    Private matrixProjection As Matrix4
    'rendu polygone
    Private RenduPolygone As PolygonMode
    ' shader program color ou program with color + lighting
    Private ReadOnly ProgId() As Integer
    ' material
    Private ReadOnly Ambient(), Diffuse(), Specular() As Color4
    Private ReadOnly Shininess() As Single
    ' FOV
    Private fovVertices(), fovNormals() As Vector3
    Private _IsFov As Boolean, _fov As Single
    'Grid
    Private _IsGrid As Boolean
    Private ReadOnly gridSize, gridStep As Integer ' step for next grid line
    ' écriture du texte
    Private ReadOnly font As Font
    Private ReadOnly Text() As TexteGL
    Private ReadOnly Information() As String
    Private ReadOnly TailleInformation() As RectangleF
    ' scene
    Private Shared _Instance As SongHo_OrbitCamera
#End Region
#Region "Constantes"
    Const GRID_SIZE As Integer = 10
    Const GRID_STEP As Integer = 1
    Const CAM_DIST As Single = 5.0F
    Const FOV_Y As Single = 50.0F
    Const NEAR_PLANE As Single = 0.1F
    Const FAR_PLANE As Single = 100.0F
    Const OBJ_MODEL As String = "data/canard.obj"
    Const OBJ_CAM As String = "data/camera.obj"

    ' blinn shading ==========================================
    Const vsSource As String = "
            varying vec3 esVertex, esNormal;
            void main()
            {
                esVertex = vec3(gl_ModelViewMatrix * gl_Vertex);
                esNormal = gl_NormalMatrix * gl_Normal;
                gl_FrontColor = gl_Color;
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            }
            "
    Const fsSource As String = "
            varying vec3 esVertex, esNormal;
            void main()
            {
                vec3 normal = normalize(esNormal);
                vec3 light;
                if(gl_LightSource[0].position.w == 0.0)
                {
                    light = normalize(gl_LightSource[0].position.xyz);
                }
                else
                {
                    light = normalize(gl_LightSource[0].position.xyz - esVertex);
                }
                vec3 view = normalize(-esVertex);
                vec3 halfv = normalize(light + view);
                vec4 color =  gl_FrontMaterial.ambient * gl_FrontLightProduct[0].ambient;
                float dotNL = max(dot(normal, light), 0.0);
                color += gl_FrontMaterial.diffuse * gl_FrontLightProduct[0].diffuse * dotNL;
                float dotNH = max(dot(normal, halfv), 0.0);
            //    vec4 specular = (vec4(1.0) - color) * gl_FrontMaterial.specular * gl_FrontLightProduct[0].specular * pow(dotNH, gl_FrontMaterial.shininess);
            //    color += specular;
                color += gl_FrontMaterial.specular * gl_FrontLightProduct[0].specular * pow(dotNH, gl_FrontMaterial.shininess);
                gl_FragColor = color;
            }
            "
#End Region
End Class

''' <summary> type d'écran associé au controleGL </summary>
Friend Enum ScreenId As Byte
    R3d = 0
    Pov = 1
End Enum