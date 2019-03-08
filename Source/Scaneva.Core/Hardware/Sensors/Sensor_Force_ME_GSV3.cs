#region Copyright (C)
// ---------------------------------------------------------------------------------------------------------------
//  <copyright file="Sensor_Force_ME_GSV3.cs" company="Scaneva">
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
using System.Text;
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

        public const int GSV_OK = 0;
        public const int GSV_ERROR = -1;
        public const int GSV_TRUE = 1;

        public const long GSV_ACTEX_FLAG_BAUDRATE = 0x00000001L;
        public const long GSV_ACTEX_FLAG_WAKEUP = 0x00000002L;
        public const long GSV_ACTEX_FLAG_HANDSHAKE = 0x00000004L;

        public const byte GSV_MEMORY_OK = 0x0; //GSV_OK
        public const byte GSV_MEMORY_OK_FUNCTION = 0x1; //&H1
                                                        /* public const byte GSV_MEMORY_ERROR_ABORT = //&HFF;
                                                         public const byte GSV_MEMORY_ERROR_SEVERE = //&H10;
                                                         public const byte GSV_MEMORY_ERROR_NA = //&H11;
                                                         public const byte GSV_MEMORY_ERROR_DATA = //&H12;
                                                         public const long GSV_ACTEX_FLAG_BAUDRATE = // &H1&;
                                                         */

        [StructLayout(LayoutKind.Sequential)]
        public struct GSV_ACTIVATE_EXTENDED
        {
            public long actex_size;    //Größe der Datenstruktur, gesetzt durch die GSV_ACTEX_SIZE Konstante(bzw.Funktion).
            public long actex_buffersize; //Anzahl der maximal zwischenzuspeichernden Meßdatenwerte.
            public long actex_flags; //Kombination von GSV_ACTEX_FLAG_xxx Konstanten, die festlegt, welche weiteren Felder verwendet werden.
            public long actex_baudrate; //(Optional). Gewünschte Baudrate für die serielle Schnittstelle(nicht verwendet = gleiche Einstellung wie bei GSVactivate).
        }

        //string dllPath = "C:\GitHub\Scaneva\Dependencies\MELibraries\MEGSV.dll";


        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#1")]
        public static extern int GSVversion();

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#2")]
        public static extern int GSVmodel(int no);

        //[DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#3")]
        //public static extern int MEreservedExec(int no, int code, int rexpro, IntPtr arg);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#4")]
        public static extern int MErequestEvent(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#8")]
        public static extern int GSVgetLocalBaudRate(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#9")]
        public static extern int GSVactivateExtended(int no, ref GSV_ACTIVATE_EXTENDED actex);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#10")]
        public static extern int GSVactivate(int no, int buffersize);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#11")]
        public static extern int GSVrelease(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#12")]
        public static extern int GSVinitialize(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#13")]
        public static extern int GSVflushBuffer(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#17")]
        public static extern int GSVgetValues(ref int no, int n);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#18")]
        public static extern int MEsendBytesToDevices(ref int no, int n, byte[] pbuf, int count);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#19")]
        public static extern int MEsendBytes(int no, byte[] pbuf, int count);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#20")]
        public static extern int GSVreceived(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#21")]
        public static extern int GSVread(int no, ref Double ad);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#22")]
        public static extern int GSVreadMultiple(int no, ref Double ad, int count, ref int valsread);

        //[DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#23")]
        //public static extern int GSVcopyMemory(int no, int dc, int addr, IntPtr buffer);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#24")]
        public static extern int GSVcheckMemory(int no, int dc, int addr, IntPtr buffer);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#25")]
        public static extern int GSVreadStatus(int no, ref Double ad, StringBuilder ps);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#26")]
        public static extern int GSVreadStatusMultiple(int no, ref Double ad, StringBuilder ps, int count, ref int valsread);

        //[DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#28")]
        //public static extern int MEasynchronousSendBytes(int no, byte[] pbuf, int count, IntPtr handle);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#29")]
        public static extern int MEsynchronizeSendBytes(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#30")]
        public static extern int GSVgetOptionsCode(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#31")]
        public static extern int GSVgetOptionsExtension3(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#32")]
        public static extern int GSVgetOptionsLinear(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#33")]
        public static extern int GSVgetOptionsExtension21(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#34")]
        public static extern int GSVgetOptionsSleepMode(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#50")]
        public static extern int GSVisBipol(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#51")]
        public static extern Double GSVgetFreq(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#52")]
        public static extern int GSVgetGain(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#53")]
        public static extern int GSVgetChannel(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#54")]
        public static extern int MEsetModeLock(int no, int slock, String id);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#55")]
        public static extern int GSVgetModeLock(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#56")]
        public static extern int GSVsetModeLinear(int no, int lin);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#57")]
        public static extern int GSVgetModeLinear(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#58")]
        public static extern int GSVsetModeAverage(int no, int avg);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#59")]
        public static extern int GSVgetModeAverage(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#60")]
        public static extern int GSVsetModeSI(int no, int msi);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#61")]
        public static extern int GSVgetModeSI(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#62")]
        public static extern int GSVsetModeText(int no, int mt);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#63")]
        public static extern int GSVgetModeText(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#64")]
        public static extern int GSVsetModeMax(int no, int mx);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#65")]
        public static extern int GSVgetModeMax(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#66")]
        public static extern int GSVsetModeLog(int no, int lg);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#67")]
        public static extern int GSVgetModeLog(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#68")]
        public static extern int GSVsetModeWindow(int no, int win);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#69")]
        public static extern int GSVgetModeWindow(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#70")]
        public static extern int GSVhasLCD(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#71")]
        public static extern int GSVhasADC(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#72")]
        public static extern int GSVhasUII(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#73")]
        public static extern int GSVisSI(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#74")]
        public static extern int GSVisWL(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#75")]
        public static extern int GSVhasAF(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#80")]
        public static extern int GSVsetBridgeType(int no, int bt);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#81")]
        public static extern int GSVgetBridgeType(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#84")]
        public static extern int GSVsetBridgeInternal(int no, int bi);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#85")]
        public static extern int GSVgetBridgeInternal(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#100")]
        public static extern int GSVresetStatus(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#101")]
        public static extern int GSVgetScale(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#102")]
        public static extern int GSVgetZero(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#103")]
        public static extern int GSVgetControl(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#104")]
        public static extern int GSVgetOffset(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#105")]
        public static extern int GSVwriteScale(int no, int scalev);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#106")]
        public static extern int GSVwriteZero(int no, int zero);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#107")]
        public static extern int GSVwriteControl(int no, int control);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#108")]
        public static extern int GSVwriteOffset(int no, int offset);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#109")]
        public static extern int GSVgetAll(int no, int pos);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#110")]
        public static extern int GSVsaveAll(int no, int pos);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#111")]
        public static extern int GSVsetCal(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#112")]
        public static extern int GSVsetZero(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#113")]
        public static extern int GSVsetScale(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#114")]
        public static extern int GSVsetOffset(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#115")]
        public static extern int GSVDispSetUnit(int no, int dispunit);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#116")]
        public static extern int GSVDispSetNorm(int no, Double norm);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#117")]
        public static extern int GSVDispSetDPoint(int no, int dpoint);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#118")]
        public static extern int GSVsetFreq(int no, Double freq);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#119")]
        public static extern int GSVsetGain(int no, int gain);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#120")]
        public static extern int GSVsetBipolar(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#121")]
        public static extern int GSVsetUnipolar(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#123")]
        public static extern int MEsetCal(int no, Double cal);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#124")]
        public static extern Double MEgetCal(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#125")]
        public static extern int MEsendID(int no, String id);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#126")]
        public static extern Double GSVDispGetNorm(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#127")]
        public static extern int GSVDispGetUnit(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#128")]
        public static extern int GSVDispGetDPoint(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#129")]
        public static extern int GSVswitch(int no, int swon);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#130")]
        public static extern int MEwriteSerialNo(int no, String number);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#131")]
        public static extern int MEreadSerialNo(int no, StringBuilder number);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#132")]
        public static extern int GSVsetThreshold(int no, Double thon, Double thoff);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#133")]
        public static extern int GSVgetThreshold(int no, ref Double thon, ref Double thoff);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#134")]
        public static extern int GSVsetChannel(int no, int channel);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#135")]
        public static extern int GSVstopTransmit(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#136")]
        public static extern int GSVstartTransmit(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#137")]
        public static extern int GSVclearBuffer(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#138")]
        public static extern int GSVsetMode(int no, int mode);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#139")]
        public static extern int GSVgetMode(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#140")]
        public static extern int MEwriteEquipment(int no, int equip);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#141")]
        public static extern int GSVgetEquipment(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#143")]
        public static extern int GSVfirmwareVersion(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#144")]
        public static extern int GSVsetGageFactor(int no, Double gf);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#145")]
        public static extern Double GSVgetGageFactor(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#146")]
        public static extern int GSVsetPoisson(int no, Double poiss);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#147")]
        public static extern Double GSVgetPoisson(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#148")]
        public static extern int GSVsetBridge(int no, int br);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#149")]
        public static extern int GSVgetBridge(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#150")]
        public static extern int MEwriteRange(int no, Double range);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#151")]
        public static extern Double GSVgetRange(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#152")]
        public static extern int MEsetOffsetWait(int no, Double ow);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#153")]
        public static extern Double MEgetOffsetWait(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#154")]
        public static extern int GSVgetOptions(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#155")]
        public static extern int MEwriteOptions(int no, int options);

        //[DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#156")]
        //public static extern int GSVreadMemory(int no, int addr, IntPtr buffer, IntPtr status);

        //[DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#157")]
        //public static extern int GSVwriteMemory(int no, int addr, int check, int value, IntPtr status);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#158")]
        public static extern Double GSVgetMemoryWait(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#159")]
        public static extern int GSVgetValue(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#160")]
        public static extern int GSVclearMaxValue(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#161")]
        public static extern int GSVDispSetDigits(int no, int digits);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#162")]
        public static extern int GSVDispGetDigits(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#163")]
        public static extern int GSVunlockUII(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#164")]
        public static extern int GSVlockUII(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#166")]
        public static extern int GSVgetLastError(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#167")]
        public static extern int GSVsetSecondThreshold(int no, Double thon, Double thoff);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#168")]
        public static extern int GSVgetSecondThreshold(int no, ref Double thon, ref Double thoff);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#169")]
        public static extern int GSVgetDeviceType(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#170")]
        public static extern int GSVDispCalcNorm(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#228")]
        public static extern int GSVsetTxMode(int no, int txmode);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#229")]
        public static extern int GSVgetTxMode(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#230")]
        public static extern int GSVsetBaud(int no, int baud);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#231")]
        public static extern int GSVgetBaud(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#234")]
        public static extern int GSVsetSlowRate(int no, int secs);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#235")]
        public static extern int GSVgetSlowRate(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#236")]
        public static extern int GSVsetSpecialMode(int no, int smode);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#237")]
        public static extern int GSVgetSpecialMode(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#238")]
        public static extern int GSVwriteSamplingRate(int no, Double freq, int factor);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#239")]
        public static extern int GSVreadSamplingRate(int no, ref Double freq, ref int factor);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#240")]
        public static extern int GSVsetCanSetting(int no, int stype, int val);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#241")]
        public static extern int GSVgetCanSetting(int no, int stype);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#244")]
        public static extern int GSVsetAnalogFilter(int no, Double freq);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#245")]
        public static extern Double GSVgetAnalogFilter(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#357")]
        public static extern int GSVgetTxModeConfig(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#358")]
        public static extern int GSVsetTxModeTransmit4(int no, int t4);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#359")]
        public static extern int GSVgetTxModeTransmit4(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#360")]
        public static extern int GSVsetTxModeRepeat3(int no, int r3);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#361")]
        public static extern int GSVgetTxModeRepeat3(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#362")]
        public static extern int GSVsetTxModeTransmit5(int no, int t5);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#363")]
        public static extern int GSVgetTxModeTransmit5(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#364")]
        public static extern int GSVsetTxModeReadOnly(int no, int ro);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#365")]
        public static extern int GSVgetTxModeReadOnly(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#372")]
        public static extern int GSVsetSpecialModeSlow(int no, int slow);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#373")]
        public static extern int GSVgetSpecialModeSlow(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#374")]
        public static extern int GSVsetSpecialModeAverage(int no, int avg);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#375")]
        public static extern int GSVgetSpecialModeAverage(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#376")]
        public static extern int GSVsetSpecialModeFilter(int no, int flt);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#377")]
        public static extern int GSVgetSpecialModeFilter(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#378")]
        public static extern int GSVsetSpecialModeMax(int no, int mx);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#379")]
        public static extern int GSVgetSpecialModeMax(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#380")]
        public static extern int GSVsetSpecialModeFilterAuto(int no, int fltauto);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#381")]
        public static extern int GSVgetSpecialModeFilterAuto(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#382")]
        public static extern int GSVsetSpecialModeFilterOrder5(int no, int fltord5);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#383")]
        public static extern int GSVgetSpecialModeFilterOrder5(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#384")]
        public static extern int GSVsetSpecialModeSleep(int no, int sleep);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#385")]
        public static extern int GSVgetSpecialModeSleep(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#404")]
        public static extern Double GSVreadSamplingFrequency(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#405")]
        public static extern int GSVreadSamplingFactor(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#406")]
        public static extern int GSVsetBaudRate(int no, int baud);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#407")]
        public static extern int GSVgetBaudRate(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#408")]
        public static extern int GSVsetCanBaudRate(int no, int baud);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#409")]
        public static extern int GSVgetCanBaudRate(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#410")]
        public static extern int GSVisSamplingFrequencyProtocolLinear(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#411")]
        public static extern int GSVisSamplingFactorProtocolLinear(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#412")]
        public static extern int GSVsetCanID(int no, int idtype, int id);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#413")]
        public static extern int GSVgetCanID(int no, int idtype);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#414")]
        public static extern int GSVsetCanBaud(int no, int baud);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#415")]
        public static extern int GSVgetCanBaud(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#417")]
        public static extern int GSVisCanAvailable(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#418")]
        public static extern int GSVsetCanActive(int no, int active);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#419")]
        public static extern int GSVgetCanActive(int no);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#420")]
        public static extern int GSVsetCanMode20B(int no, int mode20b);

        [DllImport("MEGSV.dll", CharSet = CharSet.Ansi, EntryPoint = "#421")]
        public static extern int GSVgetCanMode20B(int no);

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

            //GSV_ACTIVATE_EXTENDED actExt;
            //actExt.actex_baudrate = Settings.Baudrate;
            //actExt.actex_buffersize = Settings.Buffersize;
            //actExt.actex_flags = GSV_ACTEX_FLAG_BAUDRATE; // + GSV_ACTEX_FLAG_WAKEUP + GSV_ACTEX_FLAG_HANDSHAKE;
            //actExt.actex_size = Marshal.SizeOf(typeof(GSV_ACTIVATE_EXTENDED));

            int status = GSVactivate(Settings.COMPort, Settings.Buffersize);
            //int status = GSVactivateExtended(Settings.COMPort, ref actExt);
            if (status == GSV_OK)
            {
                log.Add("GSV-3 measurement amplifier (Port: " + Settings.COMPort + ") activated");


                mintModel = GSVmodel(Settings.COMPort);

                StringBuilder tmpStr = new StringBuilder(256);

                if (MEreadSerialNo(Settings.COMPort, tmpStr) == GSV_OK)
                {
                    mlngSerialNo = Convert.ToInt64(tmpStr.ToString());
                }         

                GSVsetFreq(Settings.COMPort, 25);
                GSVsetModeAverage(Settings.COMPort, 1);
                double freq = GSVgetFreq(Settings.COMPort);  //Holt den Frequenzwert von Port no und speichert das Ergebnis in der Variable Freq
                GSVDispSetUnit(Settings.COMPort, 3);
                GSVDispSetNorm(Settings.COMPort, 4000 / 0.4999);
                m_dblNorm = GSVDispGetNorm(Settings.COMPort);    //Dieser Funktionsaufruf ermittelt die Normierung und speichert ihn in der Variable Norm
                hwStatus = enuHWStatus.Ready;
            }
            else
            {
                log.Error("Could not connect GSV-3 measurement amplifier (Port: " + Settings.COMPort + ")");
                hwStatus = enuHWStatus.Error;
            }

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
            hwStatus = enuHWStatus.NotInitialized;
        }

        public Sensor_Force_ME_GSV3(LogHelper log) : base(log)
        {
            settings = new Sensor_Force_ME_GSV3_Settings();
            log.Add("Initializing ME GSV force sensor");
            InitTransducerChannels();

            hwStatus = enuHWStatus.NotInitialized;
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
                    if (GSVreadMultiple(Settings.COMPort, ref vals[0], vals.Length, ref valsread) == GSV_TRUE)
                    {
                        double retval = 0;
                        for (int i = 0; (i < valsread) && (i < Settings.Buffersize); i++)
                        {
                            retval += vals[i];
                        }
                        return retval / valsread;
                    }
                    return double.NaN;

                default:
                    return double.NaN;
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