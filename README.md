# RPG Game Engine (C# / .NET) - Student Project

A clean, object-oriented RPG game engine built in C#. This project was developed as a **student project** at the Warsaw University of Technology to demonstrate the practical application of **SOLID principles** and **Design Patterns** in building a scalable, maintainable software architecture.

## 🏗️ Architecture & Design Patterns

Unlike a standard monolithic game script, this engine is divided into highly decoupled modules using industry-standard design patterns:

* **Builder Pattern (`DungeonBuilder`, `DungeonDirector`)**: Encapsulates complex level construction, allowing step-by-step creation of various dungeon configurations.
* **Command Pattern (`ICommand`, `MoveCommand`)**: Decouples input handling from execution, enabling scalable player actions and potential undo systems.
* **Visitor Pattern (`IAttackVisitor`)**: Separates combat algorithms from entity structures, making it easy to add new damage types or special effects without modifying core classes.
* **Decorator Pattern (`ItemDecorator`)**: Dynamically adds properties to items at runtime, perfect for magical buffs or equipment modifications.

## 🚀 Features

* **Modular Combat System**: Extensible attack logic via the Visitor pattern.
* **Structured Generation**: Reliable dungeon building using the Builder pattern.
* **Advanced Inventory**: Support for dynamic item modifications via Decorators.

## 💻 Tech Stack

* **Language:** C#
* **Framework:** .NET
* **Focus:** Object-Oriented Programming (OOP) & Software Architecture
