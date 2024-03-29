# KeeTrayTOTP
![KeeTrayTOTP Tagged Release](https://github.com/KeeTrayTOTP/KeeTrayTOTP/workflows/KeeTrayTOTP%20Tagged%20Release/badge.svg)
![KeeTrayTOTP Pre Release](https://github.com/KeeTrayTOTP/KeeTrayTOTP/workflows/KeeTrayTOTP%20Pre%20Release/badge.svg?branch=master)
[![Latest Release](https://img.shields.io/github/release/KeeTrayTOTP/KeeTrayTOTP.svg)](https://github.com/KeeTrayTOTP/KeeTrayTOTP/releases/latest)
[![Total Downloads](https://img.shields.io/github/downloads/KeeTrayTOTP/KeeTrayTOTP/total.svg?maxAge=86400)](https://github.com/KeeTrayTOTP/KeeTrayTOTP/releases/latest)
[![GitHub license](https://img.shields.io/github/license/KeeTrayTOTP/KeeTrayTOTP)](https://github.com/KeeTrayTOTP/KeeTrayTOTP/blob/master/LICENSE)
[![first-timers-only](https://img.shields.io/badge/first--timers--only-friendly-blue.svg?style=flat-square)](https://www.firsttimersonly.com/)


This plugin is for those who already use [Tray TOTP Plugin](http://sourceforge.net/projects/traytotp-kp2/), but also want to use with Valve's Steam.

> This is a fork of the Tray TOTP Plugin for KeePass2. Originally developed by [Morphlin](http://sourceforge.net/u/morphlin/profile/). The original source code and plugin can be found on [sourceforge](http://sourceforge.net/projects/traytotp-kp2/).

Most of consumer TOTP's use the RFC6238 output style, sadly some companies (eg. Valve) decided not to adhere to the standard and instead build their own format.

In the case of Steam Mobile Authenticator the new output format was reverse engineered by various developers, and alternatives to it started popping up, most do pretty good job and can by themselves recover the TOTP secret (which is no easy task given Valve's implementation).

## Releases

The latest release can be found [here](https://github.com/KeeTrayTOTP/KeeTrayTOTP/releases).

#### Chocolatey 📦 
Or you can [use Chocolatey to install](https://community.chocolatey.org/packages/keepass-plugin-keetraytotp#install) it in a more automated manner:

```
choco install keepass-plugin-keetraytotp
```

To [upgrade KeePass Plugin KeeTrayTOTP](https://community.chocolatey.org/packages/keepass-plugin-keetraytotp#upgrade) to the [latest release version](https://community.chocolatey.org/packages/keepass-plugin-keetraytotp#versionhistory) for enjoying the newest features, run the following command from the command line or from PowerShell:

```
choco upgrade keepass-plugin-keetraytotp
```

## Dependencies

* [Keepass 2.31](http://keepass.info/)
* .NET 4.5 or superior

## Documentation

* Obtaining Steam Secrets
  * Via [Steam Desktop Authenticator](./docs/secret_sda/steam_desktop_authenticator_sda.md) (by [@raabf](https://github.com/raabf))
  * Via [Rooted android phone](./docs/secret_rooted_phone/index.md) (by [@DarkDaskin](https://github.com/DarkDaskin))
  * Via [iOS backup](./docs/secret_ios_backup/index.md)
* [Release](docs/release.md)
* [History](docs/history.md)

## Thanks

Thanks to [Dominik Reichl](http://www.dominik-reichl.de/) for KeePass and to [Morphlin](http://sourceforge.net/u/morphlin/profile/) for the original plugin.
