# Obtaining TOTP secret from a rooted Android phone

You need a rooted Android phone with a file manager application and [Steam](https://play.google.com/store/apps/details?id=com.valvesoftware.android.steam.community) installed.
Follow [this instruction](https://support.steampowered.com/kb_article.php?ref=4440-RTUI-9218) to set up Steam Guard if you did not do this before.

Then open the file manager and navigate to the `/data/data/com.valvesoftware.android.steam.community/files/` directory (requires root access). You will find a file named `Steamguard-[your Steam ID]` there, open it as a text file.
Inside the file look for the following text: `otpauth:\/\/totp\/Steam:[your Steam login]?secret=[TOTP secret]&issuer=Steam`.
Copy the value of `[TOTP secret]` info the *TOTP Seed* field of the *TOTP Setup Wizard*.