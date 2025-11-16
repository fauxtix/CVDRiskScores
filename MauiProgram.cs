using CommunityToolkit.Maui;
using CVDRiskScores.MVVM.ViewModels.Framingham;
using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.MVVM.Views.Framingham;
using CVDRiskScores.MVVM.Views.Languages;
using CVDRiskScores.MVVM.Views.SCORE2;
using CVDRiskScores.Services.Framingham;
using CVDRiskScores.Services.Navigation;
using CVDRiskScores.Services.Popup;
using CVDRiskScores.Services.SCORE2;
using CVDRiskScores.Services.UI;
using CVDRiskScores.ViewModels.Languages;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CVDRiskScores
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            CultureInfo cultureInfo = new CultureInfo("pt-PT");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).UseMauiCommunityToolkit();
            ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Languages");
            var savedCulture = Preferences.Get("AppLanguage", null);
            if (string.IsNullOrEmpty(savedCulture))
            {
                savedCulture = CultureInfo.CurrentCulture.Name;
                Preferences.Set("AppLanguage", savedCulture);
            }
            var culture = new CultureInfo(savedCulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;


            builder.Services.AddTransient<FraminghamIntroPage>();
            builder.Services.AddTransient<FraminghamRiskScorePage>();
            builder.Services.AddTransient<FraminghamResultsPage>();

            builder.Services.AddTransient<FraminghamRiskScoreViewModel>();
            builder.Services.AddTransient<IFRS_Service, FRS_Service>();

            builder.Services.AddTransient<ISCORE2_Service, SCORE2_Service>();
            builder.Services.AddTransient<IScore2NavigationStore, Score2NavigationStore>();

            builder.Services.AddTransient<Score2ViewModel>();
            builder.Services.AddTransient<Score2ResultsViewModel>();

            builder.Services.AddTransient<Score2RiskScorePage>();
            builder.Services.AddTransient<Score2ResultsPage>();
            builder.Services.AddTransient<Score2IntroPage>();

            builder.Services.AddTransient<IUIPopupService, UIPopupService>();

            builder.Services.AddSingleton<LanguageSettingsViewModel>();
            builder.Services.AddTransient<LanguageSettingsPage>();

            return builder.Build();
        }
    }
}
