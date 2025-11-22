# CVDRiskScores

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Cross%20Platform-blueviolet)](https://github.com/dotnet/maui)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
[![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20Mac%20%7C%20Tizen-brightgreen)]()
[![GitHub CI](https://img.shields.io/github/workflow/status/fauxtix/CVDRiskScores/.NET)](https://github.com/fauxtix/CVDRiskScores/actions) <!-- If you add CI -->

_CVDRiskScores is a multi-platform application for simulating and calculating cardiovascular risk scores, designed for clinicians, researchers, and health-minded individuals. The app enables users to estimate individual 10-year risk of cardiovascular events using validated algorithms such as SCORE2 and Framingham, based on key patient data (age, gender, blood pressure, cholesterol, smoking status, etc)._

Built on .NET MAUI, CVDRiskScores offers a modern, cross-platform user experience on mobile and desktop, with support for multiple languages and interactive results.

---

## Features

- **Cardiovascular Risk Calculators:**  
  - **SCORE2** (primary): 10-year fatal and nonfatal risk estimation, age range calibration.
  - **Framingham** (secondary): Classic risk assessment with sex-specific tables.
- **Personalized Inputs:**  
  Age, sex/gender, systolic blood pressure, total cholesterol, HDL cholesterol, smoking status, calibration region.
- **Interactive Results:**  
  - Popup dialogs with easy-to-understand risk breakdowns and calibration examples (low, moderate, high risk).
  - Copy-to-clipboard summaries for reporting and records.
- **Localization:**  
  - Multi-language support (currently Portuguese, English; extensible via RESX resource files).
- **Modern UI:**  
  - Responsive design with popup dialogs, navigation, and clear risk visualization.
- **Cross-Platform:**  
  - Runs on Android, iOS, Windows, Mac Catalyst, and Tizen.

---

## Technology Stack

- [.NET MAUI](https://github.com/dotnet/maui) (Multi-platform App UI)
- [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui)
- MVVM Architecture
- RESX resource files for localization

---

## Getting Started

### Prerequisites

- [.NET SDK 7.0+](https://dotnet.microsoft.com/download)
- Compatible platform: Android, iOS, Windows, Mac, Tizen
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recommended for building/debugging)

### Installation

```bash
git clone https://github.com/fauxtix/CVDRiskScores.git
cd CVDRiskScores
dotnet restore
dotnet build
```

To run on your target platform:
```bash
# Windows:
dotnet build -t:Run -f net7.0-windows10.0.19041.0

# Android/iOS/MacCatalyst/Tizen: see MAUI docs for device targets
```

---

## Usage

1. **Select Calculator:**  
   Choose SCORE2 or Framingham from the app dashboard.
2. **Enter Patient Data:**  
   Fill age, gender, risk factors (BP, cholesterol, smoking).
3. **View Results:**  
   Risk percentage and category (low/medium/high) are computed with clinical detail.
4. **Explore Calibration:**  
   Access popups for sample risk profiles, region-specific adjustments, etc.
5. **Copy or Share:**  
   Use copy-to-clipboard or reporting features to record/export results.

---

## Localization

- Change app language in settings.
- All UI text and labels are managed via resource files for easy extensibility.
- Contributions for new languages welcome!

---

## Screenshots

<!-- Add your screenshots here! -->
<!-- ![App Main Screen](screenshots/main.png) -->
<!-- ![Risk Result Popup](screenshots/result_popup.png) -->

---

## License

MIT License  
See [LICENSE.txt](LICENSE.txt) for details.

---

## Credits & References

- SCORE2 algorithm: [European Society of Cardiology](https://escardio.org)
- Framingham Risk Score: [Framingham Heart Study](https://framinghamheartstudy.org)
- Built with .NET MAUI and CommunityToolkit

---

## Contributing

Pull requests, issues and feature requests are welcome!  
See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## Contact

For questions or feedback, open an issue or contact [fauxtix](https://github.com/fauxtix).
