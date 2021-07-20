<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class settingsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.radiobuttonMultiMonitorOn = New System.Windows.Forms.RadioButton()
        Me.radiobuttonMultiMonitorOff = New System.Windows.Forms.RadioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.radiobuttonNoTree = New System.Windows.Forms.RadioButton()
        Me.radiobuttonDisplayTree = New System.Windows.Forms.RadioButton()
        Me.buttonOK = New System.Windows.Forms.Button()
        Me.buttonCancel = New System.Windows.Forms.Button()
        Me.pictureboxDisplayMode = New System.Windows.Forms.PictureBox()
        Me.GroupBox1.SuspendLayout
        Me.GroupBox2.SuspendLayout
        CType(Me.pictureboxDisplayMode,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'radiobuttonMultiMonitorOn
        '
        Me.radiobuttonMultiMonitorOn.AutoSize = true
        Me.radiobuttonMultiMonitorOn.Location = New System.Drawing.Point(259, 50)
        Me.radiobuttonMultiMonitorOn.Name = "radiobuttonMultiMonitorOn"
        Me.radiobuttonMultiMonitorOn.Size = New System.Drawing.Size(85, 17)
        Me.radiobuttonMultiMonitorOn.TabIndex = 0
        Me.radiobuttonMultiMonitorOn.TabStop = true
        Me.radiobuttonMultiMonitorOn.Text = "Multi-Monitor"
        Me.radiobuttonMultiMonitorOn.UseVisualStyleBackColor = true
        '
        'radiobuttonMultiMonitorOff
        '
        Me.radiobuttonMultiMonitorOff.AutoSize = true
        Me.radiobuttonMultiMonitorOff.Location = New System.Drawing.Point(259, 73)
        Me.radiobuttonMultiMonitorOff.Name = "radiobuttonMultiMonitorOff"
        Me.radiobuttonMultiMonitorOff.Size = New System.Drawing.Size(92, 17)
        Me.radiobuttonMultiMonitorOff.TabIndex = 1
        Me.radiobuttonMultiMonitorOff.TabStop = true
        Me.radiobuttonMultiMonitorOff.Text = "Single-Monitor"
        Me.radiobuttonMultiMonitorOff.UseVisualStyleBackColor = true
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.pictureboxDisplayMode)
        Me.GroupBox1.Controls.Add(Me.radiobuttonMultiMonitorOn)
        Me.GroupBox1.Controls.Add(Me.radiobuttonMultiMonitorOff)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(359, 127)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = false
        Me.GroupBox1.Text = "Display settings"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.radiobuttonNoTree)
        Me.GroupBox2.Controls.Add(Me.radiobuttonDisplayTree)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 157)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(136, 73)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Tree settings"
        '
        'radiobuttonNoTree
        '
        Me.radiobuttonNoTree.AutoSize = true
        Me.radiobuttonNoTree.Location = New System.Drawing.Point(27, 43)
        Me.radiobuttonNoTree.Name = "radiobuttonNoTree"
        Me.radiobuttonNoTree.Size = New System.Drawing.Size(64, 17)
        Me.radiobuttonNoTree.TabIndex = 1
        Me.radiobuttonNoTree.TabStop = true
        Me.radiobuttonNoTree.Text = "No Tree"
        Me.radiobuttonNoTree.UseVisualStyleBackColor = true
        '
        'radiobuttonDisplayTree
        '
        Me.radiobuttonDisplayTree.AutoSize = true
        Me.radiobuttonDisplayTree.Location = New System.Drawing.Point(27, 19)
        Me.radiobuttonDisplayTree.Name = "radiobuttonDisplayTree"
        Me.radiobuttonDisplayTree.Size = New System.Drawing.Size(84, 17)
        Me.radiobuttonDisplayTree.TabIndex = 0
        Me.radiobuttonDisplayTree.TabStop = true
        Me.radiobuttonDisplayTree.Text = "Display Tree"
        Me.radiobuttonDisplayTree.UseVisualStyleBackColor = true
        '
        'buttonOK
        '
        Me.buttonOK.Location = New System.Drawing.Point(215, 200)
        Me.buttonOK.Name = "buttonOK"
        Me.buttonOK.Size = New System.Drawing.Size(75, 23)
        Me.buttonOK.TabIndex = 5
        Me.buttonOK.Text = "OK"
        Me.buttonOK.UseVisualStyleBackColor = true
        '
        'buttonCancel
        '
        Me.buttonCancel.Location = New System.Drawing.Point(296, 200)
        Me.buttonCancel.Name = "buttonCancel"
        Me.buttonCancel.Size = New System.Drawing.Size(75, 23)
        Me.buttonCancel.TabIndex = 6
        Me.buttonCancel.Text = "Cancel"
        Me.buttonCancel.UseVisualStyleBackColor = true
        '
        'pictureboxDisplayMode
        '
        Me.pictureboxDisplayMode.Location = New System.Drawing.Point(13, 22)
        Me.pictureboxDisplayMode.Name = "pictureboxDisplayMode"
        Me.pictureboxDisplayMode.Size = New System.Drawing.Size(235, 94)
        Me.pictureboxDisplayMode.TabIndex = 2
        Me.pictureboxDisplayMode.TabStop = false
        '
        'settingsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(389, 242)
        Me.Controls.Add(Me.buttonCancel)
        Me.Controls.Add(Me.buttonOK)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "settingsForm"
        Me.Text = "settingsForm"
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        CType(Me.pictureboxDisplayMode,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents radiobuttonMultiMonitorOn As System.Windows.Forms.RadioButton
    Friend WithEvents radiobuttonMultiMonitorOff As System.Windows.Forms.RadioButton
    Friend WithEvents pictureboxDisplayMode As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents radiobuttonNoTree As System.Windows.Forms.RadioButton
    Friend WithEvents radiobuttonDisplayTree As System.Windows.Forms.RadioButton
    Friend WithEvents buttonOK As System.Windows.Forms.Button
    Friend WithEvents buttonCancel As System.Windows.Forms.Button
End Class
