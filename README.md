
# PS3Lib

## Description
PS3Lib is a .NET library designed for interacting with PlayStation 3 consoles. This library supports PS3 HEN and provides functionalities to control and manage various aspects of the console.

## Features
- Support for PS3 HEN
- Built with .NET 8.0
- 32-bit application generation

## Installation
To install and use PS3Lib in your project, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/iR3SH/PS3Lib.git
   ```
2. Open the solution file `PS3Lib.sln` in Visual Studio.
3. Build the project in 32-bit mode.

## Usage
Here is a basic example of how to use PS3Lib in your application:

```csharp
using PS3Lib;

class Program
{
    static void Main()
    {
        PS3API PS3 = new PS3API();
        PS3.ConnectTarget(0);
        PS3.AttachProcess();
        // Your code here
    }
}
```

## Credits
- Enstone
- Buc-ShoTz
- iMCSx
- NvZ

Original GitHub:
- [PS3Lib](https://github.com/iMCSx/PS3Lib)
- [CCAPI](https://www.enstoneworld.com/articles/view/15/ControlConsole_API)

## Contributing
Contributions are welcome! Please submit pull requests or open issues for any improvements or bug fixes.

## License
This project is licensed under the MIT License.

## Release Notes
### Version 4.6
- Added support for PS3 HEN.

### Version 4.6.1
- Fixed console list conflict with HEN.
