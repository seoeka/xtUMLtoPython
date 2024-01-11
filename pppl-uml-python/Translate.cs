using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace pppl_uml_python
{
    public partial class Translate : Form
    {
        public Translate()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(0xFF, 0xFF, 0xE8);
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
                    bt_copyJSON.Enabled = true;
                    try
                    {
                        JToken parsedJson = JToken.Parse(File.ReadAllText(selectedFilePath));
                        string formattedJson = parsedJson.ToString(Formatting.Indented);

                        textBox1.Text = formattedJson;
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
                    return;
                }
                JObject jsonObject = JObject.Parse(textBox1.Text);
                string pythonCode = GeneratePythonCode(jsonObject);
                textGeneratePython.Text = pythonCode;
                btExportPython.Enabled = true;
                bt_copyPy.Enabled = true;
                textGeneratePython.ForeColor = System.Drawing.Color.Black;
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
            var allStates = new HashSet<string>();
            if (modelArray != null)
            {
                foreach (var classDefinition in modelArray)
                {
                    string className = classDefinition["class_name"]?.ToString()?.Trim();
                    string classAttributes = "";
                    string classAttrSelf = "";
                    string classEvents = "";
                    string statesValue = "";
                    string transitionValue = "";
                    var statesArray = classDefinition["states"] as JArray;
                    var attributesArray = classDefinition["attributes"] as JArray;

                    if (statesArray!= null)
                    {
                        int stateIndex = 0;
                        foreach (var states in statesArray)
                        {
                            string stateName = states["state_name"]?.ToString();
                            stateName = stateName?.Replace(" ", "").Trim();
                            string stateValue = states["state_value"]?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(stateName))
                            {
                                statesValue += $"        {stateName} = \"{stateValue}\"{Environment.NewLine}";
                                if (stateIndex == 0)
                                {
                                    transitionValue += $"        if self.status{className} == {className}.states.{stateName}:{Environment.NewLine}";
                                }
                                else
                                {
                                    transitionValue += $"        elif self.status{className} == {className}.states.{stateName}:{Environment.NewLine}";
                                }
                                transitionValue += $"            # Implementation code here\r\n            pass\n{Environment.NewLine}";

                                stateIndex++;
                            }
                        }
                    }

                    if (attributesArray != null)
                    {
                        foreach (var attribute in attributesArray)
                        {
                            string attributeName = attribute["attribute_name"]?.ToString()?.Trim();
                            string attributeName2 = attribute["attribute_name"]?.ToString()?.Trim();
                            string dataType = attribute["data_type"]?.ToString();
                            if (!string.IsNullOrEmpty(attributeName) && !string.IsNullOrEmpty(dataType))
                            {
                                string pythonDataType = ConvertCSharpToPythonDataType(dataType);
                                string defaultValue = attribute["default_value"]?.ToString()?.Trim() ?? GetDefaultPythonValue(pythonDataType);
                                if (attributeName != null && attributeName.StartsWith("status"))
                                {
                                    attributeName2 = $"{className}.states.aktif";
                                    defaultValue = "states";
                                }
                                if (attribute["attribute_type"]?.ToString()?.Trim() == "naming_attribute")
                                {
                                    classAttributes += $"{attributeName}: {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName2} # Primary Key {Environment.NewLine}";
                                }
                                else if (attribute["attribute_type"]?.ToString()?.Trim() == "referential_attribute")
                                {
                                    classAttributes += $"{attributeName} : {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName2} # Foreign Key {Environment.NewLine}";
                                }
                                else if (attribute["attribute_type"]?.ToString()?.Trim() == "descriptive_attribute")
                                {
                                    classAttributes += $"{attributeName} : {defaultValue}, ";
                                    classAttrSelf += $"        self.{attributeName} = {attributeName2}{Environment.NewLine}";
                                }
                            }
                        }
                    }

                    if (statesArray != null)
                    {
                        foreach (var state in statesArray)
                        {
                            string stateNames = state["state_name"]?.ToString()?.Trim();

                            void AddEvent(string eventName)
                            {
                                if (!string.IsNullOrEmpty(eventName))
                                {
                                    classEvents += $"    def {eventName}():{Environment.NewLine}";
                                    classEvents += $"        # Implementation code here\r\n        pass\n{Environment.NewLine}";
                                }
                            }

                            var stateEvent = state["state_event"];
                            if (stateEvent is JArray stateEventArray && stateEventArray.Count > 0)
                            {
                                AddEvent(stateEventArray[0]?.ToString()?.Trim());
                            }
                            else
                            {
                                AddEvent(stateEvent?.ToString()?.Trim());
                            }

                            var stateFunction = state["state_function"];
                            if (stateFunction is JArray stateFunctionArray && stateFunctionArray.Count > 0)
                            {
                                AddEvent(stateFunctionArray[0]?.ToString()?.Trim());
                            }
                            else
                            {
                                AddEvent(stateFunction?.ToString()?.Trim());
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
                            if(statesValue != "")
                            {
                                pythonCodeBuilder.AppendLine($"    class states :");
                                pythonCodeBuilder.AppendLine($"{statesValue}");
                            }
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {classAttributes}):");
                        pythonCodeBuilder.AppendLine($"{classAttrSelf}");
                    }
                    if (statesValue != "")
                    {
                        pythonCodeBuilder.AppendLine($"    def transition_status_{className}(self) :");
                        pythonCodeBuilder.AppendLine($"{transitionValue}");
                    }
                    if (classEvents != "")
                    {
                        pythonCodeBuilder.AppendLine($"{classEvents}");
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
                    string associationClassAttributes = "";
                    string associationClassAttrSelf = "";
                    var associationAttributesArray = associationClass["attributes"] as JArray;
                    if (!string.IsNullOrEmpty(associationClassName))
                    {
                        pythonCodeBuilder.AppendLine();
                        for (int i = 0; i < associationAttributesArray.Count; i++)
                        {
                            var attribute = associationAttributesArray[i];
                            string dataType = attribute["data_type"]?.ToString();
                            string attributeName = attribute["attribute_name"]?.ToString();
                            string pythonDataType = ConvertCSharpToPythonDataType(dataType);
                            if (attribute["attribute_type"]?.ToString() == "naming_attribute")
                            {
                                string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                associationClassAttributes += $"{attributeName}: {defaultValue}";
                                if (i < associationAttributesArray.Count - 1)
                                {
                                    associationClassAttributes += ", ";
                                }
                                associationClassAttrSelf += $"        self.{attributeName} = {attributeName} # Primary Key {Environment.NewLine}";
                            }
                            else if (attribute["attribute_type"]?.ToString() == "referential_attribute")
                            {
                                string defaultValue = attribute["default_value"]?.ToString() ?? GetDefaultPythonValue(pythonDataType);
                                associationClassAttributes += $"{attributeName}: {defaultValue}";
                                if (i < associationAttributesArray.Count - 1)
                                {
                                    associationClassAttributes += ", ";
                                }
                                associationClassAttrSelf += $"        self.{attributeName} = {attributeName} # Foreign Key {Environment.NewLine}";
                            }
                        }
                        pythonCodeBuilder.AppendLine($"class {associationClassName}:");
                        pythonCodeBuilder.AppendLine($"    def __init__(self, {associationClassAttributes}):");
                        pythonCodeBuilder.AppendLine($"{associationClassAttrSelf}");
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
        private const string PlaceholderText = "translated python appears here..";
        private void btnClear_Click(object sender, EventArgs e)
        {
            bt_copyJSON.Enabled = false;
            bt_copyPy.Enabled = false;
            btExportPython.Enabled = false;
            textBox1.Clear();
            ClearPythonOutput();
        }
        private void ClearPythonOutput()
        {
            textGeneratePython.Text = string.Empty;
            textGeneratePython.ForeColor = System.Drawing.Color.Gray;
            textGeneratePython.Text = PlaceholderText;
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
            DialogResult result = MessageBox.Show("Are you sure you want to open the Documentation?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/seoeka/pppl-uml-python/blob/master/README.md");
            }
        }
        private void btExportPython_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please upload a JSON file and generate Python code first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // Stop further execution
            }
            if (string.IsNullOrWhiteSpace(textGeneratePython.Text))
            {
                MessageBox.Show("Please generate Python code first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // Stop further execution
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Python Files (*.py)|*.py|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    try
                    {
                        File.WriteAllText(filePath, textGeneratePython.Text);
                        MessageBox.Show("Python code exported successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting Python code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textGeneratePython_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(textGeneratePython.Text);
            MessageBox.Show("Text successfully copied to Clipboard!");
        }
    }
}
