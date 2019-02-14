{$A+,B-,D-,H-,I+,J-,L-,M-,O-,P-,Q-,R-,T-,U-,V+,W-,X+,Y-,Z4}

(*************************************************)
(*                                               *)
(*  Delphi-Unit for GSV-Functions in MEGSV.DLL   *)
(*  Version 1.50                                 *)
(*                                               *)
(*  Copyright (C) Dr. Holger Kabelitz 1999-2009  *)
(*  All rights reserved.                         *)
(*                                               *)
(*  Dr. Holger Kabelitz                          *)
(*  D-13507 Berlin                               *)
(*  Germany                                      *)
(*                                               *)
(*************************************************)

unit MEGSV;

interface

const
  GSV_OK = 0;
  GSV_ERROR = -1;
  GSV_TRUE = 1;
  GSV_MEMORY_OK = GSV_OK;
  GSV_MEMORY_OK_FUNCTION = $01;
  GSV_MEMORY_ERROR_ABORT = $FF;
  GSV_MEMORY_ERROR_SEVERE = $10;
  GSV_MEMORY_ERROR_NA = $11;
  GSV_MEMORY_ERROR_DATA = $12;
  GSV_MEMORY_ERROR_FAILED = $1E;
  GSV_MEMORY_ERROR_DENIED = $1F;
  GSV_LINEAR_EXECUTE = 0;
  GSV_LINEAR_TEST_ALL = 1;
  GSV_LINEAR_TEST_SLOPES = 2;
  GSV_LINEAR_TEST_POINTS = 3;
  GSV_LINEAR_SORT = $4000;
  GSV_LINEAR_INFO = 0;
  GSV_LINEAR_INITIALIZE = -4;
  GSV_LINEAR_OFFSETZERO = -2;
  GSV_LINEAR_COUNTER = 1;
  GSV_LINEAR_ERROR_CODE = $3C00;
  GSV_LINEAR_ERROR_POINT = $00FF;
  GSV_LINEAR_ERROR_ACT = $0100;
  GSV_LINEAR_ERROR_REF = $0200;
  GSV_LINEAR_ERROR_OK = GSV_OK;
  GSV_LINEAR_ERROR_NOIFC = $0400;
  GSV_LINEAR_ERROR_UNKNOWNIFC = $0800;
  GSV_LINEAR_ERROR_NPOINTS = $0C00;
  GSV_LINEAR_ERROR_RANGE = $1000;
  GSV_LINEAR_ERROR_ORDER = $1400;
  GSV_LINEAR_ERROR_SLOPE = $1800;
  GSV_LINEAR_ERROR_LOCKED = $1C00;
  GSV_LINEAR_ERROR_INCOMPLETE = $2000;
  GSV_LINEAR_ERROR_PARAMETER = $2400;
  GSV_LINEAR_ERROR_GAIN = $2800;
  GSV_LINEAR_ERROR_SENSITIVITY = $2C00;
  GSV_LINEAR_ERROR_NOTIMPLEMENTED = $3000;
  GSV_ACTEX_FLAG_BAUDRATE = $00000001;
  GSV_ACTEX_FLAG_WAKEUP = $00000002;
  GSV_ACTEX_FLAG_HANDSHAKE = $00000004;
  GSV_STATUS_MASK_SWITCH = $10;
  GSV_STATUS_MASK_SWITCH2 = $08;
  GSV_CANSET_MANUFACTURER_ID = $00;
  GSV_CANSET_COMMAND_ID = $01;
  GSV_CANSET_MESSAGE_ID = $02;
  GSV_CANSET_BAUD = $7E;
  GSV_CANSET_FLAGS = $7F;
  GSV_CANSET_20B = $80;
  ME_INTSTAT_FLUSH = $00000001;

type
  PDouble = ^Double;
  PInteger = ^Integer;
  PByte = ^Byte;
  MEreservedExecProc = function (no:Integer; arg:Pointer):Integer stdcall;
  GSV_ACTIVATE_EXTENDED = record
                            actex_size : LongInt;
                            actex_buffersize : LongInt;
                            actex_flags : LongInt;
                            actex_baudrate : LongInt;
                          end;

const
  GSV_ACTEX_SIZE = SizeOf(GSV_ACTIVATE_EXTENDED);

function GSVversion:LongInt; stdcall;

function GSVmodel(no:Integer):Integer; stdcall;

function MEreservedExec(no, code:Integer; rexpro:MEreservedExecProc; arg:Pointer):Integer; stdcall;

function MErequestEvent(no:Integer):Integer; stdcall;

function GSVrevision:LongInt; stdcall;

function GSVgetLocalBaudRate(no:Integer):LongInt; stdcall;

function GSVactivateExtended(no:Integer; var actex:GSV_ACTIVATE_EXTENDED):Integer; stdcall;

function GSVactivate(no:Integer; buffersize:LongInt):Integer; stdcall;

procedure GSVrelease(no:Integer); stdcall;

function GSVinitialize(no:Integer):Integer; stdcall;

function GSVflushBuffer(no:Integer):Integer; stdcall;

function MEinternalStatus(no:Integer):LongInt; stdcall;

function GSVgetValues(no:PInteger; n:Integer):Integer; stdcall;

function MEsendBytesToDevices(no:PInteger; n:Integer; var pbuf; count:Integer):Integer; stdcall;

function MEsendBytes(no:Integer; var pbuf; count:Integer):Integer; stdcall;

function GSVreceived(no:Integer):Integer; stdcall;

function GSVread(no:Integer; var ad:Double):Integer; stdcall;

function GSVreadMultiple(no:Integer; ad:PDouble; count:Integer; var valsread:Integer):Integer; stdcall;

function GSVcopyMemory(no, dc, addr:Integer; var buffer):Integer; stdcall;

function GSVcheckMemory(no, dc, addr:Integer; buffer:PByte):Integer; stdcall;

function GSVreadStatus(no:Integer; var ad:Double; var ps:Byte):Integer; stdcall;

function GSVreadStatusMultiple(no:Integer; ad:PDouble; ps:PByte; count:Integer; var valsread:Integer):Integer; stdcall;

function GSVcopyLinear(no:Integer; pd:PDouble; nd:Integer; pr:PInteger):Integer; stdcall;

function GSVasynchronousSendBytes(no:Integer; var pbuf; count:Integer; handle:Pointer):Integer; stdcall;

function GSVsynchronizeSendBytes(no:Integer):Integer; stdcall;

function GSVgetOptionsCode(no:Integer):Integer; stdcall;

function GSVgetOptionsExtension3(no:Integer):Integer; stdcall;

function GSVgetOptionsLinear(no:Integer):Integer; stdcall;

function GSVgetOptionsExtension21(no:Integer):Integer; stdcall;

function GSVgetOptionsSleepMode(no:Integer):Integer; stdcall;

function GSVgetOptionsCommandTest(no:Integer):Integer; stdcall;

function MEwriteRanges(no, channel, inx:Integer; ia:LongInt):Integer; stdcall;

function GSVisBipol(no:Integer):Integer; stdcall;

function GSVgetFreq(no:Integer):Double; stdcall;

function GSVgetGain(no:Integer):Integer; stdcall;

function GSVgetChannel(no:Integer):Integer; stdcall;

function MEsetModeLock(no, slock:Integer; id:PChar):Integer; stdcall;

function GSVgetModeLock(no:Integer):Integer; stdcall;

function GSVsetModeLinear(no, lin:Integer):Integer; stdcall;

function GSVgetModeLinear(no:Integer):Integer; stdcall;

function GSVsetModeAverage(no, avg:Integer):Integer; stdcall;

function GSVgetModeAverage(no:Integer):Integer; stdcall;

function GSVsetModeSI(no, msi:Integer):Integer; stdcall;

function GSVgetModeSI(no:Integer):Integer; stdcall;

function GSVsetModeText(no, mt:Integer):Integer; stdcall;

function GSVgetModeText(no:Integer):Integer; stdcall;

function GSVsetModeMax(no, mx:Integer):Integer; stdcall;

function GSVgetModeMax(no:Integer):Integer; stdcall;

function GSVsetModeLog(no, lg:Integer):Integer; stdcall;

function GSVgetModeLog(no:Integer):Integer; stdcall;

function GSVsetModeWindow(no, win:Integer):Integer; stdcall;

function GSVgetModeWindow(no:Integer):Integer; stdcall;

function GSVhasLCD(no:Integer):Integer; stdcall;

function GSVhasADC(no:Integer):Integer; stdcall;

function GSVhasUII(no:Integer):Integer; stdcall;

function GSVisSI(no:Integer):Integer; stdcall;

function GSVisWL(no:Integer):Integer; stdcall;

function GSVhasAF(no:Integer):Integer; stdcall;

function GSVsetBridgeType(no, bt:Integer):Integer; stdcall;

function GSVgetBridgeType(no:Integer):Integer; stdcall;

function GSVsetBridgeInternal(no, bi:Integer):Integer; stdcall;

function GSVgetBridgeInternal(no:Integer):Integer; stdcall;

function GSVresetStatus(no:Integer):Integer; stdcall;

function GSVgetScale(no:Integer):LongInt; stdcall;

function GSVgetZero(no:Integer):LongInt; stdcall;

function GSVgetControl(no:Integer):LongInt; stdcall;

function GSVgetOffset(no:Integer):LongInt; stdcall;

function GSVwriteScale(no:Integer; scale:LongInt):Integer; stdcall;

function GSVwriteZero(no:Integer; zero:LongInt):Integer; stdcall;

function GSVwriteControl(no:Integer; control:LongInt):Integer; stdcall;

function GSVwriteOffset(no:Integer; offset:LongInt):Integer; stdcall;

function GSVgetAll(no, pos:Integer):Integer; stdcall;

function GSVsaveAll(no, pos:Integer):Integer; stdcall;

function GSVsetCal(no:Integer):Integer; stdcall;

function GSVsetZero(no:Integer):Integer; stdcall;

function GSVsetScale(no:Integer):Integer; stdcall;

function GSVsetOffset(no:Integer):Integer; stdcall;

function GSVDispSetUnit(no, dispunit:Integer):Integer; stdcall;

function GSVDispSetNorm(no:Integer; norm:Double):Integer; stdcall;

function GSVDispSetDPoint(no, dpoint:Integer):Integer; stdcall;

function GSVsetFreq(no:Integer; freq:Double):Integer; stdcall;

function GSVsetGain(no, gain:Integer):Integer; stdcall;

function GSVsetBipolar(no:Integer):Integer; stdcall;

function GSVsetUnipolar(no:Integer):Integer; stdcall;

function MEsetCal(no:Integer; cal:Double):Integer; stdcall;

function MEgetCal(no:Integer):Double; stdcall;

function MEsendID(no:Integer; id:PChar):Integer; stdcall;

function GSVDispGetNorm(no:Integer):Double; stdcall;

function GSVDispGetUnit(no:Integer):Integer; stdcall;

function GSVDispGetDPoint(no:Integer):Integer; stdcall;

function GSVswitch(no, on:Integer):Integer; stdcall;

function MEwriteSerialNo(no:Integer; number:PChar):Integer; stdcall;

function GSVgetSerialNo(no:Integer; number:PChar):Integer; stdcall;

function GSVsetThreshold(no:Integer; thon, thoff:Double):Integer; stdcall;

function GSVgetThreshold(no:Integer; var thon, thoff:Double):Integer; stdcall;

function GSVsetChannel(no, channel:Integer):Integer; stdcall;

function GSVstopTransmit(no:Integer):Integer; stdcall;

function GSVstartTransmit(no:Integer):Integer; stdcall;

function GSVclearBuffer(no:Integer):Integer; stdcall;

function GSVsetMode(no, mode:Integer):Integer; stdcall;

function GSVgetMode(no:Integer):Integer; stdcall;

function MEwriteEquipment(no, equip:Integer):Integer; stdcall;

function GSVgetEquipment(no:Integer):Integer; stdcall;

function GSVfirmwareVersion(no:Integer):LongInt; stdcall;

function GSVsetGageFactor(no:Integer; gf:Double):Integer; stdcall;

function GSVgetGageFactor(no:Integer):Double; stdcall;

function GSVsetPoisson(no:Integer; poiss:Double):Integer; stdcall;

function GSVgetPoisson(no:Integer):Double; stdcall;

function GSVsetBridge(no, br:Integer):Integer; stdcall;

function GSVgetBridge(no:Integer):Integer; stdcall;

function MEwriteRange(no:Integer; range:Double):Integer; stdcall;

function GSVgetRange(no:Integer):Double; stdcall;

function MEsetOffsetWait(no:Integer; ow:Double):Integer; stdcall;

function MEgetOffsetWait(no:Integer):Double; stdcall;

function GSVgetOptions(no:Integer):LongInt; stdcall;

function MEwriteOptions(no:Integer; options:LongInt):Integer; stdcall;

function GSVreadMemory(no, addr:Integer; var buffer; status:PInteger):Integer; stdcall;

function GSVwriteMemory(no, addr, check, value:Integer; status:PInteger):Integer; stdcall;

function GSVgetMemoryWait(no:Integer):Double; stdcall;

function GSVgetValue(no:Integer):Integer; stdcall;

function GSVclearMaxValue(no:Integer):Integer; stdcall;

function GSVDispSetDigits(no, digits:Integer):Integer; stdcall;

function GSVDispGetDigits(no:Integer):Integer; stdcall;

function GSVunlockUII(no:Integer):Integer; stdcall;

function GSVlockUII(no:Integer):Integer; stdcall;

function GSVgetLastError(no:Integer):Integer; stdcall;

function GSVsetSecondThreshold(no:Integer; thon, thoff:Double):Integer; stdcall;

function GSVgetSecondThreshold(no:Integer; var thon, thoff:Double):Integer; stdcall;

function GSVgetDeviceType(no:Integer):Integer; stdcall;

function GSVDispCalcNorm(no:Integer):Integer; stdcall;

function GSVsetTxMode(no, txmode:Integer):Integer; stdcall;

function GSVgetTxMode(no:Integer):Integer; stdcall;

function GSVsetBaud(no, baud:Integer):Integer; stdcall;

function GSVgetBaud(no:Integer):Integer; stdcall;

function GSVsetSlowRate(no:Integer; secs:LongInt):Integer; stdcall;

function GSVgetSlowRate(no:Integer):LongInt; stdcall;

function GSVsetSpecialMode(no, smode:Integer):Integer; stdcall;

function GSVgetSpecialMode(no:Integer):Integer; stdcall;

function GSVwriteSamplingRate(no:Integer; freq:Double; factor:Integer):Integer; stdcall;

function GSVreadSamplingRate(no:Integer; freq:PDouble; factor:PInteger):Integer; stdcall;

function GSVsetCanSetting(no, stype, val:Integer):Integer; stdcall;

function GSVgetCanSetting(no, stype:Integer):Integer; stdcall;

function GSVsetAnalogFilter(no:Integer; freq:Double):Integer; stdcall;

function GSVgetAnalogFilter(no:Integer):Double; stdcall;

function GSVisCommandAvailable(no, cmd1, cmd2:Integer):Integer; stdcall;

function GSVsetNoiseCutThreshold(no:Integer; thr:Double):Integer; stdcall;

function GSVgetNoiseCutThreshold(no:Integer):Double; stdcall;

function GSVsetAutoZeroCounter(no:Integer; ctr:LongInt):Integer; stdcall;

function GSVgetAutoZeroCounter(no:Integer):LongInt; stdcall;

function GSVsetUserTextChar(no, addr:Integer; c:Byte):Integer; stdcall;

function GSVgetRanges(no, channel, inx:Integer; var prange:Double):Integer; stdcall;

function GSVsetRanges(no, channel, inx:Integer; range:Double):Integer; stdcall;

function GSVgetTxModeConfig(no:Integer):Integer; stdcall;

function GSVsetTxModeTransmit4(no, t4:Integer):Integer; stdcall;

function GSVgetTxModeTransmit4(no:Integer):Integer; stdcall;

function GSVsetTxModeRepeat3(no, r3:Integer):Integer; stdcall;

function GSVgetTxModeRepeat3(no:Integer):Integer; stdcall;

function GSVsetTxModeTransmit5(no, t5:Integer):Integer; stdcall;

function GSVgetTxModeTransmit5(no:Integer):Integer; stdcall;

function GSVsetTxModeReadOnly(no, ro:Integer):Integer; stdcall;

function GSVgetTxModeReadOnly(no:Integer):Integer; stdcall;

function GSVsetSpecialModeSlow(no, slow:Integer):Integer; stdcall;

function GSVgetSpecialModeSlow(no:Integer):Integer; stdcall;

function GSVsetSpecialModeAverage(no, avg:Integer):Integer; stdcall;

function GSVgetSpecialModeAverage(no:Integer):Integer; stdcall;

function GSVsetSpecialModeFilter(no, flt:Integer):Integer; stdcall;

function GSVgetSpecialModeFilter(no:Integer):Integer; stdcall;

function GSVsetSpecialModeMax(no, mx:Integer):Integer; stdcall;

function GSVgetSpecialModeMax(no:Integer):Integer; stdcall;

function GSVsetSpecialModeFilterAuto(no, fltauto:Integer):Integer; stdcall;

function GSVgetSpecialModeFilterAuto(no:Integer):Integer; stdcall;

function GSVsetSpecialModeFilterOrder5(no, fltord5:Integer):Integer; stdcall;

function GSVgetSpecialModeFilterOrder5(no:Integer):Integer; stdcall;

function GSVsetSpecialModeSleep(no, sleep:Integer):Integer; stdcall;

function GSVgetSpecialModeSleep(no:Integer):Integer; stdcall;

function GSVsetSpecialModeAutoZero(no, autozero:Integer):Integer; stdcall;

function GSVgetSpecialModeAutoZero(no:Integer):Integer; stdcall;

function GSVsetSpecialModeNoiseCut(no, noisecut:Integer):Integer; stdcall;

function GSVgetSpecialModeNoiseCut(no:Integer):Integer; stdcall;

function GSVreadSamplingFrequency(no:Integer):Double; stdcall;

function GSVreadSamplingFactor(no:Integer):Integer; stdcall;

function GSVsetBaudRate(no:Integer; baudrate:LongInt):Integer; stdcall;

function GSVgetBaudRate(no:Integer):LongInt; stdcall;

function GSVsetCanBaudRate(no:Integer; baudrate:LongInt):Integer; stdcall;

function GSVgetCanBaudRate(no:Integer):LongInt; stdcall;

function GSVisSamplingFrequencyProtocolLinear(no:Integer):Integer; stdcall;

function GSVisSamplingFactorProtocolLinear(no:Integer):Integer; stdcall;

function GSVsetCanID(no, idtype, id:Integer):Integer; stdcall;

function GSVgetCanID(no, idtype:Integer):Integer; stdcall;

function GSVsetCanBaud(no, baud:Integer):Integer; stdcall;

function GSVgetCanBaud(no:Integer):Integer; stdcall;

function GSVisCanAvailable(no:Integer):Integer; stdcall;

function GSVsetCanActive(no, active:Integer):Integer; stdcall;

function GSVgetCanActive(no:Integer):Integer; stdcall;

function GSVsetCanMode20B(no, mode20b:Integer):Integer; stdcall;

function GSVgetCanMode20B(no:Integer):Integer; stdcall;

function GSVsetUserText(no:Integer; txt:PChar):Integer; stdcall;

implementation

{$IFDEF WIN32}

const
  MEDLL = 'MEGSV.DLL';

function GSVversion:LongInt;
  external MEDLL index 1;

function GSVmodel(no:Integer):Integer;
  external MEDLL index 2;

function MEreservedExec(no, code:Integer; rexpro:MEreservedExecProc; arg:Pointer):Integer;
  external MEDLL index 3;

function MErequestEvent(no:Integer):Integer;
  external MEDLL index 4;

function GSVrevision:LongInt;
  external MEDLL index 5;

function GSVgetLocalBaudRate(no:Integer):LongInt;
  external MEDLL index 8;

function GSVactivateExtended(no:Integer; var actex:GSV_ACTIVATE_EXTENDED):Integer;
  external MEDLL index 9;

function GSVactivate(no:Integer; buffersize:LongInt):Integer;
  external MEDLL index 10;

procedure GSVrelease(no:Integer);
  external MEDLL index 11;

function GSVinitialize(no:Integer):Integer;
  external MEDLL index 12;

function GSVflushBuffer(no:Integer):Integer;
  external MEDLL index 13;

function MEinternalStatus(no:Integer):LongInt;
  external MEDLL index 14;

function GSVgetValues(no:PInteger; n:Integer):Integer;
  external MEDLL index 17;

function MEsendBytesToDevices(no:PInteger; n:Integer; var pbuf; count:Integer):Integer;
  external MEDLL index 18;

function MEsendBytes(no:Integer; var pbuf; count:Integer):Integer;
  external MEDLL index 19;

function GSVreceived(no:Integer):Integer;
  external MEDLL index 20;

function GSVread(no:Integer; var ad:Double):Integer;
  external MEDLL index 21;

function GSVreadMultiple(no:Integer; ad:PDouble; count:Integer; var valsread:Integer):Integer;
  external MEDLL index 22;

function GSVcopyMemory(no, dc, addr:Integer; var buffer):Integer;
  external MEDLL index 23;

function GSVcheckMemory(no, dc, addr:Integer; buffer:PByte):Integer;
  external MEDLL index 24;

function GSVreadStatus(no:Integer; var ad:Double; var ps:Byte):Integer;
  external MEDLL index 25;

function GSVreadStatusMultiple(no:Integer; ad:PDouble; ps:PByte; count:Integer; var valsread:Integer):Integer;
  external MEDLL index 26;

function GSVcopyLinear(no:Integer; pd:PDouble; nd:Integer; pr:PInteger):Integer;
  external MEDLL index 27;

function GSVasynchronousSendBytes(no:Integer; var pbuf; count:Integer; handle:Pointer):Integer;
  external MEDLL index 28;

function GSVsynchronizeSendBytes(no:Integer):Integer;
  external MEDLL index 29;

function GSVgetOptionsCode(no:Integer):Integer;
  external MEDLL index 30;

function GSVgetOptionsExtension3(no:Integer):Integer;
  external MEDLL index 31;

function GSVgetOptionsLinear(no:Integer):Integer;
  external MEDLL index 32;

function GSVgetOptionsExtension21(no:Integer):Integer;
  external MEDLL index 33;

function GSVgetOptionsSleepMode(no:Integer):Integer;
  external MEDLL index 34;

function GSVgetOptionsCommandTest(no:Integer):Integer;
  external MEDLL index 35;

function MEwriteRanges(no, channel, inx:Integer; ia:LongInt):Integer;
  external MEDLL index 49;

function GSVisBipol(no:Integer):Integer;
  external MEDLL index 50;

function GSVgetFreq(no:Integer):Double;
  external MEDLL index 51;

function GSVgetGain(no:Integer):Integer;
  external MEDLL index 52;

function GSVgetChannel(no:Integer):Integer;
  external MEDLL index 53;

function MEsetModeLock(no, slock:Integer; id:PChar):Integer;
  external MEDLL index 54;

function GSVgetModeLock(no:Integer):Integer;
  external MEDLL index 55;

function GSVsetModeLinear(no, lin:Integer):Integer;
  external MEDLL index 56;

function GSVgetModeLinear(no:Integer):Integer;
  external MEDLL index 57;

function GSVsetModeAverage(no, avg:Integer):Integer;
  external MEDLL index 58;

function GSVgetModeAverage(no:Integer):Integer;
  external MEDLL index 59;

function GSVsetModeSI(no, msi:Integer):Integer;
  external MEDLL index 60;

function GSVgetModeSI(no:Integer):Integer;
  external MEDLL index 61;

function GSVsetModeText(no, mt:Integer):Integer;
  external MEDLL index 62;

function GSVgetModeText(no:Integer):Integer;
  external MEDLL index 63;

function GSVsetModeMax(no, mx:Integer):Integer;
  external MEDLL index 64;

function GSVgetModeMax(no:Integer):Integer;
  external MEDLL index 65;

function GSVsetModeLog(no, lg:Integer):Integer;
  external MEDLL index 66;

function GSVgetModeLog(no:Integer):Integer;
  external MEDLL index 67;

function GSVsetModeWindow(no, win:Integer):Integer;
  external MEDLL index 68;

function GSVgetModeWindow(no:Integer):Integer;
  external MEDLL index 69;

function GSVhasLCD(no:Integer):Integer;
  external MEDLL index 70;

function GSVhasADC(no:Integer):Integer;
  external MEDLL index 71;

function GSVhasUII(no:Integer):Integer;
  external MEDLL index 72;

function GSVisSI(no:Integer):Integer;
  external MEDLL index 73;

function GSVisWL(no:Integer):Integer;
  external MEDLL index 74;

function GSVhasAF(no:Integer):Integer;
  external MEDLL index 75;

function GSVsetBridgeType(no, bt:Integer):Integer;
  external MEDLL index 80;

function GSVgetBridgeType(no:Integer):Integer;
  external MEDLL index 81;

function GSVsetBridgeInternal(no, bi:Integer):Integer;
  external MEDLL index 84;

function GSVgetBridgeInternal(no:Integer):Integer;
  external MEDLL index 85;

function GSVresetStatus(no:Integer):Integer;
  external MEDLL index 100;

function GSVgetScale(no:Integer):LongInt;
  external MEDLL index 101;

function GSVgetZero(no:Integer):LongInt;
  external MEDLL index 102;

function GSVgetControl(no:Integer):LongInt;
  external MEDLL index 103;

function GSVgetOffset(no:Integer):LongInt;
  external MEDLL index 104;

function GSVwriteScale(no:Integer; scale:LongInt):Integer;
  external MEDLL index 105;

function GSVwriteZero(no:Integer; zero:LongInt):Integer;
  external MEDLL index 106;

function GSVwriteControl(no:Integer; control:LongInt):Integer;
  external MEDLL index 107;

function GSVwriteOffset(no:Integer; offset:LongInt):Integer;
  external MEDLL index 108;

function GSVgetAll(no, pos:Integer):Integer;
  external MEDLL index 109;

function GSVsaveAll(no, pos:Integer):Integer;
  external MEDLL index 110;

function GSVsetCal(no:Integer):Integer;
  external MEDLL index 111;

function GSVsetZero(no:Integer):Integer;
  external MEDLL index 112;

function GSVsetScale(no:Integer):Integer;
  external MEDLL index 113;

function GSVsetOffset(no:Integer):Integer;
  external MEDLL index 114;

function GSVDispSetUnit(no, dispunit:Integer):Integer;
  external MEDLL index 115;

function GSVDispSetNorm(no:Integer; norm:Double):Integer;
  external MEDLL index 116;

function GSVDispSetDPoint(no, dpoint:Integer):Integer;
  external MEDLL index 117;

function GSVsetFreq(no:Integer; freq:Double):Integer;
  external MEDLL index 118;

function GSVsetGain(no, gain:Integer):Integer;
  external MEDLL index 119;

function GSVsetBipolar(no:Integer):Integer;
  external MEDLL index 120;

function GSVsetUnipolar(no:Integer):Integer;
  external MEDLL index 121;

function MEsetCal(no:Integer; cal:Double):Integer;
  external MEDLL index 123;

function MEgetCal(no:Integer):Double;
  external MEDLL index 124;

function MEsendID(no:Integer; id:PChar):Integer;
  external MEDLL index 125;

function GSVDispGetNorm(no:Integer):Double;
  external MEDLL index 126;

function GSVDispGetUnit(no:Integer):Integer;
  external MEDLL index 127;

function GSVDispGetDPoint(no:Integer):Integer;
  external MEDLL index 128;

function GSVswitch(no, on:Integer):Integer;
  external MEDLL index 129;

function MEwriteSerialNo(no:Integer; number:PChar):Integer;
  external MEDLL index 130;

function GSVgetSerialNo(no:Integer; number:PChar):Integer;
  external MEDLL index 131;

function GSVsetThreshold(no:Integer; thon, thoff:Double):Integer;
  external MEDLL index 132;

function GSVgetThreshold(no:Integer; var thon, thoff:Double):Integer;
  external MEDLL index 133;

function GSVsetChannel(no, channel:Integer):Integer;
  external MEDLL index 134;

function GSVstopTransmit(no:Integer):Integer;
  external MEDLL index 135;

function GSVstartTransmit(no:Integer):Integer;
  external MEDLL index 136;

function GSVclearBuffer(no:Integer):Integer;
  external MEDLL index 137;

function GSVsetMode(no, mode:Integer):Integer;
  external MEDLL index 138;

function GSVgetMode(no:Integer):Integer;
  external MEDLL index 139;

function MEwriteEquipment(no, equip:Integer):Integer;
  external MEDLL index 140;

function GSVgetEquipment(no:Integer):Integer;
  external MEDLL index 141;

function GSVfirmwareVersion(no:Integer):LongInt;
  external MEDLL index 143;

function GSVsetGageFactor(no:Integer; gf:Double):Integer;
  external MEDLL index 144;

function GSVgetGageFactor(no:Integer):Double;
  external MEDLL index 145;

function GSVsetPoisson(no:Integer; poiss:Double):Integer;
  external MEDLL index 146;

function GSVgetPoisson(no:Integer):Double;
  external MEDLL index 147;

function GSVsetBridge(no, br:Integer):Integer;
  external MEDLL index 148;

function GSVgetBridge(no:Integer):Integer;
  external MEDLL index 149;

function MEwriteRange(no:Integer; range:Double):Integer;
  external MEDLL index 150;

function GSVgetRange(no:Integer):Double;
  external MEDLL index 151;

function MEsetOffsetWait(no:Integer; ow:Double):Integer;
  external MEDLL index 152;

function MEgetOffsetWait(no:Integer):Double;
  external MEDLL index 153;

function GSVgetOptions(no:Integer):LongInt;
  external MEDLL index 154;

function MEwriteOptions(no:Integer; options:LongInt):Integer;
  external MEDLL index 155;

function GSVreadMemory(no, addr:Integer; var buffer; status:PInteger):Integer;
  external MEDLL index 156;

function GSVwriteMemory(no, addr, check, value:Integer; status:PInteger):Integer;
  external MEDLL index 157;

function GSVgetMemoryWait(no:Integer):Double;
  external MEDLL index 158;

function GSVgetValue(no:Integer):Integer;
  external MEDLL index 159;

function GSVclearMaxValue(no:Integer):Integer;
  external MEDLL index 160;

function GSVDispSetDigits(no, digits:Integer):Integer;
  external MEDLL index 161;

function GSVDispGetDigits(no:Integer):Integer;
  external MEDLL index 162;

function GSVunlockUII(no:Integer):Integer;
  external MEDLL index 163;

function GSVlockUII(no:Integer):Integer;
  external MEDLL index 164;

function GSVgetLastError(no:Integer):Integer;
  external MEDLL index 166;

function GSVsetSecondThreshold(no:Integer; thon, thoff:Double):Integer;
  external MEDLL index 167;

function GSVgetSecondThreshold(no:Integer; var thon, thoff:Double):Integer;
  external MEDLL index 168;

function GSVgetDeviceType(no:Integer):Integer;
  external MEDLL index 169;

function GSVDispCalcNorm(no:Integer):Integer;
  external MEDLL index 170;

function GSVsetTxMode(no, txmode:Integer):Integer;
  external MEDLL index 228;

function GSVgetTxMode(no:Integer):Integer;
  external MEDLL index 229;

function GSVsetBaud(no, baud:Integer):Integer;
  external MEDLL index 230;

function GSVgetBaud(no:Integer):Integer;
  external MEDLL index 231;

function GSVsetSlowRate(no:Integer; secs:LongInt):Integer;
  external MEDLL index 234;

function GSVgetSlowRate(no:Integer):LongInt;
  external MEDLL index 235;

function GSVsetSpecialMode(no, smode:Integer):Integer;
  external MEDLL index 236;

function GSVgetSpecialMode(no:Integer):Integer;
  external MEDLL index 237;

function GSVwriteSamplingRate(no:Integer; freq:Double; factor:Integer):Integer;
  external MEDLL index 238;

function GSVreadSamplingRate(no:Integer; freq:PDouble; factor:PInteger):Integer;
  external MEDLL index 239;

function GSVsetCanSetting(no, stype, val:Integer):Integer;
  external MEDLL index 240;

function GSVgetCanSetting(no, stype:Integer):Integer;
  external MEDLL index 241;

function GSVsetAnalogFilter(no:Integer; freq:Double):Integer;
  external MEDLL index 244;

function GSVgetAnalogFilter(no:Integer):Double;
  external MEDLL index 245;

function GSVisCommandAvailable(no, cmd1, cmd2:Integer):Integer;
  external MEDLL index 247;

function GSVsetNoiseCutThreshold(no:Integer; thr:Double):Integer;
  external MEDLL index 248;

function GSVgetNoiseCutThreshold(no:Integer):Double;
  external MEDLL index 249;

function GSVsetAutoZeroCounter(no:Integer; ctr:LongInt):Integer;
  external MEDLL index 250;

function GSVgetAutoZeroCounter(no:Integer):LongInt;
  external MEDLL index 251;

function GSVsetUserTextChar(no, addr:Integer; c:Byte):Integer;
  external MEDLL index 252;

function GSVgetRanges(no, channel, inx:Integer; var prange:Double):Integer;
  external MEDLL index 262;

function GSVsetRanges(no, channel, inx:Integer; range:Double):Integer;
  external MEDLL index 263;

function GSVgetTxModeConfig(no:Integer):Integer;
  external MEDLL index 357;

function GSVsetTxModeTransmit4(no, t4:Integer):Integer;
  external MEDLL index 358;

function GSVgetTxModeTransmit4(no:Integer):Integer;
  external MEDLL index 359;

function GSVsetTxModeRepeat3(no, r3:Integer):Integer;
  external MEDLL index 360;

function GSVgetTxModeRepeat3(no:Integer):Integer;
  external MEDLL index 361;

function GSVsetTxModeTransmit5(no, t5:Integer):Integer;
  external MEDLL index 362;

function GSVgetTxModeTransmit5(no:Integer):Integer;
  external MEDLL index 363;

function GSVsetTxModeReadOnly(no, ro:Integer):Integer;
  external MEDLL index 364;

function GSVgetTxModeReadOnly(no:Integer):Integer;
  external MEDLL index 365;

function GSVsetSpecialModeSlow(no, slow:Integer):Integer;
  external MEDLL index 372;

function GSVgetSpecialModeSlow(no:Integer):Integer;
  external MEDLL index 373;

function GSVsetSpecialModeAverage(no, avg:Integer):Integer;
  external MEDLL index 374;

function GSVgetSpecialModeAverage(no:Integer):Integer;
  external MEDLL index 375;

function GSVsetSpecialModeFilter(no, flt:Integer):Integer;
  external MEDLL index 376;

function GSVgetSpecialModeFilter(no:Integer):Integer;
  external MEDLL index 377;

function GSVsetSpecialModeMax(no, mx:Integer):Integer;
  external MEDLL index 378;

function GSVgetSpecialModeMax(no:Integer):Integer;
  external MEDLL index 379;

function GSVsetSpecialModeFilterAuto(no, fltauto:Integer):Integer;
  external MEDLL index 380;

function GSVgetSpecialModeFilterAuto(no:Integer):Integer;
  external MEDLL index 381;

function GSVsetSpecialModeFilterOrder5(no, fltord5:Integer):Integer;
  external MEDLL index 382;

function GSVgetSpecialModeFilterOrder5(no:Integer):Integer;
  external MEDLL index 383;

function GSVsetSpecialModeSleep(no, sleep:Integer):Integer;
  external MEDLL index 384;

function GSVgetSpecialModeSleep(no:Integer):Integer;
  external MEDLL index 385;

function GSVsetSpecialModeAutoZero(no, autozero:Integer):Integer;
  external MEDLL index 400;

function GSVgetSpecialModeAutoZero(no:Integer):Integer;
  external MEDLL index 401;

function GSVsetSpecialModeNoiseCut(no, noisecut:Integer):Integer;
  external MEDLL index 402;

function GSVgetSpecialModeNoiseCut(no:Integer):Integer;
  external MEDLL index 403;

function GSVreadSamplingFrequency(no:Integer):Double;
  external MEDLL index 404;

function GSVreadSamplingFactor(no:Integer):Integer;
  external MEDLL index 405;

function GSVsetBaudRate(no:Integer; baudrate:LongInt):Integer;
  external MEDLL index 406;

function GSVgetBaudRate(no:Integer):LongInt;
  external MEDLL index 407;

function GSVsetCanBaudRate(no:Integer; baudrate:LongInt):Integer;
  external MEDLL index 408;

function GSVgetCanBaudRate(no:Integer):LongInt;
  external MEDLL index 409;

function GSVisSamplingFrequencyProtocolLinear(no:Integer):Integer;
  external MEDLL index 410;

function GSVisSamplingFactorProtocolLinear(no:Integer):Integer;
  external MEDLL index 411;

function GSVsetCanID(no, idtype, id:Integer):Integer;
  external MEDLL index 412;

function GSVgetCanID(no, idtype:Integer):Integer;
  external MEDLL index 413;

function GSVsetCanBaud(no, baud:Integer):Integer;
  external MEDLL index 414;

function GSVgetCanBaud(no:Integer):Integer;
  external MEDLL index 415;

function GSVisCanAvailable(no:Integer):Integer;
  external MEDLL index 417;

function GSVsetCanActive(no, active:Integer):Integer;
  external MEDLL index 418;

function GSVgetCanActive(no:Integer):Integer;
  external MEDLL index 419;

function GSVsetCanMode20B(no, mode20b:Integer):Integer;
  external MEDLL index 420;

function GSVgetCanMode20B(no:Integer):Integer;
  external MEDLL index 421;

function GSVsetUserText(no:Integer; txt:PChar):Integer;
  external MEDLL index 448;

{$ENDIF}

end.
