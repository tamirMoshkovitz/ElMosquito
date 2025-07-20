# 🦟 EL MOSQUITO
_Advanced Unity Course Project (HUJI × Bezalel, 2025)_

---

## 🎮 Game Description

You are not just a mosquito — you're **the** mosquito.

Your mission: **bite down** a dude on a train before he slaps you dead.  
This one-level boss fight combines reactive AI, procedural animation, and decorator-driven upgrades in a fast-paced, precision-driven experience.

---

## 📽️ Videos

- 🎮 [Gameplay Demo](https://youtu.be/Kp4RKf0lAo8)  
- 🛠️ [Technical Overview](#)

---

## 🧠 Boss Design: 2 Phases of Pain

1. **Phase 1 – The Seated Slapper**  
   - He sits. You bite. He slaps.  
   - Procedural slap animations.  
   - Your job: sneak in, bite exposed skin, and dodge the hands.

2. **Phase 2 – The Pursuer**  
   - At 33% HP, he **gets up and starts chasing you**.  
   - Slaps remain procedural, but now he moves with intent.  
   - The pressure ramps up - tighter spacing, less recovery time, and constant pursuit.

---

## 🎮 Controls

| Action       | Input                |
|--------------|----------------------|
| Move         | Right Stick          |
| Boost        | Double Click L1      |

---

## 🔧 Notable Systems

### 🎨 Procedural Art  
- Fully shader-based mosquito and boss visuals (excluding title & end screen)  
- Signed Distance Fields (SDF) for organic animation

### 🎯 InverseKinematic Slap System  
- Boss arms use **Cinemachine path + TwoBoneIK**  
- Attacks trigger based on player distance and a random reaction timing

### 🧪 Decorator-Based Mosquito Powers  
- Speed, Maneuver, Damage buffs stack and modify base behavior  
- Reversible stack allows power-up loss on hit

---

## ▶️ Play the Game

----------
[ItchIO page](https://tamirmoshko.itch.io/el-mosquito)

---

## 🧾 Credits

- **Developer & Designer:** Tamir Moshkovitz  
- **Institution:** Hebrew University & Bezalel Academy  
- **Course:** Advanced Unity (2025 Minor in Game Design)

---
## 🎵 Music Credits

### Opening Scene:

- “Mosquito​” by KAJ

### In-Game:

- “Danger Zone” by Kenny Loggins

### Ending:

- “Take My Breath Away” by Berlin

- “Memories”  by Harold Faltermeyer​


---

This project was created as part of the Advanced Unity course

at the Hebrew University × Bezalel Academy (2025).

It is a non-commercial, student project made purely for pedagogical purposes.

All copyrighted music is used under the assumption of fair educational use,

and no profit or monetization is intended.

> _"Think you're a big-shot mosquito? Let's see you bite this dude until he blacks out. Oh - and yeah - he slaps. Hard."_
