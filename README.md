
# BatteryIcon
The free and open-source Windows battery icon alternative.

With the official release of Windows 11, the battery, volume, and Wi-Fi icons that were once separate are now consolidated into the 
Windows Action Center. While this makes the controls easy to use and in one place, it makes it significantly harder to customize power modes 
without entering Windows settings and tediously changing it every time you want to optimize for performance or power savings. 

BatteryIcon lives in the Windows Notification Area (often referred to as the "system tray") and is easily accessible from a single click. This application **WILL NOT** work if the standard power mode GUIDs are modified. This application is primarily built for use with x64 Windows 11 laptops.

<img src="Settings.png" alt="Windows Battery Settings" width="50%" height = "50%"/>

## Usage
- Download and install application using setup file provided in "Releases", 
to add app to startup, type in "shell:startup" into the File Explorer and add the executable.

## Compiling 
- BatteryIcon was built using Visual Studio 2022 in C# with WPF (.NET framework)
1. Install Visual Studio 2022 and select .NET
2. Open BatteryIcon.sln in Visual Studio
3. If security notification is received, disable it.
4. Choose x64 for solution platform otherwise power mode switching will be disabled
5. The raw executable is in BatteryIcon\BatteryIcon\bin\x64\Debug\BatteryIcon.exe

## Requirements
- Windows 10 2004 (20H1) or later, fully compatible with Windows 11
- 64 bit Operating System
- .NET Framework 4.7.2 (comes preinstalled with Windows)

## Screenshots

Main menu: <br /> <br /> 
<img src="Icon.png" alt="BatteryIcon" width="25%" height = "25%"/>

Right click: <br /> <br /> 
<img src="Menu.png" alt="Context Menu" width="25%" height = "25%"/>


## Features
- Color-coordinated battery indicator displays the same info as the built-in Windows icon
- Battery information and current status on taskbar hover and left click
- Can display battery percentage instead of icon
- Adjustable brightness using 5-tick slider

## NuGet Packages Used
- Microsoft.Toolkit.Uwp.Notifications
- System.Drawing.Common
- System.Threading.Tasks
- UwpDesktop-Updated

## Disclaimer
Using the powrprof.dll's undocumented API is not supported by Microsoft. This application may break in a future Windows release. 
This is not an official application, just an experiment I wanted to create for fun. I am not responsible for any changes to power settings 
that may occur if used in unsupported versions of Windows. Use discretion.

## Credit
This GUI was made possible thanks to Aaron Kelley's detailed guide on the Windows Power Slider's undocumented API.
Check it out here: https://github.com/AaronKelley/PowerMode

## Contributing
I would be very grateful for any pull requests or issues.
Notable features that are low priority include gui autostart, different icons, light theme/dark theme switcher, 
check for updates, fluent design and localization </br>
<img src="license.jpg" alt="GPL-3.0 License" width="30%" height = "30%"/>


