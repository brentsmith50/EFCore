using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Data
{
    public class SamuraiRepository
    {
        #region Initialize
        private SamuraiContext context;

        public SamuraiRepository()
        {
            context = new SamuraiContext();
            context.Database.EnsureCreated();
        }
        #endregion

        #region Methods
        public Samurai CreateNewSamurai()
        {
            // Battles are not in this
            Samurai samurai = new Samurai { };
            context.Samurais.Add(samurai);
            return samurai;
        }

        public LocalView<Samurai> SamuraisInMemoryList()
        {
            if (context.Samurais.Local.Count == 0)
            {
                context.Samurais.ToList();
            }
            return context.Samurais.Local;
        }

        public LocalView<Battle> BattlesInMemoryList()
        {
            if (context.Battles.Local.Count == 0)
            {
                context.Battles.ToList();
            }
            return context.Battles.Local;
        }

        public Samurai LoadSamuraiGraph(int samuraiId)
        {
            Samurai samurai = context.Samurais.Find(samuraiId);
            context.Entry(samurai).Reference(s => s.SecretIdentity).Load();
            context.Entry(samurai).Collection(s => s.Quotes).Load();
            return samurai;
        }

        public List<Samurai> SamuraisNotInBattle(int battleId)
        {
            List<int> existingSamurais = context.SamuraiBattles.Where(sb => sb.BattleId == battleId)
                                                               .Select(s => s.SamuraiId).ToList();

            List<Samurai> samurais = context.Samurais.AsNoTracking()
                                                     .Where(s => !existingSamurais.Contains(s.Id)).ToList();

            return samurais;
        }

        public Battle CreateNewBattle()
        {
            //samurais (many to many) will not be involved
            Battle battle = new Battle { Name = "New Battle" };
            context.Battles.Add(battle);
            return battle;
        }

        public Battle LoadBattleGraph(int battleId)
        {
            Battle battle = context.Battles.Find(battleId);
            context.Entry(battle).Collection(b => b.SamuraiBattles).Query().Include(sb => sb.Samurai).Load();

            return battle;
        }

        public void AddSamuraiBattle(SamuraiBattle samuraiBattle)
        {
            // presumes samurai and battle always already exist...
            context.Entry(samuraiBattle).State = EntityState.Added;
        }

        public void RevertBattleChanges(int id)
        {
            //this is the simplest way. 
            //Maybe later versions of EF will make it easier
            context = new SamuraiContext();
        }

        public void SaveSamuraiChanges(Samurai samuraiToSave)
        {
            context.Samurais.Add(samuraiToSave);
            context.SaveChanges();
            if (context.Samurais.Local.Any())
            {
                SamuraisInMemoryList().ToList().ForEach(s => s.IsDirty = false);
            }
        }

        // Really don't like this....
        [Obsolete]
        public void SaveChanges(Type typeBeingEdited)
        {
            context.SaveChanges();
            if (typeBeingEdited == typeof(Samurai))
            {                
                if (context.Samurais.Local.Any())
                {
                    SamuraisInMemoryList().ToList().ForEach(s => s.IsDirty = false);
                }
            }

            if (typeBeingEdited == typeof(Battle))
            {
                if (context.Battles.Local.Any())
                {
                    BattlesInMemoryList().ToList().ForEach(s => s.IsDirty = false);
                }
            }
        }
        #endregion

    }
}
