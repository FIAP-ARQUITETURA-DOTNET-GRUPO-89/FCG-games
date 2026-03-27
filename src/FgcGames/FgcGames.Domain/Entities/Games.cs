using System;
using System.Collections.Generic;
using System.Text;

namespace FgcGames.Domain.Entities
{
    public class Games
    {
        public Games(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
