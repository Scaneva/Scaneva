#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="SMC_Gnrc_Gnrc_Settings_Axis.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Christoph Pieper)
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scaneva.Core;

namespace Scaneva.Core.Settings
{
    public class SMC_Gnrc_Gnrc_Settings_Axis : ISettings
    {
        /*
         * 
         * CONTROL-motortype=0 0 0 0 
CONTROL=
CONTROL-StrgType=9
CONTROL-AxisCount=2

     CONTROL-TRotStepAxis(x)=
CONTROL-TRotStepAxis(x)-active=1
CONTROL-TRotStepAxis(x)-Linear/Rotativ=1
CONTROL-TRotStepAxis(x)-Servo/Stepper=1
CONTROL-TRotStepAxis(x)-Phasen=2
CONTROL-TRotStepAxis(x)-Dimension=1
CONTROL-TRotStepAxis(x)-Spindelsteigung=1
CONTROL-TRotStepAxis(x)-GearFactorZaehler=1
CONTROL-TRotStepAxis(x)-GearFactorNenner=1
CONTROL-TRotStepAxis(x)-DirChange=0
CONTROL-TRotStepAxis(x)-Current reduction=50
CONTROL-TRotStepAxis(x)-Current red. delay=100
CONTROL-TRotStepAxis(x)-TRotStepMotor=
CONTROL-TRotStepAxis(x)-TRotStepMotor-NennStrom=1
CONTROL-TRotStepAxis(x)-TRotStepMotor-Felddrehsinn=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-Drehmomentkonstante=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-Traegheitsmoment=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-Polpaarzahl=50
CONTROL-TRotStepAxis(x)-TRotStepMotor-StepsPerPolePair=32768
CONTROL-TRotStepAxis(x)-TRotStepMotor-Maximaldrehzahl=30
CONTROL-TRotStepAxis(x)-TRotStepMotor-TBremse=
CONTROL-TRotStepAxis(x)-TRotStepMotor-TBremse-usage=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-TBremse-VerzoegerungEin=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-TBremse-VerzoegerungAus=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-TTemperatursensor=
CONTROL-TRotStepAxis(x)-TRotStepMotor-TTemperatursensor-active=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-TTemperatursensor-minimum resistance=0
CONTROL-TRotStepAxis(x)-TRotStepMotor-TTemperatursensor-maximum resistance=0
CONTROL-TRotStepAxis(x)-TControlers=
CONTROL-TRotStepAxis(x)-TControlers-TFeedforward=
CONTROL-TRotStepAxis(x)-TControlers-TFeedforward-speed feedforward=100
CONTROL-TRotStepAxis(x)-TControlers-TFeedforward-accel feedforward=0
CONTROL-TRotStepAxis(x)-TControlers-TFeedforward-accel feedforward out pass=0
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler=
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-active=0
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-KP=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-KI=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-Output filter time=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-Nominal speed=0
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-KI adaptive=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-KP adaptive=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-Output filter time adaptive=100
CONTROL-TRotStepAxis(x)-TControlers-TLageRegler-Adaptive speed=0
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler=
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-SpeedConType=0
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-KP=100
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-KI=100
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-KD=0
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-out filter time konst=125
CONTROL-TRotStepAxis(x)-TControlers-TSpeedRegler-Speed filter time konstant=125
CONTROL-TRotStepAxis(x)-TEncoder=
CONTROL-TRotStepAxis(x)-TEncoder-vorhanden=0
CONTROL-TRotStepAxis(x)-TEncoder-EncoderKind=0
CONTROL-TRotStepAxis(x)-TEncoder-EncoderInterface=0
CONTROL-TRotStepAxis(x)-TEncoder-Richtungsumkehr=0
CONTROL-TRotStepAxis(x)-TEncoder-Referenzmarke=0
CONTROL-TRotStepAxis(x)-TEncoder-Referenzmarkenpolaritaet=0
CONTROL-TRotStepAxis(x)-TEncoder-Polteilung=0
CONTROL-TRotStepAxis(x)-TEncoder-Polpaarzahl=1000
CONTROL-TRotStepAxis(x)-TCalibSetting=
CONTROL-TRotStepAxis(x)-TCalibSetting-SearchSpeed=5
CONTROL-TRotStepAxis(x)-TCalibSetting-BackSpeed=5
CONTROL-TRotStepAxis(x)-TCalibSetting-Accel=0.01
CONTROL-TRotStepAxis(x)-TCalibSetting-Jerk=10
CONTROL-TRotStepAxis(x)-TCalibSetting-LimitSwitchMin0=1
CONTROL-TRotStepAxis(x)-TCalibSetting-LimitSwitchMinPol=0
CONTROL-TRotStepAxis(x)-TCalibSetting-LimitSwitchMax0=1
CONTROL-TRotStepAxis(x)-TCalibSetting-LimitSwitchMaxPol=0
CONTROL-TRotStepAxis(x)-TCalibSetting-KarlOffset=0.1
CONTROL-TRotStepAxis(x)-TCalibSetting-HubOffset=0.1
CONTROL-TRotStepAxis(x)-TCalibSetting-Check limitswitches during offsetdrive=1
CONTROL-TRotStepAxis(x)-TCalibSetting-switch change=0
CONTROL-TRotStepAxis(x)-TCalibSetting-DirChange=0
CONTROL-TRotStepAxis(x)-TKinematicRestrict=
CONTROL-TRotStepAxis(x)-TKinematicRestrict-AccelJerk=10
CONTROL-TRotStepAxis(x)-TKinematicRestrict-DecelJerk=10
CONTROL-TRotStepAxis(x)-TKinematicRestrict-Accel=0.001
CONTROL-TRotStepAxis(x)-TKinematicRestrict-Decel=0.001
CONTROL-TRotStepAxis(x)-TKinematicRestrict-Speed=1
CONTROL-TRotStepAxis(x)-TKinematicRestrict-StopJerk=10
CONTROL-TRotStepAxis(x)-TKinematicRestrict-StopDecel=0.01
CONTROL-TRotStepAxis(x)-TSoftlim=
CONTROL-TRotStepAxis(x)-TSoftlim-Active=1
CONTROL-TRotStepAxis(x)-TSoftlim-TAutoRange=
CONTROL-TRotStepAxis(x)-TSoftlim-TAutoRange-TAutoRange=0
CONTROL-TRotStepAxis(x)-TSoftlim-TAutoRange-MinRange=0
CONTROL-TRotStepAxis(x)-TSoftlim-TAutoRange-maxrange=270
CONTROL-TRotStepAxis(x)-TManualControl=
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick=
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-Enabled=0
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-Richtungsumkehr=0
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-MaxSpeed=10
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-Filterzeitkonstante=500
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-Joywindow=60
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-Stromreduzierung=0
CONTROL-TRotStepAxis(x)-TManualControl-TJoystick-JoystickAchse=1
CONTROL-TRotStepAxis(x)-TManualControl-TTipp=
CONTROL-TRotStepAxis(x)-TManualControl-TTipp-Enabled=0
CONTROL-TRotStepAxis(x)-TManualControl-TTipp-Richtungsumkehr=0
CONTROL-TRotStepAxis(x)-TManualControl-TTipp-MaxSpeed=10
CONTROL-TRotStepAxis(x)-TManualControl-TTipp-Filterzeitkonstante=500
CONTROL-TRotStepAxis(x)-TManualControl-TTipp-Stromreduzierung=0
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball=
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-Enabled=0
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-Richtungsumkehr=0
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-MaxSpeed=10
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-Filterzeitkonstante=500
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-Stromreduzierung=0
CONTROL-TRotStepAxis(x)-TManualControl-TTrackball-TrackballAchse=0
CONTROL-TRotStepAxis(x)-TTVR_IN=
CONTROL-TRotStepAxis(x)-TTVR_IN-Vorhanden=0
CONTROL-TRotStepAxis(x)-TTVR_IN-Modus=0
CONTROL-TRotStepAxis(x)-TTVR_IN-Faktor=0
CONTROL-TRotStepAxis(x)-TTVR_IN-Interface=0
CONTROL-TRotStepAxis(x)-TContouringErrorSupervision=
CONTROL-TRotStepAxis(x)-TContouringErrorSupervision-Active=0
CONTROL-TRotStepAxis(x)-TContouringErrorSupervision-Range=0
CONTROL-TRotStepAxis(x)-TContouringErrorSupervision-ViolationTime=0
CONTROL-TRotStepAxis(x)-TPosWindowSupervision=
CONTROL-TRotStepAxis(x)-TPosWindowSupervision-Active=0
CONTROL-TRotStepAxis(x)-TPosWindowSupervision-Range=0
CONTROL-TRotStepAxis(x)-TPosWindowSupervision-Reached Time=0
CONTROL-TRotStepAxis(x)-TPosWindowSupervision-Reached Timeout=0
CONTROL-TRotStepAxis(x)-TModulo=
CONTROL-TRotStepAxis(x)-TModulo-active=0
CONTROL-TRotStepAxis(x)-TModulo-modulo mode=0

        CONTROL-TSnapShot=
CONTROL-TSnapShot-Mode=0
CONTROL-TSnapShot-Deadtime=10
CONTROL-TSnapShot-Polarity=0
CONTROL-TSnapShot-TSnapShotPosSource=
CONTROL-TSnapShot-TSnapShotPosSource-Source x=0
CONTROL-TSnapShot-TSnapShotPosSource-Source y=0
CONTROL-TSnapShot-TSnapShotPosSource-Source z=0
CONTROL-TSnapShot-TSnapShotPosSource-Source a=0
CONTROL-TSnapShot-Active=0
CONTROL-TTrigger=
CONTROL-TTrigger-Activate=0
CONTROL-TTrigger-axis=0
CONTROL-TTrigger-signalLength=3
CONTROL-TTrigger-Triggersource=0
CONTROL-TTrigger-TrigDistance=1
CONTROL-TTrigger-TriggerHysterese=0.0001
CONTROL-TTrigger-Polarity=0
CONTROL-TTrigger-Direction=0
CONTROL-TTrigger-SpecialFunct=0
CONTROL-TTrigger-Trigmode=0
CONTROL-TTriggerTwo=
CONTROL-TTriggerTwo-Activate=0
CONTROL-TTriggerTwo-axis=0
CONTROL-TTriggerTwo-signalLength=3
CONTROL-TTriggerTwo-Triggersource=0
CONTROL-TTriggerTwo-TrigDistance=1
CONTROL-TTriggerTwo-TriggerHysterese=0.0001
CONTROL-TTriggerTwo-Polarity=0
CONTROL-TTriggerTwo-Direction=0
CONTROL-TTriggerTwo-SpecialFunct=0
CONTROL-TTriggerTwo-Trigmode=0
CONTROL-TDigOut=
CONTROL-TDigOut-TDigOutSignal=
CONTROL-TDigOut-TDigOutSignal-Ausgang 0=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 1=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 2=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 3=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 4=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 5=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 6=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 7=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 8=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 9=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 10=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 11=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 12=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 13=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 14=0
CONTROL-TDigOut-TDigOutSignal-Ausgang 15=0
CONTROL-TDigOut-TInvertDigOutSignals=
CONTROL-TDigOut-TInvertDigOutSignals-Stillstandsmeldung=0
CONTROL-TDigOut-TInvertDigOutSignals-Stopp-Aktiv-Meldung=0
CONTROL-TDigOut-TInvertDigOutSignals-Zielfenstermeldung=0
CONTROL-TDigOut-TInvertDigOutSignals-Mindestens eine Endstufe eingeschaltet=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 0 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 1 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 2 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 3 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 4 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 5 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 6 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 7 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 8 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 9 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 10 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 11 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 12 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 13 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 14 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 15 (24V)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 0 (TTL-Pegel)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 1 (TTL-Pegel)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 2 (TTL-Pegel)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 3 (TTL-Pegel)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 4 (TTL-Pegel)=0
CONTROL-TDigOut-TInvertDigOutSignals-Digitaler Eingang 5 (TTL-Pegel)=0
CONTROL-room kinematics=
CONTROL-room kinematics-AccelJerk=10
CONTROL-room kinematics-DecelJerk=10
CONTROL-room kinematics-Accel=10
CONTROL-room kinematics-Decel=1
CONTROL-room kinematics-Speed=1
CONTROL-room kinematics-StopJerk=10
CONTROL-room kinematics-StopDecel=1
CONTROL-THWConfig=
CONTROL-THWConfig-QEP-Konfiguration=4
CONTROL-THWConfig-Stoppol=0
CONTROL-THWConfig-ManModePreSelection=0
CONTROL-THWConfig-TAxisMap=
CONTROL-THWConfig-TAxisMap-Axismap x=1
CONTROL-THWConfig-TAxisMap-Axismap y=2
CONTROL-THWConfig-TAxisMap-Axismap z=3
CONTROL-THWConfig-TAxisMap-Axismap a=4
Kind-Of-Entry=TLS_ControlTreePCIe



*/
          
        private int intAxisNumber = 0; //(typ. 0=x, 1=y, 2=z)
        private long lngPosition = 0; //current axis position in µm
        private double dblMaxSpeed = 5000; //maximal speed of the stage in µm/s
        private bool blnSign = true; //direction of the axis (defaults: true = right, forward (away from me) and up for X, Y and Z respectively
        private double dblTravel = 20000; //max. travel distance in µm
        private bool blnSwitches = false; //end switches enabled?
        private double dblPitch = 1; //distance in mm per revolution
        private long lngFullSteps = 200; //full steps pro revolution

        [DisplayName("Axis Number [#]")]
        [Description("Axis Number (typ. 0=x, 1=y, 2=z)")]
        public int AxisNumber { get => intAxisNumber; set => intAxisNumber = value; }

        [DisplayName("Current position [µm]")]
        [Description("Actual axis position relative to 'Home' position in µm")]
        public long Position { get => lngPosition; set => lngPosition = value; }

        [DisplayName("Maximal speed [µm/s]")]
        [Description("Maximal allowable speed for the mechanics in µm/s")]
        public double MaxSpeed { get => dblMaxSpeed; set => dblMaxSpeed = value; }

        [DisplayName("Axis direction [#]")]
        [Description("Axis direction with 1 = direct / 0 = inverted")]
        public bool Sign { get => blnSign; set => blnSign = value; }

        [DisplayName("Axis travel distance [mm]")]
        [Description("Maximium absolute axis travel distance in mm")]
        public double Travel { get => dblTravel; set => dblTravel = value; }

        [DisplayName("End switch available? [#]")]
        [Description("End switch availability with 1 = yes / 0 = no")]
        public bool Switches { get => blnSwitches; set => blnSwitches = value; }

        [DisplayName("Pitch [mm]")]
        [Description("Distance traveled in mm per revolution")]
        public double Pitch { get => dblPitch; set => dblPitch = value; }

        [DisplayName("Full steps per revolution [#]")]
        [Description("Full steps per revolution as an integer number")]
        public long FullSteps { get => lngFullSteps; set => lngFullSteps = value; }

        public override string ToString()
        {
            return "Axis " + intAxisNumber;
        }
    }
}
