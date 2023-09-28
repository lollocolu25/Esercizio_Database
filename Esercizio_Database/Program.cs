using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esercizio_Database
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connStr = @"Server=localhost\SQLEXPRESS; " +
                                "initial catalog=orders; " +
                                "User ID=sa; Password=ss;";
            SqlConnection con = new SqlConnection(connStr);
            using (con)
            {
                Console.WriteLine($"connessione creata {con}");
                con.Open();
                Console.WriteLine("connessione aperta");
                int input = 0;
                ControlloAccount(con);
                if (!ControlloUtente(con))
                {
                    return;
                }
                while (input != 3)
                {
                    Console.WriteLine("1 crea utente");
                    Console.WriteLine("2 lista ordini");
                    Console.WriteLine("3 esci");
                    input = int.Parse(Console.ReadLine());
                    switch (input)
                    {
                        case 1:
                            NuovoUtente(con);
                            break;
                        case 2:
                            ListaOrdini(con);
                            break;
                        case 3:
                            Console.ReadKey();
                            return;
                        default:
                            Console.WriteLine("errore");
                            break;
                    }
                }
            }
            Console.ReadLine();
        }
        public static void ControlloAccount(SqlConnection con)
        {
            var cmd = new SqlCommand("select count(*) from utenti", con);
            var vuoto = cmd.ExecuteScalar();
            int i = (int)vuoto;
            if (i <= 0)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine("utente creato");
                cmd = new SqlCommand("insert into utenti values('admin','admin')", con);
                cmd.ExecuteReader().Close();
            }
            else
            {
                Console.WriteLine("c'è gia un account");
            }
        }
        public static bool ControlloUtente(SqlConnection con)
        {
            Console.WriteLine("Inserisci username: ");
            string username = Console.ReadLine();
            var cmd = new SqlCommand($"select * from utenti where username = @user", con);
            cmd.Parameters.Add("@user", username);
            using (var utenti = cmd.ExecuteReader())
            {
                if (utenti.Read())
                {
                    username = (string)utenti["username"];
                    Console.WriteLine("utente trovato");
                }
                else
                {
                    Console.WriteLine("utente non trovato");
                    Console.ReadLine();
                    return false;
                }
            }
            Console.WriteLine("inserisci password");
            var password = Console.ReadLine();
            cmd = new SqlCommand("select * from utenti where username = @user and password = @password", con);
            cmd.Parameters.Add("@password", password);
            cmd.Parameters.Add("@user", username);
            using (var utente = cmd.ExecuteReader())
            {
                if (utente.Read())
                {
                    Console.WriteLine($"benvenuto {utente["username"]}");
                    return true;
                }
                else
                {
                    Console.WriteLine("password sbagliata");
                    Console.ReadLine();
                    return false;
                }
            }
        }
        public static void ListaOrdini(SqlConnection con)
        {
            var str = "  select or.orderid, customer, orderdate, sum(price*qty) as tot speso from orders as or inner join orderitems as oi on oi.orderid = oi.orderid" +
                "  group by or.customer,or.orderid, or.orderdate";
            var cmd = new SqlCommand(str, con);
            using (var orders = cmd.ExecuteReader())
            {
                while (orders.Read())
                {
                    Console.WriteLine($"{orders["orderid"]}, {orders["customer"]}, {orders["orderdate"]} ");
                }
            }
        }
        private static bool NuovoUtente(SqlConnection con)
        {
            Console.WriteLine("crea username");
            var user = Console.ReadLine();
            Console.WriteLine("--------------------------");
            Console.WriteLine("crea password");
            var psw = Console.ReadLine();
            SqlTransaction tr = null;
            tr = con.BeginTransaction();
            Console.WriteLine("--------------------------");
            Console.WriteLine("Inserisci utente");
            var cmd = new SqlCommand("insert into utenti Values(@user, @psw)", con, tr);
            cmd.Parameters.Add("@user", user);
            cmd.Parameters.Add("@psw", psw);
            Console.WriteLine($" {cmd.ExecuteNonQuery()}");
            return true;
        }
    }
}
