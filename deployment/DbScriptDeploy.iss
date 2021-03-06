; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "DbScriptDeploy"
#define MyAppVersion "1.2.6"
#define MyAppPublisher "Matt Salmon"
#define MyAppURL "https://github.com/mrsalmon1976/DbScriptDeploy"
#define MyAppExeName "DbScriptDeploy.UI.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7DCF36B7-0E9C-4FA4-ABA2-9C45C33387C1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=DbScriptDeploy_{#MyAppVersion}
SetupIconFile=..\source\DbScriptDeploy.UI\Resources\app.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
DisableDirPage=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\source\DbScriptDeploy.UI\bin\Release\DbScriptDeploy.UI.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\source\DbScriptDeploy.UI\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion; Excludes: "user.projects" 
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent; WorkingDir: "{app}"

