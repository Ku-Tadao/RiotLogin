## About RiotLogin
RiotLogin is an automatic login tool for Riot Client. This is a fork of [lithellx/RiotLogin](https://github.com/lithellx/RiotLogin) with expanded account management features.

### What's changed in this fork
- **Clipboard-based login** — credentials are pasted via clipboard instead of keystroke emulation, handling special characters correctly and avoiding SendKeys issues
- **Auto-detect Riot Client** — checks Windows registry keys to find `RiotClientServices.exe` automatically (no manual path setup needed)
- **Auto-launch & login** — if the Riot Client isn't running, the Login button starts it, waits for the window, then fills in credentials seamlessly
- **Extended account profiles** — each account stores: Riot ID, Username, Password, Level, Server, BE, RP, SoloQ, FlexQ, Champions, Skins, Loot
- **Pull Data from League Client** — connect to the running League Client (LCU API) to auto-fill Level, Server, BE, RP, ranks, champion/skin/loot counts
- **Typed JSON storage** — migrated from raw string manipulation to a typed model (`AccountProfile`) with automatic migration from the old format
- **Renamed forms** — `Form1` → `LoginForm`, `Form2` → `AccountEditorForm`
- **Removed confirmation dialogs** — Login and Settings buttons act immediately without prompts

### Original behavior preserved
- Keyboard login flow with Remember Me checkbox
- Manual Riot Client path selection as fallback
- info.json data file (auto-created if missing)

## What Riot Support think about RiotLogin?

Riot Support staff told the original author that they can't say for sure whether this app will get you banned or not.
> "When Vanguard is turned on, using kind of programs like RiotLogin and doing stuff with it can get you banned, even it is very unlikely. Therefore, I cannot guarantee that you will not be banned."

If you're using RiotLogin for LoL, TFT or LoR and Vanguard isn't turned on then you're okay with it. **But if you use RiotLogin for VALORANT or Vanguard is turned on**, **use it on your own risk**.

## Usage
1. Run `RiotLogin.exe` (info.json will be created automatically)
2. Click **Settings** to open the Account Editor
3. Add accounts with at least a Username and Password
4. Select an account and click **Login** — the Riot Client will be found/started automatically
5. Optionally: with League of Legends running, select an account and click **Pull Data** to auto-fill stats

## Building
Requires Visual Studio 2022 with .NET Framework 4.7.2 targeting pack.
```
msbuild RiotLogin.sln /t:Restore /p:Configuration=Debug
msbuild RiotLogin.sln /p:Configuration=Debug
```

## Authors
- [lithellx](https://github.com/lithellx) — original RiotLogin
- [Ku-Tadao](https://github.com/Ku-Tadao) — extended fork
