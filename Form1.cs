using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLLToFrontend
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // wire up events
            btnDLLSelect.Click += btnDLLSelect_Click;
            btnSavingLocation.Click += btnSavingLocation_Click;
            btnGenerate.Click += btnGenerate_Click;
        }

        // ---------------------------
        // UI events
        // ---------------------------

        private void btnDLLSelect_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Managed DLL (*.dll)|*.dll",
                Title = "Select .NET DLL"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lbldlllocation.Text = ofd.FileName;
            }
        }

        private void btnSavingLocation_Click(object? sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = "Select output folder" };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lblsavinglocation.Text = fbd.SelectedPath;
            }
        }

        private void btnGenerate_Click(object? sender, EventArgs e)
        {
            var dllPath = lbldlllocation.Text?.Trim();
            var outDir = lblsavinglocation.Text?.Trim();

            if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
                 ShowStatus("Please choose a valid DLL path.", error: true);

            if (string.IsNullOrWhiteSpace(outDir) || !Directory.Exists(outDir))
                 ShowStatus("Please choose a valid saving location.", error: true);

            try
            {
                // quick managed-assembly check
                AssemblyName.GetAssemblyName(dllPath);

                // load & inspect
                var asm = Assembly.LoadFrom(dllPath);
                var meta = InspectAssembly(asm);

                if (meta.ExportableMethods.Count == 0)
                {
                    ShowStatus("No compatible public methods found. See rules below.", error: true);
                    return;
                }

                // write TOML
                var toml = BuildToml(dllPath, meta);
                File.WriteAllText(Path.Combine(outDir, "config.toml"), toml, new UTF8Encoding(false));

                // write handler.js
                var handler = BuildHandlerJs(dllPath, meta);
                File.WriteAllText(Path.Combine(outDir, "handler.js"), handler, new UTF8Encoding(false));

                ShowStatus($"Generated {meta.ExportableMethods.Count} handlers. Done!", error: false);
            }
            catch (BadImageFormatException)
            {
                ShowStatus("Selected file is not a managed .NET DLL.", error: true);
            }
            catch (ReflectionTypeLoadException rtle)
            {
                var msg = string.Join(Environment.NewLine, rtle.LoaderExceptions.Select(x => x.Message));
                ShowStatus("Failed to load some types:\n" + msg, error: true);
            }
            catch (Exception ex)
            {
                ShowStatus("Error: " + ex.Message, error: true);
            }
        }

        private void ShowStatus(string text, bool error)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = error ? Color.Red : Color.Green;
        }

        // ---------------------------
        // Reflection + generation
        // ---------------------------

        private static bool IsEdgeFriendly(MethodInfo m)
        {
            //if (!m.IsPublic || m.IsSpecialName) return false;

            //// skip void methods (edge-js can't return void)
            //if (m.ReturnType == typeof(void)) return false;

            //// only allow 0 or 1 parameter of any type
            //var parms = m.GetParameters();
            //if (parms.Length > 1) return false;

            //return true; // allow any return type

            if (!m.IsPublic || m.IsSpecialName) return false;

            // Skip void methods
            if (m.ReturnType == typeof(void)) return false;

            // Allow any number of parameters
            return true;
        }

        private static string ToSafeExportName(string ns, string cls, string method)
        {
            var full = $"{(string.IsNullOrEmpty(ns) ? "Global" : ns)}_{cls}_{method}";
            var sb = new StringBuilder();
            foreach (var ch in full)
                sb.Append(char.IsLetterOrDigit(ch) ? ch : '_');
            return sb.ToString();
        }

        private static string Q(string s) => s.Replace("\\", "\\\\").Replace("'", "\\'");

        private static AssemblyMeta InspectAssembly(Assembly asm)
        {
            var meta = new AssemblyMeta();

            foreach (var t in asm.GetTypes())
            {
                if (!t.IsClass) continue;

                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (!IsEdgeFriendly(m)) continue;

                    meta.ExportableMethods.Add(new MethodMeta
                    {
                        Namespace = t.Namespace ?? "",
                        ClassName = t.Name,
                        MethodName = m.Name,
                        IsStatic = m.IsStatic,
                        ReturnType = m.ReturnType.FullName ?? m.ReturnType.Name,
                        ParameterTypes = m.GetParameters().Select(p => p.ParameterType.FullName ?? p.ParameterType.Name).ToList()
                    });
                }
            }

            return meta;
        }

        private static string BuildToml(string dllPath, AssemblyMeta meta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[dll]");
            sb.AppendLine($"path = '{Q(dllPath)}'");
            sb.AppendLine();

            sb.AppendLine("# Methods exported via handler.js");
            foreach (var m in meta.ExportableMethods)
            {
                sb.AppendLine("[[export]]");
                sb.AppendLine($"namespace = '{Q(m.Namespace)}'");
                sb.AppendLine($"class     = '{Q(m.ClassName)}'");
                sb.AppendLine($"method    = '{Q(m.MethodName)}'");
                sb.AppendLine($"static    = {m.IsStatic.ToString().ToLower()}");
                sb.AppendLine($"returns   = '{Q(m.ReturnType)}'");
                sb.AppendLine($"params    = [{string.Join(", ", m.ParameterTypes.Select(t => $"'{Q(t)}'"))}]");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string BuildHandlerJs(string dllPath, AssemblyMeta meta)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Auto-generated by DLLToFrontend");
            sb.AppendLine("// Requires: npm i edge-js");
            sb.AppendLine("const edge = require('edge-js');");
            sb.AppendLine();

            foreach (var m in meta.ExportableMethods)
            {
                var exportName = ToSafeExportName(m.Namespace, m.ClassName, m.MethodName);
                sb.AppendLine($"const {exportName}_raw = edge.func({{");
                sb.AppendLine($"  assemblyFile: '{Q(dllPath)}',");
                sb.AppendLine($"  typeName: '{Q((string.IsNullOrEmpty(m.Namespace) ? "" : m.Namespace + ".") + m.ClassName)}',");
                sb.AppendLine($"  methodName: '{Q(m.MethodName)}'");
                sb.AppendLine("});");
                sb.AppendLine();
            }

            sb.AppendLine("const exportsMap = {");
            foreach (var m in meta.ExportableMethods)
            {
                var exportName = ToSafeExportName(m.Namespace, m.ClassName, m.MethodName);
                sb.AppendLine($"  '{exportName}': (payload) => new Promise((resolve, reject) => {{");
                sb.AppendLine($"    {exportName}_raw(payload ?? null, (err, res) => err ? reject(err) : resolve(res));");
                sb.AppendLine("  }) ,");
            }
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine("module.exports = exportsMap;");

            return sb.ToString();
        }

        // ---------------------------
        // models
        // ---------------------------

        private sealed class AssemblyMeta
        {
            public List<MethodMeta> ExportableMethods { get; } = new();
        }

        private sealed class MethodMeta
        {
            public string Namespace { get; set; } = "";
            public string ClassName { get; set; } = "";
            public string MethodName { get; set; } = "";
            public bool IsStatic { get; set; }
            public string ReturnType { get; set; } = "";
            public List<string> ParameterTypes { get; set; } = new();
        }
    }
}
