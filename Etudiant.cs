namespace Calcul_valeur_P;

public struct Etudiant
{
    public int TourAdmission { get; set; }
    public string Population { get; set; }
    public string SanctionCollegiale { get; set; }
    public float GENMELS { get; set; }
    
    public bool RenforcementFrancais { get; set; }
    public EtudiantInternational StatusImmigration { get; set; }
    public bool R18 { get; set; }
    public bool ServiceAdaptes { get; set; }
    
    public Dictionary<string, bool> CoursSecondaires = new();
    
    public List<string> CoursInscritsActuels = new();
    
    public Dictionary<string, Note> CoursSessionPasse = new();
    
    public Etudiant(int tourAdmission, string population, string sanctionCollegiale, float genmels, bool renforcementFrancais, EtudiantInternational statusImmigration, bool r18, bool serviceAdaptes, Dictionary<string, bool> coursSecondaires, List<string> coursInscritsActuels, Dictionary<string, Note> coursSessionPasse)
    {
        TourAdmission = tourAdmission;
        Population = population;
        SanctionCollegiale = sanctionCollegiale;
        GENMELS = genmels;
        RenforcementFrancais = renforcementFrancais;
        StatusImmigration = statusImmigration;
        R18 = r18;
        ServiceAdaptes = serviceAdaptes;
        CoursSecondaires = coursSecondaires;
        CoursInscritsActuels = coursInscritsActuels;
        CoursSessionPasse = coursSessionPasse;
    }

}