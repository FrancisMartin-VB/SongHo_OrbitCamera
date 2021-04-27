Imports System
Imports OpenTK
Imports OpenTK.Mathematics

Partial Class OrbitCamera
    Friend Sub New()
        Me.movingTime = 0
        Me.movingDuration = 0
        Me.moving = False
        Me.shiftingTime = 0
        Me.shiftingDuration = 0
        Me.shiftingSpeed = 0
        Me.shiftingAccel = 0
        Me.shiftingMaxSpeed = 0
        Me.shifting = False
        Me.forwardingTime = 0
        Me.forwardingDuration = 0
        Me.forwardingSpeed = 0
        Me.forwardingAccel = 0
        Me.forwardingMaxSpeed = 0
        Me.forwarding = False
        Me.turningTime = 0
        Me.turningDuration = 0
        Me.turning = False
        Me.quaternionUsed = False
        _quaternion = New Quaternion(1, 0, 0, 0)
    End Sub

    ' return camera's 3 axis vectors
    Friend ReadOnly Property LeftAxis As Vector3
        Get
            Return -_matrix.Column0.Xyz
        End Get
    End Property

    Friend ReadOnly Property UpAxis As Vector3
        Get
            Return _matrix.Column1.Xyz
        End Get
    End Property

    Friend ReadOnly Property ForwardAxis As Vector3
        Get
            Return _matrix.Column2.Xyz
        End Get
    End Property
    ' rotate the camera around the target point
    ' You can use either quaternion or Euler anagles
    Friend Sub RotateTo(angle As Vector3, Optional duration As Single = 0.0F, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        quaternionUsed = False
        If duration <= 0.0F Then
            _angle = angle
        Else
            turningAngleFrom = Me.Angle
            turningAngleTo = angle
            turningTime = 0
            turningDuration = duration
            turningMode = mode
            turning = True
        End If

    End Sub
    Friend Sub LookAt(pos As Vector3, target As Vector3, upDir As Vector3)
        ' remeber the camera posision & target position
        Me._position = pos
        Me.Target = target

        ' if pos and target are same, only translate camera to position without rotation
        If _position = target Then
            _matrix = Matrix4.CreateTranslation(-_position)
            ' rotation stuff
            matrixRotation = Matrix3.Identity
            _angle = Vector3.Zero
            _quaternion = New Quaternion(New Vector3(0, 0, 0), 1)
            Return
        End If

        Dim left, up, forward As Vector3

        ' compute the forward vector
        ' NOTE: the direction is reversed (target to camera pos) because of camera transform
        forward = Position - target
        Distance = forward.Length() ' remember the distance
        ' normalize
        forward /= Me.Distance

        ' compute the left vector
        left = Vector3.Cross(upDir, forward) ' cross product
        left.Normalize()

        ' recompute the orthonormal up vector
        up = Vector3.Cross(forward, left) ' cross product
        'up.normalize();

        ' set inverse rotation matrix: M^-1 = M^T for Euclidean transform
        matrixRotation = New Matrix3(left, up, forward)

        ' copy it to matrix
        _matrix = New Matrix4(matrixRotation)

        ' set translation
        Dim trans As New Vector3() With {
            .X = _matrix.M11 * -Position.X + _matrix.M21 * -Position.Y + _matrix.M31 * -Position.Z,
            .Y = _matrix.M12 * -Position.X + _matrix.M22 * -Position.Y + _matrix.M32 * -Position.Z,
            .Z = _matrix.M13 * -Position.X + _matrix.M23 * -Position.Y + _matrix.M33 * -Position.Z}
        _matrix.Row3.Xyz = trans

        ' set Euler angles
        _angle = MatrixToReverseAngles(New Matrix4(matrixRotation))

        ' set quaternion from angle. Avec opentk la division /2 des angles se fait en interne
        Dim reversedAngle As New Vector3(Angle.X, -Angle.Y, Angle.Z)
        _quaternion = New Quaternion(reversedAngle * DegToRad) ' pas besoin de half angle
    End Sub
    Friend Sub LookAt(px As Single, py As Single, pz As Single, tx As Single, ty As Single, tz As Single)
        SetMatriceView(New Vector3(px, py, pz), New Vector3(tx, ty, tz))
    End Sub
    Friend Sub LookAt(px As Single, py As Single, pz As Single, tx As Single, ty As Single, tz As Single,
                       ux As Single, uy As Single, uz As Single)
        LookAt(New Vector3(px, py, pz), New Vector3(tx, ty, tz), New Vector3(ux, uy, uz))
    End Sub
    Friend Sub Update(Optional frameTime As Single = 0) ' update position, target and matrix during given sec
        If moving Then
            UpdateMove(frameTime)
        End If
        If shifting OrElse shiftingSpeed <> 0 Then
            UpdateShift(frameTime)
        End If
        If forwarding OrElse forwardingSpeed <> 0 Then
            UpdateForward(frameTime)
        End If
        If turning Then
            UpdateTurn(frameTime)
        End If
    End Sub
    Friend Sub PrintSelf()
        Console.WriteLine("===== OrbitCamera =====")
        Console.WriteLine("  Position: " & Position.ToString)
        Console.WriteLine("    Target: " & Target.ToString)
        Console.WriteLine("    Matrix:" & _matrix.ToString)
        Console.WriteLine()

    End Sub
    ' move the camera position to the destination
    ' if duration(sec) is greater than 0, it will animate for the given duration
    ' otherwise, it will set the position immediately
    ' use moveForward() to move the camera forward/backward
    ' NOTE: you must call update() before getting the delta movement per frame
    Friend Sub MoveTo(De As Vector3, Optional duration As Single = 0, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        If duration <= 0.0F Then
            _position = De
        Else
            movingFrom = Position
            movingTo = De
            movingVector = movingTo - movingFrom
            movingVector.Normalize()
            movingTime = 0
            movingDuration = duration
            movingMode = mode
            moving = True
        End If
    End Sub

    Friend Sub MoveForward(delta As Single, Optional duration As Single = 0, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        If duration <= 0.0F Then
            _distance -= delta
        Else
            forwardingFrom = Distance
            forwardingTo = Distance - delta
            forwardingTime = 0
            forwardingDuration = duration
            forwardingMode = mode
            forwarding = True
        End If
    End Sub
    Friend Sub StartForward(Optional maxSpeed As Single = 1.0F, Optional accel As Single = 1.0F)
        forwardingSpeed = 0
        forwardingMaxSpeed = maxSpeed
        forwardingAccel = accel
        forwardingTime = 0
        forwardingDuration = 0
        forwarding = True
    End Sub
    Friend Sub StopForward()
        forwarding = False
    End Sub

    ' pan the camera, shift both position and target point in same direction; left/right/up/down
    ' use this function to offset the camera's rotation pivot
    Friend Sub ShiftTo(De As Vector3, Optional duration As Single = 0, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        If duration <= 0.0F Then
            _target = De
        Else
            shiftingFrom = _target
            shiftingTo = De
            shiftingVector = shiftingTo - shiftingFrom
            shiftingVector.Normalize()
            shiftingTime = 0
            shiftingDuration = duration
            shiftingMode = mode
            shifting = True
        End If

    End Sub
    Friend Sub Shift(delta As Vector2, Optional duration As Single = 0, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        ' get left & up vectors of camera
        Dim cameraLeft As Vector3 = -_matrix.Column0.Xyz
        Dim cameraUp As Vector3 = -_matrix.Column1.Xyz

        ' compute delta movement
        Dim deltaMovement As Vector3 = delta.X * cameraLeft
        deltaMovement += -delta.Y * cameraUp ' reverse up direction

        ' find new target position
        Dim newTarget As Vector3 = Target + deltaMovement

        ShiftTo(newTarget, duration, mode)

    End Sub
    Friend Sub StartShift(shiftVector As Vector2, Optional accel As Single = 1.0F)
        ' get left & up vectors of camera
        Dim cameraLeft As Vector3 = -_matrix.Column0.Xyz
        Dim cameraUp As Vector3 = -_matrix.Column1.Xyz

        ' compute new target vector
        Dim vector As Vector3 = shiftVector.X * cameraLeft
        vector += -shiftVector.Y * cameraUp ' reverse up direction

        shiftingMaxSpeed = shiftVector.Length()
        shiftingVector = vector
        shiftingVector.Normalize()
        shiftingSpeed = 0
        shiftingAccel = accel
        shiftingTime = 0
        shiftingDuration = 0
        shifting = True

    End Sub
    Friend Sub StopShift()
        shifting = False
    End Sub
    Friend Sub RotateTo(q As Quaternion, Optional duration As Single = 0.0F, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        quaternionUsed = True
        If duration <= 0.0F Then
            SetRotation(q)
        Else
            turningQuaternionFrom = Me.Quaternion
            turningQuaternionTo = q
            turningTime = 0
            turningDuration = duration
            turningMode = mode
            turning = True
        End If

    End Sub
    Friend Sub Rotate(deltaAngle As Vector3, Optional duration As Single = 0.0F, Optional mode As AnimationMode = AnimationMode.EASE_OUT)
        RotateTo(Angle + deltaAngle, duration, mode)
    End Sub


    Friend Sub SetPosition(x As Single, y As Single, z As Single)
        _position = New Vector3(x, y, z)
    End Sub

    Friend Sub SetTarget(x As Single, y As Single, z As Single)
        _target = New Vector3(x, y, z)
    End Sub

    Friend Sub SetRotation(ax As Single, ay As Single, az As Single)
        _angle = New Vector3(ax, ay, az)
    End Sub

    Friend Sub SetRotation(ByVal q As Quaternion)
        _quaternion = q

        ' quaternion to matrix : SongHo
        'matrixRotation = QuaternionToMatrix(q)
        ' quaternion to matrix : OpenTK
        Dim M = Matrix4.CreateFromQuaternion(q)
        matrixRotation = New Matrix3(M.Row0.Xyz, M.Row1.Xyz, M.Row2.Xyz)
        ' construct matrix
        CalculerMatrix()

        ' compute angle from matrix
        _angle = MatrixToReverseAngles(M)
    End Sub


    Friend Sub UpdateMove(ByVal frameTime As Single)
        movingTime += frameTime
        If movingTime >= movingDuration Then
            _position = movingTo
            moving = False
        Else
            Dim p As Vector3 = Interpolate(movingFrom, movingTo, movingTime / movingDuration, movingMode)
            _position = p
        End If

    End Sub

    Friend Sub UpdateShift(frameTime As Single)
        shiftingTime += frameTime
        ' shift with duration
        If shiftingDuration > 0 Then
            If shiftingTime >= shiftingDuration Then
                _target = shiftingTo
                shifting = False
            Else
                Dim p As Vector3 = Interpolate(shiftingFrom, shiftingTo, shiftingTime / shiftingDuration, shiftingMode)
                _target = p
            End If
            ' shift with acceleration
        Else
            shiftingSpeed = Accelerate(shifting, shiftingSpeed, shiftingMaxSpeed, shiftingAccel, frameTime)
            _target += (shiftingVector * shiftingSpeed * frameTime)
        End If

    End Sub

    Friend Sub UpdateForward(frameTime As Single)
        forwardingTime += frameTime

        ' move forward for duration
        If forwardingDuration > 0 Then
            If forwardingTime >= forwardingDuration Then
                _distance = forwardingTo
                forwarding = False
            Else
                Dim d As Single = Interpolate(forwardingFrom, forwardingTo, forwardingTime / forwardingDuration, forwardingMode)
                _distance = d
            End If

            ' move forward with acceleration
        Else
            forwardingSpeed = Accelerate(forwarding, forwardingSpeed, forwardingMaxSpeed, forwardingAccel, frameTime)
            _distance -= forwardingSpeed * frameTime
        End If

    End Sub

    Friend Sub UpdateTurn(ByVal frameTime As Single)
        turningTime += frameTime
        If turningTime >= turningDuration Then
            If quaternionUsed Then
                SetRotation(turningQuaternionTo)
            Else
                _angle = turningAngleTo
            End If
            turning = False
        Else
            If quaternionUsed Then
                Dim q As Quaternion = Slerp(turningQuaternionFrom, turningQuaternionTo, turningTime / turningDuration, turningMode)
                SetRotation(q)
            Else
                Dim p As Vector3 = Interpolate(turningAngleFrom, turningAngleTo, turningTime / turningDuration, turningMode)
                _angle = p
            End If
        End If

    End Sub

    '''<summary>/////////////////////////////////////////////////////////////////////////////
    ''' compute Euler angles(degree) from given 2 points(position and target)
    ''' NOTE: Roll angle cannot be computed with this function because of limitation
    ''' of params. It assumes the roll angle is 0.
    ''' Rx: rotation about X-axis, pitch
    ''' Ry: rotation about Y-axis, yaw(heading)
    '''/////////////////////////////////////////////////////////////////////////////</summary>
    Friend Shared Function LookAtToAngle(position As Vector3, target As Vector3) As Vector3
        Dim yaw, pitch As Single

        ' compute the vector from position to target poin
        Dim vec = target - position

        If vec.X = 0.0F AndAlso vec.Y = 0.0F Then ' vector is on the Y-axis, therefore, ' Yaw is 0, and Pitch will be +90 or -90.
            yaw = 0.0F
            If vec.Y >= 0.0F Then
                pitch = 90.0F ' facing along +Y
            Else
                pitch = -90.0F ' facing along -Y
            End If
        Else
            ' yaw: angle on X-Z plane (heading)
            ' yaw should be 0 if facing along +Z initially
            yaw = RadToDeg * CSng(Math.Atan2(-vec.X, vec.Z)) ' range -pi ~ +pi

            ' length of vector projected on X-Z plane
            Dim dxz As Single = CSng(Math.Sqrt(vec.X * vec.X + vec.Z * vec.Z))
            pitch = RadToDeg * CSng(Math.Atan2(vec.Y, dxz)) ' range -pi ~ +pi
        End If

        ' return angles(degree)
        Return New Vector3(pitch, yaw, 0)

    End Function
    '''<summary>///////////////////////////////////////////////////////////////////////////////
    '''// retrieve angles in degree from rotation matrix, M = Rx*Ry*Rz
    '''// Rx: rotation about X-axis, pitch
    '''// Ry: rotation about Y-axis, yaw(heading)
    '''// Rz: rotation about Z-axis, roll
    '''//    Rx           Ry          Rz
    '''// |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
    '''// |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
    '''// |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
    '''//
    '''// Pitch: atan(-m[7] / m[8]) = atan(SxCy/CxCy)
    '''// Yaw  : asin(m[6])         = asin(Sy)
    '''// Roll : atan(-m[3] / m[0]) = atan(SzCy/CzCy)
    '''///////////////////////////////////////////////////////////////////////////////</summary>
    Friend Shared Function MatrixToReverseAngles(matrix As Matrix4) As Vector3
        Dim angle As Vector3 = MatrixToEulerAngles(matrix)
        ' reverse yaw
        angle.Y = -angle.Y
        Return New Vector3(angle)
    End Function
    ' for position movement
    Private movingFrom As New Vector3() ' camera starting position
    Private movingTo As New Vector3() ' camera destination position
    Private movingVector As New Vector3() ' normalized direction vector
    Private movingTime As Single ' animation elapsed time (sec)
    Private movingDuration As Single ' animation duration (sec)
    Private moving As Boolean ' flag to start/stop animation
    Private movingMode As AnimationMode ' interpolation mode

    ' for target movement (shift)
    Private shiftingFrom As New Vector3() ' camera starting target
    Private shiftingTo As New Vector3() ' camera destination target
    Private shiftingVector As New Vector3() ' normalized direction vector
    Private shiftingTime As Single ' animation elapsed time (sec)
    Private shiftingDuration As Single ' animation duration (sec)
    Private shiftingSpeed As Single ' current velocity of shift vector
    Private shiftingAccel As Single ' acceleration, units per second squared
    Private shiftingMaxSpeed As Single ' max velocity of shift vector
    Private shifting As Boolean ' flag to start/stop animation
    Private shiftingMode As AnimationMode ' interpolation mode

    ' for forwarding using distance between position and target
    Private forwardingFrom As Single ' starting distance
    Private forwardingTo As Single ' ending distance
    Private forwardingTime As Single ' animation elapsed time (sec)
    Private forwardingDuration As Single ' animation duration (sec)
    Private forwardingSpeed As Single ' current velocity of moving forward
    Private forwardingAccel As Single ' acceleration, units per second squared
    Private forwardingMaxSpeed As Single ' max velocity of moving forward
    Private forwarding As Boolean ' flag to start/stop forwarding
    Private forwardingMode As AnimationMode ' interpolation mode

    ' for rotation
    Private turningAngleFrom As New Vector3() ' starting angles
    Private turningAngleTo As New Vector3() ' ending angles
    Private turningQuaternionFrom As New Quaternion() ' starting quaternion
    Private turningQuaternionTo As New Quaternion() ' ending quaternion
    Private turningTime As Single ' animation elapsed time (sec)
    Private turningDuration As Single ' animation duration (sec)
    Private turning As Boolean ' flag to start/stop rotation
    Private quaternionUsed As Boolean ' flag to use quaternion
    Private turningMode As AnimationMode ' interpolation mode
End Class

Friend Enum AnimationMode ' enum
    LINEAR = 0
    EASE_IN
    EASE_OUT
    EASE_IN_OUT
    BOUNCE
    ELASTIC
End Enum
