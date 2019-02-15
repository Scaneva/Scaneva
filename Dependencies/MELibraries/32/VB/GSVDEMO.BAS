Attribute VB_Name = "GSVDEMO"
'************************************************'
'*                                              *'
'*  Demonstrationsprogramm für die              *'
'*  GSV-Funktionen in MEGSV.DLL                 *'
'*                                              *'
'*  Copyright (C) ME-Meßsysteme GmbH 2008-2009  *'
'*  Alle Rechte vorbehalten.                    *'
'*                                              *'
'*  ME-Meßsysteme GmbH                          *'
'*  Neuendorfstr. 18a                           *'
'*  D-16761 Hennigsdorf                         *'
'*                                              *'
'************************************************'

Option Explicit

Private Const INVALID_HANDLE_VALUE As Long = &HFFFFFFFF
Private Const ENABLE_PROCESSED_INPUT As Long = &H1&
Private Const STD_INPUT_HANDLE As Long = -10&
Private Const STD_OUTPUT_HANDLE As Long = -11&
Private Const KEY_EVENT As Long = 1

Private Type KEY_RECORD
    EventType As Integer
    dummy As Integer
    bKeyDown As Long
    wRepeatCount As Integer
    wVirtualKeyCode As Integer
    wVirtualScanCode As Integer
    AsciiChar As Byte
    DummyChar As Byte
    dwControlKeyState As Long
End Type

Private Declare Function AllocConsole Lib "kernel32" () As Long
Private Declare Function FreeConsole Lib "kernel32" () As Long
Private Declare Function SetConsoleTitleA Lib "kernel32" (ByVal lpConsoleTitle As String) As Long
Private Declare Function ReadConsoleInputA Lib "kernel32" (ByVal hConsoleInput As Long, ByRef lpBuffer As KEY_RECORD, ByVal nLength As Long, ByRef lpNumberOfEventsRead As Long) As Long
Private Declare Function WriteConsoleA Lib "kernel32" (ByVal hConsoleOutput As Long, ByVal lpBuffer As String, ByVal nNumberOfCharsToWrite As Long, ByRef lpNumberOfCharsWritten As Long, ByRef lpReserved As Any) As Long
Private Declare Function GetConsoleMode Lib "kernel32" (ByVal hConsoleHandle As Long, ByRef lpMode As Long) As Long
Private Declare Function SetConsoleMode Lib "kernel32" (ByVal hConsoleHandle As Long, ByVal dwMode As Long) As Long
Private Declare Function GetNumberOfConsoleInputEvents Lib "kernel32" (ByVal hConsoleInput As Long, ByRef lpcNumberOfEvents As Long) As Long
Private Declare Function GetStdHandle Lib "kernel32" (ByVal nStdHandle As Long) As Long
Private Declare Function CharToOemA Lib "user32" (ByVal lpszSrc As String, ByVal lpszDst As String) As Long

Private Const ComNr As Long = 1 ' Nummer der seriellen Schnittstelle
Private Const BuffSize As Long = 1000 ' Größe des Puffers, in dem die AD-Werte
                                      ' abgelegt werden, bis sie mit dem Befehl
                                      ' GSVread gelesen werden

Private konsole As Long
Private konsole_out As Long
Private modus As Long
Private konalloc As Boolean
Private zeichenda As Boolean
Private zeichen As KEY_RECORD
Private befehle() As String

Private Sub InitBefehl(ByVal str As String)
    Dim i As Long
    i = UBound(befehle)
    ReDim Preserve befehle(0 To i + 1)
    befehle(i) = str
End Sub

Private Sub InitKonsole()
    konsole = GetStdHandle(STD_INPUT_HANDLE)
    konsole_out = GetStdHandle(STD_OUTPUT_HANDLE)
    If konsole = INVALID_HANDLE_VALUE Or konsole_out = INVALID_HANDLE_VALUE Then
        ' Wenn die ausführbare Datei keine Konsolen-Anwendung ist (was bei VB
        ' immer der Fall zu sein scheint), wird eine neue Konsole allokiert.
        ' Dies kann vermieden werden, durch nachträgliches Ändern der ausführbaren
        ' Datei mit einem Hilfsprogramm wie z.B. EDITBIN:
        '     EDITBIN /SUBSYSTEM:CONSOLE GSVDEMO.EXE
        konalloc = AllocConsole <> 0
        konsole = GetStdHandle(STD_INPUT_HANDLE)
        konsole_out = GetStdHandle(STD_OUTPUT_HANDLE)
        If konsole = INVALID_HANDLE_VALUE Or konsole_out = INVALID_HANDLE_VALUE Or Not konalloc Then
            MsgBox "Konnte keine Konsole allokieren", vbOKOnly Or vbExclamation, App.EXEName
            End
        End If
        SetConsoleTitleA App.EXEName
    End If
    GetConsoleMode konsole, modus
    SetConsoleMode konsole, ENABLE_PROCESSED_INPUT
    zeichenda = False
End Sub

Private Sub Writetx(ByVal str As String)
    Dim wr As Long
    WriteConsoleA konsole_out, str, Len(str), wr, ByVal 0
End Sub

Private Sub Writeln()
    Writetx vbCr & vbLf
End Sub

Private Sub Ausgabe(ByVal str As String)
    Dim temp As String
    Dim l As Long
    temp = String(5 * Len(str), vbNullChar)
    CharToOemA str, temp
    l = InStr(temp, vbNullChar)
    If l > 0 Then temp = Left(temp, l - 1)
    Writetx temp
End Sub

Private Sub BefehlsUebersicht(ByVal einzel As Boolean, ByVal eing As Byte)
    Dim i As Integer
    
    If Not einzel Then
        Ausgabe "Befehlsauswahl"
        Writeln
    End If

    i = 0
    While befehle(i) <> ""
        If Not einzel Or eing = Left(befehle(i), 1) Then
            If einzel Then
                Writeln
                Writeln
            End If
            Writetx befehle(i)
            Writeln
        End If
        i = i + 1
    Wend
    
   If Not einzel Then
      Ausgabe "ESC  : Beenden"
      Writeln
      Writeln
   End If
End Sub

Private Function Taste(ByVal warte As Boolean) As Byte
    Dim n As Long
    If Not zeichenda Then
        GetNumberOfConsoleInputEvents konsole, n
        Do While n > 0 Or warte
            ReadConsoleInputA konsole, zeichen, 1, n
            If n = 1 And zeichen.EventType = KEY_EVENT And zeichen.bKeyDown <> 0 Then
                zeichenda = True
                Exit Do
            End If
            GetNumberOfConsoleInputEvents konsole, n
        Loop
    End If

    With zeichen
        If zeichenda And .wRepeatCount > 0 And .AsciiChar > 0 And .AsciiChar <= 127 Then
            Taste = .AsciiChar
            .wRepeatCount = .wRepeatCount - 1
            zeichenda = .wRepeatCount > 0
        Else
            Taste = 0
            zeichenda = False
        End If
    End With
End Function

Private Sub LiesZeile(ByRef z As String)
    Dim k As Byte
    
    z = ""

    Do
        k = Taste(True)

        If k >= 32 And k < 127 Then
            If Len(z) < 64 Then
                z = z & Chr(k)
                Writetx Chr(k)
            End If
        ElseIf k = 8 Or k = 127 Then
            If Len(z) > 0 Then
                z = Left(z, Len(z) - 1)
                Writetx Chr(8) & " " & Chr(8)
            End If
        End If
    Loop Until k = 13
End Sub

Private Sub WerteLesen()
    Dim ad As Double
    If GSVread(ComNr, ad) = GSV_TRUE Then
        Writetx "  " & Right("    " & Format(ad, "0.000000"), 12) & Chr(13)
    End If
End Sub

Private Sub FehlerMeldung(ByVal code As Integer, ByVal ref As Integer, ByVal warn As Boolean)
    If code = ref Then
        Ausgabe "-> Erfolgreich durchgeführt"
    ElseIf code = GSV_OK Then
        Ausgabe "-> Unerwartete Zeitüberschreitung!"
    Else
        Ausgabe "-> Fehler! Nicht durchgeführt"
    End If
    Writeln
    Writeln

    BefehlsUebersicht False, 0

    If warn Then
        Ausgabe "NACH DIESEM BEFEHL MÜSSEN SetCal UND"
        Ausgabe " SetZero AUSGEFÜHRT WERDEN!!!"
        Writeln
        Writeln
    End If
End Sub

Private Sub TastaturAbfragen(ByVal ca As Byte)
    Dim zeile As String
    Dim freq As Double
    Dim gain As Integer
    Dim fehler As Integer
    
    fehler = 0
    
    BefehlsUebersicht True, ca
    
    Select Case Chr(ca)
        Case "o"
            FehlerMeldung GSVsetOffset(ComNr), GSV_TRUE, False

        Case "c"
            FehlerMeldung GSVsetCal(ComNr), GSV_TRUE, False

        Case "z"
            FehlerMeldung GSVsetZero(ComNr), GSV_TRUE, False

        Case "s"
            FehlerMeldung GSVsetScale(ComNr), GSV_TRUE, False

        Case "b"
            FehlerMeldung GSVsetBipolar(ComNr), GSV_OK, False

        Case "u"
            FehlerMeldung GSVsetUnipolar(ComNr), GSV_OK, False
            
        Case "f"
            Ausgabe "Bitte Frequenz eingeben: "
            LiesZeile zeile
            Writeln
            On Error GoTo ErrHandler
            freq = CDbl(zeile)
            On Error GoTo 0
            If fehler <> 0 Then
                Ausgabe "Ungültige Eingabe"
                Writeln
                Writeln
            Else
                ' Nach GSVsetFreq müssen GSVsetCal und
                ' GSVsetZero ausgeführt werden!
                FehlerMeldung GSVsetFreq(ComNr, freq), GSV_OK, True
            End If
            While Taste(False) <> 0: Wend
  
        Case "g"
            Ausgabe "Bitte Verstärkung (Gain) eingeben: "
            LiesZeile zeile
            Writeln
            On Error GoTo ErrHandler
            gain = CInt(zeile)
            On Error GoTo 0
            If fehler <> 0 Then
                Ausgabe "Ungültige Eingabe"
                Writeln
                Writeln
            Else
                ' Nach GSVsetGain müssen GSVsetCal und
                ' GSVsetZero ausgeführt werden!
                FehlerMeldung GSVsetGain(ComNr, gain), GSV_OK, True
            End If
            While Taste(False) <> 0: Wend
        Case Else
    End Select
    
    Exit Sub
ErrHandler:
    fehler = Err.number
    Resume Next
End Sub

Sub Main()
    Dim ch As Byte
    Dim tmp As Long

    ReDim befehle(0 To 0)
    InitBefehl "o    : GSVsetOffset(ComNr)"
    InitBefehl "c    : GSVsetCal(ComNr)"
    InitBefehl "z    : GSVsetZero(ComNr)"
    InitBefehl "s    : GSVsetScale(ComNr)"
    InitBefehl "b    : GSVsetBipolar(ComNr)"
    InitBefehl "u    : GSVsetUnipolar(ComNr)"
    InitBefehl "f    : GSVsetFreq(ComNr, Freq)"
    InitBefehl "g    : GSVsetGain(ComNr, Gain)"

    InitKonsole

    Writeln
    Ausgabe "Demonstrationsprogramm für MEGSV.DLL"
    Writeln
    Ausgabe "Version der Bibliothek: "
    tmp = GSVversion
    Writetx Format(tmp \ 65536, "0") & "."
    tmp = tmp And &HFFFF&
    Writetx Format(tmp \ 10, "0") & Format(tmp Mod 10, "0")
    Writeln

    If GSVactivate(ComNr, BuffSize) <> GSV_OK Then
        Writeln
        Ausgabe "Initialisierung des GSV fehlgeschlagen"
        Writeln
    Else
        tmp = GSVmodel(ComNr)
        Ausgabe "Baugruppen-Modell: "
        Writetx Format(tmp, "0")
        Writeln
        Writeln

        BefehlsUebersicht False, 0

        Do
            ch = Taste(False)
            WerteLesen
            TastaturAbfragen ch
        Loop Until ch = 27

        GSVrelease ComNr

        Writeln
    End If
        
    SetConsoleMode konsole, modus
    If konalloc Then FreeConsole
End Sub

