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
            this.BackColor = Color.FromArgb(0xCC, 0xCC, 0xCC);
        }

        private string selectedFilePath;
        private bool isJsonFileSelected = false;
        private const string PlaceholderText = "translated python appears here..";

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    isJsonFileSelected = true;
                    try
                    {
                        JToken parsedJson = JToken.Parse(File.ReadAllText(selectedFilePath));
                        string formattedJson = parsedJson.ToString(Formatting.Indented);
                        textBox1.Text = selectedFilePath;
                        CheckError(selectedFilePath);
                        if (string.IsNullOrWhiteSpace(msgBox.Text))
                        {
                            msgBox.Text = formattedJson;
                        }
                        else
                        {
                            msgBox.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnTranslate_Click(object sender, EventArgs e)
        {
            try
            {
                if (isJsonFileSelected == true)
                {
                    CheckError(selectedFilePath);
                    if (string.IsNullOrWhiteSpace(msgBox.Text))
                    {
                        JObject jsonObject = JObject.Parse(msgBox.Text);
                        string pythonCode = GeneratePythonCode(jsonObject);
                        textGeneratePython.Text = pythonCode;
                        btnSave.Enabled = true;
                        textGeneratePython.ForeColor = System.Drawing.Color.Black;
                    }
                    else
                    {
                        msgBox.Clear();
                        MessageBox.Show("Seems like JSON format is incorrect or incompatible, click Parse to check the error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Please select JSON file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error translating to Python: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            if (selectedFilePath == null || selectedFilePath.Length == 0 || isJsonFileSelected == false)
            {
                MessageBox.Show("Please select JSON file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                CheckError(selectedFilePath);
                if (string.IsNullOrWhiteSpace(msgBox.Text))
                {
                    msgBox.Text = "Model has successfully passed parsing";
                }
            }
        }
        private string GeneratePythonCode(JObject jsonObject)
        {
            StringBuilder pythonCodeBuilder = new StringBuilder();
            var modelArray = jsonObject["model"] as JArray;
            var allStates = new HashSet<string>();
            string newClassName = "";

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
                                if (states["transitions"] == null || !states["transitions"].Any())
                                {
                                    transitionValue += $"            # Implementation code here\r\n            pass\n{Environment.NewLine}";
                                }
                                else
                                {
                                    var transitionsArray = states["transitions"] as JArray;
                                    if (transitionsArray != null && transitionsArray.Any())
                                    {
                                        foreach (var transition in transitionsArray)
                                        {
                                            string targetStateId = transition["target_state_id"]?.ToString();

                                            var targetState = statesArray.FirstOrDefault(s => s["state_id"]?.ToString() == targetStateId);
                                            if (targetState != null)
                                            {
                                                string targetStateEvent = targetState["state_event"]?.ToString();
                                                string targetStateFunction = targetState["state_function"]?.ToString();
                                                transitionValue += $"            if self.status{className} == {className}.states.{stateName}:{Environment.NewLine}";
                                                transitionValue += $"                self.{targetStateEvent ?? targetStateFunction}()\n{Environment.NewLine}";
                                            }
                                        }
                                    }
                                }   
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
                                string pythonDataType = GetDefaultPythonValue(dataType);
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
                                else if (attribute["attribute_type"]?.ToString()?.Trim() == "related_component")
                                {
                                    string relatedClass = attribute["related_class_name"]?.ToString()?.Trim();
                                    var targetClass = modelArray.FirstOrDefault(s => s["class_name"]?.ToString() == relatedClass);
                                    if (targetClass == null)
                                    {
                                        newClassName += $"class {relatedClass} :\n    pass\n{Environment.NewLine}";
                                    }
                                    attributeName2 = $"{relatedClass}()";
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
                                    classEvents += $"    def {eventName}(self):{Environment.NewLine}";
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
                pythonCodeBuilder.AppendLine($"{newClassName}");
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
                            string pythonDataType = GetDefaultPythonValue(dataType);
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
                case "integer":
                    return "int";
                case "real":
                    return "float";
                case "float":
                    return "float";
                case "inst_ref":
                    return "ref";
                case "string":
                case "state":
                    return "str";
                default:
                    return pythonDataType;
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            isJsonFileSelected = false;
            msgBox.Clear();
            textBox1.Clear();
            ClearPythonOutput();
        }
        private void ClearPythonOutput()
        {
            textGeneratePython.Text = string.Empty;
            textGeneratePython.ForeColor = System.Drawing.Color.Gray;
            textGeneratePython.Text = PlaceholderText;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(msgBox.Text))
            {
                MessageBox.Show("Please select JSON file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(textGeneratePython.Text) || textGeneratePython.Text == PlaceholderText)
            {
                MessageBox.Show("Please translate to Python code first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
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
        }
        private void btHelp_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(btnHelp, new Point(0, btnHelp.Height));
        }
        private void howToMenuItem_Click(object sender, EventArgs e)
        {
            string message = "How to Use the Application:\n\n";
            message += "1. Select your JSON file\n";
            message += "2. Click the 'Translate' button to translate the json model into Python\n";
            message += "3. The result will be displayed on the screen\n";
            message += "4. If you want to save the result to a Python file, please click the 'Save' button";
            MessageBox.Show(message, "How to Use the Application", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void documentationMenuItem_Click(object sender, EventArgs e)
        {
            string message = "About :\n";
            message += "The xtUML JSON to Python Translator is a specialized tool designed to facilitate the conversion of data structures between the xtUML modeling language and Python. " +
                       "This application streamlines the translation process, enabling seamless interoperability between xtUML models and Python code.\n\n";
            message += "Details :\n";
            message += "Logical Organization\r\nData Organization - Centralized\r\nPhysical Organization - Direct Correspondence\r\n" +
                       "Memory Management - Operating System\r\nInstance Relationship - Relational or Object Oriented Approach\r\n" +
                       "Relationship Integrity - Require All Unconditional\r\nReferential Attributes - Store as Part of Physical 50/50 + Look Up Across\r\nDerived Attributes - Derived for Each Access";
            MessageBox.Show(message, "Documentation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void textGeneratePython_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(textGeneratePython.Text);
            MessageBox.Show("Text successfully copied to Clipboard!");
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Parsing

        private JArray ProcessJson(string filePath)
        {
            JArray jsonArray;
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                CheckJsonCompliance(jsonContent);
                jsonArray = new JArray(JToken.Parse(jsonContent));
                msgBox.Text = jsonArray.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading the file {Path.GetFileName(filePath)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                jsonArray = new JArray();
            }

            return jsonArray;
        }

        private void CheckError(string filePath)
        {
            JArray jsonArray = this.ProcessJson(filePath);

            msgBox.Clear();

            CheckParsing15.Point1(this, jsonArray);
            CheckParsing15.Point2(this, jsonArray);
            CheckParsing15.Point3(this, jsonArray);
            CheckParsing15.Point4(this, jsonArray);
            CheckParsing15.Point5(this, jsonArray);
            CheckParsing610.Point6(this, jsonArray);
            CheckParsing610.Point7(this, jsonArray);
            CheckParsing610.Point8(this, jsonArray);
            CheckParsing610.Point9(this, jsonArray);
            CheckParsing610.Point10(this, jsonArray);
            CheckParsing1115.Point11(this, jsonArray);
            CheckParsing1115.Point13(this, jsonArray);
            CheckParsing1115.Point14(this, jsonArray);
            CheckParsing1115.Point15(this, jsonArray);

            string jsonContent = File.ReadAllText(filePath);
            CheckJsonCompliance(jsonContent);

            ParsingPoint.Point25(this, jsonArray);
            ParsingPoint.Point27(this, jsonArray);
            ParsingPoint.Point28(this, jsonArray);
            ParsingPoint.Point29(this, jsonArray);
            ParsingPoint.Point30(this, jsonArray);
            ParsingPoint.Point34(this, jsonArray);
            ParsingPoint.Point35(this, jsonArray);

            CheckParsing1115.Point99(this, jsonArray);
        }

        public TextBox GetMessageBox()
        {
            return msgBox;
        }

        private void HandleError(string errorMessage)
        {
            msgBox.Text += $"{errorMessage}{Environment.NewLine}";
            Console.WriteLine(errorMessage);
        }

        private void CheckJsonCompliance(string jsonContent)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonContent);

                Dictionary<string, string> stateModels = new Dictionary<string, string>();
                HashSet<string> usedKeyLetters = new HashSet<string>();
                HashSet<int> stateNumbers = new HashSet<int>();

                JToken subsystemsToken = jsonObj["subsystems"];
                if (subsystemsToken != null && subsystemsToken.Type == JTokenType.Array)
                {
                    foreach (var subsystem in subsystemsToken)
                    {
                        JToken modelToken = subsystem["model"];
                        if (modelToken != null && modelToken.Type == JTokenType.Array)
                        {
                            foreach (var model in modelToken)
                            {
                                ValidateClassModel(model, stateModels, usedKeyLetters, stateNumbers);
                            }
                        }
                    }

                    foreach (var subsystem in subsystemsToken)
                    {
                        ValidateEventDirectedToStateModelHelper(subsystem["model"], stateModels, null);
                    }
                }
                ValidateTimerModel(jsonObj, usedKeyLetters);
            }
            catch (Exception ex)
            {
                HandleError($"Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateClassModel(JToken model, Dictionary<string, string> stateModels, HashSet<string> usedKeyLetters, HashSet<int> stateNumbers)
        {
            string objectType = model["type"]?.ToString();
            string objectName = model["class_name"]?.ToString();
            Console.WriteLine($"Running CheckKeyLetterUniqueness for {objectName}");

            if (objectType == "class")
            {
                Console.WriteLine($"Checking class: {objectName}");

                string assignerStateModelName = $"{objectName}_ASSIGNER";
                JToken assignerStateModelToken = model[assignerStateModelName];

                if (assignerStateModelToken == null || assignerStateModelToken.Type != JTokenType.Object)
                {
                    HandleError($"Syntax error 16: Assigner state model not found for {objectName}.");
                    return;
                }

                string keyLetter = model["KL"]?.ToString();

                // Pemanggilan CheckKeyLetterUniqueness
                CheckKeyLetterUniqueness(usedKeyLetters, keyLetter, objectName);

                // Check if KeyLetter is correct
                JToken keyLetterToken = assignerStateModelToken?["KeyLetter"];
                if (keyLetterToken != null && keyLetterToken.ToString() != keyLetter)
                {
                    HandleError($"Syntax error 17: KeyLetter for {objectName} does not match the rules.");
                }

                // Check uniqueness of states
                CheckStateUniqueness(stateModels, assignerStateModelToken?["states"], objectName, assignerStateModelName);

                // Check uniqueness of state numbers
                CheckStateNumberUniqueness(stateNumbers, assignerStateModelToken?["states"], objectName);

                // Store state model information
                string stateModelKey = $"{objectName}.{assignerStateModelName}";
                stateModels[stateModelKey] = objectName;
            }
        }

        private void CheckStateUniqueness(Dictionary<string, string> stateModels, JToken statesToken, string objectName, string assignerStateModelName)
        {
            if (statesToken is JArray states)
            {
                HashSet<string> uniqueStates = new HashSet<string>();

                foreach (var state in states)
                {
                    string stateName = state["state_name"]?.ToString();
                    string stateModelName = $"{objectName}.{stateName}";

                    if (!uniqueStates.Add(stateModelName))
                    {
                        HandleError($"Syntax error 18: State {stateModelName} is not unique in {assignerStateModelName}.");
                    }
                }
            }
        }

        private void CheckStateNumberUniqueness(HashSet<int> stateNumbers, JToken statesToken, string objectName)
        {
            if (statesToken is JArray stateArray)
            {
                foreach (var state in stateArray)
                {
                    int stateNumber = state["state_number"]?.ToObject<int>() ?? 0;

                    if (!stateNumbers.Add(stateNumber))
                    {
                        HandleError($"Syntax error 19: State number {stateNumber} is not unique.");
                    }
                }
            }
        }

        private void CheckKeyLetterUniqueness(HashSet<string> usedKeyLetters, string keyLetter, string objectName)
        {
            string expectedKeyLetter = $"{keyLetter}_A";
            Console.WriteLine("Running ValidateClassModel");
            Console.WriteLine($"Checking KeyLetter uniqueness: {expectedKeyLetter} for {objectName}");

            if (!usedKeyLetters.Add(expectedKeyLetter))
            {
                HandleError($"Syntax error 20: KeyLetter for {objectName} is not unique.");
            }
        }

        private void ValidateTimerModel(JObject jsonObj, HashSet<string> usedKeyLetters)
        {
            string timerKeyLetter = jsonObj["subsystems"]?[0]?["model"]?[0]?["KL"]?.ToString();
            string timerStateModelName = $"{timerKeyLetter}_ASSIGNER";

            JToken timerModelToken = jsonObj["subsystems"]?[0]?["model"]?[0];
            JToken timerStateModelToken = jsonObj["subsystems"]?[0]?["model"]?[0]?[timerStateModelName];

            if (timerStateModelToken == null || timerStateModelToken.Type != JTokenType.Object)
            {
                HandleError($"Syntax error 21: Timer state model not found for TIMER.");
                return;
            }

            JToken keyLetterToken = timerStateModelToken?["KeyLetter"];
            if (keyLetterToken == null || keyLetterToken.ToString() != timerKeyLetter)
            {
                HandleError($"Syntax error 21: KeyLetter for TIMER does not match the rules.");
            }
        }

        private void ValidateEventDirectedToStateModelHelper(JToken modelsToken, Dictionary<string, string> stateModels, string modelName)
        {
            if (modelsToken != null && modelsToken.Type == JTokenType.Array)
            {
                foreach (var model in modelsToken)
                {
                    string modelType = model["type"]?.ToString();
                    string className = model["class_name"]?.ToString();

                    if (modelType == "class")
                    {
                        JToken assignerToken = model[$"{className}_ASSIGNER"];

                        if (assignerToken != null)
                        {
                            Console.WriteLine($"assignerToken.Type: {assignerToken.Type}");

                            if (assignerToken.Type == JTokenType.Object)
                            {
                                JToken statesToken = assignerToken["states"];

                                if (statesToken != null && statesToken.Type == JTokenType.Array)
                                {
                                    JArray statesArray = (JArray)statesToken;

                                    foreach (var stateItem in statesArray)
                                    {
                                        string stateName = stateItem["state_name"]?.ToString();
                                        string stateModelName = $"{modelName}.{stateName}";

                                        JToken eventsToken = stateItem["events"];
                                        if (eventsToken is JArray events)
                                        {
                                            foreach (var evt in events)
                                            {
                                                string eventName = evt["event_name"]?.ToString();
                                                JToken targetsToken = evt["targets"];

                                                if (targetsToken is JArray targets)
                                                {
                                                    foreach (var target in targets)
                                                    {
                                                        string targetStateModel = target?.ToString();

                                                        if (!stateModels.ContainsKey(targetStateModel))
                                                        {
                                                            HandleError($"Syntax error 24: Event '{eventName}' in state '{stateModelName}' targets non-existent state model '{targetStateModel}'.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if(isJsonFileSelected == false)
                MessageBox.Show("Please select JSON file first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if(textGeneratePython.Text == PlaceholderText)
                    MessageBox.Show("Please translate to Python first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    Clipboard.SetText(textGeneratePython.Text);
                    MessageBox.Show("Text successfully copied to Clipboard!");
                }
            }
        }
    }
}
