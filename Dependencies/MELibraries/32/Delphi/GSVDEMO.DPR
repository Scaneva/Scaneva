{$A+,B-,D-,H-,I+,J-,L-,M-,O-,P-,Q-,R-,T-,U-,V+,W-,X+,Y-,Z4}
{$APPTYPE CONSOLE}
{$DESCRIPTION 'Demonstration program for MEGSV.DLL'}

(************************************************)
(*                                              *)
(*  Demonstrationsprogramm für die              *)
(*  GSV-Funktionen in MEGSV.DLL                 *)
(*                                              *)
(*  Copyright (C) ME-Meßsysteme GmbH 1999-2009  *)
(*  Alle Rechte vorbehalten.                    *)
(*                                              *)
(*  ME-Meßsysteme GmbH                          *)
(*  Neuendorfstr. 18a                           *)
(*  D-16761 Hennigsdorf                         *)
(*                                              *)
(************************************************)

program GSVDemo;

uses
  Windows,MEGSV;

const
  ComNr = 1;            (* Nummer der seriellen Schnittstelle *)
  BuffSize  = 1000;     (* Größe des Puffers, in dem die AD-Werte
                           abgelegt werden, bis sie mit dem Befehl
                           GSVread gelesen werden *)

const
  befehle : array [0..8] of PChar =
  (
    'o    : GSVsetOffset(ComNr)',
    'c    : GSVsetCal(ComNr)',
    'z    : GSVsetZero(ComNr)',
    's    : GSVsetScale(ComNr)',
    'b    : GSVsetBipolar(ComNr)',
    'u    : GSVsetUnipolar(ComNr)',
    'f    : GSVsetFreq(ComNr, Freq)',
    'g    : GSVsetGain(ComNr, Gain)',
    nil
  );

var
  ch : Char;
  tmp : LongInt;
  konsole : THandle;
  modus : DWORD;
  zeichenda : Boolean;
  zeichen : packed record case Boolean of
              false: (IR : TInputRecord);
              true: (EventType : array [0..1] of Word;
                     KeyEvent : TKeyEventRecord)
            end;

procedure InitKonsole;
  begin
    konsole := GetStdHandle(STD_INPUT_HANDLE);
    GetConsoleMode(konsole, modus);
    SetConsoleMode(konsole, ENABLE_PROCESSED_INPUT);
    zeichenda := False
  end;

procedure Ausgabe(str: PChar);
  var
    temp : array [0..80] of Char;
  begin
    CharToOem(str, temp);
    Write(temp)
  end;

procedure BefehlsUebersicht(einzel: Boolean; taste: Char);
  var
    i : Integer;
  begin
    if not einzel then
      begin
        Ausgabe('Befehlsauswahl');
        Writeln
      end;

    i := 0;
    while befehle[i] <> nil do
      begin
        if not einzel or (taste = befehle[i][0]) then
          begin
            if einzel then
              begin
                Writeln;
                Writeln
              end;
            Writeln(befehle[i])
          end;
        i := i + 1
      end;

   if not einzel then
     begin
      Ausgabe('ESC  : Beenden');
      Writeln;
      Writeln
     end
 end;

function Taste(warte: Boolean): Char;
  var
    n : DWORD;
  begin
    if not zeichenda then
      begin
        GetNumberOfConsoleInputEvents(konsole, n);
        while (n > 0) or warte do
          begin
            ReadConsoleInput(konsole, zeichen.IR, 1, n);
            if (n = 1) and (zeichen.EventType[0] = KEY_EVENT) and
               zeichen.KeyEvent.bKeyDown then
              begin
                zeichenda := True;
                break
              end;
            GetNumberOfConsoleInputEvents(konsole, n)
          end
      end;
          
    with zeichen.KeyEvent do
      begin
        if zeichenda and (wRepeatCount > 0) and
           (AsciiChar > Chr(0)) and (AsciiChar <= Chr(127)) then
          begin
            Taste := AsciiChar;
            wRepeatCount := wRepeatCount - 1;
            zeichenda := wRepeatCount > 0
          end
        else
          begin
            Taste := Chr(0);
            zeichenda := False
          end
      end
  end;

procedure LiesZeile(var z: array of Char);
  var
    i : Integer;
    k : Char;
  begin
    i := 0;

    repeat
      k := Taste(True);

      if (k >= ' ') and (k < Chr(127)) then
        begin
          if i < high(z) then
            begin
              z[i] := k;
              i := i + 1;
              Write(k)
            end
        end
      else if (k = Chr(8)) or (k = Chr(127)) then
        begin
          if i > 0 then
            begin
              i := i - 1;
              Write(Chr(8), ' ', Chr(8))
            end
        end
    until k = Chr(13);      

    z[i] := Chr(0)
  end;

procedure WerteLesen;
  var
    ad : Double;
  begin
    if GSVread(ComNr, ad) = GSV_TRUE then
      Write('  ', ad:12:6, Chr(13))
  end;

procedure FehlerMeldung(code, ref: Integer; warn: Boolean);
  begin
    if code = ref then
      Ausgabe('-> Erfolgreich durchgeführt')
    else if code = GSV_OK then
      Ausgabe('-> Unerwartete Zeitüberschreitung!')
    else
      Ausgabe('-> Fehler! Nicht durchgeführt');
    Writeln;
    Writeln;

    BefehlsUebersicht(False, Chr(0));

    if warn then
      begin
        Ausgabe('NACH DIESEM BEFEHL MÜSSEN SetCal UND');
        Ausgabe(' SetZero AUSGEFÜHRT WERDEN!!!');
        Writeln;
        Writeln
      end
  end;

procedure TastaturAbfragen(ca: Char);
  var
    zeile : array [0..64] of Char;
    freq : Double;
    gain, fehler : Integer;
  begin
    BefehlsUebersicht(True, ca);

    case ca of
      'o': FehlerMeldung(GSVsetOffset(ComNr), GSV_TRUE, False);

      'c': FehlerMeldung(GSVsetCal(ComNr), GSV_TRUE, False);

      'z': FehlerMeldung(GSVsetZero(ComNr), GSV_TRUE, False);

      's': FehlerMeldung(GSVsetScale(ComNr), GSV_TRUE, False);

      'b': FehlerMeldung(GSVsetBipolar(ComNr), GSV_OK, False);

      'u': FehlerMeldung(GSVsetUnipolar(ComNr), GSV_OK, False);

      'f': begin
             Ausgabe('Bitte Frequenz eingeben: ');
             LiesZeile(zeile);
             Writeln;
             Val(zeile, freq, fehler);
             if fehler <> 0 then
               begin
                 Ausgabe('Ungültige Eingabe');
                 Writeln;
                 Writeln
               end
             else
               begin
                 (* Nach GSVsetFreq müssen GSVsetCal und *)
                 (* GSVsetZero ausgeführt werden!        *)
                 FehlerMeldung(GSVsetFreq(ComNr, freq), GSV_OK, True)
               end;
             while (Taste(False) <> Chr(0)) do
           end;

      'g': begin
             Ausgabe('Bitte Verstärkung (Gain) eingeben: ');
             LiesZeile(zeile);
             Writeln;
             Val(zeile, gain, fehler);
             if fehler <> 0 then
               begin
                 Ausgabe('Ungültige Eingabe');
                 Writeln;
                 Writeln
               end
             else
               begin
                 (* Nach GSVsetGain müssen GSVsetCal und *)
                 (* GSVsetZero ausgeführt werden!        *)
                 FehlerMeldung(GSVsetGain(ComNr, gain), GSV_OK, True)
               end;
             while (Taste(False) <> Chr(0)) do
           end
    else
    end
  end;

begin                   (* Hauptprogramm *)
  InitKonsole;

  Writeln;
  Ausgabe('Demonstrationsprogramm für MEGSV.DLL');
  Writeln;
  Ausgabe('Version der Bibliothek: ');
  tmp := GSVversion;
  Write(tmp shr 16:1, '.');
  tmp := tmp and $FFFF;
  Writeln(tmp div 10:1, tmp mod 10:1);

  if GSVactivate(ComNr, BuffSize) <> GSV_OK then
    begin
      Writeln;
      Ausgabe('Initialisierung des GSV fehlgeschlagen');
      Writeln
    end
  else
    begin
      tmp := GSVmodel(ComNr);
      Ausgabe('Baugruppen-Modell: ');
      Writeln(tmp:1);
      Writeln;

      BefehlsUebersicht(False, Chr(0));

      repeat
        ch := Taste(False);
        WerteLesen();
        TastaturAbfragen(ch)
      until ch = Chr(27);

      GSVrelease(ComNr);

      Writeln
    end;

  SetConsoleMode(konsole, modus)
end.
