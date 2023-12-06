using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace pppl_uml_python
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string selectedFilePath;

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set the filter for JSON files
                openFileDialog.Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    try
                    {
                        // Read the content of the selected file
                        string fileContent = File.ReadAllText(selectedFilePath);

                        // Display the content in the TextBox
                        textBox1.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions, e.g., file not found or unable to read
                        MessageBox.Show($"Error reading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // Parse JSON content using Newtonsoft.Json
                JObject jsonObject = JObject.Parse(textBox1.Text);

                // Generate Python code
                string pythonCode = GeneratePythonCode(jsonObject);

                // Display the generated Python code or save it to a file
                textGeneratePython.Text = pythonCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Python code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GeneratePythonCode(JObject jsonObject)
        {
            StringBuilder pythonCodeBuilder = new StringBuilder();

            var modelArray = jsonObject["model"] as JArray;

            if (modelArray != null)
            {
                foreach (var classDefinition in modelArray)
                {
                    string className = classDefinition["class_name"]?.ToString();
                    string classAttributes = "";

                    var attributesArray = classDefinition["attributes"] as JArray;

                    if (attributesArray != null)
                    {
                        foreach (var attribute in attributesArray)
                        {
                            string attributeName = attribute["attribute_name"]?.ToString();
                            string dataType = attribute["data_type"]?.ToString();

                            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(dataType))
                            {
                                string pythonDataType = ConvertCSharpToPythonDataType(dataType);

                                // Check for naming attribute or referential attribute
                                if (attribute["attribute_type"]?.ToString() == "naming_attribute")
                                {
                                    classAttributes += $"    def __init__(self, {attributeName}):{Environment.NewLine}";
                                }
                                else if (attribute["attribute_type"]?.ToString() == "referential_attribute")
                                {
                                    // Referential attribute should not have default value
                                    classAttributes += $"    def __init__(self, {attributeName}):{Environment.NewLine}";
                                }
                                else
                                {
                                    // Descriptive attribute with default value
                                    string defaultValue = attribute["default_value"]?.ToString() ?? "";
                                    classAttributes += $"    def __init__(self, {attributeName}={defaultValue}):{Environment.NewLine}";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(className))
                    {
                        pythonCodeBuilder.AppendLine($"class {className}:{Environment.NewLine}{classAttributes}{Environment.NewLine}");
                    }
                }
            }

            return pythonCodeBuilder.ToString();
        }



        private string ConvertCSharpToPythonDataType(string csharpDataType)
        {
            switch (csharpDataType)
            {
                case "id":
                    return "str";
                case "integer":
                    return "int";
                case "real":
                    return "float";
                case "string":
                    return "str";
                case "state":
                    return "str";
                default:
                    return csharpDataType;
            }
        }

        private void bt_copyPy_Click(object sender, EventArgs e)
        {
            // Copy the generated Python code to the clipboard
            Clipboard.SetText(textGeneratePython.Text);

            // Optionally, provide user feedback
            MessageBox.Show("Python code copied to clipboard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bt_copyJSON_Click(object sender, EventArgs e)
        {
            // Copy the generated Python code to the clipboard
            Clipboard.SetText(textBox1.Text);

            // Optionally, provide user feedback
            MessageBox.Show("Python code copied to clipboard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
