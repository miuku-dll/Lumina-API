# Lumina Virtual Input Library

## Overview

Lumina Virtual Input Library is a powerful .NET library for programmatic mouse and keyboard input simulation. Designed for developers who need precise control over input operations, this library offers a high-level API to automate mouse movements, clicks, and keyboard inputs across Windows applications.

## Features

- **Mouse Simulation**
  - Cursor movement with multiple interpolation options
  - Left and right click simulation
  - Customizable movement paths (linear, bezier)

- **Keyboard Simulation**
  - Key press and release events
  - Support for all standard virtual key codes

## Getting Started

### Installation

To install Lumina Virtual Input Library, you can use the following command in your project directory:

```bash
dotnet add package Lumina.VirtualInput
```

### Basic Usage

```csharp
using Lumina_Virtual_Input_Lib;

// Mouse operations
var mouse = new VirtualMouse();
mouse.MoveTo(500, 300, MovementType.Linear);
mouse.Click(MouseButton.Left);

// Keyboard operations
var keyboard = new VirtualKeyboard();
keyboard.SendKey(0x41); // Press 'A'
keyboard.SendKey(0x41, true); // Release 'A'
```

## Documentation

For detailed documentation, examples, and API references, please visit our [Wiki](https://github.com/miuku-dll/Lumina-Virtual-Input-Library/wiki).

## Use Cases

- Automated UI testing
- Game botting and macro systems
- Accessibility tools
- Interactive tutorials and demos

## Requirements

- .NET Standard 2.0 compatible frameworks
- Windows operating system

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for more details.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions, please [open an issue](https://github.com/miuku-dll/Lumina-Virtual-Input-Library/issues) on our GitHub repository.

---

Developed with ❤️ by miuku-dll