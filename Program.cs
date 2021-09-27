using System;
using System.Threading;
namespace CSE445Project2
{
    using System;
    using System.Threading;

    public delegate void priceCutEvent(double lowerPrice, int theaterID); //define a delegate
    public class movieTheater
    {
        //Creating global variables 
        static Random rng = new Random();
        private static Semaphore semaphoreWrite, semaphoreRead;
        private static MultiCellBuffer multiCellBuffer;
        private static Confirmationbuffer confirmationbuffer;
        private static int Brokers = 5;

        public class TheaterONE
        {
            //creating variables for Theater One 
            int theaterID = 1;
            int countOne = 0;
            int seatsOne = 200; 
            public static event priceCutEvent priceCutONE; 
            private static double ticketPriceONE = 40;


            public double calculatePrice()  //Pricing Model for Theater 1 to calculate new Price

            {
                double price;
                int season = rng.Next(1, 5);

                int dayOfWeek = rng.Next(1, 8);

                if (season % 2 == 1)
                {
                    if (dayOfWeek % 3 == 1)
                    {
                        if (ticketPriceONE >= 170 && seatsOne >= 50)
                        {
                            price = ticketPriceONE / 2;
                        }
                        else if (ticketPriceONE >= 170 && seatsOne <= 50)
                        {
                            price = (ticketPriceONE / seatsOne) + rng.Next(40, 105);
                        }
                        else if (ticketPriceONE < 170 && ticketPriceONE >= 50 && seatsOne >= 50)
                        {
                            price = (ticketPriceONE * 2) / 3 + rng.Next(30, 80);
                        }
                        else if (ticketPriceONE < 170 && ticketPriceONE >= 50 && seatsOne <= 50)
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
                        if (ticketPriceONE <= 100)
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
                    if (dayOfWeek % 2 == 0)
                    {
                        if (ticketPriceONE >= 165 && seatsOne >= 30)
                        {
                            price = ticketPriceONE - rng.Next(1, 120);
                        }
                        else if (ticketPriceONE >= 165 && seatsOne <= 30)
                        {
                            price = ticketPriceONE / 3;
                        }
                        else if (ticketPriceONE < 165 && ticketPriceONE >= 55)
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
                        if (ticketPriceONE <= 100)
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
            public bool checkAvailability(Order order) //Check Ticket Availability for Order. Returns true if the order could be fulfilled and false if it could not. 
            {
                bool available = false;
                int numTickets = order.getTickets();
                if (seatsOne - numTickets >= 0) //checks if available Tickets are greater than the requested tickets
                {
                    seatsOne = seatsOne - numTickets;
                    available = true;
                   //Adds Confrimation to the Confirmation Buffer for Broker
                    confirmationbuffer.addConfirmation(order.getTicketBrokerID(), order);
                    return available;
                }
                if (numTickets <= 0) //No more tickets available
                {
                    Console.WriteLine("***************************Theater One is SOLD OUT!!***************************");
                }
                return available;
            }
 
            public void ticketSeller() //Theater Method from which Thread gets created CONSUMER
            {
                //Loop until 20 price cuts occur or until there are no more tickets left
                while (countOne < 20 && seatsOne > 0)
                {
                    Thread.Sleep(500);
                    //Check if there are any orders for theater
                    Order order = multiCellBuffer.getOrder(theaterID);
                    if (order != null) //There was an order
                    {
                        if (checkAvailability(order)) //Order Was Available and confirmed
                        {
                            Console.WriteLine("Theather{0} has sent a confirmation to Broker{1} for {2} Tickets.\nTheater{0} has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatsOne);
                        }
                        else //Order was not able to be fuliflled
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
                if(countOne == 20)
                {
                    Console.WriteLine("THEATHER ONE THREAD ENDED. 20 PRICE CUTS OCCURED");
                }
                if(seatsOne <= 0)
                {
                    Console.WriteLine("THEATER ONE THREAD ENDED. SOLD OUT TICKETS");
                }
            }

            //Checking if price cut actually happened, if yes, call delegate 
            public void lowerPrice(double newPrice) 
            {
                //if newly calculated price is lower than current price
                if (newPrice <= ticketPriceONE)
                {
                    //set new price
                    ticketPriceONE = newPrice;
                    //increment count of Price Cuts for Theater one 
                    countOne++;
                    Console.WriteLine("new price calculated in Theater ONE it is ${0}", newPrice.ToString("0.00"));
                    if (priceCutONE != null)//if there is a subscriber -> ticket broker
                    {
                        priceCutONE(newPrice, theaterID);//emit event to subscriber
                    }
                }
                //Assign new price, even if no price cut happened
                ticketPriceONE = newPrice;
            }
        }

        public class TheaterTWO
        {
            int theaterID = 2;
            int countTwo = 0;
            int seatTwo = 200;
            public static event priceCutEvent priceCutTWO;
            private static double ticketPriceTwo = 40;

            
            public double calculatePrice()  //Pricing Model

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

            //Check Ticket Availability for Order. Returns true if the order could be fulfilled and false if it could not.
            public bool checkAvailability(Order order) 
            {
                bool available = false;
                int numTickets = order.getTickets();
                //If available tickets are greater than requested tickets
                if (seatTwo - numTickets >= 0)
                {
                    //Set New Price
                    seatTwo = seatTwo - numTickets;
                    available = true;
                    //add confirmation to confimraiton buffer for broker 
                    confirmationbuffer.addConfirmation(order.getTicketBrokerID(), order);
                    return available;
                }
                //checking if the theater has sold out
                if (numTickets <= 0)
                {
                    Console.WriteLine("***************************Theater Two is SOLD OUT!!***************************");
                }
                return available;
            }


            public void ticketSeller() //Theater Method from which Thread gets created CONSUMER
            {
                //Loop until 20 price cuts occur or until theater runs out of tickets
                while (countTwo < 20 && seatTwo > 0)
                {
                    Thread.Sleep(500);
                    //Getting order from MultiCellbuffer to see if it can fulfil an order
                    Order order = multiCellBuffer.getOrder(theaterID);
                    //An Order was placed for it
                    if (order != null)
                    {
                        if (checkAvailability(order)) //Order Was Available and confirmed
                        {
                            Console.WriteLine("Theather{0} has sent a confirmation to Broker{1} for {2} Tickets.\nTheater{0} has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatTwo);
                        }
                        else //roder was not fulfilled
                        {
                            //Delete order from buffer
                            confirmationbuffer.voidOrder(order.getTicketBrokerID());
                            Console.WriteLine("Theather{0} could not fulfill order for Broker{1} for {2} Tickets.\nTheater{0} only has {3} tickets left.\n", theaterID, order.getTicketBrokerID(), order.getTickets(), seatTwo);
                        }
                    }
                    //RECALC PRICE
                    Console.WriteLine("ticket seller! Count2 is {0}", countTwo);
                    double newPrice = calculatePrice();//price calc function
                    lowerPrice(newPrice); //Checking if newly calculated price is lower than current price

                }

                //Check if price cuts were 20
                if (countTwo == 20)
                {
                    Console.WriteLine("THEATHER TWO THREAD ENDED. 20 PRICE CUTS OCCURED");
                }
                //Check if Tickets were sold out.
                if (seatTwo <= 0)
                {
                    Console.WriteLine("THEATER ONE THREAD ENDED. SOLD OUT TICKETS");
                }

            }
            public void lowerPrice(double newPrice) //Checking if price cut actually happened, if yes, call delegate 
            {
                if (newPrice <= ticketPriceTwo) //checks if new price is less than current price
                {
                    //set new price
                    ticketPriceTwo = newPrice;
                    //increment count for price cuts
                    countTwo++;
                    Console.WriteLine("new price calculated in THEATER TWO it is ${0}", newPrice.ToString("0.00"));
                    if (priceCutTWO != null)//if there is a subscriber -> ticket broker
                    {
                        priceCutTWO(newPrice, theaterID);//emit event to subscriber
                    }
                }
                //set new price
                ticketPriceTwo = newPrice;
            }
        }
        public class ticketBroker
        {
            private int ccNum; //brokers credit card number
            private int brokerID; //Broker's ID

            //Constructor For TicketBroker. Sets Credit Card Num and Broker ID
            public ticketBroker(int brokerID)
            {
                ccNum = rng.Next(500, 700);
                this.brokerID = brokerID;
            }

            public void ticketBrokerFunc() //function from which to create Broker thread
            {
                //Console.WriteLine("Broker" + Thread.CurrentThread.Name + " has Started");
                //Allow broker to check confirmations even if no event is emitted
                for (int i = 0; i < 10; i++)
                {
                    //Get Confirmation from it's cell
                    Order confirmation = confirmationbuffer.getConfirmation(brokerID);
                    if (confirmation != null) //Confirmation was present. Print confirmaiton information. 
                    {
                        Console.WriteLine("LOOP: Broker{0} confirmed a ${3} Order from Theater{1} for {2} Tickets priced at ${4} each. " + confirmation.getOrderTime(), 
                            brokerID, 
                            confirmation.getTheaterID(), 
                            confirmation.getTickets(),
                            (confirmation.getTickets()*confirmation.getTicketPrice()).ToString("0.00"),
                            confirmation.getTicketPrice().ToString("0.00"));
                    }
                }
            }
            public void ticketOnSale(double newPrice, int theaterID) //Event Handler for Broker to Purchase ticket
            {
                //Get Confirmation for Broker
                Order confirmation = confirmationbuffer.getConfirmation(brokerID);
                if (confirmation != null) //Confirmation available. Print confimraiton information.
                {
                    Console.WriteLine("EVENT: Broker{0} confirmed a ${3} Order from Theater{1} for {2} Tickets priced at ${4} each. " + confirmation.getOrderTime(),
                             brokerID,
                             confirmation.getTheaterID(),
                             confirmation.getTickets(),
                             (confirmation.getTickets() * confirmation.getTicketPrice()).ToString("0.00"),
                             confirmation.getTicketPrice().ToString("0.00"));
                }

                //Checking if Broker will buy from tickets. 
                if (rng.Next(1, 11) % 2 == this.brokerID % 2)
                {
                    //Confirming Credit card number is valid 
                    if (ccNum >= 500 && ccNum <= 700)
                    {
                        Console.WriteLine("Broker{0} has a valid credit card", brokerID);
                    }
                    else
                    {
                        Console.WriteLine("Broker{0} does not have a valid credit card", brokerID);
                        return;
                    }
                    //Calculate how many ticets to buy
                    int numOfTickets = numTickets(newPrice);
                    //Check if there is a cell available to place the order in
                    Order newOrder = new Order(this.brokerID, ccNum, numOfTickets, theaterID, newPrice, DateTime.Now);
                    multiCellBuffer.setOrder(newOrder);

                    Console.WriteLine("The new price in theater {0} is ${1} by broker {2}\n", theaterID, newPrice.ToString("0.00"), brokerID);
                }

                Thread.Sleep(rng.Next(500, 1000));
            }

            //Calculates How many ticets to buy 
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

        //Houses the Main function
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

                //start tikcetseller thread
                sellerOne.Start();
                sellerTwo.Start();

                //Start Semaphores
                semaphoreWrite = new Semaphore(0, 3);
                semaphoreRead = new Semaphore(0, 1);


                //Create array for new brokers
                Thread[] ticketBrokers = new Thread[Brokers];
                //start array of ticket brokers
                for (int i = 0; i < Brokers; i++)
                {
                    int brokerID = i + 1;
                    //Create new Broker Object and subscribe to delegate
                    ticketBroker buyer = new ticketBroker(brokerID);//consumer
                    TheaterTWO.priceCutTWO += new priceCutEvent(buyer.ticketOnSale);
                    TheaterONE.priceCutONE += new priceCutEvent(buyer.ticketOnSale); 

                    //Start Thread
                    ticketBrokers[i] = new Thread(new ThreadStart(buyer.ticketBrokerFunc));
                    ticketBrokers[i].Name = (brokerID).ToString();
                    ticketBrokers[i].Start();
                    Console.WriteLine("New thread created " + i);
                }

                Thread.Sleep(500);

                //release semaphores
                semaphoreRead.Release(1);
                semaphoreWrite.Release(3);
                Console.WriteLine("Main Thread Exit");
            }
        }

        //Order Class
        public class Order
        {
            //Instance Variables
            private int ticketBrokerID { get; set; }
            private int cardNo { get; set; }
            private int tickets { get; set; }
            private int theaterID { get; set; }
            private double ticketPrice { get; set; }
            private DateTime orderTime { get; set; }

            //Constructor to set all variables
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

        //MultiCellbuffer Class
        class MultiCellBuffer
        {
            //buffer with 3 cells 
            private Order[] orderBuffer = { null, null, null };
            private int orders = 0;

            //Get Order for Theater. Returns Order object if the theater hand an order. Returns null if it didn't. 
            public Order getOrder(int theaterID)
            {
                //Lock Buffer
                lock (this)
                {
                    //Check each cell to see if an order was for it
                    for (int i = 0; i < 3; i++)
                    {
                        if (orderBuffer[i] != null && orderBuffer[i].getTheaterID() == theaterID) //order was for theater
                        {
                            //Return order and delete it from the buffer. 
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

            //Setting an Order for Broker
            public void setOrder(Order order)
            {
                //Locking Buffer
                semaphoreWrite.WaitOne();
                lock (this)
                {
                    if (orderBuffer[0] == null) //first available. Add order to cell
                    {
                        orderBuffer[0] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[1] == null) //2nd available one. Add order to cell 
                    {
                        orderBuffer[1] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    else if (orderBuffer[2] == null) //3rd one is available. Add order to cell
                    {
                        orderBuffer[2] = order;
                        orders++;
                        Console.WriteLine("Broker{0} just placed an order for Theater{1}\n", order.getTicketBrokerID(), order.getTheaterID());
                    }
                    semaphoreWrite.Release();
                }
            }
        }

        //Confrimation Buffer
        class Confirmationbuffer
        {
            //Array of buffers, will be set to size of how many borkers exist
            private Order[] confirmations;

            //Constructor 
            public Confirmationbuffer()
            {
                //Set Size to number of brokers
                confirmations = new Order[Brokers];
                //Instantiate each to null
                for (int i = 0; i < Brokers; i++)
                {
                    confirmations[i] = null;
                }
            }

            public void addConfirmation(int brokerID, Order confirmation)
            {
                //Busy wait if Broker has not confirmed previous order 
                while (confirmations[brokerID - 1] != null) ;
                
                semaphoreRead.WaitOne();
                lock (this)//Lock buffer
                {
                    //Add Order to teh confirmation cell that belongs to broker
                    confirmations[brokerID - 1] = confirmation;
                    semaphoreRead.Release();
                }
            }

            //Get order. Returns null if no confirmation was present for Broker
            public Order getConfirmation(int brokerID)
            {
                //Lock Order
                semaphoreRead.WaitOne();
                lock (this)
                {
                    //If Broker has confirmation to be consumed
                    if (confirmations[brokerID - 1] != null)
                    {
                        //Return order and null the cell to allow others to write
                        Order confirmation = confirmations[brokerID - 1];
                        confirmations[brokerID - 1] = null;
                        semaphoreRead.Release();
                        return confirmation;
                    }
                }
                semaphoreRead.Release();
                return null;
            }

            //used to null an unfulfillable order
            public void voidOrder(int brokerID)
            {
                semaphoreRead.WaitOne();
                lock (this)
                {
                    //Nulls cell for buffers
                    confirmations[brokerID - 1] = null;
                    semaphoreRead.Release();
                }
            }
        }
    }
}