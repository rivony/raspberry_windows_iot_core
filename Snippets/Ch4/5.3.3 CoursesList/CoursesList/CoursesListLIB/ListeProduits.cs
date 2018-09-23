using SerialLIB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoursesListLIB
{
  public delegate void ProduitModifie(object sender, Produit produit);
  public delegate void CodeBarresAjoute(object sender, string codeBarres);
  public class ListeProduits : Collection<Produit>
  {
    private const char CSV_SEPARATOR = ';';
    private bool _modifiee = false;
    private object _modifieeLock = new object();
    private Timer _EnregistrementTimer = null;
    private static readonly string NOM_FICHIER_APPLICATION = "CoursesList";
    private static StorageFolder _dossierApplication = null;
    private async static Task<StorageFolder> GetDossierApplication()
    {
      if (_dossierApplication == null)
        _dossierApplication = await ApplicationData.Current.LocalFolder.CreateFolderAsync(NOM_FICHIER_APPLICATION, CreationCollisionOption.OpenIfExists);
      return _dossierApplication;
    }
    private void EnregistrementTick(object state)
    {
      List<string> lignes = null;
      lock (_modifieeLock)
      {
        if (_modifiee)
        {
          lignes = GetLignes();
          _modifiee = false;
        }
      }
      if (lignes != null)
        Enregistrer(lignes);
    }
    private List<string> GetLignes()
    {
      List<string> res = new List<string>();
      res.Add("Code-barres" + CSV_SEPARATOR + "Nombre" + CSV_SEPARATOR + "Nom");
      foreach (Produit produit in Items)
        res.Add(produit.GetLigneCSV(CSV_SEPARATOR));
      return res;
    }
    private async void Enregistrer(List<string> lignes)
    {
      StorageFolder dossierApp = await GetDossierApplication();
      string nomFichier = NOM_FICHIER_APPLICATION + ".csv";
      IStorageItem elementSurDisque = await dossierApp.TryGetItemAsync(nomFichier);
      StorageFile fichier;
      if (elementSurDisque == null)
        fichier = await dossierApp.CreateFileAsync(nomFichier);
      else
        fichier = (StorageFile)elementSurDisque;
      await FileIO.WriteLinesAsync(fichier, lignes);
      Debug.WriteLine("Enregistré !");
    }
    private async Task<IList<string>> ChargerLignes()
    {
      StorageFolder dossierApp = await GetDossierApplication();
      string nomFichier = NOM_FICHIER_APPLICATION + ".csv";
      IStorageItem elementSurDisque = await dossierApp.TryGetItemAsync(nomFichier);
      if (elementSurDisque != null)
        return await FileIO.ReadLinesAsync((StorageFile)elementSurDisque);
      else
        return null;
    }

    protected ListeProduits()
    {
      _EnregistrementTimer = new Timer(EnregistrementTick, null, new TimeSpan(0, 0, 30), new TimeSpan(0, 0, 10));
      Filtree = true;
    }
    protected override void InsertItem(int index, Produit item)
    {
      base.InsertItem(index, item);
      SurAjoutProduit?.Invoke(this, item);
    }

    public bool Filtree { get; set; }
    public bool Modifiee
    {
      get { lock (_modifieeLock) { return _modifiee; } }
      set { lock (_modifieeLock) { _modifiee = value; } }
    }
    public void AjouterCodeBarres(string codeBarres)
    {
      Produit memeProduit = Items.FirstOrDefault(p => p.CodeBarres == codeBarres);
      if (memeProduit == null)
      {
        memeProduit = Produit.CreerDepuisCodeBarres(codeBarres);
        Add(memeProduit);
      }
      memeProduit.Nb++;
      SurCodeBarresAjoute?.Invoke(this, codeBarres);
    }
    public void Deplacer(int ancienIndex, int nouvelIndex)
    {
      lock (_modifieeLock)
      {
        Produit aDeplacer = Items[ancienIndex];
        RemoveAt(ancienIndex);
        base.InsertItem(nouvelIndex, aDeplacer);
        _modifiee = true;
      }
    }
    public void Vider()
    {
      foreach (Produit prod in Items)
        prod.Nb = 0;
    }
    public async Task Charger()
    {
      IList<string> lignes = await ChargerLignes();
      if (lignes != null)
      {
        if (lignes.Count > 0)
          lignes.RemoveAt(0);
        foreach (string ligne in lignes)
          Add(Produit.CreerDepuisLigneCsv(ligne, CSV_SEPARATOR));
      }
    }
    public async void ImprimerListe(ImprimanteSerie imprimante)
    {
      await imprimante.ImprimerCommande(ImprimanteSerie.INIT());
      foreach (Produit produit in Items)
        if (produit.Nb > 0)
        {      
          await imprimante.ImprimerCommande(ImprimanteSerie.PRINTING_MODE(true, false, false, true, true, true, false, false));
          await imprimante.ImprimerTexte("x" + produit.Nb.ToString());
          await imprimante.ImprimerCommande(ImprimanteSerie.PRINTING_MODE(false, false, false, false, false, false, false, false));
          await imprimante.ImprimerCommande(ImprimanteSerie.LEFT_MARGIN(7));
          await imprimante.ImprimerTexte(produit.Nom);
          await imprimante.ImprimerCommande(ImprimanteSerie.PRINT_AND_FEED(2));
          await imprimante.ImprimerCommande(ImprimanteSerie.LEFT_MARGIN(0));
        }
      await imprimante.ImprimerCommande(ImprimanteSerie.PRINT_AND_FEED(5));
    }
    public event ProduitModifie SurAjoutProduit;
    public event CodeBarresAjoute SurCodeBarresAjoute;

    public static ListeProduits instance = new ListeProduits();
  }
}
