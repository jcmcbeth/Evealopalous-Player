using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using McBeth.EvealopalousPlayer.Models;

namespace McBeth.EvealopalousPlayer
{
    public interface IEvealopalousRepository
    {
        List<Event> GetActiveEvents();
        List<Event> GetPlayerEvents();
        List<Event> GetPlayerHistory();
    }
}
