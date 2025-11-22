# CVDRiskScores

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-Multiplataforma-blueviolet)](https://github.com/dotnet/maui)
[![Licença: MIT](https://img.shields.io/badge/Licen%C3%A7a-MIT-yellow.svg)](LICENSE.txt)
[![Plataformas](https://img.shields.io/badge/plataformas-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20Mac%20%7C%20Tizen-brightgreen)]()
[![.NET Versão](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![GitHub CI](https://img.shields.io/github/actions/workflow/status/fauxtix/CVDRiskScores/.NET.yml?branch=main)](https://github.com/fauxtix/CVDRiskScores/actions)

_CVDRiskScores é uma aplicação multiplataforma para simular e calcular scores de risco cardiovascular, desenhada para profissionais de saúde, investigadores e pessoas interessadas em saúde. Permite estimar o risco individual de eventos cardiovasculares a 10 anos, utilizando algoritmos validados como o SCORE2 e Framingham, com dados clínicos essenciais (idade, género, pressão arterial, colesterol, tabagismo, etc.)._

Desenvolvida em .NET 9 MAUI, CVDRiskScores proporciona uma experiência moderna e interativa em dispositivos móveis e desktop, com suporte a resultados detalhados e localização.

---

## 🚀 Funcionalidades

- 🩺 **Calculadoras de Risco:**  
  - ⚡ **SCORE2** — Estimativa do risco cardiovascular fatal/não fatal a 10 anos, ajuste por faixa etária.
  - 🕰️ **Framingham** — Avaliação clássica do risco, com tabelas específicas por género.
- 👤 **Dados Personalizados:**  
  Idade, género, pressão arterial sistólica, colesterol total, HDL, estado tabágico, região de calibração.
- ✨ **Resultados Interativos:**  
  - 🪟 Popups explicativos com o detalhamento do risco (baixo, moderado, alto).
  - 📋 Resumo copiável para relatórios ou registos clínicos.
- 🌍 **Localização:**  
  - 🗣️ Suporte multilingue (Português, Inglês; extensível via ficheiros RESX).
- 📱🖥️ **UI Responsiva e Moderna:**  
  Popups, navegação avançada, visualização clara dos resultados.
- 💻 **Multiplataforma:**  
  Compatível com Android, iOS, Windows, Mac Catalyst e Tizen.

---

## 🧑‍💻 Tecnologia

- [.NET 9 MAUI](https://github.com/dotnet/maui)
- [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui)
- MVVM (Model-View-ViewModel)
- Ficheiros de recursos RESX para localização

---

## 📦 Como começar

### 👁️ Pré-requisitos

- [SDK .NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Dispositivo compatível: Android, iOS, Windows, Mac, Tizen
- [Visual Studio 2022/2023](https://visualstudio.microsoft.com/vs/) ou VS Code com suporte a .NET MAUI

### 🛠️ Instalação

```bash
git clone https://github.com/fauxtix/CVDRiskScores.git
cd CVDRiskScores
dotnet restore
dotnet build
```

Para correr na sua plataforma:
```bash
# 🖥️ Windows:
dotnet build -t:Run -f net9.0-windows10.0.19041.0

# 📱 Android/iOS/MacCatalyst/Tizen: consulte a documentação MAUI para targets específicos
```

---

## 📝 Utilização

**Passos para Simular Risco:**

1. 🏁 **Selecionar Calculadora:**  
   Escolha SCORE2 ou Framingham no dashboard inicial.
2. ✏️ **Introduzir Dados:**  
   Idade, género, pressão arterial, colesterol, tabagismo, etc.
3. 🔍 **Ver Resultados:**  
   Obtém o percentil de risco, categoria (baixo/médio/alto) e feedback visual.
4. 🧾 **Explorar Calibração:**  
   Consulte popups para exemplos e ajustes por região.
5. 📋 **Copiar ou Partilhar:**  
   Exporte ou copie os resultados facilmente para registos clínicos.

---

## 🌐 Localização

- Troque o idioma nas definições (Português/Inglês disponível; expansível para outros).
- Todo o texto da UI é gerido por ficheiros RESX.
- Aceitamos contribuições para novos idiomas!

---

## 🖼️ Imagens

<!-- Adicione capturas de ecrã aqui! -->
<!-- ![Ecrã Principal](screenshots/main.png) -->
<!-- ![Popup de Resultados](screenshots/result_popup.png) -->

---

## 📄 Licença

Licença MIT  
Veja [LICENSE.txt](LICENSE.txt) para detalhes.

---

## 🍎 Créditos & Referências

- Algoritmo SCORE2: [Sociedade Europeia de Cardiologia](https://escardio.org)
- Framingham Risk Score: [Framingham Heart Study](https://framinghamheartstudy.org)
- Desenvolvido com .NET 9 MAUI e CommunityToolkit

---

## 🤝 Contribuir

Pull requests, issues e sugestões são bem-vindos!  
Consulte [CONTRIBUTING.md](CONTRIBUTING.md) para instruções.

---

## 📬 Contacto

Para questões ou comentários, abra um issue ou contacte [@fauxtix](https://github.com/fauxtix).
