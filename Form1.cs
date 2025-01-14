using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace toDo
{
    public partial class Form1 : Form
    {
        private Panel calendarPanel;
        private DateTime selectedDate;
        private Dictionary<DateTime, List<Task>> tasks = new Dictionary<DateTime, List<Task>>(); 
        private int totalCompletedTasks = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            
            this.ClientSize = new Size(800, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog; 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Calendar";
            this.BackColor = Color.FromArgb(30, 30, 30); 

            
            calendarPanel = new Panel
            {
                BackColor = Color.FromArgb(40, 40, 40), 
                Dock = DockStyle.Fill, 
                Padding = new Padding(20)
            };
            this.Controls.Add(calendarPanel);

            
            selectedDate = DateTime.Now;

            
            RenderCalendar(selectedDate);
        }

        private void RenderCalendar(DateTime date)
        {
            calendarPanel.Controls.Clear();

            
            Label currentYearLabel = new Label
            {
                Text = $"{date.Year}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point((calendarPanel.Width - 150) / 2, 10),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White 
            };
            calendarPanel.Controls.Add(currentYearLabel);

            
            Label currentMonthLabel = new Label
            {
                Text = $"{date.ToString("MMMM")}",
                Font = new Font("Arial", 14, FontStyle.Regular),
                Location = new Point((calendarPanel.Width - 150) / 2, 40),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White 
            };
            calendarPanel.Controls.Add(currentMonthLabel);

            
            string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            int topOffset = 80;
            int dayCellSize = (calendarPanel.Width - 60) / 7;

            for (int i = 0; i < daysOfWeek.Length; i++)
            {
                Label dayLabel = new Label
                {
                    Text = daysOfWeek[i],
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(dayCellSize, dayCellSize),
                    Location = new Point(20 + i * dayCellSize, topOffset),
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.White 
                };
                calendarPanel.Controls.Add(dayLabel);
            }

            
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime firstDay = new DateTime(date.Year, date.Month, 1);
            int startDay = ((int)firstDay.DayOfWeek + 6) % 7; 

            int rowOffset = topOffset + dayCellSize + 10;
            for (int day = 1; day <= daysInMonth; day++)
            {
                int row = (startDay + day - 1) / 7;
                int column = (startDay + day - 1) % 7;

                DateTime currentDay = new DateTime(date.Year, date.Month, day); 

                
                Color dayBackColor = Color.Transparent;
                if (tasks.ContainsKey(currentDay))
                {
                    int taskCount = tasks[currentDay].Count;
                    if (taskCount == 1) dayBackColor = Color.Green;
                    else if (taskCount == 2) dayBackColor = Color.Yellow;
                    else if (taskCount >= 3) dayBackColor = Color.Red;
                }

                Button dayButton = new Button
                {
                    Text = day.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(dayCellSize - 10, dayCellSize - 10),
                    Location = new Point(20 + column * dayCellSize + 5, rowOffset + row * dayCellSize + 5),
                    BackColor = dayBackColor,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 16, FontStyle.Regular), 
                    ForeColor = Color.White, 
                    FlatAppearance = { BorderColor = Color.White, BorderSize = 1 }
                };

                if (currentDay.Date == DateTime.Now.Date)
                {
                    dayButton.FlatAppearance.BorderColor = Color.DodgerBlue; 
                }

                dayButton.Click += (s, e) => ShowTaskWindow(currentDay); 
                dayButton.MouseEnter += (s, e) => dayButton.Cursor = Cursors.Hand; 

                calendarPanel.Controls.Add(dayButton);
            }

            
            Label scoreLabel = new Label
            {
                Text = $"Completion: {totalCompletedTasks}",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(calendarPanel.Width - 150, 10),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.White 
            };
            calendarPanel.Controls.Add(scoreLabel);

            
            Button selectDateButton = new Button
            {
                Text = "Select Month/Year",
                Size = new Size(150, 30),
                Location = new Point(10, 10),
                BackColor = Color.Transparent, 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Regular),
                FlatAppearance = { BorderColor = Color.White, BorderSize = 1 }
            };
            selectDateButton.Click += SelectDateButton_Click;
            calendarPanel.Controls.Add(selectDateButton);

            
            Button jumpToCurrentMonthButton = new Button
            {
                Text = "Jump to Current Month",
                Size = new Size(150, 30),
                Location = new Point(10, 50),
                BackColor = Color.Transparent, 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Regular),
                FlatAppearance = { BorderColor = Color.White, BorderSize = 1 }
            };
            jumpToCurrentMonthButton.Click += (s, e) =>
            {
                selectedDate = DateTime.Now;
                RenderCalendar(selectedDate); 
            };
            calendarPanel.Controls.Add(jumpToCurrentMonthButton);
        }

        private void SelectDateButton_Click(object sender, EventArgs e)
        {
            using (Form dateSelector = new Form())
            {
                dateSelector.Text = "Select Month and Year";
                dateSelector.Size = new Size(300, 250);
                dateSelector.StartPosition = FormStartPosition.CenterParent;
                dateSelector.BackColor = Color.FromArgb(40, 40, 40); 

                NumericUpDown yearPicker = new NumericUpDown
                {
                    Minimum = 1900,
                    Maximum = 2100,
                    Value = selectedDate.Year,
                    Location = new Point(50, 50),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(60, 60, 60) 
                };
                ComboBox monthPicker = new ComboBox
                {
                    Location = new Point(50, 80),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(60, 60, 60)
                };
                monthPicker.Items.AddRange(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToArray());
                monthPicker.SelectedIndex = selectedDate.Month - 1;

                Button confirmButton = new Button
                {
                    Text = "Select",
                    Location = new Point(50, 120),
                    Size = new Size(200, 30),
                    BackColor = Color.Transparent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                confirmButton.Click += (s, e) =>
                {
                    selectedDate = new DateTime((int)yearPicker.Value, monthPicker.SelectedIndex + 1, 1);
                    RenderCalendar(selectedDate);
                    dateSelector.Close();
                };

                dateSelector.Controls.Add(yearPicker);
                dateSelector.Controls.Add(monthPicker);
                dateSelector.Controls.Add(confirmButton);
                dateSelector.ShowDialog();
            }
        }

        private void ShowTaskWindow(DateTime date)
        {
            
            Form taskMenu = new Form
            {
                Text = $"Tasks for {date.ToString("MMMM dd, yyyy")}",
                Size = new Size(400, 300), 
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40), 
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                MaximizeBox = false
            };

            
            Button addTaskButton = new Button
            {
                Text = "Add new Task",
                Size = new Size(150, 30),
                Location = new Point(125, 50),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Button modifyTaskButton = new Button
            {
                Text = "Modify a Task",
                Size = new Size(150, 30),
                Location = new Point(125, 100),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = tasks.ContainsKey(date) && tasks[date].Count > 0
            };

            Button completeTaskButton = new Button
            {
                Text = "Complete a Task",
                Size = new Size(150, 30),
                Location = new Point(125, 150),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = tasks.ContainsKey(date) && tasks[date].Count > 0
            };

            Button deleteTaskButton = new Button
            {
                Text = "Delete a Task",
                Size = new Size(150, 30),
                Location = new Point(125, 200),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = tasks.ContainsKey(date) && tasks[date].Count > 0
            };

            addTaskButton.Click += (s, e) => AddTask(date, taskMenu);  
            modifyTaskButton.Click += (s, e) => ModifyTask(date);
            completeTaskButton.Click += (s, e) => CompleteTask(date);
            deleteTaskButton.Click += (s, e) => DeleteTask(date);

            taskMenu.Controls.Add(addTaskButton);
            taskMenu.Controls.Add(modifyTaskButton);
            taskMenu.Controls.Add(completeTaskButton);
            taskMenu.Controls.Add(deleteTaskButton);
            taskMenu.ShowDialog();
        }

        private void AddTask(DateTime date, Form taskMenu)
        {
            
            Form addTaskForm = new Form
            {
                Text = "Add Task",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40), 
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                MaximizeBox = false
            };

            TextBox taskNameTextBox = new TextBox
            {
                PlaceholderText = "Task Name",
                Location = new Point(20, 20),
                Size = new Size(340, 30),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60)
            };

            RichTextBox taskDescriptionTextBox = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(340, 60),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60)
            };

            Button saveButton = new Button
            {
                Text = "Save Task",
                Location = new Point(120, 140),
                Size = new Size(150, 30),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            saveButton.Click += (s, e) =>
            {
                if (!tasks.ContainsKey(date))
                    tasks[date] = new List<Task>();

                tasks[date].Add(new Task(taskNameTextBox.Text, taskDescriptionTextBox.Text));
                RenderCalendar(selectedDate); 
                addTaskForm.Close();
                taskMenu.Close(); 
            };

            addTaskForm.Controls.Add(taskNameTextBox);
            addTaskForm.Controls.Add(taskDescriptionTextBox);
            addTaskForm.Controls.Add(saveButton);
            addTaskForm.ShowDialog();
        }


        private void ModifyTask(DateTime date)
        {
            if (!tasks.ContainsKey(date) || tasks[date].Count == 0)
            {
                MessageBox.Show("No tasks available for modification.", "No Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }

            
            Form taskSelectionForm = new Form
            {
                Text = "Select Task to Modify",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40), 
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                MaximizeBox = false
            };

            ListBox taskListBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(340, 150),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60),
                ItemHeight = 20
            };

            
            foreach (var task in tasks[date])
            {
                taskListBox.Items.Add(task.Name); 
            }

            Button modifyButton = new Button
            {
                Text = "Modify Selected Task",
                Size = new Size(150, 30),
                Location = new Point(125, 200),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            modifyButton.Click += (s, e) =>
            {
                if (taskListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a task to modify.", "No Task Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                string selectedTaskName = taskListBox.SelectedItem.ToString();
                Task taskToModify = tasks[date].FirstOrDefault(t => t.Name == selectedTaskName);

                if (taskToModify != null)
                {
                    
                    taskSelectionForm.Close();
                    OpenModifyTaskForm(date, taskToModify);
                }
            };

            taskSelectionForm.Controls.Add(taskListBox);
            taskSelectionForm.Controls.Add(modifyButton);
            taskSelectionForm.ShowDialog();
        }
        private void OpenModifyTaskForm(DateTime date, Task taskToModify)
        {
            
            Form modifyTaskForm = new Form
            {
                Text = "Modify Task",
                Size = new Size(400, 250),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40), 
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                MaximizeBox = false
            };

            TextBox taskNameTextBox = new TextBox
            {
                Text = taskToModify.Name,
                Location = new Point(20, 20),
                Size = new Size(340, 30),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60)
            };

            RichTextBox taskDescriptionTextBox = new RichTextBox
            {
                Text = taskToModify.Description,
                Location = new Point(20, 60),
                Size = new Size(340, 60),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60)
            };

            Button saveButton = new Button
            {
                Text = "Save Changes",
                Location = new Point(120, 140),
                Size = new Size(150, 30),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            saveButton.Click += (s, e) =>
            {
                
                taskToModify.Name = taskNameTextBox.Text;
                taskToModify.Description = taskDescriptionTextBox.Text;

                RenderCalendar(selectedDate); 

                
                modifyTaskForm.Close();

                
                foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (openForm != this) 
                        openForm.Close();
                }
            };

            modifyTaskForm.Controls.Add(taskNameTextBox);
            modifyTaskForm.Controls.Add(taskDescriptionTextBox);
            modifyTaskForm.Controls.Add(saveButton);
            modifyTaskForm.ShowDialog();
        }

        private void CompleteTask(DateTime date)
        {
            if (!tasks.ContainsKey(date) || tasks[date].Count == 0)
            {
                MessageBox.Show("No tasks available for completion.", "No Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if (form != this)
                    form.Close();
            }

            Form taskSelectionForm = new Form
            {
                Text = "Select Task to Complete",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40), 
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                MaximizeBox = false
            };

            ListBox taskListBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(340, 150),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60),
                ItemHeight = 20
            };

            
            foreach (var task in tasks[date])
            {
                taskListBox.Items.Add(task.Name); 
            }

            Button completeButton = new Button
            {
                Text = "Complete Selected Task",
                Size = new Size(150, 30),
                Location = new Point(125, 200),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            completeButton.Click += (s, e) =>
            {
                if (taskListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a task to complete.", "No Task Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                string selectedTaskName = taskListBox.SelectedItem.ToString();
                Task taskToComplete = tasks[date].FirstOrDefault(t => t.Name == selectedTaskName);

                if (taskToComplete != null)
                {
                    
                    tasks[date].Remove(taskToComplete);

                    
                    totalCompletedTasks++;

                    
                    RenderCalendar(selectedDate);

                    
                    taskSelectionForm.Close();
                }
            };

            taskSelectionForm.Controls.Add(taskListBox);
            taskSelectionForm.Controls.Add(completeButton);
            taskSelectionForm.ShowDialog();
        }

        private void DeleteTask(DateTime date)
        {
            if (!tasks.ContainsKey(date) || tasks[date].Count == 0)
            {
                MessageBox.Show("No tasks available for deletion.", "No Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if (form != this)
                    form.Close();
            }

            // Form for task selection and deletion
            Form taskSelectionForm = new Form
            {
                Text = "Select Task to Delete",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(40, 40, 40),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false
            };

            ListBox taskListBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(340, 150),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 60, 60),
                ItemHeight = 20
            };

            // Populate ListBox with task names
            foreach (var task in tasks[date])
            {
                taskListBox.Items.Add(task.Name);
            }

            Button deleteButton = new Button
            {
                Text = "Delete Selected Task",
                Size = new Size(150, 30),
                Location = new Point(125, 200),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            deleteButton.Click += (s, e) =>
            {
                if (taskListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a task to delete.", "No Task Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedTaskName = taskListBox.SelectedItem.ToString();
                Task taskToDelete = tasks[date].FirstOrDefault(t => t.Name == selectedTaskName);

                if (taskToDelete != null)
                {
                    tasks[date].Remove(taskToDelete); // Remove task from the list

                    if (tasks[date].Count == 0)
                    {
                        tasks.Remove(date); // Remove the date if no tasks remain
                    }

                    RenderCalendar(selectedDate); // Refresh the calendar UI
                    taskSelectionForm.Close(); // Close the selection form
                }
            };

            taskSelectionForm.Controls.Add(taskListBox);
            taskSelectionForm.Controls.Add(deleteButton);
            taskSelectionForm.ShowDialog();
        }

    }

    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; } 

        public Task(string name, string description)
        {
            Name = name;
            Description = description;
            Completed = false; 
        }
    }
}