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
        private static Confirmationbuffer confirmationbuffer;

        public class TheaterONE
        {
            int theaterID = 1;
            int countOne = 0;
            int seatsOne = 200; // I think these should be Static???
            public static event priceCutEvent priceCutONE; //Link event to delegate
                                                           //TODO MAKE THREE THEATRE PRICING SCHEMES
                                                           //for now and testing purposes use RNG and static pricing
            private static double ticketPriceONE = 40;


            public double calculatePrice()  //Pricing Model?

            {
                double price;
                int season = rng.Next(1, 5);

                int dayOfWeek = rng.Next(1, 8);

                if (season % 2 == 1)
                {
                    if (dayOfWeek % 3 == 1)
                    {
                        if(ticketPriceONE >= 170 && seatsOne >= 50)
                        {
                            price = ticketPriceONE / 2;
                        }
                        else if(ticketPriceONE >= 170 && seatsOne <= 50)
                        {
                            price = (ticketPriceONE / seatsOne) + rng.Next(40, 105);
                        }
                        else if(ticketPriceONE < 170 && ticketPriceONE >= 50 && seatsOne >= 50)
                        {
                            price = (ticketPriceONE * 2) / 3 + rng.Next(30, 80);
                        }
                        else if(ticketPriceONE < 170 && ticketPriceONE >= 50 && seatsOne <= 50)
                        {
                            price = ticketPriceONE + rng.Next(1, 30);
                        }
                        else
                        {
                            price = ticketPriceONE * 2 + rng.Next(25, 70);
                        }
                    }
                    else
                    {
                        if(ticketPriceONE <= 100)
                        {
                            price = ticketPriceONE * 2; 
                        }
                        else
                        {
                            price = ticketPriceONE / 2;
                        }
                    }
                    return price;
                }
                else
                {
                    if(dayOfWeek % 2 == 0 )
                    {
                        if(ticketPriceONE >= 165 && seatsOne >= 30)
                        {
                            price = ticketPriceONE - rng.Next(1, 120);
                        }
                        else if (ticketPriceONE >= 165 && seatsOne <= 30)
                        {
                            price = ticketPriceONE / 3;
                        }
                        else if(ticketPriceONE < 165 && ticketPriceONE >= 55)
                        {
                            price = ticketPriceONE + rng.Next(2, 10);
                        }
                        else
                        {
                            price = ticketPriceONE * 3; 
                        }
                      
                    }
                    else
                    {
                        if(ticketPriceONE <= 100)
                        {
                            price = ticketPriceONE + (ticketPriceONE * rng.Next(1, 5) * .1);
                        }
                        else
                        {
                            price = ticketPriceONE - rng.Next(30, 60);
                        }
                        
                    }
                    return price;
                }


            }
            public double getPrice() //getter for current Ticket Price
            {
                return ticketPriceONE;
            }
            public bool checkAvailability(Order order) //This will be done to try and fulfill Orders once we are looking at buffer?
            {
                bool available = false;
                int numTickets = order.getTickets();
                if (seatsOne - numTickets >= 0)
                {
                    seatsOne = seatsOne - numTickets;
                    available = true;
                    //TODO add confirmation that order was placed and send back to ticket broker
                    Console.WriteLine("DEBUG: Theater{0} is adding Confirmation for Broker{1}", theaterID, order.getTicketBrokerID());
                    confirmationbuffer.addConfirmation(order.getTicketBrokerID(), order);
                    return available;
                }
                if (numTickets <= 0)
                {
                    Console.WriteLine("Theater One is SOLD OUT!!");
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


                while (countOne < 20 && seatsOne > 0)
                {
                    Thread.Sleep(500);
                    //take order from queue
                    //FulFill Order using Check Availability
                    //CHECK AVAILABILITY, IF NONE THEN QUIT
                    Order order = multiCellBuffer.getOrder(theaterID);
                    if (order != null)
                    {
                        if (checkAvailability(order)) //Order Was Available and confirmed
                        {
                            Console.WriteLine("Theather{0} has sent a confirmation to Broker{1} for {2} Tickets.\nTheater{0} has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatsOne);
                        }
                        else
                        {
                            confirmationbuffer.voidOrder(order.getTicketBrokerID());
                            Console.WriteLine("Theather{0} could not fulfill order for Broker{1} for {2} Tickets.\nTheater{0} only has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatsOne);
                        }
                    }
                    //RECALC PRICE
                    Console.WriteLine("ticket seller! Count1 is {0}", countOne);
                    double newPrice = calculatePrice();//price calc function
                    lowerPrice(newPrice); //what eventually triggers event for price cut

                }

            }
            public void lowerPrice(double newPrice) //Checking if price cut actually happened, if yes, call delegate 
            {
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
            int seatTwo = 200; 
            public static event priceCutEvent priceCutTWO; //Link event to delegate
                                                           //TODO MAKE THREE THEATRE PRICING SCHEMES
                                                           //for now and testing purposes use RNG and static pricing
            private static double ticketPriceTwo = 40;


            public double calculatePrice()  //Pricing Model?

            {
                double price;
                int season = rng.Next(1, 5);
                DateTime moment = DateTime.Now;
                int dayOfWeek = rng.Next(1, 8);

                if (season == 1)
                {
                    if (dayOfWeek <= 3)
                    {
                        if (ticketPriceTwo <= 150)
                        {
                            price = ticketPriceTwo + (ticketPriceTwo * (.1 * rng.Next(1, 4)));
                        }
                        else
                        {
                            price = ticketPriceTwo / 3;
                        }
                    }
                    else
                    {
                        if (ticketPriceTwo <= 120)
                        {
                            price = ticketPriceTwo + (ticketPriceTwo * (.157 * rng.Next(1, 3)));
                        }
                        else
                        {
                            price = (ticketPriceTwo / 4) + rng.Next(10, 16);
                        }
                    }
                }
                else if (season == 2)
                {
                    if (dayOfWeek <= 4)
                    {
                        if (ticketPriceTwo <= 100)
                        {
                            price = (ticketPriceTwo / 2) + rng.Next(21, 150);
                        }
                        else
                        {
                            price = ticketPriceTwo / 3 - rng.Next(8, 24);
                        }
                    }
                    else
                    {
                        if (ticketPriceTwo <= 100)
                        {
                            price = (ticketPriceTwo / 2) + rng.Next(21, 150);
                        }
                        else
                        {
                            price = ticketPriceTwo / 3;
                        }
                    }
                }
                else if (season == 3)
                {
                    if (dayOfWeek <= 2)
                    {
                        if (ticketPriceTwo <= 160)
                        {
                            price = (ticketPriceTwo / 4) + rng.Next(25, 159);
                        }
                        else
                        {
                            price = ticketPriceTwo / 2 + rng.Next(5, 118);
                        }
                    }
                    else
                    {
                        if (ticketPriceTwo <= 190)
                        {
                            price = (ticketPriceTwo / 2) + rng.Next(21, 104);
                        }
                        else
                        {
                            price = ticketPriceTwo / 3.1415;
                        }
                    }
                }
                else
                {
                    if (dayOfWeek <= 6)
                    {
                        if (ticketPriceTwo <= 160)
                        {
                            price = (ticketPriceTwo / 4) + rng.Next(25, 159);
                        }
                        else
                        {
                            price = ticketPriceTwo - rng.Next(1, 40);
                        }
                    }
                    else
                    {
                        if (ticketPriceTwo <= 160)
                        {
                            price = (ticketPriceTwo / 4) + rng.Next(25, 159);
                        }
                        else
                        {
                            price = ticketPriceTwo / 2 + rng.Next(5, 118);
                        }
                    }
                }



                return price;
            }
            public double getPrice() //getter for current Ticket Price
            {
                return ticketPriceTwo;
            }
            public bool checkAvailability(Order order) //This will be done to try and fulfill Orders once we are looking at buffer?
            {
                bool available = false;
                int numTickets = order.getTickets();
                if (seatTwo - numTickets >= 0)
                {
                    seatTwo = seatTwo - numTickets;
                    available = true;
                    //TODO add confirmation that order was placed and send back to ticket broker
                    Console.WriteLine("DEBUG: Theater{0} is adding Confirmation for Broker{1}", theaterID, order.getTicketBrokerID());
                    confirmationbuffer.addConfirmation(order.getTicketBrokerID(), order);
                    return available;
                }
                if (numTickets <= 0)
                {
                    Console.WriteLine("Theater Two is SOLD OUT!!");
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


                while (countTwo < 20 && seatTwo > 0)
                {
                    Thread.Sleep(500);
                    //take order from queue
                    //FulFill Order using Check Availability
                    //CHECK AVAILABILITY, IF NONE THEN QUIT
                    Order order = multiCellBuffer.getOrder(theaterID);
                    if (order != null)
                    {
                        if (checkAvailability(order)) //Order Was Available and confirmed
                        {
                            Console.WriteLine("Theather{0} has sent a confirmation to Broker{1} for {2} Tickets.\nTheater{0} has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatTwo);
                        }

                        else
                        {
                            confirmationbuffer.voidOrder(order.getTicketBrokerID());
                            Console.WriteLine("Theather{0} could not fulfill order for Broker{1} for {2} Tickets.\nTheater{0} only has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatTwo);
                        }
                    }
                    //RECALC PRICE
                    Console.WriteLine("ticket seller! Count2 is {0}", countTwo);
                    double newPrice = calculatePrice();//price calc function
                    lowerPrice(newPrice); //what eventually triggers event for price cut

                }

            }
            public void lowerPrice(double newPrice) //Checking if price cut actually happened, if yes, call delegate 
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
                //Console.WriteLine("Broker" + Thread.CurrentThread.Name + " has Started");
                for (int i = 0; i < 10; i++)
                {
                    Order confirmation = confirmationbuffer.getConfirmation(brokerID);
                    if (confirmation != null)
                    {
                        Console.WriteLine("LOOP: Broker{0} confirmed Order from Theater{1} for {2} Tickets at " + confirmation.getOrderTime(), brokerID, confirmation.getTheaterID(), confirmation.getTickets());
                    }
                }
            }
            public void ticketOnSale(double newPrice, int theaterID) //Event Handler
            {
                //Make the Order here. 
                //public OrderClass(string ticketBrokerID, int cardNo, int tickets, string theaterID, double ticketPrice)
                Order confirmation = confirmationbuffer.getConfirmation(brokerID);
                if (confirmation != null)
                {

                    Console.WriteLine("EVENT: Broker{0} confirmed Order from Theater{1} for {2} Tickets at " + confirmation.getOrderTime(), brokerID, confirmation.getTheaterID(), confirmation.getTickets());
                }

                if (rng.Next(1,11)%2 == this.brokerID % 2)
                {
                    int numOfTickets = numTickets(newPrice);
                    //Check if there is a cell available to place the order in
                    Order newOrder = new Order(this.brokerID, ccNum, numOfTickets, theaterID, newPrice, DateTime.Now);
                    multiCellBuffer.setOrder(newOrder);
                    //place order in Buffer.

                    Console.WriteLine("The new price in theater {0} is {1} by broker {2}\n", theaterID, newPrice, brokerID);
                }
                Thread.Sleep(rng.Next(500, 1000));
                //once confrimed that we received it??
                //semaphore.Release();

            }
            public int numTickets(double newPrice)
            {
                int numTickets;
                if (newPrice <= 40)
                {
                    numTickets = Convert.ToInt32(5 + (newPrice % 10));
                }
                else if (newPrice > 40 && newPrice <= 100)
                {
                    numTickets = Convert.ToInt32(4 + (newPrice % 10));
                }
                else if (newPrice > 100 && newPrice <= 160)
                {
                    numTickets = Convert.ToInt32(3 + (newPrice % 10));
                }
               
                else if (newPrice > 160 && newPrice <= 190)
                {
                    numTickets = Convert.ToInt32(2 + (newPrice % 10));
                }
                else
                {
                    numTickets = Convert.ToInt32(1 + (newPrice % 10));
                }
                return numTickets;
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
                confirmationbuffer = new Confirmationbuffer();

                //create ticket seller thread
                Thread sellerOne = new Thread(new ThreadStart(theaterone.ticketSeller));
                Thread sellerTwo = new Thread(new ThreadStart(theatertwo.ticketSeller));
                //seller two

                //start tikcetseller thread
                sellerOne.Start();
                sellerTwo.Start();
                semaphoreWrite = new Semaphore(0, 3);
                semaphoreRead = new Semaphore(0, 1);



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
                semaphoreRead.Release(1);
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

        public class Order
        {
            private int ticketBrokerID { get; set; }
            private int cardNo { get; set; }
            private int tickets { get; set; }
            private int theaterID { get; set; }
            private double ticketPrice { get; set; }
            private DateTime orderTime { get; set; }

            public Order(int ticketBrokerID, int cardNo, int tickets, int theaterID, double ticketPrice, DateTime orderTime)
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

            //Time Setter
            public void setOrderTime(DateTime orderTime)
            {
                this.orderTime = orderTime;
            }

            public DateTime getOrderTime()
            {
                return orderTime;
            }
        }

        class MultiCellBuffer
        {
            private Order[] orderBuffer = { null, null, null };
            private int orders = 0;

            public Order getOrder(int theaterID)
            {
                lock (this)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (orderBuffer[i] != null && orderBuffer[i].getTheaterID() == theaterID) //order was for me 
                        {
                            Order orderToReturn = orderBuffer[i];
                            orderBuffer[i] = null;
                            orders--;
                            Console.Write("Theater{0} consumed an order!\n", theaterID);
                            return orderToReturn;
                        }
                    }
                }
                return null;
            }

            public void setOrder(Order order)
            {
                semaphoreWrite.WaitOne();
                lock (this)
                {
                    if (orderBuffer[0] == null) //first available
                    {
                        orderBuffer[0] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[1] == null) //2nd available one 
                    {
                        orderBuffer[1] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[2] == null) //3rd one is available
                    {
                        orderBuffer[2] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    semaphoreWrite.Release();
                }
            }
        }

        class Confirmationbuffer
        {
            private Order[] confirmations = { null, null, null, null, null };

            public void addConfirmation(int brokerID, Order confirmation)
            {
                while (confirmations[brokerID - 1] != null)
                {
                    //Console.WriteLine("Theater{0} Stuck in Wait for Broker{1}\n", confirmation.getTheaterID(), brokerID);
                    //Monitor.Wait(this);
                }
                semaphoreRead.WaitOne();
                lock (this) //Maybe we can do try enter????
                {

                    confirmations[brokerID - 1] = confirmation;
                    //Monitor.Pulse(this);
                    semaphoreRead.Release();
                }
            }

            public Order getConfirmation(int brokerID)
            {
                // Console.WriteLine("DEBUG: Broker{0} is trying to get confirmation", brokerID);
                semaphoreRead.WaitOne();
                lock (this)
                {
                    if (confirmations[brokerID - 1] != null)
                    {
                        Order confirmation = confirmations[brokerID - 1];
                        confirmations[brokerID - 1] = null;
                        semaphoreRead.Release();
                        return confirmation;
                    }
                }
                semaphoreRead.Release();
                return null;
            }

            public void voidOrder(int brokerID)
            {
                semaphoreRead.WaitOne();
                lock (this)
                {
                    confirmations[brokerID - 1] = null;
                    semaphoreRead.Release();
                }
            }
        }
    }
}
