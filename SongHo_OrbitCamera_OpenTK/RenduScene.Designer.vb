Imports System.ComponentModel

'<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RenduScene
    Inherits Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    '<System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    '<System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CameraAngles = New System.Windows.Forms.GroupBox()
        Me.LabelAngleR = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.BarRoll = New System.Windows.Forms.TrackBar()
        Me.LabelAngleY = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.BarYaw = New System.Windows.Forms.TrackBar()
        Me.LabelAngleP = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BarPitch = New System.Windows.Forms.TrackBar()
        Me.CameraTargets = New System.Windows.Forms.GroupBox()
        Me.LabelTargetZ = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.BarZTarget = New System.Windows.Forms.TrackBar()
        Me.LabelTargetY = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.BarYTarget = New System.Windows.Forms.TrackBar()
        Me.LabelTargetX = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.BarXTarget = New System.Windows.Forms.TrackBar()
        Me.CameraPositions = New System.Windows.Forms.GroupBox()
        Me.LabelPositionZ = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BarZPosition = New System.Windows.Forms.TrackBar()
        Me.LabelPositionY = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.BarYPosition = New System.Windows.Forms.TrackBar()
        Me.LabelPositionX = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.BarXPosition = New System.Windows.Forms.TrackBar()
        Me.CameraMatrix = New System.Windows.Forms.GroupBox()
        '1ère colonne
        Me.M11 = New System.Windows.Forms.Label()
        Me.M12 = New System.Windows.Forms.Label()
        Me.M13 = New System.Windows.Forms.Label()
        Me.M14 = New System.Windows.Forms.Label()
        '2ème colonne
        Me.M21 = New System.Windows.Forms.Label()
        Me.M22 = New System.Windows.Forms.Label()
        Me.M23 = New System.Windows.Forms.Label()
        Me.M24 = New System.Windows.Forms.Label()
        '3ème colonne
        Me.M31 = New System.Windows.Forms.Label()
        Me.M32 = New System.Windows.Forms.Label()
        Me.M33 = New System.Windows.Forms.Label()
        Me.M34 = New System.Windows.Forms.Label()
        '4ème colonne
        Me.M41 = New System.Windows.Forms.Label()
        Me.M42 = New System.Windows.Forms.Label()
        Me.M43 = New System.Windows.Forms.Label()
        Me.M44 = New System.Windows.Forms.Label()
        Me.ResetCamera = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.QuatZ = New System.Windows.Forms.Label()
        Me.QuatY = New System.Windows.Forms.Label()
        Me.QuatX = New System.Windows.Forms.Label()
        Me.QuatS = New System.Windows.Forms.Label()
        Me.ShowGRID = New System.Windows.Forms.CheckBox()
        Me.ShowFOV = New System.Windows.Forms.CheckBox()
        Me.Fov = New System.Windows.Forms.NumericUpDown()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CameraAngles.SuspendLayout()
        CType(Me.BarRoll, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarYaw, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarPitch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CameraTargets.SuspendLayout()
        CType(Me.BarZTarget, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarYTarget, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarXTarget, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CameraPositions.SuspendLayout()
        CType(Me.BarZPosition, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarYPosition, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarXPosition, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CameraMatrix.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.Fov, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CameraAngles
        '
        Me.CameraAngles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CameraAngles.Controls.Add(Me.LabelAngleR)
        Me.CameraAngles.Controls.Add(Me.Label5)
        Me.CameraAngles.Controls.Add(Me.BarRoll)
        Me.CameraAngles.Controls.Add(Me.LabelAngleY)
        Me.CameraAngles.Controls.Add(Me.Label3)
        Me.CameraAngles.Controls.Add(Me.BarYaw)
        Me.CameraAngles.Controls.Add(Me.LabelAngleP)
        Me.CameraAngles.Controls.Add(Me.Label1)
        Me.CameraAngles.Controls.Add(Me.BarPitch)
        Me.CameraAngles.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.CameraAngles.Location = New System.Drawing.Point(13, 506)
        Me.CameraAngles.Name = "CameraAngles"
        Me.CameraAngles.Size = New System.Drawing.Size(255, 105)
        Me.CameraAngles.TabIndex = 2
        Me.CameraAngles.TabStop = False
        Me.CameraAngles.Text = "Angles (degrés)"
        '
        'LabelAngleR
        '
        Me.LabelAngleR.Location = New System.Drawing.Point(206, 75)
        Me.LabelAngleR.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelAngleR.Name = "LabelAngleR"
        Me.LabelAngleR.Size = New System.Drawing.Size(40, 15)
        Me.LabelAngleR.TabIndex = 8
        Me.LabelAngleR.Text = "0"
        Me.LabelAngleR.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(6, 75)
        Me.Label5.Margin = New System.Windows.Forms.Padding(0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(50, 15)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "Roll (Z)"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarRoll
        '
        Me.BarRoll.AutoSize = False
        Me.BarRoll.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarRoll.LargeChange = 45
        Me.BarRoll.Location = New System.Drawing.Point(55, 73)
        Me.BarRoll.Margin = New System.Windows.Forms.Padding(0)
        Me.BarRoll.Maximum = 180
        Me.BarRoll.Minimum = -180
        Me.BarRoll.Name = "BarRoll"
        Me.BarRoll.Size = New System.Drawing.Size(150, 20)
        Me.BarRoll.TabIndex = 6
        Me.BarRoll.Tag = "2"
        Me.BarRoll.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarRoll.Value = 90
        '
        'LabelAngleY
        '
        Me.LabelAngleY.Location = New System.Drawing.Point(206, 47)
        Me.LabelAngleY.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelAngleY.Name = "LabelAngleY"
        Me.LabelAngleY.Size = New System.Drawing.Size(40, 15)
        Me.LabelAngleY.TabIndex = 5
        Me.LabelAngleY.Text = "0"
        Me.LabelAngleY.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(6, 47)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(50, 15)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Yaw (Y)"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarYaw
        '
        Me.BarYaw.AutoSize = False
        Me.BarYaw.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarYaw.LargeChange = 45
        Me.BarYaw.Location = New System.Drawing.Point(55, 45)
        Me.BarYaw.Margin = New System.Windows.Forms.Padding(0)
        Me.BarYaw.Maximum = 180
        Me.BarYaw.Minimum = -180
        Me.BarYaw.Name = "BarYaw"
        Me.BarYaw.Size = New System.Drawing.Size(150, 20)
        Me.BarYaw.TabIndex = 3
        Me.BarYaw.Tag = "1"
        Me.BarYaw.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarYaw.Value = 90
        '
        'LabelAngleP
        '
        Me.LabelAngleP.Location = New System.Drawing.Point(206, 20)
        Me.LabelAngleP.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelAngleP.Name = "LabelAngleP"
        Me.LabelAngleP.Size = New System.Drawing.Size(40, 15)
        Me.LabelAngleP.TabIndex = 2
        Me.LabelAngleP.Text = "0"
        Me.LabelAngleP.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(6, 20)
        Me.Label1.Margin = New System.Windows.Forms.Padding(0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 15)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Pitch (X)"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarPitch
        '
        Me.BarPitch.AutoSize = False
        Me.BarPitch.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarPitch.LargeChange = 45
        Me.BarPitch.Location = New System.Drawing.Point(55, 18)
        Me.BarPitch.Margin = New System.Windows.Forms.Padding(0)
        Me.BarPitch.Maximum = 180
        Me.BarPitch.Minimum = -180
        Me.BarPitch.Name = "BarPitch"
        Me.BarPitch.Size = New System.Drawing.Size(150, 20)
        Me.BarPitch.TabIndex = 0
        Me.BarPitch.Tag = "0"
        Me.BarPitch.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarPitch.Value = 90
        '
        'CameraTargets
        '
        Me.CameraTargets.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CameraTargets.Controls.Add(Me.LabelTargetZ)
        Me.CameraTargets.Controls.Add(Me.Label6)
        Me.CameraTargets.Controls.Add(Me.BarZTarget)
        Me.CameraTargets.Controls.Add(Me.LabelTargetY)
        Me.CameraTargets.Controls.Add(Me.Label10)
        Me.CameraTargets.Controls.Add(Me.BarYTarget)
        Me.CameraTargets.Controls.Add(Me.LabelTargetX)
        Me.CameraTargets.Controls.Add(Me.Label12)
        Me.CameraTargets.Controls.Add(Me.BarXTarget)
        Me.CameraTargets.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.CameraTargets.Location = New System.Drawing.Point(502, 506)
        Me.CameraTargets.Name = "CameraTargets"
        Me.CameraTargets.Size = New System.Drawing.Size(222, 105)
        Me.CameraTargets.TabIndex = 10
        Me.CameraTargets.TabStop = False
        Me.CameraTargets.Text = "Camera Target"
        '
        'LabelTargetZ
        '
        Me.LabelTargetZ.Location = New System.Drawing.Point(176, 75)
        Me.LabelTargetZ.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelTargetZ.Name = "LabelTargetZ"
        Me.LabelTargetZ.Size = New System.Drawing.Size(40, 15)
        Me.LabelTargetZ.TabIndex = 8
        Me.LabelTargetZ.Text = "0.0"
        Me.LabelTargetZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(6, 75)
        Me.Label6.Margin = New System.Windows.Forms.Padding(0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(15, 15)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = "Z"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarZTarget
        '
        Me.BarZTarget.AutoSize = False
        Me.BarZTarget.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarZTarget.LargeChange = 25
        Me.BarZTarget.Location = New System.Drawing.Point(25, 73)
        Me.BarZTarget.Margin = New System.Windows.Forms.Padding(0)
        Me.BarZTarget.Maximum = 100
        Me.BarZTarget.Minimum = -100
        Me.BarZTarget.Name = "BarZTarget"
        Me.BarZTarget.Size = New System.Drawing.Size(150, 20)
        Me.BarZTarget.TabIndex = 6
        Me.BarZTarget.Tag = "2"
        Me.BarZTarget.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarZTarget.Value = 50
        '
        'LabelTargetY
        '
        Me.LabelTargetY.Location = New System.Drawing.Point(176, 47)
        Me.LabelTargetY.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelTargetY.Name = "LabelTargetY"
        Me.LabelTargetY.Size = New System.Drawing.Size(40, 15)
        Me.LabelTargetY.TabIndex = 5
        Me.LabelTargetY.Text = "0.0"
        Me.LabelTargetY.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(6, 47)
        Me.Label10.Margin = New System.Windows.Forms.Padding(0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(15, 15)
        Me.Label10.TabIndex = 4
        Me.Label10.Text = "Y"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarYTarget
        '
        Me.BarYTarget.AutoSize = False
        Me.BarYTarget.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarYTarget.LargeChange = 25
        Me.BarYTarget.Location = New System.Drawing.Point(25, 45)
        Me.BarYTarget.Margin = New System.Windows.Forms.Padding(0)
        Me.BarYTarget.Maximum = 100
        Me.BarYTarget.Minimum = -100
        Me.BarYTarget.Name = "BarYTarget"
        Me.BarYTarget.Size = New System.Drawing.Size(150, 20)
        Me.BarYTarget.TabIndex = 3
        Me.BarYTarget.Tag = "1"
        Me.BarYTarget.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarYTarget.Value = 50
        '
        'LabelTargetX
        '
        Me.LabelTargetX.Location = New System.Drawing.Point(176, 20)
        Me.LabelTargetX.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelTargetX.Name = "LabelTargetX"
        Me.LabelTargetX.Size = New System.Drawing.Size(40, 15)
        Me.LabelTargetX.TabIndex = 2
        Me.LabelTargetX.Text = "0.0"
        Me.LabelTargetX.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(6, 20)
        Me.Label12.Margin = New System.Windows.Forms.Padding(0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(15, 15)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "X"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarXTarget
        '
        Me.BarXTarget.AutoSize = False
        Me.BarXTarget.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarXTarget.LargeChange = 25
        Me.BarXTarget.Location = New System.Drawing.Point(25, 18)
        Me.BarXTarget.Margin = New System.Windows.Forms.Padding(0)
        Me.BarXTarget.Maximum = 100
        Me.BarXTarget.Minimum = -100
        Me.BarXTarget.Name = "BarXTarget"
        Me.BarXTarget.Size = New System.Drawing.Size(150, 20)
        Me.BarXTarget.TabIndex = 0
        Me.BarXTarget.Tag = "0"
        Me.BarXTarget.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarXTarget.Value = 50
        '
        'CameraPositions
        '
        Me.CameraPositions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CameraPositions.Controls.Add(Me.LabelPositionZ)
        Me.CameraPositions.Controls.Add(Me.Label4)
        Me.CameraPositions.Controls.Add(Me.BarZPosition)
        Me.CameraPositions.Controls.Add(Me.LabelPositionY)
        Me.CameraPositions.Controls.Add(Me.Label7)
        Me.CameraPositions.Controls.Add(Me.BarYPosition)
        Me.CameraPositions.Controls.Add(Me.LabelPositionX)
        Me.CameraPositions.Controls.Add(Me.Label9)
        Me.CameraPositions.Controls.Add(Me.BarXPosition)
        Me.CameraPositions.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.CameraPositions.Location = New System.Drawing.Point(274, 506)
        Me.CameraPositions.Name = "CameraPositions"
        Me.CameraPositions.Size = New System.Drawing.Size(222, 105)
        Me.CameraPositions.TabIndex = 9
        Me.CameraPositions.TabStop = False
        Me.CameraPositions.Text = "Camera Position"
        '
        'LabelPositionZ
        '
        Me.LabelPositionZ.Location = New System.Drawing.Point(176, 75)
        Me.LabelPositionZ.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelPositionZ.Name = "LabelPositionZ"
        Me.LabelPositionZ.Size = New System.Drawing.Size(40, 15)
        Me.LabelPositionZ.TabIndex = 8
        Me.LabelPositionZ.Text = "0.0"
        Me.LabelPositionZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(6, 75)
        Me.Label4.Margin = New System.Windows.Forms.Padding(0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(15, 15)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Z"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarZPosition
        '
        Me.BarZPosition.AutoSize = False
        Me.BarZPosition.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarZPosition.LargeChange = 25
        Me.BarZPosition.Location = New System.Drawing.Point(25, 73)
        Me.BarZPosition.Margin = New System.Windows.Forms.Padding(0)
        Me.BarZPosition.Maximum = 100
        Me.BarZPosition.Minimum = -100
        Me.BarZPosition.Name = "BarZPosition"
        Me.BarZPosition.Size = New System.Drawing.Size(150, 20)
        Me.BarZPosition.TabIndex = 6
        Me.BarZPosition.Tag = "2"
        Me.BarZPosition.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarZPosition.Value = -50
        '
        'LabelPositionY
        '
        Me.LabelPositionY.Location = New System.Drawing.Point(176, 47)
        Me.LabelPositionY.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelPositionY.Name = "LabelPositionY"
        Me.LabelPositionY.Size = New System.Drawing.Size(40, 15)
        Me.LabelPositionY.TabIndex = 5
        Me.LabelPositionY.Text = "0.0"
        Me.LabelPositionY.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(6, 47)
        Me.Label7.Margin = New System.Windows.Forms.Padding(0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(15, 15)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "Y"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarYPosition
        '
        Me.BarYPosition.AutoSize = False
        Me.BarYPosition.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarYPosition.LargeChange = 25
        Me.BarYPosition.Location = New System.Drawing.Point(25, 45)
        Me.BarYPosition.Margin = New System.Windows.Forms.Padding(0)
        Me.BarYPosition.Maximum = 100
        Me.BarYPosition.Minimum = -100
        Me.BarYPosition.Name = "BarYPosition"
        Me.BarYPosition.Size = New System.Drawing.Size(150, 20)
        Me.BarYPosition.TabIndex = 3
        Me.BarYPosition.Tag = "1"
        Me.BarYPosition.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarYPosition.Value = -50
        '
        'LabelPositionX
        '
        Me.LabelPositionX.Location = New System.Drawing.Point(176, 20)
        Me.LabelPositionX.Margin = New System.Windows.Forms.Padding(0)
        Me.LabelPositionX.Name = "LabelPositionX"
        Me.LabelPositionX.Size = New System.Drawing.Size(40, 15)
        Me.LabelPositionX.TabIndex = 2
        Me.LabelPositionX.Text = "0.0"
        Me.LabelPositionX.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(6, 20)
        Me.Label9.Margin = New System.Windows.Forms.Padding(0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(15, 15)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "X"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'BarXPosition
        '
        Me.BarXPosition.AutoSize = False
        Me.BarXPosition.Cursor = System.Windows.Forms.Cursors.Default
        Me.BarXPosition.LargeChange = 25
        Me.BarXPosition.Location = New System.Drawing.Point(25, 18)
        Me.BarXPosition.Margin = New System.Windows.Forms.Padding(0)
        Me.BarXPosition.Maximum = 100
        Me.BarXPosition.Minimum = -100
        Me.BarXPosition.Name = "BarXPosition"
        Me.BarXPosition.Size = New System.Drawing.Size(150, 20)
        Me.BarXPosition.TabIndex = 0
        Me.BarXPosition.Tag = "0"
        Me.BarXPosition.TickStyle = System.Windows.Forms.TickStyle.None
        Me.BarXPosition.Value = -50
        '
        'CameraMatrix
        '
        Me.CameraMatrix.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CameraMatrix.Controls.Add(Me.M11)
        Me.CameraMatrix.Controls.Add(Me.M12)
        Me.CameraMatrix.Controls.Add(Me.M13)
        Me.CameraMatrix.Controls.Add(Me.M14)
        Me.CameraMatrix.Controls.Add(Me.M21)
        Me.CameraMatrix.Controls.Add(Me.M22)
        Me.CameraMatrix.Controls.Add(Me.M23)
        Me.CameraMatrix.Controls.Add(Me.M24)
        Me.CameraMatrix.Controls.Add(Me.M31)
        Me.CameraMatrix.Controls.Add(Me.M32)
        Me.CameraMatrix.Controls.Add(Me.M33)
        Me.CameraMatrix.Controls.Add(Me.M34)
        Me.CameraMatrix.Controls.Add(Me.M41)
        Me.CameraMatrix.Controls.Add(Me.M42)
        Me.CameraMatrix.Controls.Add(Me.M43)
        Me.CameraMatrix.Controls.Add(Me.M44)
        Me.CameraMatrix.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.CameraMatrix.Location = New System.Drawing.Point(731, 506)
        Me.CameraMatrix.Name = "CameraMatrix"
        Me.CameraMatrix.Size = New System.Drawing.Size(220, 85)
        Me.CameraMatrix.TabIndex = 11
        Me.CameraMatrix.TabStop = False
        Me.CameraMatrix.Text = "       Camera Matrix (column-major)         "
        '
        'M44
        '
        Me.M44.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M44.Location = New System.Drawing.Point(160, 62)
        Me.M44.Name = "M44"
        Me.M44.Size = New System.Drawing.Size(52, 15)
        Me.M44.TabIndex = 18
        Me.M44.Text = "-0.000"
        Me.M44.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M34
        '
        Me.M34.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M34.Location = New System.Drawing.Point(109, 62)
        Me.M34.Name = "M34"
        Me.M34.Size = New System.Drawing.Size(52, 15)
        Me.M34.TabIndex = 17
        Me.M34.Text = "-0.000"
        Me.M34.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M24
        '
        Me.M24.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M24.Location = New System.Drawing.Point(58, 62)
        Me.M24.Name = "M24"
        Me.M24.Size = New System.Drawing.Size(52, 15)
        Me.M24.TabIndex = 16
        Me.M24.Text = "-0.000"
        Me.M24.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M14
        '
        Me.M14.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M14.Location = New System.Drawing.Point(7, 62)
        Me.M14.Name = "M14"
        Me.M14.Size = New System.Drawing.Size(52, 15)
        Me.M14.TabIndex = 15
        Me.M14.Text = "-0.000"
        Me.M14.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M43
        '
        Me.M43.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M43.Location = New System.Drawing.Point(160, 47)
        Me.M43.Name = "M43"
        Me.M43.Size = New System.Drawing.Size(52, 15)
        Me.M43.TabIndex = 14
        Me.M43.Text = "-0.000"
        Me.M43.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M33
        '
        Me.M33.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M33.Location = New System.Drawing.Point(109, 47)
        Me.M33.Name = "M33"
        Me.M33.Size = New System.Drawing.Size(52, 15)
        Me.M33.TabIndex = 13
        Me.M33.Text = "-0.000"
        Me.M33.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M23
        '
        Me.M23.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M23.Location = New System.Drawing.Point(58, 47)
        Me.M23.Name = "M23"
        Me.M23.Size = New System.Drawing.Size(52, 15)
        Me.M23.TabIndex = 12
        Me.M23.Text = "-0.000"
        Me.M23.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M13
        '
        Me.M13.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M13.Location = New System.Drawing.Point(7, 47)
        Me.M13.Name = "M13"
        Me.M13.Size = New System.Drawing.Size(52, 15)
        Me.M13.TabIndex = 11
        Me.M13.Text = "-0.000"
        Me.M13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M42
        '
        Me.M42.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M42.Location = New System.Drawing.Point(160, 32)
        Me.M42.Name = "M42"
        Me.M42.Size = New System.Drawing.Size(52, 15)
        Me.M42.TabIndex = 10
        Me.M42.Text = "-0.000"
        Me.M42.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M32
        '
        Me.M32.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M32.Location = New System.Drawing.Point(109, 32)
        Me.M32.Name = "M32"
        Me.M32.Size = New System.Drawing.Size(52, 15)
        Me.M32.TabIndex = 9
        Me.M32.Text = "-0.000"
        Me.M32.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M22
        '
        Me.M22.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M22.Location = New System.Drawing.Point(58, 32)
        Me.M22.Name = "M22"
        Me.M22.Size = New System.Drawing.Size(52, 15)
        Me.M22.TabIndex = 8
        Me.M22.Text = "-0.000"
        Me.M22.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M12
        '
        Me.M12.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M12.Location = New System.Drawing.Point(7, 32)
        Me.M12.Name = "M12"
        Me.M12.Size = New System.Drawing.Size(52, 15)
        Me.M12.TabIndex = 7
        Me.M12.Text = "-0.000"
        Me.M12.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M41
        '
        Me.M41.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M41.Location = New System.Drawing.Point(160, 17)
        Me.M41.Name = "M41"
        Me.M41.Size = New System.Drawing.Size(52, 15)
        Me.M41.TabIndex = 6
        Me.M41.Text = "-0.000"
        Me.M41.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M31
        '
        Me.M31.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M31.Location = New System.Drawing.Point(109, 17)
        Me.M31.Name = "M31"
        Me.M31.Size = New System.Drawing.Size(52, 15)
        Me.M31.TabIndex = 5
        Me.M31.Text = "-0.000"
        Me.M31.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M21
        '
        Me.M21.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M21.Location = New System.Drawing.Point(58, 17)
        Me.M21.Name = "M21"
        Me.M21.Size = New System.Drawing.Size(52, 15)
        Me.M21.TabIndex = 4
        Me.M21.Text = "-0.000"
        Me.M21.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'M11
        '
        Me.M11.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.M11.Location = New System.Drawing.Point(7, 17)
        Me.M11.Name = "M11"
        Me.M11.Size = New System.Drawing.Size(52, 15)
        Me.M11.TabIndex = 0
        Me.M11.Text = "-0.000"
        Me.M11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ResetCamera
        '
        Me.ResetCamera.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ResetCamera.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ResetCamera.Location = New System.Drawing.Point(12, 617)
        Me.ResetCamera.Name = "ResetCamera"
        Me.ResetCamera.Size = New System.Drawing.Size(113, 23)
        Me.ResetCamera.TabIndex = 12
        Me.ResetCamera.Text = "Reset Cameras"
        Me.ResetCamera.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.QuatZ)
        Me.GroupBox1.Controls.Add(Me.QuatY)
        Me.GroupBox1.Controls.Add(Me.QuatX)
        Me.GroupBox1.Controls.Add(Me.QuatS)
        Me.GroupBox1.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.GroupBox1.Location = New System.Drawing.Point(731, 595)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(220, 43)
        Me.GroupBox1.TabIndex = 19
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Camera Quaternion : (s, x, y, z)"
        '
        'QuatZ
        '
        Me.QuatZ.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.QuatZ.Location = New System.Drawing.Point(160, 17)
        Me.QuatZ.Name = "QuatZ"
        Me.QuatZ.Size = New System.Drawing.Size(52, 15)
        Me.QuatZ.TabIndex = 6
        Me.QuatZ.Text = "-0.000"
        Me.QuatZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'QuatY
        '
        Me.QuatY.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.QuatY.Location = New System.Drawing.Point(109, 17)
        Me.QuatY.Name = "QuatY"
        Me.QuatY.Size = New System.Drawing.Size(52, 15)
        Me.QuatY.TabIndex = 5
        Me.QuatY.Text = "-0.000"
        Me.QuatY.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'QuatX
        '
        Me.QuatX.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.QuatX.Location = New System.Drawing.Point(58, 17)
        Me.QuatX.Name = "QuatX"
        Me.QuatX.Size = New System.Drawing.Size(52, 15)
        Me.QuatX.TabIndex = 4
        Me.QuatX.Text = "-0.000"
        Me.QuatX.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'QuatS
        '
        Me.QuatS.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.QuatS.Location = New System.Drawing.Point(7, 17)
        Me.QuatS.Name = "QuatS"
        Me.QuatS.Size = New System.Drawing.Size(52, 15)
        Me.QuatS.TabIndex = 0
        Me.QuatS.Text = "-0.000"
        Me.QuatS.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ShowGRID
        '
        Me.ShowGRID.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ShowGRID.AutoSize = True
        Me.ShowGRID.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ShowGRID.Location = New System.Drawing.Point(160, 620)
        Me.ShowGRID.Name = "ShowGRID"
        Me.ShowGRID.Size = New System.Drawing.Size(80, 17)
        Me.ShowGRID.TabIndex = 20
        Me.ShowGRID.Text = "Show Grid"
        Me.ShowGRID.UseVisualStyleBackColor = True
        '
        'ShowFOV
        '
        Me.ShowFOV.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ShowFOV.AutoSize = True
        Me.ShowFOV.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.ShowFOV.Location = New System.Drawing.Point(258, 621)
        Me.ShowFOV.Name = "ShowFOV"
        Me.ShowFOV.Size = New System.Drawing.Size(80, 17)
        Me.ShowFOV.TabIndex = 21
        Me.ShowFOV.Text = "Show FOV"
        Me.ShowFOV.UseVisualStyleBackColor = True
        '
        'Fov
        '
        Me.Fov.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Fov.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point)
        Me.Fov.Location = New System.Drawing.Point(489, 616)
        Me.Fov.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.Fov.Name = "Fov"
        Me.Fov.Size = New System.Drawing.Size(59, 22)
        Me.Fov.TabIndex = 22
        Me.Fov.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.Fov.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(364, 620)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(125, 15)
        Me.Label2.TabIndex = 23
        Me.Label2.Text = "Vertical FOV (degrés) : "
        '
        'RenduScene
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.ClientSize = New System.Drawing.Size(992, 646)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Fov)
        Me.Controls.Add(Me.ShowFOV)
        Me.Controls.Add(Me.ShowGRID)
        Me.Controls.Add(Me.ResetCamera)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.CameraAngles)
        Me.Controls.Add(Me.CameraPositions)
        Me.Controls.Add(Me.CameraMatrix)
        Me.Controls.Add(Me.CameraTargets)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.KeyPreview = True
        Me.Name = "RenduScene"
        Me.Text = "Form1"
        Me.CameraAngles.ResumeLayout(False)
        CType(Me.BarRoll, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarYaw, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarPitch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CameraTargets.ResumeLayout(False)
        CType(Me.BarZTarget, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarYTarget, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarXTarget, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CameraPositions.ResumeLayout(False)
        CType(Me.BarZPosition, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarYPosition, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarXPosition, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CameraMatrix.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.Fov, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#Region "Variables"
    Private CameraAngles As GroupBox
    Private LabelAngleP As Label
    Private Label1 As Label
    Private BarPitch As TrackBar
    Private LabelAngleR As Label
    Private Label5 As Label
    Private BarRoll As TrackBar
    Private LabelAngleY As Label
    Private Label3 As Label
    Private BarYaw As TrackBar
    Private CameraPositions As GroupBox
    Private LabelPositionZ As Label
    Private Label4 As Label
    Private BarZPosition As TrackBar
    Private LabelPositionY As Label
    Private Label7 As Label
    Private BarYPosition As TrackBar
    Private LabelPositionX As Label
    Private Label9 As Label
    Private BarXPosition As TrackBar
    Private CameraTargets As GroupBox
    Private LabelTargetZ As Label
    Private Label6 As Label
    Private BarZTarget As TrackBar
    Private LabelTargetY As Label
    Private Label10 As Label
    Private BarYTarget As TrackBar
    Private LabelTargetX As Label
    Private Label12 As Label
    Private BarXTarget As TrackBar
    Private CameraMatrix As GroupBox
    Private M11 As Label
    Private M44 As Label
    Private M34 As Label
    Private M24 As Label
    Private M14 As Label
    Private M43 As Label
    Private M33 As Label
    Private M23 As Label
    Private M13 As Label
    Private M42 As Label
    Private M32 As Label
    Private M22 As Label
    Private M12 As Label
    Private M41 As Label
    Private M31 As Label
    Private M21 As Label
    Private ResetCamera As Button
    Private Label2 As Label
    Private Fov As NumericUpDown
    Private ShowFOV As CheckBox
    Private ShowGRID As CheckBox
    Private GroupBox1 As GroupBox
    Private QuatZ As Label
    Private QuatY As Label
    Private QuatX As Label
    Private QuatS As Label
#End Region
End Class
