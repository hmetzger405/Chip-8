# C# CHIP-8 Emulator



A fully functional Chip-8 emulator built using C# and Windows Forms. This project enables use of any Chip-8 ROM file



<img width="957" height="519" alt="image" src="https://github.com/user-attachments/assets/d7b78276-f39d-4fc1-95b3-52199558173a" />

## Where to find ROMs
This project does not include ROMs but CHIP-8 ROMs that are in the public domain can be found here: https://archive.org/download/Chip-8RomsThatAreInThePublicDomain



## Features

* Implements all 34 Opcodes used in modern Chip-8 interpreters

* Runs the CPU at 600Hz, with a 60Hz display

* Enables use with a standard QWERTY Keyboard

* Uses Windows Forms to enable loading rom from file system



## Input

This emulator maps the Chip-8 keypad to the left side of a Standard QWERTY keyboard


| Chip-8 Keypad | QWERTY Keyboard |
| :---: | :---: |
| **1 2 3 C** | **1 2 3 4** |
| **4 5 6 D** | **Q W E R** |
| **7 8 9 E** | **A S D F** |
| **A 0 B F** | **Z X C V** |



## Loading a Rom

* **File -> Open ROM**: Load a .ch8 file



## Tools Used

* **Language:** C#

* **Platform:** .NET 8.0

* **GUI Framework:** Windows Forms

* **Reference Material:** Cowgod's Chip-8 Technical Reference v1.0
