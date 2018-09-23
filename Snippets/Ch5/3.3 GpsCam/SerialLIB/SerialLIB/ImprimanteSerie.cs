using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SerialLIB
{
  public class ImprimanteSerie : PeripheriqueSerie
  {
    protected override void DonnesRecues(DataReader donnees) { }
    protected const byte ESC = 0x1B;
    protected const byte GS = 0x1D;

    public static byte[] INIT()
    {
      return new byte[2] { ESC, (byte)'@' };
    }
    public static byte[] PRINT_AND_FEED(byte nbLines)
    {
      return new byte[3] { ESC, (byte)'d', nbLines };
    }
    public static byte[] PRINTING_MODE(bool fontB, bool reverseMode, bool upDown, bool emphasis, bool doubleHeight, bool doubleWidth, bool deleteLine, bool underLine)
    {
      return new byte[3] { ESC, (byte)'!', (byte)((fontB ? 1 : 0) | (reverseMode ? 2 : 0) | (upDown ? 4 : 0) | (emphasis ? 8 : 0) | (doubleHeight ? 16 : 0) | (doubleWidth ? 32 : 0) | (deleteLine ? 64 : 0) | (underLine ? 128 : 0)) };
    }  
    public static byte[] LEFT_MARGIN(double mm)
    {
      byte nH = (byte)(mm / 0.125 / 256);
      byte nL = (byte)((mm / 0.125) % 256);
      return new byte[4] { GS, (byte)'L', nL, nH };
    }
    public static byte[] SELECT_INTERNATIONAL_CHARACTER_SET(byte carSet)
    {
      return new byte[3] { ESC, (byte)'R', carSet };
    }
    public ImprimanteSerie(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire) { }

    public virtual async Task ImprimerTexte(string texte, int codepage = 0)
    {
      await EnvoyerDonnees(Encoding.GetEncoding(codepage).GetBytes(texte));
    }
    public async Task ImprimerCommande(byte[] commande)
    {
      await EnvoyerDonnees(commande);
    }

    
  }
}
