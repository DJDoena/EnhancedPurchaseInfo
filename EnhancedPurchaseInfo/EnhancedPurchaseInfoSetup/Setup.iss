[Setup]
AppName=Enhanced Purchase Info
AppId=EnhancedPurchaseInfo
AppVerName=Enhanced Purchase Info 1.1.4
AppCopyright=Copyright � Doena Soft. 2015 - 2025
AppPublisher=Doena Soft.
AppPublisherURL=http://doena-journal.net/en/dvd-profiler-tools/
DefaultDirName={commonpf32}\Doena Soft.\Enhanced Purchase Info
DefaultGroupName=Enhanced Purchase Info
DirExistsWarning=No
SourceDir=..\EnhancedPurchaseInfo\bin\x86\Release\net481
Compression=zip/9
AppMutex=InvelosDVDPro
OutputBaseFilename=EnhancedPurchaseInfoSetup
OutputDir=..\..\..\..\..\EnhancedPurchaseInfoSetup\Setup\EnhancedPurchaseInfo
MinVersion=0,6.1sp1
PrivilegesRequired=admin
WizardStyle=modern
DisableReadyPage=yes
ShowLanguageDialog=no
VersionInfoCompany=Doena Soft.
VersionInfoCopyright=2015 - 2025
VersionInfoDescription=Enhanced Purchase Info Setup
VersionInfoVersion=1.1.4
UninstallDisplayIcon={app}\djdsoft.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
WinVersionTooLowError=This program requires Windows XP or above to be installed.%n%nWindows 9x, NT and 2000 are not supported.

[Types]
Name: "full"; Description: "Full installation"

[Files]
Source: "djdsoft.ico"; DestDir: "{app}"; Flags: ignoreversion

Source: "*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion

Source: "PurchasePriceSplitter.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "PurchasePriceSplitter.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "PurchasePriceSplitter.pdb"; DestDir: "{app}"; Flags: ignoreversion

Source: "de\*.dll"; DestDir: "{app}\de"; Flags: ignoreversion

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Item Prices"; Filename: "{app}\PurchasePriceSplitter.exe"; WorkingDir: "{app}"; IconFilename: "{app}\djdsoft.ico"

[Run]
Filename: "{win}\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"; Parameters: "/codebase ""{app}\DoenaSoft.EnhancedPurchaseInfo.dll"""; Flags: runhidden

;[UninstallDelete]

[UninstallRun]
Filename: "{win}\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"; Parameters: "/u ""{app}\DoenaSoft.EnhancedPurchaseInfo.dll"""; Flags: runhidden

[Registry]
; Register - Cleanup ahead of time in case the user didn't uninstall the previous version.
Root: HKCR; Subkey: "CLSID\{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; Flags: dontcreatekey deletekey
Root: HKCR; Subkey: "DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Plugin"; Flags: dontcreatekey deletekey
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler\Plugins\Identified"; ValueType: none; ValueName: "{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; ValueData: "0"; Flags: deletevalue
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler_beta\Plugins\Identified"; ValueType: none; ValueName: "{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; ValueData: "0"; Flags: deletevalue
Root: HKLM; Subkey: "Software\Classes\CLSID\{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; Flags: dontcreatekey deletekey
Root: HKLM; Subkey: "Software\Classes\DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Plugin"; Flags: dontcreatekey deletekey
; Unregister
Root: HKCR; Subkey: "CLSID\{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; Flags: dontcreatekey uninsdeletekey
Root: HKCR; Subkey: "DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Plugin"; Flags: dontcreatekey uninsdeletekey
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler\Plugins\Identified"; ValueType: none; ValueName: "{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; ValueData: "0"; Flags: uninsdeletevalue
Root: HKCU; Subkey: "Software\Invelos Software\DVD Profiler_beta\Plugins\Identified"; ValueType: none; ValueName: "{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; ValueData: "0"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\Classes\CLSID\{{8BDEDB77-38A9-436B-83B2-8DB82E495319}"; Flags: dontcreatekey uninsdeletekey
Root: HKLM; Subkey: "Software\Classes\DoenaSoft.DVDProfiler.EnhancedPurchaseInfo.Plugin"; Flags: dontcreatekey uninsdeletekey

[Code]
function IsDotNET4Detected(): boolean;
// Function to detect dotNet framework version 4
// Returns true if it is available, false it's not.
var
dotNetStatus: boolean;
begin
dotNetStatus := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4');
Result := dotNetStatus;
end;

function InitializeSetup(): Boolean;
// Called at the beginning of the setup package.
begin

if not IsDotNET4Detected then
begin
MsgBox( 'The Microsoft .NET Framework version 4 is not installed. Please install it and try again.', mbInformation, MB_OK );
Result := false;
end
else
Result := true;
end;