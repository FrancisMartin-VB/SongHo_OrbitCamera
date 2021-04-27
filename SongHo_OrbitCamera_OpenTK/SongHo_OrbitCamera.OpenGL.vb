Imports OpenTK.Mathematics
Imports OpenTK.Graphics.OpenGL
''' <summary> partie qui regroupe toutes les procédures avec des commandes OpenGL </summary>
Partial Friend Class SongHo_OrbitCamera
    ''' <summary> dessine la scéne à la 3 ème personne </summary>
    ''' <param name="InitViewPort"> indique si le viewport à changer </param>
    Private Sub DessinerRendu3rdPerson(InitViewPort As Boolean)
        Rendus(ScreenId.R3d).MakeCurrent()
        'indique les dimensions du rendu et met la projection perspective
        If InitViewPort Then InitialiserViewPort()
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit Or ClearBufferMask.StencilBufferBit)
        ' matrix for 3rd person camera
        Dim matView As Matrix4 = Camera3rdPerson.MatrixView
        GL.LoadMatrix(matView)
        ' draw grid
        If _IsGrid Then
            DrawGridXZ()
        End If
        ' draw line from position camera Pov to focal point (point de visée)
        DrawFocalLine()
        DrawFocalPoint()
        ' draw obj model
        DrawObjWithVbo(objModel, ScreenId.R3d)
        ' matrix for camera model
        Dim matModel As Matrix4 = CameraPointOfView.GetMatriceModel()
        Dim matModelView As Matrix4 = matModel * matView
        GL.LoadMatrix(matModelView)
        ' draw obj camera
        DrawObjWithVbo(objCam, ScreenId.R3d)
        ' draw pyramide Fov
        If _IsFov Then
            DrawFov()
        End If
        ' draw text
        DrawText(ScreenId.R3d)
        'affiche le dessin
        Rendus(ScreenId.R3d).SwapBuffers()
    End Sub
    ''' <summary> Dessine la scène du Point de Vue </summary>
    ''' <param name="InitViewPort"> indique si le viewport à changer </param>
    Private Sub DessinerRenduPointOfView(InitViewPort As Boolean)
        Rendus(ScreenId.Pov).MakeCurrent()
        'indique les dimensions du rendu et met la projection perspective
        If InitViewPort Then InitialiserViewPort()
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit Or ClearBufferMask.StencilBufferBit)
        ' matrix for camera model
        GL.LoadMatrix(CameraPointOfView.MatrixView)
        ' draw grid
        If _IsGrid Then
            DrawGridXZ()
        End If
        ' draw focal point (point de visée)
        DrawFocalPoint()
        ' draw OBJ model
        GL.PolygonMode(MaterialFace.FrontAndBack, RenduPolygone)
        DrawObjWithVbo(objModel, ScreenId.Pov)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
        ' draw texte
        DrawText(ScreenId.Pov)
        'affiche le dessin
        Rendus(ScreenId.Pov).SwapBuffers()
    End Sub
    ''' <summary> initialise les dimensions d'affichage et la matrice de projection pour le dessin </summary>
    Private Sub InitialiserViewPort()
        GL.Viewport(0, 0, RenduSize.Width, RenduSize.Height)
        GL.MatrixMode(MatrixMode.Projection)
        GL.LoadMatrix(matrixProjection)
        GL.MatrixMode(MatrixMode.Modelview)
    End Sub
    ''' <summary> dessine l'objet camera. OpenGL semi moderne </summary>
    Private Sub DrawObjWithVbo(Obj As ObjModel, ScreenId As ScreenId)
        'configure opengl selon les besoins du model
        GL.UseProgram(ProgId(ScreenId))
        GL.BindBuffer(BufferTarget.ArrayBuffer, Obj.Vbo(ScreenId))
        GL.EnableClientState(ArrayCap.NormalArray)
        GL.EnableClientState(ArrayCap.VertexArray)
        ' before draw, specify vertex and index arrays with their offsets and stride
        Dim stride As Integer = Obj.InterleavedVerticesStride
        GL.NormalPointer(NormalPointerType.Float, stride, TailleSingle * 3)
        GL.VertexPointer(3, VertexPointerType.Float, stride, 0)

        Dim ibo As Integer() = Obj.Ibo(ScreenId).ToArray
        For i As Integer = 0 To ibo.Length - 1
            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, Ambient(Obj.Instance))
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, Diffuse(Obj.Instance))
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, Specular(Obj.Instance))
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, Shininess(Obj.Instance))
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo(i))
            Dim Index As Integer = Obj.GroupIndicesCount(i)
            GL.DrawElements(BeginMode.Triangles, Index, DrawElementsType.UnsignedInt, 0)
        Next i
        'remet opengl comme avant
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)
        GL.DisableClientState(ArrayCap.VertexArray)
        GL.DisableClientState(ArrayCap.NormalArray)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        GL.UseProgram(0)
    End Sub
    ''' <summary> Dessine la ligne de focale  </summary>
    Private Sub DrawFocalLine()
        ' disable lighting
        GL.Disable(EnableCap.Lighting)
        GL.DepthFunc(DepthFunction.Always) ' to avoid visual artifacts with grid lines
        GL.Color4(1.0F, 1.0F, 0.2F, 0.7F)

        GL.Begin(PrimitiveType.Lines)
        GL.Vertex3(CameraPointOfView.Position)
        GL.Vertex3(CameraPointOfView.Target)
        GL.End()

        ' enable lighting back
        GL.DepthFunc(DepthFunction.Lequal)
        GL.Enable(EnableCap.Lighting)
    End Sub
    ''' <summary> Dessine le point visé </summary>
    Private Sub DrawFocalPoint()
        ' disable lighting
        GL.Disable(EnableCap.Lighting)
        GL.DepthFunc(DepthFunction.Always) ' to avoid visual artifacts with grid lines
        GL.PointSize(5.0F)

        GL.Color4(1.0F, 1.0F, 0.2F, 0.7F)
        GL.Begin(PrimitiveType.Points)
        GL.Vertex3(CameraPointOfView.Target)
        GL.End()

        ' enable lighting back
        GL.PointSize(1.0F)
        GL.DepthFunc(DepthFunction.Lequal)
        GL.Enable(EnableCap.Lighting)
    End Sub
    ''' <summary> dessine la pyramide tronquée du frustrum </summary>
    Private Sub DrawFov()
        'draw pyramide face arrière
        GL.CullFace(CullFaceMode.Front)
        GL.LightModel(LightModelParameter.LightModelTwoSide, 1)
        DrawPyramide()

        'draw pyramide face avant
        GL.CullFace(CullFaceMode.Back) 'mode défaut
        GL.LightModel(LightModelParameter.LightModelTwoSide, 0)
        DrawPyramide()

        'draw arrêtes pyramide 
        GL.Disable(EnableCap.CullFace)
        GL.Disable(EnableCap.Lighting)
        GL.Disable(EnableCap.DepthTest)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line)
        GL.LineWidth(0.25F)
        DrawPyramide()

        'restore les etats et les capacités
        GL.LineWidth(1.0F)
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Lighting)
        GL.Enable(EnableCap.CullFace)
    End Sub
    ''' <summary> dessine les 4 faces de la pyramide de visualisation </summary>
    ''' <param name="Alpha"> Coeficient de transparence pour le sommet de la pyaramide </param>
    Private Sub DrawPyramide()
        GL.Begin(PrimitiveType.TriangleFan)
        'semi transparent au sommet 
        GL.Color4(0.5F, 0.5F, 0.5F, 0.5F)
        GL.Vertex3(fovVertices(0))
        'et transparent à la base de la pyramide
        GL.Color4(0.5F, 0.5F, 0.5F, 0.0F)
        ' left
        GL.Vertex3(fovVertices(1))
        GL.Normal3(fovNormals(2))
        GL.Vertex3(fovVertices(3))
        ' bottom
        GL.Normal3(fovNormals(1))
        GL.Vertex3(fovVertices(4))
        ' right
        GL.Normal3(fovNormals(3))
        GL.Vertex3(fovVertices(2))
        ' top
        GL.Normal3(fovNormals(0))
        GL.Vertex3(fovVertices(1))
        GL.End()
    End Sub
    ''' <summary> Dessine une grille sur la plan XZ (horizontal du monde) de taille et avec un pas </summary>
    ''' <param name="size"> taille ou nb de lignes de part et autre des axes </param>
    ''' <param name="pas"> pas entre les ligne </param>
    Private Sub DrawGridXZ() ' draw a grid on XZ plane
        ' disable lighting
        GL.Disable(EnableCap.Lighting)
        GL.LineWidth(0.5F)

        GL.Begin(PrimitiveType.Lines)
        GL.Color4(0.5F, 0.5F, 0.5F, 0.5F)
        For i As Integer = 0 To gridSize Step gridStep
            GL.Vertex3(-gridSize, 0, i) ' lines parallel to X-axis
            GL.Vertex3(gridSize, 0, i)
            GL.Vertex3(-gridSize, 0, -i) ' lines parallel to X-axis
            GL.Vertex3(gridSize, 0, -i)

            GL.Vertex3(i, 0, -gridSize) ' lines parallel to Z-axis
            GL.Vertex3(i, 0, gridSize)
            GL.Vertex3(-i, 0, -gridSize) ' lines parallel to Z-axis
            GL.Vertex3(-i, 0, gridSize)
        Next

        ' x-axis
        GL.Color4(1, 0, 0, 0.5F)
        GL.Vertex3(-gridSize, 0, 0)
        GL.Vertex3(gridSize, 0, 0)

        ' z-axis
        GL.Color4(0, 0, 1, 0.5F)
        GL.Vertex3(0, 0, -gridSize)
        GL.Vertex3(0, 0, gridSize)

        GL.End()

        ' enable lighting back
        GL.LineWidth(1.0F)
        GL.Enable(EnableCap.Lighting)
    End Sub
    ''' <summary> dessine le texte sur les renduOpenGL </summary>
    ''' <param name="ScreenId"> N° du renduOpenGL </param>
    Private Sub DrawText(ScreenId As ScreenId)
        'initialise la matrice modele vue et de projection ortho pour le texte
        Text(ScreenId).Begin()

        ' pour ne pas avoir d'altération de la couleur du texte
        GL.Disable(EnableCap.Lighting)
        Text(ScreenId).Ecrire(Information(ScreenId), font, Color.White, TailleInformation(ScreenId))
        GL.Enable(EnableCap.Lighting)

        'restore la matrice de projection perspective et repasse sur la matrice modelview
        Text(ScreenId).End()
    End Sub
    ''' <summary> initialisation des états  et activation des capacités d'openGL </summary>
    ''' <param name="BgColor"> Couleur de fond du rendu </param>
    Private Shared Sub InitialiserEtatsOpenGL(BgColor As Color)
        GL.ShadeModel(ShadingModel.Smooth) ' shading mathod: GL_SMOOTH or GL_FLAT
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4) ' 4-byte pixel alignment
        ' enable /disable features
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest)
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.Enable(EnableCap.DepthTest)
        GL.Enable(EnableCap.Lighting)
        GL.Enable(EnableCap.Texture2D)
        GL.Enable(EnableCap.CullFace)
        GL.Enable(EnableCap.Blend)
        GL.Enable(EnableCap.ColorMaterial)
        'etats initiaux
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill)
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)
        GL.DepthFunc(DepthFunction.Lequal)
        GL.AlphaFunc(AlphaFunction.Gequal, 0.09999999F)
        GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse)
        GL.ClearColor(BgColor) ' background color
        GL.ClearStencil(0) ' clear stencil buffer
        GL.ClearDepth(1.0F) ' 0 is near, 1 is far
    End Sub
    ''' <summary> initialise la lumière de la scène </summary>
    Private Shared Sub InitialiserLumieres() ' add a white light ti scene
        ' set up light colors (ambient, diffuse, specular)
        Dim lightKa() As Single = {0.2F, 0.2F, 0.2F, 1.0F} ' ambient light
        Dim lightKd() As Single = {0.8F, 0.8F, 0.8F, 1.0F} ' diffuse light
        Dim lightKs() As Single = {1.0F, 1.0F, 1.0F, 1.0F} ' specular light
        GL.Light(LightName.Light0, LightParameter.Ambient, lightKa)
        GL.Light(LightName.Light0, LightParameter.Diffuse, lightKd)
        GL.Light(LightName.Light0, LightParameter.Specular, lightKs)
        ' position the light in eye space
        Dim lightPos() As Single = {0, 0, 1, 0} ' directional light
        GL.Light(LightName.Light0, LightParameter.Position, lightPos)
        'activer each light source after configuration
        GL.Enable(EnableCap.Light0)
    End Sub
    ''' <summary> création du VBO d'un ObjModel </summary>
    ''' <param name="Obj"> Obj Model dont il faut créer le VBO </param>
    Private Shared Function CreerVBO(Obj As ObjModel, ScreenId As ScreenId) As Boolean
        Try
            GL.GenBuffers(1, Obj.Vbo(ScreenId))
            GL.BindBuffer(BufferTarget.ArrayBuffer, Obj.Vbo(ScreenId))
            Dim interleavedVertices As Single() = Obj.InterleavedVertices()
            Dim dataSize As Integer = Obj.InterleavedVerticesSize
            GL.BufferData(BufferTarget.ArrayBuffer, dataSize, interleavedVertices, BufferUsageHint.StaticDraw)
            ' create VBO array for indices
            Dim count As Integer = Obj.GroupCount
            Dim ibo(count - 1) As Integer
            GL.GenBuffers(count, ibo)
            ' setup vbos for indices
            For i As Integer = 0 To count - 1
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo(i))
                Dim size As Integer = Obj.GroupIndicesCount(i) * TailleInteger
                Dim T() As Integer = Obj.GroupIndices(i)
                GL.BufferData(BufferTarget.ElementArrayBuffer, size, T, BufferUsageHint.StaticDraw)
            Next i
            Obj.Ibo(ScreenId) = ibo.ToList
            GL.Flush()
        Catch
            Return False
        End Try
        Return True
    End Function
    ''' <summary> Creation du shader. </summary>
    Private Shared Function CreerShader(ByRef ProgID As Integer, ByRef Message As String) As Boolean
        Dim ResultatStatus As Integer
        ' create 2nd shader and program
        Dim vsId As Integer = GL.CreateShader(ShaderType.VertexShader)
        ProgID = GL.CreateProgram()
        ' load shader sources:
        GL.ShaderSource(vsId, vsSource)
        GL.CompileShader(vsId) 'resultat de la compilation
        GL.GetShader(vsId, ShaderParameter.CompileStatus, ResultatStatus)
        Message = GL.GetShaderInfoLog(vsId)
        If ResultatStatus <> 1 Then Return CBool(ResultatStatus)
        GL.AttachShader(ProgID, vsId)

        Dim fsId As Integer = GL.CreateShader(ShaderType.FragmentShader)
        GL.ShaderSource(fsId, fsSource)
        ' compile shader sources.
        GL.CompileShader(fsId)
        GL.GetShader(fsId, ShaderParameter.CompileStatus, ResultatStatus)
        Message = GL.GetShaderInfoLog(fsId)
        If ResultatStatus <> 1 Then Return CBool(ResultatStatus)
        ' attach shaders to the program
        GL.AttachShader(ProgID, fsId)

        ' link program
        GL.LinkProgram(ProgID)
        GL.GetProgram(ProgID, GetProgramParameterName.LinkStatus, ResultatStatus)
        Message = GL.GetProgramInfoLog(ProgID)
        Return ResultatStatus
    End Function
End Class

''' <summary> Trouve les fonctions OpenGL supportées par le context actuel. Init au 1er appel de propriété </summary>
Friend Module ProceduresGL
    Private ReadOnly OpenGLProcedures As HashSet(Of String)
    Sub New()
        _NomContexteOpengl = GL.GetString(StringName.Renderer)
        Dim count = GL.GetInteger(GetPName.NumExtensions)
        OpenGLProcedures = New HashSet(Of String)()
        For i = 0 To count - 1
            Dim extension = GL.GetString(StringNameIndexed.Extensions, i)
            OpenGLProcedures.Add(extension)
        Next i
    End Sub
    Friend Property NomContexteOpengl As String
    ''' <summary> indique si la fonction est supportée par le context OpenGL actuel </summary>
    ''' <param name="NomExtensions"> Nom de la fonction recherchée </param>
    ''' <returns></returns>
    Friend ReadOnly Property IsSupported(NomExtensions As String) As Boolean
        Get
            Return OpenGLProcedures.Contains(NomExtensions)
        End Get
    End Property
End Module
