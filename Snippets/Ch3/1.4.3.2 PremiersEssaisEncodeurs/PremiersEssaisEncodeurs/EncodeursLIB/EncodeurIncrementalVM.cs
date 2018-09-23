using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace EncodeursLIB
{
  public class EncodeurIncrementalVM : VMBase
  {
    private EncodeurIncrementalBase _modele = null;
    private int _position;
    private async void MiseAJourPropriete(string NomPropriete)
    {
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged(NomPropriete);
      });
    }
    private void _modele_SurRotationEncodeur(bool sensHoraire)
    {
      if (sensHoraire)
        Position++;
      else
        Position--;
    }

    public int Position
    {
      get { return _position; }
      set
      {
        if (_position != value)
        {
          _position = value;
          MiseAJourPropriete("Position");
        }
      }
    }
    public EncodeurIncrementalVM(EncodeurIncrementalBase modele)
    {
      Position = 0;
      _modele = modele;
      _modele.SurRotationEncodeur += _modele_SurRotationEncodeur; ;
    }    
  }
}
