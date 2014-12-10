; Setup-Script.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install Setup-Script.nsi into a directory that the user selects,

;--------------------------------

; The name of the installer
Name "ArmaBrowser"

; The file to write
OutFile "ArmaBrowser-Setup.exe"

; The default installation directory
InstallDir $LOCALAPPDATA\ArmaBrowser

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\ArmaBrowser" "Install_Dir"


VIProductVersion                 "0.5.3.50"
VIAddVersionKey ProductName      "ArmaBrowser"
VIAddVersionKey Comments         ""
VIAddVersionKey CompanyName      "armabrowser.org"
VIAddVersionKey LegalCopyright   "2024"
VIAddVersionKey FileDescription  ""
VIAddVersionKey FileVersion      "0.5.3.50"
VIAddVersionKey ProductVersion   "0.5.3.50"
VIAddVersionKey InternalName     "ArmaBrowser"
VIAddVersionKey LegalTrademarks  ""
VIAddVersionKey OriginalFilename "ArmaBrowser-Setup.exe"


; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "ArmaBrowser"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "ArmaBrowser\bin\Release\ArmaBrowser.exe"
  File "ArmaBrowser\bin\Release\ArmaBrowser.exe.config"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\armabrowser" "Install_Dir" "$INSTDIR"
  WriteRegStr HKLM "Software\armabrowser" "DisplayVersion" "0.5.3"
  WriteRegStr HKLM "Software\armabrowser" "Publisher" "armabrowser.org"
  WriteRegStr HKLM "Software\armabrowser" "DisplayIcon" '"$INSTDIR\ArmaBrowser.exe"'
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "DisplayName" "ArmaBrowser"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "DisplayVersion" "0.5.3"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "Publisher" "armabrowser.org"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "DisplayIcon" '"$INSTDIR\ArmaBrowser.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "URLInfoAbout" "http://armabrowser.org"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
  CreateDirectory "$SMPROGRAMS\ArmaBrowser"
  ;CreateShortCut "$SMPROGRAMS\ArmaBrowser\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\ArmaBrowser\Start ArmaBrowser.lnk" "$INSTDIR\ArmaBrowser.exe" "" "$INSTDIR\ArmaBrowser.exe" 0
  
SectionEnd

; Optional section (can be disabled by the user)
; Section "Start Menu Shortcuts"
; 
;   CreateDirectory "$SMPROGRAMS\Example2"
;   CreateShortCut "$SMPROGRAMS\Example2\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
;   CreateShortCut "$SMPROGRAMS\Example2\Example2 (MakeNSISW).lnk" "$INSTDIR\example2.nsi" "" "$INSTDIR\example2.nsi" 0
;   
; SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\armabrowser"
  DeleteRegKey HKLM SOFTWARE\armabrowser

  ; Remove files and uninstaller
  Delete $INSTDIR\ArmaBrowser.exe
  Delete $INSTDIR\ArmaBrowser.exe.config
  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\ArmaBrowser\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\ArmaBrowser"
  RMDir "$INSTDIR"

SectionEnd
