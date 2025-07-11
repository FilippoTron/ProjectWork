namespace ProjectWork.Services;

public class LoanCalculator
{
    public double CalcoloRata(double importo, double tassoAnnuale, int durataMesi)
    {
        if (durataMesi <= 0 || tassoAnnuale < 0 || importo < 0)
        {
            throw new ArgumentException("Parametri non validi");
        }
        double tassoMensile = (tassoAnnuale / 100) / 12;
        double numeratore = tassoMensile * Math.Pow(1 + tassoMensile, durataMesi);
        double denominatore = Math.Pow(1 + tassoMensile, durataMesi) - 1;
        double rata = importo * (numeratore / denominatore);
        return Math.Round(rata, 2);
    }
}
