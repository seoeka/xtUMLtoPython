using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;

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
                openFileDialog.Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;

                    try
                    {
                        string fileContent = File.ReadAllText(selectedFilePath);
                        textBox1.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                JObject jsonObject = JObject.Parse(textBox1.Text);
                string pythonCode = GeneratePythonCode(jsonObject);
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
                    string classKeyLetters = classDefinition["KL"]?.ToString();
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

                                if (attribute["attribute_type"]?.ToString() == "naming_attribute" ||
                                    attribute["attribute_type"]?.ToString() == "referential_attribute")
                                {
                                    classAttributes += $"{attributeName}, ";
                                }
                                else
                                {
                                    string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                    classAttributes += $"{attributeName}={defaultValue}, ";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(className))
                    {
                        // Remove the last comma and space from classAttributes
                        if (classAttributes.EndsWith(", "))
                        {
                            classAttributes = classAttributes.Substring(0, classAttributes.Length - 2);
                        }

                        pythonCodeBuilder.AppendLine($"class {className}:");
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {classAttributes}):");

                        if (classKeyLetters != null)
                        {
                            pythonCodeBuilder.AppendLine($"        self.{classKeyLetters}_id = {classKeyLetters}_id");
                        }

                        pythonCodeBuilder.AppendLine();
                    }
                }

                // Generate association classes
                GenerateAssociationClasses(jsonObject, pythonCodeBuilder);
            }

            return pythonCodeBuilder.ToString();
        }

        private void GenerateAssociationClasses(JObject jsonObject, StringBuilder pythonCodeBuilder)
        {
            var associationsArray = jsonObject["model"].Where(j => j["type"].ToString() == "association").ToList();

            foreach (var association in associationsArray)
            {
                string associationName = association["name"]?.ToString();
                var associatedClasses = association["class"] as JArray;
                var associationClass = association["model"] as JObject;

                if (associationClass != null)
                {
                    string associationClassName = associationClass["class_name"]?.ToString();
                    string associationClassKeyLetters = associationClass["KL"]?.ToString();
                    string associationClassAttributes = "";

                    var associationAttributesArray = associationClass["attributes"] as JArray;

                    if (associationAttributesArray != null)
                    {
                        foreach (var attribute in associationAttributesArray)
                        {
                            string attributeName = attribute["attribute_name"]?.ToString();
                            string dataType = attribute["data_type"]?.ToString();

                            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(dataType))
                            {
                                associationClassAttributes += $"self.{attributeName}, ";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(associationClassName))
                    {
                        // Remove the last comma and space from associationClassAttributes
                        if (associationClassAttributes.EndsWith(", "))
                        {
                            associationClassAttributes = associationClassAttributes.Substring(0, associationClassAttributes.Length - 2);
                        }

                        pythonCodeBuilder.AppendLine();
                        pythonCodeBuilder.AppendLine($"class {associationClassName}:");
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {associationClassAttributes}):");

                        if (associationClassKeyLetters != null)
                        {
                            pythonCodeBuilder.AppendLine($"        self.{associationClassKeyLetters}_id = {associationClassKeyLetters}_id");
                        }

                        pythonCodeBuilder.AppendLine();

                        // Add association class to the main class
                        foreach (var associatedClass in associatedClasses)
                        {
                            string associatedClassName = associatedClass["class_name"]?.ToString();
                            string associatedClassKeyLetters = associatedClass["KL"]?.ToString();
                            pythonCodeBuilder.AppendLine($"class {associationName}{associatedClassName}:");
                            pythonCodeBuilder.AppendLine($"    def __init__(self, {associationClassKeyLetters}_id, {associatedClassKeyLetters}_id):");

                            // Include self. for each attribute
                            foreach (var attribute in associationAttributesArray)
                            {
                                string attributeName = attribute["attribute_name"]?.ToString();
                                pythonCodeBuilder.AppendLine($"        self.{attributeName} = {attributeName}");
                            }

                            pythonCodeBuilder.AppendLine();
                        }
                    }
                }
            }
        }




        private string GetDefaultPythonValue(string pythonDataType)
        {
            switch (pythonDataType)
            {
                case "str":
                    return "\"\"";
                case "int":
                    return "0";
                case "float":
                    return "0.0";
                default:
                    return "";
            }
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
                case "state":
                    return "str";
                default:
                    return csharpDataType;
            }
        }

        private void bt_copyPy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textGeneratePython.Text);
            MessageBox.Show("Python code copied to clipboard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bt_copyJSON_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("JSON content copied to clipboard!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
