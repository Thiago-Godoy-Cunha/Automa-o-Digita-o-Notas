// AutomacaoIframeDemo/Helpers/InputProcessor.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AutomacaoIframeDemo.Helpers
{
    public static class InputProcessor
    {
        public static List<int> ParseIntegerList(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                return new List<int>();

            return rawInput
                .Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out int n) ? n : 0)
                .Where(n => n > 0)
                .ToList();
        }

        public static List<string> ParseStringList(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                return new List<string>();

            return rawInput
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }
    }

    /// <summary>
    /// LogHelper mantido idêntico ao original.
    /// Gera um arquivo .log por cenário e o anexa ao resultado do NUnit.
    /// </summary>
    public static class LogHelper
    {
        private static string _logFilePath;
        private static StreamWriter _logWriter;
        private static readonly object _lock = new object();

        public static void Initialize(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            lock (_lock)
            {
                string featureName  = SanitizeFileName(featureContext.FeatureInfo.Title);
                string scenarioName = SanitizeFileName(scenarioContext.ScenarioInfo.Title);
                string timestamp    = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName     = $"{featureName}_{scenarioName}_{timestamp}.log";

                string directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Logs");
                Directory.CreateDirectory(directory);

                _logFilePath = Path.Combine(directory, fileName);
                _logWriter   = new StreamWriter(_logFilePath, append: true, encoding: System.Text.Encoding.UTF8);
                _logWriter.AutoFlush = true;

                WriteLine($"=== LOG INICIADO: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
            }
        }

        public static void WriteLine(string message)
        {
            lock (_lock)
            {
                TestContext.WriteLine(message);
                if (_logWriter != null)
                {
                    string ts = DateTime.Now.ToString("HH:mm:ss.fff");
                    _logWriter.WriteLine($"[{ts}] {message}");
                }
            }
        }

        public static void FlushAndClose()
        {
            lock (_lock)
            {
                if (_logWriter != null)
                {
                    WriteLine($"=== LOG FINALIZADO: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ===");
                    _logWriter.Close();
                    _logWriter = null;

                    if (File.Exists(_logFilePath))
                        TestContext.AddTestAttachment(_logFilePath);
                }
            }
        }

        private static string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(" ", "_");
        }
    }
}
