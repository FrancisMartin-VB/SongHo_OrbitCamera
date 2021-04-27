Imports System
Imports OpenTK
Imports OpenTK.Mathematics

Partial Module Utilitaires

    Friend Function Lerp(De As Vector3, Vers As Vector3, alpha As Single) As Vector3
        Return Vector3.Lerp(De, Vers, alpha)
    End Function
    Friend Function Lerp(De As Single, Vers As Single, alpha As Single) As Single
        Return De + alpha * (Vers - De)
    End Function

    Friend Function Interpolate(De As Vector3, Vers As Vector3, alpha As Single, mode As AnimationMode) As Vector3
        ' recompute alpha based on animation mode
        If mode = AnimationMode.EASE_IN Then
            '@@alpha = 1 - cosf(HALF_PI * alpha);
            ' with cubic function
            alpha = alpha * alpha * alpha
        ElseIf mode = AnimationMode.EASE_OUT Then
            '@@alpha = sinf(HALF_PI * alpha);
            ' with cubic function
            Dim beta As Single = 1 - alpha
            alpha = 1 - beta * beta * beta
        ElseIf mode = AnimationMode.EASE_IN_OUT Then
            '@@alpha = 0.5f * (1 - cosf(PI * alpha));
            ' with cubic function
            Dim beta As Single = 1 - alpha
            Dim scale As Single = 4.0F ' 0.5 / (0.5^3)
            If alpha < 0.5F Then
                alpha = alpha * alpha * alpha * scale
            Else
                alpha = 1 - (beta * beta * beta * scale)
            End If
        ElseIf mode = AnimationMode.BOUNCE Then
        ElseIf mode = AnimationMode.ELASTIC Then
        End If

        Return De + alpha * (Vers - De)
    End Function

    Friend Function Interpolate(De As Single, Vers As Single, alpha As Single, mode As AnimationMode) As Single
        ' recompute alpha based on animation mode
        If mode = AnimationMode.EASE_IN Then
            '@@alpha = 1 - cosf(HALF_PI * alpha);
            ' with cubic function
            alpha = alpha * alpha * alpha
        ElseIf mode = AnimationMode.EASE_OUT Then
            '@@alpha = sinf(HALF_PI * alpha);
            ' with cubic function
            Dim beta As Single = 1 - alpha
            alpha = 1 - beta * beta * beta
        ElseIf mode = AnimationMode.EASE_IN_OUT Then
            '@@alpha = 0.5f * (1 - cosf(PI * alpha));
            ' with cubic function
            Dim beta As Single = 1 - alpha
            Dim scale As Single = 4.0F ' 0.5 / (0.5^3)
            If alpha < 0.5F Then
                alpha = alpha * alpha * alpha * scale
            Else
                alpha = 1 - (beta * beta * beta * scale)
            End If
        ElseIf mode = AnimationMode.BOUNCE Then
        ElseIf mode = AnimationMode.ELASTIC Then
        End If

        Return De + alpha * (Vers - De)
    End Function

    Friend Function Accelerate(isMoving As Boolean, speed As Single, maxSpeed As Single, accel As Single, deltaTime As Single) As Single
        ' determine direction
        Dim sign As Single
        If maxSpeed > 0 Then
            sign = 1
        Else
            sign = -1
        End If

        ' accelerating
        If isMoving Then
            speed += sign * accel * deltaTime
            If (sign * speed) > (sign * maxSpeed) Then
                speed = maxSpeed
            End If
            ' deaccelerating
        Else
            speed -= sign * accel * deltaTime
            If (sign * speed) < 0 Then
                speed = 0
            End If
        End If

        Return speed
    End Function

    Friend Function GetFrame(frameStart As Integer, frameEnd As Integer, time As Single, fps As Single, Boucle As Boolean) As Integer
        Dim frame As Integer = frameStart + CInt(Math.Truncate(fps * time + 0.5F))
        If Boucle Then
            frame = frame Mod (frameEnd - frameStart + 1)
        Else
            If frame > frameEnd Then
                frame = frameEnd
            End If
        End If
        Return frame
    End Function

    Friend Function Slerp(De As Vector3, Vers As Vector3, alpha As Single, mode As AnimationMode) As Vector3
        ' re-compute alpha
        Dim t As Single = Interpolate(0.0F, 1.0F, alpha, mode)

        ' determine the angle between
        '@@ FIXME: handle if angle is ~180 degree
        'float dot = from.dot(to);
        Dim cosine As Single = Vector3.Dot(De, Vers) / (De.Length() * Vers.Length())
        Dim angle As Single = CSng(Math.Acos(cosine))
        Dim invSine As Single = 1.0F / CSng(Math.Sin(angle))

        ' compute the scale factors
        Dim scale1 As Single = CSng(Math.Sin((1 - t) * angle)) * invSine
        Dim scale2 As Single = CSng(Math.Sin(t * angle)) * invSine

        ' compute slerp-ed vector
        Return scale1 * De + scale2 * Vers
    End Function

    Friend Function Slerp(De As Quaternion, Vers As Quaternion, ByVal alpha As Single, mode As AnimationMode) As Quaternion
        ' re-compute alpha
        Dim t As Single = Interpolate(0.0F, 1.0F, alpha, mode)
        Dim angle As Single
        Dim dot As Single = De.W * Vers.W + De.X * Vers.X + De.Y * Vers.Y + De.Z * Vers.Z

        ' if 2 quaternions are close (angle ~= 0), then use lerp
        If 1 - dot < 0.001F Then
            Dim R = (De + (Vers - De) * t)
            Return (De + (Vers - De) * t)
            ' if angle is ~180 degree, then the rotation axis is undefined
            ' try to find any rotation axis in this case
        ElseIf Math.Abs(1 + dot) < 0.001F Then ' dot ~= -1
            Dim v1 = De.Xyz
            v1.Normalize()
            Dim up As Vector3
            If Math.Abs(De.X) < 0.001F Then
                up = New Vector3(1, 0, 0)
            Else
                up = New Vector3(0, 1, 0)
            End If
            Dim v2 = Vector3.Cross(v1, up) ' orthonormal to v1
            v2.Normalize()
            'std::cout << v2 << std::endl;

            ' referenced from Jonathan Blow's Understanding Slerp
            angle = CSng(Math.Cos(dot) * t)
            Dim v3 As Vector3 = v1 * CSng(Math.Cos(angle)) + v2 * CSng(Math.Sin(angle))
            Return New Quaternion(0, v3.X, v3.Y, v3.Z)
        End If

        ' determine the angle between
        angle = CSng(Math.Cos(dot))
        Dim invSine As Single = 1.0F / CSng(Math.Sin(angle))

        ' compute the scale factors
        Dim scale1 As Single = CSng(Math.Sin((1 - t) * angle)) * invSine
        Dim scale2 As Single = CSng(Math.Sin(t * angle)) * invSine
        Dim R1 = De * scale1 + Vers * scale2
        Return R1
    End Function

    ''' <summary> renvoie une matrice de point de vue à partir de la cible et d'un vecteur de verticalité
    ''' la postion du point de vue étant incorporé à la matrice. C'est donc en partie l'inverse 
    ''' du Matrix.lookat classique</summary>
    ''' <param name="M"></param>
    ''' <param name="target"></param>
    ''' <param name="upVec"></param>
    Friend Sub MatriceGetLookAt(ByRef M As Matrix4, target As Vector3, upVec As Vector3)
        ' compute forward vector and normalize
        Dim position As Vector3 = M.Row3.Xyz '(m(12), m(13), m(14))
        Dim forward As Vector3 = target - position
        forward.Normalize()

        ' compute left vector
        Dim left As Vector3 = Vector3.Cross(upVec, forward)
        left.Normalize()

        ' compute orthonormal up vector
        Dim up As Vector3 = Vector3.Cross(forward, left)
        up.Normalize()

        ' NOTE: overwrite rotation and scale info of the current matrix
        M.Row0.Xyz = left
        M.Row1.Xyz = up
        M.Row2.Xyz = forward
    End Sub

End Module
