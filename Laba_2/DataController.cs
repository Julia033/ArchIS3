using Laba_2.DataBase;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Laba_2
{
    class DataController
    {
        public int AddFilm(Cinema cinema)
        {
            using (CinemaContext db = new CinemaContext())
            {
                db.Cinemas.Add(cinema);
                db.SaveChanges();
                int id = db.Cinemas.OrderBy(s => s.ID).First().ID;
                return id;
            }
        }

        public void DeleteFilm(int comand)
        {
            using (CinemaContext db = new CinemaContext())
            {

                var cinemas = db.Cinemas;
                Cinema cinema = db.Cinemas.Find(comand);
                if (cinema == null)
                {
                    throw new ArgumentException();
                }
                else
                {
                    db.Cinemas.Remove(cinema);
                    db.SaveChanges();
                }
            }
        }

        public Cinema GetCinema(int index)
        {
            using (CinemaContext db = new CinemaContext())
            {
                //Возвращает первый объект из базы данных, который удовлетворяет условию
                return db.Cinemas.FirstOrDefault(p => p.ID == index);
            }
        }

        public List<Cinema> GetCinemas()
        {
            using (CinemaContext db = new CinemaContext())
            {
                var entities = db.Set<Cinema>();
                List<Cinema> CinemaList = new List<Cinema>();
                //Перебирает все объекты типа Cinema, которые были получены из базы данных, сортирует их по ID и добавляет в список CinemaList
                foreach (var ent in entities.OrderBy(x => x.ID).ToList())
                {
                    CinemaList.Add(ent);
                }
                return CinemaList;
            }
        }
    }
}
