using PiFaceLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiFaceDigitalDemo
{
  public enum ModesChenillard  { modeAleatoire, modeChenillard, modeClignotant, modeAllerRetour };
  public class ChenillardPi
  {
    private PiFaceDigital _piFace = null;
    private Timer _horloge = null;
    private ModesChenillard _modeEnCours = ModesChenillard.modeChenillard;
    private int _noLedAllumee = 0;
    private Random _hasard = new Random();
    private int _direction = 1;
    private bool _clignotantOn = true;
    private void _piFace_SurChangementEntrees(byte nouvellesvaleurs)
    {
      if ((nouvellesvaleurs & 1) != 0)
        _modeEnCours = ModesChenillard.modeAleatoire;
      else if ((nouvellesvaleurs & 2) != 0)
        _modeEnCours = ModesChenillard.modeChenillard;
      else if ((nouvellesvaleurs & 4) != 0)
        _modeEnCours = ModesChenillard.modeClignotant;
      else if ((nouvellesvaleurs & 8) != 0)
        _modeEnCours = ModesChenillard.modeAllerRetour;
    }
    private void _horloge_Tick(object state)
    {
      switch (_modeEnCours)
      {
        case ModesChenillard.modeAleatoire:
          _noLedAllumee = (_noLedAllumee + _hasard.Next(1, 7)) % 8;
          _piFace.EcrireSorties((byte)(1 << _noLedAllumee));
          break;
        case ModesChenillard.modeChenillard:
          _noLedAllumee = (_noLedAllumee + 1) % 8;
          _piFace.EcrireSorties((byte)(1 << _noLedAllumee));
          break;
        case ModesChenillard.modeClignotant:
          if (_clignotantOn)
            _piFace.EcrireSorties(0xFF);
          else
            _piFace.EcrireSorties(0x00);
          _clignotantOn = !_clignotantOn;
          break;
        case ModesChenillard.modeAllerRetour:
          _noLedAllumee += _direction;
          if (_noLedAllumee >= 8)
          {
            _noLedAllumee = 6;
            _direction = -1;
          }
          else if (_noLedAllumee < 0)
          {
            _noLedAllumee = 1;
            _direction = 1;
          }
          _piFace.EcrireSorties((byte)(1 << _noLedAllumee));
          break;
      }
    }

    public ChenillardPi(PiFaceDigital piFace)
    {
      _piFace = piFace;
      _piFace.SurChangementEntrees += _piFace_SurChangementEntrees;
      _horloge = new Timer(_horloge_Tick, null, Timeout.Infinite, Timeout.Infinite);
    }
    public void Demarrer(int periode)
    {
      _horloge.Change(0, periode);
    }
    public void Arrêter()
    {
      _horloge.Change(Timeout.Infinite, Timeout.Infinite);
    }    
  }
}
