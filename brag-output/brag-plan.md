# RiotLogin: brag plan

## Rubric

1. **What is it?** A Windows desktop app that switches League of Legends accounts in one click, with every account's rank, status and stats on one screen.
2. **Who is it for?** League players with several accounts (main, smurfs, other regions).
3. **The pain:** switching means signing out, typing a password, waiting and launching again, for every account.
4. **The core claim:** "One-click account switching for League of Legends, with every account's rank, status and stats on one screen." (README)
5. **Best visual:** the dark account grid with rank-coloured SoloQ/FlexQ/Peak cells and a Status column.
6. **Top highlights:** (a) select an account, press Login, the app signs out, signs in and launches League; (b) Fetch all pulls level, ranks and peak for every account from op.gg, no login; (c) account health: OK, Ranked restricted, Banned at a glance.
7. **Tone:** `app-store` with a bit of punch. Real product, feature-forward, clean slides.
8. **Format:** landscape 1920x1080, about 21s, music + SFX, no voice.
9. **Secrets:** none. Only demo data from the repo screenshots (demo1–3, Main#KR1, Demo#EUW, Smurf#EUW) and invented ranks.

## Angle

The hook shows the tedious manual switch typed out, then cuts to one click doing all of it. Every scene is the real UI, recreated in HTML from `screenshots/main.png`: dark #1f1f1f surface, Riot red rgb(220,40,60) accent, tier colours from `App.xaml.cs`, Segoe UI Variable.

## Storyboard (21.0s)

| # | Time | Scene | On screen | Motion | SFX |
|---|---|---|---|---|---|
| 1 | 0.0–3.0 | Hook | "Switching accounts?" then a password field typing ●●●●●●●● and a checklist: Sign out · Type password · Wait · Launch | Lines snap in one by one, the checklist strikes through fast | keyboard ticks under the dots, soft click per line |
| 2 | 3.0–5.5 | Reveal | Logo mark + "RiotLogin" / "One click. Any account." | Slide-wipe from the right, title scales 0.96 to 1 | impactSoft_medium at 3.0 |
| 3 | 5.5–10.0 | Login | The account grid (6 demo rows). Cursor selects Smurf#EUW, clicks Login. Status bar: "Signing out…" then "Signing in…" then "Launching League ✓" | Rows stagger in, row highlight, button press, status text swaps | click at select, click at Login, bong on "Launching" |
| 4 | 10.0–13.5 | Fetch all | Same grid, caption "Fetch all · ranks from op.gg, no login". Progress "op.gg 3/6 · Main#KR1…"; SoloQ/Peak cells fill in tier colours one by one | Cells pop in per row | soft click per row |
| 5 | 13.5–17.0 | Health | Status column zoom: OK (green), Ranked restricted 14d (amber), Banned (red). Caption "Bans and restrictions, at a glance." | Camera push into the Status column, chips land | wood tick per chip |
| 6 | 17.0–21.0 | Outro | "RiotLogin" / "Every account. One screen." / "Free for Windows · github.com/Ku-Tadao/RiotLogin" | Clean fade-up, hold | impactBell_heavy at 17.0 |

Durations: 3.0 + 2.5 + 4.5 + 3.5 + 3.5 + 4.0 = 21.0s.

## Audio

- Music: `happy-beats-business-moves-vol-1-by-ende-dot-app.mp3` at 0.35, fade out over the last 1.2s.
- SFX at 0.55–0.8 from `interface/`, `keyboard/`, `impact/` (low/medium HF risk picks only).

## Music cue guidance

Track vol-1, 120.19 BPM, beat grid every ~0.5s from 3.02s. Reveal at 3.0 sits on the 3.02 beat. Scene cuts land on beats: 5.53, 10.02, 13.51. The outro reveal hits the strong cue at 17.02. Cues are hints only; readability wins.
