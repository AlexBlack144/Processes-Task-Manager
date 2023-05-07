using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;


namespace HWDispatcher
{
    public partial class Form1 : Form
    {
        private List<Process> processes = null;
        private ListViewItemComparer comparer = null;


        public Form1()
        {
            InitializeComponent();
        }

        #region methods

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        private void GetProcentMemoryAndCPU()
        {
            double procentOfMemory = 0;
            double MemorySize = 0;
            double procentOfCPU = 0;

            for (int i = 0; i < columnHeader3.ListView.Items.Count; i++)
            {
                if(columnHeader3.ListView.Items[i].SubItems[1].Text!="0")
                { procentOfCPU = procentOfCPU + double.Parse(columnHeader3.ListView.Items[i].SubItems[1].Text); }

            }

            ulong mem = GetTotalMemoryInBytes();
            MemorySize = mem / (1024 * 1024);

            for (int i = 0; i < columnHeader2.ListView.Items.Count; i++)
            { procentOfMemory = procentOfMemory + double.Parse(columnHeader2.ListView.Items[i].SubItems[1].Text); }


            procentOfMemory = (procentOfMemory * 100) / MemorySize;


            int pom = Convert.ToInt32(procentOfMemory);
            int pocpu = Convert.ToInt32(procentOfCPU)/100;

            label1.Text = $"{pom}%";
            label2.Text = $"{pocpu}%";
        }

        private void GetProcesses()
        {
            processes.Clear();
            processes = Process.GetProcesses().ToList<Process>();


        }

        private void RefreshProcessesList()
        {
            listView1.Items.Clear();
            double memSize = 0;
            double cpu = 0;
            foreach (Process item in processes)
            {
                memSize = 0;

                PerformanceCounter counter = new PerformanceCounter();
                PerformanceCounter counterCPU = new PerformanceCounter("Process", "% Processor Time", item.ProcessName);

                counter.CategoryName = "Process";
                counter.CounterName = "Working Set - Private";
                counter.InstanceName = item.ProcessName;

                memSize = (double)counter.NextValue() / (1000 * 1000);
                cpu = (double)counterCPU.NextValue() /10;
                cpu = (double)counterCPU.NextValue() /10;

                string[] row = new string[] { item.ProcessName.ToString(), Math.Round(memSize, 1).ToString(), Math.Round(cpu, 1).ToString()};

                listView1.Items.Add(new ListViewItem(row));

                counter.Close();
                counter.Dispose();
            }
            Text = "Запущено процессов: " + processes.Count.ToString();
        }

        private void RefreshProcessesList(List<Process> processes, string keyword)
        {
            try
            {
                listView1.Items.Clear();
                double memSize = 0;

                foreach (Process item in processes)
                {
                    if (item != null)
                    {
                        memSize = 0;

                        PerformanceCounter counter = new PerformanceCounter();

                        counter.CategoryName = "Process";
                        counter.CounterName = "Working Set - Private";
                        counter.InstanceName = item.ProcessName;

                        memSize = (double)counter.NextValue() / (1000 * 1000);

                        string[] row = new string[] { item.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };

                        listView1.Items.Add(new ListViewItem(row));

                        counter.Close();
                        counter.Dispose();
                    }
                }
                Text = $"Запущено процессов '{keyword}': " + processes.Count.ToString();
            }
            catch (Exception) { }
        }

        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }
        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();
            GetProcesses();
            RefreshProcessesList();

            comparer = new ListViewItemComparer();
            comparer.ColumIndex = 0;

            GetProcentMemoryAndCPU();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetProcesses();
            RefreshProcessesList();
            GetProcentMemoryAndCPU();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processToKill);

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void завершитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processToKill);

                    GetProcesses();

                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void запуститьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");

            try
            {
                Process.Start(path);
            }
            catch (Exception) { }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filteredProcesses = processes.Where((x) => x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList();

            RefreshProcessesList(filteredProcesses, toolStripTextBox1.Text);

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumIndex = e.Column;

            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            listView1.ListViewItemSorter = comparer;

            listView1.Sort();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }
    }
}
