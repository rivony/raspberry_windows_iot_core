using SerialLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace DoMesCenterLIB
{
  public class TeleInformationVM : PeripheriqueSerieVM
  {
    private TeleInformation Modele
    {
      get { return (TeleInformation)_modele; }
    }
    private async void Modele_SurNouvellesDonnees(object sender, ValeursChangees changements)
    {
      await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
      {
        if ((changements & (ValeursChangees.vcOptionTarifaire | ValeursChangees.vcISouscrite | ValeursChangees.vcHoraireHCHP)) != 0)
          OnPropertyChanged("DescriptionAbo");
        if ((changements & (ValeursChangees.vcPInstantanee)) != 0)
          OnPropertyChanged("PInstantanee");
        if ((changements & (ValeursChangees.vcIInstantanee)) != 0)
          OnPropertyChanged("IInstantane");
        if ((changements & (ValeursChangees.vcIndexHP_BASE)) != 0)
          OnPropertyChanged("IndexHP_BASE");
        if ((changements & (ValeursChangees.vcIndexHC)) != 0)
          OnPropertyChanged("IndexHC");
        if ((changements & (ValeursChangees.vcPeriodeTarifaire)) != 0)
          OnPropertyChanged("PeriodeTarifaire");
        if ((changements & (ValeursChangees.vcDepassementI)) != 0)
          OnPropertyChanged("DepassementI");
      });
    }

    public string DescriptionAbo    { get { return Modele.OptionTarifaire + " - " + Modele.ISouscrite.ToString() + "A - " + Modele.HoraireHPHC; } }
    public uint PInstantanee        { get { return Modele.PInstantanee; } }
    public uint IInstantane         { get { return Modele.IInstantane; } }
    public ulong IndexHP_BASE       { get { return Modele.IndexHP_BASE; } }
    public ulong IndexHC            { get { return Modele.IndexHC; } }
    public string PeriodeTarifaire  { get { return Modele.PeriodeTarifaireEnCours; } }
    public bool DepassementI        { get { return Modele.DepassementI; } }
    public TeleInformationVM(PeripheriqueSerie modele) : base(modele)
    {
      Modele.SurNouvellesDonnees += Modele_SurNouvellesDonnees;
    }
  }
}
