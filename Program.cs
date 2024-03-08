using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;

namespace Calcul_valeur_P
{
    class Program
    {
        static void Main2(string[] args)
        {
            Random random = new Random();

            List<(bool, string)> groupeGeneral = new List<(bool, string)>();
            string[] notes = { "E", "D", "C", "B", "A" };

            // Génération de données aléatoires
            for (int i = 0; i < 50; i++)
            {
                bool key = random.NextDouble() > 0.5;
                string value;

                
                // Générer une note aléatoire entre E et A
                value = notes[random.Next(0, 4)];
                

                groupeGeneral.Add((key, value));
            }

            double moyenneGenerale = 0;
            
            
            // Transformation des notes en valeurs numériques
            // E = 30, D = 65, C = 75, B = 85, A = 95
            
            List<(bool, float)> groupeGeneralNumerique = new List<(bool, float)>();
            foreach (var item in groupeGeneral)
            {
                float value = 0;
                switch (item.Item2)
                {
                    case "E":
                        value = 50;
                        moyenneGenerale += 50;
                        break;
                    case "D":
                        value = 65;
                        moyenneGenerale += 65;
                        break;
                    case "C":
                        value = 75;
                        moyenneGenerale += 75;
                        break;
                    case "B":
                        value = 85;
                        moyenneGenerale += 85;
                        break;
                    case "A":
                        value = 95;
                        moyenneGenerale += 95;
                        break;
                }
                groupeGeneralNumerique.Add((item.Item1, value));
            }
            
            moyenneGenerale /= groupeGeneral.Count;

            
            // Séparation des données en deux groupes
            List<float> groupeReussi = new List<float>();
            List<float> groupeRate = new List<float>();

            foreach (var item in groupeGeneralNumerique)
            {
                if (item.Item1)
                {
                    groupeReussi.Add(item.Item2);
                }
                else
                {
                    groupeRate.Add(item.Item2);
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
            diviseur = 1f/groupeReussi.Count + 1f/groupeRate.Count;
            diviseur = Math.Sqrt(diviseur);
            
            double Zobs = dividande / (Sp * diviseur);
            
            
            
            
            double degreeOfFreedom = groupeReussi.Count + groupeRate.Count - 2;
            
            var studentT = new StudentT(0, 1, degreeOfFreedom);
            double pValue = 2 * (1 - studentT.CumulativeDistribution(Math.Abs(Zobs)));

            // Affichage de la valeur p
            Console.WriteLine($"La valeur p est : {pValue}");
            Console.ReadKey();
        }
    }
}
