SetCompress force
SetCompressor /SOLID lzma
Name "TheSign"
OutFile "TheSign_Setup.exe"
XPStyle on

InstallDir "$DESKTOP\TheSign"
InstallDirRegKey HKCU "SOFTWARE\Koehler_Programms\TheSign" ExePath
Page license
Page directory
Page instfiles
UninstPage uninstConfirm
UninstPage instfiles

LicenseData gpl-3.0.txt
LicenseForceSelection checkbox

Section "TheSign"

#!include "ZipDLL.nsh"
 # 	ReadRegStr $InstDir HKCU "SOFTWARE\Koehler_Programms\TheSign" ExePath
#	; Pop $R0 ;Get the return value
 # 	StrCmp $InstDir "" 0 +2
#	$InstDir "$DESKTOP\TheSign"

SetOutPath $INSTDIR
CreateDirectory "$INSTDIR\SignedFiles"
CreateDirectory "$INSTDIR\GnuPG"

File /oname=GnuPG\iconv.dll GnuPG\iconv.dll 
File /oname=GnuPG\gpg.exe GnuPG\gpg.exe
File /oname=GnuPG\startkey.gpg GnuPG\startkey.gpg

File SignedFiles\authorities.xml

File create_key.bat
File SignBrowser\bin\Release\SignBrowser.exe
File SignBrowser\bin\Release\TheSign.exe
File Microsoft.Office.Interop.Outlook.dll

File "TheSign.url"

WriteUninstaller "$INSTDIR\uninstaller.exe"
CreateDirectory "$INSTDIR\SignedFiles"
CreateDirectory "$SMPROGRAMS\TheSign"
CreateShortCut "$SMPROGRAMS\TheSign\TheSign.lnk" "$INSTDIR\TheSign.exe"
CreateShortCut "$SMPROGRAMS\TheSign\SignBrowser.lnk" "$INSTDIR\SignBrowser.exe"
CreateShortCut "$SMPROGRAMS\TheSign\TheSign Homepage.lnk" "$INSTDIR\TheSign.url"
CreateShortCut "$SMPROGRAMS\TheSign\Uninstall TheSign.lnk" "$INSTDIR\uninstaller.exe"
WriteRegStr HKCU "SOFTWARE\Koehler_Programms\TheSign" "ExePath" $INSTDIR
SectionEnd

Section "un.Uninstall"
Delete "$INSTDIR\GnuPG\iconv.dll"
Delete "$INSTDIR\GnuPG\gpg.exe"
Delete "$INSTDIR\GnuPG\startkey.gpg"

Delete "$INSTDIR\TheSign.exe"
Delete "$INSTDIR\SignBrowser.exe"
Delete "$INSTDIR\Microsoft.Office.Interop.Outlook.dll"
Delete "$INSTDIR\create_key.bat"

Delete "$INSTDIR\TheSign.url"
Delete "$INSTDIR\uninstaller.exe"
Delete "$INSTDIR\authorities.xml"

Delete "$SMPROGRAMS\TheSign\TheSign.lnk"
Delete "$SMPROGRAMS\TheSign\SignBrowser.lnk"
Delete "$SMPROGRAMS\TheSign\TheSign Homepage.lnk"
Delete "$SMPROGRAMS\TheSign\Uninstall TheSign.lnk"
RMDir "$SMPROGRAMS\TheSign"


Delete "$SMPROGRAMS\SKDS3\SKDS3.lnk"
Delete "$SMPROGRAMS\SKDS3\Update Custom Data.lnk"
Delete "$SMPROGRAMS\SKDS3\View SKDS Scripts.lnk"
Delete "$SMPROGRAMS\SKDS3\Copy SKDS to Stick.lnk"
Delete "$SMPROGRAMS\SKDS3\HyperTerp Online Help.lnk"
Delete "$SMPROGRAMS\SKDS3\SKDS3 Homepage.lnk"
Delete "$SMPROGRAMS\SKDS3\SKDS Classic (K-Line).lnk"
Delete "$SMPROGRAMS\SKDS3\Uninstall SKDS3.lnk"
RMDir "$SMPROGRAMS\SKDS3"
DeleteRegValue HKCU "SOFTWARE\Koehler_Programms" "TheSign"
SectionEnd

