Imports System.ComponentModel
Imports System.Data.OleDb

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        arrancar()
    End Sub


    Sub arrancar()

        enlace()
        'Label1.Text = estado
        conexion.Close()

        cargar_Categories()
        cargar_Locations()
        leer_Log(DataGridView1)
        leer_Printers(DataGridView2)
        DataGridView1.Sort(DataGridView1.Columns(0), ListSortDirection.Descending)

        Dim DGV As DataGridView
        DGV = DataGridView2
        Dim ufil As Integer
        ufil = DGV.Rows.Count - 1

        For i = 0 To ufil
            ListBox1.Items.Add(DGV.Rows(i).Cells(1).Value)
        Next
    End Sub

    Sub leer_Log(DGV As DataGridView)
        Try

            comando = New OleDb.OleDbCommand("SELECT * FROM Log", conexion)

            Dim lector As OleDbDataReader
            Dim tabla As New DataTable()

            conexion.Open()
            lector = comando.ExecuteReader()
            tabla.Load(lector)
            DGV.DataSource = tabla
            conexion.Close()

            With DGV
                .Columns(0).Visible = False
                .Columns(3).Width = 150
                .Columns(4).Width = 250
                .EditMode = False
                .RowHeadersVisible = False
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToResizeRows = False
                .AllowUserToResizeColumns = False
                .AllowDrop = False
            End With

        Catch ex As Exception
            conexion.Close()
            MsgBox("Something went wrong. Data rejected! <Read>")
        End Try
    End Sub

    Sub leer_Printers(DGV As DataGridView)
        Try

            comando = New OleDb.OleDbCommand("SELECT * FROM Printers", conexion)

            Dim lector As OleDbDataReader
            Dim tabla As New DataTable()

            conexion.Open()
            lector = comando.ExecuteReader()
            tabla.Load(lector)
            DGV.DataSource = tabla
            conexion.Close()

            With DGV
                .Columns(0).Visible = False
                .RowHeadersVisible = False
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToResizeRows = False
                .AllowUserToResizeColumns = False
                .AllowUserToOrderColumns = False
                .AllowDrop = False
            End With

        Catch ex As Exception
            conexion.Close()
            MsgBox("Something went wrong. Data rejected! <Read>")
        End Try
    End Sub

    Sub cargar_Categories()
        With ComboBox1.Items
            .Add("Printer removed or moved")
            .Add("Printer component changed")
            .Add("Printer setting changed")
        End With
    End Sub

    Sub cargar_Locations()

        With ComboBox2.Items
            .Add("SP1")
            .Add("SP2")
            .Add("SP3")
            .Add("SP4")
            .Add("SP5")
            .Add("SP6")
            .Add("BENCH")
            .Add("SHELF")
            .Add("1011")
            .Add("1012")
            .Add("1021")
            .Add("1022")
            .Add("1031")
            .Add("1032")
            .Add("1041")
            .Add("1042")
            .Add("1051")
            .Add("1052")
        End With

    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        conexion.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Try
            If ComboBox1.Text = "" Or ComboBox1.Text = "" Or TextBox1.Text = "" Then
                MsgBox("Please select a Category, Location and write a comment for your report.")
                GoTo R100
            End If

            enlace()

            Dim Printer As String
            Dim Location As String
            Dim Category As String
            Dim Report_Date As Date
            Dim Tech As String

            Printer = DataGridView2.CurrentCell.Value
            Location = ComboBox2.Text
            Category = ComboBox1.Text
            Report_Date = Today()
            Tech = Environment.UserName

            comando = New OleDb.OleDbCommand("INSERT INTO Log(Printer, Location, Category, Comment, Report_Date, Tech)" & Chr(13) _
                                             & "VALUES(Printer, Location, Category, Comment, Report_Date, User)", conexion)

            comando.Parameters.AddWithValue("@Printer", Printer)
            comando.Parameters.AddWithValue("@Location", Location)
            comando.Parameters.AddWithValue("@Category", Category)
            comando.Parameters.AddWithValue("@Comment", TextBox1.Text)
            comando.Parameters.AddWithValue("@Report_Date", Report_Date)
            comando.Parameters.AddWithValue("@Tech", Tech)

            comando.ExecuteNonQuery()

            MsgBox("Data inserted by " & Tech)
R100:
            conexion.Close()

            leer_Log(DataGridView1)
            DataGridView1.Sort(DataGridView1.Columns(0), ListSortDirection.Descending)


        Catch ex As Exception
            conexion.Close()
            MsgBox("Something went wrong. Data rejected!")
        End Try

    End Sub


    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        TextBox1.Text = ""
        Dim p As String
        p = DataGridView2.CurrentCell.Value
        If ComboBox1.Text = "Printer removed or moved" Then
            TextBox1.Text = ComboBox1.Text & " to " & ComboBox2.Text
            ComboBox2.Text = TextBox3.Text
            ComboBox2.Enabled = True
        End If

        If ComboBox1.Text = "Printer component changed" Then
            TextBox1.Text = ComboBox1.Text
            ComboBox2.Text = TextBox3.Text
            ComboBox2.Enabled = False
        End If

        If ComboBox1.Text = "Printer setting changed" Then
            TextBox1.Text = ComboBox1.Text
            ComboBox2.Text = TextBox3.Text
            ComboBox2.Enabled = False
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        TextBox1.Text = ""
        Dim p As String = DataGridView2.CurrentCell.Value
        If ComboBox1.Text = "Printer removed or moved" Then
            TextBox1.Text = ComboBox1.Text & " to " & ComboBox2.Text
        End If
    End Sub

    Sub seleccion_printer()

        'Dim p As String = DataGridView2.CurrentCell.Value
        Dim p As String = ListBox1.SelectedItem

        GroupBox6.Text = "Last report for printer " & p
        GroupBox7.Text = "Current location for printer " & p

        Dim DGV As DataGridView = DataGridView1
        Dim ufil As Integer = DGV.Rows.Count - 1

        For i = 0 To ufil
            If DGV.Rows(i).Cells(1).Value = p Then
                TextBox2.Text = DGV.Rows(i).Cells(4).Value
                TextBox3.Text = DGV.Rows(i).Cells(2).Value

                GoTo R100
            End If
        Next
R100:

        Button2.Enabled = True
    End Sub


    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        seleccion_printer()
    End Sub


End Class
