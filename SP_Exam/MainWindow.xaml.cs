using SP_Exam.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SP_Exam
{
    public partial class MainWindow : Window
    {
        int amountOfFans = 0;
        List<Fan> QueueOfFansBeforeEntrance = new List<Fan>();
        List<Fan> QueueOfFansBeforeSectors = new List<Fan>();
        List<Sector> Sectors = new List<Sector>();
        public int CounterBeforeEntrance = 0;
        public int CounterBeforeSector = 0;
        int WithoutTickets = 0;
        int WithTickets = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
            btnFinish.IsEnabled = false;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbAmount.Text, out amountOfFans) && amountOfFans>=50 && amountOfFans<=200)
            {
                btnStart.IsEnabled = false;
                Task task1 = Task.Run(FillFans);
                Task task2 = task1.ContinueWith(SecurityThreadsMethod);
                Task task3 = task2.ContinueWith(UnblockFinish);
            }
            else
            {
                MessageBox.Show("Error! Uncorect input!");
            }
        }

        private void UnblockFinish(Task obj)
        {
            Dispatcher.Invoke(() => btnFinish.IsEnabled = true);
        }

        private bool CloseSemaphore = false;
        private void SecurityThreadsMethod(Task obj)
        {
            Semaphore semaphore = new Semaphore(4, 4, "semaphore");
            Semaphore semaphore2 = new Semaphore(6, 6, "semaphore");
            while (CounterBeforeSector<amountOfFans)
            {

                if (amountOfFans - (WithoutTickets + WithTickets) >= 3)
                {
                    Thread.Sleep(50);
                }
                if (WithoutTickets + WithTickets >= amountOfFans)
                {
                    Thread.Sleep(50);
                    QueueOfFansBeforeEntrance.Clear();
                    QueueOfFansBeforeSectors.Clear();
                    Dispatcher.Invoke(() => lbControl.Items.Clear());
                    CloseSemaphore = true;
                    semaphore.Close();
                    semaphore2.Close();
                    break;
                }


                if (CounterBeforeEntrance<amountOfFans)
                {
                    ThreadPool.QueueUserWorkItem(SecurityWork, semaphore);
                }
                Thread.Sleep(200);
                if (lbControl.Items.Count > 0)
                {
                    ThreadPool.QueueUserWorkItem(SectorWork, semaphore2);
                }
            }
        }
        private void SectorWork(object state)
        {
            if (state is Semaphore semaphore)
            {
                bool stop = false;

                while (!stop)
                {
                    if (CloseSemaphore)
                    {
                        return;
                    }
                    else if (semaphore.WaitOne(250))
                    {
                        int index = CounterBeforeSector++;
                        
                        try
                        {
                            if (index < QueueOfFansBeforeSectors.Count)
                            {
                                Sectors[int.Parse(QueueOfFansBeforeSectors[index].SectorNumber.ToString()) - 1].Fans.Add(QueueOfFansBeforeSectors[index]);
                                ListBox listBox;
                                switch (int.Parse(QueueOfFansBeforeSectors[index].SectorNumber.ToString()))
                                {
                                    case 1:
                                        listBox = lbSector1;
                                        break;
                                    case 2:
                                        listBox = lbSector2;
                                        break;
                                    case 3:
                                        listBox = lbSector3;
                                        break;
                                    case 4:
                                        listBox = lbSector4;
                                        break;
                                    case 5:
                                        listBox = lbSector5;
                                        break;
                                    case 6:
                                        listBox = lbSector6;
                                        break;
                                    default:
                                        listBox = new ListBox();
                                        break;
                                }
                                Dispatcher.Invoke(() => listBox.Items.Add(QueueOfFansBeforeSectors[index].ToString()));
                                Dispatcher.Invoke(() => lbControl.Items.Remove(QueueOfFansBeforeSectors[index].ToString()));
                            }

                            Thread.Sleep(1000);
                        }
                        finally
                        {
                            if (!CloseSemaphore)
                            {
                                stop = true;
                                semaphore.Release();
                            }
                        }
                    }
                }
            }
        }

        private void SecurityWork(object state)
        {
            if (CloseSemaphore)
            {
                return;
            }
            if (state is Semaphore semaphore)
            {
                bool stop = false;

                while (!stop)
                {
                    if (CloseSemaphore)
                    {
                        return;
                    }
                    else if (semaphore.WaitOne(250) && (CounterBeforeEntrance < amountOfFans))
                    {
                        int index = CounterBeforeEntrance++;
                        try
                        {
                            if (index < amountOfFans && QueueOfFansBeforeEntrance[index].IsHaveTicket)
                            {
                                
                                QueueOfFansBeforeSectors.Add(QueueOfFansBeforeEntrance[index]);
                                Dispatcher.Invoke(() => lbControl.Items.Add(QueueOfFansBeforeEntrance[index].ToString()));
                                Dispatcher.Invoke(() => tbFanats.Text = (int.Parse(tbFanats.Text) + 1).ToString());
                                WithTickets++;
                            }
                            else 
                            {
                                Dispatcher.Invoke(() => tbWithoutTickets.Text = (int.Parse(tbWithoutTickets.Text)+1).ToString());
                                WithoutTickets++;
                            }
                            Thread.Sleep(10);
                        }
                        finally
                        {
                            stop = true;
                            semaphore.Release();
                        }
                    }
                }
            }
        }

        private void FillFans()
        {
            List<Fan> withoutTickets = new List<Fan>();
            Random rand = new Random();
            List<Sector> sectors = new List<Sector>();
            for (int i = 0; i < 6; i++) 
            {
                sectors.Add(new Sector(i + 1));
            }
            for (int i = 0, j = 1; i < amountOfFans;i++, j++)
            {
                foreach (Sector sector in sectors) 
                {
                    if (sectors.All(s=>!s.IsQueueToFill))
                    {
                        sectors.ForEach(s=>s.IsQueueToFill = true);
                    }
                    if (sector.IsQueueToFill) 
                    {
                        Fan currentFan = new Fan(i, sector.Number, i + 1);
                        int randomNum = rand.Next(1, 100);
                        if (randomNum < 10)
                        {
                            withoutTickets.Add(currentFan);
                        }
                        sector.Fans.Add(currentFan);
                        QueueOfFansBeforeEntrance.Add(currentFan);
                        sector.IsQueueToFill = false;

                        break;
                    }
                }
                j = j == 6 ? 1 : j;
            }
            Thread.Sleep(10);
            ListShaker.MakeMixList(QueueOfFansBeforeEntrance);
            foreach (Fan fan in withoutTickets)
            {
                fan.SectorNumber = null;
                fan.SeatNumber = null;
                fan.IsHaveTicket = false;
            }
            sectors.Clear();
            withoutTickets.Clear();

            for (int i = 0; i < 6; i++) 
            {
                Sectors.Add(new Sector(i+1));
            }
        }
        private void ButtonFinish_Click(object sender, RoutedEventArgs e)
        {
            Task task1 = Task.Run(RemoveFansFromSectors);
            Task task2 = task1.ContinueWith(RestartAplication);
        }

        private void RestartAplication(Task obj)
        {
            Thread.Sleep(100);
            Dispatcher.Invoke(() =>
            {
                System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            });
        }

        private void RemoveFansFromSectors()
        {
            List<ListBox> listBoxes = new List<ListBox>();
            listBoxes.Add(lbSector1);
            listBoxes.Add(lbSector2);
            listBoxes.Add(lbSector3);
            listBoxes.Add(lbSector4);
            listBoxes.Add(lbSector5);
            listBoxes.Add(lbSector6);

            while (!listBoxes.All(l=>l.Items.Count==0))
            {
                foreach (ListBox listBox in listBoxes) 
                {
                    Dispatcher.Invoke(() => 
                    {
                        if (listBox.Items.Count > 0)
                        {
                            listBox.Items.Remove(listBox.Items[0]);
                        }
                    });
                }
                foreach (Sector sector in Sectors) 
                {
                    sector.UpdateFields();
                }
                
                Thread.Sleep(500);
            }
        }
    }
}
