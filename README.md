# KeeTrayTOTP
Fork of the Tray TOTP Plugin for KeePass2.

> Original work developed by the [Morphlin](http://sourceforge.net/u/morphlin/profile/) the source code and plugin can be found at http://sourceforge.net/projects/traytotp-kp2/
> 
Most of consumer TOTPs use the RFC6238 output style, sadly some companies (eg.: Valve) decided not to adhere to the standard and instead build their own format.

In the case of Steam Mobile Authenticator the new output format was reverse engineered by various developers, and alternatives to it started popping up, most do pretty good job and can by themselves recover the TOTP secret (which is no easy task given Valve's implementation).

This plugin is for those who already use [Tray TOTP Plugin](http://sourceforge.net/projects/traytotp-kp2/), but also want to use with Valve's Steam.

## Releases

The latest release can be found [here](https://github.com/victor-rds/KeeTrayTOTP/releases).

## Dependencies
* [Keepass 2.31](http://keepass.info/)
* .NET 4.5 or superior

## So, why do this? 
Various reasons, first and most important: I don't want to use another application only for steam! KeePass and Tray TOTP (with some modifications) are more than capable enough to handle this task.

Second: for education, working with another developer's code , specially in a language you're not familiar with, its a difficult task but not  uncommon situation.

Third: I wanted only to make a slightly modification, then I needed to make a new setting options, change the way plugin generate in order ta add another step, some rewriting later, too much was changed for simply patch, so I decided to create my own fork and made more changes

### Steam TOTP Secret?!
There is no easy way to get it, there are various complications. Since Steam doesn't setup like most services (using QR Codes), the easiest way is to have through a rooted android phone.

Another way would be using totp generators that support Steam like [Windows Authenticator](https://github.com/winauth/winauth) and [Steam Desktop Authenticator](https://github.com/Jessecar96/SteamDesktopAuthenticator), as far as I know, they emulate the mobile app API calls in order to generate a new TOTP Secret, one could use them to obtain the secret (never tried though), also this have the drawback of disabling the mobile app.

#### Obtaining TOTP secret from a rooted Android phone (by [DarkDaskin](https://github.com/DarkDaskin))

You need a rooted Android phone with a file manager application and [Steam](https://play.google.com/store/apps/details?id=com.valvesoftware.android.steam.community) installed.
Follow [this instruction](https://support.steampowered.com/kb_article.php?ref=4440-RTUI-9218) to set up Steam Guard if you did not do this before.

Then open the file manager and navigate to the `/data/data/com.valvesoftware.android.steam.community/files/` directory (requires root access). You will find a file named `Steamguard-[your Steam ID]` there, open it as a text file.
Inside the file look for the following text: `otpauth:\/\/totp\/Steam:[your Steam login]?secret=[TOTP secret]&issuer=Steam`.
Copy the value of `[TOTP secret]` info the *TOTP Seed* field of the *TOTP Setup Wizard*.

TODO: Tutorial how to get Steam TOTP secret other ways

#### Obtaining TOTP secret from an iOS backup

If you use an iOS device you can use a tool like the [iPhone Backup Viewer](http://www.imactools.com/iphonebackupviewer/) to extract the secret key.
   1. Open the backup using such a tool
   2. Find the Steam App: `com.valvesoftware.Steam` in `AppDomain`
   3. Extract `Documents/Steamguard-[your Steam ID]`
   4. Open the file using a text editor and look for the text `otpauth:\/\/totp\/Steam:[your Steam login]?secret=[TOTP secret]&issuer=Steam`.
   5. Copy the value of `[TOTP secret]` info the *TOTP Seed* field of the *TOTP Setup Wizard*.

### What's next?
My first objective is complete: I made a working prototype.
But I'm not happy with the code, I want to rebuild this thing from the ground if needed, I'm by no means criticizing the original work, but it's little complex for me and rebuilding it may help to better understanding it. 
Also I'm open to suggestion, issues, new features, etc.

**Thanks to [Dominik Reichl](http://www.dominik-reichl.de/) for KeePass software and to [Morphlin](http://sourceforge.net/u/morphlin/profile/) for the original plugin**

English isn't my native language, so please excuse any mistakes.
