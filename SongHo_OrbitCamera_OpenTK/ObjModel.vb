Imports System.IO
Imports OpenTK.Mathematics

''' <summary> Décrit un objet 3D à partier de la lecture d'un fichier obj </summary>
Friend Class ObjModel
#Region "Variables"
    Private Const DEFAULT_GROUP_NAME As String = "ObjModel_default_group"
    'Private Const DEFAULT_MATERIAL_NAME As String = "ObjModel_default_material"

    Private currentGroup As Integer ' index of current group
    Private currentMaterial As Integer ' index of current material
    Private currentMaterialAssigned As Boolean

    Private groups As List(Of ObjGroup) ' obj model can have multiple proups
    Private materials As List(Of ObjMaterial) ' obj model can have multiple materials
    'on prend des list car on ne sait pas au départ le nb d'élements que représente le fichier
    Private _Vertices As List(Of Single) ' vertex position array for opengl
    Private _Normals As List(Of Single) ' vertex normal array for opengl
    Private _TexCoords As List(Of Single) ' tex-coord array for opengl
    Private Indices As List(Of Integer) ' index array for opengl
    Private FaceNormals As List(Of Vector3) ' normals per face.Il peut y en avoir plus si les objets décrits ne sont pas tous des triangles
    Private _InterleavedVertices() As Single ' for opengl interleaved vertex
    Private bound As BoundingBox
    Private stride As Integer ' # of bytes to hop to the next vertex

    ' temporary lookup buffers
    Private ListeFaces As SortedDictionary(Of String, Integer) ' for "f" lines, map list
    Private vertexLookup As Single() ' for "v" lines
    Private normalLookup As Single() ' for "vn" lines
    Private texCoordLookup As Single() ' for "vt" lines

    'Private ReadOnly defaultMaterial As ObjMaterial ' dummy material for default

    Private _objDirectory As String ' obj/mtl file location with trailing /
    Private _objFileName As String ' file name without path
    Private _mtlFileName As String

    Private ReadOnly _vbo() As Integer ' vbo et indices for model OBJ vertices
    Private ReadOnly _ibo() As List(Of Integer)
    Private _errorMessage As String
    Private Shared nbInstances As Integer
#End Region

#Region "procédures accessibles"
    ''' <summary> constructeur publique </summary>
    ''' <param name="NbScreenId"> nb d'affichage sur lequel cet objet sera affiché </param>
    Friend Sub New(NbScreenId As Integer)
        ReDim _vbo(NbScreenId - 1)
        ReDim _ibo(NbScreenId - 1)
        currentGroup = -1
        currentMaterial = -1
        _errorMessage = "No Error."
        'defaultMaterial = New ObjMaterial(DEFAULT_MATERIAL_NAME)
        _Instance = nbInstances
        nbInstances += 1
    End Sub
    ''' <summary> lit un fichier Obj et le décompose en V, VN, T et groupe et matériel </summary>
    ''' <param name="fileName"> Chemin du fichier Obj </param>
    Friend Function Read(fileName As String) As Boolean
        ' validate file name
        If Not File.Exists(fileName) Then
            _errorMessage = "File name is not defined."
            Return False
        End If
        ' remember path and file name (assume fileName has absolute path)
        _mtlFileName = "" ' start with blank
        _objDirectory = Path.GetDirectoryName(fileName) & "\"
        _objFileName = Path.GetFileName(fileName)
        'décomposition du ficheir en lignes de :
        Dim vLines As New List(Of String)() ' "v" lines from obj file
        Dim vnLines As New List(Of String)() ' "vn" lines from obj file
        Dim vtLines As New List(Of String)() ' "vt" lines from obj file
        Dim fLines As New List(Of String)() ' "f" lines from obj file, other lines as well

        Try
            ' get lines of obj file
            Using fileObj As StreamReader = New StreamReader(_objDirectory & _objFileName)
                Dim line As String
                Do Until fileObj.EndOfStream
                    line = fileObj.ReadLine
                    If line.Length < 2 Then ' skip invalid lines (must have 2 chars per line)
                        Continue Do
                    End If

                    If String.IsNullOrEmpty(line) OrElse line.Chars(0) = "#"c Then ' skip comment lines begin with #
                        Continue Do
                    End If

                    If line(0) = "v"c Then
                        If line(1) = "n"c Then ' vn
                            vnLines.Add(line)
                        ElseIf line(1) = "t"c Then ' vt
                            vtLines.Add(line)
                        ElseIf line(1) = " "c Then ' v
                            vLines.Add(line)
                        End If
                    Else
                        fLines.Add(line) ' store "f", "g", "usemtl", "mtllib"
                    End If
                Loop

            End Using
        Catch ex As Exception
            _errorMessage = "Failed to open a OBJ file to read: " & _objDirectory & _objFileName
            Return False
        End Try

        ' init arrays for opengl drawing
        Indices = New List(Of Integer)(vLines.Count * 3 - 1) ' assume they are triangles
        FaceNormals = New List(Of Vector3)(vLines.Count - 1) ' normals per face
        _Vertices = New List(Of Single)(vLines.Count * 3 - 1)
        _Normals = New List(Of Single)(vLines.Count * 3 - 1)
        If vtLines.Count > 0 Then
            _TexCoords = New List(Of Single)(vLines.Count * 2 - 1)
        End If

        ' parse "v" lines to vertexLookup
        ReDim vertexLookup(vLines.Count * 3 - 1) ' x,y,z
        ParseVertexLookup(vLines)
        vLines.Clear()

        ' parse "vn" lines to normalLookup
        ReDim normalLookup(vnLines.Count * 3 - 1) ' nx,ny,nz
        ParseNormalLookup(vnLines)
        vnLines.Clear() ' dealloc memory

        ' parse "vt" lines to texCoordLookup
        If vtLines.Count > 0 Then
            ReDim texCoordLookup(vtLines.Count * 2 - 1) ' u,v
            ParseTexCoordLookup(vtLines)
            vtLines.Clear() ' dealloc memory
        End If

        ' parse "f" lines with other lines as well: "g", "usemtl", "mtllib"
        ParseFaces(fLines)
        fLines.Clear() ' dealloc memory

        ' clear lookups
        vertexLookup = Nothing
        normalLookup = Nothing
        texCoordLookup = Nothing

        ComputeBoundingBox()
        Return True
    End Function
#End Region

#Region "procédures internes"
    Private Sub New()
    End Sub
    ''' <summary> construit le tableau InterleavedVertices </summary>
    Private Sub BuildInterleavedVertices()
        ' compute stride
        stride = 0
        If _Normals.Count = _Vertices.Count Then
            stride = 24
            If _TexCoords.Count = _Vertices.Count Then
                stride += 8
            End If
        End If

        'nb de singles dans le tableau InterleavedVertices
        Dim count As Integer = (_Vertices.Count * 3) + (_Normals.Count * 3) + (_TexCoords.Count * 2)
        ReDim _InterleavedVertices(count - 1)
        If stride > 0 Then
            BuildInterleavedVerticesVNT() ' vnt or vn
        Else
            _InterleavedVertices = _Vertices.ToArray ' copy only vertices
        End If
    End Sub
    ''' <summary> construit le tableau InterleavedVertices VN ou VNT suivant le stride </summary>
    Private Sub BuildInterleavedVerticesVNT()
        Dim count As Integer = _Vertices.Count
        Dim Cpt As Integer = 0
        Dim j As Integer = 0
        For i As Integer = 0 To count - 1 Step 3
            _InterleavedVertices(Cpt) = _Vertices(i)
            Cpt += 1
            _InterleavedVertices(Cpt) = _Vertices(i + 1)
            Cpt += 1
            _InterleavedVertices(Cpt) = _Vertices(i + 2)
            Cpt += 1
            _InterleavedVertices(Cpt) = _Normals(i)
            Cpt += 1
            _InterleavedVertices(Cpt) = _Normals(i + 1)
            Cpt += 1
            _InterleavedVertices(Cpt) = _Normals(i + 2)
            Cpt += 1
            If stride = 32 Then
                _InterleavedVertices(Cpt) = _TexCoords(j)
                Cpt += 1
                _InterleavedVertices(Cpt) = _TexCoords(j + 1)
                Cpt += 1
                j += 2
            End If
        Next
    End Sub
    ''' <summary> permet une identification plus faciles dans le procédures </summary>
    Private Enum AttributSommet As Integer
        Coordonnees = 0
        Texture = 1
        Normale = 2
    End Enum
    ''' <summary> ajoute la face (Triangle) dans le tableau des indices et les attributs dans les tableaux correspondants </summary>
    ''' <param name="faceIndices"></param>
    Private Sub AddFace(faceIndices As String())
        ' tableau temporaire pour stocker 3 sommets pour le calcul de la normale de la face
        Dim positions(2) As Vector3
        Dim vx, vy, vz As Single
        Dim normalNeeded As Boolean = False
        Dim newVertexCount As Integer = 0
        ' index du tableau lookup (peut être negatif?)
        Dim lookupIndex As Integer

        For i As Integer = 0 To faceIndices.Length - 1
            ' donne l'index dans la liste des faces
            Dim Index As Integer
            'si la face existe, index contient l'index associé dans la liste des faces, sinon il faut ajouter la face
            If Not ListeFaces.TryGetValue(faceIndices(i), Index) Then
                ' les attributs des sommets sont séparés par "/": vertex/texCoords/normal
                Dim AttributsSommets As String() = faceIndices(i).Split("/"c)
                ' AttributsSommets(0) peut être negatif, aussi il faut le rendre positif
                lookupIndex = Integer.Parse(AttributsSommets(AttributSommet.Coordonnees))
                If lookupIndex >= 0 Then
                    ' le format des fichiers OBJ utilise une base d'index 1
                    lookupIndex = (lookupIndex - 1) * 3
                Else
                    lookupIndex = vertexLookup.Length() + lookupIndex * 3
                End If
                'l'index de sommet pointe sur la valeur X, Y et Z sont à la suite
                vx = vertexLookup(lookupIndex)
                vy = vertexLookup(lookupIndex + 1)
                vz = vertexLookup(lookupIndex + 2)
                'on stocke les x,y,z dans le tableau définitif des sommets
                _Vertices.Add(vx)
                _Vertices.Add(vy)
                _Vertices.Add(vz)
                'on se souvient du nb de sommets ajoutés par face
                newVertexCount += 1
                'normalement AttributsSommets.Length est toujours 3 car il y a toujours 2 "/" obligatoires
                'dans le format de fichier OBJ donc on test la nullité
                If String.IsNullOrEmpty(AttributsSommets(AttributSommet.Texture)) And String.IsNullOrEmpty(AttributsSommets(AttributSommet.Normale)) Then
                    ' il s'agit de (vertex)
                    ' la normal par face peut être calculée dans ce cas
                    ' il faut juste se souvenir des coordonnées du sommet
                    'on marque qu'il faudra calculer la normale quand on aura les 3 sommets de la face
                    normalNeeded = True
                    positions(i Mod 3) = New Vector3(vx, vy, vz)
                    ' 2 attributs indique soit (vertex, texCoord) ou (vertex, normal)
                ElseIf String.IsNullOrEmpty(AttributsSommets(AttributSommet.Normale)) Then
                    ' il s'agit de (vertex, texCoord)
                    'on marque qu'il faudra calculer la normale quand on aura les 3 sommets de la face
                    normalNeeded = True ' mark we need add normal later
                    positions(i Mod 3) = New Vector3(vx, vy, vz) ' also, remember vertex to compute face normal

                    lookupIndex = Integer.Parse(AttributsSommets(AttributSommet.Texture))
                    If lookupIndex >= 0 Then
                        lookupIndex = (lookupIndex - 1) * 2
                    Else
                        lookupIndex = texCoordLookup.Length() + lookupIndex * 2
                    End If

                    For Cpt As Integer = 0 To 1
                        _TexCoords.Add(texCoordLookup(lookupIndex + Cpt))
                    Next
                ElseIf String.IsNullOrEmpty(AttributsSommets(AttributSommet.Texture)) Then
                    ' il s'agit de (vertex, normal)
                    positions(i Mod 3) = New Vector3(vx, vy, vz) ' remember vertex to compute face normal

                    lookupIndex = Integer.Parse(AttributsSommets(AttributSommet.Normale))
                    If lookupIndex >= 0 Then
                        lookupIndex = (lookupIndex - 1) * 3
                    Else
                        lookupIndex = normalLookup.Length() + lookupIndex * 3
                    End If

                    For Cpt As Integer = 0 To 2
                        _Normals.Add(normalLookup(lookupIndex + Cpt))
                    Next
                Else
                    ' 3 tokens means it is (vertex, texCoord, normal)
                    positions(i Mod 3) = New Vector3(vx, vy, vz) ' remember vertex to compute face normal

                    lookupIndex = Integer.Parse(AttributsSommets(AttributSommet.Texture))
                    If lookupIndex >= 0 Then
                        lookupIndex = (lookupIndex - 1) * 2
                    Else
                        lookupIndex = texCoordLookup.Length + lookupIndex * 2
                    End If

                    For Cpt As Integer = 0 To 1
                        _TexCoords.Add(texCoordLookup(lookupIndex + Cpt))
                    Next

                    lookupIndex = Integer.Parse(AttributsSommets(AttributSommet.Normale))
                    If lookupIndex >= 0 Then
                        lookupIndex = (lookupIndex - 1) * 3
                    Else
                        lookupIndex = normalLookup.Length + lookupIndex * 3
                    End If

                    For Cpt As Integer = 0 To 2
                        _Normals.Add(normalLookup(lookupIndex + Cpt))
                    Next
                End If

                ' add new index to the list
                Dim vertexIndex As Integer = _Vertices.Count \ 3 - 1
                ListeFaces(faceIndices(i)) = vertexIndex

                Indices.Add(vertexIndex)
                ' it is already in list, get the index from the list
            Else
                ' add it to only the index list
                Indices.Add(Index)
                ' for face normal generation
                lookupIndex = Index * 3
                positions(i Mod 3) = New Vector3(_Vertices(lookupIndex), _Vertices(lookupIndex + 1), _Vertices(lookupIndex + 2))
            End If

            ' finally, compute face normal per triangle
            If i Mod 3 = 2 Then
                Dim normal As Vector3 = ComputeFaceNormal(positions)
                FaceNormals.Add(normal) ' store face normal per face
                ' assign face normal as vertex normal to new vertices only
                If normalNeeded Then
                    For j As Integer = 0 To newVertexCount - 1
                        _Normals.Add(normal.X)
                        _Normals.Add(normal.Y)
                        _Normals.Add(normal.Z)
                    Next j
                End If

                newVertexCount = 0 ' reset
            End If
        Next i
    End Sub
    ''' <summary> remplit les tableaux de faces, matérial et groupe </summary>
    ''' <param name="lines"> tableau contenant les textes à décomposer </param>
    Private Sub ParseFaces(ByVal lines As List(Of String))
        ' reset the previous values
        currentMaterial = -1
        currentGroup = currentMaterial
        currentMaterialAssigned = False
        stride = 0
        groups = New List(Of ObjGroup)
        materials = New List(Of ObjMaterial)
        ListeFaces = New SortedDictionary(Of String, Integer)

        For Each Line As String In lines
            Dim Results As String() = Line.Split(New Char() {" "c}, 2)
            ' start tokenizing
            Dim token As String = Results(0)

            If token = "f" Then ' parse face (triangle)
                ' if not shown "g" before "f" yet, then create a default group
                If currentGroup = -1 Then
                    CreateGroup(DEFAULT_GROUP_NAME)
                    ' if "usemtl" shown before f, then use this mtl for this group
                    If currentMaterial >= 0 Then
                        groups(currentGroup).materialName = materials(currentMaterial).name
                        currentMaterialAssigned = True
                    End If
                End If

                ' get all face index list in a line
                Dim faceIndices() As String = Results(1).Split(" "c)
                ' convert to triangles if the face has more than 3 indices
                If faceIndices.Length > 3 Then
                    faceIndices = ConvertToTriangles(faceIndices)
                End If
                ' ajoute la face à la liste
                AddFace(faceIndices)

            ElseIf token = "g" Then
                ' parse group
                CreateGroup(Results(1)) ' create new group, mtl name will be set when "usemtl" called

                ' if "usemtl"->"g" (if a material is not assigned to a group yet),
                ' then assign the current material to this group
                If currentMaterial >= 0 AndAlso Not currentMaterialAssigned Then
                    groups(currentGroup).materialName = materials(currentMaterial).name
                    currentMaterialAssigned = True
                End If

            ElseIf token = "mtllib" Then
                ' parse material file
                ParseMaterial(Results(1)) ' pass the rest of tokens
                currentMaterial = -1 ' reset current ID after read mtl file

            ElseIf token = "usemtl" Then
                ' parse material name(ID)
                Dim materialName As String = Results(1)
                currentMaterial = FindMaterial(materialName) ' remember in case "usemtl" comes before "g"
                currentMaterialAssigned = False

                ' if "g"->"usemtl ("g" comes before "usemtl"), assign the material name to the group
                If currentMaterial >= 0 AndAlso currentGroup >= 0 Then
                    ' if material name is not set on the current group, assign it
                    If groups(currentGroup).materialName = "" Then
                        groups(currentGroup).materialName = materialName
                        currentMaterialAssigned = True
                        ' if material name is different, then create new group with mtl name
                    ElseIf groups(currentGroup).materialName <> materialName Then
                        ' create a temp group here
                        ' if "g" will appear next line, use that group,
                        ' but no "g" follows, use this temp group
                        CreateGroup(materialName)
                        groups(currentGroup).materialName = materialName
                    End If
                End If
            End If
        Next

        ' compute index count of the last group, before return
        If currentGroup >= 0 Then
            groups(currentGroup).indexCount = Indices.Count - groups(currentGroup).indexOffset
        End If

        ' delete empty groups which have no indices assigned
        For Cpt As Integer = groups.Count - 1 To 0
            If groups(Cpt).indexCount = 0 Then
                groups.RemoveAt(Cpt)
            End If
        Next

        ' clear temp map
        ListeFaces.Clear()
        ListeFaces = Nothing
    End Sub
    ''' <summary> lit un ficier de Materiel et le transforme en 1 objet Matérial</summary>
    ''' <param name="mtlName"> nom du fichier material </param>
    Private Function ParseMaterial(mtlName As String) As Boolean
        _mtlFileName = Path.GetFileName(mtlName)
        Dim pathChemin As String = _objDirectory + _mtlFileName
        ' validate file name
        If Not File.Exists(pathChemin) Then
            _errorMessage = "Mtl name is not defined."
            Return False
        End If
        ' open an MTL file
        Try
            Using fileMtl As StreamReader = New StreamReader(pathChemin)
                Do Until fileMtl.EndOfStream
                    Dim line = fileMtl.ReadLine
                    ' skip comment line
                    If String.IsNullOrEmpty(line) OrElse line.Chars(0) = "#"c Then
                        Continue Do
                    End If

                    Dim Results As String() = line.Split(New Char() {" "c}, 2)
                    Select Case Results(0)
                        Case "newmtl"
                            ' parse material name
                            Dim material As New ObjMaterial(Results(1))
                            ' add to material list
                            materials.Add(material)
                            currentMaterial = materials.Count - 1

                        Case "Ka"
                            ' parse ambient
                            Dim Kas As String() = Results(1).Split(" "c)
                            materials(currentMaterial).ambient = New Color4(Single.Parse(Kas(0)), Single.Parse(Kas(1)),
                                                                             Single.Parse(Kas(2)), 1.0F)

                        Case "Kd"
                            ' parse diffuse
                            Dim Kds As String() = Results(1).Split(" "c)
                            materials(currentMaterial).diffuse = New Color4(Single.Parse(Kds(0)), Single.Parse(Kds(1)),
                                                                             Single.Parse(Kds(2)), 1.0F)

                        Case "Ks"
                            ' parse specular
                            Dim Kss As String() = Results(1).Split(" "c)
                            materials(currentMaterial).specular = New Color4(Single.Parse(Kss(0)), Single.Parse(Kss(1)),
                                                                              Single.Parse(Kss(2)), 1.0F)

                        Case "Ke"
                            ' parse emissive
                            Dim Kes As String() = Results(1).Split(" "c)
                            materials(currentMaterial).emissive = New Color4(Single.Parse(Kes(0)), Single.Parse(Kes(1)),
                                                                              Single.Parse(Kes(2)), 1.0F)

                        Case "Ns"
                            ' parse specular exponent
                            materials(currentMaterial).shininess = Single.Parse(Results(1))

                        Case "d"
                            ' parse transparency
                            ' override alpha value
                            Dim alpha As Single = Single.Parse(Results(1))
                            materials(currentMaterial).ambient.A = alpha
                            materials(currentMaterial).emissive.A = alpha
                            materials(currentMaterial).diffuse.A = alpha
                            materials(currentMaterial).specular.A = alpha

                        ' parse texture map name
                        Case "map_Kd"
                            materials(currentMaterial).textureName = Results(1)

                        Case "Ni"
                        Case "map_Ks"
                        Case "map_Ka"
                        Case "illum"
                    End Select
                Loop
            End Using
        Catch
            _errorMessage = "Failed to open a MTL file to read: " + pathChemin
            Return False
        End Try

        Return True
    End Function
    ''' <summary> calcul la normale à une face (triangle) </summary>
    ''' <param name="Vs"> tableau qui contient les 3 sommets de la face </param>
    Private Shared Function ComputeFaceNormal(Vs As Vector3()) As Vector3
        Dim v12 As Vector3 = Vs(1) - Vs(0)
        Dim v13 As Vector3 = Vs(2) - Vs(0)
        Dim normal As Vector3 = Vector3.Cross(v12, v13)
        normal.Normalize()

        Return normal
    End Function
    ''' <summary> décompose une face à plus de 3 sommets en triangles mode Fan </summary>
    ''' <param name="faceIndices"> tableau contenant les sommets de la face à décomposer </param>
    Private Shared Function ConvertToTriangles(faceIndices As String()) As String()
        ' faire une copie de la face
        Dim count As Integer = faceIndices.Length
        'réserver assez d'espace pour les triangles à rajouter
        Dim Triangles((count - 2) * 3 - 1) As String

        faceIndices.CopyTo(Triangles, 0)

        Dim Index As Integer = 3
        ' start from 4th index, insert 2 more between the target element
        For i As Integer = 3 To count - 1
            Triangles(Index) = faceIndices(i - 1) ' insert the previous
            Index += 1
            Triangles(Index) = faceIndices(i) ' insert the target
            Index += 1
            Triangles(Index) = faceIndices(0) ' insert the first
            Index += 1
        Next i
        Return Triangles
    End Function
    ''' <summary> remplit le tableau temporaire des sommets </summary>
    ''' <param name="lines"> Tableau contenant le texte à transcrire </param>
    Private Sub ParseVertexLookup(lines As List(Of String))
        Dim Index As Integer = 0
        ' convert as float then store to vertexLookup
        For Each line As String In lines
            Dim Results As String() = line.Split(" "c)
            vertexLookup(Index) = Single.Parse(Results(1)) ' x
            Index += 1
            vertexLookup(Index) = Single.Parse(Results(2))  ' y
            Index += 1
            vertexLookup(Index) = Single.Parse(Results(3))  ' z
            Index += 1
        Next
    End Sub
    ''' <summary> remplit le tableau temporaire des normales aux sommets </summary>
    ''' <param name="lines"> Tableau contenant le texte à transcrire </param>
    Private Sub ParseNormalLookup(lines As List(Of String))
        Dim vec As New Vector3()
        Dim Index As Integer = 0
        ' convert as float then store to vertexLookup
        For Each Line As String In lines
            Dim Results As String() = Line.Split(" "c)
            vec.X = Single.Parse(Results(1))
            vec.Y = Single.Parse(Results(2))
            vec.Z = Single.Parse(Results(3))
            vec.Normalize()
            normalLookup(Index) = vec.X
            Index += 1
            normalLookup(Index) = vec.Y
            Index += 1
            normalLookup(Index) = vec.Z
            Index += 1
        Next
    End Sub
    ''' <summary> remplit le tableau temporaire des coordonnées de texture aux sommets </summary>
    ''' <param name="lines"> Tableau contenant le texte à transcrire </param>
    Private Sub ParseTexCoordLookup(ByVal lines As List(Of String))
        Dim Index As Integer = 0
        ' convert as float then store to vertexLookup
        For Each Line As String In lines
            Dim Results As String() = Line.Split(" "c)
            texCoordLookup(Index) = Single.Parse(Results(1)) ' u
            Index += 1
            'inverse v pour respecter l'orentation OpenGL. A verifier avec la procédure qui charge la texture en mémoire vidéo
            texCoordLookup(Index) = 1 - Single.Parse(Results(2)) ' v
            Index += 1
        Next
    End Sub
    ''' <summary> création d'un nouvea group</summary>
    ''' <param name="groupName"> Nom du nouveau group </param>
    Private Sub CreateGroup(groupName As String)
        ' add new group to container
        Dim group As New ObjGroup(groupName) With {.materialName = "",
                                                   .indexOffset = Indices.Count}  ' starting point
        groups.Add(group)

        ' remember the current group index
        Dim groupCount As Integer = groups.Count()
        currentGroup = groupCount - 1

        ' index count for previous group
        Dim prevGroup As Integer = groupCount - 2
        If prevGroup >= 0 Then
            groups(prevGroup).indexCount = Indices.Count - groups(prevGroup).indexOffset
        End If
    End Sub
    ''' <summary> Recherche si 1 material existe </summary>
    ''' <param name="name"> nom du matérial à tester </param>
    ''' <returns> l'index du materiel dans la collection </returns>
    Private Function FindMaterial(name As String) As Integer
        For i As Integer = 0 To materials.Count - 1
            If materials(i).name = name Then
                Return i
            End If
        Next i
        Return -1 ' not found
    End Function
    ''' <summary> recherche si un group existe </summary>
    ''' <param name="name"> nom du groupe à tester </param>
    ''' <returns> l'index du group dans la collection </returns>
    ''' <summary> calcul le cube qui contient l'objet </summary>
    Private Sub ComputeBoundingBox()
        ' prepare default bound with opposite values
        bound = New BoundingBox(Single.MaxValue, Single.MinValue, Single.MaxValue,
                                Single.MinValue, Single.MaxValue, Single.MinValue)
        Dim x, y, z As Single
        Dim count As Integer = _Vertices.Count
        For i As Integer = 0 To count - 1 Step 3
            x = _Vertices(i)
            y = _Vertices(i + 1)
            z = _Vertices(i + 2)

            If x < bound.minX Then
                bound.minX = x
            End If
            If x > bound.maxX Then
                bound.maxX = x
            End If

            If y < bound.minY Then
                bound.minY = y
            End If
            If y > bound.maxY Then
                bound.maxY = y
            End If

            If z < bound.minZ Then
                bound.minZ = z
            End If
            If z > bound.maxZ Then
                bound.maxZ = z
            End If
        Next i
    End Sub
#End Region

#Region "Propriétés accessibles"
    Friend Property Vbo(ScreenId As Integer) As Integer
        Get
            Return _vbo(ScreenId)
        End Get
        Set(value As Integer)
            _vbo(ScreenId) = value
        End Set
    End Property
    Friend Property Ibo(ScreenId As Integer) As List(Of Integer)
        Get
            Return _ibo(ScreenId)
        End Get
        Set(value As List(Of Integer))
            _ibo(ScreenId) = value
        End Set
    End Property

    Friend ReadOnly Property Instance As Integer
    ''' <summary> information sur la dernière erreur </summary>
    Friend ReadOnly Property ErrorMessage As String
        Get
            Return _errorMessage
        End Get
    End Property
    ' for Groups
    ''' <summary> nb de Groups </summary>
    Friend ReadOnly Property GroupCount As Integer
        Get
            Return groups.Count
        End Get
    End Property
    ''' <summary> nb d'indices représenté par le groupe </summary>
    ''' <param name="index"> N° de dans la collection de Groupes </param>
    Friend ReadOnly Property GroupIndicesCount(index As Integer) As Integer
        Get
            If index >= 0 AndAlso index < groups.Count Then
                Return groups(index).indexCount
            Else
                Return 0
            End If
        End Get
    End Property
    ''' <summary> tableau des indices du groupe </summary>
    ''' <param name="index"> N° de dans la collection de Groupes </param>
    Friend ReadOnly Property GroupIndices(index As Integer) As Integer()
        Get
            If index >= 0 AndAlso index < groups.Count Then
                If groups.Count > 1 Then
                    Dim R(groups(index).indexCount - 1) As Integer
                    Array.Copy(Indices.ToArray, groups(index).indexOffset, R, 0, groups(index).indexCount)
                    Return R
                Else
                    Return Indices.ToArray
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property
    ' for interleave vertex : V/N ou V/N/T
    ''' <summary> construit et retourne le tableau VTN sous forme d'un tableau à 1 dimension de single </summary>
    Friend ReadOnly Property InterleavedVertices() As Single()
        Get
            ' create one if not built yet
            If _InterleavedVertices Is Nothing Then
                BuildInterleavedVertices()
            End If
            Return _InterleavedVertices
        End Get
    End Property
    ''' <summary> donne le nb de single composée par VTN (0, 24 ou 32 </summary>
    Friend ReadOnly Property InterleavedVerticesStride As Integer
        Get
            Return stride
        End Get
    End Property
    ''' <summary> retourne le nb d'octets du tableau VTN </summary>
    Friend ReadOnly Property InterleavedVerticesSize As Integer
        Get
            Return _InterleavedVertices.Length * TailleSingle
        End Get
    End Property
#End Region
End Class
''' <summary> décrit un materiel (surface d'un triangle) </summary>
Friend Class ObjMaterial
    Implements IEquatable(Of ObjMaterial)
    Friend name As String
    Friend textureName As String
    Friend ambient, diffuse, specular, emissive As Color4
    Friend shininess As Single

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If
        Dim objAsPart As ObjMaterial = TryCast(obj, ObjMaterial)
        If objAsPart Is Nothing Then
            Return False
        Else
            Return Equals(objAsPart)
        End If
    End Function
    Public Overloads Function Equals(ByVal other As ObjMaterial) As Boolean Implements IEquatable(Of ObjMaterial).Equals
        If other Is Nothing Then
            Return False
        End If
        Return name.Equals(other.name)
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return name.GetHashCode()
    End Function

    ' ctors
    Friend Sub New(Name As String, Optional Shininess As Single = 128)
        Me.name = Name
        Me.shininess = Shininess
        ambient = New Color4()
        diffuse = New Color4()
        specular = New Color4()
        emissive = New Color4()
    End Sub
End Class
''' <summary> décrit un group d'un objet 3D. 
''' Un groupe est un ensemble d'indices partageant le même matériel </summary>
Friend Class ObjGroup
    Friend name As String
    Friend materialName As String ' "usemtl"
    Friend indexOffset As Integer ' starting position of indices for this group
    Friend indexCount As Integer ' number of indices for this group
    Friend Sub New(Nom As String)
        name = Nom
        indexOffset = 0
        indexCount = 0
    End Sub
End Class
''' <summary> volume parallélépipèdique contenant un objet 3D </summary>
Friend Structure BoundingBox
    Friend minX, maxX, minY, maxY, minZ, maxZ As Single
    ''' <summary> constructeur </summary>
    Friend Sub New(minX As Single, maxX As Single, minY As Single, maxY As Single, minZ As Single, maxZ As Single)
        Me.minX = minX
        Me.maxX = maxX
        Me.minY = minY
        Me.maxY = maxY
        Me.minZ = minZ
        Me.maxZ = maxZ
    End Sub
    ' return center values
    Friend ReadOnly Property CenterX As Single
        Get
            Return (maxX + minX) / 2.0F
        End Get
    End Property
    Friend ReadOnly Property CenterY As Single
        Get
            Return (maxY + minY) / 2.0F
        End Get
    End Property
    Friend ReadOnly Property CenterZ As Single
        Get
            Return (maxZ + minZ) / 2.0F
        End Get
    End Property

    ' return the smallest radius to fit
    Friend ReadOnly Property RadiusX As Single
        Get
            Return (maxX - minX) / 2.0F
        End Get
    End Property
    Friend ReadOnly Property RadiusY As Single
        Get
            Return (maxY - minY) / 2.0F
        End Get
    End Property
    Friend ReadOnly Property RadiusZ As Single
        Get
            Return (maxZ - minZ) / 2.0F
        End Get
    End Property
    Friend ReadOnly Property Radius As Single
        Get
            Dim x As Single = RadiusX
            Dim y As Single = RadiusY
            Dim z As Single = RadiusZ
            Return CSng(Math.Sqrt(x * x + y * y + z * z))
        End Get
    End Property
    ' return (minX, minY, minZ) - (maxX, maxY, maxZ)
    Public Overrides Function ToString() As String
        Dim ss As String = $"({minX:N3},  {minY:N3}, {minZ:N3}) - ({maxX:N3}, {maxY:N3}, {maxZ:N3})"
        Return ss
    End Function
End Structure