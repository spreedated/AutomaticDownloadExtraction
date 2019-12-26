; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "nxn-AutoExtractService"
#define MyAppVersion "1.2.0"
#define MyAppPublisher "neXn-Systems"
#define MyAppURL "http://www.nexn.systems"
#define MyAppExeName "nxn-AutoExtractService.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C8DB50EC-F8E1-414A-9383-9A2549ECFFC6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=no
; Uncomment the following line to run in non administrative install mode (install for current user only.)
PrivilegesRequired=admin 
;PrivilegesRequiredOverridesAllowed=commandline
OutputBaseFilename=AutoExtractServiceSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "C:\Users\SpReeD\Documents\Visual Studio 2019\Projects\nxn-AutoExtractService\bin\Release\nxn-AutoExtractService.exe"; DestDir: "{commonpf64}\neXn-Systems\nxn-AutoExtractService"; Flags: ignoreversion
Source: "C:\Users\SpReeD\Documents\Visual Studio 2019\Projects\nxn-AutoExtractService\bin\Release\x64\7z.dll"; DestDir: "{commonpf64}\neXn-Systems\nxn-AutoExtractService\x64"; Flags: ignoreversion

[run]
Filename: {sys}\sc.exe; Parameters: "create nxn-AutoExtractService start=delayed-auto binPath= ""{commonpf64}\neXn-Systems\nxn-AutoExtractService\nxn-AutoExtractService.exe""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start nxn-AutoExtractService" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop nxn-AutoExtractService" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete nxn-AutoExtractService" ; Flags: runhidden

