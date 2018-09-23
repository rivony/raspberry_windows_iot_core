using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace EncodeursLIB
{
  public class EncodeurAbsoluVM : VMBase
  {
    private EncodeurAbsolu _modele = null;
    private async void _modele_SurChangementPosition(byte nouvellePosition)
    {
      Position = nouvellePosition;
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("Position");
      });
    }

    public byte Position { get; private set; }
    public EncodeurAbsoluVM(EncodeurAbsolu modele)
    {
      _modele = modele;
      _modele.SurChangementPosition += _modele_SurChangementPosition; ;
      _modele_SurChangementPosition(_modele.LirePosition());
    }    
  }
}
