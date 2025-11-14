using CVDRiskScores.Enums;

namespace CVDRiskScores.Models.SCORE2;

public class Score2Model
{
    public int Age { get; set; }
    public Genero Gender { get; set; }
    public int SystolicBloodPressure { get; set; }
    public int TotalCholesterol { get; set; }
    public int HDLCholesterol { get; set; }
    public bool IsSmoker { get; set; }
    public int NonHDLCholesterol => TotalCholesterol - HDLCholesterol;

    public double AgePoints { get; set; }
    public double NonHDLPoints { get; set; }
    public double SBPPoints { get; set; }
    public double SmokingPoints { get; set; }
    public double RiskScore { get; set; }
    public string RiskCategory { get; set; }
    public Color RiskColor { get; set; }
    public string ClinicalAdvice { get; set; }
    public string ValidationError { get; set; }
}
