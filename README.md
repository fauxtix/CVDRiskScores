# CVDRiskScores

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Cross%20Platform-blueviolet)](https://github.com/dotnet/maui)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
[![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20Mac%20%7C%20Tizen-brightgreen)]()
[![GitHub CI](https://img.shields.io/github/actions/workflow/status/fauxtix/CVDRiskScores/.NET.yml?branch=main)](https://github.com/fauxtix/CVDRiskScores/actions)

_CVDRiskScores is a multi-platform application for simulating and calculating cardiovascular risk scores, designed for clinicians, researchers, and health-minded individuals. The app enables users to estimate individual 10-year risk of cardiovascular events using validated algorithms such as SCORE2 and Framingham, based on key patient data (age, gender, blood pressure, cholesterol, smoking status, etc)._

Built on .NET MAUI, CVDRiskScores offers a modern, cross-platform user experience on mobile and desktop, with support for multiple languages and interactive results.

---

## 🚀 Features

- 🩺 **Cardiovascular Risk Calculators:**  
  - ⚡ **SCORE2** — 10-year fatal and nonfatal CVD risk estimation, age-range calibration.
  - 🕰️ **Framingham** — Classic risk assessment with sex-specific tables.
- 👨‍⚕️👩‍⚕️ **Personalized Inputs:**  
  Age, gender, systolic blood pressure, total cholesterol, HDL cholesterol, smoking status, calibration region.
- ✨ **Interactive Results:**  
  - 🪟 Popup dialogs with easy-to-understand risk breakdowns and calibration examples (low, moderate, high).
  - 📋 Copy-to-clipboard summaries for reporting and records.
- 🌍 **Localization:**  
  - 🗣️ Multi-language support (currently Portuguese, English; extensible via RESX resource files).
- 📱🖥️ **Modern, Responsive UI:**  
  - Popups, navigation, and clear visualization of risk results.
- 💻 **Cross-Platform:**  
  - Runs on Android, iOS, Windows, Mac Catalyst, and Tizen.

---

## 🧑‍💻 Technology Stack

- [.NET MAUI](https://github.com/dotnet/maui) (Multi-platform App UI)
- [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui)
- MVVM Architecture
- RESX resource files for localization

---

## 📦 Getting Started

### 👁️ Prerequisites

- [.NET SDK 7.0+](https://dotnet.microsoft.com/download)
- Compatible device: Android, iOS, Windows, Mac, Tizen
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended)

### 🛠️ Installation

```bash
git clone https://github.com/fauxtix/CVDRiskScores.git
cd CVDRiskScores
dotnet restore
dotnet build
```

To run on your target platform:
```bash
# 🖥️ Windows:
dotnet build -t:Run -f net7.0-windows10.0.19041.0

# 📱 Android/iOS/MacCatalyst/Tizen: see MAUI docs for device targets
```

---

## 📝 Usage

1. **🏁 Select Calculator:**  
   Choose SCORE2 or Framingham from the dashboard.
2. **✏️ Enter Data:**  
   Fill in age, gender, blood pressure, cholesterol, smoking status, etc.
3. **🔎 View Results:**  
   See risk percentage and category (low/medium/high) — complete with colored feedback and details.
4. **📊 Explore Calibration:**  
   Use popups for sample risk profiles & region-specific adjustments.
5. **📋 Copy or Share:**  
   Copy results or export reports for patient records or further use.

---

## 🌐 Localization

- Change app language via settings (currently English and Portuguese).
- All UI text managed via resource files — easy to extend for new languages.
- Contributions for additional languages are welcome!

---

## 🖼️ Screenshots

<!-- Add your screenshots here! -->
<!-- ![App Main Screen](screenshots/main.png) -->
<!-- ![Risk Result Popup](screenshots/result_popup.png) -->

---

## 📄 License

MIT License  
See [LICENSE.txt](LICENSE.txt) for details.

---

## 🍎 Credits & References

- 🩺 SCORE2 algorithm: [European Society of Cardiology](https://escardio.org)
- 🕰️ Framingham Risk Score: [Framingham Heart Study](https://framinghamheartstudy.org)
- Built with .NET MAUI and CommunityToolkit

---

## 🤝 Contributing

Pull requests, issues, and feature requests are welcome!  
See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📬 Contact

For questions or feedback, open an issue or contact [fauxtix](https://github.com/fauxtix).
