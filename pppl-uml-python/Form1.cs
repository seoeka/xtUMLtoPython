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
using System.Drawing;

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
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter a JSON file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;  // Stop further execution
                }

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
                    string classAttributes = "";
                    string classAttrSelf = "";
                    string classStates = "";

                    var attributesArray = classDefinition["attributes"] as JArray;
                    var statesArray = classDefinition["states"] as JArray;

                    if (attributesArray != null)
                    {
                        foreach (var attribute in attributesArray)
                        {
                            string attributeName = attribute["attribute_name"]?.ToString();
                            string dataType = attribute["data_type"]?.ToString();

                            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(dataType))
                            {
                                string pythonDataType = ConvertCSharpToPythonDataType(dataType);

                                if (attribute["attribute_type"]?.ToString() == "naming_attribute" )
                                {
                                    string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                    classAttributes += $"{attributeName}: {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName} # Primary Key {Environment.NewLine}";

                                }
                                else if (attribute["attribute_type"]?.ToString() == "referential_attribute")
                                {
                                    string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                    classAttributes += $"{attributeName} : {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName} # Foreign Key {Environment.NewLine}";
                                }
                                else if (attribute["attribute_type"]?.ToString() == "descriptive_attribute")
                                {
                                    string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                    classAttributes += $"{attributeName} : {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName}{Environment.NewLine}";
                                }
                            }
                        }
                    }

                    if (statesArray != null)
                    {
                        foreach (var state in statesArray)
                        {
                            string stateName = state["state_name"]?.ToString();
                            string stateValue = state["state_value"]?.ToString();
                            var stateEvents = state["state_event"] as JArray;

                            if (!string.IsNullOrEmpty(stateName) && stateEvents != null && stateEvents.Count > 0)
                            {
                                int stateEventLength = stateEvents.Count;

                                for (int i = 0; i < stateEventLength; i++)
                                {
                                    string stateEvent = stateEvents[i]?.ToString();

                                    int eventIndex = i + 1;

                                    classStates += $"\n    def {stateEvent}(self):";
                                    classStates += $"\n        self.{stateName} = \"{stateValue}\"";

                                    if (eventIndex < stateEventLength)
                                    {
                                        classStates += "\n";
                                    }
                                }
                                classStates += "\n";
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(className))
                    {
                        if (classAttributes.EndsWith(", "))
                        {
                            classAttributes = classAttributes.Substring(0, classAttributes.Length - 2);
                        }

                        pythonCodeBuilder.AppendLine($"class {className}:");
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {classAttributes}):");
                        pythonCodeBuilder.AppendLine($"{classAttrSelf}");
                        pythonCodeBuilder.AppendLine($"{classStates}");
                    }
                }
                pythonCodeBuilder.AppendLine($"# Association");
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

                    if (!string.IsNullOrEmpty(associationClassName))
                    {
                        if (associationClassAttributes.EndsWith(", "))
                        {
                            associationClassAttributes = associationClassAttributes.Substring(0, associationClassAttributes.Length - 2);
                        }

                        pythonCodeBuilder.AppendLine();
                        pythonCodeBuilder.AppendLine($"class {associationClassName}:");
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {associationClassAttributes}):");

                        foreach (var attribute in associationAttributesArray)
                        {
                            string attributeName = attribute["attribute_name"]?.ToString();
                            if (attribute["attribute_type"]?.ToString() == "naming_attribute")
                            {
                                pythonCodeBuilder.AppendLine($"        self.{attributeName} = {attributeName} # PK ");
                            }
                            else if (attribute["attribute_type"]?.ToString() == "referential_attribute")
                            {
                                pythonCodeBuilder.AppendLine($"        self.{attributeName} = {attributeName} # FK");
                            }
                        }
                        pythonCodeBuilder.AppendLine();
                    }
                }
            }
        }


        private string GetDefaultPythonValue(string pythonDataType)
        {
            switch (pythonDataType)
            {
                case "id":
                    return "int";
                case "str":
                    return "str";
                case "int":
                    return "int";
                case "float":
                    return "float";
                default:
                    return "";
            }
        }

        private string ConvertCSharpToPythonDataType(string csharpDataType)
        {
            switch (csharpDataType)
            {
                case "id":
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textGeneratePython.Text = string.Empty;
        }

        private void btHelp_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btHelp, new Point(0, btHelp.Height));

        }

        private void howToMenuItem_Click(object sender, EventArgs e)
        {
            string message = "How to Use the Application:\n\n";
            message += "1. Upload your JSON file\n";
            message += "2. Click the 'Generate to Python' button to generate the json into Python\n";
            message += "3. The result will be displayed on the screen\n";
            message += "4. If you want to save the result to a Python file, please click the 'Save' button";

            MessageBox.Show(message, "How to Use the Application", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void documentationMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to open the Online Notes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/seoeka/pppl-uml-python/blob/master/README.md");
            }
        }
    }
}
