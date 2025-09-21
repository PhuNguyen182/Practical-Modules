# Time-Based Visual Update System

A comprehensive Unity system for managing game visuals that change based on time progression (day, month, year). This system provides a robust, performance-optimized solution for creating dynamic environments that respond to time changes.

## Features

### Core System
- **Time Provider**: Configurable time progression with customizable speed and pause functionality
- **Visual System**: Centralized management of all time-based visual components
- **Service Integration**: Seamless integration with existing ServiceLocator and PlayerLoop systems
- **Performance Optimized**: Efficient update system with configurable update frequencies

### Visual Components
- **Lighting**: Dynamic lighting changes based on time of day and season
- **Skybox**: Procedural and material-based skybox transitions
- **Materials**: Custom material property animations and state management
- **Audio**: Time-based audio transitions with fade support
- **UI**: Dynamic UI element visibility, colors, and animations

### Configuration System
- **ScriptableObject Configurations**: Designer-friendly configuration system
- **Seasonal Presets**: Built-in seasonal configurations
- **Performance Settings**: Granular control over update frequencies and optimization

## Quick Start

### 1. Basic Setup

```csharp
// Create a visual system service
var serviceGO = new GameObject("TimeBasedVisualSystem");
var service = serviceGO.AddComponent<TimeBasedVisualSystemService>();
service.InitializeService();

// Get system references
var visualSystem = service.VisualSystem;
var timeProvider = service.TimeProvider;
```

### 2. Add Visual Components

```csharp
// Add lighting component
var lighting = gameObject.AddComponent<TimeBasedLighting>();

// Add skybox component
var skybox = gameObject.AddComponent<TimeBasedSkybox>();

// Add material properties component
var materials = gameObject.AddComponent<TimeBasedMaterialProperties>();
```

### 3. Control Time Progression

```csharp
// Set time speed (1.0 = real time, 10.0 = 10x faster)
timeProvider.TimeSpeed = 10.0f;

// Pause/resume time
timeProvider.IsPaused = true;

// Set specific time
timeProvider.SetTime(18, 30, 15, 6, 2024); // 6:30 PM, June 15, 2024

// Advance time
timeProvider.AdvanceTime(3600); // Advance by 1 hour
```

## Architecture

### Core Interfaces

#### ITimeProvider
Provides time information and controls time progression:
- Current time components (hour, minute, day, month, year)
- Time speed and pause controls
- Normalized time values for interpolation
- Time change events

#### ITimeBasedVisual
Base interface for all visual components:
- Visual ID and priority system
- Active/inactive state management
- Initialize, update, and cleanup methods
- Smooth transition support

#### ITimeBasedVisualSystem
Main system interface:
- Visual component registration and management
- System lifecycle management
- Integration with Unity's PlayerLoop
- Service locator integration

### Component System

#### TimeBasedLighting
Manages lighting changes based on time:
- Main light and additional lights support
- Environment lighting control
- Fog settings management
- Smooth color and intensity transitions

#### TimeBasedSkybox
Handles skybox transitions:
- Procedural skybox property control
- Material-based skybox support
- Sun direction and atmospheric effects
- Seasonal sky variations

#### TimeBasedMaterialProperties
Controls material property changes:
- Multiple material support
- Color, float, vector, and texture properties
- Non-destructive material editing
- Property interpolation

#### TimeBasedAudio
Manages audio transitions:
- Multiple audio source control
- Volume, pitch, and pan adjustments
- Audio clip transitions
- Fade between states

#### TimeBasedUI
Controls UI element changes:
- Canvas and UI element management
- Visibility, color, and transform control
- Animation curve support
- Time-based UI states

## Configuration System

### TimeBasedVisualConfiguration

Create configurations using the ScriptableObject system:

```csharp
// Create a default configuration
var config = TimeBasedVisualConfiguration.CreateDefault();

// Create a seasonal configuration
var springConfig = TimeBasedVisualConfiguration.CreateSeasonal(Season.Spring);

// Apply configuration to system
config.ApplyToSystem(visualSystem);

// Create visual system from configuration
var systemGO = config.CreateVisualSystem();
```

### Performance Settings

Configure update frequencies for optimal performance:

```csharp
var perfSettings = new PerformanceSettings
{
    UpdateFrequency = 0.1f,           // General update frequency
    LightingUpdateFrequency = 0.1f,   // Lighting-specific frequency
    SkyboxUpdateFrequency = 0.2f,     // Skybox-specific frequency
    MaterialUpdateFrequency = 0.1f,   // Material-specific frequency
    AudioUpdateFrequency = 0.1f,      // Audio-specific frequency
    UIUpdateFrequency = 0.2f,         // UI-specific frequency
    EnableSmoothTransitions = true,   // Enable smooth transitions
    DefaultTransitionDuration = 1f    // Default transition time
};
```

## Service Integration

### ServiceLocator Integration

The system integrates seamlessly with the existing ServiceLocator pattern:

```csharp
// Register service
var serviceLocator = ServiceLocator.Global;
serviceLocator.Register<ITimeBasedVisualSystem>(visualSystem);
serviceLocator.Register<ITimeProvider>(timeProvider);

// Use service from anywhere
var visualSystem = ServiceLocator.Global.Get<ITimeBasedVisualSystem>();
var timeProvider = ServiceLocator.Global.Get<ITimeProvider>();
```

### PlayerLoop Integration

The system uses Unity's PlayerLoop for efficient updates:

```csharp
// Automatic registration with PlayerLoop
// Components implement IUpdateHandler for efficient updates
public class TimeBasedVisualSystem : MonoBehaviour, IUpdateHandler
{
    public void Tick(float deltaTime)
    {
        // Update system
    }
}
```

## Performance Considerations

### Update Optimization
- Configurable update frequencies per component type
- Throttled updates to prevent excessive computation
- Priority-based update ordering
- Efficient state change detection

### Memory Management
- Object pooling for frequently created objects
- Efficient state interpolation
- Minimal garbage collection
- Proper cleanup and disposal

### Rendering Optimization
- Material instance management
- Efficient property updates
- Minimal draw calls
- LOD-aware updates

## Best Practices

### 1. Component Organization
- Use meaningful visual IDs for easy identification
- Set appropriate update priorities
- Group related visual elements together

### 2. Performance Tuning
- Adjust update frequencies based on visual complexity
- Use smooth transitions sparingly for performance-critical scenarios
- Monitor frame rate impact during development

### 3. State Management
- Create comprehensive state configurations
- Use seasonal presets for common scenarios
- Validate configurations before runtime

### 4. Debugging
- Enable debug logging during development
- Use the demo system for testing
- Monitor system status through UI

## Demo System

The included demo system provides:

### Interactive Controls
- Play/pause time progression
- Speed control (0.1x to 100x)
- Time presets (dawn, noon, dusk, night)
- Real-time time display

### Demo Scenarios
- Time progression demonstration
- Seasonal change showcase
- Component interaction examples
- Performance testing tools

### Usage
```csharp
// Setup demo
var demo = gameObject.AddComponent<TimeBasedVisualDemo>();
demo.SetupDemo();

// Run time progression demo
demo.RunTimeProgressionDemo(duration: 60f, speed: 10f);

// Demonstrate seasonal changes
demo.DemonstrateSeasonalChanges();
```

## API Reference

### Core Classes

#### TimeBasedVisualSystem
Main system class managing all visual components.

**Key Methods:**
- `RegisterVisual(ITimeBasedVisual visual)`: Register a visual component
- `UnregisterVisual(ITimeBasedVisual visual)`: Unregister a visual component
- `GetVisual(string visualId)`: Get visual component by ID
- `SetTimeProvider(ITimeProvider timeProvider)`: Set custom time provider
- `SetAutoUpdate(bool enabled)`: Enable/disable automatic updates

#### GameTimeProvider
Default time provider implementation.

**Key Methods:**
- `SetTime(int hour, int minute, int day, int month, int year)`: Set specific time
- `AdvanceTime(float seconds)`: Advance time by specified amount
- `GetNormalizedTime(TimeType timeType)`: Get normalized time for interpolation

#### TimeBasedVisualSystemService
Service wrapper for ServiceLocator integration.

**Key Methods:**
- `InitializeService()`: Initialize with default components
- `InitializeService(ITimeProvider timeProvider)`: Initialize with custom time provider
- `RegisterWithServiceLocator()`: Register with ServiceLocator
- `SetTimeSpeed(float speed)`: Control time progression speed

### Visual Components

#### TimeBasedLighting
Lighting management component.

**Key Properties:**
- `MainLight`: Primary light source
- `AdditionalLights`: Additional light sources
- `ControlEnvironmentLighting`: Enable environment lighting control
- `ControlFog`: Enable fog control

#### TimeBasedSkybox
Skybox management component.

**Key Properties:**
- `SkyboxComponent`: Unity Skybox component
- `SkyboxMaterial`: Material-based skybox
- `UseProceduralSkybox`: Enable procedural skybox

#### TimeBasedMaterialProperties
Material property management.

**Key Properties:**
- `Materials`: Target materials
- `Renderers`: Target renderers
- `CreateMaterialInstances`: Create non-destructive material instances

## Troubleshooting

### Common Issues

1. **Visual components not updating**
   - Check if components are registered with the system
   - Verify time provider is not paused
   - Ensure components are active and enabled

2. **Performance issues**
   - Reduce update frequencies in performance settings
   - Disable smooth transitions if not needed
   - Check for excessive visual component count

3. **ServiceLocator integration issues**
   - Ensure ServiceLocator is properly initialized
   - Verify service registration order
   - Check for circular dependencies

### Debug Tools

1. **System Status Display**
   - Real-time system status
   - Visual component count
   - Time progression status

2. **Console Logging**
   - Enable debug logging in components
   - Monitor update frequencies
   - Track state transitions

3. **Demo System**
   - Interactive testing interface
   - Time progression controls
   - Visual feedback

## Future Enhancements

### Planned Features
- Weather system integration
- Advanced seasonal effects
- Custom time curve support
- Multi-scene support
- Network synchronization

### Extension Points
- Custom visual component interfaces
- Plugin system for third-party components
- Advanced state machine integration
- Machine learning-based time prediction

## License

This system is part of the Practical Game Modules collection and follows the same licensing terms.

## Support

For questions, issues, or contributions, please refer to the project documentation or contact the development team.
