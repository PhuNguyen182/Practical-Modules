# Hệ Thống Xác Suất

## Tổng Quan

Tính năng này cung cấp một hệ thống xác suất và ngẫu nhiên toàn diện cho các game Unity với hai phương pháp chính: quản lý xác suất dựa trên độ hiếm và lựa chọn ngẫu nhiên có trọng số. Hệ thống được thiết kế để xử lý các tình huống xác suất phức tạp với các bộ điều chỉnh động, bảo vệ chuỗi thất bại, và phần thưởng dựa trên thời gian trong khi duy trì hiệu suất tối ưu.

### Tính Năng Chính
- **Hệ Thống Xác Suất Dựa Trên Độ Hiếm**: Quản lý xác suất cho các cấp độ hiếm khác nhau (Common, Uncommon, Rare, Epic, Legendary, Mythic)
- **Lựa Chọn Ngẫu Nhiên Có Trọng Số**: Lựa chọn ngẫu nhiên có trọng số hiệu quả với tối ưu hóa tìm kiếm nhị phân
- **Bộ Điều Chỉnh Xác Suất Động**: Điều chỉnh xác suất theo thời gian thực dựa trên tỷ lệ thành công và chuỗi
- **Bảo Vệ Chuỗi Thất Bại**: Ngăn chặn chuỗi thất bại dài cho các vật phẩm hiếm
- **Phần Thưởng Dựa Trên Thời Gian**: Tăng xác suất dựa trên thời gian kể từ lần thành công cuối
- **Tối Ưu Hóa Hiệu Suất**: Đường dẫn nóng không cấp phát bộ nhớ, tính toán được lưu trữ, và thuật toán hiệu quả
- **Thống Kê Toàn Diện**: Theo dõi số lần thử, thành công, tỷ lệ và chuỗi cho tất cả độ hiếm

### Trường Hợp Sử Dụng
- **Hệ Thống Gacha**: Triển khai hộp loot, gói thẻ, và phần thưởng ngẫu nhiên
- **Sinh Kẻ Thù**: Tạo kẻ thù ngẫu nhiên có trọng số với các cấp độ hiếm
- **Tạo Vật Phẩm**: Tạo vật phẩm ngẫu nhiên với phân phối độ hiếm có thể cấu hình
- **Kích Hoạt Sự Kiện**: Hệ thống sự kiện dựa trên xác suất với bộ điều chỉnh động
- **Cân Bằng Game**: Điều chỉnh độ khó game thông qua điều chỉnh xác suất

### Yêu Cầu Hệ Thống
- Unity 2021.3 trở lên
- C# 8.0 trở lên
- Không cần gói bổ sung nào

## Các Thành Phần Tính Năng

Tính năng này tuân theo kiến trúc mô-đun với sự phân tách rõ ràng các mối quan tâm và bao gồm các thành phần sau:

### Cấu Trúc Thư Mục
```
Probabilities/
├── ProbabilityHandleByRarity/     # Hệ thống xác suất dựa trên độ hiếm
│   ├── ProbabilityController.cs   # Bộ điều khiển chính cho xác suất độ hiếm
│   ├── RarityProbability.cs       # Dữ liệu xác suất độ hiếm cá nhân
│   ├── Rarity.cs                  # Định nghĩa enum độ hiếm
│   ├── ProbabilityPresets.cs      # Phân phối xác suất được định nghĩa sẵn
│   └── IProbabilityController.cs  # Interface cho bộ điều khiển xác suất
├── ProbabilityHandleByWeights/    # Hệ thống lựa chọn ngẫu nhiên có trọng số
│   ├── WeightedRandomSelector.cs  # Bộ lựa chọn ngẫu nhiên có trọng số chính
│   ├── WeightedRandomUtils.cs     # Phương thức tiện ích cho các thao tác nhanh
│   └── IWeightedRandomSelector.cs # Interface cho bộ lựa chọn có trọng số
└── README.md                      # Tài liệu này
```

### Các Thành Phần Cốt Lõi

#### ProbabilityHandleByRarity/ (Xử Lý Xác Suất Theo Độ Hiếm)
- **ProbabilityController.cs**: Bộ quản lý trung tâm xử lý logic xác suất dựa trên độ hiếm với hiệu suất được tối ưu hóa
- **RarityProbability.cs**: Cấu trúc dữ liệu có thể tuần tự hóa cho cấu hình độ hiếm cá nhân với các bộ điều chỉnh động
- **Rarity.cs**: Enum định nghĩa các cấp độ hiếm từ Thường đến Thần Thoại
- **ProbabilityPresets.cs**: Lớp tiện ích tĩnh cung cấp các phân phối xác suất phổ biến
- **IProbabilityController.cs**: Interface định nghĩa hợp đồng cho quản lý xác suất

#### ProbabilityHandleByWeights/ (Xử Lý Lựa Chọn Ngẫu Nhiên Có Trọng Số)
- **WeightedRandomSelector.cs**: Lựa chọn ngẫu nhiên có trọng số hiệu suất cao với tối ưu hóa tìm kiếm nhị phân
- **WeightedRandomUtils.cs**: Phương thức tiện ích tĩnh cho các thao tác ngẫu nhiên có trọng số nhanh
- **IWeightedRandomSelector.cs**: Interface định nghĩa hợp đồng cho lựa chọn ngẫu nhiên có trọng số

## Hướng Dẫn Sử Dụng

### Thiết Lập Ban Đầu

1. **Nhập Tính Năng**
   - Sao chép thư mục Probabilities vào thư mục Assets của bạn
   - Đảm bảo tất cả các phụ thuộc được nhập đúng cách

2. **Cấu Hình Hệ Thống Độ Hiếm**
   - Tạo một asset bộ điều khiển xác suất: `Assets/Create/Randomness/Probability Controller`
   - Thiết lập xác suất độ hiếm trong Inspector
   - Cấu hình các bộ điều chỉnh toàn cục và cài đặt chuẩn hóa

### Sử Dụng Cơ Bản

#### Hệ Thống Xác Suất Dựa Trên Độ Hiếm

```csharp
// Lấy bộ điều khiển xác suất
var probabilityController = FindObjectOfType<ProbabilityController>();

// Khởi tạo với seed tùy chọn để có kết quả có thể tái tạo
probabilityController.Initialize(seed: 12345);

// Quay xúc xắc cho một độ hiếm cụ thể
bool success = probabilityController.RollForRarity(Rarity.Legendary);

// Lấy một độ hiếm ngẫu nhiên dựa trên xác suất có trọng số
Rarity selectedRarity = probabilityController.SelectRandomRarity();

// Lấy xác suất hiện tại cho một độ hiếm
float legendaryChance = probabilityController.GetProbability(Rarity.Legendary);

// Lấy thống kê cho một độ hiếm
var (attempts, successes, rate, streak) = probabilityController.GetRarityStats(Rarity.Epic);
Debug.Log($"Số lần thử Epic: {attempts}, tỷ lệ thành công: {rate:P2}, chuỗi hiện tại: {streak}");
```

**Giải thích chi tiết:**
- `Initialize(seed)`: Khởi tạo hệ thống với một seed cụ thể để đảm bảo kết quả có thể tái tạo (hữu ích cho testing)
- `RollForRarity()`: Thực hiện một lần quay xúc xắc cho độ hiếm cụ thể, trả về true/false
- `SelectRandomRarity()`: Chọn ngẫu nhiên một độ hiếm dựa trên tất cả xác suất hiện tại
- `GetProbability()`: Lấy xác suất hiện tại (đã bao gồm tất cả modifiers) của một độ hiếm
- `GetRarityStats()`: Lấy thống kê chi tiết về số lần thử, thành công, tỷ lệ và chuỗi thất bại

#### Lựa Chọn Ngẫu Nhiên Có Trọng Số

```csharp
// Tạo bộ lựa chọn có trọng số
float[] weights = { 10f, 5f, 2f, 1f }; // Trọng số cao hơn = xác suất cao hơn
var selector = new WeightedRandomSelector(weights);

// Lấy một chỉ số ngẫu nhiên
int selectedIndex = selector.GetRandomIndex();

// Lấy nhiều chỉ số ngẫu nhiên
int[] multipleIndices = selector.GetRandomIndices(3);

// Lấy các chỉ số ngẫu nhiên duy nhất (không trùng lặp)
int[] uniqueIndices = selector.GetUniqueRandomIndices(2);

// Phương thức tiện ích nhanh
int quickIndex = WeightedRandomUtils.GetRandomIndex(weights);
int[] quickIndices = WeightedRandomUtils.GetRandomIndices(weights, 5);
```

**Giải thích chi tiết:**
- `weights`: Mảng các trọng số, chỉ số có trọng số cao hơn sẽ có xác suất được chọn cao hơn
- `GetRandomIndex()`: Chọn một chỉ số dựa trên phân phối trọng số
- `GetRandomIndices()`: Chọn nhiều chỉ số (có thể trùng lặp)
- `GetUniqueRandomIndices()`: Chọn nhiều chỉ số duy nhất (không trùng lặp)
- `WeightedRandomUtils`: Các phương thức tiện ích để thực hiện nhanh mà không cần tạo object

#### Sử Dụng Nâng Cao

```csharp
// Thay đổi xác suất trong thời gian chạy
probabilityController.ModifyProbability(Rarity.Rare, 0.2f);

// Áp dụng bộ nhân tạm thời
probabilityController.ApplyTemporaryMultiplier(Rarity.Epic, 2.0f);

// Mô phỏng nhiều lần quay để kiểm tra
var simulationResults = probabilityController.SimulateRolls(10000);
foreach (var kvp in simulationResults)
{
    Debug.Log($"{kvp.Key}: {kvp.Value:P2}");
}

// Sử dụng các preset xác suất
var standardDistribution = ProbabilityPresets.CreateStandardDistribution();
var generousDistribution = ProbabilityPresets.CreateGenerousDistribution();
var harshDistribution = ProbabilityPresets.CreateHarshDistribution();
```

**Giải thích chi tiết:**
- `ModifyProbability()`: Thay đổi xác suất cơ bản của một độ hiếm (ảnh hưởng đến tất cả tính toán sau)
- `ApplyTemporaryMultiplier()`: Áp dụng bộ nhân tạm thời (chỉ ảnh hưởng đến lần quay tiếp theo)
- `SimulateRolls()`: Mô phỏng nhiều lần quay để kiểm tra phân phối xác suất
- `ProbabilityPresets`: Các phân phối xác suất được định nghĩa sẵn cho các trường hợp sử dụng phổ biến

## Thiết Lập Unity GameObject

### Các GameObject Cần Thiết

1. **Probability Controller GameObject**
   - Tạo một GameObject trống trong scene của bạn
   - Đặt tên là "ProbabilityManager"
   - Thêm component `ProbabilityController` vào GameObject này

2. **ScriptableObject Asset Setup**
   - Tạo một ProbabilityController asset trong cửa sổ Project
   - Cấu hình xác suất độ hiếm và các cài đặt
   - Tham chiếu asset này trong các script MonoBehaviour của bạn

### Cấu Hình Component

#### Component ProbabilityController
1. **Chọn ProbabilityController GameObject**
2. **Trong Inspector, cấu hình các cài đặt sau:**

**Cấu Hình Rarity Probability (Xác Suất Độ Hiếm):**
- **Rarity Probabilities**: Danh sách các đối tượng RarityProbability với xác suất cơ bản và bộ điều chỉnh

**Bộ Điều Chỉnh Toàn Cục (Global Modifiers):**
- **Global Probability Multiplier**: Bộ nhân tổng thể cho tất cả xác suất (0.1 - 3.0)
- **Enable Global Luck Factor**: Có áp dụng các điều chỉnh may mắn toàn cục hay không
- **Luck Factor**: Bộ nhân may mắn toàn cục (0.8 - 1.2)

**Cài Đặt Chuẩn Hóa (Normalization Settings):**
- **Auto Normalize Probabilities**: Tự động chuẩn hóa xác suất để tổng bằng 1.0
- **Maintain Relative Ratios**: Duy trì tỷ lệ xác suất tương đối trong quá trình chuẩn hóa

#### Cấu Hình RarityProbability
Đối với mỗi độ hiếm trong danh sách, cấu hình:

**Cài Đặt Xác Suất Cơ Bản:**
- **Rarity**: Cấp độ hiếm (Common, Uncommon, Rare, Epic, Legendary, Mythic)
- **Base Probability**: Giá trị xác suất cơ bản (0.0 - 1.0)
- **Weight Multiplier**: Bộ nhân trọng số bổ sung (0.0 - 10.0)

**Bộ Điều Chỉnh Xác Suất Động:**
- **Enable Dynamic Scaling**: Điều chỉnh xác suất dựa trên tỷ lệ thành công
- **Dynamic Scaling Factor**: Độ mạnh của mở rộng động (0.0 - 2.0)

**Bảo Vệ Chuỗi (Streak Protection):**
- **Enable Streak Protection**: Ngăn chặn chuỗi thất bại dài
- **Max Consecutive Failures**: Số lần thất bại tối đa trước khi bảo vệ kích hoạt
- **Streak Protection Multiplier**: Bộ nhân bonus khi bảo vệ hoạt động (1.0 - 5.0)

**Bộ Điều Chỉnh Dựa Trên Thời Gian:**
- **Enable Time Bonus**: Tăng xác suất dựa trên thời gian kể từ lần thành công cuối
- **Time Bonus Multiplier**: Bộ nhân bonus thời gian tối đa (1.0 - 5.0)
- **Time Bonus Duration Hours**: Số giờ cần thiết cho bonus thời gian tối đa

### Quy Trình Thiết Lập Từng Bước

1. **Tạo Probability Controller Asset**
   ```
   Nhấp chuột phải trong cửa sổ Project
   Create → Randomness → Probability Controller
   Tên: "GameProbabilityController"
   ```

2. **Cấu Hình Rarity Probabilities**
   ```
   Chọn ProbabilityController asset
   Trong Inspector, mở rộng "Rarity Probability Configuration"
   Đặt Size thành 6 (cho tất cả cấp độ hiếm)
   Cấu hình mỗi độ hiếm với xác suất phù hợp
   ```

3. **Thiết Lập Global Modifiers**
   ```
   Global Probability Multiplier: 1.0
   Enable Global Luck Factor: false (để có kết quả nhất quán)
   Luck Factor: 1.0
   ```

4. **Cấu Hình Normalization**
   ```
   Auto Normalize Probabilities: true
   Maintain Relative Ratios: true
   ```

5. **Kiểm Tra Thiết Lập**
   ```
   Tạo script test để xác minh chức năng
   Nhấn Play trong Unity
   Xác minh không có lỗi trong Console
   Test các tính toán xác suất
   ```

**Hướng dẫn chi tiết từng bước:**

**Bước 1: Tạo Asset**
- Trong Unity, mở cửa sổ Project
- Nhấp chuột phải vào thư mục Assets
- Chọn Create → Randomness → Probability Controller
- Đặt tên file là "GameProbabilityController"

**Bước 2: Cấu hình độ hiếm**
- Chọn asset vừa tạo
- Trong Inspector, tìm phần "Rarity Probability Configuration"
- Nhấn vào dấu "+" để thêm 6 độ hiếm
- Cấu hình từng độ hiếm:
  - Common: 0.5 (50%)
  - Uncommon: 0.3 (30%)
  - Rare: 0.15 (15%)
  - Epic: 0.04 (4%)
  - Legendary: 0.008 (0.8%)
  - Mythic: 0.002 (0.2%)

**Bước 3: Thiết lập modifiers toàn cục**
- Global Probability Multiplier: 1.0 (không thay đổi tổng thể)
- Enable Global Luck Factor: false (tắt để có kết quả ổn định)
- Luck Factor: 1.0 (không có may mắn bổ sung)

**Bước 4: Cấu hình chuẩn hóa**
- Auto Normalize Probabilities: true (tự động chuẩn hóa)
- Maintain Relative Ratios: true (giữ tỷ lệ tương đối)

**Bước 5: Test và kiểm tra**
- Tạo script test đơn giản để gọi các method
- Chạy game và kiểm tra Console không có lỗi
- Test các method như SelectRandomRarity() và RollForRarity()

## Tham Chiếu API

### Lớp ProbabilityController

#### Các Phương Thức Công Khai

**Initialize(int? seed = null)**
- Khởi tạo bộ điều khiển xác suất với seed tùy chọn để có kết quả có thể tái tạo
- Tham số: `seed` - Giá trị seed tùy chọn cho việc tạo số ngẫu nhiên
- Trả về: `void`
- Ví dụ:
  ```csharp
  probabilityController.Initialize(12345);
  ```

**GetProbability(Rarity rarity)**
- Lấy xác suất hiện tại cho một độ hiếm cụ thể bao gồm tất cả các bộ điều chỉnh
- Tham số: `rarity` - Độ hiếm cần lấy xác suất
- Trả về: `float` - Giá trị xác suất từ 0 đến 1
- Ví dụ:
  ```csharp
  float legendaryChance = probabilityController.GetProbability(Rarity.Legendary);
  ```

**RollForRarity(Rarity rarity)**
- Quay xúc xắc cho một độ hiếm cụ thể và trả về thành công/thất bại
- Tham số: `rarity` - Độ hiếm cần quay
- Trả về: `bool` - True nếu quay thành công
- Ví dụ:
  ```csharp
  bool gotLegendary = probabilityController.RollForRarity(Rarity.Legendary);
  ```

**SelectRandomRarity()**
- Chọn một độ hiếm ngẫu nhiên dựa trên xác suất có trọng số
- Tham số: Không có
- Trả về: `Rarity` - Độ hiếm được chọn dựa trên xác suất hiện tại
- Ví dụ:
  ```csharp
  Rarity selected = probabilityController.SelectRandomRarity();
  ```

**SimulateRolls(int rollCount)**
- Mô phỏng nhiều lần quay và trả về thống kê phân phối
- Tham số: `rollCount` - Số lần quay cần mô phỏng
- Trả về: `Dictionary<Rarity, float>` - Phần trăm phân phối
- Ví dụ:
  ```csharp
  var results = probabilityController.SimulateRolls(10000);
  ```

#### Các Thuộc Tính Công Khai

**IsInitialized**
- Lấy thông tin xem bộ điều khiển đã được khởi tạo chưa
- Kiểu: `bool` (thuộc tính nội bộ)
- Ví dụ:
  ```csharp
  if (probabilityController.IsInitialized)
  {
      // Bộ điều khiển đã sẵn sàng sử dụng
  }
  ```

### Lớp WeightedRandomSelector

#### Các Phương Thức Công Khai

**GetRandomIndex()**
- Lấy một chỉ số ngẫu nhiên dựa trên phân phối trọng số
- Tham số: Không có
- Trả về: `int` - Chỉ số ngẫu nhiên dựa trên trọng số
- Ví dụ:
  ```csharp
  int selectedIndex = selector.GetRandomIndex();
  ```

**GetRandomIndices(int count)**
- Lấy nhiều chỉ số ngẫu nhiên với thay thế
- Tham số: `count` - Số lượng chỉ số cần chọn
- Trả về: `int[]` - Mảng các chỉ số ngẫu nhiên
- Ví dụ:
  ```csharp
  int[] indices = selector.GetRandomIndices(5);
  ```

**GetUniqueRandomIndices(int count)**
- Lấy nhiều chỉ số ngẫu nhiên duy nhất không thay thế
- Tham số: `count` - Số lượng chỉ số duy nhất cần chọn
- Trả về: `int[]` - Mảng các chỉ số ngẫu nhiên duy nhất
- Ví dụ:
  ```csharp
  int[] uniqueIndices = selector.GetUniqueRandomIndices(3);
  ```

### Sự Kiện

**OnProbabilityChanged** (Nội bộ)
- Được kích hoạt khi các giá trị xác suất được thay đổi
- Kiểu Sự kiện: `Action<Rarity, float>`
- Ví dụ:
  ```csharp
  // Sự kiện nội bộ để theo dõi thay đổi xác suất
  ```

## Tùy Chọn Cấu Hình

### Cài Đặt ProbabilityController

#### Cài Đặt Chung

**Global Probability Multiplier** (float)
- Điều khiển mở rộng xác suất tổng thể cho tất cả độ hiếm
- Phạm vi: 0.1f - 3.0f
- Mặc định: 1.0f
- Ví dụ: Đặt thành 1.5f để tăng tất cả xác suất lên 50%

**Enable Global Luck Factor** (bool)
- Có áp dụng các điều chỉnh may mắn toàn cục hay không
- Mặc định: false
- Ví dụ: Đặt thành true để điều chỉnh độ khó động

**Luck Factor** (float)
- Bộ nhân may mắn toàn cục áp dụng cho tất cả xác suất
- Phạm vi: 0.8f - 1.2f
- Mặc định: 1.0f
- Ví dụ: Đặt thành 1.1f để có bonus may mắn 10%

#### Cài Đặt Chuẩn Hóa

**Auto Normalize Probabilities** (bool)
- Tự động chuẩn hóa xác suất để tổng bằng 1.0
- Mặc định: true
- Ví dụ: Đặt thành false cho phân phối xác suất tùy chỉnh

**Maintain Relative Ratios** (bool)
- Duy trì tỷ lệ xác suất tương đối trong quá trình chuẩn hóa
- Mặc định: true
- Ví dụ: Đặt thành false cho phân phối xác suất bằng nhau

### Cài Đặt RarityProbability

#### Cài Đặt Xác Suất Cơ Bản

**Base Probability** (float)
- Giá trị xác suất cơ bản cho độ hiếm
- Phạm vi: 0.0f - 1.0f
- Mặc định: Thay đổi theo độ hiếm
- Ví dụ: Đặt thành 0.05f cho 5% cơ hội cơ bản

**Weight Multiplier** (float)
- Bộ nhân trọng số bổ sung cho độ hiếm
- Phạm vi: 0.0f - 10.0f
- Mặc định: 1.0f
- Ví dụ: Đặt thành 2.0f để tăng gấp đôi xác suất hiệu quả

#### Bộ Điều Chỉnh Xác Suất Động

**Enable Dynamic Scaling** (bool)
- Điều chỉnh xác suất dựa trên tỷ lệ thành công lịch sử
- Mặc định: false
- Ví dụ: Đặt thành true cho xác suất tự cân bằng

**Dynamic Scaling Factor** (float)
- Độ mạnh của điều chỉnh mở rộng động
- Phạm vi: 0.0f - 2.0f
- Mặc định: 1.0f
- Ví dụ: Đặt thành 1.5f cho điều chỉnh động mạnh hơn

#### Bảo Vệ Chuỗi

**Enable Streak Protection** (bool)
- Ngăn chặn chuỗi thất bại dài
- Mặc định: false
- Ví dụ: Đặt thành true để đảm bảo drop hiếm sau nhiều lần thất bại

**Max Consecutive Failures** (int)
- Số lần thất bại tối đa trước khi bảo vệ kích hoạt
- Phạm vi: 1 - 1000
- Mặc định: 10
- Ví dụ: Đặt thành 50 để bảo vệ sau 50 lần thất bại

**Streak Protection Multiplier** (float)
- Bộ nhân bonus khi bảo vệ hoạt động
- Phạm vi: 1.0f - 5.0f
- Mặc định: 1.5f
- Ví dụ: Đặt thành 3.0f cho bonus xác suất 3x

#### Bộ Điều Chỉnh Dựa Trên Thời Gian

**Enable Time Bonus** (bool)
- Tăng xác suất dựa trên thời gian kể từ lần thành công cuối
- Mặc định: false
- Ví dụ: Đặt thành true cho bonus xác suất dựa trên thời gian

**Time Bonus Multiplier** (float)
- Bộ nhân bonus thời gian tối đa
- Phạm vi: 1.0f - 5.0f
- Mặc định: 1.2f
- Ví dụ: Đặt thành 2.0f cho bonus thời gian tối đa 2x

**Time Bonus Duration Hours** (float)
- Số giờ cần thiết cho bonus thời gian tối đa
- Phạm vi: 1.0f - 168.0f (1 tuần)
- Mặc định: 24.0f
- Ví dụ: Đặt thành 48.0f cho chu kỳ bonus thời gian 2 ngày

### Cấu Hình Runtime

Bạn có thể thay đổi cấu hình trong thời gian chạy:

```csharp
// Thay đổi xác suất cơ bản
probabilityController.ModifyProbability(Rarity.Rare, 0.15f);

// Áp dụng bộ nhân tạm thời
probabilityController.ApplyTemporaryMultiplier(Rarity.Epic, 2.0f);

// Đặt lại tất cả dữ liệu theo dõi
probabilityController.ResetAllTracking();

// Xác thực cấu hình hiện tại
bool isValid = probabilityController.ValidateConfiguration();
```

## Khắc Phục Sự Cố

### Các Vấn Đề Thường Gặp

#### Vấn đề: "Xác suất không hoạt động như mong đợi"
**Triệu chứng:**
- Xác suất không khớp với giá trị đã cấu hình
- Kết quả ngẫu nhiên có vẻ không chính xác

**Giải pháp:**
1. Kiểm tra ProbabilityController đã được khởi tạo đúng cách
2. Xác minh tất cả xác suất độ hiếm được cấu hình chính xác
3. Đảm bảo cài đặt auto-normalization phù hợp
4. Kiểm tra các bộ điều chỉnh toàn cục xung đột
5. Xác thực cấu hình bằng ValidateConfiguration()

#### Vấn đề: "Vấn đề hiệu suất với mô phỏng lớn"
**Triệu chứng:**
- Tốc độ khung hình giảm trong quá trình tính toán xác suất
- Kết quả mô phỏng chậm

**Giải pháp:**
1. Sử dụng kích thước batch mô phỏng nhỏ hơn
2. Lưu trữ các tính toán xác suất khi có thể
3. Tránh thay đổi xác suất thường xuyên
4. Sử dụng WeightedRandomSelector cho lựa chọn có trọng số đơn giản
5. Profile với Unity Profiler để xác định bottleneck

#### Vấn đề: "Bảo vệ chuỗi không hoạt động"
**Triệu chứng:**
- Chuỗi thất bại dài tiếp tục mặc dù có cài đặt bảo vệ
- Bộ nhân bảo vệ không áp dụng

**Giải pháp:**
1. Xác minh bảo vệ chuỗi được bật cho độ hiếm
2. Kiểm tra ngưỡng max consecutive failures
3. Đảm bảo streak protection multiplier được đặt đúng
4. Xác minh dữ liệu theo dõi được ghi lại đúng cách
5. Test với ngưỡng thấp hơn để dễ xác minh

#### Vấn đề: "Time bonus không áp dụng"
**Triệu chứng:**
- Bonus dựa trên thời gian không tăng xác suất
- Last success time không cập nhật

**Giải pháp:**
1. Kiểm tra time bonus được bật cho độ hiếm
2. Xác minh time bonus duration được đặt phù hợp
3. Đảm bảo last success time được ghi lại
4. Test với thời gian ngắn hơn để dễ xác minh
5. Kiểm tra system time và chức năng DateTime.Now

### Mẹo Debug

**Bật Debug Logging:**
```csharp
// Thêm debug logging để theo dõi tính toán xác suất
Debug.Log($"Xác suất hiện tại cho {rarity}: {probability}");
```

**Theo Dõi Thống Kê:**
```csharp
// Kiểm tra thống kê hiện tại cho tất cả độ hiếm
foreach (Rarity rarity in Enum.GetValues<Rarity>())
{
    var stats = probabilityController.GetRarityStats(rarity);
    Debug.Log($"{rarity}: {stats.attempts} lần thử, tỷ lệ thành công {stats.rate:P2}");
}
```

**Xác Thực Cấu Hình:**
```csharp
// Xác thực cấu hình và ghi log các vấn đề
if (!probabilityController.ValidateConfiguration())
{
    Debug.LogError("Cấu hình xác suất không hợp lệ!");
}
```

**Test Với Mô Phỏng:**
```csharp
// Chạy mô phỏng để xác minh phân phối xác suất
var results = probabilityController.SimulateRolls(10000);
foreach (var kvp in results)
{
    Debug.Log($"{kvp.Key}: {kvp.Value:P2} (mong đợi: {expectedProbability:P2})");
}
```

### Mẹo Tối Ưu Hóa Hiệu Suất

**Sử Dụng Cấu Trúc Dữ Liệu Phù Hợp:**
- Sử dụng arrays thay vì Lists cho collections có kích thước cố định
- Lưu trữ các giá trị được truy cập thường xuyên
- Tránh LINQ trong hot paths

**Tối Ưu Hóa Lựa Chọn Ngẫu Nhiên:**
- Sử dụng binary search cho mảng trọng số lớn
- Lưu trữ cumulative weights khi có thể
- Giảm thiểu allocations trong selection loops

**Thao Tác Batch:**
- Nhóm nhiều thay đổi xác suất
- Sử dụng bulk operations khi có thể
- Tránh cập nhật cá nhân thường xuyên

**Quản Lý Bộ Nhớ:**
- Tái sử dụng temporary collections
- Xóa các cấu trúc dữ liệu không sử dụng
- Theo dõi tần suất garbage collection

Bằng cách tuân theo các hướng dẫn khắc phục sự cố và mẹo tối ưu hóa này, bạn có thể đảm bảo hiệu suất tối ưu và hành vi chính xác của hệ thống xác suất trong dự án Unity của bạn.

## Ví Dụ Thực Tế

### Ví Dụ 1: Hệ Thống Gacha Đơn Giản

```csharp
public class GachaSystem : MonoBehaviour
{
    [SerializeField] private ProbabilityController probabilityController;
    
    public void OpenGachaBox()
    {
        // Khởi tạo nếu chưa được khởi tạo
        if (!probabilityController.IsInitialized)
        {
            probabilityController.Initialize();
        }
        
        // Chọn độ hiếm ngẫu nhiên
        Rarity selectedRarity = probabilityController.SelectRandomRarity();
        
        // Tạo vật phẩm dựa trên độ hiếm
        Item newItem = CreateItemByRarity(selectedRarity);
        
        // Hiển thị kết quả
        ShowGachaResult(newItem, selectedRarity);
        
        // Ghi log thống kê
        var stats = probabilityController.GetRarityStats(selectedRarity);
        Debug.Log($"Đã nhận {selectedRarity} item. Tổng số lần thử: {stats.attempts}");
    }
    
    private Item CreateItemByRarity(Rarity rarity)
    {
        // Logic tạo vật phẩm dựa trên độ hiếm
        switch (rarity)
        {
            case Rarity.Common:
                return new Item("Vũ khí thường", ItemType.Weapon, 100);
            case Rarity.Uncommon:
                return new Item("Vũ khí không thường", ItemType.Weapon, 200);
            case Rarity.Rare:
                return new Item("Vũ khí hiếm", ItemType.Weapon, 500);
            case Rarity.Epic:
                return new Item("Vũ khí huyền thoại", ItemType.Weapon, 1000);
            case Rarity.Legendary:
                return new Item("Vũ khí huyền thoại", ItemType.Weapon, 2000);
            case Rarity.Mythic:
                return new Item("Vũ khí thần thoại", ItemType.Weapon, 5000);
            default:
                return new Item("Vật phẩm không xác định", ItemType.Misc, 0);
        }
    }
}
```

### Ví Dụ 2: Hệ Thống Sinh Kẻ Thù Có Trọng Số

```csharp
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float[] spawnWeights = { 10f, 5f, 2f, 1f }; // Trọng số cho từng loại kẻ thù
    
    private WeightedRandomSelector enemySelector;
    
    void Start()
    {
        // Khởi tạo bộ lựa chọn có trọng số
        enemySelector = new WeightedRandomSelector(spawnWeights);
    }
    
    public void SpawnEnemy()
    {
        // Chọn loại kẻ thù ngẫu nhiên
        int enemyTypeIndex = enemySelector.GetRandomIndex();
        
        // Tạo kẻ thù
        GameObject enemyPrefab = enemyPrefabs[enemyTypeIndex];
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        Debug.Log($"Đã sinh kẻ thù loại {enemyTypeIndex} tại vị trí {spawnPosition}");
    }
    
    public void SpawnMultipleEnemies(int count)
    {
        // Sinh nhiều kẻ thù duy nhất
        int[] enemyIndices = enemySelector.GetUniqueRandomIndices(count);
        
        for (int i = 0; i < enemyIndices.Length; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[enemyIndices[i]];
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        
        Debug.Log($"Đã sinh {enemyIndices.Length} kẻ thù duy nhất");
    }
}
```

### Ví Dụ 3: Hệ Thống Phần Thưởng Hàng Ngày

```csharp
public class DailyRewardSystem : MonoBehaviour
{
    [SerializeField] private ProbabilityController rewardController;
    
    public void ClaimDailyReward()
    {
        // Kiểm tra xem đã nhận phần thưởng hôm nay chưa
        if (HasClaimedToday())
        {
            Debug.Log("Bạn đã nhận phần thưởng hôm nay rồi!");
            return;
        }
        
        // Áp dụng bonus thời gian cho phần thưởng hiếm
        rewardController.ApplyTemporaryMultiplier(Rarity.Epic, 1.5f);
        rewardController.ApplyTemporaryMultiplier(Rarity.Legendary, 2.0f);
        
        // Chọn phần thưởng
        Rarity rewardRarity = rewardController.SelectRandomRarity();
        
        // Tạo phần thưởng
        Reward dailyReward = CreateDailyReward(rewardRarity);
        
        // Lưu trạng thái đã nhận
        SaveClaimedReward(dailyReward);
        
        // Hiển thị kết quả
        ShowRewardPopup(dailyReward);
        
        Debug.Log($"Phần thưởng hàng ngày: {rewardRarity} - {dailyReward.name}");
    }
    
    private Reward CreateDailyReward(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return new Reward("Tiền vàng", RewardType.Currency, 1000);
            case Rarity.Uncommon:
                return new Reward("Tiền vàng", RewardType.Currency, 2500);
            case Rarity.Rare:
                return new Reward("Tiền vàng", RewardType.Currency, 5000);
            case Rarity.Epic:
                return new Reward("Đá quý", RewardType.Currency, 100);
            case Rarity.Legendary:
                return new Reward("Đá quý", RewardType.Currency, 250);
            case Rarity.Mythic:
                return new Reward("Đá quý", RewardType.Currency, 500);
            default:
                return new Reward("Phần thưởng không xác định", RewardType.Misc, 0);
        }
    }
}
```

Những ví dụ này cho thấy cách sử dụng hệ thống xác suất trong các tình huống thực tế phổ biến trong game development.
