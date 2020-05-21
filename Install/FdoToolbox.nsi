;----------------------------------------------------------------------
; NSIS Installer script for FDO Toolbox
;
; Author: Jackie Ng (jumpinjackie@gmail.com)
; 
; 
;----------------------------------------------------------------------

!addplugindir .\NSISPlugins

;----------------------
; Include NSIS headers
;----------------------

# .net Checker
!include "DotNetChecker.nsh"

# Modern UI 2
!include "MUI2.nsh"

# File functions
!include "FileFunc.nsh"

!include "WordFunc.nsh"
!include "LogicLib.nsh"

# VCRedist detection
!include "VCRedist14.nsh"

;-------------------------------
; Installer compilation settings
;-------------------------------

SetCompressor /SOLID /FINAL lzma

;-------------------------------
; Windows > Vista settings
;-------------------------------

RequestExecutionLevel admin

;-------------------
; Script variables
;-------------------

# Globals
!ifndef SLN_CONFIG
	!define SLN_CONFIG "Release" #"Debug"
!endif

!ifndef CPU
	!define CPU "x86"
!endif

!echo "Building installer in configuration: ${SLN_CONFIG} (${CPU})"

!define SLN_DIR ".."
!define SLN_THIRDPARTY "${SLN_DIR}\Thirdparty"
!ifndef RELEASE_VERSION
	!define RELEASE_VERSION "Trunk"
!endif

!ifndef RELEASE_LABEL
	!define RELEASE_LABEL "${RELEASE_VERSION}"
!endif

# Installer vars
!define INST_PRODUCT "FDO Toolbox"
!if ${SLN_CONFIG} == "Release"
	!define INST_PRODUCT_QUALIFIED "${INST_PRODUCT} (${CPU})"
	!define INST_PRODUCT_NAME "${INST_PRODUCT_QUALIFIED} ${RELEASE_LABEL} "
!else
	!define INST_PRODUCT_QUALIFIED "${INST_PRODUCT} (${CPU}, Debug)"
	!define INST_PRODUCT_NAME "${INST_PRODUCT_QUALIFIED} ${RELEASE_LABEL}"
!endif

!define PROJECT_URL "https://github.com/jumpinjackie/fdotoolbox"
!define INST_SRC "."
!define INST_LICENSE "..\FdoToolbox\license.txt"
!define INST_OUTPUT "FDOToolbox-${SLN_CONFIG}-${RELEASE_LABEL}-${CPU}-Setup.exe"

!if ${RELEASE_VERSION} != "master"
	VIProductVersion "${RELEASE_VERSION}"
	VIAddVersionKey "ProductName" "${INST_PRODUCT_NAME}"
	VIAddVersionKey "LegalCopyright" "� 2011 Jackie Ng"
	VIAddVersionKey "FileDescription" "Installer package for FDO Toolbox"
	VIAddVersionKey "FileVersion" "${RELEASE_VERSION}"
!endif

!define REG_KEY_UNINSTALL "Software\Microsoft\Windows\CurrentVersion\Uninstall\${INST_PRODUCT_QUALIFIED}"

# Project Output
!define INST_OUTPUT_FDOTOOLBOX "${SLN_DIR}\out\${CPU}\${SLN_CONFIG}"
!define INST_OUTDIR "${SLN_DIR}\out"

# Executables
!define EXE_FDOTOOLBOX "FdoToolbox.exe"

# Shortcuts
!define LNK_FDOTOOLBOX "${INST_PRODUCT_QUALIFIED}"

;-------------------
; General
;-------------------

; Name and file
Name "${INST_PRODUCT}"
Caption "${INST_PRODUCT_NAME} Setup"
OutFile "${INST_OUTDIR}\${INST_OUTPUT}"

; Default installation folder
!if ${CPU} == "x64"
InstallDir "$PROGRAMFILES64\${INST_PRODUCT}"
!else
InstallDir "$PROGRAMFILES\${INST_PRODUCT}"
!endif

!ifdef INST_LICENSE
LicenseText "License"
LicenseData "${INST_SRC}\${INST_LICENSE}"
!endif

;-------------------
; Interface Settings
;-------------------
!define MUI_ABORTWARNING

;-------------------
; Pages
;-------------------

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "${INST_LICENSE}"
#!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
    # These indented statements modify settings for MUI_PAGE_FINISH
    !define MUI_FINISHPAGE_NOAUTOCLOSE
    !define MUI_FINISHPAGE_RUN
    !define MUI_FINISHPAGE_RUN_CHECKED
    !define MUI_FINISHPAGE_RUN_TEXT "Run FDO Toolbox"
    !define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;-------------------
; Languages
;-------------------

!insertmacro MUI_LANGUAGE "English"

;-------------------
; Installer Sections
;-------------------

# default section
Section 
	; Windows > Vista
	SetShellVarContext all

	; Registry
	!if ${CPU} == "x64"
	SetRegView 64
	!else
	SetRegView 32
	!endif

	# Check for .net Framework
    !insertmacro CheckNetFramework 48

    # Check for vcredist
    !insertmacro InstallVCRedist14_64bit "$TEMP\FdoToolboxSetup"

	# set installation dir
	SetOutPath $INSTDIR
	
	# directories
	File /r "${INST_OUTPUT_FDOTOOLBOX}\FDO"
	File /r "${INST_OUTPUT_FDOTOOLBOX}\AddIns"
	File /r "${INST_OUTPUT_FDOTOOLBOX}\Schemas"
	# File /r "${INST_OUTPUT_FDOTOOLBOX}\Scripts"
	
	# docs
	File "${INST_OUTPUT_FDOTOOLBOX}\${HELP_USER}"
	# File "${INST_OUTPUT_FDOTOOLBOX}\${HELP_API}"
	File "${INST_OUTPUT_FDOTOOLBOX}\changelog.txt"
	File "${INST_OUTPUT_FDOTOOLBOX}\license.txt"
	File "${INST_OUTPUT_FDOTOOLBOX}\cmd_readme.txt"
	
	# data/config files
	File "${INST_OUTPUT_FDOTOOLBOX}\cscatalog.sqlite"
	#File "${INST_OUTPUT_FDOTOOLBOX}\ICSharpCode.Core.xml"
	#File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Base.XML"
	#File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Core.XML"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.exe.config"
	
	# libraries
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Base.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.Core.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\ICSharpCode.Core.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\ICSharpCode.TextEditor.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\log4net.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\SharpMap.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\SharpMap.UI.dll"
	File "${INST_OUTPUT_FDOTOOLBOX}\WeifenLuo.WinFormsUI.Docking.dll"
	
	# Scripting
	#File "${INST_OUTPUT_FDOTOOLBOX}\ipy.exe"
	#File "${INST_OUTPUT_FDOTOOLBOX}\ipyw.exe"
	#File "${INST_OUTPUT_FDOTOOLBOX}\IronPython.dll"
	#File "${INST_OUTPUT_FDOTOOLBOX}\IronPython.Modules.dll"
	#File "${INST_OUTPUT_FDOTOOLBOX}\Microsoft.Scripting.Core.dll"
	#File "${INST_OUTPUT_FDOTOOLBOX}\Microsoft.Scripting.dll"
	#File "${INST_OUTPUT_FDOTOOLBOX}\Microsoft.Scripting.ExtensionAttribute.dll"
	
	# main executables
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoCmd.exe"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoCmd.exe.config"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.exe"
	File "${INST_OUTPUT_FDOTOOLBOX}\FdoToolbox.exe.config"
	
	# create uninstaller
	WriteUninstaller "$INSTDIR\uninstall.exe"
	
	# create Add/Remove Programs entry
	WriteRegStr HKLM "${REG_KEY_UNINSTALL}" \
					 "DisplayName" "${INST_PRODUCT_NAME}"

	WriteRegStr HKLM "${REG_KEY_UNINSTALL}" \
					 "UninstallString" "$INSTDIR\uninstall.exe"
	
	WriteRegStr HKLM "${REG_KEY_UNINSTALL}" \
					 "URLInfoAbout" "${PROJECT_URL}"
	
	WriteRegStr HKLM "${REG_KEY_UNINSTALL}" \
					 "DisplayVersion" "${RELEASE_VERSION}"
	
	# TODO: Add more useful information to Add/Remove programs
	# See: http://nsis.sourceforge.net/Add_uninstall_information_to_Add/Remove_Programs
	
	# create FDO Toolbox shortcuts. Use INST_PRODUCT_QUALIFIED so we can have x86 and x64 entries that don't clash
	CreateDirectory "$SMPROGRAMS\${INST_PRODUCT_QUALIFIED}"
	
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT_QUALIFIED}\${LNK_FDOTOOLBOX}.lnk" "$INSTDIR\${EXE_FDOTOOLBOX}"
	CreateShortCut "$SMPROGRAMS\${INST_PRODUCT_QUALIFIED}\Uninstall.lnk" "$INSTDIR\uninstall.exe"
	
	CreateShortCut "$DESKTOP\${LNK_FDOTOOLBOX}.lnk" "$INSTDIR\${EXE_FDOTOOLBOX}"
	
SectionEnd

# uninstall section
Section "uninstall"
	; Windows > Vista
	SetShellVarContext all

	; Registry
	!if ${CPU} == "x64"
	SetRegView 64
	!else
	SetRegView 32
	!endif

    # remove uninstaller
	Delete "$INSTDIR\uninstall.exe"
	
	# remove desktop shortcut
	Delete "$DESKTOP\${LNK_FDOTOOLBOX}.lnk"
	
	# remove Add/Remove programs registry entry
	DeleteRegKey HKLM "${REG_KEY_UNINSTALL}"
	
	# remove installation directory
	RMDir /r "$INSTDIR"
	
	# remove shortcuts
	RMDir /r "$SMPROGRAMS\${INST_PRODUCT_QUALIFIED}"
SectionEnd

Function .onInit
	; Registry
	!if ${CPU} == "x64"
	SetRegView 64
	!else
	SetRegView 32
	!endif
	
	!insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

Function LaunchLink
	; TODO: Needs to launch under standard user. If installer was run under UAC elevated privileges, it will run under the
	; user who elevated these privileges.
	ExecShell "" "$INSTDIR\${EXE_FDOTOOLBOX}"
FunctionEnd
