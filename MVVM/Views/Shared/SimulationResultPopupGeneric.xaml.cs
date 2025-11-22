using CommunityToolkit.Maui.Views;
using CVDRiskScores.Enums;
using CVDRiskScores.MVVM.Models.Shared;
using CVDRiskScores.Resources.Languages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace CVDRiskScores.MVVM.Views.Shared
{
    public partial class SimulationResultPopupGeneric : Popup
    {
        readonly object _data;
        readonly ObservableCollection<KeyValueRow> _rows = new();

        public SimulationResultPopupGeneric(object data, string title = "Resultado", string subtitle = "", string badge = "")
        {
            InitializeComponent();
            _data = data ?? throw new ArgumentNullException(nameof(data));

            TitleLabel.Text = title;
            SubtitleLabel.Text = subtitle;
            BadgeLabel.Text = string.IsNullOrWhiteSpace(badge) ? "-" : badge;

            BuildSmartRows();
            RenderRows();
        }

        public void SetData(object data, string title = "", string subtitle = "", string badge = "")
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _rows.Clear();
            TitleLabel.Text = title;
            SubtitleLabel.Text = subtitle;
            BadgeLabel.Text = string.IsNullOrWhiteSpace(badge) ? "-" : badge;
            BuildSmartRows(data);
            RenderRows();
        }

        void BuildSmartRows() => BuildSmartRows(_data);

        void BuildSmartRows(object obj)
        {
            _rows.Clear();
            if (obj == null)
            {
                SetAdvice(string.Empty);
                SetHeaderValues("-", "-", null);
                SetRiskEmoji(null);
                return;
            }

            var nested = FindNestedModel(obj, new[] { "FraminghamModel", "Score2Model", "Model", "ResultModel", "Score2", "ModelResult" });

            var rawScore = TryGetPropAsString(nested, "RiskScore") ?? TryGetPropAsString(obj, "RiskScore") ?? "-";
            var category = TryGetPropAsString(nested, "RiskCategory") ?? TryGetPropAsString(obj, "RiskCategory") ?? "-";
            var colorVal = GetPropValue(nested, "RiskColor") ?? GetPropValue(obj, "RiskColor");

            var advice = TryGetPropAsString(nested, "ClinicalAdvice") ?? TryGetPropAsString(obj, "ClinicalAdvice") ?? string.Empty;

            bool adviceDuplicatesCategory = false;
            if (!string.IsNullOrWhiteSpace(advice) && !string.IsNullOrWhiteSpace(category))
            {
                if (advice.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    category.IndexOf(advice, StringComparison.OrdinalIgnoreCase) >= 0)
                    adviceDuplicatesCategory = true;
            }

            var resolvedColor = ResolveColor(colorVal);
            SetHeaderValues(rawScore, category, resolvedColor);
            SetRiskEmoji(resolvedColor);

            SetAdvice(adviceDuplicatesCategory ? string.Empty : advice);

            if (nested != null && nested.GetType().Name.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BuildScore2Details(nested, obj);
                return;
            }
            if (nested != null && nested.GetType().Name.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BuildFraminghamDetails(nested, obj);
                return;
            }

            var tname = obj.GetType().Name;
            if (tname.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BuildScore2Details(obj, null);
                return;
            }
            if (tname.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BuildFraminghamDetails(obj, null);
                return;
            }

            AddIfPresentFromParent(obj, new[] { "ValidationError" });
            BuildRowsFrom(obj);
        }

        void BuildScore2Details(object model, object parentVm)
        {
            var lblAge = AppResources.TituloIdade;
            var lblGender = AppResources.TituloSexo;
            var lblNonHdl = AppResources.Titulo_Score2_ColesterolTotal;
            var lblTotalChol = AppResources.TituloColesterolTotal;
            var lblSbp = AppResources.Titulo_Score2_TA_Sistolica;
            var lblSmoker = AppResources.Simulacao_Fumador;
            var lblPoints = AppResources.TituloPontos;
            var lblValidation = AppResources.TituloErroValidacao;

            AddLabelValue(lblAge, GetFormattedProp(model, "Age"));
            AddLabelValue(lblGender, GetFormattedProp(model, "Gender"));

            AddLabelValue(lblTotalChol, GetFormattedProp(model, "TotalCholesterol"));

            var nonHdl = GetPropValue(model, "NonHDLCholesterol") ?? GetPropValue(model, "TotalCholesterol");
            AddLabelValue(lblNonHdl, FormatDisplayValue(nonHdl?.GetType() ?? typeof(object), nonHdl));

            AddLabelValue(lblSbp, GetFormattedProp(model, "SystolicBloodPressure"));
            AddLabelValue(lblSmoker, GetFormattedProp(model, "IsSmoker", boolAsYesNo: true));
            AddLabelValue($"{lblPoints} — {lblAge}", GetFormattedProp(model, "AgePoints"));
            AddLabelValue($"{lblPoints} — Non‑HDL", GetFormattedProp(model, "NonHDLPoints"));
            AddLabelValue($"{lblPoints} — TA", GetFormattedProp(model, "SBPPoints"));
            AddLabelValue($"{lblPoints} — Tabaco", GetFormattedProp(model, "SmokingPoints"));
            AddLabelValue(lblValidation, TryGetPropAsString(model, "ValidationError") ?? TryGetPropAsString(parentVm, "ValidationError") ?? "-");

            TryPopulateDiagnostics(model, parentVm);
        }

        void BuildFraminghamDetails(object model, object? parentVm)
        {
            var lblAge = AppResources.TituloIdade;
            var lblGender = AppResources.TituloSexo;
            var lblTotalChol = AppResources.TituloColesterolTotal;
            var lblHdl = AppResources.TituloColesterolHDL;
            var lblSbp = AppResources.TituloPressaoArterial;
            var lblSmoker = AppResources.Simulacao_Fumador;
            var lblTreated = AppResources.Simulacao_Medicacao_TA;
            var lblPoints = AppResources.TituloPontos;
            var lblValidation = AppResources.TituloErroValidacao;

            AddLabelValue(lblAge, GetFormattedProp(model, "Age"));
            AddLabelValue(lblGender, GetFormattedProp(model, "Gender"));
            AddLabelValue(lblTotalChol, GetFormattedProp(model, "TotalCholeterol"));
            AddLabelValue(lblHdl, GetFormattedProp(model, "HDLCholesterol"));
            AddLabelValue(lblSbp, GetFormattedProp(model, "SystolicBloodPressure"));
            AddLabelValue(lblTreated, GetFormattedProp(model, "BloodPressureTreated", boolAsYesNo: true)); // <-- added
            AddLabelValue(lblSmoker, GetFormattedProp(model, "Smoker", boolAsYesNo: true));
            AddLabelValue($"{lblPoints} — {lblAge}", GetFormattedProp(model, "AgePoints"));
            AddLabelValue($"{lblPoints} — {lblSmoker}", GetFormattedProp(model, "SmokerPoints"));
            AddLabelValue($"{lblPoints} — {lblTotalChol}", GetFormattedProp(model, "TotalCholesterolPoints"));
            AddLabelValue($"{lblPoints} — {lblHdl}", GetFormattedProp(model, "HDLCholesterolPoints"));
            AddLabelValue($"{lblPoints} — {lblSbp}", GetFormattedProp(model, "SystolicBloodPressurePoints"));
            AddLabelValue(lblValidation, TryGetPropAsString(model, "ValidationError") ?? TryGetPropAsString(parentVm, "ValidationError") ?? "-");

            TryPopulateDiagnostics(model, parentVm);
        }

        bool TryPopulateDiagnostics(object? model, object? parentVm)
        {
            try
            {
                var candidates = new List<object?> { model, parentVm };
                var det1 = GetPropValue(model, "Details") ?? GetPropValue(model, "Detail") ?? GetPropValue(model, "ScoreDetails");
                var det2 = GetPropValue(parentVm, "Details") ?? GetPropValue(parentVm, "Detail") ?? GetPropValue(parentVm, "ScoreDetails");
                if (det1 != null) candidates.Insert(0, det1);
                if (det2 != null) candidates.Insert(0, det2);

                string? lp = null, meanlp = null, s0 = null, risk = null;
                string? ageC = null, nonHdlC = null, sbpC = null, smokeC = null;

                foreach (var cand in candidates.Where(c => c != null))
                {
                    if (lp == null) lp = FindFirstMatchingPropAsString(cand!, new[] { "LP", "Lp", "LinearPredictor", "Logit" });
                    if (meanlp == null) meanlp = FindFirstMatchingPropAsString(cand!, new[] { "MeanLP", "MeanLp", "Mean" });
                    if (s0 == null) s0 = FindFirstMatchingPropAsString(cand!, new[] { "S0", "S_0", "S0Value" });
                    if (risk == null) risk = FindFirstMatchingPropAsString(cand!, new[] { "Risk", "RiskScore", "RiskValue" });

                    if (ageC == null) ageC = FindFirstMatchingPropAsString(cand!, new[] { "AgeContribution", "Age_C", "AgeContributionValue" });
                    if (nonHdlC == null) nonHdlC = FindFirstMatchingPropAsString(cand!, new[] { "NonHDLContribution", "NonHDL_C", "NonHDLContributionValue" });
                    if (sbpC == null) sbpC = FindFirstMatchingPropAsString(cand!, new[] { "SBPContribution", "SBP_C", "SBPContributionValue" });
                    if (smokeC == null) smokeC = FindFirstMatchingPropAsString(cand!, new[] { "SmokingContribution", "Smoking_C", "SmokingContributionValue", "Smoke_C" });

                    if (lp != null || meanlp != null || s0 != null || ageC != null || nonHdlC != null || sbpC != null || smokeC != null)
                        break;
                }

                var any = false;
                DiagnosticsLPLabel.Text = !string.IsNullOrWhiteSpace(lp) ? (AppResources.ResourceManager.GetString("Popup_Diagnostics_LP", AppResources.Culture) ?? "LP:") + " " + lp : string.Empty;
                DiagnosticsMeanLPLabel.Text = !string.IsNullOrWhiteSpace(meanlp) ? (AppResources.ResourceManager.GetString("Popup_Diagnostics_MeanLP", AppResources.Culture) ?? "Mean LP:") + " " + meanlp : string.Empty;
                DiagnosticsS0Label.Text = !string.IsNullOrWhiteSpace(s0) ? (AppResources.ResourceManager.GetString("Popup_Diagnostics_S0", AppResources.Culture) ?? "S0:") + " " + s0 : string.Empty;

                var contribs = new List<string>();
                if (!string.IsNullOrWhiteSpace(ageC)) contribs.Add((AppResources.ResourceManager.GetString("Popup_Diagnostics_Age", AppResources.Culture) ?? "Age") + ": " + ageC);
                if (!string.IsNullOrWhiteSpace(nonHdlC)) contribs.Add((AppResources.ResourceManager.GetString("Popup_Diagnostics_NonHDL", AppResources.Culture) ?? "Non‑HDL") + ": " + nonHdlC);
                if (!string.IsNullOrWhiteSpace(sbpC)) contribs.Add((AppResources.ResourceManager.GetString("Popup_Diagnostics_SBP", AppResources.Culture) ?? "SBP") + ": " + sbpC);
                if (!string.IsNullOrWhiteSpace(smokeC)) contribs.Add((AppResources.ResourceManager.GetString("Popup_Diagnostics_Smoke", AppResources.Culture) ?? "Smoke") + ": " + smokeC);

                DiagnosticsContribLabel.Text = contribs.Count > 0 ? string.Join("; ", contribs) : string.Empty;

                if (!string.IsNullOrWhiteSpace(risk))
                    DiagnosticsTitleLabel.Text = (AppResources.ResourceManager.GetString("Popup_Diagnostics_Title", AppResources.Culture) ?? "Diagnostics") + $" — {risk}";
                else
                    DiagnosticsTitleLabel.Text = AppResources.ResourceManager.GetString("Popup_Diagnostics_Title", AppResources.Culture) ?? "Diagnostics";

                any = !string.IsNullOrWhiteSpace(DiagnosticsLPLabel.Text) || !string.IsNullOrWhiteSpace(DiagnosticsMeanLPLabel.Text) || !string.IsNullOrWhiteSpace(DiagnosticsS0Label.Text) || !string.IsNullOrWhiteSpace(DiagnosticsContribLabel.Text);
                DiagnosticsPanel.IsVisible = any;
                return any;
            }
            catch { DiagnosticsPanel.IsVisible = false; return false; }
        }

        string? FindFirstMatchingPropAsString(object obj, string[] names)
        {
            if (obj == null) return null;
            foreach (var n in names)
            {
                var v = GetPropValue(obj, n);
                if (v != null) return v.ToString();
            }

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0);
            foreach (var p in props)
            {
                try
                {
                    if (names.Any(k => p.Name.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        var vv = p.GetValue(obj);
                        if (vv != null) return vv.ToString();
                    }
                }
                catch { }
            }
            return null;
        }

        void AddLabelValue(string label, object value)
        {
            if (_rows.Any(r => string.Equals(r.Key, label, StringComparison.OrdinalIgnoreCase)))
                return;

            var formatted = value switch
            {
                null => "-",
                string s => s,
                bool b => LocalizeBool(b),
                double d => d.ToString("F2", CultureInfo.CurrentCulture),
                float f => f.ToString("F2", CultureInfo.CurrentCulture),
                decimal m => m.ToString("F2", CultureInfo.CurrentCulture),
                int i => i.ToString(CultureInfo.CurrentCulture),
                _ => value.ToString() ?? "-"
            };

            var labelLower = (label ?? string.Empty).ToLowerInvariant();
            if ((labelLower.Contains("validation") || labelLower.Contains("erro de validação") || labelLower.Contains("erro")) &&
                string.IsNullOrWhiteSpace(formatted.Replace("-", "").Trim()))
                return;
            if ((labelLower.Contains("conselho") || labelLower.Contains("clinicaladvice") || labelLower.Contains("clinical advice")) &&
                string.IsNullOrWhiteSpace(formatted.Replace("-", "").Trim()))
                return;

            _rows.Add(new KeyValueRow { Key = label, Value = formatted });
        }

        string LocalizeBool(bool b) =>
            b ? (AppResources.Sim) : (AppResources.Nao);

        string GetFormattedProp(object obj, int propIndex, bool boolAsYesNo = false)
        {
            // Get the property using the indexer
            var prop = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ElementAt(propIndex);

            var v = GetPropValue(obj, prop.Name);
            if (v == null) return "-";
            if (v is bool bb) return boolAsYesNo ? LocalizeBool(bb) : bb.ToString();
            if (v is Enum e) return LocalizeEnum(e);
            if (v is double d) return d.ToString("F2", CultureInfo.CurrentCulture);
            if (v is float f) return f.ToString("F2", CultureInfo.CurrentCulture);
            if (v is decimal m) return m.ToString("F2", CultureInfo.CurrentCulture);
            if (v is int i) return i.ToString(CultureInfo.CurrentCulture);
            return v.ToString() ?? "-";
        }

        string GetFormattedProp(object obj, string propName, bool boolAsYesNo = false)
        {
            var v = GetPropValue(obj, propName);
            if (v == null) return "-";
            if (v is bool b) return boolAsYesNo ? LocalizeBool(b) : b.ToString();
            if (v is Enum e) return LocalizeEnum(e);
            if (v is double d) return d.ToString("F2", CultureInfo.CurrentCulture);
            if (v is float f) return f.ToString("F2", CultureInfo.CurrentCulture);
            if (v is decimal m) return m.ToString("F2", CultureInfo.CurrentCulture);
            if (v is int i) return i.ToString(CultureInfo.CurrentCulture);
            return v.ToString() ?? "-";
        }

        string LocalizeEnum(Enum e)
        {
            if (e is Genero g)
            {
                return g == Genero.Male ? AppResources.TituloMasculino
                                       : AppResources.TituloFeminino;
            }

            return e.ToString();
        }

        void SetAdvice(string advice)
        {
            if (string.IsNullOrWhiteSpace(advice) || advice == "-")
            {
                AdviceLabel.Text = string.Empty;
                AdviceLabel.IsVisible = false;
                return;
            }

            AdviceLabel.Text = advice;
            AdviceLabel.IsVisible = true;
        }

        void SetHeaderValues(string rawScore, string category, Color? color)
        {
            BadgeLabel.Text = FormatRiskScore(rawScore);

            var headerColor = color ?? GetResourceColor("Primary", Color.FromArgb("#512BD4"));
            HeaderBorder.BackgroundColor = headerColor;
            ActionsBorder.BackgroundColor = headerColor;

            TitleLabel.TextColor = GetContrastTextColor(headerColor);
            SubtitleLabel.TextColor = GetContrastTextColor(headerColor);
            AdviceLabel.TextColor = GetContrastTextColor(headerColor);

            CloseAndBackButton.TextColor = GetContrastTextColor(headerColor);
            ShareButton.TextColor = GetContrastTextColor(headerColor);
            CopyButton.TextColor = GetContrastTextColor(headerColor);
        }
        void SetRiskEmoji(Color? color)
        {
            var emoji = GetRiskEmoji(color);
            if (string.IsNullOrWhiteSpace(emoji) || emoji == "⚪")
            {
                RiskEmojiLabel.IsVisible = false;
                RiskEmojiLabel.Text = string.Empty;
                return;
            }
            RiskEmojiLabel.Text = emoji;
            RiskEmojiLabel.FontSize = 24;
            RiskEmojiLabel.IsVisible = true;

            var headerColor = HeaderBorder.BackgroundColor;
            RiskEmojiLabel.TextColor = GetContrastTextColor(headerColor);
        }

        Color GetResourceColor(string key, Color fallback)
        {
            if (Application.Current?.Resources?.TryGetValue(key, out var val) == true)
            {
                if (val is Color c) return c;
                if (val is Color mgc) return (Color)mgc;
            }
            return fallback;
        }

        Color GetContrastTextColor(Color bg)
        {
            double r = bg.Red, g = bg.Green, b = bg.Blue;
            double lum = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            return lum > 0.65 ? Colors.Black : Colors.White;
        }

        object? GetPropValue(object? obj, string propName)
        {
            if (obj == null) return null;
            var p = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) return null;
            try { return p.GetValue(obj); } catch { return null; }
        }

        string? TryGetPropAsString(object? obj, string propName)
        {
            var v = GetPropValue(obj, propName);
            return v?.ToString();
        }

        Color? ResolveColor(object? val)
        {
            if (val == null) return null;
            if (val is Color c) return c;
            if (val is Color mgc) return (Color)mgc;
            var s = val.ToString()?.Trim();
            if (string.IsNullOrEmpty(s)) return null;
            try
            {
                if (s.StartsWith("[COLOR:", StringComparison.OrdinalIgnoreCase) && s.EndsWith("]"))
                    s = s.Substring(7, s.Length - 8).Trim();
                if (!s.StartsWith("#")) s = "#" + s;
                return Color.FromArgb(s);
            }
            catch { return null; }
        }

        string FormatRiskScore(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "-";
            if (double.TryParse(raw, out var d))
            {
                if (d > 1) return d.ToString("F1", CultureInfo.CurrentCulture) + "%";
                return (d * 100).ToString("F1", CultureInfo.CurrentCulture) + "%";
            }
            return raw;
        }

        string GetRiskEmoji(Color? c)
        {
            if (c != null && c != Colors.Transparent)
            {
                try
                {
                    var r = c.Red;
                    var g = c.Green;
                    var b = c.Blue;

                    if (g > r && g > b && g > 0.35) return "🟢";
                    if (r > 0.5 && g > 0.25 && b < 0.35) return "🟠";
                    if (r > g && r > b) return "🔴";
                }
                catch { }
            }

            var cat = SubtitleLabel?.Text?.ToLowerInvariant() ?? string.Empty;
            if (cat.Contains("baixo") || cat.Contains("low")) return "🟢";
            if (cat.Contains("inter") || cat.Contains("médio") || cat.Contains("medio") || cat.Contains("interm")) return "🟠";
            if (cat.Contains("alto") || cat.Contains("very") || cat.Contains("muito")) return "🔴";

            return "⚪";
        }

        void AddIfPresentFromParent(object parent, IEnumerable<string> names)
        {
            if (parent == null) return;
            foreach (var name in names)
                AddIf(parent, name, PrettyName(name));
        }

        object FindNestedModel(object parent, string[] candidateNames)
        {
            if (parent == null) return null;

            foreach (var name in candidateNames)
            {
                var prop = parent.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    try
                    {
                        var val = prop.GetValue(parent);
                        if (val != null) return val;
                    }
                    catch { }
                }
            }

            var keywords = new[] { "Score2", "Framingham", "Model", "Result" };
            var props = parent.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0);
            foreach (var p in props)
            {
                try
                {
                    var val = p.GetValue(parent);
                    if (val == null) continue;
                    var typeName = val.GetType().Name;
                    if (keywords.Any(k => typeName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                        return val;
                }
                catch { }
            }

            var parentTypeName = parent.GetType().Name;
            if (keywords.Any(k => parentTypeName.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                return parent;

            return null;
        }

        void AddIf(object obj, string propName, string label)
        {
            if (obj == null) return;
            var p = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p == null) return;
            object? val = null;
            try { val = p.GetValue(obj); } catch { val = null; }

            if (string.Equals(propName, "RiskColor", StringComparison.OrdinalIgnoreCase))
            {
                var color = ResolveColor(val);
                var emoji = GetRiskEmoji(color);
                AddLabelValue("Cor de risco", emoji);
                return;
            }

            if (p.PropertyType == typeof(Color) || p.PropertyType == typeof(Microsoft.Maui.Graphics.Color))
            {
                Color c;
                if (val is Color col) c = col;
                else if (val is Color mgc) c = (Color)mgc;
                else c = Colors.Transparent;

                var hex = c.ToHex();
                AddLabelValue(label, $"[COLOR:{hex}]");
                return;
            }

            AddLabelValue(label, FormatDisplayValue(p.PropertyType, val));
        }

        void BuildRowsFrom(object obj)
        {
            if (obj == null) return;

            if (obj is IDictionary<string, object> dictObj)
            {
                foreach (var kv in dictObj)
                {
                    var key = kv.Key;
                    if (_rows.Any(r => r.Key == key)) continue;
                    _rows.Add(new KeyValueRow { Key = key, Value = FormatValue(kv.Value) });
                }
                return;
            }
            if (obj is IDictionary<string, string> dictStr)
            {
                foreach (var kv in dictStr)
                {
                    var key = kv.Key;
                    if (_rows.Any(r => r.Key == key)) continue;
                    _rows.Add(new KeyValueRow { Key = key, Value = kv.Value });
                }
                return;
            }
            if (obj is string s)
            {
                if (!_rows.Any(r => r.Key == "Mensagem"))
                    _rows.Add(new KeyValueRow { Key = "Mensagem", Value = s });
                return;
            }

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => IsSimpleType(p.PropertyType));

            foreach (var p in props)
            {
                var label = PrettyName(p.Name);
                if (_rows.Any(r => r.Key == label)) continue;
                object val = null;
                try { val = p.GetValue(obj); } catch { val = null; }

                if (p.PropertyType == typeof(Color) || p.PropertyType == typeof(Microsoft.Maui.Graphics.Color))
                {
                    Color c;
                    if (val is Color col) c = col;
                    else if (val is Microsoft.Maui.Graphics.Color mgc) c = (Color)mgc;
                    else c = Colors.Transparent;

                    var hex = c.ToHex();
                    _rows.Add(new KeyValueRow { Key = label, Value = $"[COLOR:{hex}]" });
                }
                else
                {
                    _rows.Add(new KeyValueRow { Key = label, Value = FormatDisplayValue(p.PropertyType, val) });
                }
            }
        }

        static bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(double) || t == typeof(float) || t == typeof(Guid) || t == typeof(bool) || (Nullable.GetUnderlyingType(t) != null && IsSimpleType(Nullable.GetUnderlyingType(t)));
        }

        static string PrettyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            var chars = new List<char> { name[0] };
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]) && !char.IsUpper(name[i - 1])) chars.Add(' ');
                chars.Add(name[i]);
            }
            return new string(chars.ToArray());
        }

        static string FormatValue(object? v)
        {
            if (v == null) return "-";
            return v switch
            {
                bool b => b ? "Sim" : "Não",
                double d => d.ToString("F2", CultureInfo.CurrentCulture),
                float f => f.ToString("F2", CultureInfo.CurrentCulture),
                decimal m => m.ToString("F2", CultureInfo.CurrentCulture),
                DateTime dt => dt.ToString("g", CultureInfo.CurrentCulture),
                IFormattable form => form.ToString(null, CultureInfo.CurrentCulture),
                _ => v.ToString() ?? "-"
            };
        }

        static string FormatDisplayValue(Type type, object? v)
        {
            if (v == null) return "-";

            if (type == typeof(Color) || type == typeof(Microsoft.Maui.Graphics.Color))
            {
                try
                {
                    if (v is Color c) return c.ToHex();
                    return v.ToString() ?? "-";
                }
                catch { return v.ToString() ?? "-"; }
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                if (v is bool b) return b ? "Sim" : "Não";
            }

            if (v is double d) return d.ToString("F2", CultureInfo.CurrentCulture);
            if (v is float f) return f.ToString("F2", CultureInfo.CurrentCulture);
            if (v is decimal m) return m.ToString("F2", CultureInfo.CurrentCulture);
            if (v is int i) return i.ToString(CultureInfo.CurrentCulture);

            if (v is IFormattable form) return form.ToString(null, CultureInfo.CurrentCulture);
            return v.ToString() ?? "-";
        }

        void RenderRows()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var details = this.FindByName<CollectionView>("DetailsCollection");
                if (details != null)
                {
                    details.ItemsSource = _rows;
                    try
                    {
                        if (_rows.Count > 0)
                            details.ScrollTo(_rows.First(), position: ScrollToPosition.Start, animate: false);
                    }
                    catch { }
                }
                else
                {
                    Debug.WriteLine("[Popup] DetailsCollection not found via FindByName.");
                }
            });
        }

        void OnCloseAndBackClicked(object sender, EventArgs e)
        {
            CloseAndBackAsync();
        }
        async void OnShareClicked(object sender, EventArgs e)
        {
            try
            {
                var lines = new List<string> { TitleLabel.Text, SubtitleLabel.Text, $"Resumo:", "" };
                lines.AddRange(_rows.Select(r => $"{r.Key}: {r.Value}"));
                var request = new ShareTextRequest
                {
                    Title = "Resultado da simulação",
                    Text = string.Join(Environment.NewLine, lines)
                };
                await Share.Default.RequestAsync(request);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Partilha", "A partilha falhou.", "OK");
            }
        }
        async void OnCopyClicked(object sender, EventArgs e)
        {
            try
            {
                var text = string.Join(Environment.NewLine, _rows.Select(r => $"{r.Key}: {r.Value}"));
                await Clipboard.Default.SetTextAsync(text);
                await Shell.Current.DisplayAlert("Copiado", "Resumo copiado para a área de transferência.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", "Não foi possível copiar.", "OK");
            }
        }
        async void CloseAndBackAsync()
        {
            try
            {
                await this.CloseAsync();
            }
            catch { }
            await Task.Delay(80);
            await NavigateBackToSimulationAsync();
        }

        async Task NavigateBackToSimulationAsync()
        {
            var route = DetermineSimulationRoute(_data);
            if (string.IsNullOrEmpty(route)) return;

            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        string DetermineSimulationRoute(object obj)
        {
            try
            {
                if (obj == null) return "..";

                var nested = FindNestedModel(obj, new[] { "Score2Model", "FraminghamModel", "Model", "ResultModel" });
                if (nested != null)
                {
                    var nname = nested.GetType().Name;
                    if (nname.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0) return "//Score2IntroPage";
                    if (nname.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0) return "//FraminghamIntroPage";
                }

                var tname = obj.GetType().Name;
                if (tname.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0) return "//Score2IntroPage";
                if (tname.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0) return "//FraminghamIntroPage";

                return "..";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}