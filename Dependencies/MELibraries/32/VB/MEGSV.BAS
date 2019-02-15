Attribute VB_Name = "ME_GSV_API"
'*************************************************'
'*                                               *'
'*  VB-Module for GSV-Functions in MEGSV.DLL     *'
'*  Version 1.50                                 *'
'*                                               *'
'*  Copyright (C) Dr. Holger Kabelitz 1999-2009  *'
'*  All rights reserved.                         *'
'*                                               *'
'*  Dr. Holger Kabelitz                          *'
'*  D-13507 Berlin                               *'
'*  Germany                                      *'
'*                                               *'
'*************************************************'
Option Explicit
Public Const GSV_OK As Long = 0
Public Const GSV_ERROR As Long = -1
Public Const GSV_TRUE As Long = 1
Public Const GSV_MEMORY_OK As Byte = GSV_OK
Public Const GSV_MEMORY_OK_FUNCTION As Byte = &H1
Public Const GSV_MEMORY_ERROR_ABORT As Byte = &HFF
Public Const GSV_MEMORY_ERROR_SEVERE As Byte = &H10
Public Const GSV_MEMORY_ERROR_NA As Byte = &H11
Public Const GSV_MEMORY_ERROR_DATA As Byte = &H12
Public Const GSV_MEMORY_ERROR_FAILED As Byte = &H1E
Public Const GSV_MEMORY_ERROR_DENIED As Byte = &H1F
Public Const GSV_LINEAR_EXECUTE As Long = 0&
Public Const GSV_LINEAR_TEST_ALL As Long = 1&
Public Const GSV_LINEAR_TEST_SLOPES As Long = 2&
Public Const GSV_LINEAR_TEST_POINTS As Long = 3&
Public Const GSV_LINEAR_SORT As Long = &H4000&
Public Const GSV_LINEAR_INFO As Long = 0
Public Const GSV_LINEAR_INITIALIZE As Long = -4
Public Const GSV_LINEAR_OFFSETZERO As Long = -2
Public Const GSV_LINEAR_COUNTER As Long = 1
Public Const GSV_LINEAR_ERROR_CODE As Long = &H3C00&
Public Const GSV_LINEAR_ERROR_POINT As Long = &HFF&
Public Const GSV_LINEAR_ERROR_ACT As Long = &H100&
Public Const GSV_LINEAR_ERROR_REF As Long = &H200&
Public Const GSV_LINEAR_ERROR_OK As Long = GSV_OK
Public Const GSV_LINEAR_ERROR_NOIFC As Long = &H400&
Public Const GSV_LINEAR_ERROR_UNKNOWNIFC As Long = &H800&
Public Const GSV_LINEAR_ERROR_NPOINTS As Long = &HC00&
Public Const GSV_LINEAR_ERROR_RANGE As Long = &H1000&
Public Const GSV_LINEAR_ERROR_ORDER As Long = &H1400&
Public Const GSV_LINEAR_ERROR_SLOPE As Long = &H1800&
Public Const GSV_LINEAR_ERROR_LOCKED As Long = &H1C00&
Public Const GSV_LINEAR_ERROR_INCOMPLETE As Long = &H2000&
Public Const GSV_LINEAR_ERROR_PARAMETER As Long = &H2400&
Public Const GSV_LINEAR_ERROR_GAIN As Long = &H2800&
Public Const GSV_LINEAR_ERROR_SENSITIVITY As Long = &H2C00&
Public Const GSV_LINEAR_ERROR_NOTIMPLEMENTED As Long = &H3000&
Public Const GSV_ACTEX_FLAG_BAUDRATE As Long = &H1&
Public Const GSV_ACTEX_FLAG_WAKEUP As Long = &H2&
Public Const GSV_ACTEX_FLAG_HANDSHAKE As Long = &H4&
Public Const GSV_STATUS_MASK_SWITCH As Byte = &H10
Public Const GSV_STATUS_MASK_SWITCH2 As Byte = &H8
Public Const GSV_CANSET_MANUFACTURER_ID As Long = &H0&
Public Const GSV_CANSET_COMMAND_ID As Long = &H1&
Public Const GSV_CANSET_MESSAGE_ID As Long = &H2&
Public Const GSV_CANSET_BAUD As Long = &H7E&
Public Const GSV_CANSET_FLAGS As Long = &H7F&
Public Const GSV_CANSET_20B As Long = &H80&
Public Const ME_INTSTAT_FLUSH As Long = &H1&
Public Type GSV_ACTIVATE_EXTENDED
    actex_size As Long
    actex_buffersize As Long
    actex_flags As Long
    actex_baudrate As Long
End Type
Public Declare Function GSVversion Lib "MEGSV" Alias "#1" () As Long
Public Declare Function GSVmodel Lib "MEGSV" Alias "#2" (ByVal no As Long) As Long
Public Declare Function MEreservedExec Lib "MEGSV" Alias "#3" (ByVal no As Long, ByVal code As Long, ByVal rexpro As Long, ByRef arg As Any) As Long
Public Declare Function MErequestEvent Lib "MEGSV" Alias "#4" (ByVal no As Long) As Long
Public Declare Function GSVrevision Lib "MEGSV" Alias "#5" () As Long
Public Declare Function GSVgetLocalBaudRate Lib "MEGSV" Alias "#8" (ByVal no As Long) As Long
Public Declare Function GSVactivateExtended Lib "MEGSV" Alias "#9" (ByVal no As Long, ByRef actex As GSV_ACTIVATE_EXTENDED) As Long
Public Declare Function GSVactivate Lib "MEGSV" Alias "#10" (ByVal no As Long, ByVal buffersize As Long) As Long
Public Declare Sub GSVrelease Lib "MEGSV" Alias "#11" (ByVal no As Long)
Public Declare Function GSVinitialize Lib "MEGSV" Alias "#12" (ByVal no As Long) As Long
Public Declare Function GSVflushBuffer Lib "MEGSV" Alias "#13" (ByVal no As Long) As Long
Public Declare Function MEinternalStatus Lib "MEGSV" Alias "#14" (ByVal no As Long) As Long
Public Declare Function GSVgetValues Lib "MEGSV" Alias "#17" (ByRef no As Long, ByVal n As Long) As Long
Public Declare Function MEsendBytesToDevices Lib "MEGSV" Alias "#18" (ByRef no As Long, ByVal n As Long, ByRef pbuf As Byte, ByVal count As Long) As Long
Public Declare Function MEsendBytes Lib "MEGSV" Alias "#19" (ByVal no As Long, ByRef pbuf As Byte, ByVal count As Long) As Long
Public Declare Function GSVreceived Lib "MEGSV" Alias "#20" (ByVal no As Long) As Long
Public Declare Function GSVread Lib "MEGSV" Alias "#21" (ByVal no As Long, ByRef ad As Double) As Long
Public Declare Function GSVreadMultiple Lib "MEGSV" Alias "#22" (ByVal no As Long, ByRef ad As Double, ByVal count As Long, ByRef valsread As Long) As Long
Public Declare Function GSVcopyMemory Lib "MEGSV" Alias "#23" (ByVal no As Long, ByVal dc As Long, ByVal addr As Long, ByRef buffer As Any) As Long
Public Declare Function GSVcheckMemory Lib "MEGSV" Alias "#24" (ByVal no As Long, ByVal dc As Long, ByVal addr As Long, ByRef buffer As Byte) As Long
Public Declare Function GSVreadStatus Lib "MEGSV" Alias "#25" (ByVal no As Long, ByRef ad As Double, ByRef ps As Byte) As Long
Public Declare Function GSVreadStatusMultiple Lib "MEGSV" Alias "#26" (ByVal no As Long, ByRef ad As Double, ByRef ps As Byte, ByVal count As Long, ByRef valsread As Long) As Long
Public Declare Function GSVcopyLinear Lib "MEGSV" Alias "#27" (ByVal no As Long, ByRef pd As Double, ByVal nd As Long, ByRef pr As Long) As Long
Public Declare Function MEasynchronousSendBytes Lib "MEGSV" Alias "#28" (ByVal no As Long, ByRef pbuf As Byte, ByVal count As Long, ByRef handle As Any) As Long
Public Declare Function MEsynchronizeSendBytes Lib "MEGSV" Alias "#29" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsCode Lib "MEGSV" Alias "#30" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsExtension3 Lib "MEGSV" Alias "#31" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsLinear Lib "MEGSV" Alias "#32" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsExtension21 Lib "MEGSV" Alias "#33" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsSleepMode Lib "MEGSV" Alias "#34" (ByVal no As Long) As Long
Public Declare Function GSVgetOptionsCommandTest Lib "MEGSV" Alias "#35" (ByVal no As Long) As Long
Public Declare Function MEwriteRanges Lib "MEGSV" Alias "#49" (ByVal no As Long, ByVal channel As Long, ByVal inx As Long, ByVal ia As Long) As Long
Public Declare Function GSVisBipol Lib "MEGSV" Alias "#50" (ByVal no As Long) As Long
Public Declare Function GSVgetFreq Lib "MEGSV" Alias "#51" (ByVal no As Long) As Double
Public Declare Function GSVgetGain Lib "MEGSV" Alias "#52" (ByVal no As Long) As Long
Public Declare Function GSVgetChannel Lib "MEGSV" Alias "#53" (ByVal no As Long) As Long
Public Declare Function MEsetModeLock Lib "MEGSV" Alias "#54" (ByVal no As Long, ByVal slock As Long, ByVal id As String) As Long
Public Declare Function GSVgetModeLock Lib "MEGSV" Alias "#55" (ByVal no As Long) As Long
Public Declare Function GSVsetModeLinear Lib "MEGSV" Alias "#56" (ByVal no As Long, ByVal lin As Long) As Long
Public Declare Function GSVgetModeLinear Lib "MEGSV" Alias "#57" (ByVal no As Long) As Long
Public Declare Function GSVsetModeAverage Lib "MEGSV" Alias "#58" (ByVal no As Long, ByVal avg As Long) As Long
Public Declare Function GSVgetModeAverage Lib "MEGSV" Alias "#59" (ByVal no As Long) As Long
Public Declare Function GSVsetModeSI Lib "MEGSV" Alias "#60" (ByVal no As Long, ByVal msi As Long) As Long
Public Declare Function GSVgetModeSI Lib "MEGSV" Alias "#61" (ByVal no As Long) As Long
Public Declare Function GSVsetModeText Lib "MEGSV" Alias "#62" (ByVal no As Long, ByVal mt As Long) As Long
Public Declare Function GSVgetModeText Lib "MEGSV" Alias "#63" (ByVal no As Long) As Long
Public Declare Function GSVsetModeMax Lib "MEGSV" Alias "#64" (ByVal no As Long, ByVal mx As Long) As Long
Public Declare Function GSVgetModeMax Lib "MEGSV" Alias "#65" (ByVal no As Long) As Long
Public Declare Function GSVsetModeLog Lib "MEGSV" Alias "#66" (ByVal no As Long, ByVal lg As Long) As Long
Public Declare Function GSVgetModeLog Lib "MEGSV" Alias "#67" (ByVal no As Long) As Long
Public Declare Function GSVsetModeWindow Lib "MEGSV" Alias "#68" (ByVal no As Long, ByVal win As Long) As Long
Public Declare Function GSVgetModeWindow Lib "MEGSV" Alias "#69" (ByVal no As Long) As Long
Public Declare Function GSVhasLCD Lib "MEGSV" Alias "#70" (ByVal no As Long) As Long
Public Declare Function GSVhasADC Lib "MEGSV" Alias "#71" (ByVal no As Long) As Long
Public Declare Function GSVhasUII Lib "MEGSV" Alias "#72" (ByVal no As Long) As Long
Public Declare Function GSVisSI Lib "MEGSV" Alias "#73" (ByVal no As Long) As Long
Public Declare Function GSVisWL Lib "MEGSV" Alias "#74" (ByVal no As Long) As Long
Public Declare Function GSVhasAF Lib "MEGSV" Alias "#75" (ByVal no As Long) As Long
Public Declare Function GSVsetBridgeType Lib "MEGSV" Alias "#80" (ByVal no As Long, ByVal bt As Long) As Long
Public Declare Function GSVgetBridgeType Lib "MEGSV" Alias "#81" (ByVal no As Long) As Long
Public Declare Function GSVsetBridgeInternal Lib "MEGSV" Alias "#84" (ByVal no As Long, ByVal bi As Long) As Long
Public Declare Function GSVgetBridgeInternal Lib "MEGSV" Alias "#85" (ByVal no As Long) As Long
Public Declare Function GSVresetStatus Lib "MEGSV" Alias "#100" (ByVal no As Long) As Long
Public Declare Function GSVgetScale Lib "MEGSV" Alias "#101" (ByVal no As Long) As Long
Public Declare Function GSVgetZero Lib "MEGSV" Alias "#102" (ByVal no As Long) As Long
Public Declare Function GSVgetControl Lib "MEGSV" Alias "#103" (ByVal no As Long) As Long
Public Declare Function GSVgetOffset Lib "MEGSV" Alias "#104" (ByVal no As Long) As Long
Public Declare Function GSVwriteScale Lib "MEGSV" Alias "#105" (ByVal no As Long, ByVal scalev As Long) As Long
Public Declare Function GSVwriteZero Lib "MEGSV" Alias "#106" (ByVal no As Long, ByVal zero As Long) As Long
Public Declare Function GSVwriteControl Lib "MEGSV" Alias "#107" (ByVal no As Long, ByVal control As Long) As Long
Public Declare Function GSVwriteOffset Lib "MEGSV" Alias "#108" (ByVal no As Long, ByVal offset As Long) As Long
Public Declare Function GSVgetAll Lib "MEGSV" Alias "#109" (ByVal no As Long, ByVal pos As Long) As Long
Public Declare Function GSVsaveAll Lib "MEGSV" Alias "#110" (ByVal no As Long, ByVal pos As Long) As Long
Public Declare Function GSVsetCal Lib "MEGSV" Alias "#111" (ByVal no As Long) As Long
Public Declare Function GSVsetZero Lib "MEGSV" Alias "#112" (ByVal no As Long) As Long
Public Declare Function GSVsetScale Lib "MEGSV" Alias "#113" (ByVal no As Long) As Long
Public Declare Function GSVsetOffset Lib "MEGSV" Alias "#114" (ByVal no As Long) As Long
Public Declare Function GSVDispSetUnit Lib "MEGSV" Alias "#115" (ByVal no As Long, ByVal dispunit As Long) As Long
Public Declare Function GSVDispSetNorm Lib "MEGSV" Alias "#116" (ByVal no As Long, ByVal norm As Double) As Long
Public Declare Function GSVDispSetDPoint Lib "MEGSV" Alias "#117" (ByVal no As Long, ByVal dpoint As Long) As Long
Public Declare Function GSVsetFreq Lib "MEGSV" Alias "#118" (ByVal no As Long, ByVal freq As Double) As Long
Public Declare Function GSVsetGain Lib "MEGSV" Alias "#119" (ByVal no As Long, ByVal gain As Long) As Long
Public Declare Function GSVsetBipolar Lib "MEGSV" Alias "#120" (ByVal no As Long) As Long
Public Declare Function GSVsetUnipolar Lib "MEGSV" Alias "#121" (ByVal no As Long) As Long
Public Declare Function MEsetCal Lib "MEGSV" Alias "#123" (ByVal no As Long, ByVal cal As Double) As Long
Public Declare Function MEgetCal Lib "MEGSV" Alias "#124" (ByVal no As Long) As Double
Public Declare Function MEsendID Lib "MEGSV" Alias "#125" (ByVal no As Long, ByVal id As String) As Long
Public Declare Function GSVDispGetNorm Lib "MEGSV" Alias "#126" (ByVal no As Long) As Double
Public Declare Function GSVDispGetUnit Lib "MEGSV" Alias "#127" (ByVal no As Long) As Long
Public Declare Function GSVDispGetDPoint Lib "MEGSV" Alias "#128" (ByVal no As Long) As Long
Public Declare Function GSVswitch Lib "MEGSV" Alias "#129" (ByVal no As Long, ByVal swon As Long) As Long
Public Declare Function MEwriteSerialNo Lib "MEGSV" Alias "#130" (ByVal no As Long, ByVal number As String) As Long
Private Declare Function MEreadSerialNo Lib "MEGSV" Alias "#131" (ByVal no As Long, ByVal number As String) As Long
Public Declare Function GSVsetThreshold Lib "MEGSV" Alias "#132" (ByVal no As Long, ByVal thon As Double, ByVal thoff As Double) As Long
Public Declare Function GSVgetThreshold Lib "MEGSV" Alias "#133" (ByVal no As Long, ByRef thon As Double, ByRef thoff As Double) As Long
Public Declare Function GSVsetChannel Lib "MEGSV" Alias "#134" (ByVal no As Long, ByVal channel As Long) As Long
Public Declare Function GSVstopTransmit Lib "MEGSV" Alias "#135" (ByVal no As Long) As Long
Public Declare Function GSVstartTransmit Lib "MEGSV" Alias "#136" (ByVal no As Long) As Long
Public Declare Function GSVclearBuffer Lib "MEGSV" Alias "#137" (ByVal no As Long) As Long
Public Declare Function GSVsetMode Lib "MEGSV" Alias "#138" (ByVal no As Long, ByVal mode As Long) As Long
Public Declare Function GSVgetMode Lib "MEGSV" Alias "#139" (ByVal no As Long) As Long
Public Declare Function MEwriteEquipment Lib "MEGSV" Alias "#140" (ByVal no As Long, ByVal equip As Long) As Long
Public Declare Function GSVgetEquipment Lib "MEGSV" Alias "#141" (ByVal no As Long) As Long
Public Declare Function GSVfirmwareVersion Lib "MEGSV" Alias "#143" (ByVal no As Long) As Long
Public Declare Function GSVsetGageFactor Lib "MEGSV" Alias "#144" (ByVal no As Long, ByVal gf As Double) As Long
Public Declare Function GSVgetGageFactor Lib "MEGSV" Alias "#145" (ByVal no As Long) As Double
Public Declare Function GSVsetPoisson Lib "MEGSV" Alias "#146" (ByVal no As Long, ByVal poiss As Double) As Long
Public Declare Function GSVgetPoisson Lib "MEGSV" Alias "#147" (ByVal no As Long) As Double
Public Declare Function GSVsetBridge Lib "MEGSV" Alias "#148" (ByVal no As Long, ByVal br As Long) As Long
Public Declare Function GSVgetBridge Lib "MEGSV" Alias "#149" (ByVal no As Long) As Long
Public Declare Function MEwriteRange Lib "MEGSV" Alias "#150" (ByVal no As Long, ByVal range As Double) As Long
Public Declare Function GSVgetRange Lib "MEGSV" Alias "#151" (ByVal no As Long) As Double
Public Declare Function MEsetOffsetWait Lib "MEGSV" Alias "#152" (ByVal no As Long, ByVal ow As Double) As Long
Public Declare Function MEgetOffsetWait Lib "MEGSV" Alias "#153" (ByVal no As Long) As Double
Public Declare Function GSVgetOptions Lib "MEGSV" Alias "#154" (ByVal no As Long) As Long
Public Declare Function MEwriteOptions Lib "MEGSV" Alias "#155" (ByVal no As Long, ByVal options As Long) As Long
Public Declare Function GSVreadMemory Lib "MEGSV" Alias "#156" (ByVal no As Long, ByVal addr As Long, ByRef buffer As Any, ByRef status As Long) As Long
Public Declare Function GSVwriteMemory Lib "MEGSV" Alias "#157" (ByVal no As Long, ByVal addr As Long, ByVal check As Long, ByVal value As Long, ByRef status As Long) As Long
Public Declare Function GSVgetMemoryWait Lib "MEGSV" Alias "#158" (ByVal no As Long) As Double
Public Declare Function GSVgetValue Lib "MEGSV" Alias "#159" (ByVal no As Long) As Long
Public Declare Function GSVclearMaxValue Lib "MEGSV" Alias "#160" (ByVal no As Long) As Long
Public Declare Function GSVDispSetDigits Lib "MEGSV" Alias "#161" (ByVal no As Long, ByVal digits As Long) As Long
Public Declare Function GSVDispGetDigits Lib "MEGSV" Alias "#162" (ByVal no As Long) As Long
Public Declare Function GSVunlockUII Lib "MEGSV" Alias "#163" (ByVal no As Long) As Long
Public Declare Function GSVlockUII Lib "MEGSV" Alias "#164" (ByVal no As Long) As Long
Public Declare Function GSVgetLastError Lib "MEGSV" Alias "#166" (ByVal no As Long) As Long
Public Declare Function GSVsetSecondThreshold Lib "MEGSV" Alias "#167" (ByVal no As Long, ByVal thon As Double, ByVal thoff As Double) As Long
Public Declare Function GSVgetSecondThreshold Lib "MEGSV" Alias "#168" (ByVal no As Long, ByRef thon As Double, ByRef thoff As Double) As Long
Public Declare Function GSVgetDeviceType Lib "MEGSV" Alias "#169" (ByVal no As Long) As Long
Public Declare Function GSVDispCalcNorm Lib "MEGSV" Alias "#170" (ByVal no As Long) As Long
Public Declare Function GSVsetTxMode Lib "MEGSV" Alias "#228" (ByVal no As Long, ByVal txmode As Long) As Long
Public Declare Function GSVgetTxMode Lib "MEGSV" Alias "#229" (ByVal no As Long) As Long
Public Declare Function GSVsetBaud Lib "MEGSV" Alias "#230" (ByVal no As Long, ByVal baud As Long) As Long
Public Declare Function GSVgetBaud Lib "MEGSV" Alias "#231" (ByVal no As Long) As Long
Public Declare Function GSVsetSlowRate Lib "MEGSV" Alias "#234" (ByVal no As Long, ByVal secs As Long) As Long
Public Declare Function GSVgetSlowRate Lib "MEGSV" Alias "#235" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialMode Lib "MEGSV" Alias "#236" (ByVal no As Long, ByVal smode As Long) As Long
Public Declare Function GSVgetSpecialMode Lib "MEGSV" Alias "#237" (ByVal no As Long) As Long
Public Declare Function GSVwriteSamplingRate Lib "MEGSV" Alias "#238" (ByVal no As Long, ByVal freq As Double, ByVal factor As Long) As Long
Public Declare Function GSVreadSamplingRate Lib "MEGSV" Alias "#239" (ByVal no As Long, ByRef freq As Double, ByRef factor As Long) As Long
Public Declare Function GSVsetCanSetting Lib "MEGSV" Alias "#240" (ByVal no As Long, ByVal stype As Long, ByVal val As Long) As Long
Public Declare Function GSVgetCanSetting Lib "MEGSV" Alias "#241" (ByVal no As Long, ByVal stype As Long) As Long
Public Declare Function GSVsetAnalogFilter Lib "MEGSV" Alias "#244" (ByVal no As Long, ByVal freq As Double) As Long
Public Declare Function GSVgetAnalogFilter Lib "MEGSV" Alias "#245" (ByVal no As Long) As Double
Public Declare Function GSVisCommandAvailable Lib "MEGSV" Alias "#247" (ByVal no As Long, ByVal cmd1 As Long, ByVal cmd2 As Long) As Long
Public Declare Function GSVsetNoiseCutThreshold Lib "MEGSV" Alias "#248" (ByVal no As Long, ByVal thr As Double) As Long
Public Declare Function GSVgetNoiseCutThreshold Lib "MEGSV" Alias "#249" (ByVal no As Long) As Double
Public Declare Function GSVsetAutoZeroCounter Lib "MEGSV" Alias "#250" (ByVal no As Long, ByVal ctr As Long) As Long
Public Declare Function GSVgetAutoZeroCounter Lib "MEGSV" Alias "#251" (ByVal no As Long) As Long
Public Declare Function GSVsetUserTextChar Lib "MEGSV" Alias "#252" (ByVal no As Long, ByVal addr As Long, ByVal c As Byte) As Long
Public Declare Function GSVgetRanges Lib "MEGSV" Alias "#262" (ByVal no As Long, ByVal channel As Long, ByVal inx As Long, ByRef prange As Double) As Long
Public Declare Function GSVsetRanges Lib "MEGSV" Alias "#263" (ByVal no As Long, ByVal channel As Long, ByVal inx As Long, ByVal range As Double) As Long
Public Declare Function GSVgetTxModeConfig Lib "MEGSV" Alias "#357" (ByVal no As Long) As Long
Public Declare Function GSVsetTxModeTransmit4 Lib "MEGSV" Alias "#358" (ByVal no As Long, ByVal t4 As Long) As Long
Public Declare Function GSVgetTxModeTransmit4 Lib "MEGSV" Alias "#359" (ByVal no As Long) As Long
Public Declare Function GSVsetTxModeRepeat3 Lib "MEGSV" Alias "#360" (ByVal no As Long, ByVal r3 As Long) As Long
Public Declare Function GSVgetTxModeRepeat3 Lib "MEGSV" Alias "#361" (ByVal no As Long) As Long
Public Declare Function GSVsetTxModeTransmit5 Lib "MEGSV" Alias "#362" (ByVal no As Long, ByVal t5 As Long) As Long
Public Declare Function GSVgetTxModeTransmit5 Lib "MEGSV" Alias "#363" (ByVal no As Long) As Long
Public Declare Function GSVsetTxModeReadOnly Lib "MEGSV" Alias "#364" (ByVal no As Long, ByVal ro As Long) As Long
Public Declare Function GSVgetTxModeReadOnly Lib "MEGSV" Alias "#365" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeSlow Lib "MEGSV" Alias "#372" (ByVal no As Long, ByVal slow As Long) As Long
Public Declare Function GSVgetSpecialModeSlow Lib "MEGSV" Alias "#373" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeAverage Lib "MEGSV" Alias "#374" (ByVal no As Long, ByVal avg As Long) As Long
Public Declare Function GSVgetSpecialModeAverage Lib "MEGSV" Alias "#375" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeFilter Lib "MEGSV" Alias "#376" (ByVal no As Long, ByVal flt As Long) As Long
Public Declare Function GSVgetSpecialModeFilter Lib "MEGSV" Alias "#377" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeMax Lib "MEGSV" Alias "#378" (ByVal no As Long, ByVal mx As Long) As Long
Public Declare Function GSVgetSpecialModeMax Lib "MEGSV" Alias "#379" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeFilterAuto Lib "MEGSV" Alias "#380" (ByVal no As Long, ByVal fltauto As Long) As Long
Public Declare Function GSVgetSpecialModeFilterAuto Lib "MEGSV" Alias "#381" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeFilterOrder5 Lib "MEGSV" Alias "#382" (ByVal no As Long, ByVal fltord5 As Long) As Long
Public Declare Function GSVgetSpecialModeFilterOrder5 Lib "MEGSV" Alias "#383" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeSleep Lib "MEGSV" Alias "#384" (ByVal no As Long, ByVal sleep As Long) As Long
Public Declare Function GSVgetSpecialModeSleep Lib "MEGSV" Alias "#385" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeAutoZero Lib "MEGSV" Alias "#400" (ByVal no As Long, ByVal autozero As Long) As Long
Public Declare Function GSVgetSpecialModeAutoZero Lib "MEGSV" Alias "#401" (ByVal no As Long) As Long
Public Declare Function GSVsetSpecialModeNoiseCut Lib "MEGSV" Alias "#402" (ByVal no As Long, ByVal noisecut As Long) As Long
Public Declare Function GSVgetSpecialModeNoiseCut Lib "MEGSV" Alias "#403" (ByVal no As Long) As Long
Public Declare Function GSVreadSamplingFrequency Lib "MEGSV" Alias "#404" (ByVal no As Long) As Double
Public Declare Function GSVreadSamplingFactor Lib "MEGSV" Alias "#405" (ByVal no As Long) As Long
Public Declare Function GSVsetBaudRate Lib "MEGSV" Alias "#406" (ByVal no As Long, ByVal baud As Long) As Long
Public Declare Function GSVgetBaudRate Lib "MEGSV" Alias "#407" (ByVal no As Long) As Long
Public Declare Function GSVsetCanBaudRate Lib "MEGSV" Alias "#408" (ByVal no As Long, ByVal baud As Long) As Long
Public Declare Function GSVgetCanBaudRate Lib "MEGSV" Alias "#409" (ByVal no As Long) As Long
Public Declare Function GSVisSamplingFrequencyProtocolLinear Lib "MEGSV" Alias "#410" (ByVal no As Long) As Long
Public Declare Function GSVisSamplingFactorProtocolLinear Lib "MEGSV" Alias "#411" (ByVal no As Long) As Long
Public Declare Function GSVsetCanID Lib "MEGSV" Alias "#412" (ByVal no As Long, ByVal idtype As Long, ByVal id As Long) As Long
Public Declare Function GSVgetCanID Lib "MEGSV" Alias "#413" (ByVal no As Long, ByVal idtype As Long) As Long
Public Declare Function GSVsetCanBaud Lib "MEGSV" Alias "#414" (ByVal no As Long, ByVal baud As Long) As Long
Public Declare Function GSVgetCanBaud Lib "MEGSV" Alias "#415" (ByVal no As Long) As Long
Public Declare Function GSVisCanAvailable Lib "MEGSV" Alias "#417" (ByVal no As Long) As Long
Public Declare Function GSVsetCanActive Lib "MEGSV" Alias "#418" (ByVal no As Long, ByVal active As Long) As Long
Public Declare Function GSVgetCanActive Lib "MEGSV" Alias "#419" (ByVal no As Long) As Long
Public Declare Function GSVsetCanMode20B Lib "MEGSV" Alias "#420" (ByVal no As Long, ByVal mode20b As Long) As Long
Public Declare Function GSVgetCanMode20B Lib "MEGSV" Alias "#421" (ByVal no As Long) As Long
Public Declare Function GSVsetUserText Lib "MEGSV" Alias "#448" (ByVal no As Long, ByVal txt As String) As Long
Public Function GSV_ACTEX_SIZE() As Long
    Dim dummy As GSV_ACTIVATE_EXTENDED
    GSV_ACTEX_SIZE = Len(dummy)
End Function
Public Function GSVgetSerialNo(ByVal no As Long, ByRef number As String) As Long
    Dim sernum As String, rcode As Long, cnum As Long
    sernum = String(10, vbNullChar)
    rcode = MEreadSerialNo(no, sernum)
    cnum = InStr(1, sernum, vbNullChar)
    If rcode = GSV_OK Then
        If cnum > 0 Then sernum = Left(sernum, cnum - 1)
        number = sernum
    End If
    GSVgetSerialNo = rcode
End Function
