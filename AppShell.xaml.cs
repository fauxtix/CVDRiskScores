using CVDRiskScores.MVVM.Views.Framingham;
using CVDRiskScores.MVVM.Views.Languages;
using CVDRiskScores.MVVM.Views.SCORE2;

namespace CVDRiskScores
{
    public partial class AppShell : Shell
    {
        public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        void RegisterRoutes()
        {
            Routes.Add(nameof(FraminghamIntroPage), typeof(FraminghamIntroPage));
            Routes.Add(nameof(FraminghamRiskScorePage), typeof(FraminghamRiskScorePage));
            Routes.Add(nameof(FraminghamResultsPage), typeof(FraminghamResultsPage));

            Routes.Add(nameof(Score2IntroPage), typeof(Score2IntroPage));
            Routes.Add(nameof(Score2RiskScorePage), typeof(Score2RiskScorePage));
            Routes.Add(nameof(Score2ResultsPage), typeof(Score2ResultsPage));

            Routing.RegisterRoute(nameof(LanguageSettingsPage), typeof(LanguageSettingsPage));

            foreach (var item in Routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }

    }
}
