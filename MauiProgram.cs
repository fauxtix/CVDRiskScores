using CommunityToolkit.Maui;
using CVDRiskScores.MVVM.ViewModels.Framingham;
using CVDRiskScores.MVVM.ViewModels.SCORE2;
using CVDRiskScores.MVVM.Views.Framingham;
using CVDRiskScores.MVVM.Views.SCORE2;
using CVDRiskScores.Services.Framingham;
using CVDRiskScores.Services.Navigation;
using CVDRiskScores.Services.Popup;
using CVDRiskScores.Services.SCORE2;
using CVDRiskScores.Services.UI;
using Microsoft.Extensions.Logging;

namespace CVDRiskScores
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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

            return builder.Build();
        }
    }
}
