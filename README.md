# KeeTrayTOTP

This plugin is for those who already use [Tray TOTP Plugin](http://sourceforge.net/projects/traytotp-kp2/), but also want to use with Valve's Steam.

> This is a fork of the Tray TOTP Plugin for KeePass2. Originally developed by [Morphlin](http://sourceforge.net/u/morphlin/profile/). The source code and plugin can be found on [sourceforge](http://sourceforge.net/projects/traytotp-kp2/)

Most of consumer TOTP's use the RFC6238 output style, sadly some companies (eg. Valve) decided not to adhere to the standard and instead build their own format.

In the case of Steam Mobile Authenticator the new output format was reverse engineered by various developers, and alternatives to it started popping up, most do pretty good job and can by themselves recover the TOTP secret (which is no easy task given Valve's implementation).

## Releases

The latest release can be found [here](https://github.com/victor-rds/KeeTrayTOTP/releases).

## Dependencies

* [Keepass 2.31](http://keepass.info/)
* .NET 4.5 or superior

## Documentation

* Obtaining Steam Secrets
  * Via [Steam Desktop Authenticator](./secret_sda/steam_desktop_authenticator_sda.md) (by [@raabf](https://github.com/raabf))
  * Via [Rooted android phone](./docs/secret_rooted_phone/index.md) (by [@DarkDaskin](https://github.com/DarkDaskin))
  * Via [iOS backup](./docs/secret_ios_backup/index.md)
* [Release](docs/release.md)
* [History](docs/history.md)

## Thanks

Thanks to [Dominik Reichl](http://www.dominik-reichl.de/) for KeePass and to [Morphlin](http://sourceforge.net/u/morphlin/profile/) for the original plugin.
