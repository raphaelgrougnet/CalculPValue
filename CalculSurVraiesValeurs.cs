using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.VisualBasic.FileIO;

namespace Calcul_valeur_P;

public class CalculSurVraiesValeurs
{
    static void Main(string[] args)
    {
        const string coursAComparer = "42004AFX";
        const string critere = "SN4";
        
        List<string[]> allLines = new List<string[]>();
        TextFieldParser parser = new TextFieldParser(@"C:\Users\rapha\Desktop\Calcul valeur P\Calcul valeur P\PlusDeDonnees.csv");
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");

        while (!parser.EndOfData)
        {
            string[] fields = parser.ReadFields();
            if (fields != null)
            {
                allLines.Add(fields);
            }
        }
        
        allLines.RemoveAt(0);
        
        List<Etudiant> etudiants = new List<Etudiant>();
        
        foreach (var ligne in allLines)
        {
            if (ligne[0].Length == 0)
            {
                continue;
            }
            int TourAdmission = int.Parse(ligne[0]);
            string Population = ligne[1];
            string SanctionCollegiale = ligne[2];
            float GENMELS = float.Parse(ligne[3]);
            bool RenforcementFrancais = ligne[16].ToLower().Trim() == "oui";
            EtudiantInternational StatusImmigration = Enum.Parse<EtudiantInternational>(ligne[17]);
            bool R18 = ligne[18].ToLower().Trim() == "oui";
            bool ServiceAdaptes = ligne[19].ToLower().Trim() == "oui";
            Dictionary<string, bool> CoursSecondaires = new Dictionary<string, bool>();
            List<string> CoursInscritsActuels = new List<string>();
            Dictionary<string, Note> CoursSessionPasse = new Dictionary<string, Note>();
            
            CoursSecondaires.Add("SN4", ligne[4].ToLower().Trim() == "oui");
            CoursSecondaires.Add("TS_SN4", ligne[5].ToLower().Trim() == "oui");
            CoursSecondaires.Add("TS4_SN4+", ligne[6].ToLower().Trim() == "oui");
            CoursSecondaires.Add("CST5", ligne[7].ToLower().Trim() == "oui");
            CoursSecondaires.Add("TS_SN5", ligne[8].ToLower().Trim() == "oui");
            CoursSecondaires.Add("TS5", ligne[9].ToLower().Trim() == "oui");
            CoursSecondaires.Add("436", ligne[10].ToLower().Trim() == "oui");
            CoursSecondaires.Add("514", ligne[11].ToLower().Trim() == "oui");
            CoursSecondaires.Add("514+", ligne[12].ToLower().Trim() == "oui");
            CoursSecondaires.Add("526", ligne[13].ToLower().Trim() == "oui");
            CoursSecondaires.Add("526+", ligne[14].ToLower().Trim() == "oui");
            CoursSecondaires.Add("536", ligne[15].ToLower().Trim() == "oui");
            
            string[] CoursInscritsActuelsString = ligne[20].Split(';');
            foreach (var cours in CoursInscritsActuelsString)
            {
                CoursInscritsActuels.Add(cours);
            }
            
            string[] CoursSessionPasseString = ligne[21].Split(';');
            if (CoursSessionPasseString.Length >= 2)
            {


                for (int i = 0; i < CoursSessionPasseString.Length; i+=2)
                {
                    Note result;
                    bool parseNote = Enum.TryParse<Note>(CoursSessionPasseString[i + 1], out result);
                    if (parseNote)
                    {
                        CoursSessionPasse.Add(CoursSessionPasseString[i].Trim(), result);
                    }



                }
            }

            Etudiant etudiant = new Etudiant(TourAdmission, Population, SanctionCollegiale, GENMELS, RenforcementFrancais, StatusImmigration, R18, ServiceAdaptes, CoursSecondaires, CoursInscritsActuels, CoursSessionPasse);
            etudiants.Add(etudiant);
        }
        
        List<float> groupeReussi = new List<float>();
        List<float> groupeRate = new List<float>();
        
        foreach (var etudiant in etudiants)
        {
            if (!etudiant.CoursSessionPasse.Keys.Contains(coursAComparer))
            {
                continue;
            }
            if (etudiant.CoursSessionPasse.Count == 0)
            {
                continue;
            }
            foreach (var cours in etudiant.CoursSessionPasse)
            {
                if (cours.Key != coursAComparer)
                {
                    continue;
                }
                switch (cours.Value)
                {
                    case Note.A:
                        if (etudiant.CoursSecondaires[critere])
                            groupeReussi.Add(95);
                        else
                            groupeRate.Add(95);
                        break;
                    case Note.B:
                        if (etudiant.CoursSecondaires[critere])
                            groupeReussi.Add(85);
                        else
                            groupeRate.Add(85);
                        break;
                    case Note.C:
                        if (etudiant.CoursSecondaires[critere])
                            groupeReussi.Add(75);
                        else
                            groupeRate.Add(75);
                        break;
                    case Note.D:
                        if (etudiant.CoursSecondaires[critere])
                            groupeReussi.Add(65);
                        else
                            groupeRate.Add(65);
                        break;
                    case Note.E:
                        if (etudiant.CoursSecondaires[critere])
                            groupeReussi.Add(50);
                        else
                            groupeRate.Add(50);
                        break;
                }
                
            }
            
        }
        
        // Calcul du test t de Student
        double mean1 = Statistics.Mean(groupeReussi);
        double stdDev1 = Statistics.StandardDeviation(groupeReussi);
        double mean2 = Statistics.Mean(groupeRate);
        double stdDev2 = Statistics.StandardDeviation(groupeRate);

        double variance1 = Math.Pow(stdDev1, 2);
        double variance2 = Math.Pow(stdDev2, 2);
            
            
            
        double dividande = variance1*(groupeReussi.Count-1) + variance2*(groupeRate.Count-1);
        double diviseur = groupeReussi.Count + groupeRate.Count - 2;
            
        
        double Sp = Math.Sqrt(dividande/diviseur);
        
        dividande = mean1 - mean2;
        diviseur = Math.Sqrt(1f/groupeReussi.Count + 1f/groupeRate.Count);
            
        double Zobs = dividande / (Sp * diviseur);
        
            
        double degreeOfFreedom = groupeReussi.Count + groupeRate.Count - 2;
            
        var studentT = new StudentT(0, 1, degreeOfFreedom);
        double pValue = 2 * (1 - studentT.CumulativeDistribution(Math.Abs(Zobs)));

        // Affichage de la valeur p
        Console.WriteLine("Etudiants total ayant assité au cours : " + coursAComparer + " : " + (groupeReussi.Count + groupeRate.Count));
        Console.WriteLine("Etudiants ayant réussis le critère " + critere + " : " + groupeReussi.Count);
        Console.WriteLine("Etudiants ayant échoués le critère " + critere + " : " + groupeRate.Count);
        Console.WriteLine($"La valeur p est : {pValue}");
        Console.ReadKey();
    }
}