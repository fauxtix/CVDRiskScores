using CommunityToolkit.Mvvm.ComponentModel;
using CVDRiskScores.Resources.Languages;
using System.Net;
using System.Text.RegularExpressions;

namespace CVDRiskScores.MVVM.ViewModels.SCORE2
{
    public partial class Score2LearnMoreViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;

        [ObservableProperty]
        string bodyText;

        public Score2LearnMoreViewModel()
        {
            Title = AppResources.ResourceManager.GetString("Validation_SCORE2_Disclaimer_Title", AppResources.Culture) ?? "Notice";
            BodyText = string.Empty;
        }

        public void Initialize(string htmlOrText, string dataVersion)
        {
            Title = AppResources.ResourceManager.GetString("Validation_SCORE2_Disclaimer_Title", AppResources.Culture) ?? "Notice";
            var t = SanitizeToPlainText(htmlOrText ?? string.Empty);
            if (!string.IsNullOrEmpty(dataVersion))
            {
                t = string.IsNullOrWhiteSpace(t) ? dataVersion : t + "\n\n" + dataVersion;
            }
            BodyText = string.IsNullOrWhiteSpace(t) ? (AppResources.ResourceManager.GetString("Calibration_Examples_Failed", AppResources.Culture) ?? "(sem conteúdo)") : t;
        }

        static string SanitizeToPlainText(string htmlOrText)
        {
            if (string.IsNullOrWhiteSpace(htmlOrText)) return string.Empty;

            var looksLikeHtml = htmlOrText.Contains("<") && htmlOrText.Contains(">");
            if (!looksLikeHtml)
                return WebUtility.HtmlDecode(htmlOrText);

            var withoutTags = Regex.Replace(htmlOrText, "<[^>]+>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);
            var normalized = Regex.Replace(decoded, @"\r?\n\s*", "\n");
            normalized = Regex.Replace(normalized, @"[ \t]{2,}", " ");
            return normalized.Trim();
        }
    }
}
