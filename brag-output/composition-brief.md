# Hyperframes Composition Brief: RiotLogin

## Objective
A short launch-style brag video for RiotLogin.

## Output
- Composition: `brag-output/composition/`
- Render: `brag-output/brag.mp4`
- Landscape 1920x1080, 30 fps, 21s

## Source Material
- Project root: `RiotLogin/` (WPF .NET 10). Read: README.md, MainWindow.xaml, App.xaml.cs (colours), OpggService.cs, screenshots/main.png.
- Product name: RiotLogin
- Strongest claim: "One-click account switching for League of Legends, with every account's rank, status and stats on one screen."
- Key UI to recreate: the main window account grid (Riot ID, Username, Level, Server, SoloQ, Peak, Status) with the bottom bar (status text, Fetch all, Accounts, Login).
- Verbatim copy: "Fetch all", "Login", "Accounts", "Select an account and press Login (or Enter)", "Search accounts (Ctrl+F)".

## Creative Direction
- Tone: `app-store` with punch. Feature-forward, clean, title case captions, no aggression.
- Angle: the manual switch (sign out, type password, wait, launch) shown in 3s, then one click does all of it inside the real UI.
- Hook: "Switching League accounts?" with a password field filling with dots and the chore checklist.
- Outro: "RiotLogin / Every account. One screen. / Free for Windows · github.com/Ku-Tadao/RiotLogin"
- Avoid: generic SaaS language, abstract filler, Riot's logo or League art (trademarks): use a neutral mark.

## Visual Identity
- Background #161616, window surface #202020, rows #262626, borders #333
- Text #FFFFFF, secondary #A0A0A0
- Accent Riot red rgb(220,40,60)
- Tier colours from App.xaml.cs; status OK #2EC27E, Banned #E8636B, other #E0A84F
- Font: Segoe UI Variable via local() @font-face (the app's own font)

## Storyboard
See `brag-plan.md`. Implemented as 4 scenes: hook (0–3), reveal (3–5.5), app (5.5–17: login, fetch all, health push-in in one continuous world), outro (17–21).

## Audio
- Role: sparse professional accents over a warm bed.
- Music: happy-beats-business-moves-vol-1 at 0.35, fade out 1.2s.
- Cues: bundled preset. Beat-locked: reveal 3.02, outro 17.02. Beat-grid: fetch row refreshes on 10.52–13.02.
- Audio-reactive: none, on purpose (app-store restraint; UI should not pulse).
- SFX: keypresses under the password dots, clicks on cursor actions, bong on "Launching League", soft impact on reveal, bell on outro. Low/medium HF risk only.
