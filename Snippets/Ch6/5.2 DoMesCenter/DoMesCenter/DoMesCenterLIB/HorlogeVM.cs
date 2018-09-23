using GlobalLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoMesCenterLIB
{
  public class HorlogeVM : VMBase
  {
    private Horloge _modele = null;
    private void _modele_SurToutesLesSecondes(object sender, DateTime instant)
    {
      DateHeureAffichee = instant;
      OnPropertyChanged("DateHeureAffichee");
    }

    public DateTime DateHeureAffichee { get; private set; }
    public HorlogeVM(Horloge model)
    {
      _modele = model;
      _modele.SurToutesLesSecondes += _modele_SurToutesLesSecondes;
    }
  }
}
