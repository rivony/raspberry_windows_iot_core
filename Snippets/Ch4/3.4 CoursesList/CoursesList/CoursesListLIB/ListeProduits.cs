using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursesListLIB
{
  public delegate void ProduitModifie(object sender, Produit produit);
  public delegate void CodeBarresAjoute(object sender, string codeBarres);
  public class ListeProduits : Collection<Produit>
  {
    protected ListeProduits()
    {
      Filtree = true;
    }
    protected override void InsertItem(int index, Produit item)
    {
      base.InsertItem(index, item);
      SurAjoutProduit?.Invoke(this, item);
    }

    public bool Filtree { get; set; }
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
    public event ProduitModifie SurAjoutProduit;
    public event CodeBarresAjoute SurCodeBarresAjoute;

    public static ListeProduits instance = new ListeProduits();
  }
}
