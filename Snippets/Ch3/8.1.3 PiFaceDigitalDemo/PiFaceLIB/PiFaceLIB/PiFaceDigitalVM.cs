using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;

namespace PiFaceLIB
{
  public class PiFaceDigitalVM : VMBase
  {
    private PiFaceDigital _modele = null;
    private async void _modele_SurChangementEntrees(byte nouvellesvaleurs)
    {
      Entrees = nouvellesvaleurs;
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("Entrees");
        for (int i = 0; i < 8; i++)
          OnPropertyChanged("E" + i.ToString());
      });
    }
    private async void _modele_SurChangementSorties(byte nouvellesvaleurs)
    {
      Sorties = nouvellesvaleurs;
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        OnPropertyChanged("Sorties");
        for (int i = 0; i < 8; i++)
          OnPropertyChanged("S" + i.ToString());
      });
    }
    private bool GetBit(byte val, int noBit)
    {
      return (val & (1 << noBit)) != 0;
    }
    private void SetBitSortie(bool val, int noBit)
    {
      if (val)
        Sorties = (byte)(Sorties | (1 << noBit));
      else
        Sorties = (byte)(Sorties & ~(1 << noBit));
      _modele.EcrireSorties(Sorties);
    }

    public bool E0 { get { return GetBit(Entrees, 0); } }
    public bool E1 { get { return GetBit(Entrees, 1); } }
    public bool E2 { get { return GetBit(Entrees, 2); } }
    public bool E3 { get { return GetBit(Entrees, 3); } }
    public bool E4 { get { return GetBit(Entrees, 4); } }
    public bool E5 { get { return GetBit(Entrees, 5); } }
    public bool E6 { get { return GetBit(Entrees, 6); } }
    public bool E7 { get { return GetBit(Entrees, 7); } }
    public byte Entrees { get; private set; }

    public bool S0
    {
      get { return GetBit(Sorties, 0); }
      set { SetBitSortie(value, 0); }
    }
    public bool S1
    {
      get { return GetBit(Sorties, 1); }
      set { SetBitSortie(value, 1); }
    }
    public bool S2
    {
      get { return GetBit(Sorties, 2); }
      set { SetBitSortie(value, 2); }
    }
    public bool S3
    {
      get { return GetBit(Sorties, 3); }
      set { SetBitSortie(value, 3); }
    }
    public bool S4
    {
      get { return GetBit(Sorties, 4); }
      set { SetBitSortie(value, 4); }
    }
    public bool S5
    {
      get { return GetBit(Sorties, 5); }
      set { SetBitSortie(value, 5); }
    }
    public bool S6
    {
      get { return GetBit(Sorties, 6); }
      set { SetBitSortie(value, 6); }
    }
    public bool S7
    {
      get { return GetBit(Sorties, 7); }
      set { SetBitSortie(value, 7); }
    }
    public byte Sorties { get; private set; }
    public ICommand ToutEteindre { get; private set; }
    public ICommand ToutAllumer { get; private set; }
    public ICommand Inverser { get; private set; }

    public PiFaceDigitalVM(PiFaceDigital modele)
    {
      _modele = modele;
      _modele.SurChangementEntrees += _modele_SurChangementEntrees;
      _modele.SurChangementSorties += _modele_SurChangementSorties;
      ToutEteindre = new CommandBase((x) => _modele.EcrireSorties(0));
      ToutAllumer = new CommandBase((x) => _modele.EcrireSorties(0xFF));
      Inverser = new CommandBase((x) => _modele.EcrireSorties((byte)~Sorties));
    }   
  }
}
