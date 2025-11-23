# CVDRiskScores

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Cross%20Platform-blueviolet)](https://github.com/dotnet/maui)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)
[![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20Mac%20%7C%20Tizen-brightgreen)]()
[![.NET Version](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![GitHub CI](https://img.shields.io/github/actions/workflow/status/fauxtix/CVDRiskScores/.NET.yml?branch=main)](https://github.com/fauxtix/CVDRiskScores/actions)


> 🇵🇹 [Versão em Português](Portuguese.md)

<p align="center">
  <img width="300" height="300" alt="CVDRiskScores" src="https://github.com/user-attachments/assets/e5bf61c2-c2c7-4402-b229-7709711bdf0e" />
</p> 

---

_CVDRiskScores is a multi-platform application for simulating and calculating cardiovascular risk scores, designed for clinicians, researchers, and health-minded individuals. The app enables users to estimate individual 10-year risk of cardiovascular events using validated algorithms such as SCORE2 and Framingham, based on key patient data (age, gender, blood pressure, cholesterol, smoking status, etc)_

Built on .NET 9 MAUI, CVDRiskScores delivers a cross-platform UX on mobile and desktop, with interactive results and localization support.

---

## 🚀 Features

- 🩺 **Risk Calculators:**  
  - ⚡ **SCORE2** — 10-year fatal/nonfatal CVD risk estimation, age-range calibration.
  - 🕰️ **Framingham** — Classic risk assessment with sex-specific tables.
- 👤 **Personalized Inputs:**  
  Age, gender, systolic blood pressure, total cholesterol, HDL cholesterol, smoking status, calibration region.
- ✨ **Interactive Results:**  
  - 🪟 Popup dialogs for risk breakdowns and calibration (low, moderate, high).
  - 📋 Copy-to-clipboard summaries for reporting and records.
- 🌍 **Localization:**  
  - 🗣️ Multi-language support (currently Portuguese, English; extensible via resource files).
- 💻 **Cross-Platform:**  
  Runs on Android, iOS, Windows, Mac Catalyst, and Tizen.

---
## ℹ️ Clinical Information and Algorithms

**CVDRiskScores** is based on two validated international algorithms for cardiovascular risk assessment: **SCORE2** and **Framingham Risk Score**.

### SCORE2
SCORE2 (European Society of Cardiology) estimates 10-year fatal and non-fatal cardiovascular risk for adults aged 40-69.
**Variables:** age, sex/gender, current smoking, total cholesterol, systolic blood pressure, region/calibration.
**Risk Categories:** Low, Moderate, High (thresholds depend on region/country).

### Framingham Risk Score
Calculates 10-year cardiovascular risk from age, gender, total cholesterol, HDL cholesterol, systolic blood pressure, antihypertensive treatment, and smoking status.

#### Example Framingham Score Table:
| Age  | Men | Women |
|------|-----|-------|
|20-34 | -9  | -7    |
|...   |     |       |

Final score gives risk percentage and category: low (<10%), moderate (10–20%), high (>20%).

**App popups present:**
- Detailed scores, percentages, and risk categories
- Clinical advice and validation notes
- Advanced diagnostics (LP, S0, factor contributions)

### Disclaimer
These algorithms support clinical decision-making but do not replace medical judgment.

### Useful Links:
- SCORE2: [ESC Guidelines](https://www.escardio.org)
- Framingham: [CDC](https://www.cdc.gov/heartdisease) · [PubMed](https://pubmed.ncbi.nlm.nih.gov/18212285/)

---
## 🧑‍💻 Technology Stack

- [.NET 9 MAUI](https://github.com/dotnet/maui)
- [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui)
- MVVM Architecture
- RESX resource localization

---

## 📦 Getting Started

### 👁️ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Compatible device: Android, iOS, Windows, Mac, Tizen
- [Visual Studio 2022/2023](https://visualstudio.microsoft.com/vs/) or VS Code with .NET MAUI support

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
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# 📱 Android/iOS/MacCatalyst/Tizen: see MAUI docs for device targets
```

---

## 📝 Usage

**Step-by-step Workflow:**

1. 🏁 **Select Calculator:**  
   Choose SCORE2 or Framingham from the bottom tabbar.
2. ✏️ **Enter Data:**  
   Input age, gender, BP, cholesterol, and smoking status.
3. 🔍 **View Results:**  
   Get risk percentage, category (low/medium/high), and color-coded feedback.
4. 🧾 **Explore Calibration:**  
   Open popups for region-specific risk profiles or sample cases.
5. 📋 **Copy or Share:**  
   Quickly export risk results for records or clinical communication.

---

## 🌐 Localization

- Change languages in settings (English/Portuguese supported, extensible).
- All UI text managed via RESX files.
- Contributions for additional languages welcome!

---

## 🖼️ Screenshots

👉 See all screenshots and UI examples here: [Screenshots.md](Screenshots.md)

---

## 📄 License

MIT License  
See [LICENSE.txt](LICENSE.txt) for details.

---

## 🍎 Credits & References

- SCORE2 algorithm: [ESC - European Society of Cardiology](https://escardio.org)
- Framingham Risk Score: [Framingham Heart Study](https://framinghamheartstudy.org)
- Built with .NET 9 MAUI and CommunityToolkit

---

## 🤝 Contributing

Pull requests, issues, and feature requests are welcome!  
See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📬 Contact

For questions or feedback, open an issue or contact [@fauxtix](https://github.com/fauxtix).
