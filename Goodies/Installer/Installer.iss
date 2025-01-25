#define MyAppName "Surefire"
#define MyAppVersion "1.0.1"
#define MyAppPublisher "flashvenom"
#define MyAppURL "https://www.surefireams.com/"
#define MyAppExeName "Surefire.Desktop.exe"

[Setup]
AppId={{6ED12FAC-0A41-4613-A3F8-A1C8CBC23D78}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={localappdata}\{#MyAppName}
DisableDirPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DisableProgramGroupPage=yes
LicenseFile=C:\Users\[Username]\source\Surefire\Goodies\Installer\Lic.rtf
InfoBeforeFile=C:\Users\[Username]\source\Surefire\Goodies\Installer\Wel.rtf
WizardSmallImageFile=C:\Users\[Username]\source\Surefire\Goodies\Installer\sf-small.bmp
WizardImageFile=C:\Users\[Username]\source\Surefire\Goodies\Installer\sf-large.bmp
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline
OutputDir=C:\Users\[Username]\source\Surefire\Goodies\Installer\
OutputBaseFilename=Install_Surefire
SetupIconFile=C:\Users\[Username]\source\Surefire\Goodies\Installer\surefireicon.ico
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\[Username]\source\Surefire\Build\Files\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\[Username]\source\Surefire\Build\Files\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\[Username]\source\Surefire\Goodies\Installer\aspnetcore-runtime-9.0.0-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "C:\Users\[Username]\source\Surefire\Goodies\Installer\windowsdesktop-runtime-9.0.0-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "C:\Users\[Username]\source\Surefire\Goodies\Installer\MicrosoftEdgeWebview2Setup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{tmp}\aspnetcore-runtime-9.0.0-win-x64.exe"; Parameters: "/quiet /norestart"; Flags: waituntilterminated; StatusMsg: "Installing DotNetCore Runtime..."
Filename: "{tmp}\windowsdesktop-runtime-9.0.0-win-x64.exe"; Parameters: "/quiet /norestart"; Flags: waituntilterminated; StatusMsg: "Installing .NET Desktop Runtime..."
Filename: "{tmp}\MicrosoftEdgeWebview2Setup.exe"; Parameters: "/silent /install"; Flags: waituntilterminated; StatusMsg: "Installing WebView2 Engine..."
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
