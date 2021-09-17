using System;
using System.Threading;
namespace CSE445ProjA3A4
{
    using System;
    using System.Threading;

    public delegate void priceCutEvent(double lowerPrice, int theaterID); //define a delegate
    public class movieTheater
    {
        static Random rng = new Random();
        private static Semaphore semaphore;

        public class TheaterONE
        {
            int countOne = 0;
            int seatsOne = 100; // I think these should be Static???
            public static event priceCutEvent priceCutONE; //Link event to delegate
                                                           //TODO MAKE THREE THEATRE PRICING SCHEMES
                                                           //for now and testing purposes use RNG and static pricing
            private static int ticketPriceONE = 40;


            public int calculatePrice()  //Pricing Model?

            {
                int price = rng.Next(1, 10);

                //int price = 14;
                return price;
            }
            public int getPrice() //getter for current Ticket Price
            {
                return ticketPriceONE;
            }
            public bool checkAvailability(OrderClass order) //This will be done to try and fulfill Orders once we are looking at buffer?
            {
                bool available = false;
                int numTickets = order.getTickets();
                if (seatsOne - numTickets >= 0)
                {
                    seatsOne = seatsOne - numTickets;
                    available = true;
                    //TODO add confirmation that order was placed and send back to ticket broker
                    return available;
                }
                return available;
            }
            // take verified order from the bank
            // check if you can fufill the order
            //if yes, send the tickets and calculate price based on orders
            //if not, reject the order (just clear it?)
            public void ticketSeller() //Theater Method from which Thread gets created CONSUMER
            {
                //get current price
                //run until price drops 20x or no seats available
                //recalculate price each run
                //update price


                while (countOne < 10 && seatsOne >= 0)
                {
                    Thread.Sleep(500);
                    //take order from queue
                    //FulFill Order using Check Availability
                    //CHECK AVAILABILITY, IF NONE THEN QUIT

                    //RECALC PRICE
                    Console.WriteLine("ticket seller! Count1 is {0}", countOne);
                    int newPrice = calculatePrice();//price calc function
                    lowerPrice(newPrice); //what eventually triggers event for price cut

                }

            }
            public void lowerPrice(int newPrice) //Checking if price cut actually happened, if yes, call delegate 
            {
                Console.WriteLine("Hello!");
                if (newPrice <= ticketPriceONE)
                {
                    ticketPriceONE = newPrice;
                    countOne++;
                    Console.WriteLine("new price calculated in Theater ONE it is {0}", newPrice);
                    if (priceCutONE != null)//if there is a subscriber -> ticket broker
                    {

                        priceCutONE(newPrice, 1);//emit event to subscriber
                    }


                }
                ticketPriceONE = newPrice;


            }


        }

        public class TheaterTWO
        {
            int countTwo = 0;
            int seatTwo = 100; // I think these should be Static???
            public static event priceCutEvent priceCutTWO; //Link event to delegate
                                                           //TODO MAKE THREE THEATRE PRICING SCHEMES
                                                           //for now and testing purposes use RNG and static pricing
            private static int ticketPriceTwo = 40;


            public int calculatePrice()  //Pricing Model?

            {
                int price = rng.Next(1, 10);

                //int price = 12;
                return price;
            }
            public int getPrice() //getter for current Ticket Price
            {
                return ticketPriceTwo;
            }
            public bool checkAvailability(OrderClass order) //This will be done to try and fulfill Orders once we are looking at buffer?
            {
                bool available = false;
                int numTickets = order.getTickets();
                if (seatTwo - numTickets >= 0)
                {
                    seatTwo = seatTwo - numTickets;
                    available = true;
                    //TODO add confirmation that order was placed and send back to ticket broker
                    return available;
                }
                return available;
            }
            // take verified order from the bank
            // check if you can fufill the order
            //if yes, send the tickets and calculate price based on orders
            //if not, reject the order (just clear it?)
            public void ticketSeller() //Theater Method from which Thread gets created CONSUMER
            {
                //get current price
                //run until price drops 20x or no seats available
                //recalculate price each run
                //update price


                while (countTwo < 10 && seatTwo >= 0)
                {
                    Thread.Sleep(500);
                    //take order from queue
                    //FulFill Order using Check Availability
                    //CHECK AVAILABILITY, IF NONE THEN QUIT

                    //RECALC PRICE
                    Console.WriteLine("ticket seller! Count2 is {0}", countTwo);
                    int newPrice = calculatePrice();//price calc function
                    lowerPrice(newPrice); //what eventually triggers event for price cut

                }

            }
            public void lowerPrice(int newPrice) //Checking if price cut actually happened, if yes, call delegate 
            {
                Console.WriteLine("Hello!");
                if (newPrice <= ticketPriceTwo)
                {
                    ticketPriceTwo = newPrice;
                    countTwo++;
                    Console.WriteLine("new price calculated in THEATER TWO it is {0}", newPrice);
                    if (priceCutTWO != null)//if there is a subscriber -> ticket broker
                    {
                        priceCutTWO(newPrice, 2);//emit event to subscriber
                    }


                }
                ticketPriceTwo = newPrice;


            }
        }
        public class ticketBroker
        {
            private int ccNum;
            private int brokerID;

            public ticketBroker(int brokerID)
            {
                ccNum = rng.Next(500, 700);
                this.brokerID = brokerID;

            }
            //always buy from the cheapest theater but only check when there is a price cut
            //if a theater runs out of seats it terminates
            //can only buy tickets in increments of 5 (max 20 in a transaction)
            //start new thread PRODUCER

            public void ticketBrokerFunc()
            {
                Console.WriteLine("Broker" + Thread.CurrentThread.Name + " has Started");
                //Loop to read the Confimration buffer, if there is a confirmation placed, then print and release semaphore???? 
            }
            public void ticketOnSale(double newPrice, int theaterID) //Event Handler
            {
                //Make the Order here. 
                //public OrderClass(string ticketBrokerID, int cardNo, int tickets, string theaterID, double ticketPrice)

                //Check if there is a cell available to place the order in
                semaphore.WaitOne();

                //place order in Buffer. 

                Console.WriteLine("The new price in theater {0} is {1} by broker {2}", theaterID, newPrice, brokerID);


                //once confrimed that we received it??
                semaphore.Release();

            }


        }


        public class boxOffice
        {
            static void Main(string[] args)
            {
                //create movie theater objects
                TheaterONE theaterone = new TheaterONE();
                TheaterTWO theatertwo = new TheaterTWO();

                //create ticket seller thread
                Thread sellerOne = new Thread(new ThreadStart(theaterone.ticketSeller));
                Thread sellerTwo = new Thread(new ThreadStart(theatertwo.ticketSeller));
                //seller two

                //start tikcetseller thread
                sellerOne.Start();
                sellerTwo.Start();




                Thread[] ticketBrokers = new Thread[5];
                //create array of ticket brokers
                //start array of ticket brokers
                for (int i = 0; i < 5; i++)
                {
                    int brokerID = i + 1;
                    ticketBroker buyer = new ticketBroker(brokerID);//consumer
                    TheaterTWO.priceCutTWO += new priceCutEvent(buyer.ticketOnSale);
                    TheaterONE.priceCutONE += new priceCutEvent(buyer.ticketOnSale); //what function put here??

                    //theater two
                    //theater three
                    ticketBrokers[i] = new Thread(new ThreadStart(buyer.ticketBrokerFunc));
                    ticketBrokers[i].Name = (brokerID).ToString();

                    ticketBrokers[i].Start();
                    Console.WriteLine("New thread created " + i);
                }

                Thread.Sleep(500);
                //comment on sempahore
                semaphore.Release(3);
                Console.WriteLine("Main Thread Exit");
                //Hannah wrote this
                //Hannah wrote this ON HER OWN BRANCH
            }
        }
        /*
     * order object class
     * # tickets to order
     * CC number
     * broker name
     * theater name to purchase from
     * current price
     */

        public class OrderClass
        {
            private string ticketBrokerID { get; set; }
            private int cardNo { get; set; }
            private int tickets { get; set; }
            private string theaterID { get; set; }
            private double ticketPrice { get; set; }


            public OrderClass(string ticketBrokerID, int cardNo, int tickets, string theaterID, double ticketPrice)
            {
                this.ticketBrokerID = ticketBrokerID;
                this.cardNo = cardNo;
                this.tickets = tickets;
                this.theaterID = theaterID;
                this.ticketPrice = ticketPrice;
            }

            //ticketBrokerID getter
            public string getTicketBrokerID()
            {
                return ticketBrokerID;
            }

            //ticketBrokerID setter
            public void setTicketBrokerID(string ticketBrokerID)
            {
                this.ticketBrokerID = ticketBrokerID;
            }

            //cardNo getter
            public int getcardNo()
            {
                return cardNo;
            }

            //cardNo setter
            public void setcardNo(int cardNo)
            {
                this.cardNo = cardNo;
            }

            //ticket getter
            public int getTickets()
            {
                return tickets;
            }

            //ticket setter
            public void setTickets(int ticketsToPurchase)
            {
                tickets = ticketsToPurchase;
            }

            //theaterID getter
            public string getTheaterID()
            {
                return theaterID;
            }

            //theaterID stter 
            public void setTheaterID(string theaterID)
            {
                this.theaterID = theaterID;
            }

            //ticketPrice getter
            public double getTicketPrice()
            {
                return ticketPrice;
            }

            //ticketPrice setter
            public void setTicketPrice(double ticketPrice)
            {
                this.ticketPrice = ticketPrice;
            }
        }

    }
}
