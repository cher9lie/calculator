# Windows Forms Calculator

A high-precision calculator application for Windows with switchable Standard and Scientific modes, featuring calculation history and lossless arithmetic precision.

## Features

### Core Features
- **Mode Switching**: Switch between Standard and Scientific calculator modes
- **High-Precision Arithmetic**: Uses BigInteger-based rational arithmetic for lossless calculations  
- **Calculation History**: Persistent history with separate storage for each mode
- **Keyboard Support**: Full keyboard input support including numpad and shortcuts

### Mathematical Operations

#### Standard Mode
- Basic arithmetic: Addition (+), Subtraction (−), Multiplication (×), Division (÷)
- Percentage calculations (%)
- Sign change (±)
- Reciprocal (1/x)
- Clear (C) and Clear Entry (CE)

#### Scientific Mode  
- All Standard mode operations plus:
- Trigonometric functions: sin, cos, tan
- Logarithmic functions: log (base 10), ln (natural log)
- Exponential function: exp
- Power function: x^y
- Square root: √

### Precision Features
- Maintains exact precision for rational numbers
- Configurable display precision (16 digits standard, 32 digits scientific)
- No floating-point rounding errors for basic arithmetic
- Scientific notation support

## Requirements

### For Visual Studio 2010 (.NET Framework 4.0)
- Windows XP SP3 or later
- .NET Framework 4.0 Client Profile
- Visual Studio 2010 (any edition)

### For Modern Development (.NET 8.0)
- Windows 10 or later / Linux / macOS
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

## Setup Instructions

### Visual Studio 2010 Setup

1. **Open the project in VS2010:**
   ```
   Open WindowsFormsCalculator.sln in Visual Studio 2010
   ```

2. **Build the project:**
   ```
   Build → Build Solution (F6)
   ```

3. **Run the application:**
   ```
   Debug → Start Debugging (F5)
   ```

### Modern Development Setup

1. **Clone and navigate:**
   ```bash
   cd src/WindowsFormsCalculator
   ```

2. **Run console demo:**
   ```bash
   dotnet run -- --demo
   ```

3. **Build for Windows:**
   ```bash
   dotnet build
   ```

## Architecture

### Core Components

#### `HighPrecisionDecimal`
- BigInteger-based rational number implementation
- Supports exact arithmetic operations
- Handles scientific notation and decimal precision
- Prevents floating-point precision loss

#### `CalculationEngine`
- Main calculation logic and state management
- Handles operation precedence and expression building
- Supports both Standard and Scientific modes
- Error handling and validation

#### `CalculationHistory`
- Persistent calculation history storage
- Separate histories for different modes
- History export and management features
- Configurable maximum history size (100 items default)

#### `CalculatorForm`
- Windows Forms UI with dynamic layout
- Keyboard and mouse input handling
- Mode switching and button visibility management
- History display dialog

### Design Patterns

- **State Pattern**: Calculator modes with different operation sets
- **Command Pattern**: Operation handling and history storage
- **Observer Pattern**: UI updates based on engine state changes

## Usage Examples

### Basic Calculations
```
Input: 2 + 3 =
Result: 5

Input: 10 ÷ 3 =
Result: 3.333333333333333 (with full precision maintained internally)
```

### High-Precision Demonstration  
```
Input: 1 ÷ 3 × 3 =
Result: 1 (exact, no rounding errors)

Input: 0.1 + 0.2 =
Result: 0.3 (exact, unlike floating-point arithmetic)
```

### Scientific Mode
```
Input: 9 → √ 
Result: 3

Input: 100 → log
Result: 2

Input: 2 → x^y → 10 → =
Result: 1024
```

### Keyboard Shortcuts
- **Numbers**: 0-9, Numpad 0-9
- **Operations**: +, -, *, / (or numpad equivalents)
- **Decimal**: . or numpad decimal
- **Execute**: Enter or =
- **Clear**: Escape (C) or Delete (CE)
- **Mode Switch**: Alt+M (when implemented)

## File Structure

```
WindowsFormsCalculator/
├── CalculationEngine.cs          # Core calculation logic
├── CalculationHistory.cs         # History management
├── CalculatorForm.cs             # Main UI form
├── CalculatorForm.Designer.cs    # UI layout (auto-generated)
├── CalculatorForm.resx           # Form resources
├── CalculatorMode.cs             # Mode enumerations
├── HighPrecisionDecimal.cs       # High-precision arithmetic
├── Program.cs                    # Application entry point
├── ConsoleDemo.cs                # Demonstration program
├── Properties/                   # Assembly and resource files
├── WindowsFormsCalculator.sln    # VS2010 solution file
├── WindowsFormsCalculator.vs2010.csproj  # VS2010 project file
└── README.md                     # This documentation
```

## Extending the Calculator

### Adding New Operations
1. Add operation to `OperationType` enum in `CalculatorMode.cs`
2. Implement operation logic in `CalculationEngine.PerformOperation()`
3. Add UI button in `CalculatorForm.Designer.cs`
4. Wire button click event in `CalculatorForm.cs`

### Adding New Modes
1. Extend `CalculatorMode` enum
2. Update `CalculationEngine.Mode` property setter
3. Modify `CalculatorForm.UpdateModeDisplay()` for UI changes
4. Add mode-specific button visibility logic

## Known Limitations

1. **Scientific Functions**: Uses System.Math for transcendental functions, which introduces floating-point precision limits
2. **Memory Constraints**: Very large numbers may consume significant memory due to BigInteger storage
3. **UI Threading**: All operations are synchronous on the UI thread
4. **Platform Dependencies**: Windows Forms requires Windows or Mono compatibility layer

## Implementation Notes

### Precision Strategy
The calculator uses a hybrid approach:
- **Exact arithmetic** for rational operations (add, subtract, multiply, divide)  
- **High-precision floating-point** for transcendental functions (sin, cos, log, etc.)
- **Configurable display precision** while maintaining internal exactness

### History Management
- Separate histories for Standard and Scientific modes
- Maximum 100 items per mode (configurable)
- Items include timestamp, expression, result, and mode
- History persists during application session

### Error Handling
- Division by zero detection
- Invalid input validation
- Mathematical domain checking (e.g., negative logs)
- User-friendly error messages

## Testing

### Unit Test Coverage
The console demo (`ConsoleDemo.cs`) provides comprehensive testing of:
- High-precision decimal arithmetic
- Calculator engine operations
- History management
- Mode switching
- Error conditions

### Manual Testing Scenarios
1. **Precision Verification**: 1/3 * 3 = 1 (exactly)
2. **Mode Switching**: Operations available change correctly
3. **History Persistence**: Calculations saved and retrievable
4. **Error Handling**: Division by zero shows appropriate error
5. **Keyboard Input**: All shortcuts work as expected

## Performance Considerations

### Memory Usage
- BigInteger arithmetic requires more memory than built-in types
- History storage scales with number of calculations
- Consider clearing history for long-running sessions

### Computation Speed  
- Rational arithmetic is slower than floating-point
- Precision vs. speed trade-off is configurable
- Most operations complete in < 1ms for typical inputs

## Contributing

When modifying the calculator:
1. Maintain VS2010 compatibility in code structure
2. Preserve exact arithmetic for rational operations
3. Update both console demo and unit tests
4. Follow existing naming conventions and patterns
5. Document any new features or breaking changes

## License

This implementation follows the same MIT license as the parent repository.