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
        private void UpdateList()
        {
            // Clear all items then sort then insert from list to listbox
            listBox1.Items.Clear();
            plates.Sort();
            foreach (string i in plates) 
            {
                listBox1.Items.Add(i);
            }
            ResetStatus();
        }
        // Save Function
        private void SaveFile(bool overWrite)
        {
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
                // For Button
                if (!overWrite)
                {
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
                // For Closing
                else if (overWrite)
                {
                    int increment = 1;
                    string saveName = $"demo_0{increment}.txt";
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
            System.Windows.Forms.OpenFileDialog openFile = new()
            {
                Title = "Find Plates",
                Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                InitialDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\..\..\..\")),
            };
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

        // Exit Event
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
            // Check for empty input and duplicate
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
        // Select an entry from listbox
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

        // Double click for delete
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