using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimGame.Models;

namespace SimGame.Controllers
{
    public class HomeController : Controller
    {
        SimGameEntities db = new SimGameEntities();
        Random rnd = new Random();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Game()
        {
            //CityViewModel gameViewModel = new GameViewModel();

            City c = db.Cities.ToList().First();
            return View(c);
        }

        public ActionResult NewDay()
        {
            Session["Message"] = null;
            City c = db.Cities.ToList().First();

            if (c.Villagers <= 0)
            {
                return RedirectToAction("Loser");
            }
            else
            {
                
                if (c.Houses > c.Villagers)
                {
                    int villagersMovingIn = Convert.ToInt32(c.Houses- c.Villagers);

                    Session["Message"] = $"A new villager moved in!";
                }
                c.Villagers = c.Houses;
            }

            if (c.Villagers >= 10)
            {
                return RedirectToAction("Winner");
            }
            else
            {
                c.Water += c.Wells;
                c.Food += c.Farm;

                for (int i = 0; i < c.Villagers; i++)
                {
                    if (c.Water >= 1)
                    {
                        c.Water -= 1;
                    }
                    else
                    {
                       
                        if (c.Villagers > 0)
                        {
                            c.Villagers--;
                        }
                    }


                    if (c.Food >= 1 && c.Villagers > 0)
                    {
                        c.Food--;
                    }
                    else 
                    {
                        if (c.Villagers > 0)
                        {
                            c.Villagers--;
                        }
                    }
                }

                if (c.Villagers <= 0)
                {

                    return RedirectToAction("Loser");
                }

                c.Day++;
                c.Actions = c.Villagers;
                db.Cities.AddOrUpdate(c);
                db.SaveChanges();
            }
            return RedirectToAction("Game");
        }

        public ActionResult Build(string building)
        {
            City c = db.Cities.ToList().First();

            if (c.Actions >= 1)
            {
                c.Actions--;

                if (building == "House" && c.Wood >= 5)
                {
                    c.Wood -= 5;
                    c.Houses++;
                }
                else if (building == "Well" && c.Stone >= 5 && c.Wood >= 3)
                {
                    c.Stone -= 5;
                    c.Wood -= 3;
                    c.Wells++;
                }
                else if (building == "Farm" && c.Wood >= 8)
                {
                    c.Wood -= 8;
                    c.Farm++;
                }
                else
                {
                    return RedirectToAction("Game");
                }

                //game state
                db.Cities.AddOrUpdate(c);
                db.SaveChanges();
            }
            return RedirectToAction("Game");
        }

        public ActionResult Gather(string resource)
        {
            City c = db.Cities.ToList().First();
            if (c.Actions >= 1)
            {
                c.Actions--;
                int amount = rnd.Next(1, 7) - 1;
                if (resource == "Food")
                {
                    //Random 0-4
                    int[] foodFound = { 0, 1, 2, 2, 3, 4 };
                    c.Food += foodFound[amount];
                    Session["Message"] = $"Villager gathered {foodFound[amount]} food.";
                }
                else if (resource == "Water")
                {
                    //Random 1-5
                    int[] waterFound = { 1, 2, 3, 3, 4, 5 };
                    c.Water += waterFound[amount];
                    Session["Message"] = $"Villager gathered {waterFound[amount]} gallons of water.";
                }
                else if (resource == "Wood")
                {
                    //Random 1-5
                    int[] woodFound = { 1, 2, 3, 3, 4, 5 };
                    c.Wood += woodFound[amount];
                    Session["Message"] = $"Villager gathered {woodFound[amount]} wood.";
                }
                else if (resource == "Stone")
                {
                    //Random 1-5
                    int[] stoneFound = { 1, 2, 2, 3, 3, 4 };
                    c.Stone += stoneFound[amount];
                    Session["Message"] = $"Villager gathered {stoneFound[amount]} stones.";
                }
                else
                {
                    return RedirectToAction("Game");
                }
                db.Cities.AddOrUpdate(c);
                db.SaveChanges();
            }
            return RedirectToAction("Game");
        }

        public ActionResult Explore()
        {
            City c = db.Cities.ToList().First();

            if (c.Actions >= 1)
            {
                c.Actions--;
                int roll = rnd.Next(1, 10);
                if (roll == 1)
                {
                    int amount = rnd.Next(3, 7);
                    c.Stone += amount;
                    Session["Message"] = $"Your villager brought back {amount} stones";
                }
                else if (roll == 2)
                {
                    int amount = rnd.Next(5, 10);
                    c.Food += amount;
                    Session["Message"] = $"Your villager brought back {amount} food";
                }
                else if (roll == 3)
                {
                    int amount = rnd.Next(6, 11);
                    c.Water += amount;
                    Session["Message"] = $"Your villager brought back {amount} water.";
                }
                else if (roll == 4)
                {
                    int amount = rnd.Next(6, 11);
                    c.Wood += amount;
                    Session["Message"] = $"Your villager brought back {amount} wood.";
                }
                else if (roll == 5)
                {
                    Session["Message"] = $"You gained 1 action.";
                    c.Actions++;
                }
                else if (roll == 6)
                {
                    if (c.Actions > 0)
                    {
                        Session["Message"] = $"Your villager got hurt while exploring! You lose an action.";
                        c.Actions--;
                    }
                }
                else if (roll == 7)
                {
                    Session["Message"] = $"Your villager was killed!";
                    c.Houses--;
                    c.Villagers--;
                }

                else if (roll == 8)
                {
                    Session["Message"] = $"Your villager came back empty";
                }
                else if (roll == 9)
                {
                    Session["Message"] = $"Your villager found a friend";
                    c.Houses++;
                    c.Villagers++;
                }
                else
                {

                    return RedirectToAction("Game");
                }
                db.Cities.AddOrUpdate(c);
                db.SaveChanges();
            }
            return RedirectToAction("Game");
        }

        public ActionResult Loser()
        {
            return View();
        }

        public ActionResult Winner()
        {
            return View();
        }

        public ActionResult NewGame()
        {
            City c = db.Cities.ToList().First();

            //Set game state to game start
            c.Actions = 1;
            c.Day = 1;
            c.Farm = 0;
            c.Food = 6;
            c.Houses = 1;
            c.Stone = 0;
            c.Villagers = 1;
            c.Water = 6;
            c.Wells = 0;
            c.Wood = 0;

            db.Cities.AddOrUpdate(c);
            db.SaveChanges();

            return RedirectToAction("Game");
        }
    }
}