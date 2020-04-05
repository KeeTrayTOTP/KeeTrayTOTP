# Obtaining TOTP secret from an iOS backup

If you use an iOS device you can use a tool like the [iPhone Backup Viewer](http://www.imactools.com/iphonebackupviewer/) to extract the secret key.

1. Open the backup using such a tool
2. Find the Steam App: `com.valvesoftware.Steam` in `AppDomain`
3. Extract `Documents/Steamguard-[your Steam ID]`
4. Open the file using a text editor and look for the text `otpauth:\/\/totp\/Steam:[your Steam login]?secret=[TOTP secret]&issuer=Steam`.
5. Copy the value of `[TOTP secret]` info the *TOTP Seed* field of the *TOTP Setup Wizard*.