using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialLIB
{
  public class A2MicroThermalPrinter : ImprimanteSerie
  {
    private static readonly string CHARSET_FR = Encoding.ASCII.GetString(SELECT_INTERNATIONAL_CHARACTER_SET(1));
    private static readonly string CHARSET_US = Encoding.ASCII.GetString(SELECT_INTERNATIONAL_CHARACTER_SET(0));
    private static readonly string E_AIGU = CHARSET_FR + "{" + CHARSET_US;
    private static readonly string E_GRAV = CHARSET_FR + "}" + CHARSET_US;
    private static readonly string A_ACCE = CHARSET_FR + "@" + CHARSET_US;
    private static readonly string C_CEDI = CHARSET_FR + "\\" + CHARSET_US;
    private static readonly string U_ACCE = CHARSET_FR + "|" + CHARSET_US;

    public A2MicroThermalPrinter(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire)  { }
    public override Task ImprimerTexte(string texte, int codepage = 0)
    {
      string texteTransforme = texte.Replace("é", E_AIGU);
      texteTransforme = texteTransforme.Replace("è", E_GRAV);
      texteTransforme = texteTransforme.Replace("à", A_ACCE);
      texteTransforme = texteTransforme.Replace("ç", C_CEDI);
      texteTransforme = texteTransforme.Replace("ù", U_ACCE);

      return base.ImprimerTexte(texteTransforme, 20127);
    }
  }
}
