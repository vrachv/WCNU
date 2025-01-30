# WoW Character Name Unlocker (WotLK Client)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Client-side utility for Wrath of the Lich King (3.3.5a) that bypasses character name validation.  
**Important:** Requires matching server-side modifications to handle extended characters.

## Usage
1. Download release version or build own.
2. Drag-and-drop wow.exe to WCNU.exe
3. Creates **new client** `wow-wcnu.exe` without modifying original files
4. Done!

**Server Must Implement:**
1. Extended character whitelist
2. Correct conversion to upper and lower case
3. Must handle all validation - client accepts any input!
