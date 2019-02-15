#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Sensor_Temperature_BplusB_TLOG20.cs" company="Scaneva">
// 
//  Copyright (C) 2018 Roche Diabetes Care GmbH (Kirill Sliozberg, Christoph Pieper)
//                                                    *'
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
//  VB.NET-Module for GSV-Functions in MEGSV.DLL Version 1.40 Copyright (C) Dr. Holger Kabelitz 1999-2006
//  was adapted and integrated in the current module.                                           
//  Dr. Holger Kabelitz                          
//  D-13507 Berlin                               
//  Germany
// --------------------------------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Scaneva.Core.Settings;
using Scaneva.Tools;

namespace Scaneva.Core.Hardware
{
    [DisplayName("GSV force sensor")]
    [Category("Sensors")]
    class Sensor_Force_ME_GSV3 : ParametrizableObject, IHWManager, ITransducer
    {

        public const long GSV_OK = 0;
        public const long GSV_ERROR = -1;
        public const long GSV_TRUE = 1;
        public const byte GSV_MEMORY_OK = 0x0; //GSV_OK
        public const byte GSV_MEMORY_OK_FUNCTION = 0x1; //&H1
                                                        /* public const byte GSV_MEMORY_ERROR_ABORT = //&HFF;
                                                         public const byte GSV_MEMORY_ERROR_SEVERE = //&H10;
                                                         public const byte GSV_MEMORY_ERROR_NA = //&H11;
                                                         public const byte GSV_MEMORY_ERROR_DATA = //&H12;
                                                         public const long GSV_ACTEX_FLAG_BAUDRATE = // &H1&;
                                                         */

        public struct GSV_ACTIVATE_EXTENDED
        {
            long actex_size;    //Größe der Datenstruktur, gesetzt durch die GSV_ACTEX_SIZE Konstante(bzw.Funktion).
            long actex_buffersize; //Anzahl der maximal zwischenzuspeichernden Meßdatenwerte.
            long actex_flags; //Kombination von GSV_ACTEX_FLAG_xxx Konstanten, die festlegt, welche weiteren Felder verwendet werden.
            long actex_baudrate; //(Optional). Gewünschte Baudrate für die serielle Schnittstelle(nicht verwendet = gleiche Einstellung wie bei GSVactivate).
        }

        //string dllPath = "C:\GitHub\Scaneva\Dependencies\MELibraries\MEGSV.dll";
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVversion(); // Alias "#1" 
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVmodel(int no); // Alias "#2"
                                                   // [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
                                                   // public static extern int MEreservedExec(int no, int code, int rexpro, any arg); //Alias "#3"  ByRef arg As Any)
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int MErequestEvent(int no); //Alias "#4"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVgetLocalBaudRate(int no); //Alias "#8"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVactivateExtended(int no, GSV_ACTIVATE_EXTENDED actex); //Alias "#9"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVactivate(int no, int buffersize); //Alias "#10"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern void GSVrelease(int no); //Alias "#11"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVreceived(int no); //Alias "#20"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVreadMultiple(int no, double ad, int count, int valsread); //Alias "#22"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern double GSVgetFreq(int no); //Alias "#51"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVsetModeAverage(int no, int avg); //Alias "#58"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVDispSetUnit(int no, int dispunit); //Alias "#115"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVDispSetNorm(int no, double norm); //Alias "#116"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern int GSVsetFreq(int no, double freq); //Alias "#118"
        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi)]
        public static extern double GSVDispGetNorm(int no); //Alias "#126"

        /*
                                                                              
                                                                          public static extern GSVinitialize Lib "MEGSV" Alias "#12" (ByVal no As Integer) As Integer
                                                                          public static extern GSVflushBuffer Lib "MEGSV" Alias "#13" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetValues Lib "MEGSV" Alias "#17" (ByRef no As Integer, ByVal n As Integer) As Integer
                                                                          public static extern MEsendBytesToDevices Lib "MEGSV" Alias "#18" (ByRef no As Integer, ByVal n As Integer, ByRef pbuf As Byte, ByVal count As Integer) As Integer
                                                                          public static extern MEsendBytes Lib "MEGSV" Alias "#19" (ByVal no As Integer, ByRef pbuf As Byte, ByVal count As Integer) As Integer
                                                                         
                                                                          public static extern GSVread Lib "MEGSV" Alias "#21" (ByVal no As Integer, ByRef ad As Double) As Integer
                                                                          
                                                                          'public static extern GSVcopyMemory Lib "MEGSV" Alias "#23" (ByVal no As Integer, ByVal dc As Integer, ByVal addr As Integer, ByRef buffer As Any) As Integer
                                                                          public static extern GSVcheckMemory Lib "MEGSV" Alias "#24" (ByVal no As Integer, ByVal dc As Integer, ByVal addr As Integer, ByRef buffer As Byte) As Integer
                                                                          public static extern GSVreadStatus Lib "MEGSV" Alias "#25" (ByVal no As Integer, ByRef ad As Double, ByRef ps As Byte) As Integer
                                                                          public static extern GSVreadStatusMultiple Lib "MEGSV" Alias "#26" (ByVal no As Integer, ByRef ad As Double, ByRef ps As Byte, ByVal count As Integer, ByRef valsread As Integer) As Integer
                                                                          'public static extern MEasynchronousSendBytes Lib "MEGSV" Alias "#28" (ByVal no As Integer, ByRef pbuf As Byte, ByVal count As Integer, ByRef handle As Any) As Integer
                                                                          public static extern MEsynchronizeSendBytes Lib "MEGSV" Alias "#29" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOptionsCode Lib "MEGSV" Alias "#30" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOptionsExtension3 Lib "MEGSV" Alias "#31" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOptionsLinear Lib "MEGSV" Alias "#32" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOptionsExtension21 Lib "MEGSV" Alias "#33" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOptionsSleepMode Lib "MEGSV" Alias "#34" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisBipol Lib "MEGSV" Alias "#50" (ByVal no As Integer) As Integer
                                                                          
                                                                          public static extern GSVgetGain Lib "MEGSV" Alias "#52" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetChannel Lib "MEGSV" Alias "#53" (ByVal no As Integer) As Integer
                                                                          public static extern MEsetModeLock Lib "MEGSV" Alias "#54" (ByVal no As Integer, ByVal slock As Integer, ByVal id As String) As Integer
                                                                          public static extern GSVgetModeLock Lib "MEGSV" Alias "#55" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeLinear Lib "MEGSV" Alias "#56" (ByVal no As Integer, ByVal lin As Integer) As Integer
                                                                          public static extern GSVgetModeLinear Lib "MEGSV" Alias "#57" (ByVal no As Integer) As Integer
                                                                         
                                                                          public static extern GSVgetModeAverage Lib "MEGSV" Alias "#59" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeSI Lib "MEGSV" Alias "#60" (ByVal no As Integer, ByVal msi As Integer) As Integer
                                                                          public static extern GSVgetModeSI Lib "MEGSV" Alias "#61" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeText Lib "MEGSV" Alias "#62" (ByVal no As Integer, ByVal mt As Integer) As Integer
                                                                          public static extern GSVgetModeText Lib "MEGSV" Alias "#63" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeMax Lib "MEGSV" Alias "#64" (ByVal no As Integer, ByVal mx As Integer) As Integer
                                                                          public static extern GSVgetModeMax Lib "MEGSV" Alias "#65" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeLog Lib "MEGSV" Alias "#66" (ByVal no As Integer, ByVal lg As Integer) As Integer
                                                                          public static extern GSVgetModeLog Lib "MEGSV" Alias "#67" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetModeWindow Lib "MEGSV" Alias "#68" (ByVal no As Integer, ByVal win As Integer) As Integer
                                                                          public static extern GSVgetModeWindow Lib "MEGSV" Alias "#69" (ByVal no As Integer) As Integer
                                                                          public static extern GSVhasLCD Lib "MEGSV" Alias "#70" (ByVal no As Integer) As Integer
                                                                          public static extern GSVhasADC Lib "MEGSV" Alias "#71" (ByVal no As Integer) As Integer
                                                                          public static extern GSVhasUII Lib "MEGSV" Alias "#72" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisSI Lib "MEGSV" Alias "#73" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisWL Lib "MEGSV" Alias "#74" (ByVal no As Integer) As Integer
                                                                          public static extern GSVhasAF Lib "MEGSV" Alias "#75" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetBridgeType Lib "MEGSV" Alias "#80" (ByVal no As Integer, ByVal bt As Integer) As Integer
                                                                          public static extern GSVgetBridgeType Lib "MEGSV" Alias "#81" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetBridgeInternal Lib "MEGSV" Alias "#84" (ByVal no As Integer, ByVal bi As Integer) As Integer
                                                                          public static extern GSVgetBridgeInternal Lib "MEGSV" Alias "#85" (ByVal no As Integer) As Integer
                                                                          public static extern GSVresetStatus Lib "MEGSV" Alias "#100" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetScale Lib "MEGSV" Alias "#101" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetZero Lib "MEGSV" Alias "#102" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetControl Lib "MEGSV" Alias "#103" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetOffset Lib "MEGSV" Alias "#104" (ByVal no As Integer) As Integer
                                                                          public static extern GSVwriteScale Lib "MEGSV" Alias "#105" (ByVal no As Integer, ByVal scalev As Integer) As Integer
                                                                          public static extern GSVwriteZero Lib "MEGSV" Alias "#106" (ByVal no As Integer, ByVal zero As Integer) As Integer
                                                                          public static extern GSVwriteControl Lib "MEGSV" Alias "#107" (ByVal no As Integer, ByVal control As Integer) As Integer
                                                                          public static extern GSVwriteOffset Lib "MEGSV" Alias "#108" (ByVal no As Integer, ByVal offset As Integer) As Integer
                                                                          public static extern GSVgetAll Lib "MEGSV" Alias "#109" (ByVal no As Integer, ByVal pos As Integer) As Integer
                                                                          public static extern GSVsaveAll Lib "MEGSV" Alias "#110" (ByVal no As Integer, ByVal pos As Integer) As Integer
                                                                          public static extern GSVsetCal Lib "MEGSV" Alias "#111" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetZero Lib "MEGSV" Alias "#112" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetScale Lib "MEGSV" Alias "#113" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetOffset Lib "MEGSV" Alias "#114" (ByVal no As Integer) As Integer
                                                                         
                                                                         
                                                                          public static extern GSVDispSetDPoint Lib "MEGSV" Alias "#117" (ByVal no As Integer, ByVal dpoint As Integer) As Integer
                                                                         
                                                                          public static extern GSVsetGain Lib "MEGSV" Alias "#119" (ByVal no As Integer, ByVal gain As Integer) As Integer
                                                                          public static extern GSVsetBipolar Lib "MEGSV" Alias "#120" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetUnipolar Lib "MEGSV" Alias "#121" (ByVal no As Integer) As Integer
                                                                          public static extern MEsetCal Lib "MEGSV" Alias "#123" (ByVal no As Integer, ByVal cal As Double) As Integer
                                                                          public static extern MEgetCal Lib "MEGSV" Alias "#124" (ByVal no As Integer) As Double
                                                                          public static extern MEsendID Lib "MEGSV" Alias "#125" (ByVal no As Integer, ByVal id As String) As Integer
                                                                         
                                                                          public static extern GSVDispGetUnit Lib "MEGSV" Alias "#127" (ByVal no As Integer) As Integer
                                                                          public static extern GSVDispGetDPoint Lib "MEGSV" Alias "#128" (ByVal no As Integer) As Integer
                                                                          public static extern GSVswitch Lib "MEGSV" Alias "#129" (ByVal no As Integer, ByVal swon As Integer) As Integer
                                                                          public static extern MEwriteSerialNo Lib "MEGSV" Alias "#130" (ByVal no As Integer, ByVal number As String) As Integer
                                                                          Private Declare Function MEreadSerialNo Lib "MEGSV" Alias "#131" (ByVal no As Integer, ByVal number As String) As Integer
                                                                          public static extern GSVsetThreshold Lib "MEGSV" Alias "#132" (ByVal no As Integer, ByVal thon As Double, ByVal thoff As Double) As Integer
                                                                          public static extern GSVgetThreshold Lib "MEGSV" Alias "#133" (ByVal no As Integer, ByRef thon As Double, ByRef thoff As Double) As Integer
                                                                          public static extern GSVsetChannel Lib "MEGSV" Alias "#134" (ByVal no As Integer, ByVal channel As Integer) As Integer
                                                                          public static extern GSVstopTransmit Lib "MEGSV" Alias "#135" (ByVal no As Integer) As Integer
                                                                          public static extern GSVstartTransmit Lib "MEGSV" Alias "#136" (ByVal no As Integer) As Integer
                                                                          public static extern GSVclearBuffer Lib "MEGSV" Alias "#137" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetMode Lib "MEGSV" Alias "#138" (ByVal no As Integer, ByVal mode As Integer) As Integer
                                                                          public static extern GSVgetMode Lib "MEGSV" Alias "#139" (ByVal no As Integer) As Integer
                                                                          public static extern MEwriteEquipment Lib "MEGSV" Alias "#140" (ByVal no As Integer, ByVal equip As Integer) As Integer
                                                                          public static extern GSVgetEquipment Lib "MEGSV" Alias "#141" (ByVal no As Integer) As Integer
                                                                          public static extern GSVfirmwareVersion Lib "MEGSV" Alias "#143" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetGageFactor Lib "MEGSV" Alias "#144" (ByVal no As Integer, ByVal gf As Double) As Integer
                                                                          public static extern GSVgetGageFactor Lib "MEGSV" Alias "#145" (ByVal no As Integer) As Double
                                                                          public static extern GSVsetPoisson Lib "MEGSV" Alias "#146" (ByVal no As Integer, ByVal poiss As Double) As Integer
                                                                          public static extern GSVgetPoisson Lib "MEGSV" Alias "#147" (ByVal no As Integer) As Double
                                                                          public static extern GSVsetBridge Lib "MEGSV" Alias "#148" (ByVal no As Integer, ByVal br As Integer) As Integer
                                                                          public static extern GSVgetBridge Lib "MEGSV" Alias "#149" (ByVal no As Integer) As Integer
                                                                          public static extern MEwriteRange Lib "MEGSV" Alias "#150" (ByVal no As Integer, ByVal range As Double) As Integer
                                                                          public static extern GSVgetRange Lib "MEGSV" Alias "#151" (ByVal no As Integer) As Double
                                                                          public static extern MEsetOffsetWait Lib "MEGSV" Alias "#152" (ByVal no As Integer, ByVal ow As Double) As Integer
                                                                          public static extern MEgetOffsetWait Lib "MEGSV" Alias "#153" (ByVal no As Integer) As Double
                                                                          public static extern GSVgetOptions Lib "MEGSV" Alias "#154" (ByVal no As Integer) As Integer
                                                                          public static extern MEwriteOptions Lib "MEGSV" Alias "#155" (ByVal no As Integer, ByVal options As Integer) As Integer
                                                                          'public static extern GSVreadMemory Lib "MEGSV" Alias "#156" (ByVal no As Integer, ByVal addr As Integer, ByRef buffer As Any, ByRef status As Any) As Integer
                                                                          'public static extern GSVwriteMemory Lib "MEGSV" Alias "#157" (ByVal no As Integer, ByVal addr As Integer, ByVal check As Integer, ByVal value As Integer, ByRef status As Any) As Integer
                                                                          public static extern GSVgetMemoryWait Lib "MEGSV" Alias "#158" (ByVal no As Integer) As Double
                                                                          public static extern GSVgetValue Lib "MEGSV" Alias "#159" (ByVal no As Integer) As Integer
                                                                          public static extern GSVclearMaxValue Lib "MEGSV" Alias "#160" (ByVal no As Integer) As Integer
                                                                          public static extern GSVDispSetDigits Lib "MEGSV" Alias "#161" (ByVal no As Integer, ByVal digits As Integer) As Integer
                                                                          public static extern GSVDispGetDigits Lib "MEGSV" Alias "#162" (ByVal no As Integer) As Integer
                                                                          public static extern GSVunlockUII Lib "MEGSV" Alias "#163" (ByVal no As Integer) As Integer
                                                                          public static extern GSVlockUII Lib "MEGSV" Alias "#164" (ByVal no As Integer) As Integer
                                                                          public static extern GSVgetLastError Lib "MEGSV" Alias "#166" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSecondThreshold Lib "MEGSV" Alias "#167" (ByVal no As Integer, ByVal thon As Double, ByVal thoff As Double) As Integer
                                                                          public static extern GSVgetSecondThreshold Lib "MEGSV" Alias "#168" (ByVal no As Integer, ByRef thon As Double, ByRef thoff As Double) As Integer
                                                                          public static extern GSVgetDeviceType Lib "MEGSV" Alias "#169" (ByVal no As Integer) As Integer
                                                                          public static extern GSVDispCalcNorm Lib "MEGSV" Alias "#170" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetTxMode Lib "MEGSV" Alias "#228" (ByVal no As Integer, ByVal txmode As Integer) As Integer
                                                                          public static extern GSVgetTxMode Lib "MEGSV" Alias "#229" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetBaud Lib "MEGSV" Alias "#230" (ByVal no As Integer, ByVal baud As Integer) As Integer
                                                                          public static extern GSVgetBaud Lib "MEGSV" Alias "#231" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSlowRate Lib "MEGSV" Alias "#234" (ByVal no As Integer, ByVal secs As Integer) As Integer
                                                                          public static extern GSVgetSlowRate Lib "MEGSV" Alias "#235" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialMode Lib "MEGSV" Alias "#236" (ByVal no As Integer, ByVal smode As Integer) As Integer
                                                                          public static extern GSVgetSpecialMode Lib "MEGSV" Alias "#237" (ByVal no As Integer) As Integer
                                                                          public static extern GSVwriteSamplingRate Lib "MEGSV" Alias "#238" (ByVal no As Integer, ByVal freq As Double, ByVal factor As Integer) As Integer
                                                                          public static extern GSVreadSamplingRate Lib "MEGSV" Alias "#239" (ByVal no As Integer, ByRef freq As Double, ByRef factor As Integer) As Integer
                                                                          public static extern GSVsetCanSetting Lib "MEGSV" Alias "#240" (ByVal no As Integer, ByVal stype As Integer, ByVal val As Integer) As Integer
                                                                          public static extern GSVgetCanSetting Lib "MEGSV" Alias "#241" (ByVal no As Integer, ByVal stype As Integer) As Integer
                                                                          public static extern GSVsetAnalogFilter Lib "MEGSV" Alias "#244" (ByVal no As Integer, ByVal freq As Double) As Integer
                                                                          public static extern GSVgetAnalogFilter Lib "MEGSV" Alias "#245" (ByVal no As Integer) As Double
                                                                          public static extern GSVgetTxModeConfig Lib "MEGSV" Alias "#357" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetTxModeTransmit4 Lib "MEGSV" Alias "#358" (ByVal no As Integer, ByVal t4 As Integer) As Integer
                                                                          public static extern GSVgetTxModeTransmit4 Lib "MEGSV" Alias "#359" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetTxModeRepeat3 Lib "MEGSV" Alias "#360" (ByVal no As Integer, ByVal r3 As Integer) As Integer
                                                                          public static extern GSVgetTxModeRepeat3 Lib "MEGSV" Alias "#361" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetTxModeTransmit5 Lib "MEGSV" Alias "#362" (ByVal no As Integer, ByVal t5 As Integer) As Integer
                                                                          public static extern GSVgetTxModeTransmit5 Lib "MEGSV" Alias "#363" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetTxModeReadOnly Lib "MEGSV" Alias "#364" (ByVal no As Integer, ByVal ro As Integer) As Integer
                                                                          public static extern GSVgetTxModeReadOnly Lib "MEGSV" Alias "#365" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeSlow Lib "MEGSV" Alias "#372" (ByVal no As Integer, ByVal slow As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeSlow Lib "MEGSV" Alias "#373" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeAverage Lib "MEGSV" Alias "#374" (ByVal no As Integer, ByVal avg As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeAverage Lib "MEGSV" Alias "#375" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeFilter Lib "MEGSV" Alias "#376" (ByVal no As Integer, ByVal flt As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeFilter Lib "MEGSV" Alias "#377" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeMax Lib "MEGSV" Alias "#378" (ByVal no As Integer, ByVal mx As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeMax Lib "MEGSV" Alias "#379" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeFilterAuto Lib "MEGSV" Alias "#380" (ByVal no As Integer, ByVal fltauto As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeFilterAuto Lib "MEGSV" Alias "#381" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeFilterOrder5 Lib "MEGSV" Alias "#382" (ByVal no As Integer, ByVal fltord5 As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeFilterOrder5 Lib "MEGSV" Alias "#383" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetSpecialModeSleep Lib "MEGSV" Alias "#384" (ByVal no As Integer, ByVal sleep As Integer) As Integer
                                                                          public static extern GSVgetSpecialModeSleep Lib "MEGSV" Alias "#385" (ByVal no As Integer) As Integer
                                                                          public static extern GSVreadSamplingFrequency Lib "MEGSV" Alias "#404" (ByVal no As Integer) As Double
                                                                          public static extern GSVreadSamplingFactor Lib "MEGSV" Alias "#405" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetBaudRate Lib "MEGSV" Alias "#406" (ByVal no As Integer, ByVal baud As Integer) As Integer
                                                                          public static extern GSVgetBaudRate Lib "MEGSV" Alias "#407" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetCanBaudRate Lib "MEGSV" Alias "#408" (ByVal no As Integer, ByVal baud As Integer) As Integer
                                                                          public static extern GSVgetCanBaudRate Lib "MEGSV" Alias "#409" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisSamplingFrequencyProtocolLinear Lib "MEGSV" Alias "#410" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisSamplingFactorProtocolLinear Lib "MEGSV" Alias "#411" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetCanID Lib "MEGSV" Alias "#412" (ByVal no As Integer, ByVal idtype As Integer, ByVal id As Integer) As Integer
                                                                          public static extern GSVgetCanID Lib "MEGSV" Alias "#413" (ByVal no As Integer, ByVal idtype As Integer) As Integer
                                                                          public static extern GSVsetCanBaud Lib "MEGSV" Alias "#414" (ByVal no As Integer, ByVal baud As Integer) As Integer
                                                                          public static extern GSVgetCanBaud Lib "MEGSV" Alias "#415" (ByVal no As Integer) As Integer
                                                                          public static extern GSVisCanAvailable Lib "MEGSV" Alias "#417" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetCanActive Lib "MEGSV" Alias "#418" (ByVal no As Integer, ByVal active As Integer) As Integer
                                                                          public static extern GSVgetCanActive Lib "MEGSV" Alias "#419" (ByVal no As Integer) As Integer
                                                                          public static extern GSVsetCanMode20B Lib "MEGSV" Alias "#420" (ByVal no As Integer, ByVal mode20b As Integer) As Integer
                                                                          public static extern int GSVgetCanMode20B Lib "MEGSV" Alias "#421" (ByVal no As Integer) As Integer
                                                                          'Public Function GSV_ACTEX_SIZE() As Integer
                                                                                  */
        //status
        enuHWStatus hwStatus = enuHWStatus.NotInitialized;
        private int mintModel;
        private long mlngSerialNo;
        private double m_dblNorm;

        //transducer stuff
        public List<TransducerChannel> channels = new List<TransducerChannel>();

        //IHWCompo
        public bool IsEnabled { get; set; }

        public enuHWStatus Connect()
        {
            hwStatus = enuHWStatus.NotInitialized;
            /*
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM" + Convert.ToString(Settings.COMPort);
            _serialPort.BaudRate = Settings.Baudrate;
            _serialPort.Parity = Settings.Parity;
            _serialPort.DataBits = Settings.Databits;
            _serialPort.StopBits = Settings.StopBits;
            //todo auto eacho parameter
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();

            if (true) //todo: check RS232 connection status
            {
                InitTransducerChannels();
                hwStatus = enuHWStatus.Ready;
            }
            else
            {
                log.Add("Could not initiate sensor!", "Error");
            }
            
            Dim tmpstr    As String
    Dim freq      As Double
    Dim newfreq   As Double
    Dim factor    As Integer
    Dim newfactor As Integer
    */
            if (GSVactivate(Settings.COMPort, Settings.Buffersize) == GSV_OK)
            {
                log.Add("GSV-3 measurement amplifier (Port: " + Settings.COMPort + ") activated");
            }


            /*
            mintModel = GSVmodel(mintPort)
            string tmpstr = "";
            if (GSVgetSerialNo(Settings.COMPort, tmpstr) == GSV_OK)
            {
                mlngSerialNo = Convert.ToInt16(tmpstr);
 }         
                    */
            GSVsetFreq(Settings.COMPort, 25);
            GSVsetModeAverage(Settings.COMPort, 1);
            double freq = GSVgetFreq(Settings.COMPort);  //Holt den Frequenzwert von Port no und speichert das Ergebnis in der Variable Freq
            GSVDispSetUnit(Settings.COMPort, 3);
            GSVDispSetNorm(Settings.COMPort, 4000 / 0.4999);
            double m_dblNorm = GSVDispGetNorm(Settings.COMPort);    //Dieser Funktionsaufruf ermittelt die Normierung und speichert ihn in der Variable Norm
            return hwStatus;
        }

        public enuHWStatus Initialize()
        {
            return enuHWStatus.Ready;
        }

        public enuHWStatus HWStatus => hwStatus;

        public void Release()
        {
            GSVrelease(Settings.COMPort);
        }

        public Sensor_Force_ME_GSV3(LogHelper log) : base(log)
        {
            settings = new Sensor_Force_ME_GSV3_Settings();
            log.Add("Initializing ME GSV force sensor");
        }

        public Sensor_Force_ME_GSV3_Settings Settings
        {
            get
            {
                return (Sensor_Force_ME_GSV3_Settings)settings;
            }
            set
            {
                settings = value;
            }
        }

        //Transducer
        private void InitTransducerChannels()
        {
            channels = new List<TransducerChannel>();
            channels.Add(new TransducerChannel(this, "Force", "N", enuPrefix.none, enuChannelType.passive, enuTChannelStatus.OK));
        }

        public enuTransducerType TransducerType => enuTransducerType.Sensor;

        public List<TransducerChannel> Channels { get => channels; }

        public double GetValue(TransducerChannel channel)
        {

            switch (channel.Name)
            {
                case "Force":
                    double[] vals = new double[Settings.Buffersize];
                    int TimeOut = 500; //0.5s max timeout
                                       //if we have no values _at all_ in the buffer, we wait a bit
                    do
                    {
                        Thread.Sleep(100); // todo: optimize with multithread! Avoid software freezing
                        TimeOut = TimeOut - 100;
                    } while ((GSVreceived(Settings.COMPort) != GSV_TRUE) && (TimeOut > 0));

                    //then we process all the data we have now - maybe only 1 value...
                    int valsread = 0;
                    if (GSVreadMultiple(Settings.COMPort, vals[0], vals.Length, valsread) == GSV_TRUE)
                    {
                        return 0;
                    }
                    return 0;
                /*
            }
                                    For i = 0 To valsread -1
            res = res + vals(i)
            Next i

            res = res / valsread
            res = res / valsread
            clsHWTransducer_Value = res * m_dblNorm
            Else
            ' uh, no values. :-(
            clsHWTransducer_Value = 0
            End If
            */
                default:
                    return 0;
            }
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
                value = +GetValue(channel);
            }

            return value / channel.Averaging;

            //todo: make internal avaraging
        }

        public enuTChannelStatus SetValue(TransducerChannel channel, double _value)
        {
            return enuTChannelStatus.OK;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void ParameterChanged(string name)
        {
            base.ParameterChanged(name);
        }

        public override void SerializeParameterValues(XmlWriter writer)
        {
            base.SerializeParameterValues(writer);
        }

        public override void DeserializeParameterValues(IXPathNavigable node, Dictionary<string, Type> availableExperiments)
        {
            base.DeserializeParameterValues(node, availableExperiments);
        }
    }
}
/*
private long GSVgetSerialNo(long no, string number)
{
    string sernum = String$(10, vbNullChar);
    long rcode = MEreadSerialNo(no, sernum);
    long cnum = InStr(1, sernum, vbNullChar);
if (rcode == GSV_OK)
    {
        if (cnum > 0)
        {
            sernum = Left$(sernum, cnum - 1);
        }
        number = sernum
    }
    return rcode;
}
*/

/*




Public Property Get SerialNumber() As Long    'read only
Attribute SerialNumber.VB_UserMemId = 1745027072
    SerialNumber = mlngSerialNo
End Property




Public Function DebugInfo() As String
Attribute DebugInfo.VB_UserMemId = 1610809361
    DebugInfo = clsHWCompo_Name() & "(" & clsHWCompo_CName() & ")" & vbNewLine & "Firmware: " & CStr(GSVfirmwareVersion(mintPort)) & vbNewLine
End Function



Public Sub LoadFactoryDefaults()
Attribute LoadFactoryDefaults.VB_UserMemId = 1610809359
    Call GSVgetAll(mintPort, 1)
End Sub

Public Sub PerformInternalCalibration()
Attribute PerformInternalCalibration.VB_UserMemId = 1610809360
Dim res As Integer

    res = GSVsetCal(mintPort)
    Call Sleep(100)
    res = res Or GSVsetZero(mintPort)
    Call Sleep(150)
    res = res Or GSVsetOffset(mintPort)
    Call Sleep(MEgetOffsetWait(mintPort) * 3 * 100)
    res = res Or GSVclearBuffer(mintPort)

    If (res <> GSV_TRUE) Then
        Call objLogger.LogEvent(Me, clsHWCompo_Name & ": Error during internal calibration.", ll_error)
    End If

End Sub
*/
