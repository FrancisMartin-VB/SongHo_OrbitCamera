Imports System.Runtime.InteropServices
Imports OpenTK.Mathematics
''' <summary> Rassemble tout ce qui peut être mis en commun au niveau du projet </summary>
Module Utilitaires
    Friend Const RadToDeg As Single = 180.0 / Math.PI
    Friend Const DegToRad As Single = Math.PI / 180.0
    Friend Const PiOver2 As Single = Math.PI / 2
    Friend CrLf As String = Convert.ToChar(13) + Convert.ToChar(10)
    Friend Lf As String = Convert.ToChar(10)
    Friend Const EPSILON As Single = 0.00001F
    Friend ReadOnly TailleSingle As Integer = Marshal.SizeOf(Of Single)
    Friend ReadOnly TailleInteger As Integer = Marshal.SizeOf(Of Integer)
    ''' <summary> collection avec une clé de type V et une liste de valeur associées de type T 
    ''' correspond à peu près au MultiMap de C++ </summary>
    ''' <typeparam name="V"> Type de la clé </typeparam>
    ''' <typeparam name="T"> Type des valeurs </typeparam>
    Friend Class MultiMapList(Of V, T)
        Private ReadOnly Dictionary As Dictionary(Of V, List(Of T))
        ''' <summary> création du MultiMzp </summary>
        ''' <param name="Reserve"></param>
        Friend Sub New(Optional Reserve As Integer = 10)
            Dictionary = New Dictionary(Of V, List(Of T))(Reserve)
        End Sub

        Friend Sub Clear()
            Dictionary.Clear()
        End Sub
        ''' <summary> ajoute :
        ''' soit la clé et la valeur si la clé n'existe pas 
        ''' soit la valeur si la clé existe. La valeur peut être null ou déjà existante </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        Friend Function Add(ByVal key As V, ByVal value As T) As Boolean
            ' Add a key.
            Dim list As List(Of T) = Nothing
            If Me.Dictionary.TryGetValue(key, list) Then
                list.Add(value)
            Else
                list = New List(Of T) From {value}
                Me.Dictionary(key) = list
            End If
            Return True
        End Function

        Friend ReadOnly Property Keys() As List(Of V)
            Get
                ' Get all keys.
                Return Dictionary.Keys.ToList
            End Get
        End Property
        '' <summary> renvoie la liste des valeur associées à la clé </summary>
        ''' <param name="key"> clé dont on cherche les valeurs </param>
        Default Friend ReadOnly Property Item(ByVal key As V) As List(Of T)
            Get
                ' Get list at a key.
                Dim list As List(Of T) = Nothing
                Dictionary.TryGetValue(key, list)
                Return list
            End Get
        End Property
        ''' <summary> nb de clé dans le multiMap </summary>
        Friend ReadOnly Property Count As Integer
            Get
                Return Dictionary.Count
            End Get
        End Property
        ''' <summary> renvoie le nb de valeur dans le multiMap </summary>
        Friend ReadOnly Property NbValeurs As Integer
            Get
                Dim R As Integer
                For Each Key As V In Keys
                    R += Dictionary(Key).Count
                Next
                Return R
            End Get
        End Property
    End Class
    ''' <summary> collection avec une clé de type V et une liste de valeur associées de type T structure
    ''' correspond à peu près au MultiMap de C++ </summary>
    ''' <typeparam name="V"> Type de la clé </typeparam>
    ''' <typeparam name="T"> Type des valeurs </typeparam>
    Friend Class MultiMapHashSet(Of V, T As Structure)
        Private ReadOnly Dictionary As Dictionary(Of V, HashSet(Of T))
        ''' <summary> création du MultiMap </summary>
        ''' <param name="Reserve"></param>
        Friend Sub New(Optional Reserve As Integer = 10)
            Dictionary = New Dictionary(Of V, HashSet(Of T))(Reserve)
        End Sub
        ''' <summary> efface les données du dictionnaire </summary>
        Friend Sub Clear()
            Dictionary.Clear()
        End Sub
        ''' <summary> ajoute :
        ''' soit la clé et la valeur si la clé n'existe pas 
        ''' soit la valeur si la clé existe. La valeur ne peut être déjà existante </summary>
        ''' <param name="key"> clé associé à la valeur à ajouter </param>
        ''' <param name="value"> valeur à ajouter </param>
        ''' <returns> False indique que la valeur n'a pas pu être ajouté pour cause de doublon </returns>
        Friend Function Add(ByVal key As V, ByVal value As T) As Boolean
            Dim R As Boolean = True
            ' Add a key.
            Dim list As HashSet(Of T) = Nothing
            If Me.Dictionary.TryGetValue(key, list) Then
                Dim Bid As T = Nothing
                If Not list.TryGetValue(value, Bid) Then
                    list.Add(value)
                Else
                    R = False
                End If
            Else
                list = New HashSet(Of T) From {value}
                Me.Dictionary(key) = list
            End If
            Return R
        End Function
        ''' <summary> retourne la list des cles </summary>
        Friend ReadOnly Property Keys() As List(Of V)
            Get
                ' Get all keys.
                Return Dictionary.Keys.ToList
            End Get
        End Property
        ''' <summary> renvoie la liste des valeurs associées à la clé </summary>
        ''' <param name="key"> clé dont on cherche les valeurs </param>
        Default Friend ReadOnly Property Item(ByVal key As V) As HashSet(Of T)
            Get
                ' Get list at a key.
                Dim list As HashSet(Of T) = Nothing
                Dictionary.TryGetValue(key, list)
                Return list
            End Get
        End Property
        ''' <summary> nb de clé dans le multiMap </summary>
        Friend ReadOnly Property Count As Integer
            Get
                Return Dictionary.Count
            End Get
        End Property
        ''' <summary> renvoie le nb de valeur dans le multiMap </summary>
        Friend ReadOnly Property NbValeurs As Integer
            Get
                Dim R As Integer
                For Each Key As V In Keys
                    R += Dictionary(Key).Count
                Next
                Return R
            End Get
        End Property
    End Class
    ''' <summary> transforme Pitch, Yaw et Roll en quaternion </summary>
    ''' <param name="eulerAngles"> Vecteur contenant les angles exprimés en radian </param>
    Friend Function EulerAnglesToQuaternion(eulerAngles As Vector3) As Quaternion
        eulerAngles.Y = -eulerAngles.Y
        Return New Quaternion(eulerAngles)
    End Function
    ''' <summary> transforme un quaternion en Pitch, -Yaw et Roll 
    ''' exprimé en radians sous la forme d'un vecteur
    ''' Code de départ issu d'opentk 4.0 et plus . N'existe pas en opentk 3.0. 
    ''' Attention inverse Yaw </summary>
    ''' <param name="q"> quaternion à transformer </param>
    Friend Function QuaternionToEulerAngles(q As Quaternion) As Vector3
        Dim eulerAngles As Vector3 = q.ToEulerAngles()
        eulerAngles.Y = -eulerAngles.Y
        Return eulerAngles
    End Function
    ''' <summary> transforme la partie rotation (3*3) d'une matrice 4*4 en angles
    ''' Pitch, Yaw et Roll exprimé en radian. </summary>
    ''' <param name="m"> Matrice à transformer </param>
    Friend Function MatrixToEulerAngles(m As Matrix4) As Vector3
        Dim pitch, yaw, roll As Single

        ' find yaw (around y-axis) first
        ' NOTE: asin() returns -90~+90, so correct the angle range -180~+180
        ' using z value of forward vector
        yaw = CSng(Math.Asin(m.M31))
        If m.M33 < 0 Then
            If yaw >= 0 Then
                yaw = PiOver2 - yaw
            Else
                yaw = -PiOver2 - yaw
            End If
        End If

        ' find roll (around z-axis) and pitch (around x-axis)
        ' if forward vector is (1,0,0) or (-1,0,0), then m[0]=m[4]=m[9]=m[10]=0
        If m.M11 > -EPSILON AndAlso m.M11 < EPSILON Then
            roll = 0 '@@ assume roll=0
            pitch = CSng(Math.Atan2(m.M12, m.M22))
        Else
            roll = CSng(Math.Atan2(-m.M21, m.M11))
            pitch = CSng(Math.Atan2(-m.M32, m.M33))
        End If

        Return New Vector3(pitch, -yaw, roll)
    End Function
    ''' <summary> transforme Pitch, Yaw et Roll en partie rotation (3*3)
    ''' d'une matrice 4*4 </summary>
    ''' <param name="v"> Vecteur contenant les angles exprimés en radian </param>
    Friend Function EulerAnglesToMatrix(v As Vector3) As Matrix4
        ' rotation angle about X-axis (pitch)
        Dim theta = v.X
        Dim sx = CSng(Math.Sin(theta))
        Dim cx = CSng(Math.Cos(theta))

        ' rotation angle about Y-axis (yaw)
        theta = -v.Y
        Dim sy = CSng(Math.Sin(theta))
        Dim cy = CSng(Math.Cos(theta))

        ' rotation angle about Z-axis (roll)
        theta = v.Z
        Dim sz = CSng(Math.Sin(theta))
        Dim cz = CSng(Math.Cos(theta))

        ' construct rotation matrix
        Dim MR As New Matrix3
        ' determine left axis
        MR.Row0.X = cy * cz
        MR.Row0.Y = sx * sy * cz + cx * sz
        MR.Row0.Z = -cx * sy * cz + sx * sz

        ' determine up axis
        MR.Row1.X = -cy * sz
        MR.Row1.Y = -sx * sy * sz + cx * cz
        MR.Row1.Z = cx * sy * sz + sx * cz

        ' determine forward axis
        MR.Row2.X = sy
        MR.Row2.Y = -sx * cy
        MR.Row2.Z = cx * cy

        Return New Matrix4(MR)
    End Function
End Module
