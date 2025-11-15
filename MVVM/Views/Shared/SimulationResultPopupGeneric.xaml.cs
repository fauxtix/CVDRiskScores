using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace CVDRiskScores.MVVM.Views.Shared
{
    public partial class SimulationResultPopupGeneric : Popup
    {
        readonly object _data;
        readonly ObservableCollection<KeyValuePair<string, string>> _rows = new();

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

        // allow setting data later
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
                SetHeaderValues("", "", null);
                SetRiskEmoji(null);
                return;
            }

            // try find nested model (Score2 / Framingham etc)
            var nested = FindNestedModel(obj, new[] { "FraminghamModel", "Score2Model", "Model", "ResultModel", "Score2", "ModelResult" });

            // header pieces (prefer nested)
            var rawScore = TryGetPropAsString(nested, "RiskScore") ?? TryGetPropAsString(obj, "RiskScore");
            var category = TryGetPropAsString(nested, "RiskCategory") ?? TryGetPropAsString(obj, "RiskCategory");
            var colorVal = GetPropValue(nested, "RiskColor") ?? GetPropValue(obj, "RiskColor");

            // clinical advice
            var advice = TryGetPropAsString(nested, "ClinicalAdvice") ?? TryGetPropAsString(obj, "ClinicalAdvice");
            SetAdvice(advice);

            // resolve color and emoji for header
            var resolvedColor = ResolveColor(colorVal);
            SetHeaderValues(rawScore, category, resolvedColor);

            // set emoji in header (moves Cor de risco to header)
            SetRiskEmoji(resolvedColor);

            // Use specific builders but DO NOT add RiskScore or RiskCategory to details.
            if (nested != null && nested.GetType().Name.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                BuildScore2Details(nested, obj);
                // don't add RiskColor to details anymore
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

            // generic: include ValidationError only
            AddIfPresentFromParent(obj, new[] { "ValidationError" });
            BuildRowsFrom(obj);
        }

        void BuildScore2Details(object model, object parentVm)
        {
            AddLabelValue("Idade", GetFormattedProp(model, "Age"));
            AddLabelValue("Género", GetFormattedProp(model, "Gender"));
            var nonHdl = GetPropValue(model, "NonHDLCholesterol") ?? GetPropValue(model, "TotalCholesterol");
            AddLabelValue("Non‑HDL / Total Colesterol", FormatDisplayValue(nonHdl?.GetType() ?? typeof(object), nonHdl));
            AddLabelValue("TA sistólica (mmHg)", GetFormattedProp(model, "SystolicBloodPressure"));
            AddLabelValue("Fumador", GetFormattedProp(model, "IsSmoker", boolAsYesNo: true));

            AddLabelValue("Pontos — Idade", GetFormattedProp(model, "AgePoints"));
            AddLabelValue("Pontos — Non‑HDL", GetFormattedProp(model, "NonHDLPoints"));
            AddLabelValue("Pontos — TA", GetFormattedProp(model, "SBPPoints"));
            AddLabelValue("Pontos — Tabaco", GetFormattedProp(model, "SmokingPoints"));

            // Do NOT add "Pontuação de risco" here — it's shown in header badge only.
            // ValidationError still shown in details if present.
            AddLabelValue("Erro de validação", TryGetPropAsString(model, "ValidationError") ?? TryGetPropAsString(parentVm, "ValidationError"));
        }

        void BuildFraminghamDetails(object model, object parentVm)
        {
            AddLabelValue("Idade", GetFormattedProp(model, "Age"));
            AddLabelValue("Género", GetFormattedProp(model, "Gender"));
            AddLabelValue("Colesterol total", GetFormattedProp(model, "TotalCholeterol"));
            AddLabelValue("Colesterol HDL", GetFormattedProp(model, "HDLCholesterol"));
            AddLabelValue("TA sistólica (mmHg)", GetFormattedProp(model, "SystolicBloodPressure"));
            AddLabelValue("Fumador", GetFormattedProp(model, "Smoker", boolAsYesNo: true));

            AddLabelValue("Pontos — Idade", GetFormattedProp(model, "AgePoints"));
            AddLabelValue("Pontos — Fumador", GetFormattedProp(model, "SmokerPoints"));
            AddLabelValue("Pontos — Colesterol", GetFormattedProp(model, "TotalCholesterolPoints"));
            AddLabelValue("Pontos — HDL", GetFormattedProp(model, "HDLCholesterolPoints"));
            AddLabelValue("Pontos — TA", GetFormattedProp(model, "SystolicBloodPressurePoints"));

            // Do NOT add "Pontuação de risco" here — it's shown in header badge only.
            AddLabelValue("Erro de validação", TryGetPropAsString(model, "ValidationError") ?? TryGetPropAsString(parentVm, "ValidationError"));
        }

        // avoid duplicates and suppress empty validation/advice
        void AddLabelValue(string label, object value)
        {
            if (_rows.Any(r => string.Equals(r.Key, label, StringComparison.OrdinalIgnoreCase)))
                return;

            var formatted = value switch
            {
                null => "-",
                string s => s,
                bool b => b ? "Sim" : "Não",
                double d => d.ToString("F2", CultureInfo.CurrentCulture),
                float f => f.ToString("F2", CultureInfo.CurrentCulture),
                decimal m => m.ToString("F2", CultureInfo.CurrentCulture),
                int i => i.ToString(CultureInfo.CurrentCulture),
                _ => value.ToString() ?? "-"
            };

            var labelLower = (label ?? string.Empty).ToLowerInvariant();
            // suppress empty validation/advice rows
            if ((labelLower.Contains("validation") || labelLower.Contains("erro de validação") || labelLower.Contains("erro")) &&
                string.IsNullOrWhiteSpace(formatted.Replace("-", "").Trim()))
            {
                return;
            }
            if ((labelLower.Contains("conselho") || labelLower.Contains("clinicaladvice") || labelLower.Contains("clinical advice")) &&
                string.IsNullOrWhiteSpace(formatted.Replace("-", "").Trim()))
            {
                return;
            }

            _rows.Add(new KeyValuePair<string, string>(label, formatted));
        }

        // === Added back the GetFormattedProp helper the code expects ===
        string GetFormattedProp(object obj, string propName, bool boolAsYesNo = false)
        {
            var v = GetPropValue(obj, propName);
            if (v == null) return "-";
            if (v is bool b) return boolAsYesNo ? (b ? "Sim" : "Não") : b.ToString();
            if (v is double d) return d.ToString("F2", CultureInfo.CurrentCulture);
            if (v is float f) return f.ToString("F2", CultureInfo.CurrentCulture);
            if (v is decimal m) return m.ToString("F2", CultureInfo.CurrentCulture);
            if (v is int i) return i.ToString(CultureInfo.CurrentCulture);
            return v.ToString() ?? "-";
        }

        // set advice in header area (visible only when non-empty)
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

        // set badge (risk score) and category in header; apply category color when available
        void SetHeaderValues(string rawScore, string category, Color? color)
        {
            BadgeLabel.Text = FormatRiskScore(rawScore);

            if (!string.IsNullOrWhiteSpace(category) && category != "-")
            {
                SubtitleLabel.Text = category;
            }
            else
            {
                SubtitleLabel.Text = string.Empty;
            }

            var headerColor = color ?? GetResourceColor("Primary", Color.FromArgb("#512BD4"));
            HeaderBorder.BackgroundColor = headerColor;

            var textContrast = GetContrastTextColor(headerColor);
            TitleLabel.TextColor = textContrast;
            SubtitleLabel.TextColor = textContrast;
            AdviceLabel.TextColor = textContrast;

            CloseAndBackButton.BackgroundColor = headerColor;
            CloseAndBackButton.TextColor = GetContrastTextColor(headerColor);
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

        // helper: get resource color or fallback
        Color GetResourceColor(string key, Color fallback)
        {
            if (Application.Current?.Resources?.TryGetValue(key, out var val) == true)
            {
                if (val is Color c) return c;
                if (val is Microsoft.Maui.Graphics.Color mgc) return (Color)mgc;
            }
            return fallback;
        }

        // compute readable contrast color (white or black) from background color
        Color GetContrastTextColor(Color bg)
        {
            // relative luminance
            double r = bg.Red, g = bg.Green, b = bg.Blue;
            // use standard luminance formula
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
            if (val is Microsoft.Maui.Graphics.Color mgc) return (Color)mgc;
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

        // map color (or absent) to traffic-light emoji
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
                catch { /* fallback */ }
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
            object val = null;
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
                else if (val is Microsoft.Maui.Graphics.Color mgc) c = (Color)mgc;
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
                    _rows.Add(new KeyValuePair<string, string>(key, FormatValue(kv.Value)));
                }
                return;
            }
            if (obj is IDictionary<string, string> dictStr)
            {
                foreach (var kv in dictStr)
                {
                    var key = kv.Key;
                    if (_rows.Any(r => r.Key == key)) continue;
                    _rows.Add(new KeyValuePair<string, string>(key, kv.Value));
                }
                return;
            }
            if (obj is string s)
            {
                if (!_rows.Any(r => r.Key == "Mensagem"))
                    _rows.Add(new KeyValuePair<string, string>("Mensagem", s));
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
                    _rows.Add(new KeyValuePair<string, string>(label, $"[COLOR:{hex}]"));
                }
                else
                {
                    _rows.Add(new KeyValuePair<string, string>(label, FormatDisplayValue(p.PropertyType, val)));
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
                Debug.WriteLine($"[Popup] RenderRows: _rows.Count = {_rows.Count}");
                var details = this.FindByName<CollectionView>("DetailsCollection");
                if (details != null)
                {
                    details.ItemsSource = _rows;
                    try
                    {
                        if (_rows.Count > 0)
                            details.ScrollTo(_rows.First(), position: ScrollToPosition.Start, animate: false);
                    }
                    catch { /* ignore */ }
                }
                else
                {
                    Debug.WriteLine("[Popup] DetailsCollection not found via FindByName.");
                }
            });
        }

        // Attempt to close popup by reflection; returns true if closed
        bool TryClosePopup()
        {
            var candidateNames = new[] { "Close", "Dismiss", "Hide", "Pop" };
            foreach (var name in candidateNames)
            {
                try
                {
                    var mi = this.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, Type.EmptyTypes, null);
                    if (mi != null)
                    {
                        mi.Invoke(this, null);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TryClosePopup: reflection invoke '{name}' failed: {ex.Message}");
                }
            }

            // try the base type methods as fallback
            try
            {
                var baseType = typeof(CommunityToolkit.Maui.Views.Popup);
                foreach (var name in candidateNames)
                {
                    try
                    {
                        var baseMi = baseType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (baseMi != null)
                        {
                            baseMi.Invoke(this, null);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"TryClosePopup: base reflection invoke '{name}' failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TryClosePopup: base type reflection failed: {ex.Message}");
            }

            return false;
        }

        // Close handler: try reflection, otherwise hide/remove visual, then navigate back
        async void OnCloseAndBackClicked(object sender, EventArgs e)
        {
            bool closed = false;
            try
            {
                closed = TryClosePopup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnCloseAndBackClicked: TryClosePopup threw: {ex.Message}");
            }

            // small delay to let toolkit finish internal cleanup
            await Task.Delay(80);

            // Defensive: pop any leftover modal pages that could be blocking input
            try
            {
                var nav = Shell.Current?.Navigation;
                if (nav != null)
                {
                    // Pop modals without animation to be fast and safe
                    while (nav.ModalStack.Count > 0)
                    {
                        try { await nav.PopModalAsync(false); } catch { break; }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnCloseAndBackClicked: PopModalAsync cleanup failed: {ex.Message}");
            }

            // Ensure main page is enabled and receives input
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var mp = Application.Current?.MainPage;
                    if (mp != null)
                    {
                        mp.InputTransparent = false;
                        mp.IsEnabled = true;
                    }

                    // also ensure current shell page is interactive
                    var cp = Shell.Current?.CurrentPage;
                    if (cp != null)
                    {
                        cp.InputTransparent = false;
                        cp.IsEnabled = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OnCloseAndBackClicked: re-enable UI failed: {ex.Message}");
            }

            // finally navigate back
            await NavigateBackToSimulationAsync();
        }

        async Task NavigateBackToSimulationAsync()
        {
            var route = DetermineSimulationRoute(_data);
            Debug.WriteLine($"[Popup] NavigateBackToSimulationAsync route = '{route}'");
            if (string.IsNullOrEmpty(route)) return;

            try
            {
                if (route == "..")
                    await Shell.Current.GoToAsync(route);
                else
                    await Shell.Current.GoToAsync(route, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        string DetermineSimulationRoute(object obj)
        {
            if (obj == null) return "..";

            var nested = FindNestedModel(obj, new[] { "Score2Model", "FraminghamModel", "Model", "ResultModel" });
            if (nested != null)
            {
                var nname = nested.GetType().Name;
                if (nname.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0) return "//Score2RiskScorePage";
                if (nname.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0) return "//FraminghamRiskScorePage";
            }

            var tname = obj.GetType().Name;
            if (tname.IndexOf("Score2", StringComparison.OrdinalIgnoreCase) >= 0) return "//Score2RiskScorePage";
            if (tname.IndexOf("Framingham", StringComparison.OrdinalIgnoreCase) >= 0) return "//FraminghamRiskScorePage";

            return "..";
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
                Debug.WriteLine($"Share failed: {ex.Message}");
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
                Debug.WriteLine($"Copy failed: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Não foi possível copiar.", "OK");
            }
        }
    }
}