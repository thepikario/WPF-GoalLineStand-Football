﻿Imports System.Data
Imports GlobalResources
Imports System.Linq

Public Enum PassTypeEnum
    PBehindLOSFarL
    PBehindLOSLMid
    PBehindLOSMid
    PBehindLOSRMid
    PBehindLOSFarR
    PShortFarL
    PShortLMid
    PShortMid
    PShortRMid
    PShortFarR
    PMedFarL
    PMedLMid
    PMedMid
    PMedRMid
    PMedFarR
    PLongFarL
    PLongLMid
    PLongMid
    PLongRMid
    PLongFarR
End Enum
Public Enum RunTypeEnum
    LeftEnd
    LeftTackle
    Middle
    RightTackle
    RightEnd
End Enum
Public Enum ScoringTypeEnum
    RushingTD
    PassingTD
    OffFumRecTD
    DefFumRecTD
    IntReturnTD
    PuntRetTD
    KORetTD
    FG
    Safety
    XP
    TwoPointConv
    DefXPReturnFor2Pts
    FreeKickFG
End Enum
''' <summary>
''' This is the class where game play takes place
''' </summary>
Public Class GamePlay
    Public GameLoop As Boolean 'This is what controls whether the game is still going on or it has ended
    Private PassType As New PassTypeEnum 'Enumeration for the different types of passes
    Private ScoringType As New ScoringTypeEnum 'Determines what type of score it is
#Region "Events"
    Public Event PassCompletion(ByVal QB As Integer, ByVal recPlayer As Integer, ByVal yardsGained As Single)
    Public Event PassDropped(ByVal QB As Integer, ByVal recPlayer As Integer)
    Public Event PassDefended(ByVal defPlayer As Integer, ByVal intendedRec As Integer)
    Public Event Touchback(ByVal kicker As Integer, ByVal homeTeam As Boolean)
    Public Event FirstDown(ByVal player As Integer, ByVal homeTeam As Boolean)
    Public Event TouchDown(ByVal type As ScoringTypeEnum, ByVal homeTeam As Boolean, ByVal player As Integer)
    Public Event Interception(ByVal QBThrowing As Integer, ByVal IntPlayer As Integer, ByVal homeTeam As Boolean)
    Public Event Fumble(ByVal fumblingPlayer As Integer, ByVal recoveringPlayer As Integer, ByVal offenseRecovers As Boolean)
    Public Event Tackle(ByVal ballCarrier As Integer, ByVal tackler As Integer)
    Public Event Sack(ByVal QB As Integer, ByVal defender As Integer, ByVal yardsLost As Integer)
    Public Event Punt(ByVal punter As Integer, ByVal puntYards As Integer)
    Public Event FieldGoal()
    Public Event ChangeOfPoss(ByVal homeTeamHasBall As Boolean) 'Fires Change of Possession Event
    Public Event Timeout(ByVal homeTeamCalled As Boolean)
    Public Event EndOfQuarter()
    Public Event TwoMinuteWarning()
    Public Event HalfTime()

#End Region

#Region "Time Variables"
    Private Property StopClock As Boolean
    Private Property Pace As Integer
    Private Property GameTime As New TimeSpan(0, 15, 0) 'Sets the clock to 15 minutes(0 hours, 15 minutes, 0 seconds)
    Private Property BallSpotTime As Integer
#End Region

#Region "Passing Variables"
    Private Property PBehLOSFarLComp As Single = 64.5
    Private Property PBehLOSLMidComp As Single = 75.9
    Private Property PBehLOSMidComp As Single = 51.3
    Private Property PBehLOSRMidComp As Single = 74.7
    Private Property PBehLOSFarRComp As Single = 64.5
    Private Property PShortFarLComp As Single = 64.8
    Private Property PShortLMidComp As Single = 67.4
    Private Property PShortMidComp As Single = 70.3
    Private Property PShortRMidComp As Single = 67.1
    Private Property PShortFarRComp As Single = 67.6
    Private Property PMedFarLComp As Single = 47
    Private Property PMedLMidComp As Single = 56.7
    Private Property PMedMidComp As Single = 60.9
    Private Property PMedRMidComp As Single = 55
    Private Property PMedFarRComp As Single = 46.9
    Private Property PLongFarLComp As Single = 27.9
    Private Property PLongLMidComp As Single = 36.8
    Private Property PLongMidComp As Single = 38
    Private Property PLongRMidComp As Single = 36.1
    Private Property PLongFarRComp As Single = 30.6
#End Region

#Region "Running Variables"
    Private Property RunMid As Single
    Private Property RunLeft As Single
    Private Property RunLeftEnd As Single
    Private Property RunRight As Single
    Private Property RunRightEnd As Single
#End Region

#Region "Kicking Game"
    Private Property Kickoff As Boolean = True 'Initializes the game to start with a kickoff
    Private Property KickoffDist As Single
    Private Property KickReturnYards As Single
    Private Property PuntReturnYards As Single
    Private Property CallFairCatch As Boolean
    Private Property PuntDistance As Single
    Private Property FGDistance As Single
    Private Property ExpDecayFG As Single
#End Region

#Region "Basic Game Info"
    Private Property Down As Integer
    Private Property YardsToGo As Single
    Private Property YardLine As Single = 35 'YardLine will be from 0(Your GoalLine) to 100(Opp GoalLine)
    Private Property Quarter As Integer = 1
    Private Property HomePossession As Boolean 'Does Home Team Have the Ball?
#End Region

#Region "Turnovers"
    'Private Property Intercepted As Boolean
    Private Property IntReturnYds As Boolean
    Private Property IntReturnTD As Boolean
    'Private Property Fumble As Boolean
    Private Property DefFumRec As Boolean
    Private Property FumbleRetYds As Boolean
    Private Property FumbleRetTD As Boolean

#End Region

#Region "Teams"
    Private Home As New HomeTeam 'Filters the players DataTable to get the appropriate team's players
    Private Away As New AwayTeam 'Filters the players DataTable to get the appropriate team's players
#End Region
    ''' <summary>
    ''' Sets the depth chart by PID for lookups to the DataView
    ''' </summary>
    Private Structure HomeTeam
        Dim QB1 As DataRow
        Dim QB2 As DataRow
        Dim QB3 As DataRow
        Dim RB1 As DataRow
        Dim RB2 As DataRow

    End Structure
    Private Structure AwayTeam

    End Structure

    Sub New(ByVal homeTeamId As Integer, ByVal awayTeamId As Integer)
        StartGame(homeTeamId, awayTeamId)
    End Sub

    ''' <summary>
    ''' Start the game and pass in the 2 teams who are playing----home and away
    ''' </summary>
    ''' <param name="homeTeamId"></param>
    ''' <param name="awayTeamId"></param>
    Public Sub StartGame(ByVal homeTeamId As Integer, ByVal awayTeamId As Integer)

        Dim MyRand As New Mersenne.MersenneTwister
        HomePossession = MyRand.GenerateInt32(0, 1) = 1 'Determines who is kicking off
        'LoadDepthCharts(HomePossession, homeTeamId, awayTeamId) 'This will populate the onfield positions by which team is on the field
        While GameLoop
            ' While the GameLoop is Set to True run the game.
            If Kickoff Then 'We are kicking off
                If MyRand.GenerateInt32(0, 100) < 40 Then '39% of kicks are returned, otherwise touchback

                Else

                End If
            End If

        End While

    End Sub

    Private Sub LoadDepthCharts(homeTeamHasBall As Boolean, ByVal homeTeamId As Integer, ByVal awayTeamId As Integer)
        If homeTeamHasBall Then 'The home team is in possession, load the home team depth charts
            'Home.QB1 = TeamDT.Rows.Find(PlayerDT.AsEnumerable().GroupBy(Sub(x)
            'x.Item("TeamId") = homeTeamId.ToString() And x.Item("DepthPos") Is "QB1"
            'End Sub)
        End If
    End Sub

    Private Function GetPassType() As PassTypeEnum
        Dim MyRand As New Mersenne.MersenneTwister
        Dim PassType As String
        Select Case MyRand.GenerateDouble(0, 100) 'Generate a new Random number
            Case <= 4.0
                PassType = PassTypeEnum.PBehindLOSFarL
            Case <= 8.0
                PassType = PassTypeEnum.PBehindLOSLMid
            Case <= 9.3
                PassType = PassTypeEnum.PBehindLOSLMid
            Case <= 13.7
                PassType = PassTypeEnum.PBehindLOSRMid
            Case <= 18.9
                PassType = PassTypeEnum.PBehindLOSFarR
            Case <= 29
                PassType = PassTypeEnum.PShortFarL
            Case <= 38.6
                PassType = PassTypeEnum.PShortLMid
            Case <= 46
                PassType = PassTypeEnum.PShortMid
            Case <= 57.2
                PassType = PassTypeEnum.PShortRMid
            Case <= 68.2
                PassType = PassTypeEnum.PShortFarR
            Case <= 73.1
                PassType = PassTypeEnum.PMedFarL
            Case <= 76.8
                PassType = PassTypeEnum.PMedLMid
            Case <= 79.9
                PassType = PassTypeEnum.PMedMid
            Case <= 83.8
                PassType = PassTypeEnum.PMedRMid
            Case <= 88.9
                PassType = PassTypeEnum.PMedFarR
            Case <= 92.5
                PassType = PassTypeEnum.PLongFarL
            Case <= 93.5
                PassType = PassTypeEnum.PLongLMid
            Case <= 94.5
                PassType = PassTypeEnum.PLongMid
            Case <= 95.8
                PassType = PassTypeEnum.PLongRMid
            Case Else
                PassType = PassTypeEnum.PLongFarR
        End Select
        Return PassType
    End Function

    Private Function GetPassCompletion() As Boolean
        Dim MyRand As New Mersenne.MersenneTwister
        Dim IsComplete As Boolean 'Returns TRUE if it is Below this number, false otherwise
        Select Case PassType
            Case PassTypeEnum.PBehindLOSFarL
                IsComplete = MyRand.GenerateDouble(0, 100) <= 64.5
            Case PassTypeEnum.PBehindLOSLMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 75.9
            Case PassTypeEnum.PBehindLOSMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 51.3
            Case PassTypeEnum.PBehindLOSRMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 74.7
            Case PassTypeEnum.PBehindLOSFarR
                IsComplete = MyRand.GenerateDouble(0, 100) <= 64.5
            Case PassTypeEnum.PShortFarL
                IsComplete = MyRand.GenerateDouble(0, 100) <= 64.8
            Case PassTypeEnum.PShortLMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 67.4
            Case PassTypeEnum.PShortMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 70.3
            Case PassTypeEnum.PShortRMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 67.1
            Case PassTypeEnum.PShortFarR
                IsComplete = MyRand.GenerateDouble(0, 100) <= 67.6
            Case PassTypeEnum.PMedFarL
                IsComplete = MyRand.GenerateDouble(0, 100) <= 47.0
            Case PassTypeEnum.PMedLMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 56.7
            Case PassTypeEnum.PMedMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 60.9
            Case PassTypeEnum.PMedRMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 55.0
            Case PassTypeEnum.PMedFarR
                IsComplete = MyRand.GenerateDouble(0, 100) <= 46.9
            Case PassTypeEnum.PLongFarL
                IsComplete = MyRand.GenerateDouble(0, 100) <= 27.9
            Case PassTypeEnum.PLongLMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 36.8
            Case PassTypeEnum.PLongMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 38.0
            Case PassTypeEnum.PLongRMid
                IsComplete = MyRand.GenerateDouble(0, 100) <= 36.1
            Case PassTypeEnum.PLongFarR
                IsComplete = MyRand.GenerateDouble(0, 100) <= 30.6
        End Select

        Return IsComplete
    End Function

    Private Function GetPassYards(ByVal pass As PassTypeEnum) As Single
        Dim MyRand As New Mersenne.MersenneTwister
        Dim GenPassYards As New Mersenne.MersenneTwister
        Dim PassYards As Single
        Select Case pass
            'Pass is caught behind the LOS
            Case PassTypeEnum.PBehindLOSFarL, PassTypeEnum.PBehindLOSFarR, PassTypeEnum.PBehindLOSLMid, PassTypeEnum.PBehindLOSMid, PassTypeEnum.PBehindLOSRMid
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 12.22
                        PassYards = GenPassYards.GenerateDouble(-4, -0.1)
                    Case 12.23 To 60
                        PassYards = GenPassYards.GenerateDouble(0, 4.9)
                    Case 60.01 To 85
                        PassYards = GenPassYards.GenerateDouble(5, 9.9)
                    Case 85.01 To 92
                        PassYards = GenPassYards.GenerateDouble(10, 14.9)
                    Case 92.01 To 95.5
                        PassYards = GenPassYards.GenerateDouble(15, 19.9)
                    Case 95.51 To 97
                        PassYards = GenPassYards.GenerateDouble(20, 24.9)
                    Case 97.01 To 98
                        PassYards = GenPassYards.GenerateDouble(25, 29.9)
                    Case 98.01 To 98.5
                        PassYards = GenPassYards.GenerateDouble(30, 34.9)
                    Case 98.51 To 99
                        PassYards = GenPassYards.GenerateDouble(35, 39.9)
                    Case 99.01 To 99.25
                        PassYards = GenPassYards.GenerateDouble(40, 50)
                    Case 99.26 To 99.5
                        PassYards = GenPassYards.GenerateDouble(50.1, 60)
                    Case 99.51 To 99.75
                        PassYards = GenPassYards.GenerateDouble(60.1, 70)
                    Case 99.76 To 99.9
                        PassYards = GenPassYards.GenerateDouble(70.1, 80)
                    Case Else
                        PassYards = GenPassYards.GenerateDouble(80.1, 100)
                End Select
                'Pass is caught 0-10 yards downfield
            Case PassTypeEnum.PShortFarL, PassTypeEnum.PShortFarR, PassTypeEnum.PShortLMid, PassTypeEnum.PShortLMid, PassTypeEnum.PShortMid, PassTypeEnum.PShortRMid
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 30
                        PassYards = GenPassYards.GenerateDouble(0, 4.9)
                    Case 30.01 To 71.6
                        PassYards = GenPassYards.GenerateDouble(5, 9.9)
                    Case 71.61 To 83
                        PassYards = GenPassYards.GenerateDouble(10, 14.9)
                    Case 83.01 To 90
                        PassYards = GenPassYards.GenerateDouble(15, 19.9)
                    Case 90.01 To 94
                        PassYards = GenPassYards.GenerateDouble(20, 24.9)
                    Case 94.01 To 96
                        PassYards = GenPassYards.GenerateDouble(25, 29.9)
                    Case 96.01 To 97
                        PassYards = GenPassYards.GenerateDouble(30, 34.9)
                    Case 97.01 To 97.5
                        PassYards = GenPassYards.GenerateDouble(35, 39.9)
                    Case 97.51 To 98
                        PassYards = GenPassYards.GenerateDouble(40, 50)
                    Case 98.01 To 98.75
                        PassYards = GenPassYards.GenerateDouble(50.1, 60)
                    Case 98.76 To 99.5
                        PassYards = GenPassYards.GenerateDouble(60.1, 70)
                    Case 99.51 To 99.75
                        PassYards = GenPassYards.GenerateDouble(70.1, 80)
                    Case Else
                        PassYards = GenPassYards.GenerateDouble(80.1, 100)
                End Select
                'Medium Pass 10-20 yards
            Case PassTypeEnum.PMedFarL, PassTypeEnum.PMedFarR, PassTypeEnum.PMedLMid, PassTypeEnum.PMedRMid, PassTypeEnum.PMedMid
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 30
                        PassYards = GenPassYards.GenerateDouble(10, 14.9)
                    Case 30.01 To 53.7
                        PassYards = GenPassYards.GenerateDouble(15.0, 19.9)
                    Case 53.71 To 75
                        PassYards = GenPassYards.GenerateDouble(20, 24.9)
                    Case 75.01 To 85
                        PassYards = GenPassYards.GenerateDouble(25, 29.9)
                    Case 85.01 To 92
                        PassYards = GenPassYards.GenerateDouble(30, 34.9)
                    Case 92.01 To 95
                        PassYards = GenPassYards.GenerateDouble(35, 40)
                    Case 95.01 To 97
                        PassYards = GenPassYards.GenerateDouble(40.1, 50)
                    Case 97.01 To 98
                        PassYards = GenPassYards.GenerateDouble(50.01, 60)
                    Case 98.01 To 99
                        PassYards = GenPassYards.GenerateDouble(60.01, 70)
                    Case 99.01 To 99.5
                        PassYards = GenPassYards.GenerateDouble(70.1, 80)
                    Case Else
                        PassYards = GenPassYards.GenerateDouble(80.1, 100)
                End Select
                'Long passes---20+ yards
            Case Else
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 40
                        PassYards = GenPassYards.GenerateDouble(20, 24.9)
                    Case 40.01 To 65
                        PassYards = GenPassYards.GenerateDouble(25, 29.9)
                    Case 65.01 To 83
                        PassYards = GenPassYards.GenerateDouble(30, 34.9)
                    Case 83.01 To 94
                        PassYards = GenPassYards.GenerateDouble(35, 39.9)
                    Case 94.01 To 97
                        PassYards = GenPassYards.GenerateDouble(40, 50)
                    Case 97.01 To 98
                        PassYards = GenPassYards.GenerateDouble(50.1, 60)
                    Case 98.01 To 98.75
                        PassYards = GenPassYards.GenerateDouble(60.1, 70)
                    Case 98.76 To 99.5
                        PassYards = GenPassYards.GenerateDouble(70.1, 80)
                    Case Else
                        PassYards = GenPassYards.GenerateDouble(80.1, 100)
                End Select
        End Select
        Return PassYards
    End Function

    Private Function GetRunType() As RunTypeEnum
        Dim MyRand As New Mersenne.MersenneTwister
        Dim Run As New RunTypeEnum
        Select Case MyRand.GenerateInt32(0, 100)
            Case 0 To 10
                Run = RunTypeEnum.LeftEnd
            Case 11 To 22
                Run = RunTypeEnum.LeftTackle
            Case 23 To 78
                Run = RunTypeEnum.Middle
            Case 79 To 90
                Run = RunTypeEnum.RightTackle
            Case Else
                Run = RunTypeEnum.RightEnd
        End Select
        Return Run
    End Function
    ''' <summary>
    ''' Gets the run distance by type and the place on the field
    ''' </summary>
    ''' <param name="run"></param>
    ''' <param name="yardLine"></param>
    ''' <returns></returns>
    Private Function GetRunYards(ByVal run As RunTypeEnum, ByVal yardLine As Integer) As Integer
        Dim MyRand As New Mersenne.MersenneTwister
        Dim GetYards As New Mersenne.MersenneTwister
        Dim Yards As Integer
        Select Case yardLine
            Case < 11 ' Ball is on your own 10 or inside your own 10 yard line
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 0.7
                        Yards = -5
                    Case 0.71 To 1.05
                        Yards = -4
                    Case 1.06 To 1.76
                        Yards = -3
                    Case 1.77 To 3.89
                        Yards = -2
                    Case 3.9 To 7.44
                        Yards = -1
                    Case 7.45 To 16.31
                        Yards = 0
                    Case 16.32 To 28.19
                        Yards = 1
                    Case 28.2 To 41.31
                        Yards = 2
                    Case 41.32 To 53.37
                        Yards = 3
                    Case 53.38 To 66.14
                        Yards = 4
                    Case 66.15 To 75.89
                        Yards = 5
                    Case 75.9 To 81.74
                        Yards = 6
                    Case 81.75 To 86.35
                        Yards = 7
                    Case 86.36 To 88.65
                        Yards = 8
                    Case 88.66 To 91.31
                        Yards = 9
                    Case 91.32 To 92.02
                        Yards = 10
                    Case 92.03 To 93.62
                        Yards = 11
                    Case 93.63 To 94.68
                        Yards = 12
                    Case 94.69 To 95.39
                        Yards = 13
                    Case 95.4 To 95.92
                        Yards = 14
                    Case 95.93 To 96.27
                        Yards = 15
                    Case 96.28 To 96.45
                        Yards = 16
                    Case 96.46 To 96.98
                        Yards = 17
                    Case 96.99 To 97.51
                        Yards = 18
                    Case 97.52 To 97.86
                        Yards = 19
                    Case 97.87 To 98.21
                        Yards = 20
                    Case 98.22 To 98.56
                        Yards = GetYards.GenerateInt32(21, 30)
                    Case 98.57 To 99.09
                        Yards = GetYards.GenerateInt32(31, 40)
                    Case 99.1 To 99.44
                        Yards = GetYards.GenerateInt32(41, 50)
                    Case 99.45 To 99.54
                        Yards = GetYards.GenerateInt32(51, 60)
                    Case 99.55 To 99.64
                        Yards = GetYards.GenerateInt32(61, 70)
                    Case 99.65 To 99.74
                        Yards = GetYards.GenerateInt32(71, 80)
                    Case Else
                        Yards = GetYards.GenerateInt32(81, 100)
                End Select

            Case 11 To 89 'Ball is Between your 11 yard line and the opponent's 11 yard line.
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 0.51
                        Yards = GetYards.GenerateInt32(-9, -6)
                    Case 0.52 To 0.91
                        Yards = -5
                    Case 0.92 To 1.77
                        Yards = -4
                    Case 1.78 To 3.2
                        Yards = -3
                    Case 3.21 To 5.77
                        Yards = -2
                    Case 5.78 To 9.86
                        Yards = -1
                    Case 9.87 To 18.85
                        Yards = 0
                    Case 18.86 To 29.06
                        Yards = 1
                    Case 29.07 To 40.91
                        Yards = 2
                    Case 40.92 To 53.44
                        Yards = 3
                    Case 53.45 To 63.46
                        Yards = 4
                    Case 63.47 To 71.19
                        Yards = 5
                    Case 71.2 To 76.74
                        Yards = 6
                    Case 76.75 To 80.88
                        Yards = 7
                    Case 80.89 To 84.18
                        Yards = 8
                    Case 84.19 To 87.37
                        Yards = 9
                    Case 87.38 To 88.84
                        Yards = 10
                    Case 88.85 To 90.84
                        Yards = 11
                    Case 90.85 To 92.1
                        Yards = 12
                    Case 92.11 To 93.45
                        Yards = 13
                    Case 93.46 To 94.43
                        Yards = 14
                    Case 94.44 To 95.34
                        Yards = 15
                    Case 95.35 To 95.88
                        Yards = 16
                    Case 95.89 To 96.39
                        Yards = 17
                    Case 96.4 To 96.67
                        Yards = 18
                    Case 96.68 To 97.13
                        Yards = 19
                    Case 97.14 To 97.56
                        Yards = 20
                    Case 97.57 To 97.84
                        Yards = 21
                    Case 97.85 To 98.06
                        Yards = 22
                    Case 98.07 To 98.23
                        Yards = 23
                    Case 98.24 To 98.38
                        Yards = 24
                    Case 98.39 To 98.52
                        Yards = 25
                    Case 98.53 To 98.61
                        Yards = 26
                    Case 98.62 To 98.72
                        Yards = 27
                    Case 98.73 To 98.85
                        Yards = 28
                    Case 98.86 To 98.93
                        Yards = 29
                    Case 98.94 To 98.98
                        Yards = 30
                    Case 98.99 To 99.09
                        Yards = 31
                    Case 99.1 To 99.16
                        Yards = 32
                    Case 99.17 To 99.19
                        Yards = 33
                    Case 99.2 To 99.24
                        Yards = 34
                    Case 99.25 To 99.28
                        Yards = 35
                    Case 99.29 To 99.33
                        Yards = 36
                    Case 99.34 To 99.37
                        Yards = 37
                    Case 99.38 To 99.39
                        Yards = 38
                    Case 99.4 To 99.42
                        Yards = 39
                    Case 99.43 To 99.48
                        Yards = 40
                    Case 99.49 To 99.72
                        Yards = GetYards.GenerateInt32(41, 50)
                    Case 99.73 To 99.82
                        Yards = GetYards.GenerateInt32(51, 60)
                    Case 99.83 To 99.9
                        Yards = GetYards.GenerateInt32(61, 70)
                    Case 99.91 To 99.96
                        Yards = GetYards.GenerateInt32(71, 80)
                    Case Else
                        Yards = GetYards.GenerateInt32(81, 100)
                End Select

            Case > 89 'Ball is on or inside the Opponents 10 yard line
                Select Case MyRand.GenerateDouble(0, 100)
                    Case 0 To 0.38
                        Yards = GetYards.GenerateInt32(-8, -6)
                    Case 0.39 To 0.97
                        Yards = -5
                    Case 0.98 To 2.15
                        Yards = -4
                    Case 2.16 To 4.1
                        Yards = -3
                    Case 4.11 To 6.45
                        Yards = -2
                    Case 6.46 To 12.81
                        Yards = -1
                    Case 12.82 To 26.31
                        Yards = 0
                    Case 26.32 To 52.83
                        Yards = 1
                    Case 52.84 To 67.7
                        Yards = 2
                    Case 67.71 To 78.38
                        Yards = 3
                    Case 78.39 To 85.02
                        Yards = 4
                    Case 85.03 To 90.5
                        Yards = 5
                    Case 90.51 To 93.83
                        Yards = 6
                    Case 93.84 To 96.96
                        Yards = 7
                    Case 96.97 To 98.62
                        Yards = 8
                    Case 98.63 To 99.21
                        Yards = 9
                    Case Else
                        Yards = 10
                End Select
        End Select

        Return Yards
    End Function

End Class