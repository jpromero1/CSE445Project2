using System;
using System.Threading;
namespace CSE445Project2
{
    using System;
    using System.Threading;

    public delegate void priceCutEvent(double lowerPrice, int theaterID); //define a delegate
    public class movieTheater
    {
        static Random rng = new Random();
        private static Semaphore semaphoreWrite, semaphoreRead;
        private static int[] multiCellBufferTest = { -1, -1, -1 };
        private static MultiCellBuffer multiCellBuffer;

        public class TheaterONE
        {
            int theaterID = 1;
            int countOne = 0;
            int seatsOne = 100; // I think these should be Static???
            public static event priceCutEvent priceCutONE; //Link event to delegate
                                                           //TODO MAKE THREE THEATRE PRICING SCHEMES
                                                           //for now and testing purposes use RNG and static pricing
            private static int ticketPriceONE = 40;


            public int calculatePrice()  //Pricing Model?

            {
                int price = rng.Next(1, 10);
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
                    multiCellBuffer.getOrder(theaterID);

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

                        priceCutONE(newPrice, theaterID);//emit event to subscriber
                    }


                }
                ticketPriceONE = newPrice;


            }


        }

        public class TheaterTWO
        {
            int theaterID = 2;
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
                    multiCellBuffer.getOrder(theaterID);

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
                        priceCutTWO(newPrice, theaterID);//emit event to subscriber
                    }


                }
                ticketPriceTwo = newPrice;


            }
        }
        public class ticketBroker
        {
            private int ccNum = 1000; //TODO CHANGE
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
                OrderClass newOrder = new OrderClass(this.brokerID, ccNum, rng.Next(1, 20), theaterID, newPrice, DateTime.Now);
                multiCellBuffer.setOrder(newOrder);
                //place order in Buffer.

                Console.WriteLine("The new price in theater {0} is {1} by broker {2}\n", theaterID, newPrice, brokerID);


                //once confrimed that we received it??
                //semaphore.Release();

            }


        }


        public class boxOffice
        {
            static void Main(string[] args)
            {
                //create movie theater objects
                TheaterONE theaterone = new TheaterONE();
                TheaterTWO theatertwo = new TheaterTWO();
                multiCellBuffer = new MultiCellBuffer();

                //create ticket seller thread
                Thread sellerOne = new Thread(new ThreadStart(theaterone.ticketSeller));
                Thread sellerTwo = new Thread(new ThreadStart(theatertwo.ticketSeller));
                //seller two

                //start tikcetseller thread
                sellerOne.Start();
                sellerTwo.Start();
                semaphoreWrite = new Semaphore(0, 3);
                //semaphoreRead = new Semaphore(0, 2);



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
                //semaphoreRead.Release(3);
                semaphoreWrite.Release(3);
                Console.WriteLine("Main Thread Exit");
                //Hannah wrote this
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
            private int ticketBrokerID { get; set; }
            private int cardNo { get; set; }
            private int tickets { get; set; }
            private int theaterID { get; set; }
            private double ticketPrice { get; set; }
            private DateTime orderTime { get; set; }

            public OrderClass(int ticketBrokerID, int cardNo, int tickets, int theaterID, double ticketPrice, DateTime orderTime)
            {
                this.ticketBrokerID = ticketBrokerID;
                this.cardNo = cardNo;
                this.tickets = tickets;
                this.theaterID = theaterID;
                this.ticketPrice = ticketPrice;
                this.orderTime = orderTime;
            }

            //ticketBrokerID getter
            public int getTicketBrokerID()
            {
                return ticketBrokerID;
            }

            //ticketBrokerID setter
            public void setTicketBrokerID(int ticketBrokerID)
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
            public int getTheaterID()
            {
                return theaterID;
            }

            //theaterID stter 
            public void setTheaterID(int theaterID)
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

        class MultiCellBuffer
        {
            private OrderClass[] orderBuffer = { null, null, null };

            public OrderClass getOrder(int theaterID)
            {
                lock (this)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (orderBuffer[i] != null && orderBuffer[i].getTheaterID() == theaterID) //order was for me 
                        {
                            OrderClass orderToReturn = orderBuffer[i];
                            orderBuffer[i] = null;
                            Console.Write("Theater{0} consumed an order!\n", theaterID);
                            return orderToReturn;
                        }
                    }
                }
                return null;
            }

            public void setOrder(OrderClass order)
            {
                semaphoreWrite.WaitOne();
                lock (this)
                {
                    if (orderBuffer[0] == null) //first available
                    {
                        orderBuffer[0] = order;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[1] == null) //2nd available one 
                    {
                        orderBuffer[1] = order;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[2] == null) //3rd one is available
                    {
                        orderBuffer[2] = order;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    semaphoreWrite.Release();
                }
            }
        }
    }
}
