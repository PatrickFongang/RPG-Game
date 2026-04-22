# RPG Game Engine (C# / .NET) - Student Project

A clean, object-oriented RPG game engine built in C#. This project was developed as a **student project** at the Warsaw University of Technology to demonstrate the practical application of **SOLID principles** and **Design Patterns** in building a scalable, maintainable software architecture.

## 🏗️ Architecture & Design Patterns

Unlike a standard monolithic game script, this engine is divided into highly decoupled modules using industry-standard design patterns:

* **Builder Pattern** (`DungeonBuilder`, `DungeonDirector`): Encapsulates complex level construction, allowing step-by-step creation of various dungeon configurations.
* **Abstract Factory & Strategy Patterns** (`IDungeonTheme`): Used dynamically to generate cohesive, theme-specific environments (e.g., Sci-Fi, Library, Wealth). Themes dictate the room generation strategy and the specific families of items, enemies, and artifacts that spawn.
* **Command Pattern** (`ICommand`, `MoveCommand`): Decouples input handling from execution, enabling scalable player actions and flexible control mappings.
* **Visitor Pattern** (`IAttackVisitor`): Separates combat algorithms from entity structures, making it easy to add new attack types (Normal, Stealth, Magic) without modifying core item or player classes.
* **Decorator Pattern** (`ItemDecorator`): Dynamically adds properties to weapons and junk items at runtime (e.g., `(Strong)`, `(Lucky)` modifiers), enriching the loot pool without class explosion.
* **Composite Pattern** (`CompositeLogger`): Treats multiple logging destinations (RAM, File) as a single instance, allowing simultaneous logging streams.
* **Singleton Pattern** (`EventLogger`, `ConfigManager`): Provides strict, globally accessible instances for core services like the game configuration and the event logging system.

## 🚀 Features

* **Thematic Dungeon Generation**: Fully dynamic layouts, enemies, and loot tables based on the selected dungeon theme (Abstract Factory).
* **Modular Combat System**: Extensible, algorithm-based attack and defense logic based on equipped items.
* **Advanced Inventory & Loot**: Support for randomized, dynamically modified items via Decorators.
* **Dynamic Economic System**: A flexible wallet that supports multiple currencies (`Gold`, `Coins`, `Energy Cells`, etc.) depending on the dungeon's theme.
* **Comprehensive Event Logging**: Records combat, movement, and interactions in real-time, saving to both an interactive UI panel and an auto-generated file with timestamping to prevent overwrites.
* **JSON Configuration**: Auto-generating `config.json` for easy setup of player names and external log directories.
* **Responsive Console UI**: Custom-built rendering engine with word-wrapping, collision-free panel layouts, and interactive prompts.

## 💻 Tech Stack

* **Language:** C#
* **Framework:** .NET 9.0
* **Focus:** Object-Oriented Programming (OOP) & Clean Software Architecture
