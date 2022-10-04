using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;

namespace RegoApp
{
    public partial class Form1 : Form
    {
        public List<string> plates = new();
        public int index;

        // INIT
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = "READY";
            toolStripStatusLabel1.BackColor = Color.Green;
        }

        
        #region TOOLTIP
        // Bottom left tooltip section
        // Reset Tooltip
        private void ResetStatus()
        {
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel1.BackColor = Color.Empty;
        }
        // Display Tooltip Error        
        private void DisplayError(string err)
        {
            toolStripStatusLabel1.BackColor = Color.Red;
            toolStripStatusLabel1.Text = err;
        }
        // Display Tooltip Success
        private void DisplaySuccess(string suc)
        {
            toolStripStatusLabel1.BackColor = Color.Green;
            toolStripStatusLabel1.Text = suc;
        }
        #endregion
        // Update list function
        // Called at the end of all buttons
        private void UpdateList()
        {
            // Clear all items then sort then insert from list to listbox
            listBox1.Items.Clear();
            // Sort from list thend add to listbox
            plates.Sort();
            foreach (string i in plates) 
            {
                listBox1.Items.Add(i);
            }
            // For tooltip
            ResetStatus();
        }

        // Save Function
        private void SaveFile(bool overWrite)
        {
            // Config + updated new function
            SaveFileDialog saveFile = new()
            {
                RestoreDirectory = true,
                Title = "Save Plates",
                DefaultExt = "txt",
                Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\..\..\..\")),
            };
            
            try
            {
                // Save on Save Button
                if (!overWrite)
                {
                    // Auto file name increment
                    int increment = 1;
                    saveFile.FileName = $"demo_0{increment}.txt";
                    do
                    {
                        increment++;
                        saveFile.FileName = $"demo_0{increment}.txt";

                    } while (File.Exists(saveFile.InitialDirectory + saveFile.FileName));
                    // Save function
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter sw = new StreamWriter(saveFile.FileName))
                        {
                            foreach (string s in plates)
                            {
                                sw.WriteLine(s);
                            }
                            sw.Close();
                        }
                    }
                } 
                // Save on App Closing
                else if (overWrite)
                {
                    int increment = 1;
                    saveFile.FileName = $"demo_0{increment}.txt";
                    do
                    {
                        increment++;
                        saveFile.FileName = $"demo_0{increment}.txt";
                        
                    } while (File.Exists(saveFile.InitialDirectory + saveFile.FileName));
                    string combined = saveFile.InitialDirectory + saveFile.FileName;
                    using (StreamWriter sw = new StreamWriter(combined))
                    {
                        Debug.WriteLine(saveFile.FileName);
                        foreach (string s in plates)
                        {
                            sw.WriteLine(s);
                        }
                        sw.Close();
                    }
                }
                // Tooltip
                DisplaySuccess("SAVED");
            }
            catch (Exception err)
            {
                DisplayError(err.Message);
                MessageBox.Show("Unknown Error." + err);
            }
        }

        // Open Function
        private void OpenFile()
        {
            // Config
            System.Windows.Forms.OpenFileDialog openFile = new()
            {
                Title = "Find Plates",
                Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                InitialDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\..\..\..\")),
            };
            // Open file then update list
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFile.FileName;
                    using (StreamReader sr = new StreamReader(openFile.FileName))
                    {
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            plates.Add(line);
                        }
                        sr.Close();
                    }
                    UpdateList();
                }
                catch (Exception err)
                {
                    DisplayError(err.Message);
                    MessageBox.Show("Unknown Error." + err);
                }
            }
        }

        #region EVENTS

        // Exit App Event
        // Save file on event
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveFile(true);
            MessageBox.Show("File has been saved", "Saved", MessageBoxButtons.OK);
        }

        #endregion

        #region BUTTONS

        // Open Button
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
            DisplaySuccess("FILE OPENED");
        }

        // Save Button
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        // Enter Button
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check if not empty input and duplicate
            if (!String.IsNullOrEmpty(textBox1.Text) && !listBox1.Items.Contains(textBox1.Text))
            {
                plates.Add(textBox1.Text);
                UpdateList();
                textBox1.Clear();
                textBox1.Focus();
            } else
            {
                DisplayError("ERROR");
                MessageBox.Show("Must not be empty or a duplicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Edit Button
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // Check if not empty input and duplicate
            if (!String.IsNullOrEmpty(textBox1.Text) && listBox1.SelectedIndex != -1)
            {
                plates[index] = textBox1.Text;
                UpdateList();
                textBox1.Clear();
                textBox1.Focus();
            } else
            {
                DisplayError("ERROR");
                MessageBox.Show("Must not be empty or a duplicate.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        // Delete Button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                plates.Remove(textBox1.Text);
                UpdateList();
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        // Reset Button
        private void btnReset_Click(object sender, EventArgs e)
        {
            plates.Clear();
            textBox1.Clear();
            UpdateList();
        }

        // Tag Button
        private void tagButton_Click(object sender, EventArgs e)
        {
            // Attach text with affix "z" if not already attached else clear
            string z = textBox1.Text;
            if (z.StartsWith("z"))
            {
                plates[index] = z.Remove(0, 1);
                textBox1.Clear();
            }
            else
            {
                plates[index] = $"z{z}";
                textBox1.Clear();
            }
            UpdateList();
        }

        // Button for Binary Search
        private void btnBinarySearch_Click(object sender, EventArgs e)
        {
            // Search for text in text box return message box if true
            string selected = textBox1.Text;
            if (plates.BinarySearch(selected) >= 0)
            {
                DisplaySuccess("SUCCESS");
                MessageBox.Show($"{selected} is in the list.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DisplayError("ERROR");
                MessageBox.Show($"{selected} is not in the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        // Button for Linear Search
        private void btnLinearSearch_Click(object sender, EventArgs e)
        {
            string selected = textBox1.Text;
            bool found = false;
            // Loop through all in list change boolean if found
            // If looped and none is found display error
            foreach (var i in plates)
            {
                if (i == selected)
                {
                    DisplaySuccess("SUCCESS");
                    MessageBox.Show($"{selected} is in the list.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    found = true;
                }
            }
            if (!found)
            {
                DisplayError("ERROR");
                MessageBox.Show($"{selected} is not in the list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox1.Focus();
            }
        }
        #endregion

        #region MOUSE EVENTS
        // Single click
        // Select an entry from listbox on single click
        private void listBox1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty((string?)listBox1.SelectedItem))
            {
                textBox1.Text = listBox1.SelectedItem.ToString();
                index = listBox1.SelectedIndex;
            }
            else
            {
                DisplayError("ERROR");
            }
        }

        // Double click
        // Delete on double click
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty((string?)listBox1.SelectedItem))
            {
                plates.RemoveAt(listBox1.SelectedIndex);
                UpdateList();
                DisplaySuccess("SUCCESS");
                textBox1.Clear();
                textBox1.Focus();
                DialogResult result = MessageBox.Show("You have deleted this entry", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

    }
}