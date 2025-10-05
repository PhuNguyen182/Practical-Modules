# Popup System - MVP Architecture

Hệ thống Popup Manager được thiết kế theo nguyên tắc MVP (Model-View-Presenter) với các tính năng linh hoạt và dễ sử dụng.

## 🏗️ Kiến trúc

```
Popups/
├── Interfaces/          # Các interface cơ bản
├── Data/               # Data structures
├── Views/              # Base View classes
├── Presenters/         # Base Presenter classes
├── Core/               # PopupManager core
├── Popups/             # Các popup cụ thể
│   ├── ConfirmPopup/
│   └── WaitingPopup/
└── Helpers/            # Utility classes
```

## 🎯 MVP Pattern Flow

```
External Data → Presenter → View
                ↑           ↓
           User Events ← View Events
```

## 📋 Các loại Popup

### 1. Popup thường (BasePopupView)
- Không cần data
- Kế thừa từ BasePopupView

### 2. Popup có data (BaseDataPopupView<TData>)
- Có thể nhận data từ bên ngoài
- Kế thừa từ BaseDataPopupView<TData>

### 3. ConfirmPopup
- Có 4 loại button: Yes, No, Close, OK
- Có thể bật/tắt từng button linh hoạt
- Hỗ trợ đóng popup khi click bên ngoài

### 4. WaitingPopup
- Phủ kín màn hình
- Có thể đặt timeout
- Có progress bar
- Có cancel button

## 🚀 Cách sử dụng

### Setup cơ bản

1. Thêm PopupManager vào scene:
```csharp
// Tạo GameObject với PopupManager component
var popupManager = new GameObject("PopupManager").AddComponent<PopupManager>();
```

2. Đặt popup prefabs vào folder `Resources/Popups/`

### Sử dụng PopupUtility (Đơn giản nhất)

```csharp
using Foundations.Popups.Helpers;

// Show Yes/No confirmation
PopupUtility.ShowConfirmDialog("Are you sure?", 
    onYes: () => Debug.Log("User clicked Yes"),
    onNo: () => Debug.Log("User clicked No"));

// Show OK dialog
PopupUtility.ShowInfoDialog("Operation completed!", 
    onOk: () => Debug.Log("User clicked OK"));

// Show waiting popup
PopupUtility.ShowWaitingDialog("Please wait...", 5f,
    onCompleted: () => Debug.Log("Waiting completed"));

// Show blocking waiting popup
PopupUtility.ShowBlockingWaitingDialog("Loading...",
    onCancel: () => Debug.Log("User cancelled"));
```

### Sử dụng PopupManager trực tiếp

```csharp
using Foundations.Popups.Interfaces;
using Foundations.Popups.Core;

// Get PopupManager instance
var popupManager = FindObjectOfType<PopupManager>();

// Show popup without data
var popup = popupManager.ShowPopup<YourPopupPresenter>();

// Show popup with data
var data = new YourPopupData { message = "Hello World" };
var popup = popupManager.ShowPopup<YourPopupPresenter, YourPopupData>(data);

// Hide popup
popupManager.HidePopup(popup);

// Hide all popups
popupManager.HideAllPopups();
```

### Sử dụng Extension Methods

```csharp
using Foundations.Popups.Helpers;

var popupManager = PopupUtility.PopupManager;

// Quick confirm popup
popupManager.ShowConfirmPopup("Delete item?", "Confirm", result => {
    if (result) Debug.Log("User confirmed");
    else Debug.Log("User cancelled");
});

// Quick OK popup
popupManager.ShowOkPopup("Success!", "Info", () => Debug.Log("User clicked OK"));

// Quick waiting popup
popupManager.ShowWaitingPopup("Loading...", 3f, 
    onCompleted: () => Debug.Log("Completed"),
    onCancel: () => Debug.Log("Cancelled"));
```

## 🔧 Tạo Popup mới

### 1. Tạo Data Structure

```csharp
[Serializable]
public class MyPopupData : PopupData
{
    public string title;
    public string message;
    
    public MyPopupData(string title, string message) : base()
    {
        this.title = title;
        this.message = message;
    }
}
```

### 2. Tạo View

```csharp
public class MyPopupView : BaseDataPopupView<MyPopupData>
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    
    protected override void RefreshUI()
    {
        if (ViewData == null) return;
        
        titleText.text = ViewData.title;
        messageText.text = ViewData.message;
    }
}
```

### 3. Tạo Presenter

```csharp
public class MyPopupPresenter : BaseDataPopupPresenter<MyPopupData, MyPopupData>
{
    [SerializeField] private MyPopupView myPopupView;
    
    public override IUIView<MyPopupData> View => myPopupView;
    
    // Override methods as needed
}
```

### 4. Tạo Prefab và đặt vào Resources/Popups/

## 📝 Events

Tất cả popups đều có các events:
- `OnShown` - Khi popup được hiển thị
- `OnHidden` - Khi popup bị ẩn
- `OnDestroyed` - Khi popup bị destroy
- `OnDataUpdated` - Khi data được cập nhật (chỉ popup có data)

## 🎨 Customization

### Popup Settings
- `priority` - Độ ưu tiên khi hiển thị nhiều popup
- `canCloseOnOutsideClick` - Có thể đóng khi click bên ngoài
- `timeoutDuration` - Thời gian timeout (chỉ WaitingPopup)

### UI Elements
- Tất cả UI elements đều có thể customize qua Inspector
- Hỗ trợ animation và effects
- Responsive design

## 🔍 Best Practices

1. **Luôn sử dụng PopupUtility** cho các popup đơn giản
2. **Đặt popup prefabs** vào folder Resources/Popups/
3. **Sử dụng events** để handle user interactions
4. **Clean up events** trong OnDestroy
5. **Test popup stacking** khi có nhiều popup cùng lúc

## 🐛 Troubleshooting

### Popup không hiển thị
- Kiểm tra PopupManager có trong scene không
- Kiểm tra popup prefab có đúng path không
- Kiểm tra Canvas settings

### Events không hoạt động
- Đảm bảo đã subscribe/unsubscribe đúng cách
- Kiểm tra popup có bị destroy sớm không

### Performance issues
- Sử dụng object pooling cho popups thường xuyên
- Limit số lượng popup đồng thời
- Optimize UI elements
