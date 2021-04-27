Imports OpenTK.Mathematics
Imports System

Friend Class OrbitCamera
    ''' <summary> création de la caméra
    ''' On suppose qu'il n'y a pas de roulis. L'angle sur l'axe des Z = 0° </summary>
    ''' <param name="position"> Position de la camera ou point de vue dans l'espace mondial </param>
    ''' <param name="target"> point visée ou point de focal initial dans l'espace mondial. </param>
    Friend Sub New(position As Vector3, target As Vector3)
        SetMatriceView(position, target)
    End Sub
    '''<summary> calcul la matrice de vue pour position et target 
    ''' On suppose qu'il n'y a pas de roulis. L'angle sur l'axe des Z = 0° </summary>
    ''' <param name="position"> Position de la camera dans l'espace mondial </param>
    ''' <param name="target"> point de visée dans l'espace mondial </param>
    Friend Sub SetMatriceView(position As Vector3, target As Vector3)
        ' remeber the camera posision & target position
        _position = position
        _target = target

        ' if pos and target are same, only translate camera to position without rotation
        If _position = _target Then
            matrixRotation = Matrix3.Identity
            _matrix = Matrix4.Identity
            _matrix.Row3.Xzy = -_position
            ' rotation stuff
            _angle = New Vector3(0, 0, 0)
            _quaternion = New Quaternion(0, 0, 0, 1)
            Return
        End If

        'lookat d'opentk ne prend pas en compte les cas particuliers où
        'forward vector Is pointing + Y axis : Up = Vector3(0, 0, -1) 
        'et forward vector Is pointing -Y axis : Up = Vector3(0, 0, 1)
        Dim Up = New Vector3(0, 1, 0)
        _distance = (_position - _target).Length()
        _matrix = Matrix4.LookAt(_position, target, Up) 'donnera la même matrice sans le Roll
        matrixRotation = New Matrix3(_matrix.Row0.Xyz, _matrix.Row1.Xyz, _matrix.Row2.Xyz)

        ' set Euler angles
        MatrixToAngles()
        ' set quaternion from angle. Avec OpenTK pas besoin de diviser par 2 les angles. C'est fait en interne
        Dim reversedAngle As Vector3 = New Vector3(_angle.X, -_angle.Y, _angle.Z)
        _quaternion = New Quaternion(reversedAngle * DegToRad)
    End Sub
    ''' <summary> calcul la matrice model de la camera. Cette matrice permet de considérer
    ''' la caméra comme un objet déplacer sur la scène globale </summary>
    Friend Function GetMatriceModel() As Matrix4
        Dim M = Matrix4.CreateTranslation(_position)
        ' compute forward vector and normalize
        Dim forward As Vector3 = _target - M.Row3.Xyz 'translation
        forward.Normalize()

        ' compute left vector
        Dim left As Vector3 = Vector3.Cross(_matrix.Column1.Xyz, forward)
        left.Normalize()

        ' compute orthonormal up vector
        Dim up As Vector3 = Vector3.Cross(forward, left)
        up.Normalize()

        ' NOTE: overwrite rotation and scale info of the current matrix
        M.Row0.Xyz = left
        M.Row1.Xyz = up
        M.Row2.Xyz = forward
        Return M
    End Function
#Region "Propriéts"
    ''' <summary> point de vue de la caméra </summary>
    Friend Property Position As Vector3
        Get
            Return _position
        End Get
        Set(value As Vector3)
            SetMatriceView(value, _target)
        End Set
    End Property
    ''' <summary> point de visée ou point de focal de la caméra 
    ''' les valeurs autres que 0 sont considérées comme un déplacement (pan) du point de visée </summary>
    Friend Property Target As Vector3
        Get
            Return _target
        End Get
        Set(value As Vector3)
            _target = value
            ' forward vector of camera
            Dim forward As Vector3 = -_matrix.Column2.Xyz
            _position = _target - (_distance * forward)
            CalculerMatrix()
        End Set
    End Property

    Friend Property Angle As Vector3
        Get
            Return _angle
        End Get
        Set(value As Vector3)
            _angle = value
            'remember quaternion value
            ' NOTE: yaw must be negated again for quaternion
            Dim reversedAngle As Vector3 = New Vector3(_angle.X, -_angle.Y, _angle.Z)
            _quaternion = New Quaternion(reversedAngle * DegToRad)
            'compute rotation matrix from angle
            AnglesToMatrix()
            'construct camera matrix
            CalculerMatrix()
        End Set
    End Property
    ''' <summary> distance entre le point de vue et le point de visée</summary>
    Friend Property Distance As Single
        Get
            Return _distance
        End Get
        Set(value As Single)
            _distance = value
            CalculerMatrix()
        End Set
    End Property
    ''' <summary> matrice vue de la caméra</summary>
    Friend ReadOnly Property MatrixView As Matrix4
        Get
            Return _matrix
        End Get
    End Property
    ''' <summary> quaternion de la partie rotation (3*3) de la matrice vue </summary>
    Friend ReadOnly Property Quaternion As Quaternion
        Get
            Return _quaternion
        End Get
    End Property
#End Region
#Region "Procédure privées"
    '''<summary>///////////////////////////////////////////////////////////////////////////////
    '''// set transform matrix with rotation angles (degree)
    '''// NOTE: the angle Is For camera, so yaw value must be negated For computation.
    '''//
    '''// The order of rotation Is Roll->Yaw->Pitch (Rx*Ry*Rz)
    '''// Rx: rotation about X-axis, pitch
    '''// Ry: rotation about Y-axis, yaw(heading)
    '''// Rz: rotation about Z-axis, roll
    '''//    Rx           Ry          Rz
    '''// |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
    '''// |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
    '''// |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
    '''///////////////////////////////////////////////////////////////////////////////</summary>
    Private Sub AnglesToMatrix()
        ' rotation angle about X-axis (pitch)
        Dim theta = _angle.X * DegToRad
        Dim sx = CSng(Math.Sin(theta))
        Dim cx = CSng(Math.Cos(theta))

        ' rotation angle about Y-axis (yaw)
        theta = -_angle.Y * DegToRad
        Dim sy = CSng(Math.Sin(theta))
        Dim cy = CSng(Math.Cos(theta))

        ' rotation angle about Z-axis (roll)
        theta = _angle.Z * DegToRad
        Dim sz = CSng(Math.Sin(theta))
        Dim cz = CSng(Math.Cos(theta))

        ' determine left axis
        Dim Left = New Vector3() With {
            .X = cy * cz,
            .Y = sx * sy * cz + cx * sz,
            .Z = -cx * sy * cz + sx * sz}

        ' determine up axis
        Dim Up = New Vector3() With {
            .X = -cy * sz,
            .Y = -sx * sy * sz + cx * cz,
            .Z = cx * sy * sz + sx * cz}

        ' determine forward axis
        Dim forward = New Vector3() With {
            .X = sy,
            .Y = -sx * cy,
            .Z = cx * cy}

        ' construct rotation matrix
        matrixRotation = New Matrix3(Left, Up, forward)
    End Sub

    ''' <summary> transforme la matrice rotation de la caméra en angles exprimés en degré </summary>
    Private Sub MatrixToAngles()
        Dim pitch, yaw, roll As Single
        ' find yaw (around y-axis) first
        ' NOTE: asin() returns -90~+90, so correct the angle range -180~+180
        ' using z value of forward vector
        yaw = RadToDeg * CSng(Math.Asin(matrixRotation.M31))
        If matrixRotation.M33 < 0 Then
            If yaw >= 0 Then
                yaw = 180.0F - yaw
            Else
                yaw = -180.0F - yaw
            End If
        End If

        ' find roll (around z-axis) and pitch (around x-axis)
        ' if forward vector is (1,0,0) or (-1,0,0), then m[0]=m[4]=m[9]=m[10]=0
        If matrixRotation.M11 > -EPSILON AndAlso matrixRotation.M11 < EPSILON Then
            roll = 0 '@@ assume roll=0
            pitch = RadToDeg * CSng(Math.Atan2(matrixRotation.M12, matrixRotation.M22))
        Else
            roll = RadToDeg * CSng(Math.Atan2(-matrixRotation.M21, matrixRotation.M11))
            pitch = RadToDeg * CSng(Math.Atan2(-matrixRotation.M32, matrixRotation.M33))
        End If
        _angle = New Vector3(pitch, -yaw, roll)
    End Sub
    '''<summary>/////////////////////////////////////////////////////////////////////////////
    ''' construct camera matrix: M = Mt2 * Mr * Mt1
    ''' where Mt1: move scene to target (-x,-y,-z)
    '''       Mr : rotate scene at the target point
    '''       Mt2: move scene away from target with distance -d
    '''
    '''     Mt2             Mr               Mt1
    ''' |1  0  0  0|   |r0  r4  r8  0|   |1  0  0 -x|   |r0  r4  r8  r0*-x + r4*-y + r8*-z     |
    ''' |0  1  0  0| * |r1  r5  r9  0| * |0  1  0 -y| = |r1  r5  r9  r1*-x + r5*-y + r9*-z     |
    ''' |0  0  1 -d|   |r2  r6  r10 0|   |0  0  1 -z|   |r2  r6  r10 r2*-x + r6*-y + r10*-z - d|
    ''' |0  0  0  1|   |0   0   0   1|   |0  0  0  1|   |0   0   0   1                         |
    '''/////////////////////////////////////////////////////////////////////////////</summary>'
    Private Sub CalculerMatrix()
        ' extract left/up/forward vectors from rotation matrix
        Dim left As Vector3 = matrixRotation.Row0
        Dim up As Vector3 = matrixRotation.Row1
        Dim forward As Vector3 = matrixRotation.Row2
        ' compute translation vector
        Dim trans As New Vector3() With {
            .X = left.X * -_target.X + up.X * -_target.Y + forward.X * -_target.Z,
            .Y = left.Y * -_target.X + up.Y * -_target.Y + forward.Y * -_target.Z,
            .Z = left.Z * -_target.X + up.Z * -_target.Y + forward.Z * -_target.Z - _distance}

        ' construct matrix
        _matrix = New Matrix4(matrixRotation)
        _matrix.Row3.Xyz = trans

        ' re-compute camera position
        forward = -_matrix.Column2.Xyz
        _position = _target - (_distance * forward)
    End Sub
#End Region
#Region "Variables"
    Private _distance As Single ' distance between position and target
    Private _position As Vector3 ' camera position at world space
    Private _target As Vector3 ' camera focal(lookat) position at world space
    Private _angle As Vector3 ' angle in degree around the target (pitch, heading, roll)
    Private _matrix As Matrix4 ' 4x4 matrix combined rotation and translation
    Private matrixRotation As Matrix3 ' rotation only
    Private _quaternion As Quaternion ' quaternion for rotations
#End Region
End Class