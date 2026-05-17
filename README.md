# Match-N-Play

A Unity-based mobile matching game project built with C#, ShaderLab, and HLSL. Inspired by popular match-three puzzle games like Royal Match and Candy Crush.

## 📋 Table of Contents

- [About](#about)
- [Technology Stack](#technology-stack)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Controls](#controls)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

## About

**Match-N-Play** is a Unity mobile game development project that combines intuitive match-three puzzle gameplay with advanced graphics programming. Inspired by successful titles like Royal Match and Candy Crush, this project delivers engaging tile-matching mechanics with custom shaders and optimized graphics. The game challenges players to match tiles, complete levels, and progress through increasingly difficult puzzles with smooth animations and visual polish.

## Technology Stack

The project is built using a carefully balanced mix of technologies:

- **C# (47.5%)** - Core gameplay logic, level mechanics, UI systems, and game logic
- **ShaderLab (43.5%)** - Visual effects, tile animations, and shader implementations
- **HLSL (9%)** - High-level shader language for advanced graphics rendering and effects

## Features

- 🎮 Engaging match-three puzzle mechanics inspired by Royal Match and Candy Crush
- ✨ Custom shader effects for tile animations and visual feedback
- 🎨 Optimized graphics rendering for mobile platforms
- 📱 Mobile-first design with touch input controls
- 🎯 Progressive level system with increasing difficulty
- 🔧 Modular and extensible codebase for easy expansion
- ⚡ Performance optimized for various mobile devices

## Prerequisites

Before you begin, ensure you have the following installed:

- **Unity 6000.0.59f2** (or compatible version)
- **Visual Studio 2022** or **Rider IDE** (recommended for C# development)
- **Git** for version control
- At least **10 GB** of disk space

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/BedirhanMaden/Match-N-Play.git
cd Match-N-Play
```

### Open in Unity

1. Launch Unity Hub
2. Click **"Add"** and select the cloned project folder
3. Select the appropriate Unity version and open the project
4. Wait for Unity to import all assets and compile scripts

### Run the Game

1. Open the main scene from `Assets/Scenes/`
2. Press **Play** in the Unity Editor or build for your target platform (iOS/Android)

## Project Structure

```
Match-N-Play/
├── Assets/
│   ├── Scripts/           # C# gameplay, UI, and level management scripts
│   ├── Shaders/           # ShaderLab and HLSL shader files for effects
│   ├── Scenes/            # Game scenes and levels
│   ├── Prefabs/           # Reusable tile, UI, and level prefabs
│   ├── Materials/         # Material definitions for tiles and effects
│   ├── Textures/          # Texture assets for tiles and backgrounds
│   └── Audio/             # Sound effects and background music
├── Packages/              # Unity package dependencies
├── ProjectSettings/       # Unity project settings
└── README.md
```

## Controls

| Input | Action |
|-------|--------|
| **Touch/Mouse Click** | Select and match tiles |
| **Swipe** | Combine adjacent matching tiles |
| **ESC / Back Button** | Pause game |
| **R** | Restart current level |

*Note: Primary controls are touch-based for mobile. Desktop controls provided for development and testing.*

## Contributing

Contributions are welcome! If you'd like to contribute to this project:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is currently not licensed. Please check back or contact the repository owner for licensing information.

## Support

For questions, issues, or suggestions:

- 📧 Open an [Issue](https://github.com/BedirhanMaden/Match-N-Play/issues)
- 🔗 Visit the [Repository](https://github.com/BedirhanMaden/Match-N-Play)

---

**Made with ❤️ by [BedirhanMaden](https://github.com/BedirhanMaden)**
