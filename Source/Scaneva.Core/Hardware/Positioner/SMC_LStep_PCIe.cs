#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SMC_LStep_PCIe.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see http://www.gnu.org/licenses/.
//  </copyright>
//  <summary>
//  Url: https://github.com/Scaneva
//  </summary>
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Scaneva.Core;
using Scaneva.Core.Settings;
using System.Runtime.InteropServices;
using System.IO;

using Scaneva.Tools;
using System.Windows.Forms;

namespace Scaneva.Core.Hardware
{
    [DisplayName("LANG LStep PCIe controller")]
    [Category("Positioner")]
    class SMC_LStep_PCIe : ParametrizableObject, IHWManager, IPositioner, ITransducer
    {

        private CClassLStep.LStep Positioner;
        private enuPositionerStatus mPosStatus = enuPositionerStatus.NotInitialized;
        public List<TransducerChannel> channels = new List<TransducerChannel>();
        Dictionary<long, String> ErrorList = new Dictionary<long, string>();
        enuHWStatus mHWStatus = enuHWStatus.NotInitialized;
        public SMC_LStep_PCIe(LogHelper log) : base(log)
        {
            settings = new SMC_LStep_PCIe_Settings();
            log.Add("Initializing LANG LStep PCIe controller");
            InitTransducerChannels();

            ErrorList.Add(0, "Alles OK");
            ErrorList.Add(1, "Angegebene Achse nicht vorhanden");
            ErrorList.Add(2, "Funktion kann nicht ausgeführt werden");
            ErrorList.Add(3, "Zu viele Zeichen im Befehlsstring");
            ErrorList.Add(4, "Unbekannter Befehl");
            ErrorList.Add(5, "Außerhalb des gültigen Zahlenbereichs");
            ErrorList.Add(6, "Anzahl Parameter falsch");
            ErrorList.Add(7, "Befehl muss mit !oder ? beginnen");
            ErrorList.Add(8, "Kein TVR möglich, da Achse aktiv");
            ErrorList.Add(9, "Kein Ein - oder Ausschalten der Achsen, da TVR aktiv");
            ErrorList.Add(10, "Funktion nicht konfiguriert");
            ErrorList.Add(11, "Kein Move - Befehl möglich, da Joystick aktiv");
            ErrorList.Add(12, "Endschalter betätigt");
            ErrorList.Add(13, "Funktion kann nicht ausgeführt werden, da Encoder erkannt");
            ErrorList.Add(14, "Fehler beim Kalibrieren!Endschalter nicht korrekt freigefahren");
            ErrorList.Add(15, "Fehler beim Kalibrieren auf Referenzmarke");
            ErrorList.Add(16, "Save - Befehl fehlgeschlagen");
            ErrorList.Add(17, "Achse noch in Benutzung");
            ErrorList.Add(18, "Achse nicht bereit");
            ErrorList.Add(19, "Achse nicht kalibriert");
            ErrorList.Add(20, "Treiberrelais defekt(Sicherheitskreis K3 / K4)");
            ErrorList.Add(21, "Es dürfen nur einzelne Vektoren verfahren werden(Einrichtbetrieb)");
            ErrorList.Add(22, "Es darf kein Kalibrieren, Tischhubmessen oder Joystickbetrieb durchgeführt werden.(Tür offen oder Einrichtbetrieb)");
            ErrorList.Add(23, "SECURITY Error X - Achse");
            ErrorList.Add(24, "SECURITY Error Y - Achse");
            ErrorList.Add(25, "SECURITY Error Z - Achse");
            ErrorList.Add(26, "SECURITY Error A - Achse");
            ErrorList.Add(27, "Not - STOP");
            ErrorList.Add(28, "Fehler im Türschaltersicherheitskreis");
            ErrorList.Add(29, "Endstufen nicht eingeschaltet");
            ErrorList.Add(30, "GAL Sicherheitsfehler");
            ErrorList.Add(31, "Joystick lässt sich nicht einschalten, da move aktiv");
            ErrorList.Add(32, "Vektor außerhalb des Verfahrbereiches");

            ErrorList.Add(1010, "Anderer manueller Modus bereits aktiv");
            ErrorList.Add(1011, "Servo - und Schrittmotor nicht koppelbar(Joystick)");
            ErrorList.Add(1012, "Ausgang bereits anderer Funktion zugeordnet(digitaler Ausgang)");
            ErrorList.Add(1030, "Konfigurierung ist aktiv");

            ErrorList.Add(1031, "Achse nicht konfiguriert");
            ErrorList.Add(1032, "Interner Fehler");
            ErrorList.Add(1033, "Achse noch in Benutzung");
            ErrorList.Add(1034, "Achse in Fehlerstatus");
            ErrorList.Add(1035, "Achse nicht kalibriert");
            ErrorList.Add(1036, "Achse ohne RoomMeasure");
            ErrorList.Add(1037, "Min.Grenze unbekannt");
            ErrorList.Add(1038, "Max.Grenze unbekannt");
            ErrorList.Add(1039, "Notstopp ausgelöst");
            ErrorList.Add(1040, "Endschalter angefahren");
            ErrorList.Add(1041, "Verfahrweg zu klein");
            ErrorList.Add(1042, "Geschwindigkeit zu klein");
            ErrorList.Add(1043, "Ruck zu klein");
            ErrorList.Add(1044, "Kein Trigger Endschalter rein");
            ErrorList.Add(1045, "Kein Trigger Endschalter raus");
            ErrorList.Add(1046, "Fahrweg geclippt");
            ErrorList.Add(1047, "Endschalter überfahren");

            ErrorList.Add(1064, "Wegstrecke zu groß");
            ErrorList.Add(1065, "Bremse und Spannungsversorgung für Endschalter nicht gleichzeitig möglich");
            ErrorList.Add(1066, "Keine Kommutierung nötig");
            ErrorList.Add(1067, "Achse nicht kommutiert");

            ErrorList.Add(1096, "Min.Endschalter aktiv");
            ErrorList.Add(1097, "Max.Endschalter aktiv");
            ErrorList.Add(1098, "Nicht bereit zur Autokommutierung");
            ErrorList.Add(1099, "Kein interpolierender Geber gefunden");
            ErrorList.Add(1100, "I²T Überwachung angesprochen(Langzeit)");
            ErrorList.Add(1101, "I²T Überwachung angesprochen(Kurzzeit)");
            ErrorList.Add(1102, "Überstrom Endstufe");
            ErrorList.Add(1103, "Überstrom beim Einschalten");
            ErrorList.Add(1104, "Überspannung");
            ErrorList.Add(1105, "Sicherung Zwischenkreisspannung defekt");
            ErrorList.Add(1106, "Encoderfehler: Amplitude zu klein");
            ErrorList.Add(1107, "Encoderfehler: Amplitude zu groß");
            ErrorList.Add(1108, "Schleppfehler zu groß");
            ErrorList.Add(1109, "Geschwindigkeit zu groß");
            ErrorList.Add(1110, "Motor blockiert");
            ErrorList.Add(1111, "Motorbremse fehlerhaft");
            ErrorList.Add(1112, "Übertemperatur der Endstufe");
            ErrorList.Add(1113, "Motor überhitzt");
            ErrorList.Add(1114, "Endschalter bei Autokommutierung geschaltet");
            ErrorList.Add(1115, "Lesefehler Temeratur der Endstufe");
            ErrorList.Add(1116, "Zielfenster nicht erreicht");
            ErrorList.Add(1117, "Achse wird Verfahren");
            ErrorList.Add(1118, "Schalter für min.Fahrbereich betätigt");
            ErrorList.Add(1119, "Schalter für max.Fahrbereich betätigt");
            ErrorList.Add(1120, "Zielposition außerhalb min.Fahrbereich");
            ErrorList.Add(1121, "Zielposition außerhalb max.Fahrbereich");
            ErrorList.Add(1122, "Mehrere Endschalter gleichzeitig betätigt");
            ErrorList.Add(1123, "Endstufe durch Hardwareüberwachung ausgeschaltet");
            ErrorList.Add(1124, "Spurfehler Encoder");
            ErrorList.Add(1125, "Amplitude des Endoders zu klein, eventuell kein Geber angeschlossen.");
            ErrorList.Add(1126, "Winkel bei Autokommutierung ausserhalb des Toleranzbereichs, Achse eventuell blockiert");
            ErrorList.Add(1127, "Keine Rundachse");
            ErrorList.Add(1128, "Kein 0 Endschalter / Keine Geber Referenzmarke");
            ErrorList.Add(1129, "Kein Geber Interface");
            ErrorList.Add(1130, "Gebereingang mehrfach zugewiesen");
            ErrorList.Add(1131, "eQep - Gebereingänge nicht konfiguriert(Hardware - Konfiguration MFP)");
            ErrorList.Add(1132, "Zielfenster nicht innerhalb erlaubter Zeit erreicht");
            ErrorList.Add(1133, "Gebereingang nicht verfügbar");
            ErrorList.Add(1134, "Autokommutierungsstrom größer als Nennstrom");
            ErrorList.Add(1135, "Autokommutierungsstrom gleich Null");
            ErrorList.Add(1160, "Dynamische Checksumme des EEProms falsch");
            ErrorList.Add(1161, "Statische Checksumme des EEProms falsch");
            ErrorList.Add(1162, "Falsche EEProm version");
            ErrorList.Add(1163, "EEProm Struktur fehlerhaft");
            ErrorList.Add(1164, "Fenster für Rechenzeit überschritten(500 / 320μs)");
            ErrorList.Add(1165, "Fenster für Rechenzeit überschritten(62, 5 / 40μs)");
            ErrorList.Add(1193, "Warnung: Übertemperatur Endstufe");
            ErrorList.Add(1194, "Warnung: Motortemperatur zu hoch");
            ErrorList.Add(1195, "Treiberspannung unterschritten");
            ErrorList.Add(1196, "Achse deaktiviert");
            ErrorList.Add(1197, "Zwischenkreisspannung zu niedrig");
            ErrorList.Add(1198, "Zwischenkreisspannung zu hoch");
            ErrorList.Add(1250, "Oszilloskop Pretrigger Position größer als Oszilloskop Datengröße");

            ErrorList.Add(4001, "Interner Fehler");
            ErrorList.Add(4002, "Interner Fehler");
            ErrorList.Add(4003, "undefinierter Fehler");
            ErrorList.Add(4004, "unbekannter Schnittstellentyp(kann bei Connect… auftreten)");
            ErrorList.Add(4005, "Fehler beim Initialisieren der Schnittstelle");
            ErrorList.Add(4006, "Keine Verbindung zur Steuerung(z.B.wenn SetPitch vor Connect aufgerufen wird)");
            ErrorList.Add(4007, "Timeout während Lesen von der Steuerung");
            ErrorList.Add(4008, "Fehler bei Befehlsübertragung an die LSTEP");
            ErrorList.Add(4009, "Befehl wurde abgebrochen(mit SetAbortFlag)");
            ErrorList.Add(4010, "Befehl wird von LSTEP nicht unterstützt");
            ErrorList.Add(4011, "Joystick aktiv(kann bei SetJoystickOn / Off auftreten)");
            ErrorList.Add(4012, "Kein Verfahrbefehl möglich, da Joystick aktiv");
            ErrorList.Add(4013, "Regler - Timeout bei Move - Befehl");
            ErrorList.Add(4014, "Fehler beim Kalibrieren, Endschalter nicht korrekt freigefahren");
            ErrorList.Add(4015, "Endschalter in Verfahrrichtung betätigt");
            ErrorList.Add(4016, "Wiederholter Vektorstart!!(Regelung)");
            ErrorList.Add(4031, "Joystick lässt sich nicht einschalten, da move aktiv!");
            ErrorList.Add(4032, "Softwarelimits undefiniert");
        }


        public SMC_LStep_PCIe_Settings Settings
        {
            get
            {
                return (SMC_LStep_PCIe_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        public bool IsEnabled { get; set; }

        public enuHWStatus Connect()
        {
            Positioner = new CClassLStep.LStep();
            HWStatus |= LogError(Positioner.ConnectSimpleW(11, "COM" + Convert.ToString(Settings.COMPort), 115200, false));
            HWStatus |= LogError(Positioner.SetLanguageW("ENG"));
            HWStatus |= LogError(Positioner.FlushBuffer(0));
            HWStatus |= LogError(Positioner.EnableCommandRetry(false));

            //string pcVers = "";
            //mHWStatus |= LogError(Positioner.GetAPIVersionW(out pcVers, 100));
            //string pcSerialNr = "";
            //mHWStatus |= LogError(Positioner.GetSerialNrW(out pcSerialNr, 256)); // SerNr
            //string fwVers = "";
            //mHWStatus |= LogError(Positioner.GetVersionStrW(out fwVers, 64)); //FW version

            if (Settings.ReadConfigurationF)
            {
                if (File.Exists(Settings.Path))
                {
                    HWStatus |= LogError(Positioner.LoadConfigW(Settings.Path)); // Lade Config Datei
                    HWStatus |= LogError(Positioner.SetControlPars()); // Setzen der Geladenen Parameter in Controller
                }
                log.Add("Configuration profile was not found. The run-time profile will be " +
                    "created based on user settings. It is advised to save the profile during next program run");
            }
            else
            {
                //Beschreibung der Mechanik
                HWStatus |= LogError(Positioner.SetFactorMode(true, 1, 1, 1, 0));
                HWStatus |= LogError(Positioner.SetPitch(Settings.X.Pitch, Settings.Y.Pitch, Settings.Z.Pitch, 1));
                HWStatus |= LogError(Positioner.SetControllerSteps(Settings.X.FullSteps, Settings.Y.FullSteps, Settings.Z.FullSteps, 200));

                HWStatus |= LogError(Positioner.ConfigMaxAxes(4));

                //    SetGear()
                HWStatus |= LogError(Positioner.SetGear(Settings.X.MotorGear, Settings.Y.MotorGear, Settings.Z.MotorGear, Settings.A.MotorGear));

                //    SetDimensions() - µm
                HWStatus |= LogError(Positioner.SetDimensions(1, 1, 1, 1));

                //    SetActiveAxis()
                int e = 0;
                if (Settings.X.Enabled) e++;
                if (Settings.Y.Enabled) e = e + 2;
                if (Settings.Z.Enabled) e = e + 4;
                if (Settings.A.Enabled) e = e + 8;
                HWStatus |= LogError(Positioner.SetActiveAxes(e));

                //    SetAxisDirection()
                HWStatus |= LogError(Positioner.SetAxisDirection(Math.Abs(Convert.ToInt16(!Settings.X.Sign)),
                    Math.Abs(Convert.ToInt16(!Settings.Y.Sign)),
                    Math.Abs(Convert.ToInt16(!Settings.Z.Sign)), 1));

                //    SetXYComp(),
                HWStatus |= LogError(Positioner.SetXYAxisComp(1));

                //Konfiguration der Endschalter
                //    SetSwitchActive(),
                HWStatus |= LogError(Positioner.SetSwitchActive(5, 5, 5, 5));

                //    SetSwitchPolarity(),
                HWStatus |= LogError(Positioner.SetSwitchPolarity(5, 5, 5, 5));


                //  Configuration der Softwareendschalter
                //SetLimit(),
                HWStatus |= LogError(Positioner.SetLimit(1, 0, Settings.X.Travel));
                HWStatus |= LogError(Positioner.SetLimit(2, 0, Settings.Y.Travel));
                HWStatus |= LogError(Positioner.SetLimit(3, 0, Settings.Z.Travel));
                HWStatus |= LogError(Positioner.SetLimit(4, 0, Settings.A.Travel));

                //SetLimitControl(),
                HWStatus |= LogError(Positioner.SetLimitControl(1, true));
                HWStatus |= LogError(Positioner.SetLimitControl(2, true));
                HWStatus |= LogError(Positioner.SetLimitControl(3, true));
                HWStatus |= LogError(Positioner.SetLimitControl(4, true));

                HWStatus |= LogError(Positioner.SetLimitControlMode(1));
                //mHWStatus |= LogError(Positioner.SetAutoLimitAfterCalibRM(15);

                //rotativer 2-Phasen Schrittmotor
                HWStatus |= LogError(Positioner.SetMotorType(0, 0, 0, 0));

                //RPM
                HWStatus |= LogError(Positioner.SetMotorMaxVel((60 * (Settings.X.MaxSpeed / 1000) / Settings.X.Pitch), (60 * (Settings.Y.MaxSpeed / 1000) / Settings.Y.Pitch),
                    (60 * (Settings.Z.MaxSpeed / 1000) / Settings.Z.Pitch), (60 * (Settings.A.MaxSpeed / 1000) / Settings.A.Pitch)));

                //SetCurrent()
                HWStatus |= LogError(Positioner.SetMotorCurrent(Settings.X.MotorCurrent, Settings.Y.MotorCurrent, Settings.Z.MotorCurrent, Settings.A.MotorCurrent));

                //SetReduction()
                HWStatus |= LogError(Positioner.SetMotorCurrent(Settings.X.MotorCurrentReduction, Settings.Y.MotorCurrentReduction,
                    Settings.Z.MotorCurrentReduction, Settings.A.MotorCurrentReduction));

                //            Konfiguration der Encoder
                //SetEncoderActive();
                //            SetEncoderPeriod();
                //            SetEncoderRefSignal();
                //            SetEncoderPosition();

                //            Parametrierung des Reglers
                //SetTargetWindow();
                //            SetControllerCall();
                //            SetControllerSteps();
                //            SetControllerFaktor();
                //            SetControllerTWDelay();
                //            SetControllerTimeout();

                //            Konfiguration des Trigger
                //SetTriggerPar(),
                //SetTrigger(),

                //            Konfiguration des Snapshot
                //SetSnapshotPar(),
                //SetSnapshot(),


                HWStatus |= LogError(Positioner.SetAccel(Settings.X.Acceleration, Settings.Y.Acceleration,
                        Settings.Z.Acceleration, Settings.A.Acceleration));

                HWStatus |= LogError(Positioner.SetVel(Settings.X.FailSafeSpeed, Settings.Y.FailSafeSpeed,
                    Settings.Z.FailSafeSpeed, Settings.A.FailSafeSpeed)); // will be set at run time. Some initial values here

                //            Einstellen des TVR Modes
                //SetTVRMode(),
                HWStatus |= LogError(Positioner.SetCommandTimeout(1000, 0, 0));



                //mHWStatus |= LogError(Positioner.SetPos(0, 0, 0, 0));

            }
            HWStatus |= LogError(Positioner.SetPowerAmplifier(true)); //Schaltet die Endstufen der Steuerung Ein


            if (Settings.SaveConfigurationE)
            {
                HWStatus |= LogError(Positioner.LStepSave()); // Speichern in EEPROM
                Settings.SaveConfigurationE = false;
            }

            if (Settings.SaveConfigurationF)
            {
                HWStatus |= LogError(Positioner.SaveConfigW(Settings.Path));
                Settings.SaveConfigurationF = false;
            }

            /*
            if (Settings.SoftwareReset)
            {
                mHWStatus |= LogError(Positioner.SoftwareReset(););
                Settings.SaveConfigurationF = false;
            }
            */

            if (Settings.X.Recalibrate)
            {
                DialogResult dialogResult = MessageBox.Show("X-axis re-calibration was requested. Execute now?", "X-axis re-calibration", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= ReCalibrateAxes(1);
                }
                Settings.X.Recalibrate = false;
            }
            if (Settings.Y.Recalibrate)
            {
                DialogResult dialogResult = MessageBox.Show("Y-axis re-calibration was requested. Execute now?", "Y-axis re-calibration", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= ReCalibrateAxes(2);
                }
                Settings.Y.Recalibrate = false;
            }
            if (Settings.Z.Recalibrate)
            {
                DialogResult dialogResult = MessageBox.Show("Z-axis re-calibration was requested. Execute now?", "Z-axis re-calibration", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= ReCalibrateAxes(3);
                }
                Settings.Z.Recalibrate = false;
            }
            if (Settings.A.Recalibrate)
            {
                DialogResult dialogResult = MessageBox.Show("A-axis re-calibration was requested. Execute now?", "A-axis re-calibration", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= ReCalibrateAxes(4);
                }
                Settings.A.Recalibrate = false;
            }

            if (Settings.X.Remeasure)
            {
                DialogResult dialogResult = MessageBox.Show("X-axis re-measurement was requested. Execute now?", "X-axis re-measurement", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= RemeasureAxes(1);
                }
                Settings.X.Remeasure = false;
            }
            if (Settings.Y.Remeasure)
            {
                DialogResult dialogResult = MessageBox.Show("Y-axis re-measurement was requested. Execute now?", "Y-axis re-measurement", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= RemeasureAxes(2);
                }
                Settings.Y.Remeasure = false;
            }
            if (Settings.Z.Remeasure)
            {
                DialogResult dialogResult = MessageBox.Show("Z-axis re-measurement was requested. Execute now?", "Z-axis re-measurement", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= RemeasureAxes(3);
                }
                Settings.Z.Remeasure = false;
            }
            if (Settings.A.Remeasure)
            {
                DialogResult dialogResult = MessageBox.Show("A-axis re-measurement was requested. Execute now?", "A-axis re-measurement", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    HWStatus |= RemeasureAxes(4);
                }
                Settings.A.Remeasure = false;
            }
            return HWStatus;
        }


        private enuHWStatus ReCalibrateAxes(int _axis)
        {
            //            calibrate
            // mHWStatus |= LogError(Positioner.SetCalibRMAccel(&XD, &YD, &ZD, &AD)); //Stellt Beschleunigung ab, die für den Kalibriervorgang verwendet werden soll. eingestellten Dimension/s²
            //                measure
            //            Setzen der Verfahrgeschwindigkeiten die für das Herausfahren aus den Endschaltern
            //während des Kalibriervorgangs bzw. des Tischhubmessens verwendet
            //werden sollen.
            enuHWStatus stat = LogError(Positioner.SetCalibRMBackSpeed(500, 500, 500, 500)); //µm/s

            //            Setzen der Verfahrgeschwindigkeiten, welche während des Kalibriervorgangs
            //verwendet werden sollen.
            stat |= LogError(Positioner.SetCalibRMVel(1000, 1000, 1000, 1000));//µm/s

            //          Funktion zum Setzen eines Kalibrier - Offsets, der beim Kalibrieren nach dem
            //Freifahren des Enschalters gefahren wird.
            stat |= LogError(Positioner.SetCalibOffset(1000, 1000, 1000, 1000));

            //Funktion zum Starten der Kalibrierroutine einer einzelnen Achse.
            stat |= LogError(Positioner.CalibrateEx(Convert.ToInt16(Math.Pow(_axis, 2))));
            return stat;
        }

        private enuHWStatus RemeasureAxes(int _axis)
        {
            enuHWStatus stat = enuHWStatus.Ready;
            stat |= LogError(Positioner.RMeasureEx(Convert.ToInt16(Math.Pow(_axis, 2))));
            return stat;
        }
        /*
        private string SendString(string _string)
        {
            string Ret = "";
            mHWStatus |= LogError(Positioner.SendStringW(_string, out Ret, 256, true, 1000));
            return Ret;
        }

        private string SendPosCommand(string _string)
        {
            string Ret = "";
            mHWStatus |= LogError(Positioner.SendStringPosCmdW(_string, out Ret, 256, true, 1000));
            return Ret;
        }
        */
        public enuHWStatus Initialize()
        {
            return HWStatus;
        }

        public void Release()
        {
            HWStatus |= LogError(Positioner.SetPowerAmplifier(false)); //Schaltet die Endstufen der Steuerung aus
            if (Positioner.Disconnect() == 0)
            {
                HWStatus &= ~enuHWStatus.Ready;
                HWStatus = enuHWStatus.NotInitialized;
            }
            else
            {
                HWStatus = enuHWStatus.Error;
            }
        }
        public enuHWStatus HWStatus
        {
            get
            {
                return mHWStatus;
            }
            set
            {
                mHWStatus = value;
                if (mHWStatus == enuHWStatus.NotInitialized) mPosStatus = enuPositionerStatus.NotInitialized;
                if (mHWStatus == enuHWStatus.Ready) mPosStatus = enuPositionerStatus.Ready;
                if (mHWStatus.HasFlag(enuHWStatus.Error)) mPosStatus = enuPositionerStatus.Error;
                if (mHWStatus.HasFlag(enuHWStatus.Busy)) mPosStatus |= enuPositionerStatus.Busy;
            }
        }

        public enuPositionerStatus GetAxisStatus(enuAxes _axis)
        {
            string strStatus = new String('\0', 256);
            enuPositionerStatus axisState = enuPositionerStatus.Error;

            switch (strStatus.Substring(AxisIndex(_axis), 1))
            {
                case "M":
                    // Achse ist in Bewegung (Motion)
                    axisState = enuPositionerStatus.Busy;
                    break;
                case "-":
                    axisState = enuPositionerStatus.NotInitialized;
                    break;
                case "@":
                    axisState = enuPositionerStatus.Ready;
                    break;
                case "S":
                    // Achse steht in Endschalter
                    axisState = enuPositionerStatus.Error;
                    break;
                case "F":
                    axisState = enuPositionerStatus.Error;
                    break;
                default:
                    axisState = enuPositionerStatus.Error;
                    break;
            }

            return axisState;

            /*
                int intSwitches = 0;
                ErrCheck(Positioner.GetSwitches(ref intSwitches));

                if (intSwitches > 0)
                {
                    return enuPositionerStatus.Error;
                    //todo: log
                }

                if ((intSwitches | 1024) == 1024)
                {
                    ErrCheck(Positioner.RMeasureEx(4));
                }
                else if ((intSwitches | 512) == 512)
                {
                    ErrCheck(Positioner.RMeasureEx(2));
                }
                else if ((intSwitches | 256) == 256)
                {
                    ErrCheck(Positioner.RMeasureEx(1));
                }
                else if ((intSwitches | 4) == 4)
                {
                    ErrCheck(Positioner.CalibrateEx(4));
                }
                else if ((intSwitches | 2) == 2)
                {
                    ErrCheck(Positioner.CalibrateEx(2));
                }
                else if ((intSwitches | 1) == 1)
                {
                    ErrCheck(Positioner.CalibrateEx(1));
                }
                //todo: objLogger.LogEvent(Me, "The stage was moved across the end-switch, stored positions are now invalid!", LL_Warning)

                // return mStatus; //todo:
            }
            */
        }

        public enuPositionerStatus GetAxisSpeed(enuAxes _axis, ref double _speed)
        {
            double X = 0, Y = 0, Z = 0, A = 0;
            _speed = 0;
            if (mPosStatus == enuPositionerStatus.Ready) HWStatus = LogError(Positioner.GetVel(ref X, ref Y, ref Z, ref A));
            if (mPosStatus == enuPositionerStatus.Ready) // mPosStatus was modified by HWStatus already
            {
                switch (_axis)
                {
                    case enuAxes.XAxis:
                        _speed = X;
                        break;
                    case enuAxes.YAxis:
                        _speed = Y;
                        break;
                    case enuAxes.ZAxis:
                        _speed = Z;
                        break;
                    case enuAxes.AAxis:
                        _speed = A;
                        break;
                    default:
                        mPosStatus = enuPositionerStatus.Error;
                        break;
                }
            }
            return mPosStatus;
        }

        public enuPositionerStatus ValidateAxisSpeed(enuAxes _axis, ref double _speed)
        {
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                switch (_axis)
                {
                    case enuAxes.XAxis:
                        if (_speed > Settings.X.MaxSpeed) _speed = Settings.X.MaxSpeed;
                        if (_speed <= 0) _speed = Settings.X.FailSafeSpeed;
                        break;
                    case enuAxes.YAxis:
                        if (_speed > Settings.Y.MaxSpeed) _speed = Settings.Y.MaxSpeed;
                        if (_speed <= 0) _speed = Settings.Y.FailSafeSpeed;
                        break;
                    case enuAxes.ZAxis:
                        if (_speed > Settings.Z.MaxSpeed) _speed = Settings.Z.MaxSpeed;
                        if (_speed <= 0) _speed = Settings.Z.FailSafeSpeed;
                        break;
                    case enuAxes.AAxis:
                        if (_speed > Settings.A.MaxSpeed) _speed = Settings.A.MaxSpeed;
                        if (_speed <= 0) _speed = Settings.A.FailSafeSpeed;
                        break;
                    default:
                        mPosStatus = enuPositionerStatus.Error;
                        break;
                }
            }
            return mPosStatus;
        }

        public enuPositionerStatus SetAxisSpeed(enuAxes _axis, double _speed)
        {
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                if (Settings.AutoValidateParam) if (ValidateAxisSpeed(_axis, ref _speed) != enuPositionerStatus.Ready) return mPosStatus; // mPosStatus was modified by ValidateAxisSpeed already
                HWStatus = LogError(Positioner.SetVelSingleAxis(AxisIndex(_axis), _speed));
            }
            return mPosStatus;  // mPosStatus was modified by HWStatus already
        }

        public enuPositionerStatus ValidateAxisRelativeMovement(enuAxes _axis, ref double _distance)
        { //check, if the position can be reached including distance and precision
            return mPosStatus; //todo, log error
        }

        public enuPositionerStatus SetAxisRelativePosition(enuAxes _axis, double _increment)
        {
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                if (Settings.AutoValidateParam)
                {
                    if (ValidateAxisRelativeMovement(_axis, ref _increment) != enuPositionerStatus.Ready) return mPosStatus;// mPosStatus was modified by ValidateAxisRelativeMovement already
                    double mSpeed = 0;
                    if (GetAxisSpeed(_axis, ref mSpeed) != enuPositionerStatus.Ready) return mPosStatus; // mPosStatus was modified by GetAxisSpeed already
                    if (ValidateAxisSpeed(_axis, ref mSpeed) != enuPositionerStatus.Ready) return mPosStatus;// mPosStatus was modified by ValidateAxisSpeed already
                }
                HWStatus = LogError(Positioner.MoveRelSingleAxis(AxisIndex(_axis), _increment, true));
            }
            return mPosStatus;  // mPosStatus was evtl. modified by HWStatus already
        }

        public  enuPositionerStatus GetAxisAbsolutePosition(enuAxes _axis, ref double _pos)
        {
            if (mPosStatus == enuPositionerStatus.Ready) HWStatus = LogError(Positioner.GetPosSingleAxis(AxisIndex(_axis), ref _pos));
            return mPosStatus;// mPosStatus was modified by HWStatus already 
        }

        public enuPositionerStatus ValidateAxisAbsolutePosition(enuAxes _axis, ref double _pos)
        {
            return mPosStatus;
        }

        public enuPositionerStatus SetAxisAbsolutePosition(enuAxes _axis, double _position)
        {
            if (Settings.AutoValidateParam)
            {
                if (ValidateAxisAbsolutePosition(_axis, ref _position) != enuPositionerStatus.Ready)
                {
                    return mPosStatus;
                }

                double mSpeed = 0;
                if (GetAxisSpeed(_axis, ref mSpeed) != enuPositionerStatus.Ready)
                {
                    return mPosStatus;
                }

                if (ValidateAxisSpeed(_axis, ref mSpeed) != enuPositionerStatus.Ready)
                {
                    return mPosStatus;
                }
            }

            if (mPosStatus == enuPositionerStatus.Ready)
            {
                HWStatus = LogError(Positioner.MoveAbsSingleAxis(AxisIndex(_axis), _position, true));
                if (HWStatus == enuHWStatus.Ready) mPosStatus = enuPositionerStatus.Ready;
            }
            return mPosStatus;
        }

        public enuPositionerStatus AxisStop(enuAxes _axis)
        {
            // mHWStatus = LogError(Positioner.SetAbortFlag());
            HWStatus = LogError(Positioner.StopAxes());
            return mPosStatus;
        }



        public enuPositionerStatus GetSpeeds(ref Position _speeds)
        {
            double X = 0, Y = 0, Z = 0, A = 0;
            if (mPosStatus == enuPositionerStatus.Ready) HWStatus = LogError(Positioner.GetVel(ref X, ref Y, ref Z, ref A));
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                _speeds = new Position(X, Y, Z, A);
            }
            return mPosStatus;// mPosStatus was modified by HWStatus already 
        }

        public enuPositionerStatus ValidateSpeeds(ref Position _speeds)
        { //check, if the position can be reached including distance and precision
            return mPosStatus; //todo
        }

        public enuPositionerStatus SetSpeeds(Position _speeds)
        {
            if (mPosStatus == enuPositionerStatus.Ready) HWStatus = LogError(Positioner.SetVel(_speeds.X, _speeds.Y, _speeds.Z, 0));
            return mPosStatus;
        }

        public enuPositionerStatus ValidateRelativeMovement(ref Position _pos)
        {
            return mPosStatus; //todo
        }

        public enuPositionerStatus SetRelativePosition(Position _pos)
        {
            if (GetAbsolutePosition(ref _pos) != enuPositionerStatus.Ready) return mPosStatus;
            return (SetAbsolutePosition(_pos));
        }

        public enuPositionerStatus GetAbsolutePosition(ref Position _pos)
        {
            double X = 0, Y = 0, Z = 0, A = 0;
            if (mPosStatus == enuPositionerStatus.Ready) HWStatus = LogError(Positioner.GetPos(ref X, ref Y, ref Z, ref A));
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                _pos.X = X;
                _pos.Y = Y;
                _pos.Z = Z;
                _pos.A = A;
            }
            return mPosStatus;
        }

        public enuPositionerStatus ValidateAbsolutePosition(ref Position _pos)
        {
            return mPosStatus; //todo
        }

        public enuPositionerStatus SetAbsolutePosition(Position _pos)
        {
            if (mPosStatus == enuPositionerStatus.Ready)
            {
                if (Settings.AutoValidateParam)
                {
                    if (ValidateAbsolutePosition(ref _pos) != enuPositionerStatus.Ready) return mPosStatus;// mPosStatus was modified by ValidateAbsolutePosition already
                    Position nspeeds = new Position();
                    if (GetSpeeds(ref nspeeds) != enuPositionerStatus.Ready) return mPosStatus; // mPosStatus was modified by GetSpeeds already
                    if (ValidateSpeeds(ref nspeeds) != enuPositionerStatus.Ready) return mPosStatus;// mPosStatus was modified by ValidateSpeeds already
                    if (SetSpeeds(nspeeds) != enuPositionerStatus.Ready) return mPosStatus;// mPosStatus was modified by SetSpeeds already
                }

                // if new Z-Position is higher retract z first
                double nZ = double.NaN;
                double mZ = _pos.Z; // remeber the desired z-coordinate
                if (GetAxisAbsolutePosition(enuAxes.ZAxis, ref nZ) != enuPositionerStatus.Ready) return mPosStatus; // mPosStatus was modified by GetAxisAbsolutePosition already
                if (mZ <= nZ)
                {
                    _pos.Z = nZ; //write the current Z-coordinate in the position to move
                    if (SetAbsolutePosition(_pos) != enuPositionerStatus.Ready) return mPosStatus; // move XY while not changing Z 
                    if (SetAxisAbsolutePosition(enuAxes.ZAxis, mZ) != enuPositionerStatus.Ready) return mPosStatus; //and move Z 
                }
                else
                {
                    if (SetAxisAbsolutePosition(enuAxes.ZAxis, mZ) != enuPositionerStatus.Ready) return mPosStatus; //move Z first
                    if (SetAbsolutePosition(_pos) != enuPositionerStatus.Ready) return mPosStatus; // move XY as Z is already reached
                }
            }
            return mPosStatus;
        }

        public enuPositionerStatus StopMovement()
        {
            HWStatus = LogError(Positioner.StopAxes());
            return mPosStatus;
        }

        private int AxisIndex(enuAxes _axis)
        {
            switch (_axis)
            {
                case enuAxes.XAxis:
                    return Settings.X.AxisNumber;
                case enuAxes.YAxis:
                    return Settings.Y.AxisNumber;
                case enuAxes.ZAxis:
                    return Settings.Z.AxisNumber;
                default:
                    return -1; //todo: log an error event (implement a-axis as well?)
            }
        }

        

        private enuHWStatus LogError(long _lngErrCode)
        {
            if (_lngErrCode > 4099)
            {
                _lngErrCode -= 4100;
            }
            if (_lngErrCode != 0)
            {
                mPosStatus = enuPositionerStatus.Error;
                if (ErrorList.ContainsKey(_lngErrCode))
                {
                    log.Add(ErrorList[_lngErrCode], "ERROR");
                }
                return enuHWStatus.Error;
            }
            else
            {
                mPosStatus = enuPositionerStatus.Ready;
                return enuHWStatus.Ready;
            }

            //            TCHAR InputString[255];
            //            TCHAR TranslatedString[255];
            //            // Konvertiere ASCII-Zeichenkette nach UNICODE
            //            wcscpy_s(InputString, CString(pcData, lMaxLen));
            //            LS.TranslateErrMsg(InputString, TranslatedString, 255);
            //… // Verarbeite TranslatedString
        }

        //Transducer

        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "X-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Y-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
            channels.Add(new TransducerChannel(this, "Z-Axis", "m", enuPrefix.µ, enuChannelType.mixed, enuTChannelStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Positioner;
        public List<TransducerChannel> Channels { get => channels; }

        public enuPositionerStatus GetPositionerStatus => mPosStatus ;

        public double GetValue(TransducerChannel channel)
        {
            double res = double.NaN;
            switch (channel.Name)
            {
                case "X-Axis":
                    GetAxisAbsolutePosition(enuAxes.XAxis, ref res);
                    break;
                case "Y-Axis":
                    GetAxisAbsolutePosition(enuAxes.YAxis, ref res);
                    break;
                case "Z-Axis":
                    GetAxisAbsolutePosition(enuAxes.ZAxis, ref res);
                    break;
                case "A-Axis":
                    GetAxisAbsolutePosition(enuAxes.AAxis, ref res);
                    break;
            }
            return res;
        }

        public enuTChannelStatus SetAveraging(TransducerChannel channel, int _value)
        {
            channel.Averaging = _value;
            return enuTChannelStatus.OK;
        }

        public int GetAveraging(TransducerChannel channel)
        {
            return channel.Averaging;
        }

        public double GetAveragedValue(TransducerChannel channel)
        {
            double value = 0;
            for (int i = 1; i <= channel.Averaging; i++)
            {
                value += GetValue(channel);
            }

            return value / channel.Averaging;
        }

        public enuTChannelStatus SetValue(TransducerChannel channel, double _value)
        {
            switch (channel.Name)
            {
                case "X-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.XAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "Y-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.YAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "Z-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.ZAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                case "A-Axis":
                    if (SetAxisAbsolutePosition(enuAxes.AAxis, _value) != enuPositionerStatus.Ready) return enuTChannelStatus.Error;
                    break;
                default:
                    return enuTChannelStatus.Error;
            }
            return enuTChannelStatus.OK;
        }
    }
}
