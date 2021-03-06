﻿Imports System.Data
Imports GlobalResources
Imports GlobalResources.My.Resources.SharedResources
Imports NFL_Draft
Imports System.IO
Imports System.Linq
Imports AIEvaluation.Draft

Class NFLDraft
    Dim MyDraft As New DraftTickerControl
    Dim tempDT As New DataTable
    Dim TeamDraftInfo(32) As TeamDraftClass
    Dim DraftEval As New DraftAIEval 'Creates a new class to evaluate players and get their grades

    'Dim MyVM As New NFLDraftViewModel
    Sub New(ByVal DT As DataTable)

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        DlgReplacement.Visibility = Visibility.Visible
        DlgReplacement.BringIntoView()
        MyDraft.Visibility = Visibility.Visible
        DataContext = MyDraft
        DisplayDataGrid(DT)
        Dim ColNames As String() = {"Fname", "LName", "College", "CollegePOS", "ActualGrade"}
        Dim query = From DraftPlayers In DT.AsEnumerable()
                    Order By DraftPlayers.Field(Of Decimal)("ActualGrade") Descending
                    Take (25)
                    Select New With
                        {.Player = DraftPlayers.Field(Of String)("FName") + " " + DraftPlayers.Field(Of String)("LName"),
                        .College = DraftPlayers.Field(Of String)("College"),
                        .POS = DraftPlayers.Field(Of String)("CollegePOS"),
                        .Grade = DraftPlayers.Field(Of Decimal)("ActualGrade")}

        tempDT = SQLFunctions.SQLiteDataFunctions.FilterTable(DT, tempDT, "ActualGrade > 7", ColNames, "ActualGrade DESC").AsEnumerable().Take(25).CopyToDataTable()
        LstDraft.ItemsSource = query
        MyDraft.LstGrid.Add(tempDT)
        UpdateDraftGrid()
        AddHandler DraftPick.PickMade, AddressOf UpdateDraftGrid

        For i As Integer = 1 To 32
            TeamDraftInfo(i) = New TeamDraftClass
        Next i

    End Sub

    Private Sub DisplayDataGrid(ByVal DT As DataTable)
        MyDraft.MyDT.Add(DT)
    End Sub
    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        stkTest.Children.Add(MyDraft)
        Await Task.Run(Sub()
                           MyDraft.MyTimer(MyDraft)
                       End Sub)
    End Sub
    ''' <summary>
    ''' Once this button is clicked to close it, the draft loop starts
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TxtDraft_Click(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        DlgReplacement.Visibility = Visibility.Collapsed
        DraftStateLoop()
    End Sub
    ''' <summary>
    ''' Main Draft Loop that runs the draft...picks are stored on a stack which are then popped every time there is a selection made
    ''' </summary>
    Private Async Sub DraftStateLoop()
        Dim DraftPickList As New Queue(Of String)
        Dim TeamOnClock As String
        Dim TeamOnClockInfo As New TeamDraftClass
        For i As Integer = 1 To 7 '7 rounds
            For x As Integer = 1 To 32 '32 teams
                For team As Integer = 0 To TeamDT.Rows.Count - 1
                    If x = CInt(TeamDT.Rows(team).Item("DraftPosition")) Then
                        DraftPickList.Enqueue(TeamDT.Rows(team).Item("TeamName").ToString())
                    End If
                Next team
            Next x
        Next i

        LoadTeamDraftInfo()

        While DraftPickList.Count <> 0 ''While there are still picks left, keep running the draft
            TeamOnClock = DraftPickList.Dequeue() 'Takes the next team off the list

            For Each team In TeamDraftInfo
                If team IsNot Nothing Then
                    If team.TeamName = TeamOnClock Then
                        TeamOnClockInfo = team
                        Await Task.Run(Sub() DeterminePickOutCome(TeamOnClockInfo))
                    End If
                End If
            Next team

        End While
    End Sub
    ''' <summary>
    ''' Determines what the team on the clock does with the pick
    ''' </summary>
    ''' <param name="teamOnClockInfo"></param>
    Private Async Sub DeterminePickOutCome(teamOnClockInfo As TeamDraftClass)
        Await Task.Run(Sub() DetermineOtherTeams(teamOnClockInfo))
    End Sub
    ''' <summary>
    ''' Determines if the other teams are willing to trade up/negotiate with this team for the pick
    ''' </summary>
    ''' <param name="teamonClockInfo"></param>
    Private Sub DetermineOtherTeams(teamOnClockInfo As TeamDraftClass)
        Dim MakeTradeOffer As Boolean
        For i As Integer = 1 To 32
            If i <> teamOnClockInfo.TeamNum Then 'if this isn't the team on the clock
                MakeTradeOffer = EvaluateTrade(MakeTradeOffer)
                If MakeTradeOffer Then 'if it returns true

                End If
            End If
        Next i

    End Sub
    ''' <summary>
    ''' Evaluates whether to make a trade offer to move up the board
    ''' </summary>
    ''' <param name="makeTradeOffer"></param>
    ''' <returns></returns>
    Private Function EvaluateTrade(makeTradeOffer As Boolean) As Boolean

        Return makeTradeOffer
    End Function

    ''' <summary>
    ''' Builds the properties for each team
    ''' </summary>
    Private Sub LoadTeamDraftInfo()
        Dim myList As New List(Of Single)
        Dim str As String = ""
        Using sr As New StreamReader("DraftPickValueChart.txt")
            sr.ReadLine()
            str = sr.ReadLine
            myList = str.Split(";"c).[Select](Function(s) CType(Single.Parse(s), Single)).ToList()
        End Using
        For i As Integer = 1 To 32
            TeamDraftInfo(i).TeamNum = 1
            TeamDraftInfo(i).TeamName = TeamDT.Rows(i - 1).Item("TeamName").ToString()
            TeamDraftInfo(i).Round1Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")))
            TeamDraftInfo(i).Round2Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 32)
            TeamDraftInfo(i).Round3Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 64)
            TeamDraftInfo(i).Round4Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 96)
            TeamDraftInfo(i).Round5Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 128)
            TeamDraftInfo(i).Round6Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 160)
            TeamDraftInfo(i).Round7Picks.Add(CInt(TeamDT.Rows(i - 1).Item("DraftPosition")) + 192)
            TeamDraftInfo(i).GMStats = GetGMStats(i)

        Next i

    End Sub

    Private Function GetGMStats(teamNum As Integer) As DataRow
        Dim d As DataRow

        For i As Integer = 0 To PersonnelDT.Rows.Count - 1
            If CInt(PersonnelDT.Rows(i).Item("PersonnelType")) = 1 And CInt(PersonnelDT.Rows(i).Item("TeamID")) = teamNum Then
                d = PersonnelDT.Rows(i)
                Return d
                Exit Function
            End If
        Next i
    End Function

    ''' <summary>
    ''' Removes the Player from the DraftGrid
    ''' </summary>
    Private Sub UpdateDraftGrid()
        Dim myUI(32) As InlineUIContainer
        For i As Integer = 0 To 32
            myUI(i) = New InlineUIContainer
        Next i

        'Dim Pick1Txt As New TextBlock
        Dim Img1 As New Image
        Dim Img2 As New Image

        Img1.Source = SQLFunctions.SQLiteDataFunctions.BitMapToImage(New System.Drawing.Bitmap(Arizona_Cardinals))
        Img1.Height = 50
        Img1.Width = 67
        Img1.Margin = New Thickness(0, 0, 60, 0)

        Img2.Source = SQLFunctions.SQLiteDataFunctions.BitMapToImage(New System.Drawing.Bitmap(Buffalo_Bills))
        Img2.Height = 50
        Img2.Height = 67
        Img2.Margin = New Thickness(40, 0, 60, 0)

        myUI(0).Child = Img1
        myUI(1).Child = Img2

        MyDraft.TxtDraft.Inlines.Add(myUI(0))
        MyDraft.TxtDraft.Inlines.Add(New Run With {.Text = "Laphonse Ellis DE USC", .Foreground = Brushes.Firebrick, .FontWeight = FontWeights.Bold})
        MyDraft.TxtDraft.Inlines.Add(myUI(1))
        MyDraft.TxtDraft.Inlines.Add(New Run With {.Text = "Jimmy Lakowski QB Michigan", .Foreground = Brushes.Firebrick, .FontWeight = FontWeights.Bold})

    End Sub
    ''' <summary>
    ''' Holds all the draft realated information for one team in one place inside the draft room
    ''' </summary>
    Private Class TeamDraftClass

        Public Property TeamNum As Integer
        Public Property TeamName As String
        Public Property Round1Picks As New List(Of Integer)
        Public Property Round2Picks As New List(Of Integer)
        Public Property Round3Picks As New List(Of Integer)
        Public Property Round4Picks As New List(Of Integer)
        Public Property Round5Picks As New List(Of Integer)
        Public Property Round6Picks As New List(Of Integer)
        Public Property Round7Picks As New List(Of Integer)
        Public Property DraftPhil As String
        Public Property TeamNeeds As String
        Public Property GMStats As DataRow
        Public Property DraftBoard As SortedDictionary(Of Integer, Integer) = New SortedDictionary(Of Integer, Integer) 'Dictionary(PlayerID, Grade)--holds the list of players and their grades

        Public Property DraftPickChart As New List(Of Single)

    End Class
End Class