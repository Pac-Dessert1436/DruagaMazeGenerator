Option Strict On
Option Infer On
Imports System.Runtime.CompilerServices

Public Module DruagaMaze
    Private Const MAZE_WIDTH As Integer = 18, MAZE_HEIGHT As Integer = 9

    Public ReadOnly Property DruagaMazeLayout(seed As Integer) As Integer(,)
        Get
            If seed < -128 OrElse seed > 255 Then Throw New ArgumentException(
                "Seed value out of range; must be within [-128, 255].")
            
            Return Generate(CByte(seed)).BuildLayout()
        End Get
    End Property

    Private Enum WallDirection As Byte
        Up = 0
        Right = 1
        Down = 2
        Left = 3
    End Enum

    Friend Function Generate(seed As Byte) As Byte(,)
        Dim mapData(MAZE_WIDTH - 1, MAZE_HEIGHT - 1) As Byte
        Dim currentSeed As Byte = seed
        Dim prev As Byte = CByte(seed And 1)

        For x As Integer = MAZE_WIDTH - 2 To 0 Step -1
            For y As Integer = 0 To MAZE_HEIGHT - 2
                Dim currX As Integer = x, currY As Integer = y

                If CheckAndMarkPole(mapData, currX, currY) Then Continue For

                Dim wDir As WallDirection
                Do
                    wDir = NextDirection(currentSeed, prev)
                Loop Until MakeWall(mapData, currX, currY, wDir)
            Next y
        Next x
        Return mapData
    End Function

    Private Function CheckAndMarkPole(mapData As Byte(,), x As Integer, y As Integer) As Boolean
        If x < 0 OrElse x >= MAZE_WIDTH - 1 OrElse y < 0 OrElse y >= MAZE_HEIGHT - 1 Then
            Return True
        End If

        Return (mapData(y, x) And 1) <> 0
    End Function

    Private Function MakeWall(mapData As Byte(,), ByRef x As Integer, ByRef y As Integer,
                              wDir As WallDirection) As Boolean
        mapData(y, x) = CByte(mapData(y, x) Or 1)

        If CheckWall(mapData, x, y, wDir) Then Return False

        Select Case wDir
            Case WallDirection.Up
                mapData(y, x) = CByte(mapData(y, x) Or 2)
                y -= 1
            Case WallDirection.Right
                If x < MAZE_WIDTH - 1 Then mapData(y, x + 1) = CByte(mapData(y, x + 1) Or 4)
                x += 1
            Case WallDirection.Down
                If y < MAZE_HEIGHT - 1 Then mapData(y + 1, x) = CByte(mapData(y + 1, x) Or 2)
                y += 1
            Case WallDirection.Left
                mapData(y, x) = CByte(mapData(y, x) Or 4)
                x -= 1
        End Select
        Return CheckAndMarkPole(mapData, x, y)
    End Function

    Private Function CheckWall(mapData As Byte(,), x As Integer, y As Integer,
                               wDir As WallDirection) As Boolean
        Select Case wDir
            Case WallDirection.Up
                Return (mapData(y, x) And 2) <> 0
            Case WallDirection.Right
                If x >= MAZE_WIDTH - 1 Then Return True
                Return (mapData(y, x + 1) And 4) <> 0
            Case WallDirection.Down
                If y >= MAZE_HEIGHT - 1 Then Return True
                Return (mapData(y + 1, x) And 2) <> 0
            Case WallDirection.Left
                Return (mapData(y, x) And 4) <> 0
            Case Else
                Return True
        End Select
    End Function

    Private Function NextDirection(ByRef seed As Byte, ByRef prev As Byte) As WallDirection
        Dim r1 As Byte = CByte((seed >> 7 Xor seed >> 4) And &H1)
        Dim r2 As Byte = CByte(r1 Xor 1)
        seed = seed << 1 Or r2
        Dim result As Byte = CByte((prev << 1 Or r2) And &H3)
        prev = r2
        Return CType(result, WallDirection)
    End Function

    <Extension> Friend Function BuildLayout(mapData As Byte(,)) As Integer(,)
        Dim rows = (MAZE_HEIGHT + 1) * 2
        Dim cols = (MAZE_WIDTH + 1) * 2
        Dim result(rows - 1, cols - 1) As Integer

        ' Define the 2x2 blocks for each Unicode character
        Dim blocks As New List(Of Integer(,)) From {
            New Integer(,) {{0, 0}, {0, 1}},  ' Block 0: "▗"
            New Integer(,) {{0, 1}, {0, 1}},  ' Block 1: "▐"
            New Integer(,) {{0, 0}, {1, 1}},  ' Block 2: "▄"
            New Integer(,) {{0, 1}, {1, 1}},  ' Block 3: "▟"
            New Integer(,) {{0, 0}, {0, 0}}   ' Block 4: Empty
        }

        ' Build top border (row 0 in character grid)
        For x As Integer = 0 To MAZE_WIDTH
            Dim block = If(x = 0, blocks(0), blocks(2))
            For r As Integer = 0 To 1
                For c As Integer = 0 To 1
                    result(r, x * 2 + c) = block(r, c)
                Next c
            Next r
        Next x

        ' Build maze rows (character grid rows 1 to MAZE_HEIGHT)
        For y As Integer = 0 To MAZE_HEIGHT - 1
            Dim rowStart = (y + 1) * 2  ' Start row in integer array

            ' Left border block ("▐")
            For r As Integer = 0 To 1
                For c As Integer = 0 To 1
                    result(rowStart + r, c) = blocks(1)(r, c)
                Next c
            Next r

            ' Maze cells
            For x As Integer = 0 To MAZE_WIDTH - 1
                Dim currVal = mapData(y, x) >> 1
                ' Force bottom wall
                If y = MAZE_HEIGHT - 1 Then currVal = CByte(currVal Or 2)
                ' Force right wall
                If x = MAZE_WIDTH - 1 Then currVal = CByte(currVal Or 1)

                Dim blockIndex = currVal And 3
                Dim block = If(blockIndex < 4, blocks(blockIndex), blocks(4))
                Dim colStart = (x + 1) * 2

                For r As Integer = 0 To 1
                    For c As Integer = 0 To 1
                        result(rowStart + r, colStart + c) = block(r, c)
                    Next c
                Next r
            Next x
        Next y
        Return result
    End Function

    Public Function Demonstrate(seed As Integer)
       Dim layout = DruagaMazeLayout(seed)
       Dim floor = (CByte(seed) + 60) Mod 60 + 1
       Console.WriteLine($"FLOOR {floor} (seed={CSByte(seed)})")
       For y As Integer = 0 To UBoard(layout, 1)
           For x As Integer = 0 To UBoard(layout, 2)
               Console.Write(If(layout(y, x) = 1, "@"c, " "c))
           Next x
           Console.WriteLine()
       Next y
    End Sub
End Module
