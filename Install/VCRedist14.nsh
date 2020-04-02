; ----------------------
;     VCRedist14.nsh
; ----------------------
;
; Macros to help in installing the Visual C++ 14.0 redistributables (belonging to Visual Studio 2015.)
;
; Copyright (c) 2015 Uber Entertainment, Inc. All rights reserved.
; Authored by Jørgen P. Tjernø <jorgenpt@gmail.com>
; Adapted for vcredist 14.0 detection by Jackie Ng <jumpinjackie@gmail.com>
;
; Licensed under the MIT license, see the LICENSE file in the current directory.
;
;
; This file will define two macros:
;   InstallVCRedist14_32bit and InstallVCRedist14_64bit. The latter will only do something if it's run on a 64-bit OS, otherwise it's a no-op.
; They both take a parameter as to where they put temporary files, typically used something like:
;
;   !insertmacro InstallVCRedist14_32bit "${TEMP}\MyProduct"
;   !insertmacro InstallVCRedist14_64bit "${TEMP}\MyProduct"
;   RMDir /r "${TEMP}\MyProduct"
;
; The macros expect you to have vcredist_x86.exe and vcredist_x64.exe in the current directory, respectively. They only work right if you use them with the
; VC14.0 (Visual Studio 2015) redistributable, as they check for a specific registry key to see if the redistributable has already been installed.
;
;
; You can define the following macro in your code to control how status messages are updated:
;   !macro SetStatus Status
;       DetailPrint "Aw yiss: ${Status}"
;   !macroend
;
; You can define the following macro in your code to control wether we check for the redist before installing it. This example will *always* install them, and never check.
;   !macro SkipIfForceDependencies Label
;       Goto ${Label}
;   !macroend
;
; You can define the following macro in your code to control how error checking is done. It will be called after our ExecWait calls.
;   !macro MessageBoxIfError Message
;       IfErrors 0 +2
;           MessageBox MB_OK|MB_ICONSTOP "${Message}"
;   !macroend

!ifndef ___VC_REDIST_14__NSH___
!define ___VC_REDIST_14__NSH___

!include "X64.nsh"

!macro _VCRedist14_SetStatus Status
    !ifmacrodef SetStatus
        !insertmacro SetStatus "${Status}"
    !else
        DetailPrint "${Status}"
    !endif
!macroend

!macro _VCRedist14_SkipIfForceDependencies Label
    !ifmacrodef SkipIfForceDependencies
        !insertmacro SkipIfForceDependencies ${Label}
    !endif
!macroend

!macro _VCRedist14_MessageBoxIfError Message
    !ifmacrodef MessageBoxIfError
        !insertmacro MessageBoxIfError "${Message}"
    !endif
!macroend

!macro InstallVCRedist14_32bit TempOutPath
    ; Check if 32-bitVisual Studio 2012 redistributable is already installed
    !insertmacro _VCRedist14_SetStatus "Checking for Visual C++ Redistributable for Visual Studio 2015 (32-bit)"
    !insertmacro _VCRedist14_SkipIfForceDependencies +3
    ReadRegStr $1 HKLM "Software\Microsoft\DevDiv\vc\Servicing\14.0\RuntimeMinimum" Install
    StrCmp $1 "1" InstallVCRedist14_32bitFinish

    ; Install 32-bit Visual Studio 2012 Redistributable
    !insertmacro _VCRedist14_SetStatus "Extracting Visual C++ 2015 Redistributable (32-bit)"
    SetOutPath ${TempOutPath}
    File "vcredist_x86.exe"

    !insertmacro _VCRedist14_SetStatus "Installing Visual C++ 2015 Redistributable (32-bit)"
    ClearErrors
    ExecWait '"${TempOutPath}\vcredist_x86.exe" /s /v" /qn"'
    !insertmacro _VCRedist14_MessageBoxIfError "Failed to install 32-bit Visual C++ 2015 Redistributable."
InstallVCRedist14_32bitFinish:
!macroend

!macro InstallVCRedist14_64bit TempOutPath
    ${If} ${RunningX64}
        ; Check if 64-bit Visual Studio 2012 redistributable is already installed
        !insertmacro _VCRedist14_SetStatus "Checking for Visual C++ Redistributable for Visual Studio 2015 (64-bit)"
        !insertmacro _VCRedist14_SkipIfForceDependencies +4
        SetRegView 64
        ReadRegStr $1 HKLM "Software\Microsoft\DevDiv\vc\Servicing\14.0\RuntimeMinimum" Install
        SetRegView lastused
        StrCmp $1 "1" InstallVCRedist14_64bitFinish

        ; Install 64-bit Visual Studio 2012 Redistributable
        !insertmacro _VCRedist14_SetStatus "Extracting Visual C++ 2015 Redistributable (64-bit)"
        SetOutPath ${TempOutPath}
        File "vcredist_x64.exe"

        !insertmacro _VCRedist14_SetStatus "Installing Visual C++ 2015 Redistributable (64-bit)"
        ClearErrors
        ExecWait '"${TempOutPath}\vcredist_x64.exe" /s /v" /qn"'
        !insertmacro _VCRedist14_MessageBoxIfError "Failed to install 64-bit Visual C++ 2015 Redistributable."
InstallVCRedist14_64bitFinish:
    ${EndIf}
!macroend

!endif
