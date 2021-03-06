﻿Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Data
Imports WPFFootball.My.Resources

''' <summary>
'''     Subs and Functions that are used by more than one window.
''' </summary>
Public Class NewGameViewModel
    Implements INotifyPropertyChanged

    Public TeamEnumList As New Teams

    Public Sub New()

        MyBackgroundImg = New BitmapImage(New Uri(GetBackgroundFilePath,
                                                  UriKind.RelativeOrAbsolute))
    End Sub

#Region "INotifyPropertyChanged"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

#End Region

#Region "Private Variables"
    Private _myhelmet As ImageSource
    Private _primcolor As Brush
    Private _seccolor As Brush
    Private _trimcolor As Brush
    Private _mystadiumname As String
    Private _mystadiumcapacity As String
    Private _mycitystate As String
    Private _mystadiumpic As ImageSource
    Private _myavgattendance As String
    Private _myteamrecord As String
    Private _mybackgroundimg As ImageSource
    Private _MyDT As New ObservableCollection(Of DataTable)

#End Region

#Region "Public Properties"

    Public Property MyStadiumName As String
        Get
            Return _mystadiumname
        End Get
        Set
            _mystadiumname = Value
            OnPropertyChanged("MyStadiumName")
        End Set
    End Property

    Public Property MyPrimColor As Brush
        Get
            Return _primcolor
        End Get
        Set
            _primcolor = Value
            OnPropertyChanged("MyPrimColor")
        End Set
    End Property

    Public Property MyTrimColor As Brush
        Get
            Return _trimcolor
        End Get
        Set
            _trimcolor = Value
            OnPropertyChanged("MyTrimColor")
        End Set
    End Property

    Public Property MySecColor As Brush
        Get
            Return _seccolor
        End Get
        Set
            _seccolor = Value
            OnPropertyChanged("MySecColor")
        End Set
    End Property

    Public Property MyDTProperty As ObservableCollection(Of DataTable)
        Get
            Return _MyDT
        End Get
        Set
            _MyDT = Value
            OnPropertyChanged("MyDTProperty")
        End Set
    End Property

    Public Property MyStadiumCapacity As String
        Get

            Return _mystadiumcapacity
        End Get
        Set
            _mystadiumcapacity = Value
            OnPropertyChanged("MyStadiumCapacity")
        End Set
    End Property

    Public Property MyCityState As String
        Get
            Return _mycitystate
        End Get
        Set
            _mycitystate = Value
            OnPropertyChanged("MyCityState")
        End Set
    End Property

    Public Property MyStadiumPic As ImageSource
        Get
            Return _mystadiumpic
        End Get
        Set
            _mystadiumpic = Value
            OnPropertyChanged("MyStadiumPic")
        End Set
    End Property

    Public Property MyHelmet As ImageSource
        Get
            Return _myhelmet
        End Get
        Set(value As ImageSource)
            _myhelmet = value
            OnPropertyChanged("MyHelmet")
        End Set
    End Property

    Public Property MyAvgAttendance As String
        Get
            Return _myavgattendance
        End Get
        Set
            _myavgattendance = Value
            OnPropertyChanged("MyAvgAttendance")
        End Set
    End Property

    Public Property MyTeamRecord As String
        Get
            Return _myteamrecord
        End Get
        Set
            _myteamrecord = Value
            OnPropertyChanged("MyTeamRecord")
        End Set
    End Property

    Public Property MyBackgroundImg As ImageSource
        Get
            Return _mybackgroundimg
        End Get
        Set
            _mybackgroundimg = Value
            OnPropertyChanged("MyBackgroundImg")
        End Set
    End Property

#End Region

#Region "Enums"

    Public Enum DivisionNames
        <Description("AFC East")> AFCE = 1
        <Description("AFC North")> AFCN = 2
        <Description("AFC South")> AFCS = 3
        <Description("AFC West")> AFCW = 4
        <Description("NFC East")> NFCE = 5
        <Description("NFC North")> NFCN = 6
        <Description("NFC South")> NFCS = 7
        <Description("NFC West")> NFCW = 8
    End Enum

    Public Enum Teams

        <Description("Buffalo Bills")> BUF = 1
        <Description("New England Patriots")> NWE = 2
        <Description("New York Jets")> NYJ = 3
        <Description("Miami Dolphins")> MIA = 4
        <Description("Cincinnati Bengals")> CIN = 5
        <Description("Pittsburgh Steelers")> PIT = 6
        <Description("Baltimore Ravens")> BAL = 7
        <Description("Cleveland Browns")> CLE = 8
        <Description("Houston Texans")> HOU = 9
        <Description("Indianapolis Colts")> IND = 10
        <Description("Jacksonville Jaguars")> JAX = 11
        <Description("Tennessee Titans")> TEN = 12
        <Description("Denver Broncos")> DEN = 13
        <Description("Kansas City Chiefs")> KC = 14
        <Description("Oakland Raiders")> OAK = 15
        <Description("San Diego Chargers")> SDO = 16
        <Description("Washington Redskins")> WAS = 17
        <Description("Philadelphia Eagles")> PHI = 18
        <Description("New York Giants")> NYG = 19
        <Description("Dallas Cowboys")> DAL = 20
        <Description("Minnesota Vikings")> MIN = 21
        <Description("Green Bay Packers")> GNB = 22
        <Description("Detroit Lions")> DET = 23
        <Description("Chicago Bears")> CHI = 24
        <Description("Carolina Panthers")> CAR = 25
        <Description("Atlanta Falcons")> ATL = 26
        <Description("New Orleans Saints")> NWO = 27
        <Description("Tampa Bay Buccaneers")> TAM = 28
        <Description("Arizona Cardinals")> ARI = 29
        <Description("Seattle Seahawks")> SEA = 30
        <Description("Los Angeles Rams")> LAR = 31
        <Description("San Francisco 49ers")> SFO = 32
    End Enum

#End Region

    ''' <summary>
    '''     Sets the background picture of the screen
    ''' </summary>
    ''' <param name="TeamNum"></param>
    ''' <returns></returns>
    Public Shared Function GetBackgroundFilePath(Optional ByVal TeamNum As Integer = 32) As String
        Dim filepath = "pack://application:,,,/Project_Files/"

        Select Case TeamNum
            Case 0 : filepath += Bills02Jpg
            Case 1 : filepath += Patriots2Jpg
            Case 2 : filepath += JetsJpg
            Case 3 : filepath += Dolphins_2013Jpg
            Case 4 : filepath += Bengals3Jpg
            Case 5 : filepath += Steelers2Jpg
            Case 6 : filepath += Ravens3Jpg
            Case 7 : filepath += Browns2Jpg1
            Case 8 : filepath += Texans2Jpg
            Case 9 : filepath += Colts2Jpg
            Case 10 : filepath += Jaguars2Jpg
            Case 11 : filepath += Titans2Jpg
            Case 12 : filepath += Broncos2Jpg
            Case 13 : filepath += Chiefs3Jpg
            Case 14 : filepath += RaidersJpg
            Case 15 : filepath += Chargers5Jpg
            Case 16 : filepath += Redskins2Jpg
            Case 17 : filepath += Eagles2Jpg
            Case 18 : filepath += Giants5Jpg
            Case 19 : filepath += Cowboys3Jpg
            Case 20 : filepath += Vikings_2013_06Jpg
            Case 21 : filepath += Packers5Jpg
            Case 22 : filepath += Lions2Jpg
            Case 23 : filepath += Bears4Jpg
            Case 24 : filepath += Panthers2Jpg
            Case 25 : filepath += FalconsJpg
            Case 26 : filepath += Saints2Jpg
            Case 27 : filepath += Buccaneers2Jpg
            Case 28 : filepath += Cardinals3Jpg
            Case 29 : filepath += Seahawks2_2012Jpg
            Case 30 : filepath += RamsJpg
            Case 31 : filepath += _49ers04Jpg
            Case 32 : filepath += GlobalClass_GetBackgroundFilePath_FootballGoalLine_jpg
        End Select

        Return filepath
    End Function

    ''' <summary>
    '''     Sets the helmet image of the team on the button
    ''' </summary>
    ''' <param name="TeamNum"></param>
    ''' <returns></returns>

    Public Shared Function GetImage(TeamNum As Integer) As Image
        Dim MyImage As New Image
        Dim filepath = "pack://application:,,,/Project_Files/"

        Select Case TeamNum
            Case 0 : filepath += Bills_PHelmet_2011Jpg
            Case 1 : filepath += Patriots_PHelmetJpg
            Case 2 : filepath += Jets_PHelmetJpg
            Case 3 : filepath += Dolphins_PHelmetJpg
            Case 4 : filepath += Bengals_PHelmetJpg
            Case 5 : filepath += Steelers_PHelmetJpg
            Case 6 : filepath += Ravens_PHelmetJpg
            Case 7 : filepath += Browns_PHelmetJpg
            Case 8 : filepath += Texans_PHelmetJpg
            Case 9 : filepath += Colts_PHelmetJpg
            Case 10 : filepath += Jaguars_PHelmetJpg
            Case 11 : filepath += Titans_PHelmetJpg
            Case 12 : filepath += Broncos_PHelmetJpg
            Case 13 : filepath += Chiefs_PHelmetJpg
            Case 14 : filepath += Raiders_HelmetJpg
            Case 15 : filepath += Chargers_PHelmet2Jpg
            Case 16 : filepath += Redskins_PHelmetJpg
            Case 17 : filepath += Eagles_PHelmetJpg
            Case 18 : filepath += Giants_PHelmetJpg
            Case 19 : filepath += Cowboys_PhelmetJpg
            Case 20 : filepath += Vikings_PHelmet_2013Jpg
            Case 21 : filepath += Packers_PHelmetJpg
            Case 22 : filepath += Lions_PHelmetJpg
            Case 23 : filepath += Bears_PHelmet2Jpg
            Case 24 : filepath += Panthers_PHelmetJpg
            Case 25 : filepath += Falcons_PHelmetJpg
            Case 26 : filepath += Saints_PHelmetJpg
            Case 27 : filepath += Buccaneers_PHelmetJpg
            Case 28 : filepath += Cardinals_HelmetJpg
            Case 29 : filepath += Seahawks_PHelmet_2012Jpg
            Case 30 : filepath += Rams1Png
            Case 31 : filepath += _49ers_PHelmet_NewJpg
        End Select
        MyImage.Source = New BitmapImage(New Uri(filepath, UriKind.RelativeOrAbsolute))
        Return MyImage
    End Function

    ''' <summary>
    '''     Loads picture of the proper stadium by team
    ''' </summary>
    ''' <param name="TeamNum"></param>
    ''' <returns></returns>
    Public Shared Function GetStadiumPic(TeamNum As Integer) As String
        Dim FilePath = "pack://application:,,,/Project_Files/"
        Select Case TeamNum
            Case 0 : FilePath += RalphWilsonStadiumJpg
            Case 1 : FilePath += GilletteStadiumJpg
            Case 2, 18 : FilePath += MetLife_StadiumJpg
            Case 3 : FilePath += SunLifeStadiumJpg
            Case 4 : FilePath += PaulBrownStadiumJpg
            Case 5 : FilePath += HeinzFieldJpg
            Case 6 : FilePath += MTBankStadiumJpg
            Case 7 : FilePath += FirstEnergyStadiumJpg
            Case 8 : FilePath += NRGStadiumJpg
            Case 9 : FilePath += LucasOilStadiumJpg
            Case 10 : FilePath += EverBankFieldJpg
            Case 11 : FilePath += NissanStadiumJpg
            Case 12 : FilePath += SportsAuthorityFieldJpg
            Case 13 : FilePath += ArrowheadStadiumJpg
            Case 14 : FilePath += OaklandColiseumJpg
            Case 15 : FilePath += QualcommStadiumJpg
            Case 16 : FilePath += FedexFieldJpg
            Case 17 : FilePath += LincolnFinancialFieldJpg
            Case 19 : FilePath += ATTStadiumJpg
            Case 20 : FilePath += USBankStadiumJpg
            Case 21 : FilePath += LambeaufieldJpg
            Case 22 : FilePath += FordfieldJpg
            Case 23 : FilePath += SoldierFieldJpg
            Case 24 : FilePath += BankOfAmericaStadiumJpg
            Case 25 : FilePath += GeorgiaDomePng
            Case 26 : FilePath += Superdome_saintsJpg
            Case 27 : FilePath += RaymondJamesStadiumJpg
            Case 28 : FilePath += University_phoenix_stadiumJpg
            Case 29 : FilePath += CenturyLinkFieldJpg
            Case 30 : FilePath += LosAngelesColiseumJpg
            Case 31 : FilePath += LevisStadiumJpg
        End Select
        Return FilePath
    End Function

    Public Shared Function GetBrush(TeamNum As Integer, MyQueue As Queue, TeamDT As DataTable) As Queue
        TeamNum += 1
        For i = 0 To TeamDT.Rows.Count - 1
            If TeamDT.Rows(i).Item("TeamID") = TeamNum Then
                MyQueue.Enqueue(TeamDT.Rows(i).Item("MainColor"))
                MyQueue.Enqueue(TeamDT.Rows(i).Item("SecondaryColor"))
                MyQueue.Enqueue(TeamDT.Rows(i).Item("TrimColor"))
                Exit For
            End If
        Next i
        Return MyQueue
    End Function

    Public Shared Function ConvertColor(HexString As String) As Brush
        Dim converter = New BrushConverter()
        Dim myBrush As Brush
        myBrush = DirectCast(converter.ConvertFromString(HexString), Brush)
        Return myBrush
    End Function

End Class