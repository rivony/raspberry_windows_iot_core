using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace CoursesListLIB
{
  public class ProduitVM : VMBase
  {
    private Produit _modele = null;
    private BitmapImage _imageMiniature = null;
    private async void _modele_SurModificationProduit(object sender, ProprieteProduitModifiee propriete)
    {
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        switch (propriete)
        {
          case ProprieteProduitModifiee.propNom:
            OnPropertyChanged("Nom");
            break;
          case ProprieteProduitModifiee.propNb:
            OnPropertyChanged("Nb");
            break;
          case ProprieteProduitModifiee.propImg:
            if ((_imageMiniature == null) && (_modele.UrlImage != null))
            {
              if (_modele.UrlImage != "")
                _imageMiniature = new BitmapImage(new Uri(_modele.UrlImage));
              else
                _imageMiniature = new BitmapImage();
              OnPropertyChanged("ImageMiniature");
            }
            break;
          default:
            break;
        }
      });
    }

    public string CodeBarres { get { return _modele.CodeBarres; } }
    public string Nom
    {
      get { return _modele.Nom; }
      set { _modele.Nom = value; }
    }
    public uint Nb { get { return _modele.Nb; } }
    public BitmapImage ImageMiniature { get { return _imageMiniature; } }
    public ProduitVM(Produit modele)
    {
      _modele = modele;
      _modele.SurModificationProduit += _modele_SurModificationProduit; ;
    }    
  }
}
