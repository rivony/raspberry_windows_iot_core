using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace CoursesListLIB
{
  public class ListeProduitsVM : VMBase
  {
    private ListeProduits _modele = null;
    private ObservableCollection<ProduitVM> _listeVMProduits = new ObservableCollection<ProduitVM>();
    private async void _modele_SurAjoutProduit(object sender, Produit produit)
    {
      _listeVMProduits.Add(new ProduitVM(produit));
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("ListeVMProduits");
      });
    }
    private async void _modele_SurCodeBarresAjoute(object sender, string codeBarres)
    {
      ProduitSelectionne = _listeVMProduits.First(p => p.CodeBarres == codeBarres);
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("ProduitSelectionne");
      });
    }
    private void RafraichirListeProduits()
    {
      foreach (ProduitVM prod in _listeVMProduits)
        prod.RafraichirVisibilite();
    }

    public ObservableCollection<ProduitVM> ListeVMProduits
    {
      get { return _listeVMProduits; }
    }
    public ProduitVM ProduitSelectionne { get; set; }
    public bool Filtree
    {
      get { return _modele.Filtree; }
      set
      {
        _modele.Filtree = value;
        RafraichirListeProduits();
      }
    }
    public ListeProduitsVM(ListeProduits modele)
    {
      _modele = modele;
      _modele.SurAjoutProduit += _modele_SurAjoutProduit; ;
      _modele.SurCodeBarresAjoute += _modele_SurCodeBarresAjoute; ;
    }
  }
}
