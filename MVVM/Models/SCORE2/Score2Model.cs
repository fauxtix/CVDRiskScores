using CVDRiskScores.Enums;

namespace CVDRiskScores.Models.SCORE2;

public class Score2Model
{
    // make numeric fields nullable so Entry placeholder shows until user types
    public int? Age { get; set; }
    public Genero Gender { get; set; }
    public int? SystolicBloodPressure { get; set; }

    // CHANGED: use double? for mmol/L support
    public double? TotalCholesterol { get; set; }
    public double? HDLCholesterol { get; set; }
    public bool IsSmoker { get; set; }

    public double? NonHDLCholesterol => (TotalCholesterol.HasValue && HDLCholesterol.HasValue)
        ? TotalCholesterol.Value - HDLCholesterol.Value
        : (double?)null;

    public int AgePoints { get; set; }
    public int NonHDLPoints { get; set; }
    public int SBPPoints { get; set; }
    public int SmokingPoints { get; set; }
    public double RiskScore { get; set; }

    public string RiskCategory { get; set; } = string.Empty;
    public Color RiskColor { get; set; } = Colors.DarkGreen;
    public string ClinicalAdvice { get; set; } = string.Empty;
    public string? ValidationError { get; set; }

    // Calibration key (Low/Moderate/High) - default to Moderate
    public string CalibrationKey { get; set; } = "Low";

    // Optional: detailed calculation output (LP, MeanLP, S0, contributions, etc.)
    // Stored as object so popup can inspect properties dynamically.
    public object? ScoreDetails { get; set; }

    public bool ForceRealCalculation { get; set; } = false;
}