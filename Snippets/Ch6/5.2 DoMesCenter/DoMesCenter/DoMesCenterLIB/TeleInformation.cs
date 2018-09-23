using SerialLIB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace DoMesCenterLIB
{
  [Flags]
  public enum ValeursChangees
  {
    vcAucune = 0,
    vcIdCompteur = 1,
    vcOptionTarifaire = 2,
    vcISouscrite = 4,
    vcIndexHP_BASE = 8,
    vcIndexHC = 16,
    vcPeriodeTarifaire = 32,
    vcIInstantanee = 64,
    vcDepassementI = 128,
    vcPInstantanee = 256,
    vcHoraireHCHP = 512,
    vcToutes = 1023,
    vcToutesSaufDepassementI = 895
  }
  public static class CARS
  {
    public const char DEB_TRAME_CAR = '\u0002';
    public static readonly char[] FIN_TRAME_CARS = new char[2] { '\u0003', '\u0004'};
    public const char DEB_GROUPE_CAR = '\n';
    public const char FIN_GROUPE_CAR = '\r';
    public const char SEP_GROUPE_CAR = ' ';
  }
  public static class ETIQUETTES
  {
    public const string ADCO      = "ADCO";
    public const string OPTARIF   = "OPTARIF";
    public const string ISOUSC    = "ISOUSC";
    public const string BASE      = "BASE";
    public const string HCHC      = "HCHC";
    public const string HCHP      = "HCHP";
    public const string PTEC      = "PTEC";
    public const string IINST     = "IINST";
    public const string ADPS      = "ADPS";
    public const string IMAX      = "IMAX";
    public const string HHPHC     = "HHPHC";
    public const string PAPP      = "PAPP";
    public const string MOTDETAT  = "MOTDETAT";
  }

  public delegate void NouvellesDonnees(object sender, ValeursChangees changements);
  public class TeleInformation : PeripheriqueSerie
  {
    private class ValeursTeleInformation
    {
      private string _idCompteur              = "xxxxxxxxxxxx";
      private string _optionTarifaire         = "----";
      private byte _iSouscrite                = 0;
      private ulong _indexHP_BASE             = ulong.MaxValue;
      private ulong _indexHC                  = ulong.MaxValue;
      private string _periodeTarifaireEnCours = "----";
      private uint _iInstantane               = uint.MaxValue;
      private bool _depassementI              = false;
      private uint _pInstantanee              = uint.MaxValue;
      private string _horaireHPHC             = "-";

      private ValeursChangees _changements = ValeursChangees.vcAucune;

      public string IdCompteur
      {
        get { return _idCompteur; }
        set
        {
          if (_idCompteur != value)
          {
            _idCompteur = value;
            _changements |= ValeursChangees.vcIdCompteur;
          }
        }
      }
      public string OptionTarifaire
      {
        get { return _optionTarifaire; }
        set
        {
          if (_optionTarifaire != value)
          {
            _optionTarifaire = value;
            _changements |= ValeursChangees.vcOptionTarifaire;
          }
        }
      }
      public byte ISouscrite
      {
        get { return _iSouscrite; }
        set
        {
          if (_iSouscrite != value)
          {
            _iSouscrite = value;
            _changements |= ValeursChangees.vcISouscrite;
          }
        }
      }
      public ulong IndexHP_BASE {
        get { return _indexHP_BASE; }
        set
        {
          if (_indexHP_BASE != value)
          {
            _indexHP_BASE = value;
            _changements |= ValeursChangees.vcIndexHP_BASE;
          }
        }
      }
      public ulong IndexHC
      {
        get { return _indexHC; }
        set
        {
          if (_indexHC != value)
          {
            _indexHC = value;
            _changements |= ValeursChangees.vcIndexHC;
          }
        }
      }
      public string PeriodeTarifaireEnCours
      {
        get { return _periodeTarifaireEnCours; }
        set
        {
          if (_periodeTarifaireEnCours != value)
          {
            _periodeTarifaireEnCours = value;
            _changements |= ValeursChangees.vcPeriodeTarifaire;
          }
        }
      }
      public uint IInstantane {
        get { return _iInstantane; }
        set
        {
          if (_iInstantane != value)
          {
            _iInstantane = value;
            _changements |= ValeursChangees.vcIInstantanee;
          }
        }
      }
      public bool DepassementI
      {
        get { return _depassementI; }
        set
        {
          if (_depassementI != value)
          {
            _depassementI = value;
            _changements |= ValeursChangees.vcDepassementI;
          }
        }
      }
      public uint PInstantanee {
        get { return _pInstantanee; }
        set
        {
          if (_pInstantanee != value)
          {
            _pInstantanee = value;
            _changements |= ValeursChangees.vcPInstantanee;
          }
        }
      }
      public string HoraireHPHC
      {
        get { return _horaireHPHC; }
        set
        {
          if (_horaireHPHC != value)
          {
            _horaireHPHC = value;
            _changements |= ValeursChangees.vcHoraireHCHP;
          }
        }
      }
      public ValeursChangees Changements
      {
        get { return _changements; }
      }
      public void RazChangements()
      {
        _changements = ValeursChangees.vcAucune; ;
      }
    }
    private ValeursTeleInformation _valeurs = new ValeursTeleInformation();
    private object _donneesRecuesLock = new object();
    private string _donneesBuffer = "";
    private string DerniereTrameDeBuffer(string dernieresDonneesRecues)
    {
      string derniereTrame = "";
      _donneesBuffer += dernieresDonneesRecues;
      int idxLastETX = _donneesBuffer.LastIndexOfAny(CARS.FIN_TRAME_CARS);
      if (idxLastETX >= 0)
      {
        int idxLastSTX = _donneesBuffer.LastIndexOf(CARS.DEB_TRAME_CAR, idxLastETX);
        if (idxLastSTX >= 0)
          derniereTrame = _donneesBuffer.Substring(idxLastSTX + 1, idxLastETX - idxLastSTX - 1);
        _donneesBuffer = _donneesBuffer.Substring(idxLastETX + 1);
      }
      return derniereTrame;
    }
    private Dictionary<string, string> TraiterTrame(string trame)
    {
      Dictionary<string, string> res = new Dictionary<string, string>();
      int idxCourant = 0;
      int idxStartGroupe = 0;
      int idxFinGroupe = 0;
      do
      {
        idxStartGroupe = trame.IndexOf(CARS.DEB_GROUPE_CAR, idxCourant);
        if (idxStartGroupe >= 0)
        {
          idxFinGroupe = trame.IndexOf(CARS.FIN_GROUPE_CAR, idxStartGroupe);
          if (idxFinGroupe >= 0)
          {
            Tuple<string, string> valeurGroupe = TraiterGroupe(trame.Substring(idxStartGroupe + 1, idxFinGroupe - idxStartGroupe - 1));
            if (valeurGroupe != null)
              res.Add(valeurGroupe.Item1, valeurGroupe.Item2);
            idxCourant = idxFinGroupe + 1;
          }
        }
      } while ((idxStartGroupe >= 0) && (idxFinGroupe >= 0));
      return res;
    }
    private Tuple<string, string> TraiterGroupe(string groupe)
    {
      if ((groupe.Length >= 2) && (groupe[groupe.Length - 2] == CARS.SEP_GROUPE_CAR) && VerifierChecksum(groupe))
      {
        int idxPremierSeparateur = groupe.IndexOf(CARS.SEP_GROUPE_CAR, 0, groupe.Length - 2);
        if (idxPremierSeparateur >= 0)
        {
          string etiquette = groupe.Substring(0, idxPremierSeparateur);
          string donnees = groupe.Substring(idxPremierSeparateur + 1, (groupe.Length - 3) - idxPremierSeparateur);
          return Tuple.Create(etiquette, donnees);
        }
      }
      return null;
    }
    private bool VerifierChecksum(string groupe)
    {
      string zoneChecksum = groupe.Substring(0, groupe.Length - 2);
      ushort sum = 0;
      foreach (char car in zoneChecksum)
        sum += car;
      byte checksum = (byte)((sum & 0x003F) + 0x20);
      return checksum == groupe[groupe.Length - 1];
    }
    private ValeursChangees SetValeurs(Dictionary<string, string> valeursGroupe)
    {
      if (valeursGroupe.Count == 0)
        return ValeursChangees.vcAucune;
      lock (_valeursLock)
      {
        string chaineTemp;
        _valeurs.RazChangements();
        if (valeursGroupe.TryGetValue(ETIQUETTES.ADCO, out chaineTemp))
          _valeurs.IdCompteur = chaineTemp;
        if (valeursGroupe.TryGetValue(ETIQUETTES.OPTARIF, out chaineTemp))
          _valeurs.OptionTarifaire = chaineTemp;
        if (valeursGroupe.TryGetValue(ETIQUETTES.ISOUSC, out chaineTemp))
          _valeurs.ISouscrite = Convert.ToByte(chaineTemp);
        if (valeursGroupe.TryGetValue(ETIQUETTES.BASE, out chaineTemp))
          _valeurs.IndexHP_BASE = Convert.ToUInt64(chaineTemp);
        if (valeursGroupe.TryGetValue(ETIQUETTES.HCHP, out chaineTemp))
          _valeurs.IndexHP_BASE = Convert.ToUInt64(chaineTemp);
        if (valeursGroupe.TryGetValue(ETIQUETTES.HCHC, out chaineTemp))
          _valeurs.IndexHC = Convert.ToUInt64(chaineTemp);
        if (valeursGroupe.TryGetValue(ETIQUETTES.PTEC, out chaineTemp))
          _valeurs.PeriodeTarifaireEnCours = chaineTemp;
        if (valeursGroupe.TryGetValue(ETIQUETTES.ADPS, out chaineTemp))
        {
          _valeurs.DepassementI = true;
          _valeurs.IInstantane = Convert.ToUInt32(chaineTemp);
        }
        else if (valeursGroupe.TryGetValue(ETIQUETTES.IINST, out chaineTemp))
        {
          _valeurs.DepassementI = false;
          _valeurs.IInstantane = Convert.ToUInt32(chaineTemp);
        }
        if (valeursGroupe.TryGetValue(ETIQUETTES.PAPP, out chaineTemp))
          _valeurs.PInstantanee = Convert.ToUInt32(chaineTemp);
        if (valeursGroupe.TryGetValue(ETIQUETTES.HHPHC, out chaineTemp))
          _valeurs.HoraireHPHC = chaineTemp;
      }
      return _valeurs.Changements;
    }

    protected override void DonnesRecues(DataReader donnees)
    {
      ValeursChangees valeursModifiees;
      lock (_donneesRecuesLock)
      {
        byte[] donneesOctets = new byte[donnees.UnconsumedBufferLength];
        donnees.ReadBytes(donneesOctets);
        valeursModifiees = SetValeurs(TraiterTrame(DerniereTrameDeBuffer(Encoding.ASCII.GetString(donneesOctets))));
      }
      Debug.WriteLine("DonneesRecues : valeursModifiees = " + valeursModifiees.ToString());
      if (valeursModifiees != ValeursChangees.vcAucune)
        SurNouvellesDonnees?.Invoke(this, valeursModifiees);
    }
    
    private object _valeursLock = new object();
    public string IdCompteur                 { get { lock (_valeursLock) return _valeurs.IdCompteur; }}
    public string OptionTarifaire            { get { lock (_valeursLock) return _valeurs.OptionTarifaire; }}
    public byte ISouscrite                   { get { lock (_valeursLock) return _valeurs.ISouscrite; }}
    public ulong IndexHP_BASE                { get { lock (_valeursLock) return _valeurs.IndexHP_BASE; }}
    public ulong IndexHC                     { get { lock (_valeursLock) return _valeurs.IndexHC; }}
    public string PeriodeTarifaireEnCours    { get { lock (_valeursLock) return _valeurs.PeriodeTarifaireEnCours; }}
    public uint IInstantane                  { get { lock (_valeursLock) return _valeurs.IInstantane; }}
    public bool DepassementI                 { get { lock (_valeursLock) return _valeurs.DepassementI; }}
    public uint PInstantanee                 { get { lock (_valeursLock) return _valeurs.PInstantanee; }}
    public string HoraireHPHC                { get { lock (_valeursLock) return _valeurs.HoraireHPHC; }}
    public TeleInformation(ParametresPortSerie parametres, bool autoOuvrir, uint nbMaxOctetsALire) : base(parametres, autoOuvrir, nbMaxOctetsALire) { }
    public event NouvellesDonnees SurNouvellesDonnees;

  }
}
