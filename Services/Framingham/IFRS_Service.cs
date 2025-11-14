using CVDRiskScores.Enums;

namespace CVDRiskScores.Services.Framingham
{
    public interface IFRS_Service
    {
        int GetAgePoints(int age, Genero gender);
        int GetHDLCholesterolPoints(Genero gender, int hdlCholesterol);
        int GetSmokingPoints(Genero gender, bool isSmoker, int age);
        int GetSystolicBloodPressurePoints(int systolicBloodPressure, bool isTreated, Genero gender);
        int GetTotalCholesterolPoints(int age, Genero gender, int totalCholesterol);
    }
}