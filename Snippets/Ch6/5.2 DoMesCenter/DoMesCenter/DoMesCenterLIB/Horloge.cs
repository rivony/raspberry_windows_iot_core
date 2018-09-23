using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace DoMesCenterLIB
{
  public delegate void HorlogeTick(object sender, DateTime instant);
  public class Horloge
  {
    private DispatcherTimer _timer = new DispatcherTimer();
    private int _derniereMinute = DateTime.Now.Minute;
    private void _timer_Tick(object sender, object e)
    {
      DateTime maintenant = DateTime.Now;
      SurToutesLesSecondes?.Invoke(this, maintenant);
      if (_derniereMinute != maintenant.Minute)
      {
        _derniereMinute = maintenant.Minute;
        SurToutesLesMinutes?.Invoke(this, maintenant);
      }
    }

    public Horloge()
    {
      _timer.Interval = new TimeSpan(0, 0, 1);
      _timer.Tick += _timer_Tick;
      _timer.Start();
    }
    public event HorlogeTick SurToutesLesSecondes;
    public event HorlogeTick SurToutesLesMinutes;
  }
}
