using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiConsoleApp
{
    internal static class SimpleCrudOperations
    {
        private static SamuraiContext context = new SamuraiContext();


        #region Getting
        private static void SimpleQuery()
        {
            using (var context = new SamuraiContext())
            {
                var samurais = context.Samurais.ToList();
                foreach (var samurai in samurais)
                {
                    Console.WriteLine(samurai.Name);
                }
            }
        }

        private static void QuerySpecific()
        {
            var samurai = context.Samurais.Find(3);
            Console.WriteLine(samurai.Name);
        }
        #endregion

        #region Inserting or Adding
        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Fred" };

            using (var context = new SamuraiContext())
            {
                context.GetService<ILoggerFactory>().AddProvider(new LoggingProvider());
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }

        private static void InserMultipleSamurais()
        {
            var samurais = new List<Samurai>
            {
                new Samurai {Name = "Fred" },
                new Samurai {Name = "Barney" },
                new Samurai {Name = "Wilma" },
                new Samurai {Name = "Betty" }
            };

            using (var context = new SamuraiContext())
            {
                context.GetService<ILoggerFactory>().AddProvider(new LoggingProvider());
                context.Samurais.AddRange(samurais);
                context.SaveChanges();
            }
        }

        private static void AddSomeMoreSamurais()
        {
            context.AddRange
                (
                    new Samurai { Name = "Kambei Shimada" },
                    new Samurai { Name = "Shichirōji " },
                    new Samurai { Name = "Katsushirō Okamoto" },
                    new Samurai { Name = "Heihachi Hayashida" },
                    new Samurai { Name = "Kyūzō" },
                    new Samurai { Name = "Gorōbei Katayama" }
                );
            context.SaveChanges();
        }
        #endregion

        #region Modifying
        private static void RetrieveAndUpdate()
        {
            Samurai samurai = context.Samurais.FirstOrDefault();
            samurai.Name += "San";
            context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            List<Samurai> samurias = context.Samurais.ToList();
            samurias.ForEach(s => s.Name += "San");
            context.SaveChanges();
            SimpleQuery();
        }

        private static void MultipleOperations()
        {
            // 1. modify
            Samurai samurai = context.Samurais.FirstOrDefault();
            samurai.Name = "JulieSan";

            // 2. Add new
            context.Samurais.Add(new Samurai { Name = "SomeFrickinJapName" });
            context.SaveChanges();

        }

        private static void QueryAndUpdateSamuraiDisconnected()
        {
            string nameFromUserInput = "SomeFrickinJapName";
            Samurai samurai = context.Samurais.FirstOrDefault(s => s.Name == nameFromUserInput);
            samurai.Name += "San";

            using (var localContext = new SamuraiContext())
            {
                localContext.Samurais.Update(samurai);
                localContext.SaveChanges();
            }

            SimpleQuery();
        }

        private static void QueryNoSql()
        {
            var samurais = context.Samurais.Select(s => new { newName = ReverseString(s.Name) }).ToList();
            samurais.ForEach(s => Console.WriteLine(s.newName));
            Console.WriteLine();
        }

        private static string ReverseString(string value)
        {
            var stringChars = value.AsEnumerable();
            return string.Concat(stringChars.Reverse());
        }
        #endregion

        #region Removing

        private static void DeleteWhileNotTracked()
        {
            Samurai samurai = context.Samurais.FirstOrDefault(s => s.Name == "SomeFrickinJapNameSan");
            // Should have a null check before opening connection
            using (var newAppInstanceSimContext = new SamuraiContext())
            {
                newAppInstanceSimContext.Samurais.Remove(samurai);
                newAppInstanceSimContext.SaveChanges();
            }
            SimpleQuery();
        }

        private static void DeleteWhileTracked()
        {
            // using class scoped context here ...
            // which will stay open as long as this app is running OR this class is in memory
            var samurai = context.Samurais.FirstOrDefault(s => s.Name == "JulieSan");
            if (samurai == null) return;

            //alternates:
            // _context.Remove(samurai);
            // _context.Entry(samurai).State=EntityState.Deleted;
            // _context.Samurais.Remove(_context.Samurais.Find(1));
            context.Samurais.Remove(samurai);
            context.SaveChanges();
        }

        private static void DeleteMany()
        {
            var samurais = context.Samurais.Where(s => s.Name.Contains("San"));
            context.RemoveRange(samurais);
            context.SaveChanges();
        }
        #endregion
    }
}
