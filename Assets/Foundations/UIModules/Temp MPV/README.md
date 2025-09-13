# MVP UI System for Unity

A comprehensive UI system following the Model-View-Presenter (MVP) pattern for Unity, providing canvas management, popup stacking, overlay handling, and data binding capabilities.

## Features

### Core Components
- **MVP Pattern Implementation**: Complete Model-View-Presenter architecture
- **Canvas Management**: Multi-canvas system with different types and priorities
- **Popup System**: Stacking popups with priority management and modal support
- **Overlay System**: Full-screen overlays that can block input
- **Popup Creator**: Factory pattern for creating popups with data binding
- **Event System**: Comprehensive event handling throughout the system

### UI Canvas Types
- **Main**: Normal UI elements
- **Popup**: Popup dialogs with stacking
- **Overlay**: Full-screen blocking overlays
- **Loading**: Loading screens and progress indicators
- **Notification**: Toast notifications and alerts
- **Debug**: Developer tools and debug UI

## Architecture

### MVP Pattern
```
Model (Data & Business Logic) ←→ Presenter (Communication) ←→ View (UI & User Interaction)
```

### Base Classes
- `BaseModel<T>`: Handles data and business logic
- `BaseView<T>`: Manages UI presentation and user interaction
- `BasePresenter<TModel, TView, TData>`: Coordinates between Model and View

### Core Managers
- `UICanvasManager`: Manages different canvas types and their properties
- `UIPopupManager`: Handles popup stacking, priority, and lifecycle
- `UIOverlayManager`: Manages input-blocking overlays
- `PopupCreator`: Factory for creating popups with data binding
- `UISystemManager`: Main coordinator that ties everything together

## Usage

### 1. Setup
1. Add `UISystemManager` to your scene
2. Assign the required managers in the inspector:
   - `UICanvasManager`
   - `UIPopupManager`
   - `UIOverlayManager`
   - `PopupCreator`

### 2. Basic Usage

#### Show Simple Dialog
```csharp
UISystemManager.Instance.ShowSimpleDialog(
    "Title",
    "Message",
    () => Debug.Log("Button clicked!")
);
```

#### Show Confirm Dialog
```csharp
UISystemManager.Instance.ShowConfirmDialog(
    "Confirm Action",
    "Are you sure?",
    () => Debug.Log("Confirmed!"),
    () => Debug.Log("Cancelled!")
);
```

#### Show Loading Popup
```csharp
UISystemManager.Instance.ShowLoadingPopup("Loading...", true);
```

#### Show Overlay
```csharp
string overlayId = "loading_overlay";
UISystemManager.Instance.OverlayManager.ShowOverlay(
    overlayId,
    new Color(0, 0, 0, 0.5f),
    true,
    0
);
```

### 3. Creating Custom Popups

#### Step 1: Create Data Model
```csharp
[Serializable]
public class MyDialogData
{
    public string title;
    public string message;
    public Action onConfirm;
}
```

#### Step 2: Create Model
```csharp
public class MyDialogModel : BaseModel<MyDialogData>
{
    protected override void OnInitializeData(MyDialogData data)
    {
        Data = data;
    }
    
    public void OnConfirmClicked()
    {
        Data?.onConfirm?.Invoke();
    }
}
```

#### Step 3: Create View
```csharp
public class MyDialogView : BaseView<MyDialogData>
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    
    protected override void OnInitialize()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }
    
    protected override void OnUpdateView(MyDialogData data)
    {
        titleText.text = data.title;
        messageText.text = data.message;
    }
    
    private void OnConfirmClicked()
    {
        var presenter = GetComponent<MyDialogPresenter>();
        presenter?.OnConfirmClicked();
    }
}
```

#### Step 4: Create Presenter
```csharp
public class MyDialogPresenter : BasePresenter<MyDialogModel, MyDialogView, MyDialogData>
{
    protected override void OnTypedInitialize()
    {
        // Additional initialization
    }
    
    public void OnConfirmClicked()
    {
        TypedModel?.OnConfirmClicked();
    }
}
```

#### Step 5: Register Popup Type
```csharp
UISystemManager.Instance.PopupCreator.RegisterPopupType<MyDialogData>(
    "MyDialog",
    (data, parent) => {
        var model = new MyDialogModel();
        var view = CreateMyDialogView(parent);
        var presenter = new MyDialogPresenter();
        presenter.Initialize(model, view);
        model.Initialize(data);
        return presenter;
    }
);
```

## Example Implementations

The system includes several example implementations:

### Simple Dialog
- Basic dialog with title, message, and button
- Demonstrates simple MVP pattern usage

### Confirm Dialog
- Yes/No confirmation dialog
- Shows callback handling

### Input Dialog
- Text input with validation
- Demonstrates data binding

### Loading Popup
- Progress indicator with message
- Shows dynamic updates

## Events

### Canvas Manager Events
- `OnCanvasCreated`: Fired when a canvas is created
- `OnCanvasDestroyed`: Fired when a canvas is destroyed

### Popup Manager Events
- `OnPopupShown`: Fired when a popup is shown
- `OnPopupHidden`: Fired when a popup is hidden
- `OnPopupClosed`: Fired when a popup is closed
- `OnPopupStackChanged`: Fired when popup stack changes

### Overlay Manager Events
- `OnOverlayShown`: Fired when an overlay is shown
- `OnOverlayHidden`: Fired when an overlay is hidden

## Configuration

### Canvas Configuration
```csharp
var config = new CanvasConfig(
    UICanvasType.Popup,
    sortOrder: 100,
    isPersistent: false,
    name: "MyPopupCanvas"
);
```

### Popup Configuration
```csharp
var popupInfo = new PopupInfo(
    id: "unique_id",
    type: "MyDialog",
    priority: 0,
    modal: true,
    closeOnBackground: false,
    destroyOnClose: true,
    data: myData
);
```

## Best Practices

1. **Always use the UISystemManager singleton** for accessing UI components
2. **Implement proper disposal** in your custom MVP classes
3. **Use typed interfaces** when possible for better type safety
4. **Handle null checks** when accessing UI components
5. **Use appropriate canvas types** for different UI elements
6. **Set proper priorities** for popup stacking
7. **Clean up event subscriptions** in disposal methods

## Dependencies

- Unity 2020.3 or later
- TextMeshPro (for UI text components)
- Unity UI (uGUI) system

## License

This UI system is provided as-is for educational and development purposes.
