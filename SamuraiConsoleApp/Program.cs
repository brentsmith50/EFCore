using SamuraiApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SamuraiConsoleApp
{
    public enum InclusionTypes
    {
        Quotes,
        Battles,
        SecretIdentity,
        QuotesAndIdentities,
        BattlesAndQuotes,
        BattlesAndIdentities,
        All
    };


    class Program
    {
        private static SamuraiContext context = new SamuraiContext();

        static void Main(string[] args)
        {
            context.Database.EnsureCreated();
            context.GetService<ILoggerFactory>().AddProvider(new LoggingProvider());

            #region Basic Conntected Operations
            //      ***     Inserts     ***
            //InsertNewGraph();
            //InsertNewGraphMultipleChildren();
            //InsertOneToOne();
            //AddChildToExistingEntityWhileTracked();
            //AddOneToOneExistingEntityWhileTracked();
            //AddBattles();
            //AddManyToManySamuraiBattlesUsingFK();
            //AddManyToManyWithEntities();

            //      ***     Retrieves     ***
            //EagerLoadWithInclude();
            //EagerLoadManyToManyWithChildrenAndGrandchildren();
            //EagerLoadFilteredManyToManyWithChildrenAndGrandchildren();
            //EagerLoadWithManyBranches();
            //EagerLoadCANTFilterChildren();
            //AnonymousTypeViaProjection();
            //AnonymousTypeViaProjectionWithRelated();
            //RelatedObjectsFixUp();
            //ExplicitLoad();
            //ExplicitLoadWithChildFilter();
            UsingRelatedDataForFilters();
            #endregion

            #region Disconnected Operations   

            //AddAllNewGraph();
            //AttachNewGraph();
            UpdateAllNewGraph();
            #endregion

            Console.ReadLine();
        }

        #region Connected Operations
        #region Insert
        private static void InsertNewGraph()
        {
            Samurai samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I've come to save you" }
                }
            };
            context.Samurais.Add(samurai);
            context.SaveChanges();
            ConsoleOutput(samurai);
        }

        private static void InsertNewGraphMultipleChildren()
        {
            Samurai samurai = new Samurai
            {
                Name = "Kyūzō",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "Watch out for my sharp sword!" },
                    new Quote {Text = "I told you to watch out for the sharp sword! Oh well!" }
                }
            };
            context.Samurais.Add(samurai);
            context.SaveChanges();
            ConsoleOutput(samurai);
        }

        private static void InsertOneToOne()
        {
            Samurai samurai = new Samurai { Name = "Shichirōji" };
            samurai.SecretIdentity = new SecretIdentity { RealName = "Julie" };
            context.Add(samurai);
            context.SaveChanges();
            ConsoleOutput(samurai);
        }

        private static void AddChildToExistingEntityWhileTracked()
        {
            // this should realy be first of default AND check for null
            Samurai samurai = context.Samurais.First();
            samurai.Quotes.Add(new Quote { Text = "I bet you're happy that I've saved you!" });
            context.SaveChanges();
        }

        private static void AddOneToOneExistingEntityWhileTracked()
        {
            Samurai samurai = context.Samurais.FirstOrDefault(s => s.SecretIdentity == null);
            samurai.SecretIdentity = new SecretIdentity { RealName = "Samson" };
            context.SaveChanges();
            ConsoleOutput(samurai);
        }

        private static void AddBattles()
        {
            context.Battles.AddRange
                (
                    new Battle { Name = "Battle of Shiroyama", StartDate = new DateTime(1877, 9, 24), EndDate = new DateTime(1877, 9, 24) },
                    new Battle { Name = "Siege of Osaka", StartDate = new DateTime(1614, 1, 1), EndDate = new DateTime(1615, 12, 31) },
                    new Battle { Name = "Battle of Fuckhead Hill", StartDate = new DateTime(1868, 9, 24), EndDate = new DateTime(1869, 9, 24) }
                );
            context.SaveChanges();
        }

        private static void AddManyToManySamuraiBattlesUsingFK()
        {
            SamuraiBattle samuraiBattle = new SamuraiBattle { SamuraiId = 1, BattleId = 1 };
            context.SamuraiBattles.Add(samuraiBattle);
            context.SaveChanges();
        }

        private static void AddManyToManyWithEntities()
        {
            // This is just quick and dirty ....
            // Would need to check to see if a link already exists
            Samurai samurai = context.Samurais.FirstOrDefault(s => s.Id == 2);
            Battle battle = context.Battles.FirstOrDefault(b => b.Id == 2);

            context.SamuraiBattles.Add(new SamuraiBattle { Samurai = samurai, Battle = battle });
            context.SaveChanges();
        }

        #endregion

        #region Get
        private static void EagerLoadWithInclude()
        {
            List<Samurai> samurais = context.Samurais.Include(s => s.Quotes).ToList();
            ConsoleOutput(samurais, InclusionTypes.Quotes);
        }

        private static void EagerLoadManyToManyWithChildrenAndGrandchildren()
        {
            List<Samurai> samurais = context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(b => b.Battle)
                .ToList();

            // Example: Lazy load ... would need to iterate at some point
            //IEnumerable<Samurai> samurais = context.Samurais.Include(s => s.SamuraiBattles);
            ConsoleOutput(samurais, InclusionTypes.Battles);
        }

        private static void EagerLoadFilteredManyToManyWithChildrenAndGrandchildren()
        {
            List<Samurai> samurais = context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(b => b.Battle)
                .Where(s => s.Name == "Kyūzō").ToList();

            // Example: Lazy load ... would need to iterate at some point
            //IEnumerable<Samurai> samurais = context.Samurais.Include(s => s.SamuraiBattles);
            ConsoleOutput(samurais, InclusionTypes.Battles);
        }

        private static void EagerLoadWithManyBranches()
        {
            List<Samurai> samurais = context.Samurais
                .Include(s => s.SecretIdentity)
                .Include(s => s.Quotes)
                .ToList();

            ConsoleOutput(samurais, InclusionTypes.QuotesAndIdentities);
        }

        private static void EagerLoadCANTFilterChildren()
        {
            //this won't work. No filtering, no sorting on Include
            List<Samurai> samurais = context.Samurais
                .Include(s => s.Quotes.Where(q => q.Text.Contains("happy")))
                .ToList();
        }

        private static void AnonymousTypeViaProjection()
        {
            var quotes = context.Quotes.Select(q => new { q.Id, q.Text }).ToList();

            Console.WriteLine("All saved Quotes:\n");
            quotes.ForEach(q => Console.WriteLine("{0}: {1}", q.Id, q.Text));
        }

        private static void AnonymousTypeViaProjectionWithRelated()
        {
            var samurais = context.Samurais.Select(s => new
            {
                s.Id,
                s.Name,
                QuoteCount = s.Quotes.Count
            }).ToList();
            Console.WriteLine("Quote count for all samurais:\n");
            samurais.ForEach(s => Console.WriteLine("{0} has {1} quotes.", s.Name, s.QuoteCount));
        }

        private static void RelatedObjectsFixUp()
        {
            Samurai samurai = context.Samurais.Find(1);
            // She had a hard coded value in here .....
            // but this works and would be better anyway...I think
            List<Quote> quotes = context.Quotes.Where(q => q.SamuraiId == samurai.Id).ToList();
        }

        private static void ExplicitLoad()
        {
            Samurai samurai = context.Samurais.FirstOrDefault();

            context.Entry(samurai).Collection(q => q.Quotes).Load();
            context.Entry(samurai).Reference(s => s.SecretIdentity).Load();
        }

        private static void ExplicitLoadWithChildFilter()
        {
            Samurai samurai = context.Samurais.FirstOrDefault();

            context.Entry(samurai).Collection(q => q.Quotes)
                .Query().Where(q => q.Text.Contains("happy")).Load();
        }

        private static void UsingRelatedDataForFilters()
        {
            List<Samurai> samurais = context.Samurais.Where(s => s.Quotes.Any(q => q.Text.Contains("happy"))).ToList();
        }
        #endregion
        #endregion

        #region Disconnected Operations
        
        #region Inserts

        private static void AddAllNewGraph()
        {
            Samurai samurai = new Samurai { Name = "Kimosabe" };
            samurai.Quotes.Add(new Quote { Text = "I was quoted saying this!"});

            using (SamuraiContext newContext = new SamuraiContext())
            {
                newContext.Samurais.Add(samurai);
                EntryState();
                newContext.SaveChanges();
            }
        }
        
        private static void AttachNewGraph()
        {
            Samurai samurai = new Samurai { Name = "Super Samurai" };
            samurai.Quotes.Add(new Quote { Text = "The Super sad some shit!"});

            using (SamuraiContext newContext = new SamuraiContext())
            {
                // Not sure what this is doing ...
                // I am getting a FK exception
                newContext.Samurais.Attach(samurai);
                EntryState();
                //newContext.SaveChanges();
            }
        }


        #endregion

        #region Updates

        private static void UpdateAllNewGraph()
        {
            var samuraiToUpdate = context.Samurais.Where(s => s.Name == "Kimosabe")
                .Include(s => s.Quotes)
                .FirstOrDefault();


            //Samurai samurai = new Samurai { Name = "Kimosabe" };
            //samurai.Quotes.Add(new Quote { Text = "THIS CHNAGED: I was quoted saying this!" });

            //using (SamuraiContext newContext = new SamuraiContext())
            //{
            //    newContext.Samurais.Update(samurai);
            //    EntryState();
            //    //newContext.SaveChanges();
            //}
        }
        #endregion

        #region Methods
        private static void ConsoleOutput(Samurai samurai)
        {
            Console.WriteLine("{0} saved successfully", samurai.Name);
        }

        private static void ConsoleOutput(List<Samurai> samurais, InclusionTypes inclusion)
        {
            switch (inclusion)
            {
                case InclusionTypes.Quotes:
                    samurais.ForEach(s => s.Quotes.ForEach(q => Console.WriteLine("{0} quoted: {1}", s.Name, q.Text)));
                    break;
                case InclusionTypes.Battles:
                    samurais.ForEach(s => s.SamuraiBattles.ForEach(b => Console.WriteLine("{0} was in {1}", b.Samurai.Name, b.Battle.Name)));
                    break;
                case InclusionTypes.SecretIdentity:
                    break;
                case InclusionTypes.QuotesAndIdentities:
                    samurais.ForEach(s => s.Quotes.ForEach(q =>
                                     Console.WriteLine("The samurai with:\n\tSecret Identity: {0}\n\tIs quoted: {1}",
                                        s.SecretIdentity == null ? "Not found " : s.SecretIdentity.RealName, q.Text)));
                    break;
                case InclusionTypes.BattlesAndQuotes:
                    break;
                case InclusionTypes.BattlesAndIdentities:
                    break;
                case InclusionTypes.All:
                    break;
            }
           
        }

        private static void DisplayState(List<EntityEntry> entries, string methodName)
        {
            Console.WriteLine(methodName);
            entries.ForEach(e => Console.WriteLine($"{e.Entity.GetType().Name} : {e.State.ToString()}"));
            Console.WriteLine();
        }

        private static void EntryState()
        {
            List<EntityEntry> entries = context.ChangeTracker.Entries().ToList();
            DisplayState(entries, "AddAllNewGraph");
        }
        #endregion

        #endregion
    }
}
